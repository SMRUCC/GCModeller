﻿#Region "Microsoft.VisualBasic::09103db9f3b42def3f59569a98d7d368, Microsoft.VisualBasic.Core\ComponentModel\DataSource\Property\PropertyValue.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Class PropertyValue
    ' 
    '         Properties: obj, value
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: [New], GetValue, (+2 Overloads) Read, SetValue, ToString
    ' 
    '         Sub: SetValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace ComponentModel.DataSourceModel

    ''' <summary>
    ''' The <see cref="PropertyInfo"/> like definition of the extension property.
    ''' </summary>
    ''' <typeparam name="T"></typeparam>
    Public Class PropertyValue(Of T) : Inherits Value(Of T)

        ReadOnly __get As Func(Of T)
        ReadOnly __set As Action(Of T)

        ''' <summary>
        ''' The Extension property value.
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Property value As T
            Get
                Return __get()
            End Get
            Set(value As T)
                MyBase.value = value

                If Not __set Is Nothing Then
                    Call __set(value)  ' 因为在初始化的时候会对这个属性赋值，但是set没有被初始化，所以会出错，在这里加了一个if判断来避免空引用的错误
                End If
            End Set
        End Property

        ''' <summary>
        ''' The instance object for this extension property
        ''' </summary>
        ''' <returns></returns>
        Public Property obj As IClassObject

        ''' <summary>
        ''' Custom property value.(value generated based on the extension property host <see cref="obj"/>)
        ''' </summary>
        ''' <param name="[get]">请勿使用<see cref="GetValue"/></param>函数，否则会出现栈空间溢出
        ''' <param name="[set]">请勿使用<see cref="SetValue"/></param>方法，否则会出现栈空间溢出
        Sub New([get] As Func(Of T), [set] As Action(Of T))
            __get = [get]
            __set = [set]
        End Sub

        ''' <summary>
        ''' Tag property value.(默认是将数据写入到基本类型的值之中)
        ''' </summary>
        Sub New()
            __get = Function() MyBase.value
            __set = Sub(v) MyBase.value = v
        End Sub

        ''' <summary>
        ''' 这个主要是应用于Linq表达式之中，将属性值设置之后返回宿主对象实例
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Public Function SetValue(value As T) As IClassObject
            Call __set(value)
            Return obj
        End Function

        ''' <summary>
        ''' Property Get Value
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Overloads Shared Narrowing Operator CType(x As PropertyValue(Of T)) As T
            Return x.value
        End Operator

        ''' <summary>
        ''' <see cref="Value"/> -> <see cref="GetObjectJson"/>
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return value.GetJson
        End Function

        Public Shared Function GetValue(Of Cls As IClassObject)(x As Cls, name As String) As PropertyValue(Of T)
            Dim value As Object = x.Extension.DynamicHashTable(name)
            Dim pv As PropertyValue(Of T) = DirectCast(value, PropertyValue(Of T))
            Return pv
        End Function

        Public Shared Sub SetValue(Of Cls As IClassObject)(x As Cls, name As String, value As T)
            Dim pvo As Object = x.Extension.DynamicHashTable(name)
            Dim pv As PropertyValue(Of T) = DirectCast(pvo, PropertyValue(Of T))
            pv.value = value
        End Sub

        ''' <summary>
        ''' Creates a new extension property for the target <see cref="BaseClass"/>
        ''' </summary>
        ''' <typeparam name="Cls"></typeparam>
        ''' <param name="x"></param>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Shared Function [New](Of Cls As IClassObject)(x As Cls, name As String) As PropertyValue(Of T)
            Dim value As New PropertyValue(Of T)()
            x.Extension.DynamicHashTable(name) = value
            value.obj = x
            Return value
        End Function

        ''' <summary>
        ''' Gets the tag property value from the <see cref="BaseClass"/>.(读取<see cref="BaseClass"/>对象之中的一个拓展属性)
        ''' </summary>
        ''' <typeparam name="Cls"></typeparam>
        ''' <param name="x"></param>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Public Shared Function Read(Of Cls As IClassObject)(x As Cls, name As String) As PropertyValue(Of T)
            If x.Extension Is Nothing Then
                x.Extension = New ExtendedProps
            End If
            Dim prop As Object = x.Extension.DynamicHashTable(name)
            If prop Is Nothing Then
                prop = PropertyValue(Of T).[New](Of Cls)(x, name)
            End If
            Return DirectCast(prop, PropertyValue(Of T))
        End Function

        Public Shared Function Read(Of Cls As IClassObject)(x As Cls, pm As MethodBase) As PropertyValue(Of T)
            Return Read(Of Cls)(x, pm.Name)
        End Function
    End Class
End Namespace
