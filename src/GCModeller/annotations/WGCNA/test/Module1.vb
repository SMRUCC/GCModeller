Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports SMRUCC.genomics.Analysis.HTS
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Module Module1

    Sub Main()
        Dim raw As Matrix = Matrix.LoadData("D:\GCModeller\src\GCModeller\annotations\WGCNA\metabolome.csv")
        Dim out = WGCNA.Analysis.Run(raw)

        Call out.hclust.Plot(layout:=Layouts.Horizon).Save("D:\GCModeller\src\GCModeller\annotations\WGCNA\metabolome.png")

        Pause()
    End Sub

End Module
