Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions

    ''' <summary>
    ''' 在解释器阶段由于缺少信息还无法判断表达式的类型，则这些语句都被设定为动态的类型
    ''' </summary>
    Public Class DynamicsExpression : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.DynamicsExpression
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub

        Public ReadOnly Property IsVariable As Boolean
            Get
                Return Me._PrimaryExpression.First = "$"c
            End Get
        End Property

        Public ReadOnly Property IsConstant As Boolean
            Get
                Return Me._PrimaryExpression.First = "&"c
            End Get
        End Property

    End Class
End Namespace