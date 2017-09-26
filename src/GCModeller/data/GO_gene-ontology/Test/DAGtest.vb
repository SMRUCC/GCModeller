Imports SMRUCC.genomics.Data.GeneOntology.DAG

Module DAGtest

    Sub Main()
        Dim g As Graph = New Graph("D:\smartnucl_integrative\DATA\go.obo")

        Dim t = g.Family("GO:0000007").Select(Function(x) x.Strip).ToArray

        Pause()
    End Sub
End Module
