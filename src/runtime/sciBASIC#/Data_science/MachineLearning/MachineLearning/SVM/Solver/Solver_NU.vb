
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

Namespace SVM
    '
    ' Solver for nu-svm classification and regression
    '
    ' additional constraint: e^T \alpha = constant
    '
    Friend NotInheritable Class Solver_NU
        Inherits Solver

        Private si As SolutionInfo

        Public Overrides Sub Solve(ByVal l As Integer, ByVal Q As IQMatrix, ByVal p As Double(), ByVal y As SByte(), ByVal alpha As Double(), ByVal Cp As Double, ByVal Cn As Double, ByVal eps As Double, ByVal si As SolutionInfo, ByVal shrinking As Boolean)
            Me.si = si
            MyBase.Solve(l, Q, p, y, alpha, Cp, Cn, eps, si, shrinking)
        End Sub

        ' return 1 if already optimal, return 0 otherwise
        Protected Overrides Function select_working_set(ByVal working_set As Integer()) As Integer
            ' return i,j such that y_i = y_j and
            ' i: maximizes -y_i * grad(f)_i, i in I_up(\alpha)
            ' j: minimizes the decrease of obj value
            '    (if quadratic coefficeint <= 0, replace it with tau)
            '    -y_j*grad(f)_j < -y_i*grad(f)_i, j in I_low(\alpha)

            Dim Gmaxp = -INF
            Dim Gmaxp2 = -INF
            Dim Gmaxp_idx = -1
            Dim Gmaxn = -INF
            Dim Gmaxn2 = -INF
            Dim Gmaxn_idx = -1
            Dim Gmin_idx = -1
            Dim obj_diff_min = INF

            For t = 0 To active_size - 1

                If y(t) = +1 Then
                    If Not is_upper_bound(t) Then
                        If -G(t) >= Gmaxp Then
                            Gmaxp = -G(t)
                            Gmaxp_idx = t
                        End If
                    End If
                Else

                    If Not is_lower_bound(t) Then
                        If G(t) >= Gmaxn Then
                            Gmaxn = G(t)
                            Gmaxn_idx = t
                        End If
                    End If
                End If
            Next

            Dim ip = Gmaxp_idx
            Dim [iN] = Gmaxn_idx
            Dim Q_ip As Single() = Nothing
            Dim Q_in As Single() = Nothing
            If ip <> -1 Then Q_ip = Q.GetQ(ip, active_size) ' null Q_ip not accessed: Gmaxp=-INF if ip=-1
            If [iN] <> -1 Then Q_in = Q.GetQ([iN], active_size)

            For j = 0 To active_size - 1

                If y(j) = +1 Then
                    If Not is_lower_bound(j) Then
                        Dim grad_diff = Gmaxp + G(j)
                        If G(j) >= Gmaxp2 Then Gmaxp2 = G(j)

                        If grad_diff > 0 Then
                            Dim obj_diff As Double
                            Dim quad_coef = QD(ip) + QD(j) - 2 * Q_ip(j)

                            If quad_coef > 0 Then
                                obj_diff = -(grad_diff * grad_diff) / quad_coef
                            Else
                                obj_diff = -(grad_diff * grad_diff) / 0.000000000001
                            End If

                            If obj_diff <= obj_diff_min Then
                                Gmin_idx = j
                                obj_diff_min = obj_diff
                            End If
                        End If
                    End If
                Else

                    If Not is_upper_bound(j) Then
                        Dim grad_diff = Gmaxn - G(j)
                        If -G(j) >= Gmaxn2 Then Gmaxn2 = -G(j)

                        If grad_diff > 0 Then
                            Dim obj_diff As Double
                            Dim quad_coef = QD([iN]) + QD(j) - 2 * Q_in(j)

                            If quad_coef > 0 Then
                                obj_diff = -(grad_diff * grad_diff) / quad_coef
                            Else
                                obj_diff = -(grad_diff * grad_diff) / 0.000000000001
                            End If

                            If obj_diff <= obj_diff_min Then
                                Gmin_idx = j
                                obj_diff_min = obj_diff
                            End If
                        End If
                    End If
                End If
            Next

            If stdNum.Max(Gmaxp + Gmaxp2, Gmaxn + Gmaxn2) < eps Then Return 1

            If y(Gmin_idx) = +1 Then
                working_set(0) = Gmaxp_idx
            Else
                working_set(0) = Gmaxn_idx
            End If

            working_set(1) = Gmin_idx
            Return 0
        End Function

        Private Function be_shrunk(ByVal i As Integer, ByVal Gmax1 As Double, ByVal Gmax2 As Double, ByVal Gmax3 As Double, ByVal Gmax4 As Double) As Boolean
            If is_upper_bound(i) Then
                If y(i) = +1 Then
                    Return -G(i) > Gmax1
                Else
                    Return -G(i) > Gmax4
                End If
            ElseIf is_lower_bound(i) Then

                If y(i) = +1 Then
                    Return G(i) > Gmax2
                Else
                    Return G(i) > Gmax3
                End If
            Else
                Return False
            End If
        End Function

        Protected Overrides Sub do_shrinking()
            Dim Gmax1 = -INF    ' max { -y_i * grad(f)_i | y_i = +1, i in I_up(\alpha) }
            Dim Gmax2 = -INF    ' max { y_i * grad(f)_i | y_i = +1, i in I_low(\alpha) }
            Dim Gmax3 = -INF    ' max { -y_i * grad(f)_i | y_i = -1, i in I_up(\alpha) }
            Dim Gmax4 = -INF    ' max { y_i * grad(f)_i | y_i = -1, i in I_low(\alpha) }

            ' find maximal violating pair first
            Dim i As Integer

            For i = 0 To active_size - 1

                If Not is_upper_bound(i) Then
                    If y(i) = +1 Then
                        If -G(i) > Gmax1 Then Gmax1 = -G(i)
                    ElseIf -G(i) > Gmax4 Then
                        Gmax4 = -G(i)
                    End If
                End If

                If Not is_lower_bound(i) Then
                    If y(i) = +1 Then
                        If G(i) > Gmax2 Then Gmax2 = G(i)
                    ElseIf G(i) > Gmax3 Then
                        Gmax3 = G(i)
                    End If
                End If
            Next

            If unshrink = False AndAlso stdNum.Max(Gmax1 + Gmax2, Gmax3 + Gmax4) <= eps * 10 Then
                unshrink = True
                reconstruct_gradient()
                active_size = l
            End If

            For i = 0 To active_size - 1

                If be_shrunk(i, Gmax1, Gmax2, Gmax3, Gmax4) Then
                    active_size -= 1

                    While active_size > i

                        If Not be_shrunk(active_size, Gmax1, Gmax2, Gmax3, Gmax4) Then
                            swap_index(i, active_size)
                            Exit While
                        End If

                        active_size -= 1
                    End While
                End If
            Next
        End Sub

        Protected Overrides Function calculate_rho() As Double
            Dim nr_free1 = 0, nr_free2 = 0
            Dim ub1 = INF, ub2 = INF
            Dim lb1 = -INF, lb2 = -INF
            Dim sum_free1 As Double = 0, sum_free2 As Double = 0

            For i = 0 To active_size - 1

                If y(i) = +1 Then
                    If is_lower_bound(i) Then
                        ub1 = stdNum.Min(ub1, G(i))
                    ElseIf is_upper_bound(i) Then
                        lb1 = stdNum.Max(lb1, G(i))
                    Else
                        Threading.Interlocked.Increment(nr_free1)
                        sum_free1 += G(i)
                    End If
                Else

                    If is_lower_bound(i) Then
                        ub2 = stdNum.Min(ub2, G(i))
                    ElseIf is_upper_bound(i) Then
                        lb2 = stdNum.Max(lb2, G(i))
                    Else
                        Threading.Interlocked.Increment(nr_free2)
                        sum_free2 += G(i)
                    End If
                End If
            Next

            Dim r1, r2 As Double

            If nr_free1 > 0 Then
                r1 = sum_free1 / nr_free1
            Else
                r1 = (ub1 + lb1) / 2
            End If

            If nr_free2 > 0 Then
                r2 = sum_free2 / nr_free2
            Else
                r2 = (ub2 + lb2) / 2
            End If

            si.r = (r1 + r2) / 2
            Return (r1 - r2) / 2
        End Function
    End Class

End Namespace