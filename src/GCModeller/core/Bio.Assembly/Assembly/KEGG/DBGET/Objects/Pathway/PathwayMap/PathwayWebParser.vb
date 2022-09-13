#Region "Microsoft.VisualBasic::f065a848b6239a0ab0f3f07f259e5d9e, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Pathway\PathwayMap\PathwayWebParser.vb"

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


    ' Code Statistics:

    '   Total Lines: 230
    '    Code Lines: 174
    ' Comment Lines: 20
    '   Blank Lines: 36
    '     File Size: 9.53 KB


    '     Module PathwayWebParser
    ' 
    '         Function: __description, __entryID, __koPathways, __name, __organism
    '                   __parseHTML_ModuleList, __pathwayDrugs, PageParser, parseOrthologyTerms
    '         Enum LIST_TYPES
    ' 
    '             [Disease], [Module], [Pathway]
    ' 
    ' 
    ' 
    '  
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.base
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports XmlProperty = Microsoft.VisualBasic.Text.Xml.Models.Property

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' Html parser for <see cref="Pathway"/>
    ''' </summary>
    Module PathwayWebParser

        Const PATHWAY_SPLIT As String = "<a href=""((/kegg-bin/show_pathway)|(/pathway/)).+?"">.+?"
        Const MODULE_SPLIT As String = "<a href=""/kegg-bin/show_module.+?"">.+?"
        Const GENE_SPLIT As String = "<a href=""/dbget-bin/www_bget\?{0}:.+?"">.+?</a>"

        ''' <summary>
        ''' 这里会需要同时兼容compound和glycan这两种数据
        ''' </summary>
        Const COMPOUND_SPLIT As String = "\<a href\=""(/dbget-bin/www_bget\?((cpd)|(gl)):.+?)|(/entry/.+?)""\>.+?\</a\>.+?"

        <Extension>
        Friend Function parseOrthologyTerms(data As IEnumerable(Of NamedValue)) As OrthologyTerms
            Dim terms As XmlProperty() = data.SafeQuery _
                .Select(Function(t)
                            Dim valueTuple = t.text.GetTagValue(";")
                            Dim note$ = Strings.Trim(valueTuple.Value)

                            Return New XmlProperty(t.name, valueTuple.Name, note)
                        End Function) _
                .ToArray

            Return New OrthologyTerms With {
                .Terms = terms
            }
        End Function

        ''' <summary>
        ''' 从某一个页面url或者文件路径所指向的网页文件之中解析出模型数据
        ''' </summary>
        ''' <param name="url"></param>
        ''' <returns></returns>
        <Extension>
        Public Function PageParser(url$) As Pathway
            Dim WebForm As New WebForm(url)

            If WebForm.Count = 0 Then
                Return Nothing
            End If

            Dim compounds = WebForm.parseList(WebForm.GetValue("Compound").FirstOrDefault, COMPOUND_SPLIT).ToArray
            Dim Pathway As New Pathway With {
                .organism = WebForm.__organism,
                .EntryId = WebForm.__entryID,
                .KOpathway = WebForm.__koPathways,
                .name = WebForm.__name,
                .disease = __parseHTML_ModuleList(WebForm.GetValue("Disease").FirstOrDefault, LIST_TYPES.Disease),
                .pathwayMap = __parseHTML_ModuleList(WebForm.GetValue("Pathway map").FirstOrDefault, LIST_TYPES.Pathway).FirstOrDefault,
                .description = WebForm.__description,
                .modules = __parseHTML_ModuleList(WebForm.GetValue("Module").FirstOrDefault, LIST_TYPES.Module),
                .genes = WebForm.parseList(WebForm.GetValue("Gene").FirstOrDefault, String.Format(GENE_SPLIT, .organism.Key)).ToArray,
                .compound = compounds,
                .references = WebForm.References,
                .otherDBs = WebForm("Other DBs").FirstOrDefault.__otherDBs,
                .drugs = WebForm("Drug").FirstOrDefault.__pathwayDrugs
            }

            Return Pathway
        End Function

        <Extension> Private Function __pathwayDrugs(html$) As NamedValue()
            Dim divs = html.Strip_NOBR.DivInternals
            Dim out As New List(Of NamedValue)

            For Each d In divs.SlideWindows(2, 2)
                out += New NamedValue With {
                    .name = d(0).StripHTMLTags(stripBlank:=True),
                    .text = d(1).StripHTMLTags(stripBlank:=True)
                }
            Next

            Return out
        End Function

        <Extension>
        Private Function __koPathways(webForm As WebForm) As NamedValue()
            Dim KOpathway = webForm.GetValue("KO pathway") _
                .FirstOrDefault _
                .GetTablesHTML _
                .LastOrDefault _
                .GetRowsHTML _
                .Select(Function(row$)
                            Dim cols As String() = row.GetColumnsHTML
                            Return New NamedValue With {
                                .name = cols(0).StripHTMLTags.StripBlank,
                                .text = cols(1).StripHTMLTags.StripBlank
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
        Public Function __parseHTML_ModuleList(html$, type As LIST_TYPES) As NamedValue()
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
            Dim out As New List(Of NamedValue)

            Select Case type
                Case LIST_TYPES.Disease
                    splitRegex = "<a href=""/dbget-bin/www_bget\?ds:H.+?"">.+?</a>"
                Case LIST_TYPES.Module
                    splitRegex = "<a href=""/kegg-bin/show_module.+?"">.+?</a>"
                Case LIST_TYPES.Pathway
                    splitRegex = "<a href=""((/kegg-bin/show_pathway)|(/pathway/)).+?"">.+?</a>"
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

                out += New NamedValue With {
                    .name = entry,
                    .text = func
                }
            Next

            Dim lastP As String = If(sbuf.Length = 0, "", sbuf.Last)
            Dim p As Integer = InStr(html, lastP)
            html = Mid(html, p)

            Dim nameStr As String = Regex.Match(html, splitRegex).Value
            Dim lastEntry As New NamedValue With {
                .name = nameStr,
                .text = If(.name.StringEmpty, "", html.Replace(.name, "")) _
                    .Trim _
                    .DoCall(AddressOf WebForm.RemoveHrefLink)
            }
            ' 由于解析value属性的时候还需要使用到key的原始字符串数据
            ' 所以key的最后解析放在初始化代码外
            lastEntry.name = lastEntry.name.GetValue

            Call out.Add(lastEntry)

            For Each x As NamedValue In out
                x.name = x.name.StripHTMLTags.StripBlank
                x.text = x.text.StripHTMLTags.StripBlank
            Next

            Return out _
                .Where(Function(t) Not t.name.StringEmpty) _
                .ToArray
        End Function

        Friend Enum LIST_TYPES As Integer
            [Module]
            [Pathway]
            [Disease]
        End Enum
    End Module
End Namespace
