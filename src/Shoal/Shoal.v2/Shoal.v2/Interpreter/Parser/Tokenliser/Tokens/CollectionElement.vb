Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq.Extensions
Imports Microsoft.VisualBasic.ApplicationServices

Namespace Interpreter.Parser.Tokens

    Public Class CollectionElement : Inherits Token

        Public Property Array As Tokens.InternalExpression
        ''' <summary>
        ''' 单个位置元素或者条件表达式或者一个位置集合
        ''' </summary>
        ''' <returns></returns>
        Public Property Index As Tokens.InternalExpression

        Public Overrides ReadOnly Property TokenType As TokenTypes
            Get
                Return TokenTypes.CollectionElement
            End Get
        End Property

        Sub New(Expression As String)
            Call MyBase.New(0, Expression)

            If Expression.First = "["c AndAlso Expression.Last = "]"c Then
                Expression = Mid(Expression, 2, Len(Expression) - 2)
            End If

            Dim index As String = Regex.Match(Expression, "\[.+\]").Value
            Dim array As String = If(Not String.IsNullOrEmpty(index), Expression.Replace(index, ""), Expression)

            Me.Array = New InternalExpression(array)
            index = Mid(index, 2, Len(index) - 2)

            Dim Tokens = New Parser.TextTokenliser.MSLTokens().Parsing(index).Tokens

            If Tokens.Length = 1 Then  ' $var,  ~First,  ~Last,  &const,  
                Me.Index = New Tokens.InternalExpression(index)
            ElseIf String.Equals(Tokens.First.GetTokenValue, "where", StringComparison.OrdinalIgnoreCase) Then
                Me.Index = New InternalExpression(String.Join(" ", (From t In Tokens.Skip(1) Select t.GetTokenValue.CLIToken).ToArray))
            ElseIf (From t In Tokens Where t.GetTokenValue.Last = "," Select 1).ToArray.Length = Tokens.Length - 1 Then
                Me.Index = New InternalExpression("{" & String.Join(" ", Tokens.Select(Function(t) t.GetTokenValue).ToArray) & "}")
            Else
                Throw New SyntaxErrorException(Expression.CLIToken & " is not a value element indexing expression!")
            End If
        End Sub
    End Class
End Namespace