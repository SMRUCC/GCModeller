Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.StoreProcedure
Imports sys = System.Math

Namespace NeuralNetwork.Activations

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks>
    ''' ```
    '''         e ^ x - e ^ -x
    ''' f(x) = -----------------
    '''         e ^ x + e ^ -x
    ''' 
    ''' ```
    ''' </remarks>
    <Serializable>
    Public Class HyperbolicTangent : Inherits IActivationFunction

        Public Overrides ReadOnly Property Store As ActiveFunction
            Get
                Return New ActiveFunction() With {
                    .Arguments = {},
                    .name = NameOf(HyperbolicTangent)
                }
            End Get
        End Property

        ''' <summary>
        ''' 这个函数接受的参数应该是一个弧度值
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Overrides Function [Function](x As Double) As Double
            Dim a = Math.E ^ x
            Dim b = Math.E ^ (-x)

            Return (a - b) / (a + b)
        End Function

        ''' <summary>
        ''' 这个函数所接受的参数也是一个弧度值
        ''' </summary>
        ''' <param name="x"></param>
        ''' <returns></returns>
        Public Overrides Function Derivative(x As Double) As Double
            Return 1 / (sys.Cosh([Function](x)) ^ 2)
        End Function

        Public Overrides Function Derivative2(y As Double) As Double
            Return 1 / (sys.Cosh(y) ^ 2)
        End Function

        Public Overrides Function ToString() As String
            Return Store.ToString
        End Function
    End Class
End Namespace