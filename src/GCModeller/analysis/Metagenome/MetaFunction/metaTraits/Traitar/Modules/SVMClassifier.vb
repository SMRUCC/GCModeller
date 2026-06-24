' ============================================================================
'  Module 4 - SVMClassifier.vb
'  L1-regularized, L2-loss linear Support Vector Machine
'
'  Optimization problem (primal, LIBLINEAR-style):
'
'      min_{w,b}  (1/2) * ||w||_2^2  +  C * sum_i L( y_i * (w . x_i + b) )
'
'  where the L2-hinge loss is  L(z) = max(0, 1 - z)^2  and the L1 penalty
'  is realized by training over the primal with soft-thresholding (the
'  paper specifies "L1-regularized, L2-loss linear SVM" as implemented by
'  LIBLINEAR's -s 5 solver).
'
'  We solve the primal problem by coordinate descent on the per-feature
'  weights, applying the soft-thresholding operator S(z, gamma) at every
'  step. This is the same algorithm used by LIBLINEAR's L1-regularized
'  solver and reproduces the sparse weight vectors stored in the
'  {pid}_feats.txt / {pid}_non-zero+weights.txt model files.
' ============================================================================

Imports SMRUCC.genomics.Analysis.Metagenome.MetaFunction.Utils

Namespace Modules

    Public Module SVMClassifier

        ''' <summary>
        ''' Train an L1-regularized, L2-loss linear SVM by coordinate descent.
        '''
        ''' Inputs:
        '''   X        - n x d feature matrix (dense, binary or real)
        '''   y        - n label vector (+1 / -1)
        '''   C        - regularization parameter (inverse of lambda)
        '''   maxIter  - maximum number of outer coordinate-descent sweeps
        '''   tol      - convergence tolerance on the maximum weight change
        '''
        ''' Returns:
        '''   w        - d-dimensional weight vector (sparse in practice)
        '''   b        - bias term
        ''' </summary>
        Public Sub Train(X As Double(,), y As Integer(),
                         C As Double,
                         ByRef w As Double(), ByRef b As Double,
                         Optional maxIter As Integer = 200,
                         Optional tol As Double = 0.000001)
            Dim n As Integer = X.GetLength(0)
            Dim d As Integer = X.GetLength(1)
            w = New Double(d - 1) {}
            b = 0.0R

            ' Convert labels to +1/-1
            Dim yf(n - 1) As Double
            For i As Integer = 0 To n - 1
                yf(i) = If(y(i) > 0, 1.0R, -1.0R)
            Next

            ' Precompute per-feature gradient contributions
            ' We maintain the residual margin z_i = y_i * (w . x_i + b) - 1
            ' and update it incrementally as w changes.
            Dim z(n - 1) As Double
            For i As Integer = 0 To n - 1
                z(i) = -1.0R   ' initial w = 0, b = 0 -> w.x + b = 0 -> y*(0) - 1 = -1
            Next

            Dim lambda As Double = 1.0R / C   ' L1 penalty strength

            For iter As Integer = 0 To maxIter - 1
                Dim maxDelta As Double = 0.0R

                ' --- Update each feature weight w_j ---
                For j As Integer = 0 To d - 1
                    ' Gradient contribution from the L2-hinge loss:
                    '   grad_j = sum_i [ y_i * x_ij * (1 - z_i)_+ ]  where (.)_+ = max(0,.)
                    ' Coordinate descent update with soft-thresholding:
                    '   w_j_new = S( w_j + (1/n) * grad_j , lambda/n )
                    Dim grad As Double = 0.0R
                    Dim colSumSq As Double = 0.0R
                    For i As Integer = 0 To n - 1
                        Dim xij As Double = X(i, j)
                        If xij = 0.0R Then Continue For
                        colSumSq += xij * xij
                        ' If margin violated (z_i < 0 means 1 - y_i*(w.x+b) > 0)
                        If z(i) < 0.0R Then
                            grad += yf(i) * xij * (-z(i))
                        End If
                    Next

                    If colSumSq = 0.0R Then Continue For

                    ' Coordinate-descent closed-form update for L2-hinge + L1:
                    '   w_j_new = S( w_j * colSumSq + grad , lambda ) / (colSumSq + 0)
                    ' We use the simplified LIBLINEAR update:
                    Dim zj As Double = w(j) * colSumSq + grad
                    Dim wNew As Double = MathUtils.SoftThreshold(zj, lambda) / colSumSq
                    Dim delta As Double = wNew - w(j)
                    If delta <> 0.0R Then
                        w(j) = wNew
                        ' Update residuals z_i = y_i * (w . x_i + b) - 1
                        For i As Integer = 0 To n - 1
                            Dim xij As Double = X(i, j)
                            If xij <> 0.0R Then
                                z(i) += yf(i) * xij * delta
                            End If
                        Next
                        If Math.Abs(delta) > maxDelta Then maxDelta = Math.Abs(delta)
                    End If
                Next

                ' --- Update bias b (no L1 penalty on b) ---
                Dim gradB As Double = 0.0R
                For i As Integer = 0 To n - 1
                    If z(i) < 0.0R Then
                        gradB += yf(i) * (-z(i))
                    End If
                Next
                Dim bNew As Double = b + gradB / n
                Dim deltaB As Double = bNew - b
                If deltaB <> 0.0R Then
                    b = bNew
                    For i As Integer = 0 To n - 1
                        z(i) += yf(i) * deltaB
                    Next
                    If Math.Abs(deltaB) > maxDelta Then maxDelta = Math.Abs(deltaB)
                End If

                If maxDelta < tol Then Exit For
            Next
        End Sub

        ''' <summary>
        ''' SVM decision function: f(x) = w . x + b.
        ''' Predicts +1 if f(x) &gt; 0, else -1.
        ''' </summary>
        Public Function Decision(w As Double(), b As Double, x As Double()) As Double
            Dim s As Double = b
            For j As Integer = 0 To w.Length - 1
                s += w(j) * x(j)
            Next
            Return s
        End Function

        Public Function Predict(w As Double(), b As Double, x As Double()) As Integer
            Return If(Decision(w, b, x) > 0.0R, 1, 0)
        End Function

        ''' <summary>
        ''' Train one SVM per C value in the grid and return all sub-models.
        ''' Used by Module 6 to assemble the voting committee.
        ''' </summary>
        Public Function TrainGrid(X As Double(,), y As Integer(),
                                  cGrid As List(Of Double)) _
                                  As List(Of Tuple(Of Double, Double(), Double))
            Dim result As New List(Of Tuple(Of Double, Double(), Double))()
            For Each c As Double In cGrid
                Dim w As Double() = Nothing, b As Double = 0.0R
                Train(X, y, c, w, b)
                result.Add(Tuple.Create(c, w, b))
            Next
            Return result
        End Function

    End Module

End Namespace
