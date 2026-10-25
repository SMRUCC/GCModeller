Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics.Data
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Analysis.SSystem.Runner
Imports SSOdes = SMRUCC.genomics.Analysis.SSystem.Kernel.ODEs
Imports std = System.Math

Namespace Perturbation

    ''' <summary>
    ''' 一次虚拟扰动实验之中，某一个基因的响应记录
    ''' </summary>
    Public Class ScreeningRecord

        ''' <summary>
        ''' 被扰动的目标基因
        ''' </summary>
        ''' <returns></returns>
        Public Property Target As String
        ''' <summary>
        ''' 扰动的类型描述
        ''' </summary>
        ''' <returns></returns>
        Public Property Perturbation As String
        ''' <summary>
        ''' 观测的基因（响应者）
        ''' </summary>
        ''' <returns></returns>
        Public Property Gene As String
        ''' <summary>
        ''' 对照组在该基因之上的终末值
        ''' </summary>
        ''' <returns></returns>
        Public Property ControlLevel As Double
        ''' <summary>
        ''' 扰动组在该基因之上的终末值
        ''' </summary>
        ''' <returns></returns>
        Public Property PerturbedLevel As Double
        ''' <summary>
        ''' 终末值的倍数变化（扰动组 / 对照组）
        ''' </summary>
        ''' <returns></returns>
        Public Property FoldChange As Double
        ''' <summary>
        ''' 整条轨迹之上相对对照组的最大绝对偏差
        ''' </summary>
        ''' <returns></returns>
        Public Property MaxDeviation As Double
        ''' <summary>
        ''' 响应时间：轨迹第一次超出对照动态范围 5% 的时间点，负数表示没有任何响应
        ''' </summary>
        ''' <returns></returns>
        Public Property ResponseTime As Double

        ''' <summary>
        ''' 该基因是否为本次扰动的直接目标
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsTarget As Boolean
            Get
                Return String.Equals(Target, Gene, StringComparison.OrdinalIgnoreCase)
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Target} -> {Gene}: fold={FoldChange.ToString("G4")}, maxdev={MaxDeviation.ToString("G4")}"
        End Function
    End Class

    ''' <summary>
    ''' 批量虚拟扰动筛选的结果
    ''' </summary>
    Public Class ScreeningResult

        ''' <summary>
        ''' 本次筛选所使用的扰动设计
        ''' </summary>
        ''' <returns></returns>
        Public Property Designs As PerturbationDesign()
        ''' <summary>
        ''' 全部响应记录
        ''' </summary>
        ''' <returns></returns>
        Public Property Records As ScreeningRecord()
        ''' <summary>
        ''' 仿真输出的时间轴
        ''' </summary>
        ''' <returns></returns>
        Public Property TimePoints As Double()
        ''' <summary>
        ''' 对照组的求解结果
        ''' </summary>
        ''' <returns></returns>
        Public Property Control As ODEsOut

        ''' <summary>
        ''' 按照影响强度降序排列的响应记录（只包含被扰动的目标基因自身的响应）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Ranking As ScreeningRecord()
            Get
                If Records.IsNullOrEmpty Then
                    Return New ScreeningRecord() {}
                End If

                Return Records _
                    .Where(Function(r) r.IsTarget) _
                    .OrderByDescending(Function(r) std.Abs(r.MaxDeviation)) _
                    .ToArray
            End Get
        End Property

        ''' <summary>
        ''' 生成下游受影响最强烈的响应记录（排除被扰动的目标基因自身）
        ''' </summary>
        ''' <param name="top"></param>
        ''' <returns></returns>
        Public Function Downstream(Optional top As Integer = 20) As ScreeningRecord()
            If Records.IsNullOrEmpty Then
                Return New ScreeningRecord() {}
            End If

            Return Records _
                .Where(Function(r) Not r.IsTarget) _
                .OrderByDescending(Function(r) std.Abs(r.MaxDeviation)) _
                .Take(If(top > 0, top, Records.Length)) _
                .ToArray
        End Function

        ''' <summary>
        ''' 生成筛选结果的可读摘要
        ''' </summary>
        ''' <returns></returns>
        Public Function Summary() As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine("virtual perturbation screening")
            Call sb.AppendLine($"designs : {Designs.SafeQuery.Count}")

            For Each design As PerturbationDesign In Designs.SafeQuery
                Call sb.AppendLine($"  {design.ToString}")

                For Each record As ScreeningRecord In rankingOf(design.TargetGene, design.Type.ToString)
                    Call sb.AppendLine($"    {record.Gene}: start={format(record.ControlLevel)}, end={format(record.PerturbedLevel)}, fold={format(record.FoldChange)}, maxdev={format(record.MaxDeviation)}, response={format(record.ResponseTime)}")
                Next
            Next

            Return sb.ToString
        End Function

        Private Iterator Function rankingOf(target As String, perturbation As String) As IEnumerable(Of ScreeningRecord)
            For Each record As ScreeningRecord In Records.SafeQuery
                If record.Target = target AndAlso record.Perturbation = perturbation Then
                    Yield record
                End If
            Next
        End Function

        Private Shared Function format(x As Double) As String
            Return x.ToString("G5", CultureInfo.InvariantCulture)
        End Function
    End Class

    ''' <summary>
    ''' The virtual perturbation screening against the fitted S-system model.
    ''' (基于拟合得到的 S-system 模型做虚拟扰动实验)
    ''' </summary>
    ''' <remarks>
    ''' 每一次扰动都会构造一个 <c>Experiment</c> 对象并挂载到模型之上，
    ''' 然后复用 SSystem 的 ODE 求解链路完成仿真，因此扰动实验与
    ''' 手工编写的 PLAS 脚本的 STIMULATE 语义完全一致。
    ''' </remarks>
    Public Module VirtualScreening

        ''' <summary>
        ''' 执行批量的虚拟扰动筛选
        ''' </summary>
        ''' <param name="network">已经拟合完成的调控网络模型</param>
        ''' <param name="designs">扰动实验的设计方案</param>
        ''' <param name="outDir">可选的输出文件夹，非空的时候每一组扰动都会写出轨迹 csv</param>
        ''' <param name="finalTime">仿真的终止时间，非正数表示使用时间轴的终点</param>
        ''' <returns></returns>
        ''' <param name="context">
        ''' 可选的环境初值（<c>context</c>）：非空的时候 control 与每一组扰动
        ''' 都从这同一个状态出发，因此三个不同的 pseudobulk panel 会得到三套互不相同的基线轨迹。
        ''' 该表可以由 <see cref="GRN.SSystemNetwork.MapContext"/> 从原始 counts 构造
        ''' </param>
        Public Function Run(network As GRN.SSystemNetwork,
                            designs As IEnumerable(Of PerturbationDesign),
                            Optional outDir As String = Nothing,
                            Optional finalTime As Double = 0,
                            Optional context As System.Collections.Generic.IDictionary(Of String, Double) = Nothing) As ScreeningResult

            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If

            Dim t As Double = If(finalTime > 0, finalTime, network.Series.TimeEnd)
            Dim runOptions = network.CreateRunOptions(t)
            Dim controlModel = network.CreateModel(finalTime:=t, initial:=context)
            Dim control As ODEsOut = SSOdes.RunSystem(controlModel, Nothing, runOptions)
            Dim records As New System.Collections.Generic.List(Of ScreeningRecord)
            Dim result As New ScreeningResult With {
                .Designs = designs.SafeQuery().ToArray,
                .Control = control,
                .TimePoints = control.x
            }

            If Not String.IsNullOrEmpty(outDir) Then
                If Not Directory.Exists(outDir) Then
                    Call Directory.CreateDirectory(outDir)
                End If

                Call SSOdes.ToDataFrame(control).Save(Path.Combine(outDir, "control.csv"), Encodings.UTF8)
            End If

            For Each design As PerturbationDesign In result.Designs
                Dim baseline As Double = baselineOf(network, design.TargetGene, context)
                Dim experiment = design.ToExperiment(baseline)
                Dim model = network.CreateModel(New SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.Experiment() {experiment}, finalTime:=t, initial:=context)
                Dim perturbed As ODEsOut = SSOdes.RunSystem(model, Nothing, runOptions)

                If perturbed Is Nothing Then
                    Continue For
                End If

                If Not String.IsNullOrEmpty(outDir) Then
                    Dim file As String = $"perturb_{design.TargetGene.NormalizePathString(False,)}_{design.Type.ToString}.csv"

                    Call SSOdes.ToDataFrame(perturbed).Save(Path.Combine(outDir, file), Encodings.UTF8)
                End If

                For Each gene As String In network.Parameters.GeneNames
                    Dim record As ScreeningRecord = compare(design, gene, control, perturbed)

                    If Not record Is Nothing Then
                        Call records.Add(record)
                    End If
                Next
            Next

            result.Records = records.ToArray

            Return result
        End Function

        ''' <summary>
        ''' 在一个已经编译好的 S-system 模型之上执行批量的虚拟扰动筛选。
        ''' 与 <see cref="Run"/> 的区别是本函数不需要拟合结果，可以直接作用于任意的 .plas 模型。
        ''' </summary>
        ''' <param name="model">S-system 模型对象</param>
        ''' <param name="designs"></param>
        ''' <param name="outDir"></param>
        ''' <param name="finalTime"></param>
        ''' <param name="runOptions"></param>
        ''' <returns></returns>
        Public Function RunModel(model As SSystem.Script.Model,
                                 designs As IEnumerable(Of PerturbationDesign),
                                 Optional outDir As String = Nothing,
                                 Optional finalTime As Double = 0,
                                 Optional runOptions As RunOptions = Nothing) As ScreeningResult

            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If

            Dim t As Double = If(finalTime > 0, finalTime, model.FinalTime)

            If runOptions Is Nothing Then
                runOptions = New RunOptions With {
                    .FinalTime = t,
                    .UseODEs = True
                }
            End If

            model.FinalTime = t

            Dim result As New ScreeningResult With {
                .Designs = designs.SafeQuery().ToArray
            }
            Dim records As New List(Of ScreeningRecord)

            If Not String.IsNullOrEmpty(outDir) Then
                If Not Directory.Exists(outDir) Then
                    Call Directory.CreateDirectory(outDir)
                End If
            End If

            model.Experiments = Nothing

            Dim control As ODEsOut = SSOdes.RunSystem(model, Nothing, runOptions)

            If control Is Nothing Then
                Throw New InvalidOperationException("The control group simulation of the S-system model is failed!")
            End If

            result.Control = control
            result.TimePoints = control.x

            If Not String.IsNullOrEmpty(outDir) Then
                Call SSOdes.ToDataFrame(control).Save(Path.Combine(outDir, "control.csv"), Encodings.UTF8)
            End If

            For Each design As PerturbationDesign In result.Designs
                Dim baseline As Double = baselineOfModel(model, design.TargetGene)
                Dim experiment As SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.Experiment = design.ToExperiment(baseline)

                model.Experiments = New SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.Experiment() {experiment}

                Dim perturbed As ODEsOut = SSOdes.RunSystem(model, Nothing, runOptions)

                If perturbed Is Nothing Then
                    Continue For
                End If

                If Not String.IsNullOrEmpty(outDir) Then
                    Dim file As String = $"perturb_{design.TargetGene.NormalizePathString(alphabetOnly:=False)}_{design.Type.ToString}.csv"

                    Call SSOdes.ToDataFrame(perturbed).Save(Path.Combine(outDir, file), Encodings.UTF8)
                End If

                For Each target As SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.var In model.Vars.SafeQuery
                    Dim record As ScreeningRecord = compare(design, target.Id, control, perturbed)

                    If Not record Is Nothing Then
                        Call records.Add(record)
                    End If
                Next
            Next

            model.Experiments = Nothing
            result.Records = records.ToArray

            Return result
        End Function

        Private Function baselineOfModel(model As SMRUCC.genomics.Analysis.SSystem.Script.Model, gene As String) As Double
            For Each v As SMRUCC.genomics.Analysis.SSystem.Kernel.ObjectModels.var In model.Vars.SafeQuery
                If String.Equals(v.Id, gene, StringComparison.OrdinalIgnoreCase) Then
                    Return v.Value
                End If
            Next

            Return 1
        End Function

        ''' <summary>
        ''' 为每一个基因生成默认的敲低扰动实验并执行筛选
        ''' </summary>
        ''' <param name="network"></param>
        ''' <param name="type"></param>
        ''' <param name="magnitude"></param>
        ''' <param name="outDir"></param>
        ''' <returns></returns>
        Public Function KnockdownAll(network As GRN.SSystemNetwork,
                                     Optional type As PerturbationTypes = PerturbationTypes.KnockDown,
                                     Optional magnitude As Double = 0.8R,
                                     Optional outDir As String = Nothing) As ScreeningResult

            Dim designs As PerturbationDesign() = PerturbationDesign.Batch(network.Parameters.GeneNames, type, magnitude)

            Return Run(network, designs, outDir)
        End Function

        ''' <summary>
        ''' 取出目标基因的基线水平（敲降/过表达的幅度都是相对该基线计算的）。
        ''' 当指定了 context 环境的时候，基线取该环境之下的初值，而不是时间序列的第一个采样点，
        ''' 这样「敲降 80%」才是相对于这个 panel 的基线而言的
        ''' </summary>
        ''' <param name="network"></param>
        ''' <param name="gene"></param>
        ''' <param name="context"></param>
        ''' <returns></returns>
        Private Function baselineOf(network As GRN.SSystemNetwork,
                                    gene As String,
                                    Optional context As System.Collections.Generic.IDictionary(Of String, Double) = Nothing) As Double
            Dim value As Double

            If Not context Is Nothing Then
                If context.TryGetValue(gene, value) Then
                    Return value
                End If
            End If

            Dim idx As Integer = network.Series.GeneIndex(gene)

            If idx < 0 Then
                Return 1
            End If

            Return network.Series.X(idx)(0)
        End Function

        Private Function compare(design As PerturbationDesign,
                                 gene As String,
                                 control As ODEsOut,
                                 perturbed As ODEsOut) As ScreeningRecord

            Dim c As Double() = seriesOf(control, gene)
            Dim p As Double() = seriesOf(perturbed, gene)

            If c Is Nothing OrElse p Is Nothing OrElse c.Length <> p.Length Then
                Return Nothing
            End If

            Dim t As Double() = control.x
            Dim cmin As Double = c.Min
            Dim cmax As Double = c.Max
            Dim span As Double = std.Abs(cmax - cmin)
            Dim threshold As Double = std.Max(span * 0.05, 1.0E-9)
            Dim maxDev As Double = 0
            Dim response As Double = -1

            For k As Integer = 0 To c.Length - 1
                Dim d As Double = std.Abs(p(k) - c(k))

                If d > maxDev Then
                    maxDev = d
                End If
                If response < 0 AndAlso d > threshold Then
                    response = If(t Is Nothing, k, t(k))
                End If
            Next

            Dim cEnd As Double = c(c.Length - 1)
            Dim pEnd As Double = p(p.Length - 1)
            Dim fold As Double = If(std.Abs(cEnd) < 1.0E-12, pEnd, pEnd / cEnd)

            Return New ScreeningRecord With {
                .Target = design.TargetGene,
                .Perturbation = design.Type.ToString,
                .Gene = gene,
                .ControlLevel = cEnd,
                .PerturbedLevel = pEnd,
                .FoldChange = fold,
                .MaxDeviation = maxDev,
                .ResponseTime = response
            }
        End Function

        Private Function seriesOf(output As ODEsOut, gene As String) As Double()
            If output Is Nothing OrElse output.y Is Nothing Then
                Return Nothing
            End If
            If Not output.y.ContainsKey(gene) Then
                Return Nothing
            End If

            Return output.y(gene).Value
        End Function
    End Module
End Namespace
