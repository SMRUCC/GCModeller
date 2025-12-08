#Region "Microsoft.VisualBasic::8240666bf616bfab06a0204a16f6f6bc, analysis\Metagenome\Metagenome\RelativeStatics.vb"

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

    '   Total Lines: 106
    '    Code Lines: 77 (72.64%)
    ' Comment Lines: 21 (19.81%)
    '    - Xml Docs: 80.95%
    ' 
    '   Blank Lines: 8 (7.55%)
    '     File Size: 5.03 KB


    ' Module RelativeStatics
    ' 
    '     Function: CastView, ExportByRanks, PopulateViews, RelativeAbundance
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Metagenomics

Public Module RelativeStatics

    ''' <summary>
    ''' 计算含量相对丰度，这个函数会合并相同的分类的结果数据
    ''' </summary>
    ''' <returns>``[taxonomy => percentage]``</returns>
    ''' <remarks>
    ''' 每一个OTU计数就是其丰度值，相对丰度就是这个计数值在总计数之中的百分比值
    ''' </remarks>
    <Extension>
    Public Function RelativeAbundance(metagenome As IEnumerable(Of gastOUT)) As Dictionary(Of String, Double)
        Dim vector As gastOUT() = metagenome.ToArray
        Dim all% = Aggregate tax As gastOUT In vector Into Sum(tax.counts)
        Dim taxonomyGroup = vector _
            .GroupBy(Function(tax) tax.taxonomy) _
            .ToArray
        Dim table = taxonomyGroup _
            .ToDictionary(Function(tg) tg.Key,
                          Function(tg)
                              ' 计算出该物种峰所有的OTU的总计数的百分比得到相对丰度
                              Return (Aggregate tax As gastOUT In tg Into Sum(tax.counts)) / all
                          End Function)
        Return table
    End Function

    ''' <summary>
    ''' 进行数据视图转换
    ''' </summary>
    ''' <param name="source"></param>
    ''' <returns></returns>
    <Extension>
    Private Function CastView(source As IEnumerable(Of OTUData(Of Double))) As IEnumerable(Of OTUTable)
        Return From x As OTUData(Of Double)
               In source
               Let taxon = New gast.Taxonomy(BIOMTaxonomyParser.Parse(x.taxonomy))
               Select New OTUTable With {
                   .ID = x.OTU,
                   .Properties = x.data _
                       .ToDictionary(Function(o) o.Key,
                                     Function(o)
                                         Return o.Value * 100
                                     End Function),
                   .taxonomy = taxon
               }
    End Function

    ''' <summary>
    ''' 统计OTU在不同的物种分类层次上面每一个实验样品的相对丰度
    ''' </summary>
    ''' <param name="source">OTU统计数据</param>
    ''' <returns></returns>
    <Extension>
    Public Function ExportByRanks(source As IEnumerable(Of OTUTable)) As IEnumerable(Of NamedCollection(Of RankLevelView))
        Return source _
            .Select(Function(o)
                        Return New OTUTable() With {
                            .ID = o.ID,
                            .Properties = o.Properties,
                            .taxonomy = New gast.Taxonomy(o.taxonomy)
                        }
                    End Function) _
            .ExportByRanksInternal
    End Function

    ''' <summary>
    ''' 统计OTU在不同的物种分类层次上面每一个实验样品的相对丰度
    ''' </summary>
    ''' <param name="samples">OTU统计数据</param>
    ''' <returns></returns>
    <Extension>
    Private Iterator Function ExportByRanksInternal(samples As IEnumerable(Of OTUTable)) As IEnumerable(Of NamedCollection(Of RankLevelView))
        ' 按照rank层次进行计算
        For Each rank As SeqValue(Of String) In gast.Taxonomy.ranks.SeqIterator
            ' 按照物种树进行数据分组
            Dim groups = (From x As OTUTable
                          In samples
                          Let tree As String = DirectCast(x.taxonomy, gast.Taxonomy).GetTree(rank.i, biom_style:=True)
                          Select i = (x, tree)
                          Group By i.tree Into Group).ToArray
            Dim tuples = groups.Select(Function(ti) (tree:=ti.tree, list:=ti.Group.Select(Function(x) (x.tree, x.x)).ToArray)).ToArray
            Dim result As RankLevelView() = rank.PopulateViews(tuples).ToArray

            Yield New NamedCollection(Of RankLevelView) With {
                .name = rank.value,
                .value = result
            }
        Next
    End Function

    ''' <summary>
    ''' 统计OTU在不同的物种分类层次上面每一个实验样品的相对丰度
    ''' </summary>
    ''' <param name="source">OTU统计数据</param>
    ''' <returns></returns>
    <Extension>
    Public Function ExportByRanks(source As IEnumerable(Of OTUData(Of Double))) As IEnumerable(Of NamedCollection(Of RankLevelView))
        Return source _
            .CastView _
            .ExportByRanksInternal
    End Function

    <Extension>
    Private Iterator Function PopulateViews(rank As SeqValue(Of String), groups As (tree As String, list As (tree As String, x As OTUTable)())()) As IEnumerable(Of RankLevelView)
        For Each g In groups
            Dim gg As OTUTable() = g.list.Select(Function(x) x.x).ToArray
            Dim sampleData = (From o As KeyValuePair(Of String, Double)
                              In (From x As OTUTable
                                  In gg
                                  Select x.Properties.ToArray).IteratesALL
                              Select o
                              Group o By o.Key Into Group) _
                                  .ToDictionary(Function(x) x.Key,
                                                Function(x)
                                                    ' 计算样品丰度
                                                    Return x.Group.Sum(Function(oo)
                                                                           Return oo.Value
                                                                       End Function) / 100
                                                End Function)

            Yield New RankLevelView With {
                .OTUs = gg.Select(Function(x) x.ID).ToArray,
                .TaxonomyName = DirectCast(gg.First.taxonomy, gast.Taxonomy)(rank.i),
                .Tree = g.tree,
                .Samples = sampleData
            }
        Next
    End Function
End Module
