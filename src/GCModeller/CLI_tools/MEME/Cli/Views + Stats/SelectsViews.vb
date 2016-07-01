Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.FootprintTraceAPI
Imports Microsoft.VisualBasic.CommandLine.Reflection

Partial Module CLI

    <ExportAPI("/LDM.Selects",
               Usage:="/LDM.Selects /trace <footprints.xml> /meme <memeDIR> [/out <outDIR> /named]")>
    Public Function Selectes(args As CommandLine.CommandLine) As Integer
        Dim trace As String = args("/trace")
        Dim memeDIR As String = args("/meme")
        Dim out As String = args.GetValue("/out", trace.TrimFileExt & "-" & memeDIR.BaseName)
        Dim named As String = If(args.GetBoolean("/named"), memeDIR.BaseName & "-", "")
        Dim result As AnnotationModel() = trace.LoadXml(Of FootprintTrace).Select(memeDIR)

        For Each x As AnnotationModel In result

            If Not String.IsNullOrEmpty(named) Then
                x.Uid = named & x.Uid
            End If

            Dim path As String =
                out & "/" & x.Uid.NormalizePathString & ".xml"
            Call x.SaveAsXml(path)
        Next

        Return 0
    End Function
End Module
