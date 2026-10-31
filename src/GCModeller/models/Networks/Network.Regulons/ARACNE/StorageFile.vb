Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports System.Text
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.BioSystem.Core

Namespace ARACNE

    ''' <summary>
    ''' Zip 存档的读写：把拟合完成的 <see cref="AracneNetwork"/> 保存成一个压缩包，
    ''' 之后无需重新计算(真实单细胞数据动辄十几分钟)就可以把网络完整重建出来。
    ''' 
    ''' 压缩包内的结构：
    ''' 
    ''' ```
    ''' manifest.tsv        参数 / 统计量 / 阈值等标量信息(key<TAB>value)
    ''' genes.tsv           基因编号与转录因子标记(gene<TAB>is_tf)
    ''' tfs.txt             转录因子列表
    ''' prior.tsv           curated 先验网络(TF<TAB>Target<TAB>Confidence<TAB>Sign)
    ''' expression.bin      预处理之后的表达向量(可选)
    ''' samples.txt         样本编号(可选)
    ''' mi_network.bin      DPI 之后的互信息矩阵
    ''' mi_stage1.bin       DPI 之前的互信息矩阵
    ''' dpi_removed.bin     DPI 剔除的边
    ''' dpi_protected.bin   被先验保护的边
    ''' edges.csv           赋予方向之后的调控边
    ''' ```
    ''' </summary>
    ''' <remarks>
    ''' 二进制块统一使用 ``int32`` 计数 + 小端 ``double``，文本块统一使用 UTF-8；
    ''' 表达矩阵是否随存档保存由 <see cref="Save"/> 的 ``includeExpression`` 参数控制：
    ''' 真实的单细胞矩阵可能有数 GB，只有在需要重新推断或者复用表达向量时才需要保存。
    ''' </remarks>
    Public Module StorageFile

        ''' <summary>
        ''' 当前存档格式的版本号
        ''' </summary>
        Private Const FORMAT_VERSION As String = "1"

        ''' <summary>
        ''' Save the fitted AracneNetwork object to a zip archive file
        ''' </summary>
        ''' <param name="aracne">已经完成 <see cref="AracneNetwork.Fit"/> 的网络对象</param>
        ''' <param name="file">目标 zip 压缩包的输出流(调用返回之后流仍然保持打开)</param>
        ''' <param name="includeExpression">
        ''' 是否把预处理之后的表达向量一并写入存档，默认开启。
        ''' 
        ''' 开启之后载入的对象可以继续做互信息/相关性计算，甚至可以重新 <see cref="AracneNetwork.Fit"/>；
        ''' 关闭之后 <see cref="AracneNetwork.Vectors"/> 与 <see cref="AracneNetwork.Expression"/>
        ''' 为空，但是边表、统计量、互信息矩阵与阈值都仍然可用，适合真实的大矩阵。
        ''' </param>
        <Extension>
        Public Sub Save(aracne As AracneNetwork, file As Stream, Optional includeExpression As Boolean = True)
            If aracne Is Nothing Then
                Throw New ArgumentNullException(NameOf(aracne))
            End If
            If file Is Nothing Then
                Throw New ArgumentNullException(NameOf(file))
            End If
            If aracne.Statistics Is Nothing Then
                Throw New InvalidOperationException("the ARACNE network is not fitted yet, please call Fit() before saving it!")
            End If

            Using zip As ZipArchive = New ZipArchive(file, ZipArchiveMode.Create, leaveOpen:=True)
                Call writeManifest(zip, aracne, includeExpression)
                Call writeGenes(zip, aracne)
                Call writeTFs(zip, aracne)
                Call writePrior(zip, aracne)

                If includeExpression Then
                    Call writeExpression(zip, aracne)
                    Call writeSamples(zip, aracne)
                End If

                Call writeMiEdges(createEntry(zip, "mi_network.bin"), aracne.MiMatrix)
                Call writeMiEdges(createEntry(zip, "mi_stage1.bin"), aracne.MiMatrixBeforeDpi)

                Dim dpi As DpiResult = aracne.Statistics.dpi

                Call writeEdgeList(createEntry(zip, "dpi_removed.bin"), If(dpi Is Nothing, Nothing, dpi.RemovedEdges))
                Call writeEdgeList(createEntry(zip, "dpi_protected.bin"), If(dpi Is Nothing, Nothing, dpi.ProtectedEdges))
                Call writeEdgeTable(createEntry(zip, "edges.csv"), aracne.Regulations())
            End Using
        End Sub

        ''' <summary>
        ''' Load AracneNetwork object from zip archive file
        ''' </summary>
        ''' <param name="file">由 <see cref="Save"/> 生成的 zip 压缩包的输入流</param>
        ''' <returns>重建之后的网络对象</returns>
        ''' <remarks>
        ''' 重建出来的对象在 <see cref="AracneNetwork.Edges"/>、<see cref="AracneNetwork.MiMatrix"/>、
        ''' <see cref="AracneNetwork.MiMatrixBeforeDpi"/>、<see cref="AracneNetwork.Threshold"/>、
        ''' <see cref="AracneNetwork.Statistics"/>、<see cref="AracneNetwork.TFs"/> 之上与原对象一致。
        ''' 
        ''' 如果存档之中没有包含表达向量，那么 <see cref="AracneNetwork.Vectors"/> 与
        ''' <see cref="AracneNetwork.Expression"/> 为空，并且重新调用
        ''' <see cref="AracneNetwork.Fit"/> 会抛出 <see cref="InvalidOperationException"/>。
        ''' </remarks>
        Public Function Load(file As Stream) As AracneNetwork
            If file Is Nothing Then
                Throw New ArgumentNullException(NameOf(file))
            End If

            Using zip As ZipArchive = New ZipArchive(file, ZipArchiveMode.Read, leaveOpen:=True)
                Dim values As Dictionary(Of String, String) = readManifest(zip)
                Dim version As String = getValue(values, "format")

                If version Is Nothing Then
                    Throw New InvalidDataException("this zip archive is not a valid ARACNE model: the manifest.tsv is missing!")
                ElseIf Not String.Equals(version, FORMAT_VERSION) Then
                    Throw New InvalidDataException($"unsupported ARACNE model format '{version}', the expected format is {FORMAT_VERSION}.")
                End If

                Dim genes As String()
                Dim tfFlags As Boolean()

                Call readGenes(zip, genes, tfFlags)

                Dim tfs As String() = readTFs(zip, genes)
                Dim prior As Dictionary(Of Long, RegulatoryEdge) = readPrior(zip, genes)
                Dim options As AracneOptions = readOptions(values, tfs, prior)

                Dim network As ARACNE.MiMatrix = readMiEdges(openEntry(zip, "mi_network.bin"), genes)
                Dim stage1 As ARACNE.MiMatrix = readMiEdges(openEntry(zip, "mi_stage1.bin"), genes)
                Dim edges As AracneEdge() = readEdgeTable(openEntry(zip, "edges.csv"))

                Dim dpi As New DpiResult With {
                    .Matrix = network,
                    .EdgesBefore = getInt(values, "dpi.edgesBefore", If(stage1 Is Nothing, 0, stage1.Count)),
                    .EdgesAfter = getInt(values, "dpi.edgesAfter", If(network Is Nothing, 0, network.Count)),
                    .Passes = getInt(values, "dpi.passes", 0),
                    .TrianglesExamined = getInt(values, "dpi.triangles", 0),
                    .RemovedEdges = readEdgeList(openEntry(zip, "dpi_removed.bin")),
                    .ProtectedEdges = readEdgeList(openEntry(zip, "dpi_protected.bin"))
                }
                Dim threshold As MiThreshold = readThreshold(values)
                Dim stats As AracneStatistics = readStatistics(values, threshold, dpi)

                Dim vectors As Double()()
                Dim expression As Matrix = Nothing

                If getBool(values, "expression", False) Then
                    vectors = readExpression(zip, genes.Length)

                    If vectors IsNot Nothing Then
                        expression = buildExpression(genes, vectors, readSamples(zip))
                    End If
                Else
                    ' 没有保存表达矩阵：用空向量占位，保证 Vectors 的维度与 Genes 一致
                    vectors = New Double(genes.Length - 1)() {}

                    For i As Integer = 0 To genes.Length - 1
                        vectors(i) = New Double() {}
                    Next
                End If

                Return New AracneNetwork(
                    genes:=genes,
                    vectors:=vectors,
                    tfFlags:=tfFlags,
                    expression:=expression,
                    options:=options,
                    scanned:=getInt(values, "scanned", genes.Length),
                    preprocessSeconds:=getDouble(values, "preprocess_seconds", 0.0R),
                    prior:=prior,
                    network:=network,
                    stage1:=stage1,
                    edges:=edges,
                    threshold:=threshold,
                    stats:=stats)
            End Using
        End Function

