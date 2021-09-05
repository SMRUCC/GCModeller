Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Net.Http

Namespace LinearAlgebra.LinearProgramming

    Public Class LPPModel : Inherits XmlDataModel

        Public Property objectiveFunctionType As String
        Public Property variableNames As String()
        Public Property objectiveFunctionCoefficients As Double()
        ''' <summary>
        ''' base64 string represented matrix
        ''' </summary>
        ''' <returns></returns>
        Public Property constraintCoefficients As String()
        Public Property constraintTypes As String()
        Public Property constraintRightHandSides As Double()
        Public Property objectiveFunctionValue As Double

        ''' <summary>
        ''' the model name
        ''' </summary>
        ''' <returns></returns>
        Public Property name As String

        Sub New()
        End Sub

        Sub New(matrix As IEnumerable(Of Double()))
            constraintCoefficients = matrix _
                .Select(Function(v)
                            Return v _
                                .Select(Function(d) BitConverter.GetBytes(d)) _
                                .IteratesALL _
                                .ToBase64String
                        End Function) _
                .ToArray
        End Sub

        Public Function ParseMatrix() As Double()()
            Return constraintCoefficients _
                .Select(Function(base64Str)
                            Dim bytes As Byte() = base64Str.Base64RawBytes
                            Dim chunks = bytes.Split(8)

                            Return chunks _
                                .Select(Function(v) BitConverter.ToDouble(v, Scan0)) _
                                .ToArray
                        End Function) _
                .ToArray
        End Function

    End Class
End Namespace