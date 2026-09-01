#Region "Microsoft.VisualBasic::92b6330268b82e3b1ab4dd055623c23f, sub-system\BNLearn\DBN\DynamicBayesianNetwork.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 995
'    Code Lines: 591 (59.40%)
' Comment Lines: 256 (25.73%)
'    - Xml Docs: 56.64%
' 
'   Blank Lines: 148 (14.87%)
'     File Size: 43.30 KB


'     Class DynamicBayesianNetwork
' 
'         Properties: Config
' 
'         Constructor: (+2 Overloads) Sub New
' 
'         Function: ComputeActivationScore, ComputeDefaultDistribution, ComputeExpectedRNARate, ComputeLogLikelihood, DiscretizeValue
'                   GetAllNodes, GetGeneNodes, GetMarginalDistribution, GetMetaboliteNodes, GetNode
'                   GetOperonGeneMapping, GetSummary, GetTFNodes, GetThresholds, PredictNextState
'                   StateToScore
' 
'         Sub: BuildFromTopology, InitializeCPT, LearnParameters, LoadFromFile, ResetToTopologyPrior
'              SaveToFile, UpdateParametersOnline
' 
' 
' /********************************************************************************/

#End Region

' File: DynamicBayesianNetwork.vb
'
' Dynamic Bayesian Network (DBN) for Gene Regulatory Network Simulation
' Part of the Virtual Cell Computational Engine
'
' Features:
' 1. Topology-based DBN construction from RegulatoryLink structures
'    (works without RNAseq data - uses biological heuristics for CPT initialization)
' 2. Parameter learning from RNAseq time-series data
'    (topology serves as Dirichlet prior, data refines the parameters)
' 3. Prediction interface for ODEs coupling
'    (metabolites + TF abundances -> gene expression states + RNA abundance changes)
' 4. Bidirectional coupling with metabolic network ODEs
' 5. No third-party dependencies (uses only .NET base class library)
'
' Architecture:
'   - 2-slice Temporal Bayesian Network (2TBN): gene[t+1] depends on TF[t] and metabolite[t]
'   - Discrete states: Low, Medium, High (configurable)
'   - Noisy-OR / Noisy-AND combination for multiple regulators
'   - Dirichlet prior for Bayesian parameter estimation
'
' Coupling Rules:
'   DBN -> ODEs: Predicted gene states map to RNA transcript abundance change rates
'                dR/dt = k_synthesis * E[transcription_rate] - k_degradation * R
'   ODEs -> DBN: Metabolite concentrations and TF abundances are discretized
'                and used as evidence for DBN inference
'
' Requirements:
'   - RegulatoryLink and Effector types must be accessible (same project or global namespace)
'   - .NET Framework 4.0+ or .NET Core/5+ (uses Tuple, LINQ)

Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar

Namespace DBN

    ' ==================== Dynamic Bayesian Network (Main Class) ====================

    ''' <summary>
    ''' Dynamic Bayesian Network for gene regulatory network simulation.
    ''' 
    ''' This DBN implements a 2-slice temporal Bayesian network (2TBN) where:
    ''' - Gene/operon expression at time t+1 depends on TF and metabolite states at time t
    ''' - TF states are provided as evidence from the ODE solver (TF protein/RNA abundance)
    ''' - Metabolite concentrations are provided as evidence from the ODE solver
    ''' 
    ''' The DBN supports two modes:
    ''' 1. Topology-only mode: Uses RegulatoryLink topology to initialize CPTs based on
    '''    biological heuristics (activator/inhibitor effects via noisy-OR/AND gates).
    '''    No RNAseq data required.
    ''' 2. Data-fitting mode: Uses RNAseq time-series data to learn CPT parameters,
    '''    with the topology-based CPTs serving as a Dirichlet prior.
    ''' 
    ''' Coupling with metabolic network ODEs:
    ''' - DBN -> ODEs: Predicted gene states are mapped to RNA transcript abundance
    '''   change rates (expected transcription rate), which serve as transcription rate
    '''   terms in the ODEs: dR/dt = k_synthesis * rate - k_degradation * R
    ''' - ODEs -> DBN: Metabolite concentrations and TF abundances from the ODEs are
    '''   discretized (Low/Medium/High) and used as evidence for DBN inference.
    ''' </summary>
    Public Class DynamicBayesianNetwork

        ' Internal state
        Private _nodes As New Dictionary(Of String, DBNNode)
        Private _topologyLinks As RegulatoryLink()
        Private _operonGenes As New Dictionary(Of String, List(Of String))
        Private _rng As Random
        Private _config As DBNConfig

        ''' <summary>
        ''' 节点 ID → 预计算好的激活模型（父下标索引）。
        ''' 
        ''' 原始实现在每个父配置里都要对 ParentIds 做 IndexOf 字符串查找（O(P) 次、每次 O(P)），
        ''' 总复杂度为 O(3^P · P²)。预计算之后每个配置只需 O(P) 次数组取值。
        ''' </summary>
        Private _activationModels As New Dictionary(Of String, ActivationModel)

        ''' <summary>Configuration for the DBN (discretization, smoothing, rates, etc.)</summary>
        Public Property Config As DBNConfig
            Get
                Return _config
            End Get
            Set(value As DBNConfig)
                _config = value
                If _config IsNot Nothing Then
                    _rng = New Random(_config.Seed)
                End If
            End Set
        End Property

        Public ReadOnly Property topologySize As Integer
            Get
                Return _topologyLinks.Length
            End Get
        End Property

        ''' <summary>Default constructor with default configuration</summary>
        Public Sub New()
            _config = New DBNConfig()
            _rng = New Random(_config.Seed)
        End Sub

        ''' <summary>Constructor with custom configuration</summary>
        Public Sub New(config As DBNConfig)
            _config = config
            If _config Is Nothing Then _config = New DBNConfig()
            _rng = New Random(_config.Seed)
        End Sub

        ' ==================== Build from Topology ====================

        ''' <summary>
        ''' Build the DBN structure from a list of RegulatoryLink objects.
        ''' 
        ''' This method:
        ''' 1. Creates nodes for all TFs, effector metabolites, and target operons
        ''' 2. Sets up parent-child relationships (gene -> TF + effector parents)
        ''' 3. Initializes CPTs based on biological heuristics (noisy-OR/AND gates)
        ''' 
        ''' After calling this method, the DBN is ready for prediction (topology-only mode)
        ''' or parameter learning (data-fitting mode).
        ''' </summary>
        ''' <param name="links">List of regulatory links defining the network topology</param>
        Public Function BuildFromTopology(links As IEnumerable(Of RegulatoryLink)) As DynamicBayesianNetwork
            Call $"DynamicBayesianNetwork.BuildFromTopology: 正在构建 DBN 结构".debug

            If links Is Nothing Then
                Throw New ArgumentNullException("the gene expression regulator network should not be nothing!")
            Else
                Dim all = links.ToArray

                ' 自环（TF 调控其自身）在 2TBN 语义下没有意义：它会让节点成为自己的父节点，
                ' 并凭空使该节点 CPT 的规模翻 3 倍。这里在入口处直接剔除。
                _topologyLinks = all _
                    .Where(Function(l) Not String.Equals(l.TF_id, l.target_operon, StringComparison.OrdinalIgnoreCase)) _
                    .ToArray

                If _topologyLinks.Length <> all.Length Then
                    Call $"[DBN] 剔除自环调控边 {all.Length - _topologyLinks.Length} 条（剩余 {_topologyLinks.Length} 条）".debug
                End If

                _nodes.Clear()
                _operonGenes.Clear()
            End If

            Call "Step 1: Create nodes for all TFs, effector metabolites, and target operons".debug

            ' --- Step 1: Create nodes for all TFs, effector metabolites, and target operons ---
            For Each link As RegulatoryLink In _topologyLinks
                ' Create TF node if not exists
                If Not _nodes.ContainsKey(link.TF_id) Then
                    _nodes(link.TF_id) = New DBNNode(link.TF_id, DBNNodeType.TranscriptionFactor)
                End If

                ' Add effector metabolites to TF node and create metabolite nodes
                If link.effector IsNot Nothing Then
                    For Each kv In link.effector
                        _nodes(link.TF_id).EffectorMetabolites(kv.Key) = kv.Value
                        ' Create metabolite node if not exists
                        If Not _nodes.ContainsKey(kv.Key) Then
                            _nodes(kv.Key) = New DBNNode(kv.Key, DBNNodeType.EffectorMetabolite)
                        End If
                    Next
                End If

                ' Create operon (gene) node if not exists
                If Not _nodes.ContainsKey(link.target_operon) Then
                    _nodes(link.target_operon) = New DBNNode(link.target_operon, DBNNodeType.Gene)
                End If

                ' Store operon-gene mapping
                If link.regulate_genes IsNot Nothing AndAlso link.regulate_genes.Length > 0 Then
                    If Not _operonGenes.ContainsKey(link.target_operon) Then
                        _operonGenes(link.target_operon) = New List(Of String)()
                    End If
                    For Each g In link.regulate_genes
                        If Not _operonGenes(link.target_operon).Contains(g) Then
                            _operonGenes(link.target_operon).Add(g)
                        End If
                    Next
                End If
            Next

            Call "Step 2: Set up parent-child relationships".debug

            ' --- Step 2: Set up parent-child relationships ---
            ' dirConfidence：gene → tf → 已采用的边的置信度，用于方向冲突时做确定性仲裁
            Dim dirConfidence As New Dictionary(Of String, Dictionary(Of String, Double))

            For Each link As RegulatoryLink In _topologyLinks
                Dim geneNode = _nodes(link.target_operon)

                ' Add TF as parent (avoid duplicates for multi-effector TFs)
                If Not geneNode.ParentIds.Contains(link.TF_id) Then
                    geneNode.ParentIds.Add(link.TF_id)
                    geneNode.RegulatorTFs.Add(link.TF_id)
                End If

                ' 记录 TF → 本基因 的调控方向（调控方向是边的属性，而非 TF 的属性：
                ' 同一个 TF 完全可能对基因 A 激活、对基因 B 抑制）。
                ' 同一 (TF, gene) 存在多条方向冲突的边时以置信度较高者为准，
                ' 置信度相同则保留先出现者，保证结果确定可复现。
                If Not dirConfidence.ContainsKey(link.target_operon) Then
                    dirConfidence(link.target_operon) = New Dictionary(Of String, Double)
                End If

                Dim adoptedConf As Double = 0

                If Not dirConfidence(link.target_operon).TryGetValue(link.TF_id, adoptedConf) OrElse link.Confidence > adoptedConf Then
                    dirConfidence(link.target_operon)(link.TF_id) = link.Confidence
                    geneNode.ParentDirections(link.TF_id) = link.RegulationType
                End If

                ' Initialize TFEffectors entry if needed
                If Not geneNode.TFEffectors.ContainsKey(link.TF_id) Then
                    geneNode.TFEffectors(link.TF_id) = New List(Of String)()
                End If

                ' Add effector metabolites as parents and in TFEffectors mapping
                If link.effector IsNot Nothing Then
                    For Each kv In link.effector
                        ' Add metabolite as parent of the gene
                        If Not geneNode.ParentIds.Contains(kv.Key) Then
                            geneNode.ParentIds.Add(kv.Key)
                        End If
                        ' Track which effectors belong to this TF (for this gene)
                        If Not geneNode.TFEffectors(link.TF_id).Contains(kv.Key) Then
                            geneNode.TFEffectors(link.TF_id).Add(kv.Key)
                        End If
                    Next
                End If
            Next

            Call "Step 2b: Guard parent count and precompute activation models".debug

            ' --- Step 2b: 父节点数量上限保护 + 激活模型预计算 ---
            Call ApplyParentLimit()
            Call BuildActivationModels()

            Call "Step 3: Initialize CPTs for all nodes".debug

            ' 诊断：CPT 的行数随父节点数呈 3^P 指数增长，父节点数不受限时会直接导致
            ' 初始化阶段内存与耗时爆炸。这里把拓扑规模显式输出，便于定位问题节点。
            Call LogTopologyScale()

            Dim bar As Tqdm.ProgressBar = Nothing

            ' --- Step 3: Initialize CPTs for all nodes ---
            For Each node In Tqdm.Wrap(_nodes.Values, bar:=bar)
                'If node.ParentIds.Count >= 8 Then
                '    Call $"[DBN init] {node.NodeId} parents={node.ParentIds.Count} rows={Math.Pow(3, node.ParentIds.Count).ToString("E3")} mem={App.MemorySize}".debug
                'End If

                Call InitializeCPT(node)
                Call bar.SetLabel($"{node.NodeId} {App.MemorySize}")
            Next

            Call "[BuildFromTopology] finished!".debug

            Return Me
        End Function

        ''' <summary>
        ''' 父节点数量上限保护：对父节点数超过 Config.MaxParents 的节点做确定性截断。
        ''' 
        ''' 截断策略：优先保留调控靶标更少（特异性更强）的父节点，同度时按 ID 排序，
        ''' 保证结果可复现；ParentIds / RegulatorTFs / TFEffectors 三者同步裁剪，避免状态不一致。
        ''' 默认上限 20 只用于隔离极端异常拓扑（正常情况下不会触发），不损失真实调控关系。
        ''' </summary>
        Private Sub ApplyParentLimit()
            Dim limit As Integer = _config.MaxParents

            If limit <= 0 Then Return

            ' 统计每个调控因子在本拓扑中的出度，作为"特异性"的度量
            Dim outDegree As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For Each link As RegulatoryLink In _topologyLinks
                If outDegree.ContainsKey(link.TF_id) Then
                    outDegree(link.TF_id) += 1
                Else
                    outDegree(link.TF_id) = 1
                End If
            Next

            For Each node In _nodes.Values
                If node.ParentIds.Count <= limit Then Continue For

                Dim keep = node.ParentIds _
                    .Select(Function(pid) New With {.id = pid, .deg = If(outDegree.ContainsKey(pid), outDegree(pid), 0)}) _
                    .OrderBy(Function(x) x.deg) _
                    .ThenBy(Function(x) x.id, StringComparer.Ordinal) _
                    .Take(limit) _
                    .Select(Function(x) x.id) _
                    .ToArray

                Dim keepSet As New HashSet(Of String)(keep, StringComparer.OrdinalIgnoreCase)
                Dim origin As Integer = node.ParentIds.Count

                node.ParentIds = keep.ToList()
                node.RegulatorTFs = node.RegulatorTFs.Where(Function(t) keepSet.Contains(t)).ToList()

                For Each tfId In node.TFEffectors.Keys.ToArray()
                    If Not keepSet.Contains(tfId) Then
                        node.TFEffectors.Remove(tfId)
                    Else
                        node.TFEffectors(tfId) = node.TFEffectors(tfId).Where(Function(e) keepSet.Contains(e)).ToList()
                    End If
                Next

                ' 同步清理被裁剪掉的父节点的方向记录，保持节点状态一致
                For Each pid In node.ParentDirections.Keys.ToArray()
                    If Not keepSet.Contains(pid) Then
                        node.ParentDirections.Remove(pid)
                    End If
                Next

                Call $"[DBN] {node.NodeId} 父节点数 {origin} 超过上限 {limit}，已裁剪 {origin - keep.Length} 个".debug
            Next
        End Sub

        ''' <summary>
        ''' 为所有节点预计算激活模型：把 TF / effector 在 ParentIds 中的下标以及调控方向
        ''' 解析成定长数组，供 <see cref="ComputeActivationScore"/> 直接取用。
        ''' </summary>
        Private Sub BuildActivationModels()
            _activationModels.Clear()

            Dim nActivate As Integer = 0
            Dim nInhibit As Integer = 0

            For Each node In _nodes.Values
                Dim model = BuildActivationModel(node)

                _activationModels(node.NodeId) = model

                For k As Integer = 0 To model.Count - 1
                    If model.isInhibitor(k) Then
                        nInhibit += 1
                    Else
                        nActivate += 1
                    End If
                Next
            Next

            ' 方向分布是诊断"调控信息是否丢失"的关键指标：若抑制项数为 0，
            ' 说明拓扑构建时未传递 per-edge 的 RegulationType，此时激活得分恒为正，
            ' 归一化后的得分恒 >= 0.5，CPT 的 Low 分支（score < 0.34）不可达。
            Call $"[DBN dir] 调控项统计: 激活={nActivate}, 抑制={nInhibit}".info

            If nInhibit = 0 Then
                Call "[DBN dir] 警告: 网络中不存在任何抑制性调控，激活得分恒为正，CPT 的 Low 分支不可达".debug
            End If
        End Sub

        ''' <summary>构建单个节点的激活模型（TF 下标 / effector 下标 / 是否抑制）</summary>
        Private Function BuildActivationModel(node As DBNNode) As ActivationModel
            Dim tfIdx As New List(Of Integer)
            Dim effIdx As New List(Of Integer)
            Dim inhibitor As New List(Of Boolean)

            For Each tfId As String In node.RegulatorTFs
                Dim tIdx As Integer = node.ParentIds.IndexOf(tfId)
                If tIdx < 0 Then Continue For

                Dim effectorIds As List(Of String) = Nothing

                If node.TFEffectors.ContainsKey(tfId) Then
                    effectorIds = node.TFEffectors(tfId)
                End If

                If effectorIds Is Nothing OrElse effectorIds.Count = 0 Then
                    ' 无 effector：优先使用该 TF→本基因 边上声明的方向（per-edge 属性），
                    ' 边未声明方向时回退到该 TF 的默认方向（per-TF，见 DBNNode.DefaultRegulatoryDirection）。
                    Dim direction As Effector = Effector.Unknown

                    If Not node.ParentDirections.TryGetValue(tfId, direction) Then
                        direction = _nodes(tfId).DefaultRegulatoryDirection
                    End If

                    If direction = Effector.Activator Then
                        tfIdx.Add(tIdx)
                        effIdx.Add(-1)
                        inhibitor.Add(False)
                    ElseIf direction = Effector.Inhibitor Then
                        tfIdx.Add(tIdx)
                        effIdx.Add(-1)
                        inhibitor.Add(True)
                    End If
                Else
                    ' 有 effector：TF 与 effector 需同时存在才形成调控复合体
                    For Each effId In effectorIds
                        Dim eIdx As Integer = node.ParentIds.IndexOf(effId)
                        If eIdx < 0 Then Continue For

                        Dim effType As Effector = Effector.Unknown

                        If _nodes(tfId).EffectorMetabolites.ContainsKey(effId) Then
                            effType = _nodes(tfId).EffectorMetabolites(effId)
                        End If

                        If effType = Effector.Activator Then
                            tfIdx.Add(tIdx)
                            effIdx.Add(eIdx)
                            inhibitor.Add(False)
                        ElseIf effType = Effector.Inhibitor Then
                            tfIdx.Add(tIdx)
                            effIdx.Add(eIdx)
                            inhibitor.Add(True)
                        End If
                    Next
                End If
            Next

            Return New ActivationModel With {
                .tfIdx = tfIdx.ToArray(),
                .effIdx = effIdx.ToArray(),
                .isInhibitor = inhibitor.ToArray()
            }
        End Function

        ''' <summary>
        ''' 输出当前拓扑的规模诊断信息（父节点数分布 / CPT 行数估算 / hub 节点清单）。
        ''' 
        ''' CPT 的行数等于各父节点状态数的乘积，默认 3 态即 3^P（P = 父节点数），
        ''' 因此父节点数一旦不受限，初始化阶段的时间与内存都会指数级爆炸。
        ''' 该方法把规模显式输出到日志，用于定位导致爆炸的节点。
        ''' </summary>
        Private Sub LogTopologyScale()
            Dim hist As New Dictionary(Of Integer, Integer)
            Dim worst As New List(Of (id As String, p As Integer))
            Dim maxP As Integer = 0
            Dim nWithParents As Integer = 0
            Dim totalRows As Double = 0

            For Each node In _nodes.Values
                Dim p As Integer = node.ParentIds.Count

                If Not hist.ContainsKey(p) Then hist(p) = 0
                hist(p) += 1

                If p > maxP Then maxP = p
                If p > 0 Then
                    nWithParents += 1
                    totalRows += Math.Pow(3, p)
                End If

                worst.Add((node.NodeId, p))
            Next

            Dim top = worst _
                .OrderByDescending(Function(x) x.p) _
                .Take(10) _
                .Select(Function(x) $"{x.id}(P={x.p})") _
                .ToArray

            Call $"[DBN scale] nodes={_nodes.Count} with_parents={nWithParents} max_parents={maxP}".info
            Call $"[DBN scale] cpt_rows={totalRows.ToString("E3")} est_mem_gb={(totalRows * 180 / 1024 ^ 3).ToString("F3")}".info
            Call $"[DBN scale] parents_hist={String.Join(", ", hist.OrderBy(Function(kv) kv.Key).Select(Function(kv) $"{kv.Key}:{kv.Value}"))}".info
            Call $"[DBN scale] top_hub={String.Join(", ", top)}".info
        End Sub


        ' ==================== CPT Initialization (Topology-Based Prior) ====================

        ''' <summary>
        ''' Initialize the CPT for a node based on its type and parent relationships.
        ''' For gene nodes, uses biological heuristics (noisy-OR/AND combination of
        ''' activator and inhibitor effects). For other nodes, uses uniform distributions.
        ''' </summary>
        Private Sub InitializeCPT(node As DBNNode)
            node.CPT.VariableId = node.NodeId
            node.CPT.ParentIds = New List(Of String)(node.ParentIds)
            node.CPT.States = New List(Of String)(node.States)
            node.CPT.Table = New Dictionary(Of String, Double())
            node.CPT.OnDemandProvider = Nothing
            ' 完整展开的节点其条目数由 rows 决定且有界，不需要缓存上限
            node.CPT.MaxCacheRows = Integer.MaxValue

            If node.ParentIds.Count = 0 Then
                ' No parents: uniform prior
                Dim dist(node.States.Count - 1) As Double
                For i = 0 To dist.Length - 1
                    dist(i) = 1.0 / node.States.Count
                Next
                node.CPT.SetDistribution(New List(Of String), dist, copy:=False)
                Exit Sub
            End If

            Dim parentStatesMap As Dictionary(Of String, List(Of String)) = GetParentStatesMap(node)
            Dim rows As Long = node.CPT.GetConfigurationCount(parentStatesMap)

            If rows <= _config.MaxCPTRows Then
                ' 规模可控：完整展开。预先给定容量，避免 Dictionary 扩容时新旧桶数组
                ' 并存造成的内存瞬时翻倍；分布数组由调用方新建，无需再克隆一份。
                node.CPT.Table = New Dictionary(Of String, Double())(CInt(rows))
                node.CPT.OnDemandProvider = Nothing

                For Each cfg In node.CPT.GetAllParentConfigurations(parentStatesMap)
                    node.CPT.SetDistribution(cfg, ComputeDefaultDistribution(node, cfg), copy:=False)
                Next
            Else
                ' 规模爆炸（3^P 远超阈值）：不展开全表，改为查询时按需计算。
                ' 拓扑先验分布本身是父状态的纯函数（noisy-OR/AND 得分的确定性映射），
                ' 现场计算的结果与全表展开逐位一致，但内存从 O(3^P) 降为 O(实际访问过的配置数)。
                node.CPT.Table = New Dictionary(Of String, Double())()
                node.CPT.MaxCacheRows = _config.MaxCPTCacheRows
                node.CPT.OnDemandProvider = Function(cfg) ComputeDefaultDistribution(node, cfg)

                ' Call $"[DBN lazy] {node.NodeId} parents={node.ParentIds.Count} rows={rows} -> 按需计算（不展开 CPT）".debug
            End If
        End Sub

        ''' <summary>构造 父节点 ID → 状态列表 的映射，供配置枚举与规模估算使用</summary>
        Private Function GetParentStatesMap(node As DBNNode) As Dictionary(Of String, List(Of String))
            Dim map As New Dictionary(Of String, List(Of String))

            For Each pid As String In node.ParentIds
                map(pid) = _nodes(pid).States
            Next

            Return map
        End Function


        ''' <summary>
        ''' Compute the default probability distribution for a gene node given parent states.
        ''' Uses the activation score to determine the distribution shape:
        ''' - High activation score -> P(High) dominant
        ''' - Moderate score -> P(Medium) dominant
        ''' - Low score (inhibited) -> P(Low) dominant
        ''' </summary>
        Private Function ComputeDefaultDistribution(node As DBNNode, parentStates As List(Of String)) As Double()
            Dim dist(node.States.Count - 1) As Double

            If node.NodeType = DBNNodeType.Gene AndAlso node.ParentIds.Count > 0 Then
                ' Compute activation score [0, 1]
                Dim score = ComputeActivationScore(node, parentStates)

                Dim idxHigh = node.States.IndexOf("High")
                Dim idxMed = node.States.IndexOf("Medium")
                Dim idxLow = node.States.IndexOf("Low")

                ' Map score to probability distribution using soft thresholds
                If score >= 0.66 Then
                    ' Strong activation
                    dist(idxHigh) = 0.7
                    dist(idxMed) = 0.2
                    dist(idxLow) = 0.1
                ElseIf score >= 0.34 Then
                    ' Moderate / basal expression
                    dist(idxHigh) = 0.25
                    dist(idxMed) = 0.5
                    dist(idxLow) = 0.25
                Else
                    ' Low expression (inhibited)
                    dist(idxHigh) = 0.1
                    dist(idxMed) = 0.2
                    dist(idxLow) = 0.7
                End If
            Else
                ' Non-gene nodes or genes without parents: uniform
                For i = 0 To dist.Length - 1
                    dist(i) = 1.0 / node.States.Count
                Next
            End If

            Return dist
        End Function


        ''' <summary>
        ''' Compute the activation score for a gene given parent (TF and metabolite) states.
        ''' 
        ''' Uses a noisy-OR / noisy-AND combination model:
        ''' - For activators: P(activation) = 1 - prod(1 - p_i)  [noisy-OR]
        ''' - For inhibitors: P(inhibition) = 1 - prod(1 - q_j)  [noisy-OR]
        ''' - Net score = activation - inhibition, mapped to [0, 1]
        ''' 
        ''' Interpretation of Effector enum (net effect on gene expression):
        ''' - Activator: high effector concentration promotes gene activation
        ''' - Inhibitor: high effector concentration promotes gene inhibition
        ''' - Unknown: neutral effect (no contribution)
        ''' 
        ''' For TFs without effectors, uses the TF's DefaultRegulatoryDirection.
        ''' </summary>
        Private Function ComputeActivationScore(node As DBNNode, parentStates As List(Of String)) As Double
            ' 使用预计算的下标模型：把每个父配置内部的 O(P) 次 IndexOf 字符串查找
            ' 替换为 O(1) 的数组取值（初始化热路径的总复杂度由 O(3^P·P²) 降为 O(3^P·P)）。
            Dim model As ActivationModel = Nothing

            If Not _activationModels.TryGetValue(node.NodeId, model) OrElse model Is Nothing Then
                model = BuildActivationModel(node)
                _activationModels(node.NodeId) = model
            End If

            Dim activationScore = 0.0  ' P(at least one activator is active)
            Dim inhibitionScore = 0.0  ' P(at least one inhibitor is active)
            Dim hasActivator = False
            Dim hasInhibitor = False

            For k As Integer = 0 To model.Count - 1
                Dim itemScore As Double = StateToScore(parentStates(model.tfIdx(k)))  ' Low=0, Medium=0.5, High=1

                If model.effIdx(k) >= 0 Then
                    ' TF-effector 复合体：TF 与 effector 需同时存在，得分取二者之积
                    itemScore *= StateToScore(parentStates(model.effIdx(k)))
                End If

                If model.isInhibitor(k) Then
                    hasInhibitor = True
                    ' Noisy-OR: combine with existing inhibition
                    inhibitionScore = 1 - (1 - inhibitionScore) * (1 - itemScore)
                Else
                    hasActivator = True
                    ' Noisy-OR: combine with existing activation
                    activationScore = 1 - (1 - activationScore) * (1 - itemScore)
                End If
            Next

            ' Compute net score
            Dim netScore As Double
            If hasActivator AndAlso hasInhibitor Then
                ' Both activation and inhibition present: net = activation - inhibition
                netScore = activationScore - inhibitionScore
            ElseIf hasActivator Then
                netScore = activationScore
            ElseIf hasInhibitor Then
                netScore = -inhibitionScore
            Else
                ' No regulation: basal (netScore = 0 -> score = 0.5)
                netScore = 0.0
            End If

            ' Map to [0, 1]: 0.5 = basal, >0.5 = activated, <0.5 = inhibited
            Dim score = 0.5 + 0.5 * netScore
            Return Math.Max(0.0, Math.Min(1.0, score))
        End Function


        ''' <summary>Convert a state string to a numeric score (Low=0, Medium=0.5, High=1)</summary>
        Private Function StateToScore(state As String) As Double
            Select Case state
                Case "Low" : Return 0.0
                Case "Medium" : Return 0.5
                Case "High" : Return 1.0
                Case Else : Return 0.5
            End Select
        End Function


        ' ==================== Parameter Learning (Data-Fitting Mode) ====================

        ''' <summary>
        ''' Learn CPT parameters from RNAseq time-series data.
        ''' 
        ''' Uses Bayesian estimation with Dirichlet prior:
        '''   P(s|parents) = (count(s) + alpha * prior(s)) / (total + alpha)
        ''' where:
        '''   - count(s) is the observed count of state s given the parent configuration
        '''   - alpha is the smoothing parameter (Config.SmoothingAlpha)
        '''   - prior(s) is the topology-based probability (serves as base distribution)
        ''' 
        ''' When alpha is large, the prior (topology) dominates.
        ''' When alpha is small, the data dominates.
        ''' When no data is available for a configuration, the prior is retained.
        ''' </summary>
        ''' <param name="rnaSeqTimeSeries">
        ''' List of time points. Each time point is a dictionary mapping
        ''' node_id (gene/TF/metabolite) -> abundance value (continuous).
        ''' Values should be normalized to [0, 1] for default thresholds,
        ''' or custom thresholds should be provided.
        ''' </param>
        ''' <param name="customThresholds">
        ''' Optional per-node discretization thresholds.
        ''' Key = node ID, Value = Tuple(low_threshold, high_threshold).
        ''' If not provided, uses Config.LowThreshold and Config.HighThreshold.
        ''' </param>
        Public Sub LearnParameters(rnaSeqTimeSeries As List(Of Dictionary(Of String, Double)), Optional customThresholds As Dictionary(Of String, Tuple(Of Double, Double)) = Nothing)
            If rnaSeqTimeSeries Is Nothing OrElse rnaSeqTimeSeries.Count < 2 Then
                Throw New ArgumentException("Need at least 2 time points for parameter learning")
            Else
                Call "learn parameters...".debug
            End If

            Call "Step 1: Discretize all continuous values".debug

            ' --- Step 1: Discretize all continuous values ---
            Dim discreteSeries As New List(Of Dictionary(Of String, String))
            For Each tp In rnaSeqTimeSeries
                Dim d As New Dictionary(Of String, String)
                If tp IsNot Nothing Then
                    For Each kv In tp
                        If _nodes.ContainsKey(kv.Key) Then
                            Dim thresh = GetThresholds(kv.Key, customThresholds)
                            d(kv.Key) = DiscretizeValue(kv.Value, thresh.Item1, thresh.Item2)
                        End If
                    Next
                End If
                discreteSeries.Add(d)
            Next

            Call "Step 2: Initialize sparse count tables for all nodes with parents".debug

            ' --- Step 2: 初始化（稀疏）计数表 ---
            ' 旧实现会为每个节点预先建好全部 3^P 个父配置的计数数组，其内存开销与 CPT
            ' 同一量级，是初始化阶段的第二个内存炸弹。改为"稀疏计数"：只在数据中真正
            ' 观测到某个父配置时才建条目。对没有观测的配置，后验
            '   (0 + α·prior) / (0 + α) = prior
            ' 是恒等变换，因此"只回写有观测的配置"与"回写全部配置"在数学上完全等价。
            Dim counts As New Dictionary(Of String, Dictionary(Of String, Double()))
            For Each node In _nodes.Values
                If node.ParentIds.Count = 0 Then Continue For

                counts(node.NodeId) = New Dictionary(Of String, Double())
            Next

            Call "Step 3: Count transitions (t -> t+1)".debug

            ' --- Step 3: Count transitions (t -> t+1) ---
            For t = 0 To discreteSeries.Count - 2
                Dim cur = discreteSeries(t)
                Dim nxt = discreteSeries(t + 1)

                For Each node In _nodes.Values
                    If node.ParentIds.Count = 0 Then Continue For
                    If Not nxt.ContainsKey(node.NodeId) Then Continue For

                    ' Get parent states at time t
                    Dim parentStates As New List(Of String)
                    Dim allPresent = True
                    For Each pid As String In node.ParentIds
                        If Not cur.ContainsKey(pid) Then
                            allPresent = False
                            Exit For
                        End If
                        parentStates.Add(cur(pid))
                    Next
                    If Not allPresent Then Continue For

                    Dim childState = nxt(node.NodeId)
                    Dim childIdx = node.States.IndexOf(childState)

                    If childIdx < 0 Then Continue For

                    Dim key = String.Join("|", parentStates)
                    Dim table = counts(node.NodeId)
                    Dim c As Double() = Nothing

                    If Not table.TryGetValue(key, c) Then
                        c = New Double(node.States.Count - 1) {}
                        table(key) = c
                    End If

                    c(childIdx) += 1
                Next
            Next

            Call "Step 4: Update CPTs with Dirichlet posterior (observed configurations only)".debug

            ' --- Step 4: Update CPTs with Dirichlet posterior ---
            ' P(s|parents) = (count(s) + alpha * prior(s)) / (total + alpha)
            ' 只遍历实际观测到的配置：未观测配置的后验恒等于其先验，无需回写。
            For Each node In Tqdm.Wrap(_nodes.Values)
                If node.ParentIds.Count = 0 Then
                    Continue For
                End If

                For Each kv In counts(node.NodeId)
                    Dim cfg As New List(Of String)(kv.Key.Split("|"c))
                    Dim c = kv.Value
                    Dim total = c.Sum()

                    ' Get prior distribution from topology-based CPT
                    ' （只读场景，不需要再复制一份数组）
                    Dim prior = node.CPT.GetDistribution(cfg, copy:=False)

                    ' Compute posterior
                    Dim newDist(node.States.Count - 1) As Double
                    Dim denom = total + _config.SmoothingAlpha
                    For i = 0 To newDist.Length - 1
                        newDist(i) = (c(i) + _config.SmoothingAlpha * prior(i)) / denom
                    Next

                    node.CPT.SetDistribution(cfg, newDist, copy:=False)
                Next
            Next

            Call "[LearnParameters] finished!".debug
        End Sub


        ''' <summary>
        ''' Online parameter update using exponential moving average (EMA).
        ''' Useful for incremental learning during simulation.
        ''' 
        ''' For each transition (current -> next), updates the CPT entry:
        '''   P(s|parents) = (1 - lr) * P_old(s|parents) + lr * target(s)
        ''' where target(s) = 1 if s == observed_next_state, else 0.
        ''' </summary>
        ''' <param name="currentStates">Discrete states at time t (node_id -> state string)</param>
        ''' <param name="nextStates">Discrete states at time t+1 (node_id -> state string)</param>
        Public Sub UpdateParametersOnline(currentStates As Dictionary(Of String, String), nextStates As Dictionary(Of String, String))
            If currentStates Is Nothing OrElse nextStates Is Nothing Then
                Return
            End If

            For Each node In _nodes.Values
                If node.ParentIds.Count = 0 Then Continue For
                If Not nextStates.ContainsKey(node.NodeId) Then Continue For

                ' Get parent states at time t
                Dim parentStates As New List(Of String)
                Dim allPresent = True
                For Each pid As String In node.ParentIds
                    If Not currentStates.ContainsKey(pid) Then
                        allPresent = False
                        Exit For
                    End If
                    parentStates.Add(currentStates(pid))
                Next
                If Not allPresent Then Continue For

                Dim childIdx = node.States.IndexOf(nextStates(node.NodeId))
                If childIdx < 0 Then Continue For

                ' EMA update
                Dim dist = node.CPT.GetDistribution(parentStates, copy:=False)
                Dim lr = _config.OnlineLearningRate

                For i = 0 To dist.Length - 1
                    Dim target = If(i = childIdx, 1.0, 0.0)
                    dist(i) = (1 - lr) * dist(i) + lr * target
                Next

                node.CPT.SetDistribution(parentStates, dist, copy:=False)
            Next
        End Sub


        ' ==================== Prediction / Inference (ODEs -> DBN) ====================

        ''' <summary>
        ''' Predict the next-state gene expression given current metabolite and TF abundances.
        ''' 
        ''' This is the main interface for ODEs -> DBN coupling.
        ''' 
        ''' Algorithm:
        ''' 1. Discretize metabolite concentrations and TF abundances into Low/Medium/High
        ''' 2. For each gene node, look up CPT: P(gene[t+1] | parents[t])
        ''' 3. Determine predicted state (argmax or sampled)
        ''' 4. Compute expected RNA transcription rate from the distribution
        ''' 
        ''' Usage:
        '''   Dim result = dbn.PredictNextState(metabolites, tfAbundances)
        '''   ' result.GeneStates: operon_id -> "Low"/"Medium"/"High"
        '''   ' result.RNAAbundanceChanges: operon_id -> transcription rate [0, 1]
        ''' </summary>
        ''' <param name="metaboliteAbundances">
        ''' Metabolite ID -> concentration (normalized to [0, 1] recommended).
        ''' These come from the metabolic network ODEs.
        ''' </param>
        ''' <param name="tfAbundances">
        ''' TF ID -> abundance (normalized to [0, 1] recommended).
        ''' These come from the TF protein/RNA ODEs.
        ''' </param>
        ''' <param name="currentGeneStates">
        ''' Optional: current gene states for parent nodes that lack evidence.
        ''' Used when a gene is also a TF (regulated by other TFs).
        ''' </param>
        ''' <returns>Prediction result with gene states, probabilities, and RNA abundance changes</returns>
        Public Function PredictNextState(
            metaboliteAbundances As Dictionary(Of String, Double),
            tfAbundances As Dictionary(Of String, Double),
            Optional currentGeneStates As Dictionary(Of String, String) = Nothing
        ) As DBNPredictionResult

            Dim result As New DBNPredictionResult()

            ' Copy operon-gene mapping to result
            For Each kv In _operonGenes
                result.OperonGeneMapping(kv.Key) = New List(Of String)(kv.Value)
            Next

            ' --- Step 1: Discretize evidence ---
            Dim evidence As New Dictionary(Of String, String)

            If metaboliteAbundances IsNot Nothing Then
                For Each kv In metaboliteAbundances
                    If _nodes.ContainsKey(kv.Key) Then
                        Dim thresh = GetThresholds(kv.Key, Nothing)
                        evidence(kv.Key) = DiscretizeValue(kv.Value, thresh.Item1, thresh.Item2)
                    End If
                Next
            End If

            If tfAbundances IsNot Nothing Then
                For Each kv In tfAbundances
                    If _nodes.ContainsKey(kv.Key) Then
                        Dim thresh = GetThresholds(kv.Key, Nothing)
                        evidence(kv.Key) = DiscretizeValue(kv.Value, thresh.Item1, thresh.Item2)
                    End If
                Next
            End If

            ' --- Step 2: For each gene node, compute P(gene[t+1] | parents[t]) ---
            For Each node In _nodes.Values
                If node.NodeType <> DBNNodeType.Gene Then Continue For

                ' Get parent states from evidence or defaults
                Dim parentStates As New List(Of String)
                For Each pid As String In node.ParentIds
                    If evidence.ContainsKey(pid) Then
                        parentStates.Add(evidence(pid))
                    ElseIf currentGeneStates IsNot Nothing AndAlso currentGeneStates.ContainsKey(pid) Then
                        parentStates.Add(currentGeneStates(pid))
                    Else
                        ' Default to Medium when no evidence available
                        parentStates.Add("Medium")
                    End If
                Next

                ' Get CPT distribution（只读，避免每次查询都分配新数组）
                Dim dist = node.CPT.GetDistribution(parentStates, copy:=False)

                ' Determine predicted state
                Dim predictedState As String
                Dim predictedProb As Double

                If _config.UseMultinomialSampling Then
                    ' Stochastic: sample from distribution
                    Dim r = _rng.NextDouble()
                    Dim cum = 0.0
                    predictedState = node.States(node.States.Count - 1)
                    predictedProb = dist(dist.Length - 1)
                    For i = 0 To dist.Length - 1
                        cum += dist(i)
                        If r <= cum Then
                            predictedState = node.States(i)
                            predictedProb = dist(i)
                            Exit For
                        End If
                    Next
                Else
                    ' Deterministic: take most likely state (argmax)
                    Dim maxIdx = 0
                    For i = 1 To dist.Length - 1
                        If dist(i) > dist(maxIdx) Then maxIdx = i
                    Next
                    predictedState = node.States(maxIdx)
                    predictedProb = dist(maxIdx)
                End If

                ' Store results
                result.GeneStates(node.NodeId) = predictedState
                result.GeneProbabilities(node.NodeId) = CType(dist.Clone(), Double())
                result.GeneStateProbabilities(node.NodeId) = predictedProb
                result.RNAAbundanceChanges(node.NodeId) = ComputeExpectedRNARate(dist, node.States)
            Next

            Return result
        End Function


        ''' <summary>
        ''' Compute the expected RNA transcript abundance change rate from a distribution.
        ''' 
        ''' E[rate] = P(High) * k_high + P(Medium) * k_basal + P(Low) * k_low
        ''' 
        ''' This value represents the expected transcription rate and can be used
        ''' directly in ODE models:
        '''   dR/dt = k_synthesis * E[rate] - k_degradation * R
        ''' </summary>
        Private Function ComputeExpectedRNARate(dist As Double(), states As List(Of String)) As Double
            Dim rate = 0.0
            For i = 0 To dist.Length - 1
                Select Case states(i)
                    Case "High"
                        rate += dist(i) * _config.HighTranscriptionRate
                    Case "Medium"
                        rate += dist(i) * _config.BasalTranscriptionRate
                    Case "Low"
                        rate += dist(i) * _config.LowTranscriptionRate
                End Select
            Next
            Return rate
        End Function


        ' ==================== Model Evaluation ====================

        ''' <summary>
        ''' Compute the log-likelihood of the data given the current model.
        ''' Useful for model evaluation, comparison, and convergence checking.
        ''' </summary>
        Public Function ComputeLogLikelihood(
            rnaSeqTimeSeries As List(Of Dictionary(Of String, Double)),
            Optional customThresholds As Dictionary(Of String, Tuple(Of Double, Double)) = Nothing
        ) As Double

            If rnaSeqTimeSeries Is Nothing OrElse rnaSeqTimeSeries.Count < 2 Then Return 0.0

            ' Discretize
            Dim discreteSeries As New List(Of Dictionary(Of String, String))
            For Each tp In rnaSeqTimeSeries
                Dim d As New Dictionary(Of String, String)
                If tp IsNot Nothing Then
                    For Each kv In tp
                        If _nodes.ContainsKey(kv.Key) Then
                            Dim thresh = GetThresholds(kv.Key, customThresholds)
                            d(kv.Key) = DiscretizeValue(kv.Value, thresh.Item1, thresh.Item2)
                        End If
                    Next
                End If
                discreteSeries.Add(d)
            Next

            Dim logLik = 0.0
            For t = 0 To discreteSeries.Count - 2
                Dim cur = discreteSeries(t)
                Dim nxt = discreteSeries(t + 1)

                For Each node In _nodes.Values
                    If node.ParentIds.Count = 0 Then Continue For
                    If Not nxt.ContainsKey(node.NodeId) Then Continue For

                    Dim parentStates As New List(Of String)
                    Dim allPresent = True
                    For Each pid As String In node.ParentIds
                        If Not cur.ContainsKey(pid) Then
                            allPresent = False
                            Exit For
                        End If
                        parentStates.Add(cur(pid))
                    Next
                    If Not allPresent Then Continue For

                    Dim dist = node.CPT.GetDistribution(parentStates, copy:=False)
                    Dim childIdx = node.States.IndexOf(nxt(node.NodeId))
                    If childIdx >= 0 Then
                        Dim p = Math.Max(dist(childIdx), 0.001)  ' Avoid log(0)
                        logLik += Math.Log(p)
                    End If
                Next
            Next

            Return logLik
        End Function


        ' ==================== Utility Methods ====================

        ''' <summary>Get discretization thresholds for a node</summary>
        Private Function GetThresholds(
            nodeId As String,
            customThresholds As Dictionary(Of String, Tuple(Of Double, Double))
        ) As Tuple(Of Double, Double)
            If customThresholds IsNot Nothing AndAlso customThresholds.ContainsKey(nodeId) Then
                Return customThresholds(nodeId)
            End If

            ' per-node 阈值：学习侧与推理侧必须共用同一套阈值，
            ' 否则会出现"学习用自适应阈值、推理用固定 0.33/0.66"的不一致
            If _config.NodeThresholds IsNot Nothing AndAlso _config.NodeThresholds.ContainsKey(nodeId) Then
                Return _config.NodeThresholds(nodeId)
            End If

            Return New Tuple(Of Double, Double)(_config.LowThreshold, _config.HighThreshold)
        End Function


        ''' <summary>Discretize a continuous value into Low/Medium/High</summary>
        Private Function DiscretizeValue(
            value As Double,
            lowThreshold As Double,
            highThreshold As Double
        ) As String
            If value < lowThreshold Then Return "Low"
            If value < highThreshold Then Return "Medium"
            Return "High"
        End Function


        ''' <summary>Get a node by ID (returns Nothing if not found)</summary>
        Public Function GetNode(id As String) As DBNNode
            If _nodes.ContainsKey(id) Then Return _nodes(id)
            Return Nothing
        End Function


        ''' <summary>Get all nodes in the network</summary>
        Public Function GetAllNodes() As List(Of DBNNode)
            Return _nodes.Values.ToList()
        End Function


        ''' <summary>Get all gene/operon nodes</summary>
        Public Function GetGeneNodes() As List(Of DBNNode)
            Return _nodes.Values.Where(Function(n) n.NodeType = DBNNodeType.Gene).ToList()
        End Function


        ''' <summary>Get all transcription factor nodes</summary>
        Public Function GetTFNodes() As List(Of DBNNode)
            Return _nodes.Values.Where(Function(n) n.NodeType = DBNNodeType.TranscriptionFactor).ToList()
        End Function


        ''' <summary>Get all effector metabolite nodes</summary>
        Public Function GetMetaboliteNodes() As List(Of DBNNode)
            Return _nodes.Values.Where(Function(n) n.NodeType = DBNNodeType.EffectorMetabolite).ToList()
        End Function


        ''' <summary>Get the operon-to-genes mapping</summary>
        Public Function GetOperonGeneMapping() As Dictionary(Of String, List(Of String))
            Return New Dictionary(Of String, List(Of String))(_operonGenes)
        End Function


        ''' <summary>
        ''' Compute the marginal distribution of a node (averaging over all parent configs).
        ''' Useful for initialization and debugging.
        ''' </summary>
        Public Function GetMarginalDistribution(nodeId As String) As Double()
            Dim node = GetNode(nodeId)
            If node Is Nothing Then Return Nothing

            If node.ParentIds.Count = 0 Then
                Return node.CPT.GetDistribution(New List(Of String))
            End If

            Dim marginal(node.States.Count - 1) As Double
            Dim parentStatesMap As Dictionary(Of String, List(Of String)) = GetParentStatesMap(node)
            Dim rows As Long = node.CPT.GetConfigurationCount(parentStatesMap)

            If rows <= _config.MaxCPTRows Then
                ' 规模可控：枚举全部父配置求平均
                Dim n As Integer = 0

                For Each cfg In node.CPT.GetAllParentConfigurations(parentStatesMap)
                    Dim dist = node.CPT.GetDistribution(cfg, copy:=False)

                    n += 1
                    For i = 0 To marginal.Length - 1
                        marginal(i) += dist(i)
                    Next
                Next

                If n > 0 Then
                    For i = 0 To marginal.Length - 1
                        marginal(i) /= n
                    Next
                End If
            Else
                ' 配置空间过大（惰性 CPT）：改用蒙特卡洛采样估计，避免在此处再次触发 3^P 爆炸
                Dim n As Integer = If(_config.MarginalSampleSize > 0, _config.MarginalSampleSize, 4096)
                Dim dims As New List(Of List(Of String))
                Dim cfg As New List(Of String)(node.ParentIds.Count)

                For Each pid As String In node.ParentIds
                    If parentStatesMap.ContainsKey(pid) Then dims.Add(parentStatesMap(pid))
                Next

                For s As Integer = 0 To n - 1
                    cfg.Clear()

                    For d As Integer = 0 To dims.Count - 1
                        cfg.Add(dims(d)(_rng.Next(dims(d).Count)))
                    Next

                    Dim dist = node.CPT.GetDistribution(cfg, copy:=False)

                    For i = 0 To marginal.Length - 1
                        marginal(i) += dist(i)
                    Next
                Next

                For i = 0 To marginal.Length - 1
                    marginal(i) /= n
                Next
            End If

            Return marginal
        End Function


        ''' <summary>Reset all CPTs to topology-based defaults (discard learned parameters)</summary>
        Public Sub ResetToTopologyPrior()
            For Each node In _nodes.Values
                InitializeCPT(node)
            Next
        End Sub


        ''' <summary>Get a summary string of the DBN structure (for debugging)</summary>
        Public Function GetSummary() As String
            Dim sb As New StringBuilder()
            sb.AppendLine("=== Dynamic Bayesian Network Summary ===")
            sb.AppendLine(String.Format("Total nodes: {0}", _nodes.Count))
            sb.AppendLine(String.Format("  Genes/Operons: {0}", GetGeneNodes().Count))
            sb.AppendLine(String.Format("  Transcription Factors: {0}", GetTFNodes().Count))
            sb.AppendLine(String.Format("  Effector Metabolites: {0}", GetMetaboliteNodes().Count))
            sb.AppendLine()
            sb.AppendLine("Node Details:")
            For Each node In _nodes.Values
                sb.AppendLine(String.Format("  [{0}] {1} (parents: {2})",
                    node.NodeType.ToString(),
                    node.NodeId,
                    If(node.ParentIds.Count = 0, "none", String.Join(", ", node.ParentIds))))
            Next
            Return sb.ToString()
        End Function


        ' ==================== Persistence (Save / Load) ====================

        ''' <summary>
        ''' Save the DBN (structure + learned parameters) to a text file.
        ''' Format: simple pipe-delimited text, human-readable.
        ''' </summary>
        Public Sub SaveToFile(filePath As String)
            Using writer As New StreamWriter(filePath)
                writer.WriteLine("# Dynamic Bayesian Network")
                writer.WriteLine("# Format: DBN_V1")
                writer.WriteLine()

                ' Write nodes
                writer.WriteLine("NODES {0}", _nodes.Count)
                For Each node In _nodes.Values
                    writer.WriteLine("NODE|{0}|{1}|{2}|{3}",
                        node.NodeId,
                        CInt(node.NodeType),
                        String.Join(",", node.States),
                        String.Join(",", node.ParentIds))
                Next
                writer.WriteLine()

                ' Write CPTs
                ' 惰性 CPT 节点只持久化"实际访问过的配置"，这里把这类节点登记在文件头部，
                ' 使落盘结果可以被正确解读：缺失的配置在载入后由 OnDemandProvider 现场计算。
                Dim lazyNodes = _nodes.Values _
                    .Where(Function(n) n.CPT.OnDemandProvider IsNot Nothing) _
                    .Select(Function(n) n.NodeId) _
                    .ToArray

                If lazyNodes.Length > 0 Then
                    writer.WriteLine("# LAZY_NODES {0}", String.Join(",", lazyNodes))
                End If

                writer.WriteLine("CPTS")
                For Each node In _nodes.Values
                    For Each kv In node.CPT.Table
                        writer.WriteLine("CPT|{0}|{1}|{2}",
                            node.NodeId,
                            kv.Key,
                            String.Join(",", kv.Value.Select(
                                Function(d) d.ToString("G6", CultureInfo.InvariantCulture))))
                    Next
                Next
            End Using
        End Sub


        ''' <summary>Load the DBN from a text file (saved by SaveToFile)</summary>
        Public Sub LoadFromFile(filePath As String)
            _nodes.Clear()
            _operonGenes.Clear()
            _activationModels.Clear()

            Dim lines = File.ReadAllLines(filePath)

            For Each line In lines
                If String.IsNullOrWhiteSpace(line) OrElse line.StartsWith("#") Then Continue For

                Dim parts = line.Split("|"c)
                If parts.Length = 0 Then Continue For

                Select Case parts(0)
                    Case "NODE"
                        If parts.Length >= 5 Then
                            Dim id = parts(1)
                            Dim type = CType(Integer.Parse(parts(2)), DBNNodeType)
                            Dim states = parts(3).Split(","c).ToList()
                            Dim parents = If(String.IsNullOrEmpty(parts(4)),
                                New List(Of String)(),
                                parts(4).Split(","c).ToList())

                            Dim node As New DBNNode(id, type)
                            node.States = states
                            node.ParentIds = parents
                            node.CPT.VariableId = id
                            node.CPT.ParentIds = parents
                            node.CPT.States = states
                            _nodes(id) = node
                        End If

                    Case "CPT"
                        If parts.Length >= 4 Then
                            Dim nodeId = parts(1)
                            Dim key = parts(2)
                            Dim probs = parts.Last.Split(","c).Select(Function(s) Double.Parse(s, CultureInfo.InvariantCulture)).ToArray()

                            If _nodes.ContainsKey(nodeId) Then
                                _nodes(nodeId).CPT.Table(key) = probs
                            End If
                        End If
                End Select
            Next

            ' 文件只保存了结构与参数：父子下标索引与按需计算委托需要在内存中重建，
            ' 否则惰性 CPT 节点在查询未保存的配置时会退化成均匀分布。
            Call BuildActivationModels()

            For Each n In _nodes.Values
                If n.ParentIds.Count = 0 Then Continue For

                Dim node = n

                n.CPT.MaxCacheRows = _config.MaxCPTCacheRows

                If n.CPT.GetConfigurationCount(GetParentStatesMap(n)) > _config.MaxCPTRows Then
                    n.CPT.OnDemandProvider = Function(cfg) ComputeDefaultDistribution(node, cfg)
                End If
            Next
        End Sub

    End Class

    ''' <summary>
    ''' 节点激活模型的预计算结果：把 TF / effector 的父下标与调控方向解析成定长数组。
    ''' 
    ''' 原始实现在每一个父配置里都要对 ParentIds 做 IndexOf 字符串查找，
    ''' 单次 O(P) 比较 × 每个配置 O(P) 次 = O(P²)，在 3^P 个配置上总复杂度为 O(3^P·P²)。
    ''' 预计算之后每个配置只需 O(P) 次数组取值，无字符串比较、无装箱。
    ''' </summary>
    Public Class ActivationModel

        ''' <summary>第 k 个调控项对应的 TF 在 ParentIds 中的下标</summary>
        Public Property tfIdx As Integer()

        ''' <summary>第 k 个调控项对应的 effector 在 ParentIds 中的下标（-1 表示该 TF 无 effector）</summary>
        Public Property effIdx As Integer()

        ''' <summary>第 k 个调控项是否为抑制性调控</summary>
        Public Property isInhibitor As Boolean()

        ''' <summary>调控项数量</summary>
        Public ReadOnly Property Count As Integer
            Get
                Return If(tfIdx Is Nothing, 0, tfIdx.Length)
            End Get
        End Property

    End Class


End Namespace