#Region "manifest"

        ''' <summary>
        ''' 写入标量信息(参数、统计量、阈值)
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="aracne"></param>
        ''' <param name="includeExpression"></param>
        Private Sub writeManifest(zip As ZipArchive, aracne As AracneNetwork, includeExpression As Boolean)
            Dim values As New SortedDictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
            Dim opt As AracneOptions = aracne.Options
            Dim stat As AracneStatistics = aracne.Statistics

            values("format") = FORMAT_VERSION
            values("genes") = aracne.Size.ToString
            values("scanned") = stat.genesScanned.ToString
            values("samples") = stat.samples.ToString
            values("expression") = If(includeExpression, "1", "0")
            values("preprocess_seconds") = formatNumber(stat.preprocessSeconds)

            ' ---- 推断参数 ----
            values("opt.miEstimator") = opt.miEstimator.ToString
            values("opt.adaptiveMaxBins") = opt.adaptiveMaxBins.ToString
            values("opt.adaptiveMinPoints") = opt.adaptiveMinPoints.ToString
            values("opt.histogramBins") = opt.histogramBins.ToString
            values("opt.kernelBandwidth") = formatNumber(opt.kernelBandwidth)
            values("opt.kernelMaxSamples") = opt.kernelMaxSamples.ToString
            values("opt.thresholdMode") = opt.thresholdMode.ToString
            values("opt.miThreshold") = formatNumber(opt.miThreshold)
            values("opt.permutationPairs") = opt.permutationPairs.ToString
            values("opt.permutationQuantile") = formatNumber(opt.permutationQuantile)
            values("opt.permutationReplicates") = opt.permutationReplicates.ToString
            values("opt.tolerance") = formatNumber(opt.tolerance)
            values("opt.dpiMode") = If(opt.dpiMode.HasValue, opt.dpiMode.Value.ToString, "")
            values("opt.dpiMaxPasses") = opt.dpiMaxPasses.ToString
            values("opt.directionMode") = opt.directionMode.ToString
            values("opt.log2Transform") = If(opt.log2Transform, "1", "0")
            values("opt.standardize") = If(opt.standardize, "1", "0")
            values("opt.rankTransform") = If(opt.rankTransform, "1", "0")
            values("opt.minNonZeroRatio") = formatNumber(opt.minNonZeroRatio)
            values("opt.maxGenes") = opt.maxGenes.ToString
            values("opt.maxNeighbors") = opt.maxNeighbors.ToString
            values("opt.priorMinConfidence") = formatNumber(opt.priorMinConfidence)
            values("opt.priorThresholdFactor") = formatNumber(opt.priorThresholdFactor)
            values("opt.protectPriorEdges") = If(opt.protectPriorEdges, "1", "0")
            values("opt.priorProtectionMode") = opt.priorProtectionMode.ToString
            values("opt.priorProtectionStrength") = formatNumber(opt.priorProtectionStrength)
            values("opt.priorTfAugment") = If(opt.priorTfAugment, "1", "0")
            values("opt.maxDegreeOfParallelism") = opt.maxDegreeOfParallelism.ToString
            values("opt.randomSeed") = opt.randomSeed.ToString
            values("opt.silent") = If(opt.silent, "1", "0")
            values("opt.minSamples") = opt.minSamples.ToString

            ' ---- 显著性阈值 ----
            Dim tau As MiThreshold = aracne.Threshold

            values("threshold.present") = If(tau Is Nothing, "0", "1")

            If tau IsNot Nothing Then
                values("threshold.mode") = tau.mode.ToString
                values("threshold.value") = formatNumber(tau.value)
                values("threshold.quantile") = formatNumber(tau.quantile)
                values("threshold.nullMean") = formatNumber(tau.nullMean)
                values("threshold.nullSD") = formatNumber(tau.nullSD)
                values("threshold.nullMax") = formatNumber(tau.nullMax)
                values("threshold.samples") = tau.samples.ToString
                values("threshold.replicates") = tau.replicates.ToString
                values("threshold.estimator") = tau.estimator.ToString
            End If

            ' ---- 统计量 ----
            values("stats.genesScanned") = stat.genesScanned.ToString
            values("stats.genes") = stat.genes.ToString
            values("stats.samples") = stat.samples.ToString
            values("stats.transcriptionFactors") = stat.transcriptionFactors.ToString
            values("stats.candidatePairs") = stat.candidatePairs.ToString
            values("stats.edgesAfterThreshold") = stat.edgesAfterThreshold.ToString
            values("stats.edgesAfterDpi") = stat.edgesAfterDpi.ToString
            values("stats.edges") = stat.edges.ToString
            values("stats.priorEdges") = stat.priorEdges.ToString
            values("stats.priorSupportedEdges") = stat.priorSupportedEdges.ToString
            values("stats.priorSupportedPairs") = stat.priorSupportedPairs.ToString
            values("stats.priorProtectedEdges") = stat.priorProtectedEdges.ToString
            values("stats.priorProtection") = stat.priorProtection.ToString
            values("stats.preprocessSeconds") = formatNumber(stat.preprocessSeconds)
            values("stats.miSeconds") = formatNumber(stat.miSeconds)
            values("stats.filterSeconds") = formatNumber(stat.filterSeconds)
            values("stats.totalSeconds") = formatNumber(stat.totalSeconds)

            For Each grade As String In confidenceGrades
                Dim rescuedKey As String = "stats.rescued." & grade
                Dim pairsKey As String = "stats.pairs." & grade

                values(rescuedKey) = gradeValue(stat.priorRescuedByConfidence, grade).ToString
                values(pairsKey) = gradeValue(stat.priorPairsByConfidence, grade).ToString
            Next

            Dim dpi As DpiResult = stat.dpi

            values("dpi.edgesBefore") = If(dpi Is Nothing, 0, dpi.EdgesBefore).ToString
            values("dpi.edgesAfter") = If(dpi Is Nothing, 0, dpi.EdgesAfter).ToString
            values("dpi.passes") = If(dpi Is Nothing, 0, dpi.Passes).ToString
            values("dpi.triangles") = If(dpi Is Nothing, 0, dpi.TrianglesExamined).ToString

            Using stream As Stream = createEntry(zip, "manifest.tsv")
                Using writer As New StreamWriter(stream, Encoding.UTF8)
                    For Each item As KeyValuePair(Of String, String) In values
                        Call writer.WriteLine($"{item.Key}{ControlChars.Tab}{item.Value}")
                    Next
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 读取 manifest.tsv
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <returns></returns>
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

        ''' <summary>
        ''' 由 manifest 重建推断参数
        ''' </summary>
        ''' <param name="values"></param>
        ''' <param name="tfs">转录因子列表</param>
        ''' <param name="prior">先验网络的邻接表</param>
        ''' <returns></returns>
        Private Function readOptions(values As Dictionary(Of String, String),
                                     tfs As String(),
                                     prior As Dictionary(Of Long, RegulatoryEdge)) As AracneOptions

            Dim dpiValue As DpiMode?

            Select Case If(getValue(values, "opt.dpiMode"), "").Trim
                Case ""
                    dpiValue = Nothing
                Case Else
                    dpiValue = getEnum(values, "opt.dpiMode", ARACNE.DpiMode.Standard)
            End Select

            Return New AracneOptions With {
                .miEstimator = getEnum(values, "opt.miEstimator", MiEstimator.GaussianCopula),
                .adaptiveMaxBins = getInt(values, "opt.adaptiveMaxBins", 4),
                .adaptiveMinPoints = getInt(values, "opt.adaptiveMinPoints", 12),
                .histogramBins = getInt(values, "opt.histogramBins", 4),
                .kernelBandwidth = getDouble(values, "opt.kernelBandwidth", 0.0R),
                .kernelMaxSamples = getInt(values, "opt.kernelMaxSamples", 2000),
                .thresholdMode = getEnum(values, "opt.thresholdMode", MiThresholdMode.Permutation),
                .miThreshold = getDouble(values, "opt.miThreshold", 0.0R),
                .permutationPairs = getInt(values, "opt.permutationPairs", 0),
                .permutationQuantile = getDouble(values, "opt.permutationQuantile", 0.95R),
                .permutationReplicates = getInt(values, "opt.permutationReplicates", 1),
                .tolerance = getDouble(values, "opt.tolerance", 0.1R),
                .dpiMode = dpiValue,
                .dpiMaxPasses = getInt(values, "opt.dpiMaxPasses", 5),
                .directionMode = getEnum(values, "opt.directionMode", DirectionMode.TfDirected),
                .transcriptionFactors = tfs,
                .log2Transform = getBool(values, "opt.log2Transform", False),
                .standardize = getBool(values, "opt.standardize", False),
                .rankTransform = getBool(values, "opt.rankTransform", False),
                .minNonZeroRatio = getDouble(values, "opt.minNonZeroRatio", 0.0R),
                .maxGenes = getInt(values, "opt.maxGenes", 0),
                .maxNeighbors = getInt(values, "opt.maxNeighbors", 0),
                .priorNetwork = If(prior Is Nothing OrElse prior.Count = 0, Nothing, prior.Values.ToArray),
                .priorMinConfidence = getDouble(values, "opt.priorMinConfidence", 0.45R),
                .priorThresholdFactor = getDouble(values, "opt.priorThresholdFactor", 0.5R),
                .protectPriorEdges = getBool(values, "opt.protectPriorEdges", True),
                .priorProtectionMode = getEnum(values, "opt.priorProtectionMode", PriorProtectionMode.Graded),
                .priorProtectionStrength = getDouble(values, "opt.priorProtectionStrength", 0.5R),
                .priorTfAugment = getBool(values, "opt.priorTfAugment", True),
                .maxDegreeOfParallelism = getInt(values, "opt.maxDegreeOfParallelism", -1),
                .randomSeed = getInt(values, "opt.randomSeed", 1234567),
                .silent = getBool(values, "opt.silent", True),
                .minSamples = getInt(values, "opt.minSamples", 20)
            }
        End Function

        ''' <summary>
        ''' 由 manifest 重建显著性阈值
        ''' </summary>
        ''' <param name="values"></param>
        ''' <returns></returns>
        Private Function readThreshold(values As Dictionary(Of String, String)) As MiThreshold
            If Not getBool(values, "threshold.present", False) Then
                Return Nothing
            End If

            Return New MiThreshold With {
                .mode = getEnum(values, "threshold.mode", MiThresholdMode.Permutation),
                .value = getDouble(values, "threshold.value", 0.0R),
                .quantile = getDouble(values, "threshold.quantile", 0.95R),
                .nullMean = getDouble(values, "threshold.nullMean", 0.0R),
                .nullSD = getDouble(values, "threshold.nullSD", 0.0R),
                .nullMax = getDouble(values, "threshold.nullMax", 0.0R),
                .samples = getInt(values, "threshold.samples", 0),
                .replicates = getInt(values, "threshold.replicates", 1),
                .estimator = getEnum(values, "threshold.estimator", MiEstimator.GaussianCopula)
            }
        End Function

        ''' <summary>
        ''' 由 manifest 重建统计信息
        ''' </summary>
        ''' <param name="values"></param>
        ''' <param name="threshold"></param>
        ''' <param name="dpi"></param>
        ''' <returns></returns>
        Private Function readStatistics(values As Dictionary(Of String, String),
                                        threshold As MiThreshold,
                                        dpi As DpiResult) As AracneStatistics

            Dim rescued As New Dictionary(Of String, Integer)
            Dim pairs As New Dictionary(Of String, Integer)

            For Each grade As String In confidenceGrades
                rescued(grade) = getInt(values, "stats.rescued." & grade, 0)
                pairs(grade) = getInt(values, "stats.pairs." & grade, 0)
            Next

            Return New AracneStatistics With {
                .genesScanned = getInt(values, "stats.genesScanned", 0),
                .genes = getInt(values, "stats.genes", 0),
                .samples = getInt(values, "stats.samples", 0),
                .transcriptionFactors = getInt(values, "stats.transcriptionFactors", 0),
                .candidatePairs = getLong(values, "stats.candidatePairs", 0),
                .threshold = threshold,
                .edgesAfterThreshold = getInt(values, "stats.edgesAfterThreshold", 0),
                .edgesAfterDpi = getInt(values, "stats.edgesAfterDpi", 0),
                .edges = getInt(values, "stats.edges", 0),
                .priorEdges = getInt(values, "stats.priorEdges", 0),
                .priorSupportedEdges = getInt(values, "stats.priorSupportedEdges", 0),
                .priorSupportedPairs = getInt(values, "stats.priorSupportedPairs", 0),
                .priorProtectedEdges = getInt(values, "stats.priorProtectedEdges", 0),
                .priorProtection = getEnum(values, "stats.priorProtection", PriorProtectionMode.Graded),
                .priorRescuedByConfidence = rescued,
                .priorPairsByConfidence = pairs,
                .dpi = dpi,
                .preprocessSeconds = getDouble(values, "stats.preprocessSeconds", 0.0R),
                .miSeconds = getDouble(values, "stats.miSeconds", 0.0R),
                .filterSeconds = getDouble(values, "stats.filterSeconds", 0.0R),
                .totalSeconds = getDouble(values, "stats.totalSeconds", 0.0R)
            }
        End Function

