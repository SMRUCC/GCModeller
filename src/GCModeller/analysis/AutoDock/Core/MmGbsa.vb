' ============================================================================
' MmGbsa.vb — MM-GBSA / Nwat-MMGBSA 重打分
' ----------------------------------------------------------------------------
' [README §3.1] ΔG_bind = ΔE_vdw + ΔE_elec + ΔG_polar(GB) + ΔG_nonpolar(γ·SASA) − TΔS
'   1A 单一轨迹变体：同一构象下内部项精确抵消，只需分子间 vdW/Coulomb
'   + 三个态（复合物/受体/配体）的溶剂化差。
'
'   ΔE_vdw   = Σ 4ε[(σ/r)^12 − (σ/r)^6]，σ = R*_i + R*_j，ε = √(ε_i ε_j)
'              （简化 Amber 参数：C 0.086/1.908, N 0.077/1.824, O 0.210/1.6612,
'               S 0.250/2.0, P 0.200/2.1, F 0.061/1.75, Cl 0.265/1.948,
'               Br 0.320/2.038, I 0.400/2.19，单位 kcal/mol 与 Å）
'   ΔE_elec  = Σ 332.0636·q_i·q_j / r    （ε=1，GB 项承担屏蔽）
'   ΔG_polar = G_pol(complex) − G_pol(rec) − G_pol(lig)
'              G_pol = −(1/2)(1 − 1/ε_w) Σ_ij q_i q_j / f_ij
'              f_ij = √(r² + R_i R_j exp(−r²/(4 R_i R_j)))，自项 f_ii = R_i
'              [简化 Born 模型：逐元素静态有效 Born 半径，未实现 OBC/Neck2
'               逐对去屏蔽——差分形式下误差部分抵消，文档化偏离]
'   ΔG_np    = γ·ΔSASA，γ = 0.0072 kcal/(mol·Å²)；SASA 用 Shrake-Rupley
'              （黄金螺旋 960 采样点，probe 1.4 Å）
'   熵项：默认忽略（README §4.2 建议），未实现
'
' [README §3.2 Nwat-MMGBSA] 取距配体最近的 Nwat 个水并入受体侧参与三个态
'   的能量计算（水 O 固定电荷 −0.8；最近定义用 O 原子到配体重原子的最小距离）。
' ============================================================================

Imports System
Imports System.Collections.Generic

