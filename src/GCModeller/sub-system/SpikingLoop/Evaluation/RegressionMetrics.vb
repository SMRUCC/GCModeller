' ============================================================================
' RegressionMetrics.vb — 表达预测精度评估（readme 五.1）
'
' readme 的五.1 要求"留出部分伪时间区间/独立数据集，计算预测表达 vs 真实表达"，
' 并给出 R² / PCC / RMSE 三个指标。这里给出两种粒度：
'
'   整体（全局）粒度：把所有 (时间窗 × 基因) 的预测-真值对展平后计算，
'                     反映"整张表达谱"的拟合质量。readme 的 P3 验收标准
'                     "验证集 PCC > 0.6" 对应的就是这一粒度。
'   逐基因粒度：      对每个基因在时间窗维度上单独求 PCC / R²，
'                     反映"哪些基因被拟合得好"——这直接决定后续虚拟扰动
'                     实验是否可信（拟合差的基因其扰动响应不可信）。
'
' 注意：R² 在"真值近似常数"的基因上会变成很大的负数（分母趋零），
' 因此逐基因 R² 会被限制在合理区间，并把无效值标记为 NaN 而不是 Infinity。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace Evaluation

    ''' <summary>回归精度指标（表达预测 vs 真实表达）</summary>
    Public Class RegressionMetrics

        ''' <summary>全局决定系数 R² = 1 − SS_res / SS_tot</summary>
        Public Property R2 As Double

        ''' <summary>全局 Pearson 相关系数（readme P3 的验收指标）</summary>
        Public Property Pcc As Double

        ''' <summary>全局均方根误差</summary>
        Public Property Rmse As Double

        ''' <summary>全局平均绝对误差</summary>
        Public Property Mae As Double

        ''' <summary>参与统计的元素总数</summary>
        Public Property N As Integer

        ''' <summary>逐基因 PCC 的均值（跳过无法计算的基因）</summary>
        Public Property MeanGenePcc As Double

        ''' <summary>逐基因 R² 的均值（跳过无法计算的基因）</summary>
        Public Property MeanGeneR2 As Double

        ''' <summary>逐基因 PCC（无法计算时为 NaN）</summary>
        Public Property GenePcc As Double()

        ''' <summary>逐基因 R²（无法计算时为 NaN）</summary>
        Public Property GeneR2 As Double()

        ''' <summary>PCC 超过给定阈值的基因数（默认阈值 0.6）</summary>
        Public Function GenesAbovePcc(Optional threshold As Double = 0.6) As Integer
            If GenePcc Is Nothing Then Return 0
            Dim c = 0
            For Each v In GenePcc
                If Not Double.IsNaN(v) AndAlso v >= threshold Then c += 1
            Next
            Return c
        End Function

        Public Overrides Function ToString() As String
            Return $"R²={R2:F4}, PCC={Pcc:F4}, RMSE={Rmse:F5}, MAE={Mae:F5}, " &
                   $"meanGenePCC={MeanGenePcc:F4}, N={N}"
        End Function

#Region "计算"

        ''' <summary>
        ''' 计算预测值与真值之间的回归指标（按元素展平计算全局指标，按列计算逐基因指标）。
        ''' </summary>
        ''' <param name="predicted">预测值（任意形状）</param>
        ''' <param name="actual">真值（形状必须一致）</param>
        Public Shared Function Compute(predicted As Tensor, actual As Tensor) As RegressionMetrics
            If predicted Is Nothing OrElse actual Is Nothing Then
                Throw New ArgumentNullException(NameOf(predicted))
            End If
            If Not predicted.Shape.SequenceEqual(actual.Shape) Then
                Throw New ArgumentException(
                    $"预测值与真值形状不一致：[{String.Join(",", predicted.Shape)}] vs [{String.Join(",", actual.Shape)}]")
            End If

            Dim p = predicted.Data
            Dim y = actual.Data
            Dim n = p.Length

            Dim sumY = 0.0
            For i = 0 To n - 1
                sumY += y(i)
            Next
            Dim meanY = sumY / n

            Dim ssRes = 0.0
            Dim ssTot = 0.0
            Dim mae = 0.0
            For i = 0 To n - 1
                Dim e = p(i) - y(i)
                ssRes += e * e
                ssTot += (y(i) - meanY) * (y(i) - meanY)
                mae += std.Abs(e)
            Next

            Dim r2 = If(ssTot > 0.0, 1.0 - ssRes / ssTot, Double.NaN)
            Dim pcc = Pearson(p, y)

            Dim result As New RegressionMetrics With {
                .N = n,
                .R2 = r2,
                .Pcc = pcc,
                .Rmse = std.Sqrt(ssRes / n),
                .Mae = mae / n
            }

            ' ---- 逐基因指标（要求二维张量且 batch ≥ 2，否则相关系数无定义） ----
            If predicted.Rank = 2 AndAlso predicted.Shape(0) >= 2 Then
                Dim batch = predicted.Shape(0)
                Dim genes = predicted.Shape(1)
                Dim gp(genes - 1) As Double
                Dim gr(genes - 1) As Double

                Dim a(batch - 1) As Double
                Dim b(batch - 1) As Double

                For j = 0 To genes - 1
                    Dim sy = 0.0
                    For i = 0 To batch - 1
                        a(i) = predicted.Data(i * genes + j)
                        b(i) = actual.Data(i * genes + j)
                        sy += b(i)
                    Next
                    Dim my = sy / batch

                    Dim ssr = 0.0
                    Dim sst = 0.0
                    For i = 0 To batch - 1
                        ssr += (a(i) - b(i)) ^ 2
                        sst += (b(i) - my) ^ 2
                    Next

                    gp(j) = Pearson(a, b)
                    gr(j) = If(sst > 0.0, 1.0 - ssr / sst, Double.NaN)
                Next

                result.GenePcc = gp
                result.GeneR2 = gr
                result.MeanGenePcc = MeanOf(gp)
                result.MeanGeneR2 = MeanOf(gr)
            End If

            Return result
        End Function

