' 
' * SVM.NET Library
' * Copyright (C) 2008 Matthew Johnson
' * 
' * This program is free software: you can redistribute it and/or modify
' * it under the terms of the GNU General Public License as published by
' * the Free Software Foundation, either version 3 of the License, or
' * (at your option) any later version.
' * 
' * This program is distributed in the hope that it will be useful,
' * but WITHOUT ANY WARRANTY; without even the implied warranty of
' * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' * GNU General Public License for more details.
' * 
' * You should have received a copy of the GNU General Public License
' * along with this program.  If not, see <http://www.gnu.org/licenses/>.



Imports System.IO
Imports System.Runtime.InteropServices
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Text
Imports stdNum = System.Math

Namespace SVM


    ' An SMO algorithm in Fan et al., JMLR 6(2005), p. 1889--1918
    ' Solves:
    '
    '	min 0.5(\alpha^T Q \alpha) + p^T \alpha
    '
    '		y^T \alpha = \delta
    '		y_i = +1 or -1
    '		0 <= alpha_i <= Cp for y_i = 1
    '		0 <= alpha_i <= Cn for y_i = -1
    '
    ' Given:
    '
    '	Q, p, y, Cp, Cn, and an initial feasible point \alpha
    '	l is the size of vectors and matrices
    '	eps is the stopping tolerance
    '
    ' solution will be put in \alpha, objective value will be put in obj
    '
    Friend Class Solver
        Protected active_size As Integer
        Protected y As SByte()
        Protected G As Double()     ' gradient of objective function
        Protected Const LOWER_BOUND As Byte = 0
        Protected Const UPPER_BOUND As Byte = 1
        Protected Const FREE As Byte = 2
        Protected alpha_status As Byte()    ' LOWER_BOUND, UPPER_BOUND, FREE
        Protected alpha As Double()
        Protected Q As IQMatrix
        Protected QD As Double()
        Protected eps As Double
        Protected Cp, Cn As Double
        Protected p As Double()
        Protected active_set As Integer()
        Protected G_bar As Double()     ' gradient, if we treat free variables as 0
        Protected l As Integer
        Protected unshrink As Boolean   ' XXX
        Protected Const INF As Double = Double.PositiveInfinity

        Private Function get_C(ByVal i As Integer) As Double
            Return If(y(i) > 0, Cp, Cn)
        End Function

        Private Sub update_alpha_status(ByVal i As Integer)
            If alpha(i) >= get_C(i) Then
                alpha_status(i) = UPPER_BOUND
            ElseIf alpha(i) <= 0 Then
                alpha_status(i) = LOWER_BOUND
            Else
                alpha_status(i) = FREE
            End If
        End Sub

        Protected Function is_upper_bound(ByVal i As Integer) As Boolean
            Return alpha_status(i) = UPPER_BOUND
        End Function

        Protected Function is_lower_bound(ByVal i As Integer) As Boolean
            Return alpha_status(i) = LOWER_BOUND
        End Function

        Protected Function is_free(ByVal i As Integer) As Boolean
            Return alpha_status(i) = FREE
        End Function

        ' java: information about solution except alpha,
        ' because we cannot return multiple values otherwise...
        Public Class SolutionInfo
            Public Property obj As Double
            Public Property rho As Double
            Public Property upper_bound_p As Double
            Public Property upper_bound_n As Double
            Public Property r As Double ' for Solver_NU
        End Class

        Protected Sub swap_index(ByVal i As Integer, ByVal j As Integer)
            Q.SwapIndex(i, j)

            Do
                Dim __ = y(i)
                y(i) = y(j)
                y(j) = __
            Loop While False

            Do
                Dim __ = G(i)
                G(i) = G(j)
                G(j) = __
            Loop While False

            Do
                Dim __ = alpha_status(i)
                alpha_status(i) = alpha_status(j)
                alpha_status(j) = __
            Loop While False

            Do
                Dim __ = alpha(i)
                alpha(i) = alpha(j)
                alpha(j) = __
            Loop While False

            Do
                Dim __ = p(i)
                p(i) = p(j)
                p(j) = __
            Loop While False

            Do
                Dim __ = active_set(i)
                active_set(i) = active_set(j)
                active_set(j) = __
            Loop While False

            Do
                Dim __ = G_bar(i)
                G_bar(i) = G_bar(j)
                G_bar(j) = __
            Loop While False
        End Sub

        Protected Sub reconstruct_gradient()
            ' reconstruct inactive elements of G from G_bar and free variables

            If active_size = l Then Return
            Dim i, j As Integer
            Dim nr_free = 0

            For j = active_size To l - 1
                G(j) = G_bar(j) + p(j)
            Next

            For j = 0 To active_size - 1
                If is_free(j) Then nr_free += 1
            Next

            If 2 * nr_free < active_size Then Procedures.info(ASCII.LF & "WARNING: using -h 0 may be faster" & ASCII.LF)

            If nr_free * l > 2 * active_size * (l - active_size) Then
                For i = active_size To l - 1
                    Dim Q_i = Q.GetQ(i, active_size)

                    For j = 0 To active_size - 1
                        If is_free(j) Then G(i) += alpha(j) * Q_i(j)
                    Next
                Next
            Else

                For i = 0 To active_size - 1

                    If is_free(i) Then
                        Dim Q_i = Q.GetQ(i, l)
                        Dim alpha_i = alpha(i)

                        For j = active_size To l - 1
                            G(j) += alpha_i * Q_i(j)
                        Next
                    End If
                Next
            End If
        End Sub

        Public Overridable Sub Solve(ByVal l As Integer, ByVal Q As IQMatrix, ByVal p_ As Double(), ByVal y_ As SByte(), ByVal alpha_ As Double(), ByVal Cp As Double, ByVal Cn As Double, ByVal eps As Double, ByVal si As SolutionInfo, ByVal shrinking As Boolean)
            Me.l = l
            Me.Q = Q
            QD = Q.GetQD()
            p = CType(p_.Clone(), Double())
            y = CType(y_.Clone(), SByte())
            alpha = CType(alpha_.Clone(), Double())
            Me.Cp = Cp
            Me.Cn = Cn
            Me.eps = eps
            unshrink = False

            ' initialize alpha_status
            If True Then
                alpha_status = New Byte(l - 1) {}

                For i = 0 To l - 1
                    update_alpha_status(i)
                Next
            End If

            ' initialize active set (for shrinking)
            If True Then
                active_set = New Integer(l - 1) {}

                For i = 0 To l - 1
                    active_set(i) = i
                Next

                active_size = l
            End If

            ' initialize gradient
            If True Then
                G = New Double(l - 1) {}
                G_bar = New Double(l - 1) {}
                Dim i As Integer

                For i = 0 To l - 1
                    G(i) = p(i)
                    G_bar(i) = 0
                Next

                For i = 0 To l - 1

                    If Not is_lower_bound(i) Then
                        Dim Q_i = Q.GetQ(i, l)
                        Dim alpha_i = alpha(i)
                        Dim j As Integer

                        For j = 0 To l - 1
                            G(j) += alpha_i * Q_i(j)
                        Next

                        If is_upper_bound(i) Then
                            For j = 0 To l - 1
                                G_bar(j) += get_C(i) * Q_i(j)
                            Next
                        End If
                    End If
                Next
            End If

            ' optimization step

            Dim iter = 0
            Dim max_iter = stdNum.Max(10000000, If(l > Integer.MaxValue / 100, Integer.MaxValue, 100 * l))
            Dim counter = stdNum.Min(l, 1000) + 1
            Dim working_set = New Integer(1) {}

            While iter < max_iter
                ' show progress and do shrinking

                If Threading.Interlocked.Decrement(counter) = 0 Then
                    counter = stdNum.Min(l, 1000)
                    If shrinking Then do_shrinking()
                    info(".")
                End If

                If select_working_set(working_set) <> 0 Then
                    ' reconstruct the whole gradient
                    reconstruct_gradient()
                    ' reset active set size and check
                    active_size = l
                    info("*")

                    If select_working_set(working_set) <> 0 Then
                        Exit While
                    Else
                        counter = 1
                    End If  ' do shrinking next iteration
                End If

                Dim i = working_set(0)
                Dim j = working_set(1)
                Threading.Interlocked.Increment(iter)

                ' update alpha[i] and alpha[j], handle bounds carefully

                Dim Q_i = Q.GetQ(i, active_size)
                Dim Q_j = Q.GetQ(j, active_size)
                Dim C_i = get_C(i)
                Dim C_j = get_C(j)
                Dim old_alpha_i = alpha(i)
                Dim old_alpha_j = alpha(j)

                If y(i) <> y(j) Then
                    Dim quad_coef = QD(i) + QD(j) + 2 * Q_i(j)
                    If quad_coef <= 0 Then quad_coef = 0.000000000001
                    Dim delta = (-G(i) - G(j)) / quad_coef
                    Dim diff = alpha(i) - alpha(j)
                    alpha(i) += delta
                    alpha(j) += delta

                    If diff > 0 Then
                        If alpha(j) < 0 Then
                            alpha(j) = 0
                            alpha(i) = diff
                        End If
                    Else

                        If alpha(i) < 0 Then
                            alpha(i) = 0
                            alpha(j) = -diff
                        End If
                    End If

                    If diff > C_i - C_j Then
                        If alpha(i) > C_i Then
                            alpha(i) = C_i
                            alpha(j) = C_i - diff
                        End If
                    Else

                        If alpha(j) > C_j Then
                            alpha(j) = C_j
                            alpha(i) = C_j + diff
                        End If
                    End If
                Else
                    Dim quad_coef = QD(i) + QD(j) - 2 * Q_i(j)
                    If quad_coef <= 0 Then quad_coef = 0.000000000001
                    Dim delta = (G(i) - G(j)) / quad_coef
                    Dim sum = alpha(i) + alpha(j)
                    alpha(i) -= delta
                    alpha(j) += delta

                    If sum > C_i Then
                        If alpha(i) > C_i Then
                            alpha(i) = C_i
                            alpha(j) = sum - C_i
                        End If
                    Else

                        If alpha(j) < 0 Then
                            alpha(j) = 0
                            alpha(i) = sum
                        End If
                    End If

                    If sum > C_j Then
                        If alpha(j) > C_j Then
                            alpha(j) = C_j
                            alpha(i) = sum - C_j
                        End If
                    Else

                        If alpha(i) < 0 Then
                            alpha(i) = 0
                            alpha(j) = sum
                        End If
                    End If
                End If

                ' update G

                Dim delta_alpha_i = alpha(i) - old_alpha_i
                Dim delta_alpha_j = alpha(j) - old_alpha_j

                For k = 0 To active_size - 1
                    G(k) += Q_i(k) * delta_alpha_i + Q_j(k) * delta_alpha_j
                Next

                ' update alpha_status and G_bar

                If True Then
                    Dim ui = is_upper_bound(i)
                    Dim uj = is_upper_bound(j)
                    update_alpha_status(i)
                    update_alpha_status(j)
                    Dim k As Integer

                    If ui <> is_upper_bound(i) Then
                        Q_i = Q.GetQ(i, l)

                        If ui Then
                            For k = 0 To l - 1
                                G_bar(k) -= C_i * Q_i(k)
                            Next
                        Else

                            For k = 0 To l - 1
                                G_bar(k) += C_i * Q_i(k)
                            Next
                        End If
                    End If

                    If uj <> is_upper_bound(j) Then
                        Q_j = Q.GetQ(j, l)

                        If uj Then
                            For k = 0 To l - 1
                                G_bar(k) -= C_j * Q_j(k)
                            Next
                        Else

                            For k = 0 To l - 1
                                G_bar(k) += C_j * Q_j(k)
                            Next
                        End If
                    End If
                End If
            End While

            If iter >= max_iter Then
                If active_size < l Then
                    ' reconstruct the whole gradient to calculate objective value
                    reconstruct_gradient()
                    active_size = l
                    info("*")
                End If

                Console.Error.Write(ASCII.LF & "WARNING: reaching max number of iterations" & ASCII.LF)
            End If

            ' calculate rho

            si.rho = calculate_rho()

            ' calculate objective value
            If True Then
                Dim v As Double = 0
                Dim i As Integer

                For i = 0 To l - 1
                    v += alpha(i) * (G(i) + p(i))
                Next

                si.obj = v / 2
            End If

            ' put back the solution
            If True Then
                For i = 0 To l - 1
                    alpha_(active_set(i)) = alpha(i)
                Next
            End If

            si.upper_bound_p = Cp
            si.upper_bound_n = Cn
            Procedures.info(ASCII.LF & "optimization finished, #iter = " & iter & ASCII.LF)
        End Sub

        ' return 1 if already optimal, return 0 otherwise
        Protected Overridable Function select_working_set(ByVal working_set As Integer()) As Integer
            ' return i,j such that
            ' i: maximizes -y_i * grad(f)_i, i in I_up(\alpha)
            ' j: mimimizes the decrease of obj value
            '    (if quadratic coefficeint <= 0, replace it with tau)
            '    -y_j*grad(f)_j < -y_i*grad(f)_i, j in I_low(\alpha)

            Dim Gmax = -INF
            Dim Gmax2 = -INF
            Dim Gmax_idx = -1
            Dim Gmin_idx = -1
            Dim obj_diff_min = INF

            For t = 0 To active_size - 1

                If y(t) = +1 Then
                    If Not is_upper_bound(t) Then
                        If -G(t) >= Gmax Then
                            Gmax = -G(t)
                            Gmax_idx = t
                        End If
                    End If
                Else

                    If Not is_lower_bound(t) Then
                        If G(t) >= Gmax Then
                            Gmax = G(t)
                            Gmax_idx = t
                        End If
                    End If
                End If
            Next

            Dim i = Gmax_idx
            Dim Q_i As Single() = Nothing
            If i <> -1 Then Q_i = Q.GetQ(i, active_size) ' null Q_i not accessed: Gmax=-INF if i=-1

            For j = 0 To active_size - 1

                If y(j) = +1 Then
                    If Not is_lower_bound(j) Then
                        Dim grad_diff = Gmax + G(j)
                        If G(j) >= Gmax2 Then Gmax2 = G(j)

                        If grad_diff > 0 Then
                            Dim obj_diff As Double
                            Dim quad_coef = QD(i) + QD(j) - 2.0 * y(i) * Q_i(j)

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
                        Dim grad_diff = Gmax - G(j)
                        If -G(j) >= Gmax2 Then Gmax2 = -G(j)

                        If grad_diff > 0 Then
                            Dim obj_diff As Double
                            Dim quad_coef = QD(i) + QD(j) + 2.0 * y(i) * Q_i(j)

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

            If Gmax + Gmax2 < eps Then Return 1
            working_set(0) = Gmax_idx
            working_set(1) = Gmin_idx
            Return 0
        End Function

        Private Function be_shrunk(ByVal i As Integer, ByVal Gmax1 As Double, ByVal Gmax2 As Double) As Boolean
            If is_upper_bound(i) Then
                If y(i) = +1 Then
                    Return -G(i) > Gmax1
                Else
                    Return -G(i) > Gmax2
                End If
            ElseIf is_lower_bound(i) Then

                If y(i) = +1 Then
                    Return G(i) > Gmax2
                Else
                    Return G(i) > Gmax1
                End If
            Else
                Return False
            End If
        End Function

        Protected Overridable Sub do_shrinking()
            Dim i As Integer
            Dim Gmax1 = -INF        ' max { -y_i * grad(f)_i | i in I_up(\alpha) }
            Dim Gmax2 = -INF        ' max { y_i * grad(f)_i | i in I_low(\alpha) }

            ' find maximal violating pair first
            For i = 0 To active_size - 1

                If y(i) = +1 Then
                    If Not is_upper_bound(i) Then
                        If -G(i) >= Gmax1 Then Gmax1 = -G(i)
                    End If

                    If Not is_lower_bound(i) Then
                        If G(i) >= Gmax2 Then Gmax2 = G(i)
                    End If
                Else

                    If Not is_upper_bound(i) Then
                        If -G(i) >= Gmax2 Then Gmax2 = -G(i)
                    End If

                    If Not is_lower_bound(i) Then
                        If G(i) >= Gmax1 Then Gmax1 = G(i)
                    End If
                End If
            Next

            If unshrink = False AndAlso Gmax1 + Gmax2 <= eps * 10 Then
                unshrink = True
                reconstruct_gradient()
                active_size = l
            End If

            For i = 0 To active_size - 1

                If be_shrunk(i, Gmax1, Gmax2) Then
                    active_size -= 1

                    While active_size > i

                        If Not be_shrunk(active_size, Gmax1, Gmax2) Then
                            swap_index(i, active_size)
                            Exit While
                        End If

                        active_size -= 1
                    End While
                End If
            Next
        End Sub

        Protected Overridable Function calculate_rho() As Double
            Dim r As Double
            Dim nr_free = 0
            Dim ub = INF, lb = -INF, sum_free As Double = 0

            For i = 0 To active_size - 1
                Dim yG = y(i) * G(i)

                If is_lower_bound(i) Then
                    If y(i) > 0 Then
                        ub = stdNum.Min(ub, yG)
                    Else
                        lb = stdNum.Max(lb, yG)
                    End If
                ElseIf is_upper_bound(i) Then

                    If y(i) < 0 Then
                        ub = stdNum.Min(ub, yG)
                    Else
                        lb = stdNum.Max(lb, yG)
                    End If
                Else
                    Threading.Interlocked.Increment(nr_free)
                    sum_free += yG
                End If
            Next

            If nr_free > 0 Then
                r = sum_free / nr_free
            Else
                r = (ub + lb) / 2
            End If

            Return r
        End Function
    End Class

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

    '
    ' Q matrices for various formulations
    '
    Friend Class SVC_Q
        Inherits Kernel

        Private ReadOnly y As SByte()
        Private ReadOnly cache As Cache
        Private ReadOnly QD As Double()

        Public Sub New(ByVal prob As Problem, ByVal param As Parameter, ByVal y_ As SByte())
            MyBase.New(prob.Count, prob.X, param)
            y = CType(y_.Clone(), SByte())
            cache = New Cache(prob.Count, CLng(param.CacheSize) * (1 << 20))
            QD = New Double(prob.Count - 1) {}

            For i = 0 To prob.Count - 1
                QD(i) = KernelFunction(i, i)
            Next
        End Sub

        Public Overrides Function GetQ(ByVal i As Integer, ByVal len As Integer) As Single()
            Dim data As Single() = Nothing
            Dim start As i32 = 0, j As Integer

            If (start = cache.GetData(i, data, len)) < len Then
                For j = start To len - 1
                    data(j) = CSng(y(i) * y(j) * KernelFunction(i, j))
                Next
            End If

            Return data
        End Function

        Public Overrides Function GetQD() As Double()
            Return QD
        End Function

        Public Overrides Sub SwapIndex(ByVal i As Integer, ByVal j As Integer)
            cache.SwapIndex(i, j)
            MyBase.SwapIndex(i, j)

            Do
                Dim __ = y(i)
                y(i) = y(j)
                y(j) = __
            Loop While False

            Do
                Dim __ = QD(i)
                QD(i) = QD(j)
                QD(j) = __
            Loop While False
        End Sub
    End Class

    Friend Class ONE_CLASS_Q
        Inherits Kernel

        Private ReadOnly cache As Cache
        Private ReadOnly QD As Double()

        Public Sub New(ByVal prob As Problem, ByVal param As Parameter)
            MyBase.New(prob.Count, prob.X, param)
            cache = New Cache(prob.Count, CLng(param.CacheSize) * (1 << 20))
            QD = New Double(prob.Count - 1) {}

            For i = 0 To prob.Count - 1
                QD(i) = KernelFunction(i, i)
            Next
        End Sub

        Public Overrides Function GetQ(ByVal i As Integer, ByVal len As Integer) As Single()
            Dim data As Single() = Nothing
            Dim start As i32 = 0
            Dim j As Integer

            If (start = cache.GetData(i, data, len)) < len Then
                For j = start To len - 1
                    data(j) = CSng(KernelFunction(i, j))
                Next
            End If

            Return data
        End Function

        Public Overrides Function GetQD() As Double()
            Return QD
        End Function

        Public Overrides Sub SwapIndex(ByVal i As Integer, ByVal j As Integer)
            cache.SwapIndex(i, j)
            MyBase.SwapIndex(i, j)

            Do
                Dim __ = QD(i)
                QD(i) = QD(j)
                QD(j) = __
            Loop While False
        End Sub
    End Class

    Friend Class SVR_Q
        Inherits Kernel

        Private ReadOnly l As Integer
        Private ReadOnly cache As Cache
        Private ReadOnly sign As SByte()
        Private ReadOnly index As Integer()
        Private next_buffer As Integer
        Private buffer As Single()()
        Private ReadOnly QD As Double()

        Public Sub New(ByVal prob As Problem, ByVal param As Parameter)
            MyBase.New(prob.Count, prob.X, param)
            l = prob.Count
            cache = New Cache(l, CLng(param.CacheSize) * (1 << 20))
            QD = New Double(2 * l - 1) {}
            sign = New SByte(2 * l - 1) {}
            index = New Integer(2 * l - 1) {}

            For k = 0 To l - 1
                sign(k) = 1
                sign(k + l) = -1
                index(k) = k
                index(k + l) = k
                QD(k) = KernelFunction(k, k)
                QD(k + l) = QD(k)
            Next

            buffer = New Single()() {New Single(2 * l - 1) {}, New Single(2 * l - 1) {}}
            next_buffer = 0
        End Sub

        Public Overrides Sub SwapIndex(ByVal i As Integer, ByVal j As Integer)
            Do
                Dim __ = sign(i)
                sign(i) = sign(j)
                sign(j) = __
            Loop While False

            Do
                Dim __ = index(i)
                index(i) = index(j)
                index(j) = __
            Loop While False

            Do
                Dim __ = QD(i)
                QD(i) = QD(j)
                QD(j) = __
            Loop While False
        End Sub

        Public Overrides Function GetQ(ByVal i As Integer, ByVal len As Integer) As Single()
            Dim data As Single() = Nothing
            Dim j As Integer, real_i = index(i)

            If cache.GetData(real_i, data, l) < l Then
                For j = 0 To l - 1
                    data(j) = CSng(KernelFunction(real_i, j))
                Next
            End If

            ' reorder and copy
            Dim buf = buffer(next_buffer)
            next_buffer = 1 - next_buffer
            Dim si = sign(i)

            For j = 0 To len - 1
                buf(j) = CSng(si) * sign(j) * data(index(j))
            Next

            Return buf
        End Function

        Public Overrides Function GetQD() As Double()
            Return QD
        End Function
    End Class

    Friend Module Procedures
        Private _verbose As Boolean

        Public Property IsVerbose As Boolean
            Get
                Return _verbose
            End Get
            Set(ByVal value As Boolean)
                _verbose = value
            End Set
        End Property

        '
        ' construct and solve various formulations
        '
        Public Const LIBSVM_VERSION As Integer = 318
        Private rand As Random = New Random()
        Private svm_print_stdout As TextWriter = Console.Out

        Public Sub setRandomSeed(ByVal seed As Integer)
            rand = New Random(seed)
        End Sub

        Public Sub info(ByVal s As String)
            If IsVerbose Then svm_print_stdout.Write(s)
        End Sub

        Private Sub solve_c_svc(ByVal prob As Problem, ByVal param As Parameter, ByVal alpha As Double(), ByVal si As Solver.SolutionInfo, ByVal Cp As Double, ByVal Cn As Double)
            Dim l = prob.Count
            Dim minus_ones = New Double(l - 1) {}
            Dim y = New SByte(l - 1) {}
            Dim i As Integer

            For i = 0 To l - 1
                alpha(i) = 0
                minus_ones(i) = -1

                If prob.Y(i) > 0 Then
                    y(i) = +1
                Else
                    y(i) = -1
                End If
            Next

            Dim s As Solver = New Solver()
            s.Solve(l, New SVC_Q(prob, param, y), minus_ones, y, alpha, Cp, Cn, param.EPS, si, param.Shrinking)
            Dim sum_alpha As Double = 0

            For i = 0 To l - 1
                sum_alpha += alpha(i)
            Next

            If Cp = Cn Then Procedures.info("nu = " & sum_alpha / (Cp * prob.Count) & ASCII.LF)

            For i = 0 To l - 1
                alpha(i) *= y(i)
            Next
        End Sub

        Private Sub solve_nu_svc(ByVal prob As Problem, ByVal param As Parameter, ByVal alpha As Double(), ByVal si As Solver.SolutionInfo)
            Dim i As Integer
            Dim l = prob.Count
            Dim nu = param.Nu
            Dim y = New SByte(l - 1) {}

            For i = 0 To l - 1

                If prob.Y(i) > 0 Then
                    y(i) = +1
                Else
                    y(i) = -1
                End If
            Next

            Dim sum_pos = nu * l / 2
            Dim sum_neg = nu * l / 2

            For i = 0 To l - 1

                If y(i) = +1 Then
                    alpha(i) = stdNum.Min(1.0, sum_pos)
                    sum_pos -= alpha(i)
                Else
                    alpha(i) = stdNum.Min(1.0, sum_neg)
                    sum_neg -= alpha(i)
                End If
            Next

            Dim zeros = New Double(l - 1) {}

            For i = 0 To l - 1
                zeros(i) = 0
            Next

            Dim s As Solver_NU = New Solver_NU()
            s.Solve(l, New SVC_Q(prob, param, y), zeros, y, alpha, 1.0, 1.0, param.EPS, si, param.Shrinking)
            Dim r = si.r
            Procedures.info("C = " & 1 / r & ASCII.LF)

            For i = 0 To l - 1
                alpha(i) *= y(i) / r
            Next

            si.rho /= r
            si.obj /= r * r
            si.upper_bound_p = 1 / r
            si.upper_bound_n = 1 / r
        End Sub

        Private Sub solve_one_class(ByVal prob As Problem, ByVal param As Parameter, ByVal alpha As Double(), ByVal si As Solver.SolutionInfo)
            Dim l = prob.Count
            Dim zeros = New Double(l - 1) {}
            Dim ones = New SByte(l - 1) {}
            Dim i As Integer
            Dim n As Integer = CInt(param.Nu * prob.Count)   ' # of alpha's at upper bound

            For i = 0 To n - 1
                alpha(i) = 1
            Next

            If n < prob.Count Then alpha(n) = param.Nu * prob.Count - n

            For i = n + 1 To l - 1
                alpha(i) = 0
            Next

            For i = 0 To l - 1
                zeros(i) = 0
                ones(i) = 1
            Next

            Dim s As Solver = New Solver()
            s.Solve(l, New ONE_CLASS_Q(prob, param), zeros, ones, alpha, 1.0, 1.0, param.EPS, si, param.Shrinking)
        End Sub

        Private Sub solve_epsilon_svr(ByVal prob As Problem, ByVal param As Parameter, ByVal alpha As Double(), ByVal si As Solver.SolutionInfo)
            Dim l = prob.Count
            Dim alpha2 = New Double(2 * l - 1) {}
            Dim linear_term = New Double(2 * l - 1) {}
            Dim y = New SByte(2 * l - 1) {}
            Dim i As Integer

            For i = 0 To l - 1
                alpha2(i) = 0
                linear_term(i) = param.P - prob.Y(i)
                y(i) = 1
                alpha2(i + l) = 0
                linear_term(i + l) = param.P + prob.Y(i)
                y(i + l) = -1
            Next

            Dim s As Solver = New Solver()
            s.Solve(2 * l, New SVR_Q(prob, param), linear_term, y, alpha2, param.C, param.C, param.EPS, si, param.Shrinking)
            Dim sum_alpha As Double = 0

            For i = 0 To l - 1
                alpha(i) = alpha2(i) - alpha2(i + l)
                sum_alpha += stdNum.Abs(alpha(i))
            Next

            Procedures.info("nu = " & sum_alpha / (param.C * l) & ASCII.LF)
        End Sub

        Private Sub solve_nu_svr(ByVal prob As Problem, ByVal param As Parameter, ByVal alpha As Double(), ByVal si As Solver.SolutionInfo)
            Dim l = prob.Count
            Dim C = param.C
            Dim alpha2 = New Double(2 * l - 1) {}
            Dim linear_term = New Double(2 * l - 1) {}
            Dim y = New SByte(2 * l - 1) {}
            Dim i As Integer
            Dim sum = C * param.Nu * l / 2

            For i = 0 To l - 1
                alpha2(i + l) = stdNum.Min(sum, C)
                alpha2(i) = alpha2(i + l)
                sum -= alpha2(i)
                linear_term(i) = -prob.Y(i)
                y(i) = 1
                linear_term(i + l) = prob.Y(i)
                y(i + l) = -1
            Next

            Dim s As Solver_NU = New Solver_NU()
            s.Solve(2 * l, New SVR_Q(prob, param), linear_term, y, alpha2, C, C, param.EPS, si, param.Shrinking)
            Procedures.info("epsilon = " & -si.r & ASCII.LF)

            For i = 0 To l - 1
                alpha(i) = alpha2(i) - alpha2(i + l)
            Next
        End Sub

        '
        ' decision_function
        '
        Private Class decision_function
            Public Property alpha As Double()
            Public Property rho As Double
        End Class

        Private Function svm_train_one(ByVal prob As Problem, ByVal param As Parameter, ByVal Cp As Double, ByVal Cn As Double) As decision_function
            Dim alpha = New Double(prob.Count - 1) {}
            Dim si As Solver.SolutionInfo = New Solver.SolutionInfo()

            Select Case param.SvmType
                Case SvmType.C_SVC
                    solve_c_svc(prob, param, alpha, si, Cp, Cn)
                Case SvmType.NU_SVC
                    solve_nu_svc(prob, param, alpha, si)
                Case SvmType.ONE_CLASS
                    solve_one_class(prob, param, alpha, si)
                Case SvmType.EPSILON_SVR
                    solve_epsilon_svr(prob, param, alpha, si)
                Case SvmType.NU_SVR
                    solve_nu_svr(prob, param, alpha, si)
            End Select

            Procedures.info("obj = " & si.obj & ", rho = " & si.rho & ASCII.LF)

            ' output SVs

            Dim nSV = 0
            Dim nBSV = 0

            For i = 0 To prob.Count - 1

                If stdNum.Abs(alpha(i)) > 0 Then
                    Threading.Interlocked.Increment(nSV)

                    If prob.Y(i) > 0 Then
                        If stdNum.Abs(alpha(i)) >= si.upper_bound_p Then Threading.Interlocked.Increment(nBSV)
                    Else
                        If stdNum.Abs(alpha(i)) >= si.upper_bound_n Then Threading.Interlocked.Increment(nBSV)
                    End If
                End If
            Next

            Procedures.info("nSV = " & nSV & ", nBSV = " & nBSV & ASCII.LF)
            Dim f As decision_function = New decision_function()
            f.alpha = alpha
            f.rho = si.rho
            Return f
        End Function

        ' Platt's binary SVM Probablistic Output: an improvement from Lin et al.
        Private Sub sigmoid_train(ByVal l As Integer, ByVal dec_values As Double(), ByVal labels As Double(), ByVal probAB As Double())
            Dim A, B As Double
            Dim prior1 As Double = 0, prior0 As Double = 0
            Dim i As Integer

            For i = 0 To l - 1

                If labels(i) > 0 Then
                    prior1 += 1
                Else
                    prior0 += 1
                End If
            Next

            Dim max_iter = 100  ' Maximal number of iterations
            Dim min_step = 0.0000000001    ' Minimal step taken in line search
            Dim sigma = 0.000000000001   ' For numerically strict PD of Hessian
            Dim eps = 0.00001
            Dim hiTarget = (prior1 + 1.0) / (prior1 + 2.0)
            Dim loTarget = 1 / (prior0 + 2.0)
            Dim t = New Double(l - 1) {}
            Dim fApB, p, q, h11, h22, h21, g1, g2, det, dA, dB, gd, stepsize As Double
            Dim newA, newB, newf, d1, d2 As Double
            Dim iter As Integer

            ' Initial Point and Initial Fun Value
            A = 0.0
            B = stdNum.Log((prior0 + 1.0) / (prior1 + 1.0))
            Dim fval = 0.0

            For i = 0 To l - 1

                If labels(i) > 0 Then
                    t(i) = hiTarget
                Else
                    t(i) = loTarget
                End If

                fApB = dec_values(i) * A + B

                If fApB >= 0 Then
                    fval += t(i) * fApB + stdNum.Log(1 + stdNum.Exp(-fApB))
                Else
                    fval += (t(i) - 1) * fApB + stdNum.Log(1 + stdNum.Exp(fApB))
                End If
            Next

            For iter = 0 To max_iter - 1
                ' Update Gradient and Hessian (use H' = H + sigma I)
                h11 = sigma ' numerically ensures strict PD
                h22 = sigma
                h21 = 0.0
                g1 = 0.0
                g2 = 0.0

                For i = 0 To l - 1
                    fApB = dec_values(i) * A + B

                    If fApB >= 0 Then
                        p = stdNum.Exp(-fApB) / (1.0 + stdNum.Exp(-fApB))
                        q = 1.0 / (1.0 + stdNum.Exp(-fApB))
                    Else
                        p = 1.0 / (1.0 + stdNum.Exp(fApB))
                        q = stdNum.Exp(fApB) / (1.0 + stdNum.Exp(fApB))
                    End If

                    d2 = p * q
                    h11 += dec_values(i) * dec_values(i) * d2
                    h22 += d2
                    h21 += dec_values(i) * d2
                    d1 = t(i) - p
                    g1 += dec_values(i) * d1
                    g2 += d1
                Next

                ' Stopping Criteria
                If stdNum.Abs(g1) < eps AndAlso stdNum.Abs(g2) < eps Then Exit For

                ' Finding Newton direction: -inv(H') * g
                det = h11 * h22 - h21 * h21
                dA = -(h22 * g1 - h21 * g2) / det
                dB = -(-h21 * g1 + h11 * g2) / det
                gd = g1 * dA + g2 * dB
                stepsize = 1        ' Line Search

                While stepsize >= min_step
                    newA = A + stepsize * dA
                    newB = B + stepsize * dB

                    ' New function value
                    newf = 0.0

                    For i = 0 To l - 1
                        fApB = dec_values(i) * newA + newB

                        If fApB >= 0 Then
                            newf += t(i) * fApB + stdNum.Log(1 + stdNum.Exp(-fApB))
                        Else
                            newf += (t(i) - 1) * fApB + stdNum.Log(1 + stdNum.Exp(fApB))
                        End If
                    Next
                    ' Check sufficient decrease
                    If newf < fval + 0.0001 * stepsize * gd Then
                        A = newA
                        B = newB
                        fval = newf
                        Exit While
                    Else
                        stepsize = stepsize / 2.0
                    End If
                End While

                If stepsize < min_step Then
                    Procedures.info("Line search fails in two-class probability estimates" & ASCII.LF)
                    Exit For
                End If
            Next

            If iter >= max_iter Then Procedures.info("Reaching maximal iterations in two-class probability estimates" & ASCII.LF)
            probAB(0) = A
            probAB(1) = B
        End Sub

        Private Function sigmoid_predict(ByVal decision_value As Double, ByVal A As Double, ByVal B As Double) As Double
            Dim fApB = decision_value * A + B

            If fApB >= 0 Then
                Return stdNum.Exp(-fApB) / (1.0 + stdNum.Exp(-fApB))
            Else
                Return 1.0 / (1 + stdNum.Exp(fApB))
            End If
        End Function

        ' Method 2 from the multiclass_prob paper by Wu, Lin, and Weng
        Private Sub multiclass_probability(ByVal k As Integer, ByVal r As Double(,), ByVal p As Double())
            Dim t, j As Integer
            Dim iter = 0, max_iter = stdNum.Max(100, k)
            Dim Q = New Double(k - 1, k - 1) {}
            Dim Qp = New Double(k - 1) {}
            Dim pQp As Double, eps = 0.005 / k

            For t = 0 To k - 1
                p(t) = 1.0 / k  ' Valid if k = 1
                Q(t, t) = 0

                For j = 0 To t - 1
                    Q(t, t) += r(j, t) * r(j, t)
                    Q(t, j) = Q(j, t)
                Next

                For j = t + 1 To k - 1
                    Q(t, t) += r(j, t) * r(j, t)
                    Q(t, j) = -r(j, t) * r(t, j)
                Next
            Next

            For iter = 0 To max_iter - 1
                ' stopping condition, recalculate QP,pQP for numerical accuracy
                pQp = 0

                For t = 0 To k - 1
                    Qp(t) = 0

                    For j = 0 To k - 1
                        Qp(t) += Q(t, j) * p(j)
                    Next

                    pQp += p(t) * Qp(t)
                Next

                Dim max_error As Double = 0

                For t = 0 To k - 1
                    Dim [error] = stdNum.Abs(Qp(t) - pQp)
                    If [error] > max_error Then max_error = [error]
                Next

                If max_error < eps Then Exit For

                For t = 0 To k - 1
                    Dim diff = (-Qp(t) + pQp) / Q(t, t)
                    p(t) += diff
                    pQp = (pQp + diff * (diff * Q(t, t) + 2 * Qp(t))) / (1 + diff) / (1 + diff)

                    For j = 0 To k - 1
                        Qp(j) = (Qp(j) + diff * Q(t, j)) / (1 + diff)
                        p(j) /= 1 + diff
                    Next
                Next
            Next

            If iter >= max_iter Then Procedures.info("Exceeds max_iter in multiclass_prob" & ASCII.LF)
        End Sub

        ' Cross-validation decision values for probability estimates
        Private Sub svm_binary_svc_probability(ByVal prob As Problem, ByVal param As Parameter, ByVal Cp As Double, ByVal Cn As Double, ByVal probAB As Double())
            Dim i As Integer
            Dim nr_fold = 5
            Dim perm = New Integer(prob.Count - 1) {}
            Dim dec_values = New Double(prob.Count - 1) {}

            ' random shuffle
            For i = 0 To prob.Count - 1
                perm(i) = i
            Next

            For i = 0 To prob.Count - 1
                Dim j = i + rand.Next(prob.Count - i)

                Do
                    Dim __ = perm(i)
                    perm(i) = perm(j)
                    perm(j) = __
                Loop While False
            Next

            For i = 0 To nr_fold - 1
                Dim begin As Integer = CInt(i * prob.Count / nr_fold)
                Dim [end] As Integer = CInt((i + 1) * prob.Count / nr_fold)
                Dim j, k As Integer
                Dim subprob As Problem = New Problem()
                subprob.Count = prob.Count - ([end] - begin)
                subprob.X = New Node(subprob.Count - 1)() {}
                subprob.Y = New Double(subprob.Count - 1) {}
                k = 0

                For j = 0 To begin - 1
                    subprob.X(k) = prob.X(perm(j))
                    subprob.Y(k) = prob.Y(perm(j))
                    Threading.Interlocked.Increment(k)
                Next

                For j = [end] To prob.Count - 1
                    subprob.X(k) = prob.X(perm(j))
                    subprob.Y(k) = prob.Y(perm(j))
                    Threading.Interlocked.Increment(k)
                Next

                Dim p_count = 0, n_count = 0

                For j = 0 To k - 1

                    If subprob.Y(j) > 0 Then
                        p_count += 1
                    Else
                        n_count += 1
                    End If
                Next

                If p_count = 0 AndAlso n_count = 0 Then
                    For j = begin To [end] - 1
                        dec_values(perm(j)) = 0
                    Next
                ElseIf p_count > 0 AndAlso n_count = 0 Then

                    For j = begin To [end] - 1
                        dec_values(perm(j)) = 1
                    Next
                ElseIf p_count = 0 AndAlso n_count > 0 Then

                    For j = begin To [end] - 1
                        dec_values(perm(j)) = -1
                    Next
                Else
                    Dim subparam As Parameter = CType(param.Clone(), Parameter)
                    subparam.Probability = False
                    subparam.C = 1.0
                    subparam.Weights(1) = Cp
                    subparam.Weights(-1) = Cn
                    Dim submodel = svm_train(subprob, subparam)

                    For j = begin To [end] - 1
                        Dim dec_value = New Double(0) {}
                        svm_predict_values(submodel, prob.X(perm(j)), dec_value)
                        dec_values(perm(j)) = dec_value(0)
                        ' ensure +1 -1 order; reason not using CV subroutine
                        dec_values(perm(j)) *= submodel.ClassLabels(0)
                    Next
                End If
            Next

            sigmoid_train(prob.Count, dec_values, prob.Y, probAB)
        End Sub

        ' Return parameter of a Laplace distribution 
        Private Function svm_svr_probability(ByVal prob As Problem, ByVal param As Parameter) As Double
            Dim i As Integer
            Dim nr_fold = 5
            Dim ymv = New Double(prob.Count - 1) {}
            Dim mae As Double = 0
            Dim newparam As Parameter = CType(param.Clone(), Parameter)
            newparam.Probability = False
            svm_cross_validation(prob, newparam, nr_fold, ymv)

            For i = 0 To prob.Count - 1
                ymv(i) = prob.Y(i) - ymv(i)
                mae += stdNum.Abs(ymv(i))
            Next

            mae /= prob.Count
            Dim std = stdNum.Sqrt(2 * mae * mae)
            Dim count = 0
            mae = 0

            For i = 0 To prob.Count - 1

                If stdNum.Abs(ymv(i)) > 5 * std Then
                    count = count + 1
                Else
                    mae += stdNum.Abs(ymv(i))
                End If
            Next

            mae /= prob.Count - count
            Procedures.info("Prob. model for test data: target value = predicted value + z," & ASCII.LF & "z: Laplace distribution e^(-|z|/sigma)/(2sigma),sigma=" & mae & ASCII.LF)
            Return mae
        End Function

        ' label: label name, start: begin of each class, count: #data of classes, perm: indices to the original data
        ' perm, length l, must be allocated before calling this subroutine
        Private Sub svm_group_classes(ByVal prob As Problem, <Out> ByRef nr_class_ret As Integer, <Out> ByRef label_ret As Integer(), <Out> ByRef start_ret As Integer(), <Out> ByRef count_ret As Integer(), ByVal perm As Integer())
            Dim l = prob.Count
            Dim max_nr_class = 16
            Dim nr_class = 0
            Dim label = New Integer(max_nr_class - 1) {}
            Dim count = New Integer(max_nr_class - 1) {}
            Dim data_label = New Integer(l - 1) {}
            Dim i As Integer

            For i = 0 To l - 1
                Dim this_label As Integer = CInt(prob.Y(i))
                Dim j As Integer

                For j = 0 To nr_class - 1

                    If this_label = label(j) Then
                        Threading.Interlocked.Increment(count(j))
                        Exit For
                    End If
                Next

                data_label(i) = j

                If j = nr_class Then
                    If nr_class = max_nr_class Then
                        max_nr_class *= 2
                        Dim new_data = New Integer(max_nr_class - 1) {}
                        Array.Copy(label, 0, new_data, 0, label.Length)
                        label = new_data
                        new_data = New Integer(max_nr_class - 1) {}
                        Array.Copy(count, 0, new_data, 0, count.Length)
                        count = new_data
                    End If

                    label(nr_class) = this_label
                    count(nr_class) = 1
                    Threading.Interlocked.Increment(nr_class)
                End If
            Next

            '
            ' Labels are ordered by their first occurrence in the training set. 
            ' However, for two-class sets with -1/+1 labels and -1 appears first, 
            ' we swap labels to ensure that internally the binary SVM has positive data corresponding to the +1 instances.
            '
            If nr_class = 2 AndAlso label(0) = -1 AndAlso label(1) = +1 Then
                Do
                    Dim __ = label(0)
                    label(0) = label(1)
                    label(1) = __
                Loop While False

                Do
                    Dim __ = count(0)
                    count(0) = count(1)
                    count(1) = __
                Loop While False

                For i = 0 To l - 1

                    If data_label(i) = 0 Then
                        data_label(i) = 1
                    Else
                        data_label(i) = 0
                    End If
                Next
            End If

            Dim start = New Integer(nr_class - 1) {}
            start(0) = 0

            For i = 1 To nr_class - 1
                start(i) = start(i - 1) + count(i - 1)
            Next

            For i = 0 To l - 1
                perm(start(data_label(i))) = i
                Threading.Interlocked.Increment(start(data_label(i)))
            Next

            start(0) = 0

            For i = 1 To nr_class - 1
                start(i) = start(i - 1) + count(i - 1)
            Next

            nr_class_ret = nr_class
            label_ret = label
            start_ret = start
            count_ret = count
        End Sub

        '
        ' Interface functions
        '
        Public Function svm_train(ByVal prob As Problem, ByVal param As Parameter) As Model
            Dim model As Model = New Model()
            model.Parameter = param

            If param.SvmType = SvmType.ONE_CLASS OrElse param.SvmType = SvmType.EPSILON_SVR OrElse param.SvmType = SvmType.NU_SVR Then
                ' regression or one-class-svm
                model.NumberOfClasses = 2
                model.ClassLabels = Nothing
                model.NumberOfSVPerClass = Nothing
                model.PairwiseProbabilityA = Nothing
                model.PairwiseProbabilityB = Nothing
                model.SupportVectorCoefficients = New Double(0)() {}

                If param.Probability AndAlso (param.SvmType = SvmType.EPSILON_SVR OrElse param.SvmType = SvmType.NU_SVR) Then
                    model.PairwiseProbabilityA = New Double(0) {}
                    model.PairwiseProbabilityA(0) = svm_svr_probability(prob, param)
                End If

                Dim f = svm_train_one(prob, param, 0, 0)
                model.Rho = New Double(0) {}
                model.Rho(0) = f.rho
                Dim nSV = 0
                Dim i As Integer

                For i = 0 To prob.Count - 1
                    If stdNum.Abs(f.alpha(i)) > 0 Then Threading.Interlocked.Increment(nSV)
                Next

                model.SupportVectorCount = nSV
                model.SupportVectors = New Node(nSV - 1)() {}
                model.SupportVectorCoefficients(0) = New Double(nSV - 1) {}
                model.SupportVectorIndices = New Integer(nSV - 1) {}
                Dim j = 0

                For i = 0 To prob.Count - 1

                    If stdNum.Abs(f.alpha(i)) > 0 Then
                        model.SupportVectors(j) = prob.X(i)
                        model.SupportVectorCoefficients(0)(j) = f.alpha(i)
                        model.SupportVectorIndices(j) = i + 1
                        Threading.Interlocked.Increment(j)
                    End If
                Next
            Else
                ' classification
                Dim l = prob.Count
                Dim nr_class As Integer
                Dim label As Integer() = Nothing
                Dim start As Integer() = Nothing
                Dim count As Integer() = Nothing
                Dim perm = New Integer(l - 1) {}

                ' group training data of the same class
                svm_group_classes(prob, nr_class, label, start, count, perm)
                If nr_class = 1 Then Procedures.info("WARNING: training data in only one class. See README for details." & ASCII.LF)
                Dim x = New Node(l - 1)() {}
                Dim i As Integer

                For i = 0 To l - 1
                    x(i) = prob.X(perm(i))
                Next

                ' calculate weighted C

                Dim weighted_C = New Double(nr_class - 1) {}

                For i = 0 To nr_class - 1
                    weighted_C(i) = param.C
                Next

                For i = 0 To nr_class - 1

                    If Not param.Weights.ContainsKey(label(i)) Then
                        Console.Error.Write("WARNING: class label " & label(i) & " specified in weight is not found" & ASCII.LF)
                    Else
                        weighted_C(i) *= param.Weights(label(i))
                    End If
                Next

                ' train k*(k-1)/2 models

                Dim nonzero = New Boolean(l - 1) {}

                For i = 0 To l - 1
                    nonzero(i) = False
                Next

                Dim f = New decision_function(CInt(nr_class * (nr_class - 1) / 2) - 1) {}
                Dim probA As Double() = Nothing, probB As Double() = Nothing

                If param.Probability Then
                    probA = New Double(CInt(nr_class * (nr_class - 1) / 2) - 1) {}
                    probB = New Double(CInt(nr_class * (nr_class - 1) / 2) - 1) {}
                End If

                Dim p = 0

                For i = 0 To nr_class - 1

                    For j = i + 1 To nr_class - 1
                        Dim sub_prob As Problem = New Problem()
                        Dim si = start(i), sj = start(j)
                        Dim ci = count(i), cj = count(j)
                        sub_prob.Count = ci + cj
                        sub_prob.X = New Node(sub_prob.Count - 1)() {}
                        sub_prob.Y = New Double(sub_prob.Count - 1) {}
                        Dim k As Integer

                        For k = 0 To ci - 1
                            sub_prob.X(k) = x(si + k)
                            sub_prob.Y(k) = +1
                        Next

                        For k = 0 To cj - 1
                            sub_prob.X(ci + k) = x(sj + k)
                            sub_prob.Y(ci + k) = -1
                        Next

                        If param.Probability Then
                            Dim probAB = New Double(1) {}
                            svm_binary_svc_probability(sub_prob, param, weighted_C(i), weighted_C(j), probAB)
                            probA(p) = probAB(0)
                            probB(p) = probAB(1)
                        End If

                        f(p) = svm_train_one(sub_prob, param, weighted_C(i), weighted_C(j))

                        For k = 0 To ci - 1
                            If Not nonzero(si + k) AndAlso stdNum.Abs(f(p).alpha(k)) > 0 Then nonzero(si + k) = True
                        Next

                        For k = 0 To cj - 1
                            If Not nonzero(sj + k) AndAlso stdNum.Abs(f(p).alpha(ci + k)) > 0 Then nonzero(sj + k) = True
                        Next

                        Threading.Interlocked.Increment(p)
                    Next
                Next

                ' build output

                model.NumberOfClasses = nr_class
                model.ClassLabels = New Integer(nr_class - 1) {}

                For i = 0 To nr_class - 1
                    model.ClassLabels(i) = label(i)
                Next

                model.Rho = New Double(CInt(nr_class * (nr_class - 1) / 2) - 1) {}

                For i = 0 To CInt(nr_class * (nr_class - 1) / 2) - 1
                    model.Rho(i) = f(i).rho
                Next

                If param.Probability Then
                    model.PairwiseProbabilityA = New Double(CInt(nr_class * (nr_class - 1) / 2) - 1) {}
                    model.PairwiseProbabilityB = New Double(CInt(nr_class * (nr_class - 1) / 2) - 1) {}

                    For i = 0 To CInt(nr_class * (nr_class - 1) / 2) - 1
                        model.PairwiseProbabilityA(i) = probA(i)
                        model.PairwiseProbabilityB(i) = probB(i)
                    Next
                Else
                    model.PairwiseProbabilityA = Nothing
                    model.PairwiseProbabilityA = Nothing
                End If

                Dim nnz = 0
                Dim nz_count = New Integer(nr_class - 1) {}
                model.NumberOfSVPerClass = New Integer(nr_class - 1) {}

                For i = 0 To nr_class - 1
                    Dim nSV = 0

                    For j = 0 To count(i) - 1

                        If nonzero(start(i) + j) Then
                            Threading.Interlocked.Increment(nSV)
                            Threading.Interlocked.Increment(nnz)
                        End If
                    Next

                    model.NumberOfSVPerClass(i) = nSV
                    nz_count(i) = nSV
                Next

                Procedures.info("Total nSV = " & nnz & ASCII.LF)
                model.SupportVectorCount = nnz
                model.SupportVectors = New Node(nnz - 1)() {}
                model.SupportVectorIndices = New Integer(nnz - 1) {}
                p = 0

                For i = 0 To l - 1

                    If nonzero(i) Then
                        model.SupportVectors(p) = x(i)
                        model.SupportVectorIndices(stdNum.Min(Threading.Interlocked.Increment(p), p - 1)) = perm(i) + 1
                    End If
                Next

                Dim nz_start = New Integer(nr_class - 1) {}
                nz_start(0) = 0

                For i = 1 To nr_class - 1
                    nz_start(i) = nz_start(i - 1) + nz_count(i - 1)
                Next

                model.SupportVectorCoefficients = New Double(nr_class - 1 - 1)() {}

                For i = 0 To nr_class - 1 - 1
                    model.SupportVectorCoefficients(i) = New Double(nnz - 1) {}
                Next

                p = 0

                For i = 0 To nr_class - 1

                    For j = i + 1 To nr_class - 1
                        ' classifier (i,j): coefficients with
                        ' i are in sv_coef[j-1][nz_start[i]...],
                        ' j are in sv_coef[i][nz_start[j]...]

                        Dim si = start(i)
                        Dim sj = start(j)
                        Dim ci = count(i)
                        Dim cj = count(j)
                        Dim q = nz_start(i)
                        Dim k As Integer

                        For k = 0 To ci - 1
                            If nonzero(si + k) Then model.SupportVectorCoefficients(j - 1)(stdNum.Min(Threading.Interlocked.Increment(q), q - 1)) = f(p).alpha(k)
                        Next

                        q = nz_start(j)

                        For k = 0 To cj - 1
                            If nonzero(sj + k) Then model.SupportVectorCoefficients(i)(stdNum.Min(Threading.Interlocked.Increment(q), q - 1)) = f(p).alpha(ci + k)
                        Next

                        Threading.Interlocked.Increment(p)
                    Next
                Next
            End If

            Return model
        End Function

        ' Stratified cross validation
        Public Sub svm_cross_validation(ByVal prob As Problem, ByVal param As Parameter, ByVal nr_fold As Integer, ByVal target As Double())
            Dim i As Integer
            Dim fold_start = New Integer(nr_fold + 1 - 1) {}
            Dim l = prob.Count
            Dim perm = New Integer(l - 1) {}

            ' stratified cv may not give leave-one-out rate
            ' Each class to l folds -> some folds may have zero elements
            If (param.SvmType = SvmType.C_SVC OrElse param.SvmType = SvmType.NU_SVC) AndAlso nr_fold < l Then
                Dim nr_class As Integer
                Dim label As Integer() = Nothing
                Dim start As Integer() = Nothing
                Dim count As Integer() = Nothing
                svm_group_classes(prob, nr_class, label, start, count, perm)


                ' random shuffle and then data grouped by fold using the array perm
                Dim fold_count = New Integer(nr_fold - 1) {}
                Dim c As Integer
                Dim index = New Integer(l - 1) {}

                For i = 0 To l - 1
                    index(i) = perm(i)
                Next

                For c = 0 To nr_class - 1

                    For i = 0 To count(c) - 1
                        Dim j = i + rand.Next(count(c) - i)

                        Do
                            Dim __ = index(start(c) + j)
                            index(start(c) + j) = index(start(c) + i)
                            index(start(c) + i) = __
                        Loop While False
                    Next
                Next

                For i = 0 To nr_fold - 1
                    fold_count(i) = 0

                    For c = 0 To nr_class - 1
                        fold_count(i) += CInt((i + 1) * count(c) / nr_fold - i * count(c) / nr_fold)
                    Next
                Next

                fold_start(0) = 0

                For i = 1 To nr_fold
                    fold_start(i) = fold_start(i - 1) + fold_count(i - 1)
                Next

                For c = 0 To nr_class - 1

                    For i = 0 To nr_fold - 1
                        Dim begin As Integer = start(c) + CInt(i * count(c) / nr_fold)
                        Dim [end] As Integer = start(c) + CInt((i + 1) * count(c) / nr_fold)

                        For j = begin To [end] - 1
                            perm(fold_start(i)) = index(j)
                            fold_start(i) += 1
                        Next
                    Next
                Next

                fold_start(0) = 0

                For i = 1 To nr_fold
                    fold_start(i) = fold_start(i - 1) + fold_count(i - 1)
                Next
            Else

                For i = 0 To l - 1
                    perm(i) = i
                Next

                For i = 0 To l - 1
                    Dim j = i + rand.Next(l - i)

                    Do
                        Dim __ = perm(i)
                        perm(i) = perm(j)
                        perm(j) = __
                    Loop While False
                Next

                For i = 0 To nr_fold
                    fold_start(i) = CInt(i * l / nr_fold)
                Next
            End If

            For i = 0 To nr_fold - 1
                Dim begin = fold_start(i)
                Dim [end] = fold_start(i + 1)
                Dim j, k As Integer
                Dim subprob As Problem = New Problem()
                subprob.Count = l - ([end] - begin)
                subprob.X = New Node(subprob.Count - 1)() {}
                subprob.Y = New Double(subprob.Count - 1) {}
                k = 0

                For j = 0 To begin - 1
                    subprob.X(k) = prob.X(perm(j))
                    subprob.Y(k) = prob.Y(perm(j))
                    Threading.Interlocked.Increment(k)
                Next

                For j = [end] To l - 1
                    subprob.X(k) = prob.X(perm(j))
                    subprob.Y(k) = prob.Y(perm(j))
                    Threading.Interlocked.Increment(k)
                Next

                Dim submodel = svm_train(subprob, param)

                If param.Probability AndAlso (param.SvmType = SvmType.C_SVC OrElse param.SvmType = SvmType.NU_SVC) Then
                    Dim prob_estimates = New Double(svm_get_nr_class(submodel) - 1) {}

                    For j = begin To [end] - 1
                        target(perm(j)) = svm_predict_probability(submodel, prob.X(perm(j)), prob_estimates)
                    Next
                Else

                    For j = begin To [end] - 1
                        target(perm(j)) = svm_predict(submodel, prob.X(perm(j)))
                    Next
                End If
            Next
        End Sub

        Public Function svm_get_svm_type(ByVal model As Model) As SvmType
            Return model.Parameter.SvmType
        End Function

        Public Function svm_get_nr_class(ByVal model As Model) As Integer
            Return model.NumberOfClasses
        End Function

        Public Sub svm_get_labels(ByVal model As Model, ByVal label As Integer())
            If model.ClassLabels IsNot Nothing Then
                For i = 0 To model.NumberOfClasses - 1
                    label(i) = model.ClassLabels(i)
                Next
            End If
        End Sub

        Public Sub svm_get_sv_indices(ByVal model As Model, ByVal indices As Integer())
            If model.SupportVectorIndices IsNot Nothing Then
                For i = 0 To model.SupportVectorCount - 1
                    indices(i) = model.SupportVectorIndices(i)
                Next
            End If
        End Sub

        Public Function svm_get_nr_sv(ByVal model As Model) As Integer
            Return model.SupportVectorCount
        End Function

        Public Function svm_get_svr_probability(ByVal model As Model) As Double
            If (model.Parameter.SvmType = SvmType.EPSILON_SVR OrElse model.Parameter.SvmType = SvmType.NU_SVR) AndAlso model.PairwiseProbabilityA IsNot Nothing Then
                Return model.PairwiseProbabilityA(0)
            Else
                Console.Error.Write("Model doesn't contain information for SVR probability inference" & ASCII.LF)
                Return 0
            End If
        End Function

        Public Function svm_predict_values(ByVal model As Model, ByVal x As Node(), ByVal dec_values As Double()) As Double
            Dim i As Integer

            If model.Parameter.SvmType = SvmType.ONE_CLASS OrElse model.Parameter.SvmType = SvmType.EPSILON_SVR OrElse model.Parameter.SvmType = SvmType.NU_SVR Then
                Dim sv_coef = model.SupportVectorCoefficients(0)
                Dim sum As Double = 0

                For i = 0 To model.SupportVectorCount - 1
                    sum += sv_coef(i) * Kernel.KernelFunction(x, model.SupportVectors(i), model.Parameter)
                Next

                sum -= model.Rho(0)
                dec_values(0) = sum

                If model.Parameter.SvmType = SvmType.ONE_CLASS Then
                    Return If(sum > 0, 1, -1)
                Else
                    Return sum
                End If
            Else
                Dim nr_class = model.NumberOfClasses
                Dim l = model.SupportVectorCount
                Dim kvalue = New Double(l - 1) {}

                For i = 0 To l - 1
                    kvalue(i) = Kernel.KernelFunction(x, model.SupportVectors(i), model.Parameter)
                Next

                Dim start = New Integer(nr_class - 1) {}
                start(0) = 0

                For i = 1 To nr_class - 1
                    start(i) = start(i - 1) + model.NumberOfSVPerClass(i - 1)
                Next

                Dim vote = New Integer(nr_class - 1) {}

                For i = 0 To nr_class - 1
                    vote(i) = 0
                Next

                Dim p = 0

                For i = 0 To nr_class - 1

                    For j = i + 1 To nr_class - 1
                        Dim sum As Double = 0
                        Dim si = start(i)
                        Dim sj = start(j)
                        Dim ci = model.NumberOfSVPerClass(i)
                        Dim cj = model.NumberOfSVPerClass(j)
                        Dim k As Integer
                        Dim coef1 = model.SupportVectorCoefficients(j - 1)
                        Dim coef2 = model.SupportVectorCoefficients(i)

                        For k = 0 To ci - 1
                            sum += coef1(si + k) * kvalue(si + k)
                        Next

                        For k = 0 To cj - 1
                            sum += coef2(sj + k) * kvalue(sj + k)
                        Next

                        sum -= model.Rho(p)
                        dec_values(p) = sum

                        If dec_values(p) > 0 Then
                            Threading.Interlocked.Increment(vote(i))
                        Else
                            Threading.Interlocked.Increment(vote(j))
                        End If

                        p += 1
                    Next
                Next

                Dim vote_max_idx = 0

                For i = 1 To nr_class - 1
                    If vote(i) > vote(vote_max_idx) Then vote_max_idx = i
                Next

                Return model.ClassLabels(vote_max_idx)
            End If
        End Function

        Public Function svm_predict(ByVal model As Model, ByVal x As Node()) As Double
            Dim nr_class = model.NumberOfClasses
            Dim dec_values As Double()

            If model.Parameter.SvmType = SvmType.ONE_CLASS OrElse model.Parameter.SvmType = SvmType.EPSILON_SVR OrElse model.Parameter.SvmType = SvmType.NU_SVR Then
                dec_values = New Double(0) {}
            Else
                dec_values = New Double(CInt(nr_class * (nr_class - 1) / 2) - 1) {}
            End If

            Dim pred_result = svm_predict_values(model, x, dec_values)
            Return pred_result
        End Function

        Public Function svm_predict_probability(ByVal model As Model, ByVal x As Node(), ByVal prob_estimates As Double()) As Double
            If (model.Parameter.SvmType = SvmType.C_SVC OrElse model.Parameter.SvmType = SvmType.NU_SVC) AndAlso model.PairwiseProbabilityA IsNot Nothing AndAlso model.PairwiseProbabilityB IsNot Nothing Then
                Dim i As Integer
                Dim nr_class = model.NumberOfClasses
                Dim dec_values = New Double(CInt(nr_class * (nr_class - 1) / 2) - 1) {}
                svm_predict_values(model, x, dec_values)
                Dim min_prob = 0.0000001
                Dim pairwise_prob = New Double(nr_class - 1, nr_class - 1) {}
                Dim k = 0

                For i = 0 To nr_class - 1

                    For j = i + 1 To nr_class - 1
                        pairwise_prob(i, j) = stdNum.Min(stdNum.Max(sigmoid_predict(dec_values(k), model.PairwiseProbabilityA(k), model.PairwiseProbabilityB(k)), min_prob), 1 - min_prob)
                        pairwise_prob(j, i) = 1 - pairwise_prob(i, j)
                        k += 1
                    Next
                Next

                multiclass_probability(nr_class, pairwise_prob, prob_estimates)
                Dim prob_max_idx = 0

                For i = 1 To nr_class - 1
                    If prob_estimates(i) > prob_estimates(prob_max_idx) Then prob_max_idx = i
                Next

                Return model.ClassLabels(prob_max_idx)
            Else
                Return svm_predict(model, x)
            End If
        End Function

        Public Function svm_check_parameter(ByVal prob As Problem, ByVal param As Parameter) As String
            ' svm_type

            Dim svm_type = param.SvmType

            ' kernel_type, degree

            Dim kernel_type = param.KernelType
            If param.Gamma < 0 Then Return "gamma < 0"
            If param.Degree < 0 Then Return "degree of polynomial kernel < 0"

            ' cache_size,eps,C,nu,p,shrinking

            If param.CacheSize <= 0 Then Return "cache_size <= 0"
            If param.EPS <= 0 Then Return "eps <= 0"

            If svm_type = SvmType.C_SVC OrElse svm_type = SvmType.EPSILON_SVR OrElse svm_type = SvmType.NU_SVR Then
                If param.C <= 0 Then Return "C <= 0"
            End If

            If svm_type = SvmType.NU_SVC OrElse svm_type = SvmType.ONE_CLASS OrElse svm_type = SvmType.NU_SVR Then
                If param.Nu <= 0 OrElse param.Nu > 1 Then Return "nu <= 0 or nu > 1"
            End If

            If svm_type = SvmType.EPSILON_SVR Then
                If param.P < 0 Then Return "p < 0"
            End If

            If param.Probability AndAlso svm_type = SvmType.ONE_CLASS Then Return "one-class SVM probability output not supported yet"

            ' check whether nu-svc is feasible

            If svm_type = SvmType.NU_SVC Then
                Dim l = prob.Count
                Dim max_nr_class = 16
                Dim nr_class = 0
                Dim label = New Integer(max_nr_class - 1) {}
                Dim count = New Integer(max_nr_class - 1) {}
                Dim i As Integer

                For i = 0 To l - 1
                    Dim this_label As Integer = CInt(prob.Y(i))
                    Dim j As Integer

                    For j = 0 To nr_class - 1

                        If this_label = label(j) Then
                            Threading.Interlocked.Increment(count(j))
                            Exit For
                        End If
                    Next

                    If j = nr_class Then
                        If nr_class = max_nr_class Then
                            max_nr_class *= 2
                            Dim new_data = New Integer(max_nr_class - 1) {}
                            Array.Copy(label, 0, new_data, 0, label.Length)
                            label = new_data
                            new_data = New Integer(max_nr_class - 1) {}
                            Array.Copy(count, 0, new_data, 0, count.Length)
                            count = new_data
                        End If

                        label(nr_class) = this_label
                        count(nr_class) = 1
                        Threading.Interlocked.Increment(nr_class)
                    End If
                Next

                For i = 0 To nr_class - 1
                    Dim n1 = count(i)

                    For j = i + 1 To nr_class - 1
                        Dim n2 = count(j)
                        If param.Nu * (n1 + n2) / 2 > stdNum.Min(n1, n2) Then Return "specified nu is infeasible"
                    Next
                Next
            End If

            Return Nothing
        End Function

        Public Function svm_check_probability_model(ByVal model As Model) As Integer
            If (model.Parameter.SvmType = SvmType.C_SVC OrElse model.Parameter.SvmType = SvmType.NU_SVC) AndAlso model.PairwiseProbabilityA IsNot Nothing AndAlso model.PairwiseProbabilityB IsNot Nothing OrElse (model.Parameter.SvmType = SvmType.EPSILON_SVR OrElse model.Parameter.SvmType = SvmType.NU_SVR) AndAlso model.PairwiseProbabilityA IsNot Nothing Then
                Return 1
            Else
                Return 0
            End If
        End Function
    End Module
End Namespace
