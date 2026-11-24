Imports System.Globalization
Imports System.IO
Imports System.IO.Compression
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.XGBoost.train
Imports SMRUCC.genomics.BioSystem

''' <summary>
''' GeneTree 模型的 zip 存档读写。
''' 
''' 压缩包内的结构：
''' 
''' ```
''' manifest.tsv        标量信息（格式版本、基因数、种群大小、全部超参数、训练报告摘要）
''' genes.tsv           基因编号与转录因子标记(gene&lt;TAB>is_tf)
''' graph.tsv           裁剪之后的调控边(target&lt;TAB>regulator&lt;TAB>mi&lt;TAB>correlation&lt;TAB>interaction&lt;TAB>prior&lt;TAB>regulation)
''' baseline.bin        野生型均值与标准差(int32 计数 + 小端 double)
''' population.bin      迭代种群的初始表达(int32 基因数 + int32 细胞数 + 基因主序 double)
''' cells.bin           种群对应的表达矩阵列下标(int32 计数 + int32)
''' models/index.tsv    逐基因模型的索引与训练统计
''' models.txt          逐基因的 TGBoost 模型（分段文本，与 ModelSerializer 的格式一致）
''' report.tsv          逐基因拟合优度表（可直接用 Excel 打开）
''' importance.tsv      逐调控因子的重要性表
''' ```
''' 
''' 加载之后不需要原始表达矩阵就可以继续做虚拟扰动实验，
''' 因为迭代只需要 <c>population.bin</c> 与逐基因模型。
''' </summary>
Public Module GeneTreeStorage

    ''' <summary>当前存档格式的版本号</summary>
    Private Const FORMAT_VERSION As String = "1"

    ''' <summary>模型分段文本的节标记前缀</summary>
    Private Const MODEL_SECTION As String = "[model:"

