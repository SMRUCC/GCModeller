Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace LDM.Statements

    Public Class ClosureTokens

        Public Property Token As TokenIcer.Tokens
        Public Property Tokens As Token(Of TokenIcer.Tokens)()

        Public Overrides Function ToString() As String
            Return $"[{Token}] {Tokens.ToArray(Function(x) x.TokenValue).JoinBy(" ")}"
        End Function

        '''' <summary>
        '''' 表达式栈空间的解析
        '''' </summary>
        '''' <returns></returns>
        'Public Function ParsingStack() As Func(Of TokenIcer.Tokens)
        '    Return Tokens.Parsing(TokenIcer.stackT)
        'End Function
    End Class
End Namespace