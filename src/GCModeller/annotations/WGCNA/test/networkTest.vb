Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports SMRUCC.genomics.Analysis.HTS.WGCNA

Module networkTest

    Sub Main()
        Dim adj = CorrelationNetwork.LoadAdjacencyMatrix("C:\Users\Administrator\Downloads\WGCNA_output\adjacency_matrix.csv")
        Dim mods = ModuleMembershipResult.ReadModuleAssignment("C:\Users\Administrator\Downloads\WGCNA_output\gene_module_assignment.csv").ToArray
        Dim g As NetworkGraph = adj.ExportGraph(mods, adj_thres:=0.8)

        Call g.Tabular.Save("Z:/wgcna")
    End Sub
End Module
