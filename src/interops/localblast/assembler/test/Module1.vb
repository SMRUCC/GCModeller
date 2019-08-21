Imports SMRUCC.genomics.Interops.NCBI.Localblast.Assembler
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()
        Dim fa = "D:\biodeep\1_combined_R1.fasta"
        Dim contigs = Greedy.DeNovoAssembly(FastaFile.LoadNucleotideData(fa)).ToArray

        Pause()
    End Sub

End Module
