Imports SMRUCC.genomics.Analysis.PrimerDesigner.Restriction_enzyme
Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Program
    Sub Main(args As String())
        Dim list = WikiLoader.PullAll.ToArray
        Dim plasmid As FastaSeq = GBFF.File.Load("D:\GCModeller\src\GCModeller\visualize\data\addgene-plasmid-100854-sequence-189713.gbk").Origin.ToFasta

        Pause()
    End Sub
End Module
