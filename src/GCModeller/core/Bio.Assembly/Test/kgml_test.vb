Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Public Module kgml_test

    Sub Main()
        Dim maps = {"F:\datapool\20260301\202608-Figures\分子表达图\network\taes00941.xml",
"F:\datapool\20260301\202608-Figures\分子表达图\network\taes00999.xml",
"F:\datapool\20260301\202608-Figures\分子表达图\network\taes04120.xml",
"F:\datapool\20260301\202608-Figures\分子表达图\network\taes00940.xml"}
        Dim kgml_maps = maps.Select(Function(file) pathway.LoadMap(file)).ToArray
        Dim network = kgml_maps.Select(Function(p) GeneMetaboliteNetwork.ExtractNetwork(p)).IteratesALL.ToArray

        Call network.SaveTo("Z:/network.csv")
    End Sub
End Module
