Public Module PubMedServicesExtensions

    ReadOnly tool_info As Dictionary(Of String, String)

    ''' <summary>
    ''' Example
    ''' 
    ''' ```
    ''' https://www.ncbi.nlm.nih.gov/pubmed/?term=22007635&amp;report=xml
    ''' ```
    ''' </summary>
    ''' <param name="term"></param>
    ''' <returns></returns>
    Public Function GetArticleInfo(term As String) As PubmedArticle
        Dim url$ = $"https://www.ncbi.nlm.nih.gov/pubmed/?term={term}&report=xml"
        Dim html$ = url.GET()
        Dim xml$ = html _
            .GetBetween("<pre>", "</pre>") _
            .Replace("&lt;", "<") _
            .Replace("&gt;", ">")
        Dim info As PubmedArticle = xml.CreateObjectFromXmlFragment(Of PubmedArticle)
        Return info
    End Function
End Module
