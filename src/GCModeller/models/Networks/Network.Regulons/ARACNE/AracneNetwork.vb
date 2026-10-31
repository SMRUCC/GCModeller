Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Correlations
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.BioSystem.Core
Imports std = System.Math

Namespace ARACNE

    ''' <summary>
    ''' 一次 ARACNE 推断过程的统计信息
    ''' </summary>
    Public Class AracneStatistics

        ''' <summary>
        ''' 表达矩阵之中被扫描的基因数量
        ''' </summary>
        Public Property genesScanned As Integer

        ''' <summary>
        ''' 预处理之后实际参与计算的基因数量
        ''' </summary>
        Public Property genes As Integer

        ''' <summary>
        ''' 样本(细胞)数量
        ''' </summary>
        Public Property samples As Integer

        ''' <summary>
        ''' 匹配到的转录因子数量
        ''' </summary>
        Public Property transcriptionFactors As Integer

        ''' <summary>
        ''' 计算了互信息的基因对数量
        ''' </summary>
        Public Property candidatePairs As Long

        ''' <summary>
        ''' 互信息的显著性阈值
        ''' </summary>
        Public Property threshold As MiThreshold

        ''' <summary>
        ''' 阈值过滤以及 top-K 剪枝之后的边数量
        ''' </summary>
        Public Property edgesAfterThreshold As Integer

        ''' <summary>
        ''' DPI 过滤之后的边数量(无向)
        ''' </summary>
        Public Property edgesAfterDpi As Integer

        ''' <summary>
        ''' 赋予方向之后的调控边数量
        ''' </summary>
        Public Property edges As Integer

        ''' <summary>
        ''' curated 先验网络之中落在本次计算空间(基因集合)之内的调控关系数量
        ''' </summary>
        Public Property priorEdges As Integer

        ''' <summary>
        ''' 最终输出之中受先验支持的调控边数量(有向边的口径，TF-TF 的 mutual 边会被计算两次)
        ''' </summary>
        Public Property priorSupportedEdges As Integer

        ''' <summary>
        ''' 最终输出之中受先验支持的<b>无向基因对</b>数量。
        ''' 
        ''' 与 <see cref="priorEdges"/> 相除就得到先验网络在本次推断结果之中的召回率。
        ''' </summary>
        Public Property priorSupportedPairs As Integer

        ''' <summary>
        ''' 被先验保护的边数量：这些边满足 DPI 的剔除条件，
        ''' 但是因为 curated 先验支持而保留了下来
        ''' </summary>
        Public Property priorProtectedEdges As Integer

        ''' <summary>
        ''' 本次推断所实际使用的先验保护方式
        ''' </summary>
        Public Property priorProtection As PriorProtectionMode

        ''' <summary>
        ''' 被保护(从 DPI 之中救回来)的边按照先验置信度分级的统计：
        ''' 键名为 ``high`` / ``medium`` / ``low``。
        ''' 
        ''' 这是验证“分级保护是否生效”的直接证据：Graded 模式下
        ''' high 组应当明显多于 low 组。
        ''' </summary>
        Public Property priorRescuedByConfidence As New Dictionary(Of String, Integer)

        ''' <summary>
        ''' 落在本次计算空间之内的先验边，按照置信度分级的数量。
        ''' 
        ''' 与 <see cref="priorRescuedByConfidence"/> 相除即可得到每一档置信度
        ''' 对应的“保护成功率”，也就是分级保护是否真的按置信度倾斜的量化证据。
        ''' </summary>
        Public Property priorPairsByConfidence As New Dictionary(Of String, Integer)

        ''' <summary>
        ''' DPI 的详细统计
        ''' </summary>
        Public Property dpi As DpiResult

        ''' <summary>
        ''' 预处理阶段的耗时(秒)
        ''' </summary>
        Public Property preprocessSeconds As Double

        ''' <summary>
        ''' 互信息计算阶段的耗时(秒)
        ''' </summary>
        Public Property miSeconds As Double

        ''' <summary>
        ''' 阈值、DPI 与定向阶段的耗时(秒)
        ''' </summary>
        Public Property filterSeconds As Double

        ''' <summary>
        ''' 整个推断过程的耗时(秒)
        ''' </summary>
        Public Property totalSeconds As Double

        ''' <summary>
        ''' 平均每个基因对的互信息计算耗时(微秒)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property microSecondsPerPair As Double
            Get
                If candidatePairs <= 0 Then
                    Return 0.0R
                Else
                    Return miSeconds * 1000000.0R / candidatePairs
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Dim prior As String = If(priorEdges = 0,
                "",
                $", prior: {priorEdges} known -> {priorSupportedPairs} supported ({priorSupportedEdges} directed edges), " &
                $"{priorProtectedEdges} rescued from DPI")

            Dim graded As String = ""

            If priorRescuedByConfidence IsNot Nothing AndAlso priorRescuedByConfidence.Count > 0 Then
                graded = " (" & String.Join(", ", priorRescuedByConfidence.Select(Function(kv) $"{kv.Key}={kv.Value}")) & ")"
            End If

            Return $"{genes} genes x {samples} samples, {candidatePairs} pairs, " &
                $"{edgesAfterThreshold} -> {edgesAfterDpi} edges (threshold -> DPI), " &
                $"{edges} directed edges{prior}{graded}, {totalSeconds.ToString("F2")}s"
        End Function
    End Class

    ''' <summary>
    ''' ARACNE(Algorithm for the Reconstruction of Accurate Cellular Networks)
    ''' 基因表达调控网络的推断。
    ''' 
    ''' 整体的执行流程与 readme 之中的伪代码一一对应：
    ''' 
    ''' ```
    ''' 1. 预处理      : log2 变换 / 标准化 / 秩转换 / 零表达与零方差基因过滤
    ''' 2. 互信息矩阵  : 高斯 copula(默认) / 自适应分区 / 等频分箱 / 核密度
    ''' 3. 显著性阈值  : 置换零分布的分位数(默认 95%) 或者固定阈值
    ''' 4. DPI 过滤    : 剔除三元组之中明显最弱的间接边
    ''' 5. 方向赋予    : TF 指向靶基因 / TF 与 TF 的双向关系 / 非 TF 的无向候选边
    ''' ```
    ''' 
    ''' 提供转录因子列表的时候，计算空间会由 ``N x N`` 缩减为 ``T x N``
    ''' (只计算 TF-TF 与 TF-Target 的互信息)，这正是 readme 第四章所推荐的
    ''' “结合 TF 先验知识”的工程化流程，可以把计算量降低几个数量级。
    ''' </summary>
    ''' <remarks>
    ''' 这个类型只负责编排，具体的数值计算分别在
    ''' <see cref="MutualInformation"/>、<see cref="PermutationThreshold"/>、
    ''' <see cref="DpiFilter"/> 之中实现。
    ''' </remarks>
    Public Class AracneNetwork

        ''' <summary>
        ''' 输入的基因表达矩阵(行=基因、列=样本)，构造与推断的过程都不会修改它
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Expression As Matrix
            Get
                Return m_raw
            End Get
        End Property

        ''' <summary>
        ''' 本次推断所使用的参数配置
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Options As AracneOptions
            Get
                Return m_opt
            End Get
        End Property

        ''' <summary>
        ''' 预处理之后保留下来的基因编号(顺序与内部的表达向量一一对应)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Genes As String()
            Get
                Return m_genes
            End Get
        End Property

        ''' <summary>
        ''' 匹配到的转录因子编号
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TFs As String()
            Get
                Return m_tfs
            End Get
        End Property

        ''' <summary>
        ''' 每一个基因是否为转录因子(下标与 <see cref="Genes"/> 一致)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TfFlags As Boolean()
            Get
                Return m_tfFlags
            End Get
        End Property

        ''' <summary>
        ''' 预处理之后的基因表达向量(下标与 <see cref="Genes"/> 一致)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Vectors As Double()()
            Get
                Return m_vectors
            End Get
        End Property

        ''' <summary>
        ''' DPI 过滤之后的互信息矩阵(无向)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MiMatrix As ARACNE.MiMatrix
            Get
                Return m_network
            End Get
        End Property

        ''' <summary>
        ''' 阈值过滤以及 top-K 剪枝之后、DPI 之前的互信息矩阵
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MiMatrixBeforeDpi As ARACNE.MiMatrix
            Get
                Return m_stage1
            End Get
        End Property

        ''' <summary>
        ''' 互信息的显著性阈值
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Threshold As MiThreshold
            Get
                Return m_threshold
            End Get
        End Property

        ''' <summary>
        ''' 赋予方向之后的调控边
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Edges As AracneEdge()
            Get
                Return m_edges
            End Get
        End Property

        ''' <summary>
        ''' 推断过程的统计信息(尚未执行 <see cref="Fit"/> 的时候为<c>Nothing</c>)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Statistics As AracneStatistics
            Get
                Return m_stats
            End Get
        End Property

        ''' <summary>
        ''' 基因(节点)的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Integer
            Get
                Return m_genes.Length
            End Get
        End Property

        ReadOnly m_genes As String()
        ReadOnly m_geneIndex As Dictionary(Of String, Integer)
        ReadOnly m_vectors As Double()()
        ReadOnly m_unitVectors As Double()()
        ReadOnly m_tfFlags As Boolean()
        ReadOnly m_tfs As String()
        ReadOnly m_raw As Matrix
        ReadOnly m_opt As AracneOptions
        ReadOnly m_scanned As Integer
        ReadOnly m_preprocessSeconds As Double

        ''' <summary>
        ''' curated 先验网络解析之后的邻接表：无向边的键值(见
        ''' <see cref="MiMatrix.EdgeKey(Integer, Integer)"/>) -> 先验关系。
        ''' 
        ''' 只有置信度达到 <see cref="AracneOptions.priorMinConfidence"/> 并且
        ''' 两个端点都存在于本次计算空间之中的先验关系才会被收录。
        ''' </summary>
        ReadOnly m_prior As Dictionary(Of Long, RegulatoryEdge)

        Private m_network As ARACNE.MiMatrix
        Private m_stage1 As ARACNE.MiMatrix
        Private m_edges As AracneEdge() = New AracneEdge() {}
        Private m_threshold As MiThreshold = Nothing
        Private m_stats As AracneStatistics = Nothing

        ''' <summary>
        ''' 构造一个 ARACNE 推断对象。
        ''' 
        ''' 构造函数只做输入校验与预处理(过滤 / 变换 / TF 匹配)，互信息的计算
        ''' 需要显式调用 <see cref="Fit"/> (或者使用
        ''' <see cref="AracneExtensions.InferAracneNetwork"/> 一步完成)。
        ''' </summary>
        ''' <param name="expr">行是基因、列是样本的基因表达矩阵</param>
        ''' <param name="options">推断参数，为空的时候使用默认参数</param>
        Public Sub New(expr As Matrix, Optional options As AracneOptions = Nothing)
            If expr Is Nothing Then
                Throw New ArgumentNullException(NameOf(expr))
            End If
            If expr.expression Is Nothing OrElse expr.expression.Length < 2 Then
                Throw New InvalidDataException($"ARACNE requires at least 2 genes, but only {If(expr.expression Is Nothing, 0, expr.expression.Length)} was found!")
            End If

            m_opt = If(options, New AracneOptions())

            Dim nSamples As Integer = expr.sample_count

            If nSamples < m_opt.minSamples Then
                Throw New InvalidDataException($"ARACNE requires at least {m_opt.minSamples} samples for a stable mutual information estimation, but only {nSamples} was found!")
            End If

            m_raw = expr

            Dim duplicated As String() = expr.expression _
                .GroupBy(Function(r) r.geneID, StringComparer.OrdinalIgnoreCase) _
                .Where(Function(g) g.Count() > 1) _
                .Select(Function(g) g.Key) _
                .ToArray

            If duplicated.Length > 0 Then
                Throw New InvalidDataException($"the gene id must be unique, but these genes are duplicated: {duplicated.JoinBy(", ")}")
            End If

            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim kept As New List(Of (name As String, values As Double(), variance As Double))
            Dim scanned As Integer = 0

            For Each row As DataFrameRow In expr.expression
                scanned += 1

                If row.experiments Is Nothing OrElse row.experiments.Length <> nSamples Then
                    Throw New InvalidDataException($"the sample dimension of gene '{row.geneID}' is not equals to {nSamples}!")
                End If

                Dim values As Double()

                If m_opt.log2Transform Then
                    values = New Double(nSamples - 1) {}

                    For i As Integer = 0 To nSamples - 1
                        values(i) = MutualInformation.Log2(std.Max(0.0R, row.experiments(i)) + 1.0R)
                    Next
                Else
                    values = row.experiments
                End If

                Dim nonZero As Integer = 0
                Dim sum As Double = 0.0R

                For i As Integer = 0 To nSamples - 1
                    If values(i) > 0.0R Then
                        nonZero += 1
                    End If

                    sum += values(i)
                Next

                Dim mean As Double = sum / nSamples
                Dim variance As Double = 0.0R

                For i As Integer = 0 To nSamples - 1
                    variance += (values(i) - mean) ^ 2
                Next

                variance /= nSamples

                If variance <= 0.0R Then
                    ' 常量基因与任何基因之间的互信息都是零，直接丢弃
                    Continue For
                End If
                If m_opt.minNonZeroRatio > 0.0R AndAlso nonZero / CDbl(nSamples) < m_opt.minNonZeroRatio Then
                    Continue For
                End If

                Call kept.Add((row.geneID, values, variance))
            Next

            Dim tfSet As HashSet(Of String) = Nothing

            If m_opt.transcriptionFactors IsNot Nothing AndAlso m_opt.transcriptionFactors.Length > 0 Then
                tfSet = New HashSet(Of String)(
                    m_opt.transcriptionFactors _
                        .Where(Function(id) Not String.IsNullOrWhiteSpace(id)) _
                        .Select(Function(id) id.Trim),
                    StringComparer.OrdinalIgnoreCase)
            End If

            If m_opt.priorTfAugment AndAlso m_opt.priorNetwork IsNot Nothing AndAlso m_opt.priorNetwork.Length > 0 Then
                ' 转录因子注释本身也可能不完整：curated 先验之中的调控因子从定义上
                ' 就是转录因子，把它们补进 TF 集合可以扩大计算空间的覆盖度，
                ' 同时让 TF 导向的 DPI 与定向语义保持正确
                If tfSet Is Nothing Then
                    tfSet = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                End If

                For Each prior As RegulatoryEdge In m_opt.priorNetwork
                    If Not String.IsNullOrWhiteSpace(prior.TF) Then
                        Call tfSet.Add(prior.TF.Trim())
                    End If
                Next
            End If

            Dim selected As List(Of (name As String, values As Double(), variance As Double)) = kept

            If m_opt.maxGenes > 0 AndAlso kept.Count > m_opt.maxGenes Then
                Dim top As List(Of (name As String, values As Double(), variance As Double)) = kept _
                    .OrderByDescending(Function(r) r.variance) _
                    .Take(m_opt.maxGenes) _
                    .ToList

                If tfSet IsNot Nothing Then
                    ' 转录因子是调控网络的起点，即使方差排名靠后被截断也必须保留
                    Dim names As New HashSet(Of String)(top.Select(Function(r) r.name), StringComparer.OrdinalIgnoreCase)

                    For Each r In kept
                        If tfSet.Contains(r.name) AndAlso Not names.Contains(r.name) Then
                            Call top.Add(r)
                            Call names.Add(r.name)
                        End If
                    Next
                End If

                selected = top.OrderByDescending(Function(r) r.variance).ToList
            End If

            If selected.Count < 2 Then
                Throw New InvalidDataException($"no gene passes the preprocessing filters ({scanned} genes scanned), please relax minNonZeroRatio!")
            End If

            m_genes = selected.Select(Function(r) r.name).ToArray
            m_geneIndex = New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To m_genes.Length - 1
                m_geneIndex(m_genes(i)) = i
            Next

            m_vectors = New Double(m_genes.Length - 1)() {}
            m_unitVectors = New Double(m_genes.Length - 1)() {}
            m_tfFlags = New Boolean(m_genes.Length - 1) {}

            For i As Integer = 0 To m_genes.Length - 1
                Dim v As Double() = selected(i).values

                If m_opt.rankTransform Then
                    v = MutualInformation.Ranks(v)
                End If
                If m_opt.standardize Then
                    v = MutualInformation.Standardize(v)
                End If

                m_vectors(i) = v
                m_unitVectors(i) = MutualInformation.UnitVector(v)

                If tfSet IsNot Nothing Then
                    m_tfFlags(i) = tfSet.Contains(m_genes(i))
                End If
            Next

            m_tfs = Enumerable _
                .Range(0, m_genes.Length) _
                .Where(Function(i) m_tfFlags(i)) _
                .Select(Function(i) m_genes(i)) _
                .ToArray

            If tfSet IsNot Nothing AndAlso m_tfs.Length = 0 Then
                Throw New InvalidDataException($"none of the {tfSet.Count} transcription factors is found in the expression matrix!")
            End If

            ' ---- curated 先验网络：映射到本次计算空间的节点下标上 ----
            m_prior = New Dictionary(Of Long, RegulatoryEdge)

            If m_opt.priorNetwork IsNot Nothing Then
                For Each prior As RegulatoryEdge In m_opt.priorNetwork
                    Dim u As Integer
                    Dim v As Integer

                    If prior.Confidence < m_opt.priorMinConfidence Then
                        Continue For
                    End If
                    If String.IsNullOrWhiteSpace(prior.TF) OrElse String.IsNullOrWhiteSpace(prior.TargetGene) Then
                        Continue For
                    End If
                    If Not m_geneIndex.TryGetValue(prior.TF.Trim(), u) Then
                        Continue For
                    End If
                    If Not m_geneIndex.TryGetValue(prior.TargetGene.Trim(), v) Then
                        Continue For
                    End If
                    If u = v Then
                        Continue For
                    End If

                    Dim key As Long = ARACNE.MiMatrix.EdgeKey(u, v)

                    ' 同一条无向边可能被不同的数据库重复注释，保留置信度最高的那一条
                    If Not m_prior.ContainsKey(key) OrElse m_prior(key).Confidence < prior.Confidence Then
                        m_prior(key) = prior
                    End If
                Next
            End If

            watch.Stop()

            m_preprocessSeconds = watch.Elapsed.TotalSeconds
            m_scanned = scanned
        End Sub

        ''' <summary>
        ''' 执行 ARACNE 推断。
        ''' 
        ''' 重复调用会重新计算一次(便于在同一个表达矩阵之上比较不同的参数)。
        ''' </summary>
        ''' <returns>当前对象(便于链式调用)</returns>
        Public Function Fit() As AracneNetwork
            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim nGenes As Integer = m_genes.Length
            Dim rows As Integer() = If(m_tfs.Length > 0,
                m_tfs.Select(Function(id) m_geneIndex(id)).ToArray,
                Enumerable.Range(0, nGenes).ToArray)
            Dim fastCopula As Boolean = m_opt.miEstimator = MiEstimator.GaussianCopula
            Dim fn As MiEstimatorFn = Nothing

            If Not fastCopula Then
                fn = MutualInformation.GetEstimator(m_opt.miEstimator, m_opt)
            End If

            Dim result As List(Of (v As Integer, mi As Double))() = New List(Of (v As Integer, mi As Double))(rows.Length - 1) {}
            Dim popts As New ParallelOptions()

            If m_opt.maxDegreeOfParallelism > 0 Then
                popts.MaxDegreeOfParallelism = m_opt.maxDegreeOfParallelism
            End If

            If Not m_opt.silent Then
                Console.WriteLine($"[ARACNE] computing mutual information: {rows.Length} regulators x {nGenes} genes, " &
                                  $"{m_vectors(0).Length} samples, estimator = {m_opt.miEstimator}")
            End If

            Dim miWatch As Stopwatch = Stopwatch.StartNew()

            Parallel.For(0, rows.Length, popts,
                Sub(r)
                    Dim u As Integer = rows(r)
                    Dim list As New List(Of (v As Integer, mi As Double))
                    Dim zu As Double() = m_unitVectors(u)
                    Dim xu As Double() = m_vectors(u)

                    For v As Integer = 0 To nGenes - 1
                        If v = u Then
                            Continue For
                        End If

                        If m_tfs.Length > 0 Then
                            ' 无向的基因对只计算一次：当 v 也是转录因子的时候，
                            ' 它自己作为行的时候会再枚举一次 (v, u)
                            If v < u AndAlso m_tfFlags(v) Then
                                Continue For
                            End If
                        ElseIf v < u Then
                            Continue For
                        End If

                        Dim value As Double

                        If fastCopula Then
                            value = MutualInformation.CopulaMiFromUnitVectors(zu, m_unitVectors(v))
                        Else
                            value = fn(xu, m_vectors(v))
                        End If

                        If value > 0.0R AndAlso Not Double.IsNaN(value) Then
                            Call list.Add((v, value))
                        End If
                    Next

                    result(r) = list
                End Sub)

            miWatch.Stop()

            Dim mi As New ARACNE.MiMatrix(m_genes)
            Dim pairCount As Long = 0

            For r As Integer = 0 To rows.Length - 1
                Dim u As Integer = rows(r)
                Dim list As List(Of (v As Integer, mi As Double)) = result(r)

                If list Is Nothing Then
                    Continue For
                End If

                For Each item In list
                    Call mi.SetValue(u, item.v, item.mi)
                Next

                pairCount += list.Count
            Next

            If Not m_opt.silent Then
                Console.WriteLine($"[ARACNE] {pairCount} gene pairs, {miWatch.Elapsed.TotalSeconds:F2}s " &
                                  $"({If(pairCount = 0, 0, miWatch.Elapsed.TotalMilliseconds * 1000.0R / pairCount):F3} us/pair)")
            End If

            ' ---- 显著性阈值 tau ----
            Dim filterWatch As Stopwatch = Stopwatch.StartNew()
            Dim rng As New Random(m_opt.randomSeed)
            Dim sampled As (u As Integer, v As Integer)() = SamplePairs(rows, nGenes, rng)

            m_threshold = PermutationThreshold.Estimate(m_vectors, sampled, m_opt.miEstimator, m_opt)

            If Not m_opt.silent Then
                Console.WriteLine($"[ARACNE] {m_threshold.ToString}")
            End If

            ' ---- 阈值过滤 + top-K 剪枝(先验支持的边享有按置信度分级的放宽阈值与剪枝保护) ----
            Dim thresholdFactor As Double = std.Max(0.0R, std.Min(1.0R, m_opt.priorThresholdFactor))
            Dim priorThresholds As Dictionary(Of Long, Double) = Nothing
            Dim priorProtection As Dictionary(Of Long, Double) = Nothing
            Dim priorKeys As HashSet(Of Long) = Nothing

            If m_prior.Count > 0 Then
                priorThresholds = New Dictionary(Of Long, Double)(m_prior.Count)
                priorProtection = New Dictionary(Of Long, Double)(m_prior.Count)
                priorKeys = New HashSet(Of Long)

                ' 概率保护模式需要一个可复现的随机数发生器(与置换抽样的随机流相互独立)
                Dim protectRng As New Random(m_opt.randomSeed Xor &H5A5A)

                For Each item As KeyValuePair(Of Long, RegulatoryEdge) In m_prior
                    Dim confidence As Double = std.Max(0.0R, std.Min(1.0R, item.Value.Confidence))

                    ' 阈值放宽同样按置信度分级：
                    ' tau_prior(c) = tau * (1 - (1 - factor) * c)
                    priorThresholds(item.Key) = m_threshold.value * (1.0R - (1.0R - thresholdFactor) * confidence)
                    Call priorKeys.Add(item.Key)

                    Select Case EffectivePriorProtection()
                        Case PriorProtectionMode.Graded
                            ' 确定性的分级容错窗口：strength = priorProtectionStrength * c
                            priorProtection(item.Key) = m_opt.priorProtectionStrength * confidence
                        Case PriorProtectionMode.Sampled
                            ' 概率保护：以 p = confidence 的概率获得完全保护(强度 1)，否则不保护
                            If protectRng.NextDouble() < confidence Then
                                priorProtection(item.Key) = 1.0R
                            End If
                        Case Else
                            ' Off：逐条边都不写入保护强度
                    End Select
                Next
            End If

            Dim stage1 As ARACNE.MiMatrix = mi.Prune(m_threshold.value, priorThresholds)

            If m_opt.maxNeighbors > 0 Then
                stage1 = stage1.PruneTopK(m_opt.maxNeighbors, priorKeys)
            End If

            If Not m_opt.silent AndAlso priorKeys IsNot Nothing Then
                Dim supported As Integer = stage1.Edges().Count(Function(e) priorKeys.Contains(ARACNE.MiMatrix.EdgeKey(e.u, e.v)))

                Console.WriteLine($"[ARACNE] prior: {priorKeys.Count} known pairs, {supported} survive the graded threshold " &
                                  $"(tau = {m_threshold.value.ToString("G4")}, factor = {thresholdFactor.ToString("G3")}, " &
                                  $"protection = {EffectivePriorProtection()} x {m_opt.priorProtectionStrength.ToString("G3")})")
            End If

            m_stage1 = stage1

            ' ---- DPI 去间接边(先验支持的边可以按置信度分级地保护) ----
            Dim dpi As DpiResult = DpiFilter.Apply(stage1, m_opt, m_tfFlags, priorProtection)

            m_network = dpi.Matrix
            filterWatch.Stop()

            ' ---- 方向赋予 ----
            m_edges = AssignDirection(m_network).ToArray

            ' 先验召回率需要按“无向基因对”口径统计：TF-TF 的 mutual 边会出现两次
            Dim supportedPairs As Integer = m_edges _
                .Where(Function(e) e.prior) _
                .Select(Function(e) ARACNE.MiMatrix.EdgeKey(m_geneIndex(e.regulator), m_geneIndex(e.target))) _
                .Distinct _
                .Count

            ' 按先验置信度分级统计“被救回来”的边数量：这是分级保护是否生效的直接证据
            Dim rescued As New Dictionary(Of String, Integer) From {
                {"high", 0},
                {"medium", 0},
                {"low", 0}
            }

            For Each edge As MiEdge In dpi.ProtectedEdges
                Dim evidence As RegulatoryEdge = Nothing
                Dim confidence As Double = 0.0R

                If m_prior.TryGetValue(ARACNE.MiMatrix.EdgeKey(edge.u, edge.v), evidence) Then
                    confidence = evidence.Confidence
                End If

                Dim grade As String = ConfidenceGrade(confidence)
                rescued(grade) = rescued(grade) + 1
            Next

            Dim pairsByGrade As New Dictionary(Of String, Integer) From {
                {"high", 0},
                {"medium", 0},
                {"low", 0}
            }

            For Each evidence As RegulatoryEdge In m_prior.Values
                Dim grade As String = ConfidenceGrade(evidence.Confidence)

                pairsByGrade(grade) = pairsByGrade(grade) + 1
            Next

            watch.Stop()

            m_stats = New AracneStatistics With {
                .priorProtection = EffectivePriorProtection(),
                .priorRescuedByConfidence = rescued,
                .priorPairsByConfidence = pairsByGrade,
                .genesScanned = m_scanned,
                .genes = nGenes,
                .samples = m_vectors(0).Length,
                .transcriptionFactors = m_tfs.Length,
                .candidatePairs = pairCount,
                .threshold = m_threshold,
                .edgesAfterThreshold = stage1.Count,
                .edgesAfterDpi = m_network.Count,
                .edges = m_edges.Length,
                .priorEdges = If(priorKeys Is Nothing, 0, priorKeys.Count),
                .priorSupportedEdges = m_edges.Count(Function(e) e.prior),
                .priorSupportedPairs = supportedPairs,
                .priorProtectedEdges = dpi.ProtectedEdges.Count,
                .dpi = dpi,
                .preprocessSeconds = m_preprocessSeconds,
                .miSeconds = miWatch.Elapsed.TotalSeconds,
                .filterSeconds = filterWatch.Elapsed.TotalSeconds,
                .totalSeconds = watch.Elapsed.TotalSeconds + m_preprocessSeconds
            }

            If Not m_opt.silent Then
                Console.WriteLine($"[ARACNE] {m_stats.ToString}")
            End If

            Return Me
        End Function

        ''' <summary>
        ''' 按照 TF 先验给互信息网络之中的边赋予方向
        ''' </summary>
        ''' <param name="mi"></param>
        ''' <returns></returns>
        Private Iterator Function AssignDirection(mi As ARACNE.MiMatrix) As IEnumerable(Of AracneEdge)
            For Each e As MiEdge In mi.Edges()
                Dim u As Integer = e.u
                Dim v As Integer = e.v
                Dim tfU As Boolean = m_tfFlags(u)
                Dim tfV As Boolean = m_tfFlags(v)
                Dim cor As Double = Correlations.GetPearson(m_vectors(u), m_vectors(v))
                Dim evidence As RegulatoryEdge = Nothing
                Dim hasPrior As Boolean = m_prior.TryGetValue(ARACNE.MiMatrix.EdgeKey(u, v), evidence)

                If hasPrior Then
                    ' 先验同时给出了方向(TF -> Target)与因果符号，
                    ' 因此优先采用先验的方向，避免 TF-TF 之间被折叠成双向边
                    Dim forward As Boolean = String.Equals(m_genes(u), evidence.TF.Trim(), StringComparison.OrdinalIgnoreCase)
                    Dim regulation As String = RegulationOf(evidence.Sign)

                    If tfU AndAlso tfV Then
                        ' 两个端点都是转录因子：数据上无法定向，保留双向关系
                        Yield NewEdge(e, m_genes(u), m_genes(v), "mutual", cor, True, True, regulation)
                        Yield NewEdge(e, m_genes(v), m_genes(u), "mutual", cor, True, True, regulation)
                    Else
                        Dim source As Integer = If(forward, u, v)
                        Dim sink As Integer = If(forward, v, u)
                        Dim interaction As String = If(m_opt.directionMode = DirectionMode.TfDirected, "directed", "undirected")

                        Yield NewEdge(e, m_genes(source), m_genes(sink), interaction, cor, tfU, tfV, regulation)
                    End If
                ElseIf m_opt.directionMode = DirectionMode.TfDirected Then
                    If tfU AndAlso Not tfV Then
                        Yield NewEdge(e, m_genes(u), m_genes(v), "directed", cor, True, False, "")
                    ElseIf tfV AndAlso Not tfU Then
                        Yield NewEdge(e, m_genes(v), m_genes(u), "directed", cor, True, False, "")
                    ElseIf tfU AndAlso tfV Then
                        ' 两个端点都是转录因子：构成 TF-TF 的双向调控关系
                        Yield NewEdge(e, m_genes(u), m_genes(v), "mutual", cor, True, True, "")
                        Yield NewEdge(e, m_genes(v), m_genes(u), "mutual", cor, True, True, "")
                    Else
                        ' 两个端点都不是转录因子：保留为无向的候选边
                        Yield NewEdge(e, m_genes(u), m_genes(v), "undirected", cor, False, False, "")
                    End If
                Else
                    Yield NewEdge(e, m_genes(u), m_genes(v), "undirected", cor, tfU, tfV, "")
                End If
            Next
        End Function

        ''' <summary>
        ''' 构造一条输出边
        ''' </summary>
        Private Function NewEdge(e As MiEdge,
                                 regulator As String,
                                 target As String,
                                 interaction As String,
                                 correlation As Double,
                                 isTfSource As Boolean,
                                 isTfTarget As Boolean,
                                 regulation As String) As AracneEdge

            Return New AracneEdge With {
                .regulator = regulator,
                .target = target,
                .mi = e.mi,
                .correlation = correlation,
                .pvalue = 1.0R,
                .interaction = interaction,
                .isTfSource = isTfSource,
                .isTfTarget = isTfTarget,
                .prior = m_prior.ContainsKey(ARACNE.MiMatrix.EdgeKey(e.u, e.v)),
                .regulation = regulation
            }
        End Function

        ''' <summary>
        ''' 实际生效的先验保护方式：<see cref="AracneOptions.protectPriorEdges"/>
        ''' 关闭的时候等价于 <see cref="PriorProtectionMode.Off"/>。
        ''' </summary>
        ''' <returns></returns>
        Private Function EffectivePriorProtection() As PriorProtectionMode
            If Not m_opt.protectPriorEdges Then
                Return PriorProtectionMode.Off
            Else
                Return m_opt.priorProtectionMode
            End If
        End Function

        ''' <summary>
        ''' 把先验置信度归入 high / medium / low 三档(用于统计分级保护的效果)
        ''' </summary>
        ''' <param name="confidence"></param>
        ''' <returns></returns>
        Private Shared Function ConfidenceGrade(confidence As Double) As String
            If confidence >= 0.9R Then
                Return "high"
            ElseIf confidence >= 0.6R Then
                Return "medium"
            Else
                Return "low"
            End If
        End Function

        ''' <summary>
        ''' 把先验的因果符号转换成可读的调控类型
        ''' </summary>
        ''' <param name="sign">+1 激活、-1 抑制、0 未知</param>
        ''' <returns></returns>
        Private Shared Function RegulationOf(sign As Integer) As String
            If sign > 0 Then
                Return "activator"
            ElseIf sign < 0 Then
                Return "inhibitor"
            Else
                Return ""
            End If
        End Function

        ''' <summary>
        ''' 枚举推断出来的调控边(按照互信息降序)
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function Regulations() As IEnumerable(Of AracneEdge)
            For Each edge As AracneEdge In m_edges.OrderByDescending(Function(e) e.mi)
                Yield edge
            Next
        End Function

        ''' <summary>
        ''' 把调控边导出为 CSV 表格
        ''' (``regulator,target,mi,correlation,interaction,is_tf_source,is_tf_target``)
        ''' </summary>
        ''' <param name="csv">目标文件的路径，父文件夹不存在的时候会自动创建</param>
        Public Sub WriteEdges(csv As String)
            If String.IsNullOrEmpty(csv) Then
                Throw New ArgumentNullException(NameOf(csv))
            End If

            Dim folder As String = Path.GetDirectoryName(csv)

            If Not String.IsNullOrEmpty(folder) AndAlso Not Directory.Exists(folder) Then
                Call Directory.CreateDirectory(folder)
            End If

            Using writer As New StreamWriter(csv, False, Encoding.UTF8)
                Call writer.WriteLine("regulator,target,mi,correlation,interaction,is_tf_source,is_tf_target,prior,regulation")

                For Each edge As AracneEdge In Regulations()
                    Call writer.WriteLine(String.Join(",",
                        edge.regulator,
                        edge.target,
                        edge.mi.ToString("G6", Globalization.CultureInfo.InvariantCulture),
                        edge.correlation.ToString("G6", Globalization.CultureInfo.InvariantCulture),
                        edge.interaction,
                        If(edge.isTfSource, "1", "0"),
                        If(edge.isTfTarget, "1", "0"),
                        If(edge.prior, "1", "0"),
                        If(edge.regulation, "")))
                Next
            End Using
        End Sub

        ''' <summary>
        ''' 把互信息网络转换成工程既有的 <see cref="Matrix"/> 对象
        ''' (行是调控因子、列是靶基因)，便于复用既有的矩阵导出与可视化流程。
        ''' </summary>
        ''' <returns></returns>
        Public Function CoefMatrix() As Matrix
            Return m_network.ToMatrix($"ARACNE[{m_opt.miEstimator}, {m_opt.thresholdMode}, {m_network.Count} edges]")
        End Function

        ''' <summary>
        ''' 从候选的基因对之中随机抽取若干个，用于置换零分布的估计
        ''' </summary>
        ''' <param name="rows">参与计算的调控因子行号</param>
        ''' <param name="nGenes">基因数量</param>
        ''' <param name="rng">随机数发生器</param>
        ''' <returns></returns>
        Private Function SamplePairs(rows As Integer(), nGenes As Integer, rng As Random) As (u As Integer, v As Integer)()
            Dim count As Integer = If(m_opt.permutationPairs > 0, m_opt.permutationPairs, 1000)
            Dim result As New List(Of (u As Integer, v As Integer))(count)
            Dim attempts As Integer = 0

            ' 候选对的数量可能远小于请求的数量，因此这里用尝试次数做上限保护
            While result.Count < count AndAlso attempts < count * 10 + 100
                attempts += 1

                Dim u As Integer = rows(rng.Next(rows.Length))
                Dim v As Integer = rng.Next(nGenes)

                If v <> u Then
                    Call result.Add((u, v))
                End If
            End While

            Return result.ToArray
        End Function

        Public Overrides Function ToString() As String
            If m_stats Is Nothing Then
                Return $"[ARACNE] {m_genes.Length} genes, {m_vectors(0).Length} samples, not fitted yet"
            Else
                Return $"[ARACNE] {m_stats.ToString}"
            End If
        End Function
    End Class
End Namespace
