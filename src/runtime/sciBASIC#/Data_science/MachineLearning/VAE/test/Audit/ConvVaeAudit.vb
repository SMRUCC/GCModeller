' ============================================================================
' ConvVaeAudit.vb - 卷积变分自编码器的审计
'
' 覆盖：上采样与转置卷积算子的等价性（复用的是 Conv2DBackwardInput）、
'       伯努利输出的有界性、训练能降低重构误差、持久化往返。
' ============================================================================

Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow.Compute
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Imaging
Imports std = System.Math

''' <summary>
''' 卷积变分自编码器审计。
''' </summary>
Public Module ConvVaeAudit

    Public Sub Run()
        Call AuditSupport.RunCase("convvae.upsample", AddressOf UpsampleMustBeNearestNeighbour)
        Call AuditSupport.RunCase("convvae.reconstruction", AddressOf TrainingMustReduceReconstructionError)
        Call AuditSupport.RunCase("convvae.bounded", AddressOf OutputMustBeBounded)
        Call AuditSupport.RunCase("convvae.persistence", AddressOf ModelMustRoundTrip)
    End Sub

#Region "算子等价性"

    ''' <summary>
    ''' 用「2×2 单位核 + 步长 2」的 Conv2DBackwardInput 实现最近邻上采样：
    ''' 每个输入像素应被复制成 2×2 的块。
    ''' </summary>
    Private Sub UpsampleMustBeNearestNeighbour()
        Dim compute As ITensorCompute = Tensor.computeKernel

        Dim source As New Tensor(New Double() {1.0, 2.0, 3.0, 4.0}, 1, 2, 2, 1)
        Dim kernel As New Tensor(New Integer() {2, 2, 1, 1})
        kernel.Data(0) = 1.0
        kernel.Data(1) = 1.0
        kernel.Data(2) = 1.0
        kernel.Data(3) = 1.0

        Dim upsampled As Tensor = compute.Conv2DBackwardInput(source, kernel, New Integer() {1, 4, 4, 1}, 2, 0)

        Call DebugAudit.Test(upsampled.Shape(1) = 4 AndAlso upsampled.Shape(2) = 4,
                             $"上采样后应为 4x4，实际 {upsampled.Shape(1)}x{upsampled.Shape(2)}")

        ' 最近邻上采样的本质是「每个像素被复制 4 份」。具体排列方式取决于
        ' 卷积/转置卷积的下标对齐约定，因此这里断言的是它与约定无关的性质：
        ' 每个取值恰好出现 4 次，且总和放大 4 倍。
        Dim counts As New Dictionary(Of Double, Integer)

        For i As Integer = 0 To upsampled.Length - 1
            Dim value As Double = upsampled.Data(i)

            If counts.ContainsKey(value) Then
                counts(value) += 1
            Else
                counts(value) = 1
            End If
        Next

        For Each value As Double In New Double() {1.0, 2.0, 3.0, 4.0}
            Call DebugAudit.Test(counts.ContainsKey(value) AndAlso counts(value) = 4,
                                 $"取值 {value} 应出现 4 次，实际 {If(counts.ContainsKey(value), counts(value), 0)}")
        Next

        Call DebugAudit.Test(std.Abs(AuditSupport.SumOf(upsampled) - 4.0 * AuditSupport.SumOf(source)) < 1.0E-12,
                             "上采样后的总和应为输入的 4 倍")

        ' 上采样的反向必须是同一核上的 Conv2D 前向（伴随算子）：2×2 求和
        Dim gradient As New Tensor(New Double(15) {}, 1, 4, 4, 1)

        For i As Integer = 0 To 15
            gradient.Data(i) = i + 1
        Next

        Dim downward As Tensor = compute.Conv2D(gradient, kernel, Tensor.Zeros(New Integer() {1}), 2, 0)

        Call DebugAudit.Test(downward.Shape(1) = 2 AndAlso downward.Shape(2) = 2,
                             $"下采样后应为 2x2，实际 {downward.Shape(1)}x{downward.Shape(2)}")
        Call DebugAudit.Test(std.Abs(AuditSupport.SumOf(downward) - AuditSupport.SumOf(gradient)) < 1.0E-12,
                             "上采样的伴随算子应保持总和（2×2 全 1 核）")
    End Sub

#End Region

#Region "训练"

    ''' <summary>构造 8x8 的图像：随机位置有一个 3x3 的亮块。</summary>
    Private Function BuildImages(count As Integer, seed As Integer) As Tensor
        Dim rng As New Random(seed)
        Dim images As New Tensor(count, 8, 8, 1)
        Dim buffer As Double() = images.Data

        For n As Integer = 0 To count - 1
            Dim top As Integer = rng.Next(0, 6)
            Dim left As Integer = rng.Next(0, 6)

            For h As Integer = 0 To 7
                For w As Integer = 0 To 7
                    Dim inside As Boolean = h >= top AndAlso h < top + 3 AndAlso w >= left AndAlso w < left + 3
                    buffer(((n * 8 + h) * 8 + w)) = If(inside, 1.0, 0.0)
                Next
            Next
        Next

        Return images
    End Function

    Private Sub TrainingMustReduceReconstructionError()
        Dim images As Tensor = BuildImages(48, 2026)
        Dim model As New ConvVAE(8, 8, channels:=1, latentDim:=8, filter1:=8, filter2:=12, seed:=2026)

        Dim before As Tuple(Of Double, Double, Double) = model.Evaluate(images)

        Call model.Fit(images, epochs:=100, batchSize:=12, learningRate:=0.008, beta:=0.3, verbose:=False)

        Dim after As Tuple(Of Double, Double, Double) = model.Evaluate(images)

        DebugAudit.Capture("convvae.bce.before", before.Item1)
        DebugAudit.Capture("convvae.bce.after", after.Item1)
        DebugAudit.Capture("convvae.mse.after", after.Item2)

        Call DebugAudit.Test(AuditSupport.AllFinite(model.Loss), "训练损失必须全部为有限值")
        Call DebugAudit.Test(after.Item1 < before.Item1 * 0.7,
                             $"训练应显著降低每像素 BCE: {before.Item1:F5} -> {after.Item1:F5}")

        Dim reconstructed As Tensor = model.Reconstruct(images)

        Call DebugAudit.Test(AuditSupport.AllFinite(reconstructed), "重构结果必须为有限值")
        Call DebugAudit.Test(reconstructed.Shape(1) = 8 AndAlso reconstructed.Shape(3) = 1,
                             "重构结果的形状必须与输入一致")
    End Sub

    Private Sub OutputMustBeBounded()
        Dim images As Tensor = BuildImages(16, 77)
        Dim model As New ConvVAE(8, 8, channels:=1, latentDim:=6, filter1:=6, filter2:=8, seed:=77)

        Call model.Fit(images, epochs:=10, batchSize:=8, learningRate:=0.005, beta:=0.5, verbose:=False)

        Dim reconstructed As Tensor = model.Reconstruct(images)
        Dim generated As Tensor = model.Generate(6, seed:=123)

        Call DebugAudit.Test(IsWithinUnitInterval(reconstructed), "伯努利似然下重构值必须落在 [0, 1]")
        Call DebugAudit.Test(IsWithinUnitInterval(generated), "伯努利似然下生成值必须落在 [0, 1]")
        Call DebugAudit.Test(generated.Shape(0) = 6, $"生成样本数应为 6，实际 {generated.Shape(0)}")
    End Sub

    Private Function IsWithinUnitInterval(t As Tensor) As Boolean
        Dim buffer As Double() = t.Data

        For i As Integer = 0 To buffer.Length - 1
            If buffer(i) < 0 OrElse buffer(i) > 1 Then
                Return False
            End If
        Next

        Return True
    End Function

