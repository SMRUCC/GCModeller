Namespace Framework.Optimization

    Public Class SteepestDescentFit(Of T)

        ReadOnly predict As Func(Of Double(), T, Double)
        ReadOnly dims As Integer
        ReadOnly dx As Func(Of Double(), Double, Double)()
        ReadOnly init As Func(Of T)
        ReadOnly update As Action(Of T, Double())

        Private Sub ComputeGradients(fit As T, xValues As Double()(), yValues As Double(), ByRef grad As Double(), ByRef errors As Double())
            grad = New Double(dims - 1) {}
            errors = New Double(xValues.Length - 1) {}

            For i As Integer = 0 To xValues.Length - 1
                Dim [error] As Double = yValues(i) - predict(xValues(i), fit)

                For j As Integer = 0 To grad.Length - 1
                    grad(j) += dx(j)(xValues(i), [error])
                Next

                errors(i) = [error]
            Next
        End Sub

        Public Function SteepestDescent(xValues As Double()(), yValues As Double(),
                                        Optional iterations As Integer = 1000,
                                        Optional learningRate As Double = 0.05) As T
            Dim t As T = init()
            Dim grad As Double() = Nothing
            Dim loss As New List(Of Double)
            Dim errors As Double() = Nothing

            For iter As Integer = 0 To iterations - 1
                Call ComputeGradients(t, xValues, yValues, grad, errors)
                Call update(t, SIMD.Multiply.f64_scalar_op_multiply_f64(learningRate, grad))
                Call loss.Add(errors.Sum(Function(a) a ^ 2))
            Next

            Return t
        End Function
    End Class
End Namespace