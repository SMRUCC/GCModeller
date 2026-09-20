Imports SMRUCC.genomics.Analysis.Squiiff.NN

''' <summary>
''' SquiDiff 模型的超参数配置。
'''
''' 分组对应 README 的算法层次：
''' <list type="bullet">
''' <item>扩散过程：<see cref="DiffusionSteps"/> / <see cref="BetaStart"/> / <see cref="BetaEnd"/>；</item>
''' <item>语义编码器（VAE）：<see cref="SemanticDim"/> 等；</item>
''' <item>条件去噪网络：<see cref="DenoiserHiddenDim"/> / <see cref="TimeEmbeddingDim"/> 等；</item>
''' <item>训练：Adam 学习率、批量、轮数、梯度裁剪、学习率调度。</item>
''' </list>
''' </summary>
Public Class SquiiffConfig

#Region "扩散过程"

    ''' <summary>前向扩散总步数 T。</summary>
    Public Property DiffusionSteps As Integer = 500

    ''' <summary><c>β</c> 在 t=1 处的取值。</summary>
    Public Property BetaStart As Double = 0.001

    ''' <summary><c>β</c> 在 t=T 处的取值。</summary>
    Public Property BetaEnd As Double = 0.01

    ''' <summary><c>ᾱ</c> 的下限裁剪（避免 x̂_0 的除法放大误差）。</summary>
    Public Property AlphaBarFloor As Double = 0.00001

    ''' <summary>推理时的跨步子轨迹长度（DDIM 采样步数）。</summary>
    Public Property InferenceSteps As Integer = 100

#End Region

#Region "语义编码器（VAE）"

    ''' <summary>语义隐变量 <c>z_sem</c> 的维度。</summary>
    Public Property SemanticDim As Integer = 16

    ''' <summary>语义编码器的隐层宽度。</summary>
    Public Property EncoderHiddenDim As Integer = 128

    ''' <summary>语义编码器的残差块个数。</summary>
    Public Property EncoderBlocks As Integer = 2

    ''' <summary>
    ''' 语义编码器是否使用**条件**批归一化。
    ''' 默认 False：编码器为非条件 BN。若编码侧也以扰动标签为条件，
    ''' <c>Δz = Enc(x_pert) − Enc(x_ctrl)</c> 会出现标签泄漏，破坏隐空间向量算术的可解释性。
    ''' 该开关保留以便对比实验。
    ''' </summary>
    Public Property EncoderConditioned As Boolean = False

    ''' <summary>是否启用 VAE 重参数化采样（推理阶段始终取均值 μ）。</summary>
    Public Property UseReparameterization As Boolean = True

    ''' <summary>KL 正则权重（对应 β-VAE 的 β）。</summary>
    Public Property BetaKL As Double = 0.0001

#End Region

#Region "条件去噪网络"

    ''' <summary>去噪 MLP 的隐层宽度。</summary>
    Public Property DenoiserHiddenDim As Integer = 128

    ''' <summary>去噪 MLP 的条件残差块个数。</summary>
    Public Property DenoiserBlocks As Integer = 3

    ''' <summary>时间步正弦编码的维度（必须为偶数）。</summary>
    Public Property TimeEmbeddingDim As Integer = 32

    ''' <summary>激活函数（扩散模型默认 SiLU）。</summary>
    Public Property Activation As ActivationKind = ActivationKind.SiLU

#End Region

#Region "训练"

    ''' <summary>Adam 基础学习率。</summary>
    Public Property LearningRate As Double = 0.001

    ''' <summary>小批量大小。</summary>
    Public Property BatchSize As Integer = 64

    ''' <summary>训练轮数上限。</summary>
    Public Property Epochs As Integer = 300

    ''' <summary>解耦权重衰减（AdamW）。</summary>
    Public Property WeightDecay As Double = 0.0

    ''' <summary>全局 L2 梯度裁剪阈值（&lt;= 0 表示禁用）。</summary>
    Public Property ClipNorm As Double = 1.0

    ''' <summary>学习率 warmup 占总步数的比例。</summary>
    Public Property WarmupRatio As Double = 0.1

    ''' <summary>余弦退火的终点学习率比例（相对基础学习率）。</summary>
    Public Property MinLearningRateRatio As Double = 0.05

    ''' <summary>早停耐心值（连续多少轮无改善则停止；&lt;=0 表示禁用）。</summary>
    Public Property EarlyStoppingPatience As Integer = 40

    ''' <summary>随机种子（模型初始化与噪声采样均使用）。</summary>
    Public Property Seed As Integer = 20240920

