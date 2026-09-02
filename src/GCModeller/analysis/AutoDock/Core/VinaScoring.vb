' ============================================================================
' VinaScoring.vb — Vina 打分函数 + 力累积 + 解析梯度
' ----------------------------------------------------------------------------
' [README §1.1] e_pair(d) = w1·gauss1 + w2·gauss2 + w3·Repulsion
'                          + w4·Hydrophobic + w5·HBond，d = r_ij − R_i − R_j
'   权重（Trott 2010 表1）：w1=−0.0356, w2=−0.00516, w3=0.840, w4=−0.0351, w5=−0.587
'   gauss1: o=0, σ=1.5；gauss2: o=3, σ=2；疏水开关 0.5/1.5；氢键 −0.7/0
'   截止 8 Å。[式1-2] ΔG = 0.0585·N_rot + c_inter
'
' [README §1.3] 梯度（切空间，BFGS 以增量姿态应用方向）：
'   平移  = −ΣF                     （F_i = −de/dd·u_ij，u_ij 指向 i）
'   旋转  = −Σ (x_i − c_rigid)×F_i  （c_rigid = c0 + t，刚体旋转中心）
'   扭转k = −â·Σ_{i∈branch} (x_i − p)×F_i （最终几何；共轭律，Python 预验证通过）
'
' 近邻查询用 CellGrid（cell = cutoff），复杂度 O(配体原子 × 邻居数)。
' ============================================================================

Imports System
Imports System.Collections.Generic

