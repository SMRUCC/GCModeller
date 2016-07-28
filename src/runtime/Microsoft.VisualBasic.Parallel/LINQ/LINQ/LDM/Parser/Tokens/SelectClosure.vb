Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.LINQ.Framework.DynamicCode
Imports Microsoft.VisualBasic.Linq.LDM
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic.Linq.LDM.Statements.TokenIcer.Parser

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