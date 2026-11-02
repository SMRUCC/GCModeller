Imports System
Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports SMRUCC.genomics.Analysis.GeneTree
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.BioSystem.Intervention
Imports std = System.Math

''' <summary>
''' GeneTree 的参数化演示程序：拓扑加载 → 表达矩阵加载 → 逐基因 XGBoost 拟合 →
''' 存档往返（Save/Load） → 虚拟敲除/过表达/下调 → 报告输出。
''' 
''' 所有的检查点、耗时与关键变量都通过 <see cref="DebugAudit"/> 记录，
''' 结束时写出一份 JSON 审计报告（默认 <c>{out}/geneTree.audit.json</c>）。
''' </summary>
Public Module GeneTreeDemo

    ''' <summary>
    ''' 运行演示
    ''' </summary>
    ''' <param name="args">命令行参数</param>
    ''' <returns>退出码：0 表示全部检查通过、2 表示参数错误、1 表示运行失败</returns>
    Public Function Run(args As String()) As Integer
        Dim opt As Options = Options.Parse(args)

        If opt.ShowHelp Then
            Call Console.WriteLine(Usage)
            Return 0
        End If
        If opt.Synthetic Then
            Return RunSynthetic(opt)
        End If
        If String.IsNullOrEmpty(opt.AracneZip) OrElse String.IsNullOrEmpty(opt.ExpressionCsv) Then
            Call Console.WriteLine("缺少必需参数 --aracne / --expr")
            Call Console.WriteLine()
            Call Console.WriteLine(Usage)
            Return 2
        End If

        If Not Directory.Exists(opt.OutputDir) Then
            Call Directory.CreateDirectory(opt.OutputDir)
        End If

        Call DebugAudit.Reset()

        DebugAudit.MaxEvents = 4096

        Dim auditFile As String = Path.Combine(opt.OutputDir, "geneTree.audit.json")
        Dim exitCode As Integer = 0

        Using DebugAudit.Watch("demo", logOnExit:=False)
            Try
                exitCode = runPipeline(opt)
            Catch ex As Exception
                Call DebugAudit.Mark("fatal", ex.ToString())
                Call Console.WriteLine()
                Call Console.WriteLine("执行过程中出现异常:")
                Call Console.WriteLine(ex.ToString())

                exitCode = 1
            Finally
                Call DebugAudit.SaveReport(auditFile)
                Call Console.WriteLine()
                Call Console.WriteLine($"审计报告已写入: {auditFile}（失败检查点: {DebugAudit.FailureCount}）")
            End Try
        End Using

        If exitCode = 0 AndAlso DebugAudit.FailureCount > 0 Then
            ' 断言失败不抛异常，但退出码要反映出来
            Return 1
        End If

        Return exitCode
    End Function

    Private Function runPipeline(opt As Options) As Integer
        Dim config As GeneTreeConfig = GeneTreeConfig.FromProfile(opt.Profile)

        config.PopulationSize = opt.Population
        config.MaxIterations = opt.Iterations
        config.Verbose = True

        If opt.Cells > 0 Then
            config.MaxSamplesPerModel = opt.Cells
        End If
        If opt.MaxGenes > 0 Then
            config.MaxGenes = opt.MaxGenes
        End If
        If opt.MaxRegulators >= 0 Then
            config.MaxRegulators = opt.MaxRegulators
        End If

        ' ---- 结构发现（可选）----
        If Not String.IsNullOrEmpty(opt.DiscoveryCandidatesFile) Then
            If Not File.Exists(opt.DiscoveryCandidatesFile) Then
                Throw New FileNotFoundException($"找不到候选基因清单: {opt.DiscoveryCandidatesFile}", opt.DiscoveryCandidatesFile)
            End If

            Dim candidates As String() = File.ReadAllLines(opt.DiscoveryCandidatesFile) _
                .Select(Function(line) If(line, "").Trim()) _
                .Where(Function(line) line.Length > 0 AndAlso Not line.StartsWith("#")) _
                .ToArray

            config.EdgeDiscovery = EdgeDiscoveryMode.ResidualTwoStage
            Call config.SetDiscoveryCandidates(candidates)
            config.DiscoveryMaxPerGene = opt.DiscoveryMaxPerGene

            If opt.DiscoveryMaxCandidates > 0 Then
                config.DiscoveryMaxCandidates = opt.DiscoveryMaxCandidates
            End If

            Call DebugAudit.Mark("discovery", $"candidates={candidates.Length}, max/gene={config.DiscoveryMaxPerGene}")
        End If

        Call config.ValidateOrThrow()

        ' ---- 1. 拓扑 ----
        Dim graph As GeneRegulationGraph = Nothing

        Using DebugAudit.Watch("load-topology")
            graph = GeneRegulationGraph.LoadFromArchive(opt.AracneZip, config)
        End Using

        Call DebugAudit.Mark("topology", graph.ToString)
        Call DebugAudit.Test(graph.TargetCount > 0, "ARACNE 拓扑之中没有得到任何靶基因")

        ' ---- 2. 表达矩阵 ----
        Dim expr As Matrix = Nothing

        Using DebugAudit.Watch("load-expression")
            expr = Matrix.LoadData(opt.ExpressionCsv)
        End Using

        Call DebugAudit.Mark("expression", $"{expr.size} genes x {expr.sample_count} samples")
        Call DebugAudit.Test(expr.size > 0 AndAlso expr.sample_count > 0, "表达矩阵为空")

        ' ---- 3. 拟合 ----
        Dim grn As XGBoostGRN = XGBoostGRN.Open(graph, expr, config)
        Dim report As TrainingReport = Nothing

        Using DebugAudit.Watch("fit")
            report = grn.Fit()
        End Using

        Call DebugAudit.Mark("fit", report.ToString)
        Call DebugAudit.Test(grn.IsFitted, "模型应当已经完成拟合")
        Call DebugAudit.Test(report.BoostedModels > 0, "应当至少训练出一个梯度提升树模型")
        Call DebugAudit.Test(report.InformativeModels > 0, "应当存在 R² > 0.05 的模型，否则说明网络拓扑与表达数据不匹配")

        If grn.Discovery IsNot Nothing Then
            Call DebugAudit.Mark("discovered-edges", grn.Discovery.ToString)
            Call DebugAudit.Test(grn.Discovery.Accepted >= 0, "结构发现的接受边数应当是非负数")
        End If

        Call report.Write(opt.OutputDir)
        Call Console.WriteLine($"训练报告已写出: {Path.Combine(opt.OutputDir, "report.tsv")}、" &
                              $"{Path.Combine(opt.OutputDir, "importance.tsv")}" &
                              If(grn.Discovery Is Nothing, "", $"、{Path.Combine(opt.OutputDir, "discovery.tsv")}"))

        ' ---- 4. 选择扰动目标 ----
        Dim gene As String = If(opt.Gene, PickTargetGene(grn))

        Call DebugAudit.Test(Not String.IsNullOrEmpty(gene), "没有找到可用的扰动目标基因")
        Call DebugAudit.Mark("perturbation-target", gene)

        If String.IsNullOrEmpty(gene) Then
            Return 1
        End If

        ' ---- 5. 存档往返 ----
        Dim saved As String = Nothing

        If Not String.IsNullOrEmpty(opt.SavePath) Then
            Using DebugAudit.Watch("save")
                Using stream As Stream = File.Create(opt.SavePath)
                    Call grn.Save(stream)
                End Using
            End Using

            saved = opt.SavePath
            Call DebugAudit.Mark("saved", saved)

            Using DebugAudit.Watch("reload")
                Dim reloaded As XGBoostGRN = XGBoostGRN.Load(opt.SavePath)

                Call DebugAudit.Test(reloaded.IsFitted, "从存档加载的模型应当处于已拟合状态")
                Call DebugAudit.AreEqual(grn.GeneNames.Length, reloaded.GeneNames.Length,
                                         "基因数量在存档往返之后应当保持一致")
                Call DebugAudit.AreEqual(grn.WildtypeMeans.Length, reloaded.WildtypeMeans.Length,
                                         "基线长度在存档往返之后应当保持一致")

                Dim before As InterventionResult = grn.KnockoutGene(gene)
                Dim after As InterventionResult = reloaded.KnockoutGene(gene)
                Dim diff As Double = maxAbsDiff(before.FoldChanges, after.FoldChanges)

                Call DebugAudit.Mark("roundtrip-diff", diff)
                Call DebugAudit.Test(diff < 0.000001R, $"存档往返之后扰动结果应当一致（最大差异 {diff:G4}）")
            End Using
        End If

        ' ---- 6. 虚拟扰动 ----
        Dim ko As InterventionResult = Nothing
        Dim oe As InterventionResult = Nothing
        Dim kd As InterventionResult = Nothing

        Using DebugAudit.Watch("knockout")
            ko = grn.KnockoutGene(gene, opt.BaselineCells)
        End Using

        Using DebugAudit.Watch("overexpress")
            oe = grn.OverexpressGene(gene, opt.BaselineCells)
        End Using

        Using DebugAudit.Watch("knockdown")
            kd = grn.KnockDownGene(gene, opt.BaselineCells)
        End Using

        Call DebugAudit.Test(ko IsNot Nothing AndAlso Not ko.Undefined, "敲除结果不应为 Undefined")
        Call DebugAudit.Mark("knockout", ko.ToString)
        Call DebugAudit.Test(ko.NAffected > 0, "敲除之后应当至少有一个基因发生显著变化")

        If grn.LastSolve IsNot Nothing Then
            Call DebugAudit.Mark("solver", grn.LastSolve.ToString)
            Call DebugAudit.Test(grn.LastSolve.Iterations > 0, "求解器应当至少执行一轮迭代")
        End If

        Call ko.WriteInterventionResult(Path.Combine(opt.OutputDir, $"knockout-{gene}.tsv"))
        Call oe.WriteInterventionResult(Path.Combine(opt.OutputDir, $"overexpress-{gene}.tsv"))
        Call kd.WriteInterventionResult(Path.Combine(opt.OutputDir, $"knockdown-{gene}.tsv"))

        ' ---- 6.5 表达状态上下文：把目标基因的表达压低到 50% 之后再做扰动 ----
        Using DebugAudit.Watch("context")
            Dim idx As Integer = Array.IndexOf(grn.GeneNames, gene)
            Dim context As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase) From {
                {gene, grn.WildtypeMeans(idx) * 0.5R}
            }

            Call grn.SetContext(context, fix:=True)
            Call DebugAudit.Mark("context", $"{grn.Context.Count} gene(s), fix={grn.ContextFix}, " &
                                            $"aligned={grn.ContextAligned}, clamp={grn.ContextClampGenes}")
            Call DebugAudit.Mark("context-note", If(grn.ContextNote, "<none>"))

            Dim ctxResult As InterventionResult = grn.KnockoutGene(gene)

            Call DebugAudit.Test(ctxResult IsNot Nothing AndAlso Not ctxResult.Undefined,
                                 "设置表达状态上下文之后仍然应当能够执行虚拟扰动")
            Call ctxResult.WriteInterventionResult(Path.Combine(opt.OutputDir, $"knockout-{gene}-context.tsv"))

            ' 控制稳态（同一 context 下不做任何扰动的稳态）同时也是差异分析的基线
            Dim control As Double() = grn.ControlSteadyState

            Call DebugAudit.Test(control IsNot Nothing AndAlso control.Length = grn.GeneNames.Length,
                                 "控制稳态的维度应当与基因数量一致")

            Call grn.ClearContext()
        End Using

        ' ---- 7. 控制稳态（诊断树模型自身的漂移） ----
        Using DebugAudit.Watch("control-steady-state")
            Dim control As Double() = grn.ControlSteadyState
            Dim drift As Double = maxAbsDiff(control, grn.WildtypeMeans)

            Call DebugAudit.Mark("control-drift", drift)
            Call DebugAudit.Test(Double.IsNaN(drift) = False, "控制稳态的漂移量应当是有效数值")
        End Using

        Call Console.WriteLine()
        Call Console.WriteLine(ko.ToString())
        Call Console.WriteLine()
        Call Console.WriteLine($"结果目录: {opt.OutputDir}{If(saved Is Nothing, "", "（模型已保存到 " & saved & "）")}")

        Return 0
    End Function

    ''' <summary>
    ''' 挑选一个“扰动效应最容易传播”的基因：优先转录因子，其次 R² 高、调控者多的基因
    ''' </summary>
    ''' <summary>
    ''' 合成数据自检：构造一个“真值已知”的非线性调控网络，其中每个靶基因除了 ARACNE 拓扑给出的
    ''' 一个调控者之外，还额外受到一个**隐藏调控者**的影响；随后用残差二阶段发现去把它找回来。
    ''' </summary>
    ''' <param name="opt">命令行参数（只需要 OutputDir）</param>
    ''' <returns>退出码：0 表示全部检查通过</returns>
    Private Function RunSynthetic(opt As Options) As Integer
        If Not Directory.Exists(opt.OutputDir) Then
            Call Directory.CreateDirectory(opt.OutputDir)
        End If

        Call DebugAudit.Reset()

        Const nTf As Integer = 12
        Const nHidden As Integer = 8
        Const nTarget As Integer = 120
        Const nDecoy As Integer = 12
        Const nCells As Integer = 900

        Dim rnd As New Random(20261102)
        Dim genes As New List(Of String)
        Dim tfFlags As New List(Of Boolean)
        Dim values As New Dictionary(Of String, Double())(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To nTf - 1
            Call genes.Add($"TF{i:D2}")
            Call tfFlags.Add(True)
            values($"TF{i:D2}") = gauss(rnd, nCells)
        Next
        For i As Integer = 0 To nHidden - 1
            Call genes.Add($"HID{i:D2}")
            Call tfFlags.Add(False)
            values($"HID{i:D2}") = gauss(rnd, nCells)
        Next
        For i As Integer = 0 To nDecoy - 1
            Call genes.Add($"DECOY{i:D2}")
            Call tfFlags.Add(False)
            values($"DECOY{i:D2}") = gauss(rnd, nCells)
        Next

        ' ---- 真值网络 ----
        Dim edges As New List(Of RegulationInput)
        Dim truth As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        For t As Integer = 0 To nTarget - 1
            Dim target As String = $"TG{t:D3}"
            Dim given As String = $"TF{rnd.Next(0, nTf):D2}"
            Dim hidden As String = $"HID{rnd.Next(0, nHidden):D2}"
            Dim y As Double() = New Double(nCells - 1) {}

            For c As Integer = 0 To nCells - 1
                ' 非线性：已知调控者走正弦、隐藏调控者走 tanh，外加噪声
                y(c) = 2.0R * std.Sin(1.5R * values(given)(c)) +
                       1.5R * std.Tanh(1.2R * values(hidden)(c)) +
                       0.1R * gauss1(rnd)
            Next

            values(target) = y
            truth(target) = hidden

            Call genes.Add(target)
            Call tfFlags.Add(False)
            Call edges.Add(New RegulationInput With {
                .target = target,
                .regulator = given,
                .mi = 0.5R,
                .correlation = 0.4R,
                .interaction = "directed"
            })
        Next

        Dim samples As String() = Enumerable.Range(0, nCells).Select(Function(i) $"cell{i:D4}").ToArray
        Dim rows As DataFrameRow() = genes _
            .Select(Function(g) New DataFrameRow(g, values(g))) _
            .ToArray
        Dim expr As New Matrix With {
            .sampleID = samples,
            .expression = rows
        }

        ' ---- 训练 + 发现 ----
        Dim config As GeneTreeConfig = GeneTreeConfig.CreateFast()

        config.MaxRegulators = 5
        config.NumBoostRound = 80
        config.MaxDepth = 4
        config.Verbose = True
        config.EdgeDiscovery = EdgeDiscoveryMode.ResidualTwoStage
        Call config.SetDiscoveryCandidates(genes.Where(Function(g) Not g.StartsWith("TG", StringComparison.OrdinalIgnoreCase)))
        config.DiscoveryMaxPerGene = 1
        config.DiscoveryMaxCandidates = 4
        config.DiscoveryMinCorrelation = 0.05R
        config.DiscoveryMinDeltaR2 = 0.002R

        Call config.ValidateOrThrow()

        Dim graph As GeneRegulationGraph = GeneRegulationGraph.FromRegulations(genes.ToArray, tfFlags.ToArray, edges, config)
        Dim grn As XGBoostGRN = Nothing
        Dim report As TrainingReport = Nothing

        Using DebugAudit.Watch("synthetic-fit")
            grn = XGBoostGRN.Open(graph, expr, config)
            report = grn.Fit()
        End Using

        Call DebugAudit.Mark("synthetic-report", report.ToString)
        Call report.Write(opt.OutputDir)

        ' ---- 检查点 ----
        Call DebugAudit.Test(grn.Discovery IsNot Nothing, "开启发现模式之后应当产生发现报告")

        If grn.Discovery Is Nothing Then
            Return 1
        End If

        Dim accepted As DiscoveryEdge() = grn.Discovery.Edges.Where(Function(e) e.isAccepted).ToArray
        Dim hits As Integer = accepted.Count(Function(e) truth.ContainsKey(e.target) AndAlso
                                                         String.Equals(truth(e.target), e.regulator, StringComparison.OrdinalIgnoreCase))

        Call DebugAudit.Mark("synthetic-discovery", grn.Discovery.ToString)
        Call DebugAudit.Mark("synthetic-truth", $"{hits}/{accepted.Length} 条被接受的边就是隐藏调控者")

        Call DebugAudit.Test(accepted.Length >= nTarget * 0.6,
                             $"应当至少找回 60% 的隐藏调控边，实际接受 {accepted.Length}/{nTarget}")
        Call DebugAudit.Test(accepted.Length > 0 AndAlso hits >= accepted.Length * 0.6,
                             $"被接受的边之中至少 60% 应当是真正的隐藏调控者，实际 {hits}/{accepted.Length}")
        Call DebugAudit.Test(accepted.Length = 0 OrElse accepted.All(Function(e) e.deltaR2 > 0),
                             "被接受的边都应当带来正的 R² 增益")

        Call Console.WriteLine()
        Call Console.WriteLine($"合成数据自检: 接受 {accepted.Length}/{nTarget} 条新边，其中 {hits} 条命中隐藏调控者")
        Call Console.WriteLine($"报告: {Path.Combine(opt.OutputDir, "discovery.tsv")}")

        Call DebugAudit.SaveReport(Path.Combine(opt.OutputDir, "geneTree.synthetic.audit.json"))

        Return If(DebugAudit.FailureCount = 0, 0, 1)
    End Function

    ''' <summary>
    ''' 生成标准正态随机向量
    ''' </summary>
    Private Function gauss(rnd As Random, n As Integer) As Double()
        Dim result As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            result(i) = gauss1(rnd)
        Next

        Return result
    End Function

    ''' <summary>
    ''' Box-Muller 生成一个标准正态随机数
    ''' </summary>
    Private Function gauss1(rnd As Random) As Double
        Dim u1 As Double = 1.0R - rnd.NextDouble()
        Dim u2 As Double = rnd.NextDouble()

        Return std.Sqrt(-2.0R * std.Log(u1)) * std.Cos(2.0R * std.PI * u2)
    End Function

    Private Function PickTargetGene(grn As SMRUCC.genomics.Analysis.GeneTree.XGBoostGRN) As String
        Dim tfs As New HashSet(Of String)(grn.TFNames, StringComparer.OrdinalIgnoreCase)
        Dim best As String = Nothing
        Dim bestScore As Double = Double.MinValue

        For i As Integer = 0 To grn.Models.Length - 1
            Dim model As GeneModel = grn.Models(i)

            If model Is Nothing OrElse model.Kind <> "xgboost" Then
                Continue For
            End If

            Dim score As Double = model.Performance.r2 +
                If(tfs.Contains(model.Gene), 1.0R, 0.0R) +
                model.Regulators.Length * 0.01R

            If score > bestScore Then
                bestScore = score
                best = model.Gene
            End If
        Next

        Return best
    End Function

    Private Function maxAbsDiff(a As Double(), b As Double()) As Double
        If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length Then
            Return Double.NaN
        End If

        Dim result As Double = 0.0R

        For i As Integer = 0 To a.Length - 1
            Dim d As Double = std.Abs(a(i) - b(i))

            If d > result Then
                result = d
            End If
        Next

        Return result
    End Function

    ''' <summary>
    ''' 命令行参数
    ''' </summary>
    Private Class Options

        Public Property AracneZip As String
        Public Property ExpressionCsv As String
        Public Property OutputDir As String = "./GeneTree-demo"
        Public Property SavePath As String
        Public Property Gene As String
        Public Property Profile As GeneTreeProfile = GeneTreeProfile.Fast
        Public Property Population As Integer = 100
        Public Property Iterations As Integer = 60
        Public Property Cells As Integer = 0
        Public Property MaxGenes As Integer = 0
        Public Property MaxRegulators As Integer = -1
        Public Property BaselineCells As Integer = 0
        Public Property ShowHelp As Boolean = False

        ''' <summary>候选调控者清单文件（每行一个基因编号）；指定后开启残差二阶段结构发现</summary>
        Public Property DiscoveryCandidatesFile As String
        Public Property DiscoveryMaxPerGene As Integer = 2
        Public Property DiscoveryMaxCandidates As Integer = 0

        ''' <summary>使用合成数据自检结构发现（不需要 aracne.zip 与表达矩阵）</summary>
        Public Property Synthetic As Boolean = False

        ''' <summary>
        ''' 指定了 <c>--save</c> 但是没有给出路径时，使用输出目录下的默认存档名
        ''' </summary>
        Public Property DefaultSavePath As Boolean = False

        Public Shared Function Parse(args As String()) As Options
            Dim opt As New Options

            If args Is Nothing OrElse args.Length = 0 Then
                opt.ShowHelp = True
                Return opt
            End If

            Dim i As Integer = 0

            While i < args.Length
                Dim key As String = If(args(i), "").Trim()

                Select Case key.ToLower()
                    Case "--aracne"
                        i += 1 : opt.AracneZip = nextArg(args, i)
                    Case "--expr"
                        i += 1 : opt.ExpressionCsv = nextArg(args, i)
                    Case "--out"
                        i += 1 : opt.OutputDir = nextArg(args, i)
                    Case "--save"
                        If i + 1 < args.Length AndAlso Not If(args(i + 1), "").Trim().StartsWith("--") Then
                            i += 1
                            opt.SavePath = nextArg(args, i)
                        Else
                            opt.DefaultSavePath = True
                        End If
                    Case "--gene"
                        i += 1 : opt.Gene = nextArg(args, i)
                    Case "--preset"
                        i += 1
                        Dim text As String = nextArg(args, i)
                        Dim profile As GeneTreeProfile

                        If GeneTreeConfig.TryParseProfile(text, profile) Then
                            opt.Profile = profile
                        Else
                            Throw New ArgumentException($"无法识别的预设档 '{text}'，可选值为 fast / balanced / full")
                        End If
                    Case "--population"
                        i += 1 : opt.Population = Integer.Parse(nextArg(args, i))
                    Case "--iterations"
                        i += 1 : opt.Iterations = Integer.Parse(nextArg(args, i))
                    Case "--cells"
                        i += 1 : opt.Cells = Integer.Parse(nextArg(args, i))
                    Case "--max-genes"
                        i += 1 : opt.MaxGenes = Integer.Parse(nextArg(args, i))
                    Case "--max-regulators"
                        i += 1 : opt.MaxRegulators = Integer.Parse(nextArg(args, i))
                    Case "--baseline-cells"
                        i += 1 : opt.BaselineCells = Integer.Parse(nextArg(args, i))
                    Case "--synthetic"
                        opt.Synthetic = True
                    Case "--discovery-candidates"
                        i += 1 : opt.DiscoveryCandidatesFile = nextArg(args, i)
                    Case "--discovery-max"
                        i += 1 : opt.DiscoveryMaxPerGene = Integer.Parse(nextArg(args, i))
                    Case "--discovery-try"
                        i += 1 : opt.DiscoveryMaxCandidates = Integer.Parse(nextArg(args, i))
                    Case "--no-save"
                        opt.SavePath = Nothing
                        opt.DefaultSavePath = False
                    Case "--help", "-h", "/?"
                        opt.ShowHelp = True
                    Case Else
                        Throw New ArgumentException($"无法识别的参数 '{key}'")
                End Select

                i += 1
            End While

            If opt.DefaultSavePath Then
                opt.SavePath = Path.Combine(opt.OutputDir, "genetree.zip")
            End If

            Return opt
        End Function

        Private Shared Function nextArg(args As String(), i As Integer) As String
            If i >= args.Length Then
                Throw New ArgumentException("命令行参数缺少取值")
            Else
                Return args(i)
            End If
        End Function

    End Class

    ''' <summary>
    ''' 命令行用法说明
    ''' </summary>
    Public ReadOnly Property Usage As String
        Get
            Dim sb As New Text.StringBuilder()

            Call sb.AppendLine("GeneTree: 基于 XGBoost 的基因调控网络虚拟扰动实验")
            Call sb.AppendLine()
            Call sb.AppendLine("用法:")
            Call sb.AppendLine("  GeneTreeDemo --aracne <aracne.zip> --expr <expression.csv> [选项]")
            Call sb.AppendLine()
            Call sb.AppendLine("必需参数:")
            Call sb.AppendLine("  --aracne <file>        ARACNE 模型压缩包（只读取拓扑，不会加载存档里的表达矩阵）")
            Call sb.AppendLine("  --expr <file>          原始基因表达矩阵 CSV（行=基因、列=样本）")
            Call sb.AppendLine()
            Call sb.AppendLine("可选参数:")
            Call sb.AppendLine("  --out <dir>            输出目录（默认 ./GeneTree-demo）")
            Call sb.AppendLine("  --preset <name>        训练规模预设 fast|balanced|full（默认 fast）")
            Call sb.AppendLine("  --cells <n>            每个模型最多使用的细胞数（覆盖预设）")
            Call sb.AppendLine("  --max-genes <n>        最多训练多少个靶基因（覆盖预设）")
            Call sb.AppendLine("  --max-regulators <n>   每个靶基因保留的调控因子数量（覆盖预设）")
            Call sb.AppendLine("  --population <n>       迭代求解使用的细胞数量（默认 100）")
            Call sb.AppendLine("  --iterations <n>       定点迭代的最大轮数（默认 60）")
            Call sb.AppendLine("  --baseline-cells <n>   接口方法的 nSamples 参数（0 表示使用整个种群）")
            Call sb.AppendLine("  --gene <name>          指定扰动目标基因（默认自动挑选转录因子）")
            Call sb.AppendLine("  --discovery-candidates <file>  候选调控者清单（每行一个基因），指定后开启残差二阶段结构发现")
            Call sb.AppendLine("  --discovery-max <n>    每个靶基因最多接受几条新边（默认 2）")
            Call sb.AppendLine("  --discovery-try <n>    每个靶基因最多尝试几个候选（默认 5）")
            Call sb.AppendLine("  --save [file]          训练完成后写模型存档（不给路径时写到 {out}/genetree.zip）")
            Call sb.AppendLine("  --no-save              不写模型存档（默认行为）")
            Call sb.AppendLine("  --help                 显示本帮助")
            Call sb.AppendLine()
            Call sb.Append("说明：审计报告会写入 {out}/geneTree.audit.json，训练报告会写入 {out}/report.tsv 与 {out}/importance.tsv")

            Return sb.ToString()
        End Get
    End Property

End Module
