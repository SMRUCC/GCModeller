Imports randf = Microsoft.VisualBasic.Math.RandomExtensions

Namespace Evaluation

    Public Module FakeAUCGenerator

        ''' <summary>
        ''' Create a fake output vector based on the target AUC
        ''' </summary>
        ''' <param name="labels"></param>
        ''' <param name="auc"></param>
        ''' <returns></returns>
        Public Function BuildOutput(labels As Double(), auc As Double, Optional cutoff As Double = 0.5) As Double()
            Dim pos = labels.Count(Function(i) i >= cutoff)
            Dim neg = labels.Count(Function(i) i < cutoff)
            Dim totalPairs = pos * neg
            Dim posRank = totalPairs * (1 - auc) / 2
            Dim indices = Enumerable.Range(0, labels.Length) _
                .OrderBy(Function(x) randf.NextDouble) _
                .ToArray
            Dim label_sorts = indices.Select(Function(i) labels(i)).ToArray
            Dim probs As Double() = New Double(labels.Length - 1) {}

            For i As Integer = 0 To labels.Length - 1
                If label_sorts(i) > cutoff Then
                    If i < posRank Then
                        probs(indices(i)) = randf.NextDouble * cutoff + cutoff
                    Else
                        probs(indices(i)) = randf.NextDouble * cutoff
                    End If
                Else
                    probs(indices(i)) = randf.NextDouble * cutoff
                End If
            Next

            Return probs
        End Function

    End Module
End Namespace