Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions.Keywords

    Public Class [Return] : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.Return
            End Get
        End Property

        Public Property ValueExpression As InternalExpression

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace