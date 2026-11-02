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

        ' ---- 结构发现：候选向量只在全部训练细胞上标准化一次，供所有靶基因共享 ----
        Dim discovery As DiscoveryContext = Nothing

        If opt.IsDiscoveryEnabled Then
            discovery = DiscoveryContext.Create(opt, rows, rowIndex, cells)

            If opt.Verbose Then
                If discovery Is Nothing Then
                    Console.WriteLine("[GeneTree] 结构发现已开启，但候选基因集合之中没有可用的基因（不在矩阵中或表达为常量），本次按 ARACNE 拓扑训练")
                Else
                    Console.WriteLine($"[GeneTree] 结构发现: {opt.EdgeDiscovery}，候选 {discovery.Candidates.Length} 个（给定 {opt.DiscoveryCandidates.Length} 个），" &
                                      $"筛选细胞 {discovery.Cells.Length} 个，每基因最多接受 {opt.DiscoveryMaxPerGene} 条 / 尝试 {opt.DiscoveryMaxCandidates} 个")
                End If
            End If
        End If

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
                Call $"[GeneTree] MaxGenes={opt.MaxGenes}，仅训练前 {targets.Count} 个靶基因".info
            End If
        End If

        Dim models As GeneModel() = New GeneModel(rows.Length - 1) {}
        Dim total As Integer = targets.Count
        Dim finished As Integer = 0
        Dim popts As New ParallelOptions()

        m_logged = 0

        ' ---- 并行度解析：外层按基因并行，单模型串行（避免嵌套并行把线程池拖死） ----
        Dim outerDop As Integer = GeneTreeParallelism.ResolveOuterDop(opt)
        Dim threadsPerModel As Integer = GeneTreeParallelism.ResolveThreadsPerModel(opt, outerDop)

        popts.MaxDegreeOfParallelism = outerDop

        ' 线程池预热：让 Parallel.For 一开始就能拿到足够的 worker
        Call GeneTreeParallelism.EnsureThreadPool(outerDop)

        If opt.Verbose Then
            Console.WriteLine($"[GeneTree] 并行配置: {GeneTreeParallelism.Describe(outerDop, threadsPerModel)}")
        End If

        ' TGBoost 内部会把每一次建树都写入日志通道（18404 个模型 × 40 轮 ≈ 74 万行），
        ' 训练期间屏蔽控制台回显，避免把时间浪费在终端输出上
        Dim mute As Boolean = VBDebugger.Mute

        VBDebugger.Mute = True

        Try
            Parallel.For(0, total, popts,
                Sub(k As Integer)
                    Dim gene As String = targets(k)
                    Dim model As GeneModel = Nothing

                    Dim ctx As DiscoveryContext = discovery

                    If ctx IsNot Nothing AndAlso opt.DiscoveryMaxGenes > 0 AndAlso k >= opt.DiscoveryMaxGenes Then
                        ' 试跑限流：只对前 N 个靶基因执行结构发现
                        ctx = Nothing
                    End If

                    Try
                        model = TrainGene(gene, graph, rows, rowIndex, cells, split.train, split.validate, opt, ctx, threadsPerModel)
                    Catch ex As Exception
                        Call log($"  [GeneTree] 基因 '{gene}' 拟合失败，退化为常数模型: {ex.Message}")
                    End Try

                    models(rowIndex(gene)) = model

                    Dim done As Integer = Interlocked.Increment(finished)

                    If progress IsNot Nothing Then
                        Call progress(done, total)
                    ElseIf opt.Verbose AndAlso (done Mod std.Max(1, opt.ProgressInterval) = 0 OrElse done = total) Then
                        Call $"  [GeneTree] 已完成 {done}/{total} 个基因模型的拟合".debug
                    End If
                End Sub)
        Finally
            VBDebugger.Mute = mute
        End Try

        Dim report As TrainingReport = TrainingReport.FromModels(models, opt, cells.Length)

        If discovery IsNot Nothing AndAlso report.Discovery IsNot Nothing Then
            report.Discovery.CandidatesScreened = discovery.Candidates.Length
        End If

        If opt.Verbose Then
            Call $"[GeneTree] {report.ToString}".info
        End If

        Return New GeneTreeTrainer(opt, graph, expr, cells, models, report)
    End Function

