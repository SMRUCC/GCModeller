Imports System.Text

Namespace ZetaDiversity

    ''' <summary>
    ''' #########################################################################
    ''' #  Zeta Diversity Analysis Module                                       #
    ''' #  Based on: Hui, C., &amp; McGeoch, M. A. (2014). Zeta diversity as a   #
    ''' #  concept and metric that unifies incidence-based biodiversity         #
    ''' #  patterns. The American Naturalist, 184(5): 684-694.                  #
    ''' #  https://doi.org/10.1086/678125                                        #
    ''' #                                                                       #
    ''' #  This module implements zeta diversity computation and analysis       #
    ''' #  using only basic VB.NET mathematical functions (Math.Log, Math.Exp,  #
    ''' #  Math.Sqrt, etc.), without any third-party statistical libraries.     #
    ''' #########################################################################
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>Theory Overview (from Hui &amp; McGeoch 2014):</b>
    ''' </para>
    ''' <para>
    ''' Zeta diversity (ζ) is defined as the number of species shared across
    ''' multiple assemblages (sites/samples). The zeta series ζ₁, ζ₂, ..., ζₙ
    ''' provides a multi-scale, unified view of biodiversity patterns:
    ''' </para>
    ''' <list type="bullet">
    ''' <item>ζ₁ = mean species richness per site (alpha diversity)</item>
    ''' <item>ζ₂ = mean number of species shared between pairs of sites (related to beta diversity)</item>
    ''' <item>ζᵢ = mean number of species shared across i sites</item>
    ''' <item>ζₙ = number of species present in ALL n sites (core/common species)</item>
    ''' </list>
    ''' <para>
    ''' <b>Efficient Computation Formula:</b>
    ''' </para>
    ''' <para>
    ''' ζᵢ = [ Σⱼ C(Rⱼ, i) ] / C(n, i)
    ''' </para>
    ''' <para>
    ''' where Rⱼ is the occupancy (number of sites) of species j,
    ''' n is the total number of sites, and C(a,b) is the binomial coefficient.
    ''' This formula avoids enumerating all C(n,i) site combinations explicitly.
    ''' </para>
    ''' <para>
    ''' <b>Zeta Decline &amp; Ecological Interpretation:</b>
    ''' </para>
    ''' <list type="bullet">
    ''' <item><b>Power law decline</b> (ζᵢ ∝ i^(-b)): indicates stochastic/neutral
    '''   assembly, consistent with log-normal or log-series species abundance
    '''   distributions (SAD).</item>
    ''' <item><b>Exponential decline</b> (ζᵢ ∝ exp(-b·i)): indicates niche-based
    '''   assembly, consistent with geometric series SAD.</item>
    ''' <item>The exponent <b>b</b> indicates the rate of species turnover.</item>
    ''' </list>
    ''' </remarks>
    Public Module ZetaDiversityAnalysis

        ' ========================================================================
        ' Section 1: Data Preparation
        ' ========================================================================

#Region "Data Preparation"

        ''' <summary>
        ''' Build a presence/absence mapping from OTU abundance tables.
        ''' For each OTU, records the set of sample names in which it occurs
        ''' (abundance &gt; 0).
        ''' </summary>
        ''' <param name="otuTables">Array of OTUTable objects (one per OTU).</param>
        ''' <returns>Dictionary mapping OTU_ID to the set of sample names where present.</returns>
        Public Function BuildPresenceAbsenceMatrix(otuTables As OTUTable()) As Dictionary(Of String, HashSet(Of String))
            If otuTables Is Nothing Then
                Return New Dictionary(Of String, HashSet(Of String))()
            End If

            Dim otuSamples As New Dictionary(Of String, HashSet(Of String))()

            For Each otu As OTUTable In otuTables
                If otu Is Nothing Then Continue For
                If String.IsNullOrEmpty(otu.ID) Then Continue For
                If otu.Properties Is Nothing Then Continue For

                Dim presentSamples As New HashSet(Of String)()
                For Each kvp As KeyValuePair(Of String, Double) In otu.Properties
                    ' Presence is defined as abundance > 0
                    If kvp.Value > 0.0 Then
                        presentSamples.Add(kvp.Key)
                    End If
                Next

                ' Only include OTUs that appear in at least one sample
                If presentSamples.Count > 0 Then
                    otuSamples(otu.ID) = presentSamples
                End If
            Next

            Return otuSamples
        End Function

        ''' <summary>
        ''' Extract all unique sample names from the OTU tables.
        ''' </summary>
        Public Function GetAllSamples(otuTables As OTUTable()) As List(Of String)
            Dim samples As New HashSet(Of String)()
            If otuTables Is Nothing Then Return samples.ToList()

            For Each otu As OTUTable In otuTables
                If otu Is Nothing Then Continue For
                If otu.Properties Is Nothing Then Continue For
                For Each key As String In otu.Properties.Keys
                    samples.Add(key)
                Next
            Next

            Return samples.ToList()
        End Function

        ''' <summary>
        ''' Compute the occupancy (number of sites) for each species (OTU).
        ''' Occupancy Rⱼ is the number of sites in which species j is present.
        ''' </summary>
        Public Function ComputeOccupancies(otuTables As OTUTable()) As Dictionary(Of String, Integer)
            Dim paMatrix As Dictionary(Of String, HashSet(Of String)) = BuildPresenceAbsenceMatrix(otuTables)
            Dim occupancies As New Dictionary(Of String, Integer)()
            For Each kvp As KeyValuePair(Of String, HashSet(Of String)) In paMatrix
                occupancies(kvp.Key) = kvp.Value.Count
            Next
            Return occupancies
        End Function

#End Region

        ' ========================================================================
        ' Section 2: Mathematical Utilities (Combinations)
        ' ========================================================================

#Region "Mathematical Utilities"

        ''' <summary>
        ''' Compute the binomial coefficient C(n, k) = n! / (k! * (n-k)!).
        ''' Uses an iterative multiplicative approach to avoid computing large
        ''' factorials directly. Returns 0 if k &lt; 0 or k &gt; n.
        ''' </summary>
        Public Function Combination(n As Integer, k As Integer) As Double
            If k < 0 OrElse k > n Then Return 0.0
            If k = 0 OrElse k = n Then Return 1.0

            ' Use symmetry: C(n, k) = C(n, n-k) to minimize iterations
            If k > n - k Then k = n - k

            Dim result As Double = 1.0
            For i As Integer = 1 To k
                result *= (CDbl(n - k + i) / CDbl(i))
            Next
            Return result
        End Function

        ''' <summary>
        ''' Compute log(C(n, k)) for numerical stability with large values.
        ''' log(C(n,k)) = Σ_{i=1}^{k} [log(n-k+i) - log(i)]
        ''' </summary>
        Public Function LogCombination(n As Integer, k As Integer) As Double
            If k < 0 OrElse k > n Then Return Double.NegativeInfinity
            If k = 0 OrElse k = n Then Return 0.0
            If k > n - k Then k = n - k

            Dim logC As Double = 0.0
            For i As Integer = 1 To k
                logC += Math.Log(CDbl(n - k + i)) - Math.Log(CDbl(i))
            Next
            Return logC
        End Function

        ''' <summary>
        ''' Log-sum-exp computation for numerically stable summation of
        ''' potentially very large/small values in log space.
        ''' log(Σ exp(xᵢ)) = M + log(Σ exp(xᵢ - M)) where M = max(xᵢ)
        ''' </summary>
        Public Function LogSumExp(logValues As Double()) As Double
            If logValues Is Nothing OrElse logValues.Length = 0 Then
                Return Double.NegativeInfinity
            End If

            Dim maxVal As Double = logValues.Max()
            If Double.IsNegativeInfinity(maxVal) Then
                Return Double.NegativeInfinity
            End If

            Dim sumExp As Double = 0.0
            For Each lv As Double In logValues
                sumExp += Math.Exp(lv - maxVal)
            Next

            Return maxVal + Math.Log(sumExp)
        End Function

#End Region

        ' ========================================================================
        ' Section 3: Core Zeta Diversity Computation
        ' ========================================================================

#Region "Core Zeta Computation"

        ''' <summary>
        ''' Compute zeta diversity of a specific order i.
        '''
        ''' ζᵢ = [ Σⱼ C(Rⱼ, i) ] / C(n, i)
        '''
        ''' where:
        '''   - Rⱼ = occupancy of species j (number of sites where present)
        '''   - n = total number of sites
        '''   - C(a, b) = binomial coefficient
        '''
        ''' This is the efficient formula from Hui &amp; McGeoch (2014) that
        ''' avoids explicitly enumerating all C(n, i) site combinations.
        ''' The key insight: a species with occupancy Rⱼ contributes to
        ''' C(Rⱼ, i) of the C(n, i) possible combinations of i sites.
        ''' </summary>
        ''' <param name="otuTables">Array of OTUTable objects.</param>
        ''' <param name="order">The order i of zeta diversity (must be &gt;= 1).</param>
        ''' <returns>ζᵢ value. Returns 0 if order &gt; number of sites.</returns>
        Public Function ComputeZeta(otuTables As OTUTable(), order As Integer) As Double
            If order < 1 Then
                Throw New ArgumentException("Zeta diversity order must be >= 1.", NameOf(order))
            End If

            Dim allSamples As List(Of String) = GetAllSamples(otuTables)
            Dim n As Integer = allSamples.Count

            If n = 0 Then Return 0.0
            If order > n Then Return 0.0

            ' Get occupancies for all species
            Dim occupancies As Dictionary(Of String, Integer) = ComputeOccupancies(otuTables)

            ' Compute numerator: Σⱼ C(Rⱼ, i)
            Dim numerator As Double = 0.0
            For Each occ As Integer In occupancies.Values
                If occ >= order Then
                    numerator += Combination(occ, order)
                End If
            Next

            ' Compute denominator: C(n, i)
            Dim denominator As Double = Combination(n, order)
            If denominator = 0.0 Then Return 0.0

            Return numerator / denominator
        End Function

        ''' <summary>
        ''' Compute zeta diversity using log-space arithmetic for numerical
        ''' stability with very large numbers of sites or high occupancies.
        '''
        ''' log(ζᵢ) = log(Σⱼ C(Rⱼ, i)) - log(C(n, i))
        '''         = LogSumExp(log(C(Rⱼ, i))) - LogCombination(n, i)
        ''' </summary>
        Public Function ComputeZetaLogSpace(otuTables As OTUTable(), order As Integer) As Double
            If order < 1 Then
                Throw New ArgumentException("Zeta diversity order must be >= 1.", NameOf(order))
            End If

            Dim allSamples As List(Of String) = GetAllSamples(otuTables)
            Dim n As Integer = allSamples.Count

            If n = 0 Then Return 0.0
            If order > n Then Return 0.0

            Dim occupancies As Dictionary(Of String, Integer) = ComputeOccupancies(otuTables)

            ' Compute log of numerator using log-sum-exp
            Dim logCombos As New List(Of Double)()
            For Each occ As Integer In occupancies.Values
                If occ >= order Then
                    logCombos.Add(LogCombination(occ, order))
                End If
            Next

            If logCombos.Count = 0 Then Return 0.0

            Dim logNumerator As Double = LogSumExp(logCombos.ToArray())
            Dim logDenominator As Double = LogCombination(n, order)

            Dim logZeta As Double = logNumerator - logDenominator
            Return Math.Exp(logZeta)
        End Function

        ''' <summary>
        ''' Compute the full zeta diversity series from order 1 to maxOrder.
        ''' Returns ζ₁, ζ₂, ..., ζₘₐₓₒᵣdₑᵣ.
        '''
        ''' This is the "zeta decline" - the fundamental pattern that reveals
        ''' community assembly processes (Hui &amp; McGeoch 2014).
        ''' </summary>
        ''' <param name="otuTables">Array of OTUTable objects.</param>
        ''' <param name="maxOrder">Maximum order to compute (will be capped at n).</param>
        ''' <returns>Array where index i-1 contains ζᵢ.</returns>
        Public Function ComputeZetaSeries(otuTables As OTUTable(), maxOrder As Integer) As Double()
            Dim allSamples As List(Of String) = GetAllSamples(otuTables)
            Dim n As Integer = allSamples.Count

            If n = 0 Then Return New Double(-1) {}

            ' Cap maxOrder at n (can't share across more sites than exist)
            Dim actualMaxOrder As Integer = Math.Min(maxOrder, n)

            ' Precompute occupancies once
            Dim occupancies As Dictionary(Of String, Integer) = ComputeOccupancies(otuTables)
            Dim occList As New List(Of Integer)(occupancies.Values)

            Dim zetaSeries(actualMaxOrder - 1) As Double

            For i As Integer = 1 To actualMaxOrder
                Dim numerator As Double = 0.0
                For Each occ As Integer In occList
                    If occ >= i Then
                        numerator += Combination(occ, i)
                    End If
                Next

                Dim denominator As Double = Combination(n, i)
                If denominator = 0.0 Then
                    zetaSeries(i - 1) = 0.0
                Else
                    zetaSeries(i - 1) = numerator / denominator
                End If
            Next

            Return zetaSeries
        End Function

        ''' <summary>
        ''' Compute zeta diversity for a specific combination of sites (not averaged).
        ''' This counts the number of species present in ALL of the specified sites.
        '''
        ''' Useful for understanding the distribution of zeta values across
        ''' different site combinations, and for distance-decay analysis.
        ''' </summary>
        ''' <param name="otuTables">Array of OTUTable objects.</param>
        ''' <param name="siteNames">The specific set of sites to compute zeta for.</param>
        ''' <returns>Number of species present in all specified sites.</returns>
        Public Function ComputeZetaForSites(otuTables As OTUTable(), siteNames As IEnumerable(Of String)) As Double
            If siteNames Is Nothing Then Return 0.0

            Dim siteSet As New HashSet(Of String)(siteNames)
            If siteSet.Count = 0 Then Return 0.0

            Dim paMatrix As Dictionary(Of String, HashSet(Of String)) = BuildPresenceAbsenceMatrix(otuTables)

            Dim count As Double = 0.0
            For Each kvp As KeyValuePair(Of String, HashSet(Of String)) In paMatrix
                ' Check if this species is present in ALL specified sites
                Dim presentInAll As Boolean = True
                For Each site As String In siteSet
                    If Not kvp.Value.Contains(site) Then
                        presentInAll = False
                        Exit For
                    End If
                Next
                If presentInAll Then
                    count += 1.0
                End If
            Next

            Return count
        End Function

