Imports SMRUCC.genomics.Analysis.SequenceAlignment.MSA
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace FamilyCluster

    ''' <summary>
    ''' a single protein family discovered by the unsupervised clustering pipeline
    ''' </summary>
    Public Class ProteinFamily

        ''' <summary>
        ''' the integer family id assigned by the Louvain community detection step
        ''' </summary>
        Public Property familyId As Integer

        ''' <summary>
        ''' the titles of every member protein sequence
        ''' </summary>
        Public Property members As String()

        ''' <summary>
        ''' the member protein sequences (title + sequence data)
        ''' </summary>
        Public Property memberSequences As FastaSeq()

        ''' <summary>
        ''' the selected reference sequence: the member with the fewest edits in the MSA
        ''' </summary>
        Public Property reference As FastaSeq

        ''' <summary>
        ''' the multiple sequence alignment of the family (may be nothing if the family has a single member)
        ''' </summary>
        Public Property msa As MSAOutput

        Public Overrides Function ToString() As String
            If reference Is Nothing Then
                Return $"family_{familyId} ({members.Length} members)"
            Else
                Return $"family_{familyId} ({members.Length} members, ref={reference.Title})"
            End If
        End Function
    End Class
End Namespace
