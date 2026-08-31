Imports System.Globalization
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

''' <summary>
''' 一张"行=分子特征、列=样本（时间点）"的表达矩阵，附带时间轴与归一化统计量
''' </summary>
''' <remarks>
''' 直接由 <see cref="Matrix.LoadData"/> 从 CSV 载入；<see cref="Data"/> 的形状为
''' (特征数 × 样本数)，<see cref="ToSequence"/> 会转置为模型需要的 (时间 × 特征)。
''' </remarks>
Public Class TimeSeriesMatrix

    ''' <summary>行名（代谢物 id 或酶/反应 id）</summary>
    Public Property FeatureIds As String()

    ''' <summary>原始样本列名</summary>
    Public Property SampleNames As String()

    ''' <summary>由列名解析出的时间轴（解析失败时退化为列序号）</summary>
    Public Property Times As Double()

    ''' <summary>数据矩阵，形状 (特征数 × 样本数)</summary>
    Public Property Data As Tensor

    ''' <summary>归一化方式：raw / log1p+zscore / minmax</summary>
    Public Property Normalization As String = "raw"

    ''' <summary>z-score 的均值（按行）</summary>
    Public Property RowMeans As Double()

    ''' <summary>z-score 的标准差（按行）</summary>
    Public Property RowStds As Double()

    ''' <summary>min-max 的下界（按行）</summary>
    Public Property RowMins As Double()

    ''' <summary>min-max 的跨度（按行）</summary>
    Public Property RowSpans As Double()

    Public ReadOnly Property FeatureCount As Integer
        Get
            Return FeatureIds.Length
        End Get
    End Property

    Public ReadOnly Property SampleCount As Integer
        Get
            Return SampleNames.Length
        End Get
    End Property

    ''' <summary>取某一行的完整时间序列</summary>
    Public Function RowOf(featureId As String) As Double()
        Dim i = Array.IndexOf(FeatureIds, featureId)

        If i < 0 Then
            Throw New ArgumentException($"矩阵中不存在特征：{featureId}")
        End If

        Dim out(SampleCount - 1) As Double

        For k = 0 To SampleCount - 1
            out(k) = Data(i, k)
        Next

        Return out
    End Function

    ''' <summary>转置为模型所需的 (时间 × 特征) 序列</summary>
    Public Function ToSequence() As Tensor
        Dim seq = New Tensor(SampleCount, FeatureCount)

        For i = 0 To FeatureCount - 1
            For k = 0 To SampleCount - 1
                seq(k, i) = Data(i, k)
            Next
        Next

        Return seq
    End Function

    ''' <summary>按给定 id 列表重排/筛选行，缺失的行填 0</summary>
    Public Function Reorder(ids As String()) As Tensor
        Dim seq = New Tensor(SampleCount, ids.Length)

        For i = 0 To ids.Length - 1
            Dim src = Array.IndexOf(FeatureIds, ids(i))

            If src < 0 Then
                Continue For
            End If

            For k = 0 To SampleCount - 1
                seq(k, i) = Data(src, k)
            Next
        Next

        Return seq
    End Function

    ''' <summary>还原到归一化之前的尺度</summary>
    Public Function InverseRow(featureId As String, values As Double()) As Double()
        Dim i = Array.IndexOf(FeatureIds, featureId)
        If i < 0 OrElse Normalization = "raw" Then Return values

        Dim out(values.Length - 1) As Double

        For k = 0 To values.Length - 1
            Select Case Normalization
                Case "log1p+zscore"
                    Dim logged = values(k) * RowStds(i) + RowMeans(i)
                    out(k) = std.Exp(logged) - 1.0
                Case "minmax"
                    out(k) = values(k) * RowSpans(i) + RowMins(i)
                Case Else
                    out(k) = values(k)
            End Select
        Next

        Return out
    End Function

End Class

