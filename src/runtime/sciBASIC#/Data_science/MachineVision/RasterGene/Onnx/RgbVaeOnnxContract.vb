' ============================================================================
' RgbVaeOnnxContract.vb - RGB 生成模型的 ONNX 约定与布局换算
'
' 导出端与回读端共用这一份约定，避免两边的名字 / 布局 / 形状口径漂移：
'   * 初始化器命名（enc.conv1.weight … gen.up2.weight）
'   * 图输入输出命名（image / latent / mu / logvar / image_out）
'   * metadata 键（超参数，回读时用来重建同结构模型）
'   * 框架内部布局 ↔ ONNX 标准布局的权重换算
'
' 布局约定：
'   框架内部一律 channel-last：[N, H, W, C]、卷积核 [kH, kW, Cin, Cout]；
'   导出的 ONNX 图对外一律 channel-first：[N, C, H, W]，卷积核 [Cout, Cin, kH, kW]，
'   展平前插入 Transpose[0,2,3,1]，使展平顺序与 Dense 权重的行顺序严格一致。
' ============================================================================

Imports System.Text
Imports Microsoft.VisualBasic.MachineLearning.DataStorage.ONNX
Imports Microsoft.VisualBasic.MachineLearning.DataStorage.ONNX.Proto
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Imaging
Imports std = System.Math

