Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace AppEngine

    Public Structure JsonResponse

        <XmlAttribute>
        Public Property code As Integer
        <XmlText>
        Public Property message As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Structure
End Namespace