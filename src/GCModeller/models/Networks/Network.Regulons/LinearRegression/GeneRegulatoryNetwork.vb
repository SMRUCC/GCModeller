Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data
Imports Microsoft.VisualBasic.Data.Bootstrapping.ElasticNet
Imports Microsoft.VisualBasic.Data.Bootstrapping.LASSO
Imports Microsoft.VisualBasic.Data.Bootstrapping.Multivariate
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace LinearRegression

    ''' <summary>
    ''' 基于线性拟合的基因表达调控网络推断
    ''' 
    ''' 对于表达矩阵之中的每一个基因 ``g``，都会建立如下的线性模型：
    ''' 
    ''' ```
    ''' expr(g) = const + beta_1 * expr(r1) + beta_2 * expr(r2) + ...
    ''' ```
    ''' 
    ''' 其中 ``r1, r2, ...`` 为候选调控因子(默认从与 ``g`` 相关性最高的若干个基因之中选出，
    ''' 并且总是排除 ``g`` 自身)。把所有基因的模型汇总之后，就得到了一个
    ''' ``调控因子 -&gt; 目标基因`` 的有向加权网络：边的权重即为回归系数。
    ''' </summary>
    ''' <remarks>
    ''' 需要注意前置条件：参与拟合的自变量个数(候选调控因子数量 + 截距项)必须显著小于样本数量。
    ''' 
    ''' + 对于 <see cref="FittingMethod.MultipleLinear"/>，DataFittings 底层使用 QR 分解，
    '''   秩不足的时候会直接抛出 ``Matrix is rank deficient`` 异常；
    ''' + 对于 <see cref="FittingMethod.Lasso"/>，DataFittings 的 ``LassoFitGenerator``
    '''   在初始化内部观测数组的时候使用了错误的维度上限，自变量个数大于样本数量的时候会越界。
    ''' 
    ''' 因此这里会通过 <see cref="GrnFittingOptions.maxCandidates"/> 与
    ''' <see cref="GrnFittingOptions.candidateSampleRatio"/> 强制限制候选因子的数量。
    ''' 基因数量远大于样本数量的时候(转录组数据的常态)，请先进行表达量过滤、
    ''' 差异基因筛选或者PCA/模块降维。
    ''' </remarks>
    Public Class GeneRegulatoryNetwork

        ''' <summary>
        ''' 输入的输出表达矩阵(行=基因，列=样本)，构造的时候会做深拷贝，不会被本模块修改
        ''' </summary>
        ReadOnly raw As Matrix

        ReadOnly opt As GrnFittingOptions
        ReadOnly geneNames As String()
        ReadOnly sampleNames As String()


        ''' <summary>
        ''' 每一个基因的居中且 L2 归一化之后的表达向量，
        ''' 两个基因的 pearson 相关性等价于这两个向量的内积。
        ''' </summary>
        ReadOnly unitVectors As Double()()

        ''' <summary>
        ''' 候选调控因子在表达矩阵之中的行下标
        ''' </summary>
        ReadOnly regulatorPool As Integer()

        ''' <summary>
        ''' 需要优先纳入候选池的基因在表达矩阵之中的行下标
        ''' (来自 <see cref="GrnFittingOptions.priorityRegulators"/>)。
        ''' 
        ''' 这些基因不受候选池截断的影响，总是会被追加进每一个目标基因的候选集。
        ''' </summary>
        ReadOnly priorityPool As Integer()

        ''' <summary>
        ''' 本次 <see cref="Fit"/> 期间生效的相关性筛选后端；为空时使用内置的逐基因扫描。
        ''' 
        ''' 它只在 <see cref="Fit"/> 的生命周期内有效（Initialize 之后、Dispose 之前），
        ''' 拟合完成之后会被清空。
        ''' </summary>
        Private activeScreener As ICorrelationScreener = Nothing

        ''' <summary>
        ''' 拟合好的模型，键名是目标基因的编号
        ''' </summary>
        ReadOnly modelTable As New Dictionary(Of String, GeneRegressionModel)

        ''' <summary>
        ''' 全局交叉验证模式之下解析出来的共享 alpha，小于等于零表示没有启用该模式
        ''' </summary>
        Private cvAlpha As Double = -1

        ''' <summary>
        ''' 全局交叉验证模式之下解析出来的共享 L1 混合比例
        ''' </summary>
        Private cvL1Ratio As Double = -1

        ''' <summary>
        ''' <see cref="Baseline"/> 的惰性计算结果缓存
        ''' </summary>
        Private baselineCache As Double() = Nothing

        ''' <summary>
        ''' <see cref="BaselineSD"/> 的惰性计算结果缓存
        ''' </summary>
        Private baselineSdCache As Double() = Nothing

        ''' <summary>
        ''' 从表达矩阵构造一个待拟合的调控网络推断模型
        ''' </summary>
        ''' <param name="expr">行基因为、列样本的表达矩阵</param>
        ''' <param name="fittingOptions">线性拟合的参数配置，为空的时候使用默认参数</param>
        Public Sub New(expr As Matrix, Optional fittingOptions As GrnFittingOptions = Nothing)
            If expr Is Nothing Then
                Throw New ArgumentNullException(NameOf(expr))
            End If

            Me.raw = expr
            Me.opt = If(fittingOptions, New GrnFittingOptions())
            Me.geneNames = expr.rownames
            Me.sampleNames = expr.sampleID

            Call ValidateInput()

            Me.Vectors = expr _
                .expression _
                .Select(Function(r) DirectCast(r.experiments.Clone(), Double())) _
                .ToArray

            If Me.opt.standardize Then
                For i As Integer = 0 To Vectors.Length - 1
                    Vectors(i) = Standardize(Vectors(i))
                Next
            End If

            Me.unitVectors = Vectors _
                .Select(AddressOf UnitVector) _
                .ToArray

            If Me.opt.regulators Is Nothing Then
                Me.regulatorPool = Enumerable.Range(0, geneNames.Length).ToArray
            Else
                Dim pool As New List(Of Integer)

                For i As Integer = 0 To geneNames.Length - 1
                    If Me.opt.regulators.IndexOf(geneNames(i)) >= 0 Then
                        pool.Add(i)
                    End If
                Next

                If pool.Count = 0 Then
                    Throw New ArgumentException(
                        "none of the given regulator genes could be found in the expression matrix rows!",
                        NameOf(fittingOptions)
                    )
                End If

                Me.regulatorPool = pool.ToArray
            End If

            If Me.opt.priorityRegulators Is Nothing OrElse Me.opt.priorityRegulators.Length = 0 Then
                Me.priorityPool = New Integer() {}
            Else
                ' 大小写不敏感匹配，表达矩阵之中不存在的基因会被静默忽略
                Dim wanted As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

                For Each name As String In Me.opt.priorityRegulators
                    If Not String.IsNullOrEmpty(name) Then
                        Call wanted.Add(name)
                    End If
                Next

                Dim priority As New List(Of Integer)

                For i As Integer = 0 To geneNames.Length - 1
                    If wanted.Contains(geneNames(i)) Then
                        Call priority.Add(i)
                    End If
                Next

                Me.priorityPool = priority.ToArray
            End If
        End Sub

        ''' <summary>
        ''' 表达矩阵之中的基因编号列表
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Genes As String()
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return geneNames
            End Get
        End Property

        ''' <summary>
        ''' 基因的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Integer
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return geneNames.Length
            End Get
        End Property

        ''' <summary>
        ''' 当前分析所使用的参数配置
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Options As GrnFittingOptions
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return opt
            End Get
        End Property

        ''' <summary>
        ''' 构造当前网络对象时所使用的表达矩阵(行是基因、列是样本)。
        ''' 
        ''' 返回的就是构造函数的入参引用，本模块不会对它做任何修改，但调用方
        ''' 也不应该在拿到引用之后修改其中的数据，否则会破坏已经拟合好的模型。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Expression As Matrix
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return raw
            End Get
        End Property

        ''' <summary>
        ''' 每一个基因在拟合过程之中真正参与计算所使用的表达向量，下标与
        ''' <see cref="Genes"/> 一致。
        ''' 
        ''' 开启了 <see cref="GrnFittingOptions.standardize"/> 之后，这里保存的是
        ''' z-score 标准化之后的数据；否则就是原始表达量的一份副本。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Vectors As Double()()

        ''' <summary>
        ''' 每一个基因的基线表达量，即 <see cref="Vectors"/> 之中的样本均值。
        ''' 
        ''' 开启了 <see cref="GrnFittingOptions.standardize"/> 之后基线位于 z-score
        ''' 空间，理论上恒为零；否则就是原始表达量尺度之上的均值。
        ''' 
        ''' 这个数组是虚拟扰动计算的起点：敲除(target=0)、敲降(target=residual*基线)
        ''' 与过表达(target=fold*基线)都是以它为参照定义的。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Baseline As Double()
            Get
                If baselineCache Is Nothing Then
                    baselineCache = Vectors _
                        .Select(Function(v) If(v Is Nothing OrElse v.Length = 0, 0.0R, v.Average)) _
                        .ToArray
                End If

                Return baselineCache
            End Get
        End Property

        ''' <summary>
        ''' 每一个基因在 <see cref="Vectors"/> 之上的样本标准差(ddof = n - 1)。
        ''' 
        ''' 这个自由度约定与 <see cref="GrnFittingOptions.standardize"/> 的算法一致，
        ''' 因此开启了标准化之后该数组恒为 1。样本数量不足 2 或者表达量为常数的
        ''' 基因返回零，调用方需要自行守卫除零。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property BaselineSD As Double()
            Get
                If baselineSdCache Is Nothing Then
                    baselineSdCache = Vectors _
                        .Select(AddressOf SampleStandardDeviation) _
                        .ToArray
                End If

                Return baselineSdCache
            End Get
        End Property

        ''' <summary>
        ''' 所有已经拟合成功的基因模型
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Models As IEnumerable(Of GeneRegressionModel)
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return modelTable.Values
            End Get
        End Property

        ''' <summary>
        ''' 全局交叉验证模式之下解析出来的共享 alpha，小于等于零表示没有启用该模式
        ''' 
        ''' 这个数值只用于模型文件的持久化，不会影响已经拟合好的模型。
        ''' </summary>
        ''' <returns></returns>
        Friend ReadOnly Property CrossValidationAlpha As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return cvAlpha
            End Get
        End Property

        ''' <summary>
        ''' 全局交叉验证模式之下解析出来的共享 L1 混合比例，小于等于零表示没有启用该模式
        ''' 
        ''' 这个数值只用于模型文件的持久化，不会影响已经拟合好的模型。
        ''' </summary>
        ''' <returns></returns>
        Friend ReadOnly Property CrossValidationL1Ratio As Double
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return cvL1Ratio
            End Get
        End Property

        ''' <summary>
        ''' 使用已经拟合好的模型集合重建当前网络对象的内部状态
        ''' 
        ''' 该方法由模型文件的反序列化流程调用：表达矩阵、标准化之后的表达向量、
        ''' 候选调控因子池这些可以从原始数据推导出来的内容都由构造函数重新计算，
        ''' 这里只负责把拟合结果与交叉验证超参数回填到对象之中。
        ''' </summary>
        ''' <param name="models">每一个目标基因的线性模型</param>
        ''' <param name="crossValidationAlpha">全局交叉验证解析出来的共享 alpha</param>
        ''' <param name="crossValidationL1Ratio">全局交叉验证解析出来的共享 L1 混合比例</param>
        Friend Sub RestoreModels(models As IEnumerable(Of GeneRegressionModel),
                                 Optional crossValidationAlpha As Double = -1,
                                 Optional crossValidationL1Ratio As Double = -1)

            modelTable.Clear()

            If models IsNot Nothing Then
                For Each m As GeneRegressionModel In models
                    If m Is Nothing OrElse String.IsNullOrEmpty(m.target) Then
                        Continue For
                    End If

                    modelTable(m.target) = m
                Next
            End If

            Me.baselineCache = Nothing
            Me.baselineSdCache = Nothing
            Me.cvAlpha = crossValidationAlpha
            Me.cvL1Ratio = crossValidationL1Ratio
        End Sub

        ''' <summary>
        ''' 基于线性拟合重建基因调控网络
        ''' </summary>
        ''' <returns>返回当前对象自身，便于链式调用</returns>
        Public Function Fit() As GeneRegulatoryNetwork
            Dim nG As Integer = geneNames.Length
            Dim maxCand As Integer = CandidateBudget()
            Dim buffer As GeneRegressionModel() = New GeneRegressionModel(nG - 1) {}
            Dim popt As New ParallelOptions With {
                .MaxDegreeOfParallelism = opt.maxDegreeOfParallelism
            }

            If opt.method = FittingMethod.ElasticNet Then
                Call opt.elasticNet.Validate()

                If opt.elasticNet.mode = ElasticNetMode.GlobalCrossValidation Then
                    ' 全局交叉验证需要在逐基因拟合之前先解析出共享的超参数
                    Dim hyper = ResolveElasticNetHyperparameters(maxCand)

                    cvAlpha = hyper.alpha
                    cvL1Ratio = hyper.l1Ratio
                End If
            End If

            If Not opt.silent Then
                Call Console.WriteLine($"[GRN] {opt.ToString}")
                Call Console.WriteLine($"[GRN] {geneNames.Length} genes x {sampleNames.Length} samples, " &
                                       $"<= {maxCand} candidate regulators for each target gene")

                If opt.method = FittingMethod.ElasticNet AndAlso opt.elasticNet.mode = ElasticNetMode.GlobalCrossValidation Then
                    Call Console.WriteLine($"[GRN] elastic net global cv: alpha={cvAlpha.ToString("G4")}, l1_ratio={cvL1Ratio.ToString("G4")}")
                End If
            End If

            modelTable.Clear()

            If opt.method = FittingMethod.Lasso Then
                ' LASSO 求解器会在控制台打印每一个目标基因的正则化路径进度，
                ' 并行拟合的时候这些输出是交错的，因此默认全部关闭
                LassoFitGenerator.tqdm_verbose = False
            End If

            Call "run parallel for fit elastic network".debug

            ' 相关性筛选后端：一次性算出每个目标基因的候选集，之后 Query 只读
            Dim backend As ICorrelationScreener = opt.correlationBackend

            If opt.partialCorrelation Then
                ' 偏相关(以全局 hub 基因为条件集)优先于逐基因 Pearson 扫描；
                ' 显式指定的 correlationBackend 仍然优先于本开关
                If backend Is Nothing Then
                    backend = New PartialCorrelationScreener With {
                        .ConditionSize = std.Max(0, opt.partialConditionSize)
                    }

                    If Not opt.silent Then
                        Call Console.WriteLine($"[GRN] candidate screening uses the partial correlation, conditioning on {opt.partialConditionSize} hub gene(s)")
                    End If
                End If
            End If

            If backend IsNot Nothing Then
                Call "run parallel for the correlation screening".debug
                Call backend.Initialize(regulatorPool, Vectors, maxCand, opt.minAbsCorrelation)
                activeScreener = backend
            End If

            Dim count As i32 = 1

            Try
                Call Parallel.For(0, nG, popt, Sub(i As Integer)
                                                   buffer(i) = FitGene(i, maxCand)

                                                   SyncLock count
                                                       Call $"[finished {++count}/{nG}] {buffer(i)}".debug
                                                   End SyncLock
                                               End Sub)
            Finally
                activeScreener = Nothing
                backend?.Dispose()
            End Try

            For i As Integer = 0 To nG - 1
                modelTable(buffer(i).target) = buffer(i)
            Next

            Dim failed As Integer = modelTable.Values.Where(Function(m) Not m.IsValid).Count()

            If failed > 0 AndAlso Not opt.silent Then
                Call Console.WriteLine($"[GRN] {failed} of {nG} gene models are failed to fit")
            End If

            Return Me
        End Function

        ''' <summary>
        ''' 获取指定目标基因的线性拟合模型
        ''' </summary>
        ''' <param name="targetGene">目标基因编号</param>
        ''' <returns>不存在对应模型的时候返回空引用</returns>
        Public Function GetModel(targetGene As String) As GeneRegressionModel
            If modelTable.ContainsKey(targetGene) Then
                Return modelTable(targetGene)
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 枚举网络之中的所有调控关系
        ''' </summary>
        ''' <param name="minAbsWeight">
        ''' 回归系数绝对值的阈值，小于这个阈值的边会被丢弃；
        ''' 小于零的时候使用 <see cref="GrnFittingOptions.minAbsWeight"/> 之内的配置。
        ''' </param>
        ''' <param name="pvalueCutoff">
        ''' 显著性 p 值的阈值，大于这个阈值的边会被丢弃；
        ''' 对于 LASSO 模型，所有的 p 值都是 1，因此必须设置为 1 才能正常导出。
        ''' </param>
        ''' <returns></returns>
        Public Iterator Function Regulations(Optional minAbsWeight As Double = -1,
                                             Optional pvalueCutoff As Double = 1) As IEnumerable(Of RegulationEdge)
            Dim cutoff As Double = If(minAbsWeight < 0, opt.minAbsWeight, minAbsWeight)
            Dim all As New Dictionary(Of String, RegulationEdge)

            For Each model As GeneRegressionModel In modelTable.Values
                If Not model.IsValid Then
                    Continue For
                End If
                If model.regulators Is Nothing Then
                    Continue For
                End If

                For i As Integer = 0 To model.regulators.Length - 1
                    Dim w As Double = model.weights(i)

                    If w = 0.0R OrElse std.Abs(w) < cutoff Then
                        Continue For
                    End If
                    If model.pvalues IsNot Nothing AndAlso model.pvalues(i) > pvalueCutoff Then
                        Continue For
                    End If

                    Dim edge As New RegulationEdge With {
                        .regulator = model.regulators(i),
                        .target = model.target,
                        .weight = w,
                        .intercept = model.intercept,
                        .r2 = model.r2,
                        .alpha = model.alpha,
                        .l1Ratio = model.l1Ratio,
                        .interaction = opt.method.ToString(),
                        .pvalue = If(model.pvalues Is Nothing, 1, model.pvalues(i))
                    }

                    If Not all.ContainsKey(edge.UID) Then
                        Call all.Add(edge.UID, edge)
                    End If
                Next
            Next

            If opt.mutualOnly Then
                For Each edge As RegulationEdge In MutualOnly(all)
                    Yield edge
                Next
            Else
                For Each edge As RegulationEdge In all.Values
                    Yield edge
                Next
            End If
        End Function

        ''' <summary>
        ''' 把推断出来的调控网络导出为系数矩阵：行是目标基因，列是调控因子，
        ''' 矩阵元素即为对应的回归系数(没有边的位置为零)。
        ''' </summary>
        ''' <returns></returns>
        Public Function CoefMatrix() As Matrix
            Dim regulators As String() = geneNames
            Dim index As Index(Of String) = regulators.Indexing
            Dim rows As New List(Of DataFrameRow)

            For Each name As String In geneNames
                Dim model As GeneRegressionModel = GetModel(name)
                Dim vec As Double() = New Double(regulators.Length - 1) {}

                If model IsNot Nothing AndAlso model.IsValid AndAlso model.regulators IsNot Nothing Then
                    For i As Integer = 0 To model.regulators.Length - 1
                        Dim j As Integer = index.IndexOf(model.regulators(i))

                        If j >= 0 Then
                            vec(j) = model.weights(i)
                        End If
                    Next
                End If

                rows.Add(New DataFrameRow With {
                    .geneID = name,
                    .experiments = vec
                })
            Next

            Dim tag As String = $"GRN.{opt.method}"

            If opt.method = FittingMethod.ElasticNet Then
                ' 全局交叉验证模式下真正生效的是解析出来的 l1_ratio
                Dim ratio As Double = If(cvL1Ratio > 0, cvL1Ratio, opt.elasticNet.l1Ratio)

                tag = $"GRN.ElasticNet(l1_ratio={ratio.ToString("G4")}, mode={opt.elasticNet.mode})"
            End If

            Return New Matrix With {
                .tag = tag,
                .sampleID = regulators,
                .expression = rows.ToArray
            }
        End Function

        ''' <summary>
        ''' 把调控网络导出为边表文件(CSV 格式)
        ''' </summary>
        ''' <param name="path">输出文件的路径</param>
        ''' <param name="minAbsWeight">回归系数绝对值的阈值</param>
        ''' <param name="pvalueCutoff">显著性 p 值的阈值</param>
        ''' <returns>是否成功导出</returns>
        Public Function WriteEdges(path As String,
                                   Optional minAbsWeight As Double = -1,
                                   Optional pvalueCutoff As Double = 1) As Boolean

            Dim edges As RegulationEdge() = Regulations(minAbsWeight, pvalueCutoff) _
                .OrderByDescending(Function(e) e.absWeight) _
                .ToArray
            Dim csv As New StringBuilder()

            Call csv.AppendLine("regulator,target,weight,abs_weight,intercept,r2,pvalue,alpha,l1_ratio,interaction")

            For Each edge As RegulationEdge In edges
                Call csv.AppendLine($"{edge.regulator},{edge.target},{edge.weight},{edge.absWeight}," &
                                    $"{edge.intercept},{edge.r2},{edge.pvalue},{edge.alpha},{edge.l1Ratio},""{edge.interaction}""")
            Next

            Dim dir As String = System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(path))

            If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            Call File.WriteAllText(path, csv.ToString())

            Return True
        End Function

        ''' <summary>
        ''' 网络的摘要信息
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Dim n As Integer = modelTable.Values.Where(Function(m) m.IsValid).Count()

            Return $"GRN: {n} gene models, {opt.ToString}"
        End Function

        ''' <summary>
        ''' 输入数据的合法性检查
        ''' </summary>
        Private Sub ValidateInput()
            If geneNames Is Nothing OrElse geneNames.Length < 2 Then
                Throw New ArgumentException("the regulatory network inference requires at least 2 genes!", NameOf(raw))
            End If
            If sampleNames Is Nothing OrElse sampleNames.Length < 4 Then
                Throw New ArgumentException(
                    $"the linear model requires enough samples, but only {If(sampleNames Is Nothing, 0, sampleNames.Length)} sample was given (at least 4)!",
                    NameOf(raw))
            End If

            Dim duplicated As String() = geneNames _
                .GroupBy(Function(id) id) _
                .Where(Function(g) g.Count > 1) _
                .Select(Function(g) g.Key) _
                .ToArray

            If duplicated.Length > 0 Then
                Throw new_duplicated(duplicated)
            End If

            For i As Integer = 0 To raw.expression.Length - 1
                If raw.expression(i).experiments Is Nothing OrElse raw.expression(i).experiments.Length <> sampleNames.Length Then
                    Throw New ArgumentException(
                        $"the expression vector size of gene '{geneNames(i)}' is mis-matched with the sample size {sampleNames.Length}!",
                        NameOf(raw))
                End If
            Next
        End Sub

        Private Shared Function new_duplicated(duplicated As String()) As ArgumentException
            Dim show As String = If(duplicated.Length > 5,
                String.Join(", ", duplicated.Take(5)) & $", ... ({duplicated.Length} duplicated)",
                String.Join(", ", duplicated))

            Return New ArgumentException($"duplicated gene id was found in the expression matrix: {show}!", "raw")
        End Function

        ''' <summary>
        ''' 计算允许的候选调控因子数量的上限
        ''' </summary>
        ''' <returns></returns>
        Private Function CandidateBudget() As Integer
            Dim nS As Integer = sampleNames.Length
            Dim poolSize As Integer = regulatorPool.Length
            Dim k As Integer

            If opt.method = FittingMethod.ElasticNet Then
                ' 弹性网络没有「自变量个数必须小于样本数量」的限制，候选数量只受
                ' 计算预算约束(每一次求解都会在内部复制一份 n x p 的设计矩阵)
                Dim budget As Integer = If(opt.elasticNet.maxCandidates > 0, opt.elasticNet.maxCandidates, poolSize - 1)

                If opt.maxCandidates > 0 Then
                    k = std.Min(opt.maxCandidates, budget)
                Else
                    k = budget
                End If

                k = std.Min(k, poolSize - 1)
            Else
                Dim hard As Integer = nS - 2

                If opt.maxCandidates > 0 Then
                    k = opt.maxCandidates
                Else
                    k = CInt(std.Floor(nS * opt.candidateSampleRatio))
                End If

                ' 目标基因自身不会被作为候选调控因子，因此候选池的上限需要先扣除掉自身
                k = std.Min(k, poolSize - 1)
                k = std.Min(k, hard)
            End If

            If k < 1 Then
                Throw New ArgumentException(
                    $"the linear regression model requires more samples: there are {nS} samples only, " &
                    $"but the candidate budget is zero after truncate, please increase the sample size " &
                    $"or reduce the gene size of the expression matrix!",
                    NameOf(opt))
            End If

            If poolSize - 1 > k AndAlso Not opt.silent Then
                Call Console.WriteLine($"[GRN] the candidate regulator size is truncated " &
                                       $"from {poolSize - 1} to {k}")
            End If

            Return k
        End Function

        ''' <summary>
        ''' 拟合单个目标基因的线性模型
        ''' </summary>
        ''' <param name="target">目标基因的行下标</param>
        ''' <param name="maxCand">允许的候选调控因子数量的上限</param>
        ''' <returns>求解失败的时候，模型对象的 <see cref="GeneRegressionModel.errorMessage"/> 会被设置为对应的异常信息</returns>
        Private Function FitGene(target As Integer, maxCand As Integer) As GeneRegressionModel
            Dim name As String = geneNames(target)
            Dim y As Double() = vectors(target)

            Try
                Dim candidates As Integer() = regulatorPool _
                    .Where(Function(j) j <> target) _
                    .ToArray

                If candidates.Length > maxCand OrElse opt.minAbsCorrelation > 0 Then
                    If activeScreener Is Nothing Then
                        candidates = TopCorrelated(target, candidates, maxCand)
                    Else
                        ' 后端已经在 Fit 开始时算好，这里只是取预计算的结果
                        Dim screened As Integer() = activeScreener.Query(target)

                        If screened.Length = 0 Then
                            ' 目标基因本身不在候选池里(例如 regulators 限定了池子)，
                            ' 后端没有它的行，这里退化为逐基因扫描
                            candidates = TopCorrelated(target, candidates, maxCand)
                        Else
                            candidates = screened
                        End If
                    End If
                End If

                candidates = InjectPriorityRegulators(target, candidates)

                Select Case opt.method
                    Case FittingMethod.Lasso
                        Return FitLasso(name, y, candidates)
                    Case FittingMethod.ElasticNet
                        Return FitElasticNet(name, y, candidates)
                    Case Else
                        Return FitOls(name, y, candidates)
                End Select
            Catch ex As Exception
                Return New GeneRegressionModel With {
                    .target = name,
                    .method = opt.method,
                    .N = sampleNames.Length,
                    .regulators = {},
                    .weights = {},
                    .r2 = 0,
                    .errorMessage = ex.Message
                }
            End Try
        End Function

        ''' <summary>
        ''' 把 <see cref="priorityPool"/> 之中的基因追加进候选集。
        ''' 
        ''' 这些基因(例如转录因子)不受 <see cref="GrnFittingOptions.maxCandidates"/>
        ''' 与 <see cref="GrnFittingOptions.minAbsCorrelation"/> 截断的影响，
        ''' 从而不会因为相关性排名靠后被挤出模型；目标基因自身始终被排除。
        ''' </summary>
        ''' <param name="target">目标基因的行下标</param>
        ''' <param name="candidates">已经按照相关性挑选过的候选集</param>
        ''' <returns>合并之后的候选集(保持原有顺序，新基因追加在末尾)</returns>
        Private Function InjectPriorityRegulators(target As Integer, candidates As Integer()) As Integer()
            If priorityPool.Length = 0 Then
                Return candidates
            End If

            Dim present As New HashSet(Of Integer)(candidates)
            Dim merged As New List(Of Integer)
            Dim added As Integer = 0

            Call merged.AddRange(candidates)

            For Each idx As Integer In priorityPool
                If idx = target Then
                    Continue For
                End If
                If present.Add(idx) Then
                    Call merged.Add(idx)
                    added += 1
                End If
            Next

            If added > 0 AndAlso Not opt.silent Then
                Call Console.WriteLine($"[GRN] {added} priority regulator(s) injected into the candidate pool of '{geneNames(target)}'")
            End If

            Return merged.ToArray
        End Function

        ''' <summary>
        ''' 按与目标基因的 pearson 相关性绝对值降序挑选候选调控因子
        ''' </summary>
        Private Function TopCorrelated(target As Integer, pool As Integer(), maxCand As Integer) As Integer()
            Dim unit As Double() = unitVectors(target)

            If unit Is Nothing Then
                Return New Integer() {}
            End If

            Dim scores As Double() = New Double(pool.Length - 1) {}
            Dim hits As New List(Of Integer)

            For i As Integer = 0 To pool.Length - 1
                Dim v As Double() = unitVectors(pool(i))

                If v Is Nothing Then
                    scores(i) = 0
                Else
                    scores(i) = std.Abs(DotProduct(unit, v))
                End If

                If scores(i) >= opt.minAbsCorrelation Then
                    hits.Add(i)
                End If
            Next

            If hits.Count = 0 Then
                Return New Integer() {}
            End If

            Dim order As Integer() = hits _
                .OrderByDescending(Function(i) scores(i)) _
                .ToArray

            If order.Length <= maxCand Then
                Return order.Select(Function(i) pool(i)).ToArray
            Else
                Return order.Take(maxCand).Select(Function(i) pool(i)).ToArray
            End If
        End Function

        ''' <summary>
        ''' 构造设计矩阵
        ''' 
        ''' 普通最小二乘模型没有内建的截距项，设计矩阵的第一列必须是常数项 1；
        ''' LASSO 求解器自身会给出截距，因此不需要添加常数项列。
        ''' </summary>
        ''' <param name="candidates">候选调控因子在表达矩阵之中的行下标</param>
        ''' <param name="withIntercept">是否需要添加常数项列</param>
        ''' <param name="featureNames">返回设计矩阵的列名称</param>
        ''' <param name="columns">返回按列组织的表达矩阵，用于 LASSO 求解器的输入</param>
        ''' <returns>行是样本、列是自变量的矩形设计矩阵</returns>
        Private Function BuildDesign(candidates As Integer(), withIntercept As Boolean,
                                     ByRef featureNames As String(), ByRef columns As Double()()) As Double(,)
            Dim nS As Integer = sampleNames.Length
            Dim offset As Integer = If(withIntercept, 1, 0)
            Dim width As Integer = candidates.Length + offset
            Dim X As Double(,) = New Double(nS - 1, width - 1) {}
            Dim names As String() = New String(width - 1) {}
            Dim cols As Double()() = New Double(width - 1)() {}

            If withIntercept Then
                names(0) = "(Intercept)"
            End If

            For i As Integer = 0 To candidates.Length - 1
                names(i + offset) = geneNames(candidates(i))
            Next

            For j As Integer = 0 To width - 1
                cols(j) = New Double(nS - 1) {}
            Next

            For s As Integer = 0 To nS - 1
                If withIntercept Then
                    X(s, 0) = 1.0R
                    cols(0)(s) = 1.0R
                End If

                For i As Integer = 0 To candidates.Length - 1
                    Dim v As Double = vectors(candidates(i))(s)

                    X(s, i + offset) = v
                    cols(i + offset)(s) = v
                Next
            Next

            featureNames = names
            columns = cols

            Return X
        End Function

        ''' <summary>
        ''' 使用普通最小二乘(OLS)拟合单个目标基因
        ''' </summary>
        Private Function FitOls(name As String, y As Double(), candidates As Integer()) As GeneRegressionModel
            Dim featureNames As String() = Nothing
            Dim columns As Double()() = Nothing
            Dim X As Double(,) = BuildDesign(candidates, True, featureNames, columns)
            Dim fit As MLRFit = LinearFittingAlgorithm.LinearFitting(X, y)
            Dim beta As Double() = fit.beta
            Dim model As New GeneRegressionModel With {
                .target = name,
                .method = FittingMethod.MultipleLinear,
                .N = sampleNames.Length,
                .intercept = beta(0),
                .regulators = featureNames _
                    .Skip(1) _
                    .ToArray,
                .weights = beta _
                    .Skip(1) _
                    .ToArray,
                .r2 = fit.R2,
                .sse = fit.SSE,
                .sst = fit.SST,
                .df = sampleNames.Length - beta.Length
            }

            If Not opt.silent Then
                Call Console.WriteLine($"  [OLS] {model.ToString}")
            End If

            If opt.computeStatistics Then
                Dim test = RegressionStatistics.TTest(X, beta, fit.SSE, opt.pvalueTolerance)

                If test.tstat IsNot Nothing Then
                    model.interceptT = test.tstat(0)
                    model.interceptPvalue = test.pvalue(0)
                    model.tvalues = test.tstat.Skip(1).ToArray
                    model.pvalues = test.pvalue.Skip(1).ToArray
                End If
            End If

            Return model
        End Function

        ''' <summary>
        ''' 使用 LASSO(L1 正则化)拟合单个目标基因
        ''' </summary>
        Private Function FitLasso(name As String, y As Double(), candidates As Integer()) As GeneRegressionModel
            If candidates.Length = 0 Then
                Return Failed(name, "there is no candidate regulator for the lasso fit")
            End If

            Dim featureNames As String() = Nothing
            Dim columns As Double()() = Nothing
            Dim nS As Integer = sampleNames.Length

            Call BuildDesign(candidates, False, featureNames, columns)

            Dim generator As New LassoFitGenerator()

            Call generator.init(featureNames, nS)

            For j As Integer = 0 To featureNames.Length - 1
                Call generator.setFeatureValues(j, columns(j))
            Next
            For i As Integer = 0 To nS - 1
                Call generator.setTarget(i, y(i))
            Next

            ' 求解器的这个参数只是控制正则化路径在多少个活跃变量之后提前结束，
            ' 与最终保留下来的调控因子数量(由 PickLambdaStep 严格限制)并不是一回事：
            ' 这里取两倍的余量，保证所有非零权重数量不超过 maxRegulatorsPerGene 的
            ' lambda 步长都还在路径之上，从而让最终的挑选结果与跑完整路径时完全一致，
            ' 但又可以在特征数量很大、允许的调控因子很少的时候省下大量计算。
            ' 
            ' 注意：DataFittings 之中的 LassoFitGenerator 曾经存在两个会导致崩溃的缺陷，
            ' 它们已经在求解器内部修复：
            ' 
            ' 1. init 过程按照观测数量分配特征数组的外层维度，自变量个数大于样本数量的时候会越界；
            ' 2. maxAllowedFeaturesPerModel 受限的时候，denseActiveSet 与协方差行向量的长度不一致。
            ' 
            ' 另外，候选调控因子的数量必须小于样本数量，这一点由 maxCandidates 参数保证。
            Dim capacity As Integer = If(opt.maxRegulatorsPerGene > 0, opt.maxRegulatorsPerGene * 2, -1)
            Dim path As LassoFit = generator.fit(capacity)
            Dim stepId As Integer = PickLambdaStep(path, opt.maxRegulatorsPerGene)
            Dim model As New GeneRegressionModel With {
                .target = name,
                .method = FittingMethod.Lasso,
                .N = nS,
                .intercept = path.intercepts(stepId),
                .regulators = featureNames,
                .weights = path.getWeights(stepId),
                .r2 = path.rsquared(stepId),
                .lambda = path.lambdas(stepId),
                .df = -1
            }

            model.sst = RegressionStatistics.TotalSumOfSquares(y)
            model.sse = model.sst * (1 - model.r2)
            model.pvalues = New Double(model.weights.Length - 1) {}

            For i As Integer = 0 To model.pvalues.Length - 1
                model.pvalues(i) = 1
            Next

            If Not opt.silent Then
                Call Console.WriteLine($"  [LASSO] {model.ToString}")
            End If

            Return model
        End Function

        ''' <summary>
        ''' 使用弹性网络(Elastic Net)拟合单个目标基因
        ''' </summary>
        ''' <param name="name">目标基因的编号</param>
        ''' <param name="y">目标基因的表达向量</param>
        ''' <param name="candidates">候选调控因子在表达矩阵之中的行下标</param>
        ''' <returns></returns>
        Private Function FitElasticNet(name As String, y As Double(), candidates As Integer()) As GeneRegressionModel
            If candidates.Length = 0 Then
                Return Failed(name, "there is no candidate regulator for the elastic net fit")
            End If

            Dim featureNames As String() = Nothing
            Dim columns As Double()() = Nothing
            ' 弹性网络的截距不会被惩罚，因此设计矩阵不需要常数项列
            Call BuildDesign(candidates, withIntercept:=False, featureNames:=featureNames, columns:=columns)

            Dim result As ElasticNetFitResult

            Select Case opt.elasticNet.mode
                Case ElasticNetMode.PerGeneCrossValidation, ElasticNetMode.GlobalCrossValidation
                    ' 交叉验证需要按样本折划分，仍然走行主序入口
                    Dim X As Double()() = ToRows(columns, sampleNames.Length)

                    If opt.elasticNet.mode = ElasticNetMode.PerGeneCrossValidation Then
                        result = ElasticNetRegressor.FitByCrossValidation(
                            X, y, featureNames, opt.elasticNet, opt.maxRegulatorsPerGene)
                    Else
                        result = ElasticNetRegressor.FitFixed(
                            X, y, featureNames, opt.elasticNet, cvAlpha, cvL1Ratio, opt.maxRegulatorsPerGene)
                    End If
                Case Else
                    ' 默认路径：直接把列向量交给求解器，省掉一次 n x p 的转置拷贝
                    result = ElasticNetRegressor.FitByPathColumns(
                        columns, y, featureNames, opt.elasticNet, opt.maxRegulatorsPerGene)
            End Select

            If result Is Nothing Then
                Return Failed(name, "the elastic net solver returns no solution")
            End If

            Dim model As New GeneRegressionModel With {
                .target = name,
                .method = FittingMethod.ElasticNet,
                .N = sampleNames.Length,
                .intercept = result.Intercept,
                .regulators = featureNames,
                .weights = result.Weights,
                .r2 = result.R2,
                .sse = result.SSE,
                .sst = result.SST,
                .alpha = result.Alpha,
                .l1Ratio = result.L1Ratio,
                .df = -1
            }

            ' 弹性网络不提供显著性检验(与 LASSO 一致)
            model.pvalues = Enumerable.Repeat(1.0R, featureNames.Length).ToArray

            If Not opt.silent Then
                Call Console.WriteLine($"  [EN] {model.ToString}")
            End If

            Return model
        End Function

        ''' <summary>
        ''' 解析全局交叉验证模式之下的共享超参数
        ''' 
        ''' 按照表达方差从大到小挑选若干代表性基因(与其它基因相比，
        ''' 表达变化剧烈的基因更容易给出稳定的模型)，对它们逐一做交叉验证，
        ''' 然后取超参数的中位数。单个基因失败不会影响整体。
        ''' </summary>
        ''' <param name="maxCand">允许的候选调控因子数量的上限</param>
        ''' <returns>共享的 (alpha, l1_ratio)；解析失败的时候 alpha 为 -1</returns>
        Private Function ResolveElasticNetHyperparameters(maxCand As Integer) As (alpha As Double, l1Ratio As Double)
            Dim replicates As Integer() = Enumerable _
                .Range(0, geneNames.Length) _
                .OrderByDescending(Function(i) Variance(vectors(i))) _
                .Take(opt.elasticNet.globalCVReplicates) _
                .ToArray
            Dim results As New List(Of ElasticNetCVResult)

            For Each target As Integer In replicates
                Try
                    Dim candidates As Integer() = regulatorPool _
                        .Where(Function(j) j <> target) _
                        .ToArray

                    If candidates.Length > maxCand OrElse opt.minAbsCorrelation > 0 Then
                        candidates = TopCorrelated(target, candidates, maxCand)
                    End If
                    If candidates.Length = 0 Then
                        Continue For
                    End If

                    Dim featureNames As String() = Nothing
                    Dim columns As Double()() = Nothing

                    Call BuildDesign(candidates, withIntercept:=False, featureNames:=featureNames, columns:=columns)

                    Dim X As Double()() = ToRows(columns, sampleNames.Length)
                    Dim cv As ElasticNetCVResult = ElasticNetRegressor.CrossValidateOnce(X, vectors(target), featureNames, opt.elasticNet)

                    If cv IsNot Nothing AndAlso cv.Fit IsNot Nothing Then
                        Call results.Add(cv)
                    End If
                Catch ex As Exception
                    ' 单个基因的交叉验证失败不影响整体的超参数估计
                    If Not opt.silent Then
                        Call Console.WriteLine($"[GRN] the elastic net cv of gene '{geneNames(target)}' is failed: {ex.Message}")
                    End If
                End Try
            Next

            Return ElasticNetRegressor.MedianHyperparameters(results)
        End Function

        ''' <summary>
        ''' 把按列组织的设计矩阵转换为按行组织的设计矩阵
        ''' 
        ''' 弹性网络的求解器使用的是 ``x(样本)(特征)`` 的锯齿数组，而
        ''' <see cref="BuildDesign"/> 返回的是按列组织的矩阵。
        ''' </summary>
        ''' <param name="columns">按列组织的设计矩阵</param>
        ''' <param name="nSamples">样本数量</param>
        ''' <returns></returns>
        Private Shared Function ToRows(columns As Double()(), nSamples As Integer) As Double()()
            Dim p As Integer = columns.Length
            Dim rows As Double()() = New Double(nSamples - 1)() {}

            For s As Integer = 0 To nSamples - 1
                Dim row As Double() = New Double(p - 1) {}

                For j As Integer = 0 To p - 1
                    row(j) = columns(j)(s)
                Next

                rows(s) = row
            Next

            Return rows
        End Function

        ''' <summary>
        ''' 计算表达向量的样本标准差(ddof = n - 1)。
        ''' 
        ''' 这里刻意使用与 <see cref="Standardize"/> 相同的自由度约定：
        ''' z-score 标准化之后的标准差恰好为 1，因此 <see cref="Baseline"/> 与
        ''' <see cref="BaselineSD"/> 可以互相校验。
        ''' </summary>
        ''' <param name="v">表达向量</param>
        ''' <returns>样本数量不足 2 的时候返回零</returns>
        Private Shared Function SampleStandardDeviation(v As Double()) As Double
            If v Is Nothing OrElse v.Length < 2 Then
                Return 0
            End If

            Dim mean As Double = v.Average
            Dim sum As Double = 0

            For i As Integer = 0 To v.Length - 1
                sum += (v(i) - mean) ^ 2
            Next

            Return std.Sqrt(sum / (v.Length - 1))
        End Function

        ''' <summary>
        ''' 计算表达向量的方差，用于挑选全局交叉验证的代表性基因
        ''' </summary>
        ''' <param name="v">表达向量</param>
        ''' <returns></returns>
        Private Shared Function Variance(v As Double()) As Double
            If v Is Nothing OrElse v.Length = 0 Then
                Return 0
            End If

            Dim mean As Double = v.Average
            Dim sum As Double = 0

            For i As Integer = 0 To v.Length - 1
                sum += (v(i) - mean) ^ 2
            Next

            Return sum / v.Length
        End Function

        Private Function Failed(name As String, message As String) As GeneRegressionModel
            Return New GeneRegressionModel With {
                .target = name,
                .method = opt.method,
                .N = sampleNames.Length,
                .regulators = {},
                .weights = {},
                .r2 = 0,
                .errorMessage = message
            }
        End Function

        ''' <summary>
        ''' 在 LASSO 的正则化路径上面挑选一个合适的 lambda 步长
        ''' 
        ''' 挑选规则：在非零权重数量不超过 <paramref name="maxFeatures"/> 的步骤之中，
        ''' 取解释度 R^2 最高的那一步。
        ''' </summary>
        Private Shared Function PickLambdaStep(path As LassoFit, maxFeatures As Integer) As Integer
            Dim bestId As Integer = -1
            Dim bestR2 As Double = Double.NegativeInfinity

            For k As Integer = 0 To path.numberOfLambdas - 1
                If path.nonZeroWeights(k) = 0 Then
                    Continue For
                End If
                If maxFeatures > 0 AndAlso path.nonZeroWeights(k) > maxFeatures Then
                    Continue For
                End If
                If Not IsValid(path.rsquared(k)) Then
                    Continue For
                End If
                If path.rsquared(k) > bestR2 Then
                    bestR2 = path.rsquared(k)
                    bestId = k
                End If
            Next

            If bestId < 0 Then
                bestId = path.numberOfLambdas - 1
            End If

            Return bestId
        End Function

        Private Shared Function IsValid(x As Double) As Boolean
            Return Not Double.IsNaN(x) AndAlso Not Double.IsInfinity(x)
        End Function

        ''' <summary>
        ''' 只保留互为因果的调控关系
        ''' </summary>
        Private Shared Iterator Function MutualOnly(all As Dictionary(Of String, RegulationEdge)) As IEnumerable(Of RegulationEdge)
            Dim visited As New HashSet(Of String)

            For Each edge As RegulationEdge In all.Values
                Dim key As String

                If String.Compare(edge.regulator, edge.target) < 0 Then
                    key = $"{edge.regulator}|{edge.target}"
                Else
                    key = $"{edge.target}|{edge.regulator}"
                End If

                If Not visited.Add(key) Then
                    Continue For
                End If

                Dim reversed As RegulationEdge = Nothing

                If Not all.TryGetValue($"{edge.target}->{edge.regulator}", reversed) Then
                    Continue For
                End If
                If std.Sign(edge.weight) <> std.Sign(reversed.weight) Then
                    Continue For
                End If

                Yield New RegulationEdge With {
                    .regulator = edge.regulator,
                    .target = edge.target,
                    .weight = (edge.weight + reversed.weight) / 2,
                    .intercept = edge.intercept,
                    .r2 = std.Max(edge.r2, reversed.r2),
                    .alpha = edge.alpha,
                    .l1Ratio = edge.l1Ratio,
                    .pvalue = std.Max(edge.pvalue, reversed.pvalue),
                    .interaction = "mutual"
                }
            Next
        End Function

        ''' <summary>
        ''' 把向量居中并且做 L2 归一化，从而可以把 pearson 相关性计算化简为两个向量的内积
        ''' </summary>
        ''' <param name="v"></param>
        ''' <returns>常量向量(表达没有波动)返回空引用</returns>
        Private Shared Function UnitVector(v As Double()) As Double()
            Dim mean As Double = v.Average
            Dim z As Double() = New Double(v.Length - 1) {}
            Dim norm As Double = 0

            For i As Integer = 0 To v.Length - 1
                z(i) = v(i) - mean
                norm += z(i) * z(i)
            Next

            If norm <= 0 Then
                Return Nothing
            End If

            norm = std.Sqrt(norm)

            For i As Integer = 0 To z.Length - 1
                z(i) /= norm
            Next

            Return z
        End Function

        ''' <summary>
        ''' z-score 标准化
        ''' </summary>
        Private Shared Function Standardize(v As Double()) As Double()
            Dim mean As Double = v.Average
            Dim sd As Double = 0

            For i As Integer = 0 To v.Length - 1
                sd += (v(i) - mean) ^ 2
            Next

            sd = std.Sqrt(sd / (v.Length - 1))

            If sd <= 0 Then
                Return v.Select(Function(x) 0.0R).ToArray
            End If

            Return v.Select(Function(x) (x - mean) / sd).ToArray
        End Function

        ''' <summary>
        ''' 两个等长向量的内积
        ''' </summary>
        Private Shared Function DotProduct(a As Double(), b As Double()) As Double
            Dim sum As Double = 0

            For i As Integer = 0 To a.Length - 1
                sum += a(i) * b(i)
            Next

            Return sum
        End Function
    End Class
End Namespace
