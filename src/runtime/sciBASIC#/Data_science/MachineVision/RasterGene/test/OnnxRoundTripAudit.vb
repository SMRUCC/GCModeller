' ============================================================================
' OnnxRoundTripAudit.vb - ONNX 持久化的往返校验
'
' 只断言「可观测的事实」，不依赖内部实现：
'   1. 文件写出后结构正确（输入 / 输出 / 算子 / 初始化器齐全）
'   2. 从文件重新加载得到结构相同的模型，权重逐值一致（单精度容差内）
'   3. 重新加载的模型与内存中的模型：重构结果一致、同种子的生成结果一致
'   4. 用 MLDataStorage 的图评估器跑导出的 ONNX 图，数值与框架前向一致
'      —— 这一条在没有 onnxruntime 的环境里也能证明图是算得对的
'   5. 损坏的文件必须被拒绝，并给出中文原因
' ============================================================================

Imports System.IO
Imports System.Linq
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.MachineLearning.DataStorage.ONNX
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core.Numerics
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Imaging
Imports Microsoft.VisualBasic.MachineVision.RasterGenerative
Imports Microsoft.VisualBasic.MachineVision.RasterGenerative.Serialization
Imports std = System.Math

''' <summary>ONNX 往返校验。</summary>
Public Module OnnxRoundTripAudit

    ''' <summary>权重比对的绝对容差（权重按单精度落盘时约 1e-7 的相对误差）。</summary>
    Public Const WeightTolerance As Double = 0.00001

    ''' <summary>图像像素比对的绝对容差（误差会沿卷积链放大）。</summary>
    Public Const ImageTolerance As Double = 0.0001

    ''' <summary>导出图里允许出现的算子集合。</summary>
    Private ReadOnly Property AllowedOperators As String()
        Get
            Return New String() {"Conv", "ConvTranspose", "Gemm", "Transpose", "Flatten", "Reshape", "Relu", "Sigmoid"}
        End Get
    End Property

    ''' <summary>执行完整的往返校验。</summary>
    ''' <param name="model">已训练的内存模型。</param>
    ''' <param name="images">用于重构对照的图像批（<c>[N, H, W, 3]</c>）。</param>
    ''' <param name="onnxPath">导出目标路径。</param>
    ''' <param name="outputDir">报告与摘要的输出目录。</param>
    Public Sub Run(model As RgbGenerativeVae, images As Tensor, onnxPath As String, outputDir As String)
        Console.WriteLine("== ONNX 往返校验 ==")

        Call model.SaveOnnx(onnxPath)
        Call DebugAudit.Test(File.Exists(onnxPath), $"ONNX 文件必须写出: {onnxPath}")
        Call DebugAudit.Test(New FileInfo(onnxPath).Length > 0, "ONNX 文件不能为空")

        Dim onnxModel As OnnxModel = OnnxModel.Load(onnxPath)

        Call File.WriteAllText(Path.Combine(outputDir, "onnx-summary.txt"), onnxModel.Summary())

        Call VerifyStructure(model, onnxModel)

        ' ---- 回读 ----
        Dim errorMessage As String = Nothing
        Dim restored As RgbGenerativeVae = RgbGenerativeVae.LoadOnnx(onnxPath, errorMessage)

        Call DebugAudit.Test(restored IsNot Nothing, $"ONNX 回读失败: {errorMessage}")

        If restored Is Nothing Then
            Console.WriteLine($"  ONNX 回读失败: {errorMessage}")
            Return
        End If

        Call DebugAudit.AreEqual(model.Height, restored.Height, "回读后的图像高度必须一致")
        Call DebugAudit.AreEqual(model.Width, restored.Width, "回读后的图像宽度必须一致")
        Call DebugAudit.AreEqual(model.Channels, restored.Channels, "回读后的通道数必须一致")
        Call DebugAudit.AreEqual(model.LatentDim, restored.LatentDim, "回读后的潜变量维度必须一致")
        Call DebugAudit.AreEqual(model.Filter1, restored.Filter1, "回读后的第一层卷积宽度必须一致")
        Call DebugAudit.AreEqual(model.Filter2, restored.Filter2, "回读后的第二层卷积宽度必须一致")
        Call DebugAudit.Test(restored.IsFitted, "回读后的模型应处于已训练状态")

        Call VerifyWeights(model, restored)
        Call VerifyReconstruction(model, restored, images)
        Call VerifyGeneration(model, restored)
        Call VerifyWithEvaluator(model, onnxModel, images)
        Call VerifyCorruptedFileRejected()

        Console.WriteLine($"  ONNX 往返校验完成: {onnxPath}")
    End Sub

#Region "结构"

    Private Sub VerifyStructure(model As RgbGenerativeVae, onnxModel As OnnxModel)
        Dim inputs As String() = onnxModel.Inputs.Select(Function(v) v.Name).ToArray()
        Dim outputs As String() = onnxModel.Outputs.Select(Function(v) v.Name).ToArray()

        Call DebugAudit.AreEqual(2, inputs.Length,
                                 $"ONNX 图必须同时包含编码器与生成器的输入端，实际: {String.Join(", ", inputs)}")
        Call DebugAudit.Test(inputs.Contains(RgbVaeOnnxContract.GraphInputImage), "缺少输入 image")
        Call DebugAudit.Test(inputs.Contains(RgbVaeOnnxContract.GraphInputLatent), "缺少输入 latent")

        Call DebugAudit.AreEqual(3, outputs.Length,
                                 $"ONNX 图必须输出 mu / logvar / image_out，实际: {String.Join(", ", outputs)}")
        Call DebugAudit.Test(outputs.Contains(RgbVaeOnnxContract.GraphOutputMean), "缺少输出 mu")
        Call DebugAudit.Test(outputs.Contains(RgbVaeOnnxContract.GraphOutputLogVar), "缺少输出 logvar")
        Call DebugAudit.Test(outputs.Contains(RgbVaeOnnxContract.GraphOutputImage), "缺少输出 image_out")

        ' 图像输入必须是标准 NCHW
        Dim imageInput = onnxModel.Inputs.First(Function(v) v.Name = RgbVaeOnnxContract.GraphInputImage)
        Dim dims As Long() = imageInput.GetDims()

        Call DebugAudit.AreEqual(4, dims.Length, "输入 image 必须是 4 维 [N, C, H, W]")
        Call DebugAudit.AreEqual(CLng(model.Channels), dims(1), "输入 image 的通道维必须是 NCHW 布局的第 2 维")
        Call DebugAudit.AreEqual(CLng(model.Height), dims(2), "输入 image 的高度与模型不一致")
        Call DebugAudit.AreEqual(CLng(model.Width), dims(3), "输入 image 的宽度与模型不一致")

        ' 只允许出现可移植的标准算子
        Dim ops As String() = onnxModel.GetOpTypes()
        Dim unexpected As String() = ops.Where(Function(op) Not AllowedOperators.Contains(op)).ToArray()

        Call DebugAudit.Test(unexpected.Length = 0,
                             $"计算图中出现了预期之外的算子: {String.Join(", ", unexpected)}")
        Call DebugAudit.Test(ops.Contains("Conv") AndAlso ops.Contains("ConvTranspose") AndAlso ops.Contains("Gemm"),
                             $"计算图必须包含 Conv / ConvTranspose / Gemm，实际: {String.Join(", ", ops)}")

        ' 必需初始化器
        For Each name As String In RgbVaeOnnxContract.RequiredInitializers
            Call DebugAudit.Test(onnxModel.HasInitializer(name), $"ONNX 模型缺少初始化器: {name}")
        Next

        ' metadata 超参数
        Dim metadata As Dictionary(Of String, String) = onnxModel.Metadata

        Call DebugAudit.AreEqual(RgbVaeOnnxContract.ModelKind,
                                 If(metadata.ContainsKey(RgbVaeOnnxContract.MetaKind), metadata(RgbVaeOnnxContract.MetaKind), ""),
                                 "metadata 中的模型种类不符")
        Call DebugAudit.AreEqual(model.LatentDim.ToString(),
                                 If(metadata.ContainsKey(RgbVaeOnnxContract.MetaLatentDim), metadata(RgbVaeOnnxContract.MetaLatentDim), ""),
                                 "metadata 中的潜变量维度不符")

        Console.WriteLine($"  ONNX 图: {onnxModel.Graph.Node.Count} 个节点, {onnxModel.Initializers.Count} 个初始化器")
        Console.WriteLine($"  算子序列: {String.Join(" -> ", onnxModel.Graph.Node.Select(Function(n) n.OpType))}")
    End Sub

#End Region

#Region "权重与数值"

    Private Sub VerifyWeights(model As RgbGenerativeVae, restored As RgbGenerativeVae)
        Dim difference As Double = MaxParameterDifference(model.Parameters, restored.Parameters)

        Call DebugAudit.Capture("onnx.weight.maxdiff", difference)
        Call DebugAudit.Test(difference < WeightTolerance,
                             $"ONNX 往返后的权重最大偏差为 {difference:E3}，超出容差 {WeightTolerance:E3}")
    End Sub

    Private Sub VerifyReconstruction(model As RgbGenerativeVae, restored As RgbGenerativeVae, images As Tensor)
        Dim reference As Tensor = model.Reconstruct(images)
        Dim reloaded As Tensor = restored.Reconstruct(images)
        Dim difference As Double = MaxAbsDiff(reference, reloaded)

        Call DebugAudit.Capture("onnx.reconstruct.maxdiff", difference)
        Call DebugAudit.Test(difference < ImageTolerance,
                             $"ONNX 往返后的重构结果最大偏差为 {difference:E3}，超出容差 {ImageTolerance:E3}")
    End Sub

    Private Sub VerifyGeneration(model As RgbGenerativeVae, restored As RgbGenerativeVae)
        Const generationSeed As Integer = 20261224
        Const count As Integer = 8

        Dim reference As Tensor = model.Generate(count, generationSeed)
        Dim reloaded As Tensor = restored.Generate(count, generationSeed)
        Dim difference As Double = MaxAbsDiff(reference, reloaded)

        Call DebugAudit.Capture("onnx.generate.maxdiff", difference)
        Call DebugAudit.Test(difference < ImageTolerance,
                             $"ONNX 往返后的同种子生成结果最大偏差为 {difference:E3}，超出容差 {ImageTolerance:E3}")
    End Sub

    ''' <summary>
    ''' 用图评估器执行导出的 ONNX 图，与框架前向逐像素对照。
    ''' </summary>
    Private Sub VerifyWithEvaluator(model As RgbGenerativeVae, onnxModel As OnnxModel, images As Tensor)
        Dim shape As Integer() = images.Shape
        Dim count As Integer = shape(0)
        Dim evaluator As New OnnxGraphEvaluator(onnxModel)
        Dim nchw As OnnxTensor = ToOnnxNchw(images)

        Console.WriteLine($"  probe reshape: [{String.Join(", ", OnnxTensor.Zeros(count, 1024).Reshape({0, 8, 8, 16}).Dims)}]")

        ' ---- 1) 编码器 + 生成器端到端：图像 → mu → 图像 ----
        Dim zeros As OnnxTensor = OnnxTensor.Zeros(count, model.LatentDim)
        Dim encoderEnv As New Dictionary(Of String, OnnxTensor) From {
            {RgbVaeOnnxContract.GraphInputImage, nchw},
            {RgbVaeOnnxContract.GraphInputLatent, zeros}
        }
        Dim encoderOutput As Dictionary(Of String, OnnxTensor) = evaluator.Run(encoderEnv)
        Dim mean As OnnxTensor = encoderOutput(RgbVaeOnnxContract.GraphOutputMean)

        Call DebugAudit.AreEqual(count, mean.Dims(0), "编码器输出的批大小与输入不一致")
        Call DebugAudit.AreEqual(model.LatentDim, mean.Dims(1), "编码器输出的潜变量维度与模型不一致")

        Dim decodeEnv As New Dictionary(Of String, OnnxTensor) From {
            {RgbVaeOnnxContract.GraphInputImage, nchw},
            {RgbVaeOnnxContract.GraphInputLatent, mean}
        }
        Dim decoded As OnnxTensor = evaluator.Run(decodeEnv)(RgbVaeOnnxContract.GraphOutputImage)
        Dim expected As Tensor = model.Reconstruct(images)
        Dim difference As Double = MaxAbsDiff(decoded, expected)

        Call DebugAudit.Capture("onnx.graph.reconstruct.maxdiff", difference)
        Call DebugAudit.Test(difference < ImageTolerance,
                             $"ONNX 图（image→mu→image）与框架重构的最大偏差为 {difference:E3}，超出容差 {ImageTolerance:E3}")

        ' ---- 2) 生成分支：与 ConvVAE.Generate 使用相同的 latent ----
        Dim latent As OnnxTensor = LatentFromSeed(20261224, count, model.LatentDim)
        Dim generateEnv As New Dictionary(Of String, OnnxTensor) From {
            {RgbVaeOnnxContract.GraphInputImage, nchw},
            {RgbVaeOnnxContract.GraphInputLatent, latent}
        }
        Dim generated As OnnxTensor = evaluator.Run(generateEnv)(RgbVaeOnnxContract.GraphOutputImage)
        Dim expectedGenerated As Tensor = model.Generate(count, 20261224)
        Dim generateDifference As Double = MaxAbsDiff(generated, expectedGenerated)

        Call DebugAudit.Capture("onnx.graph.generate.maxdiff", generateDifference)
        Call DebugAudit.Test(generateDifference < ImageTolerance,
                             $"ONNX 生成分支与框架 Generate 的最大偏差为 {generateDifference:E3}，超出容差 {ImageTolerance:E3}")
    End Sub

    Private Sub VerifyCorruptedFileRejected()
        Dim brokenPath As String = Path.Combine(Path.GetTempPath(), $"rastergene-broken-{Guid.NewGuid():N}.onnx")
        Dim errorMessage As String = Nothing

        Try
            Call File.WriteAllBytes(brokenPath, New Byte() {1, 2, 3, 4, 5, 6, 7})

            Dim broken As RgbGenerativeVae = RgbGenerativeVae.LoadOnnx(brokenPath, errorMessage)

            Call DebugAudit.Test(broken Is Nothing, "损坏的 ONNX 文件不能被当成有效模型")
            Call DebugAudit.Test(Not String.IsNullOrEmpty(errorMessage), "拒绝损坏文件时必须给出原因")
        Finally
            If File.Exists(brokenPath) Then
                Call File.Delete(brokenPath)
            End If
        End Try
    End Sub

#End Region

#Region "数值辅助"

    ''' <summary>潜变量采样：与 <c>ConvVAE.Generate</c> 内部使用同一套种子派生规则。</summary>
    Private Function LatentFromSeed(seed As Integer, count As Integer, latentDim As Integer) As OnnxTensor
        Dim rng As New RandomGaussian(seed)
        Dim data As Double() = New Double(count * latentDim - 1) {}

        Call rng.NextNormals(data, 0, data.Length)

        Return OnnxTensor.FromArray(data, count, latentDim)
    End Function

    ''' <summary>把 NHWC 图像批张量转成 ONNX 的 NCHW 表示。</summary>
    Private Function ToOnnxNchw(images As Tensor) As OnnxTensor
        Dim shape As Integer() = images.Shape
        Dim count As Integer = shape(0)
        Dim height As Integer = shape(1)
        Dim width As Integer = shape(2)
        Dim channels As Integer = shape(3)
        Dim source As Double() = images.Data
        Dim data As Double() = New Double(source.Length - 1) {}

        For n As Integer = 0 To count - 1
            For c As Integer = 0 To channels - 1
                For y As Integer = 0 To height - 1
                    For x As Integer = 0 To width - 1
                        data(((n * channels + c) * height + y) * width + x) = source(((n * height + y) * width + x) * channels + c)
                    Next
                Next
            Next
        Next

        Return OnnxTensor.FromArray(data, count, channels, height, width)
    End Function

    ''' <summary>比较 NCHW 的 ONNX 张量与 NHWC 的框架张量。</summary>
    Private Function MaxAbsDiff(nchw As OnnxTensor, nhwc As Tensor) As Double
        If nchw.Rank <> 4 Then
            Return Double.PositiveInfinity
        End If

        Dim converted As OnnxTensor = nchw.Transpose({0, 2, 3, 1})

        If converted.Data.Length <> nhwc.Data.Length Then
            Return Double.PositiveInfinity
        End If

        Dim difference As Double = 0

        For i As Integer = 0 To converted.Data.Length - 1
            difference = std.Max(difference, std.Abs(converted.Data(i) - nhwc.Data(i)))
        Next

        Return difference
    End Function

    ''' <summary>比较两个同形状张量。</summary>
    Private Function MaxAbsDiff(left As Tensor, right As Tensor) As Double
        If left Is Nothing OrElse right Is Nothing Then
            Return Double.PositiveInfinity
        End If

        If left.Data.Length <> right.Data.Length Then
            Return Double.PositiveInfinity
        End If

        Dim difference As Double = 0

        For i As Integer = 0 To left.Data.Length - 1
            difference = std.Max(difference, std.Abs(left.Data(i) - right.Data(i)))
        Next

        Return difference
    End Function

    ''' <summary>比较两份参数快照中每个张量的最大绝对偏差。</summary>
    Private Function MaxParameterDifference(left As ConvVaeParameters, right As ConvVaeParameters) As Double
        Dim difference As Double = 0

        For Each name As String In ConvVaeParameters.TensorKeys
            Dim a As Tensor = left.GetTensor(name)
            Dim b As Tensor = right.GetTensor(name)

            If a Is Nothing OrElse b Is Nothing Then
                Return Double.PositiveInfinity
            End If

            If Not ConvVaeParameters.ShapeEquals(a.Shape, b.Shape) Then
                Return Double.PositiveInfinity
            End If

            For i As Integer = 0 To a.Data.Length - 1
                difference = std.Max(difference, std.Abs(a.Data(i) - b.Data(i)))
            Next
        Next

        Return difference
    End Function

#End Region

End Module
