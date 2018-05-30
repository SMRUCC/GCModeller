Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.UnixBash
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS
Imports SMRUCC.genomics.Analysis.HTS.GSEA
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology.OBO

Module Module1

    Sub Main()
        Call KEGGmodelBuildTest()
        ' Call modelBuildTest()
        Call enrichmentTest()
    End Sub

    Sub enrichmentTest()
        Dim background = "E:\GCModeller\src\GCModeller\annotations\GSEA\xcb.Xml".LoadXml(Of Genome)
        Dim list$() = "E:\GCModeller\src\GCModeller\annotations\GSEA\xcb_TCS.txt" _
            .IterateAllLines _
            .Select(Function(l) l.Split.First) _
            .ToArray

        With UniProtXML.EnumerateEntries("P:\uniprot-taxonomy%3A314565.xml") _
                       .Where(Function(prot) prot.Xrefs.ContainsKey("KEGG")) _
                       .ToDictionary(Function(prot)
                                         Return prot.Xrefs!KEGG.First.id.Split(":"c).Last
                                     End Function,
                                     Function(prot) prot.accessions)
            list = list.Select(Function(id) .ByRef(id)).IteratesALL.ToArray
            list.SaveTo("./uniprot.txt")
        End With

        Dim result = background.Enrichment(list).FDRCorrection.ToArray

        Call result.SaveTo("./result.csv")
    End Sub

    Sub modelBuildTest()
        Dim go = GSEA.Imports.GOClusters(GO_OBO.Open("E:\GCModeller-repo\GO\go.obo"))
        Dim uniprot = UniProtXML.EnumerateEntries("P:\uniprot-taxonomy%3A314565.xml")
        Dim model As Genome = GSEA.Imports.ImportsUniProt(uniprot, GSEA.UniProtGetGOTerms, define:=go)

        Call model.GetXml.SaveTo("E:\GCModeller\src\GCModeller\annotations\GSEA\xcb.Xml")

        Pause()
    End Sub

    Sub KEGGmodelBuildTest()
        Dim kegg = (ls - l - r - "*.Xml" <= "D:\GCModeller-CAD-blueprint\KGML\maps").Select(AddressOf LoadXml(Of Map))
        Dim uniprot = UniProtXML.EnumerateEntries("E:\GCModeller\src\GCModeller\annotations\GSEA\uniprot-taxonomy%3A314565.xml")
        Dim model As Genome = GSEA.Imports.ImportsUniProt(uniprot, GSEA.UniProtGetKOTerms, define:=GSEA.KEGGClusters(kegg))


        Call model.GetXml.SaveTo("E:\GCModeller\src\GCModeller\annotations\GSEA\xcb_KO.Xml")

        Pause()
    End Sub
End Module
