#Region "Microsoft.VisualBasic::7ac188c63041fb24a1ace7892a5bfc23, annotations\GSEA\FisherCore\Enrichment.vb"

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

    '   Total Lines: 160
    '    Code Lines: 110 (68.75%)
    ' Comment Lines: 33 (20.62%)
    '    - Xml Docs: 81.82%
    ' 
    '   Blank Lines: 17 (10.62%)
    '     File Size: 6.60 KB


    ' Module Enrichment
    ' 
    '     Function: BackgroundSize, calcResult, CutBackgroundBySize, Enrichment
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports F = Microsoft.VisualBasic.Math.Statistics.Hypothesis.FishersExact.FishersExactTest

<Assembly: InternalsVisibleTo("SMRUCC.genomics.Analysis.HTS.GSEA")>

''' <summary>
''' 基于Fisher Extract test算法的富集分析
''' </summary>
Public Module Enrichment

    <Extension>
    Public Function CutBackgroundBySize(genome As Background, cutSize As Integer) As Background
        Return New Background With {
            .build = genome.build,
            .comments = genome.comments,
            .id = genome.id,
            .name = genome.name,
            .clusters = genome.clusters _
                .Where(Function(cl) cl.members.Length > cutSize) _
                .ToArray,
            ' 注意：背景规模(universe, N)始终是目标基因组之中的基因总数，
            ' 不能够因为过滤掉了部分过小的功能聚类而发生改变，否则会
            ' 使得超几何检验的总量N被低估
            .size = genome.size
        }
    End Function

    ''' <summary>
    ''' evaluate the unique idset size in the given cluster background model
    ''' </summary>
    ''' <param name="clusters"></param>
    ''' <returns></returns>
    <Extension>
    Public Function BackgroundSize(clusters As IEnumerable(Of Cluster)) As Integer
        Return clusters _
            .Select(Function(c) c.members) _
            .IteratesALL _
            .Select(Function(c) c.accessionID) _
            .Distinct _
            .Count
    End Function

    ''' <summary>
    ''' 基于Fisher精确检验的基因列表富集计算分析
    ''' </summary>
    ''' <param name="genome"></param>
    ''' <param name="list">需要进行富集计算分析的目标基因列表</param>
    ''' <param name="outputAll">将会忽略掉所有没有交集的结果</param>
    ''' <param name="showProgress"></param>
    ''' <returns>
    ''' 返回来的结果没有进行FDR计算
    ''' </returns>
    <Extension>
    Public Iterator Function Enrichment(genome As Background,
                                        list As IEnumerable(Of String),
                                        Optional resize As Integer = -1,
                                        Optional cutSize As Integer = 3,
                                        Optional outputAll As Boolean = False,
                                        Optional isLocustag As Boolean = False,
                                        Optional showProgress As Boolean = True,
                                        Optional doProgress As Action(Of String) = Nothing) As IEnumerable(Of EnrichmentResult)
        Dim genes As Integer
        Dim termResult As New Value(Of EnrichmentResult)

        If list Is Nothing Then
            Return
        End If

        If cutSize > 0 Then
            genome = genome.CutBackgroundBySize(cutSize)
        End If

        If genome.size <= 0 Then
            genes = genome.clusters.BackgroundSize
        Else
            genes = genome.size
        End If

        With list.Where(Function(id) Not id.StringEmpty(, True)).ToArray
            Dim input_size As Integer = If(resize > 0, resize, .Length)
            Dim background As IEnumerable(Of Cluster) = genome.clusters
            Dim bar As Tqdm.ProgressBar = Nothing

            If showProgress Then
                background = Tqdm.Wrap(genome.clusters, bar:=bar)
            End If
            If doProgress Is Nothing Then
                If showProgress Then
                    doProgress =
                        Sub(name)
                            Call bar.SetLabel(name)
                        End Sub
                Else
                    doProgress =
                        Sub()
                            ' do nothing
                        End Sub
                End If
            End If

            ' 注意：旧版本的代码在这里遍历的是``genome.clusters``，导致上面
            ' 所创建的Tqdm进度条包装``background``变成了一个死变量，进度条
            ' 实际上从来都不会被推进
            For Each cluster As Cluster In background
                Dim enriched$() = cluster.Intersect(.ByRef, isLocustag).ToArray

                Call doProgress(cluster.names)

                If enriched.Length > 0 Then
                    If Not (termResult = cluster.calcResult(enriched, input_size, genes, outputAll)) Is Nothing Then
                        Yield termResult
                    End If
                End If
            Next
        End With
    End Function

    ''' <summary>
    ''' 计算基因集合的功能富集结果
    ''' </summary>
    ''' <param name="cluster">根据我们的先验知识所创建的一个基因集合，一般为KEGG代谢途径或者GO词条</param>
    ''' <param name="enriched">在当前的基因集合中与我们所给定的基因列表所产生交集的基因id的列表，即我们的差异基因列表中属于当前的代谢途径的基因的列表</param>
    ''' <param name="inputSize">输入的原始的通过实验所获取得到的基因列表的大小，即我们的差异基因的id的数量</param>
    ''' <param name="genes">背景基因组中的总的基因数量</param>
    ''' <param name="outputAll"></param>
    ''' <returns></returns>
    <Extension>
    Public Function calcResult(cluster As Cluster, enriched$(), inputSize%, genes%, outputAll As Boolean) As EnrichmentResult
        ' 我们的差异基因列表中，属于目标代谢途径的基因的数量
        Dim a% = enriched.Length
        ' 在目标基因组中，属于当前的代谢途径中的基因的数量
        Dim b% = cluster.size

        ' 构造标准的超几何检验2x2列联表：
        '
        '                  |  in term        | not in term      |  row margin
        '     in list      |  a              | inputSize - a    |  inputSize
        '     not in list  |  b - a          | N - b - (inputSize - a) | N - inputSize
        '     ------------------------------------------------------------------
        '     col margin   |  b              | N - b            |  N
        '
        ' 注意：旧版本的代码在这里直接使用``FishersExact(a, b, c, d)``，其中
        ' ``b = cluster.size``已经将``a``重复计入了一次，而``d = genes - b``又没有
        ' 减去``inputSize - a``，导致四格之和变成了``genes + inputSize``，
        ' 行列的边际也不再正确，最终使得p值被系统性的低估。
        Dim n11% = a
        Dim n12% = inputSize - a
        Dim n21% = b - a
        Dim n22% = genes - b - inputSize + a
        Dim pvalue#
        ' a / b 为富集分数
        Dim score# = If(b > 0, a / b, 0.0)

        ' 由于注释数据可能并不完整，所以在这里对越界的计数做一个截断保护
        If n12 < 0 Then n12 = 0
        If n21 < 0 Then n21 = 0
        If n22 < 0 Then n22 = 0

        If a <= 0 OrElse inputSize <= 0 OrElse genes <= 0 OrElse b <= 0 Then
            pvalue = 1
        Else
            pvalue = F.FishersExact(n11, n12, n21, n22).two_tail_pvalue
        End If

        If (pvalue.IsNaNImaginary OrElse enriched.Length = 0) AndAlso Not outputAll Then
            Return Nothing
        End If

        Return New EnrichmentResult With {
            .term = cluster.ID,
            .name = cluster.names.TrimNewLine,
            .description = cluster.description.TrimNewLine,
            .IDs = enriched,
            .pvalue = pvalue,
            .score = score,
            .cluster = b,
            .enriched = a,
            .category = cluster.category,
            .[class] = cluster.class
        }
    End Function
End Module
