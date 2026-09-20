Imports System.Diagnostics
Imports System.Text
Imports SMRUCC.genomics.GCModeller.CompilerServices.GPRLink

''' <summary>
''' GPR 关联算法的测试用例集合。
''' 
''' 每个用例都基于 <see cref="SyntheticData"/> 构造的合成数据，并附带明确的"已知正确答案"。
''' 用例覆盖了算法中全部证据来源，其中若干条是针对历史缺陷的**回归断言**，
''' 在旧实现下必然失败（用例描述中已注明）。
''' </summary>
Public Module GPRTestCases

    ''' <summary>
    ''' 执行全部用例。返回综合演示用例的算法实例以及它使用的合成数据，供结果表渲染使用。
    ''' </summary>
    Public Function RunAll(runner As TestRunner) As (Associator As MetabolicAssociator, Data As SyntheticCase)
        CaseDirectEC(runner)
        CaseOperon(runner)
        CaseOperonEdge(runner)
        CaseDistanceDecay(runner)
        CaseEnzymeComplex(runner)
        CaseFusionAndCompleteness(runner)
        CaseNoiseConvergence(runner)
        CaseCoexpression(runner)
        CaseConservedSynteny(runner)
        CaseDegenerateInputs(runner)

        Dim demo As (Associator As MetabolicAssociator, Data As SyntheticCase) = CaseDemo(runner)

        CaseStress(runner)

        Return demo
    End Function

#Region "用例 1：直接 EC 匹配"

    Private Sub CaseDirectEC(runner As TestRunner)
        runner.BeginCase("1. 直接 EC 匹配")

        Dim data As SyntheticCase = SyntheticData.DirectEC()
        Dim assoc As MetabolicAssociator = Compute(data)
        Dim direct As Double = New GPRParameters().DirectMatchScore

        runner.AssertEqual("g_direct1 只关联到自身 EC 命中的那一个反应",
                           "P1_R1", Joined(ReactionsOf(assoc, "g_direct1")))
        runner.AssertEqual("g_direct2 只关联到自身 EC 命中的那一个反应",
                           "P2_R2", Joined(ReactionsOf(assoc, "g_direct2")))
        runner.AssertEqual("没有任何 EC 的 g_noEC 不产生关联",
                           0, ReactionsOf(assoc, "g_noEC").Length)

        runner.AssertNear("g_direct1 的打分等于直接匹配权重", direct, ScoreOf(assoc, "g_direct1", "P1_R1"))
        runner.AssertEqual("g_direct1 的证据类型只有 DirectEC",
                           "DirectEC", Joined(KindsOf(assoc, "g_direct1", "P1_R1")))

        ' 旧实现会因为邻居基因的 EC 命中而把整条通路的反应都灌给同一个基因
        runner.AssertNotContains("g_direct1 不会被灌上同一通路内的其它反应",
                                 ReactionsOf(assoc, "g_direct1"), "P1_R2")
        runner.AssertNotContains("g_direct1 不会被灌上同一通路内的其它反应",
                                 ReactionsOf(assoc, "g_direct1"), "P1_R3")
        runner.AssertNotContains("g_direct1 不会被灌上其它通路的反应",
                                 ReactionsOf(assoc, "g_direct1"), "P2_R1")
    End Sub

#End Region

