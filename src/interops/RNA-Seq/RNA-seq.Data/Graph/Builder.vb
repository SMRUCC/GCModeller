Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.SequenceModel.FQ

Namespace Graph

    Public MustInherit Class Builder

        Sub New(reads As IEnumerable(Of FastQ))
            Call ProcessReads(reads)
        End Sub

        Protected MustOverride Sub ProcessReads(reads As IEnumerable(Of FastQ))

        Public MustOverride Function Create() As NetworkGraph

    End Class
End Namespace