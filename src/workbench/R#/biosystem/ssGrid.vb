Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.GPS
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.SGrid.GRN
Imports SMRUCC.genomics.Analysis.SGrid.IO
Imports SMRUCC.genomics.Analysis.SGrid.Perturbation
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal
Imports std = System.Math

<Package("ssGrid")>
<RTypeExport("ssgrid_network", GetType(SSystemNetwork))>
<RTypeExport("sfit_options", GetType(FittingOptions))>
Module ssGrid

    Sub Main()
        Call RInternal.generic.add("writeBin", GetType(SSystemNetwork), AddressOf saveModel)
        Call RInternal.generic.add("readBin.ssgrid_network", GetType(Stream), AddressOf loadModel)
    End Sub

    Private Function saveModel(model As SSystemNetwork, args As list, env As Environment) As Object
        Dim con As Stream = args!con

        Try
            Call model.Save(con)
            Call con.Flush()
        Catch ex As Exception
            Call App.LogException(ex)
            Call ex.Message.warning
        End Try

        Return True
    End Function

    Private Function loadModel(s As Stream, args As list, env As Environment) As Object
        Return ZipPersists.Load(s)
    End Function

    <ExportAPI("sfit")>
    Public Function sfit(x As Matrix, Optional opts As FittingOptions = Nothing, Optional env As Environment = Nothing) As SSystemNetwork
        Dim report As New StringBuilder

        Call Console.WriteLine("[INFO] fitting the S-system model...")

        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim network As SSystemNetwork = SSystemNetwork.Fit(x, opts)

        Call Console.WriteLine($"[INFO] fitting finished in {watch.Elapsed.TotalSeconds.ToString("F1")}s: {network.ToString}")

        Call report.AppendLine()
        Call report.AppendLine($"fitting  : {watch.Elapsed.TotalSeconds.ToString("F1")}s, mean slope RMSE={network.Parameters.MeanRMSE.ToString("G6")}")
        Call report.AppendLine($"edges    : {network.Edges.Length}")
        Call report.AppendLine($"prior    : {If(network.Prior Is Nothing, "none", network.Prior.EdgeCount() & " edge(s)")}")

        If Not network.PriorStatistics Is Nothing Then
            Call report.AppendLine()
            Call report.Append(network.PriorStatistics.Summary())
        End If

        Call Console.WriteLine(report.ToString)

        Return network
    End Function

    <ExportAPI("perturb")>
    Public Function perturb(network As SSystemNetwork, <RRawVectorArgument(TypeCodes.string)> geneIds As Object,
                            Optional kicks As Integer = 6,
                            Optional type As PerturbationTypes = PerturbationTypes.KnockOut,
                            Optional magnitude As Double = 0.8) As ScreeningResult

        Dim finalTime As Double = network.Series.TimeEnd
        Dim start As Double = finalTime * 0.1R
        Dim designs As New List(Of PerturbationDesign)
        ' 单次扰动很容易被 S-system 的自我衰减项在很短的时间之内拉回稳态，
        ' 故这里使用周期性的刺激（模拟持续的敲除/过表达处理）来放大下游的响应
        Dim interval As Double = std.Max((finalTime - start) / kicks, 0.000001R)

        For Each hub As String In CLRVector.asCharacter(geneIds)
            Call designs.Add(New PerturbationDesign With {
                .TargetGene = hub,
                .Type = type,
                .Start = start,
                .Interval = interval,
                .Kicks = kicks,
                .Magnitude = magnitude
            })
        Next

        Call $"virtual perturbation on {hubs.Length} hub gene(s): {hubs.JoinBy(", ")}".info

        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim screening As ScreeningResult = Nothing

        Try
            screening = VirtualScreening.Run(network, designs, options.PerturbDir, finalTime)
        Catch ex As Exception
            Call App.LogException(ex)
            Call $"the virtual perturbation screening was failed: {ex.Message}".error

            Return Nothing
        End Try

        Call $"virtual perturbation finished in {watch.Elapsed.TotalSeconds.ToString("F1")}s: {screening.Records.SafeQuery().Count()} record(s)".info

        Call Console.WriteLine(screening.Summary())

        Call Console.WriteLine()
        Call Console.WriteLine($"perturb  : {designs.Count} design(s), {screening.Records.SafeQuery().Count()} record(s) in {watch.Elapsed.TotalSeconds.ToString("F1")}s")
        Call Console.WriteLine("top downstream responses:")

        Dim top As ScreeningRecord() = screening _
            .Downstream(10) _
            .Where(Function(r) Not Double.IsNaN(r.MaxDeviation) AndAlso Not Double.IsInfinity(r.MaxDeviation)) _
            .ToArray

        For Each record As ScreeningRecord In top
            Call Console.WriteLine($"  {record.Target} ({record.Perturbation}) -> {record.Gene}: fold={record.FoldChange.ToString("G4")}, maxdev={record.MaxDeviation.ToString("G4")}, response={record.ResponseTime.ToString("G4")}")
        Next

        Return screening
    End Function
End Module
