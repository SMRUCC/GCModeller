Imports System
Imports System.Collections.Generic

Namespace Hybrid

    ''' <summary>
    ''' 概率布尔网络采样得到的扰动结果集合（逐基因的均值、离散度与定性一致性）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 之所以不复用 <c>InterventionResult</c>：那是 BNLearn / GPS 共享的契约类型
    ''' （见 <c>PerturbationTrace</c> 里「不往共享契约上加字段」的约定），
    ''' 而集成结果需要均值/标准差/置信区间/符号一致率这些 FuzzyGene 专有的东西。
    ''' 这里用一侧的新类型包装，既不改契约，也保留了原来的单点结果。
    ''' </para>
    ''' <para>
    ''' <see cref="Samples"/> 就是此前被忽略的 <c>nSamples</c>：
    ''' 语义是「按 PBN 规则概率重建网络多少次」，而不是「在同一个确定性稳态上抖噪声」。
    ''' 因此 <see cref="StandardDeviation"/> 衡量的是**模型结构不确定性**，不是数值噪声。
    ''' </para>
    ''' </remarks>
    Public Class EnsemblePerturbationResult

        ''' <summary>基因名（列顺序）。</summary>
        Public Property GeneNames As String()

        ''' <summary>实际完成的采样次数。</summary>
        Public Property Samples As Integer

        ''' <summary>逐基因均值（原始表达量）。</summary>
        Public Property Mean As Double()

        ''' <summary>逐基因标准差（跨样本）。</summary>
        Public Property StandardDeviation As Double()

        ''' <summary>逐基因的 5% 分位（小样本时退化为最小值）。</summary>
        Public Property LowerBound As Double()

        ''' <summary>逐基因的 95% 分位（小样本时退化为最大值）。</summary>
        Public Property UpperBound As Double()

        ''' <summary>
        ''' 逐基因的符号一致率：有多少比例的样本与该基因的平均变化同号（0~1）。
        ''' </summary>
        ''' <remarks>
        ''' 这是「这个基因的响应方向可信吗」的直接度量：
        ''' 0.5 表示方向完全由模型噪声决定，写进结论里就是不可信。
        ''' </remarks>
        Public Property SignAgreement As Double()

        ''' <summary>
        ''' 逐基因在布尔层采样中处于「高表达」状态的比例（0~1）；不在 PBN 范围内的基因为 <c>NaN</c>。
        ''' </summary>
        Public Property BooleanHighFraction As Double()

        ''' <summary>采样中检出的布尔吸引子周期分布（周期 → 次数）。</summary>
        Public Property AttractorPeriods As New Dictionary(Of Integer, Integer)()

        ''' <summary>连续求解成功收敛的样本比例。</summary>
        Public Property ConvergedFraction As Double

        ''' <summary>确定性参考解（<c>nSamples = 0</c> 时的唯一结果，或集成前的单点结果）。</summary>
        Public Property Reference As Double()

        ''' <summary>说明与降级信息（为什么退化、为什么没有置信区间等）。</summary>
        Public Property Notes As String

        ''' <summary>是否退化成单点结果（采样数 &lt;= 1 或 PBN 不可用）。</summary>
        Public ReadOnly Property Degenerate As Boolean
            Get
                Return Samples <= 1
            End Get
        End Property

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Dim periods As String = "n/a"

            If AttractorPeriods.Count > 0 Then
                Dim parts As New List(Of String)()

                For Each item As KeyValuePair(Of Integer, Integer) In AttractorPeriods
                    Call parts.Add($"{item.Key}x{item.Value}")
                Next

                periods = String.Join(", ", parts)
            End If

            Return $"ensemble perturbation: {Samples} sample(s), converged={ConvergedFraction:P0}, attractors=[{periods}], " &
                   $"degenerate={Degenerate}"
        End Function

    End Class

End Namespace
