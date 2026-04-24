Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports std = System.Math

Public Module EdgeRTMMNormalization

    ''' <summary>
    ''' 计算 edgeR 的 TMM 归一化因子
    ''' </summary>
    <Extension>
    Public Function CalcTMMFactors(countData As Matrix) As (normFactors As Double(), referenceSampleIndex As Integer)
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

        ' 3. 计算每个样本相对于参考样本的归一化因子
        Dim normFactors(sampleCount - 1) As Double

        ' 参考样本的归一化因子设为 1
        normFactors(referenceIndex) = 1.0

        ' 为每个非参考样本计算 TMM 因子
        For j As Integer = 0 To sampleCount - 1
            If j = referenceIndex Then Continue For

            ' 收集 M 值和 A 值
            Dim mValues As New List(Of Double)
            Dim aValues As New List(Of Double)
            Dim weights As New List(Of Double)

            ' 遍历所有基因
            For i As Integer = 0 To geneCount - 1
                Dim countRef As Double = countData.expression(i).experiments(referenceIndex)
                Dim countSample As Double = countData.expression(i).experiments(j)

                ' 跳过在两样本中均为零的基因
                If countRef = 0 AndAlso countSample = 0 Then Continue For

                ' 添加伪计数 0.5 以避免除以零（edgeR 的默认做法）
                Dim pseudoRef As Double = countRef + 0.5
                Dim pseudoSample As Double = countSample + 0.5

                ' 计算 M 值（log2 比值）和 A 值（平均表达水平）
                Dim m As Double = std.Log(pseudoSample / pseudoRef) / std.Log(2)  ' log2
                Dim a As Double = 0.5 * (std.Log(pseudoSample) / std.Log(2) + std.Log(pseudoRef) / std.Log(2))

                ' 计算权重（与 edgeR 类似，基于方差）
                Dim weight As Double = (pseudoSample - 0.5) * (pseudoRef - 0.5) / (pseudoSample + pseudoRef)

                mValues.Add(m)
                aValues.Add(a)
                weights.Add(weight)
            Next

            ' 4. 修剪：默认移除 M 值和 A 值的极端值
            ' 移除 M 值最高和最低的 30%，A 值最高和最低的 5%（edgeR 的默认参数）
            Dim trimFractionM As Double = 0.3
            Dim trimFractionA As Double = 0.05

            ' 创建索引列表用于排序
            Dim indices As List(Of Integer) = Enumerable.Range(0, mValues.Count).ToList()

            ' 按 M 值排序
            indices.Sort(Function(x, y) mValues(x).CompareTo(mValues(y)))

            ' 计算修剪边界
            Dim trimM As Integer = CInt(mValues.Count * trimFractionM / 2)

            ' 按 A 值排序
            indices.Sort(Function(x, y) aValues(x).CompareTo(aValues(y)))
            Dim trimA As Integer = CInt(mValues.Count * trimFractionA / 2)

            ' 创建有效索引集合
            Dim validIndices As New HashSet(Of Integer)(indices.Skip(trimA).Take(mValues.Count - 2 * trimA))

            ' 按 M 值排序再次过滤
            indices.Sort(Function(x, y) mValues(x).CompareTo(mValues(y)))
            Dim validAfterMTrim As New HashSet(Of Integer)
            For idx As Integer = trimM To mValues.Count - trimM - 1
                If validIndices.Contains(indices(idx)) Then
                    validAfterMTrim.Add(indices(idx))
                End If
            Next

            ' 5. 计算加权平均 M 值
            Dim sumWeightedM As Double = 0
            Dim sumWeights As Double = 0

            For Each idx As Integer In validAfterMTrim
                sumWeightedM += mValues(idx) * weights(idx)
                sumWeights += weights(idx)
            Next

            Dim weightedMeanM As Double = 0
            If sumWeights > 0 Then
                weightedMeanM = sumWeightedM / sumWeights
            End If

            ' 6. 计算归一化因子
            normFactors(j) = std.Pow(2, weightedMeanM)
        Next

        ' 7. 调整归一化因子，使其几何平均为 1
        Dim logSum As Double = 0
        Dim validFactorCount As Integer = 0

        For j As Integer = 0 To sampleCount - 1
            If normFactors(j) > 0 Then
                logSum += std.Log(normFactors(j))
                validFactorCount += 1
            End If
        Next

        If validFactorCount > 0 Then
            Dim geoMean As Double = std.Exp(logSum / validFactorCount)

            For j As Integer = 0 To sampleCount - 1
                If geoMean > 0 Then
                    normFactors(j) /= geoMean
                End If
            Next
        End If

        Return (normFactors, referenceIndex)
    End Function

    ''' <summary>
    ''' 应用 TMM 归一化因子生成归一化后的表达矩阵
    ''' </summary>
    <Extension>
    Public Function ApplyTMMNormalization(countData As Matrix, Optional useEffectiveLibSize As Boolean = True) As Matrix
        Dim sampleCount As Integer = countData.sampleID.Length
        Dim geneCount As Integer = countData.expression.Length

        ' 获取 TMM 归一化因子
        Dim tmmResult = countData.CalcTMMFactors()
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
    Public Function EdgeRTMMNormalize(countData As Matrix) As Matrix
        ' 使用有效库大小计算 CPM
        Return countData.ApplyTMMNormalization(useEffectiveLibSize:=True)
    End Function

    ''' <summary>
    ''' 计算简单的 TMM 归一化矩阵（直接除以归一化因子）
    ''' 这更类似于 DESeq2 的输出格式
    ''' </summary>
    <Extension>
    Public Function EdgeRTMMNormalizeSimple(countData As Matrix) As Matrix
        ' 直接除以归一化因子
        Return countData.ApplyTMMNormalization(useEffectiveLibSize:=False)
    End Function
End Module