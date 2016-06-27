Imports System.Text.RegularExpressions
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Linq.Extensions

Namespace Components

    <XmlType("notes")> Public Class Notes
        Public Property body As Body

        Public ReadOnly Property Text As String
            Get
                If body Is Nothing Then
                    Return ""
                Else
                    Return body.Text
                End If
            End Get
        End Property

        Public ReadOnly Property Properties As [Property]()
            Get
                If body Is Nothing Then
                    Return New [Property]() {}
                Else
                    Return body.GetProperties
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Text
        End Function
    End Class

    <XmlType("body")> Public Class Body
        <XmlElement("p")> Public Property Passage As String()
        <XmlText> Public Property Text As String

        Public Function GetProperties() As [Property]()
            Return Passage.ToArray(Function(s) [Property].Parser(s))
        End Function

        Public Overrides Function ToString() As String
            Return Text
        End Function
    End Class

    Public Class [Property] : Implements IReadOnlyId, sIdEnumerable
        Implements IKeyValuePairObject(Of String, String)

        Public Property Name As String Implements IKeyValuePairObject(Of String, String).Identifier, IReadOnlyId.Identity, sIdEnumerable.Identifier
        Public Property value As String Implements IKeyValuePairObject(Of String, String).Value

        Public Overrides Function ToString() As String
            Return $"[{Name}]  {value}"
        End Function

        Public Shared Function Parser(s As String) As [Property]
            Dim i As Integer = InStr(s, ":")
            Dim name As String = Mid(s, 1, i - 1)
            Dim value As String = Mid(s, i + 1).Trim

            Return New [Property] With {
                .Name = name,
                .value = value
            }
        End Function
    End Class
End Namespace