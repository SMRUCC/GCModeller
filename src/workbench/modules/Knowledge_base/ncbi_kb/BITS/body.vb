#Region "Microsoft.VisualBasic::8ddc034e18b414001b2a7a3654f02bb3, modules\Knowledge_base\ncbi_kb\BITS\body.vb"

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

    '   Total Lines: 63
    '    Code Lines: 39 (61.90%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 24 (38.10%)
    '     File Size: 1.75 KB


    '     Class body
    ' 
    '         Properties: sections
    ' 
    '         Function: ToString
    ' 
    '     Class section
    ' 
    '         Properties: id, list, p, ref_list, sec_type
    '                     sections, table_wrap, title
    ' 
    '         Function: GetContentText, ToString
    ' 
    '     Class list_data
    ' 
    '         Properties: list_item, list_type
    ' 
    '     Class list_item
    ' 
    '         Properties: label, p
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace BITS

    Public Class body

        <XmlElement("sec")> Public Property sections As section()

        Public Overrides Function ToString() As String
            Return sections.Select(Function(sec) sec.ToString).GetJson
        End Function

    End Class

    <XmlType("sec")>
    Public Class section

        <XmlAttribute("id")> Public Property id As String
        <XmlAttribute("sec-type")> Public Property sec_type As String

        Public Property title As Paragraph

        <XmlElement("p")>
        Public Property p As Paragraph()

        <XmlElement("table-wrap")> Public Property table_wrap As TableWrap

        <XmlElement("sec")> Public Property sections As section()

        <XmlElement("ref-list")> Public Property ref_list As RefList

        <XmlElement> Public Property list As list_data()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function GetContentText() As String
            Return p.SafeQuery.Select(Function(pi) pi.text.JoinBy(" ")).JoinBy(vbCrLf & vbCrLf)
        End Function

        Public Overrides Function ToString() As String
            Return title.ToString
        End Function

    End Class

    Public Class list_data

        <XmlAttribute("list-type")> Public Property list_type As String

        <XmlElement("list-item")> Public Property list_item As list_item()

    End Class

    Public Class list_item

        Public Property label As String
        <XmlElement("p")> Public Property p As Paragraph()

    End Class

End Namespace
