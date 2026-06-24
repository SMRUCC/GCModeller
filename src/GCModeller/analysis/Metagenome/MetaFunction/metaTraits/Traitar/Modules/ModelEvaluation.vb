' ============================================================================
'  Module 8 - ModelEvaluation.vb
'  Multi-label model evaluation (macro / micro accuracy)
'
'  Traitar predicts 67 phenotypes per genome. To summarize performance we
'  compute, for each phenotype, the confusion matrix (TP, TN, FP, FN) and
'  then aggregate:
'
'    Macro accuracy = mean over phenotypes of (TP + TN) / (TP + TN + FP + FN)
'    Micro accuracy = sum(TP) + sum(TN) over all phenotypes, divided by the
'                     total number of (sample, phenotype) predictions.
'
'  We also report per-phenotype recall (sensitivity), precision, and F1.
' ============================================================================

Namespace Modules

    Public Module ModelEvaluation

        ''' <summary>One phenotype's confusion-matrix counts.</summary>
        Public Class PhenotypeScore
            Public Property PhenotypeId As String = ""
            Public Property PhenotypeName As String = ""
            Public Property TP As Integer
            Public Property TN As Integer
            Public Property FP As Integer
            Public Property FN As Integer
            Public ReadOnly Property Accuracy As Double
                Get
                    Dim denom As Integer = TP + TN + FP + FN
                    If denom = 0 Then Return 0.0R
                    Return CDbl(TP + TN) / denom
                End Get
            End Property
            Public ReadOnly Property Recall As Double
                Get
                    Dim denom As Integer = TP + FN
                    If denom = 0 Then Return 0.0R
                    Return CDbl(TP) / denom
                End Get
            End Property
            Public ReadOnly Property Precision As Double
                Get
                    Dim denom As Integer = TP + FP
                    If denom = 0 Then Return 0.0R
                    Return CDbl(TP) / denom
                End Get
            End Property
            Public ReadOnly Property F1 As Double
                Get
                    Dim p As Double = Precision, r As Double = Recall
                    If p + r = 0 Then Return 0.0R
                    Return 2.0R * p * r / (p + r)
                End Get
            End Property
        End Class

        ''' <summary>
        ''' Compute per-phenotype confusion matrices from parallel arrays
        ''' of predicted and true labels.
        ''' </summary>
        Public Function ComputeScores(phenotypeIds As List(Of String),
                                       phenotypeNames As List(Of String),
                                       yTrue As List(Of List(Of Integer)),
                                       yPred As List(Of List(Of Integer))) _
                                       As List(Of PhenotypeScore)
            Dim scores As New List(Of PhenotypeScore)()
            For pIdx As Integer = 0 To phenotypeIds.Count - 1
                Dim s As New PhenotypeScore() With {
                    .PhenotypeId = phenotypeIds(pIdx),
                    .PhenotypeName = If(pIdx < phenotypeNames.Count, phenotypeNames(pIdx), "")
                }
                For sampleIdx As Integer = 0 To yTrue.Count - 1
                    Dim t As Integer = yTrue(sampleIdx)(pIdx)
                    Dim p As Integer = yPred(sampleIdx)(pIdx)
                    If t = 1 AndAlso p = 1 Then s.TP += 1
                    If t = 0 AndAlso p = 0 Then s.TN += 1
                    If t = 0 AndAlso p = 1 Then s.FP += 1
                    If t = 1 AndAlso p = 0 Then s.FN += 1
                Next
                scores.Add(s)
            Next
            Return scores
        End Function

        ''' <summary>
        ''' Macro accuracy = mean of per-phenotype accuracy.
        ''' </summary>
        Public Function MacroAccuracy(scores As List(Of PhenotypeScore)) As Double
            If scores.Count = 0 Then Return 0.0R
            Dim sum As Double = 0.0R
            For Each s As PhenotypeScore In scores
                sum += s.Accuracy
            Next
            Return sum / scores.Count
        End Function

        ''' <summary>
        ''' Micro accuracy = (sum TP + sum TN) / total predictions.
        ''' </summary>
        Public Function MicroAccuracy(scores As List(Of PhenotypeScore)) As Double
            Dim tpSum As Integer = 0, tnSum As Integer = 0, total As Integer = 0
            For Each s As PhenotypeScore In scores
                tpSum += s.TP
                tnSum += s.TN
                total += s.TP + s.TN + s.FP + s.FN
            Next
            If total = 0 Then Return 0.0R
            Return CDbl(tpSum + tnSum) / total
        End Function

        ''' <summary>
        ''' Macro recall (sensitivity) averaged over phenotypes.
        ''' </summary>
        Public Function MacroRecall(scores As List(Of PhenotypeScore)) As Double
            If scores.Count = 0 Then Return 0.0R
            Dim sum As Double = 0.0R
            For Each s As PhenotypeScore In scores
                sum += s.Recall
            Next
            Return sum / scores.Count
        End Function

        ''' <summary>
        ''' Pretty-print a summary table of all phenotype scores.
        ''' </summary>
        Public Function FormatScoreTable(scores As List(Of PhenotypeScore)) As String
            Dim sb As New System.Text.StringBuilder()
            sb.AppendLine("PhenotypeID" & vbTab & "Name" & vbTab & "TP" & vbTab & "TN" & vbTab &
                          "FP" & vbTab & "FN" & vbTab & "Acc" & vbTab & "Recall" & vbTab &
                          "Prec" & vbTab & "F1")
            For Each s As PhenotypeScore In scores
                sb.AppendLine(s.PhenotypeId & vbTab & s.PhenotypeName & vbTab &
                              s.TP & vbTab & s.TN & vbTab & s.FP & vbTab & s.FN & vbTab &
                              s.Accuracy.ToString("0.####") & vbTab &
                              s.Recall.ToString("0.####") & vbTab &
                              s.Precision.ToString("0.####") & vbTab &
                              s.F1.ToString("0.####"))
            Next
            sb.AppendLine()
            sb.AppendLine("Macro accuracy: " & MacroAccuracy(scores).ToString("0.####"))
            sb.AppendLine("Micro accuracy: " & MicroAccuracy(scores).ToString("0.####"))
            sb.AppendLine("Macro recall:    " & MacroRecall(scores).ToString("0.####"))
            Return sb.ToString()
        End Function

    End Module

End Namespace
