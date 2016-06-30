Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.Parser.Tokens

Namespace Interpreter.LDM.Expressions

    ''' <summary>
    ''' 只是调用方法，函数的返回值直接返回给系统变量$
    ''' 方法返回Nothing
    ''' 函数返回函数值
    ''' </summary>
    Public Class FunctionCalls : Inherits PrimaryExpression

        Public Overrides ReadOnly Property ExprTypeID As ExpressionTypes
            Get
                Return ExpressionTypes.FunctionCalls
            End Get
        End Property

        ''' <summary>
        ''' 对于<see cref="PrimaryExpression"/>类型而言，其不仅仅调用一个方法，而且还将方法的返回值赋值给一个指定的左端变量，
        ''' 由于变量之间赋值传递的情况也可能存在，故而这个属性也可能是引用一个内存地址，当找不到方法的时候，就会通过这个参数来查找内存变量
        ''' </summary>
        ''' <returns></returns>
        Public Property EntryPoint As Parser.Tokens.EntryPoint

        ''' <summary>
        ''' 解析得到的顺序应该和原始的脚本语句是一致的
        ''' </summary>
        ''' <returns></returns>
        Public Property Parameters As Dictionary(Of ParameterName, InternalExpression)
        Public Property LeftAssignedVariable As LeftAssignedVariable
        Public Property [Operator] As [Operator]
        Public Property ExtensionVariable As InternalExpression

        ''' <summary>
        ''' 只有左端引用表达式不为空，其他的元素都为空
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property IsVariable As Boolean
            Get
                If Not [Operator] Is Nothing Then
                    Return False
                End If
                If Not EntryPoint Is Nothing Then
                    Return False
                End If
                If Not Parameters.IsNullOrEmpty Then
                    Return False
                End If

                Return Not LeftAssignedVariable Is Nothing
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(Expression)
        End Sub
    End Class
End Namespace