Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions

Namespace Assembly.KEGG.WebServices.InternalWebFormParsers

    Public Module Extensions

        Public Const DBGET$ = "DBGET integrated database retrieval system"

        <Extension>
        Public Function DivInternals(html$) As String()
            Dim ms$() = Regex.Matches(html, "<div.+?</div>", RegexICSng).ToArray
            Return ms
        End Function
    End Module
End Namespace