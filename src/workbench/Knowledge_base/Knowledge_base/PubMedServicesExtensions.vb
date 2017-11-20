Public Module PubMedServicesExtensions

    ''' <summary>
    ''' > https://www.ncbi.nlm.nih.gov/pmc/tools/developers/
    ''' 
    ''' Include two parameters that help to identify your service or application to our servers:
    '''
    ''' + ``tool`` should be the name Of the application, As a String value With no internal spaces, And
    ''' + ``email`` should be the e-mail address Of the maintainer Of the tool, And should be a valid e-mail address.
    ''' </summary>
    ReadOnly tool_info As New Dictionary(Of String, String) From {
        {"tool", "GCModeller-workbench-pubmed-repository"},
        {"email", "xie.guigang@gcmodeller.org"}
    }

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
        Dim html$ = url.GET(headers:=tool_info)
        Dim xml$ = html _
            .GetBetween("<pre>", "</pre>") _
            .Replace("&lt;", "<") _
            .Replace("&gt;", ">")
        Dim info As PubmedArticle = xml.CreateObjectFromXmlFragment(Of PubmedArticle)
        Return info
    End Function
End Module
