Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace Data

    ''' <summary>
    ''' 预处理之后的表达数据视图：训练行（状态）与目标行已经按拟合路线展开成
    ''' 「行 = 样本 / 时间点」的形式，并附带反标准化与支持域限幅所需的统计量。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 训练统一在 z 空间进行（<c>Standardize</c> 为 True 时）：
    ''' LSE 的条件数更稳，且不同基因的量纲不会让岭项失衡；
    ''' 但对外报告（表达量、扰动结果）必须回到原始尺度，因此同时保存每基因的
    ''' <see cref="Means"/> / <see cref="SDs"/> 与原始取值范围。
    ''' </para>
    ''' <para>
    ''' <see cref="StateRows"/> 与 <see cref="TargetRows"/> 的长度相同：
    ''' 静态路线下两者逐行相同（<c>x_g = F(regulators)</c>）；
    ''' 滞后路线下 <c>TargetRows(k) = StateRows(k+1)</c> 的聚合值
    ''' （即 <c>x_g(t+1) = F(x(t))</c>）。
    ''' </para>
    ''' </remarks>
    Public Class GeneExpressionView

        ''' <summary>参与建模的基因名（保持输入矩阵中的相对顺序）。</summary>
        Public Property GeneNames As String()

        ''' <summary>标准化的表达矩阵 <c>[基因, 样本]</c>。</summary>
        Public Property Z As Double(,)

        ''' <summary>每基因原始表达的均值（反标准化用）。</summary>
        Public Property Means As Double()

        ''' <summary>每基因原始表达的标准差（总体标准差，反标准化与 Z 分数用）。</summary>
        Public Property SDs As Double()

        ''' <summary>每基因原始表达的最小值。</summary>
        Public Property Minimum As Double()

        ''' <summary>每基因原始表达的最大值。</summary>
        Public Property Maximum As Double()

        ''' <summary>该基因的原始表达在所有样本上恒定（既不能当靶基因也不能当调控因子）。</summary>
        Public Property Constant As Boolean()

        ''' <summary>稳态迭代的起始状态（原始尺度）：静态路线取全样本均值，滞后路线取最后一个时间点的均值。</summary>
        Public Property BaselineRaw As Double()

        ''' <summary>训练状态行 <c>[行, 基因]</c>（z 空间）。</summary>
        Public Property StateRows As Double()()

        ''' <summary>训练目标行 <c>[行, 基因]</c>（z 空间）。</summary>
        Public Property TargetRows As Double()()

        ''' <summary>每基因在状态行上的均值（相关性计算用）。</summary>
        Public Property RowMeans As Double()

        ''' <summary>每基因在状态行上的离差平方和（相关性计算用）。</summary>
        Public Property RowSs As Double()

        ''' <summary>样本时间点标签（无时序信息时为 Nothing）。</summary>
        Public Property TimePoints As Double()

        ''' <summary>排序后的唯一时间点。</summary>
        Public Property UniqueTimePoints As Double()

        ''' <summary>实际采用的拟合路线。</summary>
        Public Property Mode As FuzzyTargetMode

        ''' <summary>是否为滞后（时序）路线。</summary>
        Public Property IsLagged As Boolean

        ''' <summary>训练坐标是否为 z 空间（<see cref="GeneExpressionView.Z"/> 已标准化）。</summary>
        Public Property Standardized As Boolean

        ''' <summary>基因数量。</summary>
        Public ReadOnly Property NGene As Integer
            Get
                Return If(GeneNames Is Nothing, 0, GeneNames.Length)
            End Get
        End Property

        ''' <summary>原始样本数量。</summary>
        Public ReadOnly Property NSample As Integer
            Get
                Return Z.GetLength(1)
            End Get
        End Property

        ''' <summary>训练行数量。</summary>
        Public ReadOnly Property NRows As Integer
            Get
                Return StateRows.Length
            End Get
        End Property

        ''' <summary>按基因名查下标（大小写不敏感），缺失时返回 -1。</summary>
        Public Function IndexOf(gene As String) As Integer
            If GeneNames Is Nothing OrElse String.IsNullOrEmpty(gene) Then
                Return -1
            End If

            For i As Integer = 0 To GeneNames.Length - 1
                If String.Equals(GeneNames(i), gene, StringComparison.OrdinalIgnoreCase) Then
                    Return i
                End If
            Next

            Return -1
        End Function

        ''' <summary>由 z 值还原为原始表达量。</summary>
        Public Function ToRaw(gene As Integer, z As Double) As Double
            Return z * SDs(gene) + Means(gene)
        End Function

        ''' <summary>把原始表达量换算成 z 值。</summary>
        Public Function ToZ(gene As Integer, value As Double) As Double
            Dim sd As Double = SDs(gene)

            If std.Abs(sd) < 0.000000000001 Then
                sd = 1.0
            End If

            Return (value - Means(gene)) / sd
        End Function

    End Class

    ''' <summary>
    ''' 表达矩阵预处理：基因过滤、标准化、时间聚合与拟合路线判定。
    ''' </summary>
    Public Module ExpressionPreprocessor

        ''' <summary>
        ''' 把原始表达矩阵整理成训练视图。
        ''' </summary>
        ''' <param name="expression">基因表达矩阵（行 = 基因，列 = 样本）</param>
        ''' <param name="prior">先验调控网络；用于在 <see cref="FuzzyGeneConfig.MaxGenes"/> 过滤时保留 TF</param>
        ''' <param name="config">建模配置</param>
        ''' <returns>预处理视图</returns>
        ''' <exception cref="ArgumentException">矩阵维度不一致，或强制滞后模式但缺少时间点</exception>
        Public Function Preprocess(expression As GeneExpressionData,
                                   Optional prior As PriorNetwork = Nothing,
                                   Optional config As FuzzyGeneConfig = Nothing) As GeneExpressionView

            If expression Is Nothing Then
                Throw New ArgumentNullException(NameOf(expression))
            End If

            Dim cfg As FuzzyGeneConfig = If(config, New FuzzyGeneConfig()).ValidateGene()
            Dim genes As String() = expression.GeneNames
            Dim raw As Double(,) = expression.Matrix
            Dim nSample As Integer = expression.NSample

            If genes Is Nothing OrElse raw Is Nothing Then
                Throw New ArgumentException("the expression matrix is incomplete", NameOf(expression))
            End If
            If raw.GetLength(0) <> genes.Length OrElse raw.GetLength(1) <> nSample Then
                Throw New ArgumentException(
                    $"the matrix is {raw.GetLength(0)}x{raw.GetLength(1)} but {genes.Length} genes and {nSample} samples were declared",
                    NameOf(expression))
            End If
            If nSample < 2 Then
                Throw New ArgumentException("at least 2 samples are required", NameOf(expression))
            End If

            ' ---- 1. 基因过滤：高变异优先，但保留先验网络里的 TF ----
            Dim keep As Integer() = SelectGenes(raw, genes, prior, cfg)

            ' ---- 2. 逐基因统计量与标准化 ----
            Dim n As Integer = keep.Length
            Dim names(n - 1) As String
            Dim means(n - 1) As Double
            Dim sds(n - 1) As Double
            Dim mins(n - 1) As Double
            Dim maxs(n - 1) As Double
            Dim z(n - 1, nSample - 1) As Double

            For a As Integer = 0 To n - 1
                Dim gi As Integer = keep(a)
                Dim sum As Double = 0.0
                Dim minValue As Double = Double.MaxValue
                Dim maxValue As Double = Double.MinValue

                For s As Integer = 0 To nSample - 1
                    Dim v As Double = raw(gi, s)

                    If v.IsNaNImaginary Then
                        v = 0.0
                    End If

                    sum += v

                    If v < minValue Then minValue = v
                    If v > maxValue Then maxValue = v
                Next

                Dim mean As Double = sum / nSample
                Dim ss As Double = 0.0

                For s As Integer = 0 To nSample - 1
                    Dim v As Double = raw(gi, s)

                    If v.IsNaNImaginary Then
                        v = 0.0
                    End If

                    ss += (v - mean) * (v - mean)
                Next

                ' 总体标准差（ddof = 0）：z 空间的相关系数就等于行内积 / 行数
                Dim sd As Double = std.Sqrt(ss / nSample)

                names(a) = genes(gi)
                means(a) = mean
                mins(a) = minValue
                maxs(a) = maxValue

                If sd < 0.000000000001 Then
                    ' 常数列：标准化后全为 0，且不能作为有效的建模变量
                    sds(a) = 1.0
                Else
                    sds(a) = sd
                End If

                For s As Integer = 0 To nSample - 1
                    Dim v As Double = raw(gi, s)

                    If v.IsNaNImaginary Then
                        v = 0.0
                    End If

                    If cfg.Standardize Then
                        z(a, s) = (v - mean) / sds(a)
                    Else
                        z(a, s) = v
                    End If
                Next
            Next

            ' ---- 3. 时间点与拟合路线 ----
            Dim times As Double() = expression.TimePoints
            Dim uniqueTimes As Double() = Nothing

            If times IsNot Nothing AndAlso times.Length = nSample Then
                uniqueTimes = DistinctSorted(times)
            End If

            Dim mode As FuzzyTargetMode = cfg.TargetMode

            If mode = FuzzyTargetMode.Auto Then
                If uniqueTimes IsNot Nothing AndAlso uniqueTimes.Length >= cfg.MinTimePoints Then
                    mode = FuzzyTargetMode.LaggedRoute
                Else
                    mode = FuzzyTargetMode.StaticRoute
                End If
            End If

            If mode = FuzzyTargetMode.LaggedRoute AndAlso (uniqueTimes Is Nothing OrElse uniqueTimes.Length < 2) Then
                Throw New ArgumentException(
                    "the lagged (dynamic) route requires at least 2 distinct time points in TimePoints", NameOf(expression))
            End If

            ' ---- 4. 训练行与目标行 ----
            Dim view As New GeneExpressionView With {
                .GeneNames = names,
                .Z = z,
                .Means = means,
                .SDs = sds,
                .Minimum = mins,
                .Maximum = maxs,
                .TimePoints = times,
                .UniqueTimePoints = uniqueTimes,
                .Mode = mode,
                .IsLagged = mode = FuzzyTargetMode.LaggedRoute,
                .Standardized = cfg.Standardize
            }

            If mode = FuzzyTargetMode.LaggedRoute Then
                BuildLaggedRows(view, z, raw, keep, uniqueTimes, times)
            Else
                BuildStaticRows(view, z)
            End If

            ' ---- 5. 常数列判定 ----
            Dim constant(n - 1) As Boolean

            For g As Integer = 0 To n - 1
                Dim minZ As Double = Double.MaxValue
                Dim maxZ As Double = Double.MinValue

                For r As Integer = 0 To view.NRows - 1
                    Dim v As Double = view.StateRows(r)(g)

                    If v < minZ Then minZ = v
                    If v > maxZ Then maxZ = v
                Next

                constant(g) = (maxZ - minZ) < 0.000000000001
            Next

            view.Constant = constant

            ' ---- 6. 行均值与离差平方和（相关性筛选用） ----
            Dim rowMeans(n - 1) As Double
            Dim rowSs(n - 1) As Double

            For g As Integer = 0 To n - 1
                Dim sum As Double = 0.0

                For r As Integer = 0 To view.NRows - 1
                    sum += view.StateRows(r)(g)
                Next

                Dim mean As Double = sum / view.NRows
                Dim ss As Double = 0.0

                For r As Integer = 0 To view.NRows - 1
                    Dim d As Double = view.StateRows(r)(g) - mean
                    ss += d * d
                Next

                rowMeans(g) = mean
                rowSs(g) = ss
            Next

            view.RowMeans = rowMeans
            view.RowSs = rowSs

            Return view
        End Function

        ''' <summary>静态路线：每个样本就是一行，目标是该样本自身的表达值。</summary>
        Private Sub BuildStaticRows(view As GeneExpressionView, z As Double(,))
            Dim nSample As Integer = z.GetLength(1)
            Dim n As Integer = z.GetLength(0)
            Dim rows(nSample - 1)() As Double
            Dim target(nSample - 1)() As Double

            For s As Integer = 0 To nSample - 1
                Dim row(n - 1) As Double

                For g As Integer = 0 To n - 1
                    row(g) = z(g, s)
                Next

                rows(s) = row
                target(s) = CType(row.Clone(), Double())
            Next

            view.StateRows = rows
            view.TargetRows = target
            view.BaselineRaw = CType(view.Means.Clone(), Double())
        End Sub

        ''' <summary>
        ''' 滞后路线：唯一时间点两两相邻构成训练对；同一时间点的生物学重复先取均值。
        ''' </summary>
        Private Sub BuildLaggedRows(view As GeneExpressionView,
                                    z As Double(,),
                                    raw As Double(,),
                                    keep As Integer(),
                                    uniqueTimes As Double(),
                                    times As Double())

            Dim n As Integer = z.GetLength(0)
            Dim pairs As Integer = uniqueTimes.Length - 1
            Dim rows(pairs - 1)() As Double
            Dim target(pairs - 1)() As Double

            For k As Integer = 0 To pairs - 1
                rows(k) = MeanStateAt(z, keep, times, uniqueTimes(k))
                target(k) = MeanStateAt(z, keep, times, uniqueTimes(k + 1))
            Next

            ' 基线取最后一个时间点的原始表达均值（扰动前的稳态背景）
            Dim last As Double() = New Double(n - 1) {}
            Dim lastSamples As Integer() = SampleIndicesAt(times, uniqueTimes(uniqueTimes.Length - 1))

            For g As Integer = 0 To n - 1
                Dim sum As Double = 0.0

                For Each s As Integer In lastSamples
                    Dim v As Double = raw(keep(g), s)

                    If v.IsNaNImaginary Then v = 0.0

                    sum += v
                Next

                last(g) = If(lastSamples.Length > 0, sum / lastSamples.Length, view.Means(g))
            Next

            view.StateRows = rows
            view.TargetRows = target
            view.BaselineRaw = last
        End Sub

        ''' <summary>某时间点上所有重复样本的 z 值均值，转成整基因向量。</summary>
        Private Function MeanStateAt(z As Double(,), keep As Integer(), times As Double(), time As Double) As Double()
            Dim n As Integer = z.GetLength(0)
            Dim sampleIdx As Integer() = SampleIndicesAt(times, time)
            Dim row(n - 1) As Double

            For g As Integer = 0 To n - 1
                Dim sum As Double = 0.0

                For Each s As Integer In sampleIdx
                    sum += z(g, s)
                Next

                row(g) = If(sampleIdx.Length > 0, sum / sampleIdx.Length, 0.0)
            Next

            Return row
        End Function

        Private Function SampleIndicesAt(times As Double(), time As Double) As Integer()
            Dim result As New List(Of Integer)

            For i As Integer = 0 To times.Length - 1
                If std.Abs(times(i) - time) < 0.0000000001 Then
                    result.Add(i)
                End If
            Next

            Return result.ToArray()
        End Function

        Private Function DistinctSorted(values As Double()) As Double()
            Dim set_ As New HashSet(Of Double)(values.Where(Function(v) Not v.IsNaNImaginary))
            Dim sorted As Double() = set_.ToArray()

            Call System.Array.Sort(sorted)

            Return sorted
        End Function

        ''' <summary>
        ''' 基因过滤：按原始表达的方差从高到低保留 <see cref="FuzzyGeneConfig.MaxGenes"/> 个，
        ''' 并把先验网络的 TF 无条件保留（否则调控因子可能在过滤中被删掉）。
        ''' </summary>
        Private Function SelectGenes(raw As Double(,),
                                     genes As String(),
                                     prior As PriorNetwork,
                                     cfg As FuzzyGeneConfig) As Integer()

            Dim nGene As Integer = genes.Length

            If cfg.MaxGenes <= 0 OrElse nGene <= cfg.MaxGenes Then
                Return Enumerable.Range(0, nGene).ToArray()
            End If

            Dim nSample As Integer = raw.GetLength(1)
            Dim variances(nGene - 1) As Double

            For i As Integer = 0 To nGene - 1
                Dim sum As Double = 0.0
                Dim count As Integer = 0

                For s As Integer = 0 To nSample - 1
                    Dim v As Double = raw(i, s)

                    If Not v.IsNaNImaginary Then
                        sum += v
                        count += 1
                    End If
                Next

                Dim mean As Double = If(count > 0, sum / count, 0.0)
                Dim ss As Double = 0.0

                For s As Integer = 0 To nSample - 1
                    Dim v As Double = raw(i, s)

                    If Not v.IsNaNImaginary Then
                        ss += (v - mean) * (v - mean)
                    End If
                Next

                variances(i) = If(count > 1, ss / (count - 1), 0.0)
            Next

            Dim order As Integer() = Enumerable.Range(0, nGene) _
                .OrderByDescending(Function(i) variances(i)) _
                .ToArray()

            Dim keep As New SortedSet(Of Integer)

            For i As Integer = 0 To cfg.MaxGenes - 1
                Call keep.Add(order(i))
            Next

            If prior IsNot Nothing AndAlso prior.TFNames IsNot Nothing Then
                For i As Integer = 0 To nGene - 1
                    If prior.TFNames.Contains(genes(i)) Then
                        Call keep.Add(i)
                    End If
                Next
            End If

            Return keep.ToArray()
        End Function

    End Module

End Namespace
