' ============================================================================
' FELLA Algorithm - VB.NET Implementation
' RunPagerank.vb - Personalized PageRank analysis
' 
' Implements the FELLA PageRank method:
' 1. Create personalization vector p where p[i] = 1/n_input for input compounds
' 2. Compute PageRank scores: pr = (1-d) * (I - d*M)^{-1} * p
' 3. Compute p-scores using normality approximation or Monte Carlo simulation
' 
' PageRank models a random surfer who:
' - With probability d, follows a random edge from the current node
' - With probability (1-d), jumps to a random input compound
' The steady-state distribution gives the PageRank scores.
' ============================================================================

Namespace Core

    ''' <summary>
    ''' Personalized PageRank analysis for FELLA.
    ''' 
    ''' The PageRank method uses the personalized PageRank algorithm to propagate
    ''' importance from input compounds through the KEGG network.
    ''' 
    ''' Mathematical formulation:
    '''   M = transition matrix (column-stochastic)
    '''   d = damping factor (default 0.85)
    '''   p = personalization vector (uniform over input compounds)
    '''   PR = (1-d) * (I - d*M)^{-1} * p
    ''' 
    ''' The damping factor d controls the trade-off between:
    ''' - Following the network structure (d close to 1)
    ''' - Restarting from input compounds (d close to 0)
    ''' 
    ''' P-score computation follows the same approach as diffusion:
    '''   z_i = (pr_i - mu_i) / sigma_i
    '''   p_i = Phi(z_i)
    ''' </summary>
    Public Class RunPagerank

        ''' <summary>
        ''' Run PageRank analysis with specified approximation method.
        ''' </summary>
        ''' <param name="user">FELLA user object with defined input compounds</param>
        ''' <param name="data">FELLA data object with precomputed PageRank matrix</param>
        ''' <param name="approx">Approximation method for p-value computation</param>
        ''' <param name="nSim">Number of Monte Carlo iterations (only used if approx = Simulation)</param>
        ''' <returns>Enrichment result with PageRank scores and p-scores for all nodes</returns>
        Public Shared Function Run(user As FellaUser,
                                    data As FellaData,
                                    approx As ApproximationMethod,
                                    Optional nSim As Integer = 1000) As EnrichmentResult
            If Not user.HasValidInput Then
                Throw New InvalidOperationException("No valid input compounds defined")
            End If
            If data.PagerankMatrix Is Nothing Then
                Throw New InvalidOperationException("PageRank matrix not built. Call BuildPagerankMatrix() first.")
            End If

            Dim result As New EnrichmentResult With {
                .Method = EnrichmentMethod.PageRank,
                .Approximation = approx,
                .NSim = nSim
            }

            ' Step 1: Create personalization vector p
            ' p[i] = 1/n_input for input compound nodes, 0 otherwise
            Dim n = data.Graph.NodeCount
            Dim p(n - 1) As Double
            Dim nInput = user.InputCompounds.Count
            For Each id In user.InputCompounds
                Dim idx = data.Graph.GetIndex(id)
                If idx >= 0 Then
                    p(idx) = 1.0 / nInput
                End If
            Next

            ' Step 2: Compute PageRank scores: pr = PR_matrix * p
            Dim pr = data.PagerankMatrix.MultiplyVector(p)

            ' Step 3: Compute p-scores
            Select Case approx
                Case ApproximationMethod.Normality
                    ComputePScoresNormality(pr, data, result, nInput)
                Case ApproximationMethod.Simulation
                    ComputePScoresSimulation(pr, p, data, result, nSim, nInput)
            End Select

            ' Step 4: Apply BH correction
            result.ApplyBHCorrection()

            Return result
        End Function

        ''' <summary>
        ''' Compute p-scores using the normality approximation.
        ''' </summary>
        Private Shared Sub ComputePScoresNormality(pr As Double(), data As FellaData,
                                                     result As EnrichmentResult,
                                                     nInput As Integer)
            Dim n = data.Graph.NodeCount

            For i = 0 To n - 1
                Dim node = data.Graph.GetNode(i)
                Dim zScore As Double = 0.0
                Dim pScore As Double = 1.0

                If data.PagerankNullStats IsNot Nothing AndAlso data.PagerankNullStats.ContainsKey(i) Then
                    Dim stats = data.PagerankNullStats(i)
                    If stats.StdDev > 0 Then
                        zScore = Math.Statistics.ZScore(pr(i), stats.Mean, stats.StdDev)
                        pScore = Math.Statistics.PScoreFromZScore(zScore)
                    End If
                End If

                result.NodeResults.Add(New NodeResult With {
                    .NodeIndex = i,
                    .KeggId = node.Id,
                    .Name = node.Name,
                    .NodeType = node.NodeType,
                    .RawScore = pr(i),
                    .ZScore = zScore,
                    .PScore = pScore,
                    .AdjustedPValue = pScore
                })
            Next
        End Sub

        ''' <summary>
        ''' Compute p-scores using Monte Carlo simulation.
        ''' </summary>
        Private Shared Sub ComputePScoresSimulation(pr As Double(), p As Double(),
                                                      data As FellaData,
                                                      result As EnrichmentResult,
                                                      nSim As Integer,
                                                      nInput As Integer)
            Dim n = data.Graph.NodeCount
            Dim compoundIndices = data.Graph.GetIndicesByType(KeggNodeType.Compound)
            Dim nBackground = compoundIndices.Count

            ' Store null scores for each node
            Dim nullScores(n - 1, nSim - 1) As Double
            Dim rng As New Random(42)

            For sim = 0 To nSim - 1
                ' Random sample of input compounds
                Dim pNull(n - 1) As Double
                Dim sampled = Math.Statistics.RandomSample(nBackground, nInput, rng)
                For Each sIdx In sampled
                    pNull(compoundIndices(sIdx)) = 1.0 / nInput
                Next

                ' Compute PageRank scores for null input
                Dim prNull = data.PagerankMatrix.MultiplyVector(pNull)

                For i = 0 To n - 1
                    nullScores(i, sim) = prNull(i)
                Next
            Next

            ' Compute empirical p-values
            For i = 0 To n - 1
                Dim node = data.Graph.GetNode(i)

                Dim nodeNullScores(nSim - 1) As Double
                For sim = 0 To nSim - 1
                    nodeNullScores(sim) = nullScores(i, sim)
                Next

                Dim pScore = Math.Statistics.EmpiricalPValue(pr(i), nodeNullScores)

                Dim nullMean = Math.Statistics.Mean(nodeNullScores)
                Dim nullStd = Math.Statistics.StdDev(nodeNullScores)
                Dim zScore = If(nullStd > 0, (pr(i) - nullMean) / nullStd, 0.0)

                result.NodeResults.Add(New NodeResult With {
                    .NodeIndex = i,
                    .KeggId = node.Id,
                    .Name = node.Name,
                    .NodeType = node.NodeType,
                    .RawScore = pr(i),
                    .ZScore = zScore,
                    .PScore = pScore,
                    .AdjustedPValue = pScore
                })
            Next
        End Sub

    End Class

End Namespace
