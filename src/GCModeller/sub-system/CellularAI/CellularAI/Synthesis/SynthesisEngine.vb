Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.HTS.DataFrame.SingleCell
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Planning
Imports std = System.Math

Namespace Synthesis

    ''' <summary>
    ''' 一个上下文的<b>控制侧状态</b>：对照细胞的基因比例、平均 CPM、可调用基因、中位测序深度与
    ''' <c>log2(mean_cpm / (TS·fraction))</c> 偏移。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这些量在整条流水线里被反复使用（计划的可用性判断、总量设计的分数换算、合成的分位取值），
    ''' 因此一次性算好并常驻；它们<b>只依赖对照矩阵与模型参数</b>，不含任何随机性。
    ''' </para>
    ''' <para>
    ''' <see cref="Log2Cg"/> 是"设计用的 log2 因子"与"实测 log2FC"之间的桥：
    ''' 设计给出的是相对对照比例的倍数 <c>2^L</c>，而真实 log2FC 还要减去
    ''' <c>log2(mean_cpm/(TS·fc))</c> 这一项（文库大小归一化带来的偏移）。
    ''' 计划里的"最小 lfc"约束必须在实测口径上施加，因此 <see cref="GuardL2"/> 会做这个换算。
    ''' </para>
    ''' </remarks>
    Public Class SynthesisEngine

        ''' <summary>对照细胞矩阵（细胞 × 基因）。</summary>
        Public ReadOnly Property Controls As CsrMatrix

        ''' <summary>基因比例 <c>fc</c>（列和归一化，和为 1）。</summary>
        Public ReadOnly Property Fractions As Double()

        ''' <summary>逐基因的对照平均 CPM。</summary>
        Public ReadOnly Property MeanCpm As Double()

        ''' <summary>逐基因的"可调用"标记（<c>mean_cpm &gt; eligible_cpm</c>）。</summary>
        Public ReadOnly Property Eligible As Boolean()

        ''' <summary>中位测序深度（四舍五入的整数）。</summary>
        Public ReadOnly Property Depth As Integer

        ''' <summary>逐基因的 <c>log2(mean_cpm / (1e6·fc))</c>；非有限值取 0。</summary>
        Public ReadOnly Property Log2Cg As Double()

        Private m_cpmRows As New Dictionary(Of Integer, Double())
        Private m_tieSums As New Dictionary(Of Integer, Double)
        Private m_libraries As Double()

        Private Sub New(controls As CsrMatrix, fractions As Double(), meanCpm As Double(),
                        eligible As Boolean(), depth As Integer, log2Cg As Double())
            Me.Controls = controls
            Me.Fractions = fractions
            Me.MeanCpm = meanCpm
            Me.Eligible = eligible
            Me.Depth = depth
            Me.Log2Cg = log2Cg
        End Sub

        ''' <summary>基因数。</summary>
        Public ReadOnly Property GeneCount As Integer
            Get
                Return Fractions.Length
            End Get
        End Property

        ''' <summary>细胞数。</summary>
        Public ReadOnly Property CellCount As Integer
            Get
                Return Controls.RowDimension
            End Get
        End Property

        ''' <summary>
        ''' 该基因在对照细胞上的 CPM 升序序列（float32 精度往返，惰性计算并缓存）。
        ''' </summary>
        ''' <remarks>
        ''' 合成内层要对每个基因反复二分，因此这里缓存"已排序 + float32 化"的对照谱。
        ''' 缓存按基因惰性增长：只有被真正求解到的基因才会占内存。
        ''' 生产环境建议改为一次性按列（CSC）物化 float32 的 <c>G×n</c> 表——
        ''' 逐基因扫描 CSR 在基因数上万时是 <c>O(G·nnz)</c>，只适合中小规模。
        ''' </remarks>
        Public Function SortedCpm(gene As Integer) As Double()
            Dim cached As Double() = Nothing

            If m_cpmRows.TryGetValue(gene, cached) Then
                Return cached
            End If

            Dim n As Integer = Controls.RowDimension
            Dim values(n - 1) As Double
            Dim libraries As Double() = LibrarySizes

            For i As Integer = 0 To n - 1
                Dim librarySize As Double = std.Max(libraries(i), 0.000000000001)

                For k As Integer = Controls.Indptr(i) To Controls.Indptr(i + 1) - 1
                    If Controls.Indices(k) = gene Then
                        values(i) = CDbl(CSng(1000000.0 * Controls.Values(k) / librarySize))
                        Exit For
                    End If
                Next
            Next

            Call Array.Sort(values)

            m_cpmRows(gene) = values

            Return values
        End Function

        ''' <summary>该基因对照谱的并列和 <c>Σ(t³−t)</c>（与 <see cref="SortedCpm"/> 同步缓存）。</summary>
        Public Function TieSum(gene As Integer) As Double
            Dim cached As Double = 0.0

            If m_tieSums.TryGetValue(gene, cached) Then
                Return cached
            End If

            Dim values As Double() = SortedCpm(gene)
            Dim total As Double = 0.0
            Dim i As Integer = 0

            While i < values.Length
                Dim j As Integer = i

                While j + 1 < values.Length AndAlso values(j + 1) = values(i)
                    j += 1
                End While

                Dim group As Double = j - i + 1

                total += group * group * group - group
                i = j + 1
            End While

            m_tieSums(gene) = total

            Return total
        End Function

        ''' <summary>每个对照细胞的总计数（长度 = 细胞数，缓存一次）。</summary>
        Public ReadOnly Property LibrarySizes As Double()
            Get
                If m_libraries Is Nothing Then
                    m_libraries = Controls.RowSums()
                End If

                Return m_libraries
            End Get
        End Property

        ''' <summary>由对照矩阵构造引擎。</summary>
        Public Shared Function Create(controls As CsrMatrix, model As CellularAiModel) As SynthesisEngine
            If controls Is Nothing Then
                Throw New ArgumentNullException(NameOf(controls))
            End If
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If

            Dim nGenes As Integer = controls.ColumnDimension
            Dim columnSums As Double() = controls.ColumnSums()
            Dim total As Double = 0.0

            For i As Integer = 0 To columnSums.Length - 1
                total += columnSums(i)
            Next

            If total <= 0.0 Then
                total = 1.0
            End If

            Dim fractions(nGenes - 1) As Double

            For i As Integer = 0 To nGenes - 1
                fractions(i) = columnSums(i) / total
            Next

            ' 平均 CPM：逐细胞归一化后取均值（与序统计表的口径一致）
            Dim meanCpm As Double() = CountStatistics.ChunkedCpmMean(controls)
            Dim eligible(nGenes - 1) As Boolean

            For i As Integer = 0 To nGenes - 1
                eligible(i) = meanCpm(i) > model.Calls.EligibleCpm
            Next

            ' 中位深度（行和只算一次）
            Dim librarySizes As Double() = controls.RowSums()
            Dim depths As Double() = New Double(controls.RowDimension - 1) {}

            For i As Integer = 0 To depths.Length - 1
                depths(i) = librarySizes(i)
            Next

            Dim sorted As Double() = depths.OrderBy(Function(v) v).ToArray
            Dim medianDepth As Double = 0.0

            If sorted.Length > 0 Then
                medianDepth = If(sorted.Length Mod 2 = 1,
                                 sorted(sorted.Length \ 2),
                                 (sorted(sorted.Length \ 2 - 1) + sorted(sorted.Length \ 2)) / 2.0)
            End If

            Dim log2Cg(nGenes - 1) As Double

            For i As Integer = 0 To nGenes - 1
                Dim ratio As Double = If(fractions(i) > 0.0, meanCpm(i) / (1000000.0 * fractions(i)), Double.NaN)

                log2Cg(i) = If(Double.IsFinite(ratio) AndAlso ratio > 0.0, std.Log(ratio, 2.0), 0.0)
            Next

            Return New SynthesisEngine(controls, fractions, meanCpm, eligible,
                                       CInt(std.Round(medianDepth, MidpointRounding.AwayFromZero)),
                                       log2Cg)
        End Function

        ''' <summary>把设计因子换成"对照比例"：<c>fc·2^L</c> 再归一化。</summary>
        Public Function DesignFraction(l2 As Double()) As Double()
            Dim values As Double() = New Double(GeneCount - 1) {}
            Dim total As Double = 0.0

            For i As Integer = 0 To GeneCount - 1
                Dim exponent As Double = std.Max(std.Min(l2(i), 12.5), -12.5)
                Dim value As Double = Fractions(i) * std.Pow(2.0, exponent)

                If Not Double.IsFinite(value) OrElse value < 0.0 Then
                    value = 0.0
                End If

                values(i) = value
                total += value
            Next

            If total <= 0.0 Then
                Return DirectCast(Fractions.Clone(), Double())
            End If

            For i As Integer = 0 To values.Length - 1
                values(i) /= total
            Next

            Return values
        End Function

        ''' <summary>
        ''' 计划基因的实测 log2FC 必须达到计划方向与最小幅度：
        ''' <c>sign·(L − log2_cg) ≥ min_lfc</c>，不满足时把 <c>L</c> 抬到临界值。
        ''' </summary>
        Public Function GuardL2(l2 As Double(), planIndex As Integer(), planSign As Double(), minLfc As Double) As Double()
            Dim values As Double() = DirectCast(l2.Clone(), Double())

            For i As Integer = 0 To planIndex.Length - 1
                Dim at As Integer = planIndex(i)

                If at < 0 OrElse at >= values.Length Then
                    Continue For
                End If

                Dim sign As Double = If(i < planSign.Length, planSign(i), 1.0)

                If sign = 0.0 Then
                    sign = 1.0
                End If

                Dim offset As Double = Log2Cg(at)

                If sign * (values(at) - offset) < minLfc Then
                    values(at) = offset + sign * minLfc
                End If
            Next

            Return values
        End Function

        ''' <summary>
        ''' 每个基因的目标秩和 z：计划基因为 <c>sign·(z_plan[0] + (z_plan[1]−z_plan[0])·优先级)</c>，
        ''' 其余基因按微调方向与强度给一个较弱的约束。
        ''' </summary>
        Public Function ZTargets(planIndex As Integer(), planSign As Double(), planPriority As Double(),
                                 nudgeSign As SByte(), nudgeStrength As Double(),
                                 options As SynthesisOptions) As Double()
            Dim targets(GeneCount - 1) As Double

            For g As Integer = 0 To GeneCount - 1
                Dim direction As Double = If(nudgeSign(g) < 0, -1.0, If(nudgeSign(g) > 0, 1.0, 0.0))

                targets(g) = direction * std.Max(std.Min(nudgeStrength(g), 1.0), 0.0) * options.ZNudge
            Next

            If planIndex.Length = 0 Then
                Return targets
            End If

            ' 优先级归一化到 [0,1]（1 = 最高优先级 → z_plan[1]）
            Dim ranks As Double() = CallSizing.RankOf(planPriority)
            Dim denominator As Double = std.Max(planPriority.Length - 1, 1)

            For i As Integer = 0 To planIndex.Length - 1
                Dim at As Integer = planIndex(i)

                If at < 0 OrElse at >= targets.Length Then
                    Continue For
                End If

                Dim sign As Double = If(i < planSign.Length, planSign(i), 1.0)

                If sign = 0.0 Then
                    sign = 1.0
                End If

                Dim level As Double = If(planPriority.Length > 1,
                                         ranks(i) / denominator,
                                         1.0)

                targets(at) = sign * (options.ZPlan(0) + (options.ZPlan(1) - options.ZPlan(0)) * level)
            Next

            Return targets
        End Function

        Public Overrides Function ToString() As String
            Return $"synthesis engine: {CellCount} cell x {GeneCount} gene, depth {Depth}"
        End Function

    End Class
End Namespace
