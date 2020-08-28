Namespace SVM
    ' java: information about solution except alpha,
    ' because we cannot return multiple values otherwise...
    Public Class SolutionInfo
        Public Property obj As Double
        Public Property rho As Double
        Public Property upper_bound_p As Double
        Public Property upper_bound_n As Double
        Public Property r As Double ' for Solver_NU
    End Class
End Namespace