#Region "Microsoft.VisualBasic::f802b624648675c019f2af695f383742, ..\GCModeller\data\RegulonDatabase\Regprecise\WebServices\WebParser\Operon.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
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

Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports r = System.Text.RegularExpressions.Regex

Namespace Regprecise

    ''' <summary>
    ''' Operon that regulated in a regulon
    ''' </summary>
    Public Class Operon

        <XmlAttribute("id")>
        Public Property ID As String
        <XmlElement>
        Public Property members As RegulatedGene()

        Public Overrides Function ToString() As String
            With members _
                .Where(Function(g)
                           Return Not g.Name.StringEmpty
                       End Function) _
                .Select(Function(g) g.Name) _
                .ToArray

                If Not .IsNullOrEmpty Then
                    Return .GetJson
                Else
                    Return members _
                        .Select(Function(g) g.LocusId) _
                        .ToArray _
                        .GetJson
                End If
            End With
        End Function

        Friend Shared Function OperonParser(page As String) As Operon()
            Dim tokens$()
            Dim locus As Dictionary(Of String, String)

            page = r.Match(page, "<table id=""operontbl"">.+?</table>", RegexICSng).Value
            tokens = r.Matches(page, "<tr>.+?</tr>", RegexOptions.IgnoreCase Or RegexOptions.Singleline).ToArray
            locus = __locusParser(page)
            tokens = (From row As String In tokens Where InStr(row, "<div class=""operon"">") > 0 Select row).ToArray

            Dim operons As Operon() = tokens _
                .Select(Function(value) __operonParser(value, locus)) _
                .ToArray
            Return operons
        End Function

        Private Shared Function __operonParser(value$, locus As Dictionary(Of String, String)) As Operon
            Dim genes() = r.Matches(value, "<span>.+?</span>", RegexOptions.IgnoreCase Or RegexOptions.Singleline).ToArray
            genes = (From s As String In genes Where InStr(s, "Locus", CompareMethod.Text) > 0 Select s).ToArray

            Try
                Dim list_genes As RegulatedGene() = genes _
                    .Select(Function(s) __geneParser(s, locus)) _
                    .ToArray
                Return New Operon With {
                    .members = list_genes
                }
            Catch ex As Exception
                ex = New Exception(genes.GetJson, ex)
                Throw ex
            End Try
        End Function

        '<span> Locus tag: AB57_3864<br>Name: yciC<br>Funciton: Putative metal chaperone, GTPase Of COG0523 family
        '</span>

        Private Shared Function __geneParser(value$, locus As Dictionary(Of String, String)) As RegulatedGene
            value = Mid(value, 8)
            value = Mid(value, 1, Len(value) - 8)
            value = value.TrimNewLine("")
            value = value.Replace(vbTab, "").Trim

            Dim tokens As String() = r.Split(value, "<\s*br\s*/>", RegexOptions.IgnoreCase)
            Dim locusId As String = tokens(Scan0)
            Dim name As String = tokens(1)
            Dim func As String = tokens(2)
            locusId = Mid(locusId, 11).Trim
            name = Mid(name, 6).Trim
            func = Mid(func, 10).Trim
            Dim vmssid As String = locus.TryGetValue(locusId)
            Dim gene As New RegulatedGene With {
                .description = func,
                .LocusId = locusId,
                .Name = name,
                .vimssId = vmssid
            }
            Return gene
        End Function

        Private Shared Function __locusParser(page As String) As Dictionary(Of String, String)
            Dim locus As String() = Regex.Matches(page, "<a href="".+?"">.+?</a>", RegexOptions.IgnoreCase Or RegexOptions.Singleline).ToArray
            Dim dict = (From s As String In locus
                        Let id As String = s.GetValue, url As String = s.href
                        Let vimssid As String = MicrobesOnline.locusId(url)
                        Where Not String.IsNullOrEmpty(vimssid)
                        Select id, vimssid
                        Group By id Into Group) _
                            .ToDictionary(Function(x) x.id, Function(x) x.Group.First.vimssid)
            Return dict
        End Function

        Public Shared Function PageParser(url As String) As Operon()
            Dim page As String = url.GET
            Return OperonParser(page)
        End Function
    End Class
End Namespace
