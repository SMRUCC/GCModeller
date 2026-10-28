Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace Data

    ''' <summary>
    ''' 预处理之后的表达数据视图，采用**基因优先**（gene-major）的连续列存储。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 为什么是基因优先而不是样本优先：整套拟合里最重的两步都是「逐基因顺序扫描」——
    ''' 候选调控因子的相关性筛选要为每个靶基因扫过成百上千个候选列，ANFIS 的输入也是
    ''' 少量基因的整列。样本优先的 <c>[行, 基因]</c> 布局会让每次逐基因扫描都把整张矩阵
    ''' 拖过缓存（每个 cache line 只用到 2 个元素），在 1.85 万基因 × 3 万样本时相当于
    ''' 上百 TB 的无效内存流量；改成 <c>Columns(g)</c> 连续存放后，同一扫描变成顺序读。
    ''' </para>
    ''' <para>
    ''' 训练统一在 z 空间进行（<c>Standardize</c> 为 True 时）：LSE 的条件数更稳，
    ''' 且不同基因的量纲不会让岭项失衡；对外报告（表达量、扰动结果）必须回到原始尺度，
    ''' 因此同时保存每基因的 <see cref="Means"/> / <see cref="SDs"/>。
    ''' </para>
    ''' <para>
    ''' <see cref="Columns"/> 与 <see cref="Targets"/> 的每一行一一对应：
    ''' 静态路线下两者是**同一个数组**（<c>x_g = F(regulators)</c>，不复制第二份）；
    ''' 滞后路线下 <c>Targets(g)(k) = x_g(t_{k+1})</c>（即 <c>x_g(t+1) = F(x(t))</c>）。
    ''' </para>
    ''' </remarks>
    Public Class GeneExpressionView

        ''' <summary>参与建模的基因名（保持输入矩阵中的相对顺序）。</summary>
        Public Property GeneNames As String()

        ''' <summary>每基因原始表达的均值（反标准化用）。</summary>
        Public Property Means As Double()

        ''' <summary>每基因原始表达的标准差（总体标准差，反标准化与 Z 分数用）。</summary>
        Public Property SDs As Double()

        ''' <summary>每基因原始表达的最小值。</summary>
        Public Property Minimum As Double()

        ''' <summary>每基因原始表达的最大值。</summary>
        Public Property Maximum As Double()

        ''' <summary>该基因的训练列恒定（既不能当靶基因也不能当调控因子）。</summary>
        Public Property Constant As Boolean()

        ''' <summary>稳态迭代的起始状态（原始尺度）：静态路线取全样本均值，滞后路线取最后一个时间点的均值。</summary>
        Public Property BaselineRaw As Double()

        ''' <summary>
        ''' 训练状态列：<c>Columns(g)(r)</c> 是基因 g 在第 r 个训练行上的取值（z 空间）。
        ''' </summary>
        Public Property Columns As Double()()

        ''' <summary>
        ''' 训练目标列：<c>Targets(g)(r)</c> 是第 r 行上该基因的目标值。
        ''' 静态路线下与 <see cref="Columns"/> 共享同一组数组（不额外占内存）。
        ''' </summary>
        Public Property Targets As Double()()

        ''' <summary>每基因在训练列上的均值（相关性计算用）。</summary>
        Public Property RowMeans As Double()

        ''' <summary>每基因在训练列上的离差平方和（相关性计算用）。</summary>
        Public Property RowSs As Double()

        ''' <summary>原始样本时间点标签（无时序信息时为 Nothing）。</summary>
        Public Property TimePoints As Double()

        ''' <summary>排序后的唯一时间点。</summary>
        Public Property UniqueTimePoints As Double()

        ''' <summary>实际采用的拟合路线。</summary>
        Public Property Mode As FuzzyTargetMode

        ''' <summary>是否为滞后（时序）路线。</summary>
        Public Property IsLagged As Boolean

        ''' <summary>训练坐标是否为 z 空间（<see cref="Columns"/> 已标准化）。</summary>
        Public Property Standardized As Boolean

        ''' <summary>原始样本数量（可能大于 <see cref="NRows"/>：后者是行采样之后的训练行数）。</summary>
        Public Property NSample As Integer

        ''' <summary>基因数量。</summary>
        Public ReadOnly Property NGene As Integer
            Get
                Return If(GeneNames Is Nothing, 0, GeneNames.Length)
            End Get
        End Property

        ''' <summary>实际参与训练的行数量。</summary>
        Public ReadOnly Property NRows As Integer
            Get
                If Columns Is Nothing OrElse Columns.Length = 0 Then
                    Return 0
                End If

                Return Columns(0).Length
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
    ''' 表达矩阵预处理：基因过滤、标准化、行采样、时间聚合与拟合路线判定。
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

            ' ---- 2. 逐基因统计量（全样本、采样之前，保证反标准化与报告尺度不变）----
            Dim n As Integer = keep.Length
            Dim names(n - 1) As String
            Dim means(n - 1) As Double
            Dim sds(n - 1) As Double
            Dim mins(n - 1) As Double
            Dim maxs(n - 1) As Double

            For a As Integer = 0 To n - 1
                Dim gi As Integer = keep(a)
                Dim sum As Double = 0.0
                Dim minValue As Double = Double.MaxValue
                Dim maxValue As Double = Double.MinValue

                For s As Integer = 0 To nSample - 1
                    Dim v As Double = CleanValue(raw(gi, s))

                    sum += v

                    If v < minValue Then minValue = v
                    If v > maxValue Then maxValue = v
                Next

                Dim mean As Double = sum / nSample
                Dim ss As Double = 0.0

                For s As Integer = 0 To nSample - 1
                    Dim d As Double = CleanValue(raw(gi, s)) - mean
                    ss += d * d
                Next

                ' 总体标准差（ddof = 0）：z 空间的行内积 / 行数就等于协方差
                Dim sd As Double = std.Sqrt(ss / nSample)

                names(a) = genes(gi)
                means(a) = mean
                mins(a) = minValue
                maxs(a) = maxValue
                ' 常数列：标准化后全为 0，不能作为有效的建模变量
                sds(a) = If(sd < 0.000000000001, 1.0, sd)
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

            Dim view As New GeneExpressionView With {
                .GeneNames = names,
                .Means = means,
                .SDs = sds,
                .Minimum = mins,
                .Maximum = maxs,
                .TimePoints = times,
                .UniqueTimePoints = uniqueTimes,
                .Mode = mode,
                .IsLagged = mode = FuzzyTargetMode.LaggedRoute,
                .Standardized = cfg.Standardize,
                .NSample = nSample
            }

            ' ---- 4. 训练列与目标列 ----
            If view.IsLagged Then
                Call BuildLaggedColumns(view, raw, keep, uniqueTimes, times)
            Else
                Call BuildStaticColumns(view, raw, keep, cfg)
            End If

            ' ---- 5. 常数列判定、列均值与离差平方和（全部顺序扫描连续列）----
            Dim constant(n - 1) As Boolean
            Dim columnMeans(n - 1) As Double
            Dim columnSs(n - 1) As Double
            Dim rows As Integer = view.NRows

            For g As Integer = 0 To n - 1
                Dim column As Double() = view.Columns(g)
                Dim sum As Double = 0.0
                Dim minZ As Double = Double.MaxValue
                Dim maxZ As Double = Double.MinValue

                For r As Integer = 0 To rows - 1
                    Dim v As Double = column(r)

                    sum += v

                    If v < minZ Then minZ = v
                    If v > maxZ Then maxZ = v
                Next

                Dim mean As Double = If(rows > 0, sum / rows, 0.0)
                Dim ss As Double = 0.0

                For r As Integer = 0 To rows - 1
                    Dim d As Double = column(r) - mean
                    ss += d * d
                Next

                constant(g) = (maxZ - minZ) < 0.000000000001
                columnMeans(g) = mean
                columnSs(g) = ss
            Next

            view.Constant = constant
            view.RowMeans = columnMeans
            view.RowSs = columnSs

            Return view
        End Function

        ''' <summary>
        ''' 静态路线：每个（采样后的）样本就是一行，目标是该样本自身的表达值。
        ''' </summary>
        ''' <remarks>
        ''' <see cref="FuzzyGeneConfig.MaxTrainRows"/> 在这里直接作用于列的构造：只把采样到的行
        ''' 写进列里，于是训练时间与常驻内存同时下降（3 万样本 × 1.85 万基因时从 4.5 GB 降到几百 MB）。
        ''' </remarks>
        Private Sub BuildStaticColumns(view As GeneExpressionView, raw As Double(,), keep As Integer(), cfg As FuzzyGeneConfig)
            Dim n As Integer = keep.Length
            Dim nSample As Integer = raw.GetLength(1)
            Dim rowIndex As Integer() = SampleRows(nSample, cfg.MaxTrainRows)
            Dim rows As Integer = If(rowIndex Is Nothing, nSample, rowIndex.Length)
            Dim columns(n - 1)() As Double

            For a As Integer = 0 To n - 1
                Dim gi As Integer = keep(a)
                Dim mean As Double = view.Means(a)
                Dim sd As Double = view.SDs(a)
                Dim column(rows - 1) As Double

                If cfg.Standardize Then
                    If rowIndex Is Nothing Then
                        For s As Integer = 0 To rows - 1
                            column(s) = (CleanValue(raw(gi, s)) - mean) / sd
                        Next
                    Else
                        For r As Integer = 0 To rows - 1
                            column(r) = (CleanValue(raw(gi, rowIndex(r))) - mean) / sd
                        Next
                    End If
                Else
                    If rowIndex Is Nothing Then
                        For s As Integer = 0 To rows - 1
                            column(s) = CleanValue(raw(gi, s))
                        Next
                    Else
                        For r As Integer = 0 To rows - 1
                            column(r) = CleanValue(raw(gi, rowIndex(r)))
                        Next
                    End If
                End If

                columns(a) = column
            Next

            view.Columns = columns
            ' 静态路线下目标就是状态本身：直接共享同一组数组，不复制第二份
            view.Targets = columns
            view.BaselineRaw = CType(view.Means.Clone(), Double())
        End Sub

        ''' <summary>
        ''' 滞后路线：唯一时间点两两相邻构成训练对；同一时间点的生物学重复先取均值。
        ''' </summary>
        Private Sub BuildLaggedColumns(view As GeneExpressionView,
                                       raw As Double(,),
                                       keep As Integer(),
                                       uniqueTimes As Double(),
                                       times As Double())

            Dim n As Integer = keep.Length
            Dim pairs As Integer = uniqueTimes.Length - 1
            Dim groups As Integer()() = New Integer(uniqueTimes.Length - 1)() {}

            For k As Integer = 0 To uniqueTimes.Length - 1
                groups(k) = SampleIndicesAt(times, uniqueTimes(k))
            Next

            Dim means As Double() = view.Means
            Dim sds As Double() = view.SDs
            Dim columns(n - 1)() As Double
            Dim targets(n - 1)() As Double

            For a As Integer = 0 To n - 1
                Dim gi As Integer = keep(a)
                Dim mean As Double = means(a)
                Dim sd As Double = sds(a)
                Dim state(pairs - 1) As Double
                Dim target(pairs - 1) As Double

                For k As Integer = 0 To pairs - 1
                    state(k) = MeanAt(raw, gi, groups(k), mean, sd)
                    target(k) = MeanAt(raw, gi, groups(k + 1), mean, sd)
                Next

                columns(a) = state
                targets(a) = target
            Next

            ' 基线取最后一个时间点的原始表达均值（扰动前的稳态背景）
            Dim lastSamples As Integer() = groups(groups.Length - 1)
            Dim last(n - 1) As Double

            For a As Integer = 0 To n - 1
                Dim sum As Double = 0.0

                For Each s As Integer In lastSamples
                    sum += CleanValue(raw(keep(a), s))
                Next

                last(a) = If(lastSamples.Length > 0, sum / lastSamples.Length, means(a))
            Next

            view.Columns = columns
            view.Targets = targets
            view.BaselineRaw = last
        End Sub

        ''' <summary>某时间点上所有重复样本的（标准化的）均值。</summary>
        Private Function MeanAt(raw As Double(,), gene As Integer, samples As Integer(), mean As Double, sd As Double) As Double
            If samples.Length = 0 Then
                Return 0.0
            End If

            Dim sum As Double = 0.0

            For Each s As Integer In samples
                sum += (CleanValue(raw(gene, s)) - mean) / sd
            Next

            Return sum / samples.Length
        End Function

        ''' <summary>
        ''' 训练行采样：把训练行限制到 <paramref name="maxRows"/> 以内。
        ''' </summary>
        ''' <remarks>
        ''' 用等间隔（确定性）抽样而不是随机抽样：结果可复现，时序数据下也仍覆盖整段轨迹。
        ''' 返回 <c>Nothing</c> 表示不采样（使用全部行）。
        ''' </remarks>
        Private Function SampleRows(rows As Integer, maxRows As Integer) As Integer()
            If maxRows <= 0 OrElse rows <= maxRows Then
                Return Nothing
            End If

            Dim index(maxRows - 1) As Integer
            Dim step As Double = rows / CDbl(maxRows)

            For k As Integer = 0 To maxRows - 1
                Dim value As Integer = CInt(std.Floor(k * step))

                If value >= rows Then value = rows - 1

                index(k) = value
            Next

            Return index
        End Function

        Private Function CleanValue(value As Double) As Double
            If value.IsNaNImaginary Then
                Return 0.0
            Else
                Return value
            End If
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

            Dim variances As Double() = GeneVariances(raw, nGene)
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

        ''' <summary>逐基因的原始表达方差（基因过滤用）。</summary>
        Private Function GeneVariances(raw As Double(,), nGene As Integer) As Double()
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

            Return variances
        End Function

    End Module

End Namespace
