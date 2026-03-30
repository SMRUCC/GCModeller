Imports SMRUCC.genomics.Analysis.SequenceTools.HMMER.InterPro.Xml

Module interproReader

    Sub Main1()
        Dim data = interprodb.ReadTerms("F:\interpro.xml").ToArray

        Pause()
    End Sub
End Module
