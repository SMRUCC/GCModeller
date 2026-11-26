Namespace MetaAnalysis

    ''' <summary>
    ''' 一项研究的效应量：估计值、方差与（可选的）外部权重倍数。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <see cref="IsMeasured"/> 与 <see cref="Variance"/> 是两件不同的事：
    ''' "未测量"的研究不参与合并，"测得但方差缺失"的研究可以回填基因中位方差后参与合并。
    ''' 把两者混在一起会让合并结果在沉默的缺失下悄悄偏移，因此这里显式区分。
    ''' </para>
    ''' <para>
    ''' <see cref="Weight"/> 是逆方差权重之外的<b>额外倍数</b>（例如按相似度、置信度、
    ''' 数据质量给的折扣），缺省为 1。合并时的实际权重是
    ''' <c>Weight / (Variance + τ²)</c>。
    ''' </para>
    ''' </remarks>
    Public Class StudyEffect

        ''' <summary>
        ''' 效应量估计（例如 log2 fold change）
        ''' </summary>
        Public Property Effect As Double

        ''' <summary>
        ''' 效应量的方差（标准误的平方）；<c>NaN</c> 或非正值表示缺失
        ''' </summary>
        Public Property Variance As Double

        ''' <summary>
        ''' 逆方差权重之外的额外倍数，默认 1
        ''' </summary>
        Public Property Weight As Double = 1.0

        ''' <summary>
        ''' 该研究是否被测量（未测量的研究不参与合并）
        ''' </summary>
        Public Property IsMeasured As Boolean = True

        ''' <summary>
        ''' 构造一项研究
        ''' </summary>
        ''' <param name="effect"></param>
        ''' <param name="variance"></param>
        ''' <param name="weight">额外权重倍数</param>
        ''' <param name="measured"></param>
        ''' <returns></returns>
        Public Shared Function Create(effect As Double,
                                      variance As Double,
                                      Optional weight As Double = 1.0,
                                      Optional measured As Boolean = True) As StudyEffect
            Return New StudyEffect With {
                .Effect = effect,
                .Variance = variance,
                .Weight = weight,
                .IsMeasured = measured
            }
        End Function

        ''' <summary>
        ''' 计算逆方差权重 <c>Weight / max(Variance, floor)</c>
        ''' </summary>
        ''' <param name="varianceFloor">方差地板（绝对值）</param>
        ''' <returns>不可用时返回 0</returns>
        Public Function InverseVarianceWeight(Optional varianceFloor As Double = 0.00000001) As Double
            If Not IsMeasured OrElse Double.IsNaN(Effect) Then Return 0.0
            If Double.IsNaN(Variance) OrElse Variance <= 0.0 Then Return 0.0

            Return Weight / System.Math.Max(Variance, varianceFloor)
        End Function

        Public Overrides Function ToString() As String
            Return $"effect={Effect:G6}, variance={Variance:G6}, weight={Weight:G6}, measured={IsMeasured}"
        End Function

    End Class
End Namespace
