Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions.Driver

    ''' <summary>
    ''' 解析出来的表达式之中只含有一个词元，并且不是注释，则默认认为是变量查看操作，值默认赋值给系统变量$
    ''' </summary>
    Public Class OutDeviceRef : Inherits PrimaryExpression

        Public ReadOnly Property InnerExpression As InternalExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.OutDeviceRef
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(Expression)
            _InnerExpression = New InternalExpression(Expression)
        End Sub
    End Class
End Namespace