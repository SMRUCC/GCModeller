Namespace KdTree

    Public Class KdNodeHeapItem(Of T)

        Public Property node As KdTreeNode(Of T)
        Public Property distance As Double

        Sub New(node As KdTreeNode(Of T), dist As Double)
            Me.node = node
            Me.distance = dist
        End Sub

    End Class
End Namespace