#Region "用例 2 / 3：操纵子上下文与操纵子边界"

    Private Sub CaseOperon(runner As TestRunner)
        runner.BeginCase("2. 操纵子上下文")

        Dim data As SyntheticCase = SyntheticData.Operon()
        Dim assoc As MetabolicAssociator = Compute(data)

        runner.AssertEqual("无 EC 的操纵子成员 g_op4 从同操纵子成员获得三个反应的关联",
                           "P1_R1|P1_R2|P1_R3", Joined(ReactionsOf(assoc, "g_op4")))

        Dim kinds As String() = KindsOf(assoc, "g_op4", "P1_R1")
        runner.AssertContains("g_op4 的证据类型包含操纵子上下文", kinds, "OperonContext")
        runner.AssertNotContains("操纵子成员之间不再重复叠加窗口上下文证据", kinds, "WindowContext")

        Dim operonScore As Double = ScoreOf(assoc, "g_op4", "P1_R1")
        Dim direct As Double = New GPRParameters().DirectMatchScore

        runner.AssertTrue("操纵子上下文的分数高于置信度阈值",
                          operonScore >= New GPRParameters().ConfidenceThreshold,
                          $"实际 {operonScore:F4}")
        runner.AssertTrue("操纵子上下文属于间接证据，分数低于直接 EC 匹配",
                          operonScore < direct,
                          $"{operonScore:F4} 应小于 {direct:F4}")

        runner.AssertEqual("远离基因簇的 g_lonely 不产生任何关联",
                           0, ReactionsOf(assoc, "g_lonely").Length)

        runner.AssertEqual("尾部操纵子成员 g_tail2 同时拿到直接证据与操纵子上下文",
                           "P2_R1|P2_R2", Joined(ReactionsOf(assoc, "g_tail2")))
    End Sub

    ''' <summary>
    ''' 回归断言：历史实现在操纵子识别的循环结束之后没有 flush 最后一个操纵子，
    ''' 导致位于基因组尾部的操纵子永远不会被识别出来。
    ''' </summary>
    Private Sub CaseOperonEdge(runner As TestRunner)
        runner.BeginCase("3. 操纵子边界（回归：最后一个操纵子丢失）")

        Dim data As SyntheticCase = SyntheticData.Operon()
        Dim assoc As MetabolicAssociator = Compute(data)

        runner.AssertEqual("识别出的操纵子数量", 2, assoc.Operons.Count)

        Dim tailFound As Boolean = assoc.Operons.Any(
            Function(group)
                Return group.Count = 2 AndAlso
                       group.Contains(data.IndexOf("g_tail1")) AndAlso
                       group.Contains(data.IndexOf("g_tail2"))
            End Function)

        runner.AssertTrue("位于基因组末尾的操纵子 {g_tail1, g_tail2} 被正确识别", tailFound,
                          OperonDump(assoc, data))

        Dim headFound As Boolean = assoc.Operons.Any(
            Function(group)
                Return group.Count = 4 AndAlso group.Contains(data.IndexOf("g_op1"))
            End Function)

        runner.AssertTrue("位于基因组开头的操纵子 {g_op1..g_op4} 被正确识别", headFound,
                          OperonDump(assoc, data))
    End Sub

    Private Function OperonDump(assoc As MetabolicAssociator, data As SyntheticCase) As String
        Dim ordered As String() = data.Genes.OrderBy(Function(g) g.left).Select(Function(g) g.locus_id).ToArray()
        Dim out As New StringBuilder

        For Each group As List(Of Integer) In assoc.Operons
            Call out.Append($"[{String.Join(",", group.Select(Function(i) ordered(i)))}] ")
        Next

        Return $"实际识别结果 = {out}"
    End Function

#End Region

#Region "用例 4：距离衰减与链方向"

    Private Sub CaseDistanceDecay(runner As TestRunner)
        runner.BeginCase("4. 距离衰减与链方向权重")

        Dim data As SyntheticCase = SyntheticData.DistanceDecay()
        Dim assoc As MetabolicAssociator = Compute(data)
        Dim threshold As Double = New GPRParameters().ConfidenceThreshold

        Dim nearScore As Double = ScoreOf(assoc, "g_near", "P1_R1")
        Dim farScore As Double = ScoreOf(assoc, "g_far", "P1_R1")
        Dim crossScore As Double = ScoreOf(assoc, "g_cross", "P1_R1")

        runner.AssertTrue("近距离同链邻居产生关联", nearScore > 0, $"实际 {nearScore:F4}")
        runner.AssertTrue("邻居越远，上下文证据越弱",
                          nearScore > farScore, $"近 {nearScore:F4} 应大于 远 {farScore:F4}")
        runner.AssertTrue("异链邻居的证据强度被链方向权重削弱",
                          nearScore > crossScore, $"同链 {nearScore:F4} 应大于 异链 {crossScore:F4}")
        runner.AssertTrue("被削弱的异链证据应当低于置信度阈值而被过滤",
                          crossScore < threshold, $"实际 {crossScore:F4}，阈值 {threshold:F4}")
    End Sub

#End Region

