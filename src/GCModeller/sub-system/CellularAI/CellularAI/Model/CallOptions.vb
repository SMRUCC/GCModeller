Namespace Model

    ''' <summary>
    ''' 响应调用（哪些基因算作"被扰动影响"）的参数：对应参考配置 <c>model.calls</c> 段。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <see cref="EligibleCpm"/> 是表达资格门槛（对照平均 CPM &gt; 5 才可能被调用）；
    ''' <see cref="MinLfc"/> 是计划基因必须达到的最小 log2 效应；
    ''' <see cref="Gamma"/> 把扰动强度映射成调用数量的指数系数。
    ''' </para>
    ''' <para>
    ''' <see cref="StrengthK"/> / <see cref="StrengthC"/> / <see cref="StrengthPhi"/> 是"检测功效"模型的参数：
    ''' <c>p = Φ(k·|lfc| / √(1/μ + φ) − c)</c>，其中 μ 是该基因在该深度下的期望计数。
    ''' </para>
    ''' </remarks>
    Public Class CallOptions

        ''' <summary>表达资格门槛（对照平均 CPM），默认 5。</summary>
        Public Property EligibleCpm As Double = 5.0

        ''' <summary>计划基因的最小 |log2FC|，默认 0.02。</summary>
        Public Property MinLfc As Double = 0.02

        ''' <summary>调用数量对扰动强度的指数系数，默认 2.5。</summary>
        Public Property Gamma As Double = 2.5

        ''' <summary>每个扰动的最少调用数，默认 5。</summary>
        Public Property NMin As Integer = 5

        ''' <summary>每个扰动的最多调用数，默认 6000。</summary>
        Public Property NMax As Integer = 6000

        ''' <summary>功效得分在综合排序里的权重，默认 1.0。</summary>
        Public Property WPow As Double = 1.0

        ''' <summary>检测功效模型里的过度离散项 φ，默认 0.1。</summary>
        public property Phi As Double = 0.1

        ''' <summary>强度计算里"显著 z"的门槛，默认 3.0。</summary>
        Public Property StrengthZ As Double = 3.0

        ''' <summary>强度计算里功效的斜率 k，默认 111.2。</summary>
        Public Property StrengthK As Double = 111.2

        ''' <summary>强度计算里功效的偏移 c，默认 4.0。</summary>
        Public Property StrengthC As Double = 4.0

        ''' <summary>强度计算里的深度过度离散项，默认 0.05。</summary>
        Public Property StrengthPhi As Double = 0.05

    End Class
End Namespace
