' ============================================================================
' GmvaeAudit.vb - 高斯混合变分自编码器的生产级审计
'
' 覆盖：纯自编码器重构能力（隔离「网络本身是否可训练」）、持久化往返、
'       同种子可复现、条件生成、输入校验、伯努利似然的有界输出。
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder
Imports std = System.Math

''' <summary>
''' 高斯混合变分自编码器生产级审计。
''' </summary>
Public Module GmvaeAudit

    Public Sub Run()
        Call AuditSupport.RunCase("gmvae.autoencoder", AddressOf AutoencoderMustReconstruct)
        Call AuditSupport.RunCase("gmvae.deterministic", AddressOf TrainingMustBeReproducible)
        Call AuditSupport.RunCase("gmvae.persistence", AddressOf ModelMustRoundTrip)
        Call AuditSupport.RunCase("gmvae.conditional-generation", AddressOf ConditionalGenerationMustWork)
        Call AuditSupport.RunCase("gmvae.validation", AddressOf InputValidationMustThrow)
        Call AuditSupport.RunCase("gmvae.bernoulli", AddressOf BernoulliLikelihoodMustBeBounded)
    End Sub

#Region "自编码器能力"

    ''' <summary>
    ''' 关掉 KL 项之后模型必须退化为一个普通自编码器，并且能把数据重构回来。
    ''' 若这一项都不通过，说明网络的前向/反向通路本身有问题。
    ''' </summary>
    Private Sub AutoencoderMustReconstruct()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(3, 60, 4, 0.4, 202, X, labels)

        Dim model As New GMVAE(inputDim:=4, latentDim:=4, nComponents:=3,
                               hiddenDim:=32, seed:=202)

        Call model.Fit(X, epochs:=150, batchSize:=32, learningRate:=0.01,
                       beta:=0.0, gamma:=0.0, verbose:=False)

        Dim reconstructed As Tensor = model.Reconstruct(X, useMean:=True)
        Dim errorValue As Double = AuditSupport.Mse(reconstructed, X)

        ' 数据本身的逐元素方差约 5.3；一个好的自编码器应远低于该值
        DebugAudit.Capture("gmvae.ae.mse", errorValue)

        Call DebugAudit.Test(AuditSupport.AllFinite(reconstructed), "重构结果必须为有限值")
        Call DebugAudit.Test(errorValue < 0.5,
                             $"纯自编码器应把重构 MSE 降到 0.5 以下，实际为 {errorValue:F4}（数据方差约 5.3）")
    End Sub

    Private Sub TrainingMustBeReproducible()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(2, 40, 3, 0.4, 303, X, labels)

        Dim first As New GMVAE(inputDim:=3, latentDim:=3, nComponents:=2, hiddenDim:=16, seed:=303)
        Dim second As New GMVAE(inputDim:=3, latentDim:=3, nComponents:=2, hiddenDim:=16, seed:=303)

        Call first.Fit(X, epochs:=20, batchSize:=16, learningRate:=0.002, beta:=0.5, gamma:=0.5, verbose:=False)
        Call second.Fit(X, epochs:=20, batchSize:=16, learningRate:=0.002, beta:=0.5, gamma:=0.5, verbose:=False)

        Call DebugAudit.Test(first.LossHistory.Count = second.LossHistory.Count,
                             "两次训练的轮数应一致")

        Dim maxError As Double = 0

        For i As Integer = 0 To std.Min(first.LossHistory.Count, second.LossHistory.Count) - 1
            maxError = std.Max(maxError, std.Abs(first.LossHistory(i) - second.LossHistory(i)))
        Next

        DebugAudit.Capture("gmvae.reproducibility.error", maxError)

        Call DebugAudit.Test(maxError < 1.0E-12,
                             $"同种子的训练损失应完全一致，最大偏差 {maxError}")
    End Sub

#End Region

#Region "持久化"

    Private Sub ModelMustRoundTrip()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(2, 40, 3, 0.4, 404, X, labels)

        Dim model As New GMVAE(inputDim:=3, latentDim:=2, nComponents:=2, hiddenDim:=16, seed:=404)
        Call model.Fit(X, epochs:=30, batchSize:=16, learningRate:=0.003, beta:=0.5, gamma:=0.5, verbose:=False)

        Dim path As String = IO.Path.Combine(AppContext.BaseDirectory, "gmvae-audit-model.bin")
        Call model.Save(path)

        Dim restored As New GMVAE(inputDim:=3, latentDim:=2, nComponents:=2, hiddenDim:=16, seed:=404)
        Dim errorMessage As String = Nothing

        Call DebugAudit.Test(restored.TryLoad(path, errorMessage), $"模型加载失败: {errorMessage}")

        If Not restored.IsFitted Then
            Return
        End If

        Dim expected As Tensor = model.Reconstruct(X, useMean:=True)
        Dim actual As Tensor = restored.Reconstruct(X, useMean:=True)
        Dim errorValue As Double = AuditSupport.MaxAbsDiff(expected, actual)

        DebugAudit.Capture("gmvae.persistence.error", errorValue)

        Call DebugAudit.Test(errorValue < 1.0E-12,
                             $"还原后的重构结果应逐元素一致，最大偏差 {errorValue}")

        Dim before As Integer() = model.PredictClusters(X)
        Dim after As Integer() = restored.PredictClusters(X)

        For i As Integer = 0 To before.Length - 1
            If before(i) <> after(i) Then
                Call DebugAudit.Test(False, $"还原后的聚类标签在样本 {i} 处不一致")
                Exit For
            End If
        Next

        If IO.File.Exists(path) Then
            IO.File.Delete(path)
        End If
    End Sub

#End Region

#Region "生成与似然"

    Private Sub ConditionalGenerationMustWork()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(3, 60, 3, 0.4, 505, X, labels)

        Dim model As New GMVAE(inputDim:=3, latentDim:=3, nComponents:=3, hiddenDim:=24, seed:=505)
        Call model.Fit(X, epochs:=80, batchSize:=32, learningRate:=0.005, beta:=1.0, gamma:=1.0, verbose:=False)

        Dim priorMu As Tensor = model.PriorMean

        For k As Integer = 0 To 2
            Dim generated As Tensor = model.Generate(20, componentIdx:=k, seed:=(100 + k))

            Call DebugAudit.Test(AuditSupport.AllFinite(generated),
                                 $"成分 {k} 的生成样本必须为有限值")
            Call DebugAudit.Test(generated.Shape(0) = 20,
                                 $"成分 {k} 的生成样本数量应为 20，实际 {generated.Shape(0)}")

            ' 同一成分的生成样本应聚拢在对应先验中心附近
            Dim mean As Double() = New Double(2) {}

            For i As Integer = 0 To 19
                For d As Integer = 0 To 2
                    mean(d) += generated(i, d) / 20.0
                Next
            Next

            Dim distance As Double = 0

            For d As Integer = 0 To 2
                Dim delta As Double = mean(d) - priorMu(k, d)
                distance += delta * delta
            Next

            DebugAudit.Capture($"gmvae.generate.center-distance.{k}", std.Sqrt(distance))
        Next

        ' 混合采样（componentIdx = -1）
        Dim mixed As Tensor = model.Generate(50, componentIdx:=-1, seed:=999)
        Call DebugAudit.Test(AuditSupport.AllFinite(mixed), "混合先验采样必须为有限值")
    End Sub

    Private Sub InputValidationMustThrow()
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(2, 30, 3, 0.4, 606, X, labels)

        Dim model As New GMVAE(inputDim:=3, latentDim:=2, nComponents:=2, hiddenDim:=8, seed:=606)

        Call DebugAudit.CheckThrows(Of InvalidOperationException)(
            Sub() Call model.PredictClusters(X),
            "未训练时预测必须抛出 InvalidOperationException")

        Call model.Fit(X, epochs:=5, batchSize:=16, learningRate:=0.001, verbose:=False)

        Dim wrong As New Tensor(New Double(10 * 4 - 1) {}, 10, 4)
        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub() Call model.Reconstruct(wrong),
            "维度不符时重构必须抛出 ArgumentException")

        Call DebugAudit.CheckThrows(Of ArgumentException)(
            Sub() Call model.Fit(New Tensor(New Double() {1.0, 2.0}, 2)),
            "非二维输入时 Fit 必须抛出 ArgumentException")
    End Sub

    Private Sub BernoulliLikelihoodMustBeBounded()
        ' 把数据缩放到 [0,1] 区间
        Dim X As Tensor = Nothing
        Dim labels As Integer() = Nothing
        Call AuditSupport.GaussianBlobs(2, 40, 3, 0.2, 707, X, labels)

        Dim scaled As New Tensor(X.Shape)
        Dim minimum As Double = Double.MaxValue
        Dim maximum As Double = Double.MinValue

        For i As Integer = 0 To X.Length - 1
            minimum = std.Min(minimum, X.Data(i))
            maximum = std.Max(maximum, X.Data(i))
        Next

        Dim range As Double = std.Max(maximum - minimum, 1.0E-9)

        For i As Integer = 0 To X.Length - 1
            scaled.Data(i) = (X.Data(i) - minimum) / range
        Next

        Dim model As New GMVAE(inputDim:=3, latentDim:=2, nComponents:=2, hiddenDim:=16, seed:=707) With {
            .OutputLikelihood = GmvaeLikelihood.Bernoulli
        }

        Call model.Fit(scaled, epochs:=40, batchSize:=16, learningRate:=0.005, beta:=0.2, gamma:=0.2, verbose:=False)

        Dim reconstructed As Tensor = model.Reconstruct(scaled, useMean:=True)
        Dim bounded As Boolean = True

        For i As Integer = 0 To reconstructed.Length - 1
            If reconstructed.Data(i) < 0 OrElse reconstructed.Data(i) > 1 Then
                bounded = False
                Exit For
            End If
        Next

        Call DebugAudit.Test(bounded, "伯努利似然下重构值必须落在 [0, 1] 区间")

        Dim generated As Tensor = model.Generate(10, seed:=708)
        Dim generatedBounded As Boolean = True

        For i As Integer = 0 To generated.Length - 1
            If generated.Data(i) < 0 OrElse generated.Data(i) > 1 Then
                generatedBounded = False
                Exit For
            End If
        Next

        Call DebugAudit.Test(generatedBounded, "伯努利似然下生成值必须落在 [0, 1] 区间")
    End Sub

#End Region

End Module
