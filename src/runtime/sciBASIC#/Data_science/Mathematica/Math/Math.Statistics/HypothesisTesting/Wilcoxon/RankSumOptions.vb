Imports Microsoft.VisualBasic.Math.Statistics.OrderStatistics

Namespace Hypothesis

    ''' <summary>
    ''' Wilcoxon 秩和检验（Mann-Whitney U）的参数。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 默认值与常见的单细胞差异表达口径一致：对照组基因平均 CPM 大于 5 才参与检验，
    ''' 显著性阈值 0.05，BH 多重校正，正态近似并做并列校正。
    ''' </para>
    ''' <para>
    ''' 当两侧样本量之和不超过 <see cref="ExactThreshold"/> 且<b>没有并列值</b>时，
    ''' 自动改用精确法（枚举 U 的精确分布），否则使用带并列校正的正态近似。
    ''' </para>
    ''' </remarks>
    Public Class RankSumOptions

        ''' <summary>
        ''' 备择假设方向，默认双侧
        ''' </summary>
        ''' <returns></returns>
        Public Property Alternative As Hypothesis = Hypothesis.TwoSided

        ''' <summary>
        ''' 对照组基因的平均 CPM 门槛，低于该值的基因不参与检验（<c>NaN</c> 结果），默认 5
        ''' </summary>
        ''' <returns></returns>
        Public Property MinControlCpm As Double = 5.0

        ''' <summary>
        ''' 显著性阈值，默认 0.05
        ''' </summary>
        ''' <returns></returns>
        Public Property Alpha As Double = 0.05

        ''' <summary>
        ''' 序统计表的数值精度，默认 <see cref="ValuePrecision.Float32"/>
        ''' </summary>
        ''' <returns></returns>
        Public Property Precision As ValuePrecision = ValuePrecision.Float32

        ''' <summary>
        ''' 效应量的分子分母平滑项（<c>log2((meanX + eps)/(meanY + eps))</c>），默认 1e-9
        ''' </summary>
        ''' <returns></returns>
        Public Property EffectEpsilon As Double = 0.000000001

        ''' <summary>
        ''' 是否启用并列校正，默认 <c>True</c>
        ''' </summary>
        ''' <returns></returns>
        Public Property TieCorrection As Boolean = True

        ''' <summary>
        ''' 精确法的样本量上限（两侧之和），默认 20；设为 0 表示始终使用正态近似
        ''' </summary>
        ''' <returns></returns>
        Public Property ExactThreshold As Integer = 20

        ''' <summary>
        ''' 多重校正方法，默认 BH（Benjamini-Hochberg）
        ''' </summary>
        ''' <returns></returns>
        Public Property AdjustMethod As PValueAdjustMethod = PValueAdjustMethod.BH

        ''' <summary>
        ''' 逐基因批量检验的并行度：0 = 全部逻辑核心，1 = 串行
        ''' </summary>
        ''' <returns></returns>
        Public Property DegreeOfParallelism As Integer = 0

        ''' <summary>
        ''' 默认参数（每次返回新实例，避免共享可变状态）
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function [Default]() As RankSumOptions
            Return New RankSumOptions
        End Function

    End Class
End Namespace