Namespace Serialization

    ''' <summary>RGB 生成模型的 ONNX 导出选项。</summary>
    Public Class RgbVaeExportOptions

        ''' <summary>目标算子集版本（默认 13）。</summary>
        Public Property Opset As Long = 13

        ''' <summary>目标 IR 版本（默认 7）。</summary>
        Public Property IrVersion As Long = 7

        ''' <summary>
        ''' 权重落盘精度。默认单精度（与主流推理框架一致）；
        ''' 需要严格回归时改成 <see cref="TensorDataType.Float64"/>。
        ''' </summary>
        Public Property WeightPrecision As TensorDataType = TensorDataType.Float

        ''' <summary>生成该模型的工具名称。</summary>
        Public Property ProducerName As String = "sciBASIC.RasterGene"

        ''' <summary>生成该模型的工具版本。</summary>
        Public Property ProducerVersion As String = "1.0"

        ''' <summary>计算图名称。</summary>
        Public Property ModelName As String = "rastergene-rgb-vae"

        ''' <summary>动态批维的符号名；设为空字符串表示把批维固定写出为 1。</summary>
        Public Property BatchDimension As String = "N"

    End Class

    ''' <summary>
    ''' RGB 生成模型的 ONNX 命名、元数据与权重布局换算。
    ''' </summary>
    Public Module RgbVaeOnnxContract

        ''' <summary>metadata 中标识模型种类的值。</summary>
        Public Const ModelKind As String = "RgbGenerativeVae"

        ''' <summary>当前导出格式的结构版本。</summary>
        Public Const SchemaVersion As Integer = 1

        ''' <summary>metadata 键：模型种类。</summary>
        Public Const MetaKind As String = "rastergene.kind"

        ''' <summary>metadata 键：结构版本。</summary>
        Public Const MetaSchema As String = "rastergene.schema"

        ''' <summary>metadata 键：图像高度。</summary>
        Public Const MetaHeight As String = "rastergene.height"

        ''' <summary>metadata 键：图像宽度。</summary>
        Public Const MetaWidth As String = "rastergene.width"

        ''' <summary>metadata 键：通道数。</summary>
        Public Const MetaChannels As String = "rastergene.channels"

        ''' <summary>metadata 键：潜变量维度。</summary>
        Public Const MetaLatentDim As String = "rastergene.latentDim"

        ''' <summary>metadata 键：第一层卷积宽度。</summary>
        Public Const MetaFilter1 As String = "rastergene.filter1"

        ''' <summary>metadata 键：第二层卷积宽度。</summary>
        Public Const MetaFilter2 As String = "rastergene.filter2"

        ''' <summary>metadata 键：图像张量布局。</summary>
        Public Const MetaLayout As String = "rastergene.layout"

        ''' <summary>metadata 键：输出取值语义（伯努利概率）。</summary>
        Public Const MetaOutput As String = "rastergene.output"

        ' ---- 图输入 / 输出 ----

        ''' <summary>编码器输入（NCHW 图像）。</summary>
        Public Const GraphInputImage As String = "image"

        ''' <summary>生成器输入（潜变量）。</summary>
        Public Const GraphInputLatent As String = "latent"

        ''' <summary>编码器输出：后验均值。</summary>
        Public Const GraphOutputMean As String = "mu"

        ''' <summary>编码器输出：后验对数方差（已按框架的数值范围截断前的原始值）。</summary>
        Public Const GraphOutputLogVar As String = "logvar"

        ''' <summary>生成器输出（NCHW 图像，取值 [0, 1]）。</summary>
        Public Const GraphOutputImage As String = "image_out"

        ' ---- 初始化器名称 ----

        Public Const InitEncConv1Weight As String = "enc.conv1.weight"
        Public Const InitEncConv1Bias As String = "enc.conv1.bias"
        Public Const InitEncConv2Weight As String = "enc.conv2.weight"
        Public Const InitEncConv2Bias As String = "enc.conv2.bias"
        Public Const InitEncMeanWeight As String = "enc.mean.weight"
        Public Const InitEncMeanBias As String = "enc.mean.bias"
        Public Const InitEncLogVarWeight As String = "enc.logvar.weight"
        Public Const InitEncLogVarBias As String = "enc.logvar.bias"

        Public Const InitGenExpandWeight As String = "gen.expand.weight"
        Public Const InitGenExpandBias As String = "gen.expand.bias"
        Public Const InitGenReshapeShape As String = "gen.reshape.shape"
        Public Const InitGenUp1Weight As String = "gen.up1.weight"
        Public Const InitGenConv1Weight As String = "gen.conv1.weight"
        Public Const InitGenConv1Bias As String = "gen.conv1.bias"
        Public Const InitGenUp2Weight As String = "gen.up2.weight"
        Public Const InitGenConv2Weight As String = "gen.conv2.weight"
        Public Const InitGenConv2Bias As String = "gen.conv2.bias"

        ''' <summary>导出时必须写出的全部初始化器名称。</summary>
        Public ReadOnly Property RequiredInitializers As String()
            Get
                Return New String() {
                    InitEncConv1Weight, InitEncConv1Bias, InitEncConv2Weight, InitEncConv2Bias,
                    InitEncMeanWeight, InitEncMeanBias, InitEncLogVarWeight, InitEncLogVarBias,
                    InitGenExpandWeight, InitGenExpandBias, InitGenReshapeShape,
                    InitGenUp1Weight, InitGenConv1Weight, InitGenConv1Bias,
                    InitGenUp2Weight, InitGenConv2Weight, InitGenConv2Bias
                }
            End Get
        End Property

