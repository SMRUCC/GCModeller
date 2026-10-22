Namespace LinearRegression

    ''' <summary>
    ''' 一条来自 curated 数据库的调控关系先验：<c>TF -> TargetGene</c>。
    ''' 
    ''' 这是一个刻意做得很轻的结构：<see cref="SMRUCC.genomics.Analysis.BNLearn"/>
    ''' 之中虽然已经有功能完整的 <c>RegulatoryEdge</c>，但让
    ''' <c>Network.Regulons</c> 依赖整个 BNLearn 工程并不划算，
    ''' 因此先验在 R# 导出层被投影成本类型之后再传入拟合器。
    ''' </summary>
    ''' <remarks>
    ''' 方向性(激活/抑制)在这里被刻意丢弃：本轮只把先验用于「哪些 TF 调控这个基因」
    ''' 以及「这条边的惩罚要不要放松」，不使用先验的符号去约束回归系数。
    ''' </remarks>
    Public Structure PriorEdge

        ''' <summary>
        ''' 调控者(转录因子)的基因编号
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly TF As String

        ''' <summary>
        ''' 被调控的目标基因编号
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly TargetGene As String

        ''' <summary>
        ''' 先验的置信度，取值 0~1(High = 0.95，Medium = 0.7，Low = 0.3)
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Confidence As Double

        ''' <summary>
        ''' 构造一条先验调控关系
        ''' </summary>
        ''' <param name="tf">调控者(转录因子)的基因编号</param>
        ''' <param name="target">被调控的目标基因编号</param>
        ''' <param name="confidence">先验的置信度，取值 0~1</param>
        Public Sub New(tf As String, target As String, Optional confidence As Double = 1.0R)
            Me.TF = tf
            Me.TargetGene = target
            Me.Confidence = confidence
        End Sub

        ''' <summary>
        ''' 调试输出：<c>TF -> Target (conf=0.95)</c>
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return $"{TF} -> {TargetGene} (conf={Confidence:F2})"
        End Function

    End Structure

End Namespace
