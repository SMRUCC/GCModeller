' ============================================================================
' FELLA Algorithm - VB.NET Implementation
' RunDiffusion.vb - Network diffusion analysis
' 
' Implements the FELLA diffusion method:
' 1. Create input vector y where y[i] = 1 for input compound nodes, 0 otherwise
' 2. Compute diffusion scores: f = K * y, where K = (L + gamma*B)^{-1}
' 3. Compute p-scores using normality approximation or Monte Carlo simulation
' 
' The diffusion process models how perturbation "flows" from input compounds
' through the KEGG network. Nodes that receive more flow are more likely
' to be affected by the perturbation.
' ============================================================================

Namespace Core

    ''' <summary>
    ''' Network diffusion analysis for FELLA.
    ''' 
    ''' The diffusion method uses the regularized Laplacian kernel to propagate
    ''' perturbation signals from input compounds through the KEGG network.
    ''' 
    ''' Mathematical formulation:
    '''   L_gamma = L + gamma * B  (regularized Laplacian)
    '''   K = L_gamma^{-1}         (diffusion kernel)
    '''   f = K * y                (diffusion scores)
    ''' 
    ''' where:
    '''   L = D - A (unnormalized graph Laplacian)
    '''   D = diagonal degree matrix
    '''   A = adjacency matrix
    '''   B = diagonal "leak" matrix (1 for pathway nodes, 0 otherwise)
    '''   gamma = regularization parameter (default 1.0)
    '''   y = input vector (1 for input compounds, 0 otherwise)
    ''' 
    ''' P-score computation (normality approximation):
    '''   z_i = (f_i - mu_i) / sigma_i
    '''   p_i = Phi(z_i)
    ''' 
    ''' where mu_i and sigma_i are the mean and standard deviation of the
    ''' null distribution for node i, computed analytically.
    ''' </summary>
    Public Class RunDiffusion

        ''' <summary>
        ''' Run diffusion analysis with specified approximation method.
        ''' </summary>
        ''' <param name="user">FELLA user object with defined input compounds</param>
        ''' <param name="data">FELLA data object with precomputed diffusion kernel</param>
        ''' <param name="approx">Approximation method for p-value computation</param>
        ''' <param name="nSim">Number of Monte Carlo iterations (only used if approx = Simulation)</param>
        ''' <returns>Enrichment result with diffusion scores and p-scores for all nodes</returns>
        Public Shared Function Run(user As FellaUser,
                                    data As FellaData,
                                    approx As ApproximationMethod,
                                    Optional nSim As Integer = 1000) As EnrichmentResult
            If Not user.HasValidInput Then
                Throw New InvalidOperationException("No valid input compounds defined")
            End If
            If data.DiffusionKernel Is Nothing Then
                Throw New InvalidOperationException("Diffusion kernel not built. Call BuildDiffusionKernel() first.")
            End If

            Dim result As New EnrichmentResult With {
                .Method = EnrichmentMethod.Diffusion,
                .Approximation = approx,
                .NSim = nSim
            }

            ' Step 1: Create input vector y
            Dim n = data.Graph.NodeCount
            Dim y(n - 1) As Double
            For Each id In user.InputCompounds
                Dim idx = data.Graph.GetIndex(id)
                If idx >= 0 Then
                    y(idx) = 1.0
                End If
            Next

            ' Step 2: Compute diffusion scores f = K * y
            Dim f = data.DiffusionKernel.MultiplyVector(y)

            ' Step 3: Compute p-scores
            Select Case approx
                Case ApproximationMethod.Normality
                    ComputePScoresNormality(f, data, result)
                Case ApproximationMethod.Simulation
                    ComputePScoresSimulation(f, y, data, result, nSim)
            End Select

            ' Step 4: Apply BH correction
            result.ApplyBHCorrection()

            Return result
        End Function

        ''' <summary>
        ''' Compute p-scores using the normality approximation.
        ''' For each node i:
        '''   z_i = (f_i - mu_i) / sigma_i
        '''   p_i = Phi(z_i)
        ''' where mu_i and sigma_i are precomputed null distribution statistics.
        ''' </summary>
        Private Shared Sub ComputePScoresNormality(f As Double(), data As FellaData,
                                                     result As EnrichmentResult)
            Dim n = data.Graph.NodeCount

            For i = 0 To n - 1
                Dim node = data.Graph.GetNode(i)
                Dim zScore As Double = 0.0
                Dim pScore As Double = 1.0

                If data.DiffusionNullStats IsNot Nothing AndAlso data.DiffusionNullStats.ContainsKey(i) Then
                    Dim stats = data.DiffusionNullStats(i)
                    If stats.StdDev > 0 Then
                        zScore = Math.Statistics.ZScore(f(i), stats.Mean, stats.StdDev)
                        pScore = Math.Statistics.PScoreFromZScore(zScore)
                    End If
                End If

                result.NodeResults.Add(New NodeResult With {
                    .NodeIndex = i,
                    .KeggId = node.Id,
                    .Name = node.Name,
                    .NodeType = node.NodeType,
                    .RawScore = f(i),
                    .ZScore = zScore,
                    .PScore = pScore,
                    .AdjustedPValue = pScore
                })
            Next
        End Sub

        ''' <summary>
        ''' Compute p-scores using Monte Carlo simulation.
        ''' For each permutation:
        '''   1. Randomly sample nInput compounds from the background
        '''   2. Compute diffusion scores for the random input
        '''   3. Record the scores
        ''' 
        ''' P-score for node i = (count of null scores >= observed + 1) / (nSim + 1)
        ''' </summary>
        Private Shared Sub ComputePScoresSimulation(f As Double(), y As Double(),
                                                      data As FellaData,
                                                      result As EnrichmentResult,
                                                      nSim As Integer)
            Dim n = data.Graph.NodeCount
            Dim nInput = user_InputCount(y)
            Dim compoundIndices = data.Graph.GetIndicesByType(KeggNodeType.Compound)
            Dim nBackground = compoundIndices.Count

            ' Store null scores for each node
            Dim nullScores(n - 1, nSim - 1) As Double
            Dim rng As New Random(42)

            For sim = 0 To nSim - 1
                ' Random sample of input compounds
                Dim yNull(n - 1) As Double
                Dim sampled = Math.Statistics.RandomSample(nBackground, nInput, rng)
                For Each sIdx In sampled
                    yNull(compoundIndices(sIdx)) = 1.0
                Next

                ' Compute diffusion scores for null input
                Dim fNull = data.DiffusionKernel.MultiplyVector(yNull)

                For i = 0 To n - 1
                    nullScores(i, sim) = fNull(i)
                Next
            Next

            ' Compute empirical p-values
            For i = 0 To n - 1
                Dim node = data.Graph.GetNode(i)

                ' Extract null scores for this node
                Dim nodeNullScores(nSim - 1) As Double
                For sim = 0 To nSim - 1
                    nodeNullScores(sim) = nullScores(i, sim)
                Next

                Dim pScore = Math.Statistics.EmpiricalPValue(f(i), nodeNullScores)

                ' Compute z-score from null distribution
                Dim nullMean = Math.Statistics.Mean(nodeNullScores)
                Dim nullStd = Math.Statistics.StdDev(nodeNullScores)
                Dim zScore = If(nullStd > 0, (f(i) - nullMean) / nullStd, 0.0)

                result.NodeResults.Add(New NodeResult With {
                    .NodeIndex = i,
                    .KeggId = node.Id,
                    .Name = node.Name,
                    .NodeType = node.NodeType,
                    .RawScore = f(i),
                    .ZScore = zScore,
                    .PScore = pScore,
                    .AdjustedPValue = pScore
                })
            Next
        End Sub

        ''' <summary>Count non-zero entries in input vector</summary>
        Private Shared Function user_InputCount(y As Double()) As Integer
            Dim count = 0
            For Each v In y
                If v > 0 Then count += 1
            Next
            Return count
        End Function

    End Class

End Namespace
