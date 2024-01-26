Namespace KdTree.ApproximateNearNeighbor

    Public Structure TagVector

        Dim index As Integer
        Dim vector As Double()
        Dim tag As String

        Public ReadOnly Property size As Integer
            Get
                Return vector.Length
            End Get
        End Property

        Public Overrides Function ToString() As String
            Return $"[{index}] {vector.Take(6).JoinBy(", ")}..."
        End Function

    End Structure
End Namespace