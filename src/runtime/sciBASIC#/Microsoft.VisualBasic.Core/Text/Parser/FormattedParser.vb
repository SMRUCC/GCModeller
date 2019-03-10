﻿#Region "Microsoft.VisualBasic::523015a206ead44731de551c03512336, Microsoft.VisualBasic.Core\Text\Parser\FormattedParser.vb"

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

    '     Module FormattedParser
    ' 
    '         Function: CrossFields, FieldParser, FlagSplit
    '         Delegate Function
    ' 
    '             Function: ReadHead, UntilBlank
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Language

Namespace Text

    ''' <summary>
    ''' Parser API for the well formatted documents.
    ''' </summary>
    Public Module FormattedParser

        ''' <summary>
        ''' String collection tokens by a certain delimiter string element.
        ''' </summary>
        ''' <param name="source"></param>
        ''' <param name="isFlag">
        ''' 
        ''' </param>
        ''' <returns></returns>
        <Extension> Public Iterator Function FlagSplit(source As IEnumerable(Of String), isFlag As Func(Of String, Boolean)) As IEnumerable(Of String())
            Dim list As New List(Of String)

            ' >> .........
            ' ............
            ' ............
            ' >> .........
            ' >> .........

            ' 例如上面的一段文本，使用 >> 符号作为段落的分隔符，
            ' 则本函数会将上面的文本分为三行:
            ' >> .........
            ' ............
            ' ............
            ' 和
            ' >> .........
            ' 和
            ' >> .........

            For Each line As String In source
                If isFlag(line) Then
                    If list.Count > 0 Then
                        Yield list.ToArray
                        Call list.Clear()
                    End If
                End If

                Call list.Add(line)
            Next

            If list.Count > 0 Then
                Yield list.ToArray
            End If
        End Function

        ''' <summary>
        ''' Example as: ------- ------ -----    ------- ------ -----   ---- --  --------   ----------- 
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        <Extension> Public Function CrossFields(s As String) As Integer()
            Dim sps As String() = Regex.Matches(s, "\s+").ToArray
            Dim lens As String() = Regex.Matches(s, "-+").ToArray
            Dim fieldLens As New List(Of Integer)

            For i As Integer = 0 To lens.Length - 1
                fieldLens += sps(i).Length
                fieldLens += lens(i).Length
            Next

            Dim fields As Integer() = fieldLens.ToArray
            Return fields
        End Function

        ''' <summary>
        ''' Parsing a line of string into several fields fragments based on the fields length.
        ''' (假设表格之中的每一个字段都是等长的话, 则可以使用这个函数来进行解析)
        ''' </summary>
        ''' <param name="s">The input text line.</param>
        ''' <param name="fieldLength">The text length of each field property value.</param>
        ''' <returns></returns>
        <Extension> Public Iterator Function FieldParser(s As String, fieldLength As Integer()) As IEnumerable(Of String)
            Dim offset As Integer = Scan0

            For Each len As Integer In fieldLength.Take(fieldLength.Length - 1)
                ' 起始的位置是根据域的长度逐步叠加的
                Yield s.Substring(offset, len)
                ' 步进到下一个字段的起始位置
                offset += len
            Next

            Yield s.Substring(offset)
        End Function

        ''' <summary>
        ''' Condition for continue move the parser pointer.
        ''' </summary>
        ''' <param name="s"></param>
        ''' <returns></returns>
        Public Delegate Function DoContinute(s As String) As Boolean

        ''' <summary>
        ''' Parsing the document head section from the document.
        ''' </summary>
        ''' <param name="buf"></param>
        ''' <param name="offset">
        ''' This function will returns the new offset value from this reference parameter.
        ''' (从这里向调用者返回偏移量)
        ''' </param>
        ''' <param name="__isHead">Condition for continue move the parser pointer to the next line.</param>
        ''' <returns></returns>
        <Extension>
        Public Function ReadHead(buf As String(), ByRef offset As Integer, __isHead As DoContinute) As String()
            Do While __isHead(buf.Read(offset))
            Loop

            Dim copy As String() = New String(offset - 1) {}
            Call Array.ConstrainedCopy(buf, Scan0, copy, Scan0, offset)
            Return copy
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function UntilBlank(s As String) As Boolean
            Return Not s.StringEmpty
        End Function
    End Module
End Namespace
