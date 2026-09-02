' ============================================================================
' PoseSearch.vb — 姿态 DOF、BFGS、ILS 全局搜索
' ----------------------------------------------------------------------------
' [README §1.2/§1.3] ILS：随机初始姿态 → BFGS 局部精修 → 扰动 → Metropolis 判据；
' BFGS 使用切空间梯度（平移/负总扭矩/轴投影负扭矩），方向以增量姿态应用：
'   trans += α·p；rotvec ← log(R(α·p_rot)·R(rotvec))；torsions += α·p_tors
' Metropolis 温度 T = 293.15 K × 0.001987 kcal/(mol·K) ≈ 0.582（Vina 同款）。
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Math

Namespace MiniDock.Core

    ''' <summary>姿态算子（应用 DOF 到初始坐标；增量更新；rotvec ↔ 矩阵）</summary>
    Public Module PoseOps

        Public Function RotvecToMatrix(rotvec() As Double) As Double(,)
            Dim th = Math.Sqrt(rotvec(0) * rotvec(0) + rotvec(1) * rotvec(1) + rotvec(2) * rotvec(2))
            Dim m(2, 2) As Double
            If th < 1.0E-12 Then
                m(0, 0) = 1 : m(1, 1) = 1 : m(2, 2) = 1
                Return m
            End If
            Dim x = rotvec(0) / th, y = rotvec(1) / th, z = rotvec(2) / th
            Dim c = Cos(th), s = Sin(th), cc = 1 - c
            m(0, 0) = c + x * x * cc : m(0, 1) = x * y * cc - z * s : m(0, 2) = x * z * cc + y * s
            m(1, 0) = y * x * cc + z * s : m(1, 1) = c + y * y * cc : m(1, 2) = y * z * cc - x * s
            m(2, 0) = z * x * cc - y * s : m(2, 1) = z * y * cc + x * s : m(2, 2) = c + z * z * cc
            Return m
        End Function

        Public Function MatrixToRotvec(m As Double(,)) As Double()
            Dim cosT = 0.5 * (m(0, 0) + m(1, 1) + m(2, 2) - 1.0)
            cosT = Max(-1.0, Min(1.0, cosT))
            Dim th = Acos(cosT)
            If th < 1.0E-9 Then
                Return {0.5 * (m(2, 1) - m(1, 2)), 0.5 * (m(0, 2) - m(2, 0)), 0.5 * (m(1, 0) - m(0, 1))}
            End If
            If Abs(Math.PI - th) < 1.0E-5 Then
                Dim k = th / (2.0 * (1.0 + cosT))
                Return {k * (m(0, 0) + 1.0), k * (m(1, 1) + 1.0), k * (m(2, 2) + 1.0)}
            End If
            Dim s = 2.0 * Sin(th)
            Return {(m(2, 1) - m(1, 2)) / s * th,
                    (m(0, 2) - m(2, 0)) / s * th,
                    (m(1, 0) - m(0, 1)) / s * th}
        End Function

        Public Function MatMul(a As Double(,), b As Double(,)) As Double(,)
            Dim r(2, 2) As Double
            For i = 0 To 2
                For j = 0 To 2
                    Dim s As Double = 0
                    For k = 0 To 2
                        s += a(i, k) * b(k, j)
                    Next
                    r(i, j) = s
                Next
            Next
            Return r
        End Function

        ''' <summary>应用完整姿态（刚体 + 扭转树，父先子后）。返回最终坐标与刚体中心 c0+t</summary>
        Public Sub ApplyPose(baseCoords(,) As Double,
                             trans() As Double, rotvec() As Double,
                             axes() As Int32, branches As List(Of List(Of Int32)),
                             torsions() As Double,
                             outPos() As Double, ByRef rigidCenterX As Double,
                             ByRef rigidCenterY As Double, ByRef rigidCenterZ As Double)
            Dim n = baseCoords.GetLength(0)
            Dim cx As Double = 0, cy As Double = 0, cz As Double = 0
            For i = 0 To n - 1
                cx += baseCoords(i, 0) : cy += baseCoords(i, 1) : cz += baseCoords(i, 2)
            Next
            cx /= n : cy /= n : cz /= n

            Dim R = RotvecToMatrix(rotvec)
            For i = 0 To n - 1
                Dim rx = baseCoords(i, 0) - cx
                Dim ry = baseCoords(i, 1) - cy
                Dim rz = baseCoords(i, 2) - cz
                outPos(3 * i) = R(0, 0) * rx + R(0, 1) * ry + R(0, 2) * rz + cx + trans(0)
                outPos(3 * i + 1) = R(1, 0) * rx + R(1, 1) * ry + R(1, 2) * rz + cy + trans(1)
                outPos(3 * i + 2) = R(2, 0) * rx + R(2, 1) * ry + R(2, 2) * rz + cz + trans(2)
            Next
            rigidCenterX = cx + trans(0)
            rigidCenterY = cy + trans(1)
            rigidCenterZ = cz + trans(2)

            For t = 0 To branches.Count - 1
                Dim ai = axes(2 * t)
                Dim bi = axes(2 * t + 1)
                Dim pax = outPos(3 * ai)
                Dim pay = outPos(3 * ai + 1)
                Dim paz = outPos(3 * ai + 2)
                Dim theta = torsions(t)
                Dim axv = outPos(3 * bi) - pax
                Dim ayv = outPos(3 * bi + 1) - pay
                Dim azv = outPos(3 * bi + 2) - paz
                ' 以 (axis, theta) 直接构造旋转（无需归一化回调）
                Dim n2 = Sqrt(axv * axv + ayv * ayv + azv * azv)
                If n2 < 1.0E-12 Then Continue For
                Dim ux = axv / n2, uy = ayv / n2, uz = azv / n2
                Dim cc = Cos(theta), ss = Sin(theta), ccc = 1 - cc
                Dim M(2, 2) As Double
                M(0, 0) = cc + ux * ux * ccc : M(0, 1) = ux * uy * ccc - uz * ss : M(0, 2) = ux * uz * ccc + uy * ss
                M(1, 0) = uy * ux * ccc + uz * ss : M(1, 1) = cc + uy * uy * ccc : M(1, 2) = uy * uz * ccc - ux * ss
                M(2, 0) = uz * ux * ccc - uy * ss : M(2, 1) = uz * uy * ccc + ux * ss : M(2, 2) = cc + uz * uz * ccc
                For Each u In branches(t)
                    Dim rx = outPos(3 * u) - pax
                    Dim ry = outPos(3 * u + 1) - pay
                    Dim rz = outPos(3 * u + 2) - paz
                    outPos(3 * u) = pax + M(0, 0) * rx + M(0, 1) * ry + M(0, 2) * rz
                    outPos(3 * u + 1) = pay + M(1, 0) * rx + M(1, 1) * ry + M(1, 2) * rz
                    outPos(3 * u + 2) = paz + M(2, 0) * rx + M(2, 1) * ry + M(2, 2) * rz
                Next
            Next
        End Sub

        ''' <summary>增量姿态更新：trans += αp；rotvec ← log(R(αp_rot)·R(rotvec))；torsions += αp</summary>
        Public Sub ApplyIncrement(ByRef trans() As Double, ByRef rotvec() As Double,
                                  ByRef torsions() As Double, direction() As Double, alpha As Double)
            trans(0) += alpha * direction(0)
            trans(1) += alpha * direction(1)
            trans(2) += alpha * direction(2)
            Dim Rd = RotvecToMatrix({alpha * direction(3), alpha * direction(4), alpha * direction(5)})
            Dim Rcur = RotvecToMatrix(rotvec)
            Dim Rn = MatMul(Rd, Rcur)
            Dim nv = MatrixToRotvec(Rn)
            rotvec(0) = nv(0) : rotvec(1) = nv(1) : rotvec(2) = nv(2)
            For t = 0 To torsions.Length - 1
                torsions(t) += alpha * direction(6 + t)
            Next
        End Sub

    End Module

    ''' <summary>BFGS 目标函数适配器</summary>
    Public Interface IPoseObjective

        ''' <summary>评估能量与梯度；grads() 就地填充；返回能量</summary>
        Function Evaluate(trans() As Double, rotvec() As Double, torsions() As Double,
                          grads() As Double, rigidCenter() As Double) As Double

        ReadOnly Property NumTorsions As Int32

    End Interface

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
                gnorm = Sqrt(gnorm)
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

    ''' <summary>ILS 全局搜索 [README §1.2 步骤3]</summary>
    Public Class IlsSearcher

        Public NumRuns As Int32 = 8              ' exhaustiveness
        Public StepsPerRun As Int32 = 30
        Public BfgsMaxIter As Int32 = 300
        Public TransAmplitude As Double = 1.0
        Public RotAmplitude As Double = 0.3
        Public TorsionAmplitude As Double = 0.4
        Public Temperature As Double = 293.15 * 0.001987   ' ≈ 0.582 kcal/mol

        Private ReadOnly _rng As Random

        Public Sub New(seed As Int32)
            _rng = If(seed = 0, New Random(), New Random(seed))
        End Sub

        Public Class IlarResult

            Public Trans() As Double
            Public Rotvec() As Double
            Public Torsions() As Double
            Public Energy As Double

        End Class

        ''' <summary>
        ''' 对同一目标跑 NumRuns 次 ILS，返回能量升序的互异姿态（RMSD ≥ minRmsd）。
        ''' </summary>
        Public Function Search(obj As IPoseObjective,
                               boxCenter() As Double, boxHalf As Double,
                               numModes As Int32, minRmsd As Double,
                               buildCoords As Func(Of IlarResult, Double())) As List(Of IlarResult)
            Dim results As New List(Of IlarResult)()
            Dim nTors = obj.NumTorsions

            For run = 1 To NumRuns
                ' 初始姿态：盒内随机平移 + 随机旋转 + 随机扭转
                Dim trans(2) As Double
                Dim rotvec(2) As Double
                Dim torsions(nTors - 1) As Double
                trans(0) = boxCenter(0) + (_rng.NextDouble() * 2 - 1) * boxHalf
                trans(1) = boxCenter(1) + (_rng.NextDouble() * 2 - 1) * boxHalf
                trans(2) = boxCenter(2) + (_rng.NextDouble() * 2 - 1) * boxHalf
                RandomUnitRotvec(rotvec)
                For t = 0 To nTors - 1
                    torsions(t) = (_rng.NextDouble() * 2 - 1) * Math.PI
                Next

                Dim bfgs As New BfgsMinimizer With {.MaxIterations = BfgsMaxIter, .Gtol = 0.0001}
                Dim ev As Int32 = 0
                Dim f = bfgs.Minimize(obj, trans, rotvec, torsions, ev)

                For [step] = 1 To StepsPerRun
                    Dim pt = CType(trans.Clone(), Double())
                    Dim pr = CType(rotvec.Clone(), Double())
                    Dim ps = CType(torsions.Clone(), Double())
                    Dim dirV(6 + nTors - 1) As Double
                    dirV(0) = (_rng.NextDouble() * 2 - 1) * TransAmplitude
                    dirV(1) = (_rng.NextDouble() * 2 - 1) * TransAmplitude
                    dirV(2) = (_rng.NextDouble() * 2 - 1) * TransAmplitude
                    dirV(3) = (_rng.NextDouble() * 2 - 1) * RotAmplitude
                    dirV(4) = (_rng.NextDouble() * 2 - 1) * RotAmplitude
                    dirV(5) = (_rng.NextDouble() * 2 - 1) * RotAmplitude
                    For t = 0 To nTors - 1
                        dirV(6 + t) = (_rng.NextDouble() * 2 - 1) * TorsionAmplitude
                    Next
                    PoseOps.ApplyIncrement(pt, pr, ps, dirV, 1.0)

                    Dim b2 As New BfgsMinimizer With {.MaxIterations = 150, .Gtol = 0.0001}
                    Dim ev2 As Int32 = 0
                    Dim fNew = b2.Minimize(obj, pt, pr, ps, ev2)

                    ' Metropolis 判据 [README §1.2]
                    Dim dc = fNew - f
                    If dc < 0 OrElse _rng.NextDouble() < Exp(-dc / Temperature) Then
                        trans = pt : rotvec = pr : torsions = ps : f = fNew
                    End If
                Next

                results.Add(New IlarResult With {.Trans = trans, .Rotvec = rotvec,
                                                 .Torsions = torsions, .Energy = f})
            Next

            ' 按能量排序 + RMSD 去重（配体重原子坐标）
            results.Sort(Function(a, b) a.Energy.CompareTo(b.Energy))
            Dim kept As New List(Of IlarResult)()
            Dim keptCoords As New List(Of Double())()
            For Each r In results
                Dim coords = buildCoords(r)
                Dim dup = False
                For Each kc In keptCoords
                    If Rmsd(coords, kc) < minRmsd Then
                        dup = True
                        Exit For
                    End If
                Next
                If Not dup Then
                    kept.Add(r)
                    keptCoords.Add(coords)
                End If
                If kept.Count >= numModes Then Exit For
            Next
            Return kept
        End Function

        ''' <summary>重原子 RMSD（坐标为 x,y,z 平铺数组，长度须一致）</summary>
        Public Shared Function Rmsd(a() As Double, b() As Double) As Double
            If a.Length <> b.Length OrElse a.Length = 0 Then Return 999.0
            Dim s As Double = 0
            For i = 0 To a.Length - 1
                Dim d = a(i) - b(i)
                s += d * d
            Next
            Return Sqrt(s / (a.Length \ 3))
        End Function

        Private Sub RandomUnitRotvec(rotvec() As Double)
            ' 均匀随机方向 + 随机角（0..π）
            Dim u1 = _rng.NextDouble()
            Dim u2 = _rng.NextDouble()
            Dim u3 = _rng.NextDouble()
            Dim zz = 2.0 * u1 - 1.0
            Dim t = 2.0 * Math.PI * u2
            Dim r = Sqrt(Max(0.0, 1.0 - zz * zz))
            Dim axisX = r * Cos(t)
            Dim axisY = r * Sin(t)
            Dim axisZ = zz
            Dim angle = Math.PI * u3
            rotvec(0) = axisX * angle
            rotvec(1) = axisY * angle
            rotvec(2) = axisZ * angle
        End Sub

    End Class

End Namespace
