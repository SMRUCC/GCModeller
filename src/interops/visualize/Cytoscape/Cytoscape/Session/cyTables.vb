Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace Session

    Public Class cyTables : Implements Enumeration(Of virtualColumn)

        Public Property virtualColumns As virtualColumn()

        Public Iterator Function GenericEnumerator() As IEnumerator(Of virtualColumn) Implements Enumeration(Of virtualColumn).GenericEnumerator
            For Each item In virtualColumns
                Yield item
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of virtualColumn).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class

    Public Class virtualColumn

        <XmlAttribute> Public Property name As String
        <XmlAttribute> Public Property sourceColumn As String
        <XmlAttribute> Public Property sourceTable As String
        <XmlAttribute> Public Property sourceJoinKey As String
        <XmlAttribute> Public Property targetTable As String
        <XmlAttribute> Public Property targetJoinKey As String
        <XmlAttribute> Public Property immutable As Boolean

        Public Overrides Function ToString() As String
            Return sourceTable
        End Function

    End Class
End Namespace