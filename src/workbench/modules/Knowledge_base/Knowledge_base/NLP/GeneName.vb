Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree

Public Module GeneName

    <Extension>
    Public Iterator Function GroupBy(genes As IEnumerable(Of EntityObject), field As String, Optional cutoff As Double = 0.8) As IEnumerable(Of NamedCollection(Of EntityObject))
        Dim tree As New AVLTree(Of String, String)(AddressOf New TextSimilar(cutoff).Compare)
        Dim gene_id As New Dictionary(Of String, EntityObject)

        For Each gene As EntityObject In genes
            gene_id.Add(gene.ID, gene)
            tree.Add(gene(field), gene.ID)
        Next


    End Function

    Private Class TextSimilar

        ReadOnly cutoff As Double

        Sub New(cutoff As Double)
            Me.cutoff = cutoff
        End Sub

        Public Function Compare(s1 As String, s2 As String) As Integer

        End Function

    End Class
End Module
