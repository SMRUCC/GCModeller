Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace GRN

    ''' <summary>
    ''' The time series view of a gene expression matrix.(基因表达矩阵的时序视图)
    ''' </summary>
    ''' <remarks>
    ''' 本类型负责把 <see cref="Matrix"/> 之中的数据转换为可以被 S-system 逆问题所使用的形式：
    ''' 行是基因、列是时间点，并且会预先计算出对数表达量矩阵以提升拟合时的求值性能。
    ''' </remarks>
    Public Class ExpressionTimeSeries

        ''' <summary>
        ''' 已经规整化之后的基因编号（可以直接被数学表达式引擎所解析）
        ''' </summary>
        ''' <returns></returns>
        Public Property GeneNames As String()
        ''' <summary>
        ''' 原始的基因编号，用于结果标注
        ''' </summary>
        ''' <returns></returns>
        Public Property OriginalNames As String()
        ''' <summary>
        ''' 时间轴（每个采样点所对应的时间值）
        ''' </summary>
        ''' <returns></returns>
        Public Property TimePoints As Double()
        ''' <summary>
        ''' 表达量矩阵，第一维是基因、第二维是时间点
        ''' </summary>
        ''' <returns></returns>
        Public Property X As Double()()
        ''' <summary>
        ''' 表达量的自然对数矩阵，第一维是基因、第二维是时间点
        ''' </summary>
        ''' <returns></returns>
        Public Property LnX As Double()()
        ''' <summary>
        ''' 每一个基因在预处理时被平移的常数 δ（幂律要求表达量为正数）：
        ''' <c>X = log2(max(raw,0) + pseudo) + δ</c>。
        ''' 把外部数据（例如 pseudobulk context）映射到模型空间时必须使用同一个 δ
        ''' </summary>
        ''' <returns></returns>
        Public Property Offsets As Double()
        ''' <summary>
        ''' 建立该时序视图时所使用的归一化方法与伪计数
        ''' </summary>
        ''' <returns></returns>
        Public Property Normalize As NormalizationMethods = NormalizationMethods.Log2
        ''' <summary>
        ''' 建立该时序视图时所使用的伪计数
        ''' </summary>
        ''' <returns></returns>
        Public Property PseudoCount As Double = 1.0R
        ''' <summary>
        ''' 在预处理过程之中所产生的提示信息
        ''' </summary>
        ''' <returns></returns>
        Public Property Warnings As New List(Of String)

        ''' <summary>
        ''' 基因的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property nGenes As Integer
            Get
                If X Is Nothing Then Return 0 Else Return X.Length
            End Get
        End Property

        ''' <summary>
        ''' 时间点的数量
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property nTimePoints As Integer
            Get
                If TimePoints Is Nothing Then Return 0 Else Return TimePoints.Length
            End Get
        End Property

        ''' <summary>
        ''' 时间轴的终点值
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property TimeEnd As Double
            Get
                If TimePoints.IsNullOrEmpty Then Return 0 Else Return TimePoints(TimePoints.Length - 1)
            End Get
        End Property

        ''' <summary>
        ''' 查询目标基因在矩阵之中的行号，不存在的时候返回 -1
        ''' </summary>
        ''' <param name="geneName">规整化之后的基因编号</param>
        ''' <returns></returns>
        Public Function GeneIndex(geneName As String) As Integer
            If GeneNames.IsNullOrEmpty OrElse String.IsNullOrEmpty(geneName) Then
                Return -1
            End If

            For i As Integer = 0 To GeneNames.Length - 1
                If String.Equals(GeneNames(i), geneName, StringComparison.OrdinalIgnoreCase) Then
                    Return i
                End If
            Next

            Return -1
        End Function

        Public Overrides Function ToString() As String
            Return $"{nGenes} genes x {nTimePoints} time points [0, {TimeEnd}]"
        End Function

        ''' <summary>
        ''' 规整化基因编号：数学表达式引擎要求标识符必须以字母开头，
        ''' 并且连字符之类的字符会被当作减号，故这里需要做替换
        ''' </summary>
        ''' <param name="id"></param>
        ''' <param name="index">当编号为空的时候用于生成缺省编号的行号</param>
        ''' <returns></returns>
        Public Shared Function SanitizeGeneId(id As String, Optional index As Integer = 0) As String
            If String.IsNullOrWhiteSpace(id) Then
                Return "G" & index
            End If

            Dim sb As New StringBuilder(id.Length + 4)

            For Each c As Char In id.Trim
                If Char.IsLetterOrDigit(c) OrElse c = "_"c Then
                    Call sb.Append(c)
                Else
                    Call sb.Append("_"c)
                End If
            Next

            Dim name As String = sb.ToString()

            If name.Length = 0 Then
                Return "G" & index
            End If
            If Not Char.IsLetter(name.First) Then
                name = "G_" & name
            End If

            Return name
        End Function

        ''' <summary>
        ''' 从基因表达矩阵之中构建出时序视图
        ''' </summary>
        ''' <param name="matrix"></param>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Shared Function FromMatrix(matrix As Matrix, Optional options As FittingOptions = Nothing) As ExpressionTimeSeries
            If matrix Is Nothing Then
                Throw New ArgumentNullException(NameOf(matrix))
            End If
            If options Is Nothing Then
                options = New FittingOptions()
            End If

            Dim rows As DataFrameRow() = matrix.expression.SafeQuery.ToArray

            If rows.Length = 0 Then
                Throw New ArgumentException("The input expression matrix is empty!")
            End If

            Dim m As Integer = rows _
                .Select(Function(r) If(r.experiments Is Nothing, 0, r.experiments.Length)) _
                .Max

            If m <= 0 Then
                Throw New ArgumentException("The input expression matrix does not have any sample column!")
            End If

            Dim originals As String() = rows _
                .Select(Function(r, i) If(String.IsNullOrEmpty(r.geneID), "gene_" & i, r.geneID)) _
                .ToArray
            Dim names As String() = originals _
                .Select(Function(id, i) SanitizeGeneId(id, i)) _
                .ToArray

            ' 规整化之后可能会出现重名，这里需要保证编号的唯一性
            names = uniqueNames(names)

            Dim raw As Double()() = rows _
                .Select(Function(r) padTo(r.experiments, m)) _
                .ToArray

            Dim series As New ExpressionTimeSeries With {
                .GeneNames = names,
                .OriginalNames = originals,
                .Normalize = options.Normalize,
                .PseudoCount = options.PseudoCount
            }

            ' 1. 缺失值插补
            For i As Integer = 0 To raw.Length - 1
                raw(i) = fillMissing(raw(i), series.Warnings, names(i))
            Next

            ' 2. 归一化
            Select Case options.Normalize
                Case NormalizationMethods.Log2
                    raw = log2Transform(raw, options.PseudoCount)
                Case NormalizationMethods.RowZScore
                    raw = rowZScore(raw)
                Case NormalizationMethods.Quantile
                    raw = quantileNormalize(raw)
            End Select

            ' 3. 幂律要求所有的表达量都是严格的正数
            Dim offsets As Double() = Nothing

            raw = ensurePositive(raw, options.PseudoCount, series.Warnings, names, offsets)
            series.Offsets = offsets

            ' 4. 时间轴
            series.TimePoints = resolveTimePoints(matrix.sampleID, m, options, series.Warnings)
            series.X = raw
            series.LnX = raw _
                .Select(Function(row) row.Select(Function(x) std.Log(std.Max(x, 1.0E-12))).ToArray) _
                .ToArray

            If series.nTimePoints < 4 Then
                Call series.Warnings.Add($"There are only {series.nTimePoints} time points in the expression matrix, at least 4 points are required by the slope estimation.")
            End If

            Return series
        End Function

        Private Shared Function uniqueNames(names As String()) As String()
            Dim seen As New System.Collections.Generic.Dictionary(Of String, Integer)
            Dim out As String() = New String(names.Length - 1) {}

            For i As Integer = 0 To names.Length - 1
                Dim name As String = names(i)
                Dim n As Integer = 0

                If seen.TryGetValue(name, n) Then
                    n += 1
                    seen(name) = n
                    name = name & "_" & n
                Else
                    Call seen.Add(name, 0)
                End If

                out(i) = name
            Next

            Return out
        End Function

        Private Shared Function padTo(vec As Double(), m As Integer) As Double()
            If vec Is Nothing Then
                Return New Double(m - 1) {}
            End If
            If vec.Length = m Then
                Return CType(vec.Clone(), Double())
            End If

            Dim out As Double() = New Double(m - 1) {}

            Call System.Array.Copy(vec, out, std.Min(vec.Length, m))

            Return out
        End Function

        ''' <summary>
        ''' 对缺失值（NaN/Infinity）做线性插补
        ''' </summary>
        Private Shared Function fillMissing(vec As Double(), warns As System.Collections.Generic.List(Of String), gene As String) As Double()
            Dim miss As Integer = vec _
                .Count(Function(x) Double.IsNaN(x) OrElse Double.IsInfinity(x))

            If miss = 0 Then
                Return vec
            End If

            Dim out As Double() = CType(vec.Clone(), Double())
            Dim last As Integer = -1
            Dim lastValue As Double = 0

            For i As Integer = 0 To out.Length - 1
                If Double.IsNaN(out(i)) OrElse Double.IsInfinity(out(i)) Then
                    Continue For
                End If

                If last >= 0 AndAlso i - last > 1 Then
                    Dim d As Double = (out(i) - lastValue) / (i - last)

                    For k As Integer = last + 1 To i - 1
                        out(k) = lastValue + d * (k - last)
                    Next
                End If

                last = i
                lastValue = out(i)
            Next

            ' 首尾的缺失值使用最近的有效值补齐
            For i As Integer = 0 To out.Length - 1
                If Double.IsNaN(out(i)) OrElse Double.IsInfinity(out(i)) Then
                    out(i) = If(lastValue = 0, 1.0R, lastValue)
                Else
                    Exit For
                End If
            Next
            For i As Integer = out.Length - 1 To 0 Step -1
                If Double.IsNaN(out(i)) OrElse Double.IsInfinity(out(i)) Then
                    out(i) = If(lastValue = 0, 1.0R, lastValue)
                Else
                    Exit For
                End If
            Next

            Call warns.Add($"Imputed {miss} missing value(s) of the gene '{gene}' by linear interpolation.")

            Return out
        End Function

        Private Shared Function log2Transform(raw As Double()(), pseudo As Double) As Double()()
            Dim ln2 As Double = std.Log(2)

            Return raw _
                .Select(Function(row) row _
                    .Select(Function(x) std.Log(std.Max(x, 0) + pseudo) / ln2) _
                    .ToArray) _
                .ToArray
        End Function

        Private Shared Function rowZScore(raw As Double()()) As Double()()
            Return raw _
                .Select(Function(row)
                            Dim mean As Double = row.Average
                            Dim sd As Double = std.Sqrt(row.Sum(Function(x) (x - mean) ^ 2) / row.Length)

                            If sd < 1.0E-12 Then
                                sd = 1
                            End If

                            Return row.Select(Function(x) (x - mean) / sd).ToArray
                        End Function) _
                .ToArray
        End Function

        ''' <summary>
        ''' 分位数归一化：把每一个样本列映射到所有样本的平均分布之上
        ''' </summary>
        Private Shared Function quantileNormalize(raw As Double()()) As Double()()
            Dim n As Integer = raw.Length
            Dim m As Integer = raw(0).Length
            Dim sorted As Double()() = raw _
                .Select(Function(row)
                            Dim v As Double() = CType(row.Clone(), Double())
                            Call System.Array.Sort(v)
                            Return v
                        End Function) _
                .ToArray
            Dim mean As Double() = New Double(m - 1) {}

            For k As Integer = 0 To m - 1
                Dim sum As Double = 0

                For i As Integer = 0 To n - 1
                    sum += sorted(i)(k)
                Next

                mean(k) = sum / n
            Next

            Return raw _
                .Select(Function(row)
                            Dim order As Integer() = Enumerable _
                                .Range(0, m) _
                                .OrderBy(Function(i) row(i)) _
                                .ToArray
                            Dim out As Double() = New Double(m - 1) {}

                            For rank As Integer = 0 To order.Length - 1
                                out(order(rank)) = mean(rank)
                            Next

                            Return out
                        End Function) _
                .ToArray
        End Function

        Private Shared Function ensurePositive(raw As Double()(),
                                               pseudo As Double,
                                               warns As System.Collections.Generic.List(Of String),
                                               names As String(),
                                               ByRef offsets As Double()) As Double()()

            Dim shifted As Integer = 0
            Dim deltas As Double() = New Double(raw.Length - 1) {}

            Dim out As Double()() = raw _
                .Select(Function(row, i)
                            Dim min As Double = row.Min

                            If min > 0 Then
                                Return row
                            End If

                            shifted += 1

                            ' 平移到正数区间：这是一个单调仿射变换，不会改变基因自身的动态趋势
                            Dim delta As Double = -min + pseudo

                            deltas(i) = delta

                            Return row.Select(Function(x) x + delta).ToArray
                        End Function) _
                .ToArray

            offsets = deltas

            If shifted > 0 Then
                Call warns.Add($"Shifted {shifted} gene expression row(s) into the positive value range, the power law term requires all values to be positive.")
            End If

            Return out
        End Function

        Private Shared Function resolveTimePoints(sampleID As String(),
                                                  m As Integer,
                                                  options As FittingOptions,
                                                  warns As System.Collections.Generic.List(Of String)) As Double()

            If Not options.TimePoints Is Nothing AndAlso options.TimePoints.Length = m Then
                Return CType(options.TimePoints.Clone(), Double())
            End If

            If Not sampleID.IsNullOrEmpty AndAlso sampleID.Length = m Then
                Dim t As Double() = New Double(m - 1) {}
                Dim ok As Boolean = True

                For i As Integer = 0 To m - 1
                    If Not Double.TryParse(sampleID(i), t(i)) Then
                        ok = False
                        Exit For
                    End If
                Next

                If ok Then
                    Dim monotonic As Boolean = True

                    For i As Integer = 1 To m - 1
                        If t(i) <= t(i - 1) Then
                            monotonic = False
                            Exit For
                        End If
                    Next

                    If monotonic Then
                        Return t
                    End If

                    Call warns.Add("The sample id of the expression matrix is not a monotonic increasing time axis, using the sample index instead.")
                Else
                    Call warns.Add("The sample id of the expression matrix can not be parsed as a numeric time axis, using the sample index instead.")
                End If
            End If

            Return Enumerable.Range(0, m).Select(Function(i) CDbl(i)).ToArray
        End Function
    End Class
End Namespace
