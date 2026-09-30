#Region "Microsoft.VisualBasic::2f182399739f14e5596d661d094e25de, annotations\GSEA\GSVA\plage.vb"

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
    '     File Size: 26 B


    ' Module plage
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

''' <summary>
''' Tomfohr et al. (2005) 的 PLAGE（Pathway Level Analysis of Gene Expression）方法
''' </summary>
''' <remarks>
''' 本模块是 R 包 GSVA 中 ``R/plage.R`` 的 ``plage()`` 的 VB 移植。
''' </remarks>
Module plage

    ''' <summary>
    ''' 计算 PLAGE 富集分数
    ''' </summary>
    ''' <param name="expr">行是基因、列是样本的表达矩阵，行已经过滤过恒定表达</param>
    ''' <param name="gsetIdxList">已经映射到表达矩阵行名上的基因集</param>
    ''' <returns>通路 x 样本 的富集分数矩阵</returns>
    ''' <remarks>
    ''' 每一个基因集单独做奇异值分解，取第一右奇异向量作为该通路在各样本上的活性分数：
    ''' 
    '''   s &lt;- svd(Z[gSetIdx, ]) 
    '''   score &lt;- s$v[, 1]
    ''' 
    ''' 其中 Z 是逐行标准化（每个基因在自己的样本维度上零均值、单位方差）后的表达矩阵。
    ''' 奇异向量的符号在数学上是不确定的，不同的奇异值分解实现可能给出相差一个正负号的结果，
    ''' 这不影响统计意义，但做数值比对时需要先统一符号。
    ''' </remarks>
    Public Function plageScores(expr As Matrix,
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
                            ' 基因集子矩阵：基因数 x 样本数
                            Dim submat As Double()() = idx _
                                .Select(Function(gene) Z(gene)) _
                                .ToArray
                            Dim score As Double() = New Double(nSamples - 1) {}

                            If idx.Length >= nSamples Then
                                ' A = U * S * V'，V 是 n x n（n 为样本数），
                                ' 它的列即为右奇异向量，第 0 列就是第一右奇异向量
                                Dim svd As New SingularValueDecomposition(New NumericMatrix(submat))
                                Dim V As Double()() = svd.V.ArrayPack(deepcopy:=False)

                                For j As Integer = 0 To nSamples - 1
                                    score(j) = V(j)(0)
                                Next
                            Else
                                ' 基因数少于样本数时矩阵是「宽」的，而 JAMA 系的奇异值分解
                                ' 只对行数不少于列数的矩阵有定义。利用 A 的转置与 A 共享奇异值、
                                ' 且 A 的右奇异向量恰为 A' 的左奇异向量这一关系，
                                ' 转置后分解再取第一左奇异向量即可。
                                Dim transposed As Double()() = New Double(nSamples - 1)() {}

                                For j As Integer = 0 To nSamples - 1
                                    transposed(j) = New Double(idx.Length - 1) {}

                                    For r As Integer = 0 To idx.Length - 1
                                        transposed(j)(r) = submat(r)(j)
                                    Next
                                Next

                                Dim svd As New SingularValueDecomposition(New NumericMatrix(transposed))
                                Dim U As Double()() = svd.U.ArrayPack(deepcopy:=False)

                                For j As Integer = 0 To nSamples - 1
                                    score(j) = U(j)(0)
                                Next
                            End If

                            Return New DataFrameRow With {
                                .geneID = gset.Key,
                                .experiments = score
                            }
                        End Function) _
                .ToArray
        }
    End Function
End Module
