Imports Microsoft.VisualBasic.MachineLearning.ComponentModel.Activations
Imports Microsoft.VisualBasic.MachineLearning.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix

Public Class VariationalEncoder

    Dim linear1 As Linear
    Dim linear2 As Linear
    Dim linear3 As Linear
    Dim t2 As NumericMatrix
    Dim t3 As NumericMatrix

    Public ReadOnly Property kl As Double

    Sub New(latent_dims As Integer)
        linear1 = New Linear(784, 512)
        linear2 = New Linear(512, latent_dims)
        linear3 = New Linear(512, latent_dims)
        t2 = NumericMatrix.One(784, 512)
        t3 = NumericMatrix.One(784, 512)
    End Sub

    Public Function forward(x As Vector) As Vector
        x = ReLU.ReLU(linear1.Fit(x))

        Dim mu = linear2.Fit(x)
        Dim sigma = linear3.Fit(x)

        sigma(sigma < 0) = Vector.Scalar(0.0001)
        sigma = sigma.Log

        Dim z = mu + sigma * Vector.norm(size:=mu.Length, mu:=0, sigma:=1)
        _kl = (sigma ^ 2 + mu ^ 2 - sigma.Log - 1 / 2).Sum
        Return z
    End Function

    Public Sub backward(loss As Vector)
        Call linear1.backward(loss)
        Call linear2.backward(t2.DotMultiply(loss))
        Call linear3.backward(t3.DotMultiply(loss))
    End Sub
End Class
