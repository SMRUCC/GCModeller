Namespace Interpreter.LDM.Expressions.ControlFlows

    Public Class DoWhile : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.DoWhile
            End Get
        End Property

        Public Property BooleanIf As Parser.Tokens.InternalExpression
        Public Property Invoke As LDM.SyntaxModel

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class

    Public Class DoUntil : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.DoUntil
            End Get
        End Property

        Public Property BooleanIf As Parser.Tokens.InternalExpression
        Public Property Invoke As LDM.SyntaxModel

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace