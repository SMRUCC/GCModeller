' ============================================================================
' MolBuilder.vb — 原子类型分配 / 环感知 / 可旋转键 / 扭转树 / 残基模板
' ----------------------------------------------------------------------------
' [README §1.1] 每个原子按 X-Score 方案分配原子类型 t_i。
' Vina 类型（重原子）：C(脂肪C) A(芳香C) N(非受体N) NA(受体N) OA(受体O)
'                     S SA P F Cl Br I Metal
' 简化规则（文档化）：
'   O → OA（全部视为受体）；S → SA；N → NA，但酰胺 N / 四价 N / 芳环 NH → N；
'   芳香环上 C → A；金属离子 → Metal（只有 gauss+排斥项参与）。
' 给体/受体判定（重原子合并氢模型）：给体 = {N,NA,OA,SA}，受体 = {OA,NA,SA}。
'
' 可旋转键 [Vina 规则]：单键（order=1）、两端均不在环上、非酰胺 C-N、
'   且旋转有意义（键两侧各有 ≥1 个其余重原子，即非端基键）。
'
' 扭转树：以原子 0 为根，每条可旋转键的 branch = 从 b 侧可达的全部原子
'   （不经 a）。应用顺序父先子后；梯度用最终几何（共轭律，见 Python 预验证）。
'
' 残基模板：20 标准氨基酸的原子→类型 与 连接表（供蛋白类型分配与 PEOE）。
' ============================================================================