#Region "save"

    ''' <summary>
    ''' 把 GeneTree 模型写入 zip 存档
    ''' </summary>
    ''' <param name="grn">已经完成 <see cref="XGBoostGRN.Fit"/> 或者由存档加载的对象</param>
    ''' <param name="file">目标可写流（调用返回之后流仍然保持打开）</param>
    Public Sub Save(grn As XGBoostGRN, file As Stream)
        If grn Is Nothing Then
            Throw New ArgumentNullException(NameOf(grn))
        End If
        If file Is Nothing Then
            Throw New ArgumentNullException(NameOf(file))
        End If
        If Not grn.IsFitted Then
            Throw New InvalidOperationException("GeneTree 模型尚未拟合，请先调用 Fit() 再保存")
        End If

        Dim culture As CultureInfo = Thread.CurrentThread.CurrentCulture

        Try
            ' ModelSerializer 使用当前区域文化格式化浮点数，保存期间固定为 InvariantCulture
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture

            Using zip As New ZipArchive(file, ZipArchiveMode.Create, leaveOpen:=True)
                Call writeManifest(zip, grn)
                Call writeGenes(zip, grn)
                Call writeGraph(zip, grn)
                Call writeBaseline(zip, grn)
                Call writePopulation(zip, grn)
                Call writeModelIndex(zip, grn)
                Call writeModels(zip, grn)
                Call writeReport(zip, grn)
                Call writeDiscovery(zip, grn)
            End Using
        Finally
            Thread.CurrentThread.CurrentCulture = culture
        End Try
    End Sub

    Private Sub writeManifest(zip As ZipArchive, grn As XGBoostGRN)
        Dim values As New SortedDictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
        Dim opt As GeneTreeConfig = grn.Config

        values("format") = FORMAT_VERSION
        values("created") = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
        values("genes") = grn.GeneNames.Length.ToString
        values("population") = grn.PopulationSize.ToString
        values("models") = grn.Models.SafeQuery.Count(Function(m) m IsNot Nothing).ToString

        values("opt.maxRegulators") = opt.MaxRegulators.ToString
        values("opt.includeUndirectedEdges") = bool(opt.IncludeUndirectedEdges)
        values("opt.maxGenes") = opt.MaxGenes.ToString
        values("opt.maxSamplesPerModel") = opt.MaxSamplesPerModel.ToString
        values("opt.validationRatio") = number(opt.ValidationRatio)
        values("opt.minSamples") = opt.MinSamples.ToString
        values("opt.numBoostRound") = opt.NumBoostRound.ToString
        values("opt.maxDepth") = opt.MaxDepth.ToString
        values("opt.learningRate") = number(opt.LearningRate)
        values("opt.rowSample") = number(opt.RowSample)
        values("opt.colSample") = number(opt.ColSample)
        values("opt.lambda") = number(opt.Lambda)
        values("opt.gamma") = number(opt.Gamma)
        values("opt.minChildWeight") = number(opt.MinChildWeight)
        values("opt.minChildWeightRatio") = number(opt.MinChildWeightRatio)
        values("opt.minSampleSplit") = opt.MinSampleSplit.ToString
        values("opt.earlyStoppingRounds") = opt.EarlyStoppingRounds.ToString
        values("opt.threadsPerModel") = opt.ThreadsPerModel.ToString
        values("opt.dampingAlpha") = number(opt.DampingAlpha)
        values("opt.maxIterations") = opt.MaxIterations.ToString
        values("opt.convergenceTolerance") = number(opt.ConvergenceTolerance)
        values("opt.movingGeneTolerance") = number(opt.MovingGeneTolerance)
        values("opt.populationSize") = opt.PopulationSize.ToString
        values("opt.clippingFactor") = number(opt.ClippingFactor)
        values("opt.clipNegative") = bool(opt.ClipNegative)
        values("opt.significanceZScore") = number(opt.SignificanceZScore)
        values("opt.seed") = opt.Seed.ToString
        values("opt.maxDegreeOfParallelism") = opt.MaxDegreeOfParallelism.ToString
        values("opt.verbose") = bool(opt.Verbose)
        values("opt.progressInterval") = opt.ProgressInterval.ToString
        values("opt.knockdownResidual") = number(opt.KnockdownResidual)
        values("opt.overexpressionFactor") = number(opt.OverexpressionFactor)
        values("opt.maxClampRatio") = number(opt.MaxClampRatio)
        values("opt.maxIterationsHardCap") = opt.MaxIterationsHardCap.ToString
        values("opt.contextAlignmentMode") = opt.ContextAlignmentMode.ToString
        values("opt.baselineFromControlSteadyState") = bool(opt.BaselineFromControlSteadyState)
        values("opt.collapseUniformPopulation") = bool(opt.CollapseUniformPopulation)
        ' ---- 结构发现 ----
        values("opt.edgeDiscovery") = opt.EdgeDiscovery.ToString
        values("opt.discoveryMaxPerGene") = opt.DiscoveryMaxPerGene.ToString
        values("opt.discoveryMaxCandidates") = opt.DiscoveryMaxCandidates.ToString
        values("opt.discoveryMinCorrelation") = number(opt.DiscoveryMinCorrelation)
        values("opt.discoveryMinDeltaR2") = number(opt.DiscoveryMinDeltaR2)
        values("opt.discoverySampleSize") = opt.DiscoverySampleSize.ToString
        values("opt.discoveryMaxGenes") = opt.DiscoveryMaxGenes.ToString
        values("opt.discoveryCandidates") = CStr(If(opt.DiscoveryCandidates, New String() {}).Length)

        Dim report As TrainingReport = grn.ReportData

        If report IsNot Nothing Then
            values("report.boosted") = report.BoostedModels.ToString
            values("report.constant") = report.ConstantModels.ToString
            values("report.sourceNodes") = report.SourceNodes.ToString
            values("report.medianR2") = number(report.MedianR2)
            values("report.meanR2") = number(report.MeanR2)
            values("report.informative") = report.InformativeModels.ToString
            values("report.cells") = report.Cells.ToString
            values("report.discoveredEdges") = report.DiscoveredEdges.ToString

            If report.Discovery IsNot Nothing Then
                values("report.discoveryTargets") = report.Discovery.TargetsExamined.ToString
                values("report.discoveryScreened") = report.Discovery.CandidatesScreened.ToString
            End If
        End If

        Using stream As Stream = createEntry(zip, "manifest.tsv")
            Using writer As New StreamWriter(stream, Encoding.UTF8)
                For Each item As KeyValuePair(Of String, String) In values
                    Call writer.WriteLine($"{item.Key}{ControlChars.Tab}{item.Value}")
                Next
            End Using
        End Using
    End Sub

    Private Sub writeGenes(zip As ZipArchive, grn As XGBoostGRN)
        Using stream As Stream = createEntry(zip, "genes.tsv")
            Using writer As New StreamWriter(stream, Encoding.UTF8)
                Dim names As String() = grn.GeneNames
                Dim tfs As String() = grn.TFNames
                Dim isTf As New HashSet(Of String)(tfs, StringComparer.OrdinalIgnoreCase)

                For i As Integer = 0 To names.Length - 1
                    Call writer.WriteLine($"{names(i)}{ControlChars.Tab}{If(isTf.Contains(names(i)), "1", "0")}")
                Next
            End Using
        End Using
    End Sub

    Private Sub writeGraph(zip As ZipArchive, grn As XGBoostGRN)
        Using stream As Stream = createEntry(zip, "graph.tsv")
            Using writer As New StreamWriter(stream, Encoding.UTF8)
                Call writer.WriteLine("target" & ControlChars.Tab & "regulator" & ControlChars.Tab & "mi" &
                                      ControlChars.Tab & "correlation" & ControlChars.Tab & "interaction" &
                                      ControlChars.Tab & "prior" & ControlChars.Tab & "regulation")

                For Each input As RegulationInput In grn.Graph.Regulations()
                    Call writer.WriteLine(String.Join(ControlChars.Tab,
                        input.target,
                        input.regulator,
                        number(input.mi),
                        number(input.correlation),
                        If(input.interaction, ""),
                        If(input.prior, "1", "0"),
                        If(input.regulation, "")))
                Next
            End Using
        End Using
    End Sub

    Private Sub writeBaseline(zip As ZipArchive, grn As XGBoostGRN)
        Using stream As Stream = createEntry(zip, "baseline.bin")
            Using buffered As New BufferedStream(stream, 1 << 16)
                Using writer As New BinaryWriter(buffered, Encoding.UTF8)
                    Dim means As Double() = grn.WildtypeMeans
                    Dim sds As Double() = grn.WildtypeSDs

                    writer.Write(means.Length)

                    For i As Integer = 0 To means.Length - 1
                        writer.Write(means(i))
                    Next

                    writer.Write(sds.Length)

                    For i As Integer = 0 To sds.Length - 1
                        writer.Write(sds(i))
                    Next
                End Using
            End Using
        End Using
    End Sub

    Private Sub writePopulation(zip As ZipArchive, grn As XGBoostGRN)
        Using stream As Stream = createEntry(zip, "population.bin")
            Using buffered As New BufferedStream(stream, 1 << 20)
                Using writer As New BinaryWriter(buffered, Encoding.UTF8)
                    Dim population As Double()() = grn.PopulationData
                    Dim cells As Integer() = grn.PopulationCells
                    Dim n As Integer = population.Length
                    Dim p As Integer = If(n > 0 AndAlso population(0) IsNot Nothing, population(0).Length, 0)

                    writer.Write(n)
                    writer.Write(p)

                    For g As Integer = 0 To n - 1
                        Dim values As Double() = population(g)

                        For c As Integer = 0 To p - 1
                            writer.Write(values(c))
                        Next
                    Next

                    writer.Write(cells.Length)

                    For i As Integer = 0 To cells.Length - 1
                        writer.Write(cells(i))
                    Next
                End Using
            End Using
        End Using
    End Sub

    Private Sub writeModelIndex(zip As ZipArchive, grn As XGBoostGRN)
        Using stream As Stream = createEntry(zip, "models/index.tsv")
            Using writer As New StreamWriter(stream, Encoding.UTF8)
                Call writer.WriteLine("index" & ControlChars.Tab & "gene" & ControlChars.Tab & "kind" &
                                      ControlChars.Tab & "constant" & ControlChars.Tab & "minValue" &
                                      ControlChars.Tab & "maxValue" & ControlChars.Tab & "trees" &
                                      ControlChars.Tab & "nRegulators" & ControlChars.Tab & "regulators")

                For i As Integer = 0 To grn.Models.Length - 1
                    Dim model As GeneModel = grn.Models(i)

                    If model Is Nothing Then
                        Continue For
                    End If

                    Call writer.WriteLine(String.Join(ControlChars.Tab,
                        i.ToString,
                        model.Gene,
                        model.Kind,
                        number(model.Constant),
                        number(model.MinValue),
                        number(model.MaxValue),
                        model.TreeCount.ToString,
                        If(model.Regulators Is Nothing, "0", model.Regulators.Length.ToString),
                        If(model.Regulators Is Nothing, "", String.Join("|", model.Regulators))))
                Next
            End Using
        End Using
    End Sub

    Private Sub writeModels(zip As ZipArchive, grn As XGBoostGRN)
        Using stream As Stream = createEntry(zip, "models.txt")
            Using buffered As New BufferedStream(stream, 1 << 20)
                Using writer As New StreamWriter(buffered, Encoding.UTF8)
                    For i As Integer = 0 To grn.Models.Length - 1
                        Dim model As GeneModel = grn.Models(i)

                        If model Is Nothing OrElse model.Booster Is Nothing Then
                            Continue For
                        End If

                        Call writer.WriteLine($"{MODEL_SECTION}{i}]")

                        For Each line As String In ModelSerializer.save_model(model.Booster)
                            Call writer.WriteLine(line)
                        Next
                    Next
                End Using
            End Using
        End Using
    End Sub

    Private Sub writeReport(zip As ZipArchive, grn As XGBoostGRN)
        Using stream As Stream = createEntry(zip, "report.tsv")
            Using writer As New StreamWriter(stream, Encoding.UTF8)
                Call writer.WriteLine("gene,kind,nRegulators,nTrain,nValidate,r2,rmse,baselineRmse,improvement,trees,minValue,maxValue")

                For Each model As GeneModel In grn.Models.SafeQuery.Where(Function(m) m IsNot Nothing)
                    Dim p As ModelPerformance = model.Performance

                    Call writer.WriteLine(String.Join(",",
                        model.Gene,
                        If(p.kind, model.Kind),
                        p.nRegulators.ToString,
                        p.nTrain.ToString,
                        p.nValidate.ToString,
                        number(p.r2),
                        number(p.rmse),
                        number(p.baselineRmse),
                        number(p.improvement),
                        p.trees.ToString,
                        number(p.minValue),
                        number(p.maxValue)))
                Next
            End Using
        End Using

        Using stream As Stream = createEntry(zip, "importance.tsv")
            Using writer As New StreamWriter(stream, Encoding.UTF8)
                Call writer.WriteLine("gene,regulator,rank,mi,correlation,splits,sensitivity,source,delta_r2")

                For Each model As GeneModel In grn.Models.SafeQuery.Where(Function(m) m IsNot Nothing)
                    For Each item As RegulatorImportance In model.Performance.importance.SafeQuery
                        Call writer.WriteLine(String.Join(",",
                            model.Gene,
                            item.regulator,
                            item.rank.ToString,
                            number(item.mi),
                            number(item.correlation),
                            item.splits.ToString,
                            number(item.sensitivity),
                            If(item.source, "aracne"),
                            number(item.deltaR2)))
                    Next
                Next
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' 写出结构发现结果（<c>discovery.tsv</c> 与候选基因清单）
    ''' </summary>
    Private Sub writeDiscovery(zip As ZipArchive, grn As XGBoostGRN)
        Dim report As TrainingReport = grn.ReportData

        If report Is Nothing OrElse report.Discovery Is Nothing Then
            Return
        End If

        Using stream As Stream = createEntry(zip, "discovery.tsv")
            Using writer As New StreamWriter(stream, Encoding.UTF8)
                Call report.Discovery.WriteTo(writer)
            End Using
        End Using

        Dim candidates As String() = If(grn.Config.DiscoveryCandidates, New String() {})

        If candidates.Length > 0 Then
            Using stream As Stream = createEntry(zip, "discovery_candidates.txt")
                Using writer As New StreamWriter(stream, Encoding.UTF8)
                    For Each gene As String In candidates
                        Call writer.WriteLine(gene)
                    Next
                End Using
            End Using
        End If
    End Sub

