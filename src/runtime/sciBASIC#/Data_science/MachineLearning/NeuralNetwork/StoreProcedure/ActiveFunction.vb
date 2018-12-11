
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.MachineLearning.NeuralNetwork.Activations

Namespace NeuralNetwork.StoreProcedure

    Public Class ActiveFunction

        ''' <summary>
        ''' The function name
        ''' </summary>
        ''' <returns></returns>
        Public Property Name As String
        Public Property Arguments As Dictionary(Of String, Double)

        Public ReadOnly Property [Function]() As IActivationFunction
            Get
                With Arguments
                    Select Case Name
                        Case NameOf(Activations.BipolarSigmoidFunction)
                            Return New BipolarSigmoidFunction(!alpha)
                        Case NameOf(Activations.Sigmoid)
                            Return New Sigmoid
                        Case NameOf(Activations.SigmoidFunction)
                            Return New SigmoidFunction(!alpha)
                        Case NameOf(Activations.ThresholdFunction)
                            Return New ThresholdFunction
                        Case Else
#If DEBUG Then
                            Call "".Warning
#End If
                            Return New Activations.Sigmoid
                    End Select
                End With
            End Get
        End Property

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Function ToString() As String
            Return $"{Name}({Arguments.Select(Function(a) $"{a.Key}:={a.Value}").JoinBy(", ")})"
        End Function
    End Class
End Namespace