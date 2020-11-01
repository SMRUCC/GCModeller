Imports System
Imports System.Numerics

Namespace Utilities
    ' Based on R_ext/Complex.h definition
    Friend Structure Rcomplex
        Public r As Double
        Public i As Double
    End Structure

    ''' <summary>
    ''' An internal helper class to convert types of arrays, primarily for data operations necessary for .NET types to/from R concepts.
    ''' </summary>
    Public Module RTypesUtil
        ''' <summary> Serialize an array of complex numbers to 
        '''           an array of doubles, alternating real and imaginary values</summary>
        '''
        ''' <param name="values"> The complex values to serialize</param>
        '''
        ''' <returns> A double[].</returns>
        Public Function SerializeComplexToDouble(ByVal values As Complex()) As Double()
            Dim data = New Double(2 * values.Length - 1) {}

            For i = 0 To values.Length - 1
                data(2 * i) = values(i).Real
                data(2 * i + 1) = values(i).Imaginary
            Next

            Return data
        End Function

        Friend Function SerializeComplexToRComplex(ByVal value As Complex) As Rcomplex
            Dim data = New Rcomplex() With {
                .r = value.Real,
                .i = value.Imaginary
            }
            Return data
        End Function

        ''' <summary> Deserialize complex from double.</summary>
        '''
        ''' <exception cref="ArgumentException"> input length is not divisible by 2 </exception>
        '''
        ''' <param name="data"> The serialised complex values, even indexes are real and odd ones imaginary</param>
        '''
        ''' <returns> A Complex[].</returns>
        Public Function DeserializeComplexFromDouble(ByVal data As Double()) As Complex()
            Dim dblLen = data.Length
            If dblLen Mod 2 <> 0 Then Throw New ArgumentException("Serialised definition of complexes must be of even length")
            Dim n As Integer = dblLen / 2
            Dim res = New Complex(n - 1) {}

            For i = 0 To n - 1
                res(i) = New Complex(data(2 * i), data(2 * i + 1))
            Next

            Return res
        End Function
    End Module
End Namespace
