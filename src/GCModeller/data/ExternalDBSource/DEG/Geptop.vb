Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports SMRUCC.genomics.ComponentModel

Public Module Geptop

    Const listAPI$ = "http://cefg.uestc.cn/geptop/list.html"

    Public Function GetGenomeList() As NamedValue(Of String)()
        Dim html As String = listAPI.GET
        Dim list = html.Matches("<li.+?</li>").ToArray
        Dim genomes = list _
            .Select(Function(li)
                        Dim link = li.href
                        Dim name = li.StripHTMLTags

                        Return New NamedValue(Of String) With {
                            .Name = name,
                            .Value = link
                        }
                    End Function) _
            .ToArray

        Return genomes
    End Function

    Public Sub FetchData(save$)
        Dim web As New WebQuery(Of NamedValue(Of String))(Function(li) li.Value, Function(li) li.Name, Function(s, t) s, $"{save}/.geptop")

        For Each genome As NamedValue(Of String) In GetGenomeList()
            Call web.Query(Of String)(genome, "*.html")
        Next
    End Sub
End Module
