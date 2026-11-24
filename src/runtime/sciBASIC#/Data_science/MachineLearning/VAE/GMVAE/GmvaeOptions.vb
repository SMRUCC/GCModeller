' ============================================================================
' GmvaeOptions.vb - 高斯混合变分自编码器的行为开关
'
' 既有构造签名与公开方法签名保持不变；这里提供的是**新增**的枚举与选项，
' 让重构中修正的行为可以被显式选择，也能一键**复现旧行为**用于前后对比。
' ============================================================================

''' <summary>类别先验 p(y) 的形式。</summary>
Public Enum GmvaePriorMode
    ''' <summary>
    ''' 固定均匀先验（推荐）。KL(q(y|x)||p(y)) 会真正起判别作用，
    ''' 避免「先验跟着后验跑」导致 KL 项失效。
    ''' </summary>
    Uniform = 0
    ''' <summary>
    ''' 可学习的先验（重构前的行为，仅用于复现旧结果）。
    ''' </summary>
    Learned = 1
End Enum

''' <summary>重建项使用的似然类型。</summary>
Public Enum GmvaeLikelihood
    ''' <summary>高斯似然 + MSE（连续值输入）。</summary>
    Gaussian = 0
    ''' <summary>伯努利似然 + 交叉熵（[0,1] 区间输入，例如图像）。</summary>
    Bernoulli = 1
End Enum

''' <summary>重建损失的归一化方式。</summary>
Public Enum GmvaeReconstructionScale
    ''' <summary>按 批次 × 输入维度 归一（推荐，损失尺度与输入维度无关）。</summary>
    PerElement = 0
    ''' <summary>仅按批次归一（重构前的行为，尺度随输入维度线性增长）。</summary>
    PerBatch = 1
End Enum

''' <summary>优化器类型。</summary>
Public Enum GmvaeOptimizer
    ''' <summary>Adam（推荐）。</summary>
    Adam = 0
    ''' <summary>普通 SGD（重构前的行为）。</summary>
    Sgd = 1
End Enum
