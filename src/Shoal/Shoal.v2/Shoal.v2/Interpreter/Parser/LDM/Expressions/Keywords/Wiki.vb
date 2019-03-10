Imports System.ComponentModel

Namespace Interpreter.LDM.Expressions.Keywords

    <Description("?")>
    Public Class Wiki : Inherits LDM.Expressions.PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.Wiki
            End Get
        End Property

        Public Property [Object] As String

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace