' ============================================================================
' RgbVaeOnnxImport.vb - 从 ONNX 文件还原 RGB 生成模型
'
' 还原分三步，任何一步失败都只返回中文原因，不抛异常、不留半成品模型：
'   1. metadata  → 超参数（尺寸 / 通道 / 潜变量 / 卷积宽度）
'   2. 图输入输出 → 与 metadata 交叉校验（防止 metadata 与图不一致）
'   3. 初始化器   → 逐张量做 ONNX → 框架的布局逆变换与形状校验
'
' 上采样核不参与参数还原（框架里它是固定常量），但会校验文件里写的确实是
' 2×2 单位核，否则同一个 ONNX 文件的生成结果不可能与框架结果一致。
' ============================================================================

Imports System.IO
Imports Microsoft.VisualBasic.MachineLearning.DataStorage.ONNX
Imports Microsoft.VisualBasic.MachineLearning.DataStorage.ONNX.Proto
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Imaging

Namespace Serialization

    ''' <summary>
    ''' 从 ONNX 模型还原 <see cref="RgbGenerativeVae"/>。
    ''' </summary>
    Public Module RgbVaeOnnxImport

        ''' <summary>从 ONNX 文件还原模型；失败时返回 <c>Nothing</c> 并给出中文原因。</summary>
        ''' <param name="path">ONNX 文件路径。</param>
        ''' <param name="errorMessage">失败原因。</param>
        Public Function Load(path As String, ByRef errorMessage As String) As RgbGenerativeVae
            errorMessage = Nothing

            If String.IsNullOrEmpty(path) Then
                errorMessage = "ONNX 文件路径不能为空"
                Return Nothing
            End If

            If Not File.Exists(path) Then
                errorMessage = $"找不到 ONNX 模型文件：{path}"
                Return Nothing
            End If

            Try
                Return FromModel(OnnxModel.Load(path), errorMessage)
            Catch ex As Exception
                errorMessage = $"读取 ONNX 模型失败: {ex.Message}"
                Return Nothing
            End Try
        End Function

        ''' <summary>从 ONNX 文件还原模型；失败时抛出异常。</summary>
        ''' <param name="path">ONNX 文件路径。</param>
        Public Function Load(path As String) As RgbGenerativeVae
            Dim errorMessage As String = Nothing
            Dim model As RgbGenerativeVae = Load(path, errorMessage)

            If model Is Nothing Then
                Throw New InvalidDataException(errorMessage)
            End If

            Return model
        End Function

        ''' <summary>从已加载的 ONNX 模型对象还原模型；失败时返回 <c>Nothing</c> 并给出中文原因。</summary>
        Public Function FromModel(model As OnnxModel, ByRef errorMessage As String) As RgbGenerativeVae
            errorMessage = Nothing

            If model Is Nothing Then
                errorMessage = "ONNX 模型为空"
                Return Nothing
            End If

            Try
                Dim hyper As ConvVaeParameters = Nothing

                If Not TryReadMetadata(model, hyper, errorMessage) Then
                    Return Nothing
                End If

                Dim parameters As ConvVaeParameters = BuildParameters(model, hyper)

                Return RgbGenerativeVae.FromParameters(parameters)
            Catch ex As Exception
                errorMessage = ex.Message
                Return Nothing
            End Try
        End Function

        ''' <summary>从 ONNX 模型对象还原模型；失败时抛出异常。</summary>
        Public Function FromModel(model As OnnxModel) As RgbGenerativeVae
            Dim errorMessage As String = Nothing
            Dim result As RgbGenerativeVae = FromModel(model, errorMessage)

            If result Is Nothing Then
                Throw New InvalidDataException(errorMessage)
            End If

            Return result
        End Function

