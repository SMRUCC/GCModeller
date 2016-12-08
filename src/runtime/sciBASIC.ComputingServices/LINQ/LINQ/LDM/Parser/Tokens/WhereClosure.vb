#Region "Microsoft.VisualBasic::d8edde1544c279cd6726ee1a995be568, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Parser\Tokens\WhereClosure.vb"

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

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports sciBASIC.ComputingServices.Linq.Framework.DynamicCode

Namespace LDM.Statements.Tokens

    Public Class WhereClosure : Inherits Closure
        Implements ICodeProvider

        Public ReadOnly Property Code As String Implements ICodeProvider.Code

        Sub New(tokens As ClosureTokens(), parent As LinqStatement)
            Call MyBase.New(TokenIcer.Tokens.Where, tokens, parent)
            Code = Source.Tokens.ToArray(Function(x) x.TokenValue).JoinBy(" ")
        End Sub

        ''' <summary>
        ''' 编译Linq之中的Where条件测试函数
        ''' </summary>
        ''' <param name="Expr">必须是符合VisualBasic语法的逻辑表达式</param>
        ''' <param name="type"></param>
        ''' <returns></returns>
        Public Shared Function CreateLinqWhere(Expr As String, type As Type, compiler As DynamicCompiler) As ITest
            Dim tokens As Token(Of TokenIcer.Tokens)() = TokenIcer.GetTokens(Expr)
            For Each x As Token(Of TokenIcer.Tokens) In tokens
                If x.TokenName = TokenIcer.Tokens.VarRef Then
                    x.TokenName = TokenIcer.Tokens.String
                    x.TokenValue = obj
                End If
            Next
            Expr = String.Join(" ", tokens.ToArray(Function(x) x.TokenValue))
            Expr = GetCode(Expr, type)

            Dim project = compiler.Compile(Expr, Expr)
            Dim test As New __test With {
                .project = project
            }
            Return AddressOf test.Test
        End Function

        Private Class __test

            Public project As IProject

            ''' <summary>
            ''' <see cref="ITest"/>
            ''' </summary>
            ''' <param name="x"></param>
            ''' <returns></returns>
            Public Function Test(x As Object) As Boolean
                Dim value = project(x)
                Return value.IsTrue
            End Function
        End Class

        Const obj As String = "x_obj"

        Public Shared Function GetCode(where As String, type As Type) As String
            Dim code As String = LinqClosure.BuildClosure(obj, type, Nothing, Nothing, {obj}, where)
            Return code
        End Function

        Public Delegate Function ITest(x As Object) As Boolean
    End Class
End Namespace
