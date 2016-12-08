#Region "Microsoft.VisualBasic::992707f89d181a26221667dc06985fbb, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Parser\Tokens\SelectClosure.vb"

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

Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports sciBASIC.ComputingServices.Linq.Framework.DynamicCode

Namespace LDM.Statements.Tokens

    Public Class SelectClosure : Inherits Tokens.Closure
        Implements IProjectProvider

        ''' <summary>
        ''' 通过Select表达式所产生的数据投影
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Projects As String() Implements IProjectProvider.Projects

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="tokens">使用逗号分隔数据投影</param>
        ''' <param name="parent"></param>
        Sub New(tokens As ClosureTokens(), parent As LinqStatement)
            Call MyBase.New(TokenIcer.Tokens.Select, tokens, parent)

            Dim stacks = __getTokens.Parsing(stackT).Args
            Projects = stacks.ToArray(Function(x) x.ToString)
        End Sub

        Private Shared ReadOnly Property stackT As StackTokens(Of TokenIcer.Tokens)
            Get
                Return New StackTokens(Of TokenIcer.Tokens)(Function(a, b) a = b) With {
                    .ParamDeli = TokenIcer.Tokens.Comma,
                    .LPair = TokenIcer.Tokens.OpenParens,
                    .Pretend = TokenIcer.Tokens.Pretend,
                    .RPair = TokenIcer.Tokens.CloseParens,
                    .WhiteSpace = TokenIcer.Tokens.WhiteSpace
                }
            End Get
        End Property

        Private Function __getTokens() As Token(Of TokenIcer.Tokens)()
            Dim list As New List(Of Token(Of TokenIcer.Tokens))

            For Each x In Source.Tokens
                If x.TokenName <> TokenIcer.Tokens.Comma AndAlso
                    x.TokenValue.Last = "," Then
                    Dim a = New Token(Of TokenIcer.Tokens)(x.TokenName, Mid(x.TokenValue, 1, x.TokenValue.Length - 1))
                    Dim c As New Token(Of TokenIcer.Tokens)(TokenIcer.Tokens.Comma, ",")

                    list += a
                    list += c
                Else
                    list += x
                End If
            Next

            Return list.ToArray
        End Function
    End Class
End Namespace