#End Region

#Region "持久化"

    Private Sub ModelMustRoundTrip()
        Dim images As Tensor = BuildImages(16, 314)
        Dim model As New ConvVAE(8, 8, channels:=1, latentDim:=5, filter1:=6, filter2:=8, seed:=314)

        Call model.Fit(images, epochs:=10, batchSize:=8, learningRate:=0.005, beta:=0.5, verbose:=False)

        Dim path As String = IO.Path.Combine(AppContext.BaseDirectory, "convvae-audit-model.bin")
        Call model.Save(path)

        Dim restored As New ConvVAE(8, 8, channels:=1, latentDim:=5, filter1:=6, filter2:=8)
        Dim errorMessage As String = Nothing

        Call DebugAudit.Test(restored.TryLoad(path, errorMessage), $"模型加载失败: {errorMessage}")

        Dim errorValue As Double = AuditSupport.MaxAbsDiff(model.Reconstruct(images), restored.Reconstruct(images))

        DebugAudit.Capture("convvae.persistence.error", errorValue)

        Call DebugAudit.Test(errorValue < 1.0E-12,
                             $"还原后的重构结果应逐元素一致，最大偏差 {errorValue}")

        If IO.File.Exists(path) Then
            IO.File.Delete(path)
        End If
    End Sub

#End Region

End Module
