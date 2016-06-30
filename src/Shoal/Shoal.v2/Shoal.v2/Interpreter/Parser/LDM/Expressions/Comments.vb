Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions

    Public Class Comments : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.Comments
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(Expression)

            Dim Expr As String = Trim(Expression)
            Dim Length As Integer = CommentFLAGLen(Expr) + 1
            Me.Comments = Mid(Expr, Length).Trim
        End Sub

        ''' <summary>
        ''' -1表示不是注释
        ''' </summary>
        ''' <param name="Expression"></param>
        ''' <returns></returns>
        Private Shared Function CommentFLAGLen(Expression As String) As Integer
            If Expression.First = "#"c OrElse Expression.First = "'"c OrElse Expression.First = ";"c Then
                Return 1
            End If

            If String.Equals(Expression, "rem", StringComparison.OrdinalIgnoreCase) Then
                Return 3
            End If

            If InStr(Expression, "::") = 1 OrElse InStr(Expression, "//") = 1 Then
                Return 2
            End If

            If InStr(Expression, "<!--") = 1 Then
                Return 4
            End If

            Return -1
        End Function

        Public Shared Function IsComments(TokensFirst As Token) As Boolean
            Dim Expression As String = TokensFirst.GetTokenValue
            Return CommentFLAGLen(Expression) > 0
        End Function
    End Class
End Namespace