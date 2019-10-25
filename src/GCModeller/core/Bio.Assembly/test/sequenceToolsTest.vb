Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Module sequenceToolsTest
    Sub Main()
        Dim nt As New FastaSeq With {.Headers = {"ABC"}, .SequenceData = "abcdefghijklmnopqrstuvwxyz"}
        Dim cut = nt.CutSequenceLinear(New Location With {.left = 1, .right = 7})

        Pause()
    End Sub
End Module