#End Region

#Region "genes / tf / prior"

        ''' <summary>
        ''' 写入基因编号与转录因子标记
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="aracne"></param>
        Private Sub writeGenes(zip As ZipArchive, aracne As AracneNetwork)
            Using stream As Stream = createEntry(zip, "genes.tsv")
                Using writer As New StreamWriter(stream, Encoding.UTF8)
                    Dim flags As Boolean() = aracne.TfFlags

                    For i As Integer = 0 To aracne.Size - 1
                        Call writer.WriteLine($"{aracne.Genes(i)}{ControlChars.Tab}{If(flags(i), "1", "0")}")
                    Next
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 读取基因编号与转录因子标记
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="genes">基因编号</param>
        ''' <param name="tfFlags">转录因子标记</param>
        Private Sub readGenes(zip As ZipArchive, ByRef genes As String(), ByRef tfFlags As Boolean())
            Dim names As New List(Of String)
            Dim flags As New List(Of Boolean)

            Using stream As Stream = openEntry(zip, "genes.tsv")
                If stream Is Nothing Then
                    Throw New InvalidDataException("this ARACNE model archive is damaged: the genes.tsv is missing!")
                End If

                Using reader As New StreamReader(stream, Encoding.UTF8)
                    Dim line As String = reader.ReadLine()

                    While line IsNot Nothing
                        If line.Length > 0 Then
                            Dim fields As String() = line.Split(ControlChars.Tab)

                            Call names.Add(fields(0))
                            Call flags.Add(fields.Length > 1 AndAlso fields(1).Trim = "1")
                        End If

                        line = reader.ReadLine()
                    End While
                End Using
            End Using

            genes = names.ToArray
            tfFlags = flags.ToArray
        End Sub

        ''' <summary>
        ''' 写入转录因子列表
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="aracne"></param>
        Private Sub writeTFs(zip As ZipArchive, aracne As AracneNetwork)
            Using stream As Stream = createEntry(zip, "tfs.txt")
                Using writer As New StreamWriter(stream, Encoding.UTF8)
                    For Each id As String In aracne.TFs
                        Call writer.WriteLine(id)
                    Next
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 读取转录因子列表(为空的时候由基因的 TF 标记恢复)
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="genes"></param>
        ''' <returns></returns>
        Private Function readTFs(zip As ZipArchive, genes As String()) As String()
            Dim result As New List(Of String)

            Using stream As Stream = openEntry(zip, "tfs.txt")
                If stream Is Nothing Then
                    Return Nothing
                End If

                Using reader As New StreamReader(stream, Encoding.UTF8)
                    Dim line As String = reader.ReadLine()

                    While line IsNot Nothing
                        If line.Trim.Length > 0 Then
                            Call result.Add(line.Trim)
                        End If

                        line = reader.ReadLine()
                    End While
                End Using
            End Using

            Return result.ToArray
        End Function

        ''' <summary>
        ''' 写入先验网络
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="aracne"></param>
        Private Sub writePrior(zip As ZipArchive, aracne As AracneNetwork)
            Using stream As Stream = createEntry(zip, "prior.tsv")
                Using writer As New StreamWriter(stream, Encoding.UTF8)
                    For Each evidence As RegulatoryEdge In aracne.Prior.Values
                        Call writer.WriteLine(String.Join(ControlChars.Tab,
                            evidence.TF,
                            evidence.TargetGene,
                            formatNumber(evidence.Confidence),
                            evidence.Sign.ToString))
                    Next
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 读取先验网络，并且映射回基因下标(键值见 <see cref="MiMatrix.EdgeKey(Integer, Integer)"/>)
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="genes"></param>
        ''' <returns></returns>
        Private Function readPrior(zip As ZipArchive, genes As String()) As Dictionary(Of Long, RegulatoryEdge)
            Dim result As New Dictionary(Of Long, RegulatoryEdge)
            Dim index As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To genes.Length - 1
                index(genes(i)) = i
            Next

            Using stream As Stream = openEntry(zip, "prior.tsv")
                If stream Is Nothing Then
                    Return result
                End If

                Using reader As New StreamReader(stream, Encoding.UTF8)
                    Dim line As String = reader.ReadLine()

                    While line IsNot Nothing
                        If line.Length > 0 Then
                            Dim f As String() = line.Split(ControlChars.Tab)

                            If f.Length >= 4 Then
                                Dim u As Integer
                                Dim v As Integer

                                If index.TryGetValue(f(0), u) AndAlso index.TryGetValue(f(1), v) AndAlso u <> v Then
                                    Dim key As Long = ARACNE.MiMatrix.EdgeKey(u, v)
                                    Dim sign As Integer

                                    If Not Integer.TryParse(f(3), sign) Then
                                        sign = 0
                                    End If

                                    result(key) = New RegulatoryEdge(f(0), f(1), parseNumber(f(2), 0.0R), sign)
                                End If
                            End If
                        End If

                        line = reader.ReadLine()
                    End While
                End Using
            End Using

            Return result
        End Function

#End Region

#Region "expression"

        ''' <summary>
        ''' 写入预处理之后的表达向量
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="aracne"></param>
        Private Sub writeExpression(zip As ZipArchive, aracne As AracneNetwork)
            Dim vectors As Double()() = aracne.Vectors
            Dim nSamples As Integer = If(vectors.Length > 0 AndAlso vectors(0) IsNot Nothing, vectors(0).Length, 0)

            Using stream As Stream = createEntry(zip, "expression.bin")
                Using buffered As New BufferedStream(stream, 1 << 20)
                    Using writer As New BinaryWriter(buffered, Encoding.UTF8)
                        writer.Write(vectors.Length)
                        writer.Write(nSamples)

                        For i As Integer = 0 To vectors.Length - 1
                            Dim v As Double() = vectors(i)

                            For j As Integer = 0 To nSamples - 1
                                writer.Write(v(j))
                            Next
                        Next
                    End Using
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 读取预处理之后的表达向量
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="nGenes">期望的基因数量</param>
        ''' <returns></returns>
        Private Function readExpression(zip As ZipArchive, nGenes As Integer) As Double()()
            Using stream As Stream = openEntry(zip, "expression.bin")
                If stream Is Nothing Then
                    Return Nothing
                End If

                Using buffered As New BufferedStream(stream, 1 << 20)
                    Using reader As New BinaryReader(buffered, Encoding.UTF8)
                        Dim n As Integer = reader.ReadInt32()
                        Dim nSamples As Integer = reader.ReadInt32()

                        If n <> nGenes Then
                            Throw New InvalidDataException($"the expression block contains {n} genes, but the genes.tsv contains {nGenes} genes.")
                        End If

                        Dim vectors As Double()() = New Double(n - 1)() {}

                        For i As Integer = 0 To n - 1
                            Dim v As Double() = New Double(nSamples - 1) {}

                            For j As Integer = 0 To nSamples - 1
                                v(j) = reader.ReadDouble()
                            Next

                            vectors(i) = v
                        Next

                        Return vectors
                    End Using
                End Using
            End Using
        End Function

        ''' <summary>
        ''' 写入样本编号
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="aracne"></param>
        Private Sub writeSamples(zip As ZipArchive, aracne As AracneNetwork)
            Dim expr As Matrix = aracne.Expression
            Dim samples As String() = If(expr Is Nothing, Nothing, expr.sampleID)

            If samples Is Nothing Then
                Return
            End If

            Using stream As Stream = createEntry(zip, "samples.txt")
                Using writer As New StreamWriter(stream, Encoding.UTF8)
                    For Each id As String In samples
                        Call writer.WriteLine(id)
                    Next
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 读取样本编号
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <returns></returns>
        Private Function readSamples(zip As ZipArchive) As String()
            Dim result As New List(Of String)

            Using stream As Stream = openEntry(zip, "samples.txt")
                If stream Is Nothing Then
                    Return Nothing
                End If

                Using reader As New StreamReader(stream, Encoding.UTF8)
                    Dim line As String = reader.ReadLine()

                    While line IsNot Nothing
                        Call result.Add(line)
                        line = reader.ReadLine()
                    End While
                End Using
            End Using

            Return result.ToArray
        End Function

        ''' <summary>
        ''' 由表达向量重建表达矩阵对象
        ''' </summary>
        ''' <param name="genes"></param>
        ''' <param name="vectors"></param>
        ''' <param name="samples"></param>
        ''' <returns></returns>
        Private Function buildExpression(genes As String(), vectors As Double()(), samples As String()) As Matrix
            Dim rows As DataFrameRow() = New DataFrameRow(genes.Length - 1) {}

            For i As Integer = 0 To genes.Length - 1
                rows(i) = New DataFrameRow With {
                    .geneID = genes(i),
                    .experiments = vectors(i)
                }
            Next

            Return New Matrix With {
                .tag = "ARACNE[loaded]",
                .sampleID = samples,
                .expression = rows
            }
        End Function

#End Region

#Region "binary blocks"

        ''' <summary>
        ''' 写入互信息矩阵(``int32`` 计数 + ``(u, v, mi)`` 三元组)
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <param name="matrix"></param>
        Private Sub writeMiEdges(stream As Stream, matrix As ARACNE.MiMatrix)
            Using buffered As New BufferedStream(stream, 1 << 20)
                Using writer As New BinaryWriter(buffered, Encoding.UTF8)
                    If matrix Is Nothing Then
                        writer.Write(-1)
                    Else
                        writer.Write(matrix.Count)

                        For Each e As MiEdge In matrix.Edges()
                            writer.Write(e.u)
                            writer.Write(e.v)
                            writer.Write(e.mi)
                        Next
                    End If
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 读取互信息矩阵
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <param name="genes"></param>
        ''' <returns></returns>
        Private Function readMiEdges(stream As Stream, genes As String()) As ARACNE.MiMatrix
            If stream Is Nothing Then
                Return Nothing
            End If

            Using buffered As New BufferedStream(stream, 1 << 20)
                Using reader As New BinaryReader(buffered, Encoding.UTF8)
                    Dim count As Integer = reader.ReadInt32()

                    If count < 0 Then
                        Return Nothing
                    End If

                    Dim matrix As New ARACNE.MiMatrix(genes)

                    For i As Integer = 1 To count
                        Dim u As Integer = reader.ReadInt32()
                        Dim v As Integer = reader.ReadInt32()
                        Dim mi As Double = reader.ReadDouble()

                        Call matrix.SetValue(u, v, mi)
                    Next

                    Return matrix
                End Using
            End Using
        End Function

        ''' <summary>
        ''' 写入边的列表(DPI 剔除/保护的边)
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <param name="edges"></param>
        Private Sub writeEdgeList(stream As Stream, edges As List(Of MiEdge))
            Using buffered As New BufferedStream(stream, 1 << 20)
                Using writer As New BinaryWriter(buffered, Encoding.UTF8)
                    If edges Is Nothing Then
                        writer.Write(-1)
                    Else
                        writer.Write(edges.Count)

                        For Each e As MiEdge In edges
                            writer.Write(e.u)
                            writer.Write(e.v)
                            writer.Write(e.mi)
                        Next
                    End If
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' 读取边的列表
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <returns></returns>
        Private Function readEdgeList(stream As Stream) As List(Of MiEdge)
            Dim result As New List(Of MiEdge)

            If stream Is Nothing Then
                Return result
            End If

            Using buffered As New BufferedStream(stream, 1 << 20)
                Using reader As New BinaryReader(buffered, Encoding.UTF8)
                    Dim count As Integer = reader.ReadInt32()

                    For i As Integer = 1 To count
                        Dim u As Integer = reader.ReadInt32()
                        Dim v As Integer = reader.ReadInt32()
                        Dim mi As Double = reader.ReadDouble()

                        Call result.Add(New MiEdge(u, v, mi))
                    Next
                End Using
            End Using

            Return result
        End Function

