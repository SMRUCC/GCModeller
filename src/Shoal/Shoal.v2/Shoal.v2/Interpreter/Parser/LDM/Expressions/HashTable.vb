Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions

    Public Class HashTable : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.HashTable
            End Get
        End Property

        Public Property Table As InternalExpression
        Public Property Key As InternalExpression
        Public Property LeftAssignedVariable As LeftAssignedVariable

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace