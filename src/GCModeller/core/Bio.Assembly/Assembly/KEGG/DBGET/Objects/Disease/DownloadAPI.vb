#Region "Microsoft.VisualBasic::abeb900ab3bf8c8921499b4d12c41048, GCModeller\core\Bio.Assembly\Assembly\KEGG\DBGET\Objects\Disease\DownloadAPI.vb"

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

    '   Total Lines: 175
    '    Code Lines: 139
    ' Comment Lines: 13
    '   Blank Lines: 23
    '     File Size: 6.78 KB


    '     Module DownloadDiseases
    ' 
    '         Function: __drugMembers, __otherDBs, __pairList, Download, DownloadDrug
    '                   DownloadURL, MarkerList, Parse, ParseDrugData
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
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.InternalWebFormParsers
Imports SMRUCC.genomics.ComponentModel.DBLinkBuilder

Namespace Assembly.KEGG.DBGET.bGetObject

    ''' <summary>
    ''' 从KEGG之上下载疾病的信息
    ''' </summary>
    Public Module DownloadDiseases

        ''' <summary>
        ''' 解析KEGG上面的疾病模型数据的网页文本
        ''' </summary>
        ''' <param name="html$"></param>
        ''' <returns></returns>
        <Extension> Public Function Parse(html As WebForm) As Disease
            Dim dis As New Disease With {
                .Entry = html.GetText("Entry").Split.First,
                .Name = html.GetText("Name"),
                .Description = html.GetText("Description"),
                .Category = html.GetText("Category"),
                .Pathway = PathwayWebParser.__parseHTML_ModuleList(html("Pathway").FirstOrDefault, LIST_TYPES.Pathway),
                .Comment = html.GetText("Comment"),
                .References = html.References,
                .Markers = html("Marker").FirstOrDefault _
                    .DivInternals _
                    .Select(AddressOf HtmlLines) _
                    .IteratesALL _
                    .Select(Function(s) s.StripHTMLTags.StripBlank) _
                    .Where(Function(s) Not s.StringEmpty) _
                    .ToArray,
                .Genes = html("Gene").FirstOrDefault.MarkerList,
                .Drug = html("Drug").FirstOrDefault.__pairList(
                    Function(s$)
                        Dim id$ = Regex.Match(s, "\[.+?\]", RegexICSng) _
                            .Value _
                            .GetStackValue("[", "]")
                        Return New KeyValuePair With {
                            .Key = s,
                            .Value = id
                        }
                    End Function),
                .OtherDBs = html("Other DBs").FirstOrDefault.__otherDBs,
                .Carcinogen = html(NameOf(Disease.Carcinogen)) _
                    .FirstOrDefault _
                    .StripHTMLTags(stripBlank:=True)
            }

            Return dis
        End Function

        <Extension>
        Friend Function __otherDBs(html$) As DBLink()
            Dim lines$() = html _
                .DivInternals _
                .Select(Function(s) s.StripHTMLTags(stripBlank:=True)) _
                .ToArray
            Dim slides = lines.SlideWindows(2, offset:=2)
            Dim out As New List(Of DBLink)

            For Each s As SlideWindow(Of String) In slides
                Dim idList = s(1).StringSplit("\s+")

                out += idList _
                    .Where(Function(str) Not str.StringEmpty) _
                    .Select(Function(id)
                                Return New DBLink With {
                                    .DBName = s(Scan0).Trim(":"c),
                                    .Entry = id
                                }
                            End Function)
            Next

            Return out
        End Function

        <Extension>
        Friend Function __pairList(html$, parser As Func(Of String, KeyValuePair)) As KeyValuePair()
            Dim lines$() = html.DivInternals _
                .FirstOrDefault _
                .HtmlLines _
                .Select(Function(s) s.StripHTMLTags(stripBlank:=True)) _
                .Where(Function(s) Not s.StringEmpty) _
                .ToArray
            Dim out As KeyValuePair() = lines.Select(parser).ToArray
            Return out
        End Function

        <Extension> Private Function MarkerList(html$) As [Property]()
            Dim list As New List(Of [Property])

            html = html.DivInternals.FirstOrDefault

            Dim lines$() = html _
                .HtmlLines _
                .Select(Function(s) s.StripHTMLTags(stripBlank:=True)) _
                .Where(Function(s) Not s.StringEmpty) _
                .ToArray

            For Each line$ In lines
                Dim refs$() = Regex _
                    .Matches(line, "\[.+?\]", RegexICSng) _
                    .ToArray

                list += New [Property] With {
                    .name = line,
                    .value = refs(0),
                    .Comment = refs(1)
                }
            Next

            Return list
        End Function

        ''' <summary>
        ''' 使用疾病编号下载疾病数据模型
        ''' </summary>
        ''' <param name="ID$"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Download(ID$) As Disease
            Dim url$ = $"http://www.kegg.jp/dbget-bin/www_bget?ds:{ID}"
            Return DownloadURL(url)
        End Function

        Public Function DownloadURL(url$) As Disease
            Return New WebForm(url).Parse
        End Function

        <Extension> Public Function ParseDrugData(form As WebForm) As Drug
            Dim drug As New Drug With {
                .Entry = form.GetText("Entry").Split.First,
                .Name = form.GetText("Name"),
                .Comment = form.GetText("Comment"),
                .Metabolism = form.GetText("Metabolism"),
                .Remark = form.GetText("Remark"),
                .Target = form("Target").FirstOrDefault.MarkerList,
                .Members = form("Member").FirstOrDefault.__drugMembers
            }

            Return drug
        End Function

        <Extension>
        Private Function __drugMembers(html$) As KeyValuePair()
            Dim t = html.GetTablesHTML
            Dim out As New List(Of KeyValuePair)
            Dim rows = t.Select(Function(s) s.GetRowsHTML).IteratesALL.ToArray

            For Each row$ In rows
                Dim cols = row.GetColumnsHTML
                If cols.Length >= 2 Then
                    out += New KeyValuePair With {
                        .Key = cols(Scan0).StripHTMLTags.StripBlank,
                        .Value = cols(1).StripHTMLTags.StripBlank
                    }
                End If
            Next

            Return out
        End Function

        Public Function DownloadDrug(url$) As Drug
            Return New WebForm(url).ParseDrugData
        End Function
    End Module
End Namespace
