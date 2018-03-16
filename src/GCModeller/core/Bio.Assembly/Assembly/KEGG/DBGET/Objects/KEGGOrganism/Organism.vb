#Region "Microsoft.VisualBasic::2c512429071067367076193825fdbb23, core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\KEGGOrganism\Organism.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    '     Class OrganismInfo
    ' 
    '         Properties: Aliases, code, Comment, Created, DataSource
    '                     Definition, FullName, Keywords, Lineage, Reference
    '                     Sequence, Taxonomy, TID
    ' 
    '         Function: links, referenceParser, ShowOrganism, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports r = System.Text.RegularExpressions.Regex

Namespace Assembly.KEGG.DBGET.bGetObject.Organism

    Public Class OrganismInfo

        ''' <summary>
        ''' T number
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property TID As String
        <XmlAttribute> Public Property code As String
        <XmlAttribute> Public Property Taxonomy As String

        Public Property Aliases As String
        Public Property FullName As String
        Public Property Definition As String
        Public Property Lineage As String
        Public Property DataSource As NamedValue()
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

            Dim infoTable = html _
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
                .ToArray

            Dim rows As New Dictionary(Of String, String)

            For Each r As NamedValue(Of String) In infoTable
                ' 因为如果基因组存在质粒的话，则会出现多个sequence字段重复
                ' 所以不可以直接使用linq生成字典
                ' 在这里只添加第一个出现的字段就行了
                ' 因为基因组序列总是先于质粒序列出现的
                If Not rows.ContainsKey(r.Name) Then
                    rows.Add(r.Name, r.Value)
                End If
            Next

            Dim comment$ = rows _
                .TryGetValue("Comment") _
                .StripHTMLTags _
                .StringReplace("\s{2,}", " ") _
                .Trim
            Dim keywords$() = rows _
                .TryGetValue("Keywords") _
               ?.Split(","c)

            Return New OrganismInfo With {
                .Aliases = rows?!Aliases,
                .code = rows("Org code"),
                .Comment = comment,
                .Created = rows!Created,
                .FullName = rows("Full name"),
                .Definition = rows!Definition,
                .Keywords = keywords,
                .Sequence = rows!Sequence.href,
                .Lineage = rows!Lineage,
                .Taxonomy = rows!Taxonomy.StripHTMLTags,
                .TID = rows("T number").StripHTMLTags,
                .DataSource = links(rows("Data source")),
                .Reference = referenceParser(rows)
            }
        End Function

        Private Shared Function links(html$) As NamedValue()
            Dim a = r.Matches(html, "<a.+?</a>", RegexICSng) _
                .EachValue(Function(s)
                               Return New NamedValue With {
                                   .name = s.StripHTMLTags,
                                   .text = s.href
                               }
                           End Function) _
                .ToArray
            Return a
        End Function

        Private Shared Function referenceParser(rows As Dictionary(Of String, String)) As Reference
            Dim J$ = rows.TryGetValue("Journal")
            Dim title$ = rows.TryGetValue("Title")
            Dim DOI$

            If J.StringEmpty AndAlso title.StringEmpty Then
                Return Nothing
            End If

            If J.StringEmpty Then
                J = ""
                DOI = ""
            Else
                DOI = r.Match(J, "DOI[:].+", RegexICSng).Value
                J = J.Replace(DOI, "").StripHTMLTags.Trim
                DOI = DOI.StripHTMLTags
            End If

            Dim authors = rows.TryGetValue("Authors")?.Split(";"c)
            Dim ref$ = rows.TryGetValue("Reference").StripHTMLTags

            Return New Reference With {
                .Title = title,
                .Authors = authors,
                .Reference = ref,
                .Journal = J,
                .DOI = DOI
            }
        End Function
    End Class
End Namespace
