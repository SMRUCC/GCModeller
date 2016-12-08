#Region "Microsoft.VisualBasic::8ae9f750da15b1e9c2bf12813a0429a3, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Expression\FromClosure.vb"

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

Imports System.CodeDom
Imports Microsoft.VisualBasic.CodeDOM_VBC
Imports Microsoft.VisualBasic.Linq.Framework
Imports Microsoft.VisualBasic.Linq.Framework.DynamicCode
Imports Microsoft.VisualBasic.Linq.Framework.DynamicCode.VBC
Imports Microsoft.VisualBasic.Linq.Framework.LQueryFramework
Imports Microsoft.VisualBasic.Linq.Framework.Provider

Namespace LDM.Expression

    ''' <summary>
    ''' The init variable.
    ''' </summary>
    Public Class FromClosure : Inherits Closure

        ''' <summary>
        ''' 变量的名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Name As String
        ''' <summary>
        ''' 变量的类型标识符
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TypeId As String

        Public ReadOnly Property RegistryType As TypeEntry

        Sub New(source As Statements.Tokens.FromClosure, registry As TypeRegistry)
            Call MyBase.New(source)

            Me.Name = source.Name
            Me.TypeId = source.TypeId
            Me.RegistryType = registry.Find(source.TypeId)

            Dim fieldType As Type = RegistryType.GetType
            Me.DeclaredField = CodeDOMExpressions.Field(Name, fieldType)

            Call __init()
        End Sub

        Public Overrides Function ToString() As String
            Return String.Format("Dim {0} As {1}", Name, TypeId)
        End Function

        Public ReadOnly Property DeclaredField As CodeMemberField

        ''' <summary>
        ''' 解析为类的域
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides Function __parsing() As CodeExpression
            Dim code As CodeExpression = CodeDOMExpressions.FieldRef(Name)
            Return code
        End Function
    End Class
End Namespace
