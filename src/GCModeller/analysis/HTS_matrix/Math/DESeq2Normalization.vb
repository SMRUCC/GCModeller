#Region "Microsoft.VisualBasic::eecd302e04d91c096024b9646dea5322, analysis\HTS_matrix\Math\DESeq2Normalization.vb"

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

    '   Total Lines: 87
    '    Code Lines: 56 (64.37%)
    ' Comment Lines: 15 (17.24%)
    '    - Xml Docs: 20.00%
    ' 
    '   Blank Lines: 16 (18.39%)
    '     File Size: 3.48 KB


    ' Module DESeq2Normalization
    ' 
    '     Function: DESeq2Normalize
    ' 
    ' /********************************************************************************/

#End Region

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

        ' 第一步：挑选出在**所有**样本之中都有表达的基因，并计算其几何平均数
        '
        ' DESeq2 的 estimateSizeFactorsForMatrix() 里是这样写的：
        '
        '     loggeomeans <- rowMeans(log(counts))
        '     sf <- apply(counts, 2, function(cnts)
        '             exp(median((log(cnts) - loggeomeans)[is.finite(loggeomeans) & cnts > 0])))
        '
        ' 也就是说：任何含 0 的基因其对数几何平均是 -Inf，会被整体丢弃。
        ' 旧实现用"只对正值求几何平均"并且保留含 0 的基因，会让 0 值参与中位数，
        ' 把 size factor 系统性地压小；这里按 DESeq2 的语义修正。
        Dim pseudoReference As New List(Of Double)
        Dim validGeneIndices As New List(Of Integer)

        For i As Integer = 0 To geneCount - 1
            Dim geneExpr As Double() = countData.expression(i).experiments
            Dim sumLog As Double = 0
            Dim allPositive As Boolean = True

            For j As Integer = 0 To sampleCount - 1
                Dim v As Double = geneExpr(j)

                If v <= 0 Then
                    allPositive = False
                    Exit For
                End If

                sumLog += std.Log(v)
            Next

            If allPositive AndAlso sampleCount > 0 Then
                pseudoReference.Add(std.Exp(sumLog / sampleCount))
                validGeneIndices.Add(i)
            End If
        Next

        ' 第二步：计算每个样本的 Size Factor (中位数比例)
        Dim sizeFactors(sampleCount - 1) As Double

        If validGeneIndices.Count = 0 Then
            ' DESeq2 在这种情况下会直接报错（geoMeans 全部为 -Inf），
            ' 这里退化为不做缩放，保证函数仍然可用
            For j As Integer = 0 To sampleCount - 1
                sizeFactors(j) = 1.0
            Next
        Else
            For j As Integer = 0 To sampleCount - 1
                Dim logRatios As New List(Of Double)(validGeneIndices.Count)

                ' 遍历所有有效基因
                For k As Integer = 0 To validGeneIndices.Count - 1
                    Dim geneIndex As Integer = validGeneIndices(k)
                    Dim countValue As Double = countData.expression(geneIndex).experiments(j)
                    Dim geoMean As Double = pseudoReference(k)

                    ' DESeq2 在**对数空间**取中位数：
                    '   exp(median(log(count) - log(geoMean)))
                    ' 这与"先算比值再取中位数"并不等价：偶数个基因时前者是两个中间比值的
                    ' 几何平均，后者是算术平均，实测会带来 1e-7 量级的偏差。
                    Call logRatios.Add(std.Log(countValue) - std.Log(geoMean))
                Next

                ' 取中位数作为该样本的 Size Factor
                sizeFactors(j) = std.Exp(logRatios.Median())
            Next
        End If

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
