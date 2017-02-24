Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Clusters

    ''' <summary>
    ''' Using first token in the fasta title as the sequence uid
    ''' </summary>
    ''' <param name="source"></param>
    ''' <param name="delimiter$"></param>
    <Extension>
    Public Sub FirstTokenID(ByRef source As FastaFile, Optional delimiter$ = FastaToken.DefaultHeaderDelimiter)
        Dim tokens As Func(Of FastaToken, String())

        If delimiter = FastaToken.DefaultHeaderDelimiter Then
            tokens = Function(f) {
                f.Attributes(Scan0)
            }
        Else
            tokens = Function(f) {
                Strings.Split(f.Title, delimiter)(Scan0)
            }
        End If

        For Each f As FastaToken In source
            f.Attributes = tokens(f)
        Next
    End Sub
End Module
