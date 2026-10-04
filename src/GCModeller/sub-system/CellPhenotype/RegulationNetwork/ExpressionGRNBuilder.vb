Imports System.Diagnostics
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.WGCNA

Namespace RegulationNetwork

    ''' <summary>
    ''' 基因表达调控先验网络（<see cref="PriorNetwork"/>）构建流水线
    ''' </summary>
    ''' <remarks>
    ''' 端到端地把「多数据集合并的大型表达矩阵」转换为可用于 bnlearn 动态贝叶斯网络与
    ''' GEARS 图神经网络的先验知识网络：
    '''
    ''' <list type="number">
    ''' <item>按数据集分组做批次内标准化（<see cref="BatchNormalizer"/>）；</item>
    ''' <item>基因质量过滤（缺失率 + 方差，<see cref="GeneFilter"/>）；</item>
    ''' <item>WGCNA 模块划分（软阈值邻接 + TOM + 动态剪切）；</item>
    ''' <item>模块内 bicor 稳健相关矩阵（带符号，用于判定激活 / 抑制）；</item>
    ''' <item>p 值与 BH FDR 显著性过滤；</item>
    ''' <item>ARACNe DPI 三元组过滤去除由共同上游调控造成的间接边；</item>
    ''' <item>偏相关二次筛选（条件集为模块内高相关基因）；</item>
    ''' <item>依据 TF 注释定向，TF-TF 与非 TF-非 TF 边降级为候选边保留；</item>
    ''' <item>STRING 蛋白互做拓扑补全与置信度加权；</item>
    ''' <item>按模块输出 <see cref="PriorNetwork"/> 集合并构建跨模块调控边。</item>
    ''' </list>
    '''
    ''' 双轨制说明：模块划分轨沿用 WGCNA 内部的 GEMM Pearson（全基因组 O(n^2) 一次性完成）；
    ''' 边权轨在模块内改用 bicor（O(g^2·S)，全基因组不可行而模块内完全可行），
    ''' 因为 WGCNA 的 TOM 邻接不带符号，不能用于判定调控方向（激活 / 抑制）。
    ''' </remarks>
    Public Module ExpressionGRNBuilder

        ''' <summary>
        ''' 主入口：原始表达矩阵 → 按 WGCNA 模块拆分的先验网络集合
        ''' </summary>
        ''' <param name="samples">基因 × 样本的表达矩阵（多数据集合并后的大矩阵）</param>
        ''' <param name="TF">转录因子（上游调控因子）基因 ID 集合</param>
        ''' <param name="options">流水线配置；Nothing 时使用默认配置</param>
        ''' <returns>按模块拆分的 <see cref="PriorNetwork"/> 集合</returns>
        Public Function Build(samples As Matrix,
                              TF As IEnumerable(Of String),
                              Optional options As GRNBuildOptions = Nothing) As GRNBuildResult

            Return Build(samples, Nothing, TF, options)
        End Function

        ''' <summary>
        ''' 复用已完成的 WGCNA 分析结果（跳过模块划分），用于重复调参场景
        ''' </summary>
        ''' <param name="samples">基因 × 样本的表达矩阵（应与 WGCNA 分析使用的矩阵同源）</param>
        ''' <param name="wgcna">已完成的 WGCNA 分析结果；为 Nothing 时内部重新运行</param>
        ''' <param name="TF">转录因子基因 ID 集合</param>
        ''' <param name="options">流水线配置</param>
        ''' <returns>按模块拆分的 <see cref="PriorNetwork"/> 集合</returns>
        Public Function Build(samples As Matrix,
                              wgcna As Result,
                              TF As IEnumerable(Of String),
                              Optional options As GRNBuildOptions = Nothing) As GRNBuildResult

            If samples Is Nothing OrElse samples.expression Is Nothing Then
                Throw New ArgumentNullException(NameOf(samples), "表达矩阵不能为空")
            End If
            If TF Is Nothing Then
                Throw New ArgumentNullException(NameOf(TF), "TF 注释列表不能为空，否则无法构建调控方向")
            End If

            Dim tfSet As New HashSet(Of String)(TF, StringComparer.OrdinalIgnoreCase)

            If tfSet.Count = 0 Then
                Throw New ArgumentException("TF 注释列表不能为空，否则无法构建调控方向", NameOf(TF))
            End If

            If options Is Nothing Then
                options = New GRNBuildOptions()
            End If

            Dim clock As Stopwatch = Stopwatch.StartNew()
            Dim log As Action(Of String) = MakeLog(options.verbose)

            ' ① 预处理：批次标准化 + 基因质量过滤
            Call log($"[1/6] batch normalization + gene filtering on {samples.size} genes x {samples.sample_count} samples...")
            Dim filtered As Matrix = Preprocess(samples, options)

            If filtered.size < 2 OrElse filtered.sample_count < 3 Then
                Throw New InvalidOperationException(
                    $"预处理后矩阵规模不足（{filtered.size} 基因 x {filtered.sample_count} 样本），无法构建共表达网络")
            End If

            Dim geneIndex As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To filtered.size - 1
                Dim gid As String = filtered.expression(i).geneID

                If Not geneIndex.ContainsKey(gid) Then
                    geneIndex(gid) = i
                End If
            Next

            ' ② WGCNA 模块划分（或复用外部结果）
            If wgcna Is Nothing Then
                Call log($"[2/6] WGCNA module detection ({If(options.runBlockwise, "blockwise", "single block")})...")
                wgcna = RunWgcna(filtered, options)
            ElseIf wgcna.modules Is Nothing OrElse wgcna.modules.Count = 0 Then
                Throw New InvalidOperationException("传入的 WGCNA 结果不包含模块划分信息（Result.modules 为空）")
            Else
                Call log("[2/6] reuse external WGCNA result...")
            End If

            Dim kme As Dictionary(Of String, Double) = BuildKme(wgcna)

            ' ③ STRING 证据源（流式加载，只为网络内基因建索引）
            Dim stringEvidence As StringPriorEvidence = Nothing

            If Not String.IsNullOrEmpty(options.stringLinks) Then
                Call log("[3/6] loading STRING protein interactions...")
                stringEvidence = StringPriorEvidence.Load(
                    options.stringLinks, geneIndex.Keys, options.stringIdMap,
                    options.stringMinScore, options.verbose)
            Else
                Call log("[3/6] STRING evidence disabled (no links file).")
            End If

            ' ④ 逐模块构建先验网络
            Call log("[4/6] build module prior networks (bicor + FDR + DPI + partial correlation + TF orientation)...")

            Dim moduleList As ModulePriorNetwork() = wgcna.modules _
                .Select(Function(kv)
                            Return BuildModuleNetwork(kv.Key, kv.Value, filtered, geneIndex,
                                                      tfSet, options, stringEvidence, kme, log)
                        End Function) _
                .ToArray()

            ' ⑤ 跨模块调控边
            Call log("[5/6] build cross-module edges...")
            Call BuildCrossModuleEdges(wgcna, filtered, geneIndex, tfSet, options, stringEvidence, kme, moduleList, log)

            ' ⑥ 汇总
            clock.Stop()

            Dim summary As New BuildSummary With {
                .geneCount = filtered.size,
                .sampleCount = filtered.sample_count,
                .moduleCount = moduleList.Length,
                .moduleStatistics = moduleList.Select(Function(m) m.statistics).ToArray,
                .elapsedMilliseconds = clock.ElapsedMilliseconds
            }

            For Each stat As ModuleStatistics In summary.moduleStatistics
                summary.totalEdges += stat.directedEdges + stat.undirectedEdges
                summary.directedEdges += stat.directedEdges
                summary.undirectedEdges += stat.undirectedEdges
                summary.dpiRemovedEdges += stat.dpiRemovedEdges
                summary.partialRemovedEdges += stat.partialRemovedEdges
                summary.stringSupportedEdges += stat.stringSupportedEdges
                summary.stringOnlyEdges += stat.stringOnlyEdges
            Next

            For Each m As ModulePriorNetwork In moduleList
                If m.crossModule IsNot Nothing Then
                    summary.crossModuleEdges += m.crossModule.Edges.Count
                End If
            Next

            summary.totalEdges += summary.crossModuleEdges

            Call log($"[6/6] done: {summary.ToString}")

            Return New GRNBuildResult With {
                .modules = moduleList,
                .wgcna = wgcna,
                .summary = summary
            }
        End Function

        ''' <summary>
        ''' 读取转录因子注释表（默认取 Ensembl 列，TSV 格式）
        ''' </summary>
        ''' <param name="path">TF 注释文件路径（如 <c>Homo_sapiens_TF.txt</c>）</param>
        ''' <param name="column">基因 ID 所在列名</param>
        ''' <param name="tsv">True → 制表符分隔，False → 逗号分隔</param>
        ''' <returns>去重后的 TF 基因 ID 列表</returns>
        Public Function ReadTfList(path As String,
                                   Optional column As String = "Ensembl",
                                   Optional tsv As Boolean = True) As String()
            If String.IsNullOrEmpty(path) Then
                Throw New ArgumentException("TF 注释文件路径不能为空", NameOf(path))
            End If

            Dim sep As Char = If(tsv, ControlChars.Tab, ","c)
            Dim tfIds As New List(Of String)
            Dim columnIndex As Integer = -1
            Dim first As Boolean = True

            For Each line As String In System.IO.File.ReadLines(path)
                If String.IsNullOrWhiteSpace(line) Then Continue For

                Dim tokens As String() = line.Split(sep)

                If first Then
                    first = False

                    For i As Integer = 0 To tokens.Length - 1
                        If String.Equals(tokens(i).Trim.TrimStart("#"c), column, StringComparison.OrdinalIgnoreCase) Then
                            columnIndex = i
                            Exit For
                        End If
                    Next

                    If columnIndex = -1 Then
                        Throw New InvalidDataException($"TF 注释文件 '{path}' 中找不到列 '{column}'，表头为: {line}")
                    End If

                    Continue For
                End If

                If columnIndex < tokens.Length Then
                    Dim id As String = tokens(columnIndex).Trim

                    If Not String.IsNullOrEmpty(id) Then
                        tfIds.Add(id)
                    End If
                End If
            Next

            Call $"read {tfIds.Distinct.Count} TF ids from '{path}' (column = '{column}')".info

            Return tfIds.Distinct.ToArray
        End Function

        ''' <summary>
        ''' 从 STRING 的 Entrez ↔ STRING id 映射表构建「基因 ID → STRING protein id」字典
        ''' </summary>
        ''' <param name="path"><c>entrez_gene_id.vs.string.txt</c> 文件路径</param>
        ''' <param name="tsv">True → 制表符分隔</param>
        ''' <returns>Entrez gene id → STRING locus id 的映射</returns>
        Public Function BuildStringIdMap(path As String, Optional tsv As Boolean = True) As Dictionary(Of String, String)
            If String.IsNullOrEmpty(path) Then
                Throw New ArgumentException("STRING id 映射文件路径不能为空", NameOf(path))
            End If

            Return entrez_gene_id_vs_string.BuildMapsFromFile(path, tsv)
        End Function

        ' ============================================================
        ' 内部实现
        ' ============================================================

        ''' <summary>批次标准化 + 基因过滤</summary>
        Private Function Preprocess(samples As Matrix, options As GRNBuildOptions) As Matrix
            Dim normalized As Matrix = BatchNormalizer.Normalize(samples, options.batches, options.centerBatch)
            Dim filtered As Matrix = normalized

            If options.maxMissingRate > 0 Then
                filtered = GeneFilter.DropMissing(filtered, options.maxMissingRate)
            End If

            If options.filterTopN > 0 OrElse options.minVariance > 0 Then
                filtered = GeneFilter.ByVariance(filtered, options.filterTopN, options.minVariance)
            End If

            Return filtered
        End Function

        ''' <summary>运行 WGCNA 模块划分</summary>
        ''' <remarks>
        ''' 基因过滤已由 <see cref="Preprocess"/> 统一完成，因此把 WGCNA 配置中的过滤项重置为 0，
        ''' 避免 <c>RunBlockwise</c> 内部重复过滤导致模块基因与矩阵行不一致。
        ''' </remarks>
        Private Function RunWgcna(filtered As Matrix, options As GRNBuildOptions) As Result
            Dim config As WGCNAConfig

            If options.wgcnaConfig Is Nothing Then
                config = New WGCNAConfig()
            Else
                config = options.wgcnaConfig
            End If

            config.filterTopN = 0
            config.minVariance = 0
            config.maxMissingRate = 0
            config.buildGraph = False

            If options.runBlockwise Then
                Return Analysis.RunBlockwise(filtered, config)
            Else
                Return Analysis.Run(filtered, config)
            End If
        End Function

        ''' <summary>从 WGCNA 模块成员结果提取每个基因的 kME（取绝对值最大者）</summary>
        Private Function BuildKme(wgcna As Result) As Dictionary(Of String, Double)
            Dim kme As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            If wgcna.moduleMembership Is Nothing Then
                Return kme
            End If

            For Each m In wgcna.moduleMembership
                Dim v As Double = System.Math.Abs(m.Correlation)

                If Not kme.ContainsKey(m.GeneId) OrElse kme(m.GeneId) < v Then
                    kme(m.GeneId) = v
                End If
            Next

            Return kme
        End Function

        ''' <summary>按 kME 从大到小截取 hub 基因子集（保持原有相对顺序由调用方决定）</summary>
        Private Function TakeHubs(genes As String(), kme As Dictionary(Of String, Double), maxCount As Integer) As String()
            If maxCount <= 0 OrElse genes.Length <= maxCount Then
                Return genes
            End If

            Return genes _
                .OrderByDescending(Function(g)
                                       Dim v As Double = 0
                                       Return If(kme.TryGetValue(g, v), v, 0.0)
                                   End Function) _
                .Take(maxCount) _
                .OrderBy(Function(g) Array.IndexOf(genes, g)) _
                .ToArray()
        End Function

        ''' <summary>构建单个模块的先验网络</summary>
        Private Function BuildModuleNetwork(moduleName As String,
                                            moduleGenes As String(),
                                            filtered As Matrix,
                                            geneIndex As Dictionary(Of String, Integer),
                                            tfSet As HashSet(Of String),
                                            options As GRNBuildOptions,
                                            stringEvidence As StringPriorEvidence,
                                            kme As Dictionary(Of String, Double),
                                            log As Action(Of String)) As ModulePriorNetwork

            Dim clock As Stopwatch = Stopwatch.StartNew()
            Dim stat As New ModuleStatistics With {.moduleName = moduleName}
            Dim net As New PriorNetwork()

            ' 行下标对齐（模块基因可能不在过滤后的矩阵中）
            Dim rows As New List(Of Integer)
            Dim genes As New List(Of String)

            For Each g As String In moduleGenes
                Dim idx As Integer

                If geneIndex.TryGetValue(g, idx) Then
                    rows.Add(idx)
                    genes.Add(g)
                End If
            Next

            stat.geneCount = moduleGenes.Length

            If genes.Count < 2 OrElse filtered.sample_count < 3 Then
                stat.usedGenes = genes.Count
                stat.elapsedMilliseconds = clock.ElapsedMilliseconds
                Call log($"  [{moduleName}] skipped: {genes.Count} genes x {filtered.sample_count} samples is too small")

                Return New ModulePriorNetwork With {
                    .moduleName = moduleName,
                    .genes = genes.ToArray,
                    .moduleNetwork = net,
                    .crossModule = New PriorNetwork(),
                    .statistics = stat
                }
            End If

            ' 规模控制：超限模块按 kME 取 hub 子集
            genes = TakeHubs(genes.ToArray, kme, options.maxModuleGenes).ToList
            stat.usedGenes = genes.Count

            Dim data As Double(,) = ToArray2D(filtered, rows, genes)
            Dim n As Integer = data.GetLength(1)
            Dim cor As Double(,) = ComputeCorrelationMatrix(data, options)

            ' ① 候选边 + p 值 + BH FDR
            Dim candidates As New List(Of EdgeCand)

            For i As Integer = 0 To genes.Count - 2
                For j As Integer = i + 1 To genes.Count - 1
                    Dim r As Double = cor(i, j)

                    If Double.IsNaN(r) OrElse System.Math.Abs(r) < options.minAbsCorrelation Then
                        Continue For
                    End If

                    candidates.Add(New EdgeCand With {.i = i, .j = j, .cor = r})
                Next
            Next

            stat.candidateEdges = candidates.Count

            If candidates.Count > 0 Then
                Dim pvalues As Double() = candidates _
                    .Select(Function(c) CorrelationSignificance.PValue(c.cor, n)) _
                    .ToArray()
                Dim qvalues As Double() = CorrelationSignificance.BH(pvalues)

                For k As Integer = 0 To candidates.Count - 1
                    candidates(k).pvalue = pvalues(k)
                    candidates(k).qvalue = qvalues(k)
                Next

                candidates = candidates.Where(Function(c) c.qvalue <= options.fdrThreshold).ToList
            End If

            stat.fdrPassedEdges = candidates.Count

            ' ② DPI 去间接
            If options.dpiEnabled AndAlso candidates.Count > 0 Then
                stat.dpiRemovedEdges = ApplyDpi(candidates, cor, genes, kme, options)
            End If

            candidates = candidates.Where(Function(c) Not c.removedDpi).ToList

            ' ③ 偏相关二次筛选
            If options.partialEnabled AndAlso candidates.Count > 0 Then
                stat.partialRemovedEdges = ApplyPartialCorrelation(candidates, data, cor, genes, tfSet, options, log, moduleName)
            End If

            candidates = candidates.Where(Function(c) Not c.removedPartial).ToList

            ' ④ TF 定向 + 符号 + 置信度 + STRING 加权
            Dim existingPairs As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each c As EdgeCand In candidates
                Dim a As String = genes(c.i)
                Dim b As String = genes(c.j)
                Dim isTfA As Boolean = tfSet.Contains(a)
                Dim isTfB As Boolean = tfSet.Contains(b)
                Dim baseTag As String = If(options.useBicor,
                                           EvidenceTags.COEXPRESSION_BICOR,
                                           EvidenceTags.COEXPRESSION_PEARSON)
                Dim evidence As String = baseTag
                Dim confidence As Double = CorrelationSignificance.ExpressionConfidence(
                    System.Math.Abs(c.cor), c.qvalue, options.fdrThreshold)

                If c.attenuated Then
                    confidence *= options.partialAttenuatedRatio
                    evidence = EvidenceTags.Append(evidence, EvidenceTags.PARTIAL_ATTENUATED)
                ElseIf c.partialChecked Then
                    evidence = EvidenceTags.Append(evidence, EvidenceTags.PARTIAL_RETAINED)
                End If

                Dim tf As String, target As String
                Dim isUndirected As Boolean = False

                If isTfA AndAlso Not isTfB Then
                    tf = a : target = b
                ElseIf isTfB AndAlso Not isTfA Then
                    tf = b : target = a
                Else
                    ' 两端同为 TF 或同为非 TF：方向无法由共表达确定，降级为候选边
                    If Not options.keepUndirectedCandidates Then
                        Continue For
                    End If

                    tf = a : target = b
                    isUndirected = True
                    confidence *= options.undirectedConfidenceScale
                    evidence = EvidenceTags.Append(evidence,
                                                   If(isTfA, EvidenceTags.UNDIRECTED_TF_TF, EvidenceTags.UNDIRECTED_NON_TF))
                End If

                ' STRING 置信度加权
                If stringEvidence IsNot Nothing Then
                    Dim score As Double = stringEvidence.Lookup(a, b)

                    If score > 0 Then
                        confidence = CorrelationSignificance.MixString(
                            confidence, score, options.stringWeight, options.stringMinScore)
                        evidence = EvidenceTags.Append(evidence, EvidenceTags.STRING_PPI)
                        stat.stringSupportedEdges += 1
                    End If
                End If

                net.AddEdge(tf, target, GeneRegulatoryNetwork.InferEffector(c.cor),
                            CorrelationSignificance.Clamp01(confidence), evidence)

                If isUndirected Then
                    stat.undirectedEdges += 1
                Else
                    stat.directedEdges += 1
                End If

                Call existingPairs.Add(StringPriorEvidence.PairKey(a, b))
            Next

            ' ⑤ STRING 拓扑补全（新增纯 PPI 边）
            If stringEvidence IsNot Nothing AndAlso options.stringAddEdges Then
                Dim geneSet As New HashSet(Of String)(genes, StringComparer.OrdinalIgnoreCase)
                Dim addedPair = AddStringOnlyEdges(net, geneSet, existingPairs, tfSet,
                                                   stringEvidence, options)

                stat.stringOnlyEdges = addedPair.total
                stat.undirectedEdges += addedPair.undirected
                stat.directedEdges += addedPair.total - addedPair.undirected
            End If

            clock.Stop()
            stat.elapsedMilliseconds = clock.ElapsedMilliseconds

            Call log($"  {stat.ToString}")

            Return New ModulePriorNetwork With {
                .moduleName = moduleName,
                .genes = genes.ToArray,
                .moduleNetwork = net,
                .crossModule = New PriorNetwork(),
                .statistics = stat
            }
        End Function

        ''' <summary>把矩阵行数据组装为 genes × samples 的 Double(,)</summary>
        Private Function ToArray2D(filtered As Matrix, rows As List(Of Integer), genes As List(Of String)) As Double(,)
            Dim g As Integer = genes.Count
            Dim s As Integer = filtered.sample_count
            Dim data(g - 1, s - 1) As Double

            For i As Integer = 0 To g - 1
                Dim vec As Double() = filtered.expression(rows(i)).experiments

                For j As Integer = 0 To s - 1
                    data(i, j) = vec(j)
                Next
            Next

            Return data
        End Function

        ''' <summary>模块内带符号相关矩阵（bicor 或 Pearson）</summary>
        Private Function ComputeCorrelationMatrix(data As Double(,), options As GRNBuildOptions) As Double(,)
            If Not options.useBicor Then
                Return Bicor.CorrelationMatrix(data, robust:=False)
            End If

            If System.Math.Abs(options.bicorConstant - Bicor.DefaultConstant) < Double.Epsilon Then
                Return Bicor.CorrelationMatrix(data, robust:=True, pearsonFallback:=options.pearsonFallback)
            End If

            ' 自定义调节常数时逐对计算
            Dim g As Integer = data.GetLength(0)
            Dim s As Integer = data.GetLength(1)
            Dim r(g - 1, g - 1) As Double
            Dim cols(g - 1) As Double()

            For i As Integer = 0 To g - 1
                Dim v(s - 1) As Double

                For j As Integer = 0 To s - 1
                    v(j) = data(i, j)
                Next

                cols(i) = v
                r(i, i) = 1.0
            Next

            For i As Integer = 0 To g - 2
                For j As Integer = i + 1 To g - 1
                    Dim v As Double = Bicor.BiweightMidcorrelation(
                        cols(i), cols(j), options.bicorConstant, options.pearsonFallback)

                    r(i, j) = v
                    r(j, i) = v
                Next
            Next

            Return r
        End Function

        ''' <summary>ARACNe DPI 三元组过滤；返回被去除的边数</summary>
        ''' <remarks>
        ''' DPI 是 O(n^3) 的三角形枚举，只在 hub 基因子集（<see cref="GRNBuildOptions.maxDpiGenes"/>）内执行；
        ''' 两个端点都在子集内且 |cor| ≥ dpiMinAbsCorrelation 的边参与判定，
        ''' 其余边（弱边或 hub 之外的边）不受 DPI 影响。
        ''' </remarks>
        Private Function ApplyDpi(candidates As List(Of EdgeCand),
                                  cor As Double(,),
                                  genes As List(Of String),
                                  kme As Dictionary(Of String, Double),
                                  options As GRNBuildOptions) As Integer

            ' 参与 DPI 的基因：hub 子集（规模控制），DPI 结果只作用于子集内的边
            Dim hubNames As String() = TakeHubs(genes.ToArray, kme, options.maxDpiGenes)
            Dim hubSet As New HashSet(Of String)(hubNames, StringComparer.OrdinalIgnoreCase)
            Dim indices As New List(Of Integer)

            For i As Integer = 0 To genes.Count - 1
                If hubSet.Contains(genes(i)) Then
                    indices.Add(i)
                End If
            Next

            If indices.Count < 3 Then
                Return 0
            End If

            Dim pos As New Dictionary(Of Integer, Integer)

            For p As Integer = 0 To indices.Count - 1
                pos(indices(p)) = p
            Next

            ' 子矩阵（带符号相关）→ 高斯等价互信息 → DPI
            Dim m As Integer = indices.Count
            Dim subCor(m - 1, m - 1) As Double

            For a As Integer = 0 To m - 1
                For b As Integer = 0 To m - 1
                    subCor(a, b) = cor(indices(a), indices(b))
                Next
            Next

            ' DPI 的邻接阈值取 max(dpiMinAbsCorrelation, minAbsCorrelation) 对应的互信息
            Dim corCut As Double = System.Math.Max(options.dpiMinAbsCorrelation, options.minAbsCorrelation)
            Dim rr As Double = System.Math.Min(corCut * corCut, 0.999999999)
            Dim miThreshold As Double = -0.5 * System.Math.Log(1.0 - rr)

            Dim mi As Double(,) = AracneDpi.GaussianMutualInformation(subCor)
            Dim dpi As AracneDpi.DpiResult = AracneDpi.Filter(mi, miThreshold, options.dpiTolerance)

            Dim removed As Integer = 0

            For Each c As EdgeCand In candidates
                If Not pos.ContainsKey(c.i) OrElse Not pos.ContainsKey(c.j) Then
                    Continue For
                End If

                If System.Math.Abs(c.cor) < corCut Then
                    ' 弱边不参与 DPI 三角形判定，直接保留
                    Continue For
                End If

                If Not dpi.Adjacency(pos(c.i), pos(c.j)) Then
                    c.removedDpi = True
                    removed += 1
                End If
            Next

            Return removed
        End Function

        ''' <summary>偏相关二次筛选；返回被去除的边数</summary>
        Private Function ApplyPartialCorrelation(candidates As List(Of EdgeCand),
                                                 data As Double(,),
                                                 cor As Double(,),
                                                 genes As List(Of String),
                                                 tfSet As HashSet(Of String),
                                                 options As GRNBuildOptions,
                                                 log As Action(Of String),
                                                 moduleName As String) As Integer
            Dim n As Integer = data.GetLength(1)
            Dim maxK As Integer = System.Math.Min(options.partialConditionTopK, n - 3)

            If maxK < 1 Then
                Call log($"  [{moduleName}] partial correlation skipped: not enough samples ({n}) for a condition set")
                Return 0
            End If

            ' 按 |cor| 降序截断，控制 OLS 总开销
            Dim ordered As List(Of EdgeCand) = candidates _
                .OrderByDescending(Function(c) System.Math.Abs(c.cor)) _
                .Take(options.maxPartialEdgesPerModule) _
                .ToList()

            Dim removed As Integer = 0

            Call Parallel.For(0, ordered.Count,
                Sub(k)
                    Dim c As EdgeCand = ordered(k)
                    Dim cond As Integer() = ConditionSet(c, cor, genes, tfSet, maxK)

                    If cond Is Nothing OrElse cond.Length = 0 Then
                        c.partialChecked = False
                        Return
                    End If

                    Dim pcor As Double = PartialCorrelation.FromData(data, c.i, c.j, cond, options.partialUseRobust)

                    c.partialChecked = True
                    c.pcor = pcor

                    Dim ratio As Double = If(System.Math.Abs(c.cor) > 0,
                                             System.Math.Abs(pcor) / System.Math.Abs(c.cor), 0.0)

                    ' 符号翻转或显著衰减 → 间接关系，剔除
                    If System.Math.Sign(pcor) <> System.Math.Sign(c.cor) OrElse ratio < options.partialMinRetainRatio Then
                        c.removedPartial = True
                    ElseIf ratio < options.partialAttenuatedRatio Then
                        c.attenuated = True
                    End If
                End Sub)

            removed = ordered.Count(Function(c) c.removedPartial)

            Return removed
        End Function

        ''' <summary>
        ''' 偏相关的条件集：优先取与该边两端相关最高的 K 个 TF，不足时用其他高相关基因补齐
        ''' </summary>
        Private Function ConditionSet(c As EdgeCand,
                                      cor As Double(,),
                                      genes As List(Of String),
                                      tfSet As HashSet(Of String),
                                      maxK As Integer) As Integer()
            Dim ranked As New List(Of (idx As Integer, strength As Double, isTf As Boolean))

            For k As Integer = 0 To genes.Count - 1
                If k = c.i OrElse k = c.j Then Continue For

                Dim strength As Double = System.Math.Max(System.Math.Abs(cor(c.i, k)), System.Math.Abs(cor(c.j, k)))

                If strength > 0 Then
                    ranked.Add((k, strength, tfSet.Contains(genes(k))))
                End If
            Next

            Dim cond As New List(Of Integer)

            ' 先取 TF 条件变量（控制共同上游调控），再按相关强度补齐
            For Each t In ranked.Where(Function(x) x.isTf).OrderByDescending(Function(x) x.strength)
                cond.Add(t.idx)
                If cond.Count >= maxK Then Exit For
            Next

            If cond.Count < maxK Then
                For Each t In ranked.Where(Function(x) Not x.isTf).OrderByDescending(Function(x) x.strength)
                    cond.Add(t.idx)
                    If cond.Count >= maxK Then Exit For
                Next
            End If

            Return cond.ToArray
        End Function

        ''' <summary>STRING 拓扑补全：为共表达网络缺失但 PPI 存在的基因对新增边</summary>
        ''' <returns>(新增边总数, 其中无向候选边数)</returns>
        Private Function AddStringOnlyEdges(net As PriorNetwork,
                                            geneSet As HashSet(Of String),
                                            existing As HashSet(Of String),
                                            tfSet As HashSet(Of String),
                                            stringEvidence As StringPriorEvidence,
                                            options As GRNBuildOptions) As (total As Integer, undirected As Integer)
            Dim added As Integer = 0
            Dim undirected As Integer = 0

            For Each pair In stringEvidence.Pairs()
                If Not geneSet.Contains(pair.a) OrElse Not geneSet.Contains(pair.b) Then
                    Continue For
                End If
                If existing.Contains(StringPriorEvidence.PairKey(pair.a, pair.b)) Then
                    Continue For
                End If

                Dim confidence As Double = CorrelationSignificance.StringOnlyConfidence(
                    pair.score, options.stringMinScore)

                If confidence <= 0 Then
                    Continue For
                End If

                Dim tf As String = pair.a
                Dim target As String = pair.b
                Dim isTfA As Boolean = tfSet.Contains(pair.a)
                Dim isTfB As Boolean = tfSet.Contains(pair.b)
                Dim evidence As String = EvidenceTags.STRING_PPI_UNSIGNED
                Dim isUndirectedEdge As Boolean = False

                If isTfA AndAlso Not isTfB Then
                    tf = pair.a : target = pair.b
                ElseIf isTfB AndAlso Not isTfA Then
                    tf = pair.b : target = pair.a
                Else
                    If Not options.keepUndirectedCandidates Then
                        Continue For
                    End If

                    isUndirectedEdge = True
                    confidence *= options.undirectedConfidenceScale
                    evidence = EvidenceTags.Append(evidence,
                                                   If(isTfA, EvidenceTags.UNDIRECTED_TF_TF, EvidenceTags.UNDIRECTED_NON_TF))
                End If

                ' 纯 PPI 边无符号信息，默认按激活处理并在 Evidence 中标记
                net.AddEdge(tf, target, Effector.Activator, confidence, evidence)
                Call existing.Add(StringPriorEvidence.PairKey(pair.a, pair.b))
                added += 1

                If isUndirectedEdge Then
                    undirected += 1
                End If
            Next

            Return (added, undirected)
        End Function

        ''' <summary>构建跨模块调控边并挂到对应模块的 crossModule 网络上</summary>
        Private Sub BuildCrossModuleEdges(wgcna As Result,
                                          filtered As Matrix,
                                          geneIndex As Dictionary(Of String, Integer),
                                          tfSet As HashSet(Of String),
                                          options As GRNBuildOptions,
                                          stringEvidence As StringPriorEvidence,
                                          kme As Dictionary(Of String, Double),
                                          moduleList As ModulePriorNetwork(),
                                          log As Action(Of String))

            If Not options.crossModuleEnabled Then
                Call log("  cross-module edges disabled.")
                Return
            End If
            If wgcna.moduleEigengenes Is Nothing OrElse wgcna.moduleEigengenes.Count < 2 Then
                Call log("  cross-module edges skipped: no module eigengenes available.")
                Return
            End If

            ' ① 模块特征基因相关 → 选出显著相关的模块对
            Dim names As String() = wgcna.moduleEigengenes.Keys.ToArray()
            Dim pairs As New List(Of (a As String, b As String, r As Double))

            For i As Integer = 0 To names.Length - 2
                For j As Integer = i + 1 To names.Length - 1
                    Dim r As Double = Bicor.Pearson(wgcna.moduleEigengenes(names(i)),
                                                    wgcna.moduleEigengenes(names(j)))

                    If Not Double.IsNaN(r) AndAlso System.Math.Abs(r) >= options.crossModuleCorThreshold Then
                        pairs.Add((names(i), names(j), r))
                    End If
                Next
            Next

            pairs = pairs.OrderByDescending(Function(p) System.Math.Abs(p.r)).Take(options.maxCrossModulePairs).ToList

            If pairs.Count = 0 Then
                Call log("  cross-module edges: no significantly correlated module pairs.")
                Return
            End If

            Dim byModule As Dictionary(Of String, ModulePriorNetwork) = moduleList _
                .GroupBy(Function(m) m.moduleName, StringComparer.OrdinalIgnoreCase) _
                .ToDictionary(Function(g) g.Key, Function(g) g.First(), StringComparer.OrdinalIgnoreCase)

            ' ② 在模块对之间计算 TF × 靶基因的相关
            Dim crossCandidates As New List(Of CrossCand)

            For Each pair In pairs
                Call CollectCrossCandidates(pair.a, pair.b, wgcna.modules, filtered, geneIndex, tfSet, kme, options, crossCandidates)
                Call CollectCrossCandidates(pair.b, pair.a, wgcna.modules, filtered, geneIndex, tfSet, kme, options, crossCandidates)
            Next

            If crossCandidates.Count = 0 Then
                Call log("  cross-module edges: no candidate edges found.")
                Return
            End If

            ' ③ p 值 + BH FDR + 置信度融合 + 定向
            Dim n As Integer = filtered.sample_count
            Dim pvalues As Double() = crossCandidates _
                .Select(Function(c) CorrelationSignificance.PValue(c.r, n)) _
                .ToArray()
            Dim qvalues As Double() = CorrelationSignificance.BH(pvalues)
            Dim added As Integer = 0

            For k As Integer = 0 To crossCandidates.Count - 1
                Dim c As CrossCand = crossCandidates(k)

                If qvalues(k) > options.fdrThreshold Then
                    Continue For
                End If

                Dim evidence As String = EvidenceTags.Join(
                    If(options.useBicor, EvidenceTags.COEXPRESSION_BICOR, EvidenceTags.COEXPRESSION_PEARSON),
                    EvidenceTags.CROSS_MODULE)
                Dim confidence As Double = options.crossModuleConfidenceScale * CorrelationSignificance.ExpressionConfidence(
                    System.Math.Abs(c.r), qvalues(k), options.fdrThreshold)

                If stringEvidence IsNot Nothing Then
                    Dim score As Double = stringEvidence.Lookup(c.tf, c.target)

                    If score > 0 Then
                        confidence = CorrelationSignificance.MixString(
                            confidence, score, options.stringWeight, options.stringMinScore)
                        evidence = EvidenceTags.Append(evidence, EvidenceTags.STRING_PPI)
                    End If
                End If

                Dim owner As ModulePriorNetwork = Nothing

                If byModule.TryGetValue(c.tfModule, owner) Then
                    owner.crossModule.AddEdge(c.tf, c.target,
                                              GeneRegulatoryNetwork.InferEffector(c.r),
                                              CorrelationSignificance.Clamp01(confidence), evidence)
                    added += 1
                End If
            Next

            Call log($"  cross-module edges: {added} edges over {pairs.Count} module pairs (candidates = {crossCandidates.Count})")
        End Sub

        ''' <summary>收集一个方向（tfModule 内的 TF → targetModule 内的基因）的跨模块候选边</summary>
        Private Sub CollectCrossCandidates(tfModule As String,
                                           targetModule As String,
                                           modules As Dictionary(Of String, String()),
                                           filtered As Matrix,
                                           geneIndex As Dictionary(Of String, Integer),
                                           tfSet As HashSet(Of String),
                                           kme As Dictionary(Of String, Double),
                                           options As GRNBuildOptions,
                                           sink As List(Of CrossCand))

            Dim tfRows As New List(Of Integer)
            Dim targetRows As New List(Of Integer)
            Dim tfGenes As String() = Nothing
            Dim targetGenes As String() = Nothing

            If Not modules.TryGetValue(tfModule, tfGenes) OrElse
                Not modules.TryGetValue(targetModule, targetGenes) Then
                Return
            End If

            For Each m As String In tfGenes
                Dim idx As Integer

                If geneIndex.TryGetValue(m, idx) AndAlso tfSet.Contains(m) Then
                    tfRows.Add(idx)
                End If
            Next

            For Each m As String In targetGenes
                Dim idx As Integer

                If geneIndex.TryGetValue(m, idx) Then
                    targetRows.Add(idx)
                End If
            Next

            If tfRows.Count = 0 OrElse targetRows.Count = 0 Then
                Return
            End If

            ' 按候选边规模上限截断
            Dim maxTf As Integer = options.maxCrossModuleTfs
            Dim maxTarget As Integer = options.maxCrossModuleTargets

            If tfRows.Count > maxTf Then
                tfRows = tfRows.OrderByDescending(Function(r) KmeOf(filtered.expression(r).geneID, kme)).Take(maxTf).ToList
            End If

            If targetRows.Count > maxTarget Then
                targetRows = targetRows.OrderByDescending(Function(r) KmeOf(filtered.expression(r).geneID, kme)).Take(maxTarget).ToList
            End If

            For Each t As Integer In tfRows
                Dim tfVec As Double() = filtered.expression(t).experiments
                Dim tfName As String = filtered.expression(t).geneID

                For Each g As Integer In targetRows
                    Dim r As Double = Bicor.BiweightMidcorrelation(tfVec, filtered.expression(g).experiments)

                    If Double.IsNaN(r) OrElse System.Math.Abs(r) < options.minAbsCorrelation Then
                        Continue For
                    End If

                    sink.Add(New CrossCand With {
                        .tfModule = tfModule,
                        .tf = tfName,
                        .target = filtered.expression(g).geneID,
                        .r = r
                    })
                Next
            Next
        End Sub

        ''' <summary>查询基因的 kME</summary>
        Private Function KmeOf(gene As String, kme As Dictionary(Of String, Double)) As Double
            Dim v As Double = 0
            Return If(kme.TryGetValue(gene, v), v, 0.0)
        End Function

        ''' <summary>日志输出</summary>
        Private Function MakeLog(verbose As Boolean) As Action(Of String)
            If verbose Then
                Return Sub(msg)
                           Call msg.info
                       End Sub
            Else
                Return Sub(msg)
                           ' 静默模式
                       End Sub
            End If
        End Function

        ''' <summary>模块内候选边</summary>
        Private Class EdgeCand

            Public Property i As Integer
            Public Property j As Integer
            Public Property cor As Double
            Public Property pvalue As Double
            Public Property qvalue As Double
            Public Property pcor As Double

            ''' <summary>是否做过偏相关检验</summary>
            Public Property partialChecked As Boolean

            ''' <summary>偏相关衰减（保留但打折）</summary>
            Public Property attenuated As Boolean

            ''' <summary>被 DPI 判定为间接边</summary>
            Public Property removedDpi As Boolean

            ''' <summary>被偏相关判定为间接边</summary>
            Public Property removedPartial As Boolean
        End Class

        ''' <summary>跨模块候选边</summary>
        Private Class CrossCand

            ''' <summary>TF 所在模块（边的归属模块）</summary>
            Public Property tfModule As String
            Public Property tf As String
            Public Property target As String
            Public Property r As Double
        End Class
    End Module
End Namespace