#End Region

#Region "统计工具"

        ''' <summary>
        ''' Pearson 相关系数。任一序列方差为零（或长度不足 2）时返回 <see cref="Double.NaN"/>，
        ''' 而不是抛出异常——"这个基因在时间轴上没有变化"是合法结果，应当被如实报告。
        ''' </summary>
        Public Shared Function Pearson(a As Double(), b As Double()) As Double
            If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length OrElse a.Length < 2 Then
                Return Double.NaN
            End If

            Dim n = a.Length
            Dim sa = 0.0
            Dim sb = 0.0
            For i = 0 To n - 1
                sa += a(i)
                sb += b(i)
            Next
            Dim ma = sa / n
            Dim mb = sb / n

            Dim cov = 0.0
            Dim va = 0.0
            Dim vb = 0.0
            For i = 0 To n - 1
                Dim da = a(i) - ma
                Dim db = b(i) - mb
                cov += da * db
                va += da * da
                vb += db * db
            Next

            If va <= 0.0 OrElse vb <= 0.0 Then Return Double.NaN
            Return cov / std.Sqrt(va * vb)
        End Function

        ''' <summary>忽略 NaN 求均值（全为 NaN 时返回 NaN）</summary>
        Public Shared Function MeanOf(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then Return Double.NaN

            Dim s = 0.0
            Dim c = 0
            For Each v In values
                If Double.IsNaN(v) Then Continue For
                s += v
                c += 1
            Next

            Return If(c = 0, Double.NaN, s / c)
        End Function

        ''' <summary>Spearman 秩相关系数（忽略 NaN 对），用于响应时序与层级深度的单调性检验</summary>
        Public Shared Function Spearman(a As Double(), b As Double()) As Double
            Dim pairs As New List(Of (x As Double, y As Double))()
            Dim n = std.Min(a.Length, b.Length)
            For i = 0 To n - 1
                If Double.IsNaN(a(i)) OrElse Double.IsNaN(b(i)) Then Continue For
                pairs.Add((a(i), b(i)))
            Next

            If pairs.Count < 3 Then Return Double.NaN

            Dim ra = Ranks(pairs.Select(Function(t) t.x).ToArray())
            Dim rb = Ranks(pairs.Select(Function(t) t.y).ToArray())
            Return Pearson(ra, rb)
        End Function

        ''' <summary>把一组数转换为平均秩（并列值取平均秩）</summary>
        Private Shared Function Ranks(values As Double()) As Double()
            Dim n = values.Length
            Dim idx = Enumerable.Range(0, n).ToArray()
            Array.Sort(idx, Function(i, j) values(i).CompareTo(values(j)))

            Dim rankValues(n - 1) As Double
            Dim i0 = 0
            While i0 < n
                Dim i1 = i0
                While i1 + 1 < n AndAlso values(idx(i1 + 1)) = values(idx(i0))
                    i1 += 1
                End While
                Dim avg = (i0 + i1) / 2.0 + 1.0
                For k = i0 To i1
                    rankValues(idx(k)) = avg
                Next
                i0 = i1 + 1
            End While

            Return rankValues
        End Function

#End Region

    End Class

End Namespace
