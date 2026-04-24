Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports std = System.Math

Public Module EdgeRTMMNormalization

    ''' <summary>
    ''' 计算 edgeR 的 TMM 归一化因子
    ''' </summary>
    <Extension>
    Public Function CalcTMMFactors(countData As Matrix, Optional trimFractionM As Double = 0.3, Optional trimFractionA As Double = 0.05) As (normFactors As Double(), referenceSampleIndex As Integer)
        Dim sampleCount As Integer = countData.sampleID.Length
        Dim geneCount As Integer = countData.expression.Length

        ' 1. 计算每个样本的总计数（库大小）
        Dim libSizes(sampleCount - 1) As Double
        For j As Integer = 0 To sampleCount - 1
            Dim total As Double = 0
            For i As Integer = 0 To geneCount - 1
                total += countData.expression(i).experiments(j)
            Next
            libSizes(j) = total
        Next

        ' 2. 选择参考样本（库大小中位数）
        ' 在 edgeR 中，通常选择库大小中位数的样本作为参考
        Dim medianLibSize As Double = libSizes.Median()
        Dim referenceIndex As Integer = 0
        Dim minDiff As Double = Double.MaxValue

        For j As Integer = 0 To sampleCount - 1
            Dim diff As Double = std.Abs(libSizes(j) - medianLibSize)
            If diff < minDiff Then
                minDiff = diff
                referenceIndex = j
            End If
        Next

        ' 3. 计算归一化因子
        ' 计算每个样本相对于参考样本的归一化因子
        Dim normFactors(sampleCount - 1) As Double
        normFactors(referenceIndex) = 1.0

        ' 为每个样本计算TMM因子
        For j As Integer = 0 To sampleCount - 1
            If j = referenceIndex Then Continue For

            Dim mValues As New List(Of Double)
            Dim aValues As New List(Of Double)
            Dim weights As New List(Of Double)

            ' 遍历所有基因
            For i As Integer = 0 To geneCount - 1
                Dim countRef As Double = countData.expression(i).experiments(referenceIndex)
                Dim countSample As Double = countData.expression(i).experiments(j)

                ' 跳过在两样本中均为零的基因
                If countRef = 0 AndAlso countSample = 0 Then Continue For

                ' 添加伪计数（仅用于对数计算）
                Dim pseudoRef As Double = countRef + 0.5
                Dim pseudoSample As Double = countSample + 0.5

                ' 计算M值和A值
                Dim m As Double = std.Log(pseudoSample / pseudoRef) / std.Log(2)
                Dim a As Double = 0.5 * (std.Log(pseudoSample) / std.Log(2) + std.Log(pseudoRef) / std.Log(2))

                ' 权重计算（使用原始计数）
                ' edgeR权重公式: w = (n1 * n2) / (n1 + n2)
                Dim weight As Double = 0
                If countSample + countRef > 0 Then
                    weight = (countSample * countRef) / (countSample + countRef)
                End If

                mValues.Add(m)
                aValues.Add(a)
                weights.Add(weight)
            Next

            If mValues.Count = 0 Then
                normFactors(j) = 1.0
                Continue For
            End If

            ' 4. 修剪：基于分位数

            ' 对A值，edgeR通常只移除高表达的5%（防止高表达基因主导）
            ' 这里我们实现为：移除A值最高的5%和最低的5%
            Dim sortedM = mValues.OrderBy(Function(x) x).ToList()
            Dim sortedA = aValues.OrderBy(Function(x) x).ToList()

            ' 计算分位数索引（type=7分位数，与R的quantile()默认相同）
            Dim lowMIndex As Double = (mValues.Count - 1) * (trimFractionM / 2)
            Dim highMIndex As Double = (mValues.Count - 1) * (1 - trimFractionM / 2)

            Dim lowAIndex As Double = (aValues.Count - 1) * (trimFractionA / 2)
            Dim highAIndex As Double = (aValues.Count - 1) * (1 - trimFractionA / 2)

            ' 线性插值计算分位数值
            Dim lowMVal As Double = Quantile(sortedM, lowMIndex)
            Dim highMVal As Double = Quantile(sortedM, highMIndex)
            Dim lowAVal As Double = Quantile(sortedA, lowAIndex)
            Dim highAVal As Double = Quantile(sortedA, highAIndex)

            ' 5. 计算加权平均M值
            Dim sumWeightedM As Double = 0
            Dim sumWeights As Double = 0
            Dim countIncluded As Integer = 0

            For k As Integer = 0 To mValues.Count - 1
                ' 检查是否在修剪范围内
                If mValues(k) >= lowMVal AndAlso mValues(k) <= highMVal AndAlso
               aValues(k) >= lowAVal AndAlso aValues(k) <= highAVal Then

                    Dim weightVal As Double = weights(k)
                    ' 跳过权重为0的基因
                    If weightVal > 0 Then
                        sumWeightedM += mValues(k) * weightVal
                        sumWeights += weightVal
                        countIncluded += 1
                    End If
                End If
            Next

            Dim weightedMeanM As Double = 0
            If sumWeights > 0 AndAlso countIncluded > 0 Then
                weightedMeanM = sumWeightedM / sumWeights
            End If

            ' 6. 计算归一化因子
            normFactors(j) = std.Pow(2, weightedMeanM)
        Next

        ' 7. 调整归一化因子，使其几何平均为1
        Return NormalizeFactors(normFactors)
    End Function

    ' 辅助函数：计算分位数（线性插值，type=7）
    Private Function Quantile(sortedData As List(Of Double), index As Double) As Double
        Dim idx1 As Integer = CInt(std.Floor(index))
        Dim idx2 As Integer = CInt(std.Ceiling(index))

        If idx1 = idx2 Then
            Return sortedData(idx1)
        Else
            Dim frac As Double = index - idx1
            Return sortedData(idx1) * (1 - frac) + sortedData(idx2) * frac
        End If
    End Function

    ' 辅助函数：归一化因子调整
    Private Function NormalizeFactors(factors As Double()) As (normFactors As Double(), referenceIndex As Integer)
        Dim logSum As Double = 0
        Dim validCount As Integer = 0

        For Each factor In factors
            If factor > 0 Then
                logSum += std.Log(factor)
                validCount += 1
            End If
        Next

        If validCount > 0 Then
            Dim geoMean As Double = std.Exp(logSum / validCount)
            Dim normalizedFactors(factors.Length - 1) As Double

            For i As Integer = 0 To factors.Length - 1
                normalizedFactors(i) = If(geoMean > 0, factors(i) / geoMean, 1.0)
            Next

            Return (normalizedFactors, 0) ' 注意：这里referenceIndex需要外部传入
        Else
            ' 所有因子都为0或无效，返回1.0
            For i As Integer = 0 To factors.Length - 1
                factors(i) = 1.0
            Next
            Return (factors, 0)
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
                    ' edgeR 风格：使用有效库大小（原始库大小 * 归一化因子）
                    Dim effectiveLibSize As Double = libSizes(j) * normFactors(j)

                    ' 计算 CPM (Counts Per Million)
                    ' 注意：这里乘以 1e6 是为了得到每百万计数
                    If effectiveLibSize > 0 Then
                        normalizedValue = rawCount * 1_000_000.0 / effectiveLibSize
                    Else
                        normalizedValue = 0
                    End If
                Else
                    ' 简单归一化：直接除以归一化因子
                    ' 类似于 DESeq2 的做法，但不是 edgeR 的推荐方式
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