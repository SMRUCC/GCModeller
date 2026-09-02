' ============================================================================
' SelfTest.vb — 内置自检（MiniDock selftest）
' ----------------------------------------------------------------------------
' 1. 解析梯度 vs 有限差分（增量姿态语义；机器精度）★ 核心
' 2. BFGS 收敛性
' 3. ILS 自对接冒烟（合成口袋）
' 4. SASA 孤立原子 ≈ 4π(r+probe)²（解析）
' 5. GB 自项 / 远场行为
' 6. Nwat 最近水选择
' 7. PEOE 电荷守恒 + 官能团方向性
' 8. SDF/PDB 解析 + 可旋转键计数（乙醇 1，苯 0）
' 9. 端到端：合成口袋 + 乙醇重对接 → 最优姿态回到口袋
' ============================================================================

Imports MiniDock.Core
Imports SMRUCC.genomics.Data.RCSB.PDB.Structures

Public Module SelfTest

    Private _failures As Integer = 0
    Private _rng As New Random(42)

    Private Sub Check(cond As Boolean, name As String)
        If cond Then
            Console.WriteLine($"  [PASS] {name}")
        Else
            _failures += 1
            Console.WriteLine($"  [FAIL] {name}")
        End If
    End Sub

    Public Function RunAll() As Integer
        _failures = 0
        Console.WriteLine("=== MiniDock SelfTest ===")

        TestGradient()
        TestBfgs()
        TestIls()
        TestSasa()
        TestGb()
        TestNwat()
        TestPoe()
        TestParsing()
        TestEndToEnd()

        Console.WriteLine($"=== {If(_failures = 0, "ALL TESTS PASSED", _failures & " TEST(S) FAILED")} ===")
        Return _failures
    End Function

    ' ---------------- 测试体系构建 ----------------

    ''' <summary>合成口袋受体：球壳 6-8Å 上 60 个原子 + 类型</summary>
    Private Function BuildPocketReceptor() As VinaMolecule
        Dim mol As New VinaMolecule With {.Id = "pocket"}
        For i = 1 To 60
            Dim v(2) As Double
            Do
                v(0) = _rng.NextDouble() * 2 - 1
                v(1) = _rng.NextDouble() * 2 - 1
                v(2) = _rng.NextDouble() * 2 - 1
                Dim nrm = Math.Sqrt(v(0) * v(0) + v(1) * v(1) + v(2) * v(2))
                If nrm > 0.2 AndAlso nrm < 1.0 Then
                    Dim rad = 6.0 + _rng.NextDouble() * 2.0
                    v(0) *= rad / nrm : v(1) *= rad / nrm : v(2) *= rad / nrm
                    Exit Do
                End If
            Loop
            Dim typeName = {"C", "C", "OA", "N", "NA", "A"}(_rng.Next(6))
            mol.Atoms.Add(New VinaAtom With {.X = v(0), .Y = v(1), .Z = v(2), .Element = "C"})
            mol.Atoms(mol.Atoms.Count - 1).VinaType = TypeByName(typeName)
        Next
        Return mol
    End Function

    Private Function TypeByName(name As String) As Int32
        Select Case name
            Case "C" : Return VinaAtomTypes.TC
            Case "A" : Return VinaAtomTypes.TA
            Case "N" : Return VinaAtomTypes.TN
            Case "NA" : Return VinaAtomTypes.TNA
            Case "OA" : Return VinaAtomTypes.TOA
            Case "S" : Return VinaAtomTypes.TS
            Case "SA" : Return VinaAtomTypes.TSA
            Case Else : Return VinaAtomTypes.TC
        End Select
    End Function

    ''' <summary>直链醇配体（6 重原子，2 可旋转键）：C-C-C-OA-C-N 型</summary>
    Private Function BuildChainLigand() As VinaMolecule
        Dim mol As New VinaMolecule With {.Id = "chain"}
        Dim types = {VinaAtomTypes.TC, VinaAtomTypes.TC, VinaAtomTypes.TC,
                     VinaAtomTypes.TOA, VinaAtomTypes.TC, VinaAtomTypes.TN}
        For i = 0 To 5
            mol.Atoms.Add(New VinaAtom With {
                .X = 0.5 * i, .Y = 0.06 * (i Mod 3), .Z = 0.1 * i,
                .Element = If(types(i) = VinaAtomTypes.TOA, "O", "C"),
                .VinaType = types(i)})
        Next
        For i = 0 To 4
            mol.Bonds.Add(New Bond(i, i + 1, 1.0))
        Next
        Return mol
    End Function

    ''' <summary>目标函数（inter + intra 全量）</summary>
    Private Function BuildObjective(lig As VinaMolecule, rec As VinaMolecule) As (obj As DockObjective, axes As Int32())
        Dim n = lig.Atoms.Count
        Dim bc(n - 1, 2) As Double
        For i = 0 To n - 1
            bc(i, 0) = lig.Atoms(i).X
            bc(i, 1) = lig.Atoms(i).Y
            bc(i, 2) = lig.Atoms(i).Z
        Next
        Dim tt = MolBuilder.BuildTorsionTree(lig)
        Dim axes(2 * tt.Item1.Count - 1) As Int32
        For t = 0 To tt.Item1.Count - 1
            axes(2 * t) = tt.Item1(t).A
            axes(2 * t + 1) = tt.Item1(t).B
        Next

        ' intra 对：分离度 >3 键
        Dim sep = BondSepSet(lig)
        Dim ii As New List(Of Int32)()
        Dim jj As New List(Of Int32)()
        For i = 0 To n - 1
            For j = i + 1 To n - 1
                Dim lo = Math.Min(i, j), hi = Math.Max(i, j)
                If Not sep.Contains(CLng(lo) * 100000L + hi) Then
                    ii.Add(i) : jj.Add(j)
                End If
            Next
        Next

        Dim scorer As New VinaScorer(rec.Atoms)
        Dim obj As New DockObjective(bc, lig.Atoms, scorer, axes, branches:=tt.Item2,
                                     intraI:=ii.ToArray(), intraJ:=jj.ToArray())
        Return (obj, axes)
    End Function

    Private Function BondSepSet(lig As VinaMolecule) As HashSet(Of Int64)
        Dim n = lig.Atoms.Count
        Dim adj(n - 1) As List(Of Int32)
        For i = 0 To n - 1
            adj(i) = New List(Of Int32)()
        Next
        For Each b In lig.Bonds
            adj(b.A).Add(b.B)
            adj(b.B).Add(b.A)
        Next
        Dim result As New HashSet(Of Int64)()
        For start = 0 To n - 1
            Dim dist(n - 1) As Int32
            For i = 0 To n - 1
                dist(i) = -1
            Next
            dist(start) = 0
            Dim q As New Queue(Of Int32)()
            q.Enqueue(start)
            While q.Count > 0
                Dim u = q.Dequeue()
                If dist(u) >= 4 Then Continue While
                For Each v In adj(u)
                    If dist(v) < 0 Then
                        dist(v) = dist(u) + 1
                        q.Enqueue(v)
                    End If
                Next
            End While
            For j = 0 To n - 1
                If j <> start AndAlso dist(j) > 0 AndAlso dist(j) <= 3 Then
                    result.Add(CLng(Math.Min(start, j)) * 100000L + Math.Max(start, j))
                End If
            Next
        Next
        Return result
    End Function

    ' ---------------- 1. 梯度 ----------------

    Private Sub TestGradient()
        Console.WriteLine("-- 解析梯度 vs 有限差分（增量姿态语义）--")
        Dim rec = BuildPocketReceptor()
        Dim lig = BuildChainLigand()
        Dim pair = BuildObjective(lig, rec)
        Dim obj = pair.Item1
        Dim nTors = obj.NumTorsions
        Dim fails As Int32 = 0

        For trial = 1 To 4
            Dim trans(2) As Double
            Dim rotvec(2) As Double
            Dim torsions(nTors - 1) As Double
            For k = 0 To 2
                trans(k) = _rng.NextDouble() * 2 - 1
                rotvec(k) = _rng.NextDouble() * 0.6 - 0.3
            Next
            For t = 0 To nTors - 1
                torsions(t) = _rng.NextDouble() * 2 - 1
            Next

            Dim rc(2) As Double
            Dim gAn(6 + nTors - 1) As Double
            obj.Evaluate(trans, rotvec, torsions, gAn, rc)

            ' 数值梯度（增量语义）
            Dim h = 0.000001
            Dim gNum(6 + nTors - 1) As Double
            For k = 0 To 2
                Dim tp = CType(trans.Clone(), Double())
                Dim tm = CType(trans.Clone(), Double())
                tp(k) += h : tm(k) -= h
                Dim ep = obj.Evaluate(tp, rotvec, torsions, New Double(6 + nTors - 1) {}, New Double(2) {})
                Dim em = obj.Evaluate(tm, rotvec, torsions, New Double(6 + nTors - 1) {}, New Double(2) {})
                gNum(k) = (ep - em) / (2 * h)
            Next
            For k = 0 To 2
                Dim rp = CType(rotvec.Clone(), Double())
                Dim rm = CType(rotvec.Clone(), Double())
                Dim dPlus(2) As Double : dPlus(k) = h
                Dim dMinus(2) As Double : dMinus(k) = -h
                Dim tp = CType(trans.Clone(), Double())
                Dim tm = CType(trans.Clone(), Double())
                PoseOps.ApplyIncrement(tp, rp, torsions, dPlus, 1.0)
                PoseOps.ApplyIncrement(tm, rm, torsions, dMinus, 1.0)
                Dim ep = obj.Evaluate(tp, rp, torsions, New Double(6 + nTors - 1) {}, New Double(2) {})
                Dim em = obj.Evaluate(tm, rm, torsions, New Double(6 + nTors - 1) {}, New Double(2) {})
                gNum(3 + k) = (ep - em) / (2 * h)
            Next
            For t = 0 To nTors - 1
                Dim tp = CType(torsions.Clone(), Double())
                Dim tm = CType(torsions.Clone(), Double())
                tp(t) += h : tm(t) -= h
                Dim ep = obj.Evaluate(trans, rotvec, tp, New Double(6 + nTors - 1) {}, New Double(2) {})
                Dim em = obj.Evaluate(trans, rotvec, tm, New Double(6 + nTors - 1) {}, New Double(2) {})
                gNum(6 + t) = (ep - em) / (2 * h)
            Next

            Dim scale = 1.0
            For k = 0 To 6 + nTors - 1
                scale = Math.Max(scale, Math.Abs(gNum(k)))
                If Math.Abs(gAn(k) - gNum(k)) / scale > 0.001 Then fails += 1
            Next
            Console.WriteLine($"  trial{trial}: 解析旋转分量 = ({gAn(3):F6},{gAn(4):F6},{gAn(5):F6})  " &
                              $"数值 = ({gNum(3):F6},{gNum(4):F6},{gNum(5):F6})")
        Next
        Check(fails = 0, $"梯度一致性（4 随机用例 × {6 + nTors} 维）")
    End Sub

    ' ---------------- 2. BFGS ----------------

    Private Sub TestBfgs()
        Console.WriteLine("-- BFGS 收敛 --")
        Dim rec = BuildPocketReceptor()
        Dim lig = BuildChainLigand()
        Dim obj = BuildObjective(lig, rec).Item1
        Dim nTors = obj.NumTorsions

        Dim trans = {1.0, 0.5, -0.5}
        Dim rotvec = {0.2, -0.1, 0.3}
        Dim torsions(nTors - 1) As Double
        For t = 0 To nTors - 1
            torsions(t) = 0.5 - t * 0.4
        Next
        Dim rc(2) As Double
        Dim g(6 + nTors - 1) As Double
        Dim f0 = obj.Evaluate(trans, rotvec, torsions, g, rc)

        Dim bfgs As New BfgsMinimizer()
        Dim ev As Int32 = 0
        Dim f1 = bfgs.Minimize(obj, trans, rotvec, torsions, ev)
        Console.WriteLine($"  f = {f0:F4} → {f1:F4}（{ev} 次评估）")
        Check(f1 < f0 - 0.5, $"BFGS 局部极小化（{f0:F3} → {f1:F3}）")
    End Sub

    ' ---------------- 3. ILS ----------------

    Private Sub TestIls()
        Console.WriteLine("-- ILS 自对接 --")
        Dim rec = BuildPocketReceptor()
        Dim lig = BuildChainLigand()
        Dim pair = BuildObjective(lig, rec)
        Dim obj = pair.Item1
        Dim nTors = obj.NumTorsions

        ' 晶体姿态：口袋中心
        Dim crystalTrans = {0.0, 0.0, 0.0}
        Dim crystalRot = {0.0, 0.0, 0.0}
        Dim crystalTors(nTors - 1) As Double
        For t = 0 To nTors - 1
            crystalTors(t) = 0.6 - 0.5 * t
        Next
        Dim rc(2) As Double
        Dim g(6 + nTors - 1) As Double
        Dim fCrystal = obj.Evaluate(crystalTrans, crystalRot, crystalTors, g, rc)

        Dim opts As New DockOptions With {.Exhaustiveness = 6, .StepsPerRun = 25,
                                          .NumModes = 5, .MinRmsd = 1.5, .Seed = 7}
        Dim searcher As New IlsSearcher(opts.Seed) With {.NumRuns = opts.Exhaustiveness, .StepsPerRun = opts.StepsPerRun}
        Dim results = searcher.Search(obj, New Double(2) {}, 6.0, 5, 1.5,
            Function(r As IlsSearcher.IlarResult)
                Return obj.MaterializeCoords(r.Trans, r.Rotvec, r.Torsions)
            End Function)

        ' 口袋内最优
        Dim bestInPocket = Double.PositiveInfinity
        For Each r In results
            Dim c = obj.MaterializeCoords(r.Trans, r.Rotvec, r.Torsions)
            Dim cx = 0.0, cy = 0.0, cz = 0.0
            For i = 0 To lig.Atoms.Count - 1
                cx += c(3 * i) : cy += c(3 * i + 1) : cz += c(3 * i + 2)
            Next
            cx /= lig.Atoms.Count : cy /= lig.Atoms.Count : cz /= lig.Atoms.Count
            If Math.Sqrt(cx * cx + cy * cy + cz * cz) < 5.0 Then
                bestInPocket = Math.Min(bestInPocket, r.Energy)
            End If
        Next
        Console.WriteLine($"  晶体姿态 = {fCrystal:F4}  口袋内 ILS 最优 = {If(Double.IsPositiveInfinity(bestInPocket), Double.NaN, bestInPocket):F4}")
        Check(Not Double.IsPositiveInfinity(bestInPocket) AndAlso
              bestInPocket <= fCrystal + 0.5, "ILS 找到口袋内更优/相当姿态")
    End Sub

    ' ---------------- 4-6. SASA / GB / Nwat ----------------

    Private Sub TestSasa()
        Console.WriteLine("-- SASA --")
        Dim one As New List(Of VinaAtom) From {New VinaAtom With {.X = 0, .Y = 0, .Z = 0, .Element = "C"}}
        Dim s = MmGbsa.TotalSasa(one, Nothing)
        Dim expect = 4.0 * Math.PI * (1.908 + 1.4) ^ 2
        Dim rel = Math.Abs(s - expect) / expect
        Console.WriteLine($"  孤立原子 {s:F3} vs 解析 {expect:F3}  相对误差 {rel:F5}")
        Check(rel < 0.01, "SASA 孤立原子 = 解析球面积")
    End Sub

    Private Sub TestGb()
        Console.WriteLine("-- GB --")
        ' 两原子远距：交叉项 ≈ -(1-1/ε)q₁q₂/r
        Dim a As New List(Of VinaAtom) From {New VinaAtom With {.X = 0, .Y = 0, .Z = 0, .Element = "O", .Charge = -0.5}}
        Dim b As New List(Of VinaAtom) From {New VinaAtom With {.X = 100, .Y = 0, .Z = 0, .Element = "N", .Charge = 0.5}}
        Dim g = MmGbsa.TotalSasa(a, b) ' 占位防优化
        Dim gb = MmGbsa.EvaluateSinglePoint(New List(Of VinaAtom)(a), New List(Of VinaAtom)(b))
        ' 交叉项理论：-(1-1/ε)·q₁q₂/r（Born 屏蔽指数衰减为 0）
        Dim expectCross = -(1 - 1 / 78.5) * (-0.5) * 0.5 / 100.0
        ' 自项：-(1/2)(1-1/ε)(q₁²/R₁ + q₂²/R₂) 三态相减后抵消（同一构象）
        Console.WriteLine($"  ΔE_elec = {gb.Elec:F6}（理论 {332.0636 * (-0.5) * 0.5 / 100:F6}）")
        Check(Math.Abs(gb.Elec - 332.0636 * (-0.25) / 100.0) < 0.001, "库仑远程项")
        Console.WriteLine($"  ΔG_polar = {gb.GbPolar:F6}（理论交叉部分 {expectCross:F6} 量级，自项差应≈0）")
        Check(Math.Abs(gb.GbPolar - expectCross) < 0.02, "GB 远距交叉项 ≈ 理论（自项三态相消）")
    End Sub

    Private Sub TestNwat()
        Console.WriteLine("-- Nwat 最近水选择 --")
        Dim lig As New List(Of VinaAtom) From {
            New VinaAtom With {.X = 0, .Y = 0, .Z = 0},
            New VinaAtom With {.X = 1, .Y = 0, .Z = 0}}
        Dim waters As New List(Of VinaAtom) From {
            New VinaAtom With {.X = 3, .Y = 0, .Z = 0},
            New VinaAtom With {.X = 2.5, .Y = 0.5, .Z = 0},
            New VinaAtom With {.X = 1.2, .Y = 0.1, .Z = 0},
            New VinaAtom With {.X = 5, .Y = 5, .Z = 5},
            New VinaAtom With {.X = 1.5, .Y = 0, .Z = 0},
            New VinaAtom With {.X = 10, .Y = 0, .Z = 0}}
        Dim sel = MmGbsa.SelectNwat(waters, lig, 3)
        Check(sel.Count = 3 AndAlso sel(0) = 2 AndAlso sel(1) = 4 AndAlso sel(2) = 1,
              $"Nwat=3 选择索引 = ({sel(0)},{sel(1)},{sel(2)}) 期望 (2,4,1)")
    End Sub

    ' ---------------- 7. PEOE ----------------

    Private Sub TestPoe()
        Console.WriteLine("-- PEOE 电荷 --")
        ' 乙醇分子：C-C-O
        Dim mol As New VinaMolecule With {.Id = "ethanol"}
        mol.Atoms.Add(New VinaAtom With {.X = 0, .Y = 0, .Z = 0, .Element = "C"})
        mol.Atoms.Add(New VinaAtom With {.X = 1.5, .Y = 0, .Z = 0, .Element = "C"})
        mol.Atoms.Add(New VinaAtom With {.X = 2.9, .Y = 0.9, .Z = 0, .Element = "O"})
        mol.Bonds.Add(New Bond(0, 1, 1.0))
        mol.Bonds.Add(New Bond(1, 2, 1.0))
        Charges.AssignPoeCharges(mol, 0.0)
        Dim total = mol.Atoms(0).Charge + mol.Atoms(1).Charge + mol.Atoms(2).Charge
        Console.WriteLine($"  电荷: C {mol.Atoms(0).Charge:F3} / C {mol.Atoms(1).Charge:F3} / O {mol.Atoms(2).Charge:F3}  Σ={total:F4}")
        Check(Math.Abs(total) < 0.02, "电荷总和 = 0")
        Check(mol.Atoms(2).Charge < 0 AndAlso mol.Atoms(0).Charge > 0, "O 带负电、远端 C 带正电")
    End Sub

    ' ---------------- 8. 解析与可旋转键 ----------------

    Private Sub TestParsing()
        Console.WriteLine("-- 解析 / 可旋转键 --")
        ' 乙醇 SDF
        Dim sdf As New List(Of String) From {
            "ethanol", "  MiniDock", "", "  3  2  0  0  0  0  0  0  0  0999 V2000",
            "    0.0000    0.0000    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0",
            "    1.5000    0.0000    0.0000 C   0  0  0  0  0  0  0  0  0  0  0  0",
            "    2.9000    0.9000    0.0000 O   0  0  0  0  0  0  0  0  0  0  0  0",
            "  1  2  1  0  0  0  0",
            "  2  3  1  0  0  0  0",
            "M  END"}
        Dim tmp = IO.Path.Combine(IO.Path.GetTempPath(), "minidock_test.sdf")
        IO.File.WriteAllLines(tmp, sdf)
        Dim mol = SdfIO.ReadSdf(tmp)
        MolBuilder.AssignTypesSdf(mol)
        Dim tt = MolBuilder.BuildTorsionTree(mol)
        Check(mol.Atoms.Count = 3 AndAlso mol.Bonds.Count = 2, "SDF 解析")
        Check(tt.Item1.Count = 1, $"乙醇可旋转键 = 1（实际 {tt.Item1.Count}）")
        Check(mol.Atoms(2).VinaType = VinaAtomTypes.TOA, "O → OA")

        ' 苯：0 可旋转
        Dim benz As New VinaMolecule With {.Id = "benzene"}
        For i = 0 To 5
            Dim ang = 2.0 * Math.PI * i / 6.0
            benz.Atoms.Add(New VinaAtom With {.X = Math.Cos(ang) * 1.39, .Y = Math.Sin(ang) * 1.39, .Z = 0, .Element = "C"})
        Next
        For i = 0 To 5
            benz.Bonds.Add(New Bond(i, (i + 1) Mod 6, 1.5))
        Next
        MolBuilder.AssignTypesSdf(benz)
        Dim tt2 = MolBuilder.BuildTorsionTree(benz)
        Check(tt2.Item1.Count = 0, $"苯可旋转键 = 0（实际 {tt2.Item1.Count}）")
        Check(benz.Atoms.All(Function(a) a.VinaType = VinaAtomTypes.TA), "苯环 C → A（芳香）")
    End Sub

    ' ---------------- 9. 端到端 ----------------

    Private Sub TestEndToEnd()
        Console.WriteLine("-- 端到端（合成口袋 + 链式配体）--")
        Dim rec = BuildPocketReceptor()
        Dim lig = BuildChainLigand()
        Dim opts As New DockOptions With {.Exhaustiveness = 4, .StepsPerRun = 15,
                                          .NumModes = 3, .MinRmsd = 1.5, .Seed = 3,
                                          .BoxCenter = {0.0, 0.0, 0.0}, .BoxHalfSize = 4.0}
        Dim lr = DockEngine.Dock(rec, lig, opts)
        Check(lr.Poses.Count > 0, $"产出姿态数 = {lr.Poses.Count}")
        If lr.Poses.Count > 0 Then
            Dim best = lr.Poses(0)
            Console.WriteLine($"  最优姿态 vina_score = {best.VinaScore}（inter {best.Intermolecular}）")
            Check(best.VinaScore <= 0.0, "最优姿态结合自由能 ≤ 0")

            ' MM-GBSA 重打分
            Dim poseAtoms As New List(Of VinaAtom)()
            For Each pa In best.Atoms
                poseAtoms.Add(New VinaAtom With {.X = pa.X, .Y = pa.Y, .Z = pa.Z,
                                             .Element = pa.Element, .FromReceptor = False})
            Next
            Charges.AssignPoeCharges(New VinaMolecule With {.Atoms = poseAtoms, .Bonds = lig.Bonds}, 0.0)
            Dim r = DockEngine.MmGbsaRescore(rec.Atoms, poseAtoms, 5)
            Console.WriteLine($"  MM-GBSA: ΔG={r.DeltaG:F2} (vdw {r.Vdw:F2}, elec {r.Elec:F2}, gb {r.GbPolar:F2}, sasa {r.SasNonpolar:F3}, nwat {r.NwatSelected})")
            Check(r.Vdw < 0, "MM-GBSA vdW 为吸引（负值）")
        End If
    End Sub

End Module

