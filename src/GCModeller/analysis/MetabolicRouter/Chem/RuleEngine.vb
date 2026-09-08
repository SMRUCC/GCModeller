' ============================================================================
' RuleEngine.vb — 广义反应规则与 SMIRKS 式变换应用
' ----------------------------------------------------------------------------
' 规则 = (反应物模式, 产物模式, ΔG, 酶可得性层级)——类号对应原子映射
'   [readme.md §2 原子映射→反应中心提取→规则泛化]。
' 应用语义（双向）：
'   存活原子 = 两侧共有类（元素/电荷按目标侧覆盖）；
'   目标侧有键、匹配侧无 → 创建原子 [readme.md §2 规则泛化：新键/新原子]；
'   匹配侧有键、目标侧无 → 断键（断开的原子成为独立碎片=共产物，如 H2O/CO2/NH3）；
'   两侧皆无的原子对 → 环境键保留。
'   应用后全分子价态校验，违规 → 丢弃该应用 [化学合理性闸门]。
' ============================================================================

Namespace Chem

    ''' <summary>
    ''' 酶等级枚举
    ''' </summary>
    Public Enum EnzymeTiers
        ''' <summary>常见酶家族</summary>
        Common = 1
        ''' <summary>一般</summary>
        General = 2
        ''' <summary>特化</summary>
        Specialized = 3
    End Enum

    Public Class Rule

        Public Id As String
        Public Name As String
        Public ReactantText As String
        Public ProductText As String
        Public Reactant As Pattern
        Public Product As Pattern
        ''' <summary>
        ''' 正向 kJ/mol（启发式基团贡献代理值）
        ''' </summary>
        Public DeltaG As Double
        ''' <summary>
        ''' 1=常见酶家族 2=一般 3=特化
        ''' </summary>
        Public EnzymeTier As EnzymeTiers
        Public Reversible As Boolean

        Public Sub New(id As String, name As String, reactantSmarts As String,
                       productSmarts As String, dg As Double, tier As EnzymeTiers,
                       Optional reversible As Boolean = True)
            Me.Id = id
            Me.Name = name
            Me.ReactantText = reactantSmarts
            Me.ProductText = productSmarts
            Me.Reactant = PatternMatcher.ParsePattern(reactantSmarts)
            Me.Product = PatternMatcher.ParsePattern(productSmarts)
            Me.DeltaG = dg
            Me.EnzymeTier = tier
            Me.Reversible = reversible
        End Sub

    End Class

    Public Class ApplicationResult

        ''' <summary>
        ''' 变换后的连通碎片
        ''' </summary>
        Public Fragments As List(Of Molecule)
        ''' <summary>
        ''' (类号, 分子原子索引)
        ''' </summary>
        Public AtomMap As List(Of Tuple(Of Int32, Int32))

    End Class

    Public Module RuleEngine

        ''' <summary>正向应用：匹配反应物模式 → 产物拓扑</summary>
        Public Function ApplyForward(m As Molecule, rule As Rule, Optional limit As Int32 = 50) As List(Of ApplicationResult)
            Return Apply(m, rule.Reactant, rule.Product, limit)
        End Function

        ''' <summary>逆向应用：匹配产物模式 → 反应物拓扑（逆合成方向）</summary>
        Public Function ApplyReverse(m As Molecule, rule As Rule, Optional limit As Int32 = 50) As List(Of ApplicationResult)
            Return Apply(m, rule.Product, rule.Reactant, limit)
        End Function

        ''' <summary>核心变换：match 侧匹配，other 侧拓扑</summary>
        Public Function Apply(m As Molecule, matchSide As Pattern, otherSide As Pattern,
                              Optional limit As Int32 = 50) As List(Of ApplicationResult)
            Dim matches = PatternMatcher.Match(m, matchSide, limit)
            Dim outList As New List(Of ApplicationResult)()

            ' other 侧拓扑
            Dim otherBonded As New HashSet(Of Int32)()
            For Each b In otherSide.Bonds
                otherBonded.Add(otherSide.Atoms(b.a).Cls)
                otherBonded.Add(otherSide.Atoms(b.b).Cls)
            Next
            Dim otherAtom As New Dictionary(Of Int32, PatternAtom)()
            For Each a In otherSide.Atoms
                If Not otherAtom.ContainsKey(a.Cls) Then otherAtom(a.Cls) = a
            Next
            Dim otherBonds As New Dictionary(Of Tuple(Of Int32, Int32), Int32)()
            For Each b In otherSide.Bonds
                Dim cx = otherSide.Atoms(b.a).Cls
                Dim cy = otherSide.Atoms(b.b).Cls
                otherBonds(Tuple.Create(Math.Min(cx, cy), Math.Max(cx, cy))) = b.order
            Next
            Dim matchBonds As New Dictionary(Of Tuple(Of Int32, Int32), Int32)()
            For Each b In matchSide.Bonds
                Dim cx = matchSide.Atoms(b.a).Cls
                Dim cy = matchSide.Atoms(b.b).Cls
                matchBonds(Tuple.Create(Math.Min(cx, cy), Math.Max(cx, cy))) = b.order
            Next

            For Each mp In matches
                Dim res = m.Copy()
                Dim mapped As New List(Of Tuple(Of Int32, Int32))()
                Dim surv As New Dictionary(Of Int32, Int32)()

                ' 1) 存活原子
                For Each kvp In mp
                    Dim cls = kvp.Key
                    Dim ma = kvp.Value
                    surv(cls) = ma
                    Dim pa As PatternAtom = Nothing
                    If otherAtom.TryGetValue(cls, pa) Then
                        If pa.Element IsNot Nothing Then res.Elements(ma) = pa.Element
                        If pa.Charge <> -999 Then res.Charges(ma) = pa.Charge
                    End If
                    mapped.Add(Tuple.Create(cls, ma))
                Next
                ' 2) 创建原子（other 侧有键但未匹配）
                Dim createdCls As New List(Of Int32)(otherBonded.OrderBy(Function(x) x))
                For Each cls In createdCls
                    If surv.ContainsKey(cls) Then Continue For
                    Dim pa = otherAtom(cls)
                    Dim idx = res.AddAtom(pa.Element, If(pa.Charge <> -999, pa.Charge, 0))
                    surv(cls) = idx
                    mapped.Add(Tuple.Create(cls, idx))
                Next
                ' 3) 键处理（所有存活类对）
                Dim allCls = surv.Keys.OrderBy(Function(x) x).ToList()
                For i = 0 To allCls.Count - 2
                    For j = i + 1 To allCls.Count - 1
                        Dim ca = allCls(i)
                        Dim cb = allCls(j)
                        Dim key = Tuple.Create(Math.Min(ca, cb), Math.Max(ca, cb))
                        Dim a = surv(ca)
                        Dim b2 = surv(cb)
                        Dim inOther = otherBonds.ContainsKey(key)
                        Dim inMatch = matchBonds.ContainsKey(key)
                        If inOther Then
                            Dim order = otherBonds(key)
                            If order = 0 Then order = 1
                            Dim bo = res.BondOrder(a, b2)
                            If bo = 0 Then
                                res.Bonds.Add((a, b2, order))
                            ElseIf bo <> order Then
                                res.SetBondOrder(a, b2, order)
                            End If
                        ElseIf inMatch Then
                            res.RemoveBond(a, b2)      ' 断键 [readme.md 反应中心]
                        End If
                        ' 两者皆无 → 环境键保留
                    Next
                Next
                ' 4) 价态校验
                If res.ValenceViolations().Count > 0 Then Continue For
                ' 5) 碎片化
                Dim result As New ApplicationResult With {
                    .Fragments = res.SplitComponents(),
                    .AtomMap = mapped}
                outList.Add(result)
            Next
            Return outList
        End Function

    End Module

End Namespace
