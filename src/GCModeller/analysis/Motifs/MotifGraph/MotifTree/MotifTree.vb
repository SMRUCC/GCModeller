Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns

Public Class MotifTree

    Dim motifs As MotifComparison

    Sub New(motifs As IEnumerable(Of Probability), equals As Double, gt As Double)
        Me.motifs = New MotifComparison(motifs, equals, gt)
    End Sub

    Public Function MakeTree()
        Dim tree As AVLClusterTree(Of Probability)

    End Function

End Class
