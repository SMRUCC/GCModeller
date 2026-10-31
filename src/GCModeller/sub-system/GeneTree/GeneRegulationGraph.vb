Imports System.IO
Imports System.IO.Compression
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Model.Network.Regulons.ARACNE

Namespace SMRUCC.genomics.Analysis.GeneTree

    ''' <summary>
    ''' 一条“调控因子 → 靶基因”的输入关系（ARACNE 边的精简视图）
    ''' </summary>
    Public Class RegulationInput

        ''' <summary>靶基因（被调控的一端）</summary>
        Public Property target As String

        ''' <summary>调控因子（转录因子）</summary>
        Public Property regulator As String

        ''' <summary>两个基因之间的互信息（比特），也是排序与裁剪的依据</summary>
        Public Property mi As Double

        ''' <summary>两个基因表达谱的 pearson 相关系数，符号可用于区分激活/抑制的倾向</summary>
        Public Property correlation As Double

        ''' <summary>边的方向类型：<c>directed</c> / <c>mutual</c> / <c>undirected</c></summary>
        Public Property interaction As String

        ''' <summary>该边是否受 curated 先验网络支持</summary>
        Public Property prior As Boolean

        ''' <summary>先验给出的调控类型：<c>activator</c> / <c>inhibitor</c> / 空字符串</summary>
        Public Property regulation As String

        ''' <summary>
        ''' 数据驱动的调控倾向：相关系数为正视为激活，为负视为抑制
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property trend As String
            Get
                If correlation > 0 Then
                    Return "activator"
                ElseIf correlation < 0 Then
                    Return "inhibitor"
                Else
                    Return ""
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"{regulator} -> {target}, MI={mi.ToString("G4")}, r={correlation.ToString("G3")}"
        End Function
    End Class

    ''' <summary>
    ''' ARACNE 网络拓扑在 GeneTree 中的表示：<c>靶基因 → 调控因子集合</c>。
    ''' 
    ''' 与 <see cref="AracneNetwork"/> 的区别在于：
    ''' 
    ''' + 不保存互信息矩阵与表达向量，内存占用只与边数有关；
    ''' + 可以直接从 <c>aracne.zip</c> 的 <c>genes.tsv</c> / <c>tfs.txt</c> / <c>edges.csv</c> 三个条目重建，
    '''   因此**不需要**读取 4.5GB 的 <c>expression.bin</c>；
    ''' + 每个靶基因的调控因子已经按照互信息降序裁剪到 top-K，可以直接作为回归特征。
    ''' </summary>
    ''' <remarks>
    ''' 特征顺序（<see cref="RegulatorsOf"/> 返回的顺序）一旦确定就会被训练器、预测器与存档共同沿用，
    ''' 任何一处顺序不一致都会造成静默的数值错位，因此不要在下游重新排序。
    ''' </remarks>
    Public Class GeneRegulationGraph

        Private ReadOnly m_genes As String()
        Private ReadOnly m_tfFlags As Boolean()
        Private ReadOnly m_regulations As Dictionary(Of String, RegulationInput())
        Private ReadOnly m_forward As Dictionary(Of String, String())
        Private ReadOnly m_targets As String()
        Private ReadOnly m_maxRegulators As Integer
        Private ReadOnly m_edgeCount As Integer

        ''' <summary>
        ''' 拓扑中出现的全部基因（顺序与 ARACNE 的 <c>genes.tsv</c> 一致）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Genes As String()
            Get
                Return m_genes
            End Get
        End Property

        ''' <summary>
        ''' 每一个基因是否为转录因子（下标与 <see cref="Genes"/> 一致）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TfFlags As Boolean()
            Get
                Return m_tfFlags
            End Get
        End Property

        ''' <summary>
        ''' 拥有至少一个调控因子的靶基因列表（按调控者数量降序）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Targets As String()
            Get
                Return m_targets
            End Get
        End Property

        ''' <summary>
        ''' 本次构建时使用的 top-K 裁剪上限（0 表示未裁剪）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property MaxRegulators As Integer
            Get
                Return m_maxRegulators
            End Get
        End Property

        ''' <summary>
        ''' 裁剪之后保留下来的调控边数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property EdgeCount As Integer
            Get
                Return m_edgeCount
            End Get
        End Property

        ''' <summary>
        ''' 靶基因数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TargetCount As Integer
            Get
                Return m_targets.Length
            End Get
        End Property

        Private Sub New(genes As String(),
                        tfFlags As Boolean(),
                        regulations As Dictionary(Of String, RegulationInput()),
                        maxRegulators As Integer)

            m_genes = genes
            m_tfFlags = tfFlags
            m_regulations = regulations
            m_maxRegulators = maxRegulators
            m_targets = regulations _
                .OrderByDescending(Function(kv) kv.Value.Length) _
                .ThenBy(Function(kv) kv.Key, StringComparer.OrdinalIgnoreCase) _
                .Select(Function(kv) kv.Key) _
                .ToArray
            m_edgeCount = regulations.Values.Sum(Function(list) list.Length)

            Dim forward As New Dictionary(Of String, List(Of String))(StringComparer.OrdinalIgnoreCase)

            For Each target As String In m_targets
                For Each input As RegulationInput In regulations(target)
                    Dim list As List(Of String) = Nothing

                    If Not forward.TryGetValue(input.regulator, list) Then
                        list = New List(Of String)
                        forward(input.regulator) = list
                    End If

                    Call list.Add(target)
                Next
            Next

            m_forward = forward.ToDictionary(
                Function(kv) kv.Key,
                Function(kv) kv.Value.Distinct(StringComparer.OrdinalIgnoreCase).ToArray,
                StringComparer.OrdinalIgnoreCase)
        End Sub

        ''' <summary>
        ''' 取出一个靶基因的调控因子（已经按照互信息降序裁剪）
        ''' </summary>
        ''' <param name="target">靶基因编号</param>
        ''' <returns>调控因子数组；没有调控关系时返回空数组</returns>
        Public Function RegulatorsOf(target As String) As RegulationInput()
            Dim list As RegulationInput() = Nothing

            If Not String.IsNullOrEmpty(target) AndAlso m_regulations.TryGetValue(target, list) Then
                Return list
            Else
                Return New RegulationInput() {}
            End If
        End Function

        ''' <summary>
        ''' 取出一个调控因子所调控的全部靶基因（正向邻接，用于可达性分析）
        ''' </summary>
        ''' <param name="regulator">调控因子编号</param>
        ''' <returns>靶基因数组；该基因不调控任何基因时返回空数组</returns>
        Public Function TargetsOf(regulator As String) As String()
            Dim list As String() = Nothing

            If Not String.IsNullOrEmpty(regulator) AndAlso m_forward.TryGetValue(regulator, list) Then
                Return list
            Else
                Return New String() {}
            End If
        End Function

        ''' <summary>
        ''' 枚举全部被裁剪之后保留下来的调控关系
        ''' </summary>
        ''' <returns></returns>
        Public Iterator Function Regulations() As IEnumerable(Of RegulationInput)
            For Each target As String In m_targets
                For Each input As RegulationInput In m_regulations(target)
                    Yield input
                Next
            Next
        End Function

        ''' <summary>
        ''' 从已经加载的 <see cref="AracneNetwork"/> 构建拓扑
        ''' </summary>
        ''' <param name="network">ARACNE 推断结果</param>
        ''' <param name="config">用于 top-K 裁剪的配置；为空时使用快速档</param>
        ''' <returns></returns>
        Public Shared Function FromAracne(network As AracneNetwork,
                                          Optional config As GeneTreeConfig = Nothing) As GeneRegulationGraph

            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If

            Return FromEdges(network.Genes, network.TfFlags, network.Edges, config)
        End Function

        ''' <summary>
        ''' 由 ARACNE 的有向边集合构建拓扑
        ''' </summary>
        ''' <param name="genes">基因编号</param>
        ''' <param name="tfFlags">基因是否为转录因子</param>
        ''' <param name="edges">带方向的调控边</param>
        ''' <param name="config">用于 top-K 裁剪的配置；为空时使用快速档</param>
        ''' <returns></returns>
        Public Shared Function FromEdges(genes As String(),
                                         tfFlags As Boolean(),
                                         edges As IEnumerable(Of AracneEdge),
                                         Optional config As GeneTreeConfig = Nothing) As GeneRegulationGraph

            Dim opt As GeneTreeConfig = If(config, GeneTreeConfig.CreateFast())
            Dim records As New List(Of RegulationInput)

            For Each edge As AracneEdge In edges.SafeQuery
                If edge Is Nothing Then
                    Continue For
                End If
                If Not opt.IncludeUndirectedEdges AndAlso String.Equals(edge.interaction, "undirected", StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                Call records.Add(New RegulationInput With {
                    .target = edge.target,
                    .regulator = edge.regulator,
                    .mi = edge.mi,
                    .correlation = edge.correlation,
                    .interaction = edge.interaction,
                    .prior = edge.prior,
                    .regulation = edge.regulation
                })
            Next

            Return FromRegulations(genes, tfFlags, records, opt)
        End Function

        ''' <summary>
        ''' 由一组调控关系记录构建拓扑（同一条边的重复注释会保留互信息最大的那一条）
        ''' </summary>
        ''' <param name="genes">基因编号</param>
        ''' <param name="tfFlags">基因是否为转录因子，可以为空</param>
        ''' <param name="records">调控关系记录</param>
        ''' <param name="config">用于 top-K 裁剪的配置；为空时使用快速档</param>
        ''' <returns></returns>
        Public Shared Function FromRegulations(genes As String(),
                                               tfFlags As Boolean(),
                                               records As IEnumerable(Of RegulationInput),
                                               Optional config As GeneTreeConfig = Nothing) As GeneRegulationGraph

            Dim opt As GeneTreeConfig = If(config, GeneTreeConfig.CreateFast())
            Dim names As String() = If(genes, New String() {})
            Dim flags As Boolean() = If(tfFlags, New Boolean(names.Length - 1) {})

            If flags.Length <> names.Length Then
                Array.Resize(flags, names.Length)
            End If

            Dim bucket As New Dictionary(Of String, Dictionary(Of String, RegulationInput))(StringComparer.OrdinalIgnoreCase)

            For Each record As RegulationInput In records.SafeQuery
                If record Is Nothing Then
                    Continue For
                End If

                Dim target As String = If(record.target, "").Trim()
                Dim regulator As String = If(record.regulator, "").Trim()

                If target.Length = 0 OrElse regulator.Length = 0 Then
                    Continue For
                End If
                If String.Equals(target, regulator, StringComparison.OrdinalIgnoreCase) Then
                    ' 自环对回归没有意义
                    Continue For
                End If
                If Not opt.IncludeUndirectedEdges AndAlso String.Equals(record.interaction, "undirected", StringComparison.OrdinalIgnoreCase) Then
                    Continue For
                End If

                Dim edges As Dictionary(Of String, RegulationInput) = Nothing

                If Not bucket.TryGetValue(target, edges) Then
                    edges = New Dictionary(Of String, RegulationInput)(StringComparer.OrdinalIgnoreCase)
                    bucket(target) = edges
                End If

                Dim exists As RegulationInput = Nothing

                If edges.TryGetValue(regulator, exists) Then
                    ' 同一条无向边可能被不同的数据库重复注释：保留互信息最大的那一条
                    If exists.mi >= record.mi Then
                        Continue For
                    End If
                End If

                edges(regulator) = record
            Next

            Dim regulations As New Dictionary(Of String, RegulationInput())(StringComparer.OrdinalIgnoreCase)

            For Each item As KeyValuePair(Of String, Dictionary(Of String, RegulationInput)) In bucket
                Dim list As RegulationInput() = item.Value.Values _
                    .OrderByDescending(Function(x) x.mi) _
                    .ToArray

                If opt.MaxRegulators > 0 AndAlso list.Length > opt.MaxRegulators Then
                    list = list.Take(opt.MaxRegulators).ToArray
                End If

                regulations(item.Key) = list
            Next

            Return New GeneRegulationGraph(names, flags, regulations, opt.MaxRegulators)
        End Function

        ''' <summary>
        ''' 从一个 <c>aracne.zip</c> 存档构建拓扑。
        ''' 
        ''' 只读取 <c>genes.tsv</c>、<c>tfs.txt</c> 与 <c>edges.csv</c> 三个条目，
        ''' 因此即使存档之中包含数 GB 的表达矩阵也不会有额外的内存开销。
        ''' </summary>
        ''' <param name="zipFile">ARACNE 模型压缩包的路径</param>
        ''' <param name="config">用于 top-K 裁剪的配置；为空时使用快速档</param>
        ''' <returns></returns>
        Public Shared Function LoadFromArchive(zipFile As String,
                                              Optional config As GeneTreeConfig = Nothing) As GeneRegulationGraph

            If String.IsNullOrEmpty(zipFile) Then
                Throw New ArgumentNullException(NameOf(zipFile))
            End If
            If Not File.Exists(zipFile) Then
                Throw New FileNotFoundException($"找不到 ARACNE 模型文件: {zipFile}", zipFile)
            End If

            Using stream As Stream = File.OpenRead(zipFile)
                Return LoadFromArchive(stream, config)
            End Using
        End Function

        ''' <summary>
        ''' 从一个 <c>aracne.zip</c> 存档流构建拓扑（只读取三个小条目）
        ''' </summary>
        ''' <param name="file">zip 压缩包的输入流（调用返回之后流仍然保持打开）</param>
        ''' <param name="config">用于 top-K 裁剪的配置；为空时使用快速档</param>
        ''' <returns></returns>
        Public Shared Function LoadFromArchive(file As Stream,
                                               Optional config As GeneTreeConfig = Nothing) As GeneRegulationGraph

            If file Is Nothing Then
                Throw New ArgumentNullException(NameOf(file))
            End If

            Using zip As New ZipArchive(file, ZipArchiveMode.Read, leaveOpen:=True)
                Dim genes As String() = Nothing
                Dim tfFlags As Boolean() = Nothing

                Call readGenes(zip, genes, tfFlags)

                If genes Is Nothing OrElse genes.Length = 0 Then
                    Throw New InvalidDataException("the ARACNE archive does not contain any gene: the genes.tsv is missing or empty!")
                End If

                ' tfs.txt 是转录因子注释的权威来源，genes.tsv 的标记作为补充
                Dim tfs As String() = readTFs(zip)

                If tfs.Length > 0 Then
                    Dim index As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

                    For i As Integer = 0 To genes.Length - 1
                        index(genes(i)) = i
                    Next

                    Dim flags As Boolean() = New Boolean(genes.Length - 1) {}

                    For Each id As String In tfs
                        Dim i As Integer

                        If index.TryGetValue(id, i) Then
                            flags(i) = True
                        End If
                    Next

                    For i As Integer = 0 To genes.Length - 1
                        If tfFlags IsNot Nothing AndAlso tfFlags.Length > i AndAlso tfFlags(i) Then
                            flags(i) = True
                        End If
                    Next

                    tfFlags = flags
                End If

                Dim records As List(Of RegulationInput) = readEdges(zip)

                Return FromRegulations(genes, tfFlags, records, config)
            End Using
        End Function

#Region "archive readers"

        ''' <summary>
        ''' 读取 <c>genes.tsv</c>（<c>gene&lt;TAB&gt;is_tf</c>，无表头）
        ''' </summary>
        Private Shared Sub readGenes(zip As ZipArchive, ByRef genes As String(), ByRef tfFlags As Boolean())
            Using stream As Stream = openEntry(zip, "genes.tsv")
                If stream Is Nothing Then
                    genes = New String() {}
                    tfFlags = New Boolean() {}
                    Return
                End If

                Dim names As New List(Of String)
                Dim flags As New List(Of Boolean)

                Using reader As New StreamReader(stream, Encoding.UTF8)
                    Dim line As String = reader.ReadLine()

                    While line IsNot Nothing
                        If line.Length > 0 Then
                            Dim fields As String() = line.Split(ControlChars.Tab)

                            Call names.Add(fields(0).Trim)
                            Call flags.Add(fields.Length > 1 AndAlso fields(1).Trim() = "1")
                        End If

                        line = reader.ReadLine()
                    End While
                End Using

                genes = names.ToArray
                tfFlags = flags.ToArray
            End Using
        End Sub

        ''' <summary>
        ''' 读取 <c>tfs.txt</c>（一行一个转录因子，无表头）
        ''' </summary>
        Private Shared Function readTFs(zip As ZipArchive) As String()
            Using stream As Stream = openEntry(zip, "tfs.txt")
                If stream Is Nothing Then
                    Return New String() {}
                End If

                Dim result As New List(Of String)

                Using reader As New StreamReader(stream, Encoding.UTF8)
                    Dim line As String = reader.ReadLine()

                    While line IsNot Nothing
                        If line.Trim().Length > 0 Then
                            Call result.Add(line.Trim())
                        End If

                        line = reader.ReadLine()
                    End While
                End Using

                Return result.ToArray
            End Using
        End Function

        ''' <summary>
        ''' 读取 <c>edges.csv</c>（按表头名定位列，缺失表头时回退到固定列序）
        ''' </summary>
        Private Shared Function readEdges(zip As ZipArchive) As List(Of RegulationInput)
            Dim result As New List(Of RegulationInput)

            Using stream As Stream = openEntry(zip, "edges.csv")
                If stream Is Nothing Then
                    Return result
                End If

                Using reader As New StreamReader(stream, Encoding.UTF8)
                    Dim header As String = reader.ReadLine()

                    If header Is Nothing Then
                        Return result
                    End If

                    Dim columns As Dictionary(Of String, Integer) = parseHeader(header)
                    Dim line As String = reader.ReadLine()

                    While line IsNot Nothing
                        If line.Length > 0 Then
                            Dim f As String() = line.Split(","c)

                            If f.Length >= 2 Then
                                Dim record As New RegulationInput With {
                                    .regulator = field(f, columns, "regulator", 0),
                                    .target = field(f, columns, "target", 1),
                                    .mi = number(f, columns, "mi", 2),
                                    .correlation = number(f, columns, "correlation", 3),
                                    .interaction = field(f, columns, "interaction", 4),
                                    .prior = isTrue(field(f, columns, "prior", 7)),
                                    .regulation = field(f, columns, "regulation", 8)
                                }

                                Call result.Add(record)
                            End If
                        End If

                        line = reader.ReadLine()
                    End While
                End Using
            End Using

            Return result
        End Function

        Private Shared Function parseHeader(header As String) As Dictionary(Of String, Integer)
            Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            Dim columns As String() = header.Split(","c)

            For i As Integer = 0 To columns.Length - 1
                Dim name As String = columns(i).Trim().Trim(""""c)

                If name.Length > 0 AndAlso Not map.ContainsKey(name) Then
                    map(name) = i
                End If
            Next

            Return map
        End Function

        Private Shared Function field(f As String(), columns As Dictionary(Of String, Integer),
                                      name As String, fallback As Integer) As String

            Dim i As Integer

            If Not columns.TryGetValue(name, i) Then
                i = fallback
            End If

            If i >= 0 AndAlso i < f.Length Then
                Return f(i).Trim()
            Else
                Return ""
            End If
        End Function

        Private Shared Function number(f As String(), columns As Dictionary(Of String, Integer),
                                       name As String, fallback As Integer) As Double

            Dim text As String = field(f, columns, name, fallback)
            Dim value As Double

            If Double.TryParse(text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, value) Then
                Return value
            Else
                Return 0.0R
            End If
        End Function

        Private Shared Function isTrue(text As String) As Boolean
            Return text = "1" OrElse String.Equals(text, "true", StringComparison.OrdinalIgnoreCase)
        End Function

        Private Shared Function openEntry(zip As ZipArchive, name As String) As Stream
            Dim entry As ZipArchiveEntry = zip.GetEntry(name)

            If entry Is Nothing Then
                Return Nothing
            Else
                Return entry.Open()
            End If
        End Function

#End Region

        ''' <summary>
        ''' 构建拓扑摘要：<c>{targets} targets, {edges} regulation edges (top-K = {k})</c>
        ''' </summary>
        ''' <returns></returns>
        Public Overrides Function ToString() As String
            Return $"{m_genes.Length} genes, {m_targets.Length} targets, {m_edgeCount} regulation edges " &
                   $"(top-K = {If(m_maxRegulators = 0, "all", CStr(m_maxRegulators))})"
        End Function

    End Class
End Namespace