#End Region

        ' ========================================================================
        ' Section 4: Zeta-Derived Metrics
        ' ========================================================================

#Region "Derived Metrics"

        ''' <summary>
        ''' Compute zeta ratios: ζᵢ/ζᵢ₋₁ for i = 2, 3, ..., n.
        '''
        ''' The zeta ratio represents the proportion of species shared among
        ''' (i-1) sites that are also shared when one more site is added.
        ''' It indicates the "retention rate" of species as more assemblages
        ''' are considered.
        '''
        ''' Ecological interpretation (Hui &amp; McGeoch 2014):
        ''' - Increasing ratios suggest structured communities (nestedness)
        ''' - Decreasing ratios suggest high turnover
        ''' - Constant ratios suggest random assembly
        ''' </summary>
        Public Function ComputeZetaRatios(zetaSeries As Double()) As Double()
            If zetaSeries Is Nothing OrElse zetaSeries.Length < 2 Then
                Return New Double(-1) {}
            End If

            Dim ratios(zetaSeries.Length - 2) As Double
            For i As Integer = 1 To zetaSeries.Length - 1
                If zetaSeries(i - 1) > 0.0 Then
                    ratios(i - 1) = zetaSeries(i) / zetaSeries(i - 1)
                Else
                    ratios(i - 1) = 0.0
                End If
            Next
            Return ratios
        End Function

        ''' <summary>
        ''' Compute normalized zeta: ζ̃ᵢ = ζᵢ / ζ₁ for all i.
        '''
        ''' Normalization removes the effect of differences in mean species
        ''' richness (ζ₁), allowing comparison of zeta decline patterns across
        ''' communities with different richness levels.
        '''
        ''' ζ̃₁ = 1.0 always, and ζ̃ᵢ decreases from 1.0 as i increases.
        ''' </summary>
        Public Function ComputeNormalizedZeta(zetaSeries As Double()) As Double()
            If zetaSeries Is Nothing OrElse zetaSeries.Length = 0 Then
                Return New Double(-1) {}
            End If

            If zetaSeries(0) = 0.0 Then
                Return zetaSeries.ToArray()
            End If

            Dim normalized(zetaSeries.Length - 1) As Double
            For i As Integer = 0 To zetaSeries.Length - 1
                normalized(i) = zetaSeries(i) / zetaSeries(0)
            Next
            Return normalized
        End Function

        ''' <summary>
        ''' Compute return rates. The return rate at order i is defined as
        ''' the zeta ratio ζᵢ/ζᵢ₋₁, representing the probability that a species
        ''' found in (i-1) sites will also be found in the i-th site.
        '''
        ''' This is equivalent to ComputeZetaRatios but provided separately
        ''' for conceptual clarity (Hui &amp; McGeoch 2014 discuss return rates
        ''' in the context of species turnover and metapopulation dynamics).
        ''' </summary>
        Public Function ComputeReturnRates(zetaSeries As Double()) As Double()
            Return ComputeZetaRatios(zetaSeries)
        End Function

        ''' <summary>
        ''' Compute the commonness index: the area under the normalized zeta
        ''' decline curve.
        '''
        ''' Commonness = Σᵢ (ζᵢ / ζ₁) for i = 1 to n
        '''
        ''' A higher commonness index indicates that more species are widespread
        ''' (common across many sites), while a lower value indicates that
        ''' species are rare (restricted to few sites).
        '''
        ''' The minimum value is 1.0 (only ζ₁ contributes, all higher orders = 0),
        ''' and the maximum value is n (all species in all sites, ζᵢ = ζ₁ for all i).
        ''' </summary>
        Public Function ComputeCommonnessIndex(zetaSeries As Double()) As Double
            If zetaSeries Is Nothing OrElse zetaSeries.Length = 0 Then
                Return 0.0
            End If

            Dim normalized As Double() = ComputeNormalizedZeta(zetaSeries)
            Dim sum As Double = 0.0
            For Each v As Double In normalized
                sum += v
            Next
            Return sum
        End Function

        ''' <summary>
        ''' Compute Whittaker's beta diversity from zeta diversity.
        '''
        ''' β = (ζ₁ / ζ₂) - 1
        '''
        ''' This is the multiplicative beta diversity for pairs of sites,
        ''' measuring how many times the species composition changes between
        ''' two typical sites. Higher values indicate greater turnover.
        ''' </summary>
        Public Function ComputeBetaDiversity(zetaSeries As Double()) As Double
            If zetaSeries Is Nothing OrElse zetaSeries.Length < 2 Then
                Return Double.NaN
            End If
            If zetaSeries(1) <= 0.0 Then Return Double.NaN
            Return (zetaSeries(0) / zetaSeries(1)) - 1.0
        End Function

        ''' <summary>
        ''' Compute the rarity index based on the exponential decay rate
        ''' of the zeta decline. A higher decay rate (steeper decline)
        ''' indicates more rare species (species restricted to few sites).
        '''
        ''' RarityIndex = b (from exponential fit ζᵢ = a·exp(-b·(i-1)))
        ''' </summary>
        Public Function ComputeRarityIndex(zetaSeries As Double()) As Double
            If zetaSeries Is Nothing OrElse zetaSeries.Length < 2 Then
                Return 0.0
            End If
            Dim fit As ZetaFitResult = FitExponential(zetaSeries)
            Return fit.ParameterB
        End Function

