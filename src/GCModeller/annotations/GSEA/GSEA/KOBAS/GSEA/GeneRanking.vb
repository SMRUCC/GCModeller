Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace KOBAS

    ''' <summary>
    ''' 一个在表型比较分析之中被排序的基因。
    ''' </summary>
    Public Class RankedGene

        ''' <summary>
        ''' 基因编号（表达矩阵的行名）。
        ''' </summary>
        ''' <returns></returns>
        Public Property GeneID As String

        ''' <summary>
        ''' 排序统计量的值（s2n 或者 t 统计量）。
        ''' </summary>
        ''' <returns></returns>
        Public Property Score As Double

        ''' <summary>
        ''' 该基因在表达矩阵之中的原始行下标。
        ''' </summary>
        ''' <returns></returns>
        Public Property Index As Integer

        Public Overrides Function ToString() As String
            Return $"{GeneID}, {Score.ToString("G6")}"
        End Function
    End Class

    ''' <summary>
    ''' 基因排序（KOBAS <c>gsea.rank_pro</c> 的 VB.NET 移植）。
    ''' </summary>
    ''' <remarks>
    ''' 与 KOBAS 原实现的差异：
    ''' <list type="bullet">
    ''' <item>numpy 的 <c>std</c> 使用总体标准差（ddof=0），这里显式使用总体标准差，
    ''' 以保证与 KOBAS 的排序分数逐值一致。</item>
    ''' <item>numpy 的 <c>argsort(...)[::-1]</c> 在遇到并列值时顺序不稳定，
    ''' 这里改为稳定降序排序（并列时保持基因在表达矩阵之中的原始顺序），使结果可复现。</item>
    ''' <item>当分母为 0 时 numpy 会产生 <c>inf/nan</c>，这里统一记为 0 分。</item>
    ''' </list>
    ''' </remarks>
    Public Module GeneRanking

        ''' <summary>
        ''' 按照表型分组对表达矩阵的基因进行排序。
        ''' </summary>
        ''' <param name="expr">行是基因、列是样本的表达矩阵。</param>
        ''' <param name="pheno">表型分组信息（<c>0</c> 为 A 组，其余为 B 组）。</param>
        ''' <param name="method">排序方法。</param>
        ''' <returns>按排序统计量降序排列的基因列表。</returns>
        ''' <exception cref="InvalidOperationException">
        ''' 表型标签数量与样本数不一致，或者某一组的样本数为 0 时抛出。
        ''' </exception>
        Public Function GeneRank(expr As Matrix, pheno As phenotype, Optional method As GeneRankingMethod = GeneRankingMethod.Snr) As RankedGene()
            Dim labels As Integer() = pheno.labels

            If labels Is Nothing OrElse labels.Length <> expr.sampleID.Length Then
                Throw New InvalidOperationException(
                    "The phenotype labels does not match the sample number of the expression matrix." &
                    $"{vbCrLf}labels: {If(labels Is Nothing, 0, labels.Length)}, samples: {expr.sampleID.Length}")
            End If

            Dim genes As String() = expr.expression.Select(Function(row) row.geneID).ToArray
            Dim values As Double()() = expr.expression.Select(Function(row) row.experiments).ToArray
            Dim statistics As Double() = Scores(values, labels, method)

            Return Enumerable.Range(0, genes.Length) _
                .OrderByDescending(Function(i) statistics(i)) _
                .Select(Function(i)
                            Return New RankedGene With {
                                .GeneID = genes(i),
                                .Score = statistics(i),
                                .Index = i
                            }
                        End Function) _
                .ToArray
        End Function

        ''' <summary>
        ''' 计算每个基因的排序统计量（尚未排序）。
        ''' </summary>
        ''' <param name="values">按行存放的表达值（行=基因，列=样本）。</param>
        ''' <param name="labels">每个样本所属的表型分组（<c>0</c> 为 A 组，其余为 B 组）。</param>
        ''' <param name="method">排序方法。</param>
        ''' <returns>与行数等长的排序统计量数组。</returns>
        Public Function Scores(values As Double()(), labels As Integer(), Optional method As GeneRankingMethod = GeneRankingMethod.Snr) As Double()
            Dim group0 As Integer() = Enumerable.Range(0, labels.Length).Where(Function(i) labels(i) = 0).ToArray
            Dim group1 As Integer() = Enumerable.Range(0, labels.Length).Where(Function(i) labels(i) <> 0).ToArray

            If group0.Length = 0 OrElse group1.Length = 0 Then
                Throw New InvalidOperationException(
                    "Both phenotype groups must contain at least one sample for the gene ranking.")
            End If

            Dim result As Double() = New Double(values.Length - 1) {}

            For i As Integer = 0 To values.Length - 1
                Dim row As Double() = values(i)
                Dim mean0 As Double = Mean(row, group0)
                Dim mean1 As Double = Mean(row, group1)

                Select Case method
                    Case GeneRankingMethod.TTest
                        Dim sd0 As Double = StandardDeviation(row, group0, mean0)
                        Dim sd1 As Double = StandardDeviation(row, group1, mean1)
                        Dim denominator As Double = std.Sqrt(sd0 * sd0 / group0.Length + sd1 * sd1 / group1.Length)

                        result(i) = If(denominator = 0, 0, (mean0 - mean1) / denominator)
                    Case Else
                        Dim sd0 As Double = StandardDeviation(row, group0, mean0)
                        Dim sd1 As Double = StandardDeviation(row, group1, mean1)
                        Dim denominator As Double = sd0 + sd1

                        result(i) = If(denominator = 0, 0, (mean0 - mean1) / denominator)
                End Select
            Next

            Return result
        End Function

        ''' <summary>
        ''' 计算指定下标子集的均值。
        ''' </summary>
        Private Function Mean(values As Double(), [subset] As Integer()) As Double
            Dim sum As Double = 0

            For Each i As Integer In [subset]
                sum += values(i)
            Next

            Return sum / [subset].Length
        End Function

        ''' <summary>
        ''' 计算指定下标子集的总体标准差（ddof=0）。
        ''' </summary>
        Private Function StandardDeviation(values As Double(), [subset] As Integer(), mean As Double) As Double
            Dim sum As Double = 0

            For Each i As Integer In [subset]
                Dim d As Double = values(i) - mean

                sum += d * d
            Next

            Return std.Sqrt(sum / [subset].Length)
        End Function
    End Module
End Namespace
