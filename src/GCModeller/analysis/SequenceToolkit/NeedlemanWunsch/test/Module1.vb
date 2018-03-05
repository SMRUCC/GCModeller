Imports SMRUCC.genomics.Analysis.SequenceTools
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()
        Call scoreTest2()
        Call maxScoreTest()


        Dim x As New FastaSeq With {.SequenceData = "ATGGCGATCGGGCTCCCCCAAAGGGTCAAAAG"}
        Dim y As New FastaSeq With {.SequenceData = "ATGCCGAACGGGCTCCCAAAATAAAGCAAAAG"}
        Dim result = RunNeedlemanWunsch.RunAlign(x, y, Console.Out)

        Pause()
    End Sub

    Sub scoreTest2()
        Dim x As New FastaSeq With {.SequenceData = "ATGGCGA"}
        Dim y As New FastaSeq With {.SequenceData = "AATGGCGAACGGCG"}
        Dim result = RunNeedlemanWunsch.RunAlign(x, y, Console.Out)

        Pause()
    End Sub

    Sub maxScoreTest()

        For i As Integer = 5 To 20



            Dim seq As New FastaSeq With {.SequenceData = "ATGC".RandomCharString(i)}


            Call Console.WriteLine(seq.Length)

            Dim result = RunNeedlemanWunsch.RunAlign(seq, seq, Console.Out)

            Dim seq2 As New FastaSeq With {.SequenceData = seq.SequenceData & "A"}

            RunNeedlemanWunsch.RunAlign(seq, seq2, Console.Out)

            Console.WriteLine()

        Next

        Pause()
    End Sub
End Module
