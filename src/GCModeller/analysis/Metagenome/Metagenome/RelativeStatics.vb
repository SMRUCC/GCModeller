#Region "Microsoft.VisualBasic::a157f4c5b0b42c36776f9106d2c28714, ..\GCModeller\analysis\Metagenome\Metagenome\RelativeStatics.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
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

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Analysis.Metagenome.gast
Imports SMRUCC.genomics.Metagenomics

Public Module RelativeStatics

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
                Dim gg As View() = g.Group.ToArray(Function(x) x.x)
                result += New RankView With {
                    .OTUs = gg.ToArray(Function(x) x.OTU),
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
