Imports System.Globalization
Imports System.IO
Imports System.Text.RegularExpressions
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports EnrichBackground = SMRUCC.genomics.Analysis.HTS.GSEA.Background
Imports EnrichCluster = SMRUCC.genomics.Analysis.HTS.GSEA.Cluster
Imports EnrichGene = SMRUCC.genomics.Analysis.HTS.GSEA.BackgroundGene

Namespace Demo

    ''' <summary>
    ''' DEMO 数据概况统计（用于控制台汇报与断言）。
    ''' </summary>
    Public Class DemoDataSummary

        ''' <summary>基因数量</summary>
        Public Property NGenes As Integer

        ''' <summary>样本数量</summary>
        Public Property NSamples As Integer

        ''' <summary>不同时间点数量</summary>
        Public Property NTimePoints As Integer

        ''' <summary>每个时间点的生物学重复数（取最小值）</summary>
        Public Property Replicates As Integer

        ''' <summary>先验调控边数量</summary>
        Public Property NEdges As Integer

        ''' <summary>先验网络中的 TF 数量</summary>
        Public Property NTF As Integer

        ''' <summary>通路数量</summary>
        Public Property NPathways As Integer

        ''' <summary>被至少一条通路覆盖的基因数量</summary>
        Public Property NAnnotatedGenes As Integer

        Public Overrides Function ToString() As String
            Return $"{NGenes} 基因 × {NSamples} 样本（{NTimePoints} 时间点 × {Replicates} 重复）；" &
                   $"先验 {NEdges} 条边 / {NTF} 个 TF；{NPathways} 条通路覆盖 {NAnnotatedGenes} 个基因"
        End Function

    End Class

    ''' <summary>
    ''' DEMO 数据加载器：把 TestData1 的 CSV/JSON 转换为 GPS 与富集分析所需的模型对象。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 目录探测顺序：环境变量 <c>GPS_DEMO_DATA</c> → 从 <see cref="AppContext.BaseDirectory"/>
    ''' 逐级向上查找 <c>demo/TestData1/gene_expression_matrix.csv</c> → 当前工作目录。
    ''' 测试工程的可执行文件位于 <c>bin/...</c> 下，直接使用相对路径不可靠，因此必须向上探测。
    ''' </para>
    ''' <para>
    ''' 列名约定：<c>T{小时}h_Rep{重复号}</c>，例如 <c>T1.2h_Rep3</c>。
    ''' 解析失败直接抛异常而不是静默赋 0——时间点错误会让滑窗与切分全部失真。
    ''' </para>
    ''' </remarks>
    Public Module DemoDataLoader

        ''' <summary>表达矩阵文件名</summary>
        Public Const ExpressionFile As String = "gene_expression_matrix.csv"

        ''' <summary>先验网络文件名</summary>
        Public Const PriorFile As String = "regulatory_network_prior.csv"

        ''' <summary>通路定义文件名</summary>
        Public Const PathwayFile As String = "pathway_info.json"

        Private ReadOnly SampleNamePattern As New Regex("^T(?<hour>[0-9]+(?:\.[0-9]+)?)h_Rep(?<rep>[0-9]+)$",
                                                        RegexOptions.IgnoreCase Or RegexOptions.Compiled)

        ''' <summary>
        ''' 定位 TestData1 数据目录。
        ''' </summary>
        ''' <returns>包含三个数据文件的目录绝对路径</returns>
        ''' <exception cref="DirectoryNotFoundException">所有候选路径都不包含数据文件时抛出</exception>
        Public Function ResolveDataDirectory() As String
            Dim fromEnvironment As String = Environment.GetEnvironmentVariable("GPS_DEMO_DATA")

            If Not String.IsNullOrWhiteSpace(fromEnvironment) Then
                If IsDataDirectory(fromEnvironment) Then
                    Return Path.GetFullPath(fromEnvironment)
                End If

                Throw New DirectoryNotFoundException(
                    $"环境变量 GPS_DEMO_DATA 指向的目录不包含 {ExpressionFile}：{fromEnvironment}")
            End If

            ' ---- 从可执行文件目录逐级向上探测 ----
            Dim probe As String = AppContext.BaseDirectory

            For level As Integer = 0 To 8
                If String.IsNullOrEmpty(probe) Then Exit For

                Dim candidate As String = Path.Combine(probe, "demo", "TestData1")

                If IsDataDirectory(candidate) Then
                    Return Path.GetFullPath(candidate)
                End If

                Dim parent As DirectoryInfo = Directory.GetParent(probe)

                If parent Is Nothing Then Exit For

                probe = parent.FullName
            Next

            ' ---- 回退到当前工作目录 ----
            Dim current As String = Path.Combine(Environment.CurrentDirectory, "demo", "TestData1")

            If IsDataDirectory(current) Then
                Return Path.GetFullPath(current)
            End If

            Dim subSystem As String = Path.Combine(Environment.CurrentDirectory, "sub-system", "demo", "TestData1")

            If IsDataDirectory(subSystem) Then
                Return Path.GetFullPath(subSystem)
            End If

            Throw New DirectoryNotFoundException(
                $"找不到 DEMO 数据目录。请设置环境变量 GPS_DEMO_DATA，或在命令行用 --data 指定；" &
                $"期望目录中包含 {ExpressionFile}、{PriorFile}、{PathwayFile}")
        End Function

        ''' <summary>判断目录是否包含全部三个数据文件。</summary>
        ''' <param name="directory">候选目录</param>
        ''' <returns>三个文件都存在时返回 True</returns>
        Public Function IsDataDirectory(directory As String) As Boolean
            If String.IsNullOrWhiteSpace(directory) OrElse Not Directory.Exists(directory) Then
                Return False
            End If

            Return File.Exists(Path.Combine(directory, ExpressionFile)) AndAlso
                   File.Exists(Path.Combine(directory, PriorFile)) AndAlso
                   File.Exists(Path.Combine(directory, PathwayFile))
        End Function

        ''' <summary>
        ''' 加载时序表达矩阵。
        ''' </summary>
        ''' <param name="file">CSV 路径（首列基因名，其余列 <c>T{h}h_Rep{k}</c>）</param>
        ''' <returns>基因×样本的表达矩阵对象</returns>
        Public Function LoadExpressionMatrix(file As String) As GeneExpressionData
            If Not File.Exists(file) Then
                Throw New FileNotFoundException($"表达矩阵文件不存在：{file}", file)
            End If

            Dim lines As String() = File.ReadAllLines(file)

            If lines.Length < 2 Then
                Throw New InvalidDataException($"表达矩阵至少需要表头与一行数据：{file}")
            End If

            Dim header As String() = SplitCsv(lines(0))
            Dim nSamples As Integer = header.Length - 1

            If nSamples <= 0 Then
                Throw New InvalidDataException("表达矩阵表头缺少样本列")
            End If

            Dim sampleNames As String() = New String(nSamples - 1) {}
            Dim timePoints As Double() = New Double(nSamples - 1) {}
            Dim replicates As Integer() = New Integer(nSamples - 1) {}

            For i As Integer = 0 To nSamples - 1
                Dim name As String = header(i + 1).Trim()
                Dim match As Match = SampleNamePattern.Match(name)

                If Not match.Success Then
                    Throw New InvalidDataException(
                        $"无法解析样本列名 '{name}'，期望格式为 T{{小时}}h_Rep{{重复号}}，例如 T1.2h_Rep1")
                End If

                sampleNames(i) = name
                timePoints(i) = Double.Parse(match.Groups("hour").Value, CultureInfo.InvariantCulture)
                replicates(i) = Integer.Parse(match.Groups("rep").Value, CultureInfo.InvariantCulture)
            Next

            Dim nGenes As Integer = lines.Length - 1
            Dim geneNames As String() = New String(nGenes - 1) {}
            Dim matrix As Double(,) = New Double(nGenes - 1, nSamples - 1) {}

            For row As Integer = 1 To lines.Length - 1
                Dim fields As String() = SplitCsv(lines(row))

                If fields.Length <> header.Length Then
                    Throw New InvalidDataException(
                        $"第 {row + 1} 行有 {fields.Length} 列，与表头的 {header.Length} 列不一致")
                End If

                geneNames(row - 1) = fields(0).Trim()

                For col As Integer = 1 To fields.Length - 1
                    matrix(row - 1, col - 1) = Double.Parse(fields(col).Trim(), CultureInfo.InvariantCulture)
                Next
            Next

            Return New GeneExpressionData With {
                .GeneNames = geneNames,
                .SampleNames = sampleNames,
                .Matrix = matrix,
                .TimePoints = timePoints
            }
        End Function

        ''' <summary>
        ''' 加载先验调控网络。
        ''' </summary>
        ''' <param name="file">CSV 路径（TF,TargetGene,RegulationType,Confidence,Evidence）</param>
        ''' <returns>先验调控网络</returns>
        Public Function LoadPriorNetwork(file As String) As PriorNetwork
            If Not File.Exists(file) Then
                Throw New FileNotFoundException($"先验网络文件不存在：{file}", file)
            End If

            Dim lines As String() = File.ReadAllLines(file)
            Dim network As New PriorNetwork()

            If lines.Length = 0 Then Return network

            ' 第 0 行是表头：TF,TargetGene,RegulationType,Confidence,Evidence
            For row As Integer = 1 To lines.Length - 1
                Dim text As String = lines(row).Trim()

                If text.Length = 0 Then Continue For

                Dim fields As String() = SplitCsv(text)

                If fields.Length < 3 Then
                    Throw New InvalidDataException($"第 {row + 1} 行的先验边字段不足 3 列：{text}")
                End If

                Dim tf As String = fields(0).Trim()
                Dim target As String = fields(1).Trim()
                Dim relation As String = fields(2).Trim()
                Dim confidence As Double = 1.0
                Dim evidence As String = ""

                If fields.Length > 3 AndAlso Not String.IsNullOrWhiteSpace(fields(3)) Then
                    Double.TryParse(fields(3).Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, confidence)
                End If

                If fields.Length > 4 Then evidence = fields(4).Trim()

                If tf.Length = 0 OrElse target.Length = 0 Then Continue For

                Call network.AddEdge(tf, target, ParseEffector(relation), confidence, evidence)
            Next

            Return network
        End Function

        ''' <summary>
        ''' 加载通路定义并构造 Fisher 富集分析所需的背景模型。
        ''' </summary>
        ''' <param name="file">pathway_info.json 路径</param>
        ''' <param name="universeSize">
        ''' 背景基因组规模（超几何检验的 N）。必须显式给定参与检验的基因总数，
        ''' 否则富集会回退为"通路成员并集大小"并系统性高估显著性。
        ''' </param>
        ''' <returns>富集背景模型</returns>
        Public Function LoadPathways(file As String, universeSize As Integer) As EnrichBackground
            If Not File.Exists(file) Then
                Throw New FileNotFoundException($"通路定义文件不存在：{file}", file)
            End If

            If universeSize <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(universeSize), "背景基因组规模必须为正数")
            End If

            Using document As System.Text.Json.JsonDocument = System.Text.Json.JsonDocument.Parse(File.ReadAllText(file))
                Dim root As System.Text.Json.JsonElement = document.RootElement

                If root.ValueKind <> System.Text.Json.JsonValueKind.Object Then
                    Throw New InvalidDataException("pathway_info.json 的根节点必须是对象（通路ID → 定义）")
                End If

                Dim clusters As New List(Of EnrichCluster)()

                For Each term As System.Text.Json.JsonProperty In root.EnumerateObject()
                    Dim definition As System.Text.Json.JsonElement = term.Value
                    Dim displayName As String = term.Name
                    Dim description As String = ""
                    Dim value As System.Text.Json.JsonElement

                    If definition.TryGetProperty("name", value) Then
                        displayName = If(value.GetString(), term.Name)
                    End If

                    If definition.TryGetProperty("description", value) Then
                        description = If(value.GetString(), "")
                    End If

                    Dim genes As New List(Of String)()

                    If definition.TryGetProperty("genes", value) AndAlso
                       value.ValueKind = System.Text.Json.JsonValueKind.Array Then

                        For Each geneElement As System.Text.Json.JsonElement In value.EnumerateArray()
                            Dim gene As String = geneElement.GetString()

                            If Not String.IsNullOrWhiteSpace(gene) Then
                                Call genes.Add(gene.Trim())
                            End If
                        Next
                    End If

                    clusters.Add(New EnrichCluster With {
                        .ID = term.Name,
                        .names = displayName,
                        .description = description,
                        .category = "pathway",
                        .members = genes.Select(Function(g) New EnrichGene(g)).ToArray()
                    })
                Next

                Return New EnrichBackground With {
                    .name = "TestData1 pathways",
                    .id = "TestData1",
                    .comments = "由 pathway_info.json 构造",
                    .build = Date.Now,
                    .size = universeSize,
                    .clusters = clusters.ToArray()
                }
            End Using
        End Function

        ''' <summary>
        ''' 汇总数据规模。
        ''' </summary>
        ''' <param name="expression">表达矩阵</param>
        ''' <param name="prior">先验网络</param>
        ''' <param name="background">通路背景</param>
        ''' <returns>概况统计</returns>
        Public Function Summarize(expression As GeneExpressionData,
                                  prior As PriorNetwork,
                                  background As EnrichBackground) As DemoDataSummary

            Dim times As New HashSet(Of Double)()

            For Each t As Double In expression.TimePoints
                Call times.Add(t)
            Next

            Dim counts As New Dictionary(Of Integer, Integer)()

            For Each t As Double In expression.TimePoints
                Dim key As Integer = CInt(t * 1000.0)

                If counts.ContainsKey(key) Then
                    counts(key) += 1
                Else
                    counts(key) = 1
                End If
            Next

            Dim annotated As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each cluster As EnrichCluster In background.clusters
                For Each member As EnrichGene In cluster.members
                    Call annotated.Add(member.accessionID)
                Next
            Next

            Return New DemoDataSummary With {
                .NGenes = expression.NGene,
                .NSamples = expression.NSample,
                .NTimePoints = times.Count,
                .Replicates = If(counts.Count > 0, counts.Values.Min(), 0),
                .NEdges = prior.Edges.Count,
                .NTF = prior.TFNames.Count,
                .NPathways = background.clusters.Length,
                .NAnnotatedGenes = annotated.Count
            }
        End Function

        ''' <summary>
        ''' 把关系类型文本映射为 <see cref="Effector"/>。
        ''' </summary>
        ''' <param name="relation">CSV 中的类型文本</param>
        ''' <returns>激活 / 抑制 / 未知</returns>
        Private Function ParseEffector(relation As String) As Effector
            Select Case relation.Trim().ToLowerInvariant()
                Case "activation", "activator", "activate", "up", "+"
                    Return Effector.Activator
                Case "repression", "repressor", "inhibition", "inhibitor", "repress", "down", "-"
                    Return Effector.Inhibitor
                Case Else
                    Return Effector.Unknown
            End Select
        End Function

        ''' <summary>按逗号切分一行 CSV（数据不含引号与内嵌逗号）。</summary>
        ''' <param name="line">行文本</param>
        ''' <returns>字段数组</returns>
        Private Function SplitCsv(line As String) As String()
            Return line.Split(","c)
        End Function

    End Module

End Namespace
