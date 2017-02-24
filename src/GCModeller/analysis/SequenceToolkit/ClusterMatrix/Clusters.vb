Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Clusters

    <Extension>
    Public Sub FirstTokenID(ByRef source As FastaFile, Optional delimiter$ = "|")
        If delimiter = FastaToken.DefaultHeaderDelimiter Then

        End If

        For Each f As FastaToken In source
            f.Attributes = {
                f.Attributes(Scan0)
            }
        Next
    End Sub
End Module
