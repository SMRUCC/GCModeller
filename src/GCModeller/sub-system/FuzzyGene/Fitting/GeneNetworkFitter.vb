Imports System.Collections.Concurrent
Imports System.Threading
Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.DataMining.FuzzyLogic.TSK
Imports Microsoft.VisualBasic.DeepLearning.ANFIS
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.FuzzyGene.Data
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace Fitting

    ''' <summary>
    ''' 逐靶基因的神经模糊拟合编排器。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 流程：预处理（标准化 / 时间聚合 / 路线判定）→ 候选调控因子筛选 → 规则预算降级 →
    ''' 单靶基因 ANFIS 训练（可剪枝）→ 解析灵敏度汇总成带符号边。
    ''' </para>
    ''' <para>
    ''' 为什么要**先**做预算降级：<c>TskNetworkBuilder.CreateGrid</c> 在
    ''' <c>m^n &gt; MaxRules</c> 或 <c>m^n·(n+1) &gt; MaxConsequentParams</c> 时直接抛异常，
    ''' 而异常重试的代价远高于按公式预先算出可行的结构。
    ''' </para>
    ''' <para>
    ''' 单个基因训练失败（数据退化、LSE 病态、剪枝失败……）只记录为 <c>failed</c>，
    ''' 不会中断整次拟合——大规模批量建模时这比「一处失败全盘返回」有用得多。
    ''' </para>
    ''' </remarks>
    Public Module GeneNetworkFitter

        ''' <summary>从原始表达矩阵拟合调控网络。</summary>
        Public Function Fit(expression As GeneExpressionData,
                            Optional prior As PriorNetwork = Nothing,
                            Optional config As FuzzyGeneConfig = Nothing) As FuzzyGeneFitOutput

            Dim cfg As FuzzyGeneConfig = If(config, New FuzzyGeneConfig()).ValidateGene()

            If expression Is Nothing Then
                Throw New ArgumentNullException(NameOf(expression))
            End If

            Dim view As GeneExpressionView = ExpressionPreprocessor.Preprocess(expression, prior, cfg)

            Return Fit(view, prior, cfg)
        End Function

        ''' <summary>从预处理视图拟合调控网络（复用视图做交叉验证/重复拟合时使用）。</summary>
        Public Function Fit(view As GeneExpressionView, prior As PriorNetwork, config As FuzzyGeneConfig) As FuzzyGeneFitOutput
            If view Is Nothing Then
                Throw New ArgumentNullException(NameOf(view))
            End If

            Dim cfg As FuzzyGeneConfig = If(config, New FuzzyGeneConfig()).ValidateGene()
            Dim started As DateTime = DateTime.Now

            If view.NRows < cfg.MinSamples Then
                Throw New ArgumentException(
                    $"only {view.NRows} training rows are available, at least {cfg.MinSamples} are required", NameOf(view))
            End If

            Dim n As Integer = view.NGene
            Dim models(n - 1) As GeneRegulationModel
            Dim metrics(n - 1) As GeneFitMetric
            Dim usedPicks(n - 1)() As RegulatorPick
            Dim weights(n - 1)() As Double

            ' 候选池只构建一次（全基因组规模下每一对基因的相关性扫描都是 O(行数)）
            Dim pool As Integer() = RegulatorSelector.BuildCandidatePool(view, prior, cfg)
            ' [0] = 候选筛选耗时（Stopwatch tick），[1] = 训练耗时
            Dim phaseTicks As Long() = New Long(1) {}

            Dim completed As Integer = 0
            Dim reportStep As Integer = std.Max(1, n \ 35)
            Dim clock As New Stopwatch()

            Call clock.Start()
            Call "Start to make fuzzy gene network fitting.".info

            ' 逐基因训练在真实数据上可能要跑几十分钟，这里按固定步长打一行进度，
            ' 否则调用方在整段训练期间完全看不到进展
            Dim work As Action(Of Integer) =
                Sub(g)
                    Call FitOneGene(g, view, prior, cfg, pool, models, metrics, usedPicks, weights, phaseTicks)

                    Dim done As Integer = Interlocked.Increment(completed)

                    If done Mod reportStep = 0 OrElse done = n Then
                        Call $"[FuzzyGene] {done}/{n} target genes processed, {StringFormats.ReadableElapsedTime(clock.Elapsed.TotalMilliseconds)} elapsed".debug
                    End If
                End Sub

            ' 专用长任务而不是 Parallel.For：每个基因的训练是"粗粒度、长耗时"的，
            ' ThreadPool 的 hill-climbing 在 GC/带宽受限时会把 worker 数稳定在远低于
            ' MaxDegreeOfParallelism 的水平（128 核机器上可能只跑几十条线程）。
            ' 这里直接开 N 条 LongRunning 线程、共用一个取号游标，既保证启动即满员，
            ' 又保留动态负载均衡（不像静态均分会卡在最慢的那一块）。
            Dim workers As Integer = If(cfg.DegreeOfParallelism > 0, cfg.DegreeOfParallelism, Environment.ProcessorCount)

            workers = std.Max(1, std.Min(workers, n))

            Dim allocatedBefore As Long = GC.GetTotalAllocatedBytes(precise:=True)
            Dim gen2Before As Integer = GC.CollectionCount(2)

            If workers = 1 Then
                For g As Integer = 0 To n - 1
                    Call work(g)
                Next
            Else
                Call $"[FuzzyGene] fitting {n} target genes on {workers} worker thread(s)".debug

                Dim cursor As Integer = -1
                Dim errors As New ConcurrentBag(Of Exception)
                Dim runs(workers - 1) As Task

                For w As Integer = 0 To workers - 1
                    runs(w) = Task.Factory.StartNew(
                        Sub()
                            Try
                                Do
                                    Dim g As Integer = Interlocked.Increment(cursor)

                                    If g >= n Then
                                        Exit Do
                                    End If

                                    Call work(g)
                                Loop
                            Catch ex As Exception
                                Call errors.Add(ex)
                            End Try
                        End Sub,
                        TaskCreationOptions.LongRunning)
                Next

                Call Task.WaitAll(runs)

                If Not errors.IsEmpty Then
                    Throw New AggregateException("one or more worker threads failed while fitting the network", errors)
                End If
            End If

            Dim workersUsed As Integer = workers

            ' ---- 汇总边表（顺序遍历，保证输出可复现） ----
            Dim edges As New List(Of RegulationEdgeModel)

            For g As Integer = 0 To n - 1
                Dim model As GeneRegulationModel = models(g)

                If model Is Nothing OrElse usedPicks(g) Is Nothing Then
                    Continue For
                End If

                Dim sensitivity As Double() = weights(g)
                Dim picks As RegulatorPick() = usedPicks(g)

                For j As Integer = 0 To picks.Length - 1
                    If std.Abs(sensitivity(j)) < cfg.MinEdgeWeight Then
                        Continue For
                    End If

                    Call edges.Add(New RegulationEdgeModel With {
                        .Source = picks(j).Gene,
                        .Target = model.TargetGene,
                        .Weight = sensitivity(j),
                        .Confidence = picks(j).Confidence,
                        .IsPrior = picks(j).IsPrior
                    })
                Next
            Next

            ' ---- 汇总诊断 ----
            Dim report As New FuzzyGeneTrainingResult With {
                .Mode = view.Mode,
                .NGene = n,
                .NSample = view.NRows,
                .NSampleRaw = view.NSample,
                .NCandidatePool = If(pool Is Nothing, n, pool.Length),
                .GeneMetrics = metrics,
                .EdgeCount = edges.Count,
                .PreprocessElapsed = view.PreprocessElapsed,
                .SelectionElapsed = TicksToTimeSpan(phaseTicks(0)),
                .TrainingElapsed = TicksToTimeSpan(phaseTicks(1)),
                .ParallelWorkers = workersUsed,
                .AllocatedMB = (GC.GetTotalAllocatedBytes(precise:=True) - allocatedBefore) / 1024.0 / 1024,
                .Gen2Collections = GC.CollectionCount(2) - gen2Before,
                .Elapsed = DateTime.Now - started
            }

            Dim trained As GeneFitMetric() = metrics.Where(Function(m) m IsNot Nothing AndAlso m.Status = "ok").ToArray()

            report.NTrained = trained.Length
            report.NSkipped = metrics.Count(Function(m) m IsNot Nothing AndAlso m.Status = "skipped")
            report.NFailed = metrics.Count(Function(m) m IsNot Nothing AndAlso m.Status = "failed")
            report.MeanR2 = If(trained.Length > 0, trained.Average(Function(m) m.R2), Double.NaN)
            report.MeanRmse = If(trained.Length > 0, trained.Average(Function(m) m.RMSE), Double.NaN)

            Dim warnings As New List(Of String)

            If trained.Length = 0 Then
                Call warnings.Add("no target gene could be fitted, check MinCorrelation / Selection / MaxRegulators")
            End If
            If view.IsLagged Then
                Call warnings.Add($"lagged route: {view.UniqueTimePoints.Length} time points -> {view.NRows} training pairs")
            End If
            If view.NRows < view.NSample Then
                Call warnings.Add($"row sampling: {view.NSample} samples -> {view.NRows} training rows")
            End If
            If pool IsNot Nothing AndAlso pool.Length < n Then
                Call warnings.Add($"candidate pool: {pool.Length}/{n} genes used for the correlation screening")
            End If
            If cfg.MaxGenes > 0 AndAlso view.NGene >= cfg.MaxGenes Then
                Call warnings.Add($"{NameOf(FuzzyGeneConfig.MaxGenes)} = {cfg.MaxGenes} is active: only the highest variance genes were modelled, set it to 0 to fit every gene")
            End If

            ' 规则预算降级：这是"配了 K 个调控因子、实际却更少"的直接原因，必须显式提示
            Dim degraded As Integer = metrics.Count(
                Function(m) m IsNot Nothing AndAlso m.Message IsNot Nothing AndAlso
                            m.Message.StartsWith("degraded to", StringComparison.OrdinalIgnoreCase))

            If degraded > 0 Then
                Call warnings.Add(
                    $"{degraded} gene(s) could not keep the configured structure: the rule budget " &
                    $"(MaxRules={cfg.MaxRules}, MaxConsequentParams={cfg.MaxConsequentParams}) reduced " &
                    $"mf={cfg.MfCount} / K={cfg.MaxRegulators} to the structure shown on the 'structure:' line. " &
                    "Raise those two limits (or lower MfCount) to keep more regulators")
            End If
            For Each item As GeneFitMetric In metrics
                If item IsNot Nothing AndAlso item.Status <> "ok" Then
                    Call warnings.Add(item.ToString())
                End If
            Next

            report.Warnings = warnings.ToArray()

            Dim sds As Double() = view.SDs
            Dim supportMin As Double() = New Double(n - 1) {}
            Dim supportMax As Double() = New Double(n - 1) {}

            For g As Integer = 0 To n - 1
                If models(g) IsNot Nothing Then
                    supportMin(g) = models(g).SupportMin
                    supportMax(g) = models(g).SupportMax
                Else
                    supportMin(g) = -1.0
                    supportMax(g) = 1.0
                End If
            Next

            Dim network As New FuzzyGeneNetwork(
                view.GeneNames,
                models,
                edges.ToArray(),
                cfg,
                view.Means,
                sds,
                view.BaselineRaw,
                supportMin,
                supportMax,
                view.Mode,
                view.Standardized)

            Return New FuzzyGeneFitOutput With {
                .Network = network,
                .Report = report
            }
        End Function

        ''' <summary>
        ''' 训练单个靶基因（并行安全：只写自己下标对应的槽位）。
        ''' </summary>
        Private Sub FitOneGene(g As Integer,
                               view As GeneExpressionView,
                               prior As PriorNetwork,
                               cfg As FuzzyGeneConfig,
                               pool As Integer(),
                               models As GeneRegulationModel(),
                               metrics As GeneFitMetric(),
                               usedPicks As RegulatorPick()(),
                               weights As Double()(),
                               phaseTicks As Long())

            Dim geneName As String = view.GeneNames(g)
            Dim metric As New GeneFitMetric With {.Gene = geneName, .Status = "skipped"}

            metrics(g) = metric

            If view.Constant(g) Then
                metric.Message = "constant expression"
                Return
            End If

            Dim clock As New Stopwatch()

            clock.Start()

            ' ---- 规则预算：先算出真正可行的 (mf, K) 与划分方式 ----
            ' 必须在挑选候选**之前**算出来：否则 novelty 配额会按配置里的 K（可能远大于可用槽位）
            ' 去分配预留槽位，随后又被尾部截断吃掉，等于白配。
            Dim plan As (mfCount As Integer, inputs As Integer, scatter As Boolean) = ResolvePlan(cfg, cfg.MaxRegulators)

            If Not plan.scatter AndAlso ExceedsBudget(cfg, plan.mfCount, 1) Then
                metric.Status = "failed"
                metric.Message = $"rule budget exceeded even for a single input (MaxRules={cfg.MaxRules}, MaxConsequentParams={cfg.MaxConsequentParams})"
                Return
            End If

            Dim picks As RegulatorPick() = RegulatorSelector.SelectRegulators(view, prior, cfg, g, pool, plan.inputs)

            Call Interlocked.Add(phaseTicks(0), clock.ElapsedTicks)
            Call clock.Restart()

            If picks.Length = 0 Then
                metric.Message = $"no candidate regulator (selection={cfg.Selection}, min|r|={cfg.MinCorrelation:G3})"
                Return
            End If

            Dim mfCount As Integer = plan.mfCount
            Dim selected As New List(Of RegulatorPick)(picks)

            ' 散射划分的规则数由簇数决定，与输入维数无关 —— 这里不能再按 m^K 削输入
            If Not plan.scatter Then
                ' 兜底：正常情况下不会触发（selection 已按 feasible K 取候选）
                Do While selected.Count > 1 AndAlso ExceedsBudget(cfg, mfCount, selected.Count)
                    If mfCount > 2 Then
                        mfCount -= 1
                    Else
                        Call selected.RemoveAt(selected.Count - 1)
                    End If
                Loop

                If ExceedsBudget(cfg, mfCount, selected.Count) Then
                    metric.Status = "failed"
                    metric.Message = $"rule budget exceeded even at mf={mfCount}, inputs={selected.Count}"
                    Return
                End If
            End If

            Dim rows As Integer = view.NRows

            If rows < cfg.MinSamples Then
                metric.Message = $"only {rows} training rows"
                Return
            End If

            Dim regulators(selected.Count - 1) As String
            Dim regulatorIndex(selected.Count - 1) As Integer

            For j As Integer = 0 To selected.Count - 1
                regulators(j) = selected(j).Gene
                regulatorIndex(j) = view.IndexOf(selected(j).Gene)
            Next

            Dim x(rows - 1)() As Double
            Dim y(rows - 1) As Double
            Dim yJagged(rows - 1)() As Double
            Dim zMin As Double = Double.MaxValue
            Dim zMax As Double = Double.MinValue

            For r As Integer = 0 To rows - 1
                x(r) = New Double(regulatorIndex.Length - 1) {}
            Next

            ' 逐列填充：每一列都是连续内存，按列写 x 的对应分量比逐行取样本行更省缓存
            For j As Integer = 0 To regulatorIndex.Length - 1
                Dim column As Double() = view.Columns(regulatorIndex(j))

                For r As Integer = 0 To rows - 1
                    x(r)(j) = column(r)
                Next
            Next

            Dim targets As Double() = view.Targets(g)

            For r As Integer = 0 To rows - 1
                Dim target As Double = targets(r)

                y(r) = target
                yJagged(r) = New Double() {target}

                If target < zMin Then zMin = target
                If target > zMax Then zMax = target
            Next

            Dim options As AnfisOptions = cfg.Clone()

            options.MfCount = mfCount
            options.MfCounts = Nothing
            options.FeatureNames = regulators
            options.OutputNames = New String() {geneName}

            If plan.scatter Then
                ' 结论参数 P = 簇数 × (输入数 + 1)，受 MaxConsequentParams 约束；
                ' 顺便保证簇数不超过 MaxRules（CreateScatter 也会再校验一次）
                Dim maxClusters As Integer = std.Max(2, cfg.MaxConsequentParams \ (selected.Count + 1))

                options.RulePartition = RulePartitionKind.Scatter
                options.ScatterRuleCount = std.Max(2, std.Min(cfg.ScatterRuleCount, std.Min(maxClusters, cfg.MaxRules)))
                options.ScatterInputs = std.Max(1, std.Min(cfg.ScatterInputs, selected.Count))
                options.ScatterMinInputs = std.Max(1, std.Min(cfg.ScatterMinInputs, options.ScatterInputs))
            Else
                options.RulePartition = RulePartitionKind.Grid
            End If

            metric.Partition = If(plan.scatter, "scatter", "grid")

            If rows < 10 Then
                ' 样本太少时再切验证集会耗尽训练集，直接用训练误差做早停
                options.ValidationRatio = 0.0
            End If

            Dim model As TskNetwork
            Dim iterations As Integer
            Dim converged As Boolean
            Dim pruned As Integer = 0

            Try
                Dim fitted As AnfisFitResult = AnfisFitting.Fit(x, y, options)

                model = fitted.Model
                iterations = fitted.Iterations
                converged = fitted.Converged

                If cfg.PruneRuleThreshold > 0.0 AndAlso model.Rules.RuleCount > 2 Then
                    Try
                        Dim prunedFit As AnfisFitResult = AnfisFitting.PruneRules(
                            model, x, yJagged, cfg.PruneRuleThreshold, options)

                        If prunedFit.Model IsNot Nothing AndAlso prunedFit.Model.Rules.RuleCount > 0 Then
                            model = prunedFit.Model
                            pruned = prunedFit.PrunedRuleCount
                        End If
                    Catch ex As Exception
                        ' 剪枝失败不影响主模型（剪枝只是后处理）
                    End Try
                End If
            Catch ex As Exception
                metric.Status = "failed"
                metric.Message = ex.Message
                Call Interlocked.Add(phaseTicks(1), clock.ElapsedTicks)
                Return
            End Try

            ' ---- 训练集上的平均触发强度（可解释性/剪枝依据） ----
            Dim ruleCount As Integer = model.Rules.RuleCount
            Dim nInput As Integer = model.InputCount
            Dim meanActivation(ruleCount - 1) As Double
            Dim layers As New TskLayers(model)
            ' 一次分配同时覆盖"全量行前向"与"灵敏度网格前向"（网格点数上限约 1024）
            Dim traceWorkspace As New TskForwardWorkspace(std.Max(rows, 1024), model)

            Try
                Dim forward As TskInferenceTrace = layers.Forward(x, 0, rows, traceWorkspace)

                For t As Integer = 0 To rows - 1
                    For r As Integer = 0 To ruleCount - 1
                        meanActivation(r) += forward.Wbar(t, r)
                    Next
                Next

                For r As Integer = 0 To ruleCount - 1
                    meanActivation(r) /= rows
                Next
            Catch ex As Exception
                metric.Status = "failed"
                metric.Message = "inference failed: " & ex.Message
                Call Interlocked.Add(phaseTicks(1), clock.ElapsedTicks)
                Return
            End Try

            ' ---- 边权重：在输入支撑域的结构化网格上平均解析灵敏度 ----
            ' 训练样本上的输入通常高度共线（例如 B 与 C 互为镜像），沿数据流形求平均偏导
            ' 会让相互抵消的结论系数把符号算错。改用「逐输入取分位点、全组合」的网格做平均，
            ' 语义上等于“保持其它调控因子不变”的偏效应，也正是虚拟扰动的口径。
            Dim sensitivity(nInput - 1) As Double
            Dim weightActivation As Double() = meanActivation

            Try
                Dim grid As Double()() = BuildSensitivityGrid(x, nInput)
                Dim gridForward As TskInferenceTrace = layers.Forward(grid, 0, grid.Length, traceWorkspace)
                Dim activation(ruleCount - 1) As Double
                Dim valid As Integer = 0

                For t As Integer = 0 To grid.Length - 1
                    If Not gridForward.Valid(t) Then
                        Continue For
                    End If

                    valid += 1

                    For r As Integer = 0 To ruleCount - 1
                        activation(r) += gridForward.Wbar(t, r)
                    Next
                Next

                If valid > 0 Then
                    For r As Integer = 0 To ruleCount - 1
                        activation(r) /= valid
                    Next

                    weightActivation = activation
                End If
            Catch ex As Exception
                ' 网格前向失败时退回训练集平均（精度退化但不中断）
            End Try

            Dim theta As Double(,) = model.Theta

            For r As Integer = 0 To ruleCount - 1
                Dim w As Double = weightActivation(r)

                If w = 0.0 Then
                    Continue For
                End If

                Dim offset As Integer = r * (nInput + 1)

                For j As Integer = 0 To nInput - 1
                    sensitivity(j) += w * theta(offset + j, 0)
                Next
            Next

            ' ---- 训练指标（对最终模型重新评估，剪枝后也保持一致） ----
            Dim r2 As Double = Double.NaN
            Dim rmse As Double = Double.NaN

            Try
                Dim evaluation As AnfisFitResult = AnfisFitting.Evaluate(model, x, yJagged)

                r2 = evaluation.R2
                rmse = evaluation.RMSE
            Catch ex As Exception
                ' 指标计算失败不阻止模型导出
            End Try

            Dim geneModel As New GeneRegulationModel With {
                .TargetGene = geneName,
                .Regulators = regulators,
                .Network = model,
                .R2 = r2,
                .RMSE = rmse,
                .Iterations = iterations,
                .Converged = converged,
                .PrunedRuleCount = pruned,
                .RuleActivations = meanActivation,
                .Warnings = If(pruned > 0, New String() {$"pruned {pruned} rules"}, New String() {}),
                .Mean = view.Means(g),
                .Sd = view.SDs(g),
                .SupportMin = zMin,
                .SupportMax = zMax,
                .IsLagged = view.IsLagged,
                .SampleCount = rows
            }

            models(g) = geneModel
            usedPicks(g) = selected.ToArray()
            weights(g) = sensitivity

            metric.Status = "ok"
            metric.Regulators = regulators
            metric.R2 = r2
            metric.RMSE = rmse
            metric.RuleCount = ruleCount
            metric.MfCount = mfCount
            metric.SampleCount = rows
            metric.Iterations = iterations
            metric.Message = If(selected.Count < picks.Length,
                $"degraded to {selected.Count} inputs / mf={mfCount}", "")

            Call Interlocked.Add(phaseTicks(1), clock.ElapsedTicks)
        End Sub

        ''' <summary>Stopwatch tick → TimeSpan（Stopwatch 频率与 TimeSpan tick 不一定相等）。</summary>
        Private Function TicksToTimeSpan(ticks As Long) As TimeSpan
            If ticks <= 0 Then
                Return TimeSpan.Zero
            End If

            Return TimeSpan.FromSeconds(ticks / CDbl(Stopwatch.Frequency))
        End Function

        ''' <summary>
        ''' 构造用于估计边权重的输入网格：每个输入取若干分位点，做全组合。
        ''' </summary>
        ''' <remarks>
        ''' 网格点数按输入维数自适应（1→7、2→7²、3→5³、4→4⁴、5→3⁵、≥6→2^n），
        ''' 上限约 1000 点；全部点一次性前向，因此成本与训练集大小无关。
        ''' </remarks>
        Private Function BuildSensitivityGrid(x As Double()(), inputCount As Integer) As Double()()
            Dim levels As Integer = LevelsForInputs(inputCount)
            Dim columns(inputCount - 1)() As Double

            For j As Integer = 0 To inputCount - 1
                columns(j) = QuantileLevels(x, j, levels)
            Next

            Dim total As Integer = 1

            For j As Integer = 0 To inputCount - 1
                total *= columns(j).Length
            Next

            Dim grid(total - 1)() As Double
            Dim cursor(inputCount - 1) As Integer

            For k As Integer = 0 To total - 1
                Dim row(inputCount - 1) As Double

                For j As Integer = 0 To inputCount - 1
                    row(j) = columns(j)(cursor(j))
                Next

                grid(k) = row

                ' 里程表式进位
                For j As Integer = inputCount - 1 To 0 Step -1
                    cursor(j) += 1

                    If cursor(j) < columns(j).Length Then
                        Exit For
                    End If

                    cursor(j) = 0
                Next
            Next

            Return grid
        End Function

        Private Function LevelsForInputs(inputCount As Integer) As Integer
            Select Case inputCount
                Case 1
                    Return 7
                Case 2
                    Return 7
                Case 3
                    Return 5
                Case 4
                    Return 4
                Case 5
                    Return 3
                Case Else
                    Return 2
            End Select
        End Function

        ''' <summary>某一列输入值的等分位点（含最小与最大值）。</summary>
        Private Function QuantileLevels(x As Double()(), column As Integer, levels As Integer) As Double()
            Dim values(x.Length - 1) As Double

            For i As Integer = 0 To x.Length - 1
                values(i) = x(i)(column)
            Next

            Call Array.Sort(values)

            Dim result(levels - 1) As Double

            For k As Integer = 0 To levels - 1
                Dim pos As Integer = CInt(std.Round(k * (values.Length - 1) / CDbl(levels - 1)))

                If pos < 0 Then pos = 0
                If pos >= values.Length Then pos = values.Length - 1

                result(k) = values(pos)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 按规则预算求出实际可行的 <c>(模糊划分数, 输入个数)</c>。
        ''' </summary>
        ''' <param name="cfg">建模配置</param>
        ''' <param name="requested">期望的输入个数（通常为 <see cref="FuzzyGeneConfig.MaxRegulators"/>）</param>
        ''' <returns>降级之后的 <c>(mfCount, inputs)</c>，<c>inputs</c> 至少为 1</returns>
        ''' <remarks>
        ''' 降级顺序与训练时一致：先降模糊划分数（不低于 2），再逐个削减输入。
        ''' 之所以把这段逻辑单独暴露出来：候选筛选需要知道**真实的** K，
        ''' 否则按配置的 K 去分配 novelty 配额（见 <see cref="FuzzyGeneConfig.NoveltyQuota"/>）
        ''' 会被后续截断吃掉。
        ''' </remarks>
        Public Function ResolveStructure(cfg As FuzzyGeneConfig, requested As Integer) As (mfCount As Integer, inputs As Integer)
            Dim plan As (mfCount As Integer, inputs As Integer, scatter As Boolean) = ResolvePlan(cfg, requested)

            Return (plan.mfCount, plan.inputs)
        End Function

        ''' <summary>
        ''' 按规则划分方式与预算求出实际可行的结构。
        ''' </summary>
        ''' <param name="cfg">建模配置</param>
        ''' <param name="requested">期望的输入个数（通常为 <see cref="FuzzyGeneConfig.MaxRegulators"/>）</param>
        ''' <returns>
        ''' <c>(mfCount, inputs, scatter)</c>；<c>scatter = True</c> 表示走散射划分，
        ''' 此时 <c>inputs</c> 不会被 m^K 预算削减
        ''' </returns>
        ''' <remarks>
        ''' <para>
        ''' 网格划分（<see cref="RulePartitionKind.Grid"/>）保持原有降级顺序：先降模糊划分数、再削减输入。
        ''' </para>
        ''' <para>
        ''' <see cref="RulePartitionKind.Auto"/>：当配置的 K 装不进网格预算时**不再静默降 K**，
        ''' 而是返回 <c>scatter = True</c> 并保留原始输入个数——这正是 <c>maxRegulators = 18</c>
        ''' 能真正生效的前提（散射划分的规则数只由簇数决定，与输入维数无关）。
        ''' </para>
        ''' </remarks>
        Public Function ResolvePlan(cfg As FuzzyGeneConfig,
                                    requested As Integer) As (mfCount As Integer, inputs As Integer, scatter As Boolean)

            Dim mfCount As Integer = If(cfg Is Nothing, 3, cfg.MfCount)
            Dim n As Integer = std.Max(1, requested)

            If cfg IsNot Nothing AndAlso cfg.RulePartition = RulePartitionKind.Scatter Then
                Return (mfCount, n, True)
            End If

            If cfg IsNot Nothing AndAlso cfg.RulePartition = RulePartitionKind.Auto AndAlso ExceedsBudget(cfg, mfCount, n) Then
                Return (mfCount, n, True)
            End If

            Do While n > 1 AndAlso ExceedsBudget(cfg, mfCount, n)
                If mfCount > 2 Then
                    mfCount -= 1
                Else
                    n -= 1
                End If
            Loop

            Return (mfCount, n, False)
        End Function

        Public Function ExceedsBudget(cfg As FuzzyGeneConfig, mfCount As Integer, inputCount As Integer) As Boolean
            ' Double 幂运算：n 上限只有 MaxRegulators（通常 ≤ 6），不会真正溢出
            Dim rules As Double = std.Pow(mfCount, inputCount)

            If rules > cfg.MaxRules Then
                Return True
            End If
            If rules * (inputCount + 1) > cfg.MaxConsequentParams Then
                Return True
            End If

            Return False
        End Function

    End Module

End Namespace