#Region "图结构辅助"

        ''' <summary>构造一个带动态批维的张量类型描述。</summary>
        ''' <param name="name">张量名称。</param>
        ''' <param name="elemType">元素类型。</param>
        ''' <param name="dims">各维度长度；第 0 维在 <paramref name="batchName"/> 非空时被忽略。</param>
        ''' <param name="batchName">动态批维的符号名。</param>
        Public Function ValueInfo(name As String, elemType As TensorDataType, dims As Integer(),
                                  Optional batchName As String = Nothing) As ValueInfoProto
            Dim shape As New TensorShapeProto()

            For i As Integer = 0 To dims.Length - 1
                If i = 0 AndAlso Not String.IsNullOrEmpty(batchName) Then
                    Call shape.Dims.Add(New TensorShapeDimension With {.DimParam = batchName})
                Else
                    Call shape.Dims.Add(New TensorShapeDimension With {.DimValue = dims(i)})
                End If
            Next

            Return New ValueInfoProto With {
                .Name = name,
                .Type = New TypeProto With {
                    .TensorType = New TensorTypeInfo With {
                        .ElemType = elemType,
                        .Shape = shape
                    }
                }
            }
        End Function

        ''' <summary>构造一个整数数组属性。</summary>
        Public Function IntAttribute(name As String, ParamArray values As Integer()) As AttributeProto
            Dim attr As New AttributeProto With {
                .Name = name,
                .Type = AttributeType.Ints
            }

            For Each value As Integer In values
                Call attr.Ints.Add(value)
            Next

            Return attr
        End Function

        ''' <summary>构造一个整数属性。</summary>
        Public Function SingleIntAttribute(name As String, value As Long) As AttributeProto
            Return New AttributeProto With {
                .Name = name,
                .I = value,
                .Type = AttributeType.Int
            }
        End Function

        ''' <summary>追加一个节点并返回它的输出张量名称。</summary>
        Public Function AddNode(graph As GraphProto, opType As String, name As String,
                                inputs As String(), outputs As String(),
                                ParamArray attributes As AttributeProto()) As NodeProto

            Dim node As New NodeProto With {
                .OpType = opType,
                .Name = name
            }

            For Each input As String In inputs
                Call node.Input.Add(input)
            Next

            For Each output As String In outputs
                Call node.Output.Add(output)
            Next

            If attributes IsNot Nothing Then
                For Each attribute As AttributeProto In attributes
                    Call node.Attribute.Add(attribute)
                Next
            End If

            Call graph.Node.Add(node)

            Return node
        End Function

        ''' <summary>
        ''' 把框架内的张量写成单精度 / 双精度初始化器（形状与数据顺序保持不变）。
        ''' </summary>
        Public Function ToInitializer(name As String, source As Tensor, options As RgbVaeExportOptions) As TensorProto
            Dim shape As Integer() = source.Shape
            Dim data As Double() = New Double(source.Data.Length - 1) {}

            Call Array.Copy(source.Data, data, data.Length)

            Return OnnxTensor.FromArray(data, shape).ToTensorProto(name, precision:=options.WeightPrecision)
        End Function

        ''' <summary>把一个一维浮点数组写成初始化器。</summary>
        Public Function ToInitializer(name As String, values As Double(), options As RgbVaeExportOptions) As TensorProto
            Return OnnxTensor.FromArray(values, values.Length).ToTensorProto(name, precision:=options.WeightPrecision)
        End Function

        ''' <summary>把一段浮点数据按指定形状写成初始化器。</summary>
        Public Function ToInitializer(name As String, values As Double(), dims As Integer(),
                                      options As RgbVaeExportOptions) As TensorProto
            Return OnnxTensor.FromArray(values, dims).ToTensorProto(name, precision:=options.WeightPrecision)
        End Function

        ''' <summary>构造一个 int64 初始化器（用于 Reshape 的目标形状）。</summary>
        Public Function ToInt64Initializer(name As String, values As Long()) As TensorProto
            Dim proto As New TensorProto With {
                .Name = name,
                .DataType = TensorDataType.Int64
            }

            Call proto.Dims.Add(values.Length)

            For Each value As Long In values
                Call proto.Int64Data.Add(value)
            Next

            Return proto
        End Function

#End Region

