#Region "Microsoft.VisualBasic::001d33524d9df7f9d5252d5968b8794f, GCModeller\analysis\Metagenome\Metagenome\RankAbundance.vb"

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

    '   Total Lines: 129
    '    Code Lines: 84
    ' Comment Lines: 32
    '   Blank Lines: 13
    '     File Size: 5.78 KB


    ' Module RankAbundance
    ' 
    '     Function: GroupValue, RankAbundance, TaxonomyProfile, TaxonomyRankString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Correlations
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Metagenomics

''' <summary>
''' ``Rank Abundance``曲线
''' </summary>
Public Module RankAbundance

    ''' <summary>
    ''' ``Rank-abundance``曲线是分析多样性的一种方式。构建方法是统计单一样品中，每一个OTU 所含的序列数，
    ''' 将OTUs 按丰度（所含有的序列条数）由大到小等级排序，再以OTU 等级为横坐标，以每个OTU 中所含的序列
    ''' 数（也可用OTU 中序列数的相对百分含量）为纵坐标做图。
    ''' ``Rank-abundance``曲线可用来解释多样性的两个方面，即物种丰度和物种均匀度。在水平方向，物种的丰度
    ''' 由曲线的宽度来反映，物种的丰度越高，曲线在横轴上的范围越大；曲线的形状（平滑程度）反映了样品中物种
    ''' 的均度，曲线越平缓，物种分布越均匀。
    ''' 
    ''' > 这个函数只提供数据统计的功能，绘图操作还需要在其他模块之中完成
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks>
    ''' > Scott T Bates, Jose C Clemente, et al. Global biogeography of highly diverse protistan communities in soil. The ISME Journal (2013) 7, 652–659; doi:10.1038/ismej.2012.147.
    ''' </remarks>
    <Extension> Public Function RankAbundance(otus As IEnumerable(Of OTUTable)) As OTUTable()
        Dim vector = otus.ToArray
        Dim OTU_seqs = vector _
            .Select(Function(OTU)
                        Dim currentTotal As Double = Aggregate sample
                                                     In OTU.Properties
                                                     Into Sum(sample.Value)
                        Return (ID:=OTU.ID, sum:=currentTotal, OTU:=OTU)
                    End Function) _
            .ToArray                                            ' 每一个OTU所含的序列的数量
        Dim all = OTU_seqs.First _
            .OTU _
            .Properties _
            .Keys _
            .ToDictionary(Function(sample) sample,
                          Function(allOTU)
                              Return vector.DATA(allOTU).Sum
                          End Function)  ' 计算出每一个sample样本测序得到序列总数
        Dim ranks#() = OTU_seqs _
            .Select(Function(OTU) OTU.sum) _
            .OrdinalRanking(desc:=True)            ' 从大到小降序排序得到ranking结果
        Dim out As New List(Of OTUTable)

        For Each i In ranks.SeqIterator.OrderBy(Function(rank) rank.value)
            Dim rank$ = i.value
            Dim seq = OTU_seqs(i).OTU

            out += New OTUTable With {
                .ID = rank,
                .Properties = seq.Properties _
                    .ToDictionary(Function(sample) sample.Key,
                                  Function(percent)
                                      Return percent.Value / all(percent.Key) * 100  ' 计算出每一个样品的丰度
                                  End Function)
            }
        Next

        Return out
    End Function

    ''' <summary>
    ''' 对样本结果进行实验内合并
    ''' </summary>
    ''' <param name="otus"></param>
    ''' <param name="sampleGroups">样品的分组信息</param>
    ''' <returns></returns>
    <Extension> Public Function GroupValue(otus As IEnumerable(Of OTUTable), sampleGroups As Dictionary(Of String, String())) As OTUTable()
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' 按照rank等级来查看物种的含量为多少
    ''' </summary>
    ''' <param name="OTUs"></param>
    ''' <param name="rank"></param>
    ''' <param name="percentage"></param>
    ''' <returns></returns>
    <Extension>
    Public Function TaxonomyProfile(OTUs As IEnumerable(Of gastOUT), rank As TaxonomyRanks, Optional percentage As Boolean = True) As Dictionary(Of String, Double)
        Dim counts = OTUs _
            .Select(Function(otu)
                        Return (tax:=otu.TaxonomyLineage.TaxonomyRankString(rank), counts:=otu.counts)
                    End Function) _
            .GroupBy(Function(otu) otu.tax) _
            .ToArray

        If percentage Then

            Dim all As Integer = counts _
                .Select(Function(t) t) _
                .IteratesALL _
                .Select(Function(t) t.counts) _
                .Sum

            Return counts _
                .ToDictionary(Function(t) t.Key,
                              Function(g)
                                  Return g.Select(Function(t) t.counts).Sum / all
                              End Function)
        Else
            Return counts _
                .ToDictionary(Function(t) t.Key,
                              Function(g)
                                  Return CDbl(g.Select(Function(t) t.counts).Sum)
                              End Function)
        End If
    End Function

    <Extension>
    Public Function TaxonomyRankString(taxonomy As Metagenomics.Taxonomy, rank As TaxonomyRanks, Optional fillNA As Boolean = True) As String
        ' 2017-12-19 当指定rank为Genus的时候，直接减去100则只会返回family级别的结果
        ' 在这里添加1来修复这个BUG
        Dim length% = rank - 100 + 1
        Dim array = taxonomy.Select.Take(length).AsList

        If fillNA AndAlso array.Count < length Then
            array += Repeats("NA", length - array.Count)
        End If

        Return BIOMTaxonomy.TaxonomyString(array)
    End Function
End Module
