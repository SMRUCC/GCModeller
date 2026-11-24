Imports System.Runtime.CompilerServices
Imports std = System.Math

Namespace KOBAS

    ''' <summary>
    ''' 词条相似度衡量方法。
    ''' </summary>
    Public Enum SimilarityMethod
        ''' <summary>Cohen's kappa 系数（KOBAS <c>sim.kappa</c>）。</summary>
        Kappa
        ''' <summary>Jaccard 相似系数（KOBAS <c>sim.Jaccard</c>）。</summary>
        Jaccard
    End Enum

    ''' <summary>
    ''' 两个词条的 2x2 列联表计数。
    ''' </summary>
    Public Structure TermContingency

        ''' <summary>同时属于两个词条的基因数量。</summary>
        Public Both As Integer
        ''' <summary>只属于第一个词条的基因数量。</summary>
        Public OnlyFirst As Integer
        ''' <summary>只属于第二个词条的基因数量。</summary>
        Public OnlySecond As Integer
        ''' <summary>两个词条都不包含的基因数量。</summary>
        Public Neither As Integer
    End Structure

    ''' <summary>
    ''' 富集结果词条之间的相似度计算（KOBAS <c>src/kobas/sim.py</c> 的 VB.NET 移植）。
    ''' </summary>
    ''' <remarks>
    ''' KOBAS 的 <c>makeMatrix</c> 在 <c>termA</c> 与 <c>termB</c> 都是「基因列表」而不是
    ''' 0/1 向量，因此这里同样按集合的交集/并集计数构造四格表，与
    ''' <c>matrix[0] = |A ∩ B|</c>、<c>matrix[1] = |A| - |A ∩ B|</c>、
    ''' <c>matrix[2] = |B| - |A ∩ B|</c>、<c>matrix[3] = num - 前三者</c> 完全对应。
    ''' </remarks>
    Public Module TermSimilarity

        ''' <summary>
        ''' 构造两个词条的 2x2 列联表计数。
        ''' </summary>
        ''' <param name="first">第一个词条的基因集合。</param>
        ''' <param name="second">第二个词条的基因集合。</param>
        ''' <param name="universeSize">基因全域大小（四格之和）。</param>
        ''' <returns></returns>
        Public Function Contingency(first As IEnumerable(Of String),
                                    second As IEnumerable(Of String),
                                    universeSize As Integer) As TermContingency
            Dim setA As New HashSet(Of String)(first, StringComparer.Ordinal)
            Dim setB As New HashSet(Of String)(second, StringComparer.Ordinal)
            Dim both As Integer = setA.Where(Function(id) setB.Contains(id)).Count()

            Return New TermContingency With {
                .Both = both,
                .OnlyFirst = setA.Count - both,
                .OnlySecond = setB.Count - both,
                .Neither = universeSize - (setA.Count - both) - (setB.Count - both) - both
            }
        End Function

        ''' <summary>
        ''' 计算 Cohen's kappa 系数。
        ''' </summary>
        ''' <param name="first">第一个词条的基因集合。</param>
        ''' <param name="second">第二个词条的基因集合。</param>
        ''' <param name="universeSize">基因全域大小。</param>
        ''' <returns>取值在 <c>[-1, 1]</c> 之间的相似度。</returns>
        Public Function Kappa(first As IEnumerable(Of String),
                              second As IEnumerable(Of String),
                              universeSize As Integer) As Double
            Dim table As TermContingency = Contingency(first, second, universeSize)
            Dim total As Double = table.Both + table.OnlyFirst + table.OnlySecond + table.Neither

            If total = 0 Then
                Return 0
            End If

            Dim observed As Double = (table.Both + table.Neither) / total
            Dim expected As Double =
                ((table.Both + table.OnlyFirst) * (table.Both + table.OnlySecond) +
                 (table.OnlySecond + table.Neither) * (table.OnlyFirst + table.Neither)) / (total * total)

            If expected = 1 Then
                Return If(observed = 1, 1.0, 0.0)
            End If

            Return (observed - expected) / (1 - expected)
        End Function

        ''' <summary>
        ''' 计算 Jaccard 相似系数 <c>|A ∩ B| / |A ∪ B|</c>。
        ''' </summary>
        ''' <param name="first">第一个词条的基因集合。</param>
        ''' <param name="second">第二个词条的基因集合。</param>
        ''' <returns>取值在 <c>[0, 1]</c> 之间的相似度；两个集合都为空时返回 1。</returns>
        Public Function Jaccard(first As IEnumerable(Of String), second As IEnumerable(Of String)) As Double
            Dim setA As New HashSet(Of String)(first, StringComparer.Ordinal)
            Dim setB As New HashSet(Of String)(second, StringComparer.Ordinal)
            Dim both As Integer = setA.Where(Function(id) setB.Contains(id)).Count()
            Dim union As Integer = setA.Count + setB.Count - both

            If union = 0 Then
                Return 1
            End If

            Return both / CDbl(union)
        End Function

        ''' <summary>
        ''' 计算词条两两之间的相似度矩阵（KOBAS <c>sim.listMatrix</c> 的等价实现）。
        ''' </summary>
        ''' <param name="terms">词条的基因集合列表。</param>
        ''' <param name="universeSize">基因全域大小（仅 <see cref="SimilarityMethod.Kappa"/> 使用）。</param>
        ''' <param name="method">相似度方法。</param>
        ''' <returns>对称矩阵，对角线为 1。</returns>
        Public Function Matrix(terms As IList(Of String()),
                               universeSize As Integer,
                               Optional method As SimilarityMethod = SimilarityMethod.Kappa) As Double()()
            Dim size As Integer = terms.Count
            Dim result As Double()() = New Double(size - 1)() {}

            For i As Integer = 0 To size - 1
                result(i) = New Double(size - 1) {}
                result(i)(i) = 1
            Next

            For i As Integer = 0 To size - 1
                For j As Integer = i + 1 To size - 1
                    Dim similarity As Double

                    If method = SimilarityMethod.Jaccard Then
                        similarity = Jaccard(terms(i), terms(j))
                    Else
                        similarity = Kappa(terms(i), terms(j), universeSize)
                    End If

                    result(i)(j) = similarity
                    result(j)(i) = similarity
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 从富集结果表构造相似度矩阵。
        ''' </summary>
        ''' <param name="rows">富集结果行。</param>
        ''' <param name="universeSize">基因全域大小。</param>
        ''' <param name="method">相似度方法。</param>
        ''' <returns>与 <paramref name="rows"/> 同序的对称相似度矩阵。</returns>
        <Extension>
        Public Function SimilarityMatrix(rows As IEnumerable(Of EnrichmentRow),
                                         universeSize As Integer,
                                         Optional method As SimilarityMethod = SimilarityMethod.Kappa) As Double()()
            Dim terms As IList(Of String()) = rows.Select(Function(row) row.Input).ToList

            Return Matrix(terms, universeSize, method)
        End Function
    End Module
End Namespace
