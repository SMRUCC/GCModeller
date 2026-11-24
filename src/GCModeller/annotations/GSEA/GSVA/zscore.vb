#Region "Microsoft.VisualBasic::b74d1db250a850d6d6ffce42c89ed501, annotations\GSEA\GSVA\zscore.vb"

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


    ' Module zscore
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

''' <summary>
''' Lee et al. (2008) 的 combined z-score 方法
''' </summary>
''' <remarks>
''' 本模块是 R 包 GSVA 中 ``R/zscore.R`` 的 ``zscore()`` 的 VB 移植。
''' </remarks>
Module zscore

    ''' <summary>
    ''' 计算 combined z-score 富集分数
    ''' </summary>
    ''' <param name="expr">行是基因、列是样本的表达矩阵，行已经过滤过恒定表达</param>
    ''' <param name="gsetIdxList">已经映射到表达矩阵行名上的基因集</param>
    ''' <returns>通路 x 样本 的富集分数矩阵</returns>
    Public Function zscoreScores(expr As Matrix,
                                 gsetIdxList As Dictionary(Of String, String())) As Matrix

        Dim rowIndex As Index(Of String) = expr.rownames.Indexing
        Dim Z As Double()() = rowZScore(expr.ArrayPack)
        Dim nSamples As Integer = expr.sampleID.Length

        Return New Matrix With {
            .sampleID = expr.sampleID,
            .expression = gsetIdxList _
                .Select(Function(gset)
                            Dim idx As Integer() = gset.Value _
                                .Select(Function(id) rowIndex.IndexOf(id)) _
                                .ToArray
                            ' 对应官方实现的 colSums(Z[gSetIdx, , drop=FALSE]) / sqrt(length(gSetIdx))
                            Dim scale As Double = std.Sqrt(idx.Length)
                            Dim score As Double() = New Double(nSamples - 1) {}

                            For Each gene As Integer In idx
                                Dim zrow As Double() = Z(gene)

                                For j As Integer = 0 To nSamples - 1
                                    score(j) += zrow(j)
                                Next
                            Next

                            For j As Integer = 0 To nSamples - 1
                                score(j) /= scale
                            Next

                            Return New DataFrameRow With {
                                .geneID = gset.Key,
                                .experiments = score
                            }
                        End Function) _
                .ToArray
        }
    End Function
End Module
