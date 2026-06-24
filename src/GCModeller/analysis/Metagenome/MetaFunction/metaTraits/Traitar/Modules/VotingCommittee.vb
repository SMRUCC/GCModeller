' ============================================================================
'  Module 6 - VotingCommittee.vb
'  Ensemble voting committee (top-5 SVM majority vote)
'
'  After training 13 SVMs (one per C value in the grid), Traitar selects the
'  5 sub-models with the highest cross-validation accuracy to form a "voting
'  committee". For a new sample, each committee member casts a vote (positive
'  if w.x + b > 0, negative otherwise); the final prediction is the majority
'  (>= 3 of 5 for positive).
'
'  When loading pre-trained models from disk (the demo case), the bias file
'  lists sub-models in accuracy order (best first), so the first 5 active
'  entries form the committee.
' ============================================================================

Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.Models

Namespace Modules

    Public Module VotingCommittee

        ''' <summary>
        ''' Select the top-K sub-models by cross-validation accuracy.
        ''' Used after training a grid of SVMs.
        ''' </summary>
        Public Function SelectTopK(trainedGrid As List(Of Tuple(Of Double, Double(), Double)),
                                    accuracies As List(Of Double),
                                    K As Integer) As List(Of Tuple(Of Double, Double(), Double))
            ' Pair each trained model with its accuracy, sort by accuracy desc
            Dim paired As New List(Of Tuple(Of Double, Double(), Double, Double))()
            For i As Integer = 0 To trainedGrid.Count - 1
                paired.Add(Tuple.Create(trainedGrid(i).Item1,
                                        trainedGrid(i).Item2,
                                        trainedGrid(i).Item3,
                                        accuracies(i)))
            Next
            paired.Sort(Function(a, b) b.Item4.CompareTo(a.Item4))
            Dim result As New List(Of Tuple(Of Double, Double(), Double))()
            For i As Integer = 0 To Math.Min(K, paired.Count) - 1
                result.Add(Tuple.Create(paired(i).Item1, paired(i).Item2, paired(i).Item3))
            Next
            Return result
        End Function

        ''' <summary>
        ''' Majority-vote prediction from a committee of (C, w, b) sub-models.
        ''' Returns 1 if at least ceil(N/2) members predict positive, else 0.
        ''' </summary>
        Public Function PredictCommittee(committee As List(Of Tuple(Of Double, Double(), Double)),
                                          x As Double()) As Integer
            If committee.Count = 0 Then Return 0
            Dim positiveVotes As Integer = 0
            For Each sm As Tuple(Of Double, Double(), Double) In committee
                If SVMClassifier.Predict(sm.Item2, sm.Item3, x) = 1 Then positiveVotes += 1
            Next
            Dim threshold As Integer = CInt(Math.Ceiling(committee.Count / 2.0R))
            If committee.Count Mod 2 = 0 Then threshold += 1
            Return If(positiveVotes >= threshold, 1, 0)
        End Function

        ''' <summary>
        ''' Average decision score across the committee (a confidence measure).
        ''' </summary>
        Public Function CommitteeScore(committee As List(Of Tuple(Of Double, Double(), Double)),
                                        x As Double()) As Double
            If committee.Count = 0 Then Return 0.0R
            Dim sum As Double = 0.0R
            For Each sm As Tuple(Of Double, Double(), Double) In committee
                sum += SVMClassifier.Decision(sm.Item2, sm.Item3, x)
            Next
            Return sum / committee.Count
        End Function

        ''' <summary>
        ''' Convenience: predict a single sample against a loaded PhenotypeModel.
        ''' Delegates to PhenotypeModel.Predict (which uses the in-file order
        ''' of the bias file as the accuracy ranking).
        ''' </summary>
        Public Function PredictWithModel(model As PhenotypeModel, sample As GenomeSample) As Integer
            Return model.Predict(sample)
        End Function

        ''' <summary>
        ''' Convenience: average decision score from a loaded PhenotypeModel.
        ''' </summary>
        Public Function ScoreWithModel(model As PhenotypeModel, sample As GenomeSample) As Double
            Return model.DecisionScore(sample)
        End Function

    End Module

End Namespace
