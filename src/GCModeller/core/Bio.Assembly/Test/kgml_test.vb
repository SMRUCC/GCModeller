Imports Microsoft.VisualBasic.Data.Framework
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Assembly.KEGG.WebServices.KGML

Public Module kgml_test

    Sub Main()
        Dim maps = {"C:\Users\Administrator\Downloads\taes04120.xml",
"C:\Users\Administrator\Downloads\taes00940.xml",
"C:\Users\Administrator\Downloads\taes00941.xml",
"C:\Users\Administrator\Downloads\taes00999.xml"}
        Dim kgml_maps = maps.Select(Function(file) pathway.LoadMap(file)).ToArray
        Dim network = kgml_maps.Select(Function(p) GeneNetworkExport.ExtractFromKGML(p)).IteratesALL.ToArray

        Call network.SaveTo("Z:/network.csv")

        Pause()
    End Sub
End Module
