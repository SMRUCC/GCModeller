Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.Math.LinearAlgebra
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix

Public Class Result

    Public Property beta As BetaTest
    Public Property network As NetworkGraph
    Public Property K As Vector
    Public Property TOM As GeneralMatrix
    Public Property hclust As Cluster
    Public Property modules As Dictionary(Of String, String())
    Public Property softBeta As BetaTest()

End Class
