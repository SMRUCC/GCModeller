Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.SequenceModel.FQ

Namespace Graph

    Public MustInherit Class Builder

        Protected ReadOnly g As New NetworkGraph

        Public ReadOnly Property Graph As NetworkGraph
            Get
                Return g
            End Get
        End Property

        Sub New(reads As IEnumerable(Of FastQ))
            Call ProcessReads(reads)
        End Sub

        Protected MustOverride Sub ProcessReads(reads As IEnumerable(Of FastQ))

    End Class
End Namespace