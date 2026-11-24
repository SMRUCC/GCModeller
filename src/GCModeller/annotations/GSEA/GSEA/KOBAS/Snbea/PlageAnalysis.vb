Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports Hyp = Microsoft.VisualBasic.Math.Statistics.Hypothesis
Imports std = System.Math

Namespace KOBAS

    ''' <summary>
    ''' PLAGE 通路水平表达分析（KOBAS <c>snbea.run_plage</c> 的 VB.NET 移植）。
    ''' </summary>
    ''' <remarks>
    ''' KOBAS 的实现链路是 <c>GSVA::gsva(method = "plage")</c> → <c>limma::lmFit</c> →
    ''' <c>limma::eBayes</c> → <c>limma::topTable</c>。这里：
    ''' <list type="bullet">
    ''' <item>PLAGE 打分与 <c>GSVA::plage</c> 一致：对基因集子矩阵逐行 z-score 后取
    ''' 第一右奇异向量（宽矩阵走转置取第一左奇异向量）；</item>
    ''' <item>差异分析用经典的两样本 t 检验代替 limma 的经验贝叶斯调节
    ''' （KOBAS 侧没有现成的 moderated t 实现），因此 p 值会比 limma 略保守。</item>
    ''' </list>
    ''' </remarks>
    Public Module PlageAnalysis

        ''' <summary>
        ''' 执行 PLAGE 打分与两组比较。
        ''' </summary>
        ''' <param name="expr">表达矩阵（应当已经过 <see cref="SnbeaData.Prepare"/> 预变换）。</param>
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

            Dim scores As Double()() = PlageScores(expr, mapping)
            Dim groups = pheno.Groups()
            Dim rows As New List(Of SnbeaResult)
            Dim position As Integer = 0

            For Each item As KeyValuePair(Of String, Integer()) In mapping
                Dim id As String = item.Key
                Dim values As Double() = scores(position)
                Dim pvalue As Double = 1
                Dim statistic As Double = 0

                position += 1

                If groups.Group0.Length > 1 AndAlso groups.Group1.Length > 1 Then
                    Dim test As (T As Double, DF As Double, PValue As Double) =
                        GroupStatistics.StudentT(values, groups.Group0, groups.Group1)

                    statistic = test.T
                    pvalue = test.PValue
                End If

                Dim cluster As Cluster = background.clusters _
                    .Where(Function(c) c.ID = id) _
                    .FirstOrDefault

                rows.Add(New SnbeaResult With {
                    .ID = id,
                    .Names = If(cluster Is Nothing, id, cluster.names),
                    .Description = If(cluster Is Nothing, "", cluster.description),
                    .SetSize = item.Value.Length,
                    .Statistic = statistic,
                    .PValue = pvalue,
                    .AdjustedPValue = Double.NaN,
                    .Genes = item.Value.Select(Function(i) expr.expression(i).geneID).ToArray
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
        ''' 计算 PLAGE 活性分数矩阵。
        ''' </summary>
        ''' <param name="expr">表达矩阵。</param>
        ''' <param name="geneSets">基因集编号 → 表达矩阵行下标。</param>
        ''' <returns>按 <paramref name="geneSets"/> 枚举顺序排列的「样本活性分数」数组。</returns>
        <Extension>
        Public Function PlageScores(expr As Matrix, geneSets As Dictionary(Of String, Integer())) As Double()()
            Dim samples As Integer = expr.sampleID.Length
            Dim zscores As Double()() = RowZScore(expr)
            Dim result As New List(Of Double())

            For Each item As KeyValuePair(Of String, Integer()) In geneSets
                Dim indices As Integer() = item.Value
                Dim submatrix As Double()() = indices _
                    .Select(Function(gene) zscores(gene)) _
                    .ToArray
                Dim score As Double() = New Double(samples - 1) {}

                If indices.Length >= samples Then
                    Dim svd As New SingularValueDecomposition(New NumericMatrix(submatrix))
                    Dim v As Double()() = svd.V.ArrayPack(deepcopy:=False)

                    For j As Integer = 0 To samples - 1
                        score(j) = v(j)(0)
                    Next
                Else
                    ' 宽矩阵：A 的右奇异向量等于 A' 的左奇异向量
                    Dim transposed As Double()() = New Double(samples - 1)() {}

                    For j As Integer = 0 To samples - 1
                        transposed(j) = New Double(indices.Length - 1) {}

                        For r As Integer = 0 To indices.Length - 1
                            transposed(j)(r) = submatrix(r)(j)
                        Next
                    Next

                    Dim svd As New SingularValueDecomposition(New NumericMatrix(transposed))
                    Dim u As Double()() = svd.U.ArrayPack(deepcopy:=False)

                    For j As Integer = 0 To samples - 1
                        score(j) = u(j)(0)
                    Next
                End If

                result.Add(score)
            Next

            Return result.ToArray
        End Function

        ''' <summary>
        ''' 逐行 z-score 标准化（每个基因在自己的样本维度上零均值、单位标准差，ddof=1）。
        ''' </summary>
        ''' <param name="expr">表达矩阵。</param>
        ''' <returns>与矩阵同形的标准化数值。</returns>
        Private Function RowZScore(expr As Matrix) As Double()()
            Dim samples As Integer = expr.sampleID.Length
            Dim result As Double()() = New Double(expr.size - 1)() {}

            For g As Integer = 0 To expr.size - 1
                Dim values As Double() = expr.expression(g).experiments
                Dim mean As Double = values.Average
                Dim sd As Double = 0

                For Each value As Double In values
                    sd += (value - mean) * (value - mean)
                Next

                sd = If(samples > 1, std.Sqrt(sd / (samples - 1)), 0)

                Dim z As Double() = New Double(samples - 1) {}

                For j As Integer = 0 To samples - 1
                    z(j) = If(sd = 0, 0, (values(j) - mean) / sd)
                Next

                result(g) = z
            Next

            Return result
        End Function
    End Module
End Namespace
