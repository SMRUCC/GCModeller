Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions.HybridScript

    Public Class HybirdsScriptPush : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.HybirdsScriptPush
            End Get
        End Property

        ''' <summary>
        ''' 将要传递给外部变量的左端的表达式
        ''' </summary>
        ''' <returns></returns>
        Public Property InternalExpression As InternalExpression
        ''' <summary>
        ''' 右端的外部变量的名称
        ''' </summary>
        ''' <returns></returns>
        Public Property ExternalVariable As InternalExpression

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace