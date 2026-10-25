Imports System.Diagnostics
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.SGrid.GRN
Imports PriorEdge = SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge
Imports PriorNet = SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork

Namespace GRNDemo

    ''' <summary>
    ''' The prior knowledge driven gene selection and the streaming subset extraction
    ''' of a huge gene expression matrix.
    ''' (先验知识驱动的基因挑选，以及超大表达矩阵的流式子集提取)
    ''' </summary>
    ''' <remarks>
    ''' 表达矩阵文件动辄数个 GB（行是基因、列是细胞），整体加载到内存之中是不现实的，
    ''' 故这里只做「流式扫描」：逐行读取，每行只定位第一个逗号之后取出基因符号，
    ''' 命中所选基因的时候才把整行原样写到子集文件之中。
    ''' 
    ''' 注意：**绝不能**对每一行做 <c>Split(",")</c>，
    ''' 对于一个 3 万列 × 1.8 万行的矩阵而言那会产生数亿个临时字符串对象。
    ''' </remarks>
    Public Module GeneSelector

        ''' <summary>
        ''' 扫描表达矩阵并返回其全部基因符号
        ''' </summary>
        ''' <param name="matrixFile">表达矩阵文件</param>
        ''' <param name="cacheFile">基因清单的缓存文件；可以避免重复扫描几个 GB 的矩阵</param>
        ''' <param name="force">是否忽略缓存强制重新扫描</param>
        ''' <param name="quiet"></param>
        ''' <returns></returns>
        Public Function ReadMatrixGenes(matrixFile As String,
                                        Optional cacheFile As String = Nothing,
                                        Optional force As Boolean = False,
                                        Optional quiet As Boolean = False) As String()

            If Not String.IsNullOrEmpty(cacheFile) AndAlso Not force AndAlso File.Exists(cacheFile) Then
                Dim cached As String() = ReadGeneList(cacheFile)

                If cached.Length > 0 Then
                    If Not quiet Then
                        Call Console.WriteLine($"[INFO] matrix gene list cache hit: {cached.Length} gene(s) ({cacheFile})")
                    End If

                    Return cached
                End If
            End If

            Dim genes As New List(Of String)
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Using reader As New StreamReader(matrixFile, Encoding.UTF8, detectEncodingFromByteOrderMarks:=True, bufferSize:=1 << 20)
                Dim header As String = reader.ReadLine()
                Dim line As String

                If header Is Nothing Then
                    Throw New InvalidDataException($"The expression matrix '{matrixFile}' is empty!")
                End If

                Do
                    line = reader.ReadLine()

                    If line Is Nothing Then
                        Exit Do
                    End If
                    If line.Length = 0 Then
                        Continue Do
                    End If

                    Call genes.Add(firstField(line))

                    If Not quiet AndAlso genes.Count Mod 2000 = 0 Then
                        Call Console.WriteLine($"[INFO] scanning gene symbols: {genes.Count} row(s), {watch.Elapsed.TotalSeconds.ToString("F1")}s")
                    End If
                Loop
            End Using

            If Not quiet Then
                Call Console.WriteLine($"[INFO] expression matrix: {genes.Count} gene(s) was scanned in {watch.Elapsed.TotalSeconds.ToString("F1")}s")
            End If

            If Not String.IsNullOrEmpty(cacheFile) Then
                Call WriteGeneList(cacheFile, genes)
            End If

            Return genes.ToArray()
        End Function

        ''' <summary>
        ''' 依据先验知识挑选出一个连通的基因子集
        ''' </summary>
        ''' <param name="prior">先验调控网络</param>
        ''' <param name="matrixGenes">表达矩阵之中的全部基因符号</param>
        ''' <param name="coreSize">核心基因（最终参与拟合）的数量</param>
        ''' <param name="extractSize">需要导出到子集矩阵之中的基因数量</param>
        ''' <param name="quiet"></param>
        ''' <returns>先验连接度最高的一个连通子网</returns>
        Public Function SelectGenes(prior As PriorNet,
                                    matrixGenes As String(),
                                    Optional coreSize As Integer = 30,
                                    Optional extractSize As Integer = 200,
                                    Optional quiet As Boolean = False) As String()

            If prior Is Nothing Then
                Throw New ArgumentNullException(NameOf(prior))
            End If
            If matrixGenes.IsNullOrEmpty Then
                Throw New ArgumentException("The expression matrix does not contain any gene!", NameOf(matrixGenes))
            End If

            If coreSize < 2 Then
                coreSize = 2
            End If
            If extractSize < coreSize Then
                extractSize = coreSize
            End If

            ' 基因符号大小写不敏感：先验表使用大写的基因符号，而表达矩阵保留原始写法
            Dim actual As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

            For Each gene As String In matrixGenes
                If Not String.IsNullOrEmpty(gene) AndAlso Not actual.ContainsKey(gene) Then
                    Call actual.Add(gene, gene)
                End If
            Next

            Dim score As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
            Dim adjacent As New Dictionary(Of String, HashSet(Of String))(StringComparer.OrdinalIgnoreCase)
            Dim matched As Integer = 0

            For Each edge As PriorEdge In prior.Edges.SafeQuery
                If Not actual.ContainsKey(edge.TF) Then
                    Continue For
                End If
                If Not actual.ContainsKey(edge.TargetGene) Then
                    Continue For
                End If

                matched += 1

                Dim weight As Double = edge.EdgeWeight()

                bump(score, edge.TF, weight)
                bump(score, edge.TargetGene, weight)
                link(adjacent, edge.TF, edge.TargetGene)
                link(adjacent, edge.TargetGene, edge.TF)
            Next

            If Not quiet Then
                Call Console.WriteLine($"[INFO] {matched} prior edge(s) is matched on the expression matrix, {score.Count} gene(s) is scored.")
            End If

            If score.Count = 0 Then
                ' 先验知识全部无法匹配的时候退化为直接取矩阵的前若干个基因
                Call Console.WriteLine("[WARN] none of the prior edges can be matched on the expression matrix, the gene selection falls back to the matrix order.")

                Return matrixGenes.Take(extractSize).ToArray
            End If

            Dim selected As New List(Of String)
            Dim chosen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Dim frontier As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
            Dim seed As String = score _
                .OrderByDescending(Function(a) a.Value) _
                .Select(Function(a) a.Key) _
                .First

            Call selected.Add(seed)
            Call chosen.Add(seed)
            Call pushFrontier(adjacent, frontier, chosen, seed)

            While selected.Count < coreSize AndAlso frontier.Count > 0
                Dim nextGene As String = frontier _
                    .OrderByDescending(Function(g) scoreOf(score, g)) _
                    .ThenBy(Function(g) g, StringComparer.OrdinalIgnoreCase) _
                    .First

                Call frontier.Remove(nextGene)
                Call selected.Add(nextGene)
                Call chosen.Add(nextGene)
                Call pushFrontier(adjacent, frontier, chosen, nextGene)
            End While

            If selected.Count < coreSize Then
                ' 连通分量小于期望的核心规模的时候，用剩余的得分最高的基因补齐
                For Each gene As String In score _
                    .OrderByDescending(Function(a) a.Value) _
                    .Select(Function(a) a.Key)

                    If selected.Count >= coreSize Then
                        Exit For
                    End If
                    If chosen.Contains(gene) Then
                        Continue For
                    End If

                    Call selected.Add(gene)
                    Call chosen.Add(gene)
                Next
            End If

            ' 核心基因 + 其邻域，构成需要导出到子集矩阵的基因集合
            Dim extract As New List(Of String)
            Dim inExtract As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each gene As String In selected
                If inExtract.Add(gene) Then
                    Call extract.Add(actual(gene))
                End If
            Next

            Dim neighbors As String() = selected _
                .SelectMany(Function(gene) If(adjacent.ContainsKey(gene), adjacent(gene), New HashSet(Of String)(StringComparer.OrdinalIgnoreCase))) _
                .Where(Function(gene) Not chosen.Contains(gene)) _
                .Distinct(StringComparer.OrdinalIgnoreCase) _
                .OrderByDescending(Function(gene) scoreOf(score, gene)) _
                .ToArray

            For Each gene As String In neighbors
                If extract.Count >= extractSize Then
                    Exit For
                End If
                If inExtract.Add(gene) Then
                    Call extract.Add(actual(gene))
                End If
            Next

            If Not quiet Then
                Call Console.WriteLine($"[INFO] gene selection: {selected.Count} core gene(s), {extract.Count} gene(s) exported for the subset matrix.")
                Call Console.WriteLine($"[INFO] core genes: {selected.JoinBy(", ")}")
            End If

            Return extract.ToArray()
        End Function

        ''' <summary>
        ''' 从超大的表达矩阵之中流式提取出指定基因的子集矩阵
        ''' </summary>
        ''' <param name="matrixFile">原始表达矩阵</param>
        ''' <param name="outFile">输出的子集矩阵</param>
        ''' <param name="genes">需要提取的基因符号</param>
        ''' <param name="quiet"></param>
        ''' <returns>实际写出的基因数量</returns>
        Public Function ExtractSubset(matrixFile As String,
                                      outFile As String,
                                      genes As IEnumerable(Of String),
                                      Optional quiet As Boolean = False) As Integer

            Dim index As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each gene As String In genes.SafeQuery()
                If Not String.IsNullOrEmpty(gene) Then
                    Call index.Add(gene.Trim())
                End If
            Next

            If index.Count = 0 Then
                Throw New ArgumentException("At least one gene should be specified for the subset extraction!", NameOf(genes))
            End If

            Dim dir As String = Path.GetDirectoryName(Path.GetFullPath(outFile))

            If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            Dim written As Integer = 0
            Dim total As Long = 0
            Dim watch As Stopwatch = Stopwatch.StartNew()

            Using reader As New StreamReader(matrixFile, Encoding.UTF8, detectEncodingFromByteOrderMarks:=True, bufferSize:=1 << 20)
                Using writer As New StreamWriter(outFile, append:=False, encoding:=New UTF8Encoding(False), bufferSize:=1 << 20)
                    Dim header As String = reader.ReadLine()
                    Dim line As String

                    If header Is Nothing Then
                        Throw New InvalidDataException($"The expression matrix '{matrixFile}' is empty!")
                    End If

                    Call writer.WriteLine(header)

                    Do
                        line = reader.ReadLine()

                        If line Is Nothing Then
                            Exit Do
                        End If

                        total += 1

                        If line.Length = 0 Then
                            Continue Do
                        End If

                        If index.Contains(firstField(line)) Then
                            Call writer.WriteLine(line)
                            written += 1
                        End If

                        If Not quiet AndAlso total Mod 2000 = 0 Then
                            Call Console.WriteLine($"[INFO] extracting genes: {total} row(s) scanned, {written} row(s) written, {watch.Elapsed.TotalSeconds.ToString("F1")}s")
                        End If
                    Loop
                End Using
            End Using

            If Not quiet Then
                Call Console.WriteLine($"[INFO] subset matrix '{Path.GetFileName(outFile)}' was written: {written}/{index.Count} gene(s) in {watch.Elapsed.TotalSeconds.ToString("F1")}s")
            End If

            Return written
        End Function

        ''' <summary>
        ''' 写出基因清单
        ''' </summary>
        ''' <param name="listFile">目标文件</param>
        ''' <param name="genes"></param>
        Public Sub WriteGeneList(listFile As String, genes As IEnumerable(Of String))
            Dim dir As String = Path.GetDirectoryName(Path.GetFullPath(listFile))

            If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            Call File.WriteAllLines(listFile, genes.SafeQuery().ToArray(), New UTF8Encoding(False))
        End Sub

        ''' <summary>
        ''' 读取基因清单
        ''' </summary>
        ''' <param name="listFile"></param>
        ''' <returns></returns>
        Public Function ReadGeneList(listFile As String) As String()
            If String.IsNullOrEmpty(listFile) OrElse Not File.Exists(listFile) Then
                Return New String() {}
            End If

            Return File _
                .ReadAllLines(listFile) _
                .Where(Function(line) Not String.IsNullOrWhiteSpace(line)) _
                .Select(Function(line) line.Trim()) _
                .ToArray()
        End Function

        ''' <summary>
        ''' 取出 csv 行之中的第一个字段（基因符号），不拆分整行
        ''' </summary>
        ''' <param name="line"></param>
        ''' <returns></returns>
        Private Function firstField(line As String) As String
            Dim index As Integer = line.IndexOf(","c)
            Dim gene As String

            If index < 0 Then
                gene = line
            Else
                gene = line.Substring(0, index)
            End If

            Return gene.Trim().Trim(""""c)
        End Function

        Private Sub bump(score As Dictionary(Of String, Double), gene As String, weight As Double)
            Dim value As Double

            If score.TryGetValue(gene, value) Then
                score(gene) = value + weight
            Else
                score(gene) = weight
            End If
        End Sub

        Private Sub link(adjacent As Dictionary(Of String, HashSet(Of String)), a As String, b As String)
            Dim list As HashSet(Of String) = Nothing

            If Not adjacent.TryGetValue(a, list) Then
                list = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
                adjacent(a) = list
            End If

            Call list.Add(b)
        End Sub

        Private Sub pushFrontier(adjacent As Dictionary(Of String, HashSet(Of String)),
                                 frontier As HashSet(Of String),
                                 chosen As HashSet(Of String),
                                 gene As String)
            Dim list As HashSet(Of String) = Nothing

            If Not adjacent.TryGetValue(gene, list) Then
                Return
            End If

            For Each neighbor As String In list
                If Not chosen.Contains(neighbor) Then
                    Call frontier.Add(neighbor)
                End If
            Next
        End Sub

        Private Function scoreOf(score As Dictionary(Of String, Double), gene As String) As Double
            Dim value As Double

            If score.TryGetValue(gene, value) Then
                Return value
            Else
                Return 0
            End If
        End Function
    End Module
End Namespace