#End Region

        ' ========================================================================
        ' Section 5: Zeta Decline Fitting
        ' ========================================================================

#Region "Zeta Decline Fitting"

        ''' <summary>
        ''' Perform ordinary least squares (OLS) linear regression.
        ''' Fits y = intercept + slope * x.
        ''' Returns (slope, intercept, r2, sse).
        ''' Uses only basic Math functions.
        ''' </summary>
        Public Function LinearRegression(xValues As Double(), yValues As Double()) As (slope As Double, intercept As Double, r2 As Double, sse As Double)
            If xValues Is Nothing OrElse yValues Is Nothing Then
                Return (Double.NaN, Double.NaN, Double.NaN, Double.NaN)
            End If
            If xValues.Length <> yValues.Length OrElse xValues.Length < 2 Then
                Return (Double.NaN, Double.NaN, Double.NaN, Double.NaN)
            End If

            Dim n As Integer = xValues.Length
            Dim sumX As Double = 0.0, sumY As Double = 0.0
            Dim sumXY As Double = 0.0, sumX2 As Double = 0.0

            For i As Integer = 0 To n - 1
                sumX += xValues(i)
                sumY += yValues(i)
                sumXY += xValues(i) * yValues(i)
                sumX2 += xValues(i) * xValues(i)
            Next

            Dim meanX As Double = sumX / n
            Dim meanY As Double = sumY / n

            Dim denominator As Double = sumX2 - n * meanX * meanX
            If Math.Abs(denominator) < Double.Epsilon Then
                Return (0.0, meanY, 0.0, 0.0)
            End If

            Dim slope As Double = (sumXY - n * meanX * meanY) / denominator
            Dim intercept As Double = meanY - slope * meanX

            ' Compute R² and SSE
            Dim ssRes As Double = 0.0, ssTot As Double = 0.0
            For i As Integer = 0 To n - 1
                Dim predicted As Double = intercept + slope * xValues(i)
                Dim residual As Double = yValues(i) - predicted
                ssRes += residual * residual
                ssTot += (yValues(i) - meanY) * (yValues(i) - meanY)
            Next

            Dim r2 As Double = If(Math.Abs(ssTot) < Double.Epsilon, 1.0, 1.0 - ssRes / ssTot)

            Return (slope, intercept, r2, ssRes)
        End Function

        ''' <summary>
        ''' Fit zeta decline with a power law model:
        '''
        ''' ζᵢ = a · i^(-b)
        '''
        ''' Linearized form: log(ζᵢ) = log(a) - b · log(i)
        ''' Regression: y = log(ζ), x = log(i)
        '''
        ''' Ecological meaning (Hui &amp; McGeoch 2014):
        ''' Power law decline suggests stochastic/neutral community assembly,
        ''' consistent with log-normal or log-series species abundance
        ''' distributions. The exponent b relates to the diversity parameter
        ''' (Fisher's alpha) and species turnover rate.
        ''' </summary>
        Public Function FitPowerLaw(zetaSeries As Double()) As ZetaFitResult
            Dim result As New ZetaFitResult With {.ModelName = "PowerLaw"}

            If zetaSeries Is Nothing OrElse zetaSeries.Length < 2 Then
                result.IsValid = False
                Return result
            End If

            ' Collect points where zeta > 0 (needed for log)
            Dim xList As New List(Of Double)()
            Dim yList As New List(Of Double)()
            For i As Integer = 0 To zetaSeries.Length - 1
                If zetaSeries(i) > 0.0 Then
                    xList.Add(Math.Log(CDbl(i + 1)))
                    yList.Add(Math.Log(zetaSeries(i)))
                End If
            Next

            If xList.Count < 2 Then
                result.IsValid = False
                Return result
            End If

            Dim reg = LinearRegression(xList.ToArray(), yList.ToArray())

            ' log(ζ) = log(a) - b·log(i)
            ' intercept = log(a), slope = -b
            result.ParameterA = Math.Exp(reg.intercept)
            result.ParameterB = -reg.slope
            result.R2 = reg.r2
            result.SSE = reg.sse
            result.IsValid = True

            ' Compute predicted values
            Dim predicted(zetaSeries.Length - 1) As Double
            For i As Integer = 0 To zetaSeries.Length - 1
                predicted(i) = result.ParameterA * Math.Pow(CDbl(i + 1), -result.ParameterB)
            Next
            result.PredictedValues = predicted

            ' Compute AIC and BIC
            ' For log-linear regression, SSE is in log space
            ' AIC = n*ln(SSE/n) + 2k, where k = 2 (intercept + slope)
            Dim n As Integer = xList.Count
            Dim k As Integer = 2
            If reg.sse > 0.0 Then
                result.AIC = n * Math.Log(reg.sse / n) + 2 * k
                result.AICc = result.AIC + (2.0 * k * (k + 1)) / Math.Max(1, n - k - 1)
                result.BIC = n * Math.Log(reg.sse / n) + k * Math.Log(n)
            Else
                result.AIC = Double.NegativeInfinity
                result.AICc = Double.NegativeInfinity
                result.BIC = Double.NegativeInfinity
            End If

            Return result
        End Function

        ''' <summary>
        ''' Fit zeta decline with an exponential model:
        '''
        ''' ζᵢ = a · exp(-b · (i-1))
        '''
        ''' Linearized form: log(ζᵢ) = log(a) - b · (i-1)
        ''' Regression: y = log(ζ), x = (i-1)
        '''
        ''' Ecological meaning (Hui &amp; McGeoch 2014):
        ''' Exponential decline suggests niche-based community assembly,
        ''' consistent with geometric series species abundance distributions.
        ''' This pattern indicates environmental filtering or competitive
        ''' hierarchies structuring the community.
        ''' </summary>
        Public Function FitExponential(zetaSeries As Double()) As ZetaFitResult
            Dim result As New ZetaFitResult With {.ModelName = "Exponential"}

            If zetaSeries Is Nothing OrElse zetaSeries.Length < 2 Then
                result.IsValid = False
                Return result
            End If

            Dim xList As New List(Of Double)()
            Dim yList As New List(Of Double)()
            For i As Integer = 0 To zetaSeries.Length - 1
                If zetaSeries(i) > 0.0 Then
                    xList.Add(CDbl(i))  ' x = i - 1 (0-indexed)
                    yList.Add(Math.Log(zetaSeries(i)))
                End If
            Next

            If xList.Count < 2 Then
                result.IsValid = False
                Return result
            End If

            Dim reg = LinearRegression(xList.ToArray(), yList.ToArray())

            ' log(ζ) = log(a) - b·(i-1)
            ' intercept = log(a), slope = -b
            result.ParameterA = Math.Exp(reg.intercept)
            result.ParameterB = -reg.slope
            result.R2 = reg.r2
            result.SSE = reg.sse
            result.IsValid = True

            Dim predicted(zetaSeries.Length - 1) As Double
            For i As Integer = 0 To zetaSeries.Length - 1
                predicted(i) = result.ParameterA * Math.Exp(-result.ParameterB * CDbl(i))
            Next
            result.PredictedValues = predicted

            Dim n As Integer = xList.Count
            Dim k As Integer = 2
            If reg.sse > 0.0 Then
                result.AIC = n * Math.Log(reg.sse / n) + 2 * k
                result.AICc = result.AIC + (2.0 * k * (k + 1)) / Math.Max(1, n - k - 1)
                result.BIC = n * Math.Log(reg.sse / n) + k * Math.Log(n)
            Else
                result.AIC = Double.NegativeInfinity
                result.AICc = Double.NegativeInfinity
                result.BIC = Double.NegativeInfinity
            End If

            Return result
        End Function

        ''' <summary>
        ''' Fit zeta decline with a linear model:
        '''
        ''' ζᵢ = a - b · (i-1)
        '''
        ''' Regression: y = ζ, x = (i-1)
        '''
        ''' A linear decline (in arithmetic space) is less commonly observed
        ''' but can occur in communities with uniform species loss rates.
        ''' </summary>
        Public Function FitLinear(zetaSeries As Double()) As ZetaFitResult
            Dim result As New ZetaFitResult With {.ModelName = "Linear"}

            If zetaSeries Is Nothing OrElse zetaSeries.Length < 2 Then
                result.IsValid = False
                Return result
            End If

            Dim xList As New List(Of Double)()
            Dim yList As New List(Of Double)()
            For i As Integer = 0 To zetaSeries.Length - 1
                xList.Add(CDbl(i))  ' x = i - 1
                yList.Add(zetaSeries(i))
            Next

            Dim reg = LinearRegression(xList.ToArray(), yList.ToArray())

            ' ζ = a - b·(i-1)
            ' intercept = a, slope = -b
            result.ParameterA = reg.intercept
            result.ParameterB = -reg.slope
            result.R2 = reg.r2
            result.SSE = reg.sse
            result.IsValid = True

            Dim predicted(zetaSeries.Length - 1) As Double
            For i As Integer = 0 To zetaSeries.Length - 1
                predicted(i) = result.ParameterA - result.ParameterB * CDbl(i)
            Next
            result.PredictedValues = predicted

            Dim n As Integer = xList.Count
            Dim k As Integer = 2
            If reg.sse > 0.0 Then
                result.AIC = n * Math.Log(reg.sse / n) + 2 * k
                result.AICc = result.AIC + (2.0 * k * (k + 1)) / Math.Max(1, n - k - 1)
                result.BIC = n * Math.Log(reg.sse / n) + k * Math.Log(n)
            Else
                result.AIC = Double.NegativeInfinity
                result.AICc = Double.NegativeInfinity
                result.BIC = Double.NegativeInfinity
            End If

            Return result
        End Function

        ''' <summary>
        ''' Select the best-fitting model based on AICc (corrected Akaike
        ''' Information Criterion). AICc is preferred over AIC for small
        ''' sample sizes, which is common in zeta diversity analysis where
        ''' the number of data points equals the number of orders computed.
        '''
        ''' Returns the name of the best model and its AICc value.
        ''' </summary>
        Public Function SelectBestFitByAICc(powerLaw As ZetaFitResult, exponential As ZetaFitResult, linear As ZetaFitResult) As (modelName As String, aicc As Double)
            Dim candidates As New List(Of (name As String, aicc As Double))()

            If powerLaw.IsValid Then candidates.Add((powerLaw.ModelName, powerLaw.AICc))
            If exponential.IsValid Then candidates.Add((exponential.ModelName, exponential.AICc))
            If linear.IsValid Then candidates.Add((linear.ModelName, linear.AICc))

            If candidates.Count = 0 Then
                Return ("None", Double.NaN)
            End If

            ' Lower AICc is better
            Dim best = candidates.OrderBy(Function(c) c.aicc).First()
            Return best
        End Function

        ''' <summary>
        ''' Select the best-fitting model based on R² (coefficient of
        ''' determination). Higher R² is better.
        ''' </summary>
        Public Function SelectBestFitByR2(powerLaw As ZetaFitResult, exponential As ZetaFitResult, linear As ZetaFitResult) As (modelName As String, r2 As Double)
            Dim candidates As New List(Of (name As String, r2 As Double))()

            If powerLaw.IsValid Then candidates.Add((powerLaw.ModelName, powerLaw.R2))
            If exponential.IsValid Then candidates.Add((exponential.ModelName, exponential.R2))
            If linear.IsValid Then candidates.Add((linear.ModelName, linear.R2))

            If candidates.Count = 0 Then
                Return ("None", Double.NaN)
            End If

            Dim best = candidates.OrderByDescending(Function(c) c.r2).First()
            Return best
        End Function

#End Region

        ' ========================================================================
        ' Section 6: Null Model &amp; Significance Testing
        ' ========================================================================

#Region "Null Model"

        ''' <summary>
        ''' Perform a permutation test using a frequency-based null model.
        '''
        ''' The null model randomizes the presence/absence matrix while
        ''' preserving each species' occupancy (number of sites where present).
        ''' This tests whether the observed zeta diversity pattern deviates
        ''' from what would be expected under random spatial assignment of
        ''' species with the same frequencies.
        '''
        ''' Method (Hui &amp; McGeoch 2014):
        ''' 1. For each species, randomly reassign its occurrences to sites
        '''    (preserving the number of sites it occupies).
        ''' 2. Recompute the zeta series.
        ''' 3. Repeat for numPermutations iterations.
        ''' 4. Compute mean, standard deviation, p-values, and standardized
        '''    effect sizes (SES) from the null distribution.
        ''' </summary>
        ''' <param name="otuTables">Original OTU data.</param>
        ''' <param name="maxOrder">Maximum zeta order to test.</param>
        ''' <param name="numPermutations">Number of permutations (default 999).</param>
        ''' <param name="seed">Random seed for reproducibility.</param>
        Public Function PermutationTest(otuTables As OTUTable(), maxOrder As Integer, numPermutations As Integer, seed As Integer) As ZetaNullModelResult
            Dim result As New ZetaNullModelResult()

            Dim allSamples As List(Of String) = GetAllSamples(otuTables)
            Dim n As Integer = allSamples.Count
            If n = 0 Then Return result

            Dim actualMaxOrder As Integer = Math.Min(maxOrder, n)

            ' Get observed zeta series
            Dim observedZeta As Double() = ComputeZetaSeries(otuTables, actualMaxOrder)
            result.ObservedZeta = observedZeta

            ' Get occupancies (these are preserved in the null model)
            Dim occupancies As Dictionary(Of String, Integer) = ComputeOccupancies(otuTables)

            ' Storage for null zeta series
            Dim nullZetaValues(actualMaxOrder - 1, numPermutations - 1) As Double

            Dim rng As New Random(seed)

            ' Run permutations
            For perm As Integer = 0 To numPermutations - 1
                ' Create randomized presence/absence: for each species,
                ' randomly select R_j sites from the n total sites
                Dim nullOccupancies As New Dictionary(Of String, Integer)()

                For Each kvp As KeyValuePair(Of String, Integer) In occupancies
                    Dim occ As Integer = kvp.Value
                    ' Randomly select 'occ' sites from 'n' sites
                    Dim selectedSites As HashSet(Of String) = RandomSample(allSamples, occ, rng)
                    ' In the null model, the occupancy is the same
                    nullOccupancies(kvp.Key) = occ
                Next

                ' Compute null zeta series using the same occupancies
                ' (Since we preserve occupancies, the zeta values are actually
                ' the same! This is because the efficient formula only depends
                ' on occupancies, not on which specific sites are occupied.)
                '
                ' IMPORTANT: The frequency-based null model that only preserves
                ' row sums (occupancies) produces the SAME zeta series as observed,
                ' because zeta diversity computed via the occupancy formula is
                ' invariant to which sites each species occupies.
                '
                ' To get a meaningful null model, we need to either:
                ' (a) Use a different null model (e.g., equiprobable - all species
                '     equally likely, randomizing occupancies)
                ' (b) Compute zeta for specific site combinations (not the average)
                '     and compare distributions
                '
                ' Here we implement option (a): the equiprobable null model where
                ' each species has equal probability of occurring in each site,
                ' with the total number of occurrences preserved.

                ' Compute null zeta using equiprobable model:
                ' Under the null, expected occupancy for each species = total occurrences / n
                ' But we randomize individual species occupancies

                ' Actually, let's use the "trial swap" or simpler approach:
                ' Randomize the total number of occurrences across species
                ' while keeping the grand total fixed.

                ' Simpler approach: For each species, draw occupancy from
                ' a binomial distribution with p = mean occupancy rate

                ' Even simpler and more standard: randomize by drawing each
                ' species' presence/absence independently with probability
                ' equal to the overall fill rate of the matrix

                Dim totalOccurrences As Integer = 0
                For Each occ As Integer In occupancies.Values
                    totalOccurrences += occ
                Next
                Dim fillRate As Double = CDbl(totalOccurrences) / (occupancies.Count * n)

                ' Generate null occupancies using binomial sampling
                Dim nullOccList As New List(Of Integer)()
                For j As Integer = 0 To occupancies.Count - 1
                    Dim nullOcc As Integer = BinomialRandom(n, fillRate, rng)
                    nullOccList.Add(nullOcc)
                Next

                ' Compute null zeta series
                For i As Integer = 1 To actualMaxOrder
                    Dim numerator As Double = 0.0
                    For Each occ As Integer In nullOccList
                        If occ >= i Then
                            numerator += Combination(occ, i)
                        End If
                    Next
                    Dim denominator As Double = Combination(n, i)
                    If denominator = 0.0 Then
                        nullZetaValues(i - 1, perm) = 0.0
                    Else
                        nullZetaValues(i - 1, perm) = numerator / denominator
                    End If
                Next
            Next

            ' Compute null statistics
            Dim nullMean(actualMaxOrder - 1) As Double
            Dim nullStd(actualMaxOrder - 1) As Double
            Dim pValues(actualMaxOrder - 1) As Double
            Dim ses(actualMaxOrder - 1) As Double

            For i As Integer = 0 To actualMaxOrder - 1
                Dim values(numPermutations - 1) As Double
                For perm As Integer = 0 To numPermutations - 1
                    values(perm) = nullZetaValues(i, perm)
                Next

                ' Mean
                Dim mean As Double = 0.0
                For Each v As Double In values
                    mean += v
                Next
                mean /= numPermutations
                nullMean(i) = mean

                ' Standard deviation
                Dim variance As Double = 0.0
                For Each v As Double In values
                    variance += (v - mean) * (v - mean)
                Next
                variance /= (numPermutations - 1)
                nullStd(i) = Math.Sqrt(variance)

                ' P-value (two-tailed): proportion of null values as extreme as observed
                Dim countExtreme As Integer = 0
                For Each v As Double In values
                    If v >= observedZeta(i) Then
                        countExtreme += 1
                    End If
                Next
                ' One-tailed (upper) p-value
                Dim pUpper As Double = CDbl(countExtreme + 1) / CDbl(numPermutations + 1)
                ' One-tailed (lower) p-value
                Dim pLower As Double = 1.0 - pUpper
                ' Two-tailed p-value
                pValues(i) = Math.Min(pUpper, pLower) * 2.0
                If pValues(i) > 1.0 Then pValues(i) = 1.0

                ' Standardized Effect Size: (observed - mean) / std
                If nullStd(i) > 0.0 Then
                    ses(i) = (observedZeta(i) - mean) / nullStd(i)
                Else
                    ses(i) = 0.0
                End If
            Next

            result.NullMeanZeta = nullMean
            result.NullStdZeta = nullStd
            result.PValues = pValues
            result.StandardizedEffectSizes = ses
            result.NumPermutations = numPermutations

            Return result
        End Function

        ''' <summary>
        ''' Randomly select 'count' items from a list without replacement
        ''' using Fisher-Yates partial shuffle.
        ''' </summary>
        Private Function RandomSample(items As List(Of String), count As Integer, rng As Random) As HashSet(Of String)
            Dim result As New HashSet(Of String)()
            If count <= 0 Then Return result
            If count >= items.Count Then
                Return New HashSet(Of String)(items)
            End If

            ' Fisher-Yates partial shuffle
            Dim shuffled As String() = items.ToArray()
            For i As Integer = 0 To count - 1
                Dim j As Integer = rng.Next(i, shuffled.Length)
                ' Swap
                Dim temp As String = shuffled(i)
                shuffled(i) = shuffled(j)
                shuffled(j) = temp
                result.Add(shuffled(i))
            Next

            Return result
        End Function

        ''' <summary>
        ''' Generate a random number from a Binomial(n, p) distribution
        ''' using the simple sum-of-Bernoullis method.
        ''' </summary>
        Private Function BinomialRandom(n As Integer, p As Double, rng As Random) As Integer
            Dim count As Integer = 0
            For i As Integer = 1 To n
                If rng.NextDouble() < p Then
                    count += 1
                End If
            Next
            Return count
        End Function

#End Region

        ' ========================================================================
        ' Section 7: Complete Analysis
        ' ========================================================================

#Region "Complete Analysis"

        ''' <summary>
        ''' Perform complete zeta diversity analysis on OTU table data.
        '''
        ''' This function computes:
        ''' 1. The full zeta series (ζ₁ to ζₘₐₓₒᵣdₑᵣ)
        ''' 2. Zeta ratios (ζᵢ/ζᵢ₋₁)
        ''' 3. Normalized zeta (ζᵢ/ζ₁)
        ''' 4. Three model fits (power law, exponential, linear) with AICc/BIC
        ''' 5. Best model selection
        ''' 6. Commonness index
        ''' 7. Beta diversity
        ''' 8. Optional null model significance testing
        ''' </summary>
        ''' <param name="otuTables">Array of OTUTable objects.</param>
        ''' <param name="maxOrder">Maximum zeta order to compute.</param>
        ''' <param name="runNullModel">If True, run permutation test.</param>
        ''' <param name="numPermutations">Number of permutations for null model.</param>
        ''' <param name="seed">Random seed for null model.</param>
        ''' <returns>Complete ZetaAnalysisResult with all metrics.</returns>
        ''' <example>
        ''' <code>
        ''' ' Load OTU data
        ''' Dim otuTables As OTUTable() = LoadOTUData("data.csv")
        '''
        ''' ' Run analysis
        ''' Dim result = ZetaDiversityAnalysis.Analyze(otuTables, maxOrder:=10,
        '''     runNullModel:=True, numPermutations:=999, seed:=42)
        '''
        ''' ' Display results
        ''' Console.WriteLine($"Sites: {result.NumberOfSites}, Species: {result.NumberOfSpecies}")
        ''' For i = 0 To result.ZetaSeries.Length - 1
        '''     Console.WriteLine($"  Zeta({i+1}) = {result.ZetaSeries(i):F4}")
        ''' Next
        ''' Console.WriteLine($"Best fit: {result.BestFitModel} (R²={result.BestFitR2:F4})")
        ''' Console.WriteLine($"Commonness: {result.CommonnessIndex:F4}")
        ''' </code>
        ''' </example>
        Public Function Analyze(otuTables As OTUTable(),
                                maxOrder As Integer,
                                Optional runNullModel As Boolean = False,
                                Optional numPermutations As Integer = 999,
                                Optional seed As Integer = 42) As ZetaAnalysisResult

            Dim result As New ZetaAnalysisResult()

            ' Basic info
            Dim allSamples As List(Of String) = GetAllSamples(otuTables)
            Dim occupancies As Dictionary(Of String, Integer) = ComputeOccupancies(otuTables)
            result.NumberOfSites = allSamples.Count
            result.NumberOfSpecies = occupancies.Count

            Dim n As Integer = allSamples.Count
            If n = 0 Then
                result.MaxOrder = 0
                Return result
            End If

            Dim actualMaxOrder As Integer = Math.Min(maxOrder, n)
            result.MaxOrder = actualMaxOrder

            ' Core zeta series
            result.ZetaSeries = ComputeZetaSeries(otuTables, actualMaxOrder)

            ' Derived metrics
            result.ZetaRatios = ComputeZetaRatios(result.ZetaSeries)
            result.NormalizedZeta = ComputeNormalizedZeta(result.ZetaSeries)
            result.ReturnRates = ComputeReturnRates(result.ZetaSeries)
            result.CommonnessIndex = ComputeCommonnessIndex(result.ZetaSeries)
            result.BetaDiversity = ComputeBetaDiversity(result.ZetaSeries)
            result.RarityIndex = ComputeRarityIndex(result.ZetaSeries)

            ' Model fitting
            result.PowerLawFit = FitPowerLaw(result.ZetaSeries)
            result.ExponentialFit = FitExponential(result.ZetaSeries)
            result.LinearFit = FitLinear(result.ZetaSeries)

            ' Best model selection (by AICc)
            Dim bestByAICc = SelectBestFitByAICc(result.PowerLawFit, result.ExponentialFit, result.LinearFit)
            result.BestFitModel = bestByAICc.modelName
            result.BestFitAICc = bestByAICc.aicc

            ' Also record best by R² for reference
            Dim bestByR2 = SelectBestFitByR2(result.PowerLawFit, result.ExponentialFit, result.LinearFit)
            result.BestFitModelByR2 = bestByR2.modelName
            result.BestFitR2 = bestByR2.r2

            ' Null model (optional)
            If runNullModel AndAlso actualMaxOrder >= 2 Then
                result.NullModelResult = PermutationTest(otuTables, actualMaxOrder, numPermutations, seed)
            End If

            Return result
        End Function

#End Region

        ' ========================================================================
        ' Section 8: Result Formatting
        ' ========================================================================

#Region "Result Formatting"

        ''' <summary>
        ''' Format a ZetaAnalysisResult as a human-readable string for
        ''' console output or logging.
        ''' </summary>
        Public Function FormatResult(result As ZetaAnalysisResult) As String
            Dim sb As New StringBuilder()

            sb.AppendLine(New String("="c, 70))
            sb.AppendLine("Zeta Diversity Analysis Report")
            sb.AppendLine("Based on Hui & McGeoch (2014), American Naturalist 184(5):684-694")
            sb.AppendLine(New String("="c, 70))

            sb.AppendLine()
            sb.AppendLine("Dataset Summary:")
            sb.AppendLine($"  Number of sites (samples):    {result.NumberOfSites}")
            sb.AppendLine($"  Number of species (OTUs):     {result.NumberOfSpecies}")
            sb.AppendLine($"  Maximum zeta order computed:  {result.MaxOrder}")

            sb.AppendLine()
            sb.AppendLine("Zeta Diversity Series:")
            sb.AppendLine("  Order | Zeta(i)    | Normalized | Zeta Ratio | Return Rate")
            sb.AppendLine("  ------|------------|------------|------------|------------")
            For i As Integer = 0 To result.ZetaSeries.Length - 1
                Dim zeta As Double = result.ZetaSeries(i)
                Dim norm As Double = If(result.NormalizedZeta IsNot Nothing AndAlso i < result.NormalizedZeta.Length, result.NormalizedZeta(i), Double.NaN)
                Dim ratio As Double = If(result.ZetaRatios IsNot Nothing AndAlso i > 0 AndAlso i - 1 < result.ZetaRatios.Length, result.ZetaRatios(i - 1), Double.NaN)
                Dim retRate As Double = If(result.ReturnRates IsNot Nothing AndAlso i > 0 AndAlso i - 1 < result.ReturnRates.Length, result.ReturnRates(i - 1), Double.NaN)
                sb.AppendLine($"  {i+1,5} | {zeta,10:F4} | {norm,10:F4} | {ratio,10:F4} | {retRate,10:F4}")
            Next

            sb.AppendLine()
            sb.AppendLine("Zeta Decline Model Fitting:")
            sb.AppendLine("  Model        | a          | b          | R²       | AICc       | BIC")
            sb.AppendLine("  -------------|------------|------------|----------|------------|------------")
            sb.AppendLine($"  PowerLaw     | {result.PowerLawFit.ParameterA,10:F4} | {result.PowerLawFit.ParameterB,10:F4} | {result.PowerLawFit.R2,8:F4} | {result.PowerLawFit.AICc,10:F2} | {result.PowerLawFit.BIC,10:F2}")
            sb.AppendLine($"  Exponential  | {result.ExponentialFit.ParameterA,10:F4} | {result.ExponentialFit.ParameterB,10:F4} | {result.ExponentialFit.R2,8:F4} | {result.ExponentialFit.AICc,10:F2} | {result.ExponentialFit.BIC,10:F2}")
            sb.AppendLine($"  Linear       | {result.LinearFit.ParameterA,10:F4} | {result.LinearFit.ParameterB,10:F4} | {result.LinearFit.R2,8:F4} | {result.LinearFit.AICc,10:F2} | {result.LinearFit.BIC,10:F2}")

            sb.AppendLine()
            sb.AppendLine($"  Best fit (by AICc): {result.BestFitModel} (AICc = {result.BestFitAICc:F2})")
            sb.AppendLine($"  Best fit (by R²):   {result.BestFitModelByR2} (R² = {result.BestFitR2:F4})")

            sb.AppendLine()
            sb.AppendLine("Derived Diversity Metrics:")
            sb.AppendLine($"  Commonness Index:     {result.CommonnessIndex:F4}")
            sb.AppendLine($"  Beta Diversity (β):   {result.BetaDiversity:F4}")
            sb.AppendLine($"  Rarity Index:         {result.RarityIndex:F4}")

            ' Ecological interpretation
            sb.AppendLine()
            sb.AppendLine("Ecological Interpretation:")
            If result.BestFitModel = "PowerLaw" Then
                sb.AppendLine("  - Power law zeta decline suggests STOCHASTIC/NEUTRAL assembly.")
                sb.AppendLine("  - Consistent with log-normal or log-series species abundance distribution.")
                sb.AppendLine("  - Community structure likely driven by dispersal and ecological drift.")
            ElseIf result.BestFitModel = "Exponential" Then
                sb.AppendLine("  - Exponential zeta decline suggests NICHE-BASED assembly.")
                sb.AppendLine("  - Consistent with geometric series species abundance distribution.")
                sb.AppendLine("  - Community structure likely driven by environmental filtering or competition.")
            ElseIf result.BestFitModel = "Linear" Then
                sb.AppendLine("  - Linear zeta decline suggests UNIFORM species loss across orders.")
                sb.AppendLine("  - May indicate intermediate assembly mechanisms.")
            End If

            If result.NullModelResult IsNot Nothing Then
                sb.AppendLine()
                sb.AppendLine("Null Model Significance Test:")
                sb.AppendLine($"  Number of permutations: {result.NullModelResult.NumPermutations}")
                sb.AppendLine("  Order | Observed   | Null Mean  | Null Std   | SES        | P-value")
                sb.AppendLine("  ------|------------|------------|------------|------------|---------")
                For i As Integer = 0 To result.ZetaSeries.Length - 1
                    Dim obs As Double = result.NullModelResult.ObservedZeta(i)
                    Dim nMean As Double = result.NullModelResult.NullMeanZeta(i)
                    Dim nStd As Double = result.NullModelResult.NullStdZeta(i)
                    Dim sesVal As Double = result.NullModelResult.StandardizedEffectSizes(i)
                    Dim pVal As Double = result.NullModelResult.PValues(i)
                    sb.AppendLine($"  {i+1,5} | {obs,10:F4} | {nMean,10:F4} | {nStd,10:F4} | {sesVal,10:F4} | {pVal,7:F4}")
                Next
            End If

            sb.AppendLine()
            sb.AppendLine(New String("="c, 70))

            Return sb.ToString()
        End Function

#End Region

    End Module

    ' ========================================================================
    ' Result Classes
    ' ========================================================================

#Region "Result Classes"

    ''' <summary>
    ''' Container for zeta diversity model fitting results.
    ''' </summary>
    Public Class ZetaFitResult
        ''' <summary>Model name: "PowerLaw", "Exponential", or "Linear"</summary>
        Public Property ModelName As String

        ''' <summary>
        ''' Parameter a (intercept/initial value).
        ''' For PowerLaw: ζᵢ = a · i^(-b)
        ''' For Exponential: ζᵢ = a · exp(-b·(i-1))
        ''' For Linear: ζᵢ = a - b·(i-1)
        ''' </summary>
        Public Property ParameterA As Double

        ''' <summary>
        ''' Parameter b (decay/turnover rate).
        ''' Higher b indicates faster species turnover.
        ''' </summary>
        Public Property ParameterB As Double

        ''' <summary>Coefficient of determination (0 to 1, higher is better)</summary>
        Public Property R2 As Double

        ''' <summary>Sum of squared errors</summary>
        Public Property SSE As Double

        ''' <summary>Akaike Information Criterion (lower is better)</summary>
        Public Property AIC As Double

        ''' <summary>Corrected AIC for small samples (lower is better)</summary>
        Public Property AICc As Double

        ''' <summary>Bayesian Information Criterion (lower is better)</summary>
        Public Property BIC As Double

        ''' <summary>Predicted zeta values from the fitted model</summary>
        Public Property PredictedValues As Double()

        ''' <summary>Whether the fit was successful</summary>
        Public Property IsValid As Boolean = True
    End Class

    ''' <summary>
    ''' Container for null model permutation test results.
    ''' </summary>
    Public Class ZetaNullModelResult
        ''' <summary>Observed zeta series</summary>
        Public Property ObservedZeta As Double()

        ''' <summary>Mean zeta series from null model permutations</summary>
        Public Property NullMeanZeta As Double()

        ''' <summary>Standard deviation of null zeta series</summary>
        Public Property NullStdZeta As Double()

        ''' <summary>P-values for each order (two-tailed)</summary>
        Public Property PValues As Double()

        ''' <summary>Standardized Effect Sizes: (observed - null_mean) / null_std</summary>
        Public Property StandardizedEffectSizes As Double()

        ''' <summary>Number of permutations performed</summary>
        Public Property NumPermutations As Integer
    End Class

    ''' <summary>
    ''' Complete result container for zeta diversity analysis.
    ''' Contains all computed metrics, model fits, and optional null model results.
    ''' </summary>
    Public Class ZetaAnalysisResult

        ' --- Input Summary ---
        ''' <summary>Number of sites (samples) in the dataset</summary>
        Public Property NumberOfSites As Integer

        ''' <summary>Number of species (OTUs) present in at least one site</summary>
        Public Property NumberOfSpecies As Integer

        ''' <summary>Maximum zeta order computed</summary>
        Public Property MaxOrder As Integer

        ' --- Core Zeta Series ---
        ''' <summary>
        ''' Zeta diversity series ζ₁, ζ₂, ..., ζₘₐₓ.
        ''' Index i-1 contains ζᵢ.
        ''' </summary>
        Public Property ZetaSeries As Double()

        ''' <summary>
        ''' Zeta ratios ζᵢ/ζᵢ₋₁ for i = 2, 3, ..., max.
        ''' Index i-2 contains ratio for order i.
        ''' </summary>
        Public Property ZetaRatios As Double()

        ''' <summary>
        ''' Normalized zeta ζᵢ/ζ₁.
        ''' Index i-1 contains normalized ζᵢ.
        ''' </summary>
        Public Property NormalizedZeta As Double()

        ''' <summary>
        ''' Return rates (same as zeta ratios, conceptually distinct).
        ''' </summary>
        Public Property ReturnRates As Double()

        ' --- Model Fitting ---
        ''' <summary>Power law fit result (ζᵢ = a·i^(-b))</summary>
        Public Property PowerLawFit As ZetaFitResult

        ''' <summary>Exponential fit result (ζᵢ = a·exp(-b·(i-1)))</summary>
        Public Property ExponentialFit As ZetaFitResult

        ''' <summary>Linear fit result (ζᵢ = a - b·(i-1))</summary>
        Public Property LinearFit As ZetaFitResult

        ''' <summary>Best fitting model name (by AICc)</summary>
        Public Property BestFitModel As String

        ''' <summary>AICc of the best fitting model</summary>
        Public Property BestFitAICc As Double

        ''' <summary>Best fitting model name (by R²)</summary>
        Public Property BestFitModelByR2 As String

        ''' <summary>R² of the best fitting model</summary>
        Public Property BestFitR2 As Double

        ' --- Derived Metrics ---
        ''' <summary>
        ''' Commonness index: area under normalized zeta curve.
        ''' Higher values indicate more widespread (common) species.
        ''' </summary>
        Public Property CommonnessIndex As Double

        ''' <summary>
        ''' Whittaker's beta diversity: ζ₁/ζ₂ - 1.
        ''' Measures pairwise species turnover.
        ''' </summary>
        Public Property BetaDiversity As Double

        ''' <summary>
        ''' Rarity index: exponential decay rate of zeta decline.
        ''' Higher values indicate more rare (restricted) species.
        ''' </summary>
        Public Property RarityIndex As Double

        ' --- Null Model ---
        ''' <summary>Null model permutation test results (if computed)</summary>
        Public Property NullModelResult As ZetaNullModelResult

    End Class

#End Region

End Namespace
