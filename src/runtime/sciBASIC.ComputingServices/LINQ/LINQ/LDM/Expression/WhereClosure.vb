#Region "Microsoft.VisualBasic::c5659906a133278df957624702b46cee, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Expression\WhereClosure.vb"

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
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq.LDM.Statements.TokenIcer
Imports Microsoft.VisualBasic.Linq.Framework.DynamicCode
Imports Microsoft.VisualBasic.Linq.Framework.DynamicCode.VBC
Imports Microsoft.VisualBasic.Linq.Framework.Provider
Imports Microsoft.VisualBasic.Linq.Framework.Provider.ImportsAPI
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic.CodeDOM_VBC
Imports Microsoft.VisualBasic.Linq.LDM.Parser

Namespace LDM.Expression

    ''' <summary>
    ''' 测试的是一个对象
    ''' </summary>
    ''' <remarks>
    ''' Where 测试的一个对象类型，对象的属性则是前面的In和Let所生成的变量
    ''' Where 对象里面则通过一个逻辑值的函数来测试对象
    ''' 
    ''' Public Module Where
    ''' 
    '''     Public Function Test(x As objectType) As Boolean
    '''         Return (......) X ' 由where生成的测试语句
    '''     End Function
    ''' 
    ''' End Module
    ''' </remarks>
    Public Class WhereClosure : Inherits Closure
        Implements ICompiler

        ''' <summary>
        ''' 有where生成的模块里面的一个测试函数的函数方法信息
        ''' </summary>
        Dim __testMethod As MethodInfo
        ''' <summary>
        ''' 前面的语句所生成的匿名类型的类型信息
        ''' </summary>
        Dim __typeINFO As Type

        Sub New(source As Statements.Tokens.WhereClosure)
            Call MyBase.New(source)
        End Sub

        Sub New(expr As Func(Of Tokens), type As Type)
            Call MyBase.New(New Statements.Tokens.WhereClosure(expr))
            __typeINFO = type
        End Sub

        ''' <summary>
        ''' 在这个函数里面生成测试函数之中的表达式，然后再由vbc生成模块类型
        ''' 函数只有一个参数，并且参数名为obj
        ''' </summary>
        ''' <returns></returns>
        Protected Overrides Function __parsing() As CodeExpression
            Dim source = _source.Source.Tokens.Replace(ARGV)
            Dim parser As New LDM.Parser.Tokenizer(source)
            Dim code As CodeExpression = New LDM.Parser.Parser().ParseExpression(parser)
            Return code
        End Function

        Public Const ARGV As String = "obj"
        Public Const TestMethod As String = "___test"
        Public Const RTVL As String = "rtvl"

        Private Function __buildFunc() As CodeMemberMethod
            Dim args As New Dictionary(Of String, Type) From {
                {ARGV, Me.__typeINFO}
            }
            Dim [Function] As CodeMemberMethod =
                DeclareFunc(TestMethod, args, GetType(Boolean))
            Call [Function].Statements.Add(LocalsInit(RTVL, GetType(Boolean), init:=False))
            Call [Function].Statements.Add(ValueAssign(LocalVariable(RTVL), __parsing))
            Call [Function].Statements.Add([Return](RTVL))

            Return [Function]
        End Function

        Public Function BuildModule() As CodeTypeDeclaration
            Dim type As New CodeTypeDeclaration(NameOf(WhereClosure))
            Call type.Members.Add(__buildFunc)
            Return type
        End Function

        Sub New(expr As Token(Of Tokens)(), type As Type)
            Call MyBase.New(New Statements.Tokens.WhereClosure(expr))
            __typeINFO = type
        End Sub

        Public Sub CompileToken(types As TypeRegistry, api As APIProvider) Implements ICompiler.CompileToken
            Dim type As Type = Me.Compile(types, api)
            Me.__testMethod = type.GetMethod(TestMethod)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="obj">
        ''' From x In $source let y =func(x) Where test(x,y) Select ctor(x,y)
        ''' Where条件所测试的实际上是由Where前面的x和y通过vbc所构成的一个新的临时匿名类型的对象
        ''' </param>
        ''' <returns></returns>
        Public Function WhereTest(obj As Object) As Boolean
            Return DirectCast(__testMethod.Invoke(Nothing, {obj}), Boolean)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="type">X的类型</param>
        ''' <param name="expr">
        ''' Where逻辑表达式，表达式内部的变量都看作为对前面的对象类型的属性的引用
        ''' $var看作为由前面的let和in语句所生成的匿名类型的对象实例的引用
        ''' </param>
        ''' <returns></returns>
        Public Shared Function CreateLinqWhere(type As Type, expr As String) As WhereClosure
            Dim tokens = Statements.TokenIcer.GetTokens(expr).TrimWhiteSpace
            Return New WhereClosure(tokens, type)
        End Function
    End Class
End Namespace
