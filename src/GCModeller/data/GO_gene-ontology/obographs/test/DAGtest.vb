Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Assembly.Uniprot.XML
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Data.GeneOntology.obographs

Module DAGtest

    Sub Main()
        Dim obo As GO_OBO = GO_OBO.LoadDocument("P:\go.obo")
        Dim twocomponentSystem_genes As Index(Of String) = "E:\GCModeller\src\GCModeller\annotations\GSEA\data\xcb_TCS.txt".ReadAllLines.Select(Function(line) line.StringSplit("\s+").First).ToArray
        Dim proteins = UniProtXML.EnumerateEntries("E:\GCModeller\src\GCModeller\annotations\GSEA\data\uniprot-taxonomy_314565.XML") _
            .Where(Function(prot)
                       If Not prot.xrefs.ContainsKey("KEGG") Then
                           Return False
                       End If

                       Dim geneID = prot.xrefs("KEGG").First.id.Split(":"c).Last

                       Return geneID Like twocomponentSystem_genes
                   End Function).ToArray
        Dim terms = proteins.GoTermsFromUniProt.ToArray
        Dim DAG = obo.CreateGraph(terms)

        Call DAG.DAGasTabular.Save("./test.network/")
    End Sub
End Module
