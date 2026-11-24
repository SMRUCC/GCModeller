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

Imports Microsoft.VisualBasic.Linq

Namespace Assembly.NCBI.GenBank.GBFF.Keywords.FEATURES

    ''' <summary>
    ''' ``FEATURES``段之中位点(location)字符串的解析器。
    ''' </summary>
    ''' <remarks>
    ''' 旧版本的实现针对每一个位点都要执行2~3次正则表达式匹配(<c>Regex.Matches</c>以及
    ''' <c>Regex.Split</c>)，在人基因组规模的数据上面(约300万次调用)这是不可接受的。
    ''' 
    ''' 这里改为一个零正则的递归下降解析器，时间复杂度为<c>O(len(location))</c>，
    ''' 解析过程之中不会抛出异常(非法的输入会被降级处理并且计数)。
    ''' 
    ''' 支持的语法:
    ''' 
    ''' ```
    ''' 1..100
    ''' &lt;1..100
    ''' 1..&gt;100
    ''' 100
    ''' 100^101
    ''' complement(1..100)
    ''' join(1..100,201..300)
    ''' order(1..100,201..300)
    ''' complement(join(1..100,201..300))
    ''' join(complement(1..100),complement(201..300))
    ''' ```
    ''' </remarks>
    Public Module LocationParser

        ''' <summary>
        ''' 解析过程之中所遇到的非法输入的数量(用于诊断)
        ''' </summary>
        Public Property InvalidLocationCount As Long = 0

        ''' <summary>
        ''' 解析一个位点的表达式
        ''' </summary>
        ''' <param name="location">位点的表达式文本</param>
        ''' <returns>解析之后的位点对象，非法的输入会返回一个空的位点对象而不是抛出异常</returns>
        Public Function Parse(location As String) As Location
            If String.IsNullOrEmpty(location) Then
                Return Empty()
            End If

            Dim parser As New LocationSyntaxParser(location)
            Dim loci As Location = parser.Parse()

            If loci Is Nothing OrElse loci.SegmentCount = 0 Then
                InvalidLocationCount += 1

                If loci Is Nothing Then
                    Return Empty()
                End If
            End If

            Return loci
        End Function

        ''' <summary>
        ''' 一个空的位点对象
        ''' </summary>
        Public Function Empty() As Location
            Return New Location With {
                .Complement = False,
                .IsOrder = False,
                .IsPartial = False,
                .Locations = New RegionSegment() {}
            }
        End Function
    End Module

    ''' <summary>
    ''' 位点表达式的递归下降解析器
    ''' </summary>
    Friend NotInheritable Class LocationSyntaxParser

        Private ReadOnly text As String
        Private pos As Integer

        Sub New(text As String)
            Me.text = If(text, "").Trim()
        End Sub

        ''' <summary>
        ''' 解析整个表达式
        ''' </summary>
        Public Function Parse() As Location
            Dim loci As Location = __expression()

            If loci Is Nothing Then
                Return LocationParser.Empty()
            End If

            loci.IsPartial = loci.Locations.SafeQuery.Any(Function(seg) seg.LeftPartial OrElse seg.RightPartial)

            Return loci
        End Function

        ''' <summary>
        ''' 解析一个表达式: ``complement(...)``/``join(...)``/``order(...)``或者单个片段
        ''' </summary>
        Private Function __expression() As Location
            Call __skipWhitespace()

            Dim start As Integer = pos
            Dim name As String = __readName()

            If Not String.IsNullOrEmpty(name) AndAlso __eat("("c) Then
                Dim segments As New List(Of RegionSegment)
                Dim complement As Boolean = False
                Dim isOrder As Boolean = False

                Do
                    Call __skipWhitespace()

                    Dim item As Location = __expression()

                    If item Is Nothing Then
                        Exit Do
                    End If
                    If item.Complement Then
                        ' join 之中的每一个片段各自独立地进行反向互补
                        For Each seg As RegionSegment In item.Locations.SafeQuery
                            Call segments.Add(__flip(seg))
                        Next
                    Else
                        Dim parts As RegionSegment() = item.Locations

                        If Not parts.IsNullOrEmpty Then
                            Call segments.AddRange(parts)
                        End If
                    End If
                Loop While __eat(","c, skipWhitespace:=True)

                Call __expect(")"c)

                If String.Equals(name, "complement", StringComparison.OrdinalIgnoreCase) Then
                    complement = True
                ElseIf String.Equals(name, "order", StringComparison.OrdinalIgnoreCase) Then
                    isOrder = True
                End If

                Return New Location With {
                    .Complement = complement,
                    .IsOrder = isOrder,
                    .Locations = segments.ToArray
                }
            End If

            ' 不是函数形式的表达式: 回退为单个片段
            pos = start

            Dim segment As RegionSegment = __segment()

            If segment Is Nothing Then
                Return LocationParser.Empty()
            Else
                Return New Location With {
                    .Complement = False,
                    .IsOrder = False,
                    .Locations = {segment}
                }
            End If
        End Function

        ''' <summary>
        ''' 解析一个位点片段: ``[&lt;]start[..[&lt;|&gt;]right]``、``start^right``或者单点
        ''' </summary>
        Private Function __segment() As RegionSegment
            Call __skipWhitespace()

            Dim leftPartial As Boolean = False
            Dim rightPartial As Boolean = False
            Dim c As Char = __peek()

            If c = "<"c OrElse c = ">"c Then
                leftPartial = True
                rightPartial = (c = ">"c)
                pos += 1
            End If

            Dim start As Long

            If Not __readNumber(start) Then
                ' 无法识别的输入: 跳过当前的字符，避免死循环
                If pos < text.Length Then
                    pos += 1
                End If

                Return Nothing
            End If

            Call __skipWhitespace()

            If __eat("..") Then
                Call __skipWhitespace()

                Dim rp As Boolean = False
                Dim nextChar As Char = __peek()

                If nextChar = "<"c OrElse nextChar = ">"c Then
                    rp = True
                    pos += 1
                End If

                Dim [stop] As Long

                If Not __readNumber([stop]) Then
                    [stop] = start
                End If

                Return New RegionSegment With {
                    .Left = start,
                    .Right = [stop],
                    .LeftPartial = leftPartial,
                    .RightPartial = rp
                }
            ElseIf __eat("^"c) Then
                Call __skipWhitespace()

                Dim [stop] As Long

                If Not __readNumber([stop]) Then
                    [stop] = start + 1
                End If

                ' ``a^b``表示两个碱基之间的位点
                Return New RegionSegment With {
                    .Left = start,
                    .Right = [stop],
                    .LeftPartial = leftPartial,
                    .RightPartial = rightPartial
                }
            Else
                Return New RegionSegment With {
                    .Left = start,
                    .Right = start,
                    .LeftPartial = leftPartial,
                    .RightPartial = rightPartial
                }
            End If
        End Function

        Private Shared Function __flip(seg As RegionSegment) As RegionSegment
            If seg Is Nothing Then
                Return Nothing
            End If

            Return New RegionSegment With {
                .Left = seg.Left,
                .Right = seg.Right,
                .Complement = Not seg.Complement,
                .LeftPartial = seg.LeftPartial,
                .RightPartial = seg.RightPartial
            }
        End Function

        Private Sub __skipWhitespace()
            Do While pos < text.Length
                Dim c As Char = text(pos)

                If c = " "c OrElse c = ControlChars.Tab OrElse c = ControlChars.Cr OrElse c = ControlChars.Lf Then
                    pos += 1
                Else
                    Exit Do
                End If
            Loop
        End Sub

        Private Function __peek() As Char
            If pos < text.Length Then
                Return text(pos)
            Else
                Return ControlChars.NullChar
            End If
        End Function

        ''' <summary>
        ''' 读取一个标识符(函数名)
        ''' </summary>
        Private Function __readName() As String
            Dim start As Integer = pos

            Do While pos < text.Length
                Dim c As Char = text(pos)

                If (c >= "a"c AndAlso c <= "z"c) OrElse (c >= "A"c AndAlso c <= "Z"c) OrElse c = "_"c Then
                    pos += 1
                Else
                    Exit Do
                End If
            Loop

            If pos = start Then
                Return Nothing
            Else
                Return text.Substring(start, pos - start)
            End If
        End Function

        Private Function __readNumber(ByRef value As Long) As Boolean
            Call __skipWhitespace()

            Dim start As Integer = pos
            Dim n As Long = 0

            Do While pos < text.Length
                Dim c As Char = text(pos)

                If c >= "0"c AndAlso c <= "9"c Then
                    n = n * 10 + (AscW(c) - AscW("0"c))
                    pos += 1
                Else
                    Exit Do
                End If
            Loop

            If pos = start Then
                Return False
            Else
                value = n
                Return True
            End If
        End Function

        ''' <summary>
        ''' 尝试匹配一个指定的字符(匹配成功的时候会移动游标)
        ''' </summary>
        Private Function __eat(c As Char, Optional skipWhitespace As Boolean = False) As Boolean
            If skipWhitespace Then
                Call __skipWhitespace()
            End If

            If pos < text.Length AndAlso text(pos) = c Then
                pos += 1
                Return True
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' 尝试匹配一段指定的文本(匹配成功的时候会移动游标)
        ''' </summary>
        Private Function __eat(token As String) As Boolean
            If pos + token.Length > text.Length Then
                Return False
            End If

            If String.CompareOrdinal(text, pos, token, 0, token.Length) = 0 Then
                pos += token.Length
                Return True
            Else
                Return False
            End If
        End Function

        Private Sub __expect(c As Char)
            Call __skipWhitespace()

            If pos < text.Length AndAlso text(pos) = c Then
                pos += 1
            End If
        End Sub
    End Class
End Namespace
