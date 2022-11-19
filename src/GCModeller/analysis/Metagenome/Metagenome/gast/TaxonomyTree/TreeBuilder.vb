#Region "Microsoft.VisualBasic::825d83e14719a6617f6fdeec4ba2e883, GCModeller\analysis\Metagenome\Metagenome\gast\TaxonomyTree\TreeBuilder.vb"

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

    '   Total Lines: 103
    '    Code Lines: 77
    ' Comment Lines: 9
    '   Blank Lines: 17
    '     File Size: 3.88 KB


    '     Module TreeBuilder
    ' 
    '         Function: BuildTree
    ' 
    '         Sub: Split
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Language
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy
Imports SMRUCC.genomics.Metagenomics

Namespace gast

    Module TreeBuilder

        ReadOnly DescRanks$() = NcbiTaxonomyTree.stdranks.Objects.Reverse.ToArray

        ''' <summary>
        ''' Alignment hits by blastn
        ''' </summary>
        ''' <param name="hits"></param>
        ''' <returns></returns>
        Public Function BuildTree(hits As IEnumerable(Of Metagenomics.Taxonomy), ByRef taxa_counts%(), ByRef minrank$) As TaxonomyTree
            Dim array As Dictionary(Of String, String)() = hits _
                .Select(Function(tax)
                            Return tax.CreateTable.Value
                        End Function) _
                .ToArray
            Dim root As New TaxonomyTree With {
                .lineage = "*",
                .childs = New List(Of TaxonomyTree),
                .hits = array.Length
            }

            Call Split(root, array, i:=0)

            taxa_counts = DescRanks _
                .Select(Function(rank)
                            Return array _
                                .Select(Function(t) t(rank)) _
                                .Distinct _
                                .Count
                        End Function) _
                .ToArray

            For Each rank As String In NcbiTaxonomyTree.stdranks
                If array.Any(Function(t) Not t(rank).TaxonomyRankEmpty) Then
                    minrank = rank
                    Exit For
                End If
            Next

            Return root
        End Function

        Private Sub Split(root As TaxonomyTree, hits As Dictionary(Of String, String)(), i%)
            Dim walk As TaxonomyTree = root

            For level As Integer = i To DescRanks.Length - 1
                Dim rank As String = DescRanks(level)
                Dim g = hits _
                    .Select(Function(t) t(rank)) _
                    .GroupBy(Function(s) s) _
                    .Where(Function(t) Not t.Key.TaxonomyRankEmpty) _
                    .ToArray

                If g.Length = 1 Then
                    ' 继续延伸当前的树
                    Dim append As New TaxonomyTree(walk) With {
                        .childs = New List(Of TaxonomyTree)
                    }

                    With g.First
                        append(level) = .Key
                        append.lineage &= ";" & .Key
                        append.hits = .Count
                        append.ncbi_taxid = hits.First(Function(a) a(rank) = .Key).TryGetValue(NameOf(Taxonomy.ncbi_taxid))
                    End With

                    walk.childs.Add(append)
                    walk = append
                Else
                    ' 树分叉了，则添加新的节点
                    For Each subType As IGrouping(Of String, String) In g
                        Dim append As New TaxonomyTree(walk) With {
                            .childs = New List(Of TaxonomyTree)
                        }
                        append(level) = subType.Key
                        append.lineage &= ";" & subType.Key
                        append.hits = subType.Count

                        walk.childs.Add(append)

                        Dim subHits = hits _
                            .Where(Function(tax)
                                       Return tax(rank) = subType.Key
                                   End Function) _
                            .ToArray

                        Call Split(root:=append, hits:=subHits, i:=level + 1)
                    Next

                    ' 剩余的数据已经通过Split函数添加了，已经没有在这里进行添加的必要了
                    ' 跳出循环
                    Exit For
                End If
            Next
        End Sub
    End Module
End Namespace
