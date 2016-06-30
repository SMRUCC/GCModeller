Namespace Interpreter.LDM.Expressions.Keywords

    Public Class Cd : Inherits LDM.Expressions.PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.CD
            End Get
        End Property

        Public Property Path As Parser.Tokens.InternalExpression

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace