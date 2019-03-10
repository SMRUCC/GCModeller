Imports System.Reflection

Namespace HybridsScripting

    ''' <summary>
    ''' 与ShellScript进行混合编程的脚本语言，必须要在API的命名空间之中实现这个自定义属性
    ''' </summary>
    ''' <remarks>每一次切换脚本语句环境之前先使用!EntryName来进行，之后直接可以只用左移运算符尽心计算求值</remarks>
    ''' 
    <AttributeUsage(AttributeTargets.Class, allowmultiple:=False, inherited:=True)>
    Public Class ScriptEntryPoint : Inherits Attribute

        Public ReadOnly Property ScriptName As String
            Get
                Return _Name
            End Get
        End Property

        Dim _Name As String

        Sub New(ScriptName As String)
            _Name = ScriptName
        End Sub

        Private Shared ReadOnly _TypeInfo As System.Type = GetType(ScriptEntryPoint)

        Public Shared ReadOnly Property TypeInfo As System.Type
            Get
                Return _TypeInfo
            End Get
        End Property
    End Class

    ''' <summary>
    ''' 必须要具备两种类型的接口，目标脚本执行环境才会被成功挂载
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Method, allowmultiple:=False, inherited:=True)>
    Public Class ScriptEntryInterface : Inherits Attribute

        ''' <summary>
        ''' 混合编程的接口类型
        ''' </summary>
        ''' <remarks></remarks>
        Public Enum InterfaceTypes

            ''' <summary>
            ''' 有些简单类型可能并不需要初始化过程，所以这一个接口现在是可选的
            ''' </summary>
            ''' <remarks></remarks>
            EntryPointInit
            ''' <summary>
            ''' 接口类型：Public Function Evaluate(script As String) As Object
            ''' </summary>
            ''' <remarks></remarks>
            Evaluate
            ''' <summary>
            ''' Public Function SetValue(variableName As String, value As Object) As Boolean
            ''' </summary>
            ''' <remarks></remarks>
            SetValue
            DataConvertor
        End Enum

        Dim _InterfaceType As InterfaceTypes

        Public ReadOnly Property InterfaceType As InterfaceTypes
            Get
                Return _InterfaceType
            End Get
        End Property

        Sub New(Type As InterfaceTypes)
            _InterfaceType = Type
        End Sub

        Public Overrides Function ToString() As String
            Return _InterfaceType.ToString
        End Function

        Private Shared ReadOnly _TypeInfo As System.Type = GetType(ScriptEntryInterface)

        Public Shared ReadOnly Property TypeInfo As System.Type
            Get
                Return _TypeInfo
            End Get
        End Property
    End Class

    <AttributeUsage(AttributeTargets.Method, allowmultiple:=True, inherited:=True)>
    Public Class DataConvert : Inherits ScriptEntryInterface

        Dim _TypeChar As Char, _ConservedString As Boolean

        Public ReadOnly Property TypeChar As Char
            Get
                Return _TypeChar
            End Get
        End Property

        Public ReadOnly Property ConservedStringConvertor As Boolean
            Get
                Return _ConservedString
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="type">
        ''' 数据类型的后缀，推荐：
        ''' $ <see cref="String"></see>
        ''' &amp; <see cref="Long"></see>
        ''' % <see cref="Integer"></see>
        ''' ? <see cref="Boolean"></see>
        ''' ! <see cref="Double"></see>
        ''' @ <see cref="Date"></see>
        ''' </param>
        ''' <param name="ConservedString">在脚本环境之中必须要有一个保留的字符串转换方法</param>
        ''' <remarks></remarks>
        Sub New(type As Char, Optional ConservedString As Boolean = False)
            Call MyBase.New(InterfaceTypes.DataConvertor)
            _TypeChar = type
            _ConservedString = ConservedString
        End Sub

        Public Overrides Function ToString() As String
            Return _TypeChar.ToString
        End Function
    End Class
End Namespace