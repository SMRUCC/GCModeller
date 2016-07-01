Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis.MotifScans
Imports SMRUCC.genomics.AnalysisTools.NBCR.Extensions.MEME_Suite.Analysis
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports System.Drawing
Imports Microsoft.VisualBasic.DocumentFormat.Csv
Imports Microsoft.VisualBasic.DocumentFormat.Csv.DocumentStream.HTMLWriter
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic
Imports System.Text
Imports SMRUCC.genomics.GCModeller.Workbench.ReportBuilder
Imports Microsoft.VisualBasic.Imaging

Partial Module CLI

    <ExportAPI("/TomTOM",
               Usage:="/TomTOM /query <meme.txt> /subject <LDM.xml> [/out <outDIR> /method <pcc/ed; default:=pcc> /cost <0.7> /threshold <0.3>]")>
    Public Function TomTOMMethod(args As CommandLine.CommandLine) As Integer
        Dim queryFile As String = args("/query")
        Dim subjectFile As String = args("/subject")
        Dim method As String = args.GetValue("/method", "pcc")
        Dim cost As Double = args.GetValue("/cost", 0.7)
        Dim threshold As Double = args.GetValue("/threshold", 0.3)
        Dim motifs As AnnotationModel() = AnnotationModel.LoadDocument(queryFile)
        Dim subjectLDM = subjectFile.LoadXml(Of AnnotationModel)
        Dim out As String = args.GetValue("/out", queryFile.TrimFileExt & "-" & IO.Path.GetFileNameWithoutExtension(subjectFile))

        For Each motif As AnnotationModel In motifs
            Call __LDMTom(motif, subjectLDM, method, cost, threshold, out, $"{motif.ToString}-{subjectLDM.ToString}")
        Next

        Return 0
    End Function

    Private Sub __LDMTom(queryLDM As AnnotationModel,
                         subjectLDM As AnnotationModel,
                         method As String,
                         cost As Double,
                         threshold As Double,
                         out As String,
                         uid As String)
        Dim distResult As DistResult = Similarity.TOMQuery.TomTOm.Compare(queryLDM, subjectLDM, method, cost, threshold)
        Dim comapre As Image = Similarity.TOMQuery.TomVisual.VisualLevEdit(queryLDM, subjectLDM, distResult)
        Dim png As String = $"{out}/{uid}.png"
        Call comapre.SaveAs(png, ImageFormats.Png)
        Call $"image save to {png.ToFileURL}...".__DEBUG_ECHO
        Call Similarity.TOMQuery.CreateResult(queryLDM, subjectLDM, distResult).SaveAsXml($"{out}/{uid}.Xml")
    End Sub

    <ExportAPI("/TomTom.LDM",
               Usage:="/TomTom.LDM /query <ldm.xml> /subject <ldm.xml> [/out <outDIR> /method <pcc/ed/sw; default:=sw> /cost <0.7> /threshold <0.65>]")>
    Public Function LDMTomTom(args As CommandLine.CommandLine) As Integer
        Dim query As String = args("/query")
        Dim subject As String = args("/subject")
        Dim queryLDM = query.LoadXml(Of AnnotationModel)
        Dim subjectLDM = subject.LoadXml(Of AnnotationModel)
        Dim method As String = args.GetValue("/method", "sw")
        Dim cost As Double = args.GetValue("/cost", 0.7)
        Dim threshold As Double = args.GetValue("/threshold", 0.65)
        Dim out As String = args.GetValue("/out", query.TrimFileExt & "-" & IO.Path.GetFileNameWithoutExtension(subject) & "." & method)

        Call __LDMTom(queryLDM, subjectLDM, method, cost, threshold, out, "TOMQuery")

        Return 0
    End Function

    <ExportAPI("/Tom.Query.Batch",
               Usage:="/Tom.Query.Batch /query <inDIR> [/out <outDIR> /method <pcc/ed; default:=pcc> /cost 0.7 /threshold <0.65>]")>
    Public Function TomQueryBatch(args As CommandLine.CommandLine) As Integer
        Dim inDIR As String = args("/query")
        Dim method As String = args.GetValue("/method", "pcc")
        Dim cost As Double = args.GetValue("/cost", 0.7)
        Dim threshold As Double = args.GetValue("/threshold", 0.65)
        Dim out As String = args.GetValue("/out", inDIR & ".TomQuery/")
        Dim memeText = FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.txt")
        Dim resultSet As New List(Of Similarity.TOMQuery.CompareResult)

        For Each query As String In memeText
            Dim outDIR As String = out & "/" & IO.Path.GetFileNameWithoutExtension(query)
            Call resultSet.Add(__memeTOMQuery(query, outDIR, threshold, cost, method))
        Next

        Return resultSet.SaveTo(out & "/Motifs.Csv").CLICode
    End Function

    Private Function __memeTOMQuery(query As String, out As String, threshold As Double, cost As Double, method As String) As Similarity.TOMQuery.CompareResult()
        Dim result = Similarity.TOMQuery.TomTOm.CompareBest(query, method, cost, threshold)
        Dim LQuery = (From x In result Select x.Value.ToArray(Function(hit) Similarity.TOMQuery.TomTOm.CreateResult(x.Key, hit.Key, hit.Value))).MatrixToVector
        Dim html As New StringBuilder(4096)

        Call LQuery.SaveTo($"{out}/TomQuery.Csv")
        Call html.AppendLine(Navigation.MEMETomQuery)
        Call html.AppendLine("Prameter Summary:" & BR)
        Call html.AppendLine("Method: " & method & BR)
        Call html.AppendLine("Levenshtein cost: " & cost & BR)
        Call html.AppendLine("Threshold: " & threshold & BR)
        Call html.AppendLine("<hr>")

        For Each motif In result
            Dim queryLDM As AnnotationModel = motif.Key

            For Each hit In motif.Value
                Dim png = Similarity.TOMQuery.TomVisual.VisualLevEdit(queryLDM, hit.Key, hit.Value)
                Call png.SaveAs($"{out}/{queryLDM.ToString}-{hit.Key.ToString}.png", ImageFormats.Png)
                Call Console.Write(".")
            Next

            Dim resultSet = motif.Value.ToArray(Function(hit) Similarity.TOMQuery.TomTOm.CreateResult(queryLDM, hit.Key, hit.Value))
            Dim table As String = resultSet.ToHTMLTable("API")

            Call html.AppendLine($"<h3>Query for {queryLDM.ToString}</h3>")
            Call html.AppendLine("<p>")
            Call html.AppendLine($"{resultSet.Length} motif hits.")
            Call html.AppendLine(table)
            Call html.AppendLine("</p>")
        Next

        Dim title As String = $"Motif tom query for {IO.Path.GetFileNameWithoutExtension(query)}:"
        Call html.SaveAsHTML($"{out}/TomQuery.html", IO.Path.GetFileNameWithoutExtension(query), title)

        Return LQuery
    End Function

    ''' <summary>
    ''' 对meme的分析结果判断是哪一个motif
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Tom.Query",
               Usage:="/Tom.Query /query <ldm.xml/meme.txt> [/out <outDIR> /method <pcc/ed; default:=pcc> /cost <0.7> /threshold <0.65> /meme]")>
    Public Function TomQuery(args As CommandLine.CommandLine) As Integer
        Dim query As String = args("/query")
        Dim method As String = args.GetValue("/method", "pcc")
        Dim cost As Double = args.GetValue("/cost", 0.7)
        Dim threshold As Double = args.GetValue("/threshold", 0.65)
        Dim isMEME As Boolean = args.GetBoolean("/meme")
        Dim out As String = args.GetValue("/out", query.TrimFileExt)

        If isMEME Then
            Call __memeTOMQuery(query, out, threshold, cost, method)
        Else
            Dim queryLDM = query.LoadXml(Of AnnotationModel)
            Dim result = Similarity.TOMQuery.TomTOm.CompareBest(queryLDM, method, cost, threshold)
            Dim resultSet = result.ToArray(Function(x) Similarity.TOMQuery.TomTOm.CreateResult(queryLDM, x.Key, x.Value))
            Call resultSet.SaveTo($"{out}/TomQuery.Csv")

            For Each hit As KeyValuePair(Of AnnotationModel, DistResult) In result
                Dim png = Similarity.TOMQuery.TomVisual.VisualLevEdit(queryLDM, hit.Key, hit.Value)
                Dim path As String = $"{out}/{queryLDM.ToString}-{hit.Key.ToString}.png"

                Call png.SaveAs(path, ImageFormats.Png)
            Next
        End If

        Return 0
    End Function
End Module