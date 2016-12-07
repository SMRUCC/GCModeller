#Region "Microsoft.VisualBasic::f50344c2f2eac58abf75bc349ddffe7c, ..\sciBASIC.ComputingServices\LINQ\LINQ\Extensions.vb"

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
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq.Framework.DynamicCode.VBC
Imports Microsoft.VisualBasic.Linq.Framework.Provider
Imports Microsoft.VisualBasic.Linq.Framework.Provider.ImportsAPI
Imports Microsoft.VisualBasic.Linq.LDM.Expression

Public Module Extensions

    <Extension>
    Public Function Compile(where As WhereClosure, types As TypeRegistry, api As APIProvider) As Type
        Dim compiler As New DynamicCompiler(types, api)
        Return compiler.Compile(where.BuildModule)
    End Function

    <Extension>
    Public Function CompileTest(where As WhereClosure, types As TypeRegistry, api As APIProvider) As ITest
        Dim dynamicsType As Type = where.Compile(types, api) ' 得到动态编译出来的类型
        Return AddressOf New __where(dynamicsType).Test
    End Function

    ''' <summary>
    ''' Declare a function with a specific function name and return type. please notice that in this newly 
    ''' declare function there is always a local variable name rval using for return the value.
    ''' (申明一个方法，返回指定类型的数据并且具有一个特定的函数名，请注意，在这个新申明的函数之中，
    ''' 固定含有一个rval的局部变量用于返回数据)
    ''' </summary>
    ''' <param name="Name">Function name.(函数名)</param>
    ''' <param name="Type">Function return value type.(该函数的返回值类型)</param>
    ''' <returns>A codeDOM object model of the target function.(一个函数的CodeDom对象模型)</returns>
    ''' <remarks></remarks>
    Public Function DeclareFunction(Name As String, Type As String, Statements As CodeDom.CodeStatementCollection) As CodeDom.CodeMemberMethod
        Dim CodeMemberMethod As CodeDom.CodeMemberMethod = New CodeDom.CodeMemberMethod()
        CodeMemberMethod.Name = Name : CodeMemberMethod.ReturnType = New CodeDom.CodeTypeReference(Type) '创建一个名为“WhereTest”，返回值类型为Boolean的无参数的函数
        If String.Equals(Type, "System.Boolean", StringComparison.OrdinalIgnoreCase) Then
            CodeMemberMethod.Statements.Add(New CodeDom.CodeVariableDeclarationStatement(Type, "rval", New CodeDom.CodePrimitiveExpression(True)))   '创建一个用于返回值的局部变量，对于逻辑值，默认为真
        Else
            CodeMemberMethod.Statements.Add(New CodeDom.CodeVariableDeclarationStatement(Type, "rval"))   '创建一个用于返回值的局部变量
        End If

        If Not (Statements Is Nothing OrElse Statements.Count = 0) Then
            CodeMemberMethod.Statements.AddRange(Statements)
        End If
        CodeMemberMethod.Statements.Add(New CodeDom.CodeMethodReturnStatement(New CodeDom.CodeVariableReferenceExpression("rval")))  '引用返回值的局部变量

        Return CodeMemberMethod
    End Function
End Module

Public Delegate Function ITest(obj As Object) As Boolean

Friend Class __where

    ReadOnly __type As Type
    ReadOnly __test As MethodInfo

    Sub New(type As Type)
        __type = type
        __test.DeclaringType.GetMethod(WhereClosure.TestMethod)
    End Sub

    Public Function Test(obj As Object) As Boolean
        Return DirectCast(__test.Invoke(Nothing, {obj}), Boolean)
    End Function
End Class
