Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions

    Public Class VariableDeclaration : Inherits LDM.Expressions.PrimaryExpression

        ''' <summary>
        ''' The name of the variable.
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String
        ''' <summary>
        ''' 变量的类型约束
        ''' </summary>
        ''' <returns></returns>
        Public Property Type As String
        Public Property Initializer As InternalExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.VariableDeclaration
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace