' ============================================================================
' FELLA Algorithm - VB.NET Implementation
' Statistics.vb - Statistical functions for FELLA enrichment analysis
' 
' Implements:
' - Normal distribution CDF (Abramowitz & Stegun approximation)
' - Inverse normal CDF (Rational approximation)
' - Hypergeometric distribution PMF and CDF
' - Gamma function (Lanczos approximation)
' - Incomplete gamma function
' - Beta function
' - BH (Benjamini-Hochberg) multiple testing correction
' - Monte Carlo permutation utilities
' ============================================================================

Namespace Math

    ''' <summary>
    ''' Statistical utility functions required for FELLA enrichment analysis.
    ''' All implementations use only basic .NET math functions (System.Math).
    ''' </summary>
    Public NotInheritable Class Statistics

        Private Sub New()
        End Sub

        ' ====================================================================
        ' Normal Distribution
        ' ====================================================================

        ''' <summary>
        ''' Standard normal CDF: P(Z <= x) using Abramowitz & Stegun approximation.
        ''' Maximum absolute error: 7.5e-8
        ''' </summary>
        Public Shared Function NormalCDF(x As Double) As Double
            ' Constants for A&S approximation 26.2.17
            Const a1 As Double = 0.254829592
            Const a2 As Double = -0.284496736
            Const a3 As Double = 1.421413741
            Const a4 As Double = -1.453152027
            Const a5 As Double = 1.061405429
            Const p As Double = 0.3275911

            Dim sign As Integer = If(x < 0, -1, 1)
            x = System.Math.Abs(x) / System.Math.Sqrt(2.0)

            Dim t As Double = 1.0 / (1.0 + p * x)
            Dim y As Double = 1.0 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * System.Math.Exp(-x * x)

            Return 0.5 * (1.0 + sign * y)
        End Function

        ''' <summary>
        ''' Inverse of the standard normal CDF (quantile function).
        ''' Uses rational approximation by Peter Acklam.
        ''' </summary>
        Public Shared Function NormalInverseCDF(p As Double) As Double
            If p <= 0 Then Return Double.NegativeInfinity
            If p >= 1 Then Return Double.PositiveInfinity
            If p = 0.5 Then Return 0.0

            ' Coefficients for rational approximation
            Const a1 As Double = -3.969683028665376e+01
            Const a2 As Double = 2.209460984245205e+02
            Const a3 As Double = -2.759285104469687e+02
            Const a4 As Double = 1.383577518672690e+02
            Const a5 As Double = -3.066479806614716e+01
            Const a6 As Double = 2.506628277459239e+00

            Const b1 As Double = -5.447609879822406e+01
            Const b2 As Double = 1.615858368580409e+02
            Const b3 As Double = -1.556989798598866e+02
            Const b4 As Double = 6.680131188771972e+01
            Const b5 As Double = -1.328068155288572e+01

            Const c1 As Double = -7.784894002430293e-03
            Const c2 As Double = -3.223964580411365e-01
            Const c3 As Double = -2.400758277161838e+00
            Const c4 As Double = -2.549732539343734e+00
            Const c5 As Double = 4.374664141464968e+00
            Const c6 As Double = 2.938163982698783e+00

            Const d1 As Double = 7.784695709041462e-03
            Const d2 As Double = 3.224671290700398e-01
            Const d3 As Double = 2.445134137142996e+00
            Const d4 As Double = 3.754408661907416e+00

            Dim pLow As Double = 0.02425
            Dim pHigh As Double = 1 - pLow
            Dim q, r As Double

            If p < pLow Then
                ' Rational approximation for lower region
                q = System.Math.Sqrt(-2 * System.Math.Log(p))
                Return (((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) / _
                       ((((d1 * q + d2) * q + d3) * q + d4) * q + 1)
            ElseIf p <= pHigh Then
                ' Rational approximation for central region
                q = p - 0.5
                r = q * q
                Return (((((a1 * r + a2) * r + a3) * r + a4) * r + a5) * r + a6) * q / _
                       (((((b1 * r + b2) * r + b3) * r + b4) * r + b5) * r + 1)
            Else
                ' Rational approximation for upper region
                q = System.Math.Sqrt(-2 * System.Math.Log(1 - p))
                Return -(((((c1 * q + c2) * q + c3) * q + c4) * q + c5) * q + c6) / _
                        ((((d1 * q + d2) * q + d3) * q + d4) * q + 1)
            End If
        End Function

        ''' <summary>
        ''' Compute z-score from observed value, mean, and standard deviation.
        ''' </summary>
        Public Shared Function ZScore(observed As Double, mean As Double, stdDev As Double) As Double
            If stdDev <= 0 Then Return 0.0
            Return (observed - mean) / stdDev
        End Function

        ''' <summary>
        ''' Compute p-score from z-score using normal CDF.
        ''' In FELLA, p-score = Phi(z) where z = (score - mu) / sigma.
        ''' Lower p-scores indicate more significant nodes.
        ''' </summary>
        Public Shared Function PScoreFromZScore(z As Double) As Double
            Return NormalCDF(z)
        End Function

        ' ====================================================================
        ' Student's t Distribution
        ' ====================================================================

        ''' <summary>
        ''' Student's t-distribution CDF using regularized incomplete beta function.
        ''' P(T <= t) with df degrees of freedom.
        ''' </summary>
        Public Shared Function StudentTCDF(t As Double, df As Double) As Double
            Dim x As Double = df / (df + t * t)
            ' P(T <= t) = 1 - 0.5 * I_x(df/2, 1/2) for t > 0
            ' where I_x is the regularized incomplete beta function
            Dim ibeta As Double = RegularizedIncompleteBeta(df / 2.0, 0.5, x)
            If t >= 0 Then
                Return 1.0 - 0.5 * ibeta
            Else
                Return 0.5 * ibeta
            End If
        End Function

        ''' <summary>
        ''' Compute p-score using Student's t approximation.
        ''' </summary>
        Public Shared Function PScoreFromT(z As Double, df As Double) As Double
            Return StudentTCDF(z, df)
        End Function

        ' ====================================================================
        ' Gamma Distribution
        ' ====================================================================

        ''' <summary>
        ''' Compute p-score using Gamma approximation.
        ''' Shape = E^2/V, Scale = V/E, then p-score = GammaCDF(score; shape, scale).
        ''' </summary>
        Public Shared Function PScoreFromGamma(score As Double, mean As Double, variance As Double) As Double
            If variance <= 0 OrElse mean <= 0 Then Return 1.0
            Dim shape As Double = mean * mean / variance
            Dim scale As Double = variance / mean
            Return GammaCDF(score, shape, scale)
        End Function

        ''' <summary>
        ''' Gamma distribution CDF using regularized incomplete gamma function.
        ''' </summary>
        Public Shared Function GammaCDF(x As Double, shape As Double, scale As Double) As Double
            If x <= 0 Then Return 0.0
            ' P(X <= x) = lower_gamma(shape, x/scale) / Gamma(shape)
            Return RegularizedIncompleteGamma(shape, x / scale)
        End Function

        ' ====================================================================
        ' Gamma Function (Lanczos approximation)
        ' ====================================================================

        ''' <summary>
        ''' Log of the Gamma function using Lanczos approximation.
        ''' </summary>
        Public Shared Function LogGamma(x As Double) As Double
            ' Lanczos approximation coefficients (g=7)
            Dim c() As Double = {
                0.99999999999980993,
                676.5203681218851,
                -1259.1392167224028,
                771.32342877765313,
                -176.61502916214059,
                12.507343278686905,
                -0.13857109526572012,
                9.9843695780195716E-06,
                1.5056327351493116E-07
            }

            If x < 0.5 Then
                ' Use reflection formula: Gamma(z)*Gamma(1-z) = pi/sin(pi*z)
                Return System.Math.Log(System.Math.PI / System.Math.Sin(System.Math.PI * x)) - LogGamma(1 - x)
            End If

            x -= 1
            Dim a As Double = c(0)
            Dim t As Double = x + 7.5 ' g + 0.5

            For i = 1 To 8
                a += c(i) / (x + i)
            Next

            Return 0.5 * System.Math.Log(2 * System.Math.PI) + (x + 0.5) * System.Math.Log(t) - t + System.Math.Log(a)
        End Function

        ''' <summary>
        ''' Gamma function using Lanczos approximation.
        ''' </summary>
        Public Shared Function GammaFunc(x As Double) As Double
            If x <= 0 AndAlso System.Math.Floor(x) = x Then
                Throw New ArgumentException("Gamma function is undefined for non-positive integers")
            End If
            Return System.Math.Exp(LogGamma(x))
        End Function

        ' ====================================================================
        ' Incomplete Gamma Function
        ' ====================================================================

        ''' <summary>
        ''' Regularized lower incomplete gamma function P(a, x) = gamma(a,x)/Gamma(a).
        ''' Uses series expansion and continued fraction representation.
        ''' </summary>
        Public Shared Function RegularizedIncompleteGamma(a As Double, x As Double) As Double
            If x < 0 Then Return 0.0
            If x = 0 Then Return 0.0

            If x < a + 1 Then
                ' Use series expansion
                Return GammaSeries(a, x)
            Else
                ' Use continued fraction
                Return 1.0 - GammaContinuedFraction(a, x)
            End If
        End Function

        ''' <summary>
        ''' Series expansion for the regularized lower incomplete gamma function.
        ''' </summary>
        Private Shared Function GammaSeries(a As Double, x As Double) As Double
            Dim maxIter As Integer = 200
            Dim eps As Double = 1e-12

            Dim ap As Double = a
            Dim sum As Double = 1.0 / a
            Dim del As Double = sum

            For n = 1 To maxIter
                ap += 1
                del *= x / ap
                sum += del
                If System.Math.Abs(del) < System.Math.Abs(sum) * eps Then Exit For
            Next

            Return sum * System.Math.Exp(-x + a * System.Math.Log(x) - LogGamma(a))
        End Function

        ''' <summary>
        ''' Continued fraction representation for the upper incomplete gamma function.
        ''' Returns Q(a,x) = 1 - P(a,x).
        ''' Uses modified Lentz's method.
        ''' </summary>
        Private Shared Function GammaContinuedFraction(a As Double, x As Double) As Double
            Dim maxIter As Integer = 200
            Dim eps As Double = 1e-12
            Dim tiny As Double = 1e-30

            Dim b As Double = x + 1 - a
            Dim c As Double = 1.0 / tiny
            Dim d As Double = 1.0 / b
            Dim h As Double = d

            For i = 1 To maxIter
                Dim an As Double = -i * (i - a)
                b += 2
                d = an * d + b
                If System.Math.Abs(d) < tiny Then d = tiny
                c = b + an / c
                If System.Math.Abs(c) < tiny Then c = tiny
                d = 1.0 / d
                Dim del As Double = d * c
                h *= del
                If System.Math.Abs(del - 1.0) < eps Then Exit For
            Next

            Return System.Math.Exp(-x + a * System.Math.Log(x) - LogGamma(a)) * h
        End Function

        ' ====================================================================
        ' Beta Function and Regularized Incomplete Beta
        ' ====================================================================

        ''' <summary>
        ''' Log of the Beta function.
        ''' </summary>
        Public Shared Function LogBeta(a As Double, b As Double) As Double
            Return LogGamma(a) + LogGamma(b) - LogGamma(a + b)
        End Function

        ''' <summary>
        ''' Beta function B(a,b).
        ''' </summary>
        Public Shared Function BetaFunc(a As Double, b As Double) As Double
            Return System.Math.Exp(LogBeta(a, b))
        End Function

        ''' <summary>
        ''' Regularized incomplete beta function I_x(a,b).
        ''' Uses continued fraction representation.
        ''' </summary>
        Public Shared Function RegularizedIncompleteBeta(a As Double, b As Double, x As Double) As Double
            If x <= 0 Then Return 0.0
            If x >= 1 Then Return 1.0

            ' Use symmetry relation if x > (a+1)/(a+b+2)
            If x > (a + 1) / (a + b + 2) Then
                Return 1.0 - RegularizedIncompleteBeta(b, a, 1 - x)
            End If

            ' Continued fraction using modified Lentz's method
            Dim maxIter As Integer = 200
            Dim eps As Double = 1e-12
            Dim tiny As Double = 1e-30

            Dim qab As Double = a + b
            Dim qap As Double = a + 1
            Dim qam As Double = a - 1
            Dim c As Double = 1.0
            Dim d As Double = 1.0 - qab * x / qap
            If System.Math.Abs(d) < tiny Then d = tiny
            d = 1.0 / d
            Dim h As Double = d

            For m = 1 To maxIter
                Dim m2 As Integer = 2 * m

                ' Even step
                Dim aa As Double = m * (b - m) * x / ((qam + m2) * (a + m2))
                d = 1.0 + aa * d
                If System.Math.Abs(d) < tiny Then d = tiny
                c = 1.0 + aa / c
                If System.Math.Abs(c) < tiny Then c = tiny
                d = 1.0 / d
                h *= d * c

                ' Odd step
                aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2))
                d = 1.0 + aa * d
                If System.Math.Abs(d) < tiny Then d = tiny
                c = 1.0 + aa / c
                If System.Math.Abs(c) < tiny Then c = tiny
                d = 1.0 / d
                Dim del As Double = d * c
                h *= del

                If System.Math.Abs(del - 1.0) < eps Then Exit For
            Next

            Return System.Math.Exp(a * System.Math.Log(x) + b * System.Math.Log(1 - x) - System.Math.Log(a) - LogBeta(a, b)) * h
        End Function

        ' ====================================================================
        ' Hypergeometric Distribution
        ' ====================================================================

        ''' <summary>
        ''' Log of the binomial coefficient: log(C(n,k))
        ''' </summary>
        Public Shared Function LogBinomial(n As Integer, k As Integer) As Double
            If k < 0 OrElse k > n Then Return Double.NegativeInfinity
            If k = 0 OrElse k = n Then Return 0.0
            Return LogGamma(n + 1) - LogGamma(k + 1) - LogGamma(n - k + 1)
        End Function

        ''' <summary>
        ''' Hypergeometric PMF: P(X = k) where X ~ Hypergeometric(M, n, N)
        ''' M = population size (total compounds in background)
        ''' n = number of success states in population (compounds in pathway)
        ''' N = number of draws (input compounds)
        ''' k = number of observed successes (input compounds in pathway)
        ''' </summary>
        Public Shared Function HypergeometricPMF(k As Integer, M As Integer, n As Integer, N As Integer) As Double
            If k < System.Math.Max(0, N + n - M) OrElse k > System.Math.Min(n, N) Then
                Return 0.0
            End If
            Dim logP As Double = LogBinomial(n, k) + LogBinomial(M - n, N - k) - LogBinomial(M, N)
            Return System.Math.Exp(logP)
        End Function

        ''' <summary>
        ''' Hypergeometric CDF: P(X <= k) where X ~ Hypergeometric(M, n, N)
        ''' Used for over-representation analysis (one-tailed test).
        ''' </summary>
        Public Shared Function HypergeometricCDF(k As Integer, M As Integer, n As Integer, N As Integer) As Double
            Dim kMin As Integer = System.Math.Max(0, N + n - M)
            Dim kMax As Integer = System.Math.Min(n, N)
            If k < kMin Then Return 0.0
            If k >= kMax Then Return 1.0

            Dim sum As Double = 0.0
            For i = kMin To k
                sum += HypergeometricPMF(i, M, n, N)
            Next
            Return sum
        End Function

        ''' <summary>
        ''' Hypergeometric test p-value (one-tailed, upper tail).
        ''' Tests whether the overlap between input compounds and a pathway
        ''' is greater than expected by chance.
        ''' p-value = P(X >= k) = 1 - P(X <= k-1)
        ''' </summary>
        Public Shared Function HypergeometricPValue(k As Integer, M As Integer, n As Integer, N As Integer) As Double
            If k = 0 Then Return 1.0
            Return 1.0 - HypergeometricCDF(k - 1, M, n, N)
        End Function

        ' ====================================================================
        ' Multiple Testing Correction
        ' ====================================================================

        ''' <summary>
        ''' Benjamini-Hochberg procedure for FDR control.
        ''' Returns adjusted p-values.
        ''' </summary>
        Public Shared Function BenjaminiHochberg(pValues As Double()) As Double()
            Dim n As Integer = pValues.Length
            If n = 0 Then Return New Double() {}

            ' Create index array for sorting
            Dim indices(n - 1) As Integer
            For i = 0 To n - 1
                indices(i) = i
            Next

            ' Sort indices by p-value (ascending)
            Array.Sort(indices, Function(a, b) pValues(a).CompareTo(pValues(b)))

            ' Compute adjusted p-values
            Dim adjusted(n - 1) As Double
            Dim minSoFar As Double = 1.0

            ' Process from largest to smallest rank
            For rankIdx = n - 1 To 0 Step -1
                Dim i As Integer = indices(rankIdx)
                Dim rank As Integer = rankIdx + 1
                Dim bhValue As Double = pValues(i) * n / rank
                ' Enforce monotonicity
                minSoFar = System.Math.Min(minSoFar, bhValue)
                adjusted(i) = System.Math.Min(minSoFar, 1.0)
            Next

            Return adjusted
        End Function

        ' ====================================================================
        ' Monte Carlo Permutation
        ' ====================================================================

        ''' <summary>
        ''' Generate a random permutation of integers 0 to n-1 using Fisher-Yates shuffle.
        ''' </summary>
        Public Shared Function RandomPermutation(n As Integer, rng As Random) As Integer()
            Dim perm(n - 1) As Integer
            For i = 0 To n - 1
                perm(i) = i
            Next
            For i = n - 1 To 1 Step -1
                Dim j As Integer = rng.Next(i + 1)
                Dim temp = perm(i)
                perm(i) = perm(j)
                perm(j) = temp
            Next
            Return perm
        End Function

        ''' <summary>
        ''' Randomly select k items from a population of n items.
        ''' Returns the indices of selected items.
        ''' </summary>
        Public Shared Function RandomSample(n As Integer, k As Integer, rng As Random) As Integer()
            If k > n Then Throw New ArgumentException("Cannot sample more items than population size")
            If k = n Then
                Dim result(n - 1) As Integer
                For i = 0 To n - 1 : result(i) = i : Next
                Return result
            End If

            ' Use reservoir sampling for efficiency
            Dim selected(k - 1) As Integer
            For i = 0 To k - 1
                selected(i) = i
            Next
            For i = k To n - 1
                Dim j As Integer = rng.Next(i + 1)
                If j < k Then
                    selected(j) = i
                End If
            Next
            Return selected
        End Function

        ''' <summary>
        ''' Compute empirical p-value from Monte Carlo simulation.
        ''' p = (count_ge + 1) / (niter + 1)
        ''' where count_ge is the number of trials with score >= observed.
        ''' </summary>
        Public Shared Function EmpiricalPValue(observed As Double, nullScores As Double()) As Double
            If nullScores Is Nothing OrElse nullScores.Length = 0 Then Return 1.0
            Dim countGe As Integer = 0
            For Each s In nullScores
                If s >= observed Then countGe += 1
            Next
            Return (countGe + 1.0) / (nullScores.Length + 1.0)
        End Function

        ' ====================================================================
        ' Utility Functions
        ' ====================================================================

        ''' <summary>
        ''' Compute mean of a double array.
        ''' </summary>
        Public Shared Function Mean(values As Double()) As Double
            If values Is Nothing OrElse values.Length = 0 Then Return 0.0
            Dim sum As Double = 0.0
            For Each v In values
                sum += v
            Next
            Return sum / values.Length
        End Function

        ''' <summary>
        ''' Compute variance of a double array (sample variance with Bessel's correction).
        ''' </summary>
        Public Shared Function Variance(values As Double()) As Double
            If values Is Nothing OrElse values.Length < 2 Then Return 0.0
            Dim m As Double = Mean(values)
            Dim sumSq As Double = 0.0
            For Each v In values
                sumSq += (v - m) * (v - m)
            Next
            Return sumSq / (values.Length - 1)
        End Function

        ''' <summary>
        ''' Compute standard deviation.
        ''' </summary>
        Public Shared Function StdDev(values As Double()) As Double
            Return System.Math.Sqrt(Variance(values))
        End Function

        ''' <summary>
        ''' Compute mean and variance in a single pass.
        ''' </summary>
        Public Shared Sub ComputeMeanVariance(values As Double(), ByRef mean As Double, ByRef variance As Double)
            If values Is Nothing OrElse values.Length = 0 Then
                mean = 0.0
                variance = 0.0
                Return
            End If

            Dim n As Integer = values.Length
            Dim sum As Double = 0.0
            For Each v In values
                sum += v
            Next
            mean = sum / n

            If n < 2 Then
                variance = 0.0
                Return
            End If

            Dim sumSq As Double = 0.0
            For Each v In values
                sumSq += (v - mean) * (v - mean)
            Next
            variance = sumSq / (n - 1)
        End Sub

    End Class

End Namespace