#Region "用例 5：酶复合体"

    Private Sub CaseEnzymeComplex(runner As TestRunner)
        runner.BeginCase("5. 酶复合体检测")

        Dim data As SyntheticCase = SyntheticData.EnzymeComplex()
        Dim assoc As MetabolicAssociator = Compute(data)

        runner.AssertEqual("检测到的复合体数量", 1, assoc.Complexes.Count)
        runner.AssertEqual("复合体成员",
                           "g_cpx1|g_cpx2",
                           Joined(assoc.Complexes(0).Select(Function(g) g.locus_id)))

        Dim kinds As String() = KindsOf(assoc, "g_cpx1", "P1_R1")
        runner.AssertContains("g_cpx1 对 P1_R1 存在直接 EC 证据", kinds, "DirectEC")
        runner.AssertContains("g_cpx1 对 P1_R1 存在复合体证据", kinds, "EnzymeComplex")

        Dim score As Double = ScoreOf(assoc, "g_cpx1", "P1_R1")
        runner.AssertTrue("多条相互独立的证据把分数提升到直接匹配权重之上",
                          score > New GPRParameters().DirectMatchScore,
                          $"实际 {score:F4}")
    End Sub

#End Region

#Region "用例 6：融合基因与通路完整度"

    Private Sub CaseFusionAndCompleteness(runner As TestRunner)
        runner.BeginCase("6. 融合基因与通路完整度")

        Dim data As SyntheticCase = SyntheticData.FusionAndCompleteness()
        Dim assoc As MetabolicAssociator = Compute(data)
        Dim threshold As Double = New GPRParameters().ConfidenceThreshold

        ' 融合基因：多个 EC 对应的反应在同一条通路上连续
        runner.AssertContains("g_fusion 对 P1_R1 存在融合酶证据",
                              KindsOf(assoc, "g_fusion", "P1_R1"), "FusionGene")
        runner.AssertContains("g_fusion 对 P1_R3 存在融合酶证据",
                              KindsOf(assoc, "g_fusion", "P1_R3"), "FusionGene")

        ' 通路完整度补缺：覆盖率 3/4 = 0.75，缺口 P1_R4 与 P1_R3 直接相邻
        runner.AssertContains("g_fusion 被补齐通路中缺失的最后一个反应",
                              ReactionsOf(assoc, "g_fusion"), "P1_R4")
        runner.AssertEqual("补齐出来的反应只带通路完整度证据",
                           "PathwayCompleteness", Joined(KindsOf(assoc, "g_fusion", "P1_R4")))

        Dim filledScore As Double = ScoreOf(assoc, "g_fusion", "P1_R4")
        Dim directScore As Double = ScoreOf(assoc, "g_fusion", "P1_R1")

        runner.AssertTrue("补齐反应的分数高于置信度阈值", filledScore >= threshold, $"实际 {filledScore:F4}")
        runner.AssertTrue("补齐属于推断性证据，分数显著低于直接命中的反应",
                          filledScore < directScore, $"补齐 {filledScore:F4} 应小于 直接 {directScore:F4}")

        ' 跨通路的 EC 组合不构成融合酶
        runner.AssertNotContains("跨通路 EC 组合不产生融合酶证据",
                                 KindsOf(assoc, "g_split", "P1_R1"), "FusionGene")
        runner.AssertEqual("g_split 只关联两个自身 EC 命中的反应",
                           "P1_R1|P2_R1", Joined(ReactionsOf(assoc, "g_split")))
    End Sub

#End Region