#Region "权重布局换算"

        ''' <summary>
        ''' 卷积核：框架 <c>[kH, kW, Cin, Cout]</c> → ONNX Conv <c>[Cout, Cin, kH, kW]</c>。
        ''' </summary>
        Public Function ConvWeightsToOnnx(source As Tensor) As Double()
            Dim shape As Integer() = source.Shape
            Dim kernelHeight As Integer = shape(0)
            Dim kernelWidth As Integer = shape(1)
            Dim inChannels As Integer = shape(2)
            Dim outChannels As Integer = shape(3)
            Dim result As Double() = New Double(outChannels * inChannels * kernelHeight * kernelWidth - 1) {}
            Dim data As Double() = source.Data

            For co As Integer = 0 To outChannels - 1
                For ci As Integer = 0 To inChannels - 1
                    For kh As Integer = 0 To kernelHeight - 1
                        For kw As Integer = 0 To kernelWidth - 1
                            result(((co * inChannels + ci) * kernelHeight + kh) * kernelWidth + kw) =
                                data(((kh * kernelWidth + kw) * inChannels + ci) * outChannels + co)
                        Next
                    Next
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 转置卷积核：框架 <c>[kH, kW, Cout, Cin]</c> → ONNX ConvTranspose <c>[Cin, Cout, kH, kW]</c>。
        ''' </summary>
        ''' <remarks>
        ''' 框架里的转置卷积（上采样）核把「输出通道」放在第 3 维，与 ONNX ConvTranspose 的
        ''' <c>[Cin, Cout, kH, kW]</c> 顺序相反，这里一并完成转置。
        ''' </remarks>
        Public Function DeconvWeightsToOnnx(source As Tensor) As Double()
            Dim shape As Integer() = source.Shape
            Dim kernelHeight As Integer = shape(0)
            Dim kernelWidth As Integer = shape(1)
            Dim outChannels As Integer = shape(2)
            Dim inChannels As Integer = shape(3)
            Dim result As Double() = New Double(inChannels * outChannels * kernelHeight * kernelWidth - 1) {}
            Dim data As Double() = source.Data

            For ci As Integer = 0 To inChannels - 1
                For co As Integer = 0 To outChannels - 1
                    For kh As Integer = 0 To kernelHeight - 1
                        For kw As Integer = 0 To kernelWidth - 1
                            result(((ci * outChannels + co) * kernelHeight + kh) * kernelWidth + kw) =
                                data(((kh * kernelWidth + kw) * outChannels + co) * inChannels + ci)
                        Next
                    Next
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 全连接权重：框架 <c>[fanIn, fanOut]</c> → Gemm 的 <c>B = [fanOut, fanIn]</c>（配合 transB = 1）。
        ''' </summary>
        Public Function DenseWeightsToOnnx(source As Tensor) As Double()
            Dim shape As Integer() = source.Shape
            Dim fanIn As Integer = shape(0)
            Dim fanOut As Integer = shape(1)
            Dim result As Double() = New Double(fanIn * fanOut - 1) {}
            Dim data As Double() = source.Data

            For o As Integer = 0 To fanOut - 1
                For i As Integer = 0 To fanIn - 1
                    result(o * fanIn + i) = data(i * fanOut + o)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' ONNX Conv 权重 <c>[Cout, Cin, kH, kW]</c> → 框架卷积核 <c>[kH, kW, Cin, Cout]</c>。
        ''' </summary>
        Public Function ConvWeightsFromOnnx(weights As OnnxTensor) As Double()
            Dim dims As Integer() = weights.Dims

            If dims.Length <> 4 Then
                Throw New InvalidOperationException($"Conv 权重必须是 4 维，实际 [{String.Join(", ", dims)}]")
            End If

            Dim outChannels As Integer = dims(0)
            Dim inChannels As Integer = dims(1)
            Dim kernelHeight As Integer = dims(2)
            Dim kernelWidth As Integer = dims(3)
            Dim result As Double() = New Double(kernelHeight * kernelWidth * inChannels * outChannels - 1) {}
            Dim data As Double() = weights.Data

            For kh As Integer = 0 To kernelHeight - 1
                For kw As Integer = 0 To kernelWidth - 1
                    For ci As Integer = 0 To inChannels - 1
                        For co As Integer = 0 To outChannels - 1
                            result(((kh * kernelWidth + kw) * inChannels + ci) * outChannels + co) =
                                data(((co * inChannels + ci) * kernelHeight + kh) * kernelWidth + kw)
                        Next
                    Next
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' ONNX ConvTranspose 权重 <c>[Cin, Cout, kH, kW]</c> → 框架上采样核 <c>[kH, kW, Cout, Cin]</c>。
        ''' </summary>
        Public Function DeconvWeightsFromOnnx(weights As OnnxTensor) As Double()
            Dim dims As Integer() = weights.Dims

            If dims.Length <> 4 Then
                Throw New InvalidOperationException($"ConvTranspose 权重必须是 4 维，实际 [{String.Join(", ", dims)}]")
            End If

            Dim inChannels As Integer = dims(0)
            Dim outChannels As Integer = dims(1)
            Dim kernelHeight As Integer = dims(2)
            Dim kernelWidth As Integer = dims(3)
            Dim result As Double() = New Double(kernelHeight * kernelWidth * outChannels * inChannels - 1) {}
            Dim data As Double() = weights.Data

            For kh As Integer = 0 To kernelHeight - 1
                For kw As Integer = 0 To kernelWidth - 1
                    For co As Integer = 0 To outChannels - 1
                        For ci As Integer = 0 To inChannels - 1
                            result(((kh * kernelWidth + kw) * outChannels + co) * inChannels + ci) =
                                data(((ci * outChannels + co) * kernelHeight + kh) * kernelWidth + kw)
                        Next
                    Next
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' Gemm 权重 <c>[fanOut, fanIn]</c> → 框架全连接权重 <c>[fanIn, fanOut]</c>。
        ''' </summary>
        Public Function DenseWeightsFromOnnx(weights As OnnxTensor, expectedFanIn As Integer, expectedFanOut As Integer) As Double()
            Dim dims As Integer() = weights.Dims

            If dims.Length <> 2 Then
                Throw New InvalidOperationException($"Gemm 权重必须是 2 维，实际 [{String.Join(", ", dims)}]")
            End If

            If dims(0) <> expectedFanOut OrElse dims(1) <> expectedFanIn Then
                Throw New InvalidOperationException(
                    $"Gemm 权重形状不符：期望 [{expectedFanOut}, {expectedFanIn}]，实际 [{String.Join(", ", dims)}]")
            End If

            Dim result As Double() = New Double(expectedFanIn * expectedFanOut - 1) {}
            Dim data As Double() = weights.Data

            For i As Integer = 0 To expectedFanIn - 1
                For o As Integer = 0 To expectedFanOut - 1
                    result(i * expectedFanOut + o) = data(o * expectedFanIn + i)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 构造 2×2 的最近邻上采样核（ONNX ConvTranspose 用，形状 <c>[Cin, Cout, 2, 2]</c>）：
        ''' 每个通道映射到自身、权重全为 1，等价于框架的 <c>UpsampleKernel</c>。
        ''' </summary>
        Public Function UpsampleKernel(channels As Integer) As Double()
            Dim result As Double() = New Double(channels * channels * 4 - 1) {}

            For kh As Integer = 0 To 1
                For kw As Integer = 0 To 1
                    For c As Integer = 0 To channels - 1
                        result(((c * channels + c) * 2 + kh) * 2 + kw) = 1.0
                    Next
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 校验一个 <see cref="OnnxTensor"/> 是否就是「2×2 最近邻上采样」核。
        ''' </summary>
        ''' <remarks>
        ''' 回读时上采样核不参与参数还原（框架内它是固定常量），但仍需确认文件里写的
        ''' 确实是单位核，否则同一个 ONNX 文件的生成结果与框架结果不可能一致。
        ''' </remarks>
        Public Function IsUpsampleKernel(weights As OnnxTensor) As Boolean
            Dim dims As Integer() = weights.Dims

            If dims.Length <> 4 OrElse dims(2) <> 2 OrElse dims(3) <> 2 OrElse dims(0) <> dims(1) Then
                Return False
            End If

            Dim channels As Integer = dims(0)
            Dim expected As Double() = UpsampleKernel(channels)

            If weights.Data.Length <> expected.Length Then
                Return False
            End If

            For i As Integer = 0 To expected.Length - 1
                If std.Abs(weights.Data(i) - expected(i)) > 0.0000001 Then
                    Return False
                End If
            Next

            Return True
        End Function

#End Region

#Region "元数据"

        ''' <summary>把超参数写进模型级 metadata。</summary>
        Public Sub WriteMetadata(model As ModelProto, parameters As ConvVaeParameters, options As RgbVaeExportOptions)
            model.MetadataProps(MetaKind) = ModelKind
            model.MetadataProps(MetaSchema) = SchemaVersion.ToString()
            model.MetadataProps(MetaHeight) = parameters.Height.ToString()
            model.MetadataProps(MetaWidth) = parameters.Width.ToString()
            model.MetadataProps(MetaChannels) = parameters.Channels.ToString()
            model.MetadataProps(MetaLatentDim) = parameters.LatentDim.ToString()
            model.MetadataProps(MetaFilter1) = parameters.Filter1.ToString()
            model.MetadataProps(MetaFilter2) = parameters.Filter2.ToString()
            model.MetadataProps(MetaLayout) = "NCHW"
            model.MetadataProps(MetaOutput) = "bernoulli"
            model.MetadataProps("exported_by") = options.ProducerName
        End Sub

        ''' <summary>
        ''' 从模型 metadata 中读出超参数；任何一项缺失或非法都返回 <c>False</c> 并给出中文原因。
        ''' </summary>
        Public Function TryReadMetadata(model As OnnxModel, ByRef parameters As ConvVaeParameters,
                                        ByRef errorMessage As String) As Boolean
            parameters = Nothing
            errorMessage = Nothing

            If model Is Nothing Then
                errorMessage = "ONNX 模型为空"
                Return False
            End If

            Dim metadata As Dictionary(Of String, String) = model.Metadata

            If metadata Is Nothing Then
                errorMessage = "ONNX 模型缺少 metadata，无法确认它是不是 RGB 生成模型"
                Return False
            End If

            Dim kind As String = Nothing

            If Not metadata.TryGetValue(MetaKind, kind) OrElse Not String.Equals(kind, ModelKind, StringComparison.Ordinal) Then
                errorMessage = $"模型种类不符：期望 {ModelKind}，实际 {If(kind, "<缺失>")}"
                Return False
            End If

            Dim height As Integer
            Dim width As Integer
            Dim channels As Integer
            Dim latentDim As Integer
            Dim filter1 As Integer
            Dim filter2 As Integer

            If Not TryParse(metadata, MetaHeight, height) OrElse
               Not TryParse(metadata, MetaWidth, width) OrElse
               Not TryParse(metadata, MetaChannels, channels) OrElse
               Not TryParse(metadata, MetaLatentDim, latentDim) OrElse
               Not TryParse(metadata, MetaFilter1, filter1) OrElse
               Not TryParse(metadata, MetaFilter2, filter2) Then

                errorMessage = "ONNX 模型的 metadata 缺少或无法解析图像尺寸 / 潜变量 / 卷积宽度"
                Return False
            End If

            If height <= 0 OrElse width <= 0 OrElse latentDim <= 0 OrElse filter1 <= 0 OrElse filter2 <= 0 Then
                errorMessage = $"ONNX 模型的 metadata 含非法超参数：{height}x{width}x{channels}, latent={latentDim}, filters={filter1}/{filter2}"
                Return False
            End If

            If height Mod 4 <> 0 OrElse width Mod 4 <> 0 Then
                errorMessage = $"ONNX 模型的图像尺寸必须是 4 的倍数，实际 {height}x{width}"
                Return False
            End If

            parameters = New ConvVaeParameters With {
                .Height = height,
                .Width = width,
                .Channels = channels,
                .LatentDim = latentDim,
                .Filter1 = filter1,
                .Filter2 = filter2
            }

            Return True
        End Function

        Private Function TryParse(metadata As Dictionary(Of String, String), key As String, ByRef value As Integer) As Boolean
            Dim text As String = Nothing

            value = 0

            If Not metadata.TryGetValue(key, text) OrElse String.IsNullOrEmpty(text) Then
                Return False
            End If

            Return Integer.TryParse(text, value)
        End Function

#End Region

        ''' <summary>把导出建议写成一段可读文本（用于测试报告）。</summary>
        Public Function Describe(options As RgbVaeExportOptions) As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine($"opset = {options.Opset}, ir_version = {options.IrVersion}")
            Call sb.AppendLine($"weight precision = {options.WeightPrecision}")
            Call sb.AppendLine($"producer = {options.ProducerName} {options.ProducerVersion}")
            Call sb.AppendLine($"model name = {options.ModelName}")

            Return sb.ToString()
        End Function

    End Module

End Namespace
