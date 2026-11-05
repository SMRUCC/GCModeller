Imports Microsoft.VisualBasic.ComponentModel.Settings.Inf
Imports SMRUCC.genomics.GCModeller.ModellingEngine.Dynamics.Definitions

Module profileTest

    Sub Main()
        Dim kegg As Definition = DbContextMapping.KEGG({"C00001"})

        Call kegg.WriteClass("Z:/test.inf")
    End Sub
End Module
