#Region "Microsoft.VisualBasic::fd410f503abed88d792a31a79afd31e8, models\Networks\KEGG\PathwayMaps\ReferenceMapRender.vb"

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
'         Function: getCompoundNames, getNodeLabel, getReactionNames, (+2 Overloads) Render
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Data
Imports SMRUCC.genomics.Model.Network.KEGG.PathwayMaps.RenderStyles
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace PathwayMaps

    Public Module ReferenceMapRender

        ReadOnly reactionNames As Dictionary(Of String, String) = getReactionNames()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function getCompoundNames(repository As String) As Dictionary(Of String, String)
            Return CompoundRepository.ScanRepository(repository, False) _
                .GroupBy(Function(c) c.entry) _
                .ToDictionary(Function(cpd) cpd.Key,
                              Function(cpd)
                                  Return cpd.First _
                                      .commonNames _
                                      .FirstOrDefault Or cpd.Key.AsDefault
                              End Function)
        End Function

        Private Function getReactionNames() As Dictionary(Of String, String)
            Return EnzymaticReaction.LoadFromResource _
                .GroupBy(Function(r) r.Entry.Key) _
                .Where(Function(g) Not g.Key.StringEmpty) _
                .ToDictionary(Function(r) r.Key,
                              Function(r)
                                  Dim reaction As EnzymaticReaction = r.First

                                  If reaction.EC.StringEmpty Then
                                      Return r.Key
                                  Else
                                      Return "EC " & reaction.EC
                                  End If
                              End Function)
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
                               Optional compoundRepository$ = Nothing,
                               Optional edgeBends As Boolean = False) As GraphicsData

            Return model.ToNetworkGraph("label", "class", "group.category", "group.category.color") _
                .Render(canvasSize:=canvasSize,
                        enzymeColorSchema:=enzymeColorSchema,
                        compoundColorSchema:=compoundColorSchema,
                        reactionShapeStrokeCSS:=reactionShapeStrokeCSS,
                        convexHull:=convexHull,
                        compoundNames:=getCompoundNames(compoundRepository),
                        edgeBends:=edgeBends
                )
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
                               Optional edgeBends As Boolean = False) As GraphicsData

            Dim nodes As New Dictionary(Of String, Node)
            Dim fluxCategory = EnzymaticReaction.LoadFromResource _
                .GroupBy(Function(r) r.Entry.Key) _
                .ToDictionary(Function(r) r.Key,
                              Function(r)
                                  Return r.First
                              End Function)
            Dim compoundCategory = CompoundBrite.CompoundsWithBiologicalRoles _
                .GroupBy(Function(c) c.entry.Key) _
                .ToDictionary(Function(c) c.Key,
                              Function(c)
                                  Return c.First.class
                              End Function)
            Dim enzymeColors As Color() = Designer.GetColors(enzymeColorSchema)
            Dim compoundColors As New CategoryColorProfile(compoundCategory, compoundColorSchema)

            If compoundNames Is Nothing Then
                compoundNames = New Dictionary(Of String, String)
            End If

            If renderStyle Is Nothing Then
                renderStyle = New BlockStyle(
                    nodes:=nodes,
                    graph:=graph,
                    reactionShapeStrokeCSS:=reactionShapeStrokeCSS,
                    hideCompoundCircle:=hideCompoundCircle
                )
            End If

            For Each node As Node In graph.vertex
                If node.label.IsPattern("C\d+") Then
                    If compoundCategory.ContainsKey(node.label) Then
                        node.data.color = New SolidBrush(compoundColors.GetColor(node.label))
                    Else
                        node.data.color = Brushes.LightGray
                    End If
                Else
                    If fluxCategory.ContainsKey(node.label) Then
                        Dim enzyme% = fluxCategory(node.label).EC.Split("."c).First.ParseInteger
                        Dim color As Color = enzymeColors(enzyme)

                        node.data.color = New SolidBrush(color)
                    Else
                        node.data.color = Brushes.SkyBlue
                    End If
                End If

                nodes.Add(node.label, node)
            Next

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

            Dim yellow As Color = "#f5f572".TranslateColor

            Return NetworkVisualizer.DrawImage(
                net:=graph,
                background:="white",'"transparent",
                padding:=padding,
                canvasSize:=canvasSize,
                labelerIterations:=-1000,
                drawNodeShape:=AddressOf renderStyle.drawNode,
                hullPolygonGroups:=New NamedValue(Of String) With {
                    .Name = "group.category",
                    .Value = allCategories.JoinBy(","),
                    .Description = categoryColors.JoinBy(",")
                },
                minLinkWidth:=8,
                nodeRadius:=150,
                edgeShadowDistance:=0,
                edgeDashTypes:=DashStyle.Dot,
                defaultEdgeColor:="brown",
                getNodeLabel:=getNodeLabel(compoundNames),
                getLabelPosition:=getLabelPositoon，
                labelTextStroke:=Nothing,
                labelFontBase:="font-style: normal; font-size: 24; font-family: " & FontFace.MicrosoftYaHei & ";",
                fontSize:=New Func(Of Node, Single)(AddressOf renderStyle.getFontSize),
                defaultLabelColor:="white",
                getLabelColor:=Function(node As Node) As Color
                                   If node.label.IsPattern("C\d+") Then
                                       Return Color.Black
                                   ElseIf DirectCast(node.data.color, SolidBrush).Color.EuclideanDistance(yellow) <= 30 Then
                                       Return Color.DarkBlue
                                   Else
                                       Return Color.White
                                   End If
                               End Function,
                convexHullLabelFontCSS:="font-style: normal; font-size: 72; font-family: " & FontFace.MicrosoftYaHei & ";",
                convexHullScale:=1.025,
                drawEdgeBends:=edgeBends,
                labelWordWrapWidth:=wordWrapWidth,
                isLabelPinned:=Function(n, actualLabel)
                                   Return n.label.IsPattern("R\d+") OrElse actualLabel.Length <= wordWrapWidth
                               End Function
            )
        End Function

        Private Function getNodeLabel(compoundNames As Dictionary(Of String, String)) As Func(Of Node, String)
            Return Function(node As Node) As String
                       If node.label.IsPattern("C\d+") Then
                           Return compoundNames _
                              .TryGetValue(node.label, [default]:=node.data!label) _
                              .Split(";"c) _
                              .First
                       ElseIf node.label.IsPattern("R\d+") Then
                           Return reactionNames.TryGetValue(node.label, [default]:=node.data!label)
                       Else
                           Return node.label
                       End If
                   End Function
        End Function
    End Module
End Namespace