#Region "single gene fitting"

    ''' <summary>
    ''' 一次 GBM 拟合的结果（结构发现阶段会反复复用这个结构）
    ''' </summary>
    Private Structure FitResult
        Public gbm As GBM
        Public r2 As Double
        Public rmse As Double
        ''' <summary>同一评价集合上用标签均值作为预测时的 RMSE</summary>
        Public baselineRmse As Double
    End Structure

    ''' <summary>
    ''' 拟合单个靶基因的非线性调控函数
    ''' </summary>
    ''' <param name="gene">靶基因编号</param>
    ''' <param name="graph">ARACNE 拓扑（第一阶段只使用这里的边）</param>
    ''' <param name="rows">表达矩阵的行</param>
    ''' <param name="rowIndex">基因编号 → 行号</param>
    ''' <param name="cells">参与训练的细胞列下标</param>
    ''' <param name="trainPos"><paramref name="cells"/> 之中的训练位置</param>
    ''' <param name="validatePos"><paramref name="cells"/> 之中的验证位置</param>
    ''' <param name="opt">配置</param>
    ''' <param name="discovery">结构发现上下文；<c>Nothing</c> 表示该基因不执行发现</param>
    ''' <param name="threadsPerModel">单个模型在 TGBoost 内部的线程数（外层已并行时应当为 1）</param>
    ''' <returns>模型对象；该基因没有任何可用调控者（且未发现任何新边）时返回 <c>Nothing</c></returns>
    Private Shared Function TrainGene(gene As String,
                                      graph As GeneRegulationGraph,
                                      rows As DataFrameRow(),
                                      rowIndex As Dictionary(Of String, Integer),
                                      cells As Integer(),
                                      trainPos As Integer(),
                                      validatePos As Integer(),
                                      opt As GeneTreeConfig,
                                      discovery As DiscoveryContext,
                                      threadsPerModel As Integer) As GeneModel

        Dim inputs As RegulationInput() = graph.RegulatorsOf(gene) _
            .Where(Function(r) r IsNot Nothing AndAlso rowIndex.ContainsKey(If(r.regulator, ""))) _
            .ToArray

        If inputs.Length = 0 AndAlso discovery Is Nothing Then
            ' 没有任何调控者：这是一个源节点，迭代之中保持初始值不变
            Return Nothing
        End If

        Dim targetIndex As Integer = rowIndex(gene)
        Dim targetRow As DataFrameRow = rows(targetIndex)
        Dim labels As Double() = sanitizeLabels(targetRow, cells)

        Dim minValue As Double = labels.Min()
        Dim maxValue As Double = labels.Max()
        Dim mean As Double = labels.Average()
        ' 常数基线（标签标准差）：模型相对“永远预测均值”的改进幅度就用它来衡量
        Dim baselineRmse As Double = std.Sqrt(
            labels.Sum(Function(x) (x - mean) ^ 2) / std.Max(1, labels.Length - 1))
        Dim performance As New ModelPerformance With {
            .nRegulators = inputs.Length,
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

        ' ---- 第一阶段：只用 ARACNE 给出的调控者拟合 ----
        Dim regRows As DataFrameRow() = inputs.Select(Function(r) rows(rowIndex(r.regulator))).ToArray
        Dim regNames As String() = inputs.Select(Function(r) r.regulator).ToArray
        Dim regIndices As Integer() = inputs.Select(Function(r) rowIndex(r.regulator)).ToArray
        Dim current As FitResult

        If regRows.Length > 0 Then
            current = fitModel(gene, regRows, regNames, regIndices, targetRow, cells, trainPos, validatePos, labels, opt, threadsPerModel)

            If current.gbm Is Nothing Then
                Return constantModel(gene, inputs, rowIndex, minValue, maxValue, mean, baselineRmse, performance, "拟合异常")
            End If
        Else
            ' 源节点：先当作“只预测均值”的基线，交给发现阶段决定是否要加边
            current = New FitResult With {.gbm = Nothing, .r2 = 0.0R, .rmse = baselineRmse}
        End If

        ' ---- 第二阶段：残差二阶段结构发现 ----
        Dim discoveryEdges As New List(Of DiscoveryEdge)
        Dim discovered As New List(Of RegulationInput)

        If discovery IsNot Nothing Then
            Call discoverEdges(gene, targetIndex, rows, rowIndex, cells, trainPos, validatePos, labels, mean, opt,
                               discovery, regRows, regNames, regIndices, current, discoveryEdges, discovered,
                               threadsPerModel)
        End If

        If regRows.Length = 0 Then
            ' 源节点，且没有发现任何有效的新边
            Return Nothing
        End If

        Dim allInputs As RegulationInput() = inputs.Concat(discovered).ToArray
        Dim trainFeatures As Single()() = buildFeatures(regRows, cells, trainPos, regRows.Length)
        Dim model As New GeneModel With {
            .Gene = gene,
            .Regulators = regNames,
            .RegulatorIndex = regIndices,
            .MinValue = minValue,
            .MaxValue = maxValue,
            .Constant = mean,
            .Performance = performance,
            .Booster = current.gbm,
            .DiscoveryEdges = discoveryEdges.ToArray
        }

        performance.nRegulators = regRows.Length
        performance.r2 = current.r2
        performance.rmse = current.rmse
        performance.baselineRmse = If(current.baselineRmse > 0, current.baselineRmse, baselineRmse)
        performance.kind = model.Kind
        performance.trees = model.TreeCount
        performance.importance = computeImportance(model, allInputs, trainFeatures)

        ' 标注每一条边的来源，便于报告与验证（ARACNE 边在前，发现边追加在后）
        For Each edge As DiscoveryEdge In discoveryEdges.Where(Function(e) e.isAccepted)
            Dim item As RegulatorImportance = performance.importance.FirstOrDefault(
                Function(x) String.Equals(x.regulator, edge.regulator, StringComparison.OrdinalIgnoreCase))

            If item IsNot Nothing Then
                item.source = "discovered"
                item.deltaR2 = edge.deltaR2
            End If
        Next

        Return model
    End Function

    ''' <summary>
    ''' 在残差上筛选候选调控者，并逐个增量重拟合，以验证集 ΔR² 决定是否接受
    ''' </summary>
    ''' <remarks>
    ''' 该方法会就地修改 <paramref name="regRows"/>/<paramref name="regNames"/>/<paramref name="regIndices"/>
    ''' 与 <paramref name="current"/>，被接受的边会追加到末尾（保持特征顺序与模型一致）。
    ''' </remarks>
    Private Shared Sub discoverEdges(gene As String,
                                     targetIndex As Integer,
                                     rows As DataFrameRow(),
                                     rowIndex As Dictionary(Of String, Integer),
                                     cells As Integer(),
                                     trainPos As Integer(),
                                     validatePos As Integer(),
                                     labels As Double(),
                                     mean As Double,
                                     opt As GeneTreeConfig,
                                     discovery As DiscoveryContext,
                                     ByRef regRows As DataFrameRow(),
                                     ByRef regNames As String(),
                                     ByRef regIndices As Integer(),
                                     ByRef current As FitResult,
                                     edges As List(Of DiscoveryEdge),
                                     discovered As List(Of RegulationInput),
                                     threadsPerModel As Integer)

        Dim exclude As New HashSet(Of Integer)
        Dim accepted As Integer = 0
        Dim tried As Integer = 0

        Call exclude.Add(targetIndex)

        For Each index As Integer In regIndices
            Call exclude.Add(index)
        Next

        Dim r2Current As Double = current.r2
        Dim residual As Double() = computeResidual(current, regRows, regIndices, cells, discovery.Cells, labels, mean, opt)
        Dim ranked As List(Of (candidate As ResidualScreening.CandidateVector, r As Double)) =
            ResidualScreening.Screen(residual, discovery.Candidates, exclude, opt.DiscoveryMinCorrelation, Integer.MaxValue)
        Dim cursor As Integer = 0

        While tried < opt.DiscoveryMaxCandidates AndAlso accepted < opt.DiscoveryMaxPerGene AndAlso cursor < ranked.Count
            Dim item = ranked(cursor)

            cursor += 1

            If exclude.Contains(item.candidate.index) Then
                Continue While
            End If

            tried += 1

            Dim candidateRows As DataFrameRow() = regRows.Concat({rows(item.candidate.index)}).ToArray
            Dim candidateNames As String() = regNames.Concat({item.candidate.gene}).ToArray
            Dim candidateIndices As Integer() = regIndices.Concat({item.candidate.index}).ToArray
            Dim fit As FitResult = fitModel(gene, candidateRows, candidateNames, candidateIndices,
                                            rows(targetIndex), cells, trainPos, validatePos, labels, opt, threadsPerModel)
            Dim delta As Double = If(fit.gbm Is Nothing, 0.0R, fit.r2 - r2Current)
            Dim edge As New DiscoveryEdge With {
                .target = gene,
                .regulator = item.candidate.gene,
                .correlation = item.r,
                .deltaR2 = delta,
                .r2Before = r2Current,
                .r2After = If(fit.gbm Is Nothing, r2Current, fit.r2),
                .status = "rejected"
            }

            If fit.gbm Is Nothing Then
                edge.reason = "重拟合失败"

                Call edges.Add(edge)
                Call exclude.Add(item.candidate.index)
                Continue While
            End If

            edge.splits = countFeatureSplits(fit.gbm, candidateIndices.Length - 1)

            If delta >= opt.DiscoveryMinDeltaR2 AndAlso edge.splits > 0 Then
                ' 接受：新边追加在 ARACNE 特征之后，保证特征顺序在训练/预测/存档三处一致
                edge.status = "accepted"

                regRows = candidateRows
                regNames = candidateNames
                regIndices = candidateIndices
                current = fit
                r2Current = fit.r2
                accepted += 1

                Call exclude.Add(item.candidate.index)

                Call discovered.Add(New RegulationInput With {
                    .target = gene,
                    .regulator = item.candidate.gene,
                    .mi = 0.0R,
                    .correlation = item.r,
                    .interaction = "discovered"
                })

                ' 残差改变，重新排名剩余候选
                residual = computeResidual(current, regRows, regIndices, cells, discovery.Cells, labels, mean, opt)
                ranked = ResidualScreening.Screen(residual, discovery.Candidates, exclude, opt.DiscoveryMinCorrelation, Integer.MaxValue)
                cursor = 0
            Else
                edge.reason = If(edge.splits <= 0, "该特征未被任何树使用", $"ΔR² {delta.ToString("G4")} 低于阈值 {opt.DiscoveryMinDeltaR2.ToString("G4")}")

                Call exclude.Add(item.candidate.index)
            End If

            Call edges.Add(edge)
        End While
    End Sub

    ''' <summary>
    ''' 用给定的一组调控者拟合一个 GBM，并返回验证集 R²/RMSE
    ''' </summary>
    Private Shared Function fitModel(gene As String,
                                     regRows As DataFrameRow(),
                                     regNames As String(),
                                     regIndices As Integer(),
                                     targetRow As DataFrameRow,
                                     cells As Integer(),
                                     trainPos As Integer(),
                                     validatePos As Integer(),
                                     labels As Double(),
                                     opt As GeneTreeConfig,
                                     threadsPerModel As Integer) As FitResult

        Dim k As Integer = regRows.Length

        If k = 0 Then
            Return New FitResult With {.gbm = Nothing}
        End If

        ' 外层已经按基因并行时 threadsPerModel = 1：TGBoost 内部的建树并行必须关掉，
        ' 否则会形成“外层并行 × 内层并行”的嵌套并行，线程池被拖死后 CPU 反而用不满
        Dim numThread As Integer = If(threadsPerModel > 0, threadsPerModel, -1)

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
            columns:=regNames,
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
                numThread)
        Catch ex As Exception
            Call logLimited($"  [GeneTree] 基因 '{gene}' 的 TGBoost 拟合异常: {ex.ToString}")

            Return New FitResult With {.gbm = Nothing}
        End Try

        Dim evalFeatures As Single()()
        Dim evalLabels As Double()

        If validateFeatures.Length >= 10 Then
            evalFeatures = validateFeatures
            evalLabels = validateLabels
        Else
            evalFeatures = trainFeatures
            evalLabels = trainLabels
        End If

        ' 用紧凑打分器而不是 GBM.predict：后者对每一棵树都要启动一次 PLINQ 查询
        Dim probe As New GeneModel With {.Gene = gene, .RegulatorIndex = regIndices, .Booster = gbm}
        Dim prediction As Double() = probe.PredictBatch(evalFeatures)

        Return New FitResult With {
            .gbm = gbm,
            .r2 = Metric.r2_score(prediction, evalLabels),
            .rmse = Metric.root_mean_square_error(prediction, evalLabels),
            .baselineRmse = Metric.root_mean_square_error(
                Enumerable.Repeat(evalLabels.Average(), evalLabels.Length).ToArray, evalLabels)
        }
    End Function

    ''' <summary>
    ''' 统计某个特征在全部树之中被用作分裂特征的节点数量
    ''' </summary>
    ''' <param name="gbm">已训练的模型</param>
    ''' <param name="feature">特征下标</param>
    ''' <returns></returns>
    Private Shared Function countFeatureSplits(gbm As GBM, feature As Integer) As Integer
        If gbm Is Nothing OrElse gbm.trees Is Nothing OrElse feature < 0 Then
            Return 0
        End If

        Dim splits As Integer() = New Integer(feature) {}

        For Each tree As Tree In gbm.trees
            Call countSplits(tree.root, splits)
        Next

        Return splits(feature)
    End Function

    ''' <summary>
    ''' 计算靶基因在筛选细胞上的标准化残差（<c>label − prediction</c>）
    ''' </summary>
    ''' <param name="fit">当前模型；<c>gbm</c> 为空时退化为“预测均值”</param>
    ''' <param name="regRows">当前特征行</param>
    ''' <param name="regIndices">当前特征对应的基因行号</param>
    ''' <param name="cells">细胞列下标</param>
    ''' <param name="sample"><paramref name="cells"/> 之中用于筛选的位置</param>
    ''' <param name="labels">与 <paramref name="cells"/> 对齐的标签</param>
    ''' <param name="mean">标签均值（无模型时的常数预测）</param>
    ''' <param name="opt">配置</param>
    ''' <returns>长度为 <paramref name="sample"/>.Length 的标准分残差；残差为常量时返回零向量</returns>
    Private Shared Function computeResidual(fit As FitResult,
                                            regRows As DataFrameRow(),
                                            regIndices As Integer(),
                                            cells As Integer(),
                                            sample As Integer(),
                                            labels As Double(),
                                            mean As Double,
                                            opt As GeneTreeConfig) As Double()

        Dim n As Integer = sample.Length
        Dim residual As Double() = New Double(n - 1) {}
        Dim predicted As Double()

        If fit.gbm Is Nothing OrElse regRows.Length = 0 Then
            predicted = Enumerable.Repeat(mean, n).ToArray
        Else
            Dim probe As New GeneModel With {.RegulatorIndex = regIndices, .Booster = fit.gbm}

            predicted = probe.PredictBatch(buildFeatures(regRows, cells, sample, regRows.Length))
        End If

        Dim sum As Double = 0.0R

        For i As Integer = 0 To n - 1
            residual(i) = labels(sample(i)) - predicted(i)
            sum += residual(i)
        Next

        Dim avg As Double = sum / n
        Dim sse As Double = 0.0R

        For i As Integer = 0 To n - 1
            sse += (residual(i) - avg) ^ 2
        Next

        Dim sd As Double = std.Sqrt(sse / std.Max(1, n - 1))

        If sd <= 1.0E-09R Then
            ' 残差完全平坦：与任何候选都不相关
            Return New Double(n - 1) {}
        End If

        For i As Integer = 0 To n - 1
            residual(i) = (residual(i) - avg) / sd
        Next

        Return residual
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

