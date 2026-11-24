' ============================================================================
' IGenerativeModel.vb - 生成模型的统一契约
'
' 设计约束（零破坏）：
'   既有模型类通过「显式接口实现」挂载这些接口
'       Private Function X(...) Implements IGenerativeModel.Y
'   因此它们的公开可见面不会增加任何成员，也不会改动既有签名。
' ============================================================================

Imports Microsoft.VisualBasic.MachineLearning.TensorFlow

Namespace Core

    ''' <summary>
    ''' 生成式模型的最小契约：编码重构 + 先验采样。
    ''' </summary>
    Public Interface IGenerativeModel

        ''' <summary>输入维度。</summary>
        ReadOnly Property InputDim As Integer

        ''' <summary>模型是否已完成训练。</summary>
        ReadOnly Property IsFitted As Boolean

        ''' <summary>
        ''' 重构输入。实现方必须保证该过程不引入不可复现的随机性。
        ''' </summary>
        Function Reconstruct(x As Tensor) As Tensor

        ''' <summary>
        ''' 从先验分布生成样本。种子为 <c>Nothing</c> 时使用模型自带的随机数服务。
        ''' </summary>
        Function Generate(count As Integer, seed As Integer?) As Tensor

    End Interface

    ''' <summary>
    ''' 概率聚类模型的契约：软分配 + 硬标签。
    ''' </summary>
    Public Interface IProbabilisticClusterer

        ''' <summary>混合成分数量。</summary>
        ReadOnly Property NComponents As Integer

        ''' <summary>硬聚类标签。</summary>
        Function Predict(x As Tensor) As Integer()

        ''' <summary>后验责任度矩阵 [N, K]。</summary>
        Function PredictProba(x As Tensor) As Tensor

    End Interface

    ''' <summary>
    ''' 可以从学到的先验分布中采样的模型。
    ''' 与 <see cref="IGenerativeModel"/> 的区别在于不要求具备编码重构能力
    ''' （例如 GMM 只能聚类与采样，没有解码器）。
    ''' </summary>
    Public Interface IPriorSampler

        ''' <summary>从先验采样 <paramref name="count"/> 个样本。</summary>
        Function Sample(count As Integer, seed As Integer?) As Tensor

    End Interface

    ''' <summary>
    ''' 可供模型选择（BIC/AIC）使用的评分契约。
    ''' </summary>
    Public Interface IModelScore

        ''' <summary>训练集上的对数似然（求和，非均值）。</summary>
        ReadOnly Property LogLikelihood As Double

        ''' <summary>自由参数个数。</summary>
        ReadOnly Property ParametersCount As Integer

    End Interface

End Namespace
