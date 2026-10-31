Imports System.IO
Imports System.Threading
Imports Microsoft.VisualBasic.ComponentModel.TagData
Imports Microsoft.VisualBasic.DataMining.Evaluation
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.XGBoost.DataSet
Imports Microsoft.VisualBasic.MachineLearning.XGBoost.train
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

''' <summary>
''' 逐基因的非线性拟合器：把 <c>aracne.zip</c> 给出的调控拓扑与原始表达矩阵结合起来，
''' 为每一个靶基因训练一个梯度提升树模型 <c>f_i(X_Reg(i))</c>。
''' 
''' 训练流程（与 readme 的“路径一”一一对应）：
''' 
''' ```
''' 1. 细胞抽样       : 按 MaxSamplesPerModel 抽取细胞列，再按 ValidationRatio 做固定折分
''' 2. 特征构建       : 每个靶基因取 top-K 调控者的表达值作为特征，靶基因表达值作为标签
''' 3. 归一/缺失处理  : Double.NaN 映射为 TGBoost 的 NA 标记；标签缺失用该基因的有限均值填补
''' 4. 梯度提升树拟合 : squareloss + mse 早停 + 强正则（浅树，利于定点迭代收敛）
''' 5. 评价           : 留出集 R² / RMSE、均值基线对比、逐调控因子分裂重要性与灵敏度
''' ```
''' </summary>
Public Class GeneTreeTrainer

    ''' <summary>本次训练使用的配置</summary>
    Public ReadOnly Property Config As GeneTreeConfig

    ''' <summary>调控网络拓扑</summary>
    Public ReadOnly Property Graph As GeneRegulationGraph

    ''' <summary>训练所用的表达矩阵（行=基因，列=样本）</summary>
    Public ReadOnly Property Expression As Matrix

    ''' <summary>参与训练的细胞列下标（升序）</summary>
    Public ReadOnly Property TrainingCells As Integer()

    ''' <summary>
    ''' 每个基因的模型（下标与 <see cref="Expression"/> 的行一一对应）。
    ''' <c>Nothing</c> 表示该基因没有调控者（源节点），在迭代之中保持初始值不变。
    ''' </summary>
    Public ReadOnly Property Models As GeneModel()

    ''' <summary>训练报告</summary>
    Public ReadOnly Property Report As TrainingReport

    ''' <summary>本次训练被选中参与拟合的靶基因数量</summary>
    Public ReadOnly Property TargetCount As Integer

    ''' <summary>逐基因诊断信息最多输出的行数</summary>
    Private Const MAX_LOG_LINES As Integer = 20

    Private Shared m_logged As Integer = 0

    Private Sub New(config As GeneTreeConfig,
                    graph As GeneRegulationGraph,
                    expr As Matrix,
                    cells As Integer(),
                    models As GeneModel(),
                    report As TrainingReport)

        Me.Config = config
        Me.Graph = graph
        Me.Expression = expr
        Me.TrainingCells = cells
        Me.Models = models
        Me.Report = report
        Me.TargetCount = models.Count(Function(m) m IsNot Nothing)
    End Sub

    ''' <summary>
    ''' 执行逐基因拟合
    ''' </summary>
    ''' <param name="graph">ARACNE 调控拓扑（已经完成 top-K 裁剪）</param>
    ''' <param name="expr">原始基因表达矩阵（行=基因、列=样本）</param>
    ''' <param name="config">训练配置；为空时使用快速档</param>
    ''' <param name="progress">
    ''' 进度回调 <c>(已完成, 总数)</c>；该回调会在并行训练的多个线程之中被调用，
    ''' 因此不要在回调内部修改共享状态。
    ''' </param>
    ''' <returns></returns>
    Public Shared Function Train(graph As GeneRegulationGraph,
                                 expr As Matrix,
                                 Optional config As GeneTreeConfig = Nothing,
                                 Optional progress As Action(Of Integer, Integer) = Nothing) As GeneTreeTrainer

        If graph Is Nothing Then
            Throw New ArgumentNullException(NameOf(graph))
        End If
        If expr Is Nothing Then
            Throw New ArgumentNullException(NameOf(expr))
        End If

        Dim opt As GeneTreeConfig = If(config, GeneTreeConfig.CreateFast())

        Call opt.ValidateOrThrow()

        If expr.expression Is Nothing OrElse expr.expression.Length = 0 Then
            Throw New InvalidDataException("表达矩阵之中没有任何基因行，无法训练 GeneTree 模型")
        End If

        Dim rows As DataFrameRow() = expr.expression
        Dim rowIndex As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To rows.Length - 1
            If rows(i) Is Nothing OrElse String.IsNullOrEmpty(rows(i).geneID) Then
                Throw New InvalidDataException($"表达矩阵第 {i} 行缺少基因编号")
            End If

            rowIndex(rows(i).geneID) = i
        Next

        Dim nSamples As Integer = expr.sample_count

        If nSamples <= 0 Then
            ' 某些 CSV 没有表头样本名，此时矩阵对象不会记录 sampleID，退化为按行宽判断
            nSamples = rows(0).samples
        End If

        For i As Integer = 0 To rows.Length - 1
            If rows(i).experiments Is Nothing OrElse rows(i).experiments.Length <> nSamples Then
                Throw New InvalidDataException(
                    $"基因 '{rows(i).geneID}' 的样本维度是 {If(rows(i).experiments Is Nothing, 0, rows(i).experiments.Length)}，" &
                    $"与矩阵的样本数 {nSamples} 不一致")
            End If
        Next

        If nSamples < opt.MinSamples Then
            Throw New InvalidDataException(
                $"表达矩阵只有 {nSamples} 个样本，低于 GeneTree 所需的最小样本数 {opt.MinSamples}")
        End If

        Dim cells As Integer() = SampleCells(nSamples, opt.MaxSamplesPerModel, opt.Seed)
        Dim split As (train As Integer(), validate As Integer()) = SplitPositions(cells.Length, opt.ValidationRatio, opt.Seed Xor &H2A3F)

        ' ---- 选出参与训练的靶基因 ----
        Dim targets As New List(Of String)
        Dim missing As Integer = 0

        For Each gene As String In graph.Targets
            If rowIndex.ContainsKey(gene) Then
                Call targets.Add(gene)
            Else
                missing += 1
            End If
        Next

        If opt.Verbose Then
            Console.WriteLine($"[GeneTree] 训练靶基因: {targets.Count} 个（拓扑中有 {missing} 个基因不在表达矩阵之中）")
            Console.WriteLine($"[GeneTree] 细胞抽样: {cells.Length}/{nSamples}（训练 {split.train.Length}、验证 {split.validate.Length}）")
            Console.WriteLine($"[GeneTree] 配置: {opt.ToString}")
        End If

        If targets.Count = 0 Then
            Throw New InvalidDataException("ARACNE 拓扑之中没有任何靶基因能够映射到表达矩阵上，无法训练")
        End If

        If opt.MaxGenes > 0 AndAlso targets.Count > opt.MaxGenes Then
            ' Targets 已经按照调控者数量降序排列，直接截断即可保留调控关系最丰富的基因
            targets = targets.Take(opt.MaxGenes).ToList()

            If opt.Verbose Then
                Console.WriteLine($"[GeneTree] MaxGenes={opt.MaxGenes}，仅训练前 {targets.Count} 个靶基因")
            End If
        End If

        Dim models As GeneModel() = New GeneModel(rows.Length - 1) {}
        Dim total As Integer = targets.Count
        Dim finished As Integer = 0
        Dim popts As New ParallelOptions()

        m_logged = 0

        If opt.MaxDegreeOfParallelism > 0 Then
            popts.MaxDegreeOfParallelism = opt.MaxDegreeOfParallelism
        End If

        ' TGBoost 内部会把每一次建树都写入日志通道（18404 个模型 × 40 轮 ≈ 74 万行），
        ' 训练期间屏蔽控制台回显，避免把时间浪费在终端输出上
        Dim mute As Boolean = Microsoft.VisualBasic.VBDebugger.Mute

        Microsoft.VisualBasic.VBDebugger.Mute = True

        Try
            Parallel.For(0, total, popts,
                Sub(k As Integer)
                    Dim gene As String = targets(k)
                    Dim model As GeneModel = Nothing

                    Try
                        model = TrainGene(gene, graph, rows, rowIndex, cells, split.train, split.validate, opt)
                    Catch ex As Exception
                        Call log($"  [GeneTree] 基因 '{gene}' 拟合失败，退化为常数模型: {ex.Message}")
                    End Try

                    models(rowIndex(gene)) = model

                    Dim done As Integer = Interlocked.Increment(finished)

                    If progress IsNot Nothing Then
                        Call progress(done, total)
                    ElseIf opt.Verbose AndAlso (done Mod std.Max(1, opt.ProgressInterval) = 0 OrElse done = total) Then
                        Console.WriteLine($"  [GeneTree] 已完成 {done}/{total} 个基因模型的拟合")
                    End If
                End Sub)
        Finally
            Microsoft.VisualBasic.VBDebugger.Mute = mute
        End Try

        Dim report As TrainingReport = TrainingReport.FromModels(models, opt, cells.Length)

        If opt.Verbose Then
            Console.WriteLine($"[GeneTree] {report.ToString}")
        End If

        Return New GeneTreeTrainer(opt, graph, expr, cells, models, report)
    End Function

