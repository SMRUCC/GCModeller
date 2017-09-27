#Region "Microsoft.VisualBasic::f46b8badf058cbaae74bccb0edf7bd67, ..\GCModeller\data\RegulonDatabase\Regprecise\WebServices\WebParser\Motif\MotifWebAPI.vb"

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
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Text.HtmlParser
Imports SMRUCC.genomics.Data.Regtransbase.WebServices
Imports r = System.Text.RegularExpressions.Regex

Namespace Regprecise

    Public Module MotifWebAPI

        Public Const RegPrecise As String = "http://regprecise.lbl.gov/RegPrecise/"

        Const __logo As String = "<div id=""logoblock"">.+?</div>"
        Const __sites As String = "<a href=""[^""]+?""><b>DOWNLOAD</b></a>"
        Const __table As String = "<tbody>.+?</tbody>"

        Public Function Download(url As String) As MotifSitelog
            Dim html As String = url.GET
            Dim logo As String = r.Match(html, __logo, RegexOptions.Singleline).Value
            logo = r.Match(logo, "src="".+?""", RegexOptions.Singleline).Value
            logo = RegPrecise & Mid(logo, 6, logo.Length - 6)

            Dim propTable As String = r.Matches(html, __table, RegexOptions.Singleline).ToArray.First
            Dim rows As Pointer(Of String) = r _
                .Matches(propTable, "<tr>.+?</tr>", RegexOptions.Singleline) _
                .ToArray _
                .MarshalAs

            Dim motif As New MotifSitelog With {
                .Family = (++rows).__getValue,
                .RegulationMode = (++rows).__getValue,
                .BiologicalProcess = (++rows).__getValue,
                .Effector = (++rows).__getValue,
                .Regulog = (++rows).__getValue.__getEntry,
                .logo = logo
            }

            Dim sites As String = r.Match(html, __sites, RegexOptions.Singleline).Value

            sites = RegPrecise & sites.href
            motif.Taxonomy = html.__getTaxonomy
            motif.Sites = FastaObject.Parse(url:=sites)

            Return motif
        End Function

        <Extension>
        Private Function __getTaxonomy(html As String) As KeyValuePair
            html = Regex.Match(html, "By taxonomy -.+?</li>", RegexOptions.Singleline).Value
            html = Regex.Match(html, "<a.+?</a>", RegexOptions.Singleline).Value

            Dim key As String = html.GetValue
            Dim value As String = RegPrecise & html.href
            Return New KeyValuePair(key, value)
        End Function

        <Extension>
        Private Function __getEntry(value As String) As KeyValuePair
            Dim key As String = value _
                .GetValue _
                .TrimNewLine("") _
                .Trim
            value = RegPrecise & value.href
            Return New KeyValuePair(key, value)
        End Function

        <Extension>
        Private Function __getValue(row As String) As String
            Dim tokens As String() = Regex.Matches(row, "<td.+?</td>", RegexOptions.Singleline).ToArray
            Return tokens.Last.GetValue
        End Function
    End Module
End Namespace
