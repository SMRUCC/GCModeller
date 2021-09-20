Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Analysis.Microarray.PhenoGraph

Module Program

    Const demoDir As String = "D:\GCModeller\src\GCModeller\analysis\PhenoGraph\demo"

    Sub Main()
        Dim data As DataSet() = DataSet.LoadDataSet($"{demoDir}\HR2MSI mouse urinary bladder S096_top3.csv").ToArray
        Dim graph As NetworkGraph = CommunityGraph.CreatePhenoGraph(data, k:=120, cutoff:=0)

        Call graph.Tabular.Save($"{demoDir}\HR2MSI mouse urinary bladder S096_graph/")

        Pause()
    End Sub

End Module
