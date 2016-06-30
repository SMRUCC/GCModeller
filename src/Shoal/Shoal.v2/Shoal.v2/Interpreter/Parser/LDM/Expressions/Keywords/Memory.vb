Namespace Interpreter.LDM.Expressions.Keywords

    Public Class Memory : Inherits LDM.Expressions.PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.Memory
            End Get
        End Property

        ''' <summary>
        ''' 想要查看的变量详细信息的变量的名称，为空则打印出所有变量的摘要
        ''' </summary>
        ''' <returns></returns>
        Public Property var As String

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace