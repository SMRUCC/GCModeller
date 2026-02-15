#Region "Microsoft.VisualBasic::d0de1da6ded30277067c3e91b04d7727, core\Bio.Assembly\SequenceModel\NucleicAcid\Extensions.vb"

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


    ' Code Statistics:

    '   Total Lines: 272
    '    Code Lines: 164 (60.29%)
    ' Comment Lines: 54 (19.85%)
    '    - Xml Docs: 59.26%
    ' 
    '   Blank Lines: 54 (19.85%)
    '     File Size: 9.83 KB


    '     Module Extensions
    ' 
    '         Function: HammingDistance, LevenshteinDistance, LevenshteinDistance2, Minimum, PatternMatched
    '                   SegmentAssembler, Similarity
    ' 
    '         Sub: __assembly
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports std = System.Math

Namespace SequenceModel.NucleotideModels

    <Package("NucleotideModel.Statics")>
    <HideModuleName>
    Public Module Extensions

        <ExportAPI("Distance.Hamming")>
        Public Function HammingDistance(seq1 As String, seq2 As String) As Integer
            Dim Length As Integer = seq1.Length
            Dim HammingDist As Integer = 0

            If seq1.Length <> seq2.Length Then
                Length = std.Min(seq1.Length, seq2.Length)
                HammingDist = std.Abs(seq1.Length - seq2.Length)
            End If

            For i As Integer = 0 To Length - 1
                If seq1(i) <> seq2(i) Then
                    HammingDist += 1
                End If
            Next

            Return HammingDist
        End Function

        ''' <summary>
        ''' Compute Levenshtein distance  Michael Gilleland, Merriam Park Software.(http://www.merriampark.com/ld.htm)
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="t"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        Public Function LevenshteinDistance(s As String, t As String) As Integer
            Dim d As Integer()()  ' matrix
            Dim n As Integer = s.Length ' length of s
            Dim m As Integer = t.Length   ' length of t
            Dim s_i As Char ' ith character of s
            Dim t_j As Char  ' jth character of t
            Dim cost As Integer

            ' ========> Step 1

            If n = 0 Then
                Return m
            End If

            If m = 0 Then
                Return n
            End If

            d = RectangularArray.Matrix(Of Integer)(n + 1, m + 1)

            ' ========> Step 2

            For i As Integer = 0 To n
                d(i)(0) = i
            Next

            For j As Integer = 0 To m
                d(0)(j) = j
            Next

            ' ========> Step 3

            For i As Integer = 1 To n

                s_i = s(i - 1)

                ' ========> Step 4

                For j As Integer = 1 To m

                    t_j = t(j - 1)

                    ' ========> Step 5

                    If s_i = t_j Then
                        cost = 0
                    Else
                        cost = 1
                    End If

                    ' ========> Step 6

                    d(i)(j) = Minimum(d(i - 1)(j) + 1, d(i)(j - 1) + 1, d(i - 1)(j - 1) + cost)
                Next
            Next

            Return d(n)(m)
        End Function

        ''' <summary>
        ''' Chas Emerick.(http://www.merriampark.com/ldjava.htm)
        ''' </summary>
        ''' <param name="s"></param>
        ''' <param name="t"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' The difference between this impl. and the previous is that, rather
        ''' than creating and retaining a matrix of size s.length()+1 by t.length()+1,
        ''' we maintain two single-dimensional arrays of length s.length()+1.  The first, d,
        ''' is the 'current working' distance array that maintains the newest distance cost
        ''' counts as we iterate through the characters of String s.  Each time we increment
        ''' the index of String t we are comparing, d is copied to p, the second int[].  Doing so
        ''' allows us to retain the previous cost counts as required by the algorithm (taking
        ''' the minimum of the cost count to the left, up one, and diagonally up and to the left
        ''' of the current cost count being calculated).  (Note that the arrays aren't really
        ''' copied anymore, just switched...this is clearly much better than cloning an array
        ''' or doing a System.arraycopy() each time  through the outer loop.)
        ''' 
        ''' Effectively, the difference between the two implementations is this one does not
        ''' cause an out of memory condition when calculating the LD over two very large strings.
        ''' </remarks>
        ''' 
        Public Function LevenshteinDistance2(s As String, t As String) As Integer
            If String.IsNullOrEmpty(s) OrElse String.IsNullOrEmpty(t) Then
                Throw New ArgumentException("Strings must not be null")
            End If

            Dim n As Integer = s.Length   ' length of s
            Dim m As Integer = t.Length   ' length of t

            If n = 0 Then
                Return m
            ElseIf m = 0 Then
                Return n
            End If

            Dim p As Integer() = New Integer(n) {}
            ''previous' cost array, horizontally
            Dim d As Integer() = New Integer(n) {}
            ' cost array, horizontally
            Dim _d As Integer()
            'placeholder to assist in swapping p and d
            ' indexes into strings s and t
            Dim i As Integer
            ' iterates through s
            Dim j As Integer
            ' iterates through t
            Dim t_j As Char
            ' jth character of t
            Dim cost As Integer
            ' cost
            For i = 0 To n
                p(i) = i
            Next

            For j = 1 To m
                t_j = t(j - 1)
                d(0) = j

                For i = 1 To n
                    cost = If(s(i - 1) = t_j, 0, 1)
                    ' minimum of cell to the left+1, to the top+1, diagonally left and up +cost
                    d(i) = std.Min(Math.Min(d(i - 1) + 1, p(i) + 1), p(i - 1) + cost)
                Next

                ' copy current distance counts to 'previous row' distance counts
                _d = p
                p = d
                d = _d
            Next

            ' our last action in the above loop was to switch d and p, so p now
            ' actually has the most recent cost counts
            Return p(n)
        End Function

        <ExportAPI("Similarity")>
        Public Function Similarity(s1 As String, s2 As String) As Double
            Dim maxLength As Integer = std.Max(s1.Length, s2.Length)
            Dim value As Double = 1.0 - CDbl(LevenshteinDistance(s1, s2)) / maxLength
            Return value
        End Function

        <ExportAPI("Is.Pattern.Matched?")>
        Public Function PatternMatched(pattern1 As String, pattern2 As String, confidence As Double) As Boolean
            Dim PatternSimilarity As Double = Similarity(pattern1, pattern2)
            Return PatternSimilarity >= confidence
        End Function

        Private Function Minimum(a As Integer, b As Integer, c As Integer) As Integer
            Dim mi As Integer = a

            If b < mi Then
                mi = b
            End If

            If c < mi Then
                mi = c
            End If

            Return mi
        End Function

        ''' <summary>
        ''' 根据Jukes-Cantor模型计算校正后的进化距离。
        ''' </summary>
        ''' <param name="p">观测到的序列差异比例 (范围 0 到 0.75)。</param>
        ''' <returns>校正后的进化距离。如果输入无效，返回 Double.NaN。</returns>
        Public Function CalculateJukesCantorDistance(p As Double) As Double
            ' 输入验证
            If p < 0 OrElse p >= 0.75 Then
                ' 差异比例不能为负，且当 p >= 0.75 时，模型失效。
                Return Double.NaN
            End If

            If Math.Abs(p) < 0.000000000001 Then ' 处理p=0的情况，避免不必要的计算
                Return 0.0
            End If

            ' 应用 Jukes-Cantor 公式: d = - (3/4) * ln(1 - (4/3)*p)
            Dim term As Double = 1.0 - (4.0 / 3.0) * p
            If term <= 0 Then
                Return Double.NaN ' 理论上在 p < 0.75 时不应发生，此处为安全保护
            End If

            Dim distance As Double = -(3.0 / 4.0) * Math.Log(term)
            Return distance
        End Function

        ''' <summary>
        ''' 比较两条已对齐的DNA序列，并计算它们之间的Jukes-Cantor距离。
        ''' </summary>
        ''' <param name="sequence1">第一条DNA序列。</param>
        ''' <param name="sequence2">第二条DNA序列。</param>
        ''' <param name="ignoreCase">是否忽略大小写（默认忽略）。</param>
        ''' <param name="gapPenalty">如何处理缺失位点（-）：‘N’=视为未知不计入， ‘P’=视为差异， ‘S’=跳过整个位点。</param>
        ''' <returns>Jukes-Cantor距离。如果计算失败，返回 Double.NaN。</returns>
        Public Function CalculateJCFromSequences(sequence1 As String,
                                                 sequence2 As String,
                                                 Optional ignoreCase As Boolean = True,
                                                 Optional gapPenalty As String = "N") As Double

            ' 1. 基本检查
            If String.IsNullOrEmpty(sequence1) OrElse String.IsNullOrEmpty(sequence2) Then
                Return Double.NaN
            End If

            Dim seq1 As String = If(ignoreCase, sequence1.ToUpper(), sequence1)
            Dim seq2 As String = If(ignoreCase, sequence2.ToUpper(), sequence2)

            If seq1.Length <> seq2.Length Then
                Console.WriteLine("错误：序列长度不一致。")
                Return Double.NaN
            End If

            ' 2. 遍历序列，统计
            Dim comparedSites As Integer = 0
            Dim differentSites As Integer = 0

            For i As Integer = 0 To seq1.Length - 1
                Dim char1 As Char = seq1(i)
                Dim char2 As Char = seq2(i)

                ' 处理缺失/未知位点
                If char1 = "-"c OrElse char2 = "-"c OrElse char1 = "N"c OrElse char2 = "N"c Then
                    Select Case gapPenalty
                        Case "N"c ' 忽略此位点，不参与比较
                            Continue For
                        Case "P"c ' 视为差异
                            differentSites += 1
                            comparedSites += 1
                        Case "S"c ' 严格模式：遇到缺失即认为整个比对无效（示例中直接跳过）
                            Continue For
                        Case Else
                            Continue For
                    End Select
                Else
                    ' 正常核苷酸比较
                    comparedSites += 1
                    If char1 <> char2 Then
                        differentSites += 1
                    End If
                End If
            Next

            ' 3. 计算差异比例并调用核心JC函数
            If comparedSites = 0 Then
                Console.WriteLine("警告：没有可用于比较的有效位点。")
                Return Double.NaN
            End If

            Dim p As Double = differentSites / comparedSites
            Console.WriteLine($"信息：比较了 {comparedSites} 个位点，其中 {differentSites} 个不同，差异比例 p = {p:F4}")

            Return CalculateJukesCantorDistance(p)
        End Function
    End Module
End Namespace
