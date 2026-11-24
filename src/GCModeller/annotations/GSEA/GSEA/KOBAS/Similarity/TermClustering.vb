Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Namespace KOBAS

    ''' <summary>
    ''' 富集结果词条的相似度聚类（KOBAS <c>src/kobas/sim.py</c> 的 VB.NET 移植）。
    ''' </summary>
    ''' <remarks>
    ''' 算法步骤与 KOBAS 一致：
    ''' <list type="number">
    ''' <item>对每个词条 i，把所有与它的相似度不低于阈值的词条（含自身）作为一个候选簇；</item>
    ''' <item>候选簇内部两两相似度检查（<see cref="TestSelf"/>），达标才保留；</item>
    ''' <item>反复合并满足 <see cref="TestTwo"/> 判据的两个簇，直到无法继续合并；</item>
    ''' <item>用簇内词条的 <c>-log10(p)</c> 乘积开 n 次方作为簇的重要性评分。</item>
    ''' </list>
    ''' 与 KOBAS 原实现的差异：簇以 <c>Integer()</c> 下标数组表示，合并时按引用从集合中
    ''' 移除，避免 KOBAS 中「按值 remove」在存在重复簇内容时误删的情况。
    ''' </remarks>
    Public Module TermClustering

        ''' <summary>
        ''' 簇内自检：簇内两两相似度不低于阈值的比例是否超过 0.5（且比较次数大于 3）。
        ''' </summary>
        ''' <param name="matrix">相似度矩阵。</param>
        ''' <param name="cluster">簇内成员下标。</param>
        ''' <param name="threshold">相似度阈值。</param>
        ''' <returns>合格返回 <c>True</c>。</returns>
        Public Function TestSelf(matrix As Double()(), cluster As Integer(), threshold As Double) As Boolean
            Dim comparisons As Integer = 0
            Dim passed As Integer = 0

            For i As Integer = 0 To cluster.Length - 1
                For j As Integer = i + 1 To cluster.Length - 1
                    comparisons += 1

                    If matrix(cluster(i))(cluster(j)) >= threshold Then
                        passed += 1
                    End If
                Next
            Next

            Return comparisons > 3 AndAlso (passed / CDbl(comparisons)) > 0.5
        End Function

        ''' <summary>
        ''' 簇合并判据：两个簇的并集大小是否小于
        ''' <c>0.5 * max(|A|, |B|) + min(|A|, |B|)</c>。
        ''' </summary>
        ''' <param name="first">第一个簇的成员下标。</param>
        ''' <param name="second">第二个簇的成员下标。</param>
        ''' <returns>满足判据时返回合并后的成员下标，否则返回 <c>Nothing</c>。</returns>
        Public Function TestTwo(first As Integer(), second As Integer()) As Integer()
            Dim union As Integer() = first.Union(second).ToArray
            Dim longer As Integer = std.Max(first.Length, second.Length)
            Dim shorter As Integer = std.Min(first.Length, second.Length)

            If union.Length < (0.5 * longer + shorter) Then
                Return union
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' 生成初始候选簇。
        ''' </summary>
        ''' <param name="matrix">相似度矩阵。</param>
        ''' <param name="threshold">相似度阈值。</param>
        ''' <returns>通过自检的候选簇集合。</returns>
        Public Function Initialize(matrix As Double()(), threshold As Double) As List(Of Integer())
            Dim all As New List(Of Integer())

            For i As Integer = 0 To matrix.Length - 1
                Dim cluster As New List(Of Integer) From {i}

                For j As Integer = 0 To matrix.Length - 1
                    If j <> i AndAlso matrix(i)(j) >= threshold Then
                        cluster.Add(j)
                    End If
                Next

                Dim members As Integer() = cluster.ToArray

                If TestSelf(matrix, members, threshold) Then
                    all.Add(members)
                End If
            Next

            Return all
        End Function

        ''' <summary>
        ''' 迭代合并满足判据的簇，直到无法继续合并。
        ''' </summary>
        ''' <param name="matrix">相似度矩阵。</param>
        ''' <param name="clusters">初始簇集合。</param>
        ''' <returns>合并之后的簇集合。</returns>
        Public Function Iterate(matrix As Double()(), clusters As IEnumerable(Of Integer())) As List(Of Integer())
            Dim all As New List(Of Integer())(clusters)
            Dim i As Integer = 0
            Dim j As Integer = 1
            Dim length As Integer = all.Count

            While i < length - 1
                Dim first As Integer() = all(i)
                Dim second As Integer() = all(j)
                Dim merged As Integer() = TestTwo(first, second)

                If merged IsNot Nothing Then
                    all.Add(merged)
                    Call all.Remove(first)
                    Call all.Remove(second)
                    length -= 1
                    j = i + 1
                ElseIf j < length - 1 Then
                    j += 1
                Else
                    i += 1
                    j = i + 1
                End If
            End While

            Return all
        End Function

        ''' <summary>
        ''' 计算簇的重要性评分：簇内各词条 <c>-log10(p)</c> 之积的 n 次方根。
        ''' </summary>
        ''' <param name="cluster">簇内成员在结果表之中的下标。</param>
        ''' <param name="pvalues">与结果表同序的 p 值。</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 当某个 p 值为 0 时，KOBAS 会得到无穷大；这里把 p 值截断到 <c>1e-300</c>
        ''' （对应 <c>-log10(p) = 300</c>），保证结果有限。
        ''' </remarks>
        <Extension>
        Public Function Score(cluster As Integer(), pvalues As Double()) As Double
            If cluster Is Nothing OrElse cluster.Length = 0 Then
                Return 0
            End If

            Dim product As Double = 1

            For Each index As Integer In cluster
                Dim p As Double = pvalues(index)

                If Double.IsNaN(p) OrElse p > 1 Then
                    p = 1
                End If
                If p < 1.0E-300 Then
                    p = 1.0E-300
                End If

                product *= -std.Log10(p)
            Next

            Return std.Pow(product, 1.0 / cluster.Length)
        End Function

        ''' <summary>
        ''' 对富集结果执行相似度聚类，并按簇评分降序返回。
        ''' </summary>
        ''' <param name="rows">富集结果行（<see cref="EnrichmentRow.Input"/> 为词条成员基因）。</param>
        ''' <param name="universeSize">基因全域大小。</param>
        ''' <param name="threshold">相似度阈值，默认 0.35。</param>
        ''' <param name="method">相似度方法，默认 <see cref="SimilarityMethod.Kappa"/>。</param>
        ''' <returns>按评分降序排列的簇（元素为 <paramref name="rows"/> 的下标）。</returns>
        <Extension>
        Public Function Clusterize(rows As IList(Of EnrichmentRow),
                                   universeSize As Integer,
                                   Optional threshold As Double = 0.35,
                                   Optional method As SimilarityMethod = SimilarityMethod.Kappa) As List(Of TermCluster)

            Dim matrix As Double()() = TermSimilarity.Matrix(rows.Select(Function(row) row.Input).ToList, universeSize, method)
            Dim clusters As List(Of Integer()) = Iterate(matrix, Initialize(matrix, threshold))
            Dim pvalues As Double() = rows.Select(Function(row) row.PValue).ToArray
            Dim result As New List(Of TermCluster)

            For Each members As Integer() In clusters
                result.Add(New TermCluster With {
                    .Members = members,
                    .Score = members.Score(pvalues)
                })
            Next

            Return result _
                .OrderByDescending(Function(cluster) cluster.Score) _
                .ToList
        End Function
    End Module

    ''' <summary>
    ''' 一个富集结果词条簇。
    ''' </summary>
    Public Class TermCluster

        ''' <summary>
        ''' 簇内成员在结果表之中的下标。
        ''' </summary>
        ''' <returns></returns>
        Public Property Members As Integer()

        ''' <summary>
        ''' 簇的重要性评分。
        ''' </summary>
        ''' <returns></returns>
        Public Property Score As Double

        Public Overrides Function ToString() As String
            Return $"{{{Members.JoinBy(", ")}}} score={Score.ToString("F4")}"
        End Function
    End Class
End Namespace
