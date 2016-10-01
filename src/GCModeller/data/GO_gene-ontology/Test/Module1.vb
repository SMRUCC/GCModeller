Module Module1

    Sub Main()
        Dim file = SMRUCC.genomics.Data.GeneOntology.OBO.GO_OBO.LoadDocument("G:\GCModeller\src\GCModeller\data\GO_gene-ontology\obographs\resources\basic.obo")
        file = SMRUCC.genomics.Data.GeneOntology.OBO.GO_OBO.LoadDocument("G:\GCModeller\src\GCModeller\data\GO_gene-ontology\obographs\resources\equivNodeSetTest.obo")

        Pause()
    End Sub
End Module
