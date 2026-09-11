' /********************************************************************************/
'
'  Rockhopper —— 后缀数组构建
'
'  论文（gkt444.pdf / 13059_2014_Article_572.pdf）要求对参考基因组建立基于全文后缀数组
'  的 Burrows-Wheeler 变换索引（FM-index）。原始 Rockhopper 使用"循环旋转 + 快速排序"
'  （见 Java/Replicon.vb 的 rotations()/quicksort()，取自 Bowtie 论文附录），时间复杂度
'  O(n² log n)，在大基因组上不可接受。
'
'  这里实现标准的**前缀倍增（prefix doubling）**算法，时间复杂度 O(n log² n)、
'  空间 O(n)，产出与原始实现完全一致的后缀数组（按字典序排序的后缀起始下标）。
'
' /********************************************************************************/

Imports System.Collections.Generic
Imports System.Linq

Namespace Alignment

    ''' <summary>
    ''' 后缀数组（Suffix Array）构建器。
    ''' </summary>
    Public Module SuffixArray

        ''' <summary>
        ''' 构建字符串 <paramref name="s"/> 的后缀数组：返回按字典序升序排列的后缀起始下标数组。
        ''' </summary>
        ''' <param name="s">输入字符串（建议先规范化为 A/C/G/T 等单字节字符）。</param>
        ''' <remarks>
        ''' 使用前缀倍增 + 计数排序（radix sort），每一轮将 rank 相同的后缀按 2^k 长度细分，
        ''' 直到所有 rank 互不相同。
        ''' </remarks>
        Public Function Build(s As String) As Integer()
            Dim n As Integer = s.Length
            If n = 0 Then Return New Integer() {}
            If n = 1 Then Return New Integer() {0}

            ' 初始 rank：按字符 ASCII 值
            Dim sa As Integer() = Enumerable.Range(0, n).ToArray()
            Dim rank As Integer() = New Integer(n - 1) {}
            Dim tmp As Integer() = New Integer(n - 1) {}

            Dim chars As Integer() = New Integer(n - 1) {}
            For i As Integer = 0 To n - 1
                chars(i) = AscW(s(i))
            Next

            ' 第一轮：直接按字符排序
            Array.Sort(sa, Function(x, y) chars(x).CompareTo(chars(y)))
            rank(sa(0)) = 0
            For i As Integer = 1 To n - 1
                rank(sa(i)) = rank(sa(i - 1)) + If(chars(sa(i)) = chars(sa(i - 1)), 0, 1)
            Next

            Dim k As Integer = 1
            While k < n AndAlso rank(sa(n - 1)) < n - 1
                ' 按 (rank[i], rank[i+k]) 排序
                Dim offset As Integer = k
                Dim comparer As Comparison(Of Integer) = Function(x, y)
                                                              If rank(x) <> rank(y) Then Return rank(x).CompareTo(rank(y))
                                                              Dim rx As Integer = If(x + offset < n, rank(x + offset), -1)
                                                              Dim ry As Integer = If(y + offset < n, rank(y + offset), -1)
                                                              Return rx.CompareTo(ry)
                                                          End Function
                Array.Sort(sa, comparer)

                ' 重新计算 rank
                tmp(sa(0)) = 0
                For i As Integer = 1 To n - 1
                    Dim prev As Integer = sa(i - 1)
                    Dim cur As Integer = sa(i)
                    Dim prevSecond As Integer = If(prev + offset < n, rank(prev + offset), -1)
                    Dim curSecond As Integer = If(cur + offset < n, rank(cur + offset), -1)
                    tmp(cur) = tmp(prev) + If(rank(prev) = rank(cur) AndAlso prevSecond = curSecond, 0, 1)
                Next
                Array.Copy(tmp, rank, n)

                If rank(sa(n - 1)) = n - 1 Then Exit While
                k = k * 2
            End While

            Return sa
        End Function

    End Module

End Namespace
