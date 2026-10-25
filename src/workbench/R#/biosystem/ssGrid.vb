
Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.GPS
Imports SMRUCC.genomics.Analysis.GPS.Config
Imports SMRUCC.genomics.Analysis.GPS.Training
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.WGCNA.DynamicTreeCut
Imports SMRUCC.genomics.Analysis.SGrid.GRN
Imports SMRUCC.genomics.Analysis.SGrid.IO
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Internal.Object
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization
Imports RInternal = SMRUCC.Rsharp.Runtime.Internal

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
    Public Function sfit(x As Matrix, Optional opts As FittingOptions = Nothing, Optional env As Environment = Nothing) As Object
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
End Module
