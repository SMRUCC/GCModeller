' ============================================================================
' ConvVAE.Parameters.vb - 卷积 VAE 的参数快照（Partial 扩展）
'
' 为什么需要这一层：
'   ONNX 之类的持久化格式必须能直接读写模型权重，而 ConvVAE 的权重字段全部是
'   私有的。与其把训练/推理实现复制到下游项目，不如在这里开放一个受控的
'   参数快照：下游只依赖「超参 + 命名张量」这一稳定界面。
'
' 零破坏约定：
'   ConvVAE.vb 只把类声明改成 Partial，既有成员签名、Fit/Save/TryLoad 行为一行不改；
'   本文件只新增成员。键名与 ConvVAE.Save() 使用的键完全一致，因此同一个
'   ConvVaeParameters 既能喂给二进制格式，也能喂给 ONNX 导出器。
' ============================================================================

Imports System.IO
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace Imaging

    ''' <summary>
    ''' <see cref="ConvVAE"/> 的超参数与权重快照。
    ''' </summary>
    ''' <remarks>
    ''' <see cref="Tensors"/> 里存放的是**模型内部的张量引用**（不复制数据），
    ''' 因此既可以直接读出权重，也可以把外部张量写回模型。
    ''' </remarks>
    Public Class ConvVaeParameters

        ''' <summary>编码器第一层卷积核。</summary>
        Public Const KeyEncFilter1 As String = "encF1"

        ''' <summary>编码器第一层偏置。</summary>
        Public Const KeyEncBias1 As String = "encB1"

        ''' <summary>编码器第二层卷积核。</summary>
        Public Const KeyEncFilter2 As String = "encF2"

        ''' <summary>编码器第二层偏置。</summary>
        Public Const KeyEncBias2 As String = "encB2"

        ''' <summary>解码器中间层卷积核。</summary>
        Public Const KeyMidFilter As String = "midF"

        ''' <summary>解码器中间层偏置。</summary>
        Public Const KeyMidBias As String = "midB"

        ''' <summary>解码器输出层卷积核。</summary>
        Public Const KeyOutFilter As String = "outF"

        ''' <summary>解码器输出层偏置。</summary>
        Public Const KeyOutBias As String = "outB"

        ''' <summary>均值分支的权重。</summary>
        Public Const KeyMeanWeight As String = "mean_W"

        ''' <summary>均值分支的偏置。</summary>
        Public Const KeyMeanBias As String = "mean_b"

        ''' <summary>对数方差分支的权重。</summary>
        Public Const KeyLogVarWeight As String = "lv_W"

        ''' <summary>对数方差分支的偏置。</summary>
        Public Const KeyLogVarBias As String = "lv_b"

        ''' <summary>解码器展开层的权重。</summary>
        Public Const KeyExpandWeight As String = "expand_W"

        ''' <summary>解码器展开层的偏置。</summary>
        Public Const KeyExpandBias As String = "expand_b"

        ''' <summary>全部参数张量的键名（顺序与网络前向顺序一致）。</summary>
        Public Shared ReadOnly TensorKeys As String() = {
            KeyEncFilter1, KeyEncBias1, KeyEncFilter2, KeyEncBias2,
            KeyMidFilter, KeyMidBias, KeyOutFilter, KeyOutBias,
            KeyMeanWeight, KeyMeanBias, KeyLogVarWeight, KeyLogVarBias,
            KeyExpandWeight, KeyExpandBias
        }

        ''' <summary>图像高度。</summary>
        Public Property Height As Integer

        ''' <summary>图像宽度。</summary>
        Public Property Width As Integer

        ''' <summary>通道数（RGB 为 3）。</summary>
        Public Property Channels As Integer = 1

        ''' <summary>潜变量维度。</summary>
        Public Property LatentDim As Integer

        ''' <summary>第一层卷积的输出通道数。</summary>
        Public Property Filter1 As Integer = 16

        ''' <summary>第二层卷积的输出通道数。</summary>
        Public Property Filter2 As Integer = 32

        ''' <summary>命名权重张量；键名见 <see cref="TensorKeys"/>。</summary>
        Public Property Tensors As New Dictionary(Of String, Tensor)(StringComparer.Ordinal)

        ''' <summary>展平后的特征长度。</summary>
        Public ReadOnly Property FlattenedSize As Integer
            Get
                Return (Height \ 4) * (Width \ 4) * Filter2
            End Get
        End Property

        ''' <summary>取出张量；不存在时返回 <c>Nothing</c>。</summary>
        Public Function GetTensor(name As String) As Tensor
            Dim value As Tensor = Nothing

            If Tensors.TryGetValue(name, value) Then
                Return value
            End If

            Return Nothing
        End Function

        ''' <summary>
        ''' 取出张量并校验形状；缺失或形状不符时抛出 <see cref="InvalidDataException"/>。
        ''' </summary>
        Public Function RequireTensor(name As String, expectedShape As Integer()) As Tensor
            Dim value As Tensor = GetTensor(name)

            If value Is Nothing Then
                Throw New InvalidDataException($"参数快照缺少张量 '{name}'")
            End If

            If Not ShapeEquals(value.Shape, expectedShape) Then
                Throw New InvalidDataException(
                    $"张量 '{name}' 的形状不符：期望 [{String.Join(", ", expectedShape)}]，" &
                    $"实际 [{String.Join(", ", value.Shape)}]")
            End If

            Return value
        End Function

        ''' <summary>比较两个形状是否完全一致。</summary>
        Public Shared Function ShapeEquals(actual As Integer(), expected As Integer()) As Boolean
            If actual Is Nothing OrElse expected Is Nothing Then
                Return False
            End If

            If actual.Length <> expected.Length Then
                Return False
            End If

            For i As Integer = 0 To actual.Length - 1
                If actual(i) <> expected(i) Then
                    Return False
                End If
            Next

            Return True
        End Function

        ''' <summary>返回 <c>ConvVaeParameters(HxWxC, latent=...)</c> 形式的文本。</summary>
        Public Overrides Function ToString() As String
            Return $"ConvVaeParameters({Height}x{Width}x{Channels}, latent={LatentDim}, filters={Filter1}/{Filter2})"
        End Function

    End Class

    Partial Public Class ConvVAE

        ''' <summary>第一层卷积的输出通道数。</summary>
        Public ReadOnly Property Filter1 As Integer
            Get
                Return _filter1
            End Get
        End Property

        ''' <summary>第二层卷积的输出通道数。</summary>
        Public ReadOnly Property Filter2 As Integer
            Get
                Return _filter2
            End Get
        End Property

        ''' <summary>两级 stride-2 卷积之后的特征图高度。</summary>
        Public ReadOnly Property ReducedHeight As Integer
            Get
                Return _height \ 4
            End Get
        End Property

        ''' <summary>两级 stride-2 卷积之后的特征图宽度。</summary>
        Public ReadOnly Property ReducedWidth As Integer
            Get
                Return _width \ 4
            End Get
        End Property

        ''' <summary>展平后的特征长度。</summary>
        Public ReadOnly Property FlattenedSize As Integer
            Get
                Return ReducedHeight * ReducedWidth * _filter2
            End Get
        End Property

        ''' <summary>
        ''' 导出超参数与权重张量的快照（张量是模型内部的引用，不做拷贝）。
        ''' </summary>
        Public Function ExportParameters() As ConvVaeParameters
            Dim snapshot As New ConvVaeParameters With {
                .Height = _height,
                .Width = _width,
                .Channels = _channels,
                .LatentDim = _latentDim,
                .Filter1 = _filter1,
                .Filter2 = _filter2
            }

            Call snapshot.Tensors.Add(ConvVaeParameters.KeyEncFilter1, _encFilter1)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyEncBias1, _encBias1)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyEncFilter2, _encFilter2)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyEncBias2, _encBias2)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyMidFilter, _midFilter)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyMidBias, _midBias)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyOutFilter, _outFilter)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyOutBias, _outBias)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyMeanWeight, _meanLayer.Weights)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyMeanBias, _meanLayer.Bias)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyLogVarWeight, _logVarLayer.Weights)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyLogVarBias, _logVarLayer.Bias)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyExpandWeight, _expandLayer.Weights)
            Call snapshot.Tensors.Add(ConvVaeParameters.KeyExpandBias, _expandLayer.Bias)

            Return snapshot
        End Function

        ''' <summary>
        ''' 用参数快照重建网络：先按快照超参重新初始化结构，再逐张量写入权重。
        ''' </summary>
        ''' <remarks>
        ''' 载入成功后模型进入「已训练」状态，但 Adam 的动量状态会被清空
        ''' （旧状态与新的权重形状不再对应）。任一形状不符都会在破坏当前模型之前抛出
        ''' <see cref="InvalidDataException"/>。
        ''' </remarks>
        Public Sub ImportParameters(parameters As ConvVaeParameters)
            If parameters Is Nothing Then
                Throw New ArgumentNullException(NameOf(parameters))
            End If

            ValidateSnapshot(parameters)

            _height = parameters.Height
            _width = parameters.Width
            _channels = parameters.Channels
            _latentDim = parameters.LatentDim
            _filter1 = parameters.Filter1
            _filter2 = parameters.Filter2

            Call InitializeNetwork()
            Call _optimizers.Clear()
            _loss.Clear()

            Call LoadInto(_encFilter1, parameters, ConvVaeParameters.KeyEncFilter1)
            Call LoadInto(_encBias1, parameters, ConvVaeParameters.KeyEncBias1)
            Call LoadInto(_encFilter2, parameters, ConvVaeParameters.KeyEncFilter2)
            Call LoadInto(_encBias2, parameters, ConvVaeParameters.KeyEncBias2)
            Call LoadInto(_midFilter, parameters, ConvVaeParameters.KeyMidFilter)
            Call LoadInto(_midBias, parameters, ConvVaeParameters.KeyMidBias)
            Call LoadInto(_outFilter, parameters, ConvVaeParameters.KeyOutFilter)
            Call LoadInto(_outBias, parameters, ConvVaeParameters.KeyOutBias)
            Call LoadInto(_meanLayer.Weights, parameters, ConvVaeParameters.KeyMeanWeight)
            Call LoadInto(_meanLayer.Bias, parameters, ConvVaeParameters.KeyMeanBias)
            Call LoadInto(_logVarLayer.Weights, parameters, ConvVaeParameters.KeyLogVarWeight)
            Call LoadInto(_logVarLayer.Bias, parameters, ConvVaeParameters.KeyLogVarBias)
            Call LoadInto(_expandLayer.Weights, parameters, ConvVaeParameters.KeyExpandWeight)
            Call LoadInto(_expandLayer.Bias, parameters, ConvVaeParameters.KeyExpandBias)

            _fitted = True
        End Sub

        ''' <summary>校验快照的超参与每个张量的形状，任何不一致都在修改模型之前报错。</summary>
        Private Shared Sub ValidateSnapshot(parameters As ConvVaeParameters)
            If parameters.Height <= 0 OrElse parameters.Width <= 0 Then
                Throw New InvalidDataException($"参数快照中的图像尺寸非法：{parameters.Height}x{parameters.Width}")
            End If

            If parameters.Height Mod 4 <> 0 OrElse parameters.Width Mod 4 <> 0 Then
                Throw New InvalidDataException(
                    $"参数快照中的图像尺寸必须是 4 的倍数，实际 {parameters.Height}x{parameters.Width}")
            End If

            If parameters.Channels <= 0 OrElse parameters.LatentDim <= 0 OrElse
               parameters.Filter1 <= 0 OrElse parameters.Filter2 <= 0 Then

                Throw New InvalidDataException(
                    $"参数快照中的通道 / 潜变量 / 卷积宽度必须为正：channels={parameters.Channels}, " &
                    $"latent={parameters.LatentDim}, filters={parameters.Filter1}/{parameters.Filter2}")
            End If

            Dim channels As Integer = parameters.Channels
            Dim filter1 As Integer = parameters.Filter1
            Dim filter2 As Integer = parameters.Filter2
            Dim latent As Integer = parameters.LatentDim
            Dim flattened As Integer = parameters.FlattenedSize

            Call parameters.RequireTensor(ConvVaeParameters.KeyEncFilter1, {3, 3, channels, filter1})
            Call parameters.RequireTensor(ConvVaeParameters.KeyEncBias1, {filter1})
            Call parameters.RequireTensor(ConvVaeParameters.KeyEncFilter2, {3, 3, filter1, filter2})
            Call parameters.RequireTensor(ConvVaeParameters.KeyEncBias2, {filter2})
            Call parameters.RequireTensor(ConvVaeParameters.KeyMidFilter, {3, 3, filter2, filter1})
            Call parameters.RequireTensor(ConvVaeParameters.KeyMidBias, {filter1})
            Call parameters.RequireTensor(ConvVaeParameters.KeyOutFilter, {3, 3, filter1, channels})
            Call parameters.RequireTensor(ConvVaeParameters.KeyOutBias, {channels})
            Call parameters.RequireTensor(ConvVaeParameters.KeyMeanWeight, {flattened, latent})
            Call parameters.RequireTensor(ConvVaeParameters.KeyMeanBias, {1, latent})
            Call parameters.RequireTensor(ConvVaeParameters.KeyLogVarWeight, {flattened, latent})
            Call parameters.RequireTensor(ConvVaeParameters.KeyLogVarBias, {1, latent})
            Call parameters.RequireTensor(ConvVaeParameters.KeyExpandWeight, {latent, flattened})
            Call parameters.RequireTensor(ConvVaeParameters.KeyExpandBias, {1, flattened})
        End Sub

        ''' <summary>把快照中的张量复制进模型的参数张量。</summary>
        Private Shared Sub LoadInto(target As Tensor, parameters As ConvVaeParameters, name As String)
            Dim source As Tensor = parameters.RequireTensor(name, target.Shape)

            Call Array.Copy(source.Data, target.Data, target.Data.Length)
            Call target.MarkHostModified()
        End Sub

    End Class

End Namespace
