Namespace LinearRegression

    ''' <summary>
    ''' 先验因果符号的使用方式。
    ''' </summary>
    Public Enum PriorSignMode

        ''' <summary>
        ''' 不使用符号，只保留"候选注入 + 惩罚放松"(方案 A + B)。
        ''' </summary>
        Off = 0

        ''' <summary>
        ''' <b>符号约束</b>(signed lasso)：把先验边的系数限制在半空间内
        ''' (激活 β ≥ 0、抑制 β ≤ 0)。
        ''' 
        ''' 数据与先验冲突时系数会被压到 0 —— 即"宁可不要这条边，也不保留错误方向"。
        ''' 行为最保守，但不会产生上调。
        ''' </summary>
        Constrain = 1

        ''' <summary>
        ''' <b>保幅翻号</b>：先自由拟合(幅度来自数据)，再把先验边的系数符号翻正/翻负，
        ''' 幅度不变。
        ''' 
        ''' 与 <see cref="Constrain"/> 的区别是关键性的：共表达数据里抑制边同样是正相关，
        ''' 约束只会把它压成 0，而翻号能保留 |β| 并输出正确的因果方向
        ''' (敲降抑制因子 → 靶基因<b>上调</b>)。
        ''' </summary>
        Flip = 2

    End Enum

    ''' <summary>
    ''' 一条来自 curated 数据库的调控关系先验：<c>TF -> TargetGene</c>。
    ''' 
    ''' 这是一个刻意做得很轻的结构："SMRUCC.genomics.Analysis.BNLearn"
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
        Public ReadOnly TF As String

        ''' <summary>
        ''' 被调控的目标基因编号
        ''' </summary>
        Public ReadOnly TargetGene As String

        ''' <summary>
        ''' 先验的置信度，取值 0~1(High = 0.95，Medium = 0.7，Low = 0.3)
        ''' </summary>
        Public ReadOnly Confidence As Double

        ''' <summary>
        ''' 先验的因果符号：<c>+1</c> 激活、<c>-1</c> 抑制、<c>0</c> 未知。
        ''' 
        ''' 这个符号会被用作回归系数的<b>可行域约束</b>(signed lasso)：
        ''' 激活边只允许 β ≥ 0、抑制边只允许 β ≤ 0。
        ''' 
        ''' 之所以必须这样做，是因为共表达数据本身无法区分激活与抑制
        ''' (抽样验证过：Inhibitor 边的共表达相关系数中位数同样是正的)，
        ''' 而任何偶次惩罚(拉普拉斯、L1、L2)对 β → -β 都是对称的，
        ''' 只有把可行域限制到半空间才能固定绝对符号。
        ''' </summary>
        Public ReadOnly Sign As Integer

        ''' <summary>
        ''' 构造一条先验调控关系
        ''' </summary>
        ''' <param name="tf">调控者(转录因子)的基因编号</param>
        ''' <param name="target">被调控的目标基因编号</param>
        ''' <param name="confidence">先验的置信度，取值 0~1</param>
        ''' <param name="sign">因果符号：+1 激活、-1 抑制、0 未知</param>
        Public Sub New(tf As String, target As String,
                       Optional confidence As Double = 1.0R,
                       Optional sign As Integer = 0)

            Me.TF = tf
            Me.TargetGene = target
            Me.Confidence = confidence
            Me.Sign = Math.Sign(sign)
        End Sub

        ''' <summary>
        ''' 调试输出：<c>TF -> Target (conf=0.95, sign=-1)</c>
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return $"{TF} -> {TargetGene} (conf={Confidence:F2}, sign={Sign:+#;-#;0})"
        End Function

    End Structure

End Namespace
