Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Motif.Patterns

    Public Class Ranges : Inherits IntRange

        Public Property raw As String
        Public Property MaxExpression As String

        Sub New(token As Token(Of Tokens))
            raw = token.Text

            Dim tokens As String() = raw.Split(","c)

            If tokens.Length = 2 Then
                Min = Scripting.CTypeDynamic(Of Integer)(tokens(0))
                MaxExpression = tokens(1)
                If Information.IsNumeric(MaxExpression) Then
                    Max = Scripting.CTypeDynamic(Of Integer)(MaxExpression)
                End If
            ElseIf tokens.Length = 1 Then
                MaxExpression = tokens(0)
                If Information.IsNumeric(MaxExpression) Then
                    Max = Scripting.CTypeDynamic(Of Integer)(MaxExpression)
                End If
            Else
                Throw New SyntaxErrorException(raw)
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace