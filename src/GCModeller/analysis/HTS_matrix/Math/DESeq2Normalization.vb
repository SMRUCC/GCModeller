Imports System.Math
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Math.Statistics.Linq
Imports std = System.Math

Public Module DESeq2Normalization

    ''' <summary>
    ''' DESeq2 Median of Ratios normalization method.
    ''' </summary>
    <Extension>
    Public Function DESeq2Normalize(countData As Matrix) As Matrix
        Dim sampleCount As Integer = countData.sampleID.Length
        Dim geneCount As Integer = countData.expression.Length

        ' 第一步：计算每个基因的几何平均数，并过滤掉全为0或无法计算的基因
        ' 使用 Log 转换来防止连乘导致 Double 溢出: GeoMean = Exp(Avg(Log(x)))
        Dim pseudoReference As New List(Of Double)
        Dim validGeneIndices As New List(Of Integer)

        For i As Integer = 0 To geneCount - 1
            Dim geneExpr As Double() = countData.expression(i).experiments
            Dim sumLog As Double = 0
            Dim validCount As Integer = 0

            For j As Integer = 0 To sampleCount - 1
                If geneExpr(j) > 0 Then
                    sumLog += std.Log(geneExpr(j))
                    validCount += 1
                End If
            Next

            ' 只有在至少两个样本中表达（或者严格大于0），才计算几何平均数
            ' DESeq2的默认行为是过滤掉几何平均数为0的基因
            If validCount > 0 Then
                Dim geoMean As Double = Exp(sumLog / validCount)
                pseudoReference.Add(geoMean)
                validGeneIndices.Add(i)
            End If
        Next

        ' 第二步：计算每个样本的 Size Factor (中位数比例)
        Dim sizeFactors(sampleCount - 1) As Double

        For j As Integer = 0 To sampleCount - 1
            Dim ratios As New List(Of Double)

            ' 遍历所有有效基因
            For k As Integer = 0 To validGeneIndices.Count - 1
                Dim geneIndex As Integer = validGeneIndices(k)
                Dim countValue As Double = countData.expression(geneIndex).experiments(j)
                Dim geoMean As Double = pseudoReference(k)

                ' 计算比例: Count / GeoMean
                ratios.Add(countValue / geoMean)
            Next

            ' 取中位数作为该样本的 Size Factor
            sizeFactors(j) = ratios.Median()
        Next

        ' 第三步：应用 Size Factor 进行归一化
        Dim normExpression(countData.expression.Length - 1) As DataFrameRow

        For i As Integer = 0 To geneCount - 1
            Dim normExperiments(sampleCount - 1) As Double

            For j As Integer = 0 To sampleCount - 1
                ' 归一化公式: Count / SizeFactor
                ' 注意处理 SizeFactor 为 0 的极端情况（虽然理论上不应出现）
                Dim sf As Double = sizeFactors(j)
                normExperiments(j) = If(sf = 0, 0, countData.expression(i).experiments(j) / sf)
            Next

            normExpression(i) = New DataFrameRow With {
                .geneID = countData.expression(i).geneID,
                .experiments = normExperiments
            }
        Next

        ' 构建并返回归一化后的新矩阵
        Return New Matrix With {
            .tag = $"DESeq2Norm({countData.tag})",
            .sampleID = countData.sampleID,
            .expression = normExpression
        }
    End Function
End Module
