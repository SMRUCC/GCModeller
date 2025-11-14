#Region "Microsoft.VisualBasic::4ee1c9c5e2eb69e0f6c79e66fa3aeb68, data\RegulonDatabase\Regprecise\WebServices\WebParser\Regulator\RegulatorQuery.vb"

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

    '   Total Lines: 156
    '    Code Lines: 121 (77.56%)
    ' Comment Lines: 6 (3.85%)
    '    - Xml Docs: 83.33%
    ' 
    '   Blank Lines: 29 (18.59%)
    '     File Size: 6.79 KB


    '     Class RegulatorQuery
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: basicParser, doParseGuid, doParseObject, doParseUrl, exportServlet
    '                   getTagValue, getTagValue_td
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Text.Parser.HtmlParser
Imports Microsoft.VisualBasic.Text.Xml.Models
Imports SMRUCC.genomics.Data.Regtransbase.WebServices
Imports r = System.Text.RegularExpressions.Regex

Namespace Regprecise

    Friend Class RegulatorQuery : Inherits WebQueryModule(Of String)

        Public Sub New(<CallerMemberName>
                       Optional cache As String = Nothing,
                       Optional interval As Integer = -1,
                       Optional offline As Boolean = False)

            MyBase.New(cache:=cache, interval:=interval, offline:=offline)
        End Sub

        ' 因为在这里是进行Web请求，所以为了降低目标服务器的压力，在这里可以牺牲掉代码的执行效率

        Protected Overrides Function doParseUrl(context As String) As String
            Return basicParser(context, Nothing).regulator.text
        End Function

        Protected Overrides Function doParseGuid(context As String) As String
            Dim info = basicParser(context, Nothing).regulator
            Dim guid As String = $"[{info.text.Split("="c).Last}]{info.name}"

            Return guid
        End Function

        ''' <summary>
        ''' 解析基本的信息
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        Friend Shared Function basicParser(str As String, regulator As Regulator) As Regulator
            Dim list$() = r.Matches(str, "<td.+?</td>").ToArray
            Dim i As i32 = Scan0

            If regulator Is Nothing Then
                regulator = New Regulator
            End If

            regulator.type = If(InStr(list(++i), " RNA "), Types.RNA, Types.TF)

            Dim entry As String = r.Match(list(++i), "href="".+?"">.+?</a>").Value
            Dim url As String = "https://regprecise.lbl.gov/" & entry.href
            regulator.regulator = New NamedValue With {
                .name = RegulomeQuery.GetsId(entry),
                .text = url
            }
            regulator.effector = getTagValue(list(++i))
            regulator.pathway = getTagValue(list(++i))

            Return regulator
        End Function

        Protected Overrides Function doParseObject(html As String, schema As Type) As Object
            Dim infoTable$ = html.Match("<table class=""proptbl"">.+?</table>", RegexOptions.Singleline)
            Dim properties$() = r.Matches(infoTable, "<tr>.+?</tr>", RegexICSng).ToArray
            Dim i As i32 = 0
            Dim regulator As New Regulator

            regulator.type = If(InStr(properties(++i), "RNA regulatory element") > 0, Types.RNA, Types.TF)

            With r.Match(html, "\[<a href="".+?"">see more</a>\]", RegexOptions.IgnoreCase).Value
                If Not .StringEmpty Then
                    regulator.infoURL = $"https://regprecise.lbl.gov/{ .href}"
                End If
            End With

            If regulator.type = Types.TF Then
                Dim LocusTags As String() = r _
                    .Matches(properties(++i), "href="".+?"">.+?</a>", RegexOptions.Singleline) _
                    .ToArray

                If LocusTags.IsNullOrEmpty Then
                    Dim tmp As String = properties(CInt(i) - 1).GetColumnsHTML.ElementAtOrDefault(1)

                    If tmp = "" Then
                        regulator.locus_tags = {}
                    Else
                        LocusTags = tmp.StringSplit("[;,]")

                        regulator.locus_tags = LocusTags.Select(Function(str)
                                                                    Return New NamedValue(str)
                                                                End Function).ToArray
                    End If
                Else
                    regulator.locus_tags = LocusTags.Select(Function(str)
                                                                Return New NamedValue With {
                        .name = RegulomeQuery.GetsId(str),
                        .text = str.href
                    }
                                                            End Function).ToArray
                End If

                regulator.family = getTagValue_td(properties(++i).Replace("<td>Regulator family:</td>", ""))
            Else
                Dim Name As String = r.Matches(properties(++i), "<td>.+?</td>", RegexICSng).ToArray.Last
                Name = Mid(Name, 5)
                Name = Mid(Name, 1, Len(Name) - 5)
                regulator.locus_tags = {New NamedValue With {
                    .name = Name,
                    .text = ""
                }}
                regulator.family = r.Match(infoTable, "<td class=""[^""]+?"">RFAM:</td>[^<]+?<td>.+?</td>", RegexOptions.Singleline).Value
                regulator.family = getTagValue_td(regulator.family)
                i += 1
            End If

            If regulator.family.StringEmpty() Then

            End If

            regulator.regulationMode = getTagValue_td(properties(++i))
            regulator.biological_process = getTagValue_td(properties(++i)).StringSplit("\s*;\s*")

            Dim regulogEntry$ = r.Match(properties(i + 1), "href="".+?"">.+?</a>", RegexOptions.Singleline).Value
            Dim url As String = "https://regprecise.lbl.gov/" & regulogEntry.href

            regulator.regulog = New NamedValue With {
                .name = RegulomeQuery _
                    .GetsId(regulogEntry) _
                    .TrimNewLine("") _
                    .Replace(vbTab, "") _
                    .Trim,
                .text = url
            }

            Dim exportServletLnks$() = exportServlet(html)
            Dim motifFile$ = exportServletLnks.ElementAtOrDefault(1)
            Dim cache$ = motifFile.Replace("https://regprecise.lbl.gov/", "").NormalizePathString(True).Replace("_", "/")

            cache = $"{Me.cache}/{cache}.txt"

            If Not cache.FileLength > 10 Then
                Call motifFile.GET.SaveTo(cache)
            End If

            regulator.operons = OperonQuery.OperonParser(html, Nothing)
            regulator.regulatorySites = MotifFasta.Parse(url:=cache)

            Return regulator
        End Function

        Private Shared Function getTagValue_td(strData As String) As String
            strData = r.Match(strData, "<td>.+?</td>", RegexOptions.Singleline).Value
            If String.IsNullOrEmpty(Strings.Trim(strData)) Then
                Return ""
            End If
            strData = Mid(strData, 5)
            strData = Mid(strData, 1, Len(strData) - 5)
            Return strData
        End Function

        Private Shared Function exportServlet(pageContent As String) As String()
            Dim url As String = Regex.Match(pageContent, "<table class=""tblexport"">.+?</table>", RegexOptions.Singleline).Value
            Dim links$() = r.Matches(url, "<tr>.+?</tr>", RegexICSng).ToArray

            links = links _
                .Select(Function(s) Regex.Match(s, "href="".+?""><b>DOWNLOAD</b>").Value) _
                .Select(Function(s) "https://regprecise.lbl.gov/" & s.href) _
                .ToArray

            Return links
        End Function

        Private Shared Function getTagValue(s As String) As String
            s = Regex.Match(s, """>.+?</td>").Value
            s = Mid(s, 3)
            s = Mid(s, 1, Len(s) - 5)
            Return Strings.Trim(s)
        End Function
    End Class
End Namespace
