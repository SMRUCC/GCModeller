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
    ''' 酶可得性层级（酶可得性评分的代理指标；数值越小代表越容易找到可用酶）。
    ''' </summary>
    ''' <remarks>
    ''' readme.md §4 中"酶可得性"本应基于反应指纹与序列数据库比对（Selenzyme / BridgIT），
    ''' 本库以层级代理简化：酶得分 = mean(1/tier)。
    ''' </remarks>
    Public Enum EnzymeTiers
        ''' <summary>常见酶家族（有明确 EC 号归属，最容易获取）。</summary>
        Common = 1
        ''' <summary>一般（如自发反应，或仅知反应类型）。</summary>
        General = 2
        ''' <summary>特化（孤儿反应等，酶未知，获取难度最高）。</summary>
        Specialized = 3
    End Enum

    ''' <summary>
    ''' 一条广义反应规则：(反应物模式, 产物模式, ΔG, 酶层级)。
    ''' </summary>
    ''' <remarks>
    ''' 规则不绑定具体底物，只描述"反应中心发生了什么化学变换"——这正是它能组合出自然界
    ''' 尚不存在的反应序列的原因。两侧模式通过原子类号（<c>:n</c>）建立原子映射。
    ''' </remarks>
    Public Class Rule

        ''' <summary>规则唯一标识（内置规则如 "R001"，数据库挖掘规则用反应 ID）。</summary>
        Public Id As String
        ''' <summary>规则的可读名称（如"醇脱氢酶（氧化/还原）"）。</summary>
        Public Name As String
        ''' <summary>反应物侧模式的原始文本（SMARTS 子集）。</summary>
        Public ReactantText As String
        ''' <summary>产物侧模式的原始文本（SMARTS 子集）。</summary>
        Public ProductText As String
        ''' <summary>反应物侧已解析的模式对象。</summary>
        Public Reactant As Pattern
        ''' <summary>产物侧已解析的模式对象。</summary>
        Public Product As Pattern
        ''' <summary>
        ''' 正向（反应物 → 产物）的标准吉布斯自由能变（kJ/mol，启发式基团贡献代理值）。
        ''' </summary>
        Public DeltaG As Double
        ''' <summary>
        ''' 1=常见酶家族 2=一般 3=特化，见 <see cref="EnzymeTiers"/>。
        ''' </summary>
        Public EnzymeTier As EnzymeTiers
        ''' <summary>该反应在生理条件下是否可逆。</summary>
        Public Reversible As Boolean

        ''' <summary>
        ''' 构造一条反应规则；两侧模式字符串会在此处被立即解析，非法模式会抛异常。
        ''' </summary>
        ''' <param name="id">规则唯一标识。</param>
        ''' <param name="name">规则名称。</param>
        ''' <param name="reactantSmarts">反应物侧模式串。</param>
        ''' <param name="productSmarts">产物侧模式串。</param>
        ''' <param name="dg">正向 ΔG（kJ/mol）。</param>
        ''' <param name="tier">酶可得性层级。</param>
        ''' <param name="reversible">是否可逆。</param>
        ''' <exception cref="ArgumentException">模式串无法解析时抛出。</exception>
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

    ''' <summary>
    ''' 一次规则应用的结果：变换后得到的碎片集合与原子映射。
    ''' </summary>
    Public Class ApplicationResult

        ''' <summary>
        ''' 变换并断开化学键后得到的连通碎片（每个碎片是一个独立分子）。
        ''' </summary>
        Public Fragments As List(Of Molecule)
        ''' <summary>
        ''' 本次变换的原子映射，每项为 (模式类号, 分子中的原子索引)。
        ''' </summary>
        Public AtomMap As List(Of Tuple(Of Int32, Int32))

    End Class

    ''' <summary>
    ''' 规则引擎：把广义反应规则双向应用于分子，完成"反应中心匹配 + 原子映射 + 拓扑变换"。
    ''' </summary>
    ''' <remarks>
    ''' 变换语义（<see cref="Apply(Molecule, Pattern, Pattern, Integer)"/>）：
    ''' <list type="bullet">
    ''' <item>存活原子 = 两侧共有的类号（元素/电荷按目标侧覆盖）；</item>
    ''' <item>目标侧成键而匹配侧无键 → 创建新原子（离去/加入的辅底物）；</item>
    ''' <item>匹配侧成键而目标侧无键 → 断键（断键处即反应中心）；</item>
    ''' <item>两侧都不涉及的键 → 环境键，原样保留。</item>
    ''' </list>
    ''' 应用结束后会做全分子价态校验，违规的应用被丢弃——这是化学合理性的闸门。
    ''' </remarks>
    Public Module RuleEngine

        ''' <summary>
        ''' 正向应用规则：匹配规则的反应物模式，生成产物拓扑。
        ''' </summary>
        ''' <param name="m">被作用的底物分子。</param>
        ''' <param name="rule">待应用的规则。</param>
        ''' <param name="limit">每个模式的匹配枚举上限，用于防组合爆炸。</param>
        ''' <returns>应用结果列表；未命中或全部被价态闸门拒绝时返回空列表。</returns>
        Public Function ApplyForward(m As Molecule, rule As Rule, Optional limit As Int32 = 50) As List(Of ApplicationResult)
            Return Apply(m, rule.Reactant, rule.Product, limit)
        End Function

        ''' <summary>
        ''' 逆向应用规则：匹配规则的产物模式，反推反应物拓扑——逆合成搜索使用的方向。
        ''' </summary>
        ''' <param name="m">被分解的目标分子。</param>
        ''' <param name="rule">待应用的规则。</param>
        ''' <param name="limit">每个模式的匹配枚举上限。</param>
        ''' <returns>应用结果列表；未命中或全部被价态闸门拒绝时返回空列表。</returns>
        Public Function ApplyReverse(m As Molecule, rule As Rule, Optional limit As Int32 = 50) As List(Of ApplicationResult)
            Return Apply(m, rule.Product, rule.Reactant, limit)
        End Function

        ''' <summary>
        ''' 核心变换：在分子上匹配 <paramref name="matchSide"/> 模式，再按
        ''' <paramref name="otherSide"/> 的拓扑改写化学键。
        ''' </summary>
        ''' <param name="m">被变换的分子（不会被就地修改，内部先拷贝）。</param>
        ''' <param name="matchSide">匹配侧模式（决定在分子的哪个部位发生反应）。</param>
        ''' <param name="otherSide">目标侧模式（决定变换后的拓扑）。</param>
        ''' <param name="limit">匹配枚举上限。</param>
        ''' <returns>每个匹配对应的一个应用结果；已过滤掉价态非法的候选。</returns>
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
