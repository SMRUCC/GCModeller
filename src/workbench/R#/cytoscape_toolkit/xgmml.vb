
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

<Package("xgmml")>
Module xgmmlToolkit

    <ExportAPI("read.xgmml")>
    Public Function loadXgmml(file As String) As XGMMLgraph
        Return XGMML.RDFXml.Load(path:=file)
    End Function

    <ExportAPI("xgmml.graph")>
    Public Function createGraph(xgmml As XGMMLgraph, Optional propertyNames As String() = Nothing) As NetworkGraph
        Return xgmml.ToNetworkGraph(propertyNames)
    End Function
End Module
