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

Namespace Assembly.NCBI.GenBank.GBFF

    ''' <summary>
    ''' 快速的核酸序列操作工具。
    ''' </summary>
    ''' <remarks>
    ''' 这里的全部操作都是基于查表或者单遍扫描实现的，不会产生中间的<c>StringBuilder</c>，
    ''' 也不会使用<c>LINQ</c>表达式，因为在人基因组级别的数据上面，这些操作会被执行数百万次。
    ''' 
    ''' 所有的坐标都是<b>1-based的闭区间</b>。
    ''' </remarks>
    Public Module NucleotideSlice

        ''' <summary>
        ''' 互补碱基的查表(长度为256，覆盖了Latin1的全部字符)
        ''' </summary>
        Private ReadOnly complementTable As Char() = __createComplementTable()

        Private Function __createComplementTable() As Char()
            Dim table As Char() = New Char(255) {}

            For i As Integer = 0 To 255
                table(i) = ChrW(i)
            Next

            Call __map(table, "A"c, "T"c)
            Call __map(table, "C"c, "G"c)
            Call __map(table, "G"c, "C"c)
            Call __map(table, "T"c, "A"c)
            Call __map(table, "U"c, "A"c)

            ' 简并碱基
            Call __map(table, "R"c, "Y"c)
            Call __map(table, "Y"c, "R"c)
            Call __map(table, "K"c, "M"c)
            Call __map(table, "M"c, "K"c)
            Call __map(table, "B"c, "V"c)
            Call __map(table, "V"c, "B"c)
            Call __map(table, "D"c, "H"c)
            Call __map(table, "H"c, "D"c)
            Call __map(table, "S"c, "S"c)
            Call __map(table, "W"c, "W"c)
            Call __map(table, "N"c, "N"c)

            Return table
        End Function

        Private Sub __map(table As Char(), upper As Char, complement As Char)
            Dim lower As Char = Char.ToLowerInvariant(upper)

            table(AscW(upper)) = complement
            table(AscW(lower)) = Char.ToLowerInvariant(complement)
        End Sub

        ''' <summary>
        ''' 判断目标字符是否为一个字母(即序列数据之中的碱基或者氨基酸字符)。
        ''' 
        ''' 这里不按照列位置来判断，因为当序列的位置超过1亿的时候，
        ''' ``ORIGIN``段行首的位置号会占据9列，从而使得序列的起始列位置发生变化。
        ''' </summary>
        Public Function IsLetter(c As Char) As Boolean
            Return (c >= "A"c AndAlso c <= "Z"c) OrElse (c >= "a"c AndAlso c <= "z"c)
        End Function

        ''' <summary>
        ''' 从``ORIGIN``段的一行文本之中提取出碱基字符，写入到目标缓冲区之中。
        ''' </summary>
        ''' <param name="line">``ORIGIN``段的文本行</param>
        ''' <param name="buffer">目标缓冲区</param>
        ''' <param name="offset">写入的起始位置</param>
        ''' <returns>实际写入的字符数量</returns>
        Public Function ExtractLetters(line As String, buffer As Char(), offset As Integer) As Integer
            If String.IsNullOrEmpty(line) Then
                Return 0
            End If
            If buffer Is Nothing Then
                Return 0
            End If

            Dim j As Integer = offset

            For i As Integer = 0 To line.Length - 1
                Dim c As Char = line(i)

                If (c >= "A"c AndAlso c <= "Z"c) OrElse (c >= "a"c AndAlso c <= "z"c) Then
                    buffer(j) = c
                    j += 1
                End If
            Next

            Return j - offset
        End Function

        ''' <summary>
        ''' 统计一行``ORIGIN``段文本之中的碱基数量
        ''' </summary>
        Public Function CountLetters(line As String) As Integer
            If String.IsNullOrEmpty(line) Then
                Return 0
            End If

            Dim n As Integer = 0

            For i As Integer = 0 To line.Length - 1
                Dim c As Char = line(i)

                If (c >= "A"c AndAlso c <= "Z"c) OrElse (c >= "a"c AndAlso c <= "z"c) Then
                    n += 1
                End If
            Next

            Return n
        End Function

        ''' <summary>
        ''' 序列的切片(1-based的闭区间，超出范围的部分会被钳制)
        ''' </summary>
        ''' <param name="seq">目标序列</param>
        ''' <param name="left">起始位置(从1开始)</param>
        ''' <param name="right">结束位置(包含)</param>
        Public Function Slice(seq As String, left As Long, right As Long) As String
            If String.IsNullOrEmpty(seq) Then
                Return ""
            End If

            Dim l As Long = left
            Dim r As Long = right

            If l > r Then
                l = right
                r = left
            End If

            If l < 1 Then
                l = 1
            End If
            If r > seq.Length Then
                r = seq.Length
            End If
            If r < l Then
                Return ""
            End If

            Return seq.Substring(CInt(l - 1), CInt(r - l + 1))
        End Function

        ''' <summary>
        ''' 将多个序列片段按照顺序拼接为一个字符串
        ''' </summary>
        Public Function Concat(segments As IEnumerable(Of String)) As String
            If segments Is Nothing Then
                Return ""
            End If

            Dim list As New List(Of String)
            Dim total As Long = 0

            For Each s As String In segments
                If Not String.IsNullOrEmpty(s) Then
                    Call list.Add(s)
                    total += s.Length
                End If
            Next

            If list.Count = 0 Then
                Return ""
            ElseIf list.Count = 1 Then
                Return list(0)
            ElseIf total > Integer.MaxValue Then
                Throw New GbkFormatException($"the sequence data is too large to be concatenated: {total} bp!")
            End If

            Dim buffer As Char() = New Char(CInt(total) - 1) {}
            Dim offset As Integer = 0

            For Each s As String In list
                Call s.CopyTo(0, buffer, offset, s.Length)
                offset += s.Length
            Next

            Return New String(buffer)
        End Function

        ''' <summary>
        ''' 序列的反向互补
        ''' </summary>
        Public Function ReverseComplement(seq As String) As String
            If String.IsNullOrEmpty(seq) Then
                Return ""
            End If

            Dim out As Char() = New Char(seq.Length - 1) {}
            Dim last As Integer = seq.Length - 1

            For i As Integer = 0 To last
                out(last - i) = complementTable(AscW(seq(i)) And &HFF)
            Next

            Return New String(out)
        End Function

        ''' <summary>
        ''' 序列的互补(不反转顺序)
        ''' </summary>
        Public Function Complement(seq As String) As String
            If String.IsNullOrEmpty(seq) Then
                Return ""
            End If

            Dim out As Char() = New Char(seq.Length - 1) {}

            For i As Integer = 0 To seq.Length - 1
                out(i) = complementTable(AscW(seq(i)) And &HFF)
            Next

            Return New String(out)
        End Function

        ''' <summary>
        ''' 将互补之后的序列写入到目标缓冲区之中(不反转顺序)
        ''' </summary>
        ''' <returns>写入的字符数量</returns>
        Public Function ComplementInto(dst As Char(), src As String, offset As Integer) As Integer
            If String.IsNullOrEmpty(src) OrElse dst Is Nothing Then
                Return 0
            End If

            For i As Integer = 0 To src.Length - 1
                dst(offset + i) = complementTable(AscW(src(i)) And &HFF)
            Next

            Return src.Length
        End Function

        ''' <summary>
        ''' 将反向互补之后的序列写入到目标缓冲区之中
        ''' </summary>
        ''' <returns>写入的字符数量</returns>
        Public Function ReverseComplementInto(dst As Char(), src As String, offset As Integer) As Integer
            If String.IsNullOrEmpty(src) OrElse dst Is Nothing Then
                Return 0
            End If

            Dim last As Integer = src.Length - 1

            For i As Integer = 0 To last
                dst(offset + last - i) = complementTable(AscW(src(i)) And &HFF)
            Next

            Return src.Length
        End Function

        ''' <summary>
        ''' 计算序列的GC偏移: ``(G - C) / (G + C)``
        ''' </summary>
        Public Function GCSkew(seq As String) As Double
            If String.IsNullOrEmpty(seq) Then
                Return 0
            End If

            Dim G As Long = 0
            Dim C As Long = 0

            For i As Integer = 0 To seq.Length - 1
                Select Case seq(i)
                    Case "G"c, "g"c
                        G += 1
                    Case "C"c, "c"c
                        C += 1
                End Select
            Next

            If (G + C) = 0 Then
                Return 0
            Else
                Return (G - C) / CDbl(G + C)
            End If
        End Function
    End Module
End Namespace
