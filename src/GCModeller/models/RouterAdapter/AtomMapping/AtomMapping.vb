' ============================================================================
' AtomMapping.vb — 基于最大公共子图(MCS)的原子映射与反应中心定位
' ----------------------------------------------------------------------------
' [readme_alg.md §2.1 原子映射] 原子映射是 NP 难问题（其包含子图同构问题作为特例）。
'   此处采用"键级保守的诱导式最大公共子图"：任意两个已映射原子对之间，键的存在性
'   与键级必须完全一致（ordA(i,j) = ordB(i,j)，不存在的键记为 0）。因此反应中心处
'   （键级改变/断键/成键）的原子会自然落出 MCS，表现为未映射原子——这正是
'   [§2.2 反应中心识别] 所需要的差分信息。
' 搜索策略：
'   1) 先跑一遍贪心（按 Morgan 型不变量排序的候选优先）得到下界；
'   2) 若贪心已达到"足够好"的目标（min(|A|,|B|) - maxUnmapped）则直接返回——
'      反应中心通常很小，不必求真正的最优解；
'   3) 否则带节点预算的回溯搜索：上界剪枝 + 预算耗尽保留当前最优（随时算法）。
' ============================================================================

Imports SMRUCC.genomics.Analysis.RetroPath.Chem

Public Module AtomMapping

    ''' <summary>
    ''' 对反应两侧做原子映射。两侧都可以是多组分分子（用 "." 拼起来的底物集合/产物集合），
    ''' 匹配得到的公共子图允许跨组分断开——这正是"辅因子部分保持不变"所需要的语义。
    ''' </summary>
    ''' <param name="reactants">反应物侧分子（可含多个连通分量）</param>
    ''' <param name="products">产物侧分子（可含多个连通分量）</param>
    ''' <param name="nodeBudget">回溯搜索的节点预算，耗尽即返回当前最优</param>
    ''' <param name="maxUnmapped">允许的未映射原子数（≈反应中心规模），用于提前终止</param>
    Public Function Map(reactants As Molecule, products As Molecule,
                        Optional nodeBudget As Integer = 60000,
                        Optional maxUnmapped As Integer = 10) As MappingResult

        Dim res As New MappingResult()
        Dim nR As Integer = If(reactants Is Nothing, 0, reactants.NumAtoms())
        Dim nP As Integer = If(products Is Nothing, 0, products.NumAtoms())

        res.ReactantToProduct = If(nR > 0, New Integer(nR - 1) {}, Array.Empty(Of Integer)())
        res.ProductToReactant = If(nP > 0, New Integer(nP - 1) {}, Array.Empty(Of Integer)())

        For i As Integer = 0 To nR - 1
            res.ReactantToProduct(i) = -1
        Next
        For j As Integer = 0 To nP - 1
            res.ProductToReactant(j) = -1
        Next

        If nR = 0 OrElse nP = 0 Then Return res

        ' 以较小的一侧作为搜索探针，压缩搜索深度
        Dim swap As Boolean = nP < nR
        Dim a As Molecule = If(swap, products, reactants)
        Dim b As Molecule = If(swap, reactants, products)

        Dim search As New McsSearch(a, b, nodeBudget, maxUnmapped)
        Dim best As List(Of (Integer, Integer)) = search.Run()

        For Each pr In best
            Dim r As Integer = If(swap, pr.Item2, pr.Item1)
            Dim p As Integer = If(swap, pr.Item1, pr.Item2)

            res.Pairs.Add((r, p))
            res.ReactantToProduct(r) = p
            res.ProductToReactant(p) = r
        Next

        For i As Integer = 0 To nR - 1
            If res.ReactantToProduct(i) < 0 Then res.ReactantOnly.Add(i)
        Next
        For j As Integer = 0 To nP - 1
            If res.ProductToReactant(j) < 0 Then res.ProductOnly.Add(j)
        Next

        Return res
    End Function

End Module
