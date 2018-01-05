Imports SMRUCC.genomics.Assembly.NCBI.GenBank
Imports SMRUCC.genomics.SequenceModel

Module seqParserTest

    Sub Main()
        Dim gb As GBFF.File = GBFF.File.Load("J:\XCC\assembly\GCA_001705565.1_ASM170556v1\GCA_001705565.1_ASM170556v1_genomic.gbff")
        Dim nt = gb.Origin.ToFasta
        Dim genes = gb.GbffToPTT.GeneObjects.OrderBy(Function(g) g.Location.Left).ToArray

        For i As Integer = 0 To 9
            Dim gene = genes(i)
            Dim seq = nt.CutSequenceCircular(gene.Location - 50)
            Dim fa = seq.SimpleFasta(gene.Synonym)

            Call fa.SaveTo($"./{i}.fasta")
        Next
    End Sub
End Module
