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
            Dim root As TaxonomyTree

            For Each lineage As String In taxonomy
                Dim node As New TaxonomyTree(taxonomy:=lineage)

            Next
        End Function
    End Class
End Namespace