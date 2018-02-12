Imports SMRUCC.genomics.Analysis.SequenceTools.MSA

Module Module1

    Sub Main()
        Dim seq$() = "GATGTGGGGCCG
GATGTGCAG
CCGCTAGCAG
CCTGCTGCAG
CCTGTAGG".lTokens
        Dim matrix = "D:\GCModeller\src\GCModeller\analysis\SequenceToolkit\MSA\Matrix.txt".ReadAllLines.Select(Function(l) l.Replace(" "c, "").ToArray).ToArray
        Dim msa = seq.MultipleAlignment(matrix)

        Call Console.WriteLine(msa)

        Pause()
    End Sub
End Module
