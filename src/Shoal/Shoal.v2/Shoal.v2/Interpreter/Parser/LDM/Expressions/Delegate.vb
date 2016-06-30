Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions

    Public Class [Delegate] : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.Delegate
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub

        Public Property FuncPointer As String
        Public Property FuncExpr As LDM.SyntaxModel

    End Class
End Namespace