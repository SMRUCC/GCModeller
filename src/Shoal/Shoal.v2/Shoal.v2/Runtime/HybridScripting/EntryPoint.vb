Imports System.Reflection

Namespace Runtime.HybridsScripting

    Public Structure EntryPoint

        ''' <summary>
        ''' Script name.(混合编程的脚本名称)
        ''' </summary>
        ''' <remarks></remarks>
        Dim Language As LanguageEntryPoint
        Dim Init, Evaluate, SetValue As MethodInfo
        ''' <summary>
        ''' Basic type data convert interface.(基本数据类型的转换接口)
        ''' </summary>
        ''' <remarks></remarks>
        Dim DataConvertors As SortedDictionary(Of Char, System.Reflection.MethodInfo)
        ''' <summary>
        ''' When the <see cref="InteropAdapter.Evaluate">handlers</see> can not found the data type 
        ''' convert method for the basically type then it will try this system preserved string type convert 
        ''' method to convert the data as string as default.
        ''' (当<see cref="DataConvertors"></see>之中没有查找到目标类型的转换操作接口的时候，则默认使用本方法转换
        ''' 为字符串的格式，保留的字符串类型的转换函数)
        ''' </summary>
        ''' <remarks></remarks>
        Dim ConservedString As System.Reflection.MethodInfo
        Dim TypeFullName As String
        Dim DeclaredAssemblyType As System.Type

        Public Function EvaluateInvoke(Script As String) As Object
            Return Evaluate.Invoke(Nothing, {Script})
        End Function

        Public Function SetValueInvoke(Name As String, value As Object) As Boolean
            Return SetValue.Invoke(Nothing, {Name, value})
        End Function

        Public Function ReservedConvert(value As String) As Object
            Return ConservedString.Invoke(Nothing, {value})
        End Function

        Public Overrides Function ToString() As String
            Return Language.ToString
        End Function

        Public Function Converts(ch As Char, value As String) As Object
            Return DataConvertors(ch).Invoke(Nothing, {value})
        End Function

        ''' <summary>
        ''' 初始化外部环境
        ''' </summary>
        Public Sub InitInvoke()
            If Not Init Is Nothing Then
                Call Init.Invoke(Nothing, Nothing)
            End If
        End Sub

        ''' <summary>
        '''This property indicated that the entry data which was parsing from the assembly module is valid or not.(可以使用本属性来判断目标解析数据是否可用)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsNull As Boolean
            Get
                Return Language Is Nothing OrElse
                    Evaluate Is Nothing OrElse
                    SetValue Is Nothing
            End Get
        End Property
    End Structure
End Namespace