#Region "Microsoft.VisualBasic::5023317eeb843b66d04626cdf6054292, analysis\Metagenome\Metagenome\gast\TaxonomyTree.vb"

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

    '     Class TaxonomyTree
    ' 
    '         Properties: Childs, hits, Lineage, Parent, TreeRoot
    ' 
    '         Constructor: (+3 Overloads) Sub New
    ' 
    '         Function: BuildTree, ToString
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

    ''' <summary>
    ''' Tree node, (<see cref="Taxonomy.consensus(Taxonomy(), Double)"/>对象存在bug，当最低百分比低于50%的时候，
    ''' 会造成重复的同等级分类出现，使用这个树对象来避免这种情况的发生)
    ''' </summary>
    Public Class TaxonomyTree : Inherits Taxonomy

        Public Property Childs As New List(Of TaxonomyTree)
        Public Property Parent As TaxonomyTree
        Public Property Lineage As String
        ''' <summary>
        ''' Count of the hits numbers on this node rank 
        ''' </summary>
        ''' <returns></returns>
        Public Property hits As Integer

        Public ReadOnly Property TreeRoot As TaxonomyTree
            Get
                If Parent Is Nothing OrElse Parent.Lineage = "*" Then
                    Return Me
                Else
                    Return Parent.TreeRoot
                End If
            End Get
        End Property

        Sub New(taxonomy As String)
            Call MyBase.New(taxonomy)
        End Sub

        Sub New()
            Call MyBase.New({})
        End Sub

        Private Sub New(copy As TaxonomyTree)
            For i As Integer = 0 To 7
                Me(i) = copy(i)
            Next

            Lineage = copy.Lineage
            Parent = copy
        End Sub

        Public Overrides Function ToString() As String
            Dim lineage$ = MyBase.ToString.Trim(";"c)
            Dim rank As TaxonomyRanks

            Call GetDepth(rank)

            If lineage.StringEmpty AndAlso Me.Lineage = "*" Then
                lineage = "*"
            End If

            Return $"{lineage} ({rank.ToString}={hits})"
        End Function

        Shared ReadOnly DescRanks$() = NcbiTaxonomyTree.stdranks.Reverse.ToArray

        ''' <summary>
        ''' Alignment hits by blastn
        ''' </summary>
        ''' <param name="hits"></param>
        ''' <returns></returns>
        Public Shared Function BuildTree(hits As IEnumerable(Of Metagenomics.Taxonomy), ByRef taxa_counts%(), ByRef minrank$) As TaxonomyTree
            Dim array As Dictionary(Of String, String)() = hits _
                .Select(Function(tax)
                            Return tax.CreateTable.Value
                        End Function) _
                .ToArray
            Dim root As New TaxonomyTree With {
                .Lineage = "*",
                .Childs = New List(Of TaxonomyTree),
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

        Private Shared Sub Split(root As TaxonomyTree, hits As Dictionary(Of String, String)(), i%)
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
                        .Childs = New List(Of TaxonomyTree)
                    }

                    With g.First
                        append(level) = .Key
                        append.Lineage &= ";" & .Key
                        append.hits = .Count
                    End With

                    walk.Childs.Add(append)
                    walk = append
                Else
                    ' 树分叉了，则添加新的节点
                    For Each subType As IGrouping(Of String, String) In g
                        Dim append As New TaxonomyTree(walk) With {
                            .Childs = New List(Of TaxonomyTree)
                        }
                        append(level) = subType.Key
                        append.Lineage &= ";" & subType.Key
                        append.hits = subType.Count

                        walk.Childs.Add(append)

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
    End Class
End Namespace