#Region "还原"

        Private Function BuildParameters(model As OnnxModel, hyper As ConvVaeParameters) As ConvVaeParameters
            Dim channels As Integer = hyper.Channels
            Dim filter1 As Integer = hyper.Filter1
            Dim filter2 As Integer = hyper.Filter2
            Dim latentDim As Integer = hyper.LatentDim
            Dim flattened As Integer = hyper.FlattenedSize
            Dim reducedHeight As Integer = hyper.Height \ 4
            Dim reducedWidth As Integer = hyper.Width \ 4

            Call ValidateInterface(model, hyper)
            Call ValidateReshapeShape(model, reducedHeight, reducedWidth, filter2)

            Dim result As New ConvVaeParameters With {
                .Height = hyper.Height,
                .Width = hyper.Width,
                .Channels = channels,
                .LatentDim = latentDim,
                .Filter1 = filter1,
                .Filter2 = filter2
            }

            ' ---- 编码器 ----
            result.Tensors(ConvVaeParameters.KeyEncFilter1) = ReadConvolution(
                model, InitEncConv1Weight, {filter1, channels, 3, 3}, {3, 3, channels, filter1})
            result.Tensors(ConvVaeParameters.KeyEncBias1) = ReadFlat(
                model, InitEncConv1Bias, {filter1}, {filter1})
            result.Tensors(ConvVaeParameters.KeyEncFilter2) = ReadConvolution(
                model, InitEncConv2Weight, {filter2, filter1, 3, 3}, {3, 3, filter1, filter2})
            result.Tensors(ConvVaeParameters.KeyEncBias2) = ReadFlat(
                model, InitEncConv2Bias, {filter2}, {filter2})

            result.Tensors(ConvVaeParameters.KeyMeanWeight) = ReadDense(
                model, InitEncMeanWeight, {latentDim, flattened}, flattened, latentDim, {flattened, latentDim})
            result.Tensors(ConvVaeParameters.KeyMeanBias) = ReadFlat(
                model, InitEncMeanBias, {latentDim}, {1, latentDim})
            result.Tensors(ConvVaeParameters.KeyLogVarWeight) = ReadDense(
                model, InitEncLogVarWeight, {latentDim, flattened}, flattened, latentDim, {flattened, latentDim})
            result.Tensors(ConvVaeParameters.KeyLogVarBias) = ReadFlat(
                model, InitEncLogVarBias, {latentDim}, {1, latentDim})

            ' ---- 生成器 ----
            result.Tensors(ConvVaeParameters.KeyExpandWeight) = ReadDense(
                model, InitGenExpandWeight, {flattened, latentDim}, latentDim, flattened, {latentDim, flattened})
            result.Tensors(ConvVaeParameters.KeyExpandBias) = ReadFlat(
                model, InitGenExpandBias, {flattened}, {1, flattened})

            Call ValidateUpsampleKernel(model, InitGenUp1Weight, filter2)

            result.Tensors(ConvVaeParameters.KeyMidFilter) = ReadConvolution(
                model, InitGenConv1Weight, {filter1, filter2, 3, 3}, {3, 3, filter2, filter1})
            result.Tensors(ConvVaeParameters.KeyMidBias) = ReadFlat(
                model, InitGenConv1Bias, {filter1}, {filter1})

            Call ValidateUpsampleKernel(model, InitGenUp2Weight, filter1)

            result.Tensors(ConvVaeParameters.KeyOutFilter) = ReadConvolution(
                model, InitGenConv2Weight, {channels, filter1, 3, 3}, {3, 3, filter1, channels})
            result.Tensors(ConvVaeParameters.KeyOutBias) = ReadFlat(
                model, InitGenConv2Bias, {channels}, {channels})

            Return result
        End Function

        ''' <summary>校验图输入的形状声明是否与 metadata 一致。</summary>
        Private Sub ValidateInterface(model As OnnxModel, hyper As ConvVaeParameters)
            Dim image As ValueInfoProto = FindInput(model, GraphInputImage)

            If image Is Nothing Then
                Throw New InvalidDataException($"ONNX 图缺少输入 '{GraphInputImage}'")
            End If

            Dim imageDims As Long() = image.GetDims()

            If imageDims.Length <> 4 Then
                Throw New InvalidDataException(
                    $"输入 '{GraphInputImage}' 必须是 4 维 [N, C, H, W]，实际 [{String.Join(", ", imageDims)}]")
            End If

            If imageDims(1) <> hyper.Channels OrElse imageDims(2) <> hyper.Height OrElse imageDims(3) <> hyper.Width Then
                Throw New InvalidDataException(
                    $"输入 '{GraphInputImage}' 的形状与 metadata 不一致：图声明 [{String.Join(", ", imageDims)}]，" &
                    $"metadata 为 [N, {hyper.Channels}, {hyper.Height}, {hyper.Width}]")
            End If

            Dim latent As ValueInfoProto = FindInput(model, GraphInputLatent)

            If latent Is Nothing Then
                Throw New InvalidDataException($"ONNX 图缺少输入 '{GraphInputLatent}'")
            End If

            Dim latentDims As Long() = latent.GetDims()

            If latentDims.Length <> 2 Then
                Throw New InvalidDataException(
                    $"输入 '{GraphInputLatent}' 必须是 2 维 [N, latent]，实际 [{String.Join(", ", latentDims)}]")
            End If

            If latentDims(1) <> hyper.LatentDim Then
                Throw New InvalidDataException(
                    $"输入 '{GraphInputLatent}' 的潜变量维度与 metadata 不一致：图声明 {latentDims(1)}，metadata 为 {hyper.LatentDim}")
            End If
        End Sub

        ''' <summary>校验 Reshape 的目标形状确实对应 metadata 里的特征图尺寸。</summary>
        Private Sub ValidateReshapeShape(model As OnnxModel, reducedHeight As Integer, reducedWidth As Integer, filter2 As Integer)
            Dim shape As OnnxTensor = RequireInitializer(model, InitGenReshapeShape, {4})
            Dim data As Double() = shape.Data

            If data(1) <> reducedHeight OrElse data(2) <> reducedWidth OrElse data(3) <> filter2 Then
                Throw New InvalidDataException(
                    $"'{InitGenReshapeShape}' 的目标形状与 metadata 不一致：实际 [{String.Join(", ", data)}]，" &
                    $"期望 [0, {reducedHeight}, {reducedWidth}, {filter2}]")
            End If
        End Sub

        ''' <summary>校验上采样核是否就是 2×2 的单位核。</summary>
        Private Sub ValidateUpsampleKernel(model As OnnxModel, name As String, channels As Integer)
            Dim weights As OnnxTensor = RequireInitializer(model, name, {channels, channels, 2, 2})

            If Not IsUpsampleKernel(weights) Then
                Throw New InvalidDataException(
                    $"'{name}' 不是 2×2 的最近邻上采样核（每个通道应只映射到自身且权重为 1），" &
                    "该文件的生成结果无法与框架结果一致")
            End If
        End Sub

        Private Function FindInput(model As OnnxModel, name As String) As ValueInfoProto
            For Each input As ValueInfoProto In model.Inputs
                If String.Equals(input.Name, name, StringComparison.Ordinal) Then
                    Return input
                End If
            Next

            Return Nothing
        End Function

        ''' <summary>取出一维张量并重塑为框架要求的一维 / 二维形状。</summary>
        Private Function ReadFlat(model As OnnxModel, name As String, onnxDims As Integer(),
                                  frameworkDims As Integer()) As Tensor
            Dim source As OnnxTensor = RequireInitializer(model, name, onnxDims)
            Dim data As Double() = New Double(source.Data.Length - 1) {}

            Call Array.Copy(source.Data, data, data.Length)

            Return New Tensor(data, frameworkDims)
        End Function

        ''' <summary>取出卷积权重并转回框架的 <c>[kH, kW, Cin, Cout]</c> 布局。</summary>
        Private Function ReadConvolution(model As OnnxModel, name As String, onnxDims As Integer(),
                                         frameworkDims As Integer()) As Tensor
            Dim source As OnnxTensor = RequireInitializer(model, name, onnxDims)

            Return New Tensor(ConvWeightsFromOnnx(source), frameworkDims)
        End Function

        ''' <summary>取出 Gemm 权重并转回框架的 <c>[fanIn, fanOut]</c> 布局。</summary>
        Private Function ReadDense(model As OnnxModel, name As String, onnxDims As Integer(),
                                   fanIn As Integer, fanOut As Integer, frameworkDims As Integer()) As Tensor
            Dim source As OnnxTensor = RequireInitializer(model, name, onnxDims)

            Return New Tensor(DenseWeightsFromOnnx(source, fanIn, fanOut), frameworkDims)
        End Function

        ''' <summary>取出初始化器并校验形状。</summary>
        Private Function RequireInitializer(model As OnnxModel, name As String, expectedDims As Integer()) As OnnxTensor
            If Not model.HasInitializer(name) Then
                Throw New InvalidDataException($"ONNX 模型缺少初始化器 '{name}'")
            End If

            Dim tensor As OnnxTensor = model.GetInitializer(name)

            If Not ConvVaeParameters.ShapeEquals(tensor.Dims, expectedDims) Then
                Throw New InvalidDataException(
                    $"初始化器 '{name}' 的形状不符：期望 [{String.Join(", ", expectedDims)}]，" &
                    $"实际 [{String.Join(", ", tensor.Dims)}]")
            End If

            Return tensor
        End Function

#End Region

    End Module

End Namespace
