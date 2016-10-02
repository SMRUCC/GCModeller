Module Module1

    Sub Main()

        Dim tree = SMRUCC.genomics.Data.GeneOntology.DAG.BuildTree("H:\GO_DB\go-basic.obo")

        Dim file = SMRUCC.genomics.Data.GeneOntology.OBO.GO_OBO.LoadDocument("G:\GCModeller\src\GCModeller\data\GO_gene-ontology\obographs\resources\basic.obo")
        file = SMRUCC.genomics.Data.GeneOntology.OBO.GO_OBO.LoadDocument("G:\GCModeller\src\GCModeller\data\GO_gene-ontology\obographs\resources\equivNodeSetTest.obo")

        Pause()
    End Sub
End Module