#End Region

#Region "load"

    ''' <summary>
    ''' 从 zip 存档加载 GeneTree 模型
    ''' </summary>
    ''' <param name="file">zip 压缩包的可读流（调用返回之后流仍然保持打开）</param>
    ''' <returns>还原出来的对象，其扰动预测结果与保存前一致</returns>
    Public Function Load(file As Stream) As XGBoostGRN
        If file Is Nothing Then
            Throw New ArgumentNullException(NameOf(file))
        End If

        Dim culture As CultureInfo = Thread.CurrentThread.CurrentCulture

        Try
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture

            Using zip As New ZipArchive(file, ZipArchiveMode.Read, leaveOpen:=True)
                Dim values As Dictionary(Of String, String) = readManifest(zip)
                Dim version As String = getValue(values, "format")

                If version Is Nothing Then
                    Throw New InvalidDataException("该压缩包不是有效的 GeneTree 模型：缺少 manifest.tsv")
                ElseIf Not String.Equals(version, FORMAT_VERSION) Then
                    Throw New InvalidDataException($"不支持的 GeneTree 模型格式 '{version}'，期望 {FORMAT_VERSION}")
                End If

                Dim config As GeneTreeConfig = readConfig(values)

                Call config.SetDiscoveryCandidates(readDiscoveryCandidates(zip))

                Dim genes As String() = Nothing
                Dim tfFlags As Boolean() = Nothing

                Call readGenes(zip, genes, tfFlags)
                Call checkGeneCount(values, genes.Length)

                Dim records As RegulationInput() = readGraph(zip)
                Dim graph As GeneRegulationGraph = GeneRegulationGraph.FromRegulations(genes, tfFlags, records, config)
                Dim models As GeneModel() = readModels(zip, genes)
                Dim baseline As (means As Double(), sds As Double()) = readBaseline(zip)
                Dim population As (values As Double()(), cells As Integer()) = readPopulation(zip)

                Call readReport(zip, models)

                Dim grn As XGBoostGRN = XGBoostGRN.FromArchive(graph, config, genes, tfFlags)
                Dim report As TrainingReport = TrainingReport.FromModels(
                    models, config, getInt(values, "report.cells", 0))

                ' 结构发现的明细表按存档恢复（旧存档没有该条目时保持 Nothing）
                Dim discovery As DiscoveryReport = readDiscovery(zip, config, values)

                If discovery IsNot Nothing Then
                    report.Discovery = discovery
                End If

                Call grn.RestoreState(models, baseline.means, baseline.sds, population.values, population.cells, report)

                Return grn
            End Using
        Finally
            Thread.CurrentThread.CurrentCulture = culture
        End Try
    End Function

    Private Function readManifest(zip As ZipArchive) As Dictionary(Of String, String)
        Dim values As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

        Using stream As Stream = openEntry(zip, "manifest.tsv")
            If stream Is Nothing Then
                Return values
            End If

            Using reader As New StreamReader(stream, Encoding.UTF8)
                Dim line As String = reader.ReadLine()

                While line IsNot Nothing
                    Dim i As Integer = line.IndexOf(ControlChars.Tab)

                    If i > 0 Then
                        values(line.Substring(0, i).Trim) = line.Substring(i + 1).Trim
                    End If

                    line = reader.ReadLine()
                End While
            End Using
        End Using

        Return values
    End Function

    Private Sub checkGeneCount(values As Dictionary(Of String, String), n As Integer)
        Dim expected As Integer = getInt(values, "genes", n)

        If expected <> n Then
            Throw New InvalidDataException($"GeneTree 存档损坏：manifest 记录了 {expected} 个基因，但是 genes.tsv 只有 {n} 个")
        End If
    End Sub

    Private Function readConfig(values As Dictionary(Of String, String)) As GeneTreeConfig
        Return New GeneTreeConfig With {
            .MaxRegulators = getInt(values, "opt.maxRegulators", 10),
            .IncludeUndirectedEdges = getBool(values, "opt.includeUndirectedEdges", False),
            .MaxGenes = getInt(values, "opt.maxGenes", 0),
            .MaxSamplesPerModel = getInt(values, "opt.maxSamplesPerModel", 1000),
            .ValidationRatio = getDouble(values, "opt.validationRatio", 0.2R),
            .MinSamples = getInt(values, "opt.minSamples", 30),
            .NumBoostRound = getInt(values, "opt.numBoostRound", 40),
            .MaxDepth = getInt(values, "opt.maxDepth", 3),
            .LearningRate = getDouble(values, "opt.learningRate", 0.05R),
            .RowSample = getDouble(values, "opt.rowSample", 0.8R),
            .ColSample = getDouble(values, "opt.colSample", 0.8R),
            .Lambda = getDouble(values, "opt.lambda", 5.0R),
            .Gamma = getDouble(values, "opt.gamma", 0.0R),
            .MinChildWeight = getDouble(values, "opt.minChildWeight", 2.0R),
            .MinChildWeightRatio = getDouble(values, "opt.minChildWeightRatio", 0.05R),
            .MinSampleSplit = getInt(values, "opt.minSampleSplit", 5),
            .EarlyStoppingRounds = getInt(values, "opt.earlyStoppingRounds", 10),
            .ThreadsPerModel = getInt(values, "opt.threadsPerModel", -1),
            .DampingAlpha = getDouble(values, "opt.dampingAlpha", 0.1R),
            .MaxIterations = getInt(values, "opt.maxIterations", 60),
            .ConvergenceTolerance = getDouble(values, "opt.convergenceTolerance", 0.0001R),
            .MovingGeneTolerance = getDouble(values, "opt.movingGeneTolerance", 0.001R),
            .PopulationSize = getInt(values, "opt.populationSize", 100),
            .ClippingFactor = getDouble(values, "opt.clippingFactor", 1.2R),
            .ClipNegative = getBool(values, "opt.clipNegative", True),
            .SignificanceZScore = getDouble(values, "opt.significanceZScore", 2.0R),
            .Seed = getInt(values, "opt.seed", 1234567),
            .MaxDegreeOfParallelism = getInt(values, "opt.maxDegreeOfParallelism", -1),
            .Verbose = getBool(values, "opt.verbose", True),
            .ProgressInterval = getInt(values, "opt.progressInterval", 200),
            .KnockdownResidual = getDouble(values, "opt.knockdownResidual", 0.2R),
            .OverexpressionFactor = getDouble(values, "opt.overexpressionFactor", 2.0R),
            .MaxClampRatio = getDouble(values, "opt.maxClampRatio", 0.05R),
            .MaxIterationsHardCap = getInt(values, "opt.maxIterationsHardCap", 0),
            .ContextAlignmentMode = parseAlignmentMode(values),
            .BaselineFromControlSteadyState = getBool(values, "opt.baselineFromControlSteadyState", True),
            .CollapseUniformPopulation = getBool(values, "opt.collapseUniformPopulation", True),
            .EdgeDiscovery = parseDiscoveryMode(getValue(values, "opt.edgeDiscovery")),
            .DiscoveryMaxPerGene = getInt(values, "opt.discoveryMaxPerGene", 2),
            .DiscoveryMaxCandidates = getInt(values, "opt.discoveryMaxCandidates", 5),
            .DiscoveryMinCorrelation = getDouble(values, "opt.discoveryMinCorrelation", 0.1R),
            .DiscoveryMinDeltaR2 = getDouble(values, "opt.discoveryMinDeltaR2", 0.002R),
            .DiscoverySampleSize = getInt(values, "opt.discoverySampleSize", 1000),
            .DiscoveryMaxGenes = getInt(values, "opt.discoveryMaxGenes", 0)
            }
    End Function

    ''' <summary>
    ''' 解析存档中结构发现模式的文本（兼容缺失与非法值）
    ''' </summary>
    Private Function parseDiscoveryMode(text As String) As EdgeDiscoveryMode
        Dim mode As EdgeDiscoveryMode

        If [Enum].TryParse(If(text, "").Trim(), True, mode) Then
            Return mode
        Else
            Return EdgeDiscoveryMode.AracneOnly
        End If
    End Function

    ''' <summary>
    ''' 解析存档中的上下文对齐模式。
    ''' </summary>
    ''' <remarks>
    ''' 新存档写入 <c>opt.contextAlignmentMode</c>；旧存档只有布尔键
    ''' <c>opt.contextQuantileAlignment</c>（<c>True</c> → <see cref="ContextAlignmentMode.Quantile"/>，
    ''' <c>False</c> → <see cref="ContextAlignmentMode.None"/>），这里做向后兼容，
    ''' 避免旧模型包加载之后行为突变。
    ''' </remarks>
    Private Function parseAlignmentMode(values As Dictionary(Of String, String)) As ContextAlignmentMode
        Dim text As String = getValue(values, "opt.contextAlignmentMode")

        If Not String.IsNullOrWhiteSpace(text) Then
            Dim mode As ContextAlignmentMode

            If [Enum].TryParse(text.Trim(), True, mode) Then
                Return mode
            End If
        End If

        Return If(getBool(values, "opt.contextQuantileAlignment", True),
                  ContextAlignmentMode.Quantile,
                  ContextAlignmentMode.None)
    End Function

    Private Sub readGenes(zip As ZipArchive, ByRef genes As String(), ByRef tfFlags As Boolean())
        Using stream As Stream = openEntry(zip, "genes.tsv")
            If stream Is Nothing Then
                Throw New InvalidDataException("GeneTree 存档损坏：缺少 genes.tsv")
            End If

            Dim names As New List(Of String)
            Dim flags As New List(Of Boolean)

            Using reader As New StreamReader(stream, Encoding.UTF8)
                Dim line As String = reader.ReadLine()

                While line IsNot Nothing
                    If line.Length > 0 Then
                        Dim fields As String() = line.Split(ControlChars.Tab)

                        Call names.Add(fields(0))
                        Call flags.Add(fields.Length > 1 AndAlso fields(1).Trim() = "1")
                    End If

                    line = reader.ReadLine()
                End While
            End Using

            genes = names.ToArray
            tfFlags = flags.ToArray
        End Using
    End Sub

    Private Function readGraph(zip As ZipArchive) As RegulationInput()
        Dim result As New List(Of RegulationInput)

        Using stream As Stream = openEntry(zip, "graph.tsv")
            If stream Is Nothing Then
                Return result.ToArray
            End If

            Using reader As New StreamReader(stream, Encoding.UTF8)
                Call reader.ReadLine() ' header

                Dim line As String = reader.ReadLine()

                While line IsNot Nothing
                    If line.Length > 0 Then
                        Dim f As String() = line.Split(ControlChars.Tab)

                        If f.Length >= 7 Then
                            Call result.Add(New RegulationInput With {
                                .target = f(0),
                                .regulator = f(1),
                                .mi = parseNumber(f(2)),
                                .correlation = parseNumber(f(3)),
                                .interaction = f(4),
                                .prior = f(5) = "1",
                                .regulation = f(6)
                            })
                        End If
                    End If

                    line = reader.ReadLine()
                End While
            End Using
        End Using

        Return result.ToArray
    End Function

    Private Function readModels(zip As ZipArchive, genes As String()) As GeneModel()

        Dim models As GeneModel() = New GeneModel(genes.Length - 1) {}
        Dim index As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To genes.Length - 1
            index(genes(i)) = i
        Next

        ' ---- 索引 ----
        Using stream As Stream = openEntry(zip, "models/index.tsv")
            If stream Is Nothing Then
                Throw New InvalidDataException("GeneTree 存档损坏：缺少 models/index.tsv")
            End If

            Using reader As New StreamReader(stream, Encoding.UTF8)
                Call reader.ReadLine() ' header

                Dim line As String = reader.ReadLine()

                While line IsNot Nothing
                    If line.Length > 0 Then
                        Dim f As String() = line.Split(ControlChars.Tab)

                        If f.Length >= 9 Then
                            Dim i As Integer = Integer.Parse(f(0))
                            Dim regulators As String() = If(f(8).Length = 0, New String() {}, f(8).Split("|"c))
                            Dim indices As New List(Of Integer)

                            For Each name As String In regulators
                                Dim row As Integer

                                If index.TryGetValue(name, row) Then
                                    Call indices.Add(row)
                                Else
                                    Throw New InvalidDataException(
                                        $"GeneTree 存档损坏：基因 '{f(1)}' 的调控因子 '{name}' 不在 genes.tsv 之中")
                                End If
                            Next

                            Dim model As New GeneModel With {
                                .Gene = f(1),
                                .Constant = parseNumber(f(3)),
                                .MinValue = parseNumber(f(4)),
                                .MaxValue = parseNumber(f(5)),
                                .Regulators = regulators,
                                .RegulatorIndex = indices.ToArray,
                                .Performance = New ModelPerformance With {
                                    .kind = f(2),
                                    .trees = Integer.Parse(f(6)),
                                    .nRegulators = Integer.Parse(f(7)),
                                    .minValue = parseNumber(f(4)),
                                    .maxValue = parseNumber(f(5))
                                }
                            }

                            models(i) = model
                        End If
                    End If

                    line = reader.ReadLine()
                End While
            End Using
        End Using

        ' ---- 树模型 ----
        Using stream As Stream = openEntry(zip, "models.txt")
            If stream Is Nothing Then
                Return models
            End If

            Using reader As New StreamReader(stream, Encoding.UTF8)
                Dim buffer As List(Of String) = Nothing
                Dim current As Integer = -1
                Dim line As String = reader.ReadLine()

                While line IsNot Nothing
                    If line.StartsWith(MODEL_SECTION, StringComparison.Ordinal) Then
                        Call flush(models, current, buffer)

                        current = Integer.Parse(line.Substring(MODEL_SECTION.Length).TrimEnd("]"c))
                        buffer = New List(Of String)
                    ElseIf buffer IsNot Nothing Then
                        Call buffer.Add(line)
                    End If

                    line = reader.ReadLine()
                End While

                Call flush(models, current, buffer)
            End Using
        End Using

        Return models
    End Function

    Private Sub flush(models As GeneModel(), index As Integer, buffer As List(Of String))
        If buffer Is Nothing OrElse buffer.Count = 0 Then
            Return
        End If
        If index < 0 OrElse index >= models.Length OrElse models(index) Is Nothing Then
            Return
        End If

        Try
            models(index).Booster = ModelSerializer.load_model(buffer.ToArray)
        Catch ex As Exception
            Throw New InvalidDataException(
                $"GeneTree 存档损坏：无法解析基因 '{models(index).Gene}' 的树模型 -> {ex.Message}", ex)
        End Try
    End Sub

    Private Function readBaseline(zip As ZipArchive) As (means As Double(), sds As Double())
        Using stream As Stream = openEntry(zip, "baseline.bin")
            If stream Is Nothing Then
                Return (New Double() {}, New Double() {})
            End If

            Using buffered As New BufferedStream(stream, 1 << 16)
                Using reader As New BinaryReader(buffered, Encoding.UTF8)
                    Dim n As Integer = reader.ReadInt32()
                    Dim means As Double() = New Double(n - 1) {}

                    For i As Integer = 0 To n - 1
                        means(i) = reader.ReadDouble()
                    Next

                    Dim m As Integer = reader.ReadInt32()
                    Dim sds As Double() = New Double(m - 1) {}

                    For i As Integer = 0 To m - 1
                        sds(i) = reader.ReadDouble()
                    Next

                    Return (means, sds)
                End Using
            End Using
        End Using
    End Function

    Private Function readPopulation(zip As ZipArchive) As (values As Double()(), cells As Integer())
        Using stream As Stream = openEntry(zip, "population.bin")
            If stream Is Nothing Then
                Return (New Double()() {}, New Integer() {})
            End If

            Using buffered As New BufferedStream(stream, 1 << 20)
                Using reader As New BinaryReader(buffered, Encoding.UTF8)
                    Dim n As Integer = reader.ReadInt32()
                    Dim p As Integer = reader.ReadInt32()
                    Dim values As Double()() = New Double(n - 1)() {}

                    For g As Integer = 0 To n - 1
                        Dim row As Double() = New Double(p - 1) {}

                        For c As Integer = 0 To p - 1
                            row(c) = reader.ReadDouble()
                        Next

                        values(g) = row
                    Next

                    Dim count As Integer = reader.ReadInt32()
                    Dim cells As Integer() = New Integer(count - 1) {}

                    For i As Integer = 0 To count - 1
                        cells(i) = reader.ReadInt32()
                    Next

                    Return (values, cells)
                End Using
            End Using
        End Using
    End Function

    ''' <summary>
    ''' 把 report.tsv / importance.tsv 读回到模型的 <see cref="GeneModel.Performance"/> 上
    ''' </summary>
    Private Sub readReport(zip As ZipArchive, models As GeneModel())
        Dim index As New Dictionary(Of String, GeneModel)(StringComparer.OrdinalIgnoreCase)

        For Each model As GeneModel In models.SafeQuery.Where(Function(m) m IsNot Nothing)
            index(model.Gene) = model
        Next

        Using stream As Stream = openEntry(zip, "report.tsv")
            If stream IsNot Nothing Then
                Using reader As New StreamReader(stream, Encoding.UTF8)
                    Call reader.ReadLine() ' header

                    Dim line As String = reader.ReadLine()

                    While line IsNot Nothing
                        If line.Length > 0 Then
                            Dim f As String() = line.Split(","c)

                            If f.Length >= 12 Then
                                Dim model As GeneModel = Nothing

                                If index.TryGetValue(f(0), model) Then
                                    Dim p As ModelPerformance = model.Performance

                                    p.kind = f(1)
                                    p.nRegulators = Integer.Parse(f(2))
                                    p.nTrain = Integer.Parse(f(3))
                                    p.nValidate = Integer.Parse(f(4))
                                    p.r2 = parseNumber(f(5))
                                    p.rmse = parseNumber(f(6))
                                    p.baselineRmse = parseNumber(f(7))
                                    p.trees = Integer.Parse(f(9))
                                    p.minValue = parseNumber(f(10))
                                    p.maxValue = parseNumber(f(11))
                                End If
                            End If
                        End If

                        line = reader.ReadLine()
                    End While
                End Using
            End If
        End Using

        Using stream As Stream = openEntry(zip, "importance.tsv")
            If stream Is Nothing Then
                Return
            End If

            Dim buckets As New Dictionary(Of String, List(Of RegulatorImportance))(StringComparer.OrdinalIgnoreCase)

            Using reader As New StreamReader(stream, Encoding.UTF8)
                Call reader.ReadLine() ' header

                Dim line As String = reader.ReadLine()

                While line IsNot Nothing
                    If line.Length > 0 Then
                        Dim f As String() = line.Split(","c)

                        If f.Length >= 7 Then
                            Dim list As List(Of RegulatorImportance) = Nothing

                            If Not buckets.TryGetValue(f(0), list) Then
                                list = New List(Of RegulatorImportance)
                                buckets(f(0)) = list
                            End If

                            Call list.Add(New RegulatorImportance With {
                                .regulator = f(1),
                                .rank = Integer.Parse(f(2)),
                                .mi = parseNumber(f(3)),
                                .correlation = parseNumber(f(4)),
                                .splits = Integer.Parse(f(5)),
                                .sensitivity = parseNumber(f(6)),
                                .source = If(f.Length >= 8 AndAlso f(7).Length > 0, f(7), "aracne"),
                                .deltaR2 = If(f.Length >= 9, parseNumber(f(8)), 0.0R)
                            })
                        End If
                    End If

                    line = reader.ReadLine()
                End While
            End Using

            For Each item As KeyValuePair(Of String, List(Of RegulatorImportance)) In buckets
                Dim model As GeneModel = Nothing

                If index.TryGetValue(item.Key, model) Then
                    model.Performance.importance = item.Value _
                        .OrderBy(Function(x) x.rank) _
                        .ToArray
                End If
            Next
        End Using
    End Sub

    ''' <summary>
    ''' 读取存档之中的候选基因清单（<c>discovery_candidates.txt</c>）
    ''' </summary>
    Private Function readDiscoveryCandidates(zip As ZipArchive) As String()
        Using stream As Stream = openEntry(zip, "discovery_candidates.txt")
            If stream Is Nothing Then
                Return New String() {}
            End If

            Dim genes As New List(Of String)

            Using reader As New StreamReader(stream, Encoding.UTF8)
                Dim line As String = reader.ReadLine()

                While line IsNot Nothing
                    Dim gene As String = If(line, "").Trim()

                    If gene.Length > 0 Then
                        Call genes.Add(gene)
                    End If

                    line = reader.ReadLine()
                End While
            End Using

            Return genes.ToArray
        End Using
    End Function

    ''' <summary>
    ''' 读取存档之中的结构发现明细（<c>discovery.tsv</c>）
    ''' </summary>
    Private Function readDiscovery(zip As ZipArchive,
                                   config As GeneTreeConfig,
                                   values As Dictionary(Of String, String)) As DiscoveryReport
        Using stream As Stream = openEntry(zip, "discovery.tsv")
            If stream Is Nothing Then
                Return Nothing
            End If

            Dim edges As New List(Of DiscoveryEdge)
            Dim header As Boolean = False

            Using reader As New StreamReader(stream, Encoding.UTF8)
                Dim line As String = reader.ReadLine()

                While line IsNot Nothing
                    If line.StartsWith("#") Then
                        line = reader.ReadLine()
                        Continue While
                    End If
                    If Not header Then
                        header = True
                        line = reader.ReadLine()
                        Continue While
                    End If

                    Dim f As String() = line.Split(","c)

                    If f.Length >= 10 Then
                        Call edges.Add(New DiscoveryEdge With {
                            .target = f(0),
                            .regulator = f(1),
                            .status = f(3),
                            .correlation = parseNumber(f(4)),
                            .deltaR2 = parseNumber(f(5)),
                            .r2Before = parseNumber(f(6)),
                            .r2After = parseNumber(f(7)),
                            .splits = Integer.Parse(f(8)),
                            .sensitivity = parseNumber(f(9)),
                            .reason = If(f.Length >= 11, f(10), "")
                        })
                    End If

                    line = reader.ReadLine()
                End While
            End Using

            If edges.Count = 0 Then
                Return Nothing
            End If

            Return New DiscoveryReport With {
                .Mode = config.EdgeDiscovery,
                .Candidates = config.DiscoveryCandidates,
                .TargetsExamined = getInt(values, "report.discoveryTargets", 0),
                .CandidatesScreened = getInt(values, "report.discoveryScreened", 0),
                .Edges = edges.ToArray
            }
        End Using
    End Function

