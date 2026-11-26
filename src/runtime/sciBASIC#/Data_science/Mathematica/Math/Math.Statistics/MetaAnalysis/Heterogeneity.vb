Imports sysmath = System.Math

Namespace MetaAnalysis

    ''' <summary>
    ''' 研究间异质性的度量：Cochran Q、I²、DerSimonian-Laird 的 τ²。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 三个量回答同一个问题的不同侧面：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description><b>Q</b>：加权偏差平方和，量纲依赖效应尺度，随研究数增大；</description></item>
    ''' <item><description><b>I²</b>：<c>max(0, (Q - df)/Q)</c>，落在 <c>[0, 1]</c>，可跨数据集比较；</description></item>
    ''' <item><description><b>τ²</b>：研究间方差本身，直接进入随机效应权重 <c>1/(v + τ²)</c>。</description></item>
    ''' </list>
    ''' <para>
    ''' 全部函数在数据退化（权重和为 0、研究数不足）时返回 0 而不是抛异常；
    ''' 需要知道"是否真的估出来了"请用 <see cref="RandomEffects.Pool"/> 返回的
    ''' <see cref="PooledEvidence.Converged"/>。
    ''' </para>
    ''' </remarks>
    Public Module Heterogeneity

        ''' <summary>
        ''' Cochran Q 异质性统计量：<c>Σ w·(y - ȳ_w)²</c>
        ''' </summary>
        ''' <param name="effects">各研究的效应量</param>
        ''' <param name="weights">各研究的逆方差权重</param>
        ''' <param name="measured">哪些研究参与计算；为 <c>Nothing</c> 表示全部参与</param>
        ''' <returns>权重和为 0 时返回 0</returns>
        Public Function CochranQ(effects As Double(), weights As Double(), Optional measured As Boolean() = Nothing) As Double
            If effects Is Nothing Then Throw New ArgumentNullException(NameOf(effects))
            If weights Is Nothing Then Throw New ArgumentNullException(NameOf(weights))
            If effects.Length <> weights.Length Then
                Throw New ArgumentException($"effects length {effects.Length} does not match weights length {weights.Length}")
            End If

            Dim s0 As Double = 0.0
            Dim s1 As Double = 0.0
            Dim s2 As Double = 0.0

            For i As Integer = 0 To effects.Length - 1
                If measured IsNot Nothing AndAlso Not measured(i) Then Continue For

                Dim w As Double = weights(i)
                Dim y As Double = effects(i)

                If w <= 0.0 OrElse Double.IsNaN(y) Then Continue For

                s0 += w
                s1 += w * y
                s2 += w * y * y
            Next

            If s0 <= 0.0 Then Return 0.0

            Return sysmath.Max(0.0, s2 - s1 * s1 / s0)
        End Function

        ''' <summary>
        ''' 异质性比例 <c>I² = max(0, (Q - df)/Q)</c>
        ''' </summary>
        ''' <param name="q">Cochran Q</param>
        ''' <param name="df">自由度（研究数 - 1）</param>
        ''' <returns>落在 <c>[0, 1]</c>；<c>Q &lt;= 0</c> 时返回 0</returns>
        Public Function I2(q As Double, df As Integer) As Double
            If q <= 0.0 Then Return 0.0

            Dim value As Double = (q - df) / q

            If value < 0.0 Then Return 0.0
            If value > 1.0 Then Return 1.0

            Return value
        End Function

        ''' <summary>
        ''' 研究间方差 τ²（DerSimonian-Laird 估计）
        ''' </summary>
        ''' <param name="effects"></param>
        ''' <param name="variances"></param>
        ''' <param name="measured"></param>
        ''' <param name="options">方差地板等参数</param>
        ''' <returns></returns>
        Public Function BetweenStudyVariance(effects As Double(),
                                            variances As Double(),
                                            Optional measured As Boolean() = Nothing,
                                            Optional options As RandomEffectsOptions = Nothing) As Double
            Return RandomEffects.DerSimonianLaird(effects, variances, measured, options)
        End Function

    End Module
End Namespace
