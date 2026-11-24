' ============================================================================
' Generative.vb - RGB 图像生成模型（卷积变分自编码器）
'
' 这一层是「业务门面」：把 ConvVAE 的数学实现与图像语义（位图、预览、ONNX 持久化）
' 连接起来，自身不含任何前向 / 反向 / 优化器代码。
'
'   训练    Fit(images) ：[N, H, W, 3] ∈ [0, 1]
'   重构    Reconstruct ：使用后验均值，结果确定
'   生成    Generate    ：从标准正态先验采样
'   持久化  SaveOnnx / LoadOnnx ：单个 ONNX 文件保存双分支（编码器 + 生成器）
' ============================================================================

Imports System.Drawing
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Core
Imports Microsoft.VisualBasic.MachineLearning.VariationalAutoencoder.Imaging
Imports Microsoft.VisualBasic.MachineVision.RasterGenerative.Raster
Imports Microsoft.VisualBasic.MachineVision.RasterGenerative.Serialization
Imports std = System.Math

Namespace Global.Microsoft.VisualBasic.MachineVision.RasterGenerative

    ''' <summary>
    ''' 面向 RGB 位图的变分自编码生成模型。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 输入 / 输出统一为 channel-last 的 <c>[N, H, W, 3]</c> 张量，像素值落在 [0, 1]。
    ''' 图像尺寸必须是 4 的倍数（编码器含两级 stride-2 卷积）。
    ''' </para>
    ''' <para>
    ''' 训练、重构与采样的全部数学实现都在
    ''' <see cref="ConvVAE"/> 中，本类型只负责参数校验、图像编解码与持久化路由。
    ''' </para>
    ''' </remarks>
    Public Class RgbGenerativeVae
        Implements IGenerativeModel

        ''' <summary>默认的潜变量维度。</summary>
        Public Const DefaultLatentDim As Integer = 16

        ''' <summary>默认的第一层卷积宽度。</summary>
        Public Const DefaultFilter1 As Integer = 8

        ''' <summary>默认的第二层卷积宽度。</summary>
        Public Const DefaultFilter2 As Integer = 16

        Private ReadOnly _vae As ConvVAE

        ''' <summary>
        ''' 创建一个尚未训练的 RGB 生成模型。
        ''' </summary>
        ''' <param name="height">图像高度，必须是 4 的倍数。</param>
        ''' <param name="width">图像宽度，必须是 4 的倍数。</param>
        ''' <param name="latentDim">潜变量维度。</param>
        ''' <param name="filter1">第一层卷积的输出通道数。</param>
        ''' <param name="filter2">第二层卷积的输出通道数。</param>
        ''' <param name="seed">随机种子（用于可复现的初始化）。</param>
        Public Sub New(height As Integer, width As Integer,
                       Optional latentDim As Integer = DefaultLatentDim,
                       Optional filter1 As Integer = DefaultFilter1,
                       Optional filter2 As Integer = DefaultFilter2,
                       Optional seed As Integer? = Nothing)

            _vae = New ConvVAE(height, width,
                               channels:=RgbImageTensor.ColorChannels,
                               latentDim:=latentDim,
                               filter1:=filter1,
                               filter2:=filter2,
                               seed:=seed)
        End Sub

        ''' <summary>用参数快照重建模型（主要用于反序列化）。</summary>
        Private Sub New(parameters As ConvVaeParameters)
            _vae = New ConvVAE(parameters.Height, parameters.Width,
                               channels:=parameters.Channels,
                               latentDim:=parameters.LatentDim,
                               filter1:=parameters.Filter1,
                               filter2:=parameters.Filter2)
            _vae.ImportParameters(parameters)
        End Sub

