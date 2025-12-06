Imports System.Runtime.CompilerServices

Namespace Kmers

    Public Class SequenceHit : Inherits SequenceSource

        Public Property reads_title As String
        Public Property identities As Double
        Public Property total As Double
        Public Property score As Double
        Public Property ratio As Double

        Sub New()
        End Sub

        Sub New(info As SequenceSource)
            id = info.id
            ncbi_taxid = info.ncbi_taxid
            name = info.name
            accession_id = info.accession_id
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Unknown() As SequenceHit
            Return New SequenceHit With {
                .name = "Unknown"
            }
        End Function

    End Class
End Namespace