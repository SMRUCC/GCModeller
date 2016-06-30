Module Module1

    Sub Main()
        Dim motifs = LANS.SystemsBiology.AnalysisTools.NBCR.Extensions.MEME_Suite.DocumentFormat.MEME.Text.Load("F:\1.13.RegPrecise_network\MEME_OUT\DEGs.MEME\MMX-TO-NY.Down.fa.MEME_OUT\250bp.txt")
        Call motifs.SaveAsXml("./test.xml")
    End Sub
End Module
