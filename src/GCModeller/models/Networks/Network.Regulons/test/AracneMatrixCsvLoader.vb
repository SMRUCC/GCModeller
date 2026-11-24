Imports System.Globalization
Imports System.IO
Imports System.Text
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.BioSystem.Core
Imports std = System.Math

''' <summary>
''' 真实单细胞表达矩阵的流式读取器(专供 ARACNE demo 使用)。
''' 
''' ``GRN_expression_matrix_metacell30k_18533genes_x_30409cells.csv`` 有 4.6 GB，
''' 每一行包含 18533 个基因之中的一个以及 30409 个细胞的表达量。把它整体读进内存
''' (大约 4.5 GB 的 ``Double``) 既慢又没有必要，因此这里采用三步策略：
''' 
''' 1. <b>列抽样</b>：只在 30409 个细胞之中等距抽取若干个(默认 800)作为样本，
'''    这样互信息仍然有足够的统计量，但是内存与解析开销下降了两个数量级；
''' 2. <b>零拷贝解析</b>：逐行扫描字符串的字符来定位逗号，只对<b>被选中的列</b>
'''    做 ``Double.TryParse``，避免为 3 万个字段分配临时字符串；
''' 3. <b>基因筛选</b>：丢掉低表达(零值比例过高)与零方差的基因，保留方差最大的
'''    若干基因，同时<b>强制保留</b>转录因子列表之中的基因。
''' </summary>
Module AracneMatrixCsvLoader

    ''' <summary>
    ''' 表达矩阵的读取与抽样参数
    ''' </summary>
    Public Class LoadOptions

        ''' <summary>
        ''' 需要抽取的细胞(列)数量，默认 800
        ''' </summary>
        ''' <returns></returns>
        Public Property cells As Integer = 800

        ''' <summary>
        ''' 需要保留的基因数量(按照方差排序)，默认 2000
        ''' </summary>
        ''' <returns></returns>
        Public Property genes As Integer = 2000

        ''' <summary>
        ''' 只保留在至少这个比例的样本之中有表达的基因，默认 0.1
        ''' </summary>
        ''' <returns></returns>
        Public Property minNonZeroRatio As Double = 0.1R

        ''' <summary>
        ''' 是否对表达量做 ``log2(x + 1)`` 变换，默认开启
        ''' </summary>
        ''' <returns></returns>
        Public Property logTransform As Boolean = True

        ''' <summary>
        ''' 需要强制保留的基因(转录因子)，即使方差排名靠后也不会被截断
        ''' </summary>
        ''' <returns></returns>
        Public Property transcriptionFactors As String() = Nothing

    End Class

    ''' <summary>
    ''' 读取结果
    ''' </summary>
    Public Class LoadResult

        ''' <summary>
        ''' 行是基因、列是抽样之后的细胞的表达矩阵
        ''' </summary>
        ''' <returns></returns>
        Public Property matrix As Matrix

        ''' <summary>
        ''' CSV 之中被扫描的基因数量
        ''' </summary>
        ''' <returns></returns>
        Public Property scanned As Integer

        ''' <summary>
        ''' 最终保留下来的基因数量
        ''' </summary>
        ''' <returns></returns>
        Public Property retained As Integer

        ''' <summary>
        ''' 原始矩阵的细胞(列)数量
        ''' </summary>
        ''' <returns></returns>
        Public Property totalSamples As Integer

        ''' <summary>
        ''' 抽样之后的细胞数量
        ''' </summary>
        ''' <returns></returns>
        Public Property samples As Integer

        ''' <summary>
        ''' 读取与预处理的总耗时
        ''' </summary>
        ''' <returns></returns>
        Public Property elapsed As TimeSpan

        Public Overrides Function ToString() As String
            Return $"{retained}/{scanned} genes, {samples}/{totalSamples} cells, {elapsed.TotalSeconds:F1}s"
        End Function
    End Class

    ''' <summary>
    ''' 流式读取表达矩阵
    ''' </summary>
    ''' <param name="file">CSV 文件的路径，第一行是细胞名称，第一列是基因编号</param>
    ''' <param name="options">抽样与筛选参数</param>
    ''' <returns></returns>
    Public Function Load(file As String, Optional options As LoadOptions = Nothing) As LoadResult
        Dim opt As LoadOptions = If(options, New LoadOptions())
        Dim watch As Diagnostics.Stopwatch = Diagnostics.Stopwatch.StartNew()

        If String.IsNullOrEmpty(file) Then
            Throw New ArgumentNullException(NameOf(file))
        End If
        ' 注意：参数名 file 会遮蔽 System.IO.File 类型(VB 大小写不敏感)，这里必须写全名
        If Not System.IO.File.Exists(file) Then
            Throw New FileNotFoundException($"the expression matrix file is not found: {file}", file)
        End If

        Using stream As New FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20)
            Using reader As New StreamReader(stream, Encoding.UTF8, True, 1 << 20)
                Dim header As String = reader.ReadLine()

                If String.IsNullOrEmpty(header) Then
                    Throw New InvalidDataException("the given expression matrix is empty!")
                End If

                Dim names As String() = header.Split(","c)
                Dim totalSamples As Integer = names.Length - 1

                If totalSamples < 2 Then
                    Throw New InvalidDataException($"the expression matrix requires at least 2 samples, but {totalSamples} was found!")
                End If

                Dim cells As Integer = If(opt.cells > 0, std.Min(opt.cells, totalSamples), totalSamples)
                Dim selected As Integer() = EvenlySpaced(totalSamples, cells)
                Dim sampleID As String() = selected.Select(Function(i) names(i + 1)).ToArray
                Dim tfSet As HashSet(Of String) = Nothing

                If opt.transcriptionFactors IsNot Nothing AndAlso opt.transcriptionFactors.Length > 0 Then
                    tfSet = New HashSet(Of String)(opt.transcriptionFactors, StringComparer.OrdinalIgnoreCase)
                End If

                Dim values As Double() = New Double(cells - 1) {}
                Dim candidates As New List(Of (name As String, values As Double(), variance As Double, isTf As Boolean))
                Dim scanned As Integer = 0
                Dim line As String

                While True
                    line = reader.ReadLine()

                    If line Is Nothing Then
                        Exit While
                    End If
                    If line.Length = 0 Then
                        Continue While
                    End If

                    Dim name As String = Nothing

                    If Not ParseRow(line, selected, values, opt.logTransform, name) Then
                        Continue While
                    End If

                    scanned += 1

                    ' 零表达比例与方差
                    Dim nonZero As Integer = 0
                    Dim sum As Double = 0.0R

                    For i As Integer = 0 To cells - 1
                        If values(i) > 0.0R Then
                            nonZero += 1
                        End If

                        sum += values(i)
                    Next

                    If opt.minNonZeroRatio > 0.0R AndAlso nonZero / CDbl(cells) < opt.minNonZeroRatio Then
                        Continue While
                    End If

                    Dim mean As Double = sum / cells
                    Dim variance As Double = 0.0R

                    For i As Integer = 0 To cells - 1
                        variance += (values(i) - mean) ^ 2
                    Next

                    variance /= cells

                    If variance <= 0.0R Then
                        Continue While
                    End If

                    Dim keepTf As Boolean = tfSet IsNot Nothing AndAlso tfSet.Contains(name)
                    Dim copy As Double() = DirectCast(values.Clone(), Double())

                    Call candidates.Add((name, copy, variance, keepTf))
                End While

                ' ---- 基因筛选：方差最大的若干个 + 全部转录因子 ----
                Dim limit As Integer = If(opt.genes > 0, opt.genes, candidates.Count)
                Dim top As List(Of (name As String, values As Double(), variance As Double, isTf As Boolean)) = candidates _
                    .OrderByDescending(Function(r) r.variance) _
                    .Take(std.Min(limit, candidates.Count)) _
                    .ToList

                If tfSet IsNot Nothing Then
                    Dim hash As New HashSet(Of String)(top.Select(Function(r) r.name), StringComparer.OrdinalIgnoreCase)

                    For Each r In candidates
                        If r.isTf AndAlso Not hash.Contains(r.name) Then
                            Call top.Add(r)
                            Call hash.Add(r.name)
                        End If
                    Next
                End If

                Dim rows As DataFrameRow() = top _
                    .OrderByDescending(Function(r) r.variance) _
                    .Select(Function(r) New DataFrameRow With {
                                .geneID = r.name,
                                .experiments = r.values
                            }) _
                    .ToArray

                watch.Stop()

                Return New LoadResult With {
                    .matrix = New Matrix With {
                        .tag = $"csv:{System.IO.Path.GetFileName(file)}",
                        .sampleID = sampleID,
                        .expression = rows
                    },
                    .scanned = scanned,
                    .retained = rows.Length,
                    .totalSamples = totalSamples,
                    .samples = cells,
                    .elapsed = watch.Elapsed
                }
            End Using
        End Using
    End Function

    ''' <summary>
    ''' 解析一行表达数据：抽取基因编号以及被选中的细胞列。
    ''' 
    ''' 逐字符扫描来定位逗号(而不是 ``String.Split``)，只对环境之中被选中的列
    ''' 执行数值解析，这样可以避免每行三万个临时字符串分配。
    ''' </summary>
    ''' <param name="line">CSV 的一行</param>
    ''' <param name="selected">被选中的列下标(相对于第一个数据列，升序)</param>
    ''' <param name="values">解析结果的缓冲区</param>
    ''' <param name="logTransform">是否做 ``log2(x + 1)`` 变换</param>
    ''' <param name="name">输出的基因编号</param>
    ''' <returns>整行都可以被解析的时候返回<c>True</c></returns>
    Private Function ParseRow(line As String,
                              selected As Integer(),
                              values As Double(),
                              logTransform As Boolean,
                              ByRef name As String) As Boolean

        Dim field As Integer = 0
        Dim start As Integer = 0
        Dim pos As Integer = 0
        Dim last As Integer = selected(selected.Length - 1)

        For i As Integer = 0 To line.Length
            If i < line.Length AndAlso line(i) <> ","c Then
                Continue For
            End If

            If field = 0 Then
                name = line.Substring(0, i)
            ElseIf pos < selected.Length AndAlso selected(pos) = field - 1 Then
                Dim value As Double

                If Not Double.TryParse(line.AsSpan(start, i - start), NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                    Return False
                End If

                values(pos) = If(logTransform, Log2(value + 1.0R), value)
                pos += 1
            End If

            field += 1
            start = i + 1

            If field > last + 1 OrElse i = line.Length Then
                Exit For
            End If
        Next

        Return pos = selected.Length AndAlso name IsNot Nothing
    End Function

    ''' <summary>
    ''' 在 ``[0, n)`` 之中等距选取 ``k`` 个下标(升序、互不相同)
    ''' </summary>
    ''' <param name="n"></param>
    ''' <param name="k"></param>
    ''' <returns></returns>
    Public Function EvenlySpaced(n As Integer, k As Integer) As Integer()
        If k >= n Then
            Return Enumerable.Range(0, n).ToArray
        End If

        Dim step_ As Double = n / CDbl(k)
        Dim result As New List(Of Integer)(k)
        Dim prev As Integer = -1

        For i As Integer = 0 To k - 1
            Dim idx As Integer = std.Min(n - 1, CInt(std.Floor(i * step_)))

            If idx > prev Then
                Call result.Add(idx)
                prev = idx
            End If
        Next

        Return result.ToArray
    End Function

    ''' <summary>
    ''' 读取转录因子列表(``Homo_sapiens_TF.txt``)。
    ''' 
    ''' 真实的数据文件往往有一些“脏”的地方，这里统一做了容错处理：
    ''' 
    ''' + 文件开头可能存在空行或者 UTF-8 BOM；
    ''' + 表头行不一定在第一行(因此会在前若干行之内寻找包含 ``Symbol`` 的那一行)；
    ''' + ``Symbol`` 为空的行、``NA`` 之类的占位符会被跳过；
    ''' + 大小写不敏感去重；
    ''' + 也兼容“每行一个基因符号”的纯文本文件。
    ''' </summary>
    ''' <param name="path">文件路径</param>
    ''' <returns>去重之后的转录因子列表</returns>
    Public Function LoadTfList(path As String) As String()
        If String.IsNullOrEmpty(path) OrElse Not System.IO.File.Exists(path) Then
            Throw New FileNotFoundException($"the transcription factor list is not found: {path}", path)
        End If

        Dim lines As String() = System.IO.File.ReadAllLines(path, Encoding.UTF8)
        Dim result As New List(Of String)
        Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        ' ---- 定位表头(跳过空行与 BOM) ----
        Dim headerIndex As Integer = -1
        Dim symbolColumn As Integer = -1

        For i As Integer = 0 To std.Min(lines.Length - 1, 10)
            Dim probe As String = lines(i).TrimStart(ChrW(&HFEFF), " "c, ControlChars.Tab)

            If probe.Length = 0 Then
                Continue For
            End If

            Dim header As String() = probe.Split(ControlChars.Tab, ","c, ";")
            Dim column As Integer = Array.FindIndex(header, Function(h) String.Equals(TrimValue(h), "Symbol", StringComparison.OrdinalIgnoreCase))

            If column >= 0 Then
                headerIndex = i
                symbolColumn = column
                Exit For
            End If

            ' 第一行非空行也不是表头：说明这是纯文本的基因符号列表
            Exit For
        Next

        Dim start As Integer = If(headerIndex >= 0, headerIndex + 1, 0)

        For i As Integer = start To lines.Length - 1
            Dim line As String = TrimValue(lines(i))

            If line.Length = 0 Then
                Continue For
            End If

            Dim fields As String() = line.Split(ControlChars.Tab, ","c, ";")
            Dim symbol As String = If(symbolColumn >= 0 AndAlso symbolColumn < fields.Length,
                TrimValue(fields(symbolColumn)),
                TrimValue(fields(0)))

            If IsPlaceholder(symbol) Then
                Continue For
            End If
            If seen.Add(symbol) Then
                Call result.Add(symbol)
            End If
        Next

        Return result.ToArray
    End Function

    ''' <summary>
    ''' 读取 curated 的 ``TF -&gt; TargetGene`` 先验网络
    ''' (``TF_target_prior_curated.csv``)，只保留置信度达到要求的调控关系。
    ''' 
    ''' 文件之中置信度是 ``High`` / ``Medium`` / ``Low`` 这样的文本，
    ''' 调控类型是 ``Activator`` / ``Inhibitor``，这里把它们映射成
    ''' <see cref="RegulatoryEdge.Confidence"/> 的数值与 <see cref="RegulatoryEdge.Sign"/> 的符号。
    ''' </summary>
    ''' <param name="path">先验网络 CSV 的路径</param>
    ''' <param name="minConfidence">需要保留的最低置信度，默认 0.9(只保留 High)</param>
    ''' <returns>先验调控关系(自动去重)</returns>
    Public Function LoadPriorNetwork(path As String, Optional minConfidence As Double = 0.9R) As RegulatoryEdge()
        If String.IsNullOrEmpty(path) OrElse Not System.IO.File.Exists(path) Then
            Throw New FileNotFoundException($"the curated prior network is not found: {path}", path)
        End If

        Dim lines As String() = System.IO.File.ReadAllLines(path, Encoding.UTF8)
        Dim result As New List(Of RegulatoryEdge)
        Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        Dim headerIndex As Integer = -1
        Dim columns As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

        For i As Integer = 0 To std.Min(lines.Length - 1, 10)
            Dim probe As String = lines(i).TrimStart(ChrW(&HFEFF), " "c, ControlChars.Tab)

            If probe.Length = 0 Then
                Continue For
            End If

            Dim header As String() = probe.Split(","c)

            If header.Length >= 3 AndAlso
               Array.FindIndex(header, Function(h) String.Equals(TrimValue(h), "TF", StringComparison.OrdinalIgnoreCase)) >= 0 AndAlso
               Array.FindIndex(header, Function(h) String.Equals(TrimValue(h), "TargetGene", StringComparison.OrdinalIgnoreCase)) >= 0 Then

                headerIndex = i

                For k As Integer = 0 To header.Length - 1
                    columns(TrimValue(header(k))) = k
                Next

                Exit For
            End If

            Exit For
        Next

        If headerIndex < 0 Then
            Throw New InvalidDataException($"the prior network file has no TF/TargetGene header: {path}")
        End If

        Dim tfColumn As Integer = columns("TF")
        Dim targetColumn As Integer = columns("TargetGene")
        Dim confidenceColumn As Integer = If(columns.ContainsKey("Confidence"), columns("Confidence"), -1)
        Dim typeColumn As Integer = If(columns.ContainsKey("RegulationType"), columns("RegulationType"), -1)

        For i As Integer = headerIndex + 1 To lines.Length - 1
            Dim line As String = TrimValue(lines(i))

            If line.Length = 0 Then
                Continue For
            End If

            Dim fields As String() = SplitCsvLine(line)

            If fields.Length <= std.Max(tfColumn, targetColumn) Then
                Continue For
            End If

            Dim tf As String = TrimValue(fields(tfColumn))
            Dim target As String = TrimValue(fields(targetColumn))

            If IsPlaceholder(tf) OrElse IsPlaceholder(target) Then
                Continue For
            End If

            Dim confidence As Double = If(confidenceColumn >= 0 AndAlso confidenceColumn < fields.Length,
                ConfidenceValue(TrimValue(fields(confidenceColumn))),
                0.5R)

            If confidence < minConfidence Then
                Continue For
            End If

            Dim sign As Integer = If(typeColumn >= 0 AndAlso typeColumn < fields.Length,
                RegulationSign(TrimValue(fields(typeColumn))),
                0)

            Dim key As String = $"{tf}->{target}"

            If seen.Add(key) Then
                Call result.Add(New RegulatoryEdge(tf, target, confidence, sign))
            End If
        Next

        Return result.ToArray
    End Function

    ''' <summary>
    ''' 拆分一行 CSV(处理双引号包裹的字段与字段内部的逗号)
    ''' </summary>
    ''' <param name="line"></param>
    ''' <returns></returns>
    Private Function SplitCsvLine(line As String) As String()
        Dim fields As New List(Of String)
        Dim buffer As New Text.StringBuilder()
        Dim quoted As Boolean = False

        For i As Integer = 0 To line.Length - 1
            Dim c As Char = line(i)

            If c = """"c Then
                quoted = Not quoted
            ElseIf c = ","c AndAlso Not quoted Then
                Call fields.Add(buffer.ToString())
                Call buffer.Clear()
            Else
                Call buffer.Append(c)
            End If
        Next

        Call fields.Add(buffer.ToString())

        Return fields.ToArray
    End Function

    ''' <summary>
    ''' 把先验网络的置信度文本映射成数值
    ''' </summary>
    ''' <param name="text">High / Medium / Low 或者一个数字</param>
    ''' <returns></returns>
    Private Function ConfidenceValue(text As String) As Double
        Dim value As Double

        If Double.TryParse(text, Globalization.NumberStyles.Float, Globalization.CultureInfo.InvariantCulture, value) Then
            Return value
        End If

        Select Case text.ToLowerInvariant()
            Case "high"
                Return 0.95R
            Case "medium"
                Return 0.7R
            Case "low"
                Return 0.45R
            Case Else
                Return 0.5R
        End Select
    End Function

    ''' <summary>
    ''' 把调控类型文本映射成因果符号：+1 激活、-1 抑制、0 未知
    ''' </summary>
    ''' <param name="text"></param>
    ''' <returns></returns>
    Private Function RegulationSign(text As String) As Integer
        Select Case text.ToLowerInvariant()
            Case "activator", "activation", "up"
                Return 1
            Case "inhibitor", "repressor", "inhibition", "down"
                Return -1
            Case Else
                Return 0
        End Select
    End Function

    ''' <summary>
    ''' 去掉字段两端空白与 BOM
    ''' </summary>
    Private Function TrimValue(text As String) As String
        If text Is Nothing Then
            Return ""
        End If

        Return text.Trim(ChrW(&HFEFF), " "c, ControlChars.Tab, """"c, ControlChars.CR, ControlChars.LF)
    End Function

    ''' <summary>
    ''' 判断一个基因符号是否为占位符(空、NA、- 等)
    ''' </summary>
    Private Function IsPlaceholder(symbol As String) As Boolean
        If String.IsNullOrWhiteSpace(symbol) Then
            Return True
        End If

        Select Case symbol.Trim().ToUpperInvariant()
            Case "NA", "N/A", "-", "NULL", "NONE", "UNKNOWN"
                Return True
            Case Else
                Return False
        End Select
    End Function

    ''' <summary>
    ''' 以 2 为底的对数
    ''' </summary>
    Private Function Log2(x As Double) As Double
        If x <= 0.0R Then
            Return 0.0R
        End If

        Return std.Log(x) / std.Log(2.0R)
    End Function
End Module
