Imports System.Globalization
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace GRNDemo

    ''' <summary>
    ''' The gene expression matrix which has been binned along the pseudotime axis.
    ''' (已经沿着伪时间轴分箱之后的基因表达矩阵)
    ''' </summary>
    Public Class BinnedExpression

        ''' <summary>
        ''' 分箱之后的表达矩阵（行是基因、列是 pseudotime 箱）
        ''' </summary>
        ''' <returns></returns>
        Public Property Matrix As Matrix
        ''' <summary>
        ''' 每一个箱所对应的 pseudotime（严格单调递增）
        ''' </summary>
        ''' <returns></returns>
        Public Property TimePoints As Double()
        ''' <summary>
        ''' 没有 pseudotime 记录而被丢弃的细胞数量
        ''' </summary>
        ''' <returns></returns>
        Public Property DroppedCells As Integer = 0
        ''' <summary>
        ''' 没有落入任何细胞而被插值填充的箱数量
        ''' </summary>
        ''' <returns></returns>
        Public Property EmptyBins As Integer = 0

        ''' <summary>
        ''' 基因的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Size As Integer
            Get
                If Matrix Is Nothing Then Return 0 Else Return Matrix.expression.Length
            End Get
        End Property

        ''' <summary>
        ''' 时间点的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property nBins As Integer
            Get
                If TimePoints Is Nothing Then Return 0 Else Return TimePoints.Length
            End Get
        End Property

        Public Overrides Function ToString() As String
            If TimePoints.IsNullOrEmpty Then
                Return $"{Size} gene(s) x 0 pseudotime bin(s)"
            Else
                Return $"{Size} gene(s) x {nBins} pseudotime bin(s) [{TimePoints(0).ToString("G4")}, {TimePoints(TimePoints.Length - 1).ToString("G4")}]"
            End If
        End Function
    End Class

    ''' <summary>
    ''' The pseudotime axis construction from the single cell mapping table.
    ''' (由单细胞 pseudotime 映射表构造时序表达矩阵)
    ''' </summary>
    ''' <remarks>
    ''' 单细胞数据之中每一个细胞都可以被视作一个时间点，但是细胞数量可以达到数万，
    ''' 直接对这么多采样点做数值微分与样条插值既慢又不稳定；
    ''' 故这里先把细胞按照 pseudotime 等宽分箱，再对每一个箱取表达均值，
    ''' 从而把数据规整为「基因 × 时间点」的经典时序形式。
    ''' </remarks>
    Public Module PseudotimeMatrix

        ''' <summary>
        ''' 读取细胞到 pseudotime 的映射表
        ''' </summary>
        ''' <param name="filePath">包含 cell 与 pseudotime 两列的 csv 文件</param>
        ''' <returns></returns>
        Public Function LoadMapping(filePath As String) As Dictionary(Of String, Double)
            Dim mapping As New Dictionary(Of String, Double)(StringComparer.OrdinalIgnoreCase)
            Dim iCell As Integer = 1
            Dim iPseudotime As Integer = 2
            Dim header As Boolean = True

            Using reader As New StreamReader(filePath, Encoding.UTF8, detectEncodingFromByteOrderMarks:=True, bufferSize:=1 << 20)
                Dim line As String

                Do
                    line = reader.ReadLine()

                    If line Is Nothing Then
                        Exit Do
                    End If
                    If String.IsNullOrWhiteSpace(line) Then
                        Continue Do
                    End If

                    Dim fields As String() = SMRUCC.genomics.Analysis.SGrid.GRN.PriorKnowledge.SplitCsvLine(line)

                    If header Then
                        header = False

                        For i As Integer = 0 To fields.Length - 1
                            Dim name As String = fields(i).Trim().Trim(""""c)

                            If String.Equals(name, "cell", StringComparison.OrdinalIgnoreCase) Then
                                iCell = i
                            ElseIf String.Equals(name, "pseudotime", StringComparison.OrdinalIgnoreCase) Then
                                iPseudotime = i
                            End If
                        Next

                        Continue Do
                    End If

                    If fields.Length <= std.Max(iCell, iPseudotime) Then
                        Continue Do
                    End If

                    Dim cell As String = fields(iCell).Trim()
                    Dim time As Double

                    If String.IsNullOrEmpty(cell) Then
                        Continue Do
                    End If
                    If Not Double.TryParse(fields(iPseudotime), NumberStyles.Float, CultureInfo.InvariantCulture, time) Then
                        Continue Do
                    End If

                    mapping(cell) = time
                Loop
            End Using

            Return mapping
        End Function

        ''' <summary>
        ''' 把表达矩阵沿着 pseudotime 轴等宽分箱
        ''' </summary>
        ''' <param name="mat">行是基因、列是细胞的表达矩阵</param>
        ''' <param name="pseudotime">细胞到 pseudotime 的映射</param>
        ''' <param name="bins">分箱的数量</param>
        ''' <returns></returns>
        Public Function Bin(mat As Matrix, pseudotime As Dictionary(Of String, Double), Optional bins As Integer = 60) As BinnedExpression
            If mat Is Nothing Then
                Throw New ArgumentNullException(NameOf(mat))
            End If
            If pseudotime Is Nothing OrElse pseudotime.Count = 0 Then
                Throw New ArgumentException("The cell to pseudotime mapping is empty!", NameOf(pseudotime))
            End If
            If bins < 4 Then
                bins = 4
            End If

            Dim cells As String() = mat.sampleID
            Dim rows As DataFrameRow() = mat.expression
            Dim n As Integer = rows.Length
            Dim m As Integer = cells.Length
            Dim times As Double() = New Double(m - 1) {}
            Dim available As Boolean() = New Boolean(m - 1) {}
            Dim dropped As Integer = 0

            For j As Integer = 0 To m - 1
                Dim time As Double

                If pseudotime.TryGetValue(cells(j), time) Then
                    times(j) = time
                    available(j) = True
                Else
                    dropped += 1
                End If
            Next

            Dim tMin As Double = Double.MaxValue
            Dim tMax As Double = Double.MinValue

            For j As Integer = 0 To m - 1
                If available(j) Then
                    tMin = std.Min(tMin, times(j))
                    tMax = std.Max(tMax, times(j))
                End If
            Next

            If tMax <= tMin Then
                Throw New InvalidDataException("All of the pseudotime values are identical, the time axis can not be constructed!")
            End If

            Dim sums As Double()() = New Double(n - 1)() {}
            Dim counts As Double() = New Double(bins - 1) {}
            Dim timeSums As Double() = New Double(bins - 1) {}
            Dim data As Double()() = New Double(n - 1)() {}

            For i As Integer = 0 To n - 1
                sums(i) = New Double(bins - 1) {}
                data(i) = rows(i).experiments
            Next

            For j As Integer = 0 To m - 1
                If Not available(j) Then
                    Continue For
                End If

                Dim slot As Integer = CInt(std.Floor((times(j) - tMin) / (tMax - tMin) * bins))

                If slot < 0 Then
                    slot = 0
                ElseIf slot >= bins Then
                    slot = bins - 1
                End If

                counts(slot) += 1
                timeSums(slot) += times(j)

                For i As Integer = 0 To n - 1
                    Dim values As Double() = data(i)

                    If values Is Nothing OrElse j >= values.Length Then
                        Continue For
                    End If

                    sums(i)(slot) += values(j)
                Next
            Next

            Dim empty As New List(Of Integer)
            Dim values2 As Double()() = New Double(n - 1)() {}
            Dim axis As Double() = New Double(bins - 1) {}

            For k As Integer = 0 To bins - 1
                If counts(k) > 0 Then
                    axis(k) = timeSums(k) / counts(k)
                Else
                    Call empty.Add(k)
                End If
            Next

            For i As Integer = 0 To n - 1
                values2(i) = New Double(bins - 1) {}

                For k As Integer = 0 To bins - 1
                    If counts(k) > 0 Then
                        values2(i)(k) = sums(i)(k) / counts(k)
                    End If
                Next
            Next

            ' 空箱使用相邻的有效箱做线性插值
            Call fillEmptyBins(values2, counts, axis, n, bins)

            Dim expression As DataFrameRow() = New DataFrameRow(n - 1) {}

            For i As Integer = 0 To n - 1
                expression(i) = New DataFrameRow With {
                    .geneID = rows(i).geneID,
                    .experiments = values2(i)
                }
            Next

            Dim binned As New BinnedExpression With {
                .Matrix = New Matrix With {
                    .tag = If(mat.tag, "pseudotime"),
                    .sampleID = axis _
                        .Select(Function(t) t.ToString("G6", CultureInfo.InvariantCulture)) _
                        .ToArray,
                    .expression = expression
                },
                .TimePoints = axis,
                .DroppedCells = dropped,
                .EmptyBins = empty.Count
            }

            Return binned
        End Function

        ''' <summary>
        ''' 直接从 pseudotime 排序之后的 CSV 分箱，**不把整个表达矩阵载入内存**。
        ''' </summary>
        ''' <param name="matrixFile">行是基因、列是细胞的表达矩阵 CSV</param>
        ''' <param name="pseudotime">细胞到 pseudotime 的映射</param>
        ''' <param name="bins">分箱数量</param>
        ''' <param name="genes">需要保留的基因（<c>Nothing</c> 表示全部基因）</param>
        ''' <param name="quiet"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 与 <see cref="Bin"/> 的区别只在于数据来源：这里逐行流式扫描 CSV，
        ''' 常驻内存只有 <c>基因数 × 箱数</c>（18k × 80 ≈ 11.5 MB），
        ''' 而不是 18179 × 30409 的双精度矩阵（约 4.4 GB）。
        ''' 分箱、空箱插值与时间轴的构造逻辑与 <see cref="Bin"/> 完全一致。
        ''' </remarks>
        Public Function BinFromCsv(matrixFile As String,
                                   pseudotime As Dictionary(Of String, Double),
                                   bins As Integer,
                                   Optional genes As IEnumerable(Of String) = Nothing,
                                   Optional quiet As Boolean = False) As BinnedExpression
            If String.IsNullOrEmpty(matrixFile) OrElse Not File.Exists(matrixFile) Then
                Throw New FileNotFoundException($"the expression matrix '{matrixFile}' does not exist!", matrixFile)
            End If
            If pseudotime Is Nothing OrElse pseudotime.Count = 0 Then
                Throw New ArgumentException("The cell to pseudotime mapping is empty!", NameOf(pseudotime))
            End If
            If bins < 4 Then
                bins = 4
            End If

            Dim index As HashSet(Of String) = Nothing

            If Not genes Is Nothing Then
                index = New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

                For Each gene As String In genes.SafeQuery()
                    If Not String.IsNullOrEmpty(gene) Then
                        Call index.Add(gene.Trim())
                    End If
                Next
            End If

            Dim watch As Stopwatch = Stopwatch.StartNew()
            Dim cells As String() = Nothing
            Dim times As Double() = Nothing
            Dim slots As Integer() = Nothing
            Dim counts As Double() = Nothing
            Dim timeSums As Double() = Nothing
            Dim tMin As Double = Double.MaxValue
            Dim tMax As Double = Double.MinValue
            Dim dropped As Integer = 0
            Dim rows As New List(Of String())
            Dim sums As New List(Of Double())
            Dim line As String
            Dim m As Integer = 0

            Using reader As New StreamReader(matrixFile, Encoding.UTF8, detectEncodingFromByteOrderMarks:=True, bufferSize:=1 << 20)
                Dim header As String = reader.ReadLine()

                If String.IsNullOrEmpty(header) Then
                    Throw New InvalidDataException($"The expression matrix '{matrixFile}' is empty!")
                End If

                cells = header.Split(","c)
                m = cells.Length - 1

                If m < 4 Then
                    Throw New InvalidDataException("The expression matrix does not contain enough cell columns!")
                End If

                times = New Double(m - 1) {}
                slots = New Integer(m - 1) {}
                counts = New Double(bins - 1) {}
                timeSums = New Double(bins - 1) {}

                Dim available As Boolean() = New Boolean(m - 1) {}

                For j As Integer = 1 To m
                    Dim time As Double

                    If pseudotime.TryGetValue(cells(j).Trim(), time) Then
                        times(j - 1) = time
                        available(j - 1) = True

                        tMin = std.Min(tMin, time)
                        tMax = std.Max(tMax, time)
                    Else
                        dropped += 1
                    End If
                Next

                If tMax <= tMin Then
                    Throw New InvalidDataException("All of the pseudotime values are identical, the time axis can not be constructed!")
                End If

                For j As Integer = 0 To m - 1
                    If Not available(j) Then
                        slots(j) = -1
                        Continue For
                    End If

                    Dim slot As Integer = CInt(std.Floor((times(j) - tMin) / (tMax - tMin) * bins))

                    If slot < 0 Then
                        slot = 0
                    ElseIf slot >= bins Then
                        slot = bins - 1
                    End If

                    slots(j) = slot
                    counts(slot) += 1
                    timeSums(slot) += times(j)
                Next

                Do
                    line = reader.ReadLine()

                    If line Is Nothing Then
                        Exit Do
                    End If
                    If line.Length = 0 Then
                        Continue Do
                    End If

                    Dim comma As Integer = line.IndexOf(","c)

                    If comma <= 0 Then
                        Continue Do
                    End If

                    Dim gene As String = line.Substring(0, comma).Trim()

                    If Not index Is Nothing AndAlso Not index.Contains(gene) Then
                        Continue Do
                    End If

                    Dim values As Double() = New Double(bins - 1) {}
                    Dim fields As String() = line.Split(","c)
                    Dim value As Double

                    ' fields(0) 是基因编号，其后依次是每一个细胞
                    For j As Integer = 1 To std.Min(fields.Length, m + 1) - 1
                        Dim slot As Integer = slots(j - 1)

                        If slot < 0 Then
                            Continue For
                        End If
                        If Double.TryParse(fields(j), NumberStyles.Float, CultureInfo.InvariantCulture, value) Then
                            values(slot) += value
                        End If
                    Next

                    Call rows.Add({gene})
                    Call sums.Add(values)
                Loop
            End Using

            watch.Stop()

            If Not quiet Then
                Call Console.WriteLine($"[INFO] streamed {rows.Count} gene(s) from '{Path.GetFileName(matrixFile)}' in {watch.Elapsed.TotalSeconds.ToString("F1")}s")
            End If

            Dim n As Integer = rows.Count

            If n = 0 Then
                Return New BinnedExpression With {
                    .Matrix = New Matrix With {
                        .tag = "pseudotime",
                        .sampleID = New String() {},
                        .expression = New DataFrameRow() {}
                    },
                    .TimePoints = New Double() {},
                    .DroppedCells = dropped,
                    .EmptyBins = bins
                }
            End If

            Dim values2 As Double()() = New Double(n - 1)() {}
            Dim axis As Double() = New Double(bins - 1) {}
            Dim empty As New List(Of Integer)

            For k As Integer = 0 To bins - 1
                If counts(k) > 0 Then
                    axis(k) = timeSums(k) / counts(k)
                Else
                    Call empty.Add(k)
                End If
            Next

            For i As Integer = 0 To n - 1
                values2(i) = New Double(bins - 1) {}

                For k As Integer = 0 To bins - 1
                    If counts(k) > 0 Then
                        values2(i)(k) = sums(i)(k) / counts(k)
                    End If
                Next
            Next

            Call fillEmptyBins(values2, counts, axis, n, bins)

            Dim expression As DataFrameRow() = New DataFrameRow(n - 1) {}

            For i As Integer = 0 To n - 1
                expression(i) = New DataFrameRow With {
                    .geneID = rows(i)(0),
                    .experiments = values2(i)
                }
            Next

            Return New BinnedExpression With {
                .Matrix = New Matrix With {
                    .tag = If("pseudotime", "pseudotime"),
                    .sampleID = axis _
                        .Select(Function(t) t.ToString("G6", CultureInfo.InvariantCulture)) _
                        .ToArray,
                    .expression = expression
                },
                .TimePoints = axis,
                .DroppedCells = dropped,
                .EmptyBins = empty.Count
            }
        End Function

        ''' <summary>
        ''' 依据表达量的动态范围挑选出最活跃的若干个基因
        ''' </summary>
        ''' <param name="binned"></param>
        ''' <param name="top">需要保留的基因数量；零或者负数表示全部保留</param>
        ''' <param name="mustKeep">
        ''' 必须被保留的基因（例如敲降实验的目标基因），无论其动态范围大小；
        ''' 剩余的额度再按动态范围从高到低填充
        ''' </param>
        ''' <returns></returns>
        Public Function SelectDynamic(binned As BinnedExpression,
                                      Optional top As Integer = 30,
                                      Optional mustKeep As String() = Nothing) As BinnedExpression
            Dim rows As DataFrameRow() = binned.Matrix.expression

            If top <= 0 OrElse top >= rows.Length Then
                Return binned
            End If

            Dim keep As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

            For Each gene As String In mustKeep.SafeQuery()
                Dim key As String = If(gene, "").Trim()

                If key.Length > 0 Then
                    Call keep.Add(key)
                End If
            Next

            Dim scores As New List(Of Tuple(Of Double, DataFrameRow, Boolean))

            For Each row As DataFrameRow In rows
                Dim values As Double() = row.experiments

                If values.IsNullOrEmpty Then
                    Continue For
                End If

                Dim lo As Double = values.Min
                Dim hi As Double = values.Max
                Dim mean As Double = values.Average
                Dim dynamics As Double = ifFinite((hi - lo) / std.Max(std.Abs(mean), 1.0E-6))

                Call scores.Add(New Tuple(Of Double, DataFrameRow, Boolean)(dynamics, row, keep.Contains(row.geneID)))
            Next

            ' 必选基因先占用额度，其余按动态范围由高到低补齐
            Dim mandatory As Integer = scores.Where(Function(a) a.Item3).Count()
            Dim quota As Integer = std.Max(top - mandatory, 0)
            Dim selected As DataFrameRow() = scores _
                .Where(Function(a) a.Item3) _
                .Concat(scores _
                    .Where(Function(a) Not a.Item3) _
                    .OrderByDescending(Function(a) a.Item1) _
                    .Take(quota)) _
                .Select(Function(a) a.Item2) _
                .ToArray

            Return New BinnedExpression With {
                .Matrix = New Matrix With {
                    .tag = binned.Matrix.tag,
                    .sampleID = binned.Matrix.sampleID,
                    .expression = selected
                },
                .TimePoints = binned.TimePoints,
                .DroppedCells = binned.DroppedCells,
                .EmptyBins = binned.EmptyBins
            }
        End Function

        ''' <summary>
        ''' 把分箱之后的矩阵写为 csv 文件（首行为时间轴）
        ''' </summary>
        ''' <param name="binned"></param>
        ''' <param name="filePath"></param>
        Public Sub SaveCsv(binned As BinnedExpression, filePath As String)
            Dim dir As String = Path.GetDirectoryName(Path.GetFullPath(filePath))

            If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
                Call Directory.CreateDirectory(dir)
            End If

            Dim sb As New StringBuilder()

            Call sb.Append("gene")

            For Each t As Double In binned.TimePoints
                Call sb.Append(","c)
                Call sb.Append(t.ToString("G6", CultureInfo.InvariantCulture))
            Next

            Call sb.AppendLine()

            For Each row As DataFrameRow In binned.Matrix.expression
                Call sb.Append(row.geneID)

                For Each x As Double In row.experiments.SafeQuery()
                    Call sb.Append(","c)
                    Call sb.Append(x.ToString("G6", CultureInfo.InvariantCulture))
                Next

                Call sb.AppendLine()
            Next

            Call File.WriteAllText(filePath, sb.ToString(), New UTF8Encoding(False))
        End Sub

        ''' <summary>
        ''' 用相邻的有效箱对空箱做线性插值；时间轴同时保证严格单调递增
        ''' </summary>
        Private Sub fillEmptyBins(values As Double()(),
                                  counts As Double(),
                                  axis As Double(),
                                  n As Integer,
                                  bins As Integer)
            Dim previous As Integer = -1

            For k As Integer = 0 To bins - 1
                If counts(k) > 0 Then
                    If previous >= 0 AndAlso k - previous > 1 Then
                        Dim left As Integer = previous
                        Dim right As Integer = k

                        For j As Integer = left + 1 To right - 1
                            Dim ratio As Double = CDbl(j - left) / (right - left)

                            axis(j) = axis(left) + (axis(right) - axis(left)) * ratio

                            For i As Integer = 0 To n - 1
                                values(i)(j) = values(i)(left) + (values(i)(right) - values(i)(left)) * ratio
                            Next
                        Next
                    End If

                    previous = k
                End If
            Next

            ' 首尾的空箱直接复制最近的有效箱
            Dim firstValid As Integer = -1
            Dim lastValid As Integer = -1

            For k As Integer = 0 To bins - 1
                If counts(k) > 0 Then
                    If firstValid < 0 Then
                        firstValid = k
                    End If

                    lastValid = k
                End If
            Next

            If firstValid < 0 Then
                Return
            End If

            For k As Integer = 0 To firstValid - 1
                axis(k) = axis(firstValid)
                Call copyColumn(values, n, firstValid, k)
            Next
            For k As Integer = lastValid + 1 To bins - 1
                axis(k) = axis(lastValid)
                Call copyColumn(values, n, lastValid, k)
            Next

            ' 保证时间轴严格单调递增（相邻时间点至少相差一个很小的增量）
            Dim span As Double = std.Max(axis(bins - 1) - axis(0), 1.0E-9)
            Dim epsilon As Double = span * 1.0E-6 / bins

            For k As Integer = 1 To bins - 1
                If axis(k) <= axis(k - 1) Then
                    axis(k) = axis(k - 1) + epsilon
                End If
            Next
        End Sub

        Private Sub copyColumn(values As Double()(), n As Integer, source As Integer, target As Integer)
            For i As Integer = 0 To n - 1
                values(i)(target) = values(i)(source)
            Next
        End Sub

        Private Function ifFinite(x As Double) As Double
            If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then
                Return 0
            Else
                Return x
            End If
        End Function
    End Module
End Namespace
