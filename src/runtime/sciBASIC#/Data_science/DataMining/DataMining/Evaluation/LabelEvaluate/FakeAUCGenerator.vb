Imports randf = Microsoft.VisualBasic.Math.RandomExtensions
Imports std = System.Math

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
                        probs(indices(i)) = randf.NextDouble(cutoff, 1)
                    Else
                        probs(indices(i)) = randf.NextDouble(0, cutoff)
                    End If
                Else
                    probs(indices(i)) = randf.NextDouble(0, cutoff)
                End If
            Next

            Return probs
        End Function

        Public Function BuildOutput2(labels As Double(), auc As Double, Optional cutoff As Double = 0.5) As Double()
            Dim out As Double() = Replicate(0.5, labels.Length).ToArray
            Dim d As Integer = labels.Length * 0.3
            Dim auc_delta As Double = Double.MaxValue
            Dim best As Double() = out

            For i As Integer = 0 To 1000
                Dim copy = out.ToArray

                For j As Integer = 0 To d
                    Dim offset = randf.NextInteger(out.Length)

                    If randf.NextBoolean Then
                        copy(offset) += randf.NextDouble
                    Else
                        copy(offset) -= randf.NextDouble
                    End If

                    If copy(offset) < 0 Then
                        copy(offset) = 0
                    ElseIf copy(offset) > 1 Then
                        copy(offset) = 1
                    End If
                Next

                Dim delta = std.abs(Evaluation.AUC(out, labels) - auc)

                If delta < auc_delta Then
                    auc_delta = delta
                    best = copy
                    out = copy
                End If
            Next

            Return best
        End Function

    End Module
End Namespace