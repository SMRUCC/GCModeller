Imports System.Runtime.CompilerServices

''' <summary>
''' Transcription and Translation
''' 
''' ```
''' CDS -> RNA
''' ORF -> mRNA -> polypeptide
''' ```
''' </summary>
Public Structure CentralDogma

    Dim geneID As String
    Dim RNA As String
    Dim polypeptide As String

    Public ReadOnly Property IsmRNA As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Not polypeptide.StringEmpty
        End Get
    End Property

    Public Overrides Function ToString() As String
        If IsmRNA Then
            Return {geneID, RNA, polypeptide}.JoinBy(" => ")
        Else
            Return {geneID, RNA}.JoinBy(" -> ")
        End If
    End Function
End Structure

''' <summary>
''' Protein Modification
''' 
''' ``{polypeptide} + compounds -> protein``
''' </summary>
Public Structure Protein

    Dim polypeptides As String()
    Dim compounds As String()

End Structure