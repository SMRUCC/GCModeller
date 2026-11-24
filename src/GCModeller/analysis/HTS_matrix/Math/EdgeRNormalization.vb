Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

''' <summary>
''' edgeR 的 TMM（trimmed mean of M-values）归一化
''' </summary>
''' <remarks>
''' <para>
''' 这里的实现是对 <c>edgeR::calcNormFactors(method="TMM")</c> 的逐行移植，
''' 目的是让结果能够与 R 端对拍（见 <c>test/R/hts_matrix_reference.R</c>）。
''' </para>
''' <para>
''' **旧实现存在的数学错误**：
''' </para>
''' <list type="number">
''' <item>
''' 计算 M 值之前没有按文库大小缩放。edgeR 的定义是
''' <c>M = log2((obs/nO) / (ref/nR))</c>、<c>A = (log2(obs/nO) + log2(ref/nR))/2</c>；
''' 旧实现直接用原始 counts 计算，于是 M 里混入了文库深度比例，
''' 归一化因子实际上变成了"文库比例 × 组成偏差"。当这个因子再被用于
''' <c>libSize × factor</c> 的有效文库大小时，文库深度就被除了两次。
''' </item>
''' <item>
''' 权重用错了：edgeR 用 <c>v = (nO−obs)/nO/obs + (nR−ref)/nR/ref</c> 的倒数做权重，
''' 旧实现用的是 <c>(obs×ref)/(obs+ref)</c>。
''' </item>
''' <item>
''' 裁剪方式不一致：edgeR 对 M 与 A 都用**秩**做双侧裁剪
''' （<c>loL = floor(n×logratioTrim)+1</c>、<c>hiL = n+1−loL</c>，
''' A 侧同理用 <c>sumTrim</c>），旧实现用分位数插值、并且把 A 只裁剪了 2.5%。
''' </item>
''' <item>
''' 缺少 <c>max|M| &lt; 1e-6</c> 时直接返回 1 的短路，也缺少参考样本的正确回传
''' （旧实现总是返回 0）。
''' </item>
''' </list>
''' </remarks>
Public Module EdgeRTMMNormalization

    ''' <summary>
    ''' 计算 edgeR 的 TMM 归一化因子
    ''' </summary>
    ''' <param name="countData">原始 counts 矩阵（基因在行，样本在列）</param>
    ''' <param name="trimFractionM">
    ''' M 值两侧各自裁剪的比例，对应 edgeR 的 <c>logratioTrim</c>（默认 0.3）
    ''' </param>
    ''' <param name="trimFractionA">
    ''' A 值两侧各自裁剪的比例，对应 edgeR 的 <c>sumTrim</c>（默认 0.05）
    ''' </param>
    ''' <param name="doWeighting">是否使用按方差倒数的加权平均（edgeR 默认开启）</param>
    ''' <param name="Acutoff">A 值的下限过滤器（edgeR 默认 -1e10，即不过滤）</param>
    ''' <returns>
    ''' <c>normFactors</c>：几何平均为 1 的归一化因子；
    ''' <c>referenceSampleIndex</c>：被选作参考的样本列号
    ''' </returns>
    <Extension>
    Public Function CalcTMMFactors(countData As Matrix,
                                   Optional trimFractionM As Double = 0.3,
                                   Optional trimFractionA As Double = 0.05,
                                   Optional doWeighting As Boolean = True,
                                   Optional Acutoff As Double = -1.0E+10) As (normFactors As Double(), referenceSampleIndex As Integer)

        Dim nSample As Integer = countData.sampleID.Length
        Dim nGene As Integer = countData.expression.Length

        If nSample = 0 OrElse nGene = 0 Then
            Return (New Double() {}, 0)
        End If

        Dim columns As Double()() = New Double(nSample - 1)() {}
        Dim libSizes(nSample - 1) As Double
        Dim hasPositive(nGene - 1) As Boolean

        For j As Integer = 0 To nSample - 1
            Dim column As Double() = New Double(nGene - 1) {}

            For i As Integer = 0 To nGene - 1
                Dim v As Double = countData.expression(i).experiments(j)

                column(i) = v
                libSizes(j) += v

                If v > 0 Then
                    hasPositive(i) = True
                End If
            Next

            columns(j) = column
        Next

        ' edgeR 会先把"在所有样本里都是 0"的基因整行删除
        Dim keep As Integer() = Enumerable.Range(0, nGene) _
            .Where(Function(i) hasPositive(i)) _
            .ToArray

        If keep.Length = 0 OrElse nSample < 2 Then
            Return (Enumerable.Repeat(1.0, nSample).ToArray, 0)
        End If

        ' 参考样本：按 upper-quartile 因子挑选（edgeR 的默认策略）
        Dim f75 As Double() = calcFactorQuantile(columns, libSizes, keep, 0.75)
        Dim gm75 As Double = geometricMean(f75)

        If gm75 > 0 Then
            For j As Integer = 0 To f75.Length - 1
                f75(j) /= gm75
            Next
        End If

        Dim referenceIndex As Integer

        If f75.MedianOf < 1.0E-20 Then
            referenceIndex = 0

            Dim best As Double = -1

            For j As Integer = 0 To nSample - 1
                Dim s As Double = 0

                For Each i As Integer In keep
                    s += std.Sqrt(columns(j)(i))
                Next

                If s > best Then
                    best = s
                    referenceIndex = j
                End If
            Next
        Else
            Dim mean75 As Double = f75.Average

            referenceIndex = 0

            Dim best As Double = Double.MaxValue

            For j As Integer = 0 To f75.Length - 1
                Dim diff As Double = std.Abs(f75(j) - mean75)

                If diff < best Then
                    best = diff
                    referenceIndex = j
                End If
            Next
        End If

        Dim factors(nSample - 1) As Double

        For j As Integer = 0 To nSample - 1
            If j = referenceIndex Then
                factors(j) = 1.0
            Else
                factors(j) = calcFactorTMM(
                    columns(j),
                    columns(referenceIndex),
                    libSizes(j),
                    libSizes(referenceIndex),
                    trimFractionM,
                    trimFractionA,
                    doWeighting,
                    Acutoff
                )
            End If
        Next

        ' edgeR：f <- f/exp(mean(log(f)))
        Dim geometric As Double = geometricMean(factors)

        If geometric > 0 Then
            For j As Integer = 0 To factors.Length - 1
                factors(j) = factors(j) / geometric
            Next
        End If

        Return (factors, referenceIndex)
    End Function

    ''' <summary>
    ''' 移植 <c>edgeR:::.calcFactorTMM</c>：单个样本相对于参考样本的缩放因子
    ''' </summary>
    Private Function calcFactorTMM(obs As Double(),
                                   ref As Double(),
                                   nO As Double,
                                   nR As Double,
                                   logratioTrim As Double,
                                   sumTrim As Double,
                                   doWeighting As Boolean,
                                   Acutoff As Double) As Double

        Dim nGene As Integer = obs.Length
        Dim logR As New List(Of Double)(nGene)
        Dim absE As New List(Of Double)(nGene)
        Dim variance As New List(Of Double)(nGene)

        For i As Integer = 0 To nGene - 1
            Dim o As Double = obs(i) / nO
            Dim r As Double = ref(i) / nR
            Dim lr As Double = log2(o) - log2(r)
            Dim ae As Double = (log2(o) + log2(r)) / 2.0
            Dim v As Double = (nO - obs(i)) / nO / obs(i) + (nR - ref(i)) / nR / ref(i)

            If Not Double.IsNaN(lr) AndAlso Not Double.IsInfinity(lr) AndAlso
               Not Double.IsNaN(ae) AndAlso Not Double.IsInfinity(ae) AndAlso
               ae > Acutoff Then

                Call logR.Add(lr)
                Call absE.Add(ae)
                Call variance.Add(v)
            End If
        Next

        If logR.Count = 0 Then
            Return 1.0
        End If

        Dim maxAbsR As Double = 0

        For Each x As Double In logR
            If std.Abs(x) > maxAbsR Then
                maxAbsR = std.Abs(x)
            End If
        Next

        If maxAbsR < 0.000001 Then
            Return 1.0
        End If

        Dim n As Integer = logR.Count
        Dim rankR As Double() = RankStatistics.AverageRanks(logR.ToArray)
        Dim rankA As Double() = RankStatistics.AverageRanks(absE.ToArray)

        Dim loL As Integer = CInt(std.Floor(n * logratioTrim)) + 1
        Dim hiL As Integer = n + 1 - loL
        Dim loS As Integer = CInt(std.Floor(n * sumTrim)) + 1
        Dim hiS As Integer = n + 1 - loS

        Dim sumLogR As Double = 0
        Dim sumWeight As Double = 0
        Dim sumR As Double = 0
        Dim count As Integer = 0

        For k As Integer = 0 To n - 1
            If rankR(k) >= loL AndAlso rankR(k) <= hiL AndAlso rankA(k) >= loS AndAlso rankA(k) <= hiS Then
                If doWeighting Then
                    Dim weight As Double = 1.0 / variance(k)

                    sumLogR += logR(k) * weight
                    sumWeight += weight
                Else
                    sumR += logR(k)
                End If

                count += 1
            End If
        Next

        Dim f As Double

        If doWeighting Then
            f = If(sumWeight > 0, sumLogR / sumWeight, 0)
        Else
            f = If(count > 0, sumR / count, 0)
        End If

        If Double.IsNaN(f) Then
            f = 0
        End If

        Return std.Pow(2.0, f)
    End Function

    ''' <summary>
    ''' 移植 <c>edgeR:::.calcFactorQuantile</c>：上四分位因子，用于挑选参考样本
    ''' </summary>
    Private Function calcFactorQuantile(columns As Double()(),
                                        libSizes As Double(),
                                        keep As Integer(),
                                        p As Double) As Double()

        Dim nSample As Integer = columns.Length
        Dim f As Double() = New Double(nSample - 1) {}

        For j As Integer = 0 To nSample - 1
            Dim values As New List(Of Double)(keep.Length)

            For Each i As Integer In keep
                Dim y As Double = columns(j)(i) / libSizes(j)

                If y > 0 Then
                    Call values.Add(y)
                End If
            Next

            f(j) = RankStatistics.QuantileType7(values.ToArray.SortedAscending, p)
        Next

        Return f
    End Function

    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Private Function log2(x As Double) As Double
        Return std.Log(x) / std.Log(2.0)
    End Function

    Private Function geometricMean(values As Double()) As Double
        Dim sum As Double = 0
        Dim n As Integer = 0

        For Each x As Double In values
            If x > 0 Then
                sum += std.Log(x)
                n += 1
            End If
        Next

        If n = 0 Then
            Return 0
        Else
            Return std.Exp(sum / n)
        End If
    End Function

    ''' <summary>
    ''' 应用 TMM 归一化因子生成归一化后的表达矩阵
    ''' </summary>
    <Extension>
    Public Function ApplyTMMNormalization(countData As Matrix, Optional useEffectiveLibSize As Boolean = True, Optional trimFractionM As Double = 0.3, Optional trimFractionA As Double = 0.05) As Matrix
        Dim sampleCount As Integer = countData.sampleID.Length
        Dim geneCount As Integer = countData.expression.Length

        ' 获取 TMM 归一化因子
        Dim tmmResult = countData.CalcTMMFactors(trimFractionA:=trimFractionA, trimFractionM:=trimFractionM)
        Dim normFactors As Double() = tmmResult.normFactors

        ' 计算原始库大小
        Dim libSizes(sampleCount - 1) As Double
        For j As Integer = 0 To sampleCount - 1
            Dim total As Double = 0
            For i As Integer = 0 To geneCount - 1
                total += countData.expression(i).experiments(j)
            Next
            libSizes(j) = total
        Next

        ' 创建归一化后的表达矩阵
        Dim normExpression(geneCount - 1) As DataFrameRow

        For i As Integer = 0 To geneCount - 1
            Dim normExperiments(sampleCount - 1) As Double

            For j As Integer = 0 To sampleCount - 1
                Dim rawCount As Double = countData.expression(i).experiments(j)
                Dim normalizedValue As Double

                If useEffectiveLibSize Then
                    ' edgeR 风格：cpm(y, normalized.lib.sizes = TRUE)
                    '   有效库大小 = 原始库大小 × 归一化因子
                    Dim effectiveLibSize As Double = libSizes(j) * normFactors(j)

                    If effectiveLibSize > 0 Then
                        normalizedValue = rawCount * 1_000_000.0 / effectiveLibSize
                    Else
                        normalizedValue = 0
                    End If
                Else
                    ' 简单归一化：直接除以归一化因子（类似 DESeq2 的输出格式）
                    If normFactors(j) > 0 Then
                        normalizedValue = rawCount / normFactors(j)
                    Else
                        normalizedValue = 0
                    End If
                End If

                normExperiments(j) = normalizedValue
            Next

            normExpression(i) = New DataFrameRow With {
                .geneID = countData.expression(i).geneID,
                .experiments = normExperiments
            }
        Next

        ' 构建并返回归一化后的新矩阵
        Return New Matrix With {
            .tag = If(useEffectiveLibSize, $"TMM_CPM_Norm({countData.tag})", $"TMM_Norm({countData.tag})"),
            .sampleID = countData.sampleID,
            .expression = normExpression
        }
    End Function

    ''' <summary>
    ''' 计算 edgeR 风格的归一化表达矩阵（CPM 格式）
    ''' 这是最接近 edgeR 标准输出的方法
    ''' </summary>
    <Extension>
    Public Function EdgeRTMMNormalize(countData As Matrix, Optional trimFractionM As Double = 0.3, Optional trimFractionA As Double = 0.05) As Matrix
        ' 使用有效库大小计算 CPM
        Return countData.ApplyTMMNormalization(useEffectiveLibSize:=True, trimFractionA:=trimFractionA, trimFractionM:=trimFractionM)
    End Function

    ''' <summary>
    ''' 计算简单的 TMM 归一化矩阵（直接除以归一化因子）
    ''' 这更类似于 DESeq2 的输出格式
    ''' </summary>
    <Extension>
    Public Function EdgeRTMMNormalizeSimple(countData As Matrix, Optional trimFractionM As Double = 0.3, Optional trimFractionA As Double = 0.05) As Matrix
        ' 直接除以归一化因子
        Return countData.ApplyTMMNormalization(useEffectiveLibSize:=False, trimFractionA:=trimFractionA, trimFractionM:=trimFractionM)
    End Function
End Module
