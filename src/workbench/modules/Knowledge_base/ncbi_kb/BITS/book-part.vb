Imports System.Xml.Serialization

Namespace BITS

    <XmlType("book-part")>
    Public Class BookPart

        Public Property body As body

        Public Overrides Function ToString() As String
            Return body.ToString
        End Function

    End Class

End Namespace