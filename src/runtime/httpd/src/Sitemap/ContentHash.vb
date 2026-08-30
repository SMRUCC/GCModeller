Imports System
Imports System.Security.Cryptography
Imports System.Text
Imports System.Text.RegularExpressions

''' <summary>
''' The page content fingerprint helper for the sitemap history database.
'''
''' The raw html document of a static website usually contains the volatile
''' fragments, example as the build timestamp, the random token or the
''' inline script block, so that the md5 value of the raw html text will be
''' changed on every website rebuild even if the page content is not
''' changed at all. So that the html document will be normalized at first
''' before the md5 calculation by default.
''' </summary>
Public Module ContentHash

    ReadOnly htmlComment As New Regex("<!--.*?-->", RegexOptions.Singleline)
    ReadOnly scriptBlock As New Regex("<script\b[^>]*>.*?</script>", RegexOptions.IgnoreCase Or RegexOptions.Singleline)
    ReadOnly styleBlock As New Regex("<style\b[^>]*>.*?</style>", RegexOptions.IgnoreCase Or RegexOptions.Singleline)
    ReadOnly noscriptBlock As New Regex("<noscript\b[^>]*>.*?</noscript>", RegexOptions.IgnoreCase Or RegexOptions.Singleline)
    ReadOnly blanks As New Regex("\s+", RegexOptions.Singleline)

    ''' <summary>
    ''' normalize a html document text for the md5 fingerprint calculation:
    ''' removes the html comment, the script block, the style block and the
    ''' noscript block, and then collapse the continuous whitespace.
    ''' </summary>
    ''' <param name="html"></param>
    ''' <returns></returns>
    Public Function Normalize(html As String) As String
        If String.IsNullOrEmpty(html) Then
            Return ""
        End If

        Dim text As String = html

        text = htmlComment.Replace(text, " ")
        text = scriptBlock.Replace(text, " ")
        text = styleBlock.Replace(text, " ")
        text = noscriptBlock.Replace(text, " ")
        text = blanks.Replace(text, " ")

        Return text.Trim
    End Function

    ''' <summary>
    ''' calculate the md5 fingerprint of a html document text
    ''' </summary>
    ''' <param name="html">the html document text of a page</param>
    ''' <param name="rawMd5">
    ''' calculate the md5 value from the raw html text instead of the
    ''' normalized html text?
    ''' </param>
    ''' <returns>
    ''' a 32 characters length hex string in lower case, or Nothing if the
    ''' given html document text is null or empty.
    ''' </returns>
    Public Function Compute(html As String, Optional rawMd5 As Boolean = False) As String
        If String.IsNullOrEmpty(html) Then
            Return Nothing
        End If

        Dim text As String = If(rawMd5, html, Normalize(html))
        Dim buffer As Byte() = Encoding.UTF8.GetBytes(text)
        Dim hash As Byte() = Security.Cryptography.MD5.HashData(buffer)

        Return ToHex(hash)
    End Function

    ''' <summary>
    ''' convert a hash byte buffer as the hex string
    ''' </summary>
    ''' <param name="hash"></param>
    ''' <returns></returns>
    Public Function ToHex(hash As Byte()) As String
        If hash Is Nothing Then
            Return Nothing
        End If

        Dim hex As New StringBuilder(hash.Length * 2)

        For Each byteValue As Byte In hash
            Call hex.Append(byteValue.ToString("x2"))
        Next

        Return hex.ToString
    End Function
End Module
