Imports System.Data.Linq.Mapping
Imports System.Linq
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Services.ScriptBuilder

    Public MustInherit Class RAttribute : Inherits Attribute

        ''' <summary>
        ''' API token name
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Name As String

        ''' <summary>
        ''' Declaring a R function
        ''' </summary>
        ''' <param name="name">R function name</param>
        Sub New(name As String)
            Me.Name = name
        End Sub

        Public Overrides Function ToString() As String
            Return Name
        End Function
    End Class

    <AttributeUsage(AttributeTargets.Property, AllowMultiple:=False, Inherited:=True)>
    Public Class Parameter : Inherits RAttribute

        Public ReadOnly Property [Optional] As Boolean
        Public ReadOnly Property Type As ValueTypes
        Public ReadOnly Property ForceFirst As Boolean

        ''' <summary>
        ''' API会自动根据类型来修正路径之中的分隔符的
        ''' </summary>
        ''' <param name="name"></param>
        ''' <param name="opt">Is this parameter optional?</param>
        ''' <param name="forceFirst">是否强制当前的这个参数处于第一个序列化的位置</param>
        Sub New(name As String, Optional type As ValueTypes = ValueTypes.String, Optional opt As Boolean = False, Optional forceFirst As Boolean = False)
            MyBase.New(name)
            Me.[Optional] = opt
            Me.Type = type
            Me.ForceFirst = forceFirst
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Enum ValueTypes
        [String] = 0
        ''' <summary>
        ''' 这个是一个字符串类型的文件路径
        ''' </summary>
        [Path]
        ''' <summary>
        ''' vectors, factors or a list containing these.(list类型的参数的参数名将不会被序列化)
        ''' </summary>
        List
    End Enum

    ''' <summary>
    ''' Declaring a R function entry point.
    ''' </summary>
    <AttributeUsage(AttributeTargets.Class Or AttributeTargets.Struct, AllowMultiple:=False, Inherited:=True)>
    Public Class RFunc : Inherits RAttribute

        ''' <summary>
        ''' Declaring a R function entry point.
        ''' </summary>
        ''' <param name="name"></param>
        Sub New(name As String)
            Call MyBase.New(name)
        End Sub

        Public Shared Narrowing Operator CType(rfunc As RFunc) As String
            Return rfunc.Name
        End Operator
    End Class

    ''' <summary>
    ''' Declaring a R function entry point.
    ''' </summary>
    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class RImport : Inherits RAttribute

        Public ReadOnly Property Required As String

        ''' <summary>
        ''' Declaring a R function entry point.
        ''' </summary>
        ''' <param name="name">The R function name</param>
        ''' <param name="require">The required package name of this function.</param>
        Sub New(name As String, Optional require As String = "base")
            Call MyBase.New(name)
            Me.Required = require
        End Sub

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace