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
        ''' 依据表达量的动态范围挑选出最活跃的若干个基因
        ''' </summary>
        ''' <param name="binned"></param>
        ''' <param name="top">需要保留的基因数量；零或者负数表示全部保留</param>
        ''' <returns></returns>
        Public Function SelectDynamic(binned As BinnedExpression, Optional top As Integer = 30) As BinnedExpression
            Dim rows As DataFrameRow() = binned.Matrix.expression

            If top <= 0 OrElse top >= rows.Length Then
                Return binned
            End If

            Dim scores As New List(Of Tuple(Of Double, DataFrameRow))

            For Each row As DataFrameRow In rows
                Dim values As Double() = row.experiments

                If values.IsNullOrEmpty Then
                    Continue For
                End If

                Dim lo As Double = values.Min
                Dim hi As Double = values.Max
                Dim mean As Double = values.Average
                Dim dynamics As Double = ifFinite((hi - lo) / std.Max(std.Abs(mean), 1.0E-6))

                Call scores.Add(New Tuple(Of Double, DataFrameRow)(dynamics, row))
            Next

            Dim selected As DataFrameRow() = scores _
                .OrderByDescending(Function(a) a.Item1) _
                .Take(top) _
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
