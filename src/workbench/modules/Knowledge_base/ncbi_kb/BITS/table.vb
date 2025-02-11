#Region "Microsoft.VisualBasic::31b9e19d71ab87a655c4117374878965, modules\Knowledge_base\ncbi_kb\BITS\table.vb"

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

    '   Total Lines: 112
    '    Code Lines: 78 (69.64%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 34 (30.36%)
    '     File Size: 3.20 KB


    '     Class TableWrap
    ' 
    '         Properties: id, orientation, position, table
    ' 
    '     Class Table
    ' 
    '         Properties: frame, rules, tbody, thead
    ' 
    '     Class TBody
    ' 
    '         Properties: tr
    ' 
    '         Function: RowText
    ' 
    '     Class THead
    ' 
    '         Properties: tr
    ' 
    '         Function: HeaderText, ToString
    ' 
    '     Class HeaderRow
    ' 
    '         Properties: header_cells
    ' 
    '         Function: ToString
    ' 
    '     Class BodyRow
    ' 
    '         Properties: header_cells, row_cells
    ' 
    '         Function: ToString
    ' 
    '     Class Cell
    ' 
    '         Properties: align, colspan, headers, id, rowspan
    '                     scope, valign
    ' 
    '         Function: GetContentText, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace BITS

    Public Class TableWrap

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property orientation As String
        <XmlAttribute> Public Property position As String

        Public Property table As Table

    End Class

    Public Class Table

        <XmlAttribute> Public Property frame As String
        <XmlAttribute> Public Property rules As String

        Public Property thead As THead
        Public Property tbody As TBody

    End Class

    Public Class TBody

        <XmlElement("tr")> Public Property tr As BodyRow()

        Public Iterator Function RowText() As IEnumerable(Of String())
            If tr Is Nothing Then
                Return
            End If

            For Each tri As BodyRow In tr
                Dim cells = If(tri.row_cells, tri.header_cells)

                Yield cells _
                    .Select(Function(d) d.GetContentText.Trim) _
                    .ToArray
            Next
        End Function

    End Class

    Public Class THead

        Public Property tr As HeaderRow

        Public Iterator Function HeaderText() As IEnumerable(Of String)
            If tr.header_cells Is Nothing Then
                Return
            End If

            For Each th In tr.header_cells
                Yield th.GetContentText.Trim
            Next
        End Function

        Public Overrides Function ToString() As String
            Return tr.ToString
        End Function

    End Class

    Public Class HeaderRow

        <XmlElement("th")> Public Property header_cells As Cell()

        Public Overrides Function ToString() As String
            Return header_cells.Select(Function(th) th.GetContentText).GetJson
        End Function

    End Class

    Public Class BodyRow

        <XmlElement("td")> Public Property row_cells As Cell()
        <XmlElement("th")> Public Property header_cells As Cell()

        Public Overrides Function ToString() As String
            Return If(row_cells, header_cells).Select(Function(td) td.ToString).GetJson
        End Function

    End Class

    Public Class Cell : Inherits Paragraph

        <XmlAttribute> Public Property id As String
        <XmlAttribute> Public Property valign As String
        <XmlAttribute> Public Property align As String
        <XmlAttribute> Public Property scope As String
        <XmlAttribute> Public Property rowspan As String
        <XmlAttribute> Public Property colspan As String
        <XmlAttribute> Public Property headers As String

        Public Function GetContentText() As String
            If Not text.IsNullOrEmpty Then
                Return text.JoinBy(" ")
            ElseIf Not links.IsNullOrEmpty Then
                Return links.Select(Function(a) a.text).JoinBy(" ")
            Else
                Return ""
            End If
        End Function

        Public Overrides Function ToString() As String
            Return text.JoinBy(" ")
        End Function

    End Class
End Namespace
