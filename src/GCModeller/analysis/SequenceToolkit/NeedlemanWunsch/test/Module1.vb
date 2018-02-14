Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()
        Dim x As New FastaSeq With {.SequenceData = "ATGGCGATCGGGCTCCCCCAAAGGGTCAAAAG"}
        Dim y As New FastaSeq With {.SequenceData = "ATGCCGAACGGGCTCCCAAAATAAAGCAAAAG"}
        Dim result = RunNeedlemanWunsch.RunAlign(x, y, Console.Out)

        Pause()
    End Sub
End Module
