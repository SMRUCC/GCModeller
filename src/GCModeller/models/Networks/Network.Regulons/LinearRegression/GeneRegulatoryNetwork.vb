Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data
Imports Microsoft.VisualBasic.Data.Bootstrapping.LASSO
Imports Microsoft.VisualBasic.Data.Bootstrapping.Multivariate
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
        ''' 每一个基因的表达向量，下标与 <see cref="geneNames"/> 一致。
        ''' 开启了 <see cref="GrnFittingOptions.standardize"/> 之后这里保存的是 z-score 标准化之后的数据。
        ''' </summary>
        ReadOnly vectors As Double()()

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
        ''' 拟合好的模型，键名是目标基因的编号
        ''' </summary>
        ReadOnly modelTable As New Dictionary(Of String, GeneRegressionModel)

        ''' <summary>
        ''' 从表达矩阵构造一个待拟合的调控网络推断模型
        ''' </summary>
        ''' <param name="expr">行基因为、列样本的表达矩阵</param>
        ''' <param name="options">线性拟合的参数配置，为空的时候使用默认参数</param>
        Public Sub New(expr As Matrix, Optional options As GrnFittingOptions = Nothing)
            If expr Is Nothing Then
                Throw New ArgumentNullException(NameOf(expr))
            End If

            Me.raw = expr
            Me.options = If(options, New GrnFittingOptions())
            Me.geneNames = expr.rownames
            Me.sampleNames = expr.sampleID

            Call ValidateInput()

            Me.vectors = expr _
                .expression _
                .Select(Function(r) DirectCast(r.experiments.Clone(), Double())) _
                .ToArray

            If Me.opt.standardize Then
                For i As Integer = 0 To vectors.Length - 1
                    vectors(i) = Standardize(vectors(i))
                Next
            End If

            Me.unitVectors = vectors _
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
                        NameOf(options)
                    )
                End If

                Me.regulatorPool = pool.ToArray
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
                Return options
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

            If Not opt.silent Then
                Call Console.WriteLine($"[GRN] {opt.ToString}")
                Call Console.WriteLine($"[GRN] {geneNames.Length} genes x {sampleNames.Length} samples, " &
                                       $"<= {maxCand} candidate regulators for each target gene")
            End If

            modelTable.Clear()

            Call Parallel.For(0, nG, popt, Sub(i As Integer)
                                               buffer(i) = FitGene(i, maxCand)
                                           End Sub)

            For i As Integer = 0 To nG - 1
                models(buffer(i).target) = buffer(i)
            Next

            Dim failed As Integer = modelTable.Values.Count(Function(m) Not m.IsValid)

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
                Return models(targetGene)
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
        ''' 对于 LASSO 与 NNLS 模型，所有的 p 值都是 1，因此必须设置为 1 才能正常导出。
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

            Return New Matrix With {
                .tag = $"GRN.{opt.method}",
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

            Call csv.AppendLine("regulator,target,weight,abs_weight,intercept,r2,pvalue,interaction")

            For Each edge As RegulationEdge In edges
                Call csv.AppendLine($"{edge.regulator},{edge.target},{edge.weight},{edge.absWeight}," &
                                    $"{edge.intercept},{edge.r2},{edge.pvalue},""{edge.interaction}""")
            Next

            Dim dir As String = Path.GetDirectoryName(Path.GetFullPath(path))

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
            Dim n As Integer = modelTable.Values.Count(Function(m) m.IsValid)

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
            Dim hard As Integer = nS - 2
            Dim k As Integer

            If opt.maxCandidates > 0 Then
                k = opt.maxCandidates
            Else
                k = CInt(std.Floor(nS * opt.candidateSampleRatio))
            End If

            ' 目标基因自身不会被作为候选调控因子，因此候选池的上限需要先扣除掉自身
            k = std.Min(k, poolSize - 1)
            k = std.Min(k, hard)

            If k < 1 Then
                Throw New ArgumentException(
                    $"the linear regression model requires more samples: there are {nS} samples only, " &
                    $"but the candidate budget is zero after truncate, please increase the sample size " &
                    $"or reduce the gene size of the expression matrix!",
                    NameOf(options))
            End If

            If poolSize - 1 > k AndAlso Not opt.silent Then
                Call Console.WriteLine($"[GRN] the candidate regulator size is truncated " &
                                       $"from {poolSize - 1} to {k} for the sample size {nS}")
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
                    candidates = TopCorrelated(target, candidates, maxCand)
                End If

                Select Case opt.method
                    Case FittingMethod.MultipleLinear
                        Return FitOls(name, y, candidates)
                    Case FittingMethod.Lasso
                        Return FitLasso(name, y, candidates)
                    Case Else
                        Return FitNnls(name, y, candidates)
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
        ''' 构造设计矩阵，第一列是常数项，其余的列是各个候选调控因子的表达向量
        ''' </summary>
        Private Function DesignMatrix(candidates As Integer(), y As Double(), ByRef featureNames As String()) As Double()()
            Dim nS As Integer = sampleNames.Length
            Dim withIntercept As Boolean = opt.method = FittingMethod.MultipleLinear
            Dim offset As Integer = If(withIntercept, 1, 0)
            Dim width As Integer = candidates.Length + offset
            Dim X As Double()() = New Double(nS - 1)() {}
            Dim names As String() = New String(width - 1) {}

            If withIntercept Then
                names(0) = "(Intercept)"
            End If

            For i As Integer = 0 To candidates.Length - 1
                names(i + offset) = geneNames(candidates(i))
            Next

            For s As Integer = 0 To nS - 1
                Dim row As Double() = New Double(width - 1) {}

                If withIntercept Then
                    row(0) = 1.0R
                End If

                For i As Integer = 0 To candidates.Length - 1
                    row(i + offset) = vectors(candidates(i))(s)
                Next

                X(s) = row
            Next

            featureNames = names

            Return X
        End Function

        ''' <summary>
        ''' 使用普通最小二乘(OLS)拟合单个目标基因
        ''' </summary>
        Private Function FitOls(name As String, y As Double(), candidates As Integer()) As GeneRegressionModel
            Dim featureNames As String() = Nothing
            Dim X As Double()() = DesignMatrix(candidates, y, featureNames)
            Dim table As New NumericTable(X, sampleNames, featureNames)

            Call table.SetLabel("target", y)

            Dim fit As MLRFit = table.MultipleLinearRegression()
            Dim beta As Double() = fit.beta
            Dim offset As Integer = 1
            Dim model As New GeneRegressionModel With {
                .target = name,
                .method = FittingMethod.MultipleLinear,
                .N = sampleNames.Length,
                .intercept = beta(0),
                .regulators = featureNames _
                    .Skip(offset) _
                    .ToArray,
                .weights = beta _
                    .Skip(offset) _
                    .ToArray,
                .r2 = fit.R2,
                .sse = fit.SSE,
                .sst = fit.SST,
                .df = sampleNames.Length - X(0).Length
            }

            If Not opt.silent Then
                Call Console.WriteLine($"  [OLS] {model.ToString}")
            End If

            If opt.computeStatistics Then
                Dim test = RegressionStatistics.TTest(X, beta, fit.SSE, opt.pvalueTolerance)

                If test.tstat IsNot Nothing Then
                    model.interceptT = test.tstat(0)
                    model.interceptPvalue = test.pvalue(0)
                    model.tvalues = test.tstat.Skip(offset).ToArray
                    model.pvalues = test.pvalue.Skip(offset).ToArray
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
            Dim X As Double()() = DesignMatrix(candidates, y, featureNames)
            Dim table As New NumericTable(X, sampleNames, featureNames)

            Call table.SetLabel("target", y)

            ' LASSO 求解器的内部实现要求自变量个数不能大于样本数量，
            ' 这里已经通过 maxCandidates 参数保证了这一点
            Dim path As LassoFit = table.LassoRegression(
                maxAllowedFeaturesPerModel:=opt.maxRegulatorsPerGene,
                Silent:=True
            )
            Dim stepId As Integer = PickLambdaStep(path, opt.maxRegulatorsPerGene)
            Dim model As New GeneRegressionModel With {
                .target = name,
                .method = FittingMethod.Lasso,
                .N = sampleNames.Length,
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
        ''' 使用非负最小二乘(NNLS)拟合单个目标基因
        ''' </summary>
        Private Function FitNnls(name As String, y As Double(), candidates As Integer()) As GeneRegressionModel
            If candidates.Length = 0 Then
                Return Failed(name, "there is no candidate regulator for the nnls fit")
            End If

            Dim featureNames As String() = Nothing
            Dim X As Double()() = DesignMatrix(candidates, y, featureNames)
            Dim table As New NumericTable(X, sampleNames, featureNames)

            Call table.SetLabel("target", y)

            Dim beta As Double() = table.Nnls()
            Dim model As New GeneRegressionModel With {
                .target = name,
                .method = FittingMethod.NonNegative,
                .N = sampleNames.Length,
                .intercept = 0,
                .regulators = featureNames,
                .weights = beta,
                .df = -1
            }

            model.sst = RegressionStatistics.TotalSumOfSquares(y)
            model.sse = RegressionStatistics.SumOfSquaredErrors(X, y, beta)
            model.r2 = RegressionStatistics.Rsquared(model.sse, model.sst)

            If Not opt.silent Then
                Call Console.WriteLine($"  [NNLS] {model.ToString}")
            End If

            Return model
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
