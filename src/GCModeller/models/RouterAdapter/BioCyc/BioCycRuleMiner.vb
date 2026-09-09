' ============================================================================
' BioCycRuleMiner.vb — BioCyc 特有的"字段映射层"
' ----------------------------------------------------------------------------
' 真正的挖掘引擎已下沉到与数据源无关的 Data/RuleMiner.vb。本文件只负责把 BioCyc 的
' reactions 对象翻译成中立的 ReactionSpec，其余全部委托给通用引擎，从而保证 BioCyc
' 与 GCModeller 内部代谢模型两条通路挖出的规则语义完全一致。
'
' 与内部模型（MetabolicReaction）唯一的实质差异在"方向"：
'   BioCyc 的 reactions 带 REACTION-DIRECTION（RIGHT-TO-LEFT / PHYSIOL-RIGHT-TO-LEFT
'   表示生理方向是从右往左），必须先用只读属性 rxn.equation 归正左右两侧；
'   MetabolicReaction 没有方向枚举，一律按 left → right 理解。
' ============================================================================

Imports SMRUCC.genomics.Analysis.RetroPath.Chem
Imports SMRUCC.genomics.ComponentModel.EquaionModel.DefaultTypes
Imports SMRUCC.genomics.Data.BioCyc
Imports SMRUCC.genomics.Data.BioCyc.Assembly.MetaCyc.Schema.Metabolism
Imports SMRUCC.genomics.Model.Metabolic.RouterAdapter.Data

Public Module BioCycRuleMiner

    ''' <summary>
    ''' 把 BioCyc 的 reactions 集合映射为 ReactionSpec，再交给通用挖掘引擎。
    ''' </summary>
    ''' <param name="reactionList">BioCyc 反应集合（建议按 uniqueId 排序以保证确定性）</param>
    ''' <param name="structures">compound frame id → 已净化结构</param>
    ''' <param name="maxMoleculeAtoms">参与反应的分子重原子数上限（超出的反应跳过，控耗时）</param>
    ''' <param name="maxPatternAtoms">模式原子数上限（超出的规则过特异，跳过）</param>
    ''' <param name="mcsNodeBudget">MCS 回溯搜索的节点预算</param>
    ''' <param name="maxUnmappedAtoms">MCS 允许保留的未映射原子数（≈反应中心规模）</param>
    ''' <param name="shellRadius">反应中心向外扩展的层数</param>
    ''' <param name="includeBuiltin">是否叠加 RetroPath 内置的 9 条广义规则</param>
    ''' <param name="strictSelfCheck">严格模式：丢弃"无法重现自身反应"的退化规则</param>
    ''' <param name="skipped">跳过原因计数</param>
    ''' <param name="trace">逐条反应的跳过原因（reaction uniqueId → 原因）</param>
    ''' <returns>广义反应规则集。</returns>
    Public Function Mine(reactionList As IEnumerable(Of reactions),
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
    ''' 把 BioCyc 反应集合批量映射为 ReactionSpec（失败的条目已被剔除）。
    ''' </summary>
    ''' <param name="reactionList">BioCyc 反应集合。</param>
    ''' <param name="skipped">跳过原因计数（可为 Nothing）。</param>
    ''' <param name="trace">逐条反应的跳过原因（可为 Nothing）。</param>
    ''' <returns>可直接交给通用挖掘引擎的反应契约列表。</returns>
    Public Function ToSpecs(reactionList As IEnumerable(Of reactions),
                            Optional skipped As Dictionary(Of String, Integer) = Nothing,
                            Optional trace As Dictionary(Of String, String) = Nothing) As List(Of ReactionSpec)
        Dim specs As New List(Of ReactionSpec)()

        If reactionList Is Nothing Then Return specs

        For Each rxn As reactions In reactionList
            Dim spec As ReactionSpec = ToSpec(rxn, skipped, trace)
            If spec IsNot Nothing Then specs.Add(spec)
        Next

        Return specs
    End Function

    ''' <summary>
    ''' 单条 BioCyc 反应 → ReactionSpec；方向不可用时返回 Nothing。
    ''' </summary>
    ''' <param name="rxn">BioCyc 反应对象。</param>
    ''' <param name="skipped">跳过原因计数（可为 Nothing）。</param>
    ''' <param name="trace">逐条反应的跳过原因（可为 Nothing）。</param>
    ''' <returns>已归正的 <see cref="ReactionSpec"/>；失败时返回 Nothing。</returns>
    Public Function ToSpec(rxn As reactions,
                           Optional skipped As Dictionary(Of String, Integer) = Nothing,
                           Optional trace As Dictionary(Of String, String) = Nothing) As ReactionSpec
        If rxn Is Nothing Then
            CountSkip(skipped, "null-reaction")
            Return Nothing
        End If

        Dim rxnId As String = rxn.uniqueId

        ' 方向归正：不要直接用 left/right——equation 已按 REACTION-DIRECTION 归正
        Dim eq As Equation = Nothing

        Try
            eq = rxn.equation
        Catch ex As Exception
            Bail(skipped, trace, rxnId, "bad-equation")
            Return Nothing
        End Try

        If eq Is Nothing Then
            Bail(skipped, trace, rxnId, "no-equation")
            Return Nothing
        End If

        Dim reactantIds As List(Of String) = RuleMiner.CompoundIds(eq.Reactants)
        Dim productIds As List(Of String) = RuleMiner.CompoundIds(eq.Products)

        If reactantIds.Count = 0 OrElse productIds.Count = 0 Then
            Bail(skipped, trace, rxnId, "empty-side")
            Return Nothing
        End If

        ' 名称按"常用名 → 系统名 → id"回退
        Dim name As String = rxn.commonName
        If String.IsNullOrEmpty(name) Then name = rxn.systematicName
        If String.IsNullOrEmpty(name) Then name = rxnId

        Dim gibbs As Double = rxn.gibbs0
        If Double.IsNaN(gibbs) OrElse Double.IsInfinity(gibbs) Then gibbs = 0

        Return New ReactionSpec With {
            .Id = rxnId,
            .Name = name,
            .ReactantIds = reactantIds,
            .ProductIds = productIds,
            .ECNumbers = If(rxn.ec_number Is Nothing,
                            Nothing,
                            rxn.ec_number.Select(Function(ec) ec.ToString()).ToArray()),
            .IsSpontaneous = rxn.spontaneous,
            .Gibbs = gibbs,
            .IsReversible = (rxn.reactionDirection = ReactionDirections.Reversible)
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
