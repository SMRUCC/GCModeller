Imports System
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Program
    Sub Main(args As String())
        Dim data As FastaFile = FastaFile.LoadNucleotideData("G:\GCModeller\src\GCModeller\analysis\SequenceToolkit\data\Staphylococcaceae_LexA___Staphylococcaceae.fasta")
        Dim avgLen As Integer = data.Average(Function(seq) seq.Length)
        Dim gibbs As New GibbsSampler(data, motifLength:=avgLen)
        Dim motif As MSAMotif = gibbs.find

        Pause()
    End Sub
End Module
