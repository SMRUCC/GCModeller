Imports System.Linq

''' <summary>
''' Smith-Waterman local alignment algorithm.
'''
''' Design Note: this class implements AminoAcids interface: a simple fix customized to amino acids, since that is all we deal with in this class
''' Supporting both DNA and Aminoacids, will require a more general design.
''' </summary>
Public Class SmithWaterman : Inherits GSW(Of Char)

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="query"></param>
    ''' <param name="subject"></param>
    ''' <param name="blosum">
    ''' If the matrix parameter is null, then the default build in blosum62 matrix will be used.
    ''' </param>
    Sub New(query As String, subject As String, Optional blosum As Blosum = Nothing)
        Call MyBase.New(query.ToArray, subject.ToArray, __blosum(blosum), Function(x) x)
    End Sub

    Private Shared Function __blosum(input As Blosum) As ISimilarity(Of Char)
        If input Is Nothing Then
            input = Blosum.FromInnerBlosum62
        End If

        Return AddressOf input.getDistance
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="cutoff">0%-100%</param>
    ''' <returns></returns>
    Public Function GetOutput(cutoff As Double, minW As Integer) As Output
        Return Output.CreateObject(Me, Function(x) x, cutoff, minW)
    End Function

    Public Shared Function Align(query As SequenceModel.FASTA.FastaToken,
                                 subject As SequenceModel.FASTA.FastaToken,
                                 Optional blosum As Blosum = Nothing) As SmithWaterman
        Dim sw As New SmithWaterman(query.SequenceData, subject.SequenceData, blosum)
        Return sw
    End Function
End Class
