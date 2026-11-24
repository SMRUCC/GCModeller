' ============================================================================
' RgbVaeOnnxExport.vb - 把 RGB 生成模型导出为单文件双分支 ONNX 计算图
'
'                             ┌── encoder ──────────────────────────────────┐
'   image [N,3,H,W] ── Conv ── Relu ── Conv ── Relu ── Transpose(NHWC)
'                     ── Flatten ── Gemm(mean) ──► mu      [N,latent]
'                              └── Gemm(logvar) ─► logvar  [N,latent]
'
'                             ┌── generator ────────────────────────────────┐
'   latent [N,latent] ── Gemm ── Reshape ── Transpose(NCHW) ── Relu
'                     ── ConvTranspose(×2) ── Conv ── Relu
'                     ── ConvTranspose(×2) ── Conv ── Sigmoid ──► image_out [N,3,H,W]
'
' 对外张量一律 NCHW（与 PyTorch / onnxruntime 一致）；通道顺序的转换只发生在
' 「展平之前」这一处 Transpose 上，因此 Dense 权重不需要任何重排。
'
' 上采样用 ConvTranspose（核为 2×2 单位阵、步长 2）表示：这正是框架里
' Conv2DBackwardInput 的数学等价形式，两者在权重层面可以逐值对照。
' ============================================================================

Imports System.IO
Imports Microsoft.VisualBasic.MachineLearning.DataStorage.ONNX
Imports Microsoft.VisualBasic.MachineLearning.DataStorage.ONNX.Proto
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Imaging

Namespace Serialization

    ''' <summary>
    ''' 把 RGB 生成模型（卷积 VAE）导出为 ONNX 模型。
    ''' </summary>
    Public Module RgbVaeOnnxExport

        ''' <summary>构建（不写文件）表示该模型的 ONNX 模型对象。</summary>
        ''' <param name="parameters">模型参数快照。</param>
        ''' <param name="options">导出选项；Nothing 表示使用默认值。</param>
        Public Function ToModel(parameters As ConvVaeParameters,
                                Optional options As RgbVaeExportOptions = Nothing) As OnnxModel
            If parameters Is Nothing Then
                Throw New ArgumentNullException(NameOf(parameters))
            End If

            If options Is Nothing Then
                options = New RgbVaeExportOptions()
            End If

            Dim graph As GraphProto = BuildGraph(parameters, options)
            Dim model As New ModelProto With {
                .IrVersion = options.IrVersion,
                .ProducerName = options.ProducerName,
                .ProducerVersion = options.ProducerVersion,
                .Graph = graph,
                .DocString = $"RGB generative VAE ({parameters.Height}x{parameters.Width}x{parameters.Channels}, latent={parameters.LatentDim})"
            }

            Call model.OpsetImport.Add(New OperatorSetIdProto With {.Domain = "", .Version = options.Opset})
            Call WriteMetadata(model, parameters, options)

            Return New OnnxModel(model)
        End Function

        ''' <summary>构建表示该模型的 ONNX 模型对象。</summary>
        Public Function ToModel(model As RgbGenerativeVae,
                                Optional options As RgbVaeExportOptions = Nothing) As OnnxModel
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If

            Return ToModel(model.Parameters, options)
        End Function

        ''' <summary>把模型参数保存为 ONNX 文件。</summary>
        Public Sub Save(parameters As ConvVaeParameters, path As String,
                        Optional options As RgbVaeExportOptions = Nothing)
            If String.IsNullOrEmpty(path) Then
                Throw New ArgumentException("ONNX 文件路径不能为空", NameOf(path))
            End If

            Call ToModel(parameters, options).Save(path)
        End Sub

        ''' <summary>把已训练的模型保存为 ONNX 文件。</summary>
        Public Sub Save(model As RgbGenerativeVae, path As String,
                        Optional options As RgbVaeExportOptions = Nothing)
            If model Is Nothing Then
                Throw New ArgumentNullException(NameOf(model))
            End If

            If Not model.IsFitted Then
                Throw New InvalidOperationException("模型尚未训练，不接受未训练的权重导出")
            End If

            Call Save(model.Parameters, path, options)
        End Sub

        ''' <summary>把模型写入数据流（用于测试内直接校验而不落盘）。</summary>
        Public Sub Write(model As RgbGenerativeVae, stream As Stream,
                         Optional options As RgbVaeExportOptions = Nothing,
                         Optional leaveOpen As Boolean = True)
            Call ToModel(model, options).Save(stream, leaveOpen)
        End Sub

