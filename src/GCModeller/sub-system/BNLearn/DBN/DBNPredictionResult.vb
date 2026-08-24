Namespace DBN


    ' ==================== Prediction Result ====================

    ''' <summary>
    ''' Result of a DBN prediction step.
    ''' Contains gene expression states, probability distributions, and RNA abundance
    ''' change rates for coupling with the metabolic network ODEs.
    ''' </summary>
    Public Class DBNPredictionResult

        ''' <summary>Gene/operon ID -> predicted state ("Low", "Medium", or "High")</summary>
        Public Property GeneStates As New Dictionary(Of String, String)

        ''' <summary>Gene/operon ID -> full probability distribution over states [P(Low), P(Med), P(High)]</summary>
        Public Property GeneProbabilities As New Dictionary(Of String, Double())

        ''' <summary>Gene/operon ID -> probability of the predicted (most likely) state</summary>
        Public Property GeneStateProbabilities As New Dictionary(Of String, Double)

        ''' <summary>
        ''' Gene/operon ID -> expected RNA transcript abundance change rate.
        ''' Range: [LowTranscriptionRate, HighTranscriptionRate] (default [0, 1]).
        ''' This is the expected transcription rate: E[rate] = sum(P(state) * rate(state)).
        ''' 
        ''' Usage in ODEs:
        '''   dR/dt = k_synthesis * RNAAbundanceChange - k_degradation * R
        ''' where R is the RNA transcript concentration.
        ''' </summary>
        Public Property RNAAbundanceChanges As New Dictionary(Of String, Double)

        ''' <summary>Operon ID -> list of gene IDs in that operon</summary>
        Public Property OperonGeneMapping As New Dictionary(Of String, List(Of String))


        ''' <summary>
        ''' Get RNA abundance change for a specific gene (based on its operon's prediction).
        ''' Returns 0.0 if the gene is not found in any operon.
        ''' </summary>
        Public Function GetGeneRNAAbundanceChange(geneId As String) As Double
            For Each kv In OperonGeneMapping
                If kv.Value.Contains(geneId) Then
                    If RNAAbundanceChanges.ContainsKey(kv.Key) Then
                        Return RNAAbundanceChanges(kv.Key)
                    End If
                End If
            Next
            ' Also check direct gene ID (not in an operon)
            If RNAAbundanceChanges.ContainsKey(geneId) Then
                Return RNAAbundanceChanges(geneId)
            End If
            Return 0.0
        End Function


        ''' <summary>
        ''' Get predicted state for a specific gene (based on its operon's prediction).
        ''' Returns "Medium" if the gene is not found.
        ''' </summary>
        Public Function GetGeneState(geneId As String) As String
            For Each kv In OperonGeneMapping
                If kv.Value.Contains(geneId) Then
                    If GeneStates.ContainsKey(kv.Key) Then
                        Return GeneStates(kv.Key)
                    End If
                End If
            Next
            If GeneStates.ContainsKey(geneId) Then
                Return GeneStates(geneId)
            End If
            Return "Medium"
        End Function

    End Class


End Namespace