Public Class Citation

    Public Property authors As String()
    Public Property title As String
    Public Property journal As String
    Public Property year As String
    Public Property volume As String
    Public Property fpage As String
    Public Property lpage As String
    Public Property doi As String
    Public Property pubmed_id As UInteger
    Public Property abstract As String

    Public Overrides Function ToString() As String
        Return nlm_cite()
    End Function

    ''' <summary>
    ''' NLM
    ''' </summary>
    ''' <returns></returns>
    Public Function nlm_cite() As String
        Dim authors = Me.authors.JoinBy(", ")
        Return $"{authors}. {title}. {journal}. {year};{volume}:{fpage}-{lpage}. doi: {doi}. PMID: {pubmed_id}"
    End Function

    ''' <summary>
    ''' AMA
    ''' </summary>
    ''' <returns></returns>
    Public Function ama_cite() As String
        Dim authors = Me.authors.JoinBy(", ")
        Return $"{authors}. {title}. {journal}. {year};{volume}:{fpage}-{lpage}. doi:{doi}"
    End Function

    ''' <summary>
    ''' APA
    ''' </summary>
    ''' <returns></returns>
    Public Function apa_cite() As String
        Dim authors As String

        If Me.authors.Length = 1 Then
            authors = Me.authors(0)
        Else
            authors = Me.authors.First & $", & {Me.authors.Last}"
        End If

        Return $"{authors}. ({year}). {title}. {journal}, {volume}, {fpage}-{lpage}. https://doi.org/{doi}"
    End Function

    ''' <summary>
    ''' MLA
    ''' </summary>
    ''' <returns></returns>
    Public Function mla_cite() As String
        Dim authors As String

        If Me.authors.Length = 1 Then
            authors = Me.authors(0)
        Else
            authors = Me.authors.First & $", and {Me.authors.Last}"
        End If

        Return $"{authors}. ""{title}."" {journal} vol. {volume}({year}): {fpage}-{lpage}. doi:{doi}"
    End Function
End Class
