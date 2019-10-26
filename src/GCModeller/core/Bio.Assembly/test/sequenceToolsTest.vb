Imports SMRUCC.genomics.ComponentModel.Loci
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Module sequenceToolsTest
    Sub Main()
        Dim nt As New FastaSeq With {.Headers = {"ABC"}, .SequenceData = "abcdefghijklmnopqrstuvwxyz"}
        Dim cut = nt.CutSequenceLinear(New Location With {.left = 1, .right = 7})
        Dim cut2 = nt.CutSequenceLinear(New Location With {.left = 1, .right = 25})
        Dim cut3 = nt.CutSequenceLinear(New Location With {.left = 1, .right = 26})
        Dim cut4 = nt.CutSequenceLinear(New Location With {.left = 1, .right = 30})
        Dim cut5 = nt.CutSequenceLinear(New Location With {.left = 30, .right = 32})

        Pause()
    End Sub
End Module
