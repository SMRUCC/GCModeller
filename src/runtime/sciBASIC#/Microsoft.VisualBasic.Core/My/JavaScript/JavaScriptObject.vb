#Region "Microsoft.VisualBasic::19bfdec5cef80f52096f361a1993ed21, Microsoft.VisualBasic.Core\My\JavaScript\JavaScriptObject.vb"

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

'     Interface IJavaScriptObjectAccessor
' 
'         Properties: Accessor
' 
'     Class JavaScriptObject
' 
'         Constructor: (+1 Overloads) Sub New
'         Function: GetEnumerator, IEnumerable_GetEnumerator
' 
' 
' /********************************************************************************/

#End Region

Imports System.Reflection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel.SchemaMaps
Imports Microsoft.VisualBasic.Language

Namespace My.JavaScript

    Public Interface IJavaScriptObjectAccessor
        Default Property Accessor(name As String) As Object
    End Interface

    Public Class Descriptor

        Public Property value As Object
        Public Property writable As Boolean
        Public Property enumerable As Boolean
        Public Property configurable As Boolean

    End Class

    ''' <summary>
    ''' javascript object
    ''' </summary>
    Public Class JavaScriptObject : Implements IEnumerable(Of String), IJavaScriptObjectAccessor

        Dim members As New Dictionary(Of String, [Variant](Of BindProperty(Of DataFrameColumnAttribute), Object))

        ''' <summary>
        ''' 只针对Public的属性或者字段有效
        ''' </summary>
        ''' <param name="memberName"></param>
        ''' <returns></returns>
        Default Public Property Accessor(memberName As String) As Object Implements IJavaScriptObjectAccessor.Accessor
            Get
                If members.ContainsKey(memberName) Then
                    Dim value = members(memberName)

                    If value Is Nothing Then
                        Return Nothing
                    ElseIf value Like GetType(BindProperty(Of DataFrameColumnAttribute)) Then
                        Return value.TryCast(Of BindProperty(Of DataFrameColumnAttribute)).GetValue(Me)
                    Else
                        Return value.TryCast(Of Object)
                    End If
                Else
                    ' Returns undefined in javascript
                    Return Nothing
                End If
            End Get
            Set(value As Object)
                If members.ContainsKey(memberName) Then
                    If members(memberName) Is Nothing Then
                        members(memberName) = value
                    ElseIf members(memberName) Like GetType(BindProperty(Of DataFrameColumnAttribute)) Then
                        members(memberName).TryCast(Of BindProperty(Of DataFrameColumnAttribute)).SetValue(Me, value)
                    Else
                        members(memberName) = value
                    End If
                Else
                    ' 添加一个新的member
                    members(memberName) = value
                End If
            End Set
        End Property

        ''' <summary>
        ''' 如果存在无参数的函数，则也会被归类为只读属性？
        ''' </summary>
        Sub New()
            Dim type As Type = MyClass.GetType
            Dim properties As PropertyInfo() = type.GetProperties(PublicProperty).ToArray
            Dim fields As FieldInfo() = type.GetFields(PublicProperty).ToArray

            For Each prop As PropertyInfo In properties
                members(prop.Name) = New BindProperty(Of DataFrameColumnAttribute)(prop)
            Next
            For Each field As FieldInfo In fields
                members(field.Name) = New BindProperty(Of DataFrameColumnAttribute)(field)
            Next
        End Sub

        Public Shared Function GetDescription(jsObj As JavaScriptObject) As Dictionary(Of String, Descriptor)
            Dim desc As New Dictionary(Of String, Descriptor)

            For Each p As String In jsObj
                desc(p) = New Descriptor With {
                    .value = jsObj(p),
                    .configurable = True,
                    .enumerable = True,
                    .writable = True
                }
            Next

            Return desc
        End Function

        Public Iterator Function GetEnumerator() As IEnumerator(Of String) Implements IEnumerable(Of String).GetEnumerator
            For Each key As String In members.Keys
                Yield key
            Next
        End Function

        Private Iterator Function IEnumerable_GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
    End Class
End Namespace
