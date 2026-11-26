Namespace Model

    ''' <summary>
    ''' 响应计划（每个扰动调用多少基因、怎么排序）的参数：对应参考配置 <c>model.plan</c> 段。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <see cref="CurveSlope"/> 把"训练数据里该扰动的响应广度"映射成调用数量
    ''' （<c>n = exp(slope·(x − median))</c>，再整体缩放到中位数等于 <c>median_degs</c>）。
    ''' </para>
    ''' <para>
    ''' <see cref="SimTau"/> / <see cref="SimNtop"/> / <see cref="SimCap"/> 控制"训练细胞系与新上下文的相似度权重"：
    ''' 先对相关系数做 z 标准化，再取 <c>exp(z/τ)</c>，最后用上限水位法把单项权重压到 <c>cap</c> 以下。
    ''' </para>
    ''' <para>
    ''' <see cref="PowerK"/> 系列是"期望可检测基因数"的功效模型；<see cref="ShrinkQuantile"/> /
    ''' <see cref="ShrinkFactor"/> 把得分最低的一部分扰动的调用数量按比例收缩，把省下来的调用
    ''' 分配给得分高的扰动（保证总调用量守恒）。
    ''' </para>
    ''' </remarks>
    Public Class PlanOptions

        ''' <summary>广度→调用数量的曲线斜率，默认 3.0。</summary>
        Public Property CurveSlope As Double = 3.0

        ''' <summary>相似度 softmax 的温度 τ，默认 1.0。</summary>
        Public Property SimTau As Double = 1.0

        ''' <summary>计算相似度时取的高变基因数，默认 3000。</summary>
        Public Property SimNtop As Integer = 3000

        ''' <summary>单项相似度权重的上限，默认 0.3。</summary>
        Public Property SimCap As Double = 0.3

        ''' <summary>计划 z 中"均匀权重"的混合比例，默认 0.5。</summary>
        Public Property PlanZUniform As Double = 0.5

        ''' <summary>广度加权时的温度 τ，默认 0.5。</summary>
        Public Property XsimTau As Double = 0.5

        ''' <summary>广度加权只作用于上半分位（0.5 表示中位以上），默认 0.5。</summary>
        Public Property XsimStrength As Double = 0.5

        ''' <summary>广度加权允许的最大抬升倍数，默认 2.0。</summary>
        Public Property XsimCapFactor As Double = 2.0

        ''' <summary>广度加权允许的最大总量倍数，默认 1.2。</summary>
        Public Property XsimMaxTotal As Double = 1.2

        ''' <summary>功效模型的斜率 k，默认 111.0。</summary>
        Public Property PowerK As Double = 111.0

        ''' <summary>功效模型的偏移 c，默认 3.0。</summary>
        Public Property PowerC As Double = 3.0

        ''' <summary>功效模型的过度离散项，默认 0.05。</summary>
        Public Property PowerPhi As Double = 0.05

        ''' <summary>功效模型使用的参考深度，默认 20000。</summary>
        Public Property PowerDepth As Double = 20000.0

        ''' <summary>"宽带显著性"的门槛 |z|，默认 2.0。</summary>
        Public Property PowerZN2 As Double = 2.0

        ''' <summary>宽带显著基因数在综合得分里的惩罚系数，默认 0.5。</summary>
        Public Property PowerTPen As Double = 0.5

        ''' <summary>功效的软截断参数（|z| 小于该值时逐步降权），默认 2.0。</summary>
        Public Property PowerSoftZ As Double = 2.0

        ''' <summary>需要收缩的得分分位，默认 0.2（最低的 20%）。</summary>
        Public Property ShrinkQuantile As Double = 0.2

        ''' <summary>收缩后保留的比例，默认 0.1。</summary>
        Public Property ShrinkFactor As Double = 0.1

    End Class
End Namespace
