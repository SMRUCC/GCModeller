
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.Model.Network.KEGG.PathwayMaps
Imports SMRUCC.genomics.Model.Network.KEGG.PathwayMaps.RenderStyles
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File
Imports SMRUCC.Rsharp.Runtime.Interop

<Package("xgmml")>
Module xgmmlToolkit

    <ExportAPI("read.xgmml")>
    Public Function loadXgmml(file As String) As XGMMLgraph
        Return XGMML.RDFXml.Load(path:=file)
    End Function

    <ExportAPI("xgmml.graph")>
    Public Function createGraph(xgmml As XGMMLgraph, <RRawVectorArgument(GetType(String))> Optional propertyNames As Object = "label|class|group.category|group.category.color") As NetworkGraph
        Return xgmml.ToNetworkGraph(DirectCast(propertyNames, String()))
    End Function

    <ExportAPI("xgmml.render")>
    Public Function XgmmlRender(model As Object,
                                Optional size$ = "10(A0)",
                                Optional convexHull$() = Nothing,
                                Optional edgeBends As Boolean = False,
                                Optional altStyle As Boolean = False,
                                Optional rewriteGroupCategoryColors$ = "TSF",
                                Optional enzymeColorSchema$ = "Set1:c8",
                                Optional compoundColorSchema$ = "Clusters",
                                Optional reactionKOMapping As Dictionary(Of String, String()) = Nothing,
                                Optional compoundNames As Dictionary(Of String, String) = Nothing) As GraphicsData

        Dim style As RenderStyle = Nothing
        Dim graph As NetworkGraph

        If model Is Nothing Then
            Return Nothing
        ElseIf model.GetType Is GetType(NetworkGraph) Then
            graph = model
        ElseIf model.GetType Is GetType(XGMMLgraph) Then
            graph = DirectCast(model, XGMMLgraph).ToNetworkGraph("label", "class", "group.category", "group.category.color")
        Else
            Return Nothing
        End If

        If altStyle Then
            Dim convexHullCategoryStyle As Dictionary(Of String, String) = graph _
                .getCategoryColors(convexHull, rewriteGroupCategoryColors) _
                .TupleTable

            style = New PlainStyle(
                graph:=graph,
                convexHullCategoryStyle:=convexHullCategoryStyle,
                enzymeColorSchema:=enzymeColorSchema,
                compoundColorSchema:=compoundColorSchema
            )
        End If

        Return ReferenceMapRender.Render(
            graph:=graph,
            canvasSize:=size,
            convexHull:=convexHull.Indexing,
            compoundNames:=compoundNames,
            edgeBends:=edgeBends,
            renderStyle:=style,
            reactionKOMapping:=reactionKOMapping
        )
    End Function
End Module
