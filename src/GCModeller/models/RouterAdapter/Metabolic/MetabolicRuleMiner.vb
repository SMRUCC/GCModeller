' ============================================================================
' MetabolicRuleMiner.vb — GCModeller 内部代谢模型的"字段映射层"
' ----------------------------------------------------------------------------
' 真正的挖掘引擎在与数据源无关的 Data/RuleMiner.vb。本文件只负责把内部的
' MetabolicReaction 翻译成中立的 ReactionSpec，其余全部委托给通用引擎。
'
' 与 BioCyc 的差异（正是这层要消化的）：
'   · BioCyc 的 reactions 带 REACTION-DIRECTION，需要按方向翻转左右两侧；
'   · MetabolicReaction 没有方向枚举——按其模型注释，不可逆时方向即为 left → right，
'     可逆性只由 is_reversible 表达。因此这里直接把 left 当底物、right 当产物。
' ============================================================================

Imports SMRUCC.genomics.Analysis.RetroPath.Chem
Imports SMRUCC.genomics.MetabolicModel
Imports SMRUCC.genomics.Model.Metabolic.RouterAdapter.Data

Public Module MetabolicRuleMiner

    ''' <summary>
    ''' 内部代谢反应集合 → 广义反应规则（一步到位）。
    ''' </summary>
    ''' <param name="reactionList">内部代谢反应集合（建议按 id 排序以保证确定性）。</param>
    ''' <param name="structures">compound id → 已净化结构。</param>
    ''' <param name="maxMoleculeAtoms">参与反应的分子重原子数上限。</param>
    ''' <param name="maxPatternAtoms">模式原子数上限。</param>
    ''' <param name="mcsNodeBudget">MCS 回溯搜索的节点预算。</param>
    ''' <param name="maxUnmappedAtoms">MCS 允许保留的未映射原子数。</param>
    ''' <param name="shellRadius">反应中心向外扩展的层数。</param>
    ''' <param name="includeBuiltin">是否叠加 RetroPath 内置的 9 条广义规则。</param>
    ''' <param name="strictSelfCheck">严格模式：丢弃无法重现自身反应的退化规则。</param>
    ''' <param name="skipped">跳过原因计数。</param>
    ''' <param name="trace">逐条反应的跳过原因。</param>
    ''' <returns>广义反应规则集。</returns>
    Public Function Mine(reactionList As IEnumerable(Of MetabolicReaction),
                         structures As Dictionary(Of String, CompoundStructure),
                         Optional maxMoleculeAtoms As Integer = 80,
                         Optional maxPatternAtoms As Integer = 32,
                         Optional mcsNodeBudget As Integer = 60000,
                         Optional maxUnmappedAtoms As Integer = 3,
                         Optional shellRadius As Integer = 2,
                         Optional includeBuiltin As Boolean = False,
                         Optional strictSelfCheck As Boolean = False,
                         Optional skipped As Dictionary(Of String, Integer) = Nothing,
                         Optional trace As Dictionary(Of String, String) = Nothing) As List(Of Rule)

        Return RuleMiner.Mine(ToSpecs(reactionList, skipped, trace), structures,
                              maxMoleculeAtoms, maxPatternAtoms, mcsNodeBudget,
                              maxUnmappedAtoms, shellRadius, includeBuiltin,
                              strictSelfCheck, skipped, trace)
    End Function

    ''' <summary>
    ''' 把内部代谢反应集合批量映射为 ReactionSpec（失败的条目已被剔除）。
    ''' </summary>
    ''' <param name="reactionList">内部代谢反应集合。</param>
    ''' <param name="skipped">跳过原因计数（可为 Nothing）。</param>
    ''' <param name="trace">逐条反应的跳过原因（可为 Nothing）。</param>
    ''' <returns>可直接交给通用挖掘引擎的反应契约列表。</returns>
    Public Function ToSpecs(reactionList As IEnumerable(Of MetabolicReaction),
                            Optional skipped As Dictionary(Of String, Integer) = Nothing,
                            Optional trace As Dictionary(Of String, String) = Nothing) As List(Of ReactionSpec)
        Dim specs As New List(Of ReactionSpec)()

        If reactionList Is Nothing Then Return specs

        For Each rxn As MetabolicReaction In reactionList
            Dim spec As ReactionSpec = ToSpec(rxn, skipped, trace)
            If spec IsNot Nothing Then specs.Add(spec)
        Next

        Return specs
    End Function

    ''' <summary>
    ''' 单条内部代谢反应 → ReactionSpec；缺侧或不可用 id 时返回 Nothing。
    ''' </summary>
    ''' <param name="rxn">内部代谢反应对象。</param>
    ''' <param name="skipped">跳过原因计数（可为 Nothing）。</param>
    ''' <param name="trace">逐条反应的跳过原因（可为 Nothing）。</param>
    ''' <returns>已归正的 <see cref="ReactionSpec"/>；失败时返回 Nothing。</returns>
    Public Function ToSpec(rxn As MetabolicReaction,
                           Optional skipped As Dictionary(Of String, Integer) = Nothing,
                           Optional trace As Dictionary(Of String, String) = Nothing) As ReactionSpec
        If rxn Is Nothing Then
            CountSkip(skipped, "null-reaction")
            Return Nothing
        End If

        Dim rxnId As String = rxn.id

        If String.IsNullOrEmpty(rxnId) Then
            CountSkip(skipped, "no-reaction-id")
            Return Nothing
        End If

        ' 内部模型的方向约定：不可逆时即为 left → right，故无需像 BioCyc 那样归正
        Dim reactantIds As List(Of String) = RuleMiner.CompoundIds(rxn.left)
        Dim productIds As List(Of String) = RuleMiner.CompoundIds(rxn.right)

        If reactantIds.Count = 0 OrElse productIds.Count = 0 Then
            Bail(skipped, trace, rxnId, "empty-side")
            Return Nothing
        End If

        Dim name As String = rxn.name
        If String.IsNullOrEmpty(name) Then name = rxn.description
        If String.IsNullOrEmpty(name) Then name = rxnId

        Dim gibbs As Double = rxn.gibbs
        If Double.IsNaN(gibbs) OrElse Double.IsInfinity(gibbs) Then gibbs = 0

        Return New ReactionSpec With {
            .Id = rxnId,
            .Name = name,
            .ReactantIds = reactantIds,
            .ProductIds = productIds,
            .ECNumbers = rxn.ECNumbers,
            .IsSpontaneous = rxn.is_spontaneous,
            .Gibbs = gibbs,
            .IsReversible = rxn.is_reversible
        }
    End Function

    Private Sub CountSkip(skipped As Dictionary(Of String, Integer), reason As String)
        If skipped Is Nothing Then Return
        Dim n As Integer = 0
        skipped.TryGetValue(reason, n)
        skipped(reason) = n + 1
    End Sub

    Private Sub Bail(skipped As Dictionary(Of String, Integer),
                     trace As Dictionary(Of String, String),
                     id As String, reason As String)
        CountSkip(skipped, reason)
        If trace IsNot Nothing AndAlso id IsNot Nothing Then trace(id) = reason
    End Sub

End Module
