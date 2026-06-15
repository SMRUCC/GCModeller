' ============================================================================
' FELLA Algorithm - VB.NET Implementation
' RunHypergeom.vb - Hypergeometric over-representation analysis
' 
' Implements the classic hypergeometric test for pathway enrichment.
' For each pathway, tests whether the overlap between input compounds
' and compounds belonging to the pathway is greater than expected by chance.
' 
' This is the simplest of the three FELLA methods and does not use
' network topology - it treats each pathway independently.
' ============================================================================

Namespace Core

    ''' <summary>
    ''' Hypergeometric over-representation analysis.
    ''' 
    ''' For each pathway in the KEGG graph, computes a p-value using the
    ''' hypergeometric test:
    '''   M = total number of compounds in the background
    '''   n = number of compounds in the pathway
    '''   N = number of input compounds
    '''   k = number of input compounds in the pathway
    '''   p-value = P(X >= k) where X ~ Hypergeometric(M, n, N)
    ''' 
    ''' Lower p-values indicate pathways with more overlap than expected.
    ''' </summary>
    Public Class RunHypergeom

        ''' <summary>
        ''' Run hypergeometric enrichment analysis.
        ''' </summary>
        ''' <param name="user">FELLA user object with defined input compounds</param>
        ''' <param name="data">FELLA data object with precomputed hypergeometric matrix</param>
        ''' <returns>Enrichment result with p-scores for each pathway</returns>
        Public Shared Function Run(user As FellaUser, data As FellaData) As EnrichmentResult
            If Not user.HasValidInput Then
                Throw New InvalidOperationException("No valid input compounds defined")
            End If
            If data.HypergeomMatrix Is Nothing Then
                Throw New InvalidOperationException("Hypergeometric matrix not built. Call BuildHypergeomMatrix() first.")
            End If

            Dim result As New EnrichmentResult With {
                .Method = EnrichmentMethod.Hypergeometric,
                .Approximation = ApproximationMethod.Normality
            }

            Dim pathways = data.Graph.GetNodesByType(KeggNodeType.Pathway)
            Dim compounds = data.Graph.GetNodesByType(KeggNodeType.Compound)

            ' M = total compounds in background
            Dim M As Integer = user.BackgroundCompounds.Count

            ' N = number of input compounds
            Dim N As Integer = user.InputCompounds.Count

            ' Input compound indices
            Dim inputIndices As New HashSet(Of Integer)
            For Each id In user.InputCompounds
                Dim idx = data.Graph.GetIndex(id)
                If idx >= 0 Then inputIndices.Add(idx)
            Next

            ' For each pathway, compute hypergeometric p-value
            For pIdx = 0 To pathways.Count - 1
                Dim pathway = pathways(pIdx)

                ' Count compounds in this pathway (n) and overlap (k)
                Dim n As Integer = 0 ' compounds in pathway
                Dim k As Integer = 0 ' input compounds in pathway

                For cIdx = 0 To compounds.Count - 1
                    If data.HypergeomMatrix(pIdx, cIdx) > 0 Then
                        ' Check if this compound is in the background
                        If user.BackgroundCompounds.Contains(compounds(cIdx).Id) Then
                            n += 1
                            ' Check if this compound is in the input
                            If inputIndices.Contains(compounds(cIdx).Index) Then
                                k += 1
                            End If
                        End If
                    End If
                Next

                ' Compute hypergeometric p-value
                Dim pValue As Double = 1.0
                If n > 0 AndAlso k > 0 Then
                    pValue = Math.Statistics.HypergeometricPValue(k, M, n, N)
                End If

                ' Create result entry
                Dim nodeResult As New NodeResult With {
                    .NodeIndex = pathway.Index,
                    .KeggId = pathway.Id,
                    .Name = pathway.Name,
                    .NodeType = KeggNodeType.Pathway,
                    .RawScore = k,
                    .ZScore = If(n > 0, (k - N * n / CDbl(M)) / System.Math.Sqrt(N * n / CDbl(M) * (1 - n / CDbl(M)) * (M - N) / CDbl(M - 1)), 0.0),
                    .PScore = pValue,
                    .AdjustedPValue = pValue
                }

                result.NodeResults.Add(nodeResult)
            Next

            ' Apply BH correction
            result.ApplyBHCorrection()

            Return result
        End Function

    End Class

End Namespace
