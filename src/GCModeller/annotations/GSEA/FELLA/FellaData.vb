' ============================================================================
' FELLA Algorithm - VB.NET Implementation
' FellaData.vb - Precomputed database for FELLA enrichment analysis
' 
' Equivalent to the FELLA.DATA object in the R package.
' Stores the KEGG graph and precomputed matrices for efficient analysis:
' - Hypergeometric matrix (pathway-compound incidence)
' - Diffusion matrix (regularized Laplacian inverse)
' - PageRank matrix
' - Precomputed null distribution statistics (mean, variance per node)
' ============================================================================

Namespace Core

    ''' <summary>
    ''' Precomputed database for FELLA analysis.
    ''' Contains the KEGG graph and all precomputed matrices needed
    ''' for the three enrichment methods: hypergeometric, diffusion, PageRank.
    ''' 
    ''' This is equivalent to the FELLA.DATA S4 class in the R package.
    ''' Building this database is a one-time operation per organism;
    ''' it can be serialized and reloaded for subsequent analyses.
    ''' </summary>
    Public Class FellaData
        ''' <summary>The KEGG multi-layer graph</summary>
        Public Property Graph As KeggGraph

        ''' <summary>Background compound IDs (all compounds in the graph)</summary>
        Public Property BackgroundCompounds As New HashSet(Of String)

        ''' <summary>Pathway-compound incidence matrix for hypergeometric test.
        ''' Rows = pathways, Columns = compounds.
        ''' H[p,c] = 1 if compound c belongs to pathway p.
        ''' </summary>
        Public Property HypergeomMatrix As Math.Matrix

        ''' <summary>Precomputed inverse of the regularized Laplacian for diffusion.
        ''' K = (L + gamma*B)^{-1}
        ''' Diffusion scores = K * input_vector
        ''' </summary>
        Public Property DiffusionKernel As Math.Matrix

        ''' <summary>Precomputed matrix for PageRank.
        ''' PR = (1-d) * (I - d*M)^{-1} * p
        ''' where M is the transition matrix, d is damping factor, p is personalization.
        ''' </summary>
        Public Property PagerankMatrix As Math.Matrix

        ''' <summary>Damping factor for PageRank (default 0.85)</summary>
        Public Property DampingFactor As Double = 0.85

        ''' <summary>Regularization parameter gamma for diffusion (default 1.0)</summary>
        Public Property Gamma As Double = 1.0

        ''' <summary>
        ''' Precomputed null distribution statistics for diffusion.
        ''' For each node, stores (mean, variance) under the null distribution
        ''' where input compounds are randomly sampled from the background.
        ''' </summary>
        Public Property DiffusionNullStats As Dictionary(Of Integer, NullDistributionStats)

        ''' <summary>
        ''' Precomputed null distribution statistics for PageRank.
        ''' </summary>
        Public Property PagerankNullStats As Dictionary(Of Integer, NullDistributionStats)

        ''' <summary>
        ''' Statistics of the null distribution for a single node.
        ''' Used for the normality approximation in p-value computation.
        ''' </summary>
        Public Class NullDistributionStats
            ''' <summary>Mean of diffusion/PageRank scores under null</summary>
            Public Property Mean As Double

            ''' <summary>Variance of diffusion/PageRank scores under null</summary>
            Public Property Variance As Double

            ''' <summary>Standard deviation</summary>
            Public ReadOnly Property StdDev As Double
                Get
                    Return System.Math.Sqrt(Variance)
                End Get
            End Property
        End Class

        ''' <summary>
        ''' Build the hypergeometric matrix (pathway-compound incidence).
        ''' For each pathway, identifies which compounds are reachable
        ''' through the graph hierarchy (pathway -> module -> enzyme -> reaction -> compound).
        ''' </summary>
        Public Sub BuildHypergeomMatrix()
            If Graph Is Nothing Then Throw New InvalidOperationException("Graph must be set first")

            Dim pathways = Graph.GetNodesByType(KeggNodeType.Pathway)
            Dim compounds = Graph.GetNodesByType(KeggNodeType.Compound)

            Dim nPathways = pathways.Count
            Dim nCompounds = compounds.Count

            HypergeomMatrix = New Math.Matrix(nPathways, nCompounds)

            ' For each pathway, find all compounds reachable through the hierarchy
            For pIdx = 0 To nPathways - 1
                Dim pathway = pathways(pIdx)
                Dim reachable = FindReachableCompounds(pathway.Index)
                For Each compoundIdx In reachable
                    ' Find column index for this compound
                    Dim cIdx As Integer = 0
                    For c = 0 To nCompounds - 1
                        If compounds(c).Index = compoundIdx Then
                            cIdx = c
                            Exit For
                        End If
                    Next
                    HypergeomMatrix(pIdx, cIdx) = 1.0
                Next
            Next
        End Sub

        ''' <summary>
        ''' Find all compound nodes reachable from a given node through the graph.
        ''' Uses BFS traversal following edges.
        ''' </summary>
        Private Function FindReachableCompounds(startIndex As Integer) As HashSet(Of Integer)
            Dim reachable As New HashSet(Of Integer)
            Dim visited As New HashSet(Of Integer)
            Dim queue As New Queue(Of Integer)

            queue.Enqueue(startIndex)
            visited.Add(startIndex)

            While queue.Count > 0
                Dim current = queue.Dequeue()
                Dim node = Graph.GetNode(current)

                If node.NodeType = KeggNodeType.Compound Then
                    reachable.Add(current)
                End If

                ' Traverse to neighbors
                Dim neighbors = Graph.GetNeighbors(current)
                For Each neighbor In neighbors
                    If Not visited.Contains(neighbor) Then
                        ' Only traverse "downward" in hierarchy for hypergeometric:
                        ' pathway -> module -> enzyme -> reaction -> compound
                        Dim neighborNode = Graph.GetNode(neighbor)
                        If neighborNode.NodeType > node.NodeType OrElse
                           neighborNode.NodeType = KeggNodeType.Compound Then
                            visited.Add(neighbor)
                            queue.Enqueue(neighbor)
                        End If
                    End If
                Next
            End While

            Return reachable
        End Function

        ''' <summary>
        ''' Build the diffusion kernel matrix.
        ''' K = (L + gamma * B)^{-1}
        ''' where L is the unnormalized Laplacian, B is the diagonal "leak" matrix
        ''' (1 for pathway nodes, 0 otherwise), and gamma is the regularization parameter.
        ''' 
        ''' The diffusion score for input vector y is: f = K * y
        ''' </summary>
        Public Sub BuildDiffusionKernel()
            If Graph Is Nothing Then Throw New InvalidOperationException("Graph must be set first")

            Dim Lgamma = Graph.ComputeRegularizedLaplacian(Gamma)
            DiffusionKernel = Lgamma.Inverse()
        End Sub

        ''' <summary>
        ''' Build the PageRank matrix.
        ''' PR_matrix = (1-d) * (I - d*M)^{-1}
        ''' where M is the column-stochastic transition matrix and d is the damping factor.
        ''' 
        ''' PageRank scores for personalization vector p: pr = PR_matrix * p
        ''' </summary>
        Public Sub BuildPagerankMatrix()
            If Graph Is Nothing Then Throw New InvalidOperationException("Graph must be set first")

            Dim n = Graph.NodeCount
            Dim M = Graph.ComputeTransitionMatrix()
            Dim I = Math.Matrix.Identity(n)
            Dim d = DampingFactor

            ' (I - d*M)
            Dim A = I - d * M

            ' (I - d*M)^{-1}
            Dim Ainv = A.Inverse()

            ' (1-d) * (I - d*M)^{-1}
            PagerankMatrix = (1 - d) * Ainv
        End Sub

        ''' <summary>
        ''' Precompute null distribution statistics for diffusion using normality approximation.
        ''' For each node i, computes E[f_i] and Var(f_i) under the null hypothesis
        ''' that the input compounds are randomly sampled from the background.
        ''' 
        ''' Uses the analytical formulas:
        ''' E[f_i] = (n_input / n_background) * sum_j K[i,j]  (for compound nodes j)
        ''' Var[f_i] = (n_input / n_background) * (1 - n_input/n_background) * 
        '''            sum_j K[i,j]^2 + correction terms
        ''' </summary>
        Public Sub BuildDiffusionNullStats(nInput As Integer)
            If DiffusionKernel Is Nothing Then Throw New InvalidOperationException("Diffusion kernel must be built first")

            Dim n = Graph.NodeCount
            Dim compoundIndices = Graph.GetIndicesByType(KeggNodeType.Compound)
            Dim nBackground = compoundIndices.Count

            If nBackground = 0 Then Return

            Dim p As Double = nInput / CDbl(nBackground)

            DiffusionNullStats = New Dictionary(Of Integer, NullDistributionStats)

            For i = 0 To n - 1
                Dim stats As New NullDistributionStats

                ' E[f_i] = p * sum_{j in compounds} K[i,j]
                Dim sumK As Double = 0.0
                Dim sumK2 As Double = 0.0
                For Each j In compoundIndices
                    Dim kij = DiffusionKernel(i, j)
                    sumK += kij
                    sumK2 += kij * kij
                Next

                stats.Mean = p * sumK

                ' Variance under hypergeometric sampling
                ' Var[f_i] = p*(1-p) * sumK2 + p^2 * (correction for sampling without replacement)
                ' Simplified: use p*(1-p)*sumK2 as main term
                stats.Variance = p * (1 - p) * sumK2

                ' Avoid zero variance
                If stats.Variance < 1e-15 Then
                    stats.Variance = 1e-15
                End If

                DiffusionNullStats(i) = stats
            Next
        End Sub

        ''' <summary>
        ''' Precompute null distribution statistics for PageRank using normality approximation.
        ''' Similar to diffusion but uses the PageRank matrix instead.
        ''' </summary>
        Public Sub BuildPagerankNullStats(nInput As Integer)
            If PagerankMatrix Is Nothing Then Throw New InvalidOperationException("PageRank matrix must be built first")

            Dim n = Graph.NodeCount
            Dim compoundIndices = Graph.GetIndicesByType(KeggNodeType.Compound)
            Dim nBackground = compoundIndices.Count

            If nBackground = 0 Then Return

            Dim p As Double = nInput / CDbl(nBackground)

            PagerankNullStats = New Dictionary(Of Integer, NullDistributionStats)

            For i = 0 To n - 1
                Dim stats As New NullDistributionStats

                Dim sumK As Double = 0.0
                Dim sumK2 As Double = 0.0
                For Each j In compoundIndices
                    Dim kij = PagerankMatrix(i, j)
                    sumK += kij
                    sumK2 += kij * kij
                Next

                stats.Mean = p * sumK
                stats.Variance = p * (1 - p) * sumK2

                If stats.Variance < 1e-15 Then
                    stats.Variance = 1e-15
                End If

                PagerankNullStats(i) = stats
            Next
        End Sub

        ''' <summary>
        ''' Build all precomputed data structures.
        ''' </summary>
        Public Sub BuildAll(Optional nInput As Integer = 10,
                             Optional buildHypergeom As Boolean = True,
                             Optional buildDiffusion As Boolean = True,
                             Optional buildPagerank As Boolean = True)
            If buildHypergeom Then BuildHypergeomMatrix()
            If buildDiffusion Then
                BuildDiffusionKernel()
                BuildDiffusionNullStats(nInput)
            End If
            If buildPagerank Then
                BuildPagerankMatrix()
                BuildPagerankNullStats(nInput)
            End If
        End Sub

    End Class

End Namespace
