Imports System.Reflection

Namespace Runtime.HybridsScripting

    ''' <summary>
    ''' 必须要具备两种类型的接口，目标脚本执行环境才会被成功挂载
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class EntryInterface : Inherits Attribute

        Public ReadOnly Property InterfaceType As InterfaceTypes

        Sub New(Type As InterfaceTypes)
            _InterfaceType = Type
        End Sub

        Public Overrides Function ToString() As String
            Return _InterfaceType.ToString
        End Function

        Public Shared ReadOnly Property TypeInfo As Type = GetType(EntryInterface)

    End Class

    ''' <summary>
    ''' The hybrids programming interface description.(混合编程的接口类型)
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
        DataTransform
    End Enum
End Namespace