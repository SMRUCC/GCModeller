#Region "Microsoft.VisualBasic::8fb20903c87956ce45c94ce68fb0a7fc, analysis\Metagenome\Metagenome\RelativeStatics.vb"

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

    ' Module RelativeStatics
    ' 
    '     Function: ExportByRanks, RelativeAbundance
    '     Class RankView
    ' 
    '         Properties: OTUs, Samples, TaxonomyName, Tree
    ' 
    '         Function: ToString
    ' 
    '     Class View
    ' 
    '         Properties: OTU, Samples, TaxonTree
    ' 
    '         Function: ToString
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
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
    ''' 统计OTU在不同的物种分类层次上面每一个实验样品的相对丰度
    ''' </summary>
    ''' <param name="source">OTU统计数据</param>
    ''' <param name="EXPORT"></param>
    ''' <returns></returns>
    <Extension>
    Public Function ExportByRanks(source As IEnumerable(Of OTUData), EXPORT As String) As Boolean
        Dim samples As View() = LinqAPI.Exec(Of View) <=   ' 进行数据视图转换
            From x As OTUData
            In source
            Select New View With {
                .OTU = x.OTU,
                .Samples = x.Data.ToDictionary(
                    Function(o) o.Key,
                    Function(o) o.Value * 100),
                .TaxonTree = New gast.Taxonomy(x.Taxonomy.Split(";"c))
            }

        For Each rank As SeqValue(Of String) In gast.Taxonomy.ranks.SeqIterator   ' 按照rank层次进行计算
            Dim out As String = $"{EXPORT}/{rank.value}.Csv"
            Dim Groups = (From x As View
                          In samples
                          Let tree As String = x.TaxonTree.GetTree(rank.i)   ' 按照物种树进行数据分组
                          Select x,
                              tree
                          Group By tree Into Group).ToArray
            Dim result As New List(Of RankView)

            For Each g In Groups
                Dim gg As View() = g.Group.Select(Function(x) x.x)
                result += New RankView With {
                    .OTUs = gg.Select(Function(x) x.OTU),
                    .TaxonomyName = gg.First.TaxonTree(rank.i),
                    .Tree = g.tree,
                    .Samples = (From o As KeyValuePair(Of String, Double)
                                In (From x As View
                                    In gg
                                    Select x.Samples.ToArray).IteratesALL
                                Select o
                                Group o By o.Key Into Group) _
                                     .ToDictionary(Function(x) x.Key,
                                                   Function(x) x.Group.Sum(
                                                   Function(oo) oo.Value) / 100)  ' 计算样品丰度
                }
            Next

            Call result.SaveTo(out)
        Next

        Return True
    End Function

    Public Class RankView

        Public Property OTUs As String()
        Public Property TaxonomyName As String
        Public Property Tree As String
        <Meta(GetType(Double))>
        Public Property Samples As Dictionary(Of String, Double)

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class View

        Public Property TaxonTree As gast.Taxonomy
        Public Property Samples As Dictionary(Of String, Double)
        Public Property OTU As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Module
