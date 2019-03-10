﻿#Region "Microsoft.VisualBasic::4793f0797bad90dd6657b4522bb2cc62, analysis\SequenceToolkit\SequencePatterns.Abstract\Patterns\PatternParser.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
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



    ' /********************************************************************************/

    ' Summaries:

    '     Module PatternParser
    ' 
    '         Function: ExpressionParser, GetExpressions, SimpleTokens, TokenIcer
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.TokenIcer

Namespace Motif.Patterns

    Public Module PatternParser

        ''' <summary>
        ''' MEME的表达式，不会有复杂的计算过程
        ''' </summary>
        ''' <param name="pattern">所有的字母都是大写</param>
        ''' <returns></returns>
        Public Function SimpleTokens(pattern As String) As String()
            Dim tokens As New List(Of String)
            Dim chars As CharEnumerator = pattern.GetEnumerator

            Do While chars.MoveNext
                If __residues.Contains(chars.Current) Then
                    tokens += CStr(chars.Current)
                ElseIf chars.Current = "["c Then
                    Dim temp As New List(Of Char)

                    Do While chars.Current <> "]"c
                        temp += chars.Current
                        chars.MoveNext()
                    Loop
                    temp += chars.Current
                    tokens += New String(temp.ToArray)
                Else
                    Throw New Exception("Illegal characters in the pattern:  " & pattern)
                End If
            Loop

            Return tokens.ToArray
        End Function

        Public Function ExpressionParser(pattern As String) As PatternExpression
            Dim tokens As List(Of Token(Of Tokens)) = TokenIcer(pattern).AsList
            Dim motif As New PatternExpression With {
                .RangeExpr = (From x In tokens Where x.Type = Patterns.Tokens.Expression Select x).ToArray
            }
            For Each x As Token(Of Tokens) In motif.RangeExpr
                tokens -= x
            Next

            Dim enums As New Pointer(Of Token(Of Tokens))(tokens)
            Dim residues As New List(Of Residue)

            Do While Not enums.EndRead
                Dim t As Token(Of Tokens) = +(enums)
                Dim ranges As Ranges = Nothing

                If enums.Current.Type = Patterns.Tokens.QualifyingNumber Then
                    ranges = New Ranges(enums.Current)
                    Dim null = (+enums)
                End If

                If t.Type = Patterns.Tokens.Fragment Then
                    residues += New Residue(t.Text) With {
                        .RepeatRanges = ranges
                    }
                ElseIf t.Type = Patterns.Tokens.Residue Then
                    residues += New Residue(t.Text) With {
                        .RepeatRanges = ranges
                    }
                ElseIf t.Type = Patterns.Tokens.QualifyingMatches Then
                    residues += New Residue($"[{t.Text}]") With {
                        .RepeatRanges = ranges
                    }
                End If
            Loop

            motif.Motif = residues.ToArray

            Return motif
        End Function

        ' -, N -> .

        ' aAtTTN
        ' aAtT{3,5}N3
        ' (atT){x}(T3GG){x}

        ''' <summary>
        ''' 解析为词元
        ''' </summary>
        ''' <param name="pattern"></param>
        ''' <returns></returns>
        Public Function TokenIcer(pattern As String) As Token(Of Tokens)()
            Dim result As List(Of Token(Of Tokens)) = GetExpressions(pattern)
            Dim expr As Pointer(Of Char) = New Pointer(Of Char)(Regex.Replace(pattern, "[-nN]", ".").ToArray)

            Do While Not expr.EndRead
                Dim c As Char = +expr
                If __residues.Contains(c) Then
                    result += New Token(Of Tokens)(Tokens.Residue, CStr(c))
                ElseIf c = "{"c Then
                    Dim temp As New List(Of Char)
                    Do While Not expr.EndRead AndAlso expr.Current <> "}"c
                        temp += +expr
                    Loop
                    result += New Token(Of Tokens)(Tokens.QualifyingNumber, New String(temp.ToArray))
                    c = +expr
                ElseIf c = "("c Then
                    Dim temp As New List(Of Char)
                    Do While Not expr.EndRead AndAlso expr.Current <> ")"c
                        temp += +expr
                    Loop
                    result += New Token(Of Tokens)(Tokens.Fragment, New String(temp.ToArray))
                    c = +expr
                ElseIf c = "["c Then
                    Dim temp As New List(Of Char)
                    Do While Not expr.EndRead AndAlso expr.Current <> "]"c
                        temp += +expr
                    Loop
                    result += New Token(Of Tokens)(Tokens.QualifyingMatches, New String(temp.ToArray))
                    c = +expr
                ElseIf __numbers.Contains(c) Then
                    Dim temp As List(Of Char) = New List(Of Char)({c})
                    Do While Not expr.EndRead AndAlso __numbers.Contains(expr.Current)
                        temp += +expr
                    Loop
                    result += New Token(Of Tokens)(Tokens.QualifyingNumber, New String(temp.ToArray))
                Else
                    Throw New SyntaxErrorException(pattern & " contains illegal character!")
                End If
            Loop

            Return result.ToArray
        End Function

        ''' <summary>
        ''' 得到末尾的变量表达式
        ''' </summary>
        ''' <param name="pattern"></param>
        ''' <returns></returns>
        Public Function GetExpressions(ByRef pattern As String) As List(Of Token(Of Tokens))
            Dim ms As String() = Regex.Matches(pattern, x, RegexOptions.IgnoreCase).ToArray
            Dim sb As New StringBuilder(pattern)
            For Each s As String In ms
                Call sb.Replace(s, "")
            Next

            pattern = sb.ToString.Trim
            pattern = Mid(pattern, 1, pattern.Length - 1)  ' 尾部有一个分隔符的，去除掉

            Return ms.ToList(
            Function(m) New Token(Of Tokens)(
                Tokens.Expression,
                If(m.Last = ";"c, Mid(m, 1, m.Length - 1), m)))
        End Function

        Const x As String = "[a-z][a-z0-9]*\s*=\s*\{.+?\};?"
        Const __residues As String = "aAtTgGcC."
        Const __numbers As String = "0123456789+*?"
    End Module
End Namespace
