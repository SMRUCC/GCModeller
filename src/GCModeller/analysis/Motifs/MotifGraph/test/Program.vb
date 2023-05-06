Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics
Imports SMRUCC.genomics.Model.MotifGraph
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Program
    Sub Main(args As String())
        Dim dna As New FastaSeq With {.SequenceData = "ATGCCGCGCGTCTCTCTCGGAGAGAAAAAGGGAAA"}
        Dim graph = Builder.DNAGraph(dna)

        Call Console.WriteLine(graph.GetVector(SequenceModel.NT).GetJson)

        Pause()
    End Sub
End Module