#Region "single gene fitting"

    ''' <summary>
    ''' 拟合单个靶基因的非线性调控函数
    ''' </summary>
    ''' <returns>模型对象；该基因没有任何可用调控者时返回 <c>Nothing</c></returns>
    Private Shared Function TrainGene(gene As String,
                                      graph As GeneRegulationGraph,
                                      rows As DataFrameRow(),
                                      rowIndex As Dictionary(Of String, Integer),
                                      cells As Integer(),
                                      trainPos As Integer(),
                                      validatePos As Integer(),
                                      opt As GeneTreeConfig) As GeneModel

        Dim inputs As RegulationInput() = graph.RegulatorsOf(gene) _
            .Where(Function(r) r IsNot Nothing AndAlso rowIndex.ContainsKey(If(r.regulator, ""))) _
            .ToArray

        If inputs.Length = 0 Then
            ' 没有任何调控者：这是一个源节点，迭代之中保持初始值不变
            Return Nothing
        End If

        Dim targetRow As DataFrameRow = rows(rowIndex(gene))
        Dim regRows As DataFrameRow() = inputs.Select(Function(r) rows(rowIndex(r.regulator))).ToArray
        Dim k As Integer = regRows.Length
        Dim labels As Double() = sanitizeLabels(targetRow, cells)

        Dim minValue As Double = labels.Min()
        Dim maxValue As Double = labels.Max()
        Dim mean As Double = labels.Average()
        ' 常数基线（标签标准差）：模型相对“永远预测均值”的改进幅度就用它来衡量
        Dim baselineRmse As Double = std.Sqrt(
            labels.Sum(Function(x) (x - mean) ^ 2) / std.Max(1, labels.Length - 1))
        Dim performance As New ModelPerformance With {
            .nRegulators = k,
            .nTrain = trainPos.Length,
            .nValidate = validatePos.Length,
            .minValue = minValue,
            .maxValue = maxValue
        }

        If trainPos.Length < opt.MinSamples Then
            ' 样本太少，无法训练树模型
            Return constantModel(gene, inputs, rowIndex, minValue, maxValue, mean, baselineRmse, performance, "样本不足")
        End If
        If maxValue - minValue <= 0.0R Then
            ' 常量基因，没有任何可学习的变化
            Return constantModel(gene, inputs, rowIndex, minValue, maxValue, mean, baselineRmse, performance, "常量表达")
        End If

        Dim trainFeatures As Single()() = buildFeatures(regRows, cells, trainPos, k)
        Dim validateFeatures As Single()() = buildFeatures(regRows, cells, validatePos, k)
        Dim trainLabels As Double() = trainPos.Select(Function(pos) labels(pos)).ToArray
        Dim validateLabels As Double()

        If validatePos.Length > 0 Then
            validateLabels = validatePos.Select(Function(pos) labels(pos)).ToArray
        Else
            validateLabels = New Double() {}
        End If

        Dim trainset As TrainData = tag(trainFeatures, trainLabels).ToTrainingSet(
            columns:=inputs.Select(Function(r) r.regulator).ToArray,
            categorical_features:=New String() {})
        Dim valset As ValidationData = Nothing

        If validateFeatures.Length >= 10 Then
            valset = tag(validateFeatures, validateLabels).ToValidateSet()
        End If

        Dim gbm As New GBM()

        Try
            ' 位置参数顺序见 GBM.fit：trainset, valset, early_stopping_rounds, maximize, eval_metric,
            ' loss, eta, num_boost_round, max_depth, scale_pos_weight, rowsample, colsample,
            ' min_child_weight, min_sample_split, lambda, gamma, num_thread
            Call gbm.fit(
                trainset,
                valset,
                opt.EarlyStoppingRounds,
                False,
                Metrics.mse,
                "squareloss",
                opt.LearningRate,
                opt.NumBoostRound,
                opt.MaxDepth,
                1.0R,
                opt.RowSample,
                opt.ColSample,
                opt.GetMinChildWeight(trainPos.Length),
                opt.MinSampleSplit,
                opt.Lambda,
                opt.Gamma,
                opt.ThreadsPerModel)
        Catch ex As Exception
            Call logLimited($"  [GeneTree] 基因 '{gene}' 的 TGBoost 拟合异常，退化为常数模型: {ex.Message}")

            Return constantModel(gene, inputs, rowIndex, minValue, maxValue, mean, baselineRmse, performance, "拟合异常")
        End Try

        ' ---- 评价 ----
        Dim evalFeatures As Single()()
        Dim evalLabels As Double()

        If validateFeatures.Length >= 10 Then
            evalFeatures = validateFeatures
            evalLabels = validateLabels
        Else
            evalFeatures = trainFeatures
            evalLabels = trainLabels
        End If

        Dim model As New GeneModel With {
            .Gene = gene,
            .Regulators = inputs.Select(Function(r) r.regulator).ToArray,
            .RegulatorIndex = inputs.Select(Function(r) rowIndex(r.regulator)).ToArray,
            .MinValue = minValue,
            .MaxValue = maxValue,
            .Constant = mean,
            .Performance = performance,
            .Booster = gbm
        }

        ' 用紧凑打分器而不是 GBM.predict：后者对每一棵树都要启动一次 PLINQ 查询，
        ' 在“每个基因都要评价 + 特征重要性扫描”的场景下开销会非常明显
        Dim prediction As Double() = model.PredictBatch(evalFeatures)

        performance.r2 = Metric.r2_score(prediction, evalLabels)
        performance.rmse = Metric.root_mean_square_error(prediction, evalLabels)
        performance.baselineRmse = Metric.root_mean_square_error(
            Enumerable.Repeat(evalLabels.Average(), evalLabels.Length).ToArray, evalLabels)
        performance.kind = model.Kind
        performance.trees = model.TreeCount
        performance.importance = computeImportance(model, inputs, trainFeatures)

        Return model
    End Function

    ''' <summary>
    ''' 构造常数模型（训练失败或样本不足时的降级路径）
    ''' </summary>
    Private Shared Function constantModel(gene As String,
                                          inputs As RegulationInput(),
                                          rowIndex As Dictionary(Of String, Integer),
                                          minValue As Double,
                                          maxValue As Double,
                                          mean As Double,
                                          baselineRmse As Double,
                                          performance As ModelPerformance,
                                          reason As String) As GeneModel

        Dim stat As New ModelPerformance With {
            .r2 = 0.0R,
            .rmse = baselineRmse,
            .baselineRmse = baselineRmse,
            .nTrain = performance.nTrain,
            .nValidate = performance.nValidate,
            .nRegulators = inputs.Length,
            .kind = "constant",
            .trees = 0,
            .minValue = minValue,
            .maxValue = maxValue,
            .importance = inputs _
                .Select(Function(r, i) New RegulatorImportance With {
                    .regulator = r.regulator,
                    .mi = r.mi,
                    .correlation = r.correlation,
                    .rank = i,
                    .splits = 0,
                    .sensitivity = 0.0R
                }) _
                .ToArray
        }

        Call logLimited($"  [GeneTree] 基因 '{gene}' 使用常数模型（{reason}）")

        Return New GeneModel With {
            .Gene = gene,
            .Regulators = inputs.Select(Function(r) r.regulator).ToArray,
            .RegulatorIndex = inputs.Select(Function(r) rowIndex(r.regulator)).ToArray,
            .MinValue = minValue,
            .MaxValue = maxValue,
            .Constant = mean,
            .Performance = stat,
            .Booster = Nothing
        }
    End Function

#End Region

#Region "features"

    ''' <summary>
    ''' 把一个基因的标签列按抽样细胞取出，并把 NaN/Infinity 用该基因的有限均值填补
    ''' </summary>
    Private Shared Function sanitizeLabels(row As DataFrameRow, cells As Integer()) As Double()
        Dim values As Double() = New Double(cells.Length - 1) {}
        Dim sum As Double = 0.0R
        Dim count As Integer = 0

        For i As Integer = 0 To cells.Length - 1
            Dim x As Double = row.experiments(cells(i))

            values(i) = x

            If Not Double.IsNaN(x) AndAlso Not Double.IsInfinity(x) Then
                sum += x
                count += 1
            End If
        Next

        Dim mean As Double = If(count > 0, sum / count, 0.0R)

        For i As Integer = 0 To values.Length - 1
            If Double.IsNaN(values(i)) OrElse Double.IsInfinity(values(i)) Then
                values(i) = mean
            End If
        Next

        Return values
    End Function

    Private Shared Function buildFeatures(regRows As DataFrameRow(),
                                          cells As Integer(),
                                          positions As Integer(),
                                          k As Integer) As Single()()
        Dim features As Single()() = New Single(positions.Length - 1)() {}

        For i As Integer = 0 To positions.Length - 1
            Dim cell As Integer = cells(positions(i))
            Dim row As Single() = New Single(k - 1) {}

            For j As Integer = 0 To k - 1
                row(j) = toSingle(regRows(j).experiments(cell))
            Next

            features(i) = row
        Next

        Return features
    End Function

    ''' <summary>
    ''' <c>Double → Single</c> 的安全转换：NaN/Infinity 一律映射为 TGBoost 的缺失值标记
    ''' </summary>
    Private Shared Function toSingle(x As Double) As Single
        If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then
            Return Data.NA
        Else
            Return CSng(x)
        End If
    End Function

    Private Shared Function tag(features As Single()(), labels As Double()) As DoubleTagged(Of Single())()
        Dim result As DoubleTagged(Of Single())() = New DoubleTagged(Of Single())(features.Length - 1) {}

        For i As Integer = 0 To features.Length - 1
            result(i) = New DoubleTagged(Of Single())(labels(i), features(i))
        Next

        Return result
    End Function

#End Region

#Region "importance"

    ''' <summary>
    ''' 统计每个调控因子在所有树之中的分裂次数，并用“单特征扫描”估计灵敏度
    ''' </summary>
    Private Shared Function computeImportance(model As GeneModel,
                                              inputs As RegulationInput(),
                                              trainFeatures As Single()()) As RegulatorImportance()
        Dim k As Integer = inputs.Length
        Dim splits As Integer() = New Integer(k - 1) {}
        Dim sensitivity As Double() = New Double(k - 1) {}

        Try
            If model.Booster IsNot Nothing Then
                For Each tree As Tree In model.Booster.trees
                    Call countSplits(tree.root, splits)
                Next
            End If

            If splits.Sum() > 0 AndAlso trainFeatures.Length > 0 Then
                sensitivity = computeSensitivity(model, trainFeatures, k)
            End If
        Catch ex As Exception
            Call logLimited($"  [GeneTree] 特征重要性统计失败: {ex.Message}")
        End Try

        Dim result As RegulatorImportance() = New RegulatorImportance(k - 1) {}

        For i As Integer = 0 To k - 1
            result(i) = New RegulatorImportance With {
                .regulator = inputs(i).regulator,
                .mi = inputs(i).mi,
                .correlation = inputs(i).correlation,
                .rank = i,
                .splits = splits(i),
                .sensitivity = sensitivity(i)
            }
        Next

        Return result
    End Function

    Private Shared Sub countSplits(node As TreeNode, splits As Integer())
        If node Is Nothing OrElse node.is_leaf Then
            Return
        End If

        If node.split_feature >= 0 AndAlso node.split_feature < splits.Length Then
            splits(node.split_feature) += 1
        End If

        Call countSplits(node.left_child, splits)
        Call countSplits(node.right_child, splits)

        If node.nan_child IsNot Nothing Then
            Call countSplits(node.nan_child, splits)
        End If
    End Sub

    ''' <summary>
    ''' 廉价偏依赖：其余特征固定在中位数，把目标特征从最小扫到最大，观察预测变化幅度
    ''' </summary>
    Private Shared Function computeSensitivity(model As GeneModel, trainFeatures As Single()(), k As Integer) As Double()
        Dim baseRow As Single() = New Single(k - 1) {}
        Dim minValue As Single() = New Single(k - 1) {}
        Dim maxValue As Single() = New Single(k - 1) {}

        For j As Integer = 0 To k - 1
            Dim values As New List(Of Single)

            For i As Integer = 0 To trainFeatures.Length - 1
                Dim value As Single = trainFeatures(i)(j)

                If value <> Data.NA AndAlso Not Single.IsNaN(value) Then
                    Call values.Add(value)
                End If
            Next

            If values.Count = 0 Then
                baseRow(j) = 0.0F
                minValue(j) = 0.0F
                maxValue(j) = 0.0F
            Else
                Call values.Sort()

                minValue(j) = values(0)
                maxValue(j) = values(values.Count - 1)
                baseRow(j) = values(values.Count \ 2)
            End If
        Next

        Dim probes As Single()() = New Single(2 * k - 1)() {}

        For j As Integer = 0 To k - 1
            Dim low As Single() = CType(baseRow.Clone(), Single())
            Dim high As Single() = CType(baseRow.Clone(), Single())

            low(j) = minValue(j)
            high(j) = maxValue(j)

            probes(2 * j) = low
            probes(2 * j + 1) = high
        Next

        Dim prediction As Double() = model.PredictBatch(probes)
        Dim result As Double() = New Double(k - 1) {}

        For j As Integer = 0 To k - 1
            result(j) = std.Abs(prediction(2 * j + 1) - prediction(2 * j))
        Next

        Return result
    End Function

#End Region

#Region "sampling"

    ''' <summary>
    ''' 抽取用于训练的细胞列（可复现）
    ''' </summary>
    ''' <param name="nSamples">样本总数</param>
    ''' <param name="maxCount">最多保留的样本数，≤0 或者大于总数时表示使用全部</param>
    ''' <param name="seed">随机种子</param>
    ''' <returns>升序排列的列下标</returns>
    Public Shared Function SampleCells(nSamples As Integer, maxCount As Integer, seed As Integer) As Integer()
        If nSamples <= 0 Then
            Return New Integer() {}
        End If
        If maxCount <= 0 OrElse maxCount >= nSamples Then
            Return Enumerable.Range(0, nSamples).ToArray
        End If

        Dim pool As Integer() = Enumerable.Range(0, nSamples).ToArray
        Dim rng As New Random(seed)

        For i As Integer = 0 To maxCount - 1
            Dim j As Integer = rng.Next(i, nSamples)
            Dim tmp As Integer = pool(i)

            pool(i) = pool(j)
            pool(j) = tmp
        Next

        Dim picked As Integer() = pool.Take(maxCount).ToArray

        Call Array.Sort(picked)

        Return picked
    End Function

    ''' <summary>
    ''' 把抽样得到的细胞按固定折分切成训练集与验证集（可复现）
    ''' </summary>
    ''' <param name="count">样本数</param>
    ''' <param name="validationRatio">验证集比例</param>
    ''' <param name="seed">随机种子</param>
    ''' <returns>
    ''' 位置下标（相对于传入的细胞数组）的二元组；当验证集少于 10 个样本时返回空验证集，
    ''' 此时训练不使用早停。
    ''' </returns>
    Public Shared Function SplitPositions(count As Integer, validationRatio As Double, seed As Integer) As (train As Integer(), validate As Integer())
        If count <= 0 Then
            Return (New Integer() {}, New Integer() {})
        End If

        Dim order As Integer() = Enumerable.Range(0, count).ToArray
        Dim rng As New Random(seed)

        For i As Integer = 0 To count - 1
            Dim j As Integer = rng.Next(i, count)
            Dim tmp As Integer = order(i)

            order(i) = order(j)
            order(j) = tmp
        Next

        Dim nValidate As Integer = CInt(std.Floor(count * validationRatio))

        If nValidate < 10 OrElse count - nValidate < 10 Then
            nValidate = 0
        End If

        If nValidate = 0 Then
            Dim all As Integer() = CType(order.Clone(), Integer())

            Call Array.Sort(all)

            Return (all, New Integer() {})
        End If

        Dim validate As Integer() = order.Take(nValidate).ToArray
        Dim train As Integer() = order.Skip(nValidate).ToArray

        Call Array.Sort(train)
        Call Array.Sort(validate)

        Return (train, validate)
    End Function

#End Region

    Private Shared Sub log(msg As String)
        Call Console.WriteLine(msg)
    End Sub

    ''' <summary>
    ''' 逐基因的诊断信息：并行训练时可能出现成千上万条同类消息，这里只保留前若干条
    ''' </summary>
    Private Shared Sub logLimited(msg As String)
        If Interlocked.Increment(m_logged) <= MAX_LOG_LINES Then
            Call log(msg)
        End If
    End Sub

End Class

