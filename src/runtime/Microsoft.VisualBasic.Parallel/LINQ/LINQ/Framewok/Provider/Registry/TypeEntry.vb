Imports System.Reflection
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.ComponentModel.Collection.Generic
Imports Microsoft.VisualBasic.Scripting
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Framework.Provider

    ''' <summary>
    ''' item in the type registry table
    ''' </summary>
    ''' <remarks>方法和类型是分开的</remarks>
    Public Class TypeEntry : Implements sIdEnumerable

        ''' <summary>
        ''' 类型的简称或者别称，即本属性为LINQEntity自定义属性中的构造函数的参数
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property name As String Implements sIdEnumerable.Identifier
        ''' <summary>
        ''' Full type name for the target LINQ entity type. Assembly!typeFullName (目标LINQEntity集合中的类型全称)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>Assembly!typeFullName</remarks>
        Public Property TypeId As MetaData.TypeInfo
        ''' <summary>
        ''' 数据源的读取方法
        ''' </summary>
        ''' <returns></returns>
        Public Property Repository As MetaData.TypeInfo
        ''' <summary>
        ''' 函数名称
        ''' </summary>
        ''' <returns></returns>
        <XmlAttribute> Public Property Func As String

        Sub New()
        End Sub

        ''' <summary>
        ''' Copy base values
        ''' </summary>
        ''' <param name="base"></param>
        Sub New(base As TypeEntry)
            Me.Func = base.Func
            Me.name = base.name
            Me.Repository = base.Repository
            Me.TypeId = base.TypeId
        End Sub

        ''' <summary>
        ''' 得到集合之中的元素的类型
        ''' </summary>
        ''' <returns></returns>
        Public Overloads Function [GetType]() As Type
            Return TypeId.GetType(True)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace