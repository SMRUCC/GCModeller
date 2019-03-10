Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions.Driver

    ''' <summary>
    ''' var &lt; (typeID) {expression}
    ''' </summary>
    Public Class DynamicsCast : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.DynamicsCast
            End Get
        End Property

        Public Property LeftAssigned As LeftAssignedVariable
        ''' <summary>
        ''' 去掉了外层的括号了的
        ''' </summary>
        ''' <returns></returns>
        Public Property TypeID As InternalExpression
        Public Property SourceExpr As InternalExpression

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace