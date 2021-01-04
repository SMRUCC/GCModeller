﻿#Region "Microsoft.VisualBasic::b2c26c4d7ba492eb6c8445f3badcfb78, Microsoft.VisualBasic.Core\Serialization\BinaryDumping\ObjectVisitor.vb"

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

    '     Delegate Sub
    ' 
    ' 
    '     Class ObjectVisitor
    ' 
    '         Properties: VisitOnlyFields
    ' 
    '         Constructor: (+1 Overloads) Sub New
    ' 
    '         Function: GetAllFields
    ' 
    '         Sub: DoVisitArray, doVisitFields, DoVisitObject, DoVisitObjectFields, doVisitProperties
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language

Namespace Serialization.BinaryDumping

    ''' <summary>
    ''' Do serialization or count memory size by this interface method
    ''' </summary>
    ''' <param name="value"></param>
    ''' <param name="type"></param>
    ''' <param name="isVisited">当前的对象是否是一个已经被访问过的引用对象</param>
    Public Delegate Sub DoVisitObject(value As Object, type As Type, memberInfo As MemberInfo, isVisited As Boolean, isValueType As Boolean)

    ''' <summary>
    ''' A Common framework for visit a object
    ''' </summary>
    Public NotInheritable Class ObjectVisitor

        ''' <summary>
        ''' Only visit instance fields, otherwise visit properties
        ''' </summary>
        ''' <returns></returns>
        Public Property VisitOnlyFields As Boolean = False

        ''' <summary>
        ''' Visited reference types
        ''' </summary>
        Dim visitedReferences As New Index(Of Object)

        Public Const AllFields As BindingFlags = BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance

        Sub New()
        End Sub

        Public Sub DoVisitObject(obj As Object, type As Type, visit As DoVisitObject)
            If VisitOnlyFields Then
                Call doVisitFields(obj, GetAllFields(type).ToArray, visit)
            Else
                Call doVisitProperties(obj, type.GetProperties(PublicProperty), visit)
            End If
        End Sub

        Public Sub DoVisitObjectFields(obj As Object, type As Type, visit As DoVisitObject)
            Call doVisitFields(obj, GetAllFields(type).ToArray, visit)
        End Sub

        Public Sub DoVisitArray(array As Array, visit As DoVisitObject)
            ' get array element type
            Dim arrayType As Type = array.GetType.GetElementType

            ' visit each element
            For i As Integer = 0 To array.Length - 1
                Call DoVisitObject(array.GetValue(i), arrayType, visit)
            Next
        End Sub

        Private Sub doVisitProperties(obj As Object, properties As PropertyInfo(), visit As DoVisitObject)

        End Sub

        Public Shared Iterator Function GetAllFields(type As Type) As IEnumerable(Of FieldInfo)
            For Each field As FieldInfo In type.GetFields(AllFields)
                Yield field
            Next

            Dim base As Value(Of Type) = type

            Do While Not (base = base.Value.BaseType) Is Nothing
                For Each field As FieldInfo In base.Value.GetFields(AllFields)
                    Yield field
                Next
            Loop
        End Function

        Private Sub doVisitFields(obj As Object, fields As FieldInfo(), visit As DoVisitObject)
            Dim value As Object
            Dim type As Type
            Dim isVisited As Boolean = False
            Dim isValueType As Boolean = False

            If obj Is Nothing Then
                Return
            End If

            For Each field As FieldInfo In fields
                value = field.GetValue(obj)

                ' 因为在字段的定义中会存在继承或者接口实现的这些抽象的类型实现关系, 
                ' 所以在这里优先从 value 值之中获取真实的类型信息
                '
                ' 如果是空值,则获取字段的类型信息
                If value Is Nothing Then
                    type = field.FieldType
                Else
                    ' 如果不是空值,则获取实际的类型信息
                    type = value.GetType

                    ' value is not nothing
                    If Not type.IsValueType AndAlso Not type Is GetType(String) Then
                        If value Like visitedReferences Then
                            isVisited = True
                        Else
                            isVisited = False
                            visitedReferences += value
                        End If

                        isValueType = False
                    Else
                        isValueType = True
                    End If
                End If

                Call visit(value, type, field, isVisited, isValueType)

                If Not DataFramework.IsPrimitive(type) Then
                    ' do recursive visit of this field value
                    Call DoVisitObjectFields(value, type, visit)
                End If
            Next
        End Sub
    End Class
End Namespace
