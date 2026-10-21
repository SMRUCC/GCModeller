Imports System.Globalization
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics.Data
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels
Imports SMRUCC.genomics.Analysis.SSystem.Runner
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
        ''' 金标准网络的合成速率常数（method.md 的三基因示例）
        ''' </summary>
        Public ReadOnly Alpha As Double() = {12, 10, 5}

        ''' <summary>
        ''' 金标准网络的降解速率常数
        ''' </summary>
        Public ReadOnly Beta As Double() = {2, 3, 1.5}

        ''' <summary>
        ''' 金标准网络的合成项动力学阶矩阵
        ''' </summary>
        Public Function TruthG() As Double()()
            Dim G As Double()() = {
                New Double() {0, 0, -0.8R},
                New Double() {0.5R, 0, 0},
                New Double() {0.2R, -0.4R, 0}
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
                New Double() {0, 0, 0.5R}
            }

            Return H
        End Function

        ''' <summary>
        ''' 执行自测，返回进程退出码（零表示通过）
        ''' </summary>
        ''' <param name="verbose"></param>
        ''' <returns></returns>
        Public Function Run(Optional verbose As Boolean = True) As Integer
            Dim failures As New System.Collections.Generic.List(Of String)
            Dim report As New StringBuilder()

            Call report.AppendLine("=== SGrid self test: S-system GRN inference ===")

            Dim times As Double() = Enumerable _
                .Range(0, 41) _
                .Select(Function(i) i * 0.25R) _
                .ToArray

            ' 1. 用金标准网络生成合成的表达矩阵
            Dim matrix As Matrix = synthetic(times)

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

            If recall < 0.7R Then
                Call failures.Add($"the topological recall {recall.ToString("F2")} is lower than 0.70")
            End If
            If precision < 0.5R Then
                Call failures.Add($"the topological precision {precision.ToString("F2")} is lower than 0.50")
            End If

            ' 4. 参数的还原程度
            Call report.AppendLine()
            Call report.AppendLine("rate constant recovery:")

            For i As Integer = 0 To GeneNames.Length - 1
                Dim ra As Double = std.Abs(network.Parameters.alpha(i) - Alpha(i)) / std.Max(Alpha(i), 1.0E-6)
                Dim rb As Double = std.Abs(network.Parameters.beta(i) - Beta(i)) / std.Max(Beta(i), 1.0E-6)

                Call report.AppendLine($"  {GeneNames(i)}: alpha {network.Parameters.alpha(i).ToString("G5")} (truth {Alpha(i)}), beta {network.Parameters.beta(i).ToString("G5")} (truth {Beta(i)}), relerr=({ra.ToString("F2")}, {rb.ToString("F2")})")

                If std.Max(ra, rb) > 0.5R Then
                    Call failures.Add($"the rate constant of the gene '{GeneNames(i)}' is not recovered (rel err alpha={ra.ToString("F2")}, beta={rb.ToString("F2")})")
                End If
            Next

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

            Dim target As Perturbation.ScreeningRecord = screening _
                .Records _
                .SafeQuery() _
                .FirstOrDefault(Function(r) r.IsTarget)

            Call report.AppendLine()
            Call report.AppendLine("virtual perturbation:")

            For Each record As Perturbation.ScreeningRecord In screening.Records.SafeQuery
                Call report.AppendLine($"  X1 knock-down -> {record.Gene}: end={record.PerturbedLevel.ToString("G4")} (control {record.ControlLevel.ToString("G4")}), maxdev={record.MaxDeviation.ToString("G4")}, response={record.ResponseTime.ToString("G3")}")
            Next

            If target Is Nothing Then
                Call failures.Add("the virtual perturbation experiment did not produce any response record for the target gene")
            ElseIf target.MaxDeviation < 1.0E-6 Then
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
                initial = New Double() {5, 3, 4}
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
