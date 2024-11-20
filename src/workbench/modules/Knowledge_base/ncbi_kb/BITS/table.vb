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
            For Each tri As BodyRow In tr
                Yield tri.row_cells _
                    .Select(Function(d) d.GetContentText.Trim) _
                    .ToArray
            Next
        End Function

    End Class

    Public Class THead

        Public Property tr As HeaderRow

        Public Iterator Function HeaderText() As IEnumerable(Of String)
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

        Public Overrides Function ToString() As String
            Return row_cells.Select(Function(td) td.ToString).GetJson
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