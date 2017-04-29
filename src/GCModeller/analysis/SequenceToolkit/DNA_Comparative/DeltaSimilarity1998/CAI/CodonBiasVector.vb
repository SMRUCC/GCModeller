Imports System.Xml.Serialization
Imports Microsoft.VisualBasic.Mathematical

Namespace DeltaSimilarity1998.CAI

    Public Structure CodonBiasVector

        <XmlAttribute> Dim Codon As String
        <XmlAttribute> Dim XY#, YZ#, XZ#

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