Namespace MiniDock.Core

    ''' <summary>空间网格（配体求受体近邻用）</summary>
    Public Class CellGrid

        Private ReadOnly _cells As New Dictionary(Of Int64, List(Of Int32))()
        Private ReadOnly _cellSize As Double
        Private ReadOnly _atoms As List(Of Atom)

        Public Sub New(atoms As List(Of Atom), cellSize As Double)
            _atoms = atoms
            _cellSize = cellSize
            For i = 0 To atoms.Count - 1
                Dim key = CellKey(atoms(i).X, atoms(i).Y, atoms(i).Z)
                Dim list As List(Of Int32) = Nothing
                If Not _cells.TryGetValue(key, list) Then
                    list = New List(Of Int32)()
                    _cells(key) = list
                End If
                list.Add(i)
            Next
        End Sub

        Private Function CellKey(x As Double, y As Double, z As Double) As Int64
            Dim cx = CInt(Math.Floor(x / _cellSize))
            Dim cy = CInt(Math.Floor(y / _cellSize))
            Dim cz = CInt(Math.Floor(z / _cellSize))
            Return (CLng(cx + 4096) << 26) Or (CLng(cy + 4096) << 13) Or CLng(cz + 4096)
        End Function

        ''' <summary>访问 (x,y,z) 半径 radius 内的全部原子（回调避免分配）</summary>
        Public Sub ForNeighbors(x As Double, y As Double, z As Double, radius As Double,
                                action As Action(Of Atom))
            Dim cx = CInt(Math.Floor(x / _cellSize))
            Dim cy = CInt(Math.Floor(y / _cellSize))
            Dim cz = CInt(Math.Floor(z / _cellSize))
            Dim span = CInt(Math.Ceiling(radius / _cellSize))
            Dim r2 = radius * radius
            For dx = -span To span
                For dy = -span To span
                    For dz = -span To span
                        Dim list As List(Of Int32) = Nothing
                        If _cells.TryGetValue(CellKeyOf(cx + dx, cy + dy, cz + dz), list) Then
                            For Each idx In list
                                Dim a = _atoms(idx)
                                Dim ddx = a.X - x, ddy = a.Y - y, ddz = a.Z - z
                                If ddx * ddx + ddy * ddy + ddz * ddz <= r2 Then
                                    action(a)
                                End If
                            Next
                        End If
                    Next
                Next
            Next
        End Sub

        Private Function CellKeyOf(cx As Int32, cy As Int32, cz As Int32) As Int64
            Return (CLng(cx + 4096) << 26) Or (CLng(cy + 4096) << 13) Or CLng(cz + 4096)
        End Function

    End Class

    ''' <summary>Vina 打分上下文（一次构建，多姿态复用）</summary>
    Public Class VinaScorer

        Public Const W1 As Double = -0.0356
        Public Const W2 As Double = -0.00516
        Public Const W3 As Double = 0.84
        Public Const W4 As Double = -0.0351
        Public Const W5 As Double = -0.587
        Public Const G1O As Double = 0.0
        Public Const G1W As Double = 1.5
        Public Const G2O As Double = 3.0
        Public Const G2W As Double = 2.0
        Public Const HbLo As Double = -0.7
        Public Const HbHi As Double = 0.0
        Public Const HydLo As Double = 0.5
        Public Const HydHi As Double = 1.5
        Public Const Cutoff As Double = 8.0
        Public Const RotatableWeight As Double = 0.0585

        Private ReadOnly _recAtoms As List(Of Atom)
        Private ReadOnly _recGrid As CellGrid

        ''' <summary>受体重建网格（蛋白/水，作为刚性环境）</summary>
        Public Sub New(receptorAtoms As List(Of Atom))
            _recAtoms = receptorAtoms
            _recGrid = New CellGrid(receptorAtoms, Cutoff)
        End Sub

        ''' <summary>单原子对（返回能量与作用在 a 上的力）</summary>
        Public Shared Function ScorePair(ax As Double, ay As Double, az As Double,
                                         bx As Double, by As Double, bz As Double,
                                         ta As Int32, tb As Int32,
                                         ByRef fx As Double, ByRef fy As Double, ByRef fz As Double) As Double
            Dim dx = ax - bx
            Dim dy = ay - by
            Dim dz = az - bz
            Dim r2 = dx * dx + dy * dy + dz * dz
            If r2 > Cutoff * Cutoff OrElse r2 < 1.0E-9 Then
                fx = 0 : fy = 0 : fz = 0
                Return 0.0
            End If
            Dim r = Math.Sqrt(r2)
            Dim d = r - VinaAtomTypes.Radii(ta) - VinaAtomTypes.Radii(tb)

            Dim x1 = d - G1O
            Dim x2 = d - G2O
            Dim g1 = Math.Exp(-(x1 / G1W) * (x1 / G1W))
            Dim g2 = Math.Exp(-(x2 / G2W) * (x2 / G2W))
            Dim e = W1 * g1 + W2 * g2
            Dim de = W1 * g1 * (-2.0 * x1 / (G1W * G1W)) + W2 * g2 * (-2.0 * x2 / (G2W * G2W))

            If d <= 0 Then
                e += W3 * d * d
                de += W3 * 2.0 * d
            End If
            If VinaAtomTypes.Hydrophobic(ta) AndAlso VinaAtomTypes.Hydrophobic(tb) Then
                If d <= HydLo Then
                    e += W4
                ElseIf d < HydHi Then
                    e += W4 * (HydHi - d) / (HydHi - HydLo)
                    de += W4 * (-1.0 / (HydHi - HydLo))
                End If
            End If
            If (VinaAtomTypes.Acceptor(ta) AndAlso VinaAtomTypes.Donor(tb)) OrElse
               (VinaAtomTypes.Acceptor(tb) AndAlso VinaAtomTypes.Donor(ta)) Then
                If d <= HbLo Then
                    e += W5
                ElseIf d < HbHi Then
                    e += W5 * (HbHi - d) / (HbHi - HbLo)
                    de += W5 * (-1.0 / (HbHi - HbLo))
                End If
            End If

            Dim u = de / r
            fx = -u * dx
            fy = -u * dy
            fz = -u * dz
            Return e
        End Function

        ''' <summary>
        ''' 配体姿态能量与梯度（配体原子坐标已变换到最终位置）。
        ''' interPairs：仅配体×受体；intraPairs：配体内距 &gt;3 键的原子对。
        ''' grads 长度 = 6 + nTorsions（trans3, rot3, torsionN）。
        ''' </summary>
        Public Function Evaluate(ligAtoms As List(Of Atom),
                                 rigidCenter() As Double,
                                 intraI() As Int32, intraJ() As Int32,
                                 torsionAxes() As Int32,
                                 branches As List(Of List(Of Int32)),
                                 grads() As Double) As Double
            Dim inter As Double = 0.0
            Dim n = ligAtoms.Count
            Dim fx(n - 1) As Double
            Dim fy(n - 1) As Double
            Dim fz(n - 1) As Double

            ' ---- 配体 × 受体（网格近邻）----
            For i = 0 To n - 1
                Dim a = ligAtoms(i)
                Dim ta = a.VinaType
                Dim ex As Double = 0, ey As Double = 0, ez As Double = 0
                _recGrid.ForNeighbors(a.X, a.Y, a.Z, Cutoff,
                    Sub(b As Atom)
                        Dim tfx As Double, tfy As Double, tfz As Double
                        inter += ScorePair(a.X, a.Y, a.Z, b.X, b.Y, b.Z, ta, b.VinaType, tfx, tfy, tfz)
                        ex += tfx : ey += tfy : ez += tfz
                    End Sub)
                fx(i) = ex : fy(i) = ey : fz(i) = ez
            Next

            ' ---- 配体内 1-4 以上原子对 ----
            If intraI IsNot Nothing Then
                For k = 0 To intraI.Length - 1
                    Dim i = intraI(k)
                    Dim j = intraJ(k)
                    Dim a = ligAtoms(i)
                    Dim b = ligAtoms(j)
                    Dim tfx As Double, tfy As Double, tfz As Double
                    inter += ScorePair(a.X, a.Y, a.Z, b.X, b.Y, b.Z, a.VinaType, b.VinaType, tfx, tfy, tfz)
                    fx(i) += tfx : fy(i) += tfy : fz(i) += tfz
                    fx(j) -= tfx : fy(j) -= tfy : fz(j) -= tfz
                Next
            End If

            ' ---- 梯度组装 [README §1.3] ----
            ' 平移 = -ΣF；旋转 = -(x - c_rigid)×F；扭转k = -â·Σ_{i∈branch}(x-p)×F
            Dim sgx As Double = 0, sgy As Double = 0, sgz As Double = 0
            Dim tx As Double = 0, ty As Double = 0, tz As Double = 0
            For i = 0 To n - 1
                sgx += fx(i) : sgy += fy(i) : sgz += fz(i)
                Dim rx = ligAtoms(i).X - rigidCenter(0)
                Dim ry = ligAtoms(i).Y - rigidCenter(1)
                Dim rz = ligAtoms(i).Z - rigidCenter(2)
                tx += ry * fz(i) - rz * fy(i)
                ty += rz * fx(i) - rx * fz(i)
                tz += rx * fy(i) - ry * fx(i)
            Next
            grads(0) = -sgx
            grads(1) = -sgy
            grads(2) = -sgz
            grads(3) = -tx
            grads(4) = -ty
            grads(5) = -tz
            For t = 0 To branches.Count - 1
                Dim ai = torsionAxes(2 * t)
                Dim bi = torsionAxes(2 * t + 1)
                Dim pax = ligAtoms(ai).X
                Dim pay = ligAtoms(ai).Y
                Dim paz = ligAtoms(ai).Z
                Dim axv = ligAtoms(bi).X - pax
                Dim ayv = ligAtoms(bi).Y - pay
                Dim azv = ligAtoms(bi).Z - paz
                Dim na = Math.Sqrt(axv * axv + ayv * ayv + azv * azv)
                If na < 1.0E-9 Then
                    grads(6 + t) = 0
                    Continue For
                End If
                Dim acc As Double = 0
                For Each u In branches(t)
                    Dim rx = ligAtoms(u).X - pax
                    Dim ry = ligAtoms(u).Y - pay
                    Dim rz = ligAtoms(u).Z - paz
                    Dim c1 = ry * fz(u) - rz * fy(u)
                    Dim c2 = rz * fx(u) - rx * fz(u)
                    Dim c3 = rx * fy(u) - ry * fx(u)
                    acc += (axv * c1 + ayv * c2 + azv * c3) / na
                Next
                grads(6 + t) = -acc
            Next
            Return inter
        End Function

    End Class

End Namespace
