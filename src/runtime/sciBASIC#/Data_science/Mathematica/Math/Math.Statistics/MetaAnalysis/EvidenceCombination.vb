Imports sysmath = System.Math

Namespace MetaAnalysis

    ''' <summary>
    ''' 证据合并：加权均值、Stouffer / Liptak 合并、权重归一化与集中度诊断。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>加权均值</b>有两种缺失语义，必须显式选择：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description><c>missingAsZero:=True</c>：未测项按"该研究报了 0"计入分母，
    ''' 适合"效应缺失意味着无响应"的场景（上游池化的口径）；</description></item>
    ''' <item><description><c>missingAsZero:=False</c>：未测项完全剔除，
    ''' 适合"缺失就是不知道"的场景。</description></item>
    ''' </list>
    ''' <para>
    ''' <b>Stouffer 合并</b>返回<b>带符号</b>的合并 z（<c>Σwz/√Σw²</c>）；
    ''' 上游流水线里用的是它的绝对值 <c>|z|</c>，需要时请自行取绝对值。
    ''' </para>
    ''' </remarks>
    Public Module EvidenceCombination

        ''' <summary>
        ''' 加权均值；未测项按 <paramref name="missingAsZero"/> 决定是否计入分母
        ''' </summary>
        ''' <param name="values">各研究的取值</param>
        ''' <param name="weights">各研究的权重</param>
        ''' <param name="measured">哪些研究有测量；为 <c>Nothing</c> 表示全部有</param>
        ''' <param name="missingAsZero">未测项是否以"值为 0、权重照算"的方式计入</param>
        ''' <returns>权重和为 0 时返回 <see cref="Double.NaN"/></returns>
        Public Function WeightedMean(values As Double(),
                                     weights As Double(),
                                     Optional measured As Boolean() = Nothing,
                                     Optional missingAsZero As Boolean = True) As Double
            If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))
            If weights Is Nothing Then Throw New ArgumentNullException(NameOf(weights))
            If values.Length <> weights.Length Then
                Throw New ArgumentException($"values length {values.Length} does not match weights length {weights.Length}")
            End If

            Dim numerator As Double = 0.0
            Dim denominator As Double = 0.0

            For i As Integer = 0 To values.Length - 1
                Dim isMeasured As Boolean = measured Is Nothing OrElse measured(i)
                Dim w As Double = weights(i)

                If w <= 0.0 Then Continue For

                If isMeasured Then
                    If Double.IsNaN(values(i)) Then
                        If Not missingAsZero Then Continue For
                    Else
                        numerator += w * values(i)
                        denominator += w
                        Continue For
                    End If
                End If

                ' 未测（或缺失）项：按上游口径计入分母，分子贡献 0
                If missingAsZero Then denominator += w
            Next

            If denominator <= 0.0 Then Return Double.NaN

            Return numerator / denominator
        End Function

        ''' <summary>
        ''' Stouffer 合并：<c>z = Σ(w·z_i) / √Σw²</c>（只累加有测量的项）
        ''' </summary>
        ''' <param name="zScores">各研究的 z 值</param>
        ''' <param name="weights">各研究的权重</param>
        ''' <param name="measured">哪些研究有测量</param>
        ''' <returns>带符号的合并 z；有效权重为 0 时返回 0</returns>
        Public Function Stouffer(zScores As Double(),
                                 weights As Double(),
                                 Optional measured As Boolean() = Nothing) As Double
            If zScores Is Nothing Then Throw New ArgumentNullException(NameOf(zScores))
            If weights Is Nothing Then Throw New ArgumentNullException(NameOf(weights))
            If zScores.Length <> weights.Length Then
                Throw New ArgumentException($"z length {zScores.Length} does not match weights length {weights.Length}")
            End If

            Dim numerator As Double = 0.0
            Dim squareSum As Double = 0.0

            For i As Integer = 0 To zScores.Length - 1
                If measured IsNot Nothing AndAlso Not measured(i) Then Continue For

                Dim w As Double = weights(i)
                Dim z As Double = zScores(i)

                If w <= 0.0 OrElse Double.IsNaN(z) Then Continue For

                numerator += w * z
                squareSum += w * w
            Next

            If squareSum <= 0.0 Then Return 0.0

            Return numerator / sysmath.Sqrt(squareSum)
        End Function

        ''' <summary>
        ''' Liptak 合并：先把双侧 P 值转成带符号的正态分数，再走 Stouffer
        ''' </summary>
        ''' <param name="pValues">各研究的双侧 P 值</param>
        ''' <param name="weights">各研究的权重</param>
        ''' <param name="effectSigns">各研究效应的符号（<c>+1</c> / <c>-1</c>）；为 <c>Nothing</c> 表示全取正</param>
        ''' <returns></returns>
        Public Function Liptak(pValues As Double(),
                               weights As Double(),
                               Optional effectSigns As Double() = Nothing) As Double
            If pValues Is Nothing Then Throw New ArgumentNullException(NameOf(pValues))
            If effectSigns IsNot Nothing AndAlso effectSigns.Length <> pValues.Length Then
                Throw New ArgumentException(
                    $"sign length {effectSigns.Length} does not match p-value length {pValues.Length}", NameOf(effectSigns))
            End If

            Dim scores As Double() = New Double(pValues.Length - 1) {}

            For i As Integer = 0 To pValues.Length - 1
                Dim sign As Double = If(effectSigns Is Nothing, 1.0, effectSigns(i))

                scores(i) = NormalDistribution.SignedScore(pValues(i), sign)
            Next

            Return Stouffer(scores, weights)
        End Function

        ''' <summary>
        ''' 归一化权重，使其和为 1
        ''' </summary>
        ''' <param name="weights"></param>
        ''' <returns>权重和 <c>&lt;= 0</c> 时返回全 0（保持长度）</returns>
        Public Function NormalizeWeights(weights As Double()) As Double()
            If weights Is Nothing Then Throw New ArgumentNullException(NameOf(weights))

            Dim out As Double() = New Double(weights.Length - 1) {}
            Dim sum As Double = 0.0

            For i As Integer = 0 To weights.Length - 1
                If weights(i) > 0.0 Then sum += weights(i)
            Next

            If sum <= 0.0 Then Return out

            For i As Integer = 0 To weights.Length - 1
                If weights(i) > 0.0 Then out(i) = weights(i) / sum
            Next

            Return out
        End Function

        ''' <summary>
        ''' 有效研究数 <c>(Σw)²/Σw²</c>（权重归一化后等于 <c>1/Σw²</c>）
        ''' </summary>
        ''' <param name="weights"></param>
        ''' <returns>权重全为 0 时返回 0</returns>
        Public Function EffectiveSampleSize(weights As Double()) As Double
            If weights Is Nothing Then Throw New ArgumentNullException(NameOf(weights))

            Dim sum As Double = 0.0
            Dim squareSum As Double = 0.0

            For i As Integer = 0 To weights.Length - 1
                If weights(i) > 0.0 Then
                    sum += weights(i)
                    squareSum += weights(i) * weights(i)
                End If
            Next

            If squareSum <= 0.0 Then Return 0.0

            Return sum * sum / squareSum
        End Function

        ''' <summary>
        ''' 最大单项权重占比 <c>max(w)/Σw</c>
        ''' </summary>
        ''' <param name="weights"></param>
        ''' <returns>权重全为 0 时返回 0</returns>
        Public Function WeightShare(weights As Double()) As Double
            If weights Is Nothing Then Throw New ArgumentNullException(NameOf(weights))

            Dim sum As Double = 0.0
            Dim max As Double = 0.0

            For i As Integer = 0 To weights.Length - 1
                If weights(i) > 0.0 Then
                    sum += weights(i)

                    If weights(i) > max Then max = weights(i)
                End If
            Next

            If sum <= 0.0 Then Return 0.0

            Return max / sum
        End Function

    End Module
End Namespace
