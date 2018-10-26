Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel

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
    Dim RNA As NamedValue(Of RNATypes)
    Dim polypeptide As String

    Public ReadOnly Property IsRNAGene As Boolean
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Get
            Return Not polypeptide.StringEmpty
        End Get
    End Property

    Public ReadOnly Property RNAName As String
        Get
            Return $"{geneID}::{RNA.Value.Description}"
        End Get
    End Property

    Public Overrides Function ToString() As String
        If IsRNAGene Then
            Return {geneID, RNAName, polypeptide}.JoinBy(" => ")
        Else
            Return {geneID, RNAName}.JoinBy(" -> ")
        End If
    End Function
End Structure

Public Enum RNATypes As Byte
    mRNA = 0
    tRNA
    ribosomalRNA
End Enum

''' <summary>
''' Protein Modification
''' 
''' ``{polypeptide} + compounds -> protein``
''' </summary>
Public Structure Protein

    Dim polypeptides As String()
    Dim compounds As String()

    ''' <summary>
    ''' 这个蛋白质是由一条多肽链所构成的
    ''' </summary>
    ''' <param name="proteinId"></param>
    Sub New(proteinId As String)
        polypeptides = {proteinId}
    End Sub

End Structure