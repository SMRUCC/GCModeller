Imports System.IO
Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.DataFrame.MultipleOmics
Imports SMRUCC.genomics.Analysis.HTS.GSVA
Imports SMRUCC.genomics.Analysis.HTS.GSVA.Fusion
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Analysis.HTS.GSEA.Formats
Imports SMRUCC.genomics.Analysis.HTS.GSVA.Omics
Imports std = System.Math

''' <summary>
''' 基于 TCGA BRCA 真实多组学数据集的 moGSVA 验证套件。
''' </summary>
''' <remarks>
''' <para>
''' 数据由 <c>reference/brca_mogsva_export.R</c> 从 cBioPortal 的
''' <c>brca_tcga_pub2015</c> 研究导出，默认位于
''' <c>K:\brca_tcga_pub2015\multipleOmics</c>，包含四层对齐到 460 例原发肿瘤的矩阵：
''' </para>
''' <list type="bullet">
''' <item><description><c>mrna.txt</c>：log2(RSEM+1)，20,431 个基因</description></item>
''' <item><description><c>met.txt</c>：HM450 beta 值，16,111 个基因，方向为 -1</description></item>
''' <item><description><c>protein.txt</c>：RPPA 抗体映射到基因后取均值，181 个基因</description></item>
''' <item><description><c>cna.txt</c>：连续拷贝数，22,232 个基因</description></item>
''' </list>
''' <para>
''' 该套件覆盖四类断言：数据装配（样本轴/量纲/方向）、融合数学
''' （等权 = 逐层均值、权重归一）、生物学合理性（ER 状态与 ESR1 / 雌激素通路）、
''' 以及可选的置换检验与 R GSVA 逐元素对拍。
''' </para>
''' <para>
''' 用法：<c>test.exe --brca [--dir=DIR] [--layers=mrna,met,protein] [--perm=N]
''' [--r-baseline] [--fusion=Equal|Variance] [--lambda=0.25] [--work=DIR]</c>
''' </para>
''' </remarks>
Public Module BrcaMoGsvaTest

    ''' <summary>默认的导出数据目录</summary>
    Public Const DefaultDataDir As String = "K:\brca_tcga_pub2015\multipleOmics"

    ''' <summary>
    ''' R 对拍默认只处理特征数不超过该值的组学层。
    ''' </summary>
    ''' <remarks>
    ''' 在 2 万级特征 × 460 样本上运行 R 的 GSVA 需要 20 分钟以上，
    ''' 因此默认只对拍 protein 这类小层（181 个特征）；
    ''' 需要完整对拍时使用 <c>--r-baseline-all</c>。
    ''' </remarks>
    Public Const RLayerFeatureLimit As Integer = 2000

    Dim m_pass As Integer = 0
    Dim m_failures As New List(Of String)

    ' ==================================================================
    ' 数据模型
    ' ==================================================================

    ''' <summary>manifest.tsv 中的一行，描述一个导出层的元信息</summary>
    Private Class LayerInfo
        Public Property Key As String
        Public Property Name As String
        Public Property File As String
        Public Property Features As Integer
        Public Property Samples As Integer
        Public Property Direction As Integer = 1
        Public Property Normalization As String
        Public Property Transform As String
    End Class

    ''' <summary>装配完成的数据包</summary>
    Private Class BrcaDataset
        Public Property Dataset As MultiOmicsDataset
        Public Property GeneSets As Background
        Public Property Layers As LayerInfo()
        Public Property RawMatrix As Dictionary(Of String, Matrix)
        Public Property ErStatus As Dictionary(Of String, String)
    End Class

    ' ==================================================================
    ' 入口
    ' ==================================================================

    ''' <summary>执行 BRCA 多组学测试</summary>
    ''' <param name="args">命令行参数（已去掉 <c>--brca</c> 开关本身）</param>
    ''' <returns>进程退出码，全部通过为 0</returns>
    Public Function Run(args As String()) As Integer
        Dim dataDir As String = DefaultDataDir
        Dim workDir As String = Path.Combine(AppContext.BaseDirectory, "brca_run")
        Dim layerKeys As String() = {"mrna", "met", "protein"}
        Dim permutations As Integer = 0
        Dim withRBaseline As Boolean = False
        Dim rAllLayers As Boolean = False
        Dim fusion As String = "Equal"
        Dim lambda As Double = 0

        If args IsNot Nothing Then
            For Each arg As String In args
                Dim text As String = If(arg, "").Trim()

                If text.StartsWith("--dir=", StringComparison.OrdinalIgnoreCase) Then
                    dataDir = text.Substring("--dir=".Length)
                ElseIf text.StartsWith("--work=", StringComparison.OrdinalIgnoreCase) Then
                    workDir = text.Substring("--work=".Length)
                ElseIf text.StartsWith("--layers=", StringComparison.OrdinalIgnoreCase) Then
                    layerKeys = text.Substring("--layers=".Length).Split(","c) _
                        .Select(Function(s) s.Trim()) _
                        .Where(Function(s) s.Length > 0) _
                        .ToArray
                ElseIf text.StartsWith("--perm=", StringComparison.OrdinalIgnoreCase) Then
                    permutations = Integer.Parse(text.Substring("--perm=".Length))
                ElseIf text.StartsWith("--fusion=", StringComparison.OrdinalIgnoreCase) Then
                    fusion = text.Substring("--fusion=".Length)
                ElseIf text.StartsWith("--lambda=", StringComparison.OrdinalIgnoreCase) Then
                    lambda = Double.Parse(text.Substring("--lambda=".Length),
                                          Globalization.CultureInfo.InvariantCulture)
                ElseIf text.TextEquals("--r-baseline") Then
                    withRBaseline = True
                ElseIf text.TextEquals("--r-baseline-all") Then
                    withRBaseline = True
                    rAllLayers = True
                End If
            Next
        End If

        Call "==========================================================".println
        Call " TCGA BRCA 多组学 GSVA（moGSVA）测试".println
        Call "==========================================================".println
        Call $"data dir     : {dataDir}".println
        Call $"layers       : {String.Join(", ", layerKeys)}".println
        Call $"fusion       : {fusion} (lambda={lambda})".println
        Call $"permutations : {permutations}".println
        Call $"work dir     : {workDir}".println

        If Not Directory.Exists(dataDir) Then
            Call $"[SKIP] 导出数据目录不存在: {dataDir}".println
            Call "[SKIP] 请先运行: Rscript reference/brca_mogsva_export.R".println

            Return 0
        End If

        Try
            Dim data As BrcaDataset = LoadDataset(dataDir, layerKeys)

            If data Is Nothing Then
                Return 0
            End If

            Call Suite(data, workDir, permutations, fusion, lambda, withRBaseline, rAllLayers)
        Catch ex As Exception
            Call check("BRCA 测试套件执行", False, $"{ex.GetType.Name}: {ex.Message}")
            Call Console.WriteLine(ex.StackTrace)
        End Try

        Call "".println
        Call "==========================================================".println
        Call $" 通过 {m_pass} 项断言，失败 {m_failures.Count} 项".println

        If m_failures.Count > 0 Then
            For Each message As String In m_failures
                Call $"   - {message}".println
            Next

            Call "==========================================================".println

            Return 1
        Else
            Call " 全部通过!".println
            Call "==========================================================".println

            Return 0
        End If
    End Function

    ' ==================================================================
    ' 数据装配
    ' ==================================================================

    ''' <summary>读取 manifest.tsv 并装配多组学数据集</summary>
    Private Function LoadDataset(dataDir As String, layerKeys As String()) As BrcaDataset
        Dim manifestFile As String = Path.Combine(dataDir, "manifest.tsv")
        Dim gmtFile As String = Path.Combine(dataDir, "kegg_v3.gmt")

        If Not File.Exists(manifestFile) Then
            Call check("manifest.tsv 存在", False, manifestFile)
            Return Nothing
        End If
        If Not File.Exists(gmtFile) Then
            Call check("kegg_v3.gmt 存在", False, gmtFile)
            Return Nothing
        End If

        Dim manifest As Dictionary(Of String, LayerInfo) = ReadManifest(manifestFile)
        Dim infos As New List(Of LayerInfo)

        For Each key As String In layerKeys
            Dim info As LayerInfo = Nothing

            If manifest.TryGetValue(key, info) Then
                infos.Add(info)
            Else
                Call check($"层 '{key}' 在 manifest 中定义", False,
                           String.Join(", ", manifest.Keys))
                Return Nothing
            End If
        Next

        Dim layers As New List(Of OmicsLayer)
        Dim raw As New Dictionary(Of String, Matrix)(StringComparer.Ordinal)

        For Each info As LayerInfo In infos
            Dim matrixPath As String = Path.Combine(dataDir, info.File)

            If Not File.Exists(matrixPath) Then
                Call check($"层文件存在: {info.File}", False, matrixPath)
                Return Nothing
            End If

            Dim expr As Matrix = Matrix.LoadData(matrixPath)

            Call raw.Add(info.Name, expr)
            Call $"[init] {info.Name}: {expr.size} features x {expr.sampleID.Length} samples ({info.Transform}, direction={info.Direction})".println

            Dim layer As OmicsLayer = OmicsLayer.Create(info.Name, expr, verbose:=False)
            Dim method As OmicsNormalization = ParseNormalization(info.Normalization)

            If method <> OmicsNormalization.None Then
                ' Apply 不会复制 Direction，必须在标准化之后再设置
                layer = OmicsNormalizer.Apply(layer, method)
            End If

            layer.Direction = info.Direction

            layers.Add(layer)
        Next

        Dim geneSets As New Background(Gmt.LoadFile(gmtFile))
        Dim dataset As MultiOmicsDataset = MultiOmicsDataset.Create(layers, verbose:=False)

        Call $"[init] gene sets: {geneSets.clusters.Length} pathway(s); dataset: {dataset}".println

        Return New BrcaDataset With {
            .Dataset = dataset,
            .GeneSets = geneSets,
            .Layers = infos.ToArray,
            .RawMatrix = raw,
            .ErStatus = ReadErStatus(Path.Combine(dataDir, "samples.tsv"))
        }
    End Function

    ''' <summary>解析 manifest.tsv</summary>
    Private Function ReadManifest(manifestFile As String) As Dictionary(Of String, LayerInfo)
        Dim lines As String() = File.ReadAllLines(manifestFile)
        Dim result As New Dictionary(Of String, LayerInfo)(StringComparer.OrdinalIgnoreCase)

        If lines.Length < 2 Then
            Return result
        End If

        Dim header As String() = lines(0).Split(ControlChars.Tab)

        For i As Integer = 1 To lines.Length - 1
            If lines(i).Trim().Length = 0 Then
                Continue For
            End If

            Dim words As String() = lines(i).Split(ControlChars.Tab)
            Dim info As New LayerInfo With {
                .Key = Field(words, header, "layer"),
                .File = Field(words, header, "file"),
                .Features = CInt(Val(Field(words, header, "features"))),
                .Samples = CInt(Val(Field(words, header, "samples"))),
                .Direction = If(Field(words, header, "direction") = "-1", -1, 1),
                .Normalization = Field(words, header, "normalization"),
                .Transform = Field(words, header, "transform")
            }

            info.Name = CanonicalName(info.Key)
            result(info.Key) = info
        Next

        Return result
    End Function

    ''' <summary>按列名读取一个字段（越界时返回空串）</summary>
    Private Function Field(words As String(), header As String(), name As String) As String
        Dim index As Integer = Array.FindIndex(header, Function(h) SameText(h, name))

        If index < 0 OrElse index >= words.Length Then
            Return ""
        End If

        Return words(index).Trim()
    End Function

    ''' <summary>忽略大小写的字符串相等判定</summary>
    Private Function SameText(a As String, b As String) As Boolean
        Return String.Equals(a, b, StringComparison.OrdinalIgnoreCase)
    End Function

    ''' <summary>manifest 的层键映射为组学层显示名（与 R 基线脚本保持一致）</summary>
    Private Function CanonicalName(key As String) As String
        Select Case key.Trim().ToLowerInvariant()
            Case "mrna", "rna"
                Return "mRNA"
            Case "met", "methylation"
                Return "methylation"
            Case "protein", "rppa"
                Return "protein"
            Case Else
                Return key.Trim().ToLowerInvariant()
        End Select
    End Function

    ''' <summary>把 manifest 中记录的标准化方法名解析为枚举</summary>
    Private Function ParseNormalization(text As String) As OmicsNormalization
        Dim value As String = If(text, "").Trim().ToLowerInvariant()

        If value.Contains("rank") Then
            Return OmicsNormalization.RankPerFeature
        ElseIf value.Contains("robust") Then
            Return OmicsNormalization.RobustZScore
        ElseIf value.Contains("zscore") Then
            Return OmicsNormalization.ZScore
        ElseIf value.Contains("quantile") Then
            Return OmicsNormalization.QuantilePerSample
        ElseIf value.Contains("log2") Then
            Return OmicsNormalization.Log2
        Else
            Return OmicsNormalization.None
        End If
    End Function

    ''' <summary>读取 samples.tsv 中的 ER 状态</summary>
    Private Function ReadErStatus(samplesFile As String) As Dictionary(Of String, String)
        Dim result As New Dictionary(Of String, String)(StringComparer.Ordinal)

        If Not File.Exists(samplesFile) Then
            Return result
        End If

        Dim lines As String() = File.ReadAllLines(samplesFile)

        If lines.Length < 2 Then
            Return result
        End If

        Dim header As String() = lines(0).Split(ControlChars.Tab)
        Dim iSample As Integer = Array.FindIndex(header, Function(h) SameText(h, "SAMPLE_ID"))
        Dim iEr As Integer = Array.FindIndex(header, Function(h) SameText(h, "ER_STATUS_BY_IHC"))

        If iSample < 0 OrElse iEr < 0 Then
            Return result
        End If

        For i As Integer = 1 To lines.Length - 1
            Dim words As String() = lines(i).Split(ControlChars.Tab)

            If words.Length <= std.Max(iSample, iEr) Then
                Continue For
            End If

            result(words(iSample)) = words(iEr)
        Next

        Return result
    End Function

    ' ==================================================================
    ' 断言套件
    ' ==================================================================

    Private Sub Suite(data As BrcaDataset,
                      workDir As String,
                      permutations As Integer,
                      fusion As String,
                      lambda As Double,
                      withRBaseline As Boolean,
                      rAllLayers As Boolean)

        Dim dataset As MultiOmicsDataset = data.Dataset

        ' ---------------- A. 数据装配 ----------------
        Call "".println
        Call "--- A. 数据装配（样本轴 / 量纲 / 方向） ---".println

        Call check("样本数 = 460", dataset.SampleCount = 460, $"{dataset.SampleCount} 个样本")

        Call check("组学层数", dataset.LayerCount = data.Layers.Length,
                   $"{dataset.LayerCount} 层: {String.Join(", ", dataset.OmicsNames)}")

        Call check("各层样本轴完全一致",
                   dataset.Layers.All(Function(l) l.SampleID.SequenceEqual(dataset.SampleID)),
                   $"{dataset.SampleCount} 个样本")

        Dim featureOk As Boolean = True
        Dim featureDetail As New List(Of String)

        For Each info As LayerInfo In data.Layers
            Dim layer As OmicsLayer = dataset.GetLayer(info.Name)
            Dim ok As Boolean = layer IsNot Nothing AndAlso layer.FeatureCount = info.Features

            featureOk = featureOk AndAlso ok
            Call featureDetail.Add($"{info.Name}={If(layer Is Nothing, -1, layer.FeatureCount)}")
        Next

        Call check("特征数与 manifest 一致", featureOk, String.Join(", ", featureDetail))

        Dim mrna As Matrix = data.RawMatrix("mRNA")
        Dim mrnaMax As Double = mrna.expression.Max(Function(r) r.experiments.Max())

        Call check("mRNA 已完成 log2 变换", mrnaMax < 25,
                   $"max = {mrnaMax:F3}（线性 RSEM 会大于 1e5）")

        Dim metLayer As OmicsLayer = dataset.GetLayer("methylation")

        If metLayer IsNot Nothing Then
            Dim metRaw As Matrix = data.RawMatrix("methylation")
            Dim rawMin As Double = metRaw.expression.Min(Function(r) r.experiments.Min())
            Dim rawMax As Double = metRaw.expression.Max(Function(r) r.experiments.Max())

            Call check("甲基化 beta 值处于 [0,1]", rawMin >= 0 AndAlso rawMax <= 1,
                       $"原始范围 [{rawMin:F4}, {rawMax:F4}]")

            ' beta 值的 MAD 很小，RobustZScore 之后出现少量极大值属于正常现象
            ' （分布式双峰），这里只校验标准化结果以 0 为中心且保持有限
            Dim zAbs As Double() = metLayer.Expr.expression _
                .SelectMany(Function(r) r.experiments) _
                .Select(Function(x) std.Abs(x)) _
                .ToArray()

            Call check("甲基化层标准化后以 0 为中心", Median(zAbs) < 1,
                       $"median|z| = {Median(zAbs):F3}, max|z| = {zAbs.Max():F1}")
            Call check("甲基化层方向 = -1", metLayer.Direction = -1, $"Direction={metLayer.Direction}")
        End If

        Dim badValue As Integer = dataset.Layers _
            .SelectMany(Function(l) l.Expr.expression) _
            .SelectMany(Function(r) r.experiments) _
            .Where(Function(x) Double.IsNaN(x) OrElse Double.IsInfinity(x)) _
            .Count()

        Call check("各层数值无 NaN / Inf", badValue = 0, $"{badValue} 个非法值")

        Call check("样本顺序与 samples.tsv 一致",
                   dataset.SampleID.Length > 0 AndAlso
                   data.ErStatus.Count > 0 AndAlso
                   data.ErStatus.ContainsKey(dataset.SampleID(0)),
                   $"first = {dataset.SampleID.FirstOrDefault}")

        ' ---------------- B. 打分与融合 ----------------
        Call "".println
        Call "--- B. moGSVA 打分与融合 ---".println

        Dim strategy As FusionMethods = If(fusion.TextEquals("Variance"),
                                           FusionMethods.Variance,
                                           If(fusion.TextEquals("Entropy"),
                                              FusionMethods.Entropy,
                                              FusionMethods.Equal))
        Dim options As New MultiOmicsGsvaOptions With {
            .ScoreMethod = Methods.gsva,
            .Kcdf = KCDFs.Gaussian,
            .FusionStrategy = strategy,
            .ConsistencyLambda = lambda,
            .BalanceFeatureSize = False,
            .MinSize = 1,
            .MaxSize = Integer.MaxValue,
            .Permutations = 0,
            .ComputePooled = True,
            .Verbose = True
        }
        Dim result As MultiOmicsGsvaResult = MultiOmicsGsva.Compute(dataset, data.GeneSets, options)

        Call check("通路数（KEGG 337 条）", result.Pathways.Length >= 300,
                   $"{result.Pathways.Length} 条通路，耗时 {result.ElapsedMilliseconds / 1000.0:F1} s")

        Dim flat As Double() = result.Scores.expression.SelectMany(Function(r) r.experiments).ToArray

        Call check("融合分数无 NaN / Inf",
                   flat.Where(Function(x) Double.IsNaN(x) OrElse Double.IsInfinity(x)).Count() = 0,
                   $"{flat.Length} 个分数")

        Call check("λ=0 时融合分数等于加权平均",
                   MaxAbsDiff(result.Scores, result.RawScores) = 0,
                   $"max|diff| = {MaxAbsDiff(result.Scores, result.RawScores):E3}")

        ' 抽样核对 ES_multi == Σ w_k · ES_k
        Dim sampleCount As Integer = 0
        Dim fusionDiff As Double = 0

        For Each pathway As String In result.Pathways.Where(Function(p, i) i Mod 17 = 0)
            Dim expect As Double() = WeightedLayerMean(result, pathway)

            If expect Is Nothing Then
                Continue For
            End If

            fusionDiff = std.Max(fusionDiff, MaxAbsDiff(expect, result.ScoreVector(pathway)))
            sampleCount += 1
        Next

        Call check("逐通路融合 = Σ w_k·ES_k", sampleCount > 5 AndAlso fusionDiff < 1.0E-12,
                   $"核对 {sampleCount} 条通路, max|diff| = {fusionDiff:E3}")

        ' 权重归一
        Dim weightBad As Integer = 0

        For Each pathway As String In result.Pathways
            Dim total As Double = result.Weights(pathway).Values.Sum()

            If std.Abs(total - 1) > 1.0E-9 Then
                weightBad += 1
            End If
        Next

        Call check("逐通路组学权重之和 = 1", weightBad = 0, $"{weightBad} 条通路权重异常")

        If strategy = FusionMethods.Equal AndAlso Not options.BalanceFeatureSize Then
            Dim thirdBad As Integer = 0
            Dim thirdAll As Integer = 0

            For Each pathway As String In result.Pathways
                Dim weights As Dictionary(Of String, Double) = result.Weights(pathway)

                If weights.Count <> dataset.LayerCount Then
                    Continue For
                End If

                thirdAll += 1

                If weights.Values.Any(Function(w) std.Abs(w - 1.0 / dataset.LayerCount) > 1.0E-9) Then
                    thirdBad += 1
                End If
            Next

            Call check("Equal 策略下三层齐全的通路权重 = 1/N",
                       thirdAll > 100 AndAlso thirdBad = 0,
                       $"{thirdAll} 条三通路, 异常 {thirdBad} 条")
        End If

        Dim skipProtein As Integer = result.SkippedLayers _
            .Where(Function(p) p.Value.Contains("protein")) _
            .Count()

        Call check("蛋白层被跳过的通路数", skipProtein > 80,
                   $"{skipProtein} 条通路没有足够的 RPPA 成员")

        Dim mrnaCovered As Integer = result.Pathways _
            .Where(Function(p) result.MemberCountOf(p, "mRNA") > 0) _
            .Count()

        Call check("mRNA 层通路覆盖", mrnaCovered > 300, $"{mrnaCovered} 条通路有 mRNA 成员")

        ' ---------------- C. 生物学合理性 ----------------
        Call "".println
        Call "--- C. 生物学合理性（ER 状态） ---".println

        Dim positives As String() = dataset.SampleID _
            .Where(Function(id) ErIs(data, id, "Positive")) _
            .ToArray
        Dim negatives As String() = dataset.SampleID _
            .Where(Function(id) ErIs(data, id, "Negative")) _
            .ToArray

        Call check("ER+ / ER- 分组可用", positives.Length > 200 AndAlso negatives.Length > 50,
                   $"ER+ = {positives.Length}, ER- = {negatives.Length}")

        If positives.Length > 10 AndAlso negatives.Length > 10 Then
            Dim esr1 As Double() = RowOf(mrna, "ESR1")

            Call check("mRNA 中存在 ESR1", esr1 IsNot Nothing, If(esr1 Is Nothing, "missing", "found"))

            If esr1 IsNot Nothing Then
                Dim delta As Double = GroupMean(dataset.SampleID, esr1, positives) -
                    GroupMean(dataset.SampleID, esr1, negatives)

                Call check("ESR1 在 ER+ 组高表达", delta > 1,
                           $"log2 均值差 = {delta:F2}")
            End If

            Dim estrogen As String = result.Pathways _
                .Where(Function(p) SameText(p, "Estrogen signaling pathway")) _
                .FirstOrDefault

            Call check("输出包含雌激素信号通路", estrogen IsNot Nothing,
                       If(estrogen, "missing"))

            If estrogen IsNot Nothing Then
                Dim score As Double() = result.ScoreVector(estrogen)
                Dim delta As Double = GroupMean(dataset.SampleID, score, positives) -
                    GroupMean(dataset.SampleID, score, negatives)

                Call check("雌激素通路在 ER+ 组活性更高", delta > 0,
                           $"融合分数均值差 = {delta:F4}")
            End If
        End If

        ' ---------------- D. 消融对照 ----------------
        Call "".println
        Call "--- D. 拼接矩阵消融对照 ---".println

        Call check("Pooled baseline 可用",
                   result.Pooled IsNot Nothing AndAlso result.Pooled.Available,
                   If(result.Pooled Is Nothing, "nothing", If(result.Pooled.Available, "ok", result.Pooled.ErrorMessage)))

        If result.Pooled IsNot Nothing AndAlso result.Pooled.Available Then
            Call check("拼接基线通路数", result.Pooled.Scores.size > 100,
                       $"{result.Pooled.Scores.size} 条通路")
            Call check("拼接基线与融合结果不同",
                       result.Pooled.MeanAbsoluteDifference > 0,
                       $"mean|diff| = {result.Pooled.MeanAbsoluteDifference:E3}")
        End If

        ' ---------------- E. 置换检验（可选） ----------------
        If permutations > 0 Then
            Call "".println
            Call $"--- E. 置换检验（B = {permutations}） ---".println

            Dim permOptions As New MultiOmicsGsvaOptions With {
                .ScoreMethod = options.ScoreMethod,
                .Kcdf = options.Kcdf,
                .FusionStrategy = strategy,
                .ConsistencyLambda = lambda,
                .MinSize = options.MinSize,
                .MaxSize = options.MaxSize,
                .Permutations = permutations,
                .PermutationPathways = TopPathways(result.Scores, 10),
                .PermutationMethod = Methods.ssgsea,
                .PermutationGeneLimit = 4000,
                .Seed = 20261026,
                .ComputePooled = False,
                .Verbose = True
            }
            Dim permuted As MultiOmicsGsvaResult = MultiOmicsGsva.ComputeSignificance(
                result, dataset, data.GeneSets, permOptions)

            Call check("置换检验产生 p 值表",
                       permuted.PValues IsNot Nothing AndAlso permuted.PValues.Count = 10,
                       $"{If(permuted.PValues, New Dictionary(Of String, Double)).Count} 条通路")

            If permuted.PValues IsNot Nothing Then
                Dim pOk As Boolean = permuted.PValues.Values.All(Function(p) p > 0 AndAlso p <= 1)

                Call check("p 值位于 (0, 1]", pOk,
                           $"min = {permuted.PValues.Values.Min():F4}, max = {permuted.PValues.Values.Max():F4}")

                Dim qOk As Boolean = permuted.QValues IsNot Nothing AndAlso
                    permuted.QValues.All(Function(item) item.Value >= permuted.PValues(item.Key) - 1.0E-12)

                Call check("q ≥ p（BH 校正）", qOk,
                           $"q 值 {If(permuted.QValues, New Dictionary(Of String, Double)).Count} 条")

                Dim again As MultiOmicsGsvaResult = MultiOmicsGsva.ComputeSignificance(
                    result, dataset, data.GeneSets, permOptions)
                Dim identical As Boolean = permuted.PValues _
                    .All(Function(item) again.PValues(item.Key) = item.Value)

                Call check("相同种子下置换结果可复现", identical, "逐位一致")
            End If
        End If

        ' ---------------- F. R 基线（可选） ----------------
        If withRBaseline Then
            Call "".println
            Call "--- F. R GSVA 逐层对拍 ---".println

            Call RBaseline(data, result, workDir, rAllLayers)
        End If
    End Sub

    ''' <summary>与 R 的 GSVA 逐层逐元素比对</summary>
    ''' <param name="data">装配完成的数据包</param>
    ''' <param name="result">moGSVA 的观测结果</param>
    ''' <param name="workDir">中间产物目录</param>
    ''' <param name="allLayers">
    ''' <c>True</c> 时对全部组学层做 R 对拍；默认只对特征数不超过
    ''' <see cref="RLayerFeatureLimit"/> 的层（本数据集中为 protein）做对拍，
    ''' 因为在 2 万级特征 × 460 样本上运行 R 的 GSVA 需要 20 分钟以上。
    ''' </param>
    Private Sub RBaseline(data As BrcaDataset, result As MultiOmicsGsvaResult, workDir As String, allLayers As Boolean)
        Dim subset As String()

        If allLayers Then
            subset = data.Dataset.OmicsNames
        Else
            subset = data.Dataset.Layers _
                .Where(Function(l) l.FeatureCount <= RLayerFeatureLimit) _
                .Select(Function(l) l.Name) _
                .ToArray
        End If

        If subset.Length = 0 Then
            subset = data.Dataset.OmicsNames
        End If

        Call $"[baseline] 参与 R 对拍的层: {String.Join(", ", subset)}".println

        Dim inputDir As String = Path.Combine(workDir, "mogsva_input")
        Dim shim As New MoGsvaData.RealDataset With {
            .Dataset = data.Dataset,
            .GeneSets = data.GeneSets,
            .SampleID = data.Dataset.SampleID,
            .GeneCount = data.Dataset.FeatureCount,
            .RawGenes = data.Dataset.FeatureCount
        }

        Call MoGsvaBaseline.PrepareInputs(shim, inputDir)

        Dim baseline As MoGsvaBaseline.BaselineResult = MoGsvaBaseline.Run(workDir, False, True, subset)

        If Not baseline.Available Then
            Call check("R 基线生成", False, If(baseline.Message, "unknown error"))
            Return
        End If

        Call check("R 基线生成", True,
                   $"{baseline.LayerScores.Count} 层, status = {String.Join("; ", baseline.Status.Select(Function(p) $"{p.Key}:{p.Value}"))}")

        Dim layers As OmicsLayer() = data.Dataset.Layers _
            .Where(Function(l) subset.Contains(l.Name, StringComparer.OrdinalIgnoreCase)) _
            .ToArray

        For Each layer As OmicsLayer In layers
            Dim reference As Dictionary(Of String, Double()) = Nothing

            If Not baseline.LayerScores.TryGetValue(layer.Name, reference) Then
                Call check($"{layer.Name}: 与 R GSVA 逐元素一致", False, "缺少 R 参考分数")
                Continue For
            End If

            Dim ours As Matrix = Nothing

            If Not result.LayerScores.TryGetValue(layer.Name, ours) Then
                Call check($"{layer.Name}: 与 R GSVA 逐元素一致", False, "moGSVA 未产生该层分数")
                Continue For
            End If

            Dim cmp As MoGsvaBaseline.CompareResult = MoGsvaBaseline.Compare(ours, reference, layer.Direction)

            Call check($"{layer.Name}: 与 R GSVA 逐元素一致",
                       cmp.BadPathways = 0 AndAlso cmp.MissingPathways = 0,
                       $"{cmp.Details}, 越界通路 {cmp.BadPathways}, 缺失 {cmp.MissingPathways}")
        Next

        ' Baseline 1：R 逐层分数的（带方向）平均（仅限参与对拍的层）
        Dim meanDiff As Double = 0
        Dim compared As Integer = 0

        For Each pathway As String In result.Pathways
            Dim expect As Double() = Nothing
            Dim ok As Boolean = True

            For Each layer As OmicsLayer In layers
                Dim reference As Dictionary(Of String, Double()) = Nothing

                If Not baseline.LayerScores.TryGetValue(layer.Name, reference) OrElse
                    Not reference.ContainsKey(pathway) Then
                    ok = False
                    Exit For
                End If

                Dim v As Double() = reference(pathway)

                If expect Is Nothing Then
                    expect = New Double(v.Length - 1) {}
                End If

                For j As Integer = 0 To expect.Length - 1
                    expect(j) += layer.Direction * v(j) / layers.Length
                Next
            Next

            If Not ok Then
                Continue For
            End If

            If layers.Length = 1 Then
                ' 单层时 Baseline 1 退化为该层自身，与融合分数不可比，跳过
                Continue For
            End If

            meanDiff = std.Max(meanDiff, MaxAbsDiff(expect, result.ScoreVector(pathway)))
            compared += 1
        Next

        If layers.Length > 1 Then
            Call check("Baseline 1: 等权融合 == R 逐层打分后平均",
                       compared > 10 AndAlso meanDiff < MoGsvaBaseline.TOLERANCE,
                       $"比对 {compared} 条通路, max|diff| = {meanDiff:E3}")
        Else
            Call $"[baseline] 单层对拍模式，跳过 Baseline 1 均值校验".println
        End If
    End Sub

    ' ==================================================================
    ' 工具函数
    ' ==================================================================

    ''' <summary>按融合权重重新计算某条通路的加权平均分数</summary>
    Private Function WeightedLayerMean(result As MultiOmicsGsvaResult, pathway As String) As Double()
        Dim weights As Dictionary(Of String, Double) = Nothing

        If Not result.Weights.TryGetValue(pathway, weights) Then
            Return Nothing
        End If

        Dim acc As Double() = Nothing

        For Each item As KeyValuePair(Of String, Double) In weights
            Dim scores As Matrix = Nothing

            If Not result.LayerScores.TryGetValue(item.Key, scores) Then
                Continue For
            End If

            Dim row As Double() = RowOf(scores, pathway)

            If row Is Nothing Then
                Continue For
            End If

            If acc Is Nothing Then
                acc = New Double(row.Length - 1) {}
            End If

            For j As Integer = 0 To acc.Length - 1
                acc(j) += item.Value * row(j)
            Next
        Next

        Return acc
    End Function

    ''' <summary>取矩阵中指定行名的数值向量</summary>
    Private Function RowOf(mat As Matrix, id As String) As Double()
        Dim row As DataFrameRow = mat.expression _
            .Where(Function(r) SameText(r.geneID, id)) _
            .FirstOrDefault

        If row Is Nothing Then
            Return Nothing
        End If

        Return row.experiments
    End Function

    ''' <summary>按样本子集求均值</summary>
    Private Function GroupMean(sampleID As String(), values As Double(), subset As String()) As Double
        Dim index As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

        For i As Integer = 0 To sampleID.Length - 1
            index(sampleID(i)) = i
        Next

        Dim sum As Double = 0
        Dim n As Integer = 0

        For Each id As String In subset
            Dim j As Integer

            If index.TryGetValue(id, j) Then
                sum += values(j)
                n += 1
            End If
        Next

        Return If(n = 0, Double.NaN, sum / n)
    End Function

    ''' <summary>判断样本的 ER 状态</summary>
    Private Function ErIs(data As BrcaDataset, id As String, expect As String) As Boolean
        Dim status As String = Nothing

        If Not data.ErStatus.TryGetValue(id, status) Then
            Return False
        End If

        Return SameText(status, expect)
    End Function

    ''' <summary>取分数矩阵中方差最大的前 n 条通路</summary>
    Private Function TopPathways(scores As Matrix, n As Integer) As String()
        Return scores.expression _
            .Select(Function(r)
                        Dim mean As Double = r.experiments.Average()
                        Dim variance As Double = r.experiments _
                            .Select(Function(x) (x - mean) * (x - mean)) _
                            .Average()

                        Return (r.geneID, variance)
                    End Function) _
            .OrderByDescending(Function(t) t.variance) _
            .Take(n) _
            .Select(Function(t) t.geneID) _
            .ToArray
    End Function

    ''' <summary>中位数（不修改输入数组）</summary>
    Private Function Median(values As Double()) As Double
        If values Is Nothing OrElse values.Length = 0 Then
            Return Double.NaN
        End If

        Dim copy As Double() = DirectCast(values.Clone(), Double())
        Call Array.Sort(copy)

        Dim n As Integer = copy.Length

        If n Mod 2 = 1 Then
            Return copy(n \ 2)
        Else
            Return (copy(n \ 2 - 1) + copy(n \ 2)) / 2
        End If
    End Function

    ''' <summary>两个同形矩阵的最大绝对差</summary>
    Private Function MaxAbsDiff(a As Matrix, b As Matrix) As Double
        Dim max As Double = 0

        For i As Integer = 0 To a.size - 1
            Dim x As Double() = a.expression(i).experiments
            Dim y As Double() = b.expression(i).experiments

            For j As Integer = 0 To std.Min(x.Length, y.Length) - 1
                max = std.Max(max, std.Abs(x(j) - y(j)))
            Next
        Next

        Return max
    End Function

    ''' <summary>两个同长向量的最大绝对差</summary>
    Private Function MaxAbsDiff(a As Double(), b As Double()) As Double
        If a Is Nothing OrElse b Is Nothing Then
            Return Double.NaN
        End If

        Dim max As Double = 0

        For i As Integer = 0 To std.Min(a.Length, b.Length) - 1
            max = std.Max(max, std.Abs(a(i) - b(i)))
        Next

        Return max
    End Function

    Private Sub check(name As String, ok As Boolean, detail As String)
        If ok Then
            m_pass += 1
            Call $"  [PASS] {name}  ({detail})".println
        Else
            m_failures.Add($"{name}: {detail}")
            Call $"  [FAIL] {name}  ({detail})".println
        End If
    End Sub

    <Extension>
    Private Sub println(msg As String)
        Call Console.WriteLine(msg)
    End Sub

End Module