#End Region

#Region "edge table"

        ''' <summary>
        ''' 写入调控边表格(逐列与 <see cref="AracneNetwork.WriteEdges"/> 一致)
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <param name="edges"></param>
        Private Sub writeEdgeTable(stream As Stream, edges As IEnumerable(Of AracneEdge))
            Using writer As New StreamWriter(stream, Encoding.UTF8)
                Call writer.WriteLine("regulator,target,mi,correlation,interaction,is_tf_source,is_tf_target,prior,regulation")

                For Each edge As AracneEdge In edges
                    Call writer.WriteLine(String.Join(",",
                        edge.regulator,
                        edge.target,
                        formatNumber(edge.mi),
                        formatNumber(edge.correlation),
                        edge.interaction,
                        If(edge.isTfSource, "1", "0"),
                        If(edge.isTfTarget, "1", "0"),
                        If(edge.prior, "1", "0"),
                        If(edge.regulation, "")))
                Next
            End Using
        End Sub

        ''' <summary>
        ''' 读取调控边表格
        ''' </summary>
        ''' <param name="stream"></param>
        ''' <returns></returns>
        Private Function readEdgeTable(stream As Stream) As AracneEdge()
            Dim result As New List(Of AracneEdge)

            If stream Is Nothing Then
                Return result.ToArray
            End If

            Using reader As New StreamReader(stream, Encoding.UTF8)
                Call reader.ReadLine() ' header

                Dim line As String = reader.ReadLine()

                While line IsNot Nothing
                    If line.Length > 0 Then
                        Dim f As String() = line.Split(","c)

                        If f.Length >= 9 Then
                            Call result.Add(New AracneEdge With {
                                .regulator = f(0),
                                .target = f(1),
                                .mi = parseNumber(f(2), 0.0R),
                                .correlation = parseNumber(f(3), 0.0R),
                                .interaction = f(4),
                                .isTfSource = f(5) = "1",
                                .isTfTarget = f(6) = "1",
                                .prior = f(7) = "1",
                                .regulation = f(8)
                            })
                        End If
                    End If

                    line = reader.ReadLine()
                End While
            End Using

            Return result.ToArray
        End Function

#End Region

#Region "zip / primitives"

        ''' <summary>
        ''' 置信度的分级名称，与 <see cref="AracneNetwork"/> 的统计口径保持一致
        ''' </summary>
        Private ReadOnly confidenceGrades As String() = {"high", "medium", "low"}

        ''' <summary>
        ''' 创建一个新的压缩包条目，并返回它的写入流
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Private Function createEntry(zip As ZipArchive, name As String) As Stream
            Return zip.CreateEntry(name, CompressionLevel.Optimal).Open()
        End Function

        ''' <summary>
        ''' 打开一个压缩包条目，条目不存在的时候返回<c>Nothing</c>
        ''' </summary>
        ''' <param name="zip"></param>
        ''' <param name="name"></param>
        ''' <returns></returns>
        Private Function openEntry(zip As ZipArchive, name As String) As Stream
            Dim entry As ZipArchiveEntry = zip.GetEntry(name)

            If entry Is Nothing Then
                Return Nothing
            Else
                Return entry.Open()
            End If
        End Function

        ''' <summary>
        ''' 以可逆的方式格式化浮点数
        ''' </summary>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Private Function formatNumber(value As Double) As String
            Return value.ToString("G17", Globalization.CultureInfo.InvariantCulture)
        End Function

        ''' <summary>
        ''' 解析浮点数
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="fallback"></param>
        ''' <returns></returns>
        Private Function parseNumber(text As String, fallback As Double) As Double
            Dim value As Double

            If text IsNot Nothing AndAlso Double.TryParse(text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, value) Then
                Return value
            Else
                Return fallback
            End If
        End Function

        ''' <summary>
        ''' 读取一个字符串值
        ''' </summary>
        ''' <param name="values"></param>
        ''' <param name="key"></param>
        ''' <returns></returns>
        Private Function getValue(values As Dictionary(Of String, String), key As String) As String
            Dim text As String

            If values.TryGetValue(key, text) Then
                Return text
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 读取一个整数值
        ''' </summary>
        ''' <param name="values"></param>
        ''' <param name="key"></param>
        ''' <param name="fallback"></param>
        ''' <returns></returns>
        Private Function getInt(values As Dictionary(Of String, String), key As String, fallback As Integer) As Integer
            Dim value As Integer
            Dim text As String = getValue(values, key)

            If text IsNot Nothing AndAlso Integer.TryParse(text, value) Then
                Return value
            Else
                Return fallback
            End If
        End Function

        ''' <summary>
        ''' 读取一个长整数值
        ''' </summary>
        ''' <param name="values"></param>
        ''' <param name="key"></param>
        ''' <param name="fallback"></param>
        ''' <returns></returns>
        Private Function getLong(values As Dictionary(Of String, String), key As String, fallback As Long) As Long
            Dim value As Long
            Dim text As String = getValue(values, key)

            If text IsNot Nothing AndAlso Long.TryParse(text, value) Then
                Return value
            Else
                Return fallback
            End If
        End Function

        ''' <summary>
        ''' 读取一个浮点数值
        ''' </summary>
        ''' <param name="values"></param>
        ''' <param name="key"></param>
        ''' <param name="fallback"></param>
        ''' <returns></returns>
        Private Function getDouble(values As Dictionary(Of String, String), key As String, fallback As Double) As Double
            Return parseNumber(getValue(values, key), fallback)
        End Function

        ''' <summary>
        ''' 读取一个布尔值(``1``/``true`` 为真)
        ''' </summary>
        ''' <param name="values"></param>
        ''' <param name="key"></param>
        ''' <param name="fallback"></param>
        ''' <returns></returns>
        Private Function getBool(values As Dictionary(Of String, String), key As String, fallback As Boolean) As Boolean
            Dim text As String = getValue(values, key)

            If text Is Nothing Then
                Return fallback
            Else
                Return text = "1" OrElse String.Equals(text, "true", StringComparison.OrdinalIgnoreCase)
            End If
        End Function

        ''' <summary>
        ''' 读取一个枚举值
        ''' </summary>
        ''' <typeparam name="T">枚举类型</typeparam>
        ''' <param name="values"></param>
        ''' <param name="key"></param>
        ''' <param name="fallback"></param>
        ''' <returns></returns>
        Private Function getEnum(Of T As Structure)(values As Dictionary(Of String, String), key As String, fallback As T) As T
            Dim text As String = getValue(values, key)
            Dim value As T

            If text IsNot Nothing AndAlso text.Length > 0 AndAlso [Enum].TryParse(Of T)(text, value) Then
                Return value
            Else
                Return fallback
            End If
        End Function

        ''' <summary>
        ''' 读取分级统计之中的一个取值
        ''' </summary>
        ''' <param name="counts"></param>
        ''' <param name="grade"></param>
        ''' <returns></returns>
        Private Function gradeValue(counts As Dictionary(Of String, Integer), grade As String) As Integer
            Dim value As Integer

            If counts IsNot Nothing AndAlso counts.TryGetValue(grade, value) Then
                Return value
            Else
                Return 0
            End If
        End Function

#End Region

    End Module

End Namespace
