Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Webservices.Bing

Public Module BingSearch

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    <Extension>
    Public Function CreateRefer(refer As String) As Dictionary(Of String, String)
        Return New Dictionary(Of String, String) From {{NameOf(refer), refer}}
    End Function

    ''' <summary>
    ''' Bing search for wiki english
    ''' </summary>
    ''' <param name="term"></param>
    ''' <returns></returns>
    ''' 
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function CreateQueryURL(term As String) As String
        Return $"https://cn.bing.com/search?q={term.UrlEncode}+site%3A+en.wikipedia.org&qs=n&form=QBRE&sp=-1&pq={term.UrlEncode}+site%3A+en.wikipedia.org&sc=0-42&sk=&cvid=939096C192AD4B758FD3CBA281CBD413"
    End Function

    Public Function GetWikiMarkdown(term As String) As String
        Dim url$ = $"https://en.wikipedia.org/w/index.php?title={term}&action=edit"
        Dim html$ = url.GET()

    End Function

    Public Iterator Function WikiBingSearch(term As String, Optional limits% = 100) As IEnumerable(Of WebResult)
        Dim url$ = CreateQueryURL(term)
        Dim list = SearchEngineProvider.DownloadResult(url)
        Dim n% = 0

        Do While n < limits

            For Each entry In list.CurrentPage
                Yield entry
            Next

            n += list.CurrentPage.Length

            If list.HaveNext Then
                list = list.NextPage
            End If
        Loop
    End Function
End Module
