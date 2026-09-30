#Region "Microsoft.VisualBasic::378034ab99f0e582155b69030484b43a, annotations\GSEA\GSVA\ssgsea.vb"

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

    '   Total Lines: 3
    '    Code Lines: 2 (66.67%)
    ' Comment Lines: 0 (0.00%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 1 (33.33%)
    '     File Size: 27 B


    ' Module ssgsea
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

''' <summary>
''' Barbie et al. (2009) 的 ssGSEA（single sample GSEA）方法
''' </summary>
''' <remarks>
''' 本模块是 R 包 GSVA 中 ``R/ssgsea.R`` 的 ``ssgsea()`` 的 VB 移植，
''' 富集分数采用 Alexey Sergushichev 提出的闭式加速形式（``.fastRndWalk``），
''' 与朴素实现 ``.rndWalk`` 在数学上等价。
''' </remarks>
Module ssgsea

    ''' <summary>
    ''' 计算 ssGSEA 富集分数
    ''' </summary>
    ''' <param name="expr">行是基因、列是样本的表达矩阵</param>
    ''' <param name="gsetIdxList">已经映射到表达矩阵行名上的基因集</param>
    ''' <param name="alpha">随机游走尾部权重指数，官方默认值为 0.25</param>
    ''' <param name="normalize">
    ''' TRUE 时按 Barbie et al. (2009) online methods 的做法，
    ''' 用全矩阵分数的最大最小值之差做归一化
    ''' </param>
    ''' <returns>通路 x 样本 的富集分数矩阵</returns>
    Public Function ssgseaScores(expr As Matrix,
                                 gsetIdxList As Dictionary(Of String, String()),
                                 Optional alpha As Double = 0.25,
                                 Optional normalize As Boolean = True) As Matrix

        Dim rowIndex As Index(Of String) = expr.rownames.Indexing
        Dim nGenes As Integer = expr.size
        Dim nSamples As Integer = expr.sampleID.Length
        ' 逐列平均秩：1 表示表达量最小，并列的一组取得平均秩，对应 R 的 ties.method = "average"
        Dim rankData As Double()() = colRanksAverage(New NumericMatrix(expr.ArrayPack))
        Dim Ra As Double()() = rankData
        Dim noAlpha As Boolean = (alpha = 1.0)

        If Not noAlpha Then
            Ra = rankData _
                .Select(Function(r)
                            Dim v As Double() = New Double(r.Length - 1) {}

                            For i As Integer = 0 To r.Length - 1
                                v(i) = std.Pow(r(i), alpha)
                            Next

                            Return v
                        End Function) _
                .ToArray
        End If

        ' 每个样本上按表达量降序排列的基因下标，以及「基因 -> 降序位置(1 基)」的反查表
        Dim rankings As Integer()() = New Integer(nSamples - 1)() {}
        Dim posOf As Integer()() = New Integer(nSamples - 1)() {}

        For j As Integer = 0 To nSamples - 1
            Dim ranking As Integer() = descendingOrder(rankData(j))
            Dim positions As Integer() = New Integer(nGenes - 1) {}

            For k As Integer = 0 To nGenes - 1
                positions(ranking(k)) = k + 1
            Next

            rankings(j) = ranking
            posOf(j) = positions
        Next

        Dim rows As New List(Of DataFrameRow)
        Dim minScore As Double = Double.PositiveInfinity
        Dim maxScore As Double = Double.NegativeInfinity

        For Each gset As KeyValuePair(Of String, String()) In gsetIdxList
            Dim genes As Integer() = gset.Value _
                .Select(Function(id) rowIndex.IndexOf(id)) _
                .ToArray
            Dim score As Double() = New Double(nSamples - 1) {}

            For j As Integer = 0 To nSamples - 1
                score(j) = fastRndWalk(genes, Ra(j), posOf(j), nGenes)

                If score(j) < minScore Then
                    minScore = score(j)
                End If
                If score(j) > maxScore Then
                    maxScore = score(j)
                End If
            Next

            rows.Add(New DataFrameRow With {
                .geneID = gset.Key,
                .experiments = score
            })
        Next

        If normalize Then
            Dim rng As Double = maxScore - minScore

            If rng > 0 Then
                For Each row As DataFrameRow In rows
                    For j As Integer = 0 To nSamples - 1
                        row.experiments(j) /= rng
                    Next
                Next
            End If
        End If

        Return New Matrix With {
            .sampleID = expr.sampleID,
            .expression = rows.ToArray
        }
    End Function

    ''' <summary>
    ''' 按取值降序对下标排序，等价于 R 的 ``order(x, decreasing = TRUE)``
    ''' </summary>
    ''' <param name="v">单个样本（列）上的取值向量</param>
    ''' <returns>排序后的下标</returns>
    ''' <remarks>
    ''' 同时以取值与下标作为比较键，构成全序关系，
    ''' 因此并列的元素保留下标靠前的次序，与 R 的稳定排序一致。
    ''' </remarks>
    Private Function descendingOrder(v As Double()) As Integer()
        Dim p As Integer = v.Length
        Dim order As Integer() = New Integer(p - 1) {}

        For i As Integer = 0 To p - 1
            order(i) = i
        Next

        Array.Sort(order,
            Function(a As Integer, b As Integer) As Integer
                Dim c As Integer = v(b).CompareTo(v(a))

                If c = 0 Then
                    Return a.CompareTo(b)
                Else
                    Return c
                End If
            End Function)

        Return order
    End Function

    ''' <summary>
    ''' 单个样本上单个基因集的 ssGSEA 富集分数（闭式加速形式）
    ''' </summary>
    ''' <param name="genes">基因集所含基因的下标（0 基）</param>
    ''' <param name="Ra">当前样本上每个基因的秩的 alpha 次幂</param>
    ''' <param name="posOf">「基因 -> 降序位置」反查表，取值 1..p，1 为表达最高</param>
    ''' <param name="n">基因总数 p</param>
    ''' <returns>富集分数</returns>
    ''' <remarks>
    ''' 对应官方实现的 ``.fastRndWalk(gSetIdx, geneRanking, j, Ra)``：
    ''' 
    '''   stepCDFinGeneSet  = sum(Ra[gSetRank] * (n - position + 1)) / sum(Ra[gSetRank])
    '''   stepCDFoutGeneSet = (n * (n + 1) / 2 - sum(n - position + 1)) / (n - k)
    '''   score             = stepCDFinGeneSet - stepCDFoutGeneSet
    ''' 
    ''' 其中的权重 ``n - position + 1`` 来自朴素实现里
    ''' 「集内累计和 - 集外累计和」沿着降序位置做前缀展开后的解析解。
    ''' </remarks>
    Private Function fastRndWalk(genes As Integer(),
                                 Ra As Double(),
                                 posOf As Integer(),
                                 n As Integer) As Double

        Dim k As Integer = genes.Length
        Dim num As Double = 0
        Dim den As Double = 0
        Dim sumWeight As Double = 0

        For Each gene As Integer In genes
            Dim position As Integer = posOf(gene)
            Dim weight As Double = n - position + 1

            num += Ra(gene) * weight
            den += Ra(gene)
            sumWeight += weight
        Next

        Dim stepIn As Double = num / den
        Dim stepOut As Double = (n * (n + 1) / 2.0 - sumWeight) / (n - k)

        Return stepIn - stepOut
    End Function
End Module
