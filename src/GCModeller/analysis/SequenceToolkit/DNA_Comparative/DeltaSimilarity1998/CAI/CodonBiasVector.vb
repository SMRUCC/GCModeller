Imports Microsoft.VisualBasic.Mathematical

Namespace DeltaSimilarity1998

    Public Structure CodonBiasVector
        Dim XY#, YZ#, XZ#
        Dim Codon As String

        ''' <summary>
        ''' 对Profile进行归一化处理
        ''' </summary>
        ''' <returns></returns>
        Public Function EuclideanNormalization() As Double
            Return {XY, YZ, XZ}.EuclideanDistance
        End Function

        Public Overrides Function ToString() As String
            Return $"{Codon} -> (pXY={XY}, pYZ={YZ}, pXZ={XZ})"
        End Function
    End Structure
End Namespace