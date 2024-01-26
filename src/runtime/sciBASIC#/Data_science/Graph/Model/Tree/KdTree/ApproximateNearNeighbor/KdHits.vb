Namespace KdTree.ApproximateNearNeighbor

    Public Structure KdHits

        Dim size As Integer
        Dim indices As Integer()
        Dim weights As Double()

        Sub New(size As Integer, indices As Integer(), weights As Double())
            Me.size = size
            Me.indices = indices
            Me.weights = weights
        End Sub

    End Structure
End Namespace