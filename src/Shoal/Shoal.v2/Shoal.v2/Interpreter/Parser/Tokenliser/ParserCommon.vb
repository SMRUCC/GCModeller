Namespace Interpreter.Parser.TextTokenliser

    Public Module ParserCommon

        Public Const BRACKETS As String = "{[("
        Public Const PAIRED_BRACKETS As String = ")}]"""

        Public Function IsPaired(Left As Char, Right As Char) As Boolean

            Select Case Left
                Case "{"c : Return Right = "}"c
                Case "["c : Return Right = "]"c
                Case """"c : Return Right = """"c
                Case "("c : Return Right = ")"c

                Case Else
                    Dim s_MSG = $"The bracket is not paired! {NameOf(Left)}:= {Left},  {NameOf(Right)}:= {Right}"
                    Throw New SyntaxErrorException(s_MSG)

            End Select
        End Function

        ''' <summary>
        ''' 注释前导符号
        ''' </summary>
        Public Const COMMENTS As String = "#';%"
    End Module

    Public MustInherit Class Parserbase

        Public MustOverride ReadOnly Property Tokens As Tokens.Token()
    End Class
End Namespace