Namespace MiniDock.Core

    Public Class MmGbsaResult

        Public DeltaG As Double
        Public Vdw As Double
        Public Elec As Double
        Public GbPolar As Double
        Public SasNonpolar As Double
        Public NwatRequested As Int32
        Public NwatSelected As Int32

    End Class

    Public Module MmGbsa

        Private ReadOnly Gamma As Double = 0.0072
        Private ReadOnly DielectricWater As Double = 78.5
        Private ReadOnly CoulombK As Double = 332.0636
        Private ReadOnly ProbeRadius As Double = 1.4
        Private ReadOnly SasPoints As Int32 = 960

        ' 简化 Amber LJ 参数：ε (kcal/mol), R* (Å, = r_min/2)
        Private ReadOnly LjParams As New Dictionary(Of String, Double()) From {
            {"C", {0.086, 1.908}}, {"N", {0.077, 1.824}}, {"O", {0.21, 1.6612}},
            {"S", {0.25, 2.0}}, {"P", {0.2, 2.1}}, {"F", {0.061, 1.75}},
            {"Cl", {0.265, 1.948}}, {"Br", {0.32, 2.038}}, {"I", {0.4, 2.19}},
            {"MG", {0.015, 1.36}}, {"CA", {0.05, 1.74}}, {"FE", {0.05, 1.25}},
            {"ZN", {0.05, 1.25}}, {"MN", {0.05, 1.39}}}

        ' 简化静态有效 Born 半径（Å）
        Private ReadOnly BornRadii As New Dictionary(Of String, Double) From {
            {"C", 2.0}, {"N", 1.75}, {"O", 1.6}, {"S", 2.0}, {"P", 2.0},
            {"F", 1.5}, {"Cl", 1.8}, {"Br", 2.0}, {"I", 2.1},
            {"MG", 1.2}, {"CA", 1.5}, {"FE", 1.2}, {"ZN", 1.2}, {"MN", 1.2}}

        Private Sub FillLjAndBorn(a As Atom)
            If a.LjEps > 0 Then Return   ' 已填
            Dim p() As Double = Nothing
            If Not LjParams.TryGetValue(a.Element, p) Then p = {0.086, 1.908}
            a.LjEps = p(0)
            a.LjRmin = p(1)
        End Sub

        Private Function BornRadiusOf(a As Atom) As Double
            Dim r As Double = 1.7
            If BornRadii.TryGetValue(a.Element, r) Then Return r
            Return 1.7
        End Function

        ''' <summary>
        ''' 单点 MM-GBSA。recAtoms：受体侧（含选定的 Nwat 水）；ligAtoms：配体侧。
        ''' 内部项抵消（1A 单一轨迹），只算分子间 vdW/Coulomb + 三态溶剂化差。
        ''' </summary>
        Public Function EvaluateSinglePoint(recAtoms As List(Of Atom), ligAtoms As List(Of Atom)) As MmGbsaResult
            For Each a In recAtoms : FillLjAndBorn(a) : Next
            For Each a In ligAtoms : FillLjAndBorn(a) : Next

            ' ---- ΔE_vdw + ΔE_elec（分子间）----
            Dim vdw As Double = 0
            Dim elec As Double = 0
            For Each la In ligAtoms
                Dim e1 = If(LjParams.ContainsKey(la.Element), la.LjEps, 0.086)
                Dim r1 = la.LjRmin
                For Each ra In recAtoms
                    Dim dx = la.X - ra.X
                    Dim dy = la.Y - ra.Y
                    Dim dz = la.Z - ra.Z
                    Dim r2 = dx * dx + dy * dy + dz * dz
                    If r2 > 144.0 OrElse r2 < 0.0001 Then Continue For   ' 12 Å 截止
                    Dim r = Math.Sqrt(r2)
                    Dim rmin = ra.LjRmin + r1
                    Dim e2 = If(LjParams.ContainsKey(ra.Element), ra.LjEps, 0.086)
                    Dim eps = Math.Sqrt(e1 * e2)
                    Dim s = rmin / r
                    Dim s6 = s * s * s * s * s * s
                    vdw += eps * (s6 * s6 - 2.0 * s6)
                    elec += CoulombK * la.Charge * ra.Charge / r
                Next
            Next

            ' ---- GB 三态差 ----
            Dim gComplex = GbEnergy(recAtoms, ligAtoms)
            Dim gRec = GbEnergy(recAtoms, Nothing)
            Dim gLig = GbEnergy(New List(Of Atom)(), ligAtoms)
            Dim gbPolar = gComplex - gRec - gLig

            ' ---- SASA 三态差 ----
            Dim sComplex = TotalSasa(recAtoms, ligAtoms)
            Dim sRec = TotalSasa(recAtoms, Nothing)
            Dim sLig = TotalSasa(New List(Of Atom)(), ligAtoms)
            Dim sasNonpolar = Gamma * (sComplex - sRec - sLig)

            Dim result As New MmGbsaResult With {
                .Vdw = vdw, .Elec = elec, .GbPolar = gbPolar,
                .SasNonpolar = sasNonpolar,
                .DeltaG = vdw + elec + gbPolar + sasNonpolar}
            Return result
        End Function

        ''' <summary>GB 极性溶剂化能（单态）</summary>
        Private Function GbEnergy(recAtoms As List(Of Atom), ligAtoms As List(Of Atom)) As Double
            ' 合并原子列表（避免分配：直接双段循环）
            Dim nRec = recAtoms.Count
            Dim nLig = If(ligAtoms IsNot Nothing, ligAtoms.Count, 0)
            Dim n = nRec + nLig
            If n = 0 Then Return 0
            Dim acc As Double = 0
            Dim scale = -0.5 * (1.0 - 1.0 / DielectricWater)

            ' 自项 + 交叉项（i<j 全对，含跨段）
            For i = 0 To n - 1
                Dim ai = If(i < nRec, recAtoms(i), ligAtoms(i - nRec))
                Dim Ri = BornRadiusOf(ai)
                ' 自项
                acc += ai.Charge * ai.Charge / Ri
                For j = i + 1 To n - 1
                    Dim aj = If(j < nRec, recAtoms(j), ligAtoms(j - nRec))
                    Dim dx = ai.X - aj.X
                    Dim dy = ai.Y - aj.Y
                    Dim dz = ai.Z - aj.Z
                    Dim r2 = dx * dx + dy * dy + dz * dz
                    Dim Rj = BornRadiusOf(aj)
                    Dim f As Double
                    If r2 < 1.0E-8 Then
                        f = 0.5 * (Ri + Rj)
                    Else
                        Dim r = Math.Sqrt(r2)
                        f = Math.Sqrt(r2 + Ri * Rj * Math.Exp(-r2 / (4.0 * Ri * Rj)))
                    End If
                    acc += ai.Charge * aj.Charge / f
                Next
            Next
            Return scale * acc
        End Function

        ''' <summary>全部原子 SASA 总和（Shrake-Rupley）</summary>
        Public Function TotalSasa(recAtoms As List(Of Atom), ligAtoms As List(Of Atom)) As Double
            ' 构建点集 + 邻居表
            Dim all As New List(Of Atom)()
            all.AddRange(recAtoms)
            If ligAtoms IsNot Nothing Then all.AddRange(ligAtoms)
            If all.Count = 0 Then Return 0

            ' 黄金螺旋球面点
            Dim pts(SasPoints - 1, 2) As Double
            Dim offset = 2.0 / SasPoints
            Dim inc = Math.PI * (3.0 - Math.Sqrt(5.0))
            For k = 0 To SasPoints - 1
                Dim y = ((k * offset) - 1.0) + offset / 2.0
                Dim r = Math.Sqrt(Max(0.0, 1.0 - y * y))
                Dim phi = k * inc
                pts(k, 0) = Math.Cos(phi) * r
                pts(k, 1) = y
                pts(k, 2) = Math.Sin(phi) * r
            Next

            ' 半径表 + 网格
            Dim radii(all.Count - 1) As Double
            For i = 0 To all.Count - 1
                Dim r As Double = 1.7
                If LjParams.ContainsKey(all(i).Element) Then
                    r = LjParams(all(i).Element)(1)
                End If
                radii(i) = r + ProbeRadius
            Next
            Dim grid As New CellGrid(all, 6.0)

            Dim total As Double = 0
            For i = 0 To all.Count - 1
                Dim a = all(i)
                Dim ri = radii(i)
                Dim accessible As Int32 = 0
                ' 采样点可及性
                For k = 0 To SasPoints - 1
                    Dim sx = a.X + pts(k, 0) * ri
                    Dim sy = a.Y + pts(k, 1) * ri
                    Dim sz = a.Z + pts(k, 2) * ri
                    Dim blocked = False
                    grid.ForNeighbors(sx, sy, sz, 4.5,
                        Sub(b As Atom)
                            If blocked OrElse b Is a Then Exit Sub
                            Dim dx = sx - b.X
                            Dim dy = sy - b.Y
                            Dim dz = sz - b.Z
                            ' 遮挡半径 = 邻原子 LJ R* + probe
                            Dim rr = b.LjRmin + ProbeRadius
                            If dx * dx + dy * dy + dz * dz < rr * rr Then blocked = True
                        End Sub)
                    If Not blocked Then accessible += 1
                Next
                Dim areaPerPoint = 4.0 * Math.PI * ri * ri / SasPoints
                total += accessible * areaPerPoint
            Next
            Return total
        End Function

        ''' <summary>
        ''' Nwat 水选择：按 水 O 到配体重原子最小距离 升序取前 nwat 个。
        ''' waters：全部水 O 原子；返回入选索引。
        ''' </summary>
        Public Function SelectNwat(waters As List(Of Atom), ligAtoms As List(Of Atom), nwat As Int32) As List(Of Int32)
            Dim ranked As New List(Of Tuple(Of Double, Int32))()
            For w = 0 To waters.Count - 1
                Dim best = Double.PositiveInfinity
                For Each la In ligAtoms
                    Dim dx = waters(w).X - la.X
                    Dim dy = waters(w).Y - la.Y
                    Dim dz = waters(w).Z - la.Z
                    Dim d = Math.Sqrt(dx * dx + dy * dy + dz * dz)
                    If d < best Then best = d
                Next
                ranked.Add(Tuple.Create(best, w))
            Next
            ranked.Sort(Function(a, b) a.Item1.CompareTo(b.Item1))
            Dim outList As New List(Of Int32)()
            For k = 0 To Math.Min(nwat, ranked.Count) - 1
                outList.Add(ranked(k).Item2)
            Next
            Return outList
        End Function

    End Module

End Namespace
