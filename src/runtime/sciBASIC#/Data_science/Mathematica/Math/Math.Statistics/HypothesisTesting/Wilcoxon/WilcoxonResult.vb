Namespace Hypothesis

    ''' <summary>
    ''' Wilcoxon 秩和检验（Mann-Whitney U）的检验方法。
    ''' </summary>
    Public Enum RankSumMethod
        ''' <summary>
        ''' 带并列校正的正态近似（大样本默认）
        ''' </summary>
        NormalApproximation = 0
        ''' <summary>
        ''' 枚举 U 的精确分布（小样本且无并列）
        ''' </summary>
        Exact = 1
    End Enum

    ''' <summary>
    ''' 一次两样本 Wilcoxon 秩和检验的结果。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <see cref="U"/> 是"第一组相对第二组"的 Mann-Whitney U 统计量：
    ''' 对第一组的每个观测，统计第二组中小于它的个数，并对相等的情况计 0.5。
    ''' 因此 <c>U(x, y) + U(y, x) = n1 · n2</c> 恒成立。
    ''' </para>
    ''' <para>
    ''' <see cref="TieCorrection"/> 是并入方差计算的校正因子
    ''' <c>1 - Σ(t³ - t)/(N³ - N)</c>，取值在 <c>(0, 1]</c>；全并列时该值为 0
    ''' （此时 <see cref="Z"/> 与 <see cref="PValue"/> 无定义，会返回 0 / <c>NaN</c>）。
    ''' </para>
    ''' </remarks>
    Public Class WilcoxonResult

        ''' <summary>
        ''' Mann-Whitney U 统计量（可能是半整数）
        ''' </summary>
        Public Property U As Double

        ''' <summary>
        ''' 正态化后的 z 值
        ''' </summary>
        Public Property Z As Double

        ''' <summary>
        ''' P 值（按 <see cref="Alternative"/> 的方向）
        ''' </summary>
        Public Property PValue As Double

        ''' <summary>
        ''' 多重校正后的 P 值（两样本入口不校正，值等于 <see cref="PValue"/>）
        ''' </summary>
        Public Property PAdjusted As Double

        ''' <summary>
        ''' 效应量：<c>log2((mean(X) + eps) / (mean(Y) + eps))</c>
        ''' </summary>
        Public Property Effect As Double

        ''' <summary>
        ''' 在参数 <c>Alpha</c> 的显著性水平下是否显著
        ''' </summary>
        Public Property Significant As Boolean

        ''' <summary>
        ''' 备择假设方向
        ''' </summary>
        Public Property Alternative As Hypothesis

        ''' <summary>
        ''' 使用的检验方法
        ''' </summary>
        Public Property Method As RankSumMethod

        ''' <summary>
        ''' 并列校正因子 <c>1 - Σ(t³-t)/(N³-N)</c>
        ''' </summary>
        Public Property TieCorrection As Double

        ''' <summary>
        ''' 第一组样本量
        ''' </summary>
        Public Property N1 As Integer

        ''' <summary>
        ''' 第二组样本量
        ''' </summary>
        Public Property N2 As Integer

        ''' <summary>
        ''' 诊断信息（例如退化原因）；正常情况下为空
        ''' </summary>
        Public Property Diagnostics As String

        Public Overrides Function ToString() As String
            Return $"U={U:G6}, z={Z:G6}, p={PValue:G6}, method={Method}, n=({N1},{N2})"
        End Function

    End Class
End Namespace
