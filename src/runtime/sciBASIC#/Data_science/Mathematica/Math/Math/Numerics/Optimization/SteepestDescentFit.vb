Imports Microsoft.VisualBasic.Parallel

Namespace Framework.Optimization

    Public MustInherit Class OptimizationObject

        Public MustOverride ReadOnly Property GradientDims As Integer

        Public MustOverride Function PartialDerivative(x As Double(), err As Double) As Double()
        Public MustOverride Function Predict(x As Double()) As Double
        Public MustOverride Sub Update(gradient As Double())
        Public MustOverride Sub AddLoss(errors As Double())

    End Class

    Public Class SteepestDescentFit(Of T As {New, OptimizationObject})

        Dim learningRate As Double
        Dim xValues As Double()()
        Dim yValues As Double()
        Dim errors As Double() = Nothing

        Private Sub ComputeGradients(fit As T, ByRef grad As Double())
            Dim task As New SDTask(Me, fit)

            grad = New Double(fit.GradientDims - 1) {}
            task.Run()

            For Each batch As Double() In task.batches
                grad = SIMD.Add.f64_op_add_f64(grad, batch)
            Next

            grad = SIMD.Multiply.f64_scalar_op_multiply_f64(learningRate, grad)
        End Sub

        Private Class SDTask : Inherits VectorTask

            Dim errors As Double()
            Dim fit As T
            Dim sd As SteepestDescentFit(Of T)

            Friend ReadOnly batches As Double()()

            Public Sub New(sdf As SteepestDescentFit(Of T), obj As T,
                           Optional verbose As Boolean = False,
                           Optional workers As Integer? = Nothing)

                MyBase.New(sdf.xValues.Length, verbose, workers)

                fit = obj
                errors = sd.errors
                sd = sdf
                batches = Allocate(Of Double())(all:=False)
            End Sub

            Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
                Dim grad As Double() = New Double(fit.GradientDims - 1) {}

                For i As Integer = start To ends
                    Dim [error] As Double = sd.yValues(i) - fit.Predict(sd.xValues(i))
                    Dim pdx As Double() = fit.PartialDerivative(sd.xValues(i), [error])

                    grad = SIMD.Add.f64_op_add_f64(pdx, grad)
                    errors(i) = [error]
                Next

                batches(cpu_id) = grad
            End Sub
        End Class

        Public Shared Function SteepestDescent(xValues As Double()(), yValues As Double(),
                                               Optional iterations As Integer = 1000,
                                               Optional learningRate As Double = 0.05) As T
            Dim t As T = New T()
            Dim grad As Double() = Nothing
            Dim sdf As New SteepestDescentFit(Of T) With {
                .learningRate = learningRate,
                .xValues = xValues,
                .yValues = yValues,
                .errors = New Double(xValues.Length - 1) {}
            }

            For iter As Integer = 0 To iterations - 1
                Call sdf.ComputeGradients(t, grad)
                Call t.Update(grad)
                Call t.AddLoss(sdf.errors)
            Next

            Return t
        End Function
    End Class
End Namespace