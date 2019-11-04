Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes
Imports Microsoft.VisualBasic.Imaging.Driver
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace PathwayMaps

    Public Module ReferenceMapRender

        ReadOnly compoundNames As Dictionary(Of String, String) = getCompoundNames()
        ReadOnly reactionNames As Dictionary(Of String, String) = getReactionNames()

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function getCompoundNames() As Dictionary(Of String, String)
            Return CompoundBrite.GetAllCompoundResources _
                .Values _
                .IteratesALL _
                .GroupBy(Function(name) name.entry.Key) _
                .Where(Function(g) Not g.Key.StringEmpty) _
                .ToDictionary(Function(name) name.Key,
                              Function(terms)
                                  Return terms.First.entry.Value
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
        <Extension>
        Public Function Render(model As XGMMLgraph,
                               Optional canvasSize$ = "11480,9200",
                               Optional enzymeColorSchema$ = "Set1:c8",
                               Optional compoundColorSchema$ = "Clusters",
                               Optional reactionShapeStrokeCSS$ = "stroke: white; stroke-width: 5px; stroke-dash: dash;") As GraphicsData

            Dim graph As NetworkGraph = model.ToNetworkGraph
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

            Dim reactionShapeStroke As Pen = Stroke.TryParse(reactionShapeStrokeCSS)
            Dim rectShadow As New Shadow(30, 45, 1.25, 1.25)
            Dim circleShadow As New Shadow(130, 45, 2, 2)
            Dim drawNode As DrawNodeShape =
                Sub(id$, g As IGraphics, br As Brush, radius!, center As PointF)
                    Dim node As Node = nodes(id)
                    Dim connectedNodes = graph.GetConnectedVertex(id)

                    If node.label.IsPattern("C\d+") Then
                        ' 圆形
                        radius = radius * 0.4
                        center = center.OffSet2D(20, 20)

                        Dim rect As New Rectangle With {
                            .X = center.X - radius,
                            .Y = center.Y + radius,
                            .Width = radius,
                            .Height = radius
                        }

                        Call circleShadow.Circle(g, center, radius)
                        Call g.FillEllipse(br, rect)
                        Call g.DrawEllipse(New Pen(DirectCast(br, SolidBrush).Color.Darken, 10), rect)
                    Else
                        ' 方形
                        Dim offset As New PointF(30, 20)

                        center = center.OffSet2D(offset)
                        radius = radius * 0.8

                        Dim rect As New Rectangle With {
                            .X = center.X - radius * 3 / 4,
                            .Y = center.Y + radius / 2,
                            .Width = radius,
                            .Height = radius / 2
                        }

                        br = New SolidBrush(DirectCast(br, SolidBrush).Color.Alpha(240))

                        Call rectShadow.RoundRectangle(g, rect, 30)
                        Call g.FillPath(br, RoundRect.GetRoundedRectPath(rect, 30))
                        Call g.DrawPath(reactionShapeStroke, RoundRect.GetRoundedRectPath(rect, 30))
                    End If
                End Sub

            Return NetworkVisualizer.DrawImage(
                net:=graph,
                background:="#7ac1d0",
                padding:="padding: 500px 500px 500px 500px;",
                canvasSize:=canvasSize,
                labelerIterations:=5,
                doEdgeBundling:=True,
                drawNodeShape:=drawNode,
                minLinkWidth:=5,
                nodeRadius:=220,
                edgeShadowDistance:=0,
                edgeDashTypes:=DashStyle.Solid,
                defaultEdgeColor:=NameOf(Color.LightGray),
                getNodeLabel:=AddressOf getNodeLabel
            )
        End Function

        Private Function getNodeLabel(node As Node) As String
            If node.label.IsPattern("C\d+") Then
                Return compoundNames.TryGetValue(node.label, [default]:=node.label)
            ElseIf node.label.IsPattern("R\d+") Then
                Return reactionNames.TryGetValue(node.label, [default]:=node.label)
            Else
                Return node.label
            End If
        End Function
    End Module
End Namespace