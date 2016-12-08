#Region "Microsoft.VisualBasic::7c9ca529f995f24db314efccdb89a6ec, ..\sciBASIC.ComputingServices\LINQ\LINQ\Framewok\Provider\LinqEntity.vb"

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

Imports System.Text.RegularExpressions

Namespace Framework.Provider

    ''' <summary>
    ''' LINQ entity type
    ''' </summary>
    ''' <remarks></remarks>
    <AttributeUsage(AttributeTargets.Method, AllowMultiple:=False, Inherited:=True)>
    Public Class LinqEntity : Inherits Attribute

        Public ReadOnly Property Type As String
        Public ReadOnly Property RefType As Type

        Public Shared ReadOnly Property ILinqEntity As Type = GetType(LinqEntity)

        ''' <summary>
        ''' 方法应该申明在模块之中，或者Class之中应该是共享的静态方法
        ''' </summary>
        ''' <param name="type">类型的简称</param>
        ''' <param name="ref">实际引用的类型位置</param>
        Sub New(type As String, ref As Type)
            Me.Type = type
            Me.RefType = ref
        End Sub

        Public Overrides Function ToString() As String
            Return Type
        End Function

        ''' <summary>
        ''' 获取目标类型上的自定义属性中的LINQEntity类型对象中的EntityType属性值
        ''' </summary>
        ''' <param name="type"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetEntityType(type As Type) As String
            Dim attr As Object() = type.GetCustomAttributes(ILinqEntity, True)
            If attr.IsNullOrEmpty Then
                Return ""
            Else
                Return DirectCast(attr(Scan0), LinqEntity).Type
            End If
        End Function
    End Class
End Namespace