#End Region

#Region "批归一化推理行为"

    ''' <summary>
    ''' 生成阶段是否仍使用当前批统计量。
    ''' 默认 False = 使用训练期累积的滑动统计量，使单个细胞的生成结果与批组成无关、可复现。
    ''' </summary>
    Public Property UseBatchStatsAtInference As Boolean = False

#End Region

    ''' <summary>校验配置合法性，返回错误信息（为空表示合法）。</summary>
    Public Function Validate() As String
        If DiffusionSteps < 1 Then Return "DiffusionSteps 必须 >= 1"
        If BetaStart <= 0.0 OrElse BetaStart >= 1.0 Then Return "BetaStart 必须在 (0,1) 内"
        If BetaEnd <= 0.0 OrElse BetaEnd >= 1.0 Then Return "BetaEnd 必须在 (0,1) 内"
        If BetaStart > BetaEnd Then Return "BetaStart 不应大于 BetaEnd"
        If SemanticDim < 1 Then Return "SemanticDim 必须 >= 1"
        If EncoderHiddenDim < 1 Then Return "EncoderHiddenDim 必须 >= 1"
        If DenoiserHiddenDim < 1 Then Return "DenoiserHiddenDim 必须 >= 1"
        If DenoiserBlocks < 1 Then Return "DenoiserBlocks 必须 >= 1"
        If TimeEmbeddingDim < 2 OrElse TimeEmbeddingDim Mod 2 <> 0 Then Return "TimeEmbeddingDim 必须为 >= 2 的偶数"
        If BatchSize < 1 Then Return "BatchSize 必须 >= 1"
        If Epochs < 1 Then Return "Epochs 必须 >= 1"
        If LearningRate <= 0.0 Then Return "LearningRate 必须 > 0"
        If InferenceSteps < 1 Then Return "InferenceSteps 必须 >= 1"
        If BetaKL < 0.0 Then Return "BetaKL 必须 >= 0"

        Return String.Empty
    End Function

    ''' <summary>配置摘要（写入 demo 报告与模型存档）。</summary>
    Public Function Describe() As String
        Return $"T={DiffusionSteps} β∈[{BetaStart},{BetaEnd}] z_sem={SemanticDim} " &
               $"编码器={EncoderHiddenDim}×{EncoderBlocks}块 去噪器={DenoiserHiddenDim}×{DenoiserBlocks}块 " &
               $"时间嵌入={TimeEmbeddingDim} 激活={Activation} " &
               $"lr={LearningRate} batch={BatchSize} epochs={Epochs} " &
               $"KL权重={BetaKL} 重参数化={UseReparameterization} 梯度裁剪={ClipNorm} 推理步={InferenceSteps}"
    End Function

    ''' <summary>深拷贝。</summary>
    Public Function Clone() As SquiiffConfig
        Return New SquiiffConfig With {
            .DiffusionSteps = Me.DiffusionSteps,
            .BetaStart = Me.BetaStart,
            .BetaEnd = Me.BetaEnd,
            .AlphaBarFloor = Me.AlphaBarFloor,
            .InferenceSteps = Me.InferenceSteps,
            .SemanticDim = Me.SemanticDim,
            .EncoderHiddenDim = Me.EncoderHiddenDim,
            .EncoderBlocks = Me.EncoderBlocks,
            .EncoderConditioned = Me.EncoderConditioned,
            .UseReparameterization = Me.UseReparameterization,
            .BetaKL = Me.BetaKL,
            .DenoiserHiddenDim = Me.DenoiserHiddenDim,
            .DenoiserBlocks = Me.DenoiserBlocks,
            .TimeEmbeddingDim = Me.TimeEmbeddingDim,
            .Activation = Me.Activation,
            .LearningRate = Me.LearningRate,
            .BatchSize = Me.BatchSize,
            .Epochs = Me.Epochs,
            .WeightDecay = Me.WeightDecay,
            .ClipNorm = Me.ClipNorm,
            .WarmupRatio = Me.WarmupRatio,
            .MinLearningRateRatio = Me.MinLearningRateRatio,
            .EarlyStoppingPatience = Me.EarlyStoppingPatience,
            .Seed = Me.Seed,
            .UseBatchStatsAtInference = Me.UseBatchStatsAtInference
        }
    End Function
End Class
