Imports SMRUCC.genomics.Analysis.SequenceTools.MSA

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

        Call msa.SaveTo("./msa.txt")
        Call Console.WriteLine(msa)

        Pause()
    End Sub
End Module
