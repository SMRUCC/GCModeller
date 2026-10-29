Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports std = System.Math

Namespace Data

    ''' <summary>
    ''' 一个候选调控因子（ANFIS 的一个输入变量）。
    ''' </summary>
    Public Class RegulatorPick

        ''' <summary>调控因子基因名。</summary>
        Public Property Gene As String

        ''' <summary>该因子是否来自先验调控网络。</summary>
        Public Property IsPrior As Boolean

        ''' <summary>先验置信度（<see cref="IsPrior"/> 为 False 时为 0）。</summary>
        Public Property PriorConfidence As Double

        ''' <summary>与靶基因表达的 Pearson 相关系数（先验因子也可能被补齐步骤顺带计算）。</summary>
        Public Property Correlation As Double

        ''' <summary>边的置信度：先验优先取先验置信度，否则取相关性的绝对值。</summary>
        Public ReadOnly Property Confidence As Double
            Get
                If IsPrior AndAlso PriorConfidence > 0.0 Then
                    Return PriorConfidence
                Else
                    Return std.Abs(Correlation)
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{Gene} (prior={IsPrior}, conf={Confidence:G3}, r={Correlation:G3})"
        End Function

    End Class

    ''' <summary>
    ''' 候选调控因子筛选：先验网络优先，相关性补齐。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 每个靶基因的输入维数必须压到 3–6 个（<see cref="FuzzyGeneConfig.MaxRegulators"/>）：
    ''' 网格划分的规则数是 <c>m^n</c>，否则参数规模会迅速超过样本量。
    ''' </para>
    ''' <para>
    ''' 选择顺序遵循「先验提供因果方向、数据提供相关性证据」：
    ''' 先按先验置信度降序取 TF，再用与靶基因 |Pearson| 最强的基因补齐；
    ''' 相关性在 <see cref="GeneExpressionView.Columns"/>（基因优先的连续列）上顺序计算，
    ''' 因此每个候选对的成本是一次长度为行数的顺序扫描，而不是把整张矩阵拖过缓存。
    ''' </para>
    ''' <para>
    ''' 全基因组规模下，逐靶基因扫描全部基因是 <c>O(n²·m)</c>，
    ''' 所以可以由 <see cref="FuzzyGeneConfig.MaxCandidateGenes"/> 指定候选池
    ''' （先验 TF ∪ 方差最高的若干基因），见 <see cref="BuildCandidatePool"/>。
    ''' </para>
    ''' </remarks>
    Public Module RegulatorSelector

        ''' <summary>
        ''' 构建相关性候选池：先验调控网络的 TF 无条件入池，其余按原始表达的标准差降序补齐。
        ''' </summary>
        ''' <param name="view">预处理视图</param>
        ''' <param name="prior">先验调控网络（可为 Nothing）</param>
        ''' <param name="config">建模配置</param>
        ''' <returns>候选基因下标；<c>Nothing</c> 表示不限制（使用全部基因）</returns>
        ''' <remarks>
        ''' 之所以用**原始标准差**排序而不是 z 空间的方差：标准化的目的就是让 z 空间的方差
        ''' 全部等于 1，那个数字无法区分基因；<see cref="GeneExpressionView.SDs"/> 保留的是原始尺度的离散度。
        ''' </remarks>
        Public Function BuildCandidatePool(view As GeneExpressionView,
                                           prior As PriorNetwork,
                                           config As FuzzyGeneConfig) As Integer()

            If view Is Nothing Then
                Throw New ArgumentNullException(NameOf(view))
            End If

            Dim cfg As FuzzyGeneConfig = If(config, New FuzzyGeneConfig())
            Dim limit As Integer = cfg.MaxCandidateGenes

            If limit <= 0 OrElse view.NGene <= limit Then
                Return Nothing
            End If

            Dim keep As New HashSet(Of Integer)

            ' 先验 TF 无条件入池：它们承载因果方向，被方差过滤掉就失去先验的意义
            If prior IsNot Nothing AndAlso prior.TFNames IsNot Nothing Then
                For i As Integer = 0 To view.NGene - 1
                    If prior.TFNames.Contains(view.GeneNames(i)) Then
                        Call keep.Add(i)
                    End If
                Next
            End If

            If keep.Count >= limit Then
                Return keep.Take(limit).ToArray()
            End If

            Dim order As Integer() = Enumerable _
                .Range(0, view.NGene) _
                .OrderByDescending(Function(i) view.SDs(i)) _
                .ToArray()

            For Each idx As Integer In order
                If keep.Count >= limit Then
                    Exit For
                End If

                Call keep.Add(idx)
            Next

            Return keep.ToArray()
        End Function

        ''' <summary>
        ''' 为指定靶基因挑选候选调控因子。
        ''' </summary>
        ''' <param name="view">预处理视图</param>
        ''' <param name="prior">先验调控网络（可为 Nothing）</param>
        ''' <param name="config">建模配置</param>
        ''' <param name="targetIndex">靶基因在 <paramref name="view"/> 中的下标</param>
        ''' <param name="pool">
        ''' 相关性候选池（见 <see cref="BuildCandidatePool"/>）；<c>Nothing</c> 表示扫描全部基因
        ''' </param>
        ''' <param name="limit">
        ''' 实际可用的输入槽位数（通常由 <c>GeneNetworkFitter.ResolveStructure</c> 按规则预算算出的 K）；
        ''' <c>0</c> 表示使用 <see cref="FuzzyGeneConfig.MaxRegulators"/>
        ''' </param>
        ''' <returns>按输入顺序排列的候选因子；空数组表示该基因无法建模</returns>
        ''' <remarks>
        ''' <para>
        ''' 当 <see cref="FuzzyGeneConfig.NoveltyQuota"/> &gt; 0 时，会先为「不在该靶基因先验边里」的
        ''' 相关性候选预留 <c>round(limit × quota)</c> 个槽位，再让先验 TF 与其余相关性候选去填剩下的位置。
        ''' 这是让模型**从数据里发现新调控关系**的关键：默认的「先验优先 + 尾部截断」会让数据侧候选永远进不来。
        ''' </para>
        ''' </remarks>
        Public Function SelectRegulators(view As GeneExpressionView,
                                         prior As PriorNetwork,
                                         config As FuzzyGeneConfig,
                                         targetIndex As Integer,
                                         Optional pool As Integer() = Nothing,
                                         Optional limit As Integer = 0) As RegulatorPick()

            If view Is Nothing Then
                Throw New ArgumentNullException(NameOf(view))
            End If

            Dim cfg As FuzzyGeneConfig = If(config, New FuzzyGeneConfig())
            Dim targetGene As String = view.GeneNames(targetIndex)
            Dim slots As Integer = If(limit > 0, std.Min(limit, cfg.MaxRegulators), cfg.MaxRegulators)
            Dim priorPicks As New List(Of RegulatorPick)
            Dim taken As New HashSet(Of Integer)

            ' ---- 1. 先验候选（按置信度降序） ----
            If cfg.Selection <> RegulatorSelection.CorrelationOnly AndAlso prior IsNot Nothing Then
                Dim edges As List(Of RegulatoryEdge) = prior.GetRegulators(targetGene)

                If edges IsNot Nothing Then
                    For Each edge As RegulatoryEdge In edges.OrderByDescending(Function(e) e.Confidence)
                        Dim idx As Integer = view.IndexOf(edge.TF)

                        If idx < 0 OrElse idx = targetIndex Then
                            Continue For
                        End If
                        If view.Constant(idx) Then
                            Continue For
                        End If
                        If Not taken.Add(idx) Then
                            Continue For
                        End If

                        Call priorPicks.Add(New RegulatorPick With {
                            .Gene = view.GeneNames(idx),
                            .IsPrior = True,
                            .PriorConfidence = edge.Confidence,
                            .Correlation = Pearson(view, targetIndex, idx)
                        })

                        If priorPicks.Count >= slots Then
                            Exit For
                        End If
                    Next
                End If
            End If

            ' ---- 2. 相关性排名 ----
            ' 只有"还缺槽位"或"留了 novelty 配额"时才扫描候选：
            ' 否则（先验已经填满且 quota=0）这一步纯属浪费——全量数据下每个基因要扫 1.8 万列。
            Dim needData As Boolean = cfg.Selection <> RegulatorSelection.PriorOnly AndAlso
                                      (priorPicks.Count < slots OrElse cfg.NoveltyQuota > 0.0)

            Dim dataPicks As New List(Of RegulatorPick)

            If needData Then
                Dim scored As New List(Of (index As Integer, correlation As Double))
                Dim strong As New List(Of (index As Integer, correlation As Double))
                Dim bestAbs As Double = 0.0

                For Each j As Integer In CandidateIndices(view, pool)
                    If j = targetIndex OrElse view.Constant(j) OrElse taken.Contains(j) Then
                        Continue For
                    End If

                    Dim r As Double = Pearson(view, targetIndex, j)
                    Dim strength As Double = std.Abs(r)

                    Call scored.Add((j, r))

                    If strength > bestAbs Then
                        bestAbs = strength
                    End If
                    If strength >= cfg.MinCorrelation Then
                        Call strong.Add((j, r))
                    End If
                Next

                ' 有效性门槛：既没有先验入边、最佳相关性又低于门槛 → 该靶基因没有可解释信号，
                ' 拟合出来的 R² 也会接近 0，直接跳过以节省算力
                If priorPicks.Count = 0 AndAlso cfg.MinBestCorrelation > 0.0 AndAlso bestAbs < cfg.MinBestCorrelation Then
                    Return New RegulatorPick() {}
                End If

                ' MinCorrelation 是“偏好”而不是硬门槛：单细胞/元细胞表达矩阵里大量低丰度基因
                ' 的 |r| 普遍偏低，若把它当硬门槛，这些基因会整条被丢掉。因此当没有任何候选达到
                ' 阈值时，退化为「取相关性最强的 K 个」，只在相关性数值上确实为 0 时才放弃。
                Dim candidates As List(Of (index As Integer, correlation As Double))

                If strong.Count > 0 Then
                    candidates = strong
                Else
                    candidates = scored.Where(Function(x) std.Abs(x.correlation) > 0.000001).ToList()
                End If

                candidates.Sort(Function(a, b) std.Abs(b.correlation).CompareTo(std.Abs(a.correlation)))

                For Each item In candidates
                    If dataPicks.Count >= slots Then
                        Exit For
                    End If

                    Call dataPicks.Add(New RegulatorPick With {
                        .Gene = view.GeneNames(item.index),
                        .IsPrior = False,
                        .PriorConfidence = 0.0,
                        .Correlation = item.correlation
                    })
                Next
            End If

            ' ---- 3. 组装：novelty 配额 > 0 时先给数据候选留固定槽位 ----
            Dim reserved As Integer = 0

            If cfg.NoveltyQuota > 0.0 AndAlso slots > 1 AndAlso dataPicks.Count > 0 AndAlso
               cfg.Selection <> RegulatorSelection.PriorOnly Then

                reserved = CInt(std.Round(slots * cfg.NoveltyQuota))
                reserved = std.Max(0, std.Min(reserved, slots - 1))
                reserved = std.Min(reserved, dataPicks.Count)
            End If

            Dim picks As New List(Of RegulatorPick)

            ' 先放预留的数据发现槽位
            For i As Integer = 0 To reserved - 1
                Call picks.Add(dataPicks(i))
            Next
            ' 再用先验填（保持"先验提供因果方向"的原有优先级）
            For Each item As RegulatorPick In priorPicks
                If picks.Count >= slots Then
                    Exit For
                End If

                Call picks.Add(item)
            Next
            ' 剩下的位置继续用相关性候选补齐
            For i As Integer = reserved To dataPicks.Count - 1
                If picks.Count >= slots Then
                    Exit For
                End If

                Call picks.Add(dataPicks(i))
            Next

            Return picks.ToArray()
        End Function

        Private Iterator Function CandidateIndices(view As GeneExpressionView, pool As Integer()) As IEnumerable(Of Integer)
            If pool Is Nothing Then
                For i As Integer = 0 To view.NGene - 1
                    Yield i
                Next
            Else
                For Each i As Integer In pool
                    Yield i
                Next
            End If
        End Function

        ''' <summary>
        ''' 两个基因在训练列上的 Pearson 相关系数。
        ''' </summary>
        ''' <remarks>
        ''' 用 <c>cov = Σ(a·b) − n·m_a·m_b</c> 而不是逐点减均值：
        ''' 每次迭代少两次减法、少一次额外的数组读取，且两列都是连续内存，
        ''' JIT 能把内层循环向量化。
        ''' </remarks>
        Public Function Pearson(view As GeneExpressionView, i As Integer, j As Integer) As Double
            Dim rows As Integer = view.NRows
            Dim a As Double() = view.Columns(i)
            Dim b As Double() = view.Columns(j)
            Dim dot As Double = 0.0

            For r As Integer = 0 To rows - 1
                dot += a(r) * b(r)
            Next

            Dim covariance As Double = dot - rows * view.RowMeans(i) * view.RowMeans(j)
            Dim denominator As Double = std.Sqrt(view.RowSs(i) * view.RowSs(j))

            If denominator < 0.000000000001 Then
                Return 0.0
            End If

            Return covariance / denominator
        End Function

    End Module

End Namespace
