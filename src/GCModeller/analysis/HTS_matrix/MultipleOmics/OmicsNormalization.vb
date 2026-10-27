Namespace MultipleOmics

    ''' <summary>
    ''' 组学特异的稳健标准化方法。
    ''' </summary>
    ''' <remarks>
    ''' 所有方法都只在<b>单个组学层的内部</b>生效：要么沿样本轴逐特征变换，
    ''' 要么沿特征轴逐样本变换，绝不跨组学混合，
    ''' 因此不会把某个组学的量纲信息带入另一个组学。
    ''' </remarks>
    Public Enum OmicsNormalization
        ''' <summary>不做任何变换</summary>
        None = 0
        ''' <summary>逐特征 z 分数（均值/标准差，标准差用 n-1）</summary>
        ZScore = 1
        ''' <summary>逐特征稳健 z 分数（中位数/MAD，尺度因子 1.4826）</summary>
        RobustZScore = 2
        ''' <summary>逐特征在样本轴上的秩，线性映射到 [0, 1]</summary>
        RankPerFeature = 3
        ''' <summary>逐样本在特征轴上的分位数归一化（层内参考分布）</summary>
        QuantilePerSample = 4
        ''' <summary>以 2 为底的对数（负值整体平移后再取对数）</summary>
        Log2 = 5
    End Enum
End Namespace