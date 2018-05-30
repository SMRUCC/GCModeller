Imports GSEA
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Module Module1

    Sub Main()
        Call modelBuildTest()

    End Sub

    Sub modelBuildTest()
        Dim go = GSEA.Imports.GOClusters(GO_OBO.Open("E:\GCModeller-repo\GO\go.obo"))
        Dim uniprot = UniProtXML.EnumerateEntries("P:\uniprot-taxonomy%3A314565.xml")
        Dim model As Genome = GSEA.Imports.ImportsUniProt(uniprot, GSEA.UniProtGetGOTerms, define:=go)

        Call model.GetXml.SaveTo("./test_model.Xml")

        Pause()
    End Sub
End Module
