Namespace Core

    Public Class Boundary

        Public Property forward As Double
        Public Property reverse As Double

        Public Overrides Function ToString() As String
            Return $"[reactant <- {reverse} | {forward} -> product]"
        End Function

        Public Shared Widening Operator CType(range As Double()) As Boundary
            Return New Boundary With {.forward = range(0), .reverse = range(1)}
        End Operator

        Public Shared Widening Operator CType(range As Integer()) As Boundary
            Return New Boundary With {.forward = range(0), .reverse = range(1)}
        End Operator

    End Class
End Namespace