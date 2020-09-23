#Region "Microsoft.VisualBasic::00773cc9e858d82b84c2f2c0584abb97, CLI_tools\MEME\Cli\MotifSimilarity\TomTom.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Module CLI
    ' 
    '     Function: __memeTOMQuery, LDMTomTom, TomQuery, TomQueryBatch, TomTOMMethod
    ' 
    '     Sub: __LDMTom
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Text
Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming.Levenshtein
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.DATA
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.GCModeller.Workbench.ReportBuilder
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis
Imports SMRUCC.genomics.Interops.NBCR.MEME_Suite.Analysis.MotifScans

Partial Module CLI

    <ExportAPI("/TomTOM",
               Usage:="/TomTOM /query <meme.txt> /subject <LDM.xml> [/out <outDIR> /method <pcc/ed; default:=pcc> /cost <0.7> /threshold <0.3>]")>
    Public Function TomTOMMethod(args As CommandLine) As Integer
        Dim queryFile As String = args("/query")
        Dim subjectFile As String = args("/subject")
        Dim method As String = args.GetValue("/method", "pcc")
        Dim cost As Double = args.GetValue("/cost", 0.7)
        Dim threshold As Double = args.GetValue("/threshold", 0.3)
        Dim motifs As AnnotationModel() = AnnotationModel.LoadDocument(queryFile)
        Dim subjectLDM = subjectFile.LoadXml(Of AnnotationModel)
        Dim out As String = args.GetValue("/out", queryFile.TrimSuffix & "-" & BaseName(subjectFile))

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
    Public Function LDMTomTom(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim subject As String = args("/subject")
        Dim queryLDM = query.LoadXml(Of AnnotationModel)
        Dim subjectLDM = subject.LoadXml(Of AnnotationModel)
        Dim method As String = args.GetValue("/method", "sw")
        Dim cost As Double = args.GetValue("/cost", 0.7)
        Dim threshold As Double = args.GetValue("/threshold", 0.65)
        Dim out As String = args.GetValue("/out", query.TrimSuffix & "-" & BaseName(subject) & "." & method)

        Call __LDMTom(queryLDM, subjectLDM, method, cost, threshold, out, "TOMQuery")

        Return 0
    End Function

    <ExportAPI("/Tom.Query.Batch",
               Usage:="/Tom.Query.Batch /query <inDIR> [/out <outDIR> /method <pcc/ed; default:=pcc> /cost 0.7 /threshold <0.65>]")>
    Public Function TomQueryBatch(args As CommandLine) As Integer
        Dim inDIR As String = args("/query")
        Dim method As String = args.GetValue("/method", "pcc")
        Dim cost As Double = args.GetValue("/cost", 0.7)
        Dim threshold As Double = args.GetValue("/threshold", 0.65)
        Dim out As String = args.GetValue("/out", inDIR & ".TomQuery/")
        Dim memeText = FileIO.FileSystem.GetFiles(inDIR, FileIO.SearchOption.SearchTopLevelOnly, "*.txt")
        Dim resultSet As New List(Of Similarity.TOMQuery.CompareResult)

        For Each query As String In memeText
            Dim outDIR As String = out & "/" & BaseName(query)
            Call resultSet.Add(__memeTOMQuery(query, outDIR, threshold, cost, method))
        Next

        Return resultSet.SaveTo(out & "/Motifs.Csv").CLICode
    End Function

    Private Function __memeTOMQuery(query As String, out As String, threshold As Double, cost As Double, method As String) As Similarity.TOMQuery.CompareResult()
        Dim result = Similarity.TOMQuery.TomTOm.CompareBest(query, method, cost, threshold)
        Dim LQuery = (From x In result Select x.Value.Select(Function(hit) Similarity.TOMQuery.TomTOm.CreateResult(x.Key, hit.Key, hit.Value))).ToVector
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

            Dim resultSet = motif.Value.Select(Function(hit) Similarity.TOMQuery.TomTOm.CreateResult(queryLDM, hit.Key, hit.Value)).ToArray
            Dim table As String = resultSet.ToHTMLTable("API")

            Call html.AppendLine($"<h3>Query for {queryLDM.ToString}</h3>")
            Call html.AppendLine("<p>")
            Call html.AppendLine($"{resultSet.Length} motif hits.")
            Call html.AppendLine(table)
            Call html.AppendLine("</p>")
        Next

        Dim title As String = $"Motif tom query for {BaseName(query)}:"
        Call html.SaveAsHTML($"{out}/TomQuery.html", BaseName(query), title)

        Return LQuery
    End Function

    ''' <summary>
    ''' 对meme的分析结果判断是哪一个motif
    ''' </summary>
    ''' <param name="args"></param>
    ''' <returns></returns>
    <ExportAPI("/Tom.Query",
               Usage:="/Tom.Query /query <ldm.xml/meme.txt> [/out <outDIR> /method <pcc/ed; default:=pcc> /cost <0.7> /threshold <0.65> /meme]")>
    Public Function TomQuery(args As CommandLine) As Integer
        Dim query As String = args("/query")
        Dim method As String = args.GetValue("/method", "pcc")
        Dim cost As Double = args.GetValue("/cost", 0.7)
        Dim threshold As Double = args.GetValue("/threshold", 0.65)
        Dim isMEME As Boolean = args.GetBoolean("/meme")
        Dim out As String = args.GetValue("/out", query.TrimSuffix)

        If isMEME Then
            Call __memeTOMQuery(query, out, threshold, cost, method)
        Else
            Dim queryLDM = query.LoadXml(Of AnnotationModel)
            Dim result = Similarity.TOMQuery.TomTOm.CompareBest(queryLDM, method, cost, threshold)
            Dim resultSet = result.Select(Function(x) Similarity.TOMQuery.TomTOm.CreateResult(queryLDM, x.Key, x.Value))
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
