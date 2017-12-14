Imports Microsoft.VisualBasic.ComponentModel.Ranges

Namespace gast

    ''' <summary>
    ''' Tree node
    ''' </summary>
    Public Class TaxonomyTree : Inherits Taxonomy

        Public Property Childs As New List(Of TaxonomyTree)

        Sub New(taxonomy As String)
            Call MyBase.New(taxonomy)
        End Sub

        Public Shared Function BuildTree(taxonomy As IEnumerable(Of String)) As TaxonomyTree
            Dim root As TaxonomyTree = Nothing

            For Each lineage As String In taxonomy
                Dim node As New TaxonomyTree(taxonomy:=lineage)

                If root Is Nothing Then
                    root = node
                Else
                    Select Case node.CompareWith(root)
                        Case Relations.Include
                            ' 当前的节点包含了root节点，则将root接入当前节点之后替换掉root节点
                        Case Relations.Equals
                            ' 可能是相同的strain
                        Case Relations.Irrelevant
                            ' 可能有相同的大分类ranks，可能只是差异在小分类上的不同
                    End Select
                End If
            Next

            Return root
        End Function
    End Class
End Namespace