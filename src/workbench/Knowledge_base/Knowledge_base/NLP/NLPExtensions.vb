Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Webservices.Bing.Academic
Imports Microsoft.VisualBasic.Data.NLP

Public Module NLPExtensions

    <Extension>
    Public Function InformationAbstract(kb As IEnumerable(Of ArticleProfile)) As Dictionary(Of String, Double)
        Dim text$ = kb _
            .Select(Function(a) a.abstract) _
            .Where(Function(s) Not s.StringEmpty) _
            .JoinBy(ASCII.LF)
        Dim abstract = text.TextGraph.Abstract

        Return abstract
    End Function
End Module