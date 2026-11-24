Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.GPS.Data
Imports SMRUCC.genomics.Analysis.GPS.Model

Namespace Training

    ''' <summary>
    ''' 模型评估指标（method.md §7.2）。
    ''' </summary>
    Public Class EvaluationMetrics

        ''' <summary>窗口数量</summary>
        Public Property NWindows As Integer

        ''' <summary>参与评估的基因数量</summary>
        Public Property NGenes As Integer

        ''' <summary>全基因均方误差</summary>
        Public Property MSE As Double

        ''' <summary>决定系数 <c>1 − SSE/SST</c></summary>
        Public Property R2 As Double

        ''' <summary>预测与真值的 Pearson 相关系数</summary>
        Public Property Pearson As Double

        ''' <summary>每个窗口内变化最大的 top-K 基因上的均方误差（困难指标）</summary>
        Public Property TopKMse As Double

        ''' <summary>方向准确率 <c>mean(sign(ŷ−prev) == sign(y−prev))</c></summary>
        Public Property DirectionAccuracy As Double

        ''' <summary>先验图外基因子集上的均方误差（未提供掩码时为 NaN）</summary>
        Public Property IsolatedMSE As Double = Double.NaN

        ''' <summary>先验图外基因子集上的决定系数（未提供掩码时为 NaN）</summary>
        Public Property IsolatedR2 As Double = Double.NaN

        ''' <summary>输出单行摘要</summary>
        ''' <returns>指标摘要文本</returns>
        Public Overrides Function ToString() As String
            Return $"N={NWindows}×{NGenes}  MSE={MSE:G4}  R²={R2:F4}  r={Pearson:F4}  " &
                   $"topK_MSE={TopKMse:G4}  dir={DirectionAccuracy:P1}"
        End Function

    End Class

    ''' <summary>
    ''' 模型评估器与消融对比工具。
    ''' </summary>
    ''' <remarks>
    ''' 评估严格使用调用方传入的<b>测试时间段</b>窗口（<see cref="TimeSeriesWindowDataset.Test"/>），
    ''' 不做任何随机划分。图外基因子集通过传入的 <c>isolated</c> 掩码单独统计，
    ''' 用于回答"GNN+Transformer 是否为先验图外基因带来了跨基因信息"这一关键问题。
    ''' </remarks>
    Public Module Evaluator

        ''' <summary>
        ''' 在给定窗口集合上评估模型。
        ''' </summary>
        ''' <param name="model">已训练的模型</param>
        ''' <param name="windows">评估窗口（通常为测试集）</param>
        ''' <param name="topK">困难基因指标的 K 值</param>
        ''' <param name="isolated">长度等于基因数的"图外基因"掩码，可选</param>
        ''' <returns>评估指标</returns>
        Public Function Evaluate(model As GRNModel,
                                 windows As List(Of TimeSeriesWindow),
                                 Optional topK As Integer = 20,
                                 Optional isolated As Boolean() = Nothing) As EvaluationMetrics

            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            If windows Is Nothing OrElse windows.Count = 0 Then
                Return New EvaluationMetrics With {.NWindows = 0, .NGenes = model.NumGenes}
            End If

            Dim n As Integer = model.NumGenes
            Dim total As Integer = windows.Count * n
            Dim prediction As Double() = New Double(total - 1) {}
            Dim truth As Double() = New Double(total - 1) {}
            Dim previous As Double() = New Double(total - 1) {}
            Dim topKSum As Double = 0.0
            Dim topKCount As Integer = 0
            Dim directionAgree As Integer = 0

            For w As Integer = 0 To windows.Count - 1
                Dim window As TimeSeriesWindow = windows(w)
                Dim pred As Tensor = model.Forward(window.Input, False)
                Dim offset As Integer = w * n

                For i As Integer = 0 To n - 1
                    prediction(offset + i) = pred.Data(i)
                    truth(offset + i) = window.Target(i)
                    previous(offset + i) = window.Previous(i)

                    If System.Math.Sign(pred.Data(i) - window.Previous(i)) = System.Math.Sign(window.Target(i) - window.Previous(i)) Then
                        directionAgree += 1
                    End If
                Next

                ' ---- 困难基因：本窗口内真实变化最大的 top-K ----
                Dim order As Integer() = Enumerable.Range(0, n).
                    OrderByDescending(Function(i) System.Math.Abs(window.Target(i) - window.Previous(i))).
                    ToArray()

                For k As Integer = 0 To System.Math.Min(topK, n) - 1
                    Dim i As Integer = order(k)
                    Dim e As Double = pred.Data(i) - window.Target(i)

                    topKSum += e * e
                    topKCount += 1
                Next
            Next

            Dim metrics As New EvaluationMetrics With {
                .NWindows = windows.Count,
                .NGenes = n,
                .MSE = MeanSquaredError(prediction, truth),
                .R2 = RSquared(prediction, truth),
                .Pearson = PearsonCorrelation(prediction, truth),
                .TopKMse = If(topKCount > 0, topKSum / topKCount, Double.NaN),
                .DirectionAccuracy = directionAgree / CDbl(total)
            }

            If isolated IsNot Nothing Then
                Dim subsetPrediction As New List(Of Double)()
                Dim subsetTruth As New List(Of Double)()

                For w As Integer = 0 To windows.Count - 1
                    Dim offset As Integer = w * n

                    For i As Integer = 0 To n - 1
                        If isolated(i) Then
                            Call subsetPrediction.Add(prediction(offset + i))
                            Call subsetTruth.Add(truth(offset + i))
                        End If
                    Next
                Next

                If subsetPrediction.Count > 0 Then
                    Dim p As Double() = subsetPrediction.ToArray()
                    Dim t As Double() = subsetTruth.ToArray()

                    metrics.IsolatedMSE = MeanSquaredError(p, t)
                    metrics.IsolatedR2 = RSquared(p, t)
                End If
            End If

            Return metrics
        End Function

        ''' <summary>
        ''' "历史持续性"基线：直接用窗口最后一个时刻作为预测值（AR(1) 无参数基线）。
        ''' </summary>
        ''' <param name="windows">评估窗口</param>
        ''' <param name="isolated">图外基因掩码，可选</param>
        ''' <returns>基线指标</returns>
        Public Function EvaluatePersistence(windows As List(Of TimeSeriesWindow),
                                            Optional isolated As Boolean() = Nothing) As EvaluationMetrics
            If windows Is Nothing OrElse windows.Count = 0 Then
                Return New EvaluationMetrics With {.NWindows = 0}
            End If

            Dim n As Integer = windows(0).NGene
            Dim total As Integer = windows.Count * n
            Dim prediction As Double() = New Double(total - 1) {}
            Dim truth As Double() = New Double(total - 1) {}
            Dim directionAgree As Integer = 0

            For w As Integer = 0 To windows.Count - 1
                Dim offset As Integer = w * n

                For i As Integer = 0 To n - 1
                    prediction(offset + i) = windows(w).Previous(i)
                    truth(offset + i) = windows(w).Target(i)

                    If System.Math.Sign(prediction(offset + i) - windows(w).Previous(i)) =
                       System.Math.Sign(truth(offset + i) - windows(w).Previous(i)) Then
                        directionAgree += 1
                    End If
                Next
            Next

            Dim metrics As New EvaluationMetrics With {
                .NWindows = windows.Count,
                .NGenes = n,
                .MSE = MeanSquaredError(prediction, truth),
                .R2 = RSquared(prediction, truth),
                .Pearson = PearsonCorrelation(prediction, truth),
                .TopKMse = Double.NaN,
                .DirectionAccuracy = directionAgree / CDbl(total)
            }

            If isolated IsNot Nothing Then
                Dim p As New List(Of Double)()
                Dim t As New List(Of Double)()

                For w As Integer = 0 To windows.Count - 1
                    Dim offset As Integer = w * n

                    For i As Integer = 0 To n - 1
                        If isolated(i) Then
                            Call p.Add(prediction(offset + i))
                            Call t.Add(truth(offset + i))
                        End If
                    Next
                Next

                If p.Count > 0 Then
                    metrics.IsolatedMSE = MeanSquaredError(p.ToArray(), t.ToArray())
                    metrics.IsolatedR2 = RSquared(p.ToArray(), t.ToArray())
                End If
            End If

            Return metrics
        End Function

        ''' <summary>
        ''' 根据调控图计算"先验图外基因"掩码（出入度均为 0 的节点）。
        ''' </summary>
        ''' <param name="model">模型（提供图编码的度信息）</param>
        ''' <returns>长度为基因数的布尔掩码，True 表示图外基因</returns>
        Public Function IsolatedGeneMask(model As GRNModel) As Boolean()
            Dim mask As Boolean() = New Boolean(model.NumGenes - 1) {}

            For i As Integer = 0 To mask.Length - 1
                mask(i) = model.Encoding.InDegree(i) = 0 AndAlso model.Encoding.OutDegree(i) = 0
            Next

            Return mask
        End Function

        ''' <summary>均方误差</summary>
        ''' <param name="prediction">预测值</param>
        ''' <param name="truth">真值</param>
        ''' <returns>MSE</returns>
        Public Function MeanSquaredError(prediction As Double(), truth As Double()) As Double
            Dim sum As Double = 0.0

            For i As Integer = 0 To prediction.Length - 1
                Dim e As Double = prediction(i) - truth(i)
                sum += e * e
            Next

            Return If(prediction.Length > 0, sum / prediction.Length, 0.0)
        End Function

        ''' <summary>决定系数</summary>
        ''' <param name="prediction">预测值</param>
        ''' <param name="truth">真值</param>
        ''' <returns>R²；真值方差为 0 时返回 NaN</returns>
        Public Function RSquared(prediction As Double(), truth As Double()) As Double
            Dim mean As Double = 0.0

            For i As Integer = 0 To truth.Length - 1
                mean += truth(i)
            Next

            mean /= truth.Length

            Dim sse As Double = 0.0
            Dim sst As Double = 0.0

            For i As Integer = 0 To truth.Length - 1
                Dim e As Double = prediction(i) - truth(i)
                Dim d As Double = truth(i) - mean

                sse += e * e
                sst += d * d
            Next

            If sst <= 0.0 Then Return Double.NaN

            Return 1.0 - sse / sst
        End Function

        ''' <summary>Pearson 相关系数</summary>
        ''' <param name="prediction">预测值</param>
        ''' <param name="truth">真值</param>
        ''' <returns>相关系数；任一序列方差为 0 时返回 0</returns>
        Public Function PearsonCorrelation(prediction As Double(), truth As Double()) As Double
            Dim n As Integer = prediction.Length
            Dim meanP As Double = 0.0
            Dim meanT As Double = 0.0

            For i As Integer = 0 To n - 1
                meanP += prediction(i)
                meanT += truth(i)
            Next

            meanP /= n
            meanT /= n

            Dim cov As Double = 0.0
            Dim varP As Double = 0.0
            Dim varT As Double = 0.0

            For i As Integer = 0 To n - 1
                Dim dp As Double = prediction(i) - meanP
                Dim dt As Double = truth(i) - meanT

                cov += dp * dt
                varP += dp * dp
                varT += dt * dt
            Next

            If varP <= 0.0 OrElse varT <= 0.0 Then Return 0.0

            Return cov / System.Math.Sqrt(varP * varT)
        End Function

    End Module

End Namespace
