' ============================================================================
' MoleculeSnapshot.vb — 分子不变量快照（匹配期的只读加速结构）
' ----------------------------------------------------------------------------
' 子图匹配阶段要对「每个模式原子 × 每个分子原子」做约束判定（元素、电荷、氢数、
' 重原子度、!O / !=O 邻居、键级一致性）。原先每次判定都要回到 Molecule 现算，
' 而 TotalH/Degree 各自都要扫一遍键表，整体退化到 O(模式原子 × 分子原子 × 键数)。
'
' 一次匹配期间分子不会改变，因此可以在匹配入口一次性算出全部不变量，之后所有
' 判定都变成 O(1) 或 O(度) 的查表。
' ============================================================================

Namespace Chem

    ''' <summary>
    ''' 分子在一次匹配期间的只读不变量快照：元素 / 电荷 / 氢总数 / 重原子度 + 邻接表。
    ''' </summary>
    ''' <remarks>
    ''' 只在分子不再被修改的时间窗内使用（<see cref="PatternMatcher.Match"/> 正是这种场景）。
    ''' </remarks>
    Public Class MoleculeSnapshot

        ''' <summary>各原子的元素符号。</summary>
        Public ReadOnly Element As String()
        ''' <summary>各原子的形式电荷。</summary>
        Public ReadOnly Charge As Int32()
        ''' <summary>各原子的氢总数（显式 + 隐式）。</summary>
        Public ReadOnly TotalH As Int32()
        ''' <summary>各原子的重原子度。</summary>
        Public ReadOnly Degree As Int32()
        ''' <summary>邻接表：每个原子一个 (邻居索引, 键级) 列表。</summary>
        Public ReadOnly Adj As List(Of List(Of (Int32, Int32)))

        ''' <summary>
        ''' 为一个分子构建不变量快照（O(原子数 + 键数)）。
        ''' </summary>
        ''' <param name="m">被快照的分子（在快照使用期间不应再被修改）。</param>
        Public Sub New(m As Molecule)
            Dim n As Int32 = m.NumAtoms()

            Element = New String(n - 1) {}
            Charge = New Int32(n - 1) {}
            TotalH = New Int32(n - 1) {}
            Degree = New Int32(n - 1) {}
            Adj = m.Adjacency()

            For a = 0 To n - 1
                Element(a) = m.Elements(a)
                Charge(a) = m.Charges(a)
                Degree(a) = Adj(a).Count
                TotalH(a) = m.TotalH(a)
            Next
        End Sub

        ''' <summary>查两原子间的键级（O(度)）；无键返回 0。</summary>
        Public Function BondOrder(a As Int32, b As Int32) As Int32
            For Each nb In Adj(a)
                If nb.Item1 = b Then Return nb.Item2
            Next
            Return 0
        End Function

    End Class

End Namespace