''' <summary>
''' 代谢网络时序数据的载入与归一化
''' </summary>
Public Module MetabolicDataIO

    ''' <summary>
    ''' 载入 CSV 表达矩阵（行=分子，列=样本/时间点）
    ''' </summary>
    ''' <param name="path">CSV 路径</param>
    ''' <param name="timeHeader">
    ''' 是否把首行首列当作表头；列名会被解析为时间轴（例如 T0.5、0.5、time_12）
    ''' </param>
    Public Function LoadCsv(path As String, Optional timeHeader As Boolean = True) As TimeSeriesMatrix
        Dim raw As Matrix = Matrix.LoadData(path)
        Dim featureIds = raw.expression.Select(Function(g) g.geneID).ToArray()
        Dim sampleNames = raw.sampleID
        Dim features = featureIds.Length
        Dim samples = sampleNames.Length
        Dim data = New Tensor(features, samples)

        For i = 0 To features - 1
            Dim row = raw.expression(i).experiments

            For k = 0 To samples - 1
                data(i, k) = row(k)
            Next
        Next

        Dim times = ParseTimes(sampleNames)

        Return New TimeSeriesMatrix With {
            .FeatureIds = featureIds,
            .SampleNames = sampleNames,
            .Times = times,
            .Data = data,
            .Normalization = "raw"
        }
    End Function

    ''' <summary>
    ''' 从样本列名中解析时间；解析失败时退化为列序号（保证时间轴仍然单调递增）
    ''' </summary>
    Public Function ParseTimes(sampleNames As String()) As Double()
        Dim times(sampleNames.Length - 1) As Double

        For k = 0 To sampleNames.Length - 1
            Dim m = Regex.Match(sampleNames(k), "-?\d+(\.\d+)?")

            If m.Success Then
                times(k) = Double.Parse(m.Value, CultureInfo.InvariantCulture)
            Else
                times(k) = k
            End If
        Next

        Return times
    End Function

    ''' <summary>
    ''' log1p + z-score 归一化（按行）。
    ''' 代谢物浓度跨越多个数量级，readme 建议先 log-transform 再做 z-score。
    ''' </summary>
    Public Function LogZScoreNormalize(source As TimeSeriesMatrix) As TimeSeriesMatrix
        Dim features = source.FeatureCount
        Dim samples = source.SampleCount
        Dim out = New Tensor(features, samples)
        Dim means(features - 1) As Double
        Dim stds(features - 1) As Double

        For i = 0 To features - 1
            ' log1p
            Dim logged(samples - 1) As Double

            For k = 0 To samples - 1
                logged(k) = std.Log(1.0 + std.Max(0.0, source.Data(i, k)))
            Next

            Dim mean As Double = 0.0
            For k = 0 To samples - 1
                mean += logged(k)
            Next
            mean /= samples

            Dim variance As Double = 0.0
            For k = 0 To samples - 1
                Dim d = logged(k) - mean
                variance += d * d
            Next
            variance /= std.Max(1, samples)

            Dim sd = std.Sqrt(variance)

            If sd < 0.0000000001 Then
                sd = 1.0
            End If

            means(i) = mean
            stds(i) = sd

            For k = 0 To samples - 1
                out(i, k) = (logged(k) - mean) / sd
            Next
        Next

        Return New TimeSeriesMatrix With {
            .FeatureIds = source.FeatureIds,
            .SampleNames = source.SampleNames,
            .Times = source.Times,
            .Data = out,
            .Normalization = "log1p+zscore",
            .RowMeans = means,
            .RowStds = stds
        }
    End Function

    ''' <summary>
    ''' 逐行 min-max 归一化到 [0,1]。
    ''' 酶表达量用这种方式，因为通量读取头 <c>v = e ⊙ σ(·)</c> 直接把 e 当作容量上限。
    ''' </summary>
    Public Function MinMaxNormalize(source As TimeSeriesMatrix) As TimeSeriesMatrix
        Dim features = source.FeatureCount
        Dim samples = source.SampleCount
        Dim out = New Tensor(features, samples)
        Dim mins(features - 1) As Double
        Dim spans(features - 1) As Double

        For i = 0 To features - 1
            Dim lo As Double = Double.PositiveInfinity
            Dim hi As Double = Double.NegativeInfinity

            For k = 0 To samples - 1
                Dim v = source.Data(i, k)
                If v < lo Then lo = v
                If v > hi Then hi = v
            Next

            Dim span = hi - lo
            If span < 0.0000000001 Then span = 1.0

            mins(i) = lo
            spans(i) = span

            For k = 0 To samples - 1
                out(i, k) = (source.Data(i, k) - lo) / span
            Next
        Next

        Return New TimeSeriesMatrix With {
            .FeatureIds = source.FeatureIds,
            .SampleNames = source.SampleNames,
            .Times = source.Times,
            .Data = out,
            .Normalization = "minmax",
            .RowMins = mins,
            .RowSpans = spans
        }
    End Function

    ''' <summary>
    ''' 写出 (行=特征, 列=样本) 的 CSV，与 <see cref="LoadCsv"/> 的格式约定保持一致
    ''' </summary>
    Public Sub SaveCsv(path As String, featureIds As String(), sampleNames As String(), data As Tensor)
        Dim sb As New StringBuilder()

        sb.Append("ID").Append(","c)
        sb.AppendLine(String.Join(",", sampleNames))

        For i = 0 To featureIds.Length - 1
            sb.Append(featureIds(i))

            For k = 0 To sampleNames.Length - 1
                sb.Append(","c).Append(data(i, k).ToString("G6", CultureInfo.InvariantCulture))
            Next

            sb.AppendLine()
        Next

        Dim dir = Path.GetDirectoryName(path)

        If Not String.IsNullOrEmpty(dir) AndAlso Not Directory.Exists(dir) Then
            Directory.CreateDirectory(dir)
        End If

        File.WriteAllText(path, sb.ToString())
    End Sub

End Module
