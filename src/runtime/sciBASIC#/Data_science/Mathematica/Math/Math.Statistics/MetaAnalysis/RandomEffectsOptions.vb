Namespace MetaAnalysis

    ''' <summary>
    ''' 随机效应元分析的参数。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <see cref="VarianceFloor"/> 与 <see cref="RelativeVarianceFloor"/> 解决同一个问题的两个尺度：
    ''' 绝对地板防止 <c>1/variance</c> 溢出，相对地板（相对于该基因的方差中位数）防止
    ''' "某一项方差极小 ⇒ 权重独占整个合并结果"。两者可以同时生效，取较大者。
    ''' </para>
    ''' </remarks>
    Public Class RandomEffectsOptions

        ''' <summary>
        ''' 方差绝对地板，默认 1e-8
        ''' </summary>
        ''' <returns></returns>
        Public Property VarianceFloor As Double = 0.00000001

        ''' <summary>
        ''' 方差相对地板（乘以该基因的方差中位数），默认 0（关闭）
        ''' </summary>
        ''' <returns></returns>
        Public Property RelativeVarianceFloor As Double = 0.0

        ''' <summary>
        ''' 参与合并的最少研究数，默认 1；不足时返回未收敛的结果而不是抛异常
        ''' </summary>
        ''' <returns></returns>
        Public Property MinStudies As Integer = 1

        ''' <summary>
        ''' 方差缺失时是否用同组研究的方差中位数回填，默认 <c>True</c>
        ''' </summary>
        ''' <returns></returns>
        Public Property FallbackToMedian As Boolean = True

        ''' <summary>
        ''' 额外的逐研究权重倍数（下标 = 研究序号）；为 <c>Nothing</c> 表示不加权
        ''' </summary>
        ''' <returns></returns>
        Public Property WeightMultiplier As Func(Of Integer, Double) = Nothing

        ''' <summary>
        ''' 默认参数（每次返回新实例）
        ''' </summary>
        ''' <returns></returns>
        Public Shared Function [Default]() As RandomEffectsOptions
            Return New RandomEffectsOptions
        End Function

    End Class
End Namespace
