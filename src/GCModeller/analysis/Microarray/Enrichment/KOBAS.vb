Imports System.Collections.Specialized
Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Data
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace KOBAS

    ''' <summary>
    ''' 做富集分析使用的
    ''' </summary>
    Public Module KOBAS

        Public Function GenelistEnrichment(list$, SpeciesSearch$, Optional input_type$ = "id:uniprot") As String
            Dim args As New Dictionary(Of String, String())

            If list.FileExists(True) Then
                list = list.ReadAllText
            End If

            Call args.Add(NameOf(SpeciesSearch), {SpeciesSearch})
            Call args.Add(NameOf(input_type), {input_type})
            Call args.Add("input_seq1", {list})
            Call args.Add("kobasdb[]", {"G", "K"})
            Call args.Add("Run", {"Run"})

            Dim html$ = "http://kobas.cbi.pku.edu.cn/run_kobas.php".POST(args, Referer:="http://kobas.cbi.pku.edu.cn/anno_iden.php")
            Dim link As String = Regex.Match(html, "javascript:window.location.href='./download_file.php?type=run_kobas&userid=.+?'", RegexICSng).Value
            Dim tmp = App.GetAppSysTempFile

            link = "http://kobas.cbi.pku.edu.cn/" & link.GetStackValue("'", "'").Trim("."c, "/"c)
            link.DownloadFile(tmp)
            tmp = tmp.ReadAllText

            Return tmp
        End Function

        Public Sub SplitData(path$)
            Dim lines$() = path _
                .ReadAllLines _
                .Where(Function(s) Not s.IsBlank AndAlso Not Regex.Match(s, "[-]+").Value = s) _
                .Skip(3) _
                .ToArray
            Dim terms = csv.ImportsTsv(Of EnrichmentTerm)(lines).GroupBy(Function(t) t.Database)

            For Each d In terms
                Dim file$ = path.TrimSuffix & "-" & d.Key.NormalizePathString(False) & ".csv"
                Call d.ToArray.SaveTo(file)
            Next
        End Sub

        ''' <summary>
        ''' ``-<see cref="Math.Log10(Double)"/>(<see cref="EnrichmentTerm.Pvalue"/>)``
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        <Extension> Public Function P(x As EnrichmentTerm) As Double
            Return -Math.Log10(x.Pvalue)
        End Function
    End Module

    Public Class EnrichmentTerm

        <Column("#Term")>
        Public Property Term As String
        Public Property Database As String
        Public Property ID As String

        ''' <summary>
        ''' Input number
        ''' </summary>
        ''' <returns></returns>
        <Column("Input number")> Public Property number As Integer

        ''' <summary>
        ''' Background number
        ''' </summary>
        ''' <returns></returns>
        <Column("Background number")> Public Property Backgrounds As Integer

        ''' <summary>
        ''' P-Value
        ''' </summary>
        ''' <returns></returns>
        <Column("P-Value")> Public Property Pvalue As Double

        ''' <summary>
        ''' Corrected P-Value
        ''' </summary>
        ''' <returns></returns>
        <Column("Corrected P-Value")> Public Property CorrectedPvalue As Double

        ''' <summary>
        ''' The group of this input gene id list
        ''' </summary>
        ''' <returns></returns>
        Public Property Input As String

        <Column("Hyperlink")>
        Public Property link As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace