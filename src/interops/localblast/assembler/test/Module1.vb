Imports SMRUCC.genomics.Interops.NCBI.Localblast.Assembler
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()
        'Dim fa = "D:\biodeep\1_combined_R1.fasta"
        'Dim contigs = Greedy.DeNovoAssembly(FastaFile.LoadNucleotideData(fa), identity:=0.1, similar:=0.05).ToArray

        'Call New FastaFile(contigs).Save("D:\biodeep\1_combined_R1_contigs.fasta")

        Dim fa = "E:\Resources\JCC1_combined_R1.fasta"
        Dim contigs = Greedy.DeNovoAssembly(FastaFile.LoadNucleotideData(fa), identity:=0.1, similar:=0.05).ToArray

        Call New FastaFile(contigs).Save("E:\Resources\JCC1_combined_R1_contigs.fasta")

        Pause()
    End Sub

End Module
