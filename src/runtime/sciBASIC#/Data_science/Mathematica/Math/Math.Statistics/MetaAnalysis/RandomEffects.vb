Imports sysmath = System.Math

Namespace MetaAnalysis

    ''' <summary>
    ''' 随机效应元分析：DerSimonian-Laird 研究间方差、逆方差权重、加权合并与证据合并。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 固定效应模型假设所有研究估计同一个真值；随机效应模型允许每个研究有自己的真值
    ''' （服从 <c>N(μ, τ²)</c>），因此权重是 <c>1/(v_i + τ²)</c>——
    ''' 研究间差异越大，单个研究的话语权就被压得越低。
    ''' </para>
    ''' <para>
    ''' <b>DerSimonian-Laird</b> 用矩估计给出 τ²：
    ''' </para>
    ''' <code>
    ''' Q  = Σ w_i (y_i - ȳ_w)²
    ''' C  = Σ w_i - Σ w_i² / Σ w_i
    ''' df = k - 1
    ''' τ² = max(0, (Q - df) / C)
    ''' </code>
    ''' <para>
    ''' 除了"研究数不足/方差全缺失"之外，本模块不会因为数值退化中断计算：
    ''' 权重集中、τ² 被截断到 0 这类情况只体现在 <see cref="PooledEvidence.Diagnostics"/> 里。
    ''' </para>
    ''' </remarks>
    Public Module RandomEffects

        ''' <summary>
        ''' DerSimonian-Laird 研究间方差 τ²
        ''' </summary>
        ''' <param name="effects">各研究的效应量</param>
        ''' <param name="variances">各研究的效应量方差</param>
        ''' <param name="measured">哪些研究参与计算；<c>Nothing</c> 表示全部参与（缺失方差仍会被剔除）</param>
        ''' <param name="options">方差地板等参数</param>
        ''' <returns>τ²，下界为 0；没有可用研究时返回 0</returns>
        Public Function DerSimonianLaird(effects As Double(),
                                         variances As Double(),
                                         Optional measured As Boolean() = Nothing,
                                         Optional options As RandomEffectsOptions = Nothing) As Double

            If effects Is Nothing Then Throw New ArgumentNullException(NameOf(effects))
            If variances Is Nothing Then Throw New ArgumentNullException(NameOf(variances))
            If effects.Length <> variances.Length Then
                Throw New ArgumentException($"effects length {effects.Length} does not match variances length {variances.Length}")
            End If
            If options Is Nothing Then options = RandomEffectsOptions.Default()

            Dim floor As Double = options.VarianceFloor
            Dim s0 As Double = 0.0
            Dim s1 As Double = 0.0
            Dim s2 As Double = 0.0
            Dim squareSum As Double = 0.0
            Dim studies As Integer = 0

            For i As Integer = 0 To effects.Length - 1
                If measured IsNot Nothing AndAlso Not measured(i) Then Continue For

                Dim variance As Double = variances(i)
                Dim y As Double = effects(i)

                If Double.IsNaN(y) OrElse Double.IsNaN(variance) OrElse variance <= 0.0 Then Continue For

                Dim w As Double = 1.0 / sysmath.Max(variance, floor)

                s0 += w
                s1 += w * y
                s2 += w * y * y
                squareSum += w * w
                studies += 1
            Next

            If studies < 2 OrElse s0 <= 0.0 Then Return 0.0

            Dim q As Double = sysmath.Max(0.0, s2 - s1 * s1 / s0)
            Dim c As Double = s0 - squareSum / s0
            Dim df As Double = studies - 1

            If c <= 0.0 Then Return 0.0

            Return sysmath.Max(0.0, (q - df) / c)
        End Function

        ''' <summary>
        ''' 逆方差权重 <c>w_i = 1 / max(v_i, floor)</c>
        ''' </summary>
        ''' <param name="variances"></param>
        ''' <param name="varianceFloor"></param>
        ''' <returns>方差缺失的研究权重为 0</returns>
        Public Function InverseVarianceWeights(variances As Double(),
                                             Optional varianceFloor As Double = 0.00000001) As Double()
            If variances Is Nothing Then Throw New ArgumentNullException(NameOf(variances))

            Dim out As Double() = New Double(variances.Length - 1) {}

            For i As Integer = 0 To variances.Length - 1
                Dim v As Double = variances(i)

                If Double.IsNaN(v) OrElse v <= 0.0 Then Continue For

                out(i) = 1.0 / sysmath.Max(v, varianceFloor)
            Next

            Return out
        End Function

        ''' <summary>
        ''' 随机效应合并（对象入口）
        ''' </summary>
        ''' <param name="studies">各研究的效应量</param>
        ''' <param name="options">合并参数</param>
        ''' <returns></returns>
        Public Function Pool(studies As StudyEffect(), Optional options As RandomEffectsOptions = Nothing) As PooledEvidence
            If studies Is Nothing Then Throw New ArgumentNullException(NameOf(studies))
            If options Is Nothing Then options = RandomEffectsOptions.Default()

            Dim n As Integer = studies.Length
            Dim effects As Double() = New Double(n - 1) {}
            Dim variances As Double() = New Double(n - 1) {}
            Dim multipliers As Double() = New Double(n - 1) {}
            Dim measured As Boolean() = New Boolean(n - 1) {}

            For i As Integer = 0 To n - 1
                Dim study As StudyEffect = studies(i)

                If study Is Nothing Then Continue For

                effects(i) = study.Effect
                variances(i) = study.Variance
                measured(i) = study.IsMeasured
                multipliers(i) = If(Double.IsNaN(study.Weight), 1.0, study.Weight)

                If options.WeightMultiplier IsNot Nothing Then
                    multipliers(i) *= options.WeightMultiplier(i)
                End If
            Next

            Return PoolCore(effects, variances, multipliers, measured, MedianVariance(variances, measured), options)
        End Function

        ''' <summary>
        ''' 随机效应合并（矩阵入口，逐基因）
        ''' </summary>
        ''' <param name="effects">形状 [研究, 基因] 的效应量</param>
        ''' <param name="variances">形状 [研究, 基因] 的方差</param>
        ''' <param name="measured">形状 [研究, 基因] 的观测掩码；<c>Nothing</c> 表示全部有测量</param>
        ''' <param name="options">合并参数</param>
        ''' <returns>每个基因一个合并结果</returns>
        Public Function PoolMatrix(effects As Double(,),
                                   variances As Double(,),
                                   Optional measured As Boolean(,) = Nothing,
                                   Optional options As RandomEffectsOptions = Nothing) As PooledEvidence()

            If effects Is Nothing Then Throw New ArgumentNullException(NameOf(effects))
            If variances Is Nothing Then Throw New ArgumentNullException(NameOf(variances))
            If options Is Nothing Then options = RandomEffectsOptions.Default()

            Dim studies As Integer = effects.GetLength(0)
            Dim genes As Integer = effects.GetLength(1)

            If variances.GetLength(0) <> studies OrElse variances.GetLength(1) <> genes Then
                Throw New ArgumentException(
                    $"variance matrix shape [{variances.GetLength(0)}x{variances.GetLength(1)}] does not match effects [{studies}x{genes}]",
                    NameOf(variances))
            End If

            Dim results As PooledEvidence() = New PooledEvidence(genes - 1) {}
            Dim columnEffects As Double() = New Double(studies - 1) {}
            Dim columnVariances As Double() = New Double(studies - 1) {}
            Dim columnMeasured As Boolean() = New Boolean(studies - 1) {}
            Dim multipliers As Double() = New Double(studies - 1) {}

            For i As Integer = 0 To studies - 1
                multipliers(i) = 1.0

                If options.WeightMultiplier IsNot Nothing Then
                    multipliers(i) = options.WeightMultiplier(i)
                End If
            Next

            For g As Integer = 0 To genes - 1
                For i As Integer = 0 To studies - 1
                    columnEffects(i) = effects(i, g)
                    columnVariances(i) = variances(i, g)
                    columnMeasured(i) = If(measured Is Nothing, True, measured(i, g))
                Next

                results(g) = PoolCore(columnEffects,
                                      columnVariances,
                                      multipliers,
                                      columnMeasured,
                                      MedianVariance(columnVariances, columnMeasured),
                                      options)
            Next

            Return results
        End Function

        ''' <summary>
        ''' 合并带符号的证据：<c>z = Σ(w·z_i)/√Σw²</c>（见 <see cref="EvidenceCombination.Stouffer"/>）
        ''' </summary>
        ''' <param name="zScores"></param>
        ''' <param name="weights"></param>
        ''' <param name="measured"></param>
        ''' <returns></returns>
        Public Function CombineZ(zScores As Double(),
                                 weights As Double(),
                                 Optional measured As Boolean() = Nothing) As Double
            Return EvidenceCombination.Stouffer(zScores, weights, measured)
        End Function

        ''' <summary>
        ''' 合并核心：单基因的单遍实现
        ''' </summary>
        Friend Function PoolCore(effects As Double(),
                                 variances As Double(),
                                 multipliers As Double(),
                                 measured As Boolean(),
                                 medianVariance As Double,
                                 options As RandomEffectsOptions) As PooledEvidence

            Dim result As New PooledEvidence
            Dim n As Integer = effects.Length
            Dim floor As Double = options.VarianceFloor
            Dim relative As Double = If(Double.IsNaN(medianVariance), 0.0, medianVariance * options.RelativeVarianceFloor)
            Dim effectiveFloor As Double = sysmath.Max(floor, relative)

            Dim weights As Double() = New Double(n - 1) {}
            Dim values As Double() = New Double(n - 1) {}
            Dim used As Boolean() = New Boolean(n - 1) {}
            Dim studies As Integer = 0
            Dim s0 As Double = 0.0
            Dim s1 As Double = 0.0
            Dim s2 As Double = 0.0
            Dim squareSum As Double = 0.0

            For i As Integer = 0 To n - 1
                If Not measured(i) Then Continue For

                Dim y As Double = effects(i)
                Dim v As Double = variances(i)

                If Double.IsNaN(y) Then Continue For

                If Double.IsNaN(v) OrElse v <= 0.0 Then
                    If Not options.FallbackToMedian Then Continue For
                    If Double.IsNaN(medianVariance) OrElse medianVariance <= 0.0 Then Continue For

                    v = medianVariance
                End If

                Dim mult As Double = multipliers(i)

                If Double.IsNaN(mult) OrElse mult <= 0.0 Then mult = 1.0

                Dim w As Double = mult / sysmath.Max(v, effectiveFloor)

                weights(i) = w
                values(i) = y
                used(i) = True
                studies += 1
                s0 += w
                s1 += w * y
                s2 += w * y * y
                squareSum += w * w
            Next

            result.Studies = studies

            If studies < options.MinStudies OrElse s0 <= 0.0 Then
                result.Converged = False
                result.Diagnostics = If(studies = 0, "no measured study with a usable variance",
                                        $"only {studies} usable study/studies but {options.MinStudies} required")
                Return result
            End If

            Dim fixedMean As Double = s1 / s0
            Dim q As Double = sysmath.Max(0.0, s2 - s1 * s1 / s0)
            Dim c As Double = s0 - squareSum / s0
            Dim df As Integer = studies - 1
            Dim tau2 As Double = 0.0

            If df > 0 AndAlso c > 0.0 Then
                tau2 = sysmath.Max(0.0, (q - df) / c)
            End If

            ' 重新按 (v + τ²) 加权
            Dim numerator As Double = 0.0
            Dim denominator As Double = 0.0
            Dim squareSum2 As Double = 0.0
            Dim maxWeight As Double = 0.0

            For i As Integer = 0 To n - 1
                If Not used(i) Then Continue For

                Dim v As Double = variances(i)

                If Double.IsNaN(v) OrElse v <= 0.0 Then v = medianVariance

                Dim mult As Double = multipliers(i)

                If Double.IsNaN(mult) OrElse mult <= 0.0 Then mult = 1.0

                ' 随机效应权重：mult / (v + τ²)
                Dim w As Double = mult / (sysmath.Max(v, effectiveFloor) + tau2)

                numerator += w * values(i)
                denominator += w
                squareSum2 += w * w

                If w > maxWeight Then maxWeight = w
            Next

            If denominator <= 0.0 Then
                result.Converged = False
                result.Diagnostics = "all random-effects weights vanished"
                Return result
            End If

            Dim effect As Double = numerator / denominator
            Dim se As Double = 1.0 / sysmath.Sqrt(denominator)
            Dim z As Double = effect / se

            result.Effect = effect
            result.StandardError = se
            result.Z = z
            result.PValue = NormalDistribution.TwoSidedPValue(z)
            result.Tau2 = tau2
            result.Q = q
            result.Df = df
            result.I2 = Heterogeneity.I2(q, df)
            result.NEffective = If(squareSum2 > 0.0, denominator * denominator / squareSum2, 0.0)
            result.MaxWeightShare = If(denominator > 0.0, maxWeight / denominator, 0.0)
            result.Converged = True
            result.Diagnostics = If(tau2 = 0.0, "tau2 truncated to zero (homogeneous studies)", Nothing)

            Return result
        End Function

        ''' <summary>
        ''' 参与计算的研究的方差中位数（用于相对方差地板与缺失回填）
        ''' </summary>
        ''' <param name="variances"></param>
        ''' <param name="measured"></param>
        ''' <returns>没有可用方差时返回 <see cref="Double.NaN"/></returns>
        Friend Function MedianVariance(variances As Double(), measured As Boolean()) As Double
            Dim pool As New List(Of Double)(variances.Length)

            For i As Integer = 0 To variances.Length - 1
                If measured IsNot Nothing AndAlso Not measured(i) Then Continue For

                Dim v As Double = variances(i)

                If Not Double.IsNaN(v) AndAlso v > 0.0 Then Call pool.Add(v)
            Next

            If pool.Count = 0 Then Return Double.NaN

            Dim sorted As Double() = pool.ToArray()

            Call Array.Sort(sorted)

            Return OrderStatistics.Quantiles.MedianOfSorted(sorted)
        End Function

    End Module
End Namespace
