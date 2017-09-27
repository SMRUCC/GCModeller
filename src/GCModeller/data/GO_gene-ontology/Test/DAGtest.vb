Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports SMRUCC.genomics.Data.GeneOntology
Imports SMRUCC.genomics.Data.GeneOntology.DAG

Module DAGtest

    Sub Main()
        Dim g As Graph = New Graph("D:\smartnucl_integrative\DATA\go.obo")

        Dim t = g.Family("GO:0000007").Select(Function(x) x.Strip).ToArray


        Dim stat As New Dictionary(Of String, NamedValue(Of Integer)()) From {
            {"biological_process",
                {
                    New NamedValue(Of Integer)("GO:0009409", 1),
                    New NamedValue(Of Integer)("GO:0009725", 1),
                    New NamedValue(Of Integer)("GO:0033993", 1),
                    New NamedValue(Of Integer)("GO:0097305", 1),
                    New NamedValue(Of Integer)("GO:0009743", 1),
                    New NamedValue(Of Integer)("GO:0014070", 1)
                }
            }
        }


        Dim level3 = stat.LevelGOTerms(3, g)


        Pause()
    End Sub
End Module
