Imports SMRUCC.genomics.Data.GeneOntology.OBO
Imports SMRUCC.genomics.Data.GeneOntology.obographs

Module DAGtest

    Sub Main()
        Dim obo As GO_OBO = GO_OBO.LoadDocument("P:\go.obo")
        Dim terms = {}
        Dim DAG = obo.CreateGraph(terms)

        Call DAG.DAGasTabular.Save("./test.network/")
    End Sub
End Module
