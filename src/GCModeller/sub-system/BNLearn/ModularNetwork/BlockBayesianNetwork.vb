Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn.DBN
Imports SMRUCC.genomics.Analysis.BNLearn.Inference

Namespace ModularNetwork

    Public Class BlockBayesianNetwork

        Public Property moduleDBs As ModuleDBN()
        ''' <summary>
        ''' ③ 模块间关联图（基于 eigengene 轨迹相关度）
        ''' </summary>
        ''' <returns></returns>
        Public Property graph As Dictionary(Of String, List(Of (modColor As String, weight As Double)))

        Public Property TF As String()

        ''' <summary>
        ''' get length of <see cref="moduleDBs"/> array
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property blocks As Integer
            Get
                Return moduleDBs.TryCount
            End Get
        End Property

        Public ReadOnly Property allgenes As String()
            Get
                Return moduleDBs.SelectMany(Function(m) m.Genes).Distinct().ToArray()
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="subblocks"></param>
        ''' <param name="TFs"></param>
        ''' <param name="crossModuleCorThreshold">
        ''' 模块 eigengene 相关阈值：|cor| 超过才建立模块间关联，默认 0.3。
        ''' </param>
        Sub New(subblocks As IEnumerable(Of ModuleDBN), TFs As IEnumerable(Of String), Optional crossModuleCorThreshold As Double = 0.3)
            moduleDBs = subblocks.SafeQuery.ToArray
            graph = BuildModuleCorrelationGraph(moduleDBs, crossModuleCorThreshold)
            TF = TFs.SafeQuery.ToArray

            Call $"GRN.TrainModularDBNIntervene: 模块关联边数={graph.Values.Sum(Function(l) l.Count)}".info
        End Sub

        ''' <summary>
        ''' 对单个扰动基因执行全局级联虚拟扰动：
        '''   - 在其所属模块内固定 Low 并多步推演本模块基因状态轨迹；
        '''   - 计算本模块 eigengene 变化，沿模块关联图 BFS 逐级注入下游模块（作为模块整体状态偏置），
        '''     在下游模块内做受迫推演，形成级联；
        '''   - 汇总所有模块基因的最终状态为全局响应向量（按 allGenes 顺序，Low=0/Med=1/High=2）。
        ''' </summary>
        Public Function CascadeIntervene(knockGene As String, steps As Integer, trajectories As Dictionary(Of String, Dictionary(Of String, List(Of Double)))) As Double()
            ' 定位扰动基因所属模块
            Dim m0 As ModuleDBN = Nothing
            Dim allGenes As String() = Me.allgenes
            Dim tfSet As New HashSet(Of String)(TF)

            For Each m In moduleDBs
                If m.GeneIndex.ContainsKey(knockGene) Then
                    m0 = m
                    Exit For
                End If
            Next
            If m0 Is Nothing Then
                Call $"GRN.CascadeIntervene: 警告: 扰动基因 '{knockGene}' 不在任何模块中，跳过".info
                Dim zero As Double() = allGenes.Select(Function(g) 1.0).ToArray()
                trajectories(knockGene) = New Dictionary(Of String, List(Of Double))
                Return zero
            End If

            ' 每个模块维护基因离散状态（初始 Medium），以及各自的轨迹容器
            Dim moduleStates As New Dictionary(Of String, Dictionary(Of String, String))
            Dim moduleTraj As New Dictionary(Of String, Dictionary(Of String, List(Of Double)))
            For Each m In moduleDBs
                Dim st As New Dictionary(Of String, String)
                Dim tr As New Dictionary(Of String, List(Of Double))
                For Each g In m.Genes
                    st(g) = "Medium"
                    tr(g) = New List(Of Double)(New Double(steps - 1) {})
                Next
                moduleStates(m.ModuleColor) = st
                moduleTraj(m.ModuleColor) = tr
            Next

            ' 初始步：扰动基因固定 Low
            moduleStates(m0.ModuleColor)(knockGene) = "Low"
            For Each g In m0.Genes
                moduleTraj(m0.ModuleColor)(g)(0) = StateToValue(moduleStates(m0.ModuleColor)(g))
            Next

            ' 本模块多步推演
            Dim m0Rates = RunModuleSteps(m0, moduleStates(m0.ModuleColor), knockGene, steps, tfSet, moduleTraj(m0.ModuleColor))
            ' 计算本模块 eigengene 变化（最终步 RNA 速率均值）
            Dim delta0 = If(m0Rates.Count > 0, m0Rates.Values.Average(), 0.0)

            ' 沿模块关联图 BFS 级联
            Dim visited As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase) From {m0.ModuleColor}
            Dim queue As New Queue(Of (modColor As String, delta As Double))
            queue.Enqueue((modColor:=m0.ModuleColor, delta:=delta0))

            While queue.Count > 0
                Dim cur = queue.Dequeue()
                If Not graph.ContainsKey(cur.modColor) Then Continue While
                For Each adj In graph(cur.modColor)
                    If visited.Contains(adj.modColor) Then Continue For
                    visited.Add(adj.modColor)
                    Dim mNext = moduleDBs.First(Function(m) String.Equals(m.ModuleColor, adj.modColor, StringComparison.OrdinalIgnoreCase))
                    ' 上游变化按关联权重注入下游模块（作为模块整体状态偏置）
                    Dim upstreamDelta = cur.delta * adj.weight
                    Dim fixedInNext = If(mNext.GeneIndex.ContainsKey(knockGene), knockGene, Nothing)
                    Dim nextRates = RunModuleForced(mNext, upstreamDelta, fixedInNext, steps, tfSet, moduleStates(mNext.ModuleColor), moduleTraj(mNext.ModuleColor))
                    Dim deltaNext = If(nextRates.Count > 0, nextRates.Values.Average(), 0.0)
                    queue.Enqueue((modColor:=mNext.ModuleColor, delta:=deltaNext))
                Next
            End While

            ' 汇总全局最终响应向量（显式双层循环，避免 SelectMany 对 Double() 轨迹的深层展平）
            Dim geneToTraj As New Dictionary(Of String, List(Of Double))(StringComparer.OrdinalIgnoreCase)
            For Each kvModule In moduleTraj
                For Each kvGene In kvModule.Value
                    geneToTraj(kvGene.Key) = kvGene.Value
                Next
            Next

            Dim resp(allGenes.Length - 1) As Double
            For i = 0 To allGenes.Length - 1
                Dim g = allGenes(i)
                If geneToTraj.ContainsKey(g) Then
                    resp(i) = geneToTraj(g)(steps - 1)
                Else
                    resp(i) = 1.0  ' 未参与任何模块：中性 Medium
                End If
            Next

            Dim trajMerged As New Dictionary(Of String, List(Of Double))(StringComparer.OrdinalIgnoreCase)
            For Each kvModule In moduleTraj
                For Each kvGene In kvModule.Value
                    trajMerged(kvGene.Key) = kvGene.Value
                Next
            Next
            trajectories(knockGene) = trajMerged

            Call $"GRN.CascadeIntervene: 对基因 '{knockGene}'（模块 {m0.ModuleColor}）完成级联虚拟扰动，本模块 eigengene 变化 δ={delta0:F4}".info
            Return resp
        End Function

        ''' <summary>
        ''' 在单个模块子网络内多步推演（扰动基因固定 Low）。返回各基因最终 RNA 丰度变化率。
        ''' </summary>
        Private Function RunModuleSteps(m As ModuleDBN,
                                        geneStates As Dictionary(Of String, String),
                                        fixedGene As String,
                                        steps As Integer,
                                        tfSet As HashSet(Of String),
                                        traj As Dictionary(Of String, List(Of Double))) As Dictionary(Of String, Double)
            Dim lastRates As New Dictionary(Of String, Double)

            For t As Integer = 1 To steps - 1
                ' 模块内 TF 基因的连续 abundance（由当前离散状态映射，与证据一致）
                Dim tfAbund As New Dictionary(Of String, Double)

                For Each gene_id As String In m.Genes
                    If tfSet.Contains(gene_id) Then
                        tfAbund(gene_id) = StateToScore(geneStates(gene_id))
                    End If
                Next

                Dim result As DBNPredictionResult = m.Net.PredictNextState(Nothing, tfAbund, geneStates)

                For Each gene_id As String In m.Genes
                    If result.GeneStates.ContainsKey(gene_id) Then
                        geneStates(gene_id) = result.GeneStates(gene_id)
                    End If
                    ' 持续固定扰动基因 Low，避免被反馈回路恢复
                    If Not String.IsNullOrEmpty(fixedGene) Then
                        geneStates(fixedGene) = "Low"
                    End If

                    traj(gene_id)(t) = StateToValue(geneStates(gene_id))
                Next

                For Each gene_id As String In m.Genes
                    If result.RNAAbundanceChanges.ContainsKey(gene_id) Then
                        lastRates(gene_id) = result.RNAAbundanceChanges(gene_id)
                    End If
                Next
            Next

            Return lastRates
        End Function

        ''' <summary>
        ''' 受迫推演：下游模块接收上游 eigengene 变化偏置，初始整体状态偏移后多步推演。
        ''' </summary>
        Private Function RunModuleForced(m As ModuleDBN,
                                         upstreamDelta As Double,
                                         fixedGene As String,
                                         steps As Integer,
                                         tfSet As HashSet(Of String),
                                         geneStates As Dictionary(Of String, String),
                                         traj As Dictionary(Of String, List(Of Double))) As Dictionary(Of String, Double)
            ' 初始整体状态偏置：上游正向变化 → High，负向 → Low，近 0 → Medium
            Dim initState As String = If(upstreamDelta > 0.1, "High", If(upstreamDelta < -0.1, "Low", "Medium"))
            For Each g In m.Genes
                geneStates(g) = initState
            Next
            If Not String.IsNullOrEmpty(fixedGene) Then geneStates(fixedGene) = "Low"
            For Each g In m.Genes
                traj(g)(0) = StateToValue(geneStates(g))
            Next

            Dim lastRates As New Dictionary(Of String, Double)
            For t = 1 To steps - 1
                Dim tfAbund As New Dictionary(Of String, Double)
                For Each g In m.Genes
                    If tfSet.Contains(g) Then
                        ' 上游变化注入 TF abundance（clamp 到合理范围）
                        tfAbund(g) = Math.Max(0.0, Math.Min(2.0, StateToScore(geneStates(g)) * (1.0 + upstreamDelta)))
                    End If
                Next

                Dim result = m.Net.PredictNextState(Nothing, tfAbund, geneStates)
                For Each g In m.Genes
                    If result.GeneStates.ContainsKey(g) Then
                        geneStates(g) = result.GeneStates(g)
                    End If
                    If Not String.IsNullOrEmpty(fixedGene) Then geneStates(fixedGene) = "Low"
                    traj(g)(t) = StateToValue(geneStates(g))
                Next
                For Each g In m.Genes
                    If result.RNAAbundanceChanges.ContainsKey(g) Then lastRates(g) = result.RNAAbundanceChanges(g)
                Next
            Next

            Return lastRates
        End Function

        ''' <summary>
        ''' 模型文件格式版本（用于 LoadModel 的兼容性校验）
        ''' </summary>
        Private Const ModelFormatVersion As Integer = 1

        ''' <summary>
        ''' save model as zip archive file
        ''' </summary>
        ''' <param name="file">
        ''' 目标输出流。由 R# 的 ``writeBin`` 传入（文件路径或连接），调用方负责流的释放，
        ''' 因此这里以 leaveOpen 的方式使用 ZipArchive。
        ''' </param>
        ''' 
        ''' zip 布局：
        ''' ```
        ''' meta.txt            version / blocks / tf_count
        ''' TF.txt              每行一个 TF id
        ''' graph.tsv           from, to, weight
        ''' modules/0000/
        '''     color.txt       模块颜色
        '''     genes.txt       每行一个基因
        '''     eigengene.txt   每行一个 double
        '''     links.tsv       由网络节点反推出来的调控边（用于重建拓扑）
        '''     cpt.tsv         nodeId, key, p1,p2,p3
        ''' ```
        ''' 
        ''' 说明：之所以保存拓扑边而不是直接用 <see cref="DynamicBayesianNetwork.SaveToFile"/>，
        ''' 是因为后者不保存 RegulatorTFs / TFEffectors / EffectorMetabolites，加载后激活得分会
        ''' 恒为 0.5，导致惰性 CPT 在未缓存配置上退化为 basal 分布。保存拓扑后可由
        ''' BuildFromTopology 完整重建结构语义，再把学到的 CPT 条目回填。
        Public Sub SaveModel(file As Stream)
            Dim blocks As ModuleDBN() = If(moduleDBs, New ModuleDBN() {})

            Using zip As New ZipArchive(file, ZipArchiveMode.Create, leaveOpen:=True)
                Call WriteText(zip, "meta.txt", Sub(w)
                                                    w.WriteLine($"version={ModelFormatVersion}")
                                                    w.WriteLine($"blocks={blocks.Length}")
                                                    w.WriteLine($"tf_count={TF.TryCount}")
                                                End Sub)

                Call WriteText(zip, "TF.txt", Sub(w)
                                                  For Each id As String In TF.SafeQuery
                                                      w.WriteLine(id)
                                                  Next
                                              End Sub)

                Call WriteText(zip, "graph.tsv", Sub(w)
                                                     w.WriteLine(String.Join(vbTab, {"from", "to", "weight"}))

                                                     If graph IsNot Nothing Then
                                                         For Each kv In graph
                                                             For Each adj In kv.Value
                                                                 w.WriteLine(String.Join(vbTab, {
                                                                     kv.Key,
                                                                     adj.modColor,
                                                                     adj.weight.ToString(CultureInfo.InvariantCulture)
                                                                 }))
                                                             Next
                                                         Next
                                                     End If
                                                 End Sub)

                For i As Integer = 0 To blocks.Length - 1
                    Dim dir As String = ModuleDir(i)
                    Dim m As ModuleDBN = blocks(i)

                    Call WriteText(zip, dir & "color.txt", Sub(w) w.WriteLine(m.ModuleColor))

                    Call WriteText(zip, dir & "genes.txt", Sub(w)
                                                               For Each g As String In m.Genes.SafeQuery
                                                                   w.WriteLine(g)
                                                               Next
                                                           End Sub)

                    Call WriteText(zip, dir & "eigengene.txt", Sub(w)
                                                                   For Each x As Double In m.Eigengene.SafeQuery
                                                                       w.WriteLine(x.ToString(CultureInfo.InvariantCulture))
                                                                   Next
                                                               End Sub)

                    Call WriteText(zip, dir & "links.tsv", Sub(w) Call WriteLinks(w, ExportLinks(m.Net)))
                    Call WriteText(zip, dir & "cpt.tsv", Sub(w) Call WriteCPT(w, m.Net))
                Next
            End Using

            Call $"[BlockBayesianNetwork] 模型已导出: blocks={blocks.Length}, genes={allgenes.Length}".info
        End Sub

        ''' <summary>
        ''' load model from zip archive file
        ''' </summary>
        ''' <param name="file">zip 压缩包输入流（由 R# 的 ``readBin`` 传入，调用方负责释放）</param>
        ''' <returns>还原后的模块化贝叶斯网络模型，可直接用于级联虚拟扰动</returns>
        Public Shared Function LoadModel(file As Stream) As BlockBayesianNetwork
            Dim modules As New List(Of ModuleDBN)
            Dim tfList As String() = {}
            Dim links As Dictionary(Of String, List(Of (modColor As String, weight As Double)))
            Dim blockCount As Integer = 0

            Using zip As New ZipArchive(file, ZipArchiveMode.Read, leaveOpen:=True)
                Dim meta As Dictionary(Of String, String) = ReadMeta(GetEntry(zip, "meta.txt"))
                Dim verText As String = Nothing
                Dim blocksText As String = Nothing
                Dim version As Integer = 0

                If meta Is Nothing OrElse Not meta.TryGetValue("version", verText) Then
                    Throw New InvalidDataException("bnlearn 模型文件缺少版本信息，可能不是有效的模型压缩包。")
                End If
                If Not Integer.TryParse(verText, version) OrElse version <> ModelFormatVersion Then
                    Throw New InvalidDataException($"bnlearn 模型文件版本不匹配：文件为 {verText}，当前程序支持 {ModelFormatVersion}。")
                End If
                If Not meta.TryGetValue("blocks", blocksText) OrElse Not Integer.TryParse(blocksText, blockCount) Then
                    Throw New InvalidDataException("bnlearn 模型文件缺少模块数量信息（blocks）。")
                End If
                tfList = ReadLines(GetEntry(zip, "TF.txt"))
                links = ReadGraph(GetEntry(zip, "graph.tsv"))

                For i As Integer = 0 To blockCount - 1
                    Dim dir As String = ModuleDir(i)
                    Dim color As String = ReadLines(GetEntry(zip, dir & "color.txt")).FirstOrDefault()
                    Dim genes As String() = ReadLines(GetEntry(zip, dir & "genes.txt"))
                    Dim eig As Double() = ReadLines(GetEntry(zip, dir & "eigengene.txt")) _
                        .Select(Function(s) Double.Parse(s, CultureInfo.InvariantCulture)) _
                        .ToArray()
                    Dim topology As RegulatoryLink() = ReadLinks(GetEntry(zip, dir & "links.tsv"))

                    ' 由拓扑重建：会自动恢复 ParentIds / RegulatorTFs / TFEffectors /
                    ' EffectorMetabolites 以及惰性节点的按需计算委托，与训练时语义一致。
                    Dim net As DynamicBayesianNetwork = New DynamicBayesianNetwork().BuildFromTopology(topology)

                    ' 再把学习到的（或已缓存的）CPT 参数回填
                    Call ImportCPT(GetEntry(zip, dir & "cpt.tsv"), net)

                    Dim m As New ModuleDBN With {
                        .ModuleColor = color,
                        .Genes = genes,
                        .Net = net,
                        .Eigengene = eig
                    }

                    For j As Integer = 0 To genes.Length - 1
                        m.GeneIndex(genes(j)) = j
                    Next

                    modules.Add(m)
                Next
            End Using

            Dim model As New BlockBayesianNetwork(modules, tfList)

            ' 构造函数会用 eigengene 重算模块关联图（默认阈值 0.3），
            ' 这里以文件中保存的图覆盖，避免依赖阈值一致性
            model.graph = links

            Call $"[BlockBayesianNetwork] 模型已载入: blocks={model.blocks}, genes={model.allgenes.Length}".info

            Return model
        End Function

        ''' <summary>模块在 zip 内的目录名（用序号命名，避免模块颜色中的特殊字符影响 entry 名）</summary>
        Private Shared Function ModuleDir(index As Integer) As String
            Return $"modules/{index.ToString("D4")}/"
        End Function

        Private Shared Sub WriteText(zip As ZipArchive, name As String, write As Action(Of TextWriter))
            Dim entry As ZipArchiveEntry = zip.CreateEntry(name, CompressionLevel.Optimal)

            Using w As New StreamWriter(entry.Open())
                Call write(w)
            End Using
        End Sub

        Private Shared Function GetEntry(zip As ZipArchive, name As String) As ZipArchiveEntry
            Dim target As String = name.Replace("\"c, "/"c)

            For Each e As ZipArchiveEntry In zip.Entries
                If String.Equals(e.FullName.Replace("\"c, "/"c), target, StringComparison.OrdinalIgnoreCase) Then
                    Return e
                End If
            Next

            Return Nothing
        End Function

        Private Shared Function ReadLines(entry As ZipArchiveEntry) As String()
            If entry Is Nothing Then
                Return New String() {}
            End If

            Dim lines As New List(Of String)

            Using sr As New StreamReader(entry.Open())
                Do While Not sr.EndOfStream
                    Dim line As String = sr.ReadLine()

                    If Not String.IsNullOrWhiteSpace(line) Then
                        lines.Add(line)
                    End If
                Loop
            End Using

            Return lines.ToArray()
        End Function

        Private Shared Function ReadMeta(entry As ZipArchiveEntry) As Dictionary(Of String, String)
            Dim meta As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            For Each line As String In ReadLines(entry)
                Dim i As Integer = line.IndexOf("="c)

                If i <= 0 Then Continue For

                meta(line.Substring(0, i).Trim()) = line.Substring(i + 1).Trim()
            Next

            Return meta
        End Function

        Private Shared Function ReadGraph(entry As ZipArchiveEntry) As Dictionary(Of String, List(Of (modColor As String, weight As Double)))
            Dim g As New Dictionary(Of String, List(Of (modColor As String, weight As Double)))

            For Each line As String In ReadLines(entry)
                If line.StartsWith("from", StringComparison.OrdinalIgnoreCase) Then Continue For

                Dim parts As String() = line.Split(ChrW(9))

                If parts.Length < 3 Then Continue For
                If Not g.ContainsKey(parts(0)) Then
                    g(parts(0)) = New List(Of (modColor As String, weight As Double))
                End If

                g(parts(0)).Add((modColor:=parts(1), weight:=Double.Parse(parts(2), CultureInfo.InvariantCulture)))
            Next

            Return g
        End Function

        ''' <summary>
        ''' 从已训练网络的节点状态反推出调控边集合（用于重建拓扑）。
        ''' 
        ''' 网络内部保存的是 ParentIds / RegulatorTFs / TFEffectors 等"展开后"的结构，
        ''' 而 <see cref="DynamicBayesianNetwork.BuildFromTopology"/> 需要的是调控边，
        ''' 这里把前者还原成后者：一条 (基因, 其调控 TF) 对应一条边，effector 的类型
        ''' 从 TF 节点的 EffectorMetabolites 取回。
        ''' </summary>
        Private Shared Iterator Function ExportLinks(net As DynamicBayesianNetwork) As IEnumerable(Of RegulatoryLink)
            If net Is Nothing Then Return

            For Each node As DBNNode In net.GetAllNodes()
                If node.NodeType <> DBNNodeType.Gene Then Continue For

                For Each tfId As String In node.RegulatorTFs
                    Dim effMap As Dictionary(Of String, Effector) = Nothing
                    Dim effIds As List(Of String) = Nothing

                    If node.TFEffectors.TryGetValue(tfId, effIds) AndAlso effIds IsNot Nothing AndAlso effIds.Count > 0 Then
                        Dim tfNode As DBNNode = net.GetNode(tfId)

                        effMap = New Dictionary(Of String, Effector)

                        For Each effId As String In effIds
                            Dim effType As Effector = Effector.Unknown

                            If tfNode IsNot Nothing AndAlso tfNode.EffectorMetabolites.ContainsKey(effId) Then
                                effType = tfNode.EffectorMetabolites(effId)
                            End If

                            effMap(effId) = effType
                        Next
                    End If

                    Yield New RegulatoryLink With {
                        .TF_id = tfId,
                        .target_operon = node.NodeId,
                        .regulate_genes = {node.NodeId},
                        .effector = effMap
                    }
                Next
            Next
        End Function

        ''' <summary>
        ''' 写出调控边（制表符分隔：TF_id / TF_family / TFBS_id / target_operon / genes / effectors）
        ''' </summary>
        Private Shared Sub WriteLinks(w As TextWriter, links As IEnumerable(Of RegulatoryLink))
            Dim tab As String = ChrW(9)

            For Each l As RegulatoryLink In links
                Dim genes As String = If(l.regulate_genes Is Nothing, "", String.Join(";", l.regulate_genes))
                Dim effectors As String = ""

                If l.effector IsNot Nothing AndAlso l.effector.Count > 0 Then
                    effectors = String.Join(";", l.effector.Select(Function(kv) $"{kv.Key}:{CInt(kv.Value)}"))
                End If

                w.WriteLine(String.Join(tab, {
                    Text(l.TF_id),
                    Text(l.TF_family),
                    Text(l.TFBS_id),
                    Text(l.target_operon),
                    genes,
                    effectors
                }))
            Next
        End Sub

        Private Shared Function ReadLinks(entry As ZipArchiveEntry) As RegulatoryLink()
            Dim links As New List(Of RegulatoryLink)

            For Each line As String In ReadLines(entry)
                Dim parts As String() = line.Split(ChrW(9))
                Dim genes As String() = If(parts.Length > 4 AndAlso parts(4).Length > 0,
                    parts(4).Split(";"c),
                    New String() {})
                Dim effMap As Dictionary(Of String, Effector) = Nothing

                If parts.Length > 5 AndAlso parts(5).Length > 0 Then
                    effMap = New Dictionary(Of String, Effector)

                    For Each item As String In parts(5).Split(";"c)
                        Dim kv As String() = item.Split(":"c)

                        If kv.Length <> 2 Then Continue For

                        effMap(kv(0)) = CType(Integer.Parse(kv(1), CultureInfo.InvariantCulture), Effector)
                    Next
                End If

                links.Add(New RegulatoryLink With {
                    .TF_id = parts(0),
                    .TF_family = If(parts.Length > 1, parts(1), Nothing),
                    .TFBS_id = If(parts.Length > 2, parts(2), Nothing),
                    .target_operon = If(parts.Length > 3, parts(3), Nothing),
                    .regulate_genes = genes,
                    .effector = effMap
                })
            Next

            Return links.ToArray()
        End Function

        ''' <summary>
        ''' 流式写出全部 CPT 条目：nodeId / key / p1,p2,p3
        ''' 
        ''' 必须使用制表符而非 "|" 作为字段分隔符：CPT 的 key 本身就是用 "|" 连接各父状态
        ''' 得到的，用 "|" 切分会把 key 截断（既有的 SaveToFile / LoadFromFile 就有这个问题）。
        ''' 概率用 G17 保证 double 往返无损。
        ''' </summary>
        Private Shared Sub WriteCPT(w As TextWriter, net As DynamicBayesianNetwork)
            If net Is Nothing Then Return

            Dim tab As String = ChrW(9)

            For Each node As DBNNode In net.GetAllNodes()
                For Each kv In node.CPT.Table
                    w.WriteLine(String.Join(tab, {
                        node.NodeId,
                        kv.Key,
                        String.Join(",", kv.Value.Select(Function(d) d.ToString("G17", CultureInfo.InvariantCulture)))
                    }))
                Next
            Next
        End Sub

        ''' <summary>
        ''' 回填 CPT 条目。只写 node.CPT.Table(key)，不触碰 OnDemandProvider / MaxCacheRows，
        ''' 否则惰性节点会退化成均匀分布。
        ''' </summary>
        Private Shared Sub ImportCPT(entry As ZipArchiveEntry, net As DynamicBayesianNetwork)
            If entry Is Nothing OrElse net Is Nothing Then Return

            Using sr As New StreamReader(entry.Open())
                Do While Not sr.EndOfStream
                    Dim line As String = sr.ReadLine()

                    If String.IsNullOrWhiteSpace(line) Then Continue Do

                    Dim parts As String() = line.Split(ChrW(9))

                    If parts.Length < 3 Then Continue Do

                    Dim node As DBNNode = net.GetNode(parts(0))

                    If node Is Nothing Then Continue Do

                    Dim probs As Double() = parts(2).Split(","c) _
                        .Select(Function(s) Double.Parse(s, CultureInfo.InvariantCulture)) _
                        .ToArray()

                    node.CPT.Table(parts(1)) = probs
                Loop
            End Using
        End Sub

        ''' <summary>文本字段规整：Nothing 转空串，并剔除会破坏行/列结构的制表符与换行</summary>
        Private Shared Function Text(s As String) As String
            If String.IsNullOrEmpty(s) Then Return ""

            Return s.Replace(vbTab, " ").Replace(vbCr, " ").Replace(vbLf, " ")
        End Function
    End Class
End Namespace