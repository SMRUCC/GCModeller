Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Webservices.Bing.Academic
Imports Microsoft.VisualBasic.Data.NLP

Public Module NLPExtensions

    <Extension>
    Public Function InformationAbstract(kb As IEnumerable(Of ArticleProfile), Optional minWeight# = 0.05) As Dictionary(Of String, Double)
        Dim text$ = kb _
            .Select(Function(a) a.abstract) _
            .Where(Function(s) Not s.StringEmpty) _
            .JoinBy(ASCII.LF)
        Dim abstract = text.TextGraph.Abstract(minWeight:=minWeight)

        Return abstract
    End Function
End Module