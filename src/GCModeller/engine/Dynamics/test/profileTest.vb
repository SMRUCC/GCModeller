Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Definitions

Module profileTest

    Const test = "Z:/test.inf"

    Sub Main()
        Call write()
        Call load()
    End Sub

    Sub load()
        Dim result = LoadIni(Of Definition)(test)

        Call Console.WriteLine(result.GetJson)
    End Sub

    Sub write()
        Dim kegg As Definition = DbContextMapping.KEGG({"C00001"})

        Call kegg.WriteClass(test)
    End Sub
End Module
