' ============================================================================
' Charges.vb — Gasteiger PEOE 部分电荷（从头实现）
' ----------------------------------------------------------------------------
' [README §2.2 说明] AM1-BCC/RESP 需要半经验量子计算，超出零依赖范围；
' 采用 Gasteiger-Marsili PEOE（轨道电负性均衡，迭代部分电荷转移）替代：
'   χ_i(Q) = A_i + B_i·Q + C_i·Q²，每轮按 χ 差在键两端转移 dq，
'   dq = (χ_B − χ_A)/(χ_A_den + χ_B_den)·0.5^k（k 为迭代轮次），
'   经典实现 4~6 轮。本文档化简化：单套元素参数（不区分杂化）。
' 残基电荷：逐残基 PEOE 后加形式电荷（ASP/GLU −1，LYS/ARG +1）；
' 水：O 固定 −0.8；SDF M CHG 形式电荷在 PEOE 前已置入并按残差校正。
' ============================================================================

Imports System
Imports System.Collections.Generic

Namespace MiniDock.Core

    Public Module Charges

        ' 电负性参数 χ(Q) = A + B·Q + C·Q²（Marsili-Gasteiger 单元素近似）
        Private ReadOnly ChiA As New Dictionary(Of String, Double) From {
            {"C", 8.3}, {"N", 12.0}, {"O", 14.18}, {"S", 10.14}, {"P", 8.42},
            {"F", 14.48}, {"Cl", 11.0}, {"Br", 10.08}, {"I", 9.9}, {"H", 7.17}}
        Private ReadOnly ChiB As New Dictionary(Of String, Double) From {
            {"C", 9.2}, {"N", 10.6}, {"O", 12.92}, {"S", 9.13}, {"P", 8.62},
            {"F", 13.85}, {"Cl", 9.69}, {"Br", 8.47}, {"I", 7.96}, {"H", 6.24}}
        Private ReadOnly ChiC As New Dictionary(Of String, Double) From {
            {"C", 1.6}, {"N", 0.62}, {"O", 0.73}, {"S", 1.28}, {"P", 1.77},
            {"F", 0.9}, {"Cl", 1.35}, {"Br", 1.25}, {"I", 0.77}, {"H", -0.56}}

        Private Function Chi(e As String, q As Double) As Double
            Dim a As Double = 8.3, b As Double = 9.2, c As Double = 1.6
            If ChiA.ContainsKey(e) Then
                a = ChiA(e) : b = ChiB(e) : c = ChiC(e)
            End If
            Return a + b * q + c * q * q
        End Function



        ''' <summary>对整分子做 PEOE；中性化后按 totalChargeTarget 校正最后一个极性原子</summary>
        Public Sub AssignPoeCharges(mol As Molecule, totalChargeTarget As Double)
            Dim n = mol.Atoms.Count
            Dim q(n - 1) As Double

            ' M CHG 已存的 formal charge 计入起点
            Dim formalSum As Double = 0
            For i = 0 To n - 1
                q(i) = mol.Atoms(i).Charge
                formalSum += q(i)
            Next

            Dim adj(n - 1) As List(Of Int32)
            Dim bord(n - 1) As List(Of Double)
            For i = 0 To n - 1
                adj(i) = New List(Of Int32)()
                bord(i) = New List(Of Double)()
            Next
            For Each b In mol.Bonds
                adj(b.A).Add(b.B) : bord(b.A).Add(b.Order)
                adj(b.B).Add(b.A) : bord(b.B).Add(b.Order)
            Next

            ' 6 轮部分均衡（第 k 轮阻尼 0.5^k）
            ' 界定式转移：dq = 0.3·damp·(χ_i − χ_j)/(χ_i + χ_j)，|dq| < 0.3·damp ≤ 0.15
            ' （保证收缩收敛；替代 Marsili-Gasteiger 的绝对分母形式，文档化简化）
            For it = 1 To 6
                Dim damp = Math.Pow(0.5, it)
                Dim transferred(n - 1) As Double
                For i = 0 To n - 1
                    Dim chiI = Chi(mol.Atoms(i).Element, q(i))
                    For k = 0 To adj(i).Count - 1
                        Dim j = adj(i)(k)
                        If j <= i Then Continue For
                        Dim chiJ = Chi(mol.Atoms(j).Element, q(j))
                        ' 电子从低电负性原子 i 流向高电负性原子 j：
                        ' dq = 0.3·damp·(χ_j − χ_i)/(χ_j + χ_i) > 0 时电子 i→j，
                        ' q_i 增（更正）、q_j 减（更负）
                        Dim dChi = chiJ - chiI
                        Dim dSum = chiI + chiJ
                        If Math.Abs(dSum) < 0.000000001 Then Continue For
                        Dim dq = 0.3 * damp * dChi / dSum
                        transferred(i) += dq
                        transferred(j) -= dq
                    Next
                Next
                For i = 0 To n - 1
                    q(i) += transferred(i)
                Next
            Next

            ' 校正到目标总电荷
            Dim total As Double = 0
            For i = 0 To n - 1
                total += q(i)
            Next
            Dim residual = totalChargeTarget - total - formalSum
            ' 残差加到电负性最强的极性原子上（简单确定性策略）
            Dim anchor = -1
            Dim bestChi As Double = Double.NegativeInfinity
            For i = 0 To n - 1
                Dim e = mol.Atoms(i).Element
                If e = "N" OrElse e = "O" OrElse e = "S" Then
                    Dim c = Chi(e, q(i))
                    If c > bestChi Then
                        bestChi = c
                        anchor = i
                    End If
                End If
            Next
            If anchor < 0 AndAlso n > 0 Then anchor = 0
            If anchor >= 0 Then q(anchor) += residual

            For i = 0 To n - 1
                mol.Atoms(i).Charge = q(i)
            Next
        End Sub

        ''' <summary>蛋白：逐残基 PEOE + 形式电荷；水：O = −0.8</summary>
        Public Sub AssignProteinCharges(mol As Molecule)
            ' 按残基分组
            Dim groups As New Dictionary(Of String, List(Of Int32))()
            For i = 0 To mol.Atoms.Count - 1
                Dim a = mol.Atoms(i)
                If a.IsWater Then Continue For
                Dim key = $"{a.ChainId}:{a.ResName}:{a.ResSeq}"
                If Not groups.ContainsKey(key) Then groups(key) = New List(Of Int32)()
                groups(key).Add(i)
            Next

            For Each kv In groups
                Dim idxList = kv.Value
                Dim resName = mol.Atoms(idxList(0)).ResName
                Dim [sub] As New Molecule With {.Id = resName}
                For Each i In idxList
                    [sub].Atoms.Add(mol.Atoms(i))
                Next
                ' 残基内键：模板
                Dim specs() As String = Nothing
                If ResidueTemplates.TryGetBondSpecs(resName, specs) Then
                    Dim nameIdx As New Dictionary(Of String, Int32)(StringComparer.Ordinal)
                    For k = 0 To idxList.Count - 1
                        nameIdx(mol.Atoms(idxList(k)).AtomName) = k
                    Next
                    For Each spec In specs
                        Dim order As Double = 1.0
                        Dim pairNames = spec
                        If spec.Contains("="c) Then
                            order = 2.0
                            pairNames = spec.Replace("="c, "-"c)
                        End If
                        Dim pp = pairNames.Split("-"c)
                        If pp.Length = 2 AndAlso nameIdx.ContainsKey(pp(0)) AndAlso nameIdx.ContainsKey(pp(1)) Then
                            [sub].Bonds.Add(New Bond(nameIdx(pp(0)), nameIdx(pp(1)), order))
                        End If
                    Next
                Else
                    StructureIO.PerceiveBonds([sub])
                End If
                AssignPoeCharges([sub], ResidueTemplates.FormalCharge(resName))
            Next

            ' 水
            For Each a In mol.Atoms
                If a.IsWater Then a.Charge = -0.8
            Next
        End Sub

    End Module

End Namespace
