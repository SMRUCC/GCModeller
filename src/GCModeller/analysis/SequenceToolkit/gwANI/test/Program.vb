Imports SMRUCC.genomics.Analysis.SequenceTools.DNA_Comparative.gwANI
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Program
    Sub Main(args As String())
        Dim nt As FastaFile = FastaFile.LoadNucleotideData("G:\GCModeller\src\GCModeller\analysis\SequenceToolkit\data\K03406.fasta")
        Dim result = gwANIMethod.Evaluate(msa:=nt, fast:=False)


    End Sub
End Module
