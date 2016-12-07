#Region "Microsoft.VisualBasic::4667ebdd0b8d1e5a0b2058d9a3f5af1f, ..\sciBASIC.ComputingServices\LINQ\LINQ\Framewok\Provider\Registry\TypeEntry.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

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
    Public Class TypeEntry : Implements INamedValue

        ''' <summary>
        ''' 类型的简称或者别称，即本属性为LINQEntity自定义属性中的构造函数的参数
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <XmlAttribute> Public Property name As String Implements INamedValue.Key
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
