#Region "Microsoft.VisualBasic::9bd884ee7b758c7300ddb29ee5b7d401, analysis\HTS_matrix\Math\ExpressionScale.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 55
    '    Code Lines: 46 (83.64%)
    ' Comment Lines: 2 (3.64%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 7 (12.73%)
    '     File Size: 1.97 KB


    ' Module ExpressionScale
    ' 
    '     Function: LogScale, RelativeScale
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports SMRUCC.genomics.GCModeller.Workbench.ExperimentDesigner
Imports std = System.Math
Imports std_vec = Microsoft.VisualBasic.Math.LinearAlgebra.Vector

Public Module ExpressionScale

    <Extension>
    Public Function RelativeScale(gene As DataFrameRow, Optional median As Boolean = False) As DataFrameRow
        Dim factor As Double = If(median, gene.experiments.Median, gene.experiments.Max)

        If median AndAlso factor = 0.0 Then
            Dim minmax As DoubleRange = gene.experiments

            ' try to avoid divid zero
            If minmax.Length = 0 Then
                ' all zero
                Return New DataFrameRow With {
                    .geneID = gene.geneID,
                    .experiments = gene.experiments.ToArray
                }
            Else
                factor = minmax.Max / 2
            End If
        End If

        Return New DataFrameRow With {
            .geneID = gene.geneID,
            .experiments = New std_vec(gene.experiments) / factor
        }
    End Function

    <Extension>
    Public Function LogScale(exp As DataFrameRow, base As Double) As DataFrameRow
        Dim min As Double = exp.experiments _
            .Where(Function(v) v > 0 AndAlso Not v.IsNaNImaginary) _
            .DefaultIfEmpty(0) _
            .Min

        Return New DataFrameRow With {
            .geneID = exp.geneID,
            .experiments = exp.experiments _
                .Select(Function(v)
                            If v <= 0 Then
                                Return 0
                            Else
                                Return std.Log(v + 1 - min, newBase:=base)
                            End If
                        End Function) _
                .ToArray
        }
    End Function

    ''' <summary>
    ''' 依据表达值的分布特征，从转录组表达矩阵之中拆分出每一个品种(line/genotype)的基因组
    ''' 所实际携带的基因集合。
    ''' </summary>
    ''' <param name="exp">基因在行，样本在列的表达矩阵。</param>
    ''' <param name="sampleinfo">
    ''' 样本的元数据信息，其中<paramref name="group"/>所指定的元数据字段(默认为"line")的
    ''' 值即为品种编号。
    ''' </param>
    ''' <param name="group">
    ''' 用于进行样本分组的元数据键名，如果这个参数为空的话，则直接使用
    ''' <see cref="SampleInfo.sample_info"/>进行分组。
    ''' </param>
    ''' <param name="rawCounts">
    ''' 输入数据的模式开关：
    ''' 
    ''' + True 表示输入的矩阵为原始的count矩阵，函数会首先进行文库深度校正
    '''   (基于品种间共有基因的median-of-ratios大小因子)；
    ''' + False 表示输入的矩阵已经完成了归一化(TPM/FPKM/CPM)，不做任何校正；
    ''' + 空值表示自动检测输入矩阵是否为原始的count矩阵。
    ''' </param>
    ''' <param name="logBase">进行表达值分布变换的时候所使用的对数底数，默认为2</param>
    ''' <param name="kMAD">
    ''' 判定阈值相对于低表达本底(即基因组之中不存在的基因所产生的零或者近零信号)的
    ''' 标准差倍数，默认为3个标准差之外。
    ''' </param>
    ''' <param name="presenceQuantile">
    ''' 品种内部的生物学重复的汇总分位数，默认取中位数；这个参数等价于存在性判据
    ''' "基因至少要在多少比例的重复样本之中被检测到"(0为最严格：所有重复都要检出；
    ''' 1为最宽松：只要有一个重复检出即可)。
    ''' </param>
    ''' <param name="minAbsentFraction">
    ''' 双峰检验之中的最小缺失组分占比：如果低表达组分或者高表达组分的占比低于这个值，
    ''' 则认为当前的品种的表达值分布为单峰分布(例如携带有两个母本全部基因的8倍体子代)，
    ''' 该品种之下的所有基因都会被保留下来，不做切分。
    ''' </param>
    ''' <param name="minGap">
    ''' 双峰检验之中的最小峰间分离度(gap = (mu_high - mu_low) / max(sd_low, sd_high))，
    ''' 低于这个值的时候认为无法从分布之中分离出缺失组分。
    ''' </param>
    ''' <param name="absFloor">判定为存在所要求的原始表达量下限，默认为0</param>
    ''' <param name="verbose">是否输出每一个品种的阈值诊断信息？</param>
    ''' <param name="thresholds">
    ''' 通过这个引用参数可以选择性的获取得到每一个品种所对应的判定阈值(对数空间之中的阈值)
    ''' </param>
    ''' <returns>
    ''' 键名为品种编号，键值为该品种的基因组所携带的基因编号集合
    ''' </returns>
    ''' 
    <Extension>
    Public Function ExpressionGroups(exp As Matrix,
                                     sampleinfo As IReadOnlyCollection(Of SampleInfo),
                                     Optional group As String = Nothing,
                                     Optional rawCounts As Boolean? = Nothing,
                                     Optional logBase As Double = 2,
                                     Optional kMAD As Double = 3,
                                     Optional presenceQuantile As Double = 0.5,
                                     Optional minAbsentFraction As Double = 0.01,
                                     Optional minGap As Double = 1.5,
                                     Optional absFloor As Double = 0,
                                     Optional verbose As Boolean = True,
                                     Optional ByRef thresholds As Dictionary(Of String, Double) = Nothing
                                    ) As Dictionary(Of String, String())

        Dim result As New Dictionary(Of String, String())

        If thresholds Is Nothing Then
            thresholds = New Dictionary(Of String, Double)
        End If

        If exp Is Nothing OrElse exp.expression Is Nothing OrElse exp.size = 0 Then
            Return result
        End If
        If exp.sampleID Is Nothing OrElse exp.sampleID.Length = 0 Then
            Return result
        End If
        If sampleinfo Is Nothing OrElse sampleinfo.Count = 0 Then
            Return result
        End If
        If logBase <= 1 OrElse Double.IsNaN(logBase) OrElse Double.IsInfinity(logBase) Then
            logBase = 2
        End If
        If presenceQuantile < 0 Then
            presenceQuantile = 0
        ElseIf presenceQuantile > 1 Then
            presenceQuantile = 1
        End If

        ' 第一步：建立样本编号到品种编号之间的映射关系
        Dim sampleTag As New Dictionary(Of String, String)
        Dim fallback As Integer = 0

        For Each s As SampleInfo In sampleinfo
            If s Is Nothing OrElse String.IsNullOrEmpty(s.ID) Then
                Continue For
            End If

            Dim tag As String = If(group Is Nothing, Nothing, s(group))

            If tag Is Nothing Then
                tag = s.sample_info
                fallback += 1
            End If
            If tag Is Nothing Then
                tag = s.ID
            End If

            sampleTag(s.ID) = tag
        Next

        If fallback > 0 AndAlso verbose AndAlso Not group Is Nothing Then
            Call $"{fallback} samples have no metadata tag '{group}', use the sample_info value as the variety tag instead.".warning
        End If

        ' 第二步：按照品种编号对表达矩阵的样本列进行分组
        Dim groups As New Dictionary(Of String, List(Of Integer))
        Dim missed As New List(Of String)

        For i As Integer = 0 To exp.sampleID.Length - 1
            Dim id As String = exp.sampleID(i)
            Dim tag As String = Nothing

            If Not sampleTag.TryGetValue(id, tag) OrElse tag Is Nothing Then
                Call missed.Add(id)
                Continue For
            End If

            If Not groups.ContainsKey(tag) Then
                Call groups.Add(tag, New List(Of Integer))
            End If

            Call groups(tag).Add(i)
        Next

        If missed.Count > 0 AndAlso verbose Then
            Call $"{missed.Count} samples in the expression matrix have no variety tag and were ignored: {missed.JoinBy(", ")}".warning
        End If

        ' 第三步：原始数据模式之下，先做文库深度校正
        Dim isCount As Boolean = If(rawCounts Is Nothing, IsCountMatrix(exp), rawCounts.Value)

        If isCount Then
            If verbose Then
                Call "the input matrix is treated as the raw count data, apply the library size normalization at first.".warning
            End If

            exp = NormalizeCounts(exp, groups)
        End If

        ' 第四步：对每一个品种独立的估计其低表达本底阈值，然后拆分出基因集合
        For Each line As KeyValuePair(Of String, List(Of Integer)) In groups
            Dim cols As Integer() = line.Value.ToArray
            Dim rows As Integer = exp.size
            Dim x As Double() = New Double(rows - 1) {}
            Dim raw As Double() = New Double(rows - 1) {}
            Dim genes As New List(Of String)(rows)
            Dim detected As Integer = 0

            For i As Integer = 0 To rows - 1
                Dim v As Double = ReplicateQuantile(exp.expression(i), cols, presenceQuantile)

                raw(i) = v
                x(i) = std.Log(v + 1, newBase:=logBase)

                If v > 0 Then
                    detected += 1
                End If
            Next

            Dim sorted As Double() = x.ToArray

            Call Array.Sort(sorted)

            Dim lowFraction As Double = 0
            Dim gap As Double = 0
            Dim cutoff As Double = EstimateCutoff(sorted, kMAD, minAbsentFraction, minGap, lowFraction, gap)

            thresholds(line.Key) = cutoff

            For i As Integer = 0 To rows - 1
                If x(i) > cutoff AndAlso raw(i) >= absFloor Then
                    Call genes.Add(exp.expression(i).geneID)
                End If
            Next

            result(line.Key) = genes.ToArray

            If verbose Then
                Dim cut As String

                If Double.IsNegativeInfinity(cutoff) Then
                    cut = "<none: unimodal>"
                ElseIf Double.IsPositiveInfinity(cutoff) Then
                    cut = "<all zero>"
                Else
                    cut = $"{cutoff.ToString("F4")}(log{logBase}) / {std.Pow(logBase, cutoff) - 1} (raw)"
                End If

                Call $"line '{line.Key}': {cols.Length} samples, cutoff = {cut}, detected = {detected}/{rows} (min={sorted(0).ToString("F3")}, median={sorted(CInt(rows / 2)).ToString("F3")}, max={sorted(rows - 1).ToString("F3")}), {genes.Count}/{rows} genes were kept.".warning
            End If
        Next

        Return result
    End Function

    ''' <summary>
    ''' 对原始的count矩阵进行文库深度校正
    ''' </summary>
    ''' <param name="exp">原始的reads count矩阵</param>
    ''' <param name="groups">样本列按照品种编号的分组结果</param>
    ''' <returns>完成了文库深度校正之后的表达矩阵</returns>
    ''' 
    ''' <remarks>
    ''' 由于不同的品种之间的基因组基因组成是不一样的，所以在这里不能够直接使用总计数
    ''' (或者CPM)进行归一化：携带的基因数量更多的品种(例如8倍体的子代)其总计数天然就
    ''' 会更高，这会引入系统性的偏差。
    ''' 
    ''' 在这里使用的是"在所有的品种之中都有表达"的基因作为内参基因集(内参基因集不会受到
    ''' 品种之间的基因组成差异的影响)，然后再按照DESeq2的median-of-ratios方法计算出
    ''' 每一个样本的大小因子。
    ''' </remarks>
    Private Function NormalizeCounts(exp As Matrix, groups As Dictionary(Of String, List(Of Integer))) As Matrix
        Dim nSamples As Integer = exp.sampleID.Length
        Dim nGenes As Integer = exp.size
        Dim core As New List(Of Integer)

        ' 第一步：挑选出在所有的品种之中都有表达的内参基因
        For i As Integer = 0 To nGenes - 1
            Dim v As Double() = exp.expression(i).experiments
            Dim inAllLines As Boolean = True

            For Each line As KeyValuePair(Of String, List(Of Integer)) In groups
                Dim detected As Boolean = False

                For Each j As Integer In line.Value
                    If v(j) > 0 Then
                        detected = True
                        Exit For
                    End If
                Next

                If Not detected Then
                    inAllLines = False
                    Exit For
                End If
            Next

            If inAllLines Then
                Call core.Add(i)
            End If
        Next

        If core.Count < 50 Then
            ' 品种之间的共有基因太少，退化为在绝大多数的样本之中都有表达的基因
            Dim minSamples As Integer = CInt(std.Ceiling(0.8 * nSamples))

            Call core.Clear()

            For i As Integer = 0 To nGenes - 1
                Dim v As Double() = exp.expression(i).experiments
                Dim n As Integer = 0

                For j As Integer = 0 To nSamples - 1
                    If v(j) > 0 Then
                        n += 1
                    End If
                Next

                If n >= minSamples Then
                    Call core.Add(i)
                End If
            Next
        End If
        If core.Count < 50 Then
            Call core.Clear()

            For i As Integer = 0 To nGenes - 1
                Call core.Add(i)
            Next
        End If

        ' 第二步：计算每一个内参基因的几何平均数
        Dim geoMean As Double() = New Double(core.Count - 1) {}

        For k As Integer = 0 To core.Count - 1
            Dim v As Double() = exp.expression(core(k)).experiments
            Dim sumLog As Double = 0
            Dim n As Integer = 0

            For j As Integer = 0 To nSamples - 1
                If v(j) > 0 Then
                    sumLog += std.Log(v(j))
                    n += 1
                End If
            Next

            geoMean(k) = If(n > 0, std.Exp(sumLog / n), 1)
        Next

        ' 第三步：median-of-ratios计算每一个样本的大小因子
        Dim sizeFactors As Double() = New Double(nSamples - 1) {}

        For j As Integer = 0 To nSamples - 1
            Dim ratios As New List(Of Double)(core.Count)

            For k As Integer = 0 To core.Count - 1
                Dim v As Double = exp.expression(core(k)).experiments(j)

                ' 只使用有表达的样本参与计算，避免dropout所产生的零值
                ' 将大小因子拉低为零
                If v > 0 Then
                    Call ratios.Add(v / geoMean(k))
                End If
            Next

            sizeFactors(j) = If(ratios.Count > 0, ratios.Median, 1)

            If sizeFactors(j) <= 0 OrElse Double.IsNaN(sizeFactors(j)) OrElse Double.IsInfinity(sizeFactors(j)) Then
                sizeFactors(j) = 1
            End If
        Next

        Dim rows As DataFrameRow() = New DataFrameRow(nGenes - 1) {}

        For i As Integer = 0 To nGenes - 1
            Dim v As Double() = exp.expression(i).experiments
            Dim nv As Double() = New Double(nSamples - 1) {}

            For j As Integer = 0 To nSamples - 1
                nv(j) = v(j) / sizeFactors(j)
            Next

            rows(i) = New DataFrameRow With {
                .geneID = exp.expression(i).geneID,
                .experiments = nv
            }
        Next

        Return New Matrix With {
            .sampleID = exp.sampleID,
            .tag = $"depthNorm({exp.tag})",
            .expression = rows
        }
    End Function

    ''' <summary>
    ''' 取一个基因在指定的一组生物学重复样本之中的分位数汇总值
    ''' </summary>
    Private Function ReplicateQuantile(gene As DataFrameRow, cols As Integer(), p As Double) As Double
        Dim n As Integer = cols.Length
        Dim v As Double() = New Double(n - 1) {}

        For i As Integer = 0 To n - 1
            Dim x As Double = gene.experiments(cols(i))

            If Double.IsNaN(x) OrElse Double.IsInfinity(x) OrElse x < 0 Then
                x = 0
            End If

            v(i) = x
        Next

        If n > 1 Then
            Call Array.Sort(v)
        End If

        Dim k As Integer = CInt(std.Floor(p * n))

        If k < 0 Then
            k = 0
        ElseIf k > n - 1 Then
            k = n - 1
        End If

        Return v(k)
    End Function

    ''' <summary>
    ''' 自动检测输入的表达矩阵是否为原始的count矩阵
    ''' </summary>
    Private Function IsCountMatrix(exp As Matrix) As Boolean
        Dim maxVal As Double = 0
        Dim n As Integer = 0
        Dim steps As Integer = std.Max(1, exp.size \ 200)

        For i As Integer = 0 To exp.size - 1 Step steps
            Dim reps As Double() = exp.expression(i).experiments

            For j As Integer = 0 To reps.Length - 1
                Dim v As Double = reps(j)

                If Double.IsNaN(v) OrElse Double.IsInfinity(v) Then
                    Continue For
                End If
                If v < 0 Then
                    ' 存在负值，说明数据已经被做过标准化处理
                    Return False
                End If
                If std.Abs(v - std.Round(v)) > 0.000001 * std.Max(1.0, v) Then
                    ' 存在小数，说明不是测序reads计数
                    Return False
                End If
                If v > maxVal Then
                    maxVal = v
                End If

                n += 1
            Next
        Next

        Return n > 0 AndAlso maxVal >= 10
    End Function

    ''' <summary>
    ''' 使用Otsu方法(最大化类间方差)从表达值的分布之中获取得到一个初始的分割阈值
    ''' </summary>
    ''' <param name="sorted">已经完成了升序排序的表达值向量(对数空间)</param>
    ''' <param name="nLow">输出低表达组分之中的基因数量</param>
    ''' <returns>低表达组分与高表达组分之间的分割阈值</returns>
    Private Function OtsuThreshold(sorted As Double(), ByRef nLow As Integer) As Double
        Dim n As Integer = sorted.Length
        Dim prefix As Double() = New Double(n) {}

        For i As Integer = 1 To n
            prefix(i) = prefix(i - 1) + sorted(i - 1)
        Next

        Dim total As Double = prefix(n)
        Dim lo As Integer = CInt(std.Floor(0.01 * (n - 1)))
        Dim hi As Integer = CInt(std.Ceiling(0.99 * (n - 1)))
        Dim steps As Integer = 199
        Dim bestVar As Double = -1
        Dim bestK As Integer = lo

        If hi <= lo Then
            hi = std.Min(n - 2, lo + 1)
        End If
        If hi < 0 Then
            hi = 0
        End If

        For s As Integer = 0 To steps
            Dim k As Integer = lo + CInt(std.Floor((hi - lo) * s / steps))

            If k < 0 Then
                k = 0
            ElseIf k > n - 2 Then
                k = n - 2
            End If

            Dim n0 As Integer = k + 1
            Dim n1 As Integer = n - n0

            If n0 < 1 OrElse n1 < 1 Then
                Continue For
            End If

            Dim mu0 As Double = prefix(n0) / n0
            Dim mu1 As Double = (total - prefix(n0)) / n1
            Dim w0 As Double = n0 / n
            Dim w1 As Double = 1 - w0
            Dim bcVar As Double = w0 * w1 * (mu0 - mu1) * (mu0 - mu1)

            If bcVar >= bestVar Then
                bestVar = bcVar
                bestK = k
            End If
        Next

        nLow = bestK + 1

        Return sorted(bestK)
    End Function

    ''' <summary>
    ''' 下半支的中位数绝对偏差(MAD)尺度估计
    ''' </summary>
    ''' <remarks>
    ''' 只使用小于等于中位数的那一部分数据来估计标准差，这样可以避免高表达基因的
    ''' 尾部数据对本底噪声分布的污染，从而保证阈值迭代精化过程的收敛性。
    ''' </remarks>
    Private Function LowHalfSigma(values As Double(), median As Double) As Double
        Dim dev As New List(Of Double)()

        For Each x As Double In values
            If x <= median Then
                Call dev.Add(median - x)
            End If
        Next

        If dev.Count = 0 Then
            Return 0
        End If

        Return 1.4826 * dev.Median
    End Function

    ''' <summary>
    ''' 估计低表达本底(基因组之中不存在的基因所产生的零信号或者近零信号)之上的判定阈值
    ''' </summary>
    ''' <param name="sorted">升序排序的表达值向量(对数空间)</param>
    ''' <param name="kMAD">相对于本底标准差的倍数</param>
    ''' <param name="minAbsentFraction">双峰检验之中的最小组分占比</param>
    ''' <param name="minGap">双峰检验之中的最小峰间分离度</param>
    ''' <param name="lowFraction">输出低表达组分的占比</param>
    ''' <param name="gap">输出两个峰之间的分离度</param>
    ''' <returns>
    ''' 对数空间之中的判定阈值；如果当前的分布为单峰分布(即该品种不存在缺失的基因，
    ''' 例如携带有两个母本全部基因的8倍体子代)，则会返回负无穷大，表示所有的基因都被保留。
    ''' </returns>
    Private Function EstimateCutoff(sorted As Double(),
                                    kMAD As Double,
                                    minAbsentFraction As Double,
                                    minGap As Double,
                                    ByRef lowFraction As Double,
                                    ByRef gap As Double) As Double

        Dim n As Integer = sorted.Length

        lowFraction = 0
        gap = 0

        If n < 4 Then
            Return Double.NegativeInfinity
        End If
        If sorted(n - 1) - sorted(0) <= 0.0000000001 Then
            ' 所有的表达值都是一样的
            Return If(sorted(0) > 0, Double.NegativeInfinity, Double.PositiveInfinity)
        End If

        Dim nLow As Integer = 0
        Dim tau As Double = OtsuThreshold(sorted, nLow)
        Dim low As Double() = sorted.Take(nLow).ToArray
        Dim high As Double() = sorted.Skip(nLow).ToArray

        lowFraction = nLow / n

        If low.Length < 2 OrElse high.Length < 2 Then
            Return Double.NegativeInfinity
        End If

        Dim mu0 As Double = low.Median
        Dim mu1 As Double = high.Median
        Dim sd0 As Double = LowHalfSigma(low, mu0)
        Dim sd1 As Double = LowHalfSigma(high, mu1)
        Dim scale As Double = std.Max(sd0, sd1)

        gap = If(scale > 0, (mu1 - mu0) / scale, 0)

        ' 单峰保护：组分占比过小，或者两个峰之间没有足够的分离度
        If lowFraction < minAbsentFraction OrElse lowFraction > 1 - minAbsentFraction Then
            Return Double.NegativeInfinity
        End If
        If gap < minGap Then
            Return Double.NegativeInfinity
        End If

        ' 使用中位数加上下半支MAD对本底分布的上界做迭代精化
        Dim cutoff As Double = tau

        For iter As Integer = 1 To 10
            Dim vals As Double() = sorted.Where(Function(x) x <= cutoff).ToArray

            If vals.Length < 2 Then
                Exit For
            End If

            Dim median As Double = vals.Median
            Dim sd As Double = LowHalfSigma(vals, median)
            Dim nextCut As Double = median + kMAD * sd

            If nextCut < 0 Then
                nextCut = median
            End If
            If std.Abs(nextCut - cutoff) < 0.0000000001 Then
                cutoff = nextCut
                Exit For
            End If

            cutoff = nextCut
        Next

        If Double.IsNaN(cutoff) Then
            Return tau
        End If

        Return cutoff
    End Function
End Module

