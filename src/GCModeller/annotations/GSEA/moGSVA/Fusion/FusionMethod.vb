Namespace Fusion

    ''' <summary>
    ''' 组学加权融合策略（method.md 步骤4）。
    ''' </summary>
    ''' <remarks>
    ''' 所有权重都是<b>逐通路</b>计算的：每条通路只使用在该通路中有分数的组学层，
    ''' 因此不同通路可以有不同的组学权重组合。
    ''' </remarks>
    Public Enum FusionMethods
        ''' <summary>等权融合 w_k = 1/K（Baseline 1）</summary>
        Equal = 0
        ''' <summary>方差倒数加权 w_k ∝ 1/Var(ES_k)（抑制噪声大的组学）</summary>
        Variance = 1
        ''' <summary>信息熵加权 w_k ∝ H(ES_k)（偏好区分度高的组学）</summary>
        Entropy = 2
        ''' <summary>互信息一致性加权 w_k ∝ mean_k' I(k, k')（偏好与其他组学一致的组学）</summary>
        MutualInformation = 3
        ''' <summary>组学特征数平衡 w_k ∝ 1/log2(2+|S_k|)（抑制成员多的组学）</summary>
        Balanced = 4
    End Enum

End Namespace
