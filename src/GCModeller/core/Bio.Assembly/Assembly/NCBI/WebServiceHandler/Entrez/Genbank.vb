Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser

Namespace Assembly.NCBI.Entrez

    ''' <summary>
    ''' The NCBI genbank data api handler
    ''' </summary>
    Public Module Genbank

        ' https://www.ncbi.nlm.nih.gov/sviewer/viewer.cgi?
        ' tool=portal
        ' save=file
        ' log$=seqview
        ' db=nuccore
        ' report=gbwithparts
        ' id=229599883
        ' withparts=on
        ' showgi=1

        Const sviewerApi$ = "https://www.ncbi.nlm.nih.gov/sviewer/viewer.cgi?tool=portal&save=file&log$=seqview&db=nuccore&report=gbwithparts"

        Public Sub Fetch(accessionID$, saveAs$, Optional full As Boolean = True, Optional showgi As Boolean = True)
            Dim parameters As New Dictionary(Of String, String)

            parameters!id = getUid(accessionID)
            parameters!withparts = "on" Or "off".When(Not full)
            parameters!showgi = 0 Or 1.When(showgi)

            Dim query$ = parameters.Select(Function(a) $"{a.Key}={a.Value}").JoinBy("&")
            Dim url = $"{sviewerApi}&{query}"

            Call url.DownloadFile(saveAs)
        End Sub

        Public Function GetMetaInfo(accessionID As String) As Dictionary(Of String, String)
            Dim url$ = $"https://www.ncbi.nlm.nih.gov/nuccore/{accessionID}"
            Dim html$ = url.GET
            Dim meta As Dictionary(Of String, String) = html.ParseHtmlMeta

            Return meta
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function getUid(accessionID As String) As String
            Return GetMetaInfo(accessionID) _
                .Where(Function(meta)
                           Return meta.Key.TextEquals("ncbi_uidlist")
                       End Function) _
                .FirstOrDefault _
                .Value
        End Function
    End Module
End Namespace