Imports SMRUCC.genomics.Assembly.NCBI.GenBank

Module seqParserTest

    Sub Main()
        Dim gb As GBFF.File = GBFF.File.Load("J:\XCC\assembly\GCA_001705565.1_ASM170556v1\GCA_001705565.1_ASM170556v1_genomic.gbff")
        Dim nt = gb.Origin.ToFasta
        Dim genes = gb.GbffToPTT

    End Sub
End Module
