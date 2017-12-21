#Region "Microsoft.VisualBasic::6a5ad99b545415de77bcdddf78ec44d9, ..\GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayWebParser.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers

Namespace Assembly.KEGG.DBGET.bGetObject

    Module PathwayWebParser

        Const PATHWAY_SPLIT As String = "<a href=""/kegg-bin/show_pathway.+?"">.+?"
        Const MODULE_SPLIT As String = "<a href=""/kegg-bin/show_module.+?"">.+?"
        Const GENE_SPLIT As String = "<a href=""/dbget-bin/www_bget\?{0}:.+?"">.+?</a>"

        ''' <summary>
        ''' 这里会需要同时兼容compound和glycan这两种数据
        ''' </summary>
        Const COMPOUND_SPLIT As String = "\<a href\=""/dbget-bin/www_bget\?((cpd)|(gl)):.+?""\>.+?\</a\>.+?"

        ''' <summary>
        ''' 从某一个页面url或者文件路径所指向的网页文件之中解析出模型数据
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        <Extension> Public Function PageParser(url$) As Pathway
            Dim WebForm As New WebForm(url)

            If WebForm.Count = 0 Then
                Return Nothing
            End If

            Dim Pathway As New Pathway With {
                .Organism = WebForm.__organism,
                .EntryId = WebForm.__entryID,
                .KOpathway = WebForm.__koPathways,
                .Name = WebForm.__name,
                .Disease = __parseHTML_ModuleList(WebForm.GetValue("Disease").FirstOrDefault, LIST_TYPES.Disease),
                .PathwayMap = __parseHTML_ModuleList(WebForm.GetValue("Pathway map").FirstOrDefault, LIST_TYPES.Pathway).FirstOrDefault,
                .Description = WebForm.__description,
                .Modules = __parseHTML_ModuleList(WebForm.GetValue("Module").FirstOrDefault, LIST_TYPES.Module),
                .Genes = WebForm.parseList(WebForm.GetValue("Gene").FirstOrDefault, String.Format(GENE_SPLIT, .Organism.Key)),
                .Compound = WebForm.parseList(WebForm.GetValue("Compound").FirstOrDefault, COMPOUND_SPLIT),
                .References = WebForm.References,
                .OtherDBs = WebForm("Other DBs").FirstOrDefault.__otherDBs,
                .Drugs = WebForm("Drug").FirstOrDefault.__pathwayDrugs
            }

            Return Pathway
        End Function

        <Extension> Private Function __pathwayDrugs(html$) As KeyValuePair()
            Dim divs = html.Strip_NOBR.DivInternals
            Dim out As New List(Of KeyValuePair)

            For Each d In divs.SlideWindows(2, 2)
                out += New KeyValuePair With {
                    .Key = d(0).StripHTMLTags(stripBlank:=True),
                    .Value = d(1).StripHTMLTags(stripBlank:=True)
                }
            Next

            Return out
        End Function

        <Extension>
        Private Function __koPathways(webForm As WebForm) As KeyValuePair()
            Dim KOpathway = webForm.GetValue("KO pathway") _
                .FirstOrDefault _
                .GetTablesHTML _
                .LastOrDefault _
                .GetRowsHTML _
                .Select(Function(row$)
                            Dim cols As String() = row.GetColumnsHTML
                            Return New KeyValuePair With {
                                .Key = cols(0).StripHTMLTags.StripBlank,
                                .Value = cols(1).StripHTMLTags.StripBlank
                            }
                        End Function).ToArray
            Return KOpathway
        End Function

        <Extension> Private Function __organism(webForm As WebForm) As KeyValuePair
            Dim html As String = webForm.GetValue("Organism").FirstOrDefault
            Dim speciesCode = Regex.Match(html, "\[GN:<a href="".+?"">.+?</a>]").Value.GetValue

            html = html.StripHTMLTags(True)
            html = html.GetTagValue(vbLf).Value

            Return New KeyValuePair With {
                .Key = speciesCode,
                .Value = html
            }
        End Function

        <Extension> Private Function __description(webForm As WebForm) As String
            Dim html$ = webForm.GetValue("Description").FirstOrDefault
            html = html.StripHTMLTags(True)
            html = html.GetTagValue(vbLf).Value
            Return html
        End Function

        <Extension> Private Function __name(webForm As WebForm) As String
            Dim html$ = webForm.GetValue("Name").FirstOrDefault
            html = html.StripHTMLTags(True).GetTagValue(vbLf, True).Value
            Return html
        End Function

        <Extension> Private Function __entryID(webForm As WebForm) As String
            Dim html$ = webForm.GetValue("Entry").FirstOrDefault
            html = html.StripHTMLTags.StripBlank
            Return Regex.Match(html, "[a-z]+\d+", RegexOptions.IgnoreCase).Value
        End Function

        ''' <summary>
        ''' Pathway和Module的格式都是一样的，所以在这里通过<paramref name="type"/>参数来控制对象的类型
        ''' </summary>
        ''' <param name="html"></param>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' 
        <Extension>
        Public Function __parseHTML_ModuleList(html$, type As LIST_TYPES) As KeyValuePair()
            If String.IsNullOrEmpty(html) Then
                Return {}
            End If

            Dim splitRegex As String = ""

            Select Case type
                Case LIST_TYPES.Disease
                    splitRegex = "<a href=""/dbget-bin/www_bget\?ds:H.+?"">.+?"
                Case LIST_TYPES.Module
                    splitRegex = MODULE_SPLIT
                Case LIST_TYPES.Pathway
                    splitRegex = PATHWAY_SPLIT
            End Select

            Dim sbuf As String() = Regex _
                .Matches(html, splitRegex) _
                .ToArray _
                .Distinct _
                .ToArray
            Dim out As New List(Of KeyValuePair)

            Select Case type
                Case LIST_TYPES.Disease
                    splitRegex = "<a href=""/dbget-bin/www_bget\?ds:H.+?"">.+?</a>"
                Case LIST_TYPES.Module
                    splitRegex = "<a href=""/kegg-bin/show_module.+?"">.+?</a>"
                Case LIST_TYPES.Pathway
                    splitRegex = "<a href=""/kegg-bin/show_pathway.+?"">.+?</a>"
            End Select

            For i As Integer = 0 To sbuf.Length - 2
                Dim p1 As Integer = InStr(html, sbuf(i))
                Dim p2 As Integer = InStr(html, sbuf(i + 1))
                Dim len As Integer = p2 - p1
                Dim strTemp As String = Mid(html, p1, len)

                Dim entry As String = Regex.Match(strTemp, splitRegex).Value
                Dim func$ = strTemp.Replace(entry, "").Trim

                entry = entry.GetValue
                func = WebForm.RemoveHrefLink(func)

                out += New KeyValuePair With {
                    .Key = entry,
                    .Value = func
                }
            Next

            Dim p As Integer = InStr(html, sbuf.Last)
            html = Mid(html, p)
            Dim lastEntry As New KeyValuePair With {
                .Key = Regex.Match(html, splitRegex).Value,
                .Value = WebForm.RemoveHrefLink(html.Replace(.Key, "").Trim)
            }
            ' 由于解析value属性的时候还需要使用到key的原始字符串数据
            ' 所以key的最后解析放在初始化代码外
            lastEntry.Key = lastEntry.Key.GetValue

            Call out.Add(lastEntry)

            For Each x As KeyValuePair In out
                x.Key = x.Key.StripHTMLTags.StripBlank
                x.Value = x.Value.StripHTMLTags.StripBlank
            Next

            Return out.ToArray
        End Function

        Friend Enum LIST_TYPES As Integer
            [Module]
            [Pathway]
            [Disease]
        End Enum
    End Module
End Namespace
