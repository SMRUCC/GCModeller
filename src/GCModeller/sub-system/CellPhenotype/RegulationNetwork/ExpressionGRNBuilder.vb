Imports System.Diagnostics
Imports Microsoft.VisualBasic.Math.Matrix
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.WGCNA
Imports SMRUCC.genomics.Data.STRING.Tabular.Tsv
Imports SMRUCC.genomics.InteractionModel

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

            ' ⓪ GPU 后端：WGCNA 模块划分（相关矩阵 GEMM + TOM）是全流水线唯一的 O(G^2) 计算
            If options.enableGpu Then
                Call log("[0/6] enabling CUDA GPU backend for WGCNA GEMM/TOM...")
            End If

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
            Dim backend As String = HTS.WGCNA.Analysis.Backend

            Call log($"[2/6] WGCNA done, compute backend = {backend}.")

            ' ③ STRING 证据源（流式加载，只为网络内基因建索引）
            Dim stringEvidence As StringPriorEvidence = LoadStringEvidence(options, geneIndex.Keys, log)

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

            Dim summary As BuildSummary = MakeSummary(moduleList, filtered.size, filtered.sample_count, clock.ElapsedMilliseconds)

            Call log($"[6/6] done: {summary.ToString}")

            Return New GRNBuildResult With {
                .modules = moduleList,
                .wgcna = wgcna,
                .summary = summary
            }
        End Function

        ''' <summary>
        ''' 存储驱动入口：基于已持久化的相关矩阵存储（<see cref="CorrelationMatrixStore"/>）构建先验网络
        ''' </summary>
        ''' <param name="store">
        ''' 已打开的相关矩阵存储（由 <see cref="CorrelationMatrixWriter"/> 对原始 NxN 相关矩阵
        ''' 一次性计算并持久化得到）。候选边与 p 值直接来自存储，**跳过 bicor 重算**，
        ''' 使得不同的阈值参数实验只需分钟级的存储查询。
        ''' </param>
        ''' <param name="TF">转录因子基因 ID 集合</param>
        ''' <param name="modules">
        ''' WGCNA 模块划分结果（模块名 → 基因列表），来自一次性的 WGCNA 模块划分运行；
        ''' 存储中不存在的基因会被跳过
        ''' </param>
        ''' <param name="options">流水线配置；Nothing 时使用默认配置</param>
        ''' <param name="expr">
        ''' 可选的原始表达矩阵：提供时才执行偏相关二次筛选（偏相关需要原始表达数据）；
        ''' 为 Nothing 时跳过偏相关，其余流程（FDR/DPI/TF 定向/STRING）不受影响
        ''' </param>
        ''' <returns>按模块拆分的先验网络集合；<see cref="GRNBuildResult.wgcna"/> 为 Nothing</returns>
        ''' <remarks>
        ''' 与表达矩阵路径的区别：本重载不运行 WGCNA（模块划分由调用方传入）、
        ''' 不重算相关矩阵（候选边来自存储）、不构建跨模块边（跨模块边依赖模块特征基因，
        ''' 需要完整 WGCNA Result；如需跨模块边请使用表达矩阵路径）。
        ''' </remarks>
        Public Function Build(store As CorrelationMatrixStore,
                              TF As IEnumerable(Of String),
                              modules As Dictionary(Of String, String()),
                              options As GRNBuildOptions,
                              Optional expr As Matrix = Nothing) As GRNBuildResult

            If store Is Nothing Then
                Throw New ArgumentNullException(NameOf(store), "相关矩阵存储不能为空")
            End If
            If TF Is Nothing Then
                Throw New ArgumentNullException(NameOf(TF), "TF 注释列表不能为空，否则无法构建调控方向")
            End If
            If modules Is Nothing OrElse modules.Count = 0 Then
                Throw New ArgumentNullException(NameOf(modules), "模块划分结果不能为空")
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

            ' 基因行下标（仅当提供表达矩阵时需要，用于偏相关的子矩阵组装）
            Dim geneIndex As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            If expr IsNot Nothing Then
                For i As Integer = 0 To expr.size - 1
                    Dim gid As String = expr.expression(i).geneID

                    If Not geneIndex.ContainsKey(gid) Then
                        geneIndex(gid) = i
                    End If
                Next
            End If

            Call log($"[1/3] store-driven GRN build: {store.size} genes x {store.SampleN} samples (p-values from store), " &
                     $"{modules.Count} modules, partial correlation = {If(expr IsNot Nothing, "enabled", "skipped (no expression matrix)")}...")

            ' ② STRING 证据源
            Dim stringEvidence As StringPriorEvidence = LoadStringEvidence(options, store.genes, log)

            ' ③ 逐模块装配先验网络
            Dim moduleList As ModulePriorNetwork() = modules _
                .Select(Function(kv)
                            Return BuildModuleNetworkFromStore(store, kv.Key, kv.Value,
                                                               tfSet, options, stringEvidence,
                                                               expr, geneIndex, log)
                        End Function) _
                .ToArray()

            clock.Stop()

            Dim summary As BuildSummary = MakeSummary(moduleList, store.size, store.SampleN, clock.ElapsedMilliseconds)

            Call log($"done: {summary.ToString}")

            Return New GRNBuildResult With {
                .modules = moduleList,
                .wgcna = Nothing,
                .summary = summary
            }
        End Function

        ''' <summary>加载 STRING 证据源（含 ID 映射的自动构建）</summary>
        Private Function LoadStringEvidence(options As GRNBuildOptions,
                                            geneIds As IEnumerable(Of String),
                                            log As Action(Of String)) As StringPriorEvidence
            If String.IsNullOrEmpty(options.stringLinks) Then
                Call log("STRING evidence disabled (no links file).")
                Return Nothing
            End If

            ' ID 映射：优先使用显式提供的映射表，其次从 STRING 别名表自动提取 ENSG → STRING id
            Dim idMap As Dictionary(Of String, String) = options.stringIdMap

            If idMap Is Nothing AndAlso Not String.IsNullOrEmpty(options.stringAliases) Then
                Call log("build gene id -> STRING protein id map from aliases file...")
                idMap = StringPriorEvidence.LoadEnsemblAliasMap(options.stringAliases, options.verbose)
            End If

            Call log("loading STRING protein interactions...")
            Return StringPriorEvidence.Load(options.stringLinks, geneIds, idMap,
                                            options.stringMinScore, options.verbose)
        End Function

        ''' <summary>汇总各模块统计</summary>
        Private Function MakeSummary(moduleList As ModulePriorNetwork(),
                                     geneCount As Integer,
                                     sampleCount As Integer,
                                     elapsedMs As Double) As BuildSummary
            Dim summary As New BuildSummary With {
                .geneCount = geneCount,
                .sampleCount = sampleCount,
                .moduleCount = moduleList.Length,
                .moduleStatistics = moduleList.Select(Function(m) m.statistics).ToArray,
                .elapsedMilliseconds = elapsedMs
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

            Return summary
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
                        Throw New System.IO.InvalidDataException($"TF 注释文件 '{path}' 中找不到列 '{column}'，表头为: {line}")
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

        ''' <summary>
        ''' 在 STRING 数据文件夹中自动发现 links 与 aliases 文件
        ''' </summary>
        ''' <param name="folder">STRING 解压后的数据文件夹（如 <c>K:\hsa_grn\string-db</c>）</param>
        ''' <returns>(links 文件路径, aliases 文件路径)；未找到时对应元素为 Nothing</returns>
        ''' <remarks>
        ''' links 文件的选择优先级：精简版 <c>9606.protein.links.v*.txt</c>（3 列，体积最小）＞
        ''' detailed 版 ＞ 其他任意 <c>*protein.links*.txt</c>；始终跳过 <c>.gz</c> 与 physical 子集。
        ''' </remarks>
        Public Function FindStringLinks(folder As String) As (links As String, aliases As String)
            If String.IsNullOrEmpty(folder) OrElse Not System.IO.Directory.Exists(folder) Then
                Return (Nothing, Nothing)
            End If

            Dim files As String() = System.IO.Directory.GetFiles(folder, "*.txt", System.IO.SearchOption.TopDirectoryOnly)
            Dim links As String = Nothing
            Dim aliases As String = Nothing

            ' 优先级 1：精简版 links（protein1 / protein2 / combined_score 三列）
            links = files.FirstOrDefault(Function(f)
                                             Dim name = System.IO.Path.GetFileName(f)
                                             Return name.StartsWith("9606.protein.links.", StringComparison.OrdinalIgnoreCase) _
                                                 AndAlso Not name.Contains("detailed") AndAlso Not name.Contains("full")
                                         End Function)

            ' 优先级 2：detailed 版
            If links Is Nothing Then
                links = files.FirstOrDefault(Function(f) System.IO.Path.GetFileName(f).EndsWith("protein.links.detailed.v12.0.txt", StringComparison.OrdinalIgnoreCase))
            End If

            ' 优先级 3：任意 links 文件（排除 physical 子集）
            If links Is Nothing Then
                links = files.FirstOrDefault(Function(f)
                                                 Dim name = System.IO.Path.GetFileName(f)
                                                 Return name.IndexOf("protein.links", StringComparison.OrdinalIgnoreCase) >= 0 _
                                                     AndAlso Not name.Contains("physical")
                                             End Function)
            End If

            aliases = files.FirstOrDefault(Function(f)
                                               Dim name = System.IO.Path.GetFileName(f)
                                               Return name.IndexOf("protein.aliases", StringComparison.OrdinalIgnoreCase) >= 0
                                           End Function)

            Return (links, aliases)
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
            config.useGpu = options.enableGpu

            If options.runBlockwise Then
                Return HTS.WGCNA.Analysis.RunBlockwise(filtered, config)
            Else
                Return HTS.WGCNA.Analysis.Run(filtered, config)
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

            ' ①-⑤ 候选边 → FDR → DPI → 偏相关 → TF 定向 → STRING 整合
            '（与存储驱动路径 BuildModuleNetworkFromStore 共享同一套装配逻辑）
            Call AssembleModuleEdges(cor, data, n, genes, tfSet, options, stringEvidence, kme, log, moduleName, net, stat)

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

        ''' <summary>
        ''' 模块网络装配（两条路径共享）：候选边 → p 值/BH FDR → DPI 去间接 → 偏相关筛选 →
        ''' TF 定向/符号/置信度 → STRING 加权与拓扑补全
        ''' </summary>
        ''' <param name="cor">模块内带符号相关矩阵（来自 bicor 计算，或来自 CorrelationMatrixStore）</param>
        ''' <param name="data">
        ''' 模块内表达子矩阵（偏相关需要）；为 Nothing 时跳过偏相关筛选（存储驱动且未提供表达矩阵的场景）
        ''' </param>
        ''' <param name="sampleN">p 值重算依据的样本数（表达矩阵路径为样本数，存储路径为 <c>store.SampleN</c>）</param>
        Private Sub AssembleModuleEdges(cor As Double(,),
                                        data As Double(,),
                                        sampleN As Integer,
                                        genes As List(Of String),
                                        tfSet As HashSet(Of String),
                                        options As GRNBuildOptions,
                                        stringEvidence As StringPriorEvidence,
                                        kme As Dictionary(Of String, Double),
                                        log As Action(Of String),
                                        moduleName As String,
                                        net As PriorNetwork,
                                        stat As ModuleStatistics)

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
                    .Select(Function(c) CorrelationPValues.PValue(c.cor, sampleN)) _
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

            ' ③ 偏相关二次筛选（需要原始表达数据；存储驱动且未提供表达矩阵时跳过）
            If options.partialEnabled AndAlso data IsNot Nothing AndAlso candidates.Count > 0 Then
                stat.partialRemovedEdges = ApplyPartialCorrelation(candidates, data, cor, genes, tfSet, options, log, moduleName)
            ElseIf options.partialEnabled AndAlso data Is Nothing Then
                Call log($"  [{moduleName}] partial correlation skipped: no expression data available")
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
        End Sub

        ''' <summary>
        ''' 存储驱动路径：从 <see cref="CorrelationMatrixStore"/> 读取模块内相关矩阵，
        ''' 装配单个模块的先验网络（跳过 bicor 重算）
        ''' </summary>
        ''' <param name="store">已打开的相关矩阵存储</param>
        ''' <param name="moduleName">模块名</param>
        ''' <param name="moduleGenes">模块基因列表（来自既有的 WGCNA 模块划分结果）</param>
        ''' <param name="tfSet">TF 集合</param>
        ''' <param name="options">配置</param>
        ''' <param name="stringEvidence">STRING 证据源</param>
        ''' <param name="expr">可选的原始表达矩阵（提供时才执行偏相关筛选）</param>
        ''' <param name="log">日志</param>
        Private Function BuildModuleNetworkFromStore(store As CorrelationMatrixStore,
                                                     moduleName As String,
                                                     moduleGenes As String(),
                                                     tfSet As HashSet(Of String),
                                                     options As GRNBuildOptions,
                                                     stringEvidence As StringPriorEvidence,
                                                     expr As Matrix,
                                                     geneIndex As Dictionary(Of String, Integer),
                                                     log As Action(Of String)) As ModulePriorNetwork

            Dim clock As Stopwatch = Stopwatch.StartNew()
            Dim stat As New ModuleStatistics With {.moduleName = moduleName}
            Dim net As New PriorNetwork()

            ' 基因对齐：只保留存在于存储中的基因（去重，防御同一基因出现在多模块）
            Dim genes As New List(Of String)

            For Each g As String In moduleGenes
                If store.Contains(g) AndAlso Not genes.Contains(g, StringComparer.OrdinalIgnoreCase) Then
                    genes.Add(g)
                End If
            Next

            stat.geneCount = moduleGenes.Length

            If genes.Count < 2 OrElse store.SampleN < 3 Then
                stat.usedGenes = genes.Count
                stat.elapsedMilliseconds = clock.ElapsedMilliseconds
                Call log($"  [{moduleName}] skipped: {genes.Count} genes x {store.SampleN} samples is too small")

                Return New ModulePriorNetwork With {
                    .moduleName = moduleName,
                    .genes = genes.ToArray,
                    .moduleNetwork = net,
                    .crossModule = New PriorNetwork(),
                    .statistics = stat
                }
            End If

            ' 逐行取回存储中的相关系数（并发），同时统计连接度作为 hub 排序依据
            Dim rowNumbers As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For Each g As String In genes
                rowNumbers(g) = store.IndexOf(g)
            Next

            ' 并发取行：按位置写入数组（数组不同下标的写入是线程安全的；
            ' 不能用普通 Dictionary 并发写——内部结构会被并发写损坏）
            Dim rowCache(genes.Count - 1)() As Single

            Call Parallel.For(0, genes.Count,
                Sub(k)
                    rowCache(k) = store.ReadRow(rowNumbers(genes(k)), useCache:=False)
                End Sub)

            ' 规模控制：hub 子集按连接度（邻居数）排序截断
            Dim degree As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)

            For k As Integer = 0 To genes.Count - 1
                Dim g As String = genes(k)
                Dim cnt As Integer = 0
                Dim row As Single() = rowCache(k)

                For Each other As String In genes
                    If other <> g AndAlso Not Single.IsNaN(row(rowNumbers(other))) _
                        AndAlso System.Math.Abs(row(rowNumbers(other))) >= options.minAbsCorrelation Then
                        cnt += 1
                    End If
                Next

                degree(g) = cnt
            Next

            ' hub 截断后保留 基因 → 行缓存下标 的映射（rowCache 按截断前的位置索引）
            Dim cachePos As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For k As Integer = 0 To genes.Count - 1
                cachePos(genes(k)) = k
            Next

            genes = TakeHubs(genes.ToArray, degree, options.maxModuleGenes).ToList
            stat.usedGenes = genes.Count

            ' 从取回的行组装模块内带符号相关子矩阵
            Dim gCount As Integer = genes.Count
            Dim cor(gCount - 1, gCount - 1) As Double

            Call Parallel.For(0, gCount,
                Sub(i)
                    Dim row As Single() = rowCache(cachePos(genes(i)))

                    cor(i, i) = 1.0

                    For j As Integer = i + 1 To gCount - 1
                        Dim v As Double = row(rowNumbers(genes(j)))

                        cor(i, j) = v
                        cor(j, i) = v
                    Next
                End Sub)

            ' 表达子矩阵（偏相关可选）
            Dim data As Double(,) = Nothing

            If expr IsNot Nothing AndAlso expr.sample_count >= 3 Then
                Dim exprRows As New List(Of Integer)

                For Each g As String In genes
                    Dim idx As Integer

                    If geneIndex.TryGetValue(g, idx) Then
                        exprRows.Add(idx)
                    End If
                Next

                If exprRows.Count = gCount Then
                    data = ToArray2D(expr, exprRows, genes)
                End If
            End If

            Call AssembleModuleEdges(cor, data, store.SampleN, genes, tfSet, options,
                                     stringEvidence, degree, log, moduleName, net, stat)

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

        ''' <summary>
        ''' 模块内带符号相关矩阵（bicor 或 Pearson），多线程并行计算
        ''' </summary>
        ''' <remarks>
        ''' 模块内 bicor 是 O(g^2·S) 的逐对计算：g=1000、S=1888 时约 9.4 亿次浮点运算，
        ''' 单线程需要数十秒，按基因行做 <see cref="Parallel"/> 并行可以充分利用多核。
        ''' 每对 (i, j) 只由线程 i 写入 r(i, j) / r(j, i)，不同线程之间无写冲突。
        ''' </remarks>
        Private Function ComputeCorrelationMatrix(data As Double(,), options As GRNBuildOptions) As Double(,)
            Dim g As Integer = data.GetLength(0)
            Dim s As Integer = data.GetLength(1)
            Dim r(g - 1, g - 1) As Double
            Dim cols(g - 1)() As Double

            ' 行向量提取（PartialCorrelation.FromData 与跨模块计算都会复用同样的行顺序）
            Call Parallel.For(0, g,
                Sub(i)
                    Dim v(s - 1) As Double

                    For j As Integer = 0 To s - 1
                        v(j) = data(i, j)
                    Next

                    cols(i) = v
                End Sub)

            Dim useBicor As Boolean = options.useBicor
            Dim constant As Double = options.bicorConstant
            Dim fallback As Boolean = options.pearsonFallback

            Call Parallel.For(0, g,
                Sub(i)
                    r(i, i) = 1.0
                    Dim ci As Double() = cols(i)

                    For j As Integer = i + 1 To g - 1
                        Dim v As Double

                        If useBicor Then
                            v = Bicor.BiweightMidcorrelation(ci, cols(j), constant, fallback)
                        Else
                            v = Bicor.Pearson(ci, cols(j))
                        End If

                        r(i, j) = v
                        r(j, i) = v
                    Next
                End Sub)

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

            Dim removedCount As Integer = 0

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

            For Each c As EdgeCand In ordered
                If c.removedPartial Then
                    removedCount += 1
                End If
            Next

            Return removedCount
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
        ''' <remarks>
        ''' 通过 <see cref="StringPriorEvidence.LinksOf"/> 的按基因索引遍历模块内基因，
        ''' 复杂度为 O(Σ deg)，避免了对全部边对的反复全表扫描。
        ''' </remarks>
        ''' <returns>(新增边总数, 其中无向候选边数)</returns>
        Private Function AddStringOnlyEdges(net As PriorNetwork,
                                            geneSet As HashSet(Of String),
                                            existing As HashSet(Of String),
                                            tfSet As HashSet(Of String),
                                            stringEvidence As StringPriorEvidence,
                                            options As GRNBuildOptions) As (total As Integer, undirected As Integer)
            Dim added As Integer = 0
            Dim undirected As Integer = 0

            For Each gene As String In geneSet
                For Each link In stringEvidence.LinksOf(gene)
                    If Not geneSet.Contains(link.other) Then
                        Continue For
                    End If

                    Dim key As String = StringPriorEvidence.PairKey(gene, link.other)

                    If existing.Contains(key) Then
                        Continue For
                    End If

                    Dim confidence As Double = CorrelationSignificance.StringOnlyConfidence(
                        link.score, options.stringMinScore)

                    If confidence <= 0 Then
                        Continue For
                    End If

                    Dim isTfA As Boolean = tfSet.Contains(gene)
                    Dim isTfB As Boolean = tfSet.Contains(link.other)
                    Dim evidence As String = EvidenceTags.STRING_PPI_UNSIGNED
                    Dim isUndirectedEdge As Boolean = False
                    Dim tf As String = gene
                    Dim target As String = link.other

                    If isTfA AndAlso Not isTfB Then
                        ' 方向保持 gene -> other
                    ElseIf isTfB AndAlso Not isTfA Then
                        tf = link.other : target = gene
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
                    Call existing.Add(key)
                    added += 1

                    If isUndirectedEdge Then
                        undirected += 1
                    End If
                Next
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
