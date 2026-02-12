Imports Microsoft.VisualBasic.DataMining.BinaryTree
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns

Public Class MotifTree

    Dim clusters As BTreeCluster()

    Public Shared Function MakeTree(motifs As IEnumerable(Of Probability), equals As Double, gt As Double) As MotifTree
        Dim motifSet As New MotifComparison(motifs, equals, gt)
        Dim tree As BTreeCluster = motifSet.motifIDs.BTreeCluster(alignment:=motifs)
        Dim clusters As New List(Of BTreeCluster)

        Call BTreeCluster.PullAllClusterNodes(tree, pull:=clusters)

        Return New MotifTree With {
            .clusters = clusters.ToArray
        }
    End Function

End Class
