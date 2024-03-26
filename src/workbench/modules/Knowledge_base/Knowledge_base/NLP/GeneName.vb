Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.DataMining.BinaryTree

Public Module GeneName

    <Extension>
    Public Iterator Function GroupBy(genes As IEnumerable(Of EntityObject), field As String, Optional cutoff As Double = 0.8) As IEnumerable(Of NamedCollection(Of EntityObject))
        Dim tree As New AVLTree(Of String, String)(New TextSimilar(cutoff).GetComparer)
        Dim gene_id As New Dictionary(Of String, EntityObject)

        For Each gene As EntityObject In genes
            gene_id.Add(gene.ID, gene)
            tree.Add(gene(field), gene.ID)
        Next


    End Function

    Private Class TextSimilar : Inherits ComparisonProvider

        Sub New(cutoff As Double)
            Call MyBase.New(cutoff, cutoff / 2)
        End Sub

        Public Overrides Function GetSimilarity(x As String, y As String) As Double

        End Function

        Public Overrides Function GetObject(id As String) As Object
            Return id
        End Function
    End Class
End Module
