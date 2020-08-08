Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace Text.Parser

    Public MustInherit Class SyntaxTokenlizer(Of T As IComparable, Token As CodeToken(Of T))

        Protected ReadOnly text As CharPtr
        Protected buffer As CharBuffer

        Sub New(text As [Variant](Of String, CharPtr))
            If text Like GetType(String) Then
                Me.text = New CharPtr(text.TryCast(Of String))
            Else
                Me.text = text.TryCast(Of CharPtr)
            End If
        End Sub

        Public Overridable Iterator Function GetTokens(Of SyntaxToken As Token)() As IEnumerable(Of SyntaxToken)
            Dim token As New Value(Of SyntaxToken)

            Do While text
                If Not token = walkChar(Of SyntaxToken)(++text) Is Nothing Then
                    Yield CType(token, SyntaxToken)
                End If
            Loop

            If buffer > 0 Then
                Yield popOutToken(Of SyntaxToken)()
            End If
        End Function

        Protected MustOverride Function walkChar(Of SyntaxToken As Token)(c As Char) As SyntaxToken
        Protected MustOverride Function popOutToken(Of SyntaxToken As Token)() As SyntaxToken
    End Class
End Namespace