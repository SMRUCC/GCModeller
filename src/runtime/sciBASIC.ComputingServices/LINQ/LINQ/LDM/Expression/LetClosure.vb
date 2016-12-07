#Region "Microsoft.VisualBasic::e7919a133cdb009b0a35a0758873d49a, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Expression\LetClosure.vb"

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
Imports System.Text
Imports System.CodeDom
Imports Microsoft.VisualBasic.LINQ.Framework.Provider
Imports Microsoft.VisualBasic.Linq.LDM.Statements
Imports Microsoft.VisualBasic.LINQ.Extensions
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic.CodeDOM_VBC

Namespace LDM.Expression

    ''' <summary>
    ''' Object declared using a LET expression.(使用Let语句所声明的只读对象)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class LetClosure : Inherits Closure

        Public ReadOnly Property FieldDeclaration As CodeMemberField

        Sub New(source As Statements.Tokens.LetClosure, registry As TypeRegistry)
            Call MyBase.New(source)

            Dim type As Type = __getType(registry)
            FieldDeclaration = Field(source.Name, type.GetType)
            Call __init()
        End Sub

        Private Function __getType(registry As TypeRegistry) As Type
            Dim source As Statements.Tokens.LetClosure =
                DirectCast(_source, Statements.Tokens.LetClosure)
            Dim type As TypeEntry = registry.Find(source.Type)
            If type Is Nothing Then
                ' 尝试系统类型
                Dim typeDef As Type = Scripting.GetType(source.Type, True)
                Return typeDef
            Else
                Return type.GetType
            End If
        End Function

        Public Overrides Function ToString() As String
            Return Me._source.ToString
        End Function

        ''' <summary>
        ''' 在这里解析初始化赋值的表达式
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides Function __parsing() As CodeExpression
            Dim init As Func(Of TokenIcer.Tokens) =
                DirectCast(_source, Tokens.LetClosure).Expression.Args.First
        End Function
    End Class
End Namespace
