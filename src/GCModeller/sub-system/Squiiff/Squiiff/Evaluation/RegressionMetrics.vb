Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace Evaluation

    ''' <summary>
    ''' 回归 / 相关评估指标（口径与同目录 SpikingLoop 的 <c>RegressionMetrics</c> 保持一致：
    ''' 方差为 0 或有效样本不足时返回 <see cref="Double.NaN"/> 而不是抛异常）。
    '''
    ''' 除全局指标外，还给出**逐细胞（行）**与**逐基因（列）**的相关系数：
    ''' 前者衡量"每个细胞被还原得有多准"，后者衡量"每个基因的跨细胞变化趋势是否被捕捉"，
    ''' 二者对单细胞扰动预测都很重要。
    ''' </summary>
    Public Class RegressionMetrics

        ''' <summary>参与统计的元素总数。</summary>
        Public Property N As Integer

        ''' <summary>全局 Pearson 相关系数。</summary>
        Public Property Pcc As Double = Double.NaN

        ''' <summary>全局决定系数 <c>1 − SSres/SStot</c>。</summary>
        Public Property R2 As Double = Double.NaN

        ''' <summary>均方根误差。</summary>
        Public Property Rmse As Double = Double.NaN

        ''' <summary>平均绝对误差。</summary>
        Public Property Mae As Double = Double.NaN

        ''' <summary>逐细胞（第 0 维）的 Pearson 相关系数。</summary>
        Public Property CellPcc As Double()

        ''' <summary>逐基因（第 1 维）的 Pearson 相关系数。</summary>
        Public Property GenePcc As Double()

        ''' <summary>逐基因（第 1 维）的决定系数。</summary>
        Public Property GeneR2 As Double()

        ''' <summary>逐细胞 PCC 的均值（忽略 NaN）。</summary>
        Public Function MeanCellPcc() As Double
            Return MeanOf(CellPcc)
        End Function

        ''' <summary>逐基因 PCC 的均值（忽略 NaN）。</summary>
        Public Function MeanGenePcc() As Double
            Return MeanOf(GenePcc)
        End Function

        ''' <summary>逐基因 R² 的均值（忽略 NaN）。</summary>
        Public Function MeanGeneR2() As Double
            Return MeanOf(GeneR2)
        End Function

        ''' <summary>逐基因 PCC 达到阈值的基因数。</summary>
        Public Function GenesAbove(threshold As Double) As Integer
            If GenePcc Is Nothing Then Return 0

            Dim count As Integer = 0
            For Each value In GenePcc
                If Not Double.IsNaN(value) AndAlso value >= threshold Then count += 1
            Next
            Return count
        End Function

        ''' <summary>
        ''' 计算预测与真实之间的全部指标。两者形状必须一致；
        ''' 当形状为 <c>[细胞, 基因]</c> 且两侧维度都 >= 2 时给出逐行 / 逐列指标。
        ''' </summary>
        Public Shared Function Compute(predicted As Tensor, actual As Tensor) As RegressionMetrics
            If Not predicted.Shape.SequenceEqual(actual.Shape) Then
                Throw New ArgumentException($"预测与真实的形状必须一致: [{String.Join(",", predicted.Shape)}] vs [{String.Join(",", actual.Shape)}]")
            End If

            Dim result As New RegressionMetrics With {.N = predicted.Length}
            Dim p = predicted.Data
            Dim a = actual.Data

            result.Pcc = Pearson(p, a)
            result.Rmse = RootMeanSquareError(p, a)
            result.Mae = MeanAbsoluteError(p, a)
            result.R2 = R2Value(p, a)

            If predicted.Rank = 2 AndAlso predicted.Shape(1) >= 2 Then
                ' 逐细胞（行）
                Dim rows = predicted.Shape(0)
                Dim columns = predicted.Shape(1)
                Dim cellPcc(rows - 1) As Double
                For i As Integer = 0 To rows - 1
                    Dim pr(columns - 1) As Double
                    Dim ar(columns - 1) As Double
                    Dim offset = i * columns

                    For j As Integer = 0 To columns - 1
                        pr(j) = p(offset + j)
                        ar(j) = a(offset + j)
                    Next

                    cellPcc(i) = Pearson(pr, ar)
                Next
                result.CellPcc = cellPcc

                ' 逐基因（列）：仅当存在多个细胞时才有意义
                If rows >= 2 Then
                    Dim genePcc(columns - 1) As Double
                    Dim geneR2(columns - 1) As Double
                    Dim pv(rows - 1) As Double
                    Dim av(rows - 1) As Double

                    For j As Integer = 0 To columns - 1
                        For i As Integer = 0 To rows - 1
                            pv(i) = p(i * columns + j)
                            av(i) = a(i * columns + j)
                        Next

                        genePcc(j) = Pearson(pv, av)
                        geneR2(j) = R2Value(pv, av)
                    Next

                    result.GenePcc = genePcc
                    result.GeneR2 = geneR2
                End If
            End If

            Return result
        End Function

        ''' <summary>Pearson 相关系数（长度不等、样本 &lt; 2 或任一序列方差为 0 时返回 NaN）。</summary>
        Public Shared Function Pearson(a As Double(), b As Double()) As Double
            If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length OrElse a.Length < 2 Then
                Return Double.NaN
            End If

            Dim n As Double = a.Length
            Dim meanA As Double = 0.0
            Dim meanB As Double = 0.0
            For i As Integer = 0 To a.Length - 1
                meanA += a(i)
                meanB += b(i)
            Next
            meanA /= n
            meanB /= n

            Dim covariance As Double = 0.0
            Dim varianceA As Double = 0.0
            Dim varianceB As Double = 0.0
            For i As Integer = 0 To a.Length - 1
                Dim da = a(i) - meanA
                Dim db = b(i) - meanB
                covariance += da * db
                varianceA += da * da
                varianceB += db * db
            Next

            If varianceA <= 1.0E-300 OrElse varianceB <= 1.0E-300 Then Return Double.NaN

            Return covariance / std.Sqrt(varianceA * varianceB)
        End Function

        ''' <summary>Spearman 秩相关（忽略 NaN 对，有效对数 &lt; 3 返回 NaN）。</summary>
        Public Shared Function Spearman(a As Double(), b As Double()) As Double
            If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length OrElse a.Length < 3 Then
                Return Double.NaN
            End If

            Dim valid As New List(Of Integer)
            For i As Integer = 0 To a.Length - 1
                If Not Double.IsNaN(a(i)) AndAlso Not Double.IsNaN(b(i)) Then valid.Add(i)
            Next

            If valid.Count < 3 Then Return Double.NaN

            Dim pa(valid.Count - 1) As Double
            Dim pb(valid.Count - 1) As Double
            For i As Integer = 0 To valid.Count - 1
                pa(i) = a(valid(i))
                pb(i) = b(valid(i))
            Next

            Return Pearson(Rank(pa), Rank(pb))
        End Function

        ''' <summary>均方根误差。</summary>
        Public Shared Function RootMeanSquareError(a As Double(), b As Double()) As Double
            If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length OrElse a.Length = 0 Then Return Double.NaN

            Dim sum As Double = 0.0
            For i As Integer = 0 To a.Length - 1
                Dim d = a(i) - b(i)
                sum += d * d
            Next

            Return std.Sqrt(sum / a.Length)
        End Function

        ''' <summary>平均绝对误差。</summary>
        Public Shared Function MeanAbsoluteError(a As Double(), b As Double()) As Double
            If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length OrElse a.Length = 0 Then Return Double.NaN

            Dim sum As Double = 0.0
            For i As Integer = 0 To a.Length - 1
                sum += std.Abs(a(i) - b(i))
            Next

            Return sum / a.Length
        End Function

        ''' <summary>决定系数 <c>1 − SSres/SStot</c>（以 <paramref name="actual"/> 为基准）。</summary>
        Public Shared Function R2Value(predicted As Double(), actual As Double()) As Double
            If predicted Is Nothing OrElse actual Is Nothing OrElse predicted.Length <> actual.Length OrElse actual.Length = 0 Then
                Return Double.NaN
            End If

            Dim mean As Double = 0.0
            For i As Integer = 0 To actual.Length - 1
                mean += actual(i)
            Next
            mean /= actual.Length

            Dim ssRes As Double = 0.0
            Dim ssTot As Double = 0.0
            For i As Integer = 0 To actual.Length - 1
                Dim d = actual(i) - predicted(i)
                ssRes += d * d

                Dim t = actual(i) - mean
                ssTot += t * t
            Next

            If ssTot <= 1.0E-300 Then Return Double.NaN
            Return 1.0 - ssRes / ssTot
        End Function

        ''' <summary>忽略 NaN 的均值（全为 NaN 时返回 NaN）。</summary>
        Public Shared Function MeanOf(values As Double()) As Double
            If values Is Nothing Then Return Double.NaN

            Dim sum As Double = 0.0
            Dim count As Integer = 0
            For Each value In values
                If Not Double.IsNaN(value) Then
                    sum += value
                    count += 1
                End If
            Next

            If count = 0 Then Return Double.NaN
            Return sum / count
        End Function

        ''' <summary>平均秩（并列取平均）。</summary>
        Private Shared Function Rank(values As Double()) As Double()
            Dim order = Enumerable.Range(0, values.Length).ToArray()
            Array.Sort(order, Function(x, y) values(x).CompareTo(values(y)))

            Dim ranks(values.Length - 1) As Double
            Dim i As Integer = 0
            While i < order.Length
                Dim j = i
                While j + 1 < order.Length AndAlso std.Abs(values(order(j + 1)) - values(order(i))) < 1.0E-12
                    j += 1
                End While

                Dim average = (i + j) / 2.0 + 1.0
                For k As Integer = i To j
                    ranks(order(k)) = average
                Next

                i = j + 1
            End While

            Return ranks
        End Function

        Public Overrides Function ToString() As String
            Return $"PCC={Pcc:F4} R²={R2:F4} RMSE={Rmse:F4} MAE={Mae:F4} 逐细胞PCC={MeanCellPcc():F4} 逐基因PCC={MeanGenePcc():F4}"
        End Function
    End Class
End Namespace
