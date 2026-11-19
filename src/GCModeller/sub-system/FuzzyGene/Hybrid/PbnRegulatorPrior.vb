Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.DataMining.FuzzyLogic.BooleanNetworks
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.FuzzyGene.Data
Imports SMRUCC.genomics.BioSystem.Core
Imports std = System.Math

Namespace Hybrid

    ''' <summary>
    ''' 用概率布尔网络给 TSK 的调控因子候选提供**定性**证据（CoD）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 纯表达相关的候选筛选有一个结构性缺陷：它只看「谁和靶基因更相关」，
    ''' 于是会把**下游基因**选成上游调控因子（A→B 时 B 与 A 的相关性同样高），
    ''' 拟合出 <c>A = f(B)</c> 这种反函数。这种边在稳态迭代里是近单位增益的环，
    ''' 既破坏可解释性，也是谱半径超限的来源之一。
    ''' </para>
    ''' <para>
    ''' 布尔网络的一致性（CoD）给出的是完全不同的证据：它问「给定父节点取值，
    ''' 靶基因的取值是否被确定」。反函数在布尔化之后依然能拟合得很好，
    ''' 但**多父集**上的 CoD 增长会揭示「这条边是否真的携带方向信息」：
    ''' 只与 A 相关而与 A 的父集无关的基因，其 CoD 在多父集下不会增长。
    ''' 因此把 CoD 作为权重/门槛，能显著抑制纯相关边。
    ''' </para>
    ''' <para>
    ''' <b>规模闸门</b>：<c>InferenceHelper.Combinations</c> 会枚举全部组合，
    ''' 全基因组两两枚举是不可能的（<c>C(18532,3)</c>）。这里只对
    ''' 「先验 TF ∪ 方差最高的 <see cref="HybridOptions.PbnMaxGenes"/> 个基因」建网，
    ''' 每个靶基因只在「先验调控因子 + top-P 相关基因」这个小子集里枚举
    ''' <c>k ≤ PbnMaxParents</c> 的父集（默认每基因 ≤ 21 个子集）。
    ''' </para>
    ''' <para>
    ''' 训练行也会被等间隔抽样到 <see cref="MaxRows"/>：真值表拟合的成本是
    ''' <c>行数 × 子集数</c>，几万行对布尔一致性没有额外信息。
    ''' </para>
    ''' </remarks>
    Public Class PbnRegulatorPrior

        ''' <summary>真值表拟合使用的最大行数（等间隔确定性抽样）。</summary>
        Private Const MaxRows As Integer = 2048

        ''' <summary>每个靶基因保留的候选规则数（用于概率采样）。</summary>
        Private Const MaxRulesPerTarget As Integer = 3

        Private ReadOnly _cod As New Dictionary(Of String, Dictionary(Of String, Double))(StringComparer.OrdinalIgnoreCase)
        Private ReadOnly _rules As New Dictionary(Of String, List(Of PbnRule))(StringComparer.OrdinalIgnoreCase)
        Private ReadOnly _index As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
        Private ReadOnly _mode As PbnRegularizationMode

        ''' <summary>参与建模的基因（顺序与二值化矩阵一致）。</summary>
        Public Property GeneNames As String()

        ''' <summary>成功分析了父集的靶基因数。</summary>
        Public Property TargetsAnalysed As Integer

        ''' <summary>被规模闸门跳过的基因数。</summary>
        Public Property Skipped As Integer

        ''' <summary>建模过程中的告警。</summary>
        Public Property Warnings As String()

        Private Sub New(mode As PbnRegularizationMode)
            _mode = mode
        End Sub

        ''' <summary>PBN 对该靶基因是否有可用结论。</summary>
        Public Function HasTarget(targetGene As String) As Boolean
            Return Not String.IsNullOrEmpty(targetGene) AndAlso _cod.ContainsKey(targetGene)
        End Function

        ''' <summary>
        ''' <paramref name="regulator"/> 对 <paramref name="targetGene"/> 的 CoD（0~1）。
        ''' </summary>
        ''' <remarks>
        ''' 取「包含该调控因子的所有父集中最大的 CoD」：父集越大 CoD 通常越高，
        ''' 这会让热门调控因子略微占优；对「过滤纯相关边」这个用途来说方向是对的。
        ''' </remarks>
        Public Function CoDOf(targetGene As String, regulator As String) As Double
            Dim table As Dictionary(Of String, Double) = Nothing

            If String.IsNullOrEmpty(targetGene) OrElse String.IsNullOrEmpty(regulator) Then
                Return 0.0
            End If
            If Not _cod.TryGetValue(targetGene, table) Then
                Return 0.0
            End If

            Dim value As Double

            If table.TryGetValue(regulator, value) Then
                Return value
            End If

            Return 0.0
        End Function

        ''' <summary>
        ''' 软加权用的权重：CoD 归一化到 <c>[0, 1]</c>，再乘以配置的 <c>PbnWeight</c>。
        ''' </summary>
        Public Function PriorWeight(targetGene As String, regulator As String, weight As Double) As Double
            Return std.Max(0.0, weight) * CoDOf(targetGene, regulator)
        End Function

        ''' <summary>基因在 PBN 状态向量里的下标；不在 PBN 建模范围内时返回 <c>-1</c>。</summary>
        Public Function GeneIndexOf(geneName As String) As Integer
            Dim idx As Integer

            If Not String.IsNullOrEmpty(geneName) AndAlso _index.TryGetValue(geneName, idx) Then
                Return idx
            End If

            Return -1
        End Function

        ''' <summary>该靶基因的候选规则（按 CoD 降序）；没有可用规则时返回空列表。</summary>
        Public Function RulesOf(targetGene As String) As List(Of PbnRule)
            Dim rules As List(Of PbnRule) = Nothing

            If String.IsNullOrEmpty(targetGene) OrElse Not _rules.TryGetValue(targetGene, rules) Then
                Return New List(Of PbnRule)()
            End If

            Return rules
        End Function

        ''' <summary>
        ''' 按 CoD 概率为每个基因抽一条规则，得到一个确定性布尔网络实例。
        ''' </summary>
        ''' <param name="random">随机源（调用方负责种子，保证可复现）</param>
        ''' <remarks>
        ''' 这就是概率布尔网络的原始语义：规则集固定、每次更新按概率选一条。
        ''' 这里把「一次网络更新」整体作为一个样本，于是每个样本内部是确定性网络，
        ''' 样本之间才体现不确定性——与 <c>PbnInference</c> 的分组方式一致。
        ''' </remarks>
        Public Function SampleNetwork(random As Random) As Dictionary(Of String, PbnRule)
            Dim chosen As New Dictionary(Of String, PbnRule)(StringComparer.OrdinalIgnoreCase)

            For Each item As KeyValuePair(Of String, List(Of PbnRule)) In _rules
                Dim rules As List(Of PbnRule) = item.Value
                Dim total As Double = 0.0

                For i As Integer = 0 To rules.Count - 1
                    total += std.Max(0.0, rules(i).Cod)
                Next

                Dim pick As PbnRule = rules(0)

                If total > 0.0 Then
                    Dim threshold As Double = random.NextDouble() * total
                    Dim accumulated As Double = 0.0

                    For i As Integer = 0 To rules.Count - 1
                        accumulated += std.Max(0.0, rules(i).Cod)

                        If threshold <= accumulated Then
                            pick = rules(i)

                            Exit For
                        End If
                    Next
                End If

                chosen(item.Key) = pick
            Next

            Return chosen
        End Function

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"PBN prior ({_mode}): {TargetsAnalysed} target gene(s) analysed, {Skipped} skipped, " &
                   $"{GeneNames?.Length} gene(s) in the boolean network"
        End Function

        ''' <summary>
        ''' 从表达视图构建 PBN 定性先验。
        ''' </summary>
        ''' <param name="view">预处理视图</param>
        ''' <param name="config">建模配置</param>
        ''' <param name="prior">先验调控网络（其 TF 无条件进入候选池）</param>
        ''' <returns>先验对象；未启用 PBN 时返回 <c>Nothing</c></returns>
        Public Shared Function Build(view As GeneExpressionView,
                                     config As FuzzyGeneConfig,
                                     Optional prior As PriorNetwork = Nothing) As PbnRegulatorPrior

            If view Is Nothing Then
                Throw New ArgumentNullException(NameOf(view))
            End If

            Dim cfg As FuzzyGeneConfig = If(config, New FuzzyGeneConfig())
            Dim options As HybridOptions = cfg.Hybrid

            If options.PbnRegularization = PbnRegularizationMode.None Then
                Return Nothing
            End If

            Dim limit As Integer = options.PbnMaxGenes

            If limit > 0 AndAlso view.NGene > limit Then
                ' 只对「先验 TF ∪ 高方差基因」建网：低方差基因的布尔化基本是噪声
                Dim keep As New SortedSet(Of Integer)

                For i As Integer = 0 To view.NGene - 1
                    If prior IsNot Nothing AndAlso prior.TFNames IsNot Nothing AndAlso prior.TFNames.Contains(view.GeneNames(i)) Then
                        Call keep.Add(i)
                    End If
                Next

                Dim order As Integer() = Enumerable.Range(0, view.NGene) _
                    .OrderByDescending(Function(i) view.SDs(i)) _
                    .ToArray()

                For Each idx As Integer In order
                    If keep.Count >= limit Then
                        Exit For
                    End If

                    Call keep.Add(idx)
                Next

                Return BuildCore(view, cfg, prior, keep.ToArray(), direct:=False)
            End If

            Dim all As Integer() = Enumerable.Range(0, view.NGene).ToArray()

            Return BuildCore(view, cfg, prior, all, direct:=True)
        End Function

        ''' <summary>
        ''' 在给定的基因子集上建网。
        ''' </summary>
        ''' <param name="direct">
        ''' <c>True</c> 表示 <paramref name="genes"/> 就是全部基因（可以省一次下标映射）
        ''' </param>
        Private Shared Function BuildCore(view As GeneExpressionView,
                                          cfg As FuzzyGeneConfig,
                                          prior As PriorNetwork,
                                          genes As Integer(),
                                          direct As Boolean) As PbnRegulatorPrior
            Dim options As HybridOptions = cfg.Hybrid
            Dim result As New PbnRegulatorPrior(options.PbnRegularization)
            Dim warnings As New List(Of String)
            Dim geneCount As Integer = genes.Length

            ' ---- 1. 行抽样 + 二值化（列优先，按基因扫描连续内存）----
            Dim rows As Integer() = SampleRows(view.NRows, MaxRows)
            Dim sampleCount As Integer = rows.Length
            Dim values(sampleCount - 1, geneCount - 1) As Boolean?
            Dim names(geneCount - 1) As String

            For c As Integer = 0 To geneCount - 1
                Dim column As Double() = view.Columns(genes(c))
                Dim threshold As Double = Quantile(column, rows, options.BinarizeQuantile)

                names(c) = view.GeneNames(genes(c))

                For r As Integer = 0 To sampleCount - 1
                    values(r, c) = column(rows(r)) > threshold
                Next
            Next

            Dim data As New BooleanExpressionData(names, values)

            ' ---- 2. 逐靶基因枚举小子集 ----
            Dim compact As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For c As Integer = 0 To geneCount - 1
                compact(names(c)) = c
                result._index(names(c)) = c
            Next

            Dim analysed As Integer = 0
            Dim skipped As Integer = 0

            For c As Integer = 0 To geneCount - 1
                Dim target As String = names(c)
                Dim candidates As List(Of Integer) = CandidateParents(view, prior, genes, compact, c, options)

                If candidates.Count = 0 Then
                    skipped += 1

                    Continue For
                End If

                Dim baselineCount As Integer = 0
                Dim baselineErrors As Integer = 0

                Call InferenceHelper.Baseline(data, c, baselineCount, baselineErrors)

                Dim table As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
                Dim rules As New List(Of PbnRule)
                Dim subsets As List(Of Integer()) = EnumerateSubsets(candidates, options.PbnMaxParents)

                For Each subset As Integer() In subsets
                    Dim fitted As FittedRule = InferenceHelper.FitRule(data, c, subset)

                    If fitted Is Nothing OrElse fitted.UsableSamples = 0 Then
                        Continue For
                    End If

                    Dim cod As Double = InferenceHelper.ComputeCod(baselineErrors, fitted.Errors, fitted.UsableSamples)

                    ' 空父集没有"调控"含义，只作为基线，不进入规则池
                    If subset.Length > 0 AndAlso cod >= options.PbnMinCod Then
                        Call rules.Add(New PbnRule With {
                            .Target = target,
                            .Parents = subset,
                            .Truth = fitted.TruthTable,
                            .Cod = cod
                        })
                    End If

                    For j As Integer = 0 To subset.Length - 1
                        Dim regulator As String = names(subset(j))
                        Dim previous As Double

                        If Not table.TryGetValue(regulator, previous) OrElse cod > previous Then
                            table(regulator) = cod
                        End If
                    Next
                Next

                If table.Count > 0 Then
                    result._cod(target) = table
                    analysed += 1
                Else
                    skipped += 1
                End If

                If rules.Count > 0 Then
                    ' 只保留 CoD 最高的几条：采样时按 CoD 加权，等价于「更可信的规则更常被抽到」
                    rules.Sort(Function(a, b) b.Cod.CompareTo(a.Cod))

                    If rules.Count > MaxRulesPerTarget Then
                        Call rules.RemoveRange(MaxRulesPerTarget, rules.Count - MaxRulesPerTarget)
                    End If

                    result._rules(target) = rules
                End If
            Next

            result.GeneNames = names
            result.TargetsAnalysed = analysed
            result.Skipped = skipped

            If Not direct Then
                Call warnings.Add(
                    $"PBN was built on {geneCount}/{view.NGene} genes (PbnMaxGenes = {options.PbnMaxGenes}): " &
                    "the rest keep the pure correlation screening")
            End If

            result.Warnings = warnings.ToArray()

            Return result
        End Function

        ''' <summary>
        ''' 构建某个靶基因的候选父集：先验调控因子 ∪ top-P 相关基因。
        ''' </summary>
        Private Shared Function CandidateParents(view As GeneExpressionView,
                                                 prior As PriorNetwork,
                                                 genes As Integer(),
                                                 compact As Dictionary(Of String, Integer),
                                                 localTarget As Integer,
                                                 options As HybridOptions) As List(Of Integer)
            Dim result As New List(Of Integer)
            Dim taken As New HashSet(Of Integer)
            Dim targetGene As String = view.GeneNames(genes(localTarget))

            If prior IsNot Nothing Then
                Dim edges As List(Of RegulatoryEdge) = prior.GetRegulators(targetGene)

                If edges IsNot Nothing Then
                    For Each edge As RegulatoryEdge In edges
                        Dim idx As Integer

                        If compact.TryGetValue(edge.TF, idx) AndAlso idx <> localTarget AndAlso taken.Add(idx) Then
                            Call result.Add(idx)
                        End If
                    Next
                End If
            End If

            Dim scored As New List(Of (index As Integer, strength As Double))
            Dim source As Integer = genes(localTarget)
            Dim limit As Integer = options.PbnCandidateSize

            If limit > 0 Then
                For c As Integer = 0 To genes.Length - 1
                    If c = localTarget OrElse taken.Contains(c) OrElse view.Constant(genes(c)) Then
                        Continue For
                    End If

                    Dim r As Double = RegulatorSelector.Pearson(view, source, genes(c))

                    Call scored.Add((c, std.Abs(r)))
                Next

                scored.Sort(Function(a, b) b.strength.CompareTo(a.strength))

                For k As Integer = 0 To std.Min(limit, scored.Count) - 1
                    Dim idx As Integer = scored(k).index

                    If taken.Add(idx) Then
                        Call result.Add(idx)
                    End If
                Next
            End If

            ' 父集上限也限制了「有意义的枚举规模」：候选太多时截断
            Dim cap As Integer = std.Max(1, options.PbnCandidateSize + options.PbnMaxParents)

            If result.Count > cap Then
                Call result.RemoveRange(cap, result.Count - cap)
            End If

            Return result
        End Function

        ''' <summary>枚举大小 <c>0..maxSize</c> 的全部子集。</summary>
        Private Shared Function EnumerateSubsets(pool As List(Of Integer), maxSize As Integer) As List(Of Integer())
            Dim result As New List(Of Integer())()
            Dim size As Integer = std.Min(std.Max(0, maxSize), pool.Count)

            For k As Integer = 0 To size
                ' 循环变量不能直接被 lambda 捕获（BC42324）：复制到局部量再闭包
                Dim subsetSize As Integer = k
                Dim current(k - 1) As Integer

                Dim build As Action(Of Integer, Integer) = Nothing

                build = Sub(start As Integer, depth As Integer)
                            If depth = subsetSize Then
                                Call result.Add(CType(current.Clone(), Integer()))

                                Return
                            End If

                            For i As Integer = start To pool.Count - (subsetSize - depth)
                                current(depth) = pool(i)
                                Call build(i + 1, depth + 1)
                            Next
                        End Sub

                Call build(0, 0)
            Next

            Return result
        End Function

        ''' <summary>等间隔确定性行采样（与预处理的行采样同一套约定：可复现）。</summary>
        Private Shared Function SampleRows(rows As Integer, maxRows As Integer) As Integer()
            If maxRows <= 0 OrElse rows <= maxRows Then
                Return Enumerable.Range(0, rows).ToArray()
            End If

            Dim index(maxRows - 1) As Integer
            Dim stride As Double = rows / CDbl(maxRows)

            For k As Integer = 0 To maxRows - 1
                Dim value As Integer = CInt(std.Floor(k * stride))

                If value >= rows Then value = rows - 1

                index(k) = value
            Next

            Return index
        End Function

        ''' <summary>列在抽样行上的分位点（布尔化阈值）。</summary>
        Private Shared Function Quantile(column As Double(), rows As Integer(), ratio As Double) As Double
            Dim values(rows.Length - 1) As Double

            For i As Integer = 0 To rows.Length - 1
                values(i) = column(rows(i))
            Next

            Call System.Array.Sort(values)

            Dim pos As Integer = CInt(std.Floor(ratio * (values.Length - 1)))

            If pos < 0 Then pos = 0
            If pos >= values.Length Then pos = values.Length - 1

            Return values(pos)
        End Function

        ''' <summary>导出为传输对象（随模型包一起持久化）。</summary>
        Public Function ToDto() As PbnPriorDto
            Dim rules As New List(Of PbnRuleDto)

            For Each item As KeyValuePair(Of String, List(Of PbnRule)) In _rules
                For Each rule As PbnRule In item.Value
                    Call rules.Add(New PbnRuleDto With {
                        .target = rule.Target,
                        .parents = CType(rule.Parents.Clone(), Integer()),
                        .truth = CType(rule.Truth.Clone(), Boolean()),
                        .cod = rule.Cod
                    })
                Next
            Next

            Return New PbnPriorDto With {
                .mode = _mode,
                .geneNames = GeneNames,
                .targetsAnalysed = TargetsAnalysed,
                .skipped = Skipped,
                .warnings = Warnings,
                .rules = rules.ToArray()
            }
        End Function

        ''' <summary>
        ''' 由传输对象还原（模型包读取路径）。
        ''' </summary>
        ''' <remarks>
        ''' <c>_cod</c> 表不随包保存，而是由规则重建：同一个调控因子出现在多条规则里时取最大的 CoD。
        ''' 这与拟合期「按候选子集累计」的口径略有差异（拟合期保留的是每条子集的 CoD），
        ''' 但 <see cref="CoDOf"/> 的用途是排序与门槛，取最大值是最保守的选择。
        ''' </remarks>
        Public Shared Function FromDto(dto As PbnPriorDto) As PbnRegulatorPrior
            If dto Is Nothing OrElse dto.geneNames Is Nothing Then
                Return Nothing
            End If

            Dim result As New PbnRegulatorPrior(dto.mode) With {
                .GeneNames = dto.geneNames,
                .TargetsAnalysed = dto.targetsAnalysed,
                .Skipped = dto.skipped,
                .Warnings = dto.warnings
            }

            For c As Integer = 0 To dto.geneNames.Length - 1
                result._index(dto.geneNames(c)) = c
            Next

            If dto.rules Is Nothing Then
                Return result
            End If

            For Each item As PbnRuleDto In dto.rules
                If String.IsNullOrEmpty(item.target) Then
                    Continue For
                End If

                Dim bucket As List(Of PbnRule) = Nothing

                If Not result._rules.TryGetValue(item.target, bucket) Then
                    bucket = New List(Of PbnRule)
                    result._rules(item.target) = bucket
                End If

                Call bucket.Add(New PbnRule With {
                    .Target = item.target,
                    .Parents = item.parents,
                    .Truth = item.truth,
                    .Cod = item.cod
                })

                Dim table As Dictionary(Of String, Double) = Nothing

                If Not result._cod.TryGetValue(item.target, table) Then
                    table = New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
                    result._cod(item.target) = table
                End If

                If item.parents Is Nothing Then
                    Continue For
                End If

                For Each parent As Integer In item.parents
                    If parent >= 0 AndAlso parent < dto.geneNames.Length Then
                        Dim name As String = dto.geneNames(parent)
                        Dim previous As Double = 0.0

                        Call table.TryGetValue(name, previous)

                        If item.cod > previous Then
                            table(name) = item.cod
                        End If
                    End If
                Next
            Next

            Return result
        End Function

    End Class

    ''' <summary>
    ''' 一条布尔规则：父节点集合 + 真值表 + 一致性（CoD）。
    ''' </summary>
    ''' <remarks>
    ''' 父节点用 PBN 内部的下标（<see cref="PbnRegulatorPrior.GeneIndexOf"/> 给出），
    ''' 真值表的下标是「父节点位模式」：第 j 个父节点为真时置位 <c>1 &lt;&lt; j</c>。
    ''' </remarks>
    Public Class PbnRule

        ''' <summary>靶基因名。</summary>
        Public Property Target As String

        ''' <summary>父节点下标（PBN 内部下标）。</summary>
        Public Property Parents As Integer()

        ''' <summary>真值表（长度 = <c>2^Parents.Length</c>）。</summary>
        Public Property Truth As Boolean()

        ''' <summary>该规则在训练数据上的一致性（0~1）。</summary>
        Public Property Cod As Double

        ''' <summary>按父节点取值求值。</summary>
        ''' <param name="state">PBN 状态向量</param>
        Public Function Evaluate(state As Boolean()) As Boolean
            Dim index As Integer = 0

            For j As Integer = 0 To Parents.Length - 1
                If state(Parents(j)) Then
                    index = index Or (1 << j)
                End If
            Next

            Return Truth(index)
        End Function

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"{Target} <- {Parents.Length} parent(s), CoD={Cod:G3}"
        End Function

    End Class

    ''' <summary>PBN 定性层的传输对象（随模型包一起持久化）。</summary>
    ''' <remarks>
    ''' <para>
    ''' 之所以要持久化：PBN 是「拟合期从表达矩阵二值化得到的规则」，推理期没有表达矩阵，
    ''' 无法重建。而生产流程恰恰是「训练一次 → 之后反复做虚拟扰动」（甚至在另一台机器上），
    ''' 所以不存进包里的话，<c>fuzzygene_ensemble</c> 只能在训练进程内使用。
    ''' </para>
    ''' <para>
    ''' 只存规则（target / parents / truth / cod）：<c>_cod</c> 表可以由规则重建，
    ''' 重复存一份会让 manifest 大一倍。
    ''' </para>
    ''' </remarks>
    Public Class PbnPriorDto

        ''' <summary>格式版本。</summary>
        Public Property formatVersion As String = "1.0"

        ''' <summary>正则化方式（Filter / Weighted；None 不会产生该对象）。</summary>
        Public Property mode As PbnRegularizationMode

        ''' <summary>PBN 内部基因名（顺序即 <c>Parents</c> 的下标空间）。</summary>
        Public Property geneNames As String()

        ''' <summary>成功分析了父集的靶基因数。</summary>
        Public Property targetsAnalysed As Integer

        ''' <summary>被规模闸门跳过的基因数。</summary>
        Public Property skipped As Integer

        ''' <summary>建模过程中的告警。</summary>
        Public Property warnings As String()

        ''' <summary>逐靶基因的候选规则。</summary>
        Public Property rules As PbnRuleDto()

    End Class

    ''' <summary>单条布尔规则的传输对象。</summary>
    Public Class PbnRuleDto

        ''' <summary>靶基因名。</summary>
        Public Property target As String
        ''' <summary>父节点下标（PBN 内部下标）。</summary>
        Public Property parents As Integer()
        ''' <summary>真值表（长度 = <c>2^Parents.Length</c>）。</summary>
        Public Property truth As Boolean()
        ''' <summary>一致性。</summary>
        Public Property cod As Double

    End Class

End Namespace
