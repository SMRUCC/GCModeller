' ============================================================================
' ApplyPlan.vb — 规则某一应用方向的只读预计算拓扑
' ----------------------------------------------------------------------------
' RuleEngine.Apply 原先在「每次调用」时都要遍历规则两侧模式，重建 4 个拓扑结构
'（目标侧成键类号集合、类号→模式原子、目标侧键表、匹配侧键表）。这些结构只依赖
' 规则本身、与分子无关，却在「分子 × 规则 × 方向」的规模上被重建数百万次。
'
' 本类型把这部分工作提前到规则构造期完成一次；应用期只读共享，因此天然线程安全，
' 是搜索主循环并行化的前提之一。
' ============================================================================

Namespace Chem

    ''' <summary>
    ''' 一条规则在某个应用方向（匹配侧 → 目标侧）上的预计算拓扑。
    ''' </summary>
    ''' <remarks>
    ''' 构造后内容不再变化，可被多个线程同时读取。
    ''' </remarks>
    Public Class ApplyPlan

        ''' <summary>匹配侧模式（决定规则在分子的哪个部位发生反应）。</summary>
        Public ReadOnly MatchSide As Pattern

        ''' <summary>目标侧模式（决定变换后的拓扑）。</summary>
        Public ReadOnly OtherSide As Pattern

        ''' <summary>目标侧中出现于键里的类号集合。</summary>
        Public ReadOnly OtherBonded As New HashSet(Of Int32)()

        ''' <summary>类号 → 目标侧的模式原子（同一类号取首个出现的模式原子）。</summary>
        Public ReadOnly AtomOfCls As New Dictionary(Of Int32, PatternAtom)()

        ''' <summary>目标侧的键：(较小类号, 较大类号) → 键级。</summary>
        Public ReadOnly OtherBonds As New Dictionary(Of Tuple(Of Int32, Int32), Int32)()

        ''' <summary>匹配侧的键：(较小类号, 较大类号) → 键级。</summary>
        Public ReadOnly MatchBonds As New Dictionary(Of Tuple(Of Int32, Int32), Int32)()

        ''' <summary>
        ''' 需要「创建」的类号：目标侧成键、但匹配侧未出现的类。已按升序排列，
        ''' 保证创建顺序确定（原子索引与映射顺序都与串行版本一致）。
        ''' </summary>
        Public ReadOnly CreatedClasses As New List(Of Int32)()

        ''' <summary>
        ''' 预计算一个方向的拓扑。
        ''' </summary>
        ''' <param name="matchSide">匹配侧模式。</param>
        ''' <param name="otherSide">目标侧模式。</param>
        Public Sub New(matchSide As Pattern, otherSide As Pattern)
            Me.MatchSide = matchSide
            Me.OtherSide = otherSide

            For Each b In otherSide.Bonds
                OtherBonded.Add(otherSide.Atoms(b.a).Cls)
                OtherBonded.Add(otherSide.Atoms(b.b).Cls)
            Next

            For Each a In otherSide.Atoms
                If Not AtomOfCls.ContainsKey(a.Cls) Then AtomOfCls(a.Cls) = a
            Next

            For Each b In otherSide.Bonds
                Dim cx = otherSide.Atoms(b.a).Cls
                Dim cy = otherSide.Atoms(b.b).Cls
                OtherBonds(ApplyPlan.KeyOf(cx, cy)) = b.order
            Next

            For Each b In matchSide.Bonds
                Dim cx = matchSide.Atoms(b.a).Cls
                Dim cy = matchSide.Atoms(b.b).Cls
                MatchBonds(ApplyPlan.KeyOf(cx, cy)) = b.order
            Next

            CreatedClasses.AddRange(OtherBonded.OrderBy(Function(x) x))
        End Sub

        ''' <summary>无序类号对的规范化字典键</summary>
        Public Shared Function KeyOf(cx As Int32, cy As Int32) As Tuple(Of Int32, Int32)
            Return Tuple.Create(Math.Min(cx, cy), Math.Max(cx, cy))
        End Function

    End Class

End Namespace
