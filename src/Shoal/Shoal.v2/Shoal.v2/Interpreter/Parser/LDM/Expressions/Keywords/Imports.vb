Namespace Interpreter.LDM.Expressions.Keywords

    ''' <summary>
    ''' Imports Namespace1, Namespace2, Namespace3 
    ''' (使用本方法导入外部命令，这样子就可以直接调用方法而不需要每一个命令行都添加模块名称了)
    ''' </summary>
    Public Class [Imports] : Inherits LDM.Expressions.PrimaryExpression

        ''' <summary>
        ''' 字符串常量或者内部表达式
        ''' </summary>
        ''' <returns></returns>
        Public Property Namespaces As String()

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.Imports
            End Get
        End Property
    End Class

End Namespace