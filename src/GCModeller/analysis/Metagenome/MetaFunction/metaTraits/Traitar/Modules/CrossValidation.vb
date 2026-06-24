' ============================================================================
'  Module 5 - CrossValidation.vb
'  Nested K-fold cross-validation & hyperparameter tuning
'
'  Outer loop: 10-fold CV to estimate generalization performance.
'  Inner loop: 10-fold CV (within each outer training fold) to select the
'  best regularization parameter C from the grid
'      C in { 1e-3, 1e-2, 5e-3, 7e-3, 2e-2, 5e-2, 7e-2, 1e-1, 2e-1, 5e-1, 7e-1, 1.0 }
'
'  This prevents data leakage: the test fold in the outer loop never
'  influences the choice of C.
' ============================================================================
Imports System
Imports System.Collections.Generic
Imports TraitarVBNet.Utils

Namespace Modules

    Public Module CrossValidation

        ' The 13 C values used by Traitar (matching the bias file columns)
        Public ReadOnly DefaultCGrid As New List(Of Double) From {
            0.001, 0.002, 0.005, 0.007, 0.01, 0.02, 0.05, 0.07, 0.1, 0.2, 0.5, 0.7, 1.0
        }

        ''' <summary>
        ''' Split the indices [0, n) into k folds. Returns a list of
        ''' (trainIndices, testIndices) pairs, one per fold.
        ''' </summary>
        Public Function MakeFolds(n As Integer, k As Integer, Optional seed As Integer = 42) _
            As List(Of Tuple(Of List(Of Integer), List(Of Integer)))
            Dim idx(n - 1) As Integer
            For i As Integer = 0 To n - 1
                idx(i) = i
            Next
            ' Deterministic shuffle (Fisher-Yates with a simple LCG)
            Dim rng As New Random(seed)
            For i As Integer = n - 1 To 1 Step -1
                Dim j As Integer = rng.Next(i + 1)
                Dim tmp As Integer = idx(i) : idx(i) = idx(j) : idx(j) = tmp
            Next

            Dim folds As New List(Of Tuple(Of List(Of Integer), List(Of Integer)))()
            Dim foldSize As Integer = n \ k
            Dim remainder As Integer = n Mod k
            Dim start As Integer = 0
            For f As Integer = 0 To k - 1
                Dim size As Integer = foldSize + If(f < remainder, 1, 0)
                Dim testIdx As New List(Of Integer)()
                For j As Integer = 0 To size - 1
                    testIdx.Add(idx(start + j))
                Next
                Dim trainIdx As New List(Of Integer)()
                For j As Integer = 0 To n - 1
                    If j < start OrElse j >= start + size Then trainIdx.Add(idx(j))
                Next
                folds.Add(Tuple.Create(trainIdx, testIdx))
                start += size
            Next
            Return folds
        End Function

        ''' <summary>
        ''' Slice rows of a dense matrix by index list.
        ''' </summary>
        Public Function SliceRows(X As Double(,), indices As List(Of Integer)) As Double(,)
            Dim n As Integer = indices.Count
            Dim d As Integer = X.GetLength(1)
            Dim result(n - 1, d - 1) As Double
            For i As Integer = 0 To n - 1
                For j As Integer = 0 To d - 1
                    result(i, j) = X(indices(i), j)
                Next
            Next
            Return result
        End Function

        ''' <summary>Slice an integer array by index list.</summary>
        Public Function SliceArray(y As Integer(), indices As List(Of Integer)) As Integer()
            Dim result(indices.Count - 1) As Integer
            For i As Integer = 0 To indices.Count - 1
                result(i) = y(indices(i))
            Next
            Return result
        End Function

        ''' <summary>
        ''' Inner-loop CV: for a given training set, find the C that maximizes
        ''' accuracy on the inner 10-fold CV. Returns the best C and its accuracy.
        ''' </summary>
        Public Function SelectBestC(Xtrain As Double(,), yTrain As Integer(),
                                     cGrid As List(Of Double),
                                     Optional innerFolds As Integer = 10) _
                                     As Tuple(Of Double, Double)
            Dim bestC As Double = cGrid(0)
            Dim bestAcc As Double = -1.0R

            For Each c As Double In cGrid
                Dim folds As List(Of Tuple(Of List(Of Integer), List(Of Integer))) =
                    MakeFolds(Xtrain.GetLength(0), innerFolds)
                Dim correct As Integer = 0, total As Integer = 0
                For Each fold As Tuple(Of List(Of Integer), List(Of Integer)) In folds
                    Dim Xtr As Double(,) = SliceRows(Xtrain, fold.Item1)
                    Dim ytr As Integer() = SliceArray(yTrain, fold.Item1)
                    Dim Xte As Double(,) = SliceRows(Xtrain, fold.Item2)
                    Dim yte As Integer() = SliceArray(yTrain, fold.Item2)

                    Dim w As Double() = Nothing, b As Double = 0.0R
                    SVMClassifier.Train(Xtr, ytr, c, w, b)
                    For i As Integer = 0 To Xte.GetLength(0) - 1
                        Dim xRow(Xte.GetLength(1) - 1) As Double
                        For j As Integer = 0 To Xte.GetLength(1) - 1
                            xRow(j) = Xte(i, j)
                        Next
                        Dim pred As Integer = SVMClassifier.Predict(w, b, xRow)
                        If pred = yte(i) Then correct += 1
                        total += 1
                    Next
                Next
                Dim acc As Double = If(total > 0, CDbl(correct) / total, 0.0R)
                If acc > bestAcc Then
                    bestAcc = acc
                    bestC = c
                End If
            Next
            Return Tuple.Create(bestC, bestAcc)
        End Function

        ''' <summary>
        ''' Outer-loop nested CV: estimate the generalization accuracy of the
        ''' full pipeline (inner CV for C selection + retraining on the outer
        ''' training fold). Returns the mean accuracy across outer folds.
        ''' </summary>
        Public Function NestedCrossValidate(X As Double(,), y As Integer(),
                                            Optional outerFolds As Integer = 10,
                                            Optional innerFolds As Integer = 10,
                                            Optional cGrid As List(Of Double) = Nothing) _
                                            As Double
            If cGrid Is Nothing Then cGrid = DefaultCGrid
            Dim folds As List(Of Tuple(Of List(Of Integer), List(Of Integer))) =
                MakeFolds(X.GetLength(0), outerFolds)
            Dim correct As Integer = 0, total As Integer = 0
            For Each fold As Tuple(Of List(Of Integer), List(Of Integer)) In folds
                Dim Xtr As Double(,) = SliceRows(X, fold.Item1)
                Dim ytr As Integer() = SliceArray(y, fold.Item1)
                Dim Xte As Double(,) = SliceRows(X, fold.Item2)
                Dim yte As Integer() = SliceArray(y, fold.Item2)

                Dim best As Tuple(Of Double, Double) = SelectBestC(Xtr, ytr, cGrid, innerFolds)
                Dim w As Double() = Nothing, b As Double = 0.0R
                SVMClassifier.Train(Xtr, ytr, best.Item1, w, b)

                For i As Integer = 0 To Xte.GetLength(0) - 1
                    Dim xRow(Xte.GetLength(1) - 1) As Double
                    For j As Integer = 0 To Xte.GetLength(1) - 1
                        xRow(j) = Xte(i, j)
                    Next
                    Dim pred As Integer = SVMClassifier.Predict(w, b, xRow)
                    If pred = yte(i) Then correct += 1
                    total += 1
                Next
            Next
            Return If(total > 0, CDbl(correct) / total, 0.0R)
        End Function

    End Module

End Namespace
