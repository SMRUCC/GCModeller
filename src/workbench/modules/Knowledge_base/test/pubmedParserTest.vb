Imports SMRUCC.genomics.GCModeller.Workbench.Knowledge_base.NCBI

Module pubmedParserTest

    Sub Main()
        Dim articles = PlainTextParser.LoadArticles("C:\Users\Administrator\Downloads\pubmed-barleyspik-set.txt").ToArray

        Pause()
    End Sub
End Module
