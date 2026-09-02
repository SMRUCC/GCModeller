' ============================================================================
' PoseSearch.vb — 姿态 DOF、BFGS、ILS 全局搜索
' ----------------------------------------------------------------------------
' [README §1.2/§1.3] ILS：随机初始姿态 → BFGS 局部精修 → 扰动 → Metropolis 判据；
' BFGS 使用切空间梯度（平移/负总扭矩/轴投影负扭矩），方向以增量姿态应用：
'   trans += α·p；rotvec ← log(R(α·p_rot)·R(rotvec))；torsions += α·p_tors
' Metropolis 温度 T = 293.15 K × 0.001987 kcal/(mol·K) ≈ 0.582（Vina 同款）。
' ============================================================================

Imports System.Math

Namespace Core

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