#Region "构图"

        Private Function BuildGraph(parameters As ConvVaeParameters, options As RgbVaeExportOptions) As GraphProto
            Dim height As Integer = parameters.Height
            Dim width As Integer = parameters.Width
            Dim channels As Integer = parameters.Channels
            Dim latentDim As Integer = parameters.LatentDim
            Dim filter1 As Integer = parameters.Filter1
            Dim filter2 As Integer = parameters.Filter2
            Dim reducedHeight As Integer = height \ 4
            Dim reducedWidth As Integer = width \ 4
            Dim flattened As Integer = parameters.FlattenedSize
            Dim elemType As TensorDataType = options.WeightPrecision
            Dim graph As New GraphProto With {.Name = options.ModelName}

            Call graph.Input.Add(ValueInfo(GraphInputImage, elemType, {1, channels, height, width}, options.BatchDimension))
            Call graph.Input.Add(ValueInfo(GraphInputLatent, elemType, {1, latentDim}, options.BatchDimension))

            Call WriteEncoderWeights(graph, parameters, options)
            Call WriteGeneratorWeights(graph, parameters, options)

            ' ---------------- 编码器 ----------------
            Call AddNode(graph, "Conv", "enc_conv1",
                         {GraphInputImage, InitEncConv1Weight, InitEncConv1Bias},
                         {"enc_conv1_out"},
                         IntAttribute("kernel_shape", 3, 3),
                         IntAttribute("strides", 2, 2),
                         IntAttribute("pads", 1, 1, 1, 1))

            Call AddNode(graph, "Relu", "enc_relu1", {"enc_conv1_out"}, {"enc_relu1_out"})

            Call AddNode(graph, "Conv", "enc_conv2",
                         {"enc_relu1_out", InitEncConv2Weight, InitEncConv2Bias},
                         {"enc_conv2_out"},
                         IntAttribute("kernel_shape", 3, 3),
                         IntAttribute("strides", 2, 2),
                         IntAttribute("pads", 1, 1, 1, 1))

            Call AddNode(graph, "Relu", "enc_relu2", {"enc_conv2_out"}, {"enc_relu2_out"})

            ' NCHW -> NHWC：让展平顺序变成 (y, x, c)，与 Dense 权重的行顺序一致
            Call AddNode(graph, "Transpose", "enc_nhwc", {"enc_relu2_out"}, {"enc_nhwc_out"},
                         IntAttribute("perm", 0, 2, 3, 1))

            Call AddNode(graph, "Flatten", "enc_flatten", {"enc_nhwc_out"}, {"enc_flatten_out"},
                         SingleIntAttribute("axis", 1))

            Call AddNode(graph, "Gemm", "enc_mean",
                         {"enc_flatten_out", InitEncMeanWeight, InitEncMeanBias},
                         {GraphOutputMean},
                         SingleIntAttribute("transB", 1))

            Call AddNode(graph, "Gemm", "enc_logvar",
                         {"enc_flatten_out", InitEncLogVarWeight, InitEncLogVarBias},
                         {GraphOutputLogVar},
                         SingleIntAttribute("transB", 1))

            ' ---------------- 生成器 ----------------
            Call AddNode(graph, "Gemm", "gen_expand",
                         {GraphInputLatent, InitGenExpandWeight, InitGenExpandBias},
                         {"gen_expand_out"},
                         SingleIntAttribute("transB", 1))

            ' 展平特征按 (y, x, c) 排列，先还原成 NHWC，再转成卷积要求的 NCHW
            Call AddNode(graph, "Reshape", "gen_reshape",
                         {"gen_expand_out", InitGenReshapeShape},
                         {"gen_reshape_out"})

            Call AddNode(graph, "Transpose", "gen_nchw", {"gen_reshape_out"}, {"gen_nchw_out"},
                         IntAttribute("perm", 0, 3, 1, 2))

            Call AddNode(graph, "Relu", "gen_relu0", {"gen_nchw_out"}, {"gen_relu0_out"})

            Call AddNode(graph, "ConvTranspose", "gen_up1",
                         {"gen_relu0_out", InitGenUp1Weight},
                         {"gen_up1_out"},
                         IntAttribute("kernel_shape", 2, 2),
                         IntAttribute("strides", 2, 2),
                         IntAttribute("pads", 0, 0, 0, 0))

            Call AddNode(graph, "Conv", "gen_conv1",
                         {"gen_up1_out", InitGenConv1Weight, InitGenConv1Bias},
                         {"gen_conv1_out"},
                         IntAttribute("kernel_shape", 3, 3),
                         IntAttribute("strides", 1, 1),
                         IntAttribute("pads", 1, 1, 1, 1))

            Call AddNode(graph, "Relu", "gen_relu1", {"gen_conv1_out"}, {"gen_relu1_out"})

            Call AddNode(graph, "ConvTranspose", "gen_up2",
                         {"gen_relu1_out", InitGenUp2Weight},
                         {"gen_up2_out"},
                         IntAttribute("kernel_shape", 2, 2),
                         IntAttribute("strides", 2, 2),
                         IntAttribute("pads", 0, 0, 0, 0))

            Call AddNode(graph, "Conv", "gen_conv2",
                         {"gen_up2_out", InitGenConv2Weight, InitGenConv2Bias},
                         {"gen_conv2_out"},
                         IntAttribute("kernel_shape", 3, 3),
                         IntAttribute("strides", 1, 1),
                         IntAttribute("pads", 1, 1, 1, 1))

            Call AddNode(graph, "Sigmoid", "gen_sigmoid", {"gen_conv2_out"}, {GraphOutputImage})

            Call graph.Output.Add(ValueInfo(GraphOutputMean, elemType, {1, latentDim}, options.BatchDimension))
            Call graph.Output.Add(ValueInfo(GraphOutputLogVar, elemType, {1, latentDim}, options.BatchDimension))
            Call graph.Output.Add(ValueInfo(GraphOutputImage, elemType, {1, channels, height, width}, options.BatchDimension))

            Call System.Diagnostics.Debug.Assert(reducedHeight * reducedWidth * filter2 = flattened,
                                                 "展平长度必须与 reshape 目标一致")
            Call System.Diagnostics.Debug.Assert(reducedHeight * 4 = height AndAlso reducedWidth * 4 = width,
                                                 "两级上采样必须还原原图尺寸")

            Return graph
        End Function

        ''' <summary>写出编码器分支的全部初始化器。</summary>
        Private Sub WriteEncoderWeights(graph As GraphProto, parameters As ConvVaeParameters, options As RgbVaeExportOptions)
            Dim channels As Integer = parameters.Channels
            Dim filter1 As Integer = parameters.Filter1
            Dim filter2 As Integer = parameters.Filter2
            Dim latentDim As Integer = parameters.LatentDim
            Dim flattened As Integer = parameters.FlattenedSize

            Dim conv1 As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyEncFilter1, {3, 3, channels, filter1})
            Dim conv1Bias As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyEncBias1, {filter1})
            Dim conv2 As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyEncFilter2, {3, 3, filter1, filter2})
            Dim conv2Bias As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyEncBias2, {filter2})
            Dim meanWeight As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyMeanWeight, {flattened, latentDim})
            Dim meanBias As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyMeanBias, {1, latentDim})
            Dim logVarWeight As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyLogVarWeight, {flattened, latentDim})
            Dim logVarBias As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyLogVarBias, {1, latentDim})

            Call graph.Initializer.Add(ToInitializer(InitEncConv1Weight,
                ConvWeightsToOnnx(conv1), {filter1, channels, 3, 3}, options))
            Call graph.Initializer.Add(ToInitializer(InitEncConv1Bias, conv1Bias, options))
            Call graph.Initializer.Add(ToInitializer(InitEncConv2Weight,
                ConvWeightsToOnnx(conv2), {filter2, filter1, 3, 3}, options))
            Call graph.Initializer.Add(ToInitializer(InitEncConv2Bias, conv2Bias, options))

            ' Dense 的偏置在框架里是 [1, fanOut]，Gemm 的 C 必须是一维 [fanOut]
            Call graph.Initializer.Add(ToInitializer(InitEncMeanWeight,
                DenseWeightsToOnnx(meanWeight), {latentDim, flattened}, options))
            Call graph.Initializer.Add(ToInitializer(InitEncMeanBias, meanBias.Data, {latentDim}, options))
            Call graph.Initializer.Add(ToInitializer(InitEncLogVarWeight,
                DenseWeightsToOnnx(logVarWeight), {latentDim, flattened}, options))
            Call graph.Initializer.Add(ToInitializer(InitEncLogVarBias, logVarBias.Data, {latentDim}, options))
        End Sub

        ''' <summary>写出生成器分支的全部初始化器。</summary>
        Private Sub WriteGeneratorWeights(graph As GraphProto, parameters As ConvVaeParameters, options As RgbVaeExportOptions)
            Dim channels As Integer = parameters.Channels
            Dim filter1 As Integer = parameters.Filter1
            Dim filter2 As Integer = parameters.Filter2
            Dim latentDim As Integer = parameters.LatentDim
            Dim flattened As Integer = parameters.FlattenedSize
            Dim reducedHeight As Integer = parameters.Height \ 4
            Dim reducedWidth As Integer = parameters.Width \ 4

            Dim expandWeight As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyExpandWeight, {latentDim, flattened})
            Dim expandBias As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyExpandBias, {1, flattened})
            Dim midFilter As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyMidFilter, {3, 3, filter2, filter1})
            Dim midBias As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyMidBias, {filter1})
            Dim outFilter As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyOutFilter, {3, 3, filter1, channels})
            Dim outBias As Tensor = parameters.RequireTensor(ConvVaeParameters.KeyOutBias, {channels})

            Call graph.Initializer.Add(ToInitializer(InitGenExpandWeight,
                DenseWeightsToOnnx(expandWeight), {flattened, latentDim}, options))
            Call graph.Initializer.Add(ToInitializer(InitGenExpandBias, expandBias.Data, {flattened}, options))

            ' reshape 的目标形状：batch 维用 0 表示「沿用输入」
            Call graph.Initializer.Add(ToInt64Initializer(InitGenReshapeShape,
                {0L, reducedHeight, reducedWidth, filter2}))

            ' 上采样核是固定常量：2×2 的单位阵，每个通道只映射到自身
            Call graph.Initializer.Add(ToInitializer(InitGenUp1Weight,
                UpsampleKernel(filter2), {filter2, filter2, 2, 2}, options))
            Call graph.Initializer.Add(ToInitializer(InitGenConv1Weight,
                ConvWeightsToOnnx(midFilter), {filter1, filter2, 3, 3}, options))
            Call graph.Initializer.Add(ToInitializer(InitGenConv1Bias, midBias, options))
            Call graph.Initializer.Add(ToInitializer(InitGenUp2Weight,
                UpsampleKernel(filter1), {filter1, filter1, 2, 2}, options))
            Call graph.Initializer.Add(ToInitializer(InitGenConv2Weight,
                ConvWeightsToOnnx(outFilter), {channels, filter1, 3, 3}, options))
            Call graph.Initializer.Add(ToInitializer(InitGenConv2Bias, outBias, options))
        End Sub

#End Region

    End Module

End Namespace
