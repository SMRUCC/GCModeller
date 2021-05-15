Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports Microsoft.VisualBasic.Text.Parser

Public Class TokenIcer : Inherits SyntaxTokenlizer(Of Tokens, Token)

    Public Sub New(text As [Variant](Of String, CharPtr))
        MyBase.New(text)
    End Sub

    Protected Overrides Function walkChar(c As Char) As Token
        Select Case c
            Case "{"c, "("c, "["c, "}"c, "]"c, ")"c, "|"c
                Dim t As Token = popOutToken()
                buffer += c
                Return t
            Case " "c, ASCII.TAB, ASCII.CR, ASCII.LF
                Return popOutToken()
            Case Else
                buffer += c
        End Select

        Return Nothing
    End Function

    Protected Overrides Function popOutToken() As Token
        If buffer = 0 Then
            Return Nothing
        End If

        Dim text As New String(buffer.PopAllChars)

        Select Case text
            Case "{", "[", "(" : Return New Token(Tokens.open, text)
            Case "}", "]", ")" : Return New Token(Tokens.close, text)

        End Select
    End Function
End Class

