Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Namespace Assembly.KEGG.WebServices.InternalWebFormParsers

    Public Module Extensions

        Public Const DBGET$ = "DBGET integrated database retrieval system"

        <Extension>
        Public Function DivInternals(html$) As String()
            If html.StringEmpty Then
                Return {}
            Else
                Dim ms$() = Regex.Matches(html, "<div.+?</div>", RegexICSng).ToArray
                Return ms
            End If
        End Function

        ''' <summary>
        ''' 这个函数只会将第一个nobr标签，即key标签字符串部分给删除掉，其他的nobr标签会被保留
        ''' </summary>
        ''' <param name="html$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Strip_NOBR(html$) As String
            If html.StringEmpty Then
                Return ""
            Else
                Dim m$ = Regex.Match(html, "<nobr>.+?</nobr>", RegexICSng).Value
                If Not m.Length = 0 Then
                    html = html.Replace(m, "")
                End If
                Return html
            End If
        End Function
    End Module
End Namespace