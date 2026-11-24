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

Imports System.Text
Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Linq

Namespace Assembly.NCBI.GenBank.GBFF.Keywords

    ''' <summary>
    ''' ``CONTIG``段之中的一个片段。
    ''' </summary>
    ''' <remarks>
    ''' 形如:
    ''' 
    ''' ```
    ''' CONTIG      join(gap(10000),NT_077402.3:1..197666,gap(50000),NT_077402.3:247666..500000)
    ''' ```
    ''' </remarks>
    Public Class ContigSegment

        ''' <summary>
        ''' 片段所属的记录的编号(``gap``片段为空)
        ''' </summary>
        <XmlAttribute> Public Property Accession As String
        <XmlAttribute> Public Property Left As Long
        <XmlAttribute> Public Property Right As Long
        ''' <summary>
        ''' 这个片段是否为一个``gap``(即未知的序列区域)
        ''' </summary>
        <XmlAttribute> Public Property IsGap As Boolean
        ''' <summary>
        ''' ``gap``片段的长度(仅当<see cref="IsGap"/>=<c>True</c>的时候有效)
        ''' </summary>
        <XmlAttribute> Public Property GapLength As Long

        ''' <summary>
        ''' 片段所包含的碱基数量
        ''' </summary>
        Public ReadOnly Property Length As Long
            Get
                If IsGap Then
                    Return GapLength
                Else
                    Return Math.Abs(Right - Left) + 1
                End If
            End Get
        End Property

        ''' <summary>
        ''' 生成单个片段的文本表示
        ''' </summary>
        Public Overrides Function ToString() As String
            If IsGap Then
                Return $"gap({GapLength})"
            End If

            Dim range As String

            If Left = Right Then
                range = Left.ToString()
            Else
                range = $"{Left}..{Right}"
            End If

            If String.IsNullOrEmpty(Accession) Then
                Return range
            Else
                Return $"{Accession}:{range}"
            End If
        End Function
    End Class

    ''' <summary>
    ''' ``CONTIG``段: 对于``CON``(constructed)类型的记录(例如人基因组的染色体记录)，
    ''' 这个字段描述了序列是由哪些片段拼接而成的。
    ''' </summary>
    <XmlType("gbff-contig")>
    Public Class CONTIG : Inherits KeyWord

        ''' <summary>
        ''' 组成序列的全部片段
        ''' </summary>
        <XmlArrayItem("segment")>
        Public Property Segments As ContigSegment()

        ''' <summary>
        ''' 全部片段所拼接出来的序列的总长度
        ''' </summary>
        Public ReadOnly Property TotalLength As Long
            Get
                Dim n As Long = 0

                For Each seg As ContigSegment In Segments.SafeQuery
                    n += seg.Length
                Next

                Return n
            End Get
        End Property

        ''' <summary>
        ''' 是否含有未知序列(gap)的片段
        ''' </summary>
        Public ReadOnly Property HasGap As Boolean
            Get
                For Each seg As ContigSegment In Segments.SafeQuery
                    If seg.IsGap Then
                        Return True
                    End If
                Next

                Return False
            End Get
        End Property

        ''' <summary>
        ''' 解析``CONTIG``段
        ''' </summary>
        ''' <param name="lines">``CONTIG``段的全部文本行(第一行为``CONTIG``字段名所在的行)</param>
        Public Shared Function Parser(lines As String()) As CONTIG
            If lines.IsNullOrEmpty Then
                Return New CONTIG With {.Segments = New ContigSegment() {}}
            End If

            Dim sb As New StringBuilder()
            Dim first As Boolean = True

            For Each line As String In lines
                If String.IsNullOrEmpty(line) Then
                    Continue For
                End If

                If first Then
                    ' 去掉第一行之中的字段名
                    Dim i As Integer = line.IndexOf(" "c)

                    If i >= 0 Then
                        Call sb.Append(line.Substring(i + 1).Trim())
                    End If

                    first = False
                Else
                    ' 折行的片段之间直接拼接，不会产生空白字符
                    Call sb.Append(line.Trim())
                End If
            Next

            Dim tokens As String() = __splitSegments(sb.ToString())
            Dim segments As ContigSegment() = tokens _
                .Select(AddressOf __parseSegment) _
                .Where(Function(s) s IsNot Nothing) _
                .ToArray

            Return New CONTIG With {.Segments = segments}
        End Function

        ''' <summary>
        ''' 取出``join(...)``或者``order(...)``括号之中的内容，并且按照顶层逗号进行分割
        ''' </summary>
        Private Shared Function __splitSegments(text As String) As String()
            If String.IsNullOrEmpty(text) Then
                Return New String() {}
            End If

            Dim start As Integer = text.IndexOf("("c)
            Dim endIndex As Integer = text.LastIndexOf(")"c)

            If start >= 0 AndAlso endIndex > start Then
                text = text.Substring(start + 1, endIndex - start - 1)
            End If

            Dim tokens As New List(Of String)
            Dim depth As Integer = 0
            Dim sliceStart As Integer = 0

            For i As Integer = 0 To text.Length - 1
                Select Case text(i)
                    Case "("c
                        depth += 1
                    Case ")"c
                        depth -= 1
                    Case ","c
                        If depth = 0 Then
                            Call tokens.Add(text.Substring(sliceStart, i - sliceStart))
                            sliceStart = i + 1
                        End If
                End Select
            Next

            If sliceStart < text.Length Then
                Call tokens.Add(text.Substring(sliceStart))
            End If

            Return tokens.ToArray
        End Function

        ''' <summary>
        ''' 解析单个片段: ``NT_077402.3:1..197666`` 或者 ``gap(10000)``
        ''' </summary>
        Private Shared Function __parseSegment(token As String) As ContigSegment
            If String.IsNullOrEmpty(token) Then
                Return Nothing
            End If

            token = token.Trim()

            If token.Length = 0 Then
                Return Nothing
            End If

            If token.StartsWith("gap", StringComparison.OrdinalIgnoreCase) Then
                Return New ContigSegment With {
                    .IsGap = True,
                    .GapLength = __number(token)
                }
            End If

            Dim seg As New ContigSegment()
            Dim range As String = token
            Dim colon As Integer = token.IndexOf(":"c)

            If colon > 0 Then
                seg.Accession = token.Substring(0, colon).Trim()
                range = token.Substring(colon + 1)
            End If

            Dim dot As Integer = range.IndexOf("..", StringComparison.Ordinal)

            If dot > 0 Then
                Call Long.TryParse(__numberText(range.Substring(0, dot)), seg.Left)
                Call Long.TryParse(__numberText(range.Substring(dot + 2)), seg.Right)
            Else
                Call Long.TryParse(__numberText(range), seg.Left)
                seg.Right = seg.Left
            End If

            If seg.Right < seg.Left Then
                Dim tmp As Long = seg.Left
                seg.Left = seg.Right
                seg.Right = tmp
            End If

            Return seg
        End Function

        ''' <summary>
        ''' 从文本之中提取出数字
        ''' </summary>
        Private Shared Function __number(token As String) As Long
            Dim value As Long = 0

            Call Long.TryParse(__numberText(token), value)

            Return value
        End Function

        Private Shared Function __numberText(text As String) As String
            If String.IsNullOrEmpty(text) Then
                Return ""
            End If

            Dim sb As New StringBuilder()

            For i As Integer = 0 To text.Length - 1
                Dim c As Char = text(i)

                If c >= "0"c AndAlso c <= "9"c Then
                    Call sb.Append(c)
                ElseIf sb.Length > 0 Then
                    Exit For
                End If
            Next

            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 按照NCBI的标准格式重新生成``CONTIG``段的文本表示
        ''' </summary>
        Public Overrides Function ToString() As String
            If Segments.IsNullOrEmpty Then
                Return ""
            End If

            Dim sb As New StringBuilder("join(")

            For i As Integer = 0 To Segments.Length - 1
                If i > 0 Then
                    Call sb.Append(",")
                End If

                Call sb.Append(Segments(i).ToString())
            Next

            Call sb.Append(")")

            Return sb.ToString()
        End Function
    End Class
End Namespace
