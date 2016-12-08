#Region "Microsoft.VisualBasic::7f3b058d81060f8dcdd1d39d58fce8d8, ..\sciBASIC.ComputingServices\LINQ\LINQ\LDM\Parser\ParserAPI\Tokenizer.vb"

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
Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq.LDM.Statements
Imports Microsoft.VisualBasic.Linq.LDM.Statements.TokenIcer
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace LDM.Parser

    ''' <summary>
    ''' Divides the string into tokens.
    ''' </summary>
    Public Class Tokenizer

        ReadOnly _tokens As Iterator(Of Token(Of Tokens))

        Dim _prevToken As Token = Token.NullToken

        ''' <summary>
        ''' A tokenizer is always constructed on a single string.  Create one tokenizer per string.
        ''' </summary>
        ''' <param name="s">string to tokenize</param>
        Public Sub New(s As String)
            Call Me.New(TokenIcer.GetTokens(s))
        End Sub

        Sub New(tokens As IEnumerable(Of Token(Of Tokens)))
            _tokens = New Iterator(Of Token(Of Tokens))(tokens)
        End Sub

        ''' <summary>
        ''' Moves to the next character.  If there are no more characters, then the tokenizer is
        ''' invalid.
        ''' </summary>
        Private Sub MoveNext()
            Call _tokens.MoveNext()

            'If Not _tokens.MoveNext() Then
            '    _IsInvalid = True
            'Else
            '    ' _prevToken = New Token(_tokens.GetCurrent)
            'End If
        End Sub

        Public Sub DirectlyMoveNext()
            Call _tokens.MoveNext()

            Dim token__1 As Token

            If IsChar Then
                token__1 = GetString()
            ElseIf IsComma Then
                token__1 = New Token(",", Tokens.ParamDeli, TokenPriority.None)
            ElseIf IsDot Then
                token__1 = New Token(".", Tokens.CallFunc, TokenPriority.None)
            ElseIf IsNumber Then
                token__1 = GetNumber()
            ElseIf IsSpace Then
                token__1 = GetNextToken()
            ElseIf IsOperator Then
                token__1 = GetOperator()
            ElseIf IsParens Then
                token__1 = GetOperator()
            Else
                token__1 = Token.NullToken
            End If

            _prevToken = token__1
        End Sub

        ''' <summary>
        ''' Is the current character a letter or underscore?
        ''' </summary>
        Public ReadOnly Property IsParens() As Boolean
            Get
                If IsInvalid Then
                    Return False
                Else
                    If _tokens.Current Is Nothing Then
                        Return False
                    End If
                End If
                Dim Current = _tokens.GetCurrent
                Return Current.TokenName = Tokens.LPair OrElse Current.TokenName = Tokens.RPair
            End Get
        End Property

        ''' <summary>
        ''' Allows access to the token most recently parsed.
        ''' </summary>
        Public ReadOnly Property Current() As Token
            Get
                Return _prevToken
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return Scripting.ToString(Current)
        End Function

        ''' <summary>
        ''' Indicates that there are no more characters in the string and tokenizer is finished.
        ''' </summary>
        Public ReadOnly Property IsInvalid() As Boolean
            Get
                Return _tokens.ReadDone AndAlso _tokens.Current Is Nothing
            End Get
        End Property

        ''' <summary>
        ''' Is the current character a letter or underscore?
        ''' </summary>
        Public ReadOnly Property IsChar() As Boolean
            Get
                If IsInvalid Then
                    Return False
                Else
                    If _tokens.Current Is Nothing Then
                        Return False
                    End If
                End If
                Dim Current = _tokens.GetCurrent
                Return Current.TokenName = Tokens.String OrElse Current.TokenName = Tokens.Identifier
            End Get
        End Property

        ''' <summary>
        ''' Is the current character a dot (".")?
        ''' </summary>
        Public ReadOnly Property IsDot() As Boolean
            Get
                If IsInvalid Then
                    Return False
                Else
                    If _tokens.Current Is Nothing Then
                        Return False
                    End If
                End If
                Return _tokens.GetCurrent.TokenName = Tokens.CallFunc
            End Get
        End Property

        ''' <summary>
        ''' Is the current character a comma?
        ''' </summary>
        Public ReadOnly Property IsComma() As Boolean
            Get
                If IsInvalid Then
                    Return False
                Else
                    If _tokens.Current Is Nothing Then
                        Return False
                    End If
                End If
                Return _tokens.GetCurrent.TokenName = Tokens.ParamDeli
            End Get
        End Property

        ''' <summary>
        ''' Is the current character a number?
        ''' </summary>
        Public ReadOnly Property IsNumber() As Boolean
            Get
                If IsInvalid Then
                    Return False
                Else
                    If _tokens.Current Is Nothing Then
                        Return False
                    End If
                End If
                Dim t As Tokens = _tokens.GetCurrent.TokenName
                Return t = Tokens.Integer OrElse t = Tokens.Float
            End Get
        End Property

        ''' <summary>
        ''' Is the current character a whitespace character?
        ''' </summary>
        Public ReadOnly Property IsSpace() As Boolean
            Get
                If IsInvalid Then
                    Return False
                Else
                    If _tokens.Current Is Nothing Then
                        Return False
                    End If
                End If
                Return _tokens.GetCurrent.TokenName = Tokens.WhiteSpace
            End Get
        End Property

        ''' <summary>
        ''' Is the current character an operator?
        ''' </summary>
        Public ReadOnly Property IsOperator() As Boolean
            Get
                If IsInvalid Then
                    Return False
                Else
                    If _tokens.Current Is Nothing Then
                        Return False
                    End If
                End If

                Return _tokens.GetCurrent.TokenName.IsOperator
            End Get
        End Property

        ''' <summary>
        ''' Gets the next token in the string.  Reads as many characters as necessary to retrieve
        ''' that token.
        ''' </summary>
        ''' <returns>next token</returns>
        Public Function GetNextToken() As Token
            If IsInvalid Then
                Return Token.NullToken
            End If

            Dim token__1 As Token
            If IsChar Then
                token__1 = GetString()
                MoveNext()
            ElseIf IsComma Then
                token__1 = New Token(",", Tokens.ParamDeli, TokenPriority.None)
                MoveNext()
            ElseIf IsDot Then
                token__1 = New Token(".", Tokens.CallFunc, TokenPriority.None)
                MoveNext()
            ElseIf IsNumber Then
                token__1 = GetNumber()
                MoveNext()
            ElseIf IsSpace Then
                ' Eat space and do recursive call.
                MoveNext()
                token__1 = GetNextToken()
            ElseIf IsOperator OrElse IsParens Then
                token__1 = GetOperator()
            Else
                token__1 = Token.NullToken
                MoveNext()
            End If

            _prevToken = token__1
            Return token__1
        End Function

        ''' <summary>
        ''' Anything that starts with a character is considered a string.  This could be a 
        ''' primitive quoted string, a primitive expression, or an identifier
        ''' </summary>
        ''' <returns></returns>
        Private Function GetString() As Token
            Dim s As String = _tokens.GetCurrent.TokenValue
            ' "false" or "true" is a primitive expression.
            If s = "false" OrElse s = "true" Then
                Return New Token([Boolean].Parse(s), Tokens.String, TokenPriority.None)
            End If

            ' The previous token was a quote, so this is a primitive string.
            Return New Token(_tokens.GetCurrent, TokenPriority.None)
        End Function

        ''' <summary>
        ''' A token that starts with a number can be an integer, a long, or a double.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks>
        ''' An integer is the default for numbers.  Numbers can also be followed by a
        ''' l, L, d, or D character to indicate a long or a double value respectively.
        ''' Any numbers containing a dot (".") are considered doubles.
        ''' </remarks>
        Private Function GetNumber() As Token
            Dim Current = _tokens.GetCurrent
            Dim s As String = Current.TokenValue

            If Current.TokenName = Tokens.Float Then
                Return New Token([Double].Parse(s), Tokens.String, TokenPriority.None)
            End If
            Return New Token(Int32.Parse(s), Tokens.String, TokenPriority.None)
        End Function

        ''' <summary>
        ''' Some operators take more than one character.  Also, the tokenizer is able to 
        ''' categorize the token's priority based on what kind of operator it is.
        ''' </summary>
        ''' <returns></returns>
        Private Function GetOperator() As Token
            Dim Current = _tokens.GetCurrent

            Select Case Current.TokenName
                Case Tokens.Equals, Tokens.Is, Tokens.GT, Tokens.GT_EQ, Tokens.LT, Tokens.LT_EQ
                    Return New Token(Current.TokenValue, Tokens.Is, TokenPriority.Equality)
                Case Tokens.Minus
                    MoveNext()
                    If _prevToken.TokenName = Primitive OrElse _prevToken.Type = Tokens.Identifier Then
                        Return New Token(Current.TokenValue, Tokens.Minus, TokenPriority.PlusMinus)
                    Else
                        Return New Token(Current.TokenValue, Tokens.Minus, TokenPriority.UnaryMinus)
                    End If
                Case Tokens.Plus
                    MoveNext()
                    Return New Token(Current.TokenValue, Tokens.Plus, TokenPriority.PlusMinus)
                Case Tokens.Not
                    MoveNext()
                    If _tokens.GetCurrent.TokenName = Tokens.Equals Then
                        MoveNext()
                        Return New Token(_tokens.GetCurrent.TokenValue, Tokens.Equals, TokenPriority.Equality)
                    Else
                        Return New Token(_tokens.GetCurrent.TokenValue, Tokens.Not, TokenPriority.[Not])
                    End If
                Case Tokens.Asterisk, Tokens.Slash
                    MoveNext()
                    Return New Token(_tokens.GetCurrent.TokenValue, _tokens.GetCurrent.TokenName, TokenPriority.MulDiv)
                Case Tokens.Mod
                    MoveNext()
                    Return New Token("%", Tokens.Mod, TokenPriority.[Mod])
                Case Tokens.Or
                    MoveNext()
                    Return New Token(_tokens.GetCurrent.TokenValue, Tokens.Or, TokenPriority.[Or])
                Case Tokens.And
                    MoveNext()
                    Return New Token(_tokens.GetCurrent, Tokens.And, TokenPriority.[And])
                Case Tokens.LPair
                    MoveNext()
                    Return New Token("(", OpenParens, TokenPriority.None)
                Case Tokens.RPair
                    MoveNext()
                    Return New Token(")", CloseParens, TokenPriority.None)
                Case Tokens.OpenBracket
                    MoveNext()
                    Return New Token("[", Tokens.OpenBracket, TokenPriority.None)
                Case Tokens.CloseBracket
                    MoveNext()
                    Return New Token("]", Tokens.CloseBracket, TokenPriority.None)
                Case Else
                    ' When we detect a quote, we can just ignore it since the user doesn't really need to know about it.
                    MoveNext()
                    _prevToken = New Token(_tokens.GetCurrent.TokenValue, Tokens.String, TokenPriority.None)
                    Return GetString()
            End Select
            Return Token.NullToken
        End Function
    End Class
End Namespace
