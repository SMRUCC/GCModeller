Namespace Interpreter.Parser.Tokens

    ''' <summary>
    ''' 开关参数：只适用于逻辑值参数，有表示True，没有则表示False，开关参数使用-或者--或者\或者/开头
    ''' 例如有如下的函数定义
    ''' Function(a As Object, b as Boolean) 
    ''' 则调用的时候可以有下面的形式
    ''' Function a $a b T/F/True/False/1/0/yesy/no
    ''' 或者开关形式
    ''' Function a $a -b 或者 --b 或者 /b 或者 \b
    ''' 当然也可以
    ''' Call $a -> Function True/False/yes/No/1/0/T/F
    ''' Call $a -> Function /a 或者 \a 或者 -a 或者 --a
    ''' </summary>
    Public Class ParameterName : Inherits InternalExpression

        ''' <summary>
        ''' 参数名出了普通类型的参数名需要填充参数名之外，其他类型的参数名都可以留空
        ''' </summary>
        Public Enum ParameterType
            Normal = 0
            ''' <summary>
            ''' 拓展函数的调用参数，即函数定义之中的第一个参数
            ''' </summary>
            ExtensionMethodCaller
            ''' <summary>
            ''' 当函数有两个参数的时候，使用拓展函数的形式调用，则第二个参数会可以看作为伪单参数
            ''' </summary>
            EXtensionSingleParameter
            ''' <summary>
            ''' 函数只有一个参数，则可以忽略参数名直接调用
            ''' </summary>
            SingleParameter
            ''' <summary>
            ''' 逻辑值类型的开关参数
            ''' </summary>
            BooleanSwitch

            ''' <summary>
            ''' 函数的参数之间是按照函数的定义顺序引用的
            ''' </summary>
            OrderReference
        End Enum

#Region "Reference Type String"

        Friend Shared ReadOnly Property s_Normal As String = NameOf(ParameterType.Normal)
        Friend Shared ReadOnly Property s_ExtensionMethodCaller As String = NameOf(ParameterType.ExtensionMethodCaller)
        Friend Shared ReadOnly Property s_EXtensionSingleParameter As String = NameOf(ParameterType.EXtensionSingleParameter)
        Friend Shared ReadOnly Property s_SingleParameter As String = NameOf(ParameterType.SingleParameter)
        Friend Shared ReadOnly Property s_BooleanSwitch As String = NameOf(ParameterType.BooleanSwitch)

#End Region

        Public ReadOnly Property Type As ParameterName.ParameterType

        Public Overrides ReadOnly Property TokenType As TokenTypes
            Get
                Return TokenTypes.ParameterName
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Type">普通类型还是特殊类型</param>
        ''' <param name="Expression">获得参数名称的一个表达式字符串</param>
        Sub New(Type As ParameterName.ParameterType, Expression As String)
            Call MyBase.New(Expression)
            Me.Type = Type
        End Sub

        Public Overrides Function ToString() As String
            Return If(Type = ParameterType.Normal, Me.Expression.ToString, $"({Type.ToString}) {Expression.ToString}")
        End Function
    End Class
End Namespace