#Region "用例 7：上下文作用域收敛（核心回归）"

    ''' <summary>
    ''' **本 Demo 中最关键的一条回归断言。**
    ''' 
    ''' 旧实现的做法是：只要邻居基因的某个 EC 命中了一个反应，就把该反应所在整条通路的全部反应
    ''' 都灌上分数。于是无 EC 的 g_context 会因为邻居携带 P1 主链的 EC，被一并关联到
    ''' 通路内与主链完全无关的诱饵反应上，结果表因此失去区分度。
    ''' </summary>
    Private Sub CaseNoiseConvergence(runner As TestRunner)
        runner.BeginCase("7. 上下文作用域收敛（核心回归：通路灌分）")

        Dim data As SyntheticCase = SyntheticData.NoiseConvergence()
        Dim assoc As MetabolicAssociator = Compute(data)

        Dim contextReactions As String() = ReactionsOf(assoc, "g_context")

        runner.AssertTrue("g_context 通过上下文获得了关联", contextReactions.Length > 0)
        runner.AssertNotContains("g_context 不会被关联到通路内与邻居 EC 无关的诱饵反应（旧实现会失败）",
                                 contextReactions, "P1_R_orphan")
        runner.AssertEqual("g_context 的关联范围严格收敛在邻居所携带的四个反应上",
                           "P1_R1|P1_R2|P1_R3|P1_R4", Joined(contextReactions))

        ' 诱饵反应唯一的携带者：只应该有直接证据
        runner.AssertEqual("g_orphan 只关联到诱饵反应", "P1_R_orphan", Joined(ReactionsOf(assoc, "g_orphan")))
        runner.AssertEqual("g_orphan 对诱饵反应的证据类型只有 DirectEC",
                           "DirectEC", Joined(KindsOf(assoc, "g_orphan", "P1_R_orphan")))

        ' 诱饵反应在整个结果中只能被它的唯一携带者关联到一次
        Dim orphanOwners As Integer = 0

        For Each gene As GeneAssociation In assoc.GenomeModel.MetabolicNetwork.Values
            If gene.Reactions.ContainsKey("P1_R_orphan") Then orphanOwners += 1
        Next

        runner.AssertEqual("诱饵反应在整个结果中只被唯一的携带者关联（旧实现会被多个邻居基因灌分）",
                           1, orphanOwners)
    End Sub

#End Region

#Region "用例 8：共表达"

    Private Sub CaseCoexpression(runner As TestRunner)
        runner.BeginCase("8. 共表达（回归：证据在空网络上运行）")

        Dim data As SyntheticCase = SyntheticData.Coexpression()

        ' 不提供表达数据时不应该凭空产生关联
        Dim without As MetabolicAssociator = Compute(data)
        runner.AssertEqual("没有表达数据时 g_expr2 不产生关联",
                           0, ReactionsOf(without, "g_expr2").Length)

        ' 提供表达数据之后共表达证据生效
        Dim withExpr As MetabolicAssociator = Compute(data, coexp:=New CoexpressionAnalyzer(SyntheticData.CoexpressionMatrix()))

        runner.AssertContains("提供表达数据后 g_expr2 通过共表达被关联到 P1_R1",
                              ReactionsOf(withExpr, "g_expr2"), "P1_R1")
        runner.AssertContains("g_expr2 对 P1_R1 的证据类型包含 Coexpression",
                              KindsOf(withExpr, "g_expr2", "P1_R1"), "Coexpression")
        runner.AssertNotContains("与 g_expr1 负相关的 g_expr3 不会被关联",
                                 ReactionsOf(withExpr, "g_expr3"), "P1_R1")

        Dim expected As Double = New GPRParameters().BaseCoexpressionScore * 1.0
        runner.AssertNear("共表达证据的分数 = 相关系数 × 权重",
                          expected, ScoreOf(withExpr, "g_expr2", "P1_R1"), 0.000001)
    End Sub

#End Region

#Region "用例 9：保守共线性"

    Private Sub CaseConservedSynteny(runner As TestRunner)
        runner.BeginCase("9. 保守共线性（回归：类型转换崩溃）")

        Dim data As SyntheticCase = SyntheticData.ConservedSynteny()
        Dim synteny As New ConservedSyntenyAnalyzer(SyntheticData.ConservedClusters())

        Dim assoc As MetabolicAssociator = Nothing

        ' 旧实现把 ConservedCluster 对象当作集合传给 Enumerable.Intersect，
        ' 在 Option Strict Off 下可以编译，但运行到这里必然抛出类型转换异常。
        runner.AssertNoThrow("保守共线性分析正常完成（旧实现会抛出 InvalidCastException）",
                             Sub() assoc = Compute(data, synteny:=synteny))

        If assoc Is Nothing Then Return

        runner.AssertContains("g_syn3 通过保守共线性被关联到 P1_R1",
                              ReactionsOf(assoc, "g_syn3"), "P1_R1")
        runner.AssertContains("证据类型包含 ConservedSynteny",
                              KindsOf(assoc, "g_syn3", "P1_R1"), "ConservedSynteny")
    End Sub

#End Region

#Region "用例 10：退化输入与健壮性"

    Private Sub CaseDegenerateInputs(runner As TestRunner)
        runner.BeginCase("11. 退化输入与健壮性")

        ' 空基因组 + 空通路
        Dim empty As SyntheticCase = SyntheticData.EmptyGenome()
        Dim emptyAssoc As MetabolicAssociator = Nothing

        runner.AssertNoThrow("空基因组与空通路不会抛出异常",
                             Sub() emptyAssoc = Compute(empty))
        runner.AssertTrue("空输入返回空结果",
                          emptyAssoc IsNot Nothing AndAlso emptyAssoc.GenomeModel.MetabolicNetwork.Count = 0)

        ' 空通路 / 没有 EC 与底物产物的退化反应 / 未映射 EC / 未排序基因组
        Dim data As SyntheticCase = SyntheticData.Degenerate()
        Dim assoc As MetabolicAssociator = Nothing

        runner.AssertNoThrow("包含空通路与退化反应的输入不会抛出异常",
                             Sub() assoc = Compute(data))

        If assoc Is Nothing Then Return

        runner.AssertEqual("未按坐标排序的基因组仍能正确关联",
                           "P1_R1", Joined(ReactionsOf(assoc, "g_out1")))
        runner.AssertEqual("EC 在参考网络中找不到反应的基因不产生关联",
                           0, ReactionsOf(assoc, "g_unmapped").Length)
        runner.AssertEqual("未映射的 EC 编号被单独记录，不会伪装成反应编号",
                           "R0.0.0.0",
                           Joined(assoc.GenomeModel.GetGeneAssociation("g_unmapped").UnmappedECNumbers))
        runner.AssertEqual("完全没有 EC 的基因不产生关联",
                           0, ReactionsOf(assoc, "g_emptyEC").Length)
    End Sub

#End Region

#Region "用例 12：综合演示与结果完整性"

    Private Function CaseDemo(runner As TestRunner) As (Associator As MetabolicAssociator, Data As SyntheticCase)
        runner.BeginCase("10. 综合演示与结果完整性")

        Dim data As SyntheticCase = SyntheticData.Demo()
        Dim assoc As MetabolicAssociator = Compute(data)
        Dim valid As New HashSet(Of String)(data.ReactionIds(), StringComparer.OrdinalIgnoreCase)

        Dim reactions As New List(Of String)
        Dim scores As New List(Of Double)
        Dim duplicated As New List(Of String)

        For Each association As GeneAssociation In assoc.GenomeModel.MetabolicNetwork.Values
            Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each reaction As ScoredReaction In association.Reactions.Values
                reactions.Add(reaction.Id)
                scores.Add(reaction.Score)

                If Not seen.Add(reaction.Id) Then
                    duplicated.Add($"{association.GeneId}:{reaction.Id}")
                End If
            Next
        Next

        runner.AssertTrue("综合演示用例产生了关联结果", reactions.Count > 0)

        Dim invalid As String() = reactions _
            .Where(Function(id) Not valid.Contains(id)) _
            .Distinct(StringComparer.OrdinalIgnoreCase) _
            .ToArray()

        runner.AssertEqual("结果中的反应编号全部是参考网络中的真实反应（没有伪反应编号）",
                           0, invalid.Length)
        runner.AssertEqual("同一个基因内不存在重复的反应条目", 0, duplicated.Count)
        runner.AssertEqual("全部打分都落在 [0, 1] 区间内",
                           0, scores.Where(Function(s) s < 0 OrElse s > 1).Count())
        runner.AssertEqual("每个基因都有对应的关联结果对象",
                           data.Genes.Length, assoc.GenomeModel.MetabolicNetwork.Count)

        runner.AssertTrue("反应反向索引可以反查到基因",
                          assoc.GenomeModel.GetGenesForReaction("P1_R1").Any())
        runner.AssertTrue("结果与 Genome.MetabolicNetwork 来自同一份数据",
                          assoc.GenomeModel.GetGeneAssociation("GMP014") IsNot Nothing)

        ' 可复现性
        Dim again As MetabolicAssociator = Compute(data)

        runner.AssertEqual("两次运行的完整结果签名完全一致（可复现）",
                           Signature(assoc), Signature(again))

        ' 通路覆盖情况
        Dim covered As Integer = data.Pathways _
            .Where(Function(p) reactions.Any(Function(id) p.metabolicNetwork.Any(Function(r) String.Equals(r.id, id, StringComparison.OrdinalIgnoreCase)))) _
            .Count()

        runner.AssertEqual("全部参考通路都至少被关联到了一次", data.Pathways.Length, covered)

        Return (assoc, data)
    End Function

#End Region

#Region "用例 13：规模与性能"

    Private Sub CaseStress(runner As TestRunner)
        runner.BeginCase("12. 规模与性能")

        Dim data As SyntheticCase = SyntheticData.Stress(600)
        Dim watch As Stopwatch = Stopwatch.StartNew()
        Dim assoc As MetabolicAssociator = Compute(data)
        watch.Stop()

        runner.AssertTrue("大规模输入正常完成", assoc IsNot Nothing)
        runner.AssertTrue(
            $"{data.Genes.Length} 基因 / {data.Pathways.Length} 通路 / {data.ReactionIds().Length} 反应的处理耗时在 30 秒以内（实际 {watch.ElapsedMilliseconds} ms）",
            watch.ElapsedMilliseconds < 30000)
        runner.AssertTrue("大规模输入下仍然产生了关联结果",
                          assoc.GenomeModel.MetabolicNetwork.Values.Any(Function(a) a.GPRLinks > 0))

        Dim density As Double = ReactionDensity(assoc, data)
        runner.AssertTrue($"大规模输入下的关联密度保持在低位（实际 {density:F4}）", density < 0.25)
    End Sub

#End Region

#Region "工具函数"

    Private Function Compute(data As SyntheticCase,
                             Optional opt As GPRParameters = Nothing,
                             Optional coexp As CoexpressionAnalyzer = Nothing,
                             Optional synteny As ConservedSyntenyAnalyzer = Nothing) As MetabolicAssociator

        Dim associator As New MetabolicAssociator(opt, data.Genes, data.Pathways, coexp, synteny)
        Call associator.AssociateGenesToReactions()

        Return associator
    End Function

    Private Function ReactionsOf(associator As MetabolicAssociator, geneId As String) As String()
        Dim association As GeneAssociation = associator.GenomeModel.GetGeneAssociation(geneId)
        If association Is Nothing Then Return New String() {}

        Return association.Reactions.Keys.OrderBy(Function(id) id, StringComparer.OrdinalIgnoreCase).ToArray()
    End Function

    Private Function ScoreOf(associator As MetabolicAssociator, geneId As String, reactionId As String) As Double
        Dim association As GeneAssociation = associator.GenomeModel.GetGeneAssociation(geneId)
        If association Is Nothing Then Return 0

        Dim reaction As ScoredReaction = Nothing
        If association.Reactions.TryGetValue(reactionId, reaction) Then Return reaction.Score

        Return 0
    End Function

    Private Function KindsOf(associator As MetabolicAssociator, geneId As String, reactionId As String) As String()
        Dim association As GeneAssociation = associator.GenomeModel.GetGeneAssociation(geneId)
        If association Is Nothing Then Return New String() {}

        Dim reaction As ScoredReaction = Nothing
        If Not association.Reactions.TryGetValue(reactionId, reaction) Then Return New String() {}

        Return reaction.EvidenceKinds
    End Function

    Private Function Joined(values As IEnumerable(Of String)) As String
        If values Is Nothing Then Return ""
        Return String.Join("|", values.OrderBy(Function(text) text, StringComparer.OrdinalIgnoreCase))
    End Function

    ''' <summary>
    ''' 关联密度 = 实际关联数 / (基因数 × 反应数)。
    ''' 
    ''' 这是一个直接衡量"结果是否具有区分度"的指标：如果算法把整条通路灌给每个基因，
    ''' 这个比值会迅速逼近 1。
    ''' </summary>
    Private Function ReactionDensity(associator As MetabolicAssociator, data As SyntheticCase) As Double
        Dim total As Integer = data.Genes.Length * data.ReactionIds().Length
        If total = 0 Then Return 0

        Dim links As Integer = associator.GenomeModel.MetabolicNetwork.Values.Sum(Function(a) a.GPRLinks)

        Return CDbl(links) / total
    End Function

    ''' <summary>
    ''' 生成整个结果的稳定签名，用于验证算法的可复现性
    ''' </summary>
    Private Function Signature(associator As MetabolicAssociator) As String
        Dim lines As New List(Of String)

        For Each geneId As String In associator.GenomeModel.MetabolicNetwork.Keys.OrderBy(Function(id) id, StringComparer.OrdinalIgnoreCase)
            Dim association As GeneAssociation = associator.GenomeModel.GetGeneAssociation(geneId)

            For Each reaction As ScoredReaction In association.Reactions.Values.OrderBy(Function(r) r.Id, StringComparer.OrdinalIgnoreCase)
                lines.Add($"{geneId}|{reaction.Id}|{reaction.Score:F6}|{reaction.EvidenceSummary}")
            Next
        Next

        Return String.Join(vbLf, lines)
    End Function

#End Region

End Module
