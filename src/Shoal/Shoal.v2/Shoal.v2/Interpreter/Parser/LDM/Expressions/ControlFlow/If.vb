Namespace Interpreter.LDM.Expressions.ControlFlows

    Public Class [If] : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.If
            End Get
        End Property

        Public Property BooleanIf As Parser.Tokens.InternalExpression
        Public Property Invoke As LDM.SyntaxModel

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class

    Public Class [ElseIf] : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.ElseIf
            End Get
        End Property

        Public Property BooleanIf As Parser.Tokens.InternalExpression
        Public Property Invoke As LDM.SyntaxModel

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class

    Public Class [Else] : Inherits PrimaryExpression

        Public Property Invoke As LDM.SyntaxModel

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.Else
            End Get
        End Property
    End Class

End Namespace