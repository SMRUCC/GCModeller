﻿#Region "Microsoft.VisualBasic::094493db5fc159bbc472b3114c3e7649, Data\DataFrame\IO\csv\Tokenizer.vb"

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

    '     Module Tokenizer
    ' 
    '         Function: CharsParser, IsEmptyRow, RegexTokenizer
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Option Explicit On
Option Strict Off

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Emit.Marshal
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Scripting.TokenIcer
Imports Microsoft.VisualBasic.Text

Namespace IO

    ''' <summary>
    ''' RowObject parsers
    ''' </summary>
    Public Module Tokenizer

        ''' <summary>
        ''' A regex expression string that use for split the line text.
        ''' </summary>
        ''' <remarks></remarks>
        Const SplitRegxExpression As String = "[" & vbTab & ",](?=(?:[^""]|""[^""]*"")*$)"

        ''' <summary>
        ''' Parsing the row data from the input string line.(通过正则表达式来解析域)
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Function RegexTokenizer(s As String) As List(Of String)
            If String.IsNullOrEmpty(s) Then
                Return New List(Of String)
            End If

            Dim Row As String() = Regex.Split(s, SplitRegxExpression)
            For i As Integer = 0 To Row.Length - 1
                s = Row(i)

                If Not String.IsNullOrEmpty(s) AndAlso s.Length > 1 Then
                    If s.First = """"c AndAlso s.Last = """"c Then
                        s = Mid(s, 2, s.Length - 2)
                    End If
                End If

                Row(i) = s
            Next

            Return Row.AsList
        End Function

        ''' <summary>
        ''' 通过Chars枚举来解析域，分隔符默认为逗号
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Function CharsParser(s$, Optional delimiter As Char = ","c, Optional quot As Char = ASCII.Quot) As List(Of String)
            ' row data 
            Dim tokens As New List(Of String)
            Dim temp As New List(Of Char)
            ' 解析器是否是处于由双引号所产生的栈之中？
            Dim openStack As Boolean = False
            Dim buffer As New Pointer(Of Char)(s)
            Dim doubleQuot$ = quot & quot

            Do While Not buffer.EndRead
                Dim c As Char = +buffer

                If openStack Then

                    If c = quot Then

                        ' \" 会被转义为单个字符 "
                        If temp.StartEscaping Then
                            Call temp.RemoveLast
                            Call temp.Add(c)
                        Else
                            ' 查看下一个字符是否为分隔符
                            ' 因为前面的 Dim c As Char = +buffer 已经位移了，所以在这里直接取当前的字符
                            Dim peek = buffer.Current
                            ' 也有可能是 "" 转义 为单个 "
                            Dim lastQuot = (temp > 0 AndAlso temp.Last <> quot)

                            If temp = 0 AndAlso peek = delimiter Then

                                ' openStack意味着前面已经出现一个 " 了
                                ' 这里又出现了一个 " 并且下一个字符为分隔符
                                ' 则说明是 "", 当前的cell内容是一个空字符串
                                tokens += ""
                                temp *= 0
                                buffer += 1
                                openStack = False

                            ElseIf (peek = delimiter OrElse buffer.EndRead) AndAlso lastQuot Then

                                ' 下一个字符为分隔符，则结束这个token
                                tokens += New String(temp).Replace(doubleQuot, quot)
                                temp *= 0
                                ' 跳过下一个分隔符，因为已经在这里判断过了
                                buffer += 1
                                openStack = False

                            Else
                                ' 不是，则继续添加
                                temp += c
                            End If
                        End If
                    Else
                        ' 由于双引号而产生的转义                   
                        temp += c
                    End If
                Else
                    If temp.Count = 0 AndAlso c = quot Then
                        ' token的第一个字符串为双引号，则开始转义
                        openStack = True
                    Else
                        If c = delimiter Then
                            tokens += New String(temp).Replace(doubleQuot, quot)
                            temp *= 0
                        Else
                            temp += c
                        End If
                    End If
                End If
            Loop

            If temp.Count > 0 Then
                tokens += New String(temp).Replace(doubleQuot, quot)
            End If

            Return tokens
        End Function

        ''' <summary>
        ''' 是否等于``,,,,,,,,,``
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        <Extension> Public Function IsEmptyRow(s$, del As Char) As Boolean
            Dim l% = Strings.Len(s)

            If l = 0 Then
                Return True
            End If

            For Each c As Char In s
                If c = del Then
                    l -= 1
                End If
            Next

            ' 长度为零说明整个字符串都是分隔符，即为空行
            Return l = 0
        End Function
    End Module
End Namespace
