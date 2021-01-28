Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports SMRUCC.genomics.Analysis.HTS
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

Module Module1

    Sub Main()
        Dim raw As Matrix = Matrix.LoadData("K:\20210127\result\T25+37.csv")
        Dim out = WGCNA.Analysis.Run(raw)

        Call out.hclust.Plot(layout:=Layouts.Horizon).Save("K:\20210127\result\T25+37.tree.png")

        Pause()
    End Sub

End Module
