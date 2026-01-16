#Region "Microsoft.VisualBasic::b31adf2eecccdc23f448cf209bcf646e, R#\cytoscape_toolkit\xgmml.vb"

' Author:
' 
'       asuka (amethyst.asuka@gcmodeller.org)
'       xie (genetics@smrucc.org)
'       xieguigang (xie.guigang@live.com)
' 
' Copyright (c) 2018 GPL3 Licensed
' 
' 
' GNU GENERAL PUBLIC LICENSE (GPL3)
' 
' 
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
' 
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
' 
' You should have received a copy of the GNU General Public License
' along with this program. If not, see <http://www.gnu.org/licenses/>.



' /********************************************************************************/

' Summaries:


' Code Statistics:

'   Total Lines: 88
'    Code Lines: 61 (69.32%)
' Comment Lines: 19 (21.59%)
'    - Xml Docs: 100.00%
' 
'   Blank Lines: 8 (9.09%)
'     File Size: 3.79 KB


' Module xgmmlToolkit
' 
'     Function: loadXgmml, XgmmlRender
' 
' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports SMRUCC.genomics.GCModeller.Workbench.KEGGReport
Imports SMRUCC.genomics.Model.Network.KEGG.GraphVisualizer.PathwayMaps
Imports SMRUCC.genomics.Model.Network.KEGG.GraphVisualizer.PathwayMaps.RenderStyles
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File
Imports SMRUCC.Rsharp.Runtime
Imports SMRUCC.Rsharp.Runtime.Components
Imports SMRUCC.Rsharp.Runtime.Interop
Imports SMRUCC.Rsharp.Runtime.Vectorization

<Package("xgmml")>
<RTypeExport("xgmml", GetType(XGMMLgraph))>
Module xgmmlToolkit

    ''' <summary>
    ''' read cytoscape xgmml file
    ''' </summary>
    ''' <param name="file"></param>
    ''' <returns></returns>
    <ExportAPI("read.xgmml")>
    Public Function loadXgmml(file As String) As XGMMLgraph
        Return XGMML.RDFXml.Load(path:=file)
    End Function

    <ExportAPI("set_images")>
    Public Function SetImages(model As XGMMLgraph, dir As String, attr As String) As XGMMLgraph
        Dim nodes As NodeRepresentation = NodeRepresentation.LoadFromFolder(dir)

        For Each node As XGMMLnode In model.nodes
            Dim attrKey As String = node(attr)
            Dim filepath As String = nodes.imagefiles(attrKey)
            Dim uri As String = $"org.cytoscape.ding.customgraphics.bitmap.URLImageCustomGraphics,2,file:/{filepath},bitmap image"

            node.graphics.SetAttribute("NODE_CUSTOMGRAPHICS_1", uri)
        Next

        Return model
    End Function

    ''' <summary>
    ''' render the cytoscape network graph model as image
    ''' </summary>
    ''' <param name="model">the network graph object or the cytoscape network model</param>
    ''' <param name="size">the size of the output image</param>
    ''' <param name="convexHull$"></param>
    ''' <param name="edgeBends"></param>
    ''' <param name="altStyle"></param>
    ''' <param name="rewriteGroupCategoryColors$"></param>
    ''' <param name="enzymeColorSchema$"></param>
    ''' <param name="compoundColorSchema$"></param>
    ''' <param name="reactionKOMapping"></param>
    ''' <param name="compoundNames"></param>
    ''' <returns></returns>
    <ExportAPI("xgmml.render")>
    Public Function XgmmlRender(model As Object,
                                Optional size$ = "10(A0)",
                                Optional convexHull$() = Nothing,
                                Optional edgeBends As Boolean = False,
                                Optional altStyle As Boolean = False,
                                Optional rewriteGroupCategoryColors$ = "TSF",
                                Optional enzymeColorSchema$ = "Set1",
                                Optional compoundColorSchema$ = "Clusters",
                                Optional reactionKOMapping As Dictionary(Of String, String()) = Nothing,
                                Optional renderStyle As Boolean = False,
                                Optional nodes As NodeRepresentation = Nothing,
                                Optional compoundNames As Dictionary(Of String, String) = Nothing,
                                <RRawVectorArgument(TypeCodes.string)>
                                Optional attrs As Object = "label|class",
                                Optional env As Environment = Nothing) As GraphicsData

        Dim style As RenderStyle = Nothing
        Dim graph As NetworkGraph

        If model Is Nothing Then
            Return Nothing
        ElseIf model.GetType Is GetType(NetworkGraph) Then
            graph = model
        ElseIf model.GetType Is GetType(XGMMLgraph) Then
            graph = DirectCast(model, XGMMLgraph).ToNetworkGraph(CLRVector.asCharacter(attrs))
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

        Dim driver As Drivers = env.getDriver

        If renderStyle Then
            Return ReferenceMapRender.Render(
                graph:=graph,
                canvasSize:=size,
                convexHull:=convexHull.Indexing,
                compoundNames:=compoundNames,
                edgeBends:=edgeBends,
                renderStyle:=style,
                reactionKOMapping:=reactionKOMapping,
                edgeColorByNodeMixed:=False,
                driver:=driver
            )
        Else
            Dim drawNode As DrawNodeShape = Nothing

            If nodes IsNot Nothing Then
                drawNode = AddressOf nodes.SetGraph(graph, "kegg").DrawNodeShape
            End If

            Return NetworkVisualizer.DrawImage(
                net:=graph,
                padding:="padding: 500px 500px 500px 500px;",
                canvasSize:=size,
                drawEdgeBends:=edgeBends,
                labelerIterations:=-1,
                minLinkWidth:=5,
                drawNodeShape:=drawNode,
                driver:=driver,
                displayId:=False
            )
        End If
    End Function
End Module
