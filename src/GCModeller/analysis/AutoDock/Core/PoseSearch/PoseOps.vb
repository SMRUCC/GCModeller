Namespace Core


    ''' <summary>姿态算子（应用 DOF 到初始坐标；增量更新；rotvec ↔ 矩阵）</summary>
    Public Module PoseOps

        Public Function RotvecToMatrix(rotvec() As Double) As Double(,)
            Dim th = Math.Sqrt(rotvec(0) * rotvec(0) + rotvec(1) * rotvec(1) + rotvec(2) * rotvec(2))
            Dim m(2, 2) As Double
            If th < 0.000000000001 Then
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
            If th < 0.000000001 Then
                Return {0.5 * (m(2, 1) - m(1, 2)), 0.5 * (m(0, 2) - m(2, 0)), 0.5 * (m(1, 0) - m(0, 1))}
            End If
            If Abs(Math.PI - th) < 0.00001 Then
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
                If n2 < 0.000000000001 Then Continue For
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

End Namespace