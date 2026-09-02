' ============================================================================
' DockEngine.vb — 对接引擎编排
' ----------------------------------------------------------------------------
' [README §1] 全流程：结构读取 → 类型/电荷/键 → 刚性受体网格 → ILS+BFGS 搜索
'   → 姿态去重 → （可选）MM-GBSA/Nwat-MMGBSA 重打分 → JSON。
'
' 两种模式：
'   ligand           配体 = 小分子（SDF，可扭转）；受体 = 蛋白（PDB）
'   protein-protein  配体 = 第二条蛋白（PDB，刚体 6 DOF，无扭转）
' ============================================================================

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports MiniDock.Model

Namespace MiniDock.Core

    ''' <summary>IPoseObjective 适配器：绑定配体/受体/扭转树</summary>
    Public Class DockObjective
        Implements IPoseObjective

        Private ReadOnly _baseCoords(,) As Double        ' 配体初始坐标 (n,3)
        Private ReadOnly _ligAtoms As List(Of Atom)      ' 配体原子（类型/半径在 atom 上）
        Private ReadOnly _scorer As VinaScorer
        Private ReadOnly _axes() As Int32
        Private ReadOnly _branches As List(Of List(Of Int32))
        Private ReadOnly _intraI() As Int32
        Private ReadOnly _intraJ() As Int32
        Private ReadOnly _workPos() As Double
        Private ReadOnly _rigidCenter(2) As Double

        Public Sub New(baseCoords(,) As Double, ligAtoms As List(Of Atom), scorer As VinaScorer,
                       axes() As Int32, branches As List(Of List(Of Int32)),
                       intraI() As Int32, intraJ() As Int32)
            _baseCoords = baseCoords
            _ligAtoms = ligAtoms
            _scorer = scorer
            _axes = axes
            _branches = branches
            _intraI = intraI
            _intraJ = intraJ
            _workPos = New Double(3 * ligAtoms.Count - 1) {}
        End Sub

        Public ReadOnly Property NumTorsions As Int32 Implements IPoseObjective.NumTorsions
            Get
                Return _branches.Count
            End Get
        End Property

        Public Function Evaluate(trans() As Double, rotvec() As Double, torsions() As Double,
                                 grads() As Double, rigidCenter() As Double) As Double Implements IPoseObjective.Evaluate
            PoseOps.ApplyPose(_baseCoords, trans, rotvec, _axes, _branches, torsions,
                              _workPos, rigidCenter(0), rigidCenter(1), rigidCenter(2))
            For i = 0 To _ligAtoms.Count - 1
                _ligAtoms(i).X = _workPos(3 * i)
                _ligAtoms(i).Y = _workPos(3 * i + 1)
                _ligAtoms(i).Z = _workPos(3 * i + 2)
            Next
            Return _scorer.Evaluate(_ligAtoms, rigidCenter, _intraI, _intraJ, _axes, _branches, grads)
        End Function

        ''' <summary>物化当前姿态坐标（x,y,z 平铺）</summary>
        Public Function MaterializeCoords(trans() As Double, rotvec() As Double, torsions() As Double) As Double()
            Dim rc(2) As Double
            PoseOps.ApplyPose(_baseCoords, trans, rotvec, _axes, _branches, torsions,
                              _workPos, rc(0), rc(1), rc(2))
            Return CType(_workPos.Clone(), Double())
        End Function

    End Class

    Public Class DockEngine

        ''' <summary>
        ''' 执行对接。receptorMol / ligandMol 已完成类型与电荷分配。
        ''' </summary>
        Public Shared Function Dock(receptorMol As Molecule, ligandMol As Molecule,
                                    opts As DockOptions) As LigandResult
            ' 1. 内部 1-4 以上原子对（配体柔性时）
            Dim intraI() As Int32 = Nothing
            Dim intraJ() As Int32 = Nothing
            If ligandMol.Bonds.Count > 0 Then
                BuildIntraPairs(ligandMol, intraI, intraJ)
            Else
                ' 刚体大分子：内部项恒定，不参与优化
                intraI = New Int32(-1) {}
                intraJ = New Int32(-1) {}
            End If

            Dim branches As List(Of List(Of Int32)) = Nothing
            Dim axes() As Int32 = Nothing
            If ligandMol.Bonds.Count > 0 Then
                Dim tt = MolBuilder.BuildTorsionTree(ligandMol)
                branches = tt.Item2
                axes = New Int32(2 * tt.Item1.Count - 1) {}
                For t = 0 To tt.Item1.Count - 1
                    axes(2 * t) = tt.Item1(t).A
                    axes(2 * t + 1) = tt.Item1(t).B
                Next
            Else
                branches = New List(Of List(Of Int32))()
                axes = New Int32(-1) {}
            End If

            ' 2. 基础坐标矩阵
            Dim n = ligandMol.Atoms.Count
            Dim baseCoords(n - 1, 2) As Double
            For i = 0 To n - 1
                baseCoords(i, 0) = ligandMol.Atoms(i).X
                baseCoords(i, 1) = ligandMol.Atoms(i).Y
                baseCoords(i, 2) = ligandMol.Atoms(i).Z
            Next

            ' 3. 打分器 + 目标函数
            Dim scorer As New VinaScorer(receptorMol.Atoms)
            Dim objective As New DockObjective(baseCoords, ligandMol.Atoms, scorer,
                                               axes, branches, intraI, intraJ)

            ' 4. 盒中心：默认受体形心
            Dim bc(2) As Double
            If opts.BoxCenter IsNot Nothing Then
                bc = opts.BoxCenter
            Else
                For Each a In receptorMol.Atoms
                    If Not a.IsWater Then
                        bc(0) += a.X : bc(1) += a.Y : bc(2) += a.Z
                    End If
                Next
                Dim cnt = receptorMol.Atoms.Count
                If cnt > 0 Then
                    bc(0) /= cnt : bc(1) /= cnt : bc(2) /= cnt
                End If
            End If
            Dim boxHalf = If(opts.BoxHalfSize > 0, opts.BoxHalfSize, 12.0)

            ' 5. ILS 搜索 [README §1.2]
            Dim searcher As New IlsSearcher(opts.Seed) With {
                .NumRuns = opts.Exhaustiveness,
                .StepsPerRun = opts.StepsPerRun}
            Dim poses = searcher.Search(objective, bc, boxHalf, opts.NumModes, opts.MinRmsd,
                Function(r As IlsSearcher.IlarResult)
                    Return objective.MaterializeCoords(r.Trans, r.Rotvec, r.Torsions)
                End Function)

            ' 6. 组装结果
            Dim lr As New LigandResult With {
                .Id = ligandMol.Id,
                .NumAtoms = n,
                .NumRotatableBonds = branches.Count}
            Dim poseList As New List(Of Pose)()
            Dim rank = 1
            For Each pr In poses
                ' 重算分量：inter / intra
                Dim trans = pr.Trans
                Dim rotvec = pr.Rotvec
                Dim torsions = pr.Torsions
                Dim grads(6 + branches.Count - 1) As Double
                Dim rc(2) As Double
                Dim inter = objective.Evaluate(trans, rotvec, torsions, grads, rc)
                Dim affinity = inter + VinaScorer.RotatableWeight * branches.Count

                Dim pose As New Pose With {
                    .Rank = rank,
                    .VinaScore = Math.Round(affinity, 3),
                    .Intermolecular = Math.Round(inter, 3),
                    .Intramolecular = 0.0,
                    .NumTorsions = branches.Count}

                ' 坐标
                Dim coords = objective.MaterializeCoords(trans, rotvec, torsions)
                Dim atomList As New List(Of PoseAtom)()
                For i = 0 To n - 1
                    Dim src = ligandMol.Atoms(i)
                    atomList.Add(New PoseAtom With {
                        .Element = src.Element,
                        .X = Math.Round(coords(3 * i), 3),
                        .Y = Math.Round(coords(3 * i + 1), 3),
                        .Z = Math.Round(coords(3 * i + 2), 3),
                        .Chain = src.ChainId,
                        .ResName = src.ResName,
                        .ResSeq = src.ResSeq,
                        .AtomName = src.AtomName})
                Next
                pose.Atoms = atomList
                poseList.Add(pose)
                rank += 1
            Next
            lr.Poses = poseList
            Return lr
        End Function

        ''' <summary>构建配体内部 1-4 以上原子对索引（初始距离 &lt; 2×cutoff）</summary>
        Private Shared Sub BuildIntraPairs(mol As Molecule, ByRef intraI() As Int32, ByRef intraJ() As Int32)
            Dim n = mol.Atoms.Count
            Dim sep = BondSeparation(mol)
            Dim ii As New List(Of Int32)()
            Dim jj As New List(Of Int32)()
            For i = 0 To n - 1
                For j = i + 1 To n - 1
                    ' 1-2/1-3/1-4 排除：分离度 ≤3 键（集合只存 ≤3 键对，
                    ' 命中即排除；4 键分离允许进入（作为 1-4 边界）
                    Dim key = CLng(i) * 100000L + j
                    If sep.Contains(key) Then
                        ' 4 键分离的对允许进入，≤3 键排除：重新用集合不可区分，
                        ' 故构建集合时只放 ≤3 的对——此处直接排除
                        Continue For
                    End If
                    Dim dx = mol.Atoms(i).X - mol.Atoms(j).X
                    Dim dy = mol.Atoms(i).Y - mol.Atoms(j).Y
                    Dim dz = mol.Atoms(i).Z - mol.Atoms(j).Z
                    If dx * dx + dy * dy + dz * dz < 256.0 Then    ' 16 Å 以内才可能进入 8 Å 作用域
                        ii.Add(i) : jj.Add(j)
                    End If
                Next
            Next
            intraI = ii.ToArray()
            intraJ = jj.ToArray()
        End Sub

        ''' <summary>原子对键分离度（BFS 多源；忽略；返回 ≤4 的对）</summary>
        Private Shared Function BondSeparation(mol As Molecule) As HashSet(Of (Int32, Int32))
            Dim n = mol.Atoms.Count
            Dim adj(n - 1) As List(Of Int32)
            For i = 0 To n - 1
                adj(i) = New List(Of Int32)()
            Next
            For Each b In mol.Bonds
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
                Next
                For j = 0 To n - 1
                    If j <> start AndAlso dist(j) > 0 AndAlso dist(j) <= 3 Then
                        Dim lo = Math.Min(start, j)
                        Dim hi = Math.Max(start, j)
                        result.Add(CLng(lo) * 100000L + hi)
                    End If
                Next
            Next
            Return result
        End Function

        ''' <summary>
        ''' MM-GBSA 重打分（对接姿态或独立复合物帧）。
        ''' recAll：受体全部原子（含水）；ligAtoms：配体原子；nwat：保留水数。
        ''' </summary>
        Public Shared Function MmGbsaRescore(recAll As List(Of Atom), ligAtoms As List(Of Atom),
                                             nwat As Int32) As MmGbsaResult
            ' 水分组
            Dim waters As New List(Of Atom)()
            Dim recNoWater As New List(Of Atom)()
            For Each a In recAll
                If a.IsWater Then
                    waters.Add(a)
                Else
                    recNoWater.Add(a)
                End If
            Next

            Dim recSide As New List(Of Atom)()
            recSide.AddRange(recNoWater)
            Dim nSelected As Int32 = 0
            If nwat > 0 AndAlso waters.Count > 0 Then
                Dim sel = MmGbsa.SelectNwat(waters, ligAtoms, nwat)
                nSelected = sel.Count
                For Each idx In sel
                    recSide.Add(waters(idx))
                Next
            End If

            Dim r = MmGbsa.EvaluateSinglePoint(recSide, ligAtoms)
            r.NwatRequested = nwat
            r.NwatSelected = nSelected
            Return r
        End Function

    End Class

    Public Class DockOptions

        Public Exhaustiveness As Int32 = 8
        Public StepsPerRun As Int32 = 30
        Public NumModes As Int32 = 9
        Public MinRmsd As Double = 1.5
        Public BoxCenter() As Double = Nothing
        Public BoxHalfSize As Double = 12.0
        Public Seed As Int32 = 0
        Public Mmgbsa As Boolean = False
        Public Nwat As Int32 = 0
        Public MmgbsaTop As Int32 = 3

    End Class

End Namespace
