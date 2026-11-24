Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization
Imports SMRUCC.genomics.SequenceModel

Namespace DeltaSimilarity1998.CAI

    ''' <summary>
    ''' The site-specific codon signature triple vector of one codon XYZ:
    ''' 
    ''' ```
    '''    XY = rhoXY(1, 2) = fXY(1, 2)/fX(1)fY(2)
    '''    YZ = rhoYZ(2, 3) = fYZ(2, 3)/fY(2)fZ(3)
    '''    XZ = rhoXZ(1, 3) = fXZ(1, 3)/fX(1)fZ(3)
    ''' ```
    ''' 
    ''' (the three components are computed from a <see cref="CodonSignatureProfile"/>, 
    ''' see <see cref="CodonSignatureProfile.CodonBiasVector"/>)
    ''' </summary>
    Public Structure CodonBiasVector

        ''' <summary>
        ''' 三联体密码子
        ''' </summary>
        <XmlAttribute> Dim Codon As String
        <XmlAttribute> Dim XY#, YZ#, XZ#

        Public Overrides Function ToString() As String
            Return $"{Codon} -> (rhoXY(1,2)={XY:F3}, rhoYZ(2,3)={YZ:F3}, rhoXZ(1,3)={XZ:F3})"
        End Function

        ''' <summary>
        ''' 简单的产生三个残基单元产生的Triple片段对象
        ''' </summary>
        ''' <param name="seq"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function PopulateTriples(seq As SeqTypes) As IEnumerable(Of String)
            Return PopulateTriples(vec:=seq.GetVector)
        End Function

        ''' <summary>
        ''' 简单的产生三个残基单元产生的Triple片段对象
        ''' </summary>
        ''' <returns></returns>
        Public Shared Iterator Function PopulateTriples(vec As IReadOnlyCollection(Of Char)) As IEnumerable(Of String)
            For Each i As Char In vec
                For Each j As Char In vec
                    For Each k As Char In vec
                        Yield New String({i, j, k})
                    Next
                Next
            Next
        End Function

    End Structure
End Namespace