#Region "模型信息"

        ''' <summary>图像高度。</summary>
        Public ReadOnly Property Height As Integer
            Get
                Return _vae.Height
            End Get
        End Property

        ''' <summary>图像宽度。</summary>
        Public ReadOnly Property Width As Integer
            Get
                Return _vae.Width
            End Get
        End Property

        ''' <summary>通道数（固定为 3）。</summary>
        Public ReadOnly Property Channels As Integer
            Get
                Return _vae.Channels
            End Get
        End Property

        ''' <summary>潜变量维度。</summary>
        Public ReadOnly Property LatentDim As Integer
            Get
                Return _vae.LatentDim
            End Get
        End Property

        ''' <summary>第一层卷积的输出通道数。</summary>
        Public ReadOnly Property Filter1 As Integer
            Get
                Return _vae.Filter1
            End Get
        End Property

        ''' <summary>第二层卷积的输出通道数。</summary>
        Public ReadOnly Property Filter2 As Integer
            Get
                Return _vae.Filter2
            End Get
        End Property

        ''' <summary>输入维度（单张图像的像素总数）。</summary>
        Public ReadOnly Property InputDim As Integer Implements IGenerativeModel.InputDim
            Get
                Return Height * Width * Channels
            End Get
        End Property

        ''' <summary>模型是否已完成训练。</summary>
        Public ReadOnly Property IsFitted As Boolean Implements IGenerativeModel.IsFitted
            Get
                Return _vae.IsFitted
            End Get
        End Property

        ''' <summary>逐轮的训练损失。</summary>
        Public ReadOnly Property Loss As List(Of Double)
            Get
                Return _vae.Loss
            End Get
        End Property

        ''' <summary>底层卷积 VAE（供高级用例直接访问）。</summary>
        Public ReadOnly Property Model As ConvVAE
            Get
                Return _vae
            End Get
        End Property

        ''' <summary>当前权重的快照（张量为模型内部引用）。</summary>
        Public ReadOnly Property Parameters As ConvVaeParameters
            Get
                Return _vae.ExportParameters()
            End Get
        End Property

#End Region

#Region "训练"

        ''' <summary>
        ''' 用一批 RGB 图像训练模型。
        ''' </summary>
        ''' <param name="images">形状为 <c>[N, H, W, 3]</c>、取值在 [0, 1] 的图像张量。</param>
        ''' <param name="options">训练超参数；Nothing 表示使用 <see cref="CreateDefaultOptions"/>。</param>
        Public Function Fit(images As Tensor, Optional options As TrainingOptions = Nothing) As RgbGenerativeVae
            Call ValidateImages(images)

            If options Is Nothing Then
                options = CreateDefaultOptions()
            End If

            Call _vae.Fit(images, options)

            Return Me
        End Function

        ''' <summary>用常用超参数训练模型（其余参数取默认值）。</summary>
        Public Function Fit(images As Tensor,
                            Optional epochs As Integer = 80,
                            Optional batchSize As Integer = 32,
                            Optional learningRate As Double = 0.002,
                            Optional beta As Double = 1.0,
                            Optional verbose As Boolean = False) As RgbGenerativeVae

            Dim options As New TrainingOptions With {
                .Epochs = epochs,
                .BatchSize = batchSize,
                .LearningRate = learningRate,
                .Beta = beta,
                .Verbose = verbose,
                .KlWarmupEpochs = std.Max(1, epochs \ 5)
            }

            Return Fit(images, options)
        End Function

        ''' <summary>
        ''' 训练用的默认超参数：80 轮、批大小 32、学习率 0.002，并带有 KL 退火。
        ''' </summary>
        Public Shared Function CreateDefaultOptions() As TrainingOptions
            Dim options As New TrainingOptions With {
                .Epochs = 80,
                .BatchSize = 32,
                .LearningRate = 0.002,
                .Beta = 1.0,
                .Verbose = False
            }

            options.KlWarmupEpochs = std.Max(1, options.Epochs \ 5)

            Return options
        End Function

#End Region

