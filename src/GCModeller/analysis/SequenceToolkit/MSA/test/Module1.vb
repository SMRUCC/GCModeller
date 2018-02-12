Imports SMRUCC.genomics.Analysis.SequenceTools.MSA

Module Module1

    Sub Main()
        Dim seq$() = "GATGTGCCG
GATGTGCAG
CCGCTAGCAG
CCTGCTGCAG
CCTGTAGG".lTokens

        Dim msa = seq.MultipleAlignment()

        Pause()
    End Sub
End Module
