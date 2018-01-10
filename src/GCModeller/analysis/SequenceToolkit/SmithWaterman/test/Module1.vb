Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()
        Dim s1 As New FastaToken("ATGCCCCCCCCCCTGGGAAAAAAAATGCCCACCCCTTTAA", "1")
        Dim s2 As New FastaToken("CCCTGGGAAAAAAAATGCCCCTGGGAAATCCTTTAAAAA", "2")
        Dim align As New SmithWaterman(s1, s2)
        Dim result = align.GetOutput(0.6, 6)

        Pause()
    End Sub
End Module
