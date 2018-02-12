Imports SMRUCC.genomics.Analysis.SequenceTools.MSA
Imports SMRUCC.genomics.SequenceModel.FASTA

Module Module1

    Sub Main()
        Dim seq$() = "GATGTGGGGCCG
AAGTCCGAG
GATGTGCAG
CCGTCTAGCAGT
CCTGCTGCAG
CCTGTAGGAACAG".lTokens
        Dim matrix = "D:\GCModeller\src\GCModeller\analysis\SequenceToolkit\MSA\Matrix.txt".ReadAllLines.Select(Function(l) l.Replace(" "c, "").ToArray).ToArray
        Dim msa = seq.MultipleAlignment(matrix)

        Call msa.ToFasta.Save("./msa.txt")
        Call Console.WriteLine(msa)

        Console.WriteLine(vbCrLf)

        msa = FastaFile.LoadNucleotideData("D:\GCModeller\src\GCModeller\analysis\SequenceToolkit\data\Xanthomonadales_MetR___Xanthomonadales.fasta").MultipleAlignment(matrix)

        Call msa.Print(15)

        Pause()
    End Sub
End Module
