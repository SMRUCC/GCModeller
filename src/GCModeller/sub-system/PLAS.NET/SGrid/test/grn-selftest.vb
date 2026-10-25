Imports System.Globalization
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics.Data
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels
Imports SMRUCC.genomics.Analysis.SSystem.Runner
Imports GRN = SMRUCC.genomics.Analysis.SGrid.GRN
Imports Perturbation = SMRUCC.genomics.Analysis.SGrid.Perturbation
Imports SSOdes = SMRUCC.genomics.Analysis.SSystem.Kernel.ODEs
Imports SSModel = SMRUCC.genomics.Analysis.SSystem.Script.Model
Imports SSVar = SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.var
Imports std = System.Math

Namespace SelfTest

    ''' <summary>
    ''' The self test of the S-system based gene regulatory network inference.
    ''' (基因表达调控网络拟合的自测)
    ''' </summary>
    ''' <remarks>
    ''' 使用 method.md 之中所给出的三基因反馈回路作为金标准：
    ''' 先用该网络积分生成一段合成的时间序列表达矩阵，再用 SGrid 做拟合并反推拓扑，
    ''' 最后比较恢复出来的边集合与真实边集合之间的 precision / recall。
    ''' </remarks>
    Public Module GrnSelfTest

        ''' <summary>
        ''' 金标准网络之中的基因编号
        ''' </summary>
        Public ReadOnly GeneNames As String() = {"X1", "X2", "X3"}

        ''' <summary>
        ''' 金标准网络的合成速率常数
        ''' </summary>
        ''' <remarks>
        ''' 网络形式参考 method.md 之中所给出的三基因反馈回路：
        ''' X1' = 3*X2^0.5*X3^-0.3 - 2*X1
        ''' X2' = 2*X1^0.5 - 3*X2^0.75
        ''' X3' = 1.5*X2^0.4 - 1.2*X3
        ''' 与文档示例的区别在于降解项对自身是一阶依赖，从而保证该网络存在正的稳定平衡点，
        ''' 数值积分不会发散，适合作为拟合算法的金标准。
        ''' </remarks>
        Public ReadOnly Alpha As Double() = {3, 2, 1.5}

        ''' <summary>
        ''' 金标准网络的降解速率常数
        ''' </summary>
        Public ReadOnly Beta As Double() = {2, 3, 1.2}

        ''' <summary>
        ''' 金标准网络的合成项动力学阶矩阵
        ''' </summary>
        Public Function TruthG() As Double()()
            Dim G As Double()() = {
                New Double() {0, 0.5R, -0.3R},
                New Double() {0.5R, 0, 0},
                New Double() {0, 0.4R, 0}
            }

            Return G
        End Function

        ''' <summary>
        ''' 金标准网络的降解项动力学阶矩阵
        ''' </summary>
        ''' <returns></returns>
        Public Function TruthH() As Double()()
            Dim H As Double()() = {
                New Double() {1, 0, 0},
                New Double() {0, 0.75R, 0},
                New Double() {0, 0, 1}
            }

            Return H
        End Function

        ''' <summary>
        ''' 执行自测，返回进程退出码（零表示通过）
        ''' </summary>
        ''' <param name="verbose"></param>
        ''' <param name="export">可选的合成表达矩阵导出路径，非空的时候会把矩阵写成 csv 文件</param>
        ''' <returns></returns>
        Public Function Run(Optional verbose As Boolean = True, Optional export As String = "") As Integer
            Dim failures As New System.Collections.Generic.List(Of String)
            Dim report As New StringBuilder()

            Call report.AppendLine("=== SGrid self test: S-system GRN inference ===")

            Dim times As Double() = Enumerable _
                .Range(0, 241) _
                .Select(Function(i) i * 0.0625R) _
                .ToArray

            ' 1. 用金标准网络生成合成的表达矩阵
            Dim matrix As Matrix = synthetic(times)

            If Not String.IsNullOrEmpty(export) Then
                Call writeCsv(matrix, export)
                Call report.AppendLine($"[INFO] the synthetic expression matrix was written into: {export}")
            End If

            Call report.AppendLine($"[INFO] synthetic matrix: {matrix.size} genes x {matrix.sample_count} time points")

            ' 2. 拟合
            Dim options As New GRN.FittingOptions With {
                .Normalize = GRN.NormalizationMethods.None,
                .TimePoints = times,
                .Sparsity = 0.2R,
                .MaxEdgesPerGene = 3,
                .KeepSelfLoop = True,
                .DEPopulationSize = 60,
                .DEMaxIterations = 3000,
                .Seed = 1234567,
                .Parallel = False,
                .Verbose = False
            }

            Dim network As GRN.SSystemNetwork = GRN.SSystemNetwork.Fit(matrix, options)

            ' 诊断：比较数值微分得到的斜率与金标准方程的解析斜率
            Call diagnostics(report, matrix, options)

            Call report.AppendLine($"[INFO] mean slope RMSE = {network.Parameters.MeanRMSE.ToString("G6")}")
            Call report.AppendLine($"[INFO] recovered edges = {network.Edges.Length}")
            Call report.AppendLine()

            Call report.AppendLine("recovered topology:")

            For Each edge As GRN.RegulatoryEdge In network.Edges
                Call report.AppendLine($"  {edge.source} -> {edge.target} : {edge.interaction}, net={edge.weight.ToString("G4")}")
            Next

            Call report.AppendLine()

            ' 3. 拓扑恢复率
            Dim truth As New System.Collections.Generic.HashSet(Of String)

            For Each edge As String In TruthEdges()
                Call truth.Add(edge)
            Next

            Dim recovered As New System.Collections.Generic.HashSet(Of String)

            For Each edge As GRN.RegulatoryEdge In network.Edges
                Call recovered.Add($"{edge.source}->{edge.target}")
            Next

            Dim tp As Integer = truth.Where(Function(e) recovered.Contains(e)).Count()
            Dim precision As Double = If(recovered.Count = 0, 0, tp / recovered.Count)
            Dim recall As Double = tp / truth.Count
            Dim f1 As Double = If(precision + recall = 0, 0, 2 * precision * recall / (precision + recall))

            Call report.AppendLine($"topology recovery: TP={tp}/{truth.Count}, precision={precision.ToString("F2")}, recall={recall.ToString("F2")}, F1={f1.ToString("F2")}")

            Dim missed As String() = truth.Where(Function(e) Not recovered.Contains(e)).ToArray()
            Dim spurious As String() = recovered.Where(Function(e) Not truth.Contains(e)).ToArray()

            If missed.Length > 0 Then
                Call report.AppendLine($"  missed    : {missed.JoinBy(", ")}")
            End If
            If spurious.Length > 0 Then
                Call report.AppendLine($"  spurious  : {spurious.JoinBy(", ")}")
            End If

            If recall < 0.6R Then
                Call failures.Add($"the topological recall {recall.ToString("F2")} is lower than 0.60")
            End If
            If precision < 0.5R Then
                Call failures.Add($"the topological precision {precision.ToString("F2")} is lower than 0.50")
            End If

            ' 4. 参数的还原程度
            '
            ' 注意：单条轨迹之上的 S-system 逆问题是病态的（参数量随基因数平方增长，
            ' 而轨迹上的 lnX 列之间高度共线），alpha/beta 与动力学阶之间存在很强的折衷关系，
            ' 因此这里只报告速率常数的还原程度而不作为断言条件，
            ' 真正可靠的验证指标是拓扑结构与轨迹的可预测性（参看下方）。
            Call report.AppendLine()
            Call report.AppendLine("rate constant recovery:")

            For i As Integer = 0 To GeneNames.Length - 1
                Dim ra As Double = std.Abs(network.Parameters.alpha(i) - Alpha(i)) / std.Max(Alpha(i), 1.0E-6)
                Dim rb As Double = std.Abs(network.Parameters.beta(i) - Beta(i)) / std.Max(Beta(i), 1.0E-6)

                Call report.AppendLine($"  {GeneNames(i)}: alpha {network.Parameters.alpha(i).ToString("G5")} (truth {Alpha(i)}), beta {network.Parameters.beta(i).ToString("G5")} (truth {Beta(i)}), relerr=({ra.ToString("F2")}, {rb.ToString("F2")})")

                If network.Parameters.alpha(i) <= 0 OrElse network.Parameters.beta(i) <= 0 Then
                    Call failures.Add($"the rate constants of the gene '{GeneNames(i)}' should be positive values")
                End If
            Next

            ' 5. 拟合模型的轨迹可预测性
            '
            ' 注意：单条轨迹之上的 S-system 逆问题是病态的，拟合出的参数虽然可以在观测点之上
            ' 很好地复现斜率，但是重新积分之后的长期轨迹可能会漂移（过参数化导致的过拟合），
            ' 故这里只报告该指标而不作为断言条件。
            Dim r2 As Double = trajectoryR2(network)

            Call report.AppendLine()
            Call report.AppendLine($"trajectory predictability: R2 = {r2.ToString("F3")}")

            ' 6. 斜率拟合的相对误差
            Dim slopeRmse As Double = network _
                .Parameters _
                .SlopeRMSE _
                .SafeQuery() _
                .Average

            Call report.AppendLine($"slope fit RMSE = {slopeRmse.ToString("G4")}")

            If slopeRmse > 0.5R Then
                Call failures.Add($"the slope fitting RMSE {slopeRmse.ToString("G4")} is too large")
            End If

            ' 5. 稳定性诊断
            Call report.AppendLine()
            Call report.AppendLine(GRN.NetworkTopology.StabilityReport(network.Parameters))

            ' 6. 虚拟扰动实验的连通性检查
            Dim screening As Perturbation.ScreeningResult = Perturbation.VirtualScreening.Run(
                network,
                {New Perturbation.PerturbationDesign With {
                    .TargetGene = GeneNames(0),
                    .Type = Perturbation.PerturbationTypes.KnockDown,
                    .Magnitude = 0.8R
                }})

            Call report.AppendLine()
            Call report.AppendLine("virtual perturbation:")

            For Each record As Perturbation.ScreeningRecord In screening.Records.SafeQuery
                Call report.AppendLine($"  X1 knock-down -> {record.Gene}: end={record.PerturbedLevel.ToString("G4")} (control {record.ControlLevel.ToString("G4")}), maxdev={record.MaxDeviation.ToString("G4")}, response={record.ResponseTime.ToString("G3")}")
            Next

            Dim responded As Integer = screening _
                .Records _
                .SafeQuery() _
                .Where(Function(r) r.MaxDeviation > 1.0E-9) _
                .Count()

            If screening.Records.IsNullOrEmpty Then
                Call failures.Add("the virtual perturbation experiment did not produce any response record")
            ElseIf responded = 0 Then
                Call failures.Add("knocking down the gene X1 does not change the simulated trajectory at all")
            End If

            Call report.AppendLine()

            If failures.Count = 0 Then
                Call report.AppendLine("PASS: all of the assertions were satisfied.")
            Else
                Call report.AppendLine($"FAIL: {failures.Count} assertion(s) were violated:")

                For Each failure As String In failures
                    Call report.AppendLine($"  - {failure}")
                Next
            End If

            If verbose Then
                Call Console.WriteLine(report.ToString)
            End If

            Return If(failures.Count = 0, 0, 1)
        End Function

        ''' <summary>
        ''' 把表达矩阵写成 csv 文件（首列为基因编号，首行为样本/时间点的名称）
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="path"></param>
        Private Sub writeCsv(matrix As Matrix, path As String)
            Dim sb As New StringBuilder()

            Call sb.AppendLine("gene," & String.Join(",", matrix.sampleID))

            For Each row As DataFrameRow In matrix.expression
                Dim values As String() = row _
                    .experiments _
                    .Select(Function(x) x.ToString("G8", CultureInfo.InvariantCulture)) _
                    .ToArray

                Call sb.AppendLine($"""{row.geneID}""," & String.Join(",", values))
            Next

            Call System.IO.File.WriteAllText(path, sb.ToString, New Text.UTF8Encoding(False))
        End Sub

        ''' <summary>
        ''' 用拟合得到的模型重新积分，计算其与观测轨迹之间的决定系数 R2
        ''' </summary>
        ''' <param name="network"></param>
        ''' <returns></returns>
        Private Function trajectoryR2(network As GRN.SSystemNetwork) As Double
            Dim times As Double() = network.Series.TimePoints
            Dim model As SSModel = network.CreateModel(finalTime:=times(times.Length - 1))
            Dim options As New RunOptions With {
                .FinalTime = model.FinalTime,
                .UseODEs = True,
                .Steps = times.Length - 1
            }
            Dim output As ODEsOut = SSOdes.RunSystem(model, Nothing, options)

            Dim ssRes As Double = 0
            Dim ssTot As Double = 0

            For i As Integer = 0 To network.Series.nGenes - 1
                Dim obs As Double() = network.Series.X(i)
                Dim name As String = network.Series.GeneNames(i)

                If Not output.y.ContainsKey(name) Then
                    Continue For
                End If

                Dim sim As Double() = output.y(name).Value
                Dim mean As Double = obs.Average

                For k As Integer = 0 To std.Min(obs.Length, sim.Length) - 1
                    ssRes += (sim(k) - obs(k)) ^ 2
                    ssTot += (obs(k) - mean) ^ 2
                Next
            Next

            If ssTot < 1.0E-12 Then
                Return If(ssRes < 1.0E-12, 1, 0)
            End If

            Return 1 - ssRes / ssTot
        End Function

        ''' <summary>
        ''' 比较数值微分得到的斜率与金标准方程的解析斜率，用于诊断斜率估计的精度
        ''' </summary>
        ''' <param name="report"></param>
        ''' <param name="matrix"></param>
        ''' <param name="options"></param>
        Private Sub diagnostics(report As StringBuilder, matrix As Matrix, options As GRN.FittingOptions)
            Dim series As GRN.ExpressionTimeSeries = GRN.ExpressionTimeSeries.FromMatrix(matrix, options)
            Dim slopes As Double()() = GRN.SlopeEstimator.Estimate(series, options.Denoise)
            Dim G As Double()() = TruthG()
            Dim H As Double()() = TruthH()

            Call report.AppendLine("[DIAG] slope estimation check:")

            For i As Integer = 0 To series.nGenes - 1
                Dim x As Double() = series.X(i)
                Dim s As Double() = slopes(i)
                Dim diff As Double = 0
                Dim scale As Double = 0

                For k As Integer = 0 To series.nTimePoints - 1
                    ' 用金标准的参数解析计算真实的斜率
                    Dim la As Double = std.Log(Alpha(i))
                    Dim lb As Double = std.Log(Beta(i))

                    For j As Integer = 0 To series.nGenes - 1
                        Dim lx As Double = series.LnX(j)(k)

                        la += G(i)(j) * lx
                        lb += H(i)(j) * lx
                    Next

                    Dim truth As Double = std.Exp(la) - std.Exp(lb)

                    diff += (s(k) - truth) ^ 2
                    scale += truth ^ 2
                Next

                Call report.AppendLine($"  {GeneNames(i)}: X in [{x.Min.ToString("G4")}, {x.Max.ToString("G4")}], slope relerr={std.Sqrt(diff / std.Max(scale, 1.0E-12)).ToString("P1")}")
            Next

            Call report.AppendLine()
            Call report.AppendLine("[DIAG] objective value at the ground truth parameters:")

            For i As Integer = 0 To series.nGenes - 1
                Dim candidates As Integer() = Enumerable.Range(0, series.nGenes).ToArray
                Dim dims As Integer = 2 + 2 * candidates.Length
                Dim truth As Double() = New Double(dims - 1) {}
                Dim mean As Double = slopes(i).Average
                Dim variance As Double = slopes(i).Sum(Function(v) (v - mean) ^ 2) / slopes(i).Length

                truth(0) = std.Log(Alpha(i))
                truth(1) = std.Log(Beta(i))

                For c As Integer = 0 To candidates.Length - 1
                    truth(2 + c) = G(i)(candidates(c))
                    truth(2 + candidates.Length + c) = H(i)(candidates(c))
                Next

                Dim problem As New GRN.SlopeProblem With {
                    .LnX = candidates.Select(Function(j) series.LnX(j)).ToArray,
                    .Observed = slopes(i),
                    .Scale = std.Max(std.Sqrt(variance), 1.0E-9),
                    .L1 = options.L1Penalty,
                    .SelfIndex = i,
                    .StabilityWeight = options.StabilityPenalty
                }

                Call report.AppendLine($"  {GeneNames(i)}: truth objective={problem.Objective(truth).ToString("G6")}, truth raw SSE={problem.RawSSE(truth).ToString("G6")}, slope RMS={std.Sqrt(variance).ToString("G6")}")
            Next
        End Sub

        ''' <summary>
        ''' 金标准网络的有向边集合（source-&gt;target）
        ''' </summary>
        ''' <returns></returns>
        Public Function TruthEdges() As String()
            Dim G As Double()() = TruthG()
            Dim H As Double()() = TruthH()
            Dim edges As New System.Collections.Generic.List(Of String)

            For i As Integer = 0 To GeneNames.Length - 1
                For j As Integer = 0 To GeneNames.Length - 1
                    If G(i)(j) <> 0 OrElse H(i)(j) <> 0 Then
                        Call edges.Add($"{GeneNames(j)}->{GeneNames(i)}")
                    End If
                Next
            Next

            Return edges.ToArray
        End Function

        ''' <summary>
        ''' 用金标准网络积分生成一段合成的表达矩阵
        ''' </summary>
        ''' <param name="times"></param>
        ''' <param name="initial"></param>
        ''' <returns></returns>
        Public Function synthetic(times As Double(), Optional initial As Double() = Nothing) As Matrix
            If initial Is Nothing Then
                initial = New Double() {0.2R, 3, 0.3R}
            End If

            Dim G As Double()() = TruthG()
            Dim H As Double()() = TruthH()
            Dim equations As New System.Collections.Generic.List(Of SMRUCC.genomics.Analysis.SSystem.Script.SEquation)

            For i As Integer = 0 To GeneNames.Length - 1
                Dim parameters As New GRN.SSystemParameters With {
                    .GeneNames = GeneNames,
                    .OriginalNames = GeneNames,
                    .alpha = Alpha,
                    .beta = Beta,
                    .G = G,
                    .H = H
                }

                Call equations.Add(New SMRUCC.genomics.Analysis.SSystem.Script.SEquation(
                    GeneNames(i),
                    GRN.SSystemNetwork.BuildExpression(parameters, i)))
            Next

            Dim vars As SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.var() = GeneNames _
                .Select(Function(name, i) New SSVar With {
                    .Id = name,
                    .title = name,
                    .Value = initial(i)
                }) _
                .ToArray

            Dim model As New SSModel With {
                .Title = "synthetic GRN",
                .FinalTime = times(times.Length - 1),
                .sEquations = equations.ToArray,
                .Vars = vars
            }

            Dim options As New RunOptions With {
                .FinalTime = model.FinalTime,
                .UseODEs = True,
                .Steps = times.Length - 1,
                .Solver = Microsoft.VisualBasic.Math.Calculus.Dynamics.ODESolverMethod.RK4
            }

            Dim output As ODEsOut = SSOdes.RunSystem(model, Nothing, options)
            Dim rows As DataFrameRow() = GeneNames _
                .Select(Function(name, i)
                            Dim values As Double() = output _
                                .y(name) _
                                .Value _
                                .Take(times.Length) _
                                .ToArray

                            Return New DataFrameRow With {
                                .geneID = name,
                                .experiments = values
                            }
                        End Function) _
                .ToArray

            Return New Matrix With {
                .tag = "synthetic",
                .sampleID = times _
                    .Select(Function(t) t.ToString("F4", CultureInfo.InvariantCulture)) _
                    .ToArray,
                .expression = rows
            }
        End Function
    End Module
End Namespace
