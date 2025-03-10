Namespace Framework.Optimization

    Public MustInherit Class OptimizationObject

        Public ReadOnly Property GradientDims As Integer

        Public MustOverride Function PartialDerivative(x As Double(), err As Double) As Double()
        Public MustOverride Function Predict(x As Double()) As Double
        Public MustOverride Sub Update(gradient As Double())
        Public MustOverride Sub AddLoss(errors As Double())

    End Class

    Public Class SteepestDescentFit(Of T As {New, OptimizationObject})

        Private Sub ComputeGradients(fit As T, xValues As Double()(), yValues As Double(), ByRef grad As Double(), ByRef errors As Double())
            grad = New Double(fit.GradientDims - 1) {}
            errors = New Double(xValues.Length - 1) {}

            For i As Integer = 0 To xValues.Length - 1
                Dim [error] As Double = yValues(i) - fit.Predict(xValues(i))
                Dim pdx As Double() = fit.PartialDerivative(xValues(i), [error])

                grad = SIMD.Add.f64_op_add_f64(pdx, grad)
                errors(i) = [error]
            Next
        End Sub

        Public Function SteepestDescent(xValues As Double()(), yValues As Double(),
                                        Optional iterations As Integer = 1000,
                                        Optional learningRate As Double = 0.05) As T
            Dim t As T = New T()
            Dim grad As Double() = Nothing
            Dim errors As Double() = Nothing

            For iter As Integer = 0 To iterations - 1
                Call ComputeGradients(t, xValues, yValues, grad, errors)
                Call t.Update(SIMD.Multiply.f64_scalar_op_multiply_f64(learningRate, grad))
                Call t.AddLoss(errors)
            Next

            Return t
        End Function
    End Class
End Namespace