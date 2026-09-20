Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports std = System.Math

Namespace Evaluation

    ''' <summary>
    ''' 扰动响应预测的评估结果。
    '''
    ''' 关键设计：除了"预测 vs 真实"，还同时给出**对照基线**（不做扰动建模、直接拿对照细胞当预测）
    ''' 的指标，只有显著优于基线才能说明模型真的学到了扰动效应。
    ''' </summary>
    Public Class PerturbationPredictionResult

        ''' <summary>预测转录组 vs 真实转录组。</summary>
        Public Property Prediction As RegressionMetrics

        ''' <summary>对照基线（对照细胞 vs 真实扰动细胞）。</summary>
        Public Property ControlBaseline As RegressionMetrics

        ''' <summary>
        ''' 扰动"效应量"相关性：<c>(预测 − 对照)</c> 与 <c>(真实 − 对照)</c> 的总体 Pearson。
        ''' 该指标排除了基线表达水平，直接衡量扰动方向是否被预测正确。
        ''' </summary>
        Public Property DeltaProfilePcc As Double = Double.NaN

        ''' <summary>逐细胞扰动效应量 PCC 的均值。</summary>
        Public Property MeanCellDeltaPcc As Double = Double.NaN

        ''' <summary>Top-K 差异表达基因的重叠率（逐细胞平均）。</summary>
        Public Property MeanTopKOverlap As Double = Double.NaN

        ''' <summary>被预测细胞数。</summary>
        Public Property NCell As Integer

        ''' <summary>Top-K 使用的 K。</summary>
        Public Property TopK As Integer

        ''' <summary>预测相对于对照基线的 PCC 提升量。</summary>
        Public ReadOnly Property PccGain As Double
            Get
                If Prediction Is Nothing OrElse ControlBaseline Is Nothing Then Return Double.NaN
                Return Prediction.Pcc - ControlBaseline.Pcc
            End Get
        End Property

        ''' <summary>预测相对于对照基线的逐基因 PCC 提升量。</summary>
        Public ReadOnly Property MeanGenePccGain As Double
            Get
                If Prediction Is Nothing OrElse ControlBaseline Is Nothing Then Return Double.NaN
                Return Prediction.MeanGenePcc() - ControlBaseline.MeanGenePcc()
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"PCC={Prediction?.Pcc:F4}(基线 {ControlBaseline?.Pcc:F4}, +{PccGain:F4}) " &
                   $"ΔPCC={DeltaProfilePcc:F4} TopK重叠={MeanTopKOverlap:F3}"
        End Function
    End Class

    ''' <summary>扰动预测的评估与隐空间方向分析。</summary>
    Public Module PerturbationMetrics

        ''' <summary>
        ''' 完整评估：预测 / 真实 / 对照三者的形状必须一致 <c>[cell, gene]</c>。
        ''' </summary>
        Public Function Evaluate(predicted As Tensor, actual As Tensor, control As Tensor,
                                 Optional topK As Integer = 20) As PerturbationPredictionResult
            Dim result As New PerturbationPredictionResult With {
                .Prediction = RegressionMetrics.Compute(predicted, actual),
                .ControlBaseline = RegressionMetrics.Compute(control, actual),
                .NCell = predicted.Shape(0),
                .TopK = topK
            }

            Dim predDelta = Subtract(predicted, control)
            Dim trueDelta = Subtract(actual, control)

            result.DeltaProfilePcc = RegressionMetrics.Pearson(predDelta.Data, trueDelta.Data)

            Dim rows = predicted.Shape(0)
            Dim columns = predicted.Shape(1)
            Dim p = predDelta.Data
            Dim t = trueDelta.Data

            Dim cellDeltaPcc(rows - 1) As Double
            Dim overlaps(rows - 1) As Double

            For i As Integer = 0 To rows - 1
                Dim offset = i * columns
                Dim pd(columns - 1) As Double
                Dim td(columns - 1) As Double
                For j As Integer = 0 To columns - 1
                    pd(j) = p(offset + j)
                    td(j) = t(offset + j)
                Next

                cellDeltaPcc(i) = RegressionMetrics.Pearson(pd, td)
                overlaps(i) = TopKOverlap(pd, td, topK)
            Next

            result.MeanCellDeltaPcc = RegressionMetrics.MeanOf(cellDeltaPcc)
            result.MeanTopKOverlap = RegressionMetrics.MeanOf(overlaps)

            Return result
        End Function

        ''' <summary>
        ''' 隐空间方向一致性：给定两组 <c>Δz</c> 向量集合，逐行计算余弦相似度并取均值。
        ''' 用于衡量"同一扰动在不同细胞类型上是否沿相近的语义方向"。
        ''' </summary>
        Public Function DirectionConsistency(deltaA As Tensor, deltaB As Tensor) As Double
            If Not deltaA.Shape.SequenceEqual(deltaB.Shape) Then
                Throw New ArgumentException("两个 Δz 矩阵形状必须一致")
            End If

            Dim rows = deltaA.Shape(0)
            Dim columns = deltaA.Shape(1)
            Dim a = deltaA.Data
            Dim b = deltaB.Data
            Dim cosines(rows - 1) As Double

            For i As Integer = 0 To rows - 1
                Dim offset = i * columns
                Dim dot As Double = 0.0
                Dim normA As Double = 0.0
                Dim normB As Double = 0.0

                For j As Integer = 0 To columns - 1
                    dot += a(offset + j) * b(offset + j)
                    normA += a(offset + j) * a(offset + j)
                    normB += b(offset + j) * b(offset + j)
                Next

                If normA <= 1.0E-300 OrElse normB <= 1.0E-300 Then
                    cosines(i) = Double.NaN
                Else
                    cosines(i) = dot / (std.Sqrt(normA) * std.Sqrt(normB))
                End If
            Next

            Return RegressionMetrics.MeanOf(cosines)
        End Function

        ''' <summary>逐行余弦相似度（用于导出一致性矩阵）。</summary>
        Public Function RowCosines(deltaA As Tensor, deltaB As Tensor) As Double()
            Dim rows = deltaA.Shape(0)
            Dim columns = deltaA.Shape(1)
            Dim a = deltaA.Data
            Dim b = deltaB.Data
            Dim result(rows - 1) As Double

            For i As Integer = 0 To rows - 1
                Dim offset = i * columns
                Dim dot As Double = 0.0
                Dim normA As Double = 0.0
                Dim normB As Double = 0.0

                For j As Integer = 0 To columns - 1
                    dot += a(offset + j) * b(offset + j)
                    normA += a(offset + j) * a(offset + j)
                    normB += b(offset + j) * b(offset + j)
                Next

                result(i) = If(normA <= 1.0E-300 OrElse normB <= 1.0E-300,
                               Double.NaN,
                               dot / (std.Sqrt(normA) * std.Sqrt(normB)))
            Next

            Return result
        End Function

        ''' <summary>两个等长向量按 |值| 排序后 Top-K 集合的重叠率（Jaccard 式：交集 / K）。</summary>
        Public Function TopKOverlap(a As Double(), b As Double(), k As Integer) As Double
            If a Is Nothing OrElse b Is Nothing OrElse a.Length <> b.Length OrElse a.Length = 0 Then Return Double.NaN

            Dim n = std.Min(k, a.Length)
            If n <= 0 Then Return Double.NaN

            Dim setA = TopIndices(a, n)
            Dim setB = TopIndices(b, n)

            Dim intersection As Integer = 0
            For Each index In setA
                If setB.Contains(index) Then intersection += 1
            Next

            Return CDbl(intersection) / n
        End Function

        ''' <summary>元素相减（形状需一致）。</summary>
        Public Function Subtract(a As Tensor, b As Tensor) As Tensor
            Return Tensor.computeKernel.Subtract(a, b)
        End Function

        ''' <summary>取绝对值最大的前 <paramref name="n"/> 个元素下标。</summary>
        Private Function TopIndices(values As Double(), n As Integer) As HashSet(Of Integer)
            Dim order = Enumerable.Range(0, values.Length).ToArray()
            Array.Sort(order, Function(x, y) std.Abs(values(y)).CompareTo(std.Abs(values(x))))

            Dim result As New HashSet(Of Integer)
            For i As Integer = 0 To n - 1
                result.Add(order(i))
            Next

            Return result
        End Function
    End Module
End Namespace