Namespace Core

    ''' <summary>Vina 原子类型（重原子）</summary>
    Public Module VinaAtomTypes

        Public Const TC As Int32 = 0
        Public Const TA As Int32 = 1
        Public Const TN As Int32 = 2
        Public Const TNA As Int32 = 3
        Public Const TOA As Int32 = 4
        Public Const TS As Int32 = 5
        Public Const TSA As Int32 = 6
        Public Const TP As Int32 = 7
        Public Const TF As Int32 = 8
        Public Const TCl As Int32 = 9
        Public Const TBr As Int32 = 10
        Public Const TI As Int32 = 11
        Public Const TMetal As Int32 = 12
        Public Const TCount As Int32 = 13

        ''' <summary>原子半径（Å）——Vina 表值</summary>
        Public ReadOnly Radii() As Double = {
            1.9, 1.9, 1.8, 1.8, 1.7, 2.0, 2.0, 2.1, 1.5, 1.9, 2.1, 2.2, 1.5}

        Public ReadOnly Hydrophobic() As Boolean = {
            True, True, False, False, False, False, False, False, True, True, True, True, False}

        Public ReadOnly Acceptor() As Boolean = {
            False, False, False, True, True, False, True, False, False, False, False, False, False}

        Public ReadOnly Donor() As Boolean = {
            False, False, True, True, True, False, True, False, False, False, False, False, False}

        Public Function TypeName(t As Int32) As String
            Select Case t
                Case TC : Return "C"
                Case TA : Return "A"
                Case TN : Return "N"
                Case TNA : Return "NA"
                Case TOA : Return "OA"
                Case TS : Return "S"
                Case TSA : Return "SA"
                Case TP : Return "P"
                Case TF : Return "F"
                Case TCl : Return "Cl"
                Case TBr : Return "Br"
                Case TI : Return "I"
                Case Else : Return "Metal"
            End Select
        End Function

        Public Function RadiusOf(t As Int32) As Double
            Return Radii(t)
        End Function

    End Module

    Public Module MolBuilder

        ' ---------------- 环感知（剥度法）----------------

        ''' <summary>返回是否处于环上的原子集合（反复剥离去度=1 原子）</summary>
        Public Function RingAtoms(nAtoms As Int32, bonds As List(Of Bond)) As HashSet(Of Int32)
            Dim deg(nAtoms - 1) As Int32
            Dim adj(nAtoms - 1) As List(Of Int32)
            For i = 0 To nAtoms - 1
                adj(i) = New List(Of Int32)()
            Next
            For Each b In bonds
                deg(b.A) += 1 : deg(b.B) += 1
                adj(b.A).Add(b.B) : adj(b.B).Add(b.A)
            Next
            Dim alive(nAtoms - 1) As Boolean
            For i = 0 To nAtoms - 1
                alive(i) = True
            Next
            Dim queue As New Queue(Of Int32)()
            For i = 0 To nAtoms - 1
                If deg(i) <= 1 Then queue.Enqueue(i)
            Next
            While queue.Count > 0
                Dim u = queue.Dequeue()
                If Not alive(u) Then Continue While
                alive(u) = False
                For Each v In adj(u)
                    If alive(v) Then
                        deg(v) -= 1
                        If deg(v) = 1 Then queue.Enqueue(v)
                    End If
                Next
            End While
            Dim rings As New HashSet(Of Int32)()
            For i = 0 To nAtoms - 1
                If alive(i) Then rings.Add(i)
            Next
            Return rings
        End Function

        ''' <summary>环尺寸（原子所在最小环；简单近似：环原子所属连通块大小）</summary>
        Public Function RingSizes(nAtoms As Int32, bonds As List(Of Bond), ringAtoms As HashSet(Of Int32)) As Dictionary(Of Int32, Int32)
            ' 并查集求环原子连通块
            Dim parent(nAtoms - 1) As Int32
            For i = 0 To nAtoms - 1
                parent(i) = i
            Next
            For Each b In bonds
                If ringAtoms.Contains(b.A) AndAlso ringAtoms.Contains(b.B) Then
                    Union(parent, b.A, b.B)
                End If
            Next
            Dim sizeMap As New Dictionary(Of Int32, Int32)()
            For i = 0 To nAtoms - 1
                If ringAtoms.Contains(i) Then
                    Dim r = Find(parent, i)
                    If sizeMap.ContainsKey(r) Then sizeMap(r) += 1 Else sizeMap(r) = 1
                End If
            Next
            Dim result As New Dictionary(Of Int32, Int32)()
            For i = 0 To nAtoms - 1
                If ringAtoms.Contains(i) Then
                    result(i) = sizeMap(Find(parent, i))
                End If
            Next
            Return result
        End Function

        Private Function Find(parent() As Int32, x As Int32) As Int32
            While parent(x) <> x
                parent(x) = parent(parent(x))
                x = parent(x)
            End While
            Return x
        End Function

        Private Sub Union(parent() As Int32, a As Int32, b As Int32)
            Dim ra = Find(parent, a)
            Dim rb = Find(parent, b)
            If ra <> rb Then parent(ra) = rb
        End Sub

        ' ---------------- 原子类型分配 ----------------

        ''' <summary>判断酰胺 N：N 的邻居中有 C 且该 C 双键连 O</summary>
        Private Function IsAmideNitrogen(idx As Int32, atoms As List(Of Atom), bonds As List(Of Bond)) As Boolean
            Dim neighbors As New List(Of Int32)()
            For Each b In bonds
                If b.A = idx Then neighbors.Add(b.B)
                If b.B = idx Then neighbors.Add(b.A)
            Next
            For Each nb In neighbors
                If atoms(nb).Element = "C" Then
                    For Each b2 In bonds
                        If (b2.A = nb AndAlso atoms(b2.B).Element = "O" AndAlso b2.B <> idx AndAlso b2.Order >= 2.0) OrElse
                           (b2.B = nb AndAlso atoms(b2.A).Element = "O" AndAlso b2.A <> idx AndAlso b2.Order >= 2.0) Then
                            Return True
                        End If
                    Next
                End If
            Next
            Return False
        End Function

        ''' <summary>为 SDF 小分子分配 Vina 类型</summary>
        Public Sub AssignTypesSdf(mol As Molecule)
            Dim ringAtoms = ringAtoms(mol.Atoms.Count, mol.Bonds)
            Dim ringSize = RingSizes(mol.Atoms.Count, mol.Bonds, ringAtoms)

            For i = 0 To mol.Atoms.Count - 1
                Dim a = mol.Atoms(i)
                Select Case a.Element
                    Case "C"
                        Dim aromatic = ringAtoms.Contains(i) AndAlso
                                       ringSize.ContainsKey(i) AndAlso
                                       (ringSize(i) = 5 OrElse ringSize(i) = 6) AndAlso
                                       IsAromaticRingAtom(mol, i, ringAtoms)
                        a.VinaType = If(aromatic, VinaAtomTypes.TA, VinaAtomTypes.TC)
                    Case "N"
                        Dim deg = DegreeOf(i, mol.Bonds)
                        If IsAmideNitrogen(i, mol.Atoms, mol.Bonds) OrElse deg >= 4 OrElse a.Charge > 0 Then
                            a.VinaType = VinaAtomTypes.TN
                        Else
                            a.VinaType = VinaAtomTypes.TNA
                        End If
                    Case "O"
                        a.VinaType = VinaAtomTypes.TOA
                    Case "S"
                        a.VinaType = VinaAtomTypes.TSA
                    Case "P"
                        a.VinaType = VinaAtomTypes.TP
                    Case "F"
                        a.VinaType = VinaAtomTypes.TF
                    Case "Cl"
                        a.VinaType = VinaAtomTypes.TCl
                    Case "Br"
                        a.VinaType = VinaAtomTypes.TBr
                    Case "I"
                        a.VinaType = VinaAtomTypes.TI
                    Case "MG", "CA", "FE", "ZN", "MN", "CU", "NI", "CO"
                        a.VinaType = VinaAtomTypes.TMetal
                    Case Else
                        a.VinaType = VinaAtomTypes.TC
                End Select
            Next
        End Sub

        Private Function IsAromaticRingAtom(mol As Molecule, idx As Int32, ringAtoms As HashSet(Of Int32)) As Boolean
            ' SDF order=4 或 1.5 → 显式芳香；否则 5/6 元环内 C/N 混合且键级交替近似判定
            Dim orders As New List(Of Double)()
            For Each b In mol.Bonds
                If (b.A = idx OrElse b.B = idx) AndAlso ringAtoms.Contains(b.A) AndAlso ringAtoms.Contains(b.B) Then
                    orders.Add(b.Order)
                End If
            Next
            If orders.Count = 0 Then Return False
            ' 环内键若为 1.5/4 → 芳香
            For Each o In orders
                If o = 1.5 Then Return True
            Next
            ' 交替单双键（Kekulé）判定：环内该原子恰有 1 条双键
            Dim dbl As Int32 = 0
            For Each o In orders
                If o >= 2.0 Then dbl += 1
            Next
            Return dbl = 1
        End Function

        Private Function DegreeOf(idx As Int32, bonds As List(Of Bond)) As Int32
            Dim d As Int32 = 0
            For Each b In bonds
                If b.A = idx OrElse b.B = idx Then d += 1
            Next
            Return d
        End Function

        ''' <summary>为 PDB 蛋白/配体分配类型（残基模板 + 通用规则）</summary>
        Public Sub AssignTypesPdb(mol As Molecule)
            Dim hetIdx As New List(Of Int32)()
            For i = 0 To mol.Atoms.Count - 1
                Dim a = mol.Atoms(i)
                If a.IsWater Then
                    a.VinaType = VinaAtomTypes.TOA    ' 水 O 作为受体
                    Continue For
                End If
                Dim t As Int32
                If ResidueTemplates.TryGetType(a.ResName, a.AtomName, a.Element, t) Then
                    a.VinaType = t
                Else
                    hetIdx.Add(i)
                End If
            Next
            ' 模板未覆盖的残基（HETATM 配体等）：距离感知成键 + SDF 规则
            If hetIdx.Count > 0 Then
                Dim [sub] As New Molecule()
                Dim map As New Dictionary(Of Int32, Int32)()
                For Each i In hetIdx
                    map(i) = [sub].Atoms.Count
                    [sub].Atoms.Add(mol.Atoms(i))
                Next
                PerceiveBonds([sub])
                AssignTypesSdf([sub])
                For Each kv In map
                    mol.Atoms(kv.Key).VinaType = [sub].Atoms(kv.Value).VinaType
                Next
            End If
        End Sub

        ' ---------------- 可旋转键与扭转树 ----------------

        ''' <summary>
        ''' 找可旋转键并构建扭转树。返回（键列表，branch 列表）。
        ''' 判据：单键、两端不在环上、非酰胺 C-N、非端基键。
        ''' </summary>
        Public Function BuildTorsionTree(mol As Molecule) As Tuple(Of List(Of Bond), List(Of List(Of Int32)))
            Dim n = mol.Atoms.Count
            Dim results As New List(Of Bond)()
            If n < 4 Then Return Tuple.Create(results, New List(Of List(Of Int32))())

            Dim ringAtoms = ringAtoms(n, mol.Bonds)
            Dim adj(n - 1) As List(Of Int32)
            For i = 0 To n - 1
                adj(i) = New List(Of Int32)()
            Next
            For Each b In mol.Bonds
                adj(b.A).Add(b.B)
                adj(b.B).Add(b.A)
            Next

            ' 端基判定：移除键 (a,b) 后 b 侧（不含 a）无重原子 → 端基
            For Each b In mol.Bonds
                If b.Order <> 1.0 Then Continue For
                If ringAtoms.Contains(b.A) OrElse ringAtoms.Contains(b.B) Then Continue For
                ' 酰胺 C-N：N 的邻居中有 C=O
                If IsAmideNitrogen(b.B, mol.Atoms, mol.Bonds) AndAlso mol.Atoms(b.A).Element = "C" Then Continue For
                If IsAmideNitrogen(b.A, mol.Atoms, mol.Bonds) AndAlso mol.Atoms(b.B).Element = "C" Then Continue For

                ' 连通性划分
                Dim sideB = Reachable(b.B, b.A, adj)
                ' 端基键判据：移动侧（b 侧）除轴端点 b 自身外无其他原子
                ' （b 在旋转轴上，转动它为空操作 DOF，如 C-OH 的 C-O 键）
                ' 注意 sideA ≤1（如乙醇 C0-C1 的甲基端）不代表空操作——b 侧仍在移动
                If sideB.Count <= 1 Then Continue For
                results.Add(New Bond(b.A, b.B, 1.0))
            Next

            ' branch：从 b 侧可达（根 = 原子 0 所在一侧不转）
            Dim branches As New List(Of List(Of Int32))()
            For Each b In results
                Dim branch = Reachable(b.B, b.A, adj)
                If branch.Contains(0) Then
                    ' 根在 b 侧 → 改用 a 侧
                    Dim branch2 = Reachable(b.A, b.B, adj)
                    branches.Add(branch2)
                    ' 交换键方向保持 (axisRoot, axisEnd) 语义：axisRoot = 不动侧
                    results(results.IndexOf(b)) = New Bond(b.B, b.A, 1.0)
                Else
                    branches.Add(branch)
                End If
            Next
            Return Tuple.Create(results, branches)
        End Function

        Private Function Reachable(start As Int32, blocked As Int32, adj() As List(Of Int32)) As List(Of Int32)
            Dim seen As New HashSet(Of Int32) From {start, blocked}
            Dim stack As New Stack(Of Int32)()
            stack.Push(start)
            Dim outList As New List(Of Int32)()
            While stack.Count > 0
                Dim u = stack.Pop()
                outList.Add(u)
                For Each v In adj(u)
                    If Not seen.Contains(v) Then
                        seen.Add(v)
                        stack.Push(v)
                    End If
                Next
            End While
            ' blocked 不在 branch 中
            outList.Remove(blocked)
            Return outList
        End Function

    End Module

End Namespace