#Region "推理与生成"

        ''' <summary>确定性地重构输入图像（使用后验均值）。</summary>
        Public Function Reconstruct(images As Tensor) As Tensor Implements IGenerativeModel.Reconstruct
            Call ValidateImages(images)

            Return _vae.Reconstruct(images)
        End Function

        ''' <summary>从标准正态先验采样生成图像张量。</summary>
        Public Function Generate(count As Integer, seed As Integer?) As Tensor Implements IGenerativeModel.Generate
            If count <= 0 Then
                Throw New ArgumentException($"生成样本数必须为正，实际为 {count}", NameOf(count))
            End If

            Return _vae.Generate(count, seed)
        End Function

        ''' <summary>从标准正态先验采样生成图像张量（使用模型自带的随机数服务）。</summary>
        Public Function Generate(count As Integer) As Tensor
            Return Generate(count, Nothing)
        End Function

        ''' <summary>重构输入图像并返回位图。</summary>
        Public Function ReconstructImages(images As Tensor) As Bitmap()
            Return RgbImageTensor.ToBitmaps(Reconstruct(images))
        End Function

        ''' <summary>随机生成图像并返回位图。</summary>
        Public Function GenerateImages(count As Integer, Optional seed As Integer? = Nothing) As Bitmap()
            Return RgbImageTensor.ToBitmaps(Generate(count, seed))
        End Function

        ''' <summary>
        ''' 评估重构质量，返回 (每像素 BCE, 每像素 MSE, 每样本 KL)。
        ''' </summary>
        Public Function Evaluate(images As Tensor) As Tuple(Of Double, Double, Double)
            Call ValidateImages(images)

            Return _vae.Evaluate(images)
        End Function

#End Region

#Region "持久化"

        ''' <summary>
        ''' 把模型保存为单个 ONNX 文件（编码器 + 生成器双分支，对外 NCHW 布局）。
        ''' </summary>
        ''' <param name="path">目标文件路径，通常以 <c>.onnx</c> 结尾。</param>
        ''' <param name="options">导出选项；Nothing 表示使用默认值（opset 13、单精度权重）。</param>
        Public Sub SaveOnnx(path As String, Optional options As RgbVaeExportOptions = Nothing)
            Call RgbVaeOnnxExport.Save(Me, path, options)
        End Sub

        ''' <summary>
        ''' 从 ONNX 文件还原模型；失败时返回 <c>Nothing</c> 并给出中文原因。
        ''' </summary>
        ''' <param name="path">ONNX 文件路径。</param>
        ''' <param name="errorMessage">失败原因。</param>
        Public Shared Function LoadOnnx(path As String, ByRef errorMessage As String) As RgbGenerativeVae
            Return RgbVaeOnnxImport.Load(path, errorMessage)
        End Function

        ''' <summary>从 ONNX 文件还原模型；失败时抛出 <see cref="IO.InvalidDataException"/>。</summary>
        ''' <param name="path">ONNX 文件路径。</param>
        Public Shared Function LoadOnnx(path As String) As RgbGenerativeVae
            Return RgbVaeOnnxImport.Load(path)
        End Function

#End Region

#Region "辅助"

        ''' <summary>用参数快照重建模型。</summary>
        Public Shared Function FromParameters(parameters As ConvVaeParameters) As RgbGenerativeVae
            If parameters Is Nothing Then
                Throw New ArgumentNullException(NameOf(parameters))
            End If

            If parameters.Channels <> RgbImageTensor.ColorChannels Then
                Throw New ArgumentException(
                    $"参数快照的通道数为 {parameters.Channels}，而 RGB 生成模型要求 {RgbImageTensor.ColorChannels}",
                    NameOf(parameters))
            End If

            Return New RgbGenerativeVae(parameters)
        End Function

        ''' <summary>校验输入图像的形状与模型结构一致。</summary>
        Private Sub ValidateImages(images As Tensor)
            Dim shape As Integer() = RgbImageTensor.AssertImageBatch(images)

            If shape(1) <> Height OrElse shape(2) <> Width Then
                Throw New ArgumentException(
                    $"图像尺寸不符：模型为 {Height}x{Width}，输入为 {shape(1)}x{shape(2)}")
            End If
        End Sub

#End Region

        ''' <summary>返回 <c>RgbGenerativeVae(32x32x3, latent=16, filters=8/16)</c> 形式的文本。</summary>
        Public Overrides Function ToString() As String
            Return $"RgbGenerativeVae({Height}x{Width}x{Channels}, latent={LatentDim}, filters={Filter1}/{Filter2}, fitted={IsFitted})"
        End Function

    End Class

End Namespace
