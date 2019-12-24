Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace ComponentModel.DBLinkBuilder

    <XmlType("synonym")>
    Public Class Synonym : Implements Enumeration(Of String)

        <XmlAttribute> Public Property accessionID As String
        <XmlElement> Public Property [alias] As String()

        Public Overrides Function ToString() As String
            Return accessionID
        End Function

        Public Iterator Function GenericEnumerator() As IEnumerator(Of String) Implements Enumeration(Of String).GenericEnumerator
            Yield accessionID

            For Each id As String In [alias]
                Yield id
            Next
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator Implements Enumeration(Of String).GetEnumerator
            Yield GenericEnumerator()
        End Function
    End Class
End Namespace