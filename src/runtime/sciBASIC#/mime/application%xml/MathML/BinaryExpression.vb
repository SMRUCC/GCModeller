Imports System.ComponentModel
Imports Microsoft.VisualBasic.Language

Namespace MathML

    Public Class BinaryExpression

        Public Property [operator] As String

        Public Property applyleft As [Variant](Of BinaryExpression, String)
        Public Property applyright As [Variant](Of BinaryExpression, String)

        Public Overrides Function ToString() As String
            Return contentBuilder.ToString(Me)
        End Function

        Public Shared Function FromMathML(xmlText As String) As BinaryExpression
            Return XmlParser.ParseXml(xmlText).ParseXml
        End Function

    End Class

    Public Enum mathOperators
        <Description("+")> plus
        <Description("*")> times
        <Description("/")> divide
        <Description("^")> power
        <Description("-")> minus
    End Enum
End Namespace