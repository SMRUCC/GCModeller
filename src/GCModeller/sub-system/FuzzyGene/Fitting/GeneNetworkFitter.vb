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
    ''' 为什么要**先**做预算降级：<see cref="TSK.TskNetworkBuilder.CreateGrid"/> 在
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
        Public Function Fit(view As GeneExpressionView,
                            prior As PriorNetwork,
                            config As FuzzyGeneConfig) As FuzzyGeneFitOutput

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
            Dim usedPicks(n - 1) As RegulatorPick()
            Dim weights(n - 1) As Double()

            Dim work As Action(Of Integer) =
                Sub(g)
                    Call FitOneGene(g, view, prior, cfg, models, metrics, usedPicks, weights)
                End Sub

            If cfg.DegreeOfParallelism = 1 OrElse n = 1 Then
                For g As Integer = 0 To n - 1
                    Call work(g)
                Next
            Else
                Dim options As New ParallelOptions()

                options.MaxDegreeOfParallelism = If(cfg.DegreeOfParallelism > 0, cfg.DegreeOfParallelism, -1)
                Call System.Threading.Tasks.Parallel.For(0, n, options, work)
            End If

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
                .GeneMetrics = metrics,
                .EdgeCount = edges.Count,
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
                view.Mode)

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
                               models As GeneRegulationModel(),
                               metrics As GeneFitMetric(),
                               usedPicks As RegulatorPick()(),
                               weights As Double()())

            Dim geneName As String = view.GeneNames(g)
            Dim metric As New GeneFitMetric With {.Gene = geneName, .Status = "skipped"}

            metrics(g) = metric

            If view.Constant(g) Then
                metric.Message = "constant expression"
                Return
            End If

            Dim picks As RegulatorPick() = RegulatorSelector.SelectRegulators(view, prior, cfg, g)

            If picks.Length = 0 Then
                metric.Message = $"no candidate regulator (selection={cfg.Selection}, min|r|={cfg.MinCorrelation:G3})"
                Return
            End If

            ' ---- 规则预算降级：先降模糊划分数，再逐个裁掉最弱的输入 ----
            Dim mfCount As Integer = cfg.MfCount
            Dim selected As New List(Of RegulatorPick)(picks)

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
            Dim yMatrix(rows - 1, 0) As Double
            Dim zMin As Double = Double.MaxValue
            Dim zMax As Double = Double.MinValue

            For r As Integer = 0 To rows - 1
                Dim row(regulatorIndex.Length - 1) As Double
                Dim state As Double() = view.StateRows(r)

                For j As Integer = 0 To regulatorIndex.Length - 1
                    row(j) = state(regulatorIndex(j))
                Next

                x(r) = row

                Dim target As Double = view.TargetRows(r)(g)

                y(r) = target
                yMatrix(r, 0) = target

                If target < zMin Then zMin = target
                If target > zMax Then zMax = target
            Next

            Dim options As AnfisOptions = cfg.Clone()

            options.MfCount = mfCount
            options.MfCounts = Nothing
            options.FeatureNames = regulators
            options.OutputNames = New String() {geneName}

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
                            model, x, yMatrix, cfg.PruneRuleThreshold, options)

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
                Return
            End Try

            ' ---- 触发强度与解析灵敏度（一次批量前向，避免逐样本重复前向） ----
            Dim ruleCount As Integer = model.Rules.RuleCount
            Dim nInput As Integer = model.InputCount
            Dim meanActivation(ruleCount - 1) As Double

            Try
                Dim layers As New TskLayers(model)
                Dim forward As TskInferenceTrace = layers.Forward(x, 0, rows)

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
                Return
            End Try

            Dim sensitivity(nInput - 1) As Double
            Dim theta As Double(,) = model.Theta

            For r As Integer = 0 To ruleCount - 1
                Dim w As Double = meanActivation(r)

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
                Dim evaluation As AnfisFitResult = AnfisFitting.Evaluate(model, x, yMatrix)

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
        End Sub

        ''' <summary>
        ''' 判断给定的结构是否超出规则/参数预算。
        ''' </summary>
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
