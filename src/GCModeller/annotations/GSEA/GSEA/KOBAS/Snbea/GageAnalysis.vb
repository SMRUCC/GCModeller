Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Statistics.Distributions
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace KOBAS

    ''' <summary>
    ''' GAGE 风格的基因集富集分析（KOBAS <c>snbea.run_gage</c> 的 VB.NET 移植）。
    ''' </summary>
    ''' <remarks>
    ''' KOBAS 直接调用 R 包 <c>gage::gage(same_dir = FALSE)</c>，其中 <c>greater</c>
    ''' 结果表的上调检验统计量为
    ''' <c>z = Σ t_g / √n</c>（<c>t_g</c> 为基因水平的 moderated t 统计量），
    ''' 并以上尾正态概率作为 p 值。
    ''' 
    ''' 本实现的差异：
    ''' <list type="bullet">
    ''' <item>基因水平统计量使用经典两样本 t 检验（KOBAS 用的是 limma 的 moderated t）；</item>
    ''' <item>基因集水平的 z 值按 <c>Σ t_g / √n</c> 聚合，p 值取标准正态上尾；</item>
    ''' <item><c>same_dir = FALSE</c> 时 gage 会分别报告上下调，这里只给出上调（greater）
    ''' 方向的结果，可以通过对表达矩阵取负号来分析下调方向。</item>
    ''' </list>
    ''' </remarks>
    Public Module GageAnalysis

        ''' <summary>
        ''' 执行 GAGE 风格的富集分析。
        ''' </summary>
        ''' <param name="expr">表达矩阵（建议先经过 <see cref="SnbeaData.Prepare"/> 预变换）。</param>
        ''' <param name="background">基因集背景模型。</param>
        ''' <param name="pheno">表型分组。</param>
        ''' <param name="minSize">基因集可用成员数下限（闭区间），默认 5。</param>
        ''' <param name="maxSize">基因集可用成员数上限（闭区间），默认 500。</param>
        ''' <param name="fdrMethod">多重校正方法，默认 BH。</param>
        ''' <returns>按 p 值升序排列的结果。</returns>
        Public Function Compute(expr As Matrix,
                                background As Background,
                                pheno As phenotype,
                                Optional minSize As Integer = 5,
                                Optional maxSize As Integer = 500,
                                Optional fdrMethod As FdrMethod = FdrMethod.BH) As SnbeaResult()

            Call ExpressionTable.Validate(expr, pheno)

            Dim mapping As Dictionary(Of String, Integer()) = expr.MapGeneSets(background, minSize, maxSize)

            If mapping.Count = 0 Then
                Throw New InvalidOperationException(
                    $"None of the gene set has {minSize}..{maxSize} members mapped on the expression matrix.")
            End If

            Dim groups = pheno.Groups()
            Dim statistics As Double() = GeneStatistics(expr, groups.Group0, groups.Group1)
            Dim rows As New List(Of SnbeaResult)

            For Each item As KeyValuePair(Of String, Integer()) In mapping
                Dim indices As Integer() = item.Value
                Dim sum As Double = 0

                For Each gene As Integer In indices
                    sum += statistics(gene)
                Next

                Dim z As Double = sum / std.Sqrt(indices.Length)
                Dim pvalue As Double = std.Max(0.0, std.Min(1.0, 1 - Distribution.ZNormal(z)))
                Dim cluster As Cluster = background.clusters _
                    .Where(Function(c) c.ID = item.Key) _
                    .FirstOrDefault

                rows.Add(New SnbeaResult With {
                    .ID = item.Key,
                    .Names = If(cluster Is Nothing, item.Key, cluster.names),
                    .Description = If(cluster Is Nothing, "", cluster.description),
                    .SetSize = indices.Length,
                    .Statistic = z,
                    .PValue = pvalue,
                    .AdjustedPValue = Double.NaN,
                    .Genes = indices.Select(Function(i) expr.expression(i).geneID).ToArray
                })
            Next

            Dim adjusted As Double() = FdrAdjust.Adjust(rows.Select(Function(row) row.PValue), fdrMethod)

            For i As Integer = 0 To rows.Count - 1
                rows(i).AdjustedPValue = adjusted(i)
            Next

            Return rows _
                .OrderBy(Function(row) row.PValue) _
                .ToArray
        End Function

        ''' <summary>
        ''' 计算每个基因的两样本 t 统计量（第二组相对第一组）。
        ''' </summary>
        ''' <param name="expr">表达矩阵。</param>
        ''' <param name="group0">第一组（对照）的样本下标。</param>
        ''' <param name="group1">第二组（处理）的样本下标。</param>
        ''' <returns>与矩阵行数等长的 t 统计量。</returns>
        Private Function GeneStatistics(expr As Matrix, group0 As Integer(), group1 As Integer()) As Double()
            Dim result As Double() = New Double(expr.size - 1) {}

            For g As Integer = 0 To expr.size - 1
                Dim values As Double() = expr.expression(g).experiments
                Dim mean0 As Double = Mean(values, group0)
                Dim mean1 As Double = Mean(values, group1)
                Dim variance0 As Double = Variance(values, group0, mean0)
                Dim variance1 As Double = Variance(values, group1, mean1)
                Dim denominator As Double = std.Sqrt(variance0 / group0.Length + variance1 / group1.Length)

                result(g) = If(denominator = 0, 0, (mean1 - mean0) / denominator)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 计算子集均值。
        ''' </summary>
        Private Function Mean(values As Double(), [subset] As Integer()) As Double
            Dim sum As Double = 0

            For Each i As Integer In [subset]
                sum += values(i)
            Next

            Return If([subset].Length = 0, 0, sum / [subset].Length)
        End Function

        ''' <summary>
        ''' 计算子集的无偏方差（ddof=1），用于 t 检验。
        ''' </summary>
        Private Function Variance(values As Double(), [subset] As Integer(), mean As Double) As Double
            If [subset].Length < 2 Then
                Return 0
            End If

            Dim sum As Double = 0

            For Each i As Integer In [subset]
                Dim d As Double = values(i) - mean

                sum += d * d
            Next

            Return sum / ([subset].Length - 1)
        End Function
    End Module
End Namespace
