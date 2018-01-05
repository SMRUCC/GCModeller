Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models

Namespace Assembly.KEGG.DBGET.bGetObject.Organism

    Public Class OrganismInfo

        ''' <summary>
        ''' T number
        ''' </summary>
        ''' <returns></returns>
        Public Property TID As String
        Public Property code As String
        Public Property Aliases As String
        Public Property FullName As String
        Public Property Definition As String
        Public Property Taxonomy As String
        Public Property Lineage As String
        Public Property DataSource As String
        Public Property Keywords As String()
        Public Property Comment As String
        Public Property Sequence As String
        Public Property Created As String
        Public Property Reference As Reference

        Public Overrides Function ToString() As String
            Return $"({code}) {FullName}"
        End Function

        Public Shared Function ShowOrganism(code As String) As OrganismInfo
            Dim html$ = $"http://www.kegg.jp/kegg-bin/show_organism?org={code}".GET(refer:="http://www.kegg.jp/kegg/catalog/org_list.html")
            html = Strings.Split(html, "</form>").Last
            html = TableParser.GetTablesHTML(html,) _
                              .First _
                              .Replace("&nbsp;", " ") _
                              .Trim

            Dim rows = html _
                .GetRowsHTML _
                .Select(Function(r)
                            Dim cols = r.GetColumnsHTML
                            Dim name$ = cols(0).StripHTMLTags(True)
                            Dim value = cols(1)

                            Return New NamedValue(Of String) With {
                                .Name = name,
                                .Value = value
                            }
                        End Function) _
                .ToDictionary(Function(r) r.Name,
                              Function(r) r.Value)

            Return New OrganismInfo With {
                .Aliases = rows!Aliases,
                .code = rows("Org code"),
                .Comment = rows!Comment,
                .Created = rows!Created,
                .FullName = rows("Full name"),
                .Definition = rows!Definition,
                .Keywords = rows!Keywords.Split(","c),
                .Sequence = rows!Sequence,
                .Lineage = rows!Lineage,
                .Taxonomy = rows!Taxonomy,
                .TID = rows("T number"),
                .DataSource = rows("Data source"),
                .Reference = New Reference With {
                    .Title = rows!Title,
                    .Authors = rows!Authors.Split(";"c),
                    .Reference = rows!Reference,
                    .Journal = rows!Journal
                }
            }
        End Function
    End Class
End Namespace