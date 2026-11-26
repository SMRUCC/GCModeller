Imports System
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports Microsoft.VisualBasic.Math.Statistics
Imports CellularAI.Synthesis
Imports std = System.Math

Namespace Data

    ''' <summary>一次两组对比的结果（逐基因）。</summary>
    Public Class Contrast

        ''' <summary>对比名（同时作为伪扰动名）。</summary>
        Public Property Name As String

        ''' <summary>基因轴。</summary>
        Public Property Genes As String()

        ''' <summary>log2FC（case / control）。</summary>
        Public Property Effect As Double()

        ''' <summary>秩和 z。</summary>
        Public Property Z As Double()

        ''' <summary>双侧 p 值。</summary>
        Public Property PValue As Double()

        ''' <summary>BH 校正后的 q 值。</summary>
        Public Property Adjusted As Double()

        ''' <summary>证据概率 <c>1 - q</c>（夹在 [0,1]）。</summary>
        Public Property Probability As Double()

        ''' <summary>参与对比的 case / control 细胞数。</summary>
        Public Property CaseCells As Integer
        Public Property ControlCells As Integer

    End Class

    ''' <summary>
    ''' 两组细胞的差异表达：逐基因的 log2FC、精确秩和 p 值、BH 校正与证据概率。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 入参是"细胞 × 基因"的 CSR。逐基因取列需要按基因聚集的布局，因此调用方传入
    ''' 一次转置好的 <see cref="CscMatrix"/>（整个 view 的所有对比共用同一份转置，避免反复转置）。
    ''' </para>
    ''' <para>
    ''' <b>为什么用精确秩和而不是 t 检验</b>：单细胞计数严重右偏且存在大量并列零，
    ''' 均值/方差推断不稳；秩和只依赖名次，配合 CellularAI 的并列校正可以直接得到可比的 z，
    ''' 与后续合成侧的显著性口径一致。
    ''' </para>
    ''' <para>
    ''' <b>自身靶不在这里剔除</b>：伪扰动的"靶"可能是签名基因本身（BATF_high），
    ''' 保留它的上调信号正是我们要建模的效应；剔除只发生在计划层（<c>SelectCalls</c>）。
    ''' </para>
    ''' </remarks>
    Public Module DifferentialPanel

        ''' <summary>
        ''' 计算一次两组对比。
        ''' </summary>
        ''' <param name="name">对比名</param>
        ''' <param name="byGene">按基因聚集的矩阵（列 = 基因）</param>
        ''' <param name="librarySizes">逐细胞文库大小</param>
        ''' <param name="caseCells">实验组细胞下标</param>
        ''' <param name="controlCells">对照组细胞下标</param>
        ''' <param name="genes">基因轴</param>
        ''' <param name="normalizer">CPM 归一化常数</param>
        Public Function Contrast(name As String, byGene As CscMatrix, librarySizes As Double(),
                                 caseCells As Integer(), controlCells As Integer(),
                                 genes As String(), Optional normalizer As Double = 1000000.0) As Contrast
            Dim nGenes As Integer = If(genes Is Nothing, 0, genes.Length)
            Dim effect(nGenes - 1) As Double
            Dim zScores(nGenes - 1) As Double
            Dim pValues(nGenes - 1) As Double
            Dim inCase As Boolean() = Mask(byGene.RowDimension, caseCells)
            Dim inControl As Boolean() = Mask(byGene.RowDimension, controlCells)

            For g As Integer = 0 To nGenes - 1
                Dim column As (rows As Integer(), values As Double()) = byGene.ColumnNonZero(g)
                Dim caseCpm As New List(Of Double)
                Dim caseLibrary As New List(Of Double)
                Dim controlCpm As New List(Of Double)

                For k As Integer = 0 To column.rows.Length - 1
                    Dim cell As Integer = column.rows(k)

                    If inCase(cell) Then
                        Call caseCpm.Add(normalizer * column.values(k) / std.Max(librarySizes(cell), 1.0E-12))
                        Call caseLibrary.Add(librarySizes(cell))
                    ElseIf inControl(cell) Then
                        Call controlCpm.Add(normalizer * column.values(k) / std.Max(librarySizes(cell), 1.0E-12))
                    End If
                Next

                If caseCpm.Count = 0 OrElse controlCpm.Count = 0 Then
                    ' 两组之一完全没有观测：效应记 0、证据记无
                    effect(g) = 0.0
                    pValues(g) = 1.0
                    Continue For
                End If

                ' 对照组缺失的细胞（计数为 0）也必须计入分布，否则会系统性抬高 z
                Dim controlAll(controlCells.Length - 1) As Double
                Dim positions As New Dictionary(Of Integer, Integer)

                For c As Integer = 0 To controlCells.Length - 1
                    positions(controlCells(c)) = c
                Next

                For k As Integer = 0 To column.rows.Length - 1
                    Dim cell As Integer = column.rows(k)
                    Dim at As Integer = -1

                    If inControl(cell) AndAlso positions.TryGetValue(cell, at) Then
                        controlAll(at) = normalizer * column.values(k) / std.Max(librarySizes(cell), 1.0E-12)
                    End If
                Next

                Call Array.Sort(controlAll)

                Dim tie As Double = TieSum(controlAll)
                Dim caseValues As Double() = caseCpm.ToArray
                Dim caseSizes As Double() = caseLibrary.ToArray
                Dim z As Double = WilcoxonKernel.ZExact(caseValues, caseSizes, controlAll, tie)

                zScores(g) = z
                pValues(g) = NormalDistribution.TwoSidedPValue(z)

                Dim meanCase As Double = caseValues.Average
                Dim meanControl As Double = 0.0

                For c As Integer = 0 To controlAll.Length - 1
                    meanControl += controlAll(c)
                Next

                meanControl /= std.Max(controlAll.Length, 1)
                effect(g) = std.Log((meanCase + 1.0) / (meanControl + 1.0), 2.0)
            Next

            Dim adjusted As Double() = FalseDiscoveryRate.PValueAdjust(pValues, PValueAdjustMethod.BH, nGenes)
            Dim probability(nGenes - 1) As Double

            For g As Integer = 0 To nGenes - 1
                probability(g) = std.Max(std.Min(1.0 - adjusted(g), 1.0), 0.0)
            Next

            Return New Contrast With {
                .Name = name,
                .Genes = genes,
                .Effect = effect,
                .Z = zScores,
                .PValue = pValues,
                .Adjusted = adjusted,
                .Probability = probability,
                .CaseCells = caseCells.Length,
                .ControlCells = controlCells.Length
            }
        End Function

        ''' <summary>某个基因在某组细胞上的表达值（用于按表达量分层）。</summary>
        Public Function GeneExpression(byGene As CscMatrix, gene As Integer,
                                       librarySizes As Double(), nCells As Integer) As Double()
            Dim result(nCells - 1) As Double
            Dim column As (rows As Integer(), values As Double()) = byGene.ColumnNonZero(gene)

            For k As Integer = 0 To column.rows.Length - 1
                result(column.rows(k)) = column.values(k) / std.Max(librarySizes(column.rows(k)), 1.0E-12)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 按表达量取上/下分位，返回 (high, low) 两组细胞下标。
        ''' </summary>
        ''' <param name="values">逐细胞表达（未在组内的细胞会被排除）</param>
        ''' <param name="members">候选细胞下标</param>
        ''' <param name="quantile">分层比例（默认 0.25 表示上/下四分位）</param>
        Public Function Stratify(values As Double(), members As Integer(),
                                 Optional quantile As Double = 0.25) As (High As Integer(), Low As Integer())
            Dim ranked As Integer() = members.
                OrderBy(Function(i) values(i)).
                ThenBy(Function(i) i).
                ToArray
            Dim take As Integer = CInt(std.Floor(ranked.Length * quantile))

            If take < 3 Then
                Return (ranked, ranked)
            End If

            Dim low As Integer() = ranked.Take(take).ToArray
            Dim high As Integer() = ranked.Skip(ranked.Length - take).ToArray

            Return (high, low)
        End Function

        Private Function Mask(n As Integer, members As Integer()) As Boolean()
            Dim flags(n - 1) As Boolean

            For Each i As Integer In members
                If i >= 0 AndAlso i < n Then
                    flags(i) = True
                End If
            Next

            Return flags
        End Function

        Private Function TieSum(sorted As Double()) As Double
            Dim total As Double = 0.0
            Dim i As Integer = 0

            While i < sorted.Length
                Dim j As Integer = i

                While j + 1 < sorted.Length AndAlso sorted(j + 1) = sorted(i)
                    j += 1
                End While

                Dim group As Double = j - i + 1

                total += group * group * group - group
                i = j + 1
            End While

            Return total
        End Function

    End Module
End Namespace
