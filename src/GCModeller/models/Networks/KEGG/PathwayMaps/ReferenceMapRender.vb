#Region "Microsoft.VisualBasic::c55c3023eac275bda1281e8a015d97a0, models\Networks\KEGG\PathwayMaps\ReferenceMapRender.vb"

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

'     Module ReferenceMapRender
' 
'         Function: getCategoryColors, (+2 Overloads) GetNodeLabel, getReactionName, getReactionNames, (+2 Overloads) Render
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Analysis
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports SMRUCC.genomics.Model.Network.KEGG.PathwayMaps.RenderStyles
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File
Imports r = System.Text.RegularExpressions.Regex

Namespace PathwayMaps

    Public Module ReferenceMapRender

        ''' <summary>
        ''' geneName -> EC number -> reactionId
        ''' </summary>
        ''' <param name="reactionKOMapping"></param>
        ''' <returns></returns>
        Private Function getReactionNames(reactionKOMapping As Dictionary(Of String, String())) As Dictionary(Of String, String)
            Dim KOnames = PathwayMapping.DefaultKOTable

            If reactionKOMapping Is Nothing Then
                reactionKOMapping = New Dictionary(Of String, String())
            End If

            Return EnzymaticReaction.LoadFromResource _
                .GroupBy(Function(r) r.Entry.Key) _
                .Where(Function(g) Not g.Key.StringEmpty) _
                .ToDictionary(Function(r) r.Key,
                              Function(rxn)
                                  Return rxn.getReactionName(reactionKOMapping, KOnames)
                              End Function)
        End Function

        <Extension>
        Private Function getReactionName(rxn As IGrouping(Of String, EnzymaticReaction),
                                         reactionKOMapping As Dictionary(Of String, String()),
                                         KOnames As Dictionary(Of String, BriteHText)) As String

            Dim reaction As EnzymaticReaction = rxn.First

            If reactionKOMapping.ContainsKey(rxn.Key) Then
                Dim KO = reactionKOMapping(rxn.Key)
                Dim names As String = KO _
                    .Where(AddressOf KOnames.ContainsKey) _
                    .Select(Function(id) KOnames(id).description.Split(";"c).First) _
                    .Distinct _
                    .OrderBy(Function(name) name.Length) _
                    .FirstOrDefault

                If Not names.StringEmpty Then
                    names = r.Replace(names, "E\s*\d(\.\d+)+[,]?", "").Trim

                    If Not names.StringEmpty Then
                        Return names
                    End If
                End If
            End If

            If reaction.EC.StringEmpty Then
                Return rxn.Key
            Else
                Return "EC " & reaction.EC
            End If
        End Function

        ''' <summary>
        ''' 将完成node和edge布局操作的网络模型进行渲染
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Render(model As XGMMLgraph,
                               Optional canvasSize$ = "11480,9200",
                               Optional enzymeColorSchema$ = "Set1:c8",
                               Optional compoundColorSchema$ = "Clusters",
                               Optional reactionShapeStrokeCSS$ = "stroke: white; stroke-width: 5px; stroke-dash: dash;",
                               Optional convexHull As String() = Nothing,
                               Optional compoundNamesJson$ = Nothing,
                               Optional reactionKOMappingJson$ = Nothing,
                               Optional edgeBends As Boolean = False,
                               Optional altStyle As Boolean = False,
                               Optional rewriteGroupCategoryColors$ = "TSF") As GraphicsData

            Dim style As RenderStyle = Nothing
            Dim graph As NetworkGraph = model.ToNetworkGraph("label", "class", "group.category", "group.category.color")

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

            Return graph.Render(
                canvasSize:=canvasSize,
                enzymeColorSchema:=enzymeColorSchema,
                compoundColorSchema:=compoundColorSchema,
                reactionShapeStrokeCSS:=reactionShapeStrokeCSS,
                reactionKOMapping:=reactionKOMappingJson _
                    .ReadAllText _
                    .LoadJSON(Of Dictionary(Of String, String())),
                convexHull:=convexHull,
                compoundNames:=compoundNamesJson _
                    .ReadAllText _
                    .LoadJSON(Of Dictionary(Of String, String)),
                edgeBends:=edgeBends,
                renderStyle:=style,
                rewriteGroupCategoryColors:=rewriteGroupCategoryColors
            )
        End Function

        <Extension>
        Public Function getCategoryColors(graph As NetworkGraph,
                                          convexHull As Index(Of String),
                                          rewriteGroupCategoryColors As String) As (allCategory As String(), categoryColors As String())

            Dim allCategories$() = graph.vertex _
                .Select(Function(n)
                            Return n.data("group.category")
                        End Function) _
                .Distinct _
                .Where(Function(cat)
                           Return cat Like convexHull
                       End Function) _
                .ToArray
            Dim rewriteGroupCategoryColor As LoopArray(Of Color) = Designer.GetColors(rewriteGroupCategoryColors)
            Dim categoryColors = allCategories _
                .Select(Function(c)
                            If rewriteGroupCategoryColor.Length = 0 Then
                                Return graph.vertex _
                                    .First(Function(n)
                                               Return n.data("group.category") = c
                                           End Function) _
                                    .data("group.category.color")
                            Else
                                Return rewriteGroupCategoryColor.Next().ToHtmlColor
                            End If
                        End Function) _
                .ToArray

            If Not allCategories.IsNullOrEmpty Then
                Call $"Network canvas will render {allCategories.Length} category data for convexHull...".__INFO_ECHO

                For Each category As SeqValue(Of String) In allCategories.SeqIterator
                    Call $"  {category.value} -> {categoryColors(category)}".__INFO_ECHO
                Next
            End If

            Return (allCategories, categoryColors)
        End Function

        ''' <summary>
        ''' 将完成node和edge布局操作的网络模型进行渲染
        ''' </summary>
        ''' <returns></returns>
        <Extension>
        Public Function Render(graph As NetworkGraph,
                               Optional canvasSize$ = "11480,9200",
                               Optional padding$ = "padding: 300px 300px 300px 300px;",
                               Optional renderStyle As RenderStyle = Nothing,
                               Optional enzymeColorSchema$ = "Set1:c8",
                               Optional compoundColorSchema$ = "Clusters",
                               Optional reactionShapeStrokeCSS$ = "stroke: white; stroke-width: 5px; stroke-dash: dash;",
                               Optional hideCompoundCircle As Boolean = True,
                               Optional convexHull As Index(Of String) = Nothing,
                               Optional compoundNames As Dictionary(Of String, String) = Nothing,
                               Optional wordWrapWidth% = 14,
                               Optional rewriteGroupCategoryColors$ = "TSF",
                               Optional edgeBends As Boolean = False,
                               Optional edgeColorByNodeMixed As Boolean = True,
                               Optional reactionKOMapping As Dictionary(Of String, String()) = Nothing) As GraphicsData

            Dim reactionNames As Dictionary(Of String, String) = getReactionNames(reactionKOMapping)

            If compoundNames Is Nothing Then
                compoundNames = New Dictionary(Of String, String)
            End If

            If renderStyle Is Nothing Then
                Dim convexHullCategoryStyle = graph.getCategoryColors(convexHull, rewriteGroupCategoryColors)

                renderStyle = New BlockStyle(
                    graph:=graph,
                    convexHullCategoryStyle:=convexHullCategoryStyle,
                    reactionShapeStrokeCSS:=reactionShapeStrokeCSS,
                    hideCompoundCircle:=hideCompoundCircle,
                    enzymeColorSchema:=enzymeColorSchema,
                    compoundColorSchema:=compoundColorSchema
                )
            End If

            Call $"Network render style engine is: {renderStyle.GetType.FullName}".__DEBUG_ECHO

            Dim getLabelPositoon As GetLabelPosition =
                Function(node As Node, label$, shapeLayout As RectangleF, labelSize As SizeF)
                    If node.label.IsPattern("C\d+") Then
                        Return New PointF(
                            x:=shapeLayout.Left + (shapeLayout.Width - labelSize.Width) / 2,
                            y:=shapeLayout.Top + (shapeLayout.Height - labelSize.Height) / 2
                        )
                    Else
                        Return New PointF(
                            x:=shapeLayout.Left + (shapeLayout.Width - labelSize.Width) / 2,
                            y:=shapeLayout.Top + (shapeLayout.Height - labelSize.Height) / 2
                        )
                    End If
                End Function

            If convexHull Is Nothing Then
                convexHull = New Index(Of String)
            End If

            Dim degrees As New Dictionary(Of String, Integer)

            If edgeColorByNodeMixed Then
                degrees = graph.ComputeNodeDegrees
            End If

            Return NetworkVisualizer.DrawImage(
                net:=If(edgeColorByNodeMixed, graph.mixEdgeColor(renderStyle, degrees), graph),
                background:="white",'"transparent",
                padding:=padding,
                canvasSize:=canvasSize,
                labelerIterations:=-1000,
                drawNodeShape:=AddressOf renderStyle.drawNode,
                hullPolygonGroups:=renderStyle.getHullPolygonGroups,
                minLinkWidth:=10,
                nodeRadius:=300,
                edgeShadowDistance:=0,
                edgeDashTypes:=renderStyle.edgeDashType,
                defaultEdgeColor:="brown",
                getNodeLabel:=GetNodeLabel(compoundNames, reactionNames),
                getLabelPosition:=getLabelPositoon，
                labelTextStroke:=Nothing,
                labelFontBase:="font-style: normal; font-size: 24; font-family: " & FontFace.MicrosoftYaHei & ";",
                fontSize:=New Func(Of Node, Single)(AddressOf renderStyle.getFontSize),
                defaultLabelColor:="white",
                getLabelColor:=AddressOf renderStyle.getLabelColor,
                convexHullLabelFontCSS:="font-style: normal; font-size: 72; font-family: " & FontFace.MicrosoftYaHei & ";",
                convexHullScale:=1.25,
                convexHullCurveDegree:=1,
                fillConvexHullPolygon:=False,
                drawEdgeBends:=edgeBends,
                labelWordWrapWidth:=wordWrapWidth,
                isLabelPinned:=Function(n, actualLabel)
                                   Return n.label.IsPattern("R\d+") OrElse actualLabel.Length <= wordWrapWidth
                               End Function
            )
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="g"></param>
        ''' <param name="renderStyle"></param>
        ''' <param name="degrees"><see cref="ComputeNodeDegrees"/></param>
        ''' <returns></returns>
        <Extension>
        Private Function mixEdgeColor(g As NetworkGraph, renderStyle As RenderStyle, degrees As Dictionary(Of String, Integer)) As NetworkGraph
            Dim markBlacks As New List(Of Edge)

            For Each edge As Edge In g.graphEdges
                Dim u As Color = renderStyle.getLabelColor(edge.U)
                Dim v As Color = renderStyle.getLabelColor(edge.V)

                If GDIColors.Equals(u, v) Then
                    edge.data.color = New SolidBrush(u)
                ElseIf GDIColors.Equals(u, Color.Black) Then
                    edge.data.color = New SolidBrush(v)
                ElseIf GDIColors.Equals(v, Color.Black) Then
                    edge.data.color = New SolidBrush(u)
                Else
                    ' color by max degree node
                    If degrees(edge.U.label) >= degrees(edge.V.label) Then
                        edge.data.color = New SolidBrush(u)
                    Else
                        edge.data.color = New SolidBrush(v)
                    End If
                End If

                If GDIColors.Equals(edge.data.color.Color, Color.Black) Then
                    markBlacks.Add(edge)
                End If
            Next

            For Each edge As Edge In markBlacks
                Dim u = edge.U.getMostNearbyColor(g)
                Dim v = edge.V.getMostNearbyColor(g)
                Dim du As Integer = u.sumLinkDegree(degrees)
                Dim dv As Integer = v.sumLinkDegree(degrees)

                If du > dv Then
                    edge.data.color = New SolidBrush(u.Key)
                Else
                    edge.data.color = New SolidBrush(v.Key)
                End If
            Next

            Return g
        End Function

        <Extension>
        Private Function sumLinkDegree(collection As IGrouping(Of Color, Edge), degrees As Dictionary(Of String, Integer)) As Integer
            If collection Is Nothing Then
                Return 0
            Else
                Return Aggregate link In collection Let dsum = degrees(link.U.label) + degrees(link.V.label) Into Sum(dsum)
            End If
        End Function

        <Extension>
        Private Function getMostNearbyColor(node As Node, g As NetworkGraph) As IGrouping(Of Color, Edge)
            Return g.graphEdges _
                .Where(Function(e) e.U Is node OrElse e.V Is node) _
                .Where(Function(a) Not GDIColors.Equals(a.data.color.Color, Color.Black)) _
                .GroupBy(Function(a) a.data.color.Color) _
                .OrderByDescending(Function(a) a.Count) _
                .FirstOrDefault
        End Function

        ''' <summary>
        ''' Converts the KEGG compound id and reaction id as metabolite name or enzyme gene name
        ''' </summary>
        ''' <param name="compoundNames"></param>
        ''' <param name="reactionKOMapping"></param>
        ''' <returns></returns>
        Public Function GetNodeLabel(compoundNames As Dictionary(Of String, String), reactionKOMapping As Dictionary(Of String, String())) As Func(Of FileStream.Node, String)
            Dim reactionNames As Dictionary(Of String, String) = getReactionNames(reactionKOMapping)
            Dim getNodeName As Func(Of Node, String) = GetNodeLabel(compoundNames, reactionNames)

            Return Function(node) As String
                       Return New Node With {
                           .label = node.ID,
                           .data = New NodeData With {
                               .label = node.ID,
                               .Properties = New Dictionary(Of String, String) From {
                                   {"label", node.ID}
                               }
                           }
                       }.DoCall(getNodeName)
                   End Function
        End Function

        ''' <summary>
        ''' Converts the KEGG compound id and reaction id as metabolite name or enzyme gene name
        ''' </summary>
        ''' <param name="compoundNames"></param>
        ''' <param name="reactionNames"></param>
        ''' <returns></returns>
        Public Function GetNodeLabel(compoundNames As Dictionary(Of String, String), reactionNames As Dictionary(Of String, String)) As Func(Of Node, String)
            Return Function(node As Node) As String
                       If node.label.IsPattern("C\d+") Then
                           If (Not compoundNames.ContainsKey(node.label)) AndAlso node.data!label.StringEmpty Then
                               Return node.label
                           Else
                               Return compoundNames _
                                  .TryGetValue(node.label, [default]:=node.data!label) _
                                  .Split(";"c) _
                                  .First
                           End If
                       ElseIf node.label.IsPattern("R\d+") Then
                           Return reactionNames.TryGetValue(node.label, [default]:=node.data!label)
                       Else
                           Return node.label
                       End If
                   End Function
        End Function
    End Module
End Namespace
