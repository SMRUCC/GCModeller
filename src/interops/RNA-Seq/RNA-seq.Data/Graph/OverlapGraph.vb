Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.SequenceModel.FQ

Namespace Graph

    Public Class OverlapGraph : Inherits Builder

        Public Sub New(reads As IEnumerable(Of FastQ))
            MyBase.New(reads)
        End Sub

        Protected Overrides Sub ProcessReads(reads As IEnumerable(Of FQ.FastQ))
            Dim edge As Edge = Nothing

            For Each read As FastQ In reads
                g.CreateNode(read.SEQ_ID).data("reads") = read.SequenceData
            Next

            For Each u As Node In TqdmWrapper.Wrap(g.vertex.ToArray)
                For Each v As Node In g.vertex
                    If u Is v Then
                        Continue For
                    End If


                Next
            Next
        End Sub
    End Class
End Namespace