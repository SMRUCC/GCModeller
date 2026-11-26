Namespace Copula

    ''' <summary>
    ''' Gaussian copula 的一次采样结果：隐变量、分位数与降序名次。
    ''' </summary>
    ''' <remarks>
    ''' <see cref="Quantiles"/> 是"按升序名次排到 <c>(i + 0.5)/n</c>"的锚点，
    ''' <see cref="Order"/> 是从大到小的下标序列（用于"优先分配"类的算法，
    ''' 例如把整数总量按名次从高到低投放）。
    ''' </remarks>
    Public Class CopulaSample

        ''' <summary>
        ''' 混合后的隐变量 <c>e = r·z + √(1-r²)·e0</c>
        ''' </summary>
        Public Property Latent As Double()

        ''' <summary>
        ''' 每个元素的分位锚点 <c>(rank + 0.5)/n</c>
        ''' </summary>
        Public Property Quantiles As Double()

        ''' <summary>
        ''' 按下标给出的降序名次（<c>Order(0)</c> 是隐变量最大的那个下标）
        ''' </summary>
        Public Property Order As Integer()

        ''' <summary>
        ''' 本次采样使用的相关系数
        ''' </summary>
        Public Property Correlation As Double

        ''' <summary>
        ''' 是否在搜索中收敛（直接给定相关系数时恒为 <c>True</c>）
        ''' </summary>
        Public Property Converged As Boolean = True

        Public Overrides Function ToString() As String
            Return $"copula sample, n={If(Latent Is Nothing, 0, Latent.Length)}, r={Correlation:G6}, converged={Converged}"
        End Function

    End Class

    ''' <summary>
    ''' 分位锚点与名次的结果结构
    ''' </summary>
    Public Structure QuantileRanks
        ''' <summary>
        ''' 每个元素的分位锚点 <c>(rank + 0.5)/n</c>
        ''' </summary>
        Public Quantiles As Double()
        ''' <summary>
        ''' 降序名次（下标序列）
        ''' </summary>
        Public [Order] As Integer()
    End Structure
End Namespace
