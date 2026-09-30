#Region "Microsoft.VisualBasic::af6a6e9afae784e9fa23e4bb033ec8f1, annotations\GSEA\GSVA\utils.vb"

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

    '   Total Lines: 67
    '    Code Lines: 42 (62.69%)
    ' Comment Lines: 15 (22.39%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 10 (14.93%)
    '     File Size: 2.84 KB


    ' Module utils
    ' 
    '     Function: filterFeatures, filterGeneSets, mapGeneSetsToFeatures
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports std = System.Math

Module utils

    Public Function filterGeneSets(mapped_gset_idx_list As Dictionary(Of String, String()), min_sz As Integer, max_sz As Integer) As Dictionary(Of String, String())
        Return mapped_gset_idx_list _
            .Where(Function(d)
                       Return d.Value.Length >= min_sz AndAlso d.Value.Length <= max_sz
                   End Function) _
            .ToDictionary
    End Function

    ''' <summary>
    ''' filter out genes with constant expression values
    ''' </summary>
    ''' <param name="expr"></param>
    ''' <param name="method"></param>
    ''' <returns></returns>
    Public Function filterFeatures(expr As Matrix, method As Methods) As Matrix
        Dim sdGenes As Vector = expr.rowSds.Values.AsVector

        sdGenes(sdGenes < 0.0000000001) = Vector.Zero

        If sdGenes.Any(Function(x) x = 0.0 OrElse x.IsNaNImaginary) Then
            Call $"{(sdGenes = 0.0 OrElse sdGenes.IsNaNImaginary).Sum} genes with constant expression values throuhgout the samples.".Warning

            If method <> Methods.ssgsea Then
                Call "Since argument method!='ssgsea', genes with constant expression values are discarded.".Warning

                expr = expr(sdGenes > 0 & Not sdGenes.IsNaNImaginary)
            End If
        End If

        If expr.size < 2 Then
            Throw New InvalidProgramException("Less than two genes in the input assay object")
        Else
            Return expr
        End If
    End Function

    ''' <summary>
    ''' maps gene sets content in 'gsets' to 'features', where 'gsets'
    ''' Is a 'list' object with character string vectors as elements,
    ''' And 'features' is a character string vector object. it assumes
    ''' features In both input objects follow the same nomenclature
    ''' </summary>
    ''' <param name="gsets"></param>
    ''' <param name="features"></param>
    ''' <returns></returns>
    Public Function mapGeneSetsToFeatures(gsets As Background, features As String()) As Dictionary(Of String, String())
        Dim mapdgenesets = gsets.clusters _
            .ToDictionary(Function(c) c.ID,
                          Function(c)
                              Return c.Intersect(features).ToArray
                          End Function)

        If mapdgenesets.Values.IteratesALL.Count = 0 Then
            Throw New InvalidProgramException("No identifiers in the gene sets could be matched to the identifiers in the expression data.")
        Else
            Return mapdgenesets
        End If
    End Function

    ''' <summary>
    ''' 按 (取值升序, 下标升序) 对一列数据的下标做排序
    ''' </summary>
    ''' <param name="v">单个样本（列）上的取值向量</param>
    ''' <returns>排序后的基因下标</returns>
    ''' <remarks>
    ''' 由于比较函数中同时比较了取值与下标，这是一个全序关系，
    ''' 因此无论底层排序算法是否稳定，结果都是确定的：
    ''' 并列的元素中下标靠后的会取得更大的秩，与 R 的 ``ties.method = "last"`` 一致。
    ''' 非数值（NaN）被统一排到末尾，避免出现与 R 不一致的随机次序。
    ''' </remarks>
    Private Function ascendingOrder(v As Double()) As Integer()
        Dim p As Integer = v.Length
        Dim order As Integer() = New Integer(p - 1) {}

        For i As Integer = 0 To p - 1
            order(i) = i
        Next

        Array.Sort(order,
            Function(a As Integer, b As Integer) As Integer
                Dim x As Double = v(a)
                Dim y As Double = v(b)
                Dim nanX As Boolean = Double.IsNaN(x)
                Dim nanY As Boolean = Double.IsNaN(y)

                If nanX Then
                    Return If(nanY, a.CompareTo(b), 1)
                ElseIf nanY Then
                    Return -1
                End If

                Dim c As Integer = x.CompareTo(y)

                If c = 0 Then
                    Return a.CompareTo(b)
                Else
                    Return c
                End If
            End Function)

        Return order
    End Function

    ''' <summary>
    ''' 逐列计算序数排名，等价于 R 的 ``colRanks(Z, ties.method = "last")``
    ''' </summary>
    ''' <param name="m">行是基因（特征）、列是样本的矩阵</param>
    ''' <returns>
    ''' 按样本组织的排名数组 ``ranks(样本)(基因)``，取值 1..p，其中 1 表示该样本中取值最小的基因。
    ''' </returns>
    Public Function colRanksLast(m As NumericMatrix) As Integer()()
        Dim rows As Double()() = m.ArrayPack(deepcopy:=False)
        Dim nGenes As Integer = m.RowDimension
        Dim nSamples As Integer = m.ColumnDimension
        Dim ranks As Integer()() = New Integer(nSamples - 1)() {}

        For j As Integer = 0 To nSamples - 1
            Dim v As Double() = New Double(nGenes - 1) {}

            For i As Integer = 0 To nGenes - 1
                v(i) = rows(i)(j)
            Next

            Dim order As Integer() = ascendingOrder(v)
            Dim rank As Integer() = New Integer(nGenes - 1) {}

            For k As Integer = 0 To nGenes - 1
                rank(order(k)) = k + 1
            Next

            ranks(j) = rank
        Next

        Return ranks
    End Function

    ''' <summary>
    ''' 逐列计算平均排名，等价于 R 的 ``colRanks(X, ties.method = "average")``
    ''' </summary>
    ''' <param name="m">行是基因（特征）、列是样本的矩阵</param>
    ''' <returns>
    ''' 按样本组织的排名数组 ``ranks(样本)(基因)``；并列的一组元素取得它们所占据秩的平均值。
    ''' </returns>
    Public Function colRanksAverage(m As NumericMatrix) As Double()()
        Dim rows As Double()() = m.ArrayPack(deepcopy:=False)
        Dim nGenes As Integer = m.RowDimension
        Dim nSamples As Integer = m.ColumnDimension
        Dim ranks As Double()() = New Double(nSamples - 1)() {}

        For j As Integer = 0 To nSamples - 1
            Dim v As Double() = New Double(nGenes - 1) {}

            For g As Integer = 0 To nGenes - 1
                v(g) = rows(g)(j)
            Next

            Dim order As Integer() = ascendingOrder(v)
            Dim rank As Double() = New Double(nGenes - 1) {}
            Dim cursor As Integer = 0

            ' 排序后取值相同的元素必然相邻，逐个并列组计算平均秩即可
            While cursor < nGenes
                Dim k As Integer = cursor + 1

                While k < nGenes AndAlso v(order(k)) = v(order(cursor))
                    k += 1
                End While

                If k - cursor > 1 Then
                    Dim avgRank As Double = (cursor + 1 + k) / 2.0

                    For t As Integer = cursor To k - 1
                        rank(order(t)) = avgRank
                    Next
                Else
                    rank(order(cursor)) = cursor + 1
                End If

                cursor = k
            End While

            ranks(j) = rank
        Next

        Return ranks
    End Function

    ''' <summary>
    ''' 逐行做标准化，等价于 R 的 ``t(scale(t(X)))``
    ''' </summary>
    ''' <param name="X">行是基因、列是样本的原始数据</param>
    ''' <returns>每个基因在其样本维度上零均值、单位标准差的标准化矩阵</returns>
    ''' <remarks>
    ''' 标准差使用 n - 1 作为分母（样本标准差），与 R 的 ``sd()``、``rowSds()`` 一致。
    ''' 标准差为零（恒定表达）的行在算法上游已经被过滤掉，此处仍做保护以免产生除零。
    ''' </remarks>
    Public Function rowZScore(X As Double()()) As Double()()
        Dim nGenes As Integer = X.Length
        Dim nSamples As Integer = X(0).Length
        Dim out As Double()() = New Double(nGenes - 1)() {}

        For i As Integer = 0 To nGenes - 1
            Dim row As Double() = X(i)
            Dim mean As Double = 0
            Dim sumSq As Double = 0
            Dim sd As Double

            For Each xi As Double In row
                mean += xi
            Next

            mean /= nSamples

            For Each xi As Double In row
                sumSq += (xi - mean) ^ 2
            Next

            If nSamples < 2 Then
                sd = Double.NaN
            Else
                sd = std.Sqrt(sumSq / (nSamples - 1))
            End If

            Dim z As Double() = New Double(nSamples - 1) {}

            If sd = 0 OrElse Double.IsNaN(sd) Then
                ' 恒定表达的行没有信息量，标准化后记为零向量
                For j As Integer = 0 To nSamples - 1
                    z(j) = 0
                Next
            Else
                For j As Integer = 0 To nSamples - 1
                    z(j) = (row(j) - mean) / sd
                Next
            End If

            out(i) = z
        Next

        Return out
    End Function
End Module
