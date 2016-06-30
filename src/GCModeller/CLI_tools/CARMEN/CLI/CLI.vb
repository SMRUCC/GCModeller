Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData

<PackageNamespace("CARMEN.CLI", Category:=APICategories.CLI_MAN)>
Module CLI

    <ExportAPI("--Reconstruct.KEGG.Online", Usage:="--Reconstruct.KEGG.Online /sp <organism> [/pathway <KEGG.pathwayId> /out <outDIR>]")>
    Public Function Reconstruct(args As CommandLine.CommandLine) As Integer
        Dim sp As String = args("/sp")
        Dim pathway As String = args("/pathway")
        Dim out As String = args.GetValue("/out", __getOutDIR(sp, pathway))

        If Not String.IsNullOrEmpty(pathway) Then
            Return LANS.SystemsBiology.AnalysisTools.CARMEN.WebHandler.Reconstruct(sp, pathway, out).CLICode
        Else
            Return __reconstructAll(sp, outDIR:=out)
        End If
    End Function

    Private Function __reconstructAll(sp As String, outDIR As String) As Integer
        If LANS.SystemsBiology.AnalysisTools.CARMEN.lstPathways.IsNullOrEmpty Then
            Call LANS.SystemsBiology.AnalysisTools.CARMEN.LoadList()
        End If

        For Each pathway In LANS.SystemsBiology.AnalysisTools.CARMEN.lstPathways
            Dim name As String = pathway.Value
            Dim DIR As String = outDIR & "/" & name.NormalizePathString

            Try
                Call LANS.SystemsBiology.AnalysisTools.CARMEN.WebHandler.Reconstruct(sp, pathway.Key, DIR).CLICode
            Catch ex As Exception
                Call App.LogException(ex)
            End Try
        Next

        Return 0
    End Function

    Private Function __getOutDIR(sp As String, pathway As String) As String
        Dim outDIR As String = App.CurrentDirectory & $"/{sp.NormalizePathString}/"

        Call LANS.SystemsBiology.AnalysisTools.CARMEN.LoadList()

        If String.IsNullOrEmpty(pathway) Then
            Return outDIR
        End If

        outDIR = $"{outDIR}/{LANS.SystemsBiology.AnalysisTools.CARMEN.lstPathways(pathway).NormalizePathString}/"
        Return outDIR
    End Function

    <ExportAPI("--lstId.Downloads",
               Info:="Download the Avaliable organism name And available pathways' name.",
               Usage:="--lstId.Downloads [/o <out.DIR>]")>
    Public Function DownloadList(args As CommandLine.CommandLine) As Integer
        Dim out As String = args.GetValue("/o", App.CurrentDirectory)
        Call LANS.SystemsBiology.AnalysisTools.CARMEN.LoadList()
        Call LANS.SystemsBiology.AnalysisTools.CARMEN.lstOrganisms.SaveTo(out & "/Organisms.txt")
        Call LANS.SystemsBiology.AnalysisTools.CARMEN.lstPathways.SaveTo(out & "/Pathways.txt")
        Return 0
    End Function
End Module
