Imports std = System.Math

Namespace Core



    ''' <summary>BFGS（Armijo 回溯线搜索）[README §1.2 步骤2]</summary>
    Public Class BfgsMinimizer

        Public MaxIterations As Int32 = 300
        Public Gtol As Double = 0.0001

        Public Function Minimize(obj As IPoseObjective,
                                 ByRef trans() As Double, ByRef rotvec() As Double,
                                 ByRef torsions() As Double,
                                 ByRef evalCount As Int32) As Double
            Dim n = 6 + obj.NumTorsions
            Dim grads(n - 1) As Double
            Dim rigidCenter(2) As Double
            Dim f = obj.Evaluate(trans, rotvec, torsions, grads, rigidCenter)
            evalCount = 1

            Dim H(n - 1, n - 1) As Double
            For i = 0 To n - 1
                H(i, i) = 1.0
            Next

            For iter = 0 To MaxIterations - 1
                Dim gnorm = 0.0
                For i = 0 To n - 1
                    gnorm += grads(i) * grads(i)
                Next
                gnorm = std.Sqrt(gnorm)
                If gnorm < Gtol Then Exit For

                Dim p(n - 1) As Double
                For i = 0 To n - 1
                    Dim s As Double = 0
                    For k = 0 To n - 1
                        s += H(i, k) * grads(k)
                    Next
                    p(i) = -s
                Next
                Dim slope As Double = 0
                For i = 0 To n - 1
                    slope += p(i) * grads(i)
                Next
                If slope >= 0 Then
                    For i = 0 To n - 1
                        For j = 0 To n - 1
                            H(i, j) = If(i = j, 1.0, 0.0)
                        Next
                    Next
                    Continue For
                End If

                Dim alpha = 1.0
                Dim f0 = f
                Dim t2(trans.Length - 1) As Double
                Dim r2(rotvec.Length - 1) As Double
                Dim s2(torsions.Length - 1) As Double
                Dim gn(n - 1) As Double
                Dim fn As Double = 0
                While True
                    Dim tt = CType(trans.Clone(), Double())
                    Dim rr = CType(rotvec.Clone(), Double())
                    Dim ss = CType(torsions.Clone(), Double())
                    PoseOps.ApplyIncrement(tt, rr, ss, p, alpha)
                    fn = obj.Evaluate(tt, rr, ss, gn, rigidCenter)
                    evalCount += 1
                    If fn <= f0 + 0.0001 * alpha * slope OrElse alpha < 0.000000000001 Then
                        t2 = tt : r2 = rr : s2 = ss
                        Exit While
                    End If
                    alpha *= 0.5
                End While

                Dim sy As Double = 0
                Dim y(n - 1) As Double
                Dim sV(n - 1) As Double
                For i = 0 To n - 1
                    sV(i) = alpha * p(i)
                    y(i) = gn(i) - grads(i)
                    sy += sV(i) * y(i)
                Next
                If sy > 0.0000000001 Then
                    Dim rho = 1.0 / sy
                    Dim Hy(n - 1) As Double
                    Dim yH(n - 1) As Double
                    For i = 0 To n - 1
                        For k = 0 To n - 1
                            Hy(i) += H(i, k) * y(k)
                        Next
                    Next
                    For j = 0 To n - 1
                        For k = 0 To n - 1
                            yH(j) += y(k) * H(k, j)
                        Next
                    Next
                    Dim yHy As Double = 0
                    For k = 0 To n - 1
                        yHy += y(k) * Hy(k)
                    Next
                    Dim Hn(n - 1, n - 1) As Double
                    For i = 0 To n - 1
                        For j = 0 To n - 1
                            Hn(i, j) = H(i, j) - rho * sV(i) * yH(j) - rho * Hy(i) * sV(j) +
                                       rho * rho * yHy * sV(i) * sV(j) + rho * sV(i) * sV(j)
                        Next
                    Next
                    H = Hn
                End If

                trans = t2 : rotvec = r2 : torsions = s2
                f = fn
                For i = 0 To n - 1
                    grads(i) = gn(i)
                Next
            Next
            Return f
        End Function

    End Class

End Namespace