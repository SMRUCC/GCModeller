Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Analysis.Microarray.PhenoGraph

Module Program

    Sub Main()
        Dim data As DataSet() = DataSet.LoadDataSet("E:\GCModeller\src\GCModeller\analysis\PhenoGraph\demo\GSE98734_Count_table.txt", tsv:=True).Transpose.ToArray
        Dim graph As NetworkGraph = CommunityGraph.CreatePhenoGraph(data)


        Pause()
    End Sub

End Module