#End Region

#Region "primitives"

    Private Function createEntry(zip As ZipArchive, name As String) As Stream
        Return zip.CreateEntry(name, CompressionLevel.Optimal).Open()
    End Function

    Private Function openEntry(zip As ZipArchive, name As String) As Stream
        Dim entry As ZipArchiveEntry = zip.GetEntry(name)

        If entry Is Nothing Then
            Return Nothing
        Else
            Return entry.Open()
        End If
    End Function

    Private Function number(x As Double) As String
        Return x.ToString("G17", CultureInfo.InvariantCulture)
    End Function

    Private Function bool(x As Boolean) As String
        Return If(x, "1", "0")
    End Function

    Private Function parseNumber(text As String) As Double
        Dim value As Double

        If Double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
            Return value
        Else
            Return 0.0R
        End If
    End Function

    Private Function getValue(values As Dictionary(Of String, String), key As String) As String
        Dim text As String = Nothing

        If values.TryGetValue(key, text) Then
            Return text
        Else
            Return Nothing
        End If
    End Function

    Private Function getInt(values As Dictionary(Of String, String), key As String, fallback As Integer) As Integer
        Dim value As Integer
        Dim text As String = getValue(values, key)

        If text IsNot Nothing AndAlso Integer.TryParse(text, value) Then
            Return value
        Else
            Return fallback
        End If
    End Function

    Private Function getDouble(values As Dictionary(Of String, String), key As String, fallback As Double) As Double
        Dim text As String = getValue(values, key)

        If text Is Nothing Then
            Return fallback
        Else
            Return parseNumber(text)
        End If
    End Function

    Private Function getBool(values As Dictionary(Of String, String), key As String, fallback As Boolean) As Boolean
        Dim text As String = getValue(values, key)

        If text Is Nothing Then
            Return fallback
        Else
            Return text = "1" OrElse String.Equals(text, "true", StringComparison.OrdinalIgnoreCase)
        End If
    End Function

#End Region

End Module
