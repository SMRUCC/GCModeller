Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports SMRUCC.genomics.Assembly.NCBI.Taxonomy

Namespace gast

    ''' <summary>
    ''' Tree node, (<see cref="Taxonomy.consensus(Taxonomy(), Double)"/>对象存在bug，当最低百分比低于50%的时候，
    ''' 会造成重复的同等级分类出现，使用这个树对象来避免这种情况的发生)
    ''' </summary>
    Public Class TaxonomyTree : Inherits Taxonomy

        Public Property Childs As New List(Of TaxonomyTree)
        Public Property Lineage As String

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
        End Sub

        Public Shared Function AssignTaxonomy(tree As TaxonomyTree) As gastOUT

        End Function

        Shared ReadOnly DescRanks$() = NcbiTaxonomyTree.stdranks.Reverse.ToArray

        ''' <summary>
        ''' Alignment hits by blastn
        ''' </summary>
        ''' <param name="hits"></param>
        ''' <returns></returns>
        Public Shared Function BuildTree(hits As IEnumerable(Of Metagenomics.Taxonomy)) As TaxonomyTree
            Dim root As New TaxonomyTree With {
                .Lineage = "*",
                .Childs = New List(Of TaxonomyTree)
            }
            Dim array As Dictionary(Of String, String)() = hits _
                .Select(Function(tax)
                            Return tax.CreateTable.Value
                        End Function) _
                .ToArray

            Call Split(root, array, i:=0)

            Return root
        End Function

        Private Shared Sub Split(root As TaxonomyTree, hits As Dictionary(Of String, String)(), i%)
            Dim walk As TaxonomyTree = root

            For level As Integer = i To 8
                Dim rank As String = DescRanks(level)
                Dim g = hits _
                    .Select(Function(t) t(rank)) _
                    .GroupBy(Function(s) s) _
                    .ToArray

                If g.Length = 1 Then
                    ' 继续延伸当前的树
                    Dim append As New TaxonomyTree(walk) With {
                        .Childs = New List(Of TaxonomyTree)
                    }
                    append.Lineage &= ";" & g.First.Key
                    walk.Childs.Add(append)
                    walk = append
                Else
                    ' 树分叉了，则添加新的节点
                    For Each subType In g
                        Dim append As New TaxonomyTree(walk) With {
                            .Childs = New List(Of TaxonomyTree)
                        }
                        append.Lineage &= ";" & subType.Key
                        walk.Childs.Add(append)

                        Dim subHits = hits _
                            .Where(Function(tax) tax(rank) = subType.Key) _
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