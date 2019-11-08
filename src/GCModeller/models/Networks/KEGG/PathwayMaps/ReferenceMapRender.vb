Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
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
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        <Extension>
        Public Function Render(model As XGMMLgraph,
                               Optional canvasSize$ = "11480,9200",
                               Optional enzymeColorSchema$ = "Set1:c8",
                               Optional compoundColorSchema$ = "Clusters",
                               Optional reactionShapeStrokeCSS$ = "stroke: white; stroke-width: 5px; stroke-dash: dash;",
                               Optional convexHull As String() = Nothing) As GraphicsData

            Return model.ToNetworkGraph("label", "class", "group.category", "group.category.color") _
                .Render(canvasSize:=canvasSize,
                        enzymeColorSchema:=enzymeColorSchema,
                        compoundColorSchema:=compoundColorSchema,
                        reactionShapeStrokeCSS:=reactionShapeStrokeCSS,
                        convexHull:=convexHull
                )
        End Function

        ''' <summary>
        ''' 将完成node和edge布局操作的网络模型进行渲染
        ''' </summary>
        ''' <returns></returns>
        <Extension>
        Public Function Render(graph As NetworkGraph,
                               Optional canvasSize$ = "11480,9200",
                               Optional enzymeColorSchema$ = "Set1:c8",
                               Optional compoundColorSchema$ = "Clusters",
                               Optional reactionShapeStrokeCSS$ = "stroke: white; stroke-width: 5px; stroke-dash: dash;",
                               Optional hideCompoundCircle As Boolean = True,
                               Optional convexHull As Index(Of String) = Nothing) As GraphicsData

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
            Dim rectShadow As New Shadow(10, 30, 1.125, 1.25)
            Dim circleShadow As New Shadow(130, 45, 2, 2)

            Dim drawNode As DrawNodeShape =
                Function(id$, g As IGraphics, br As Brush, radius!, center As PointF)
                    Dim node As Node = nodes(id)
                    Dim connectedNodes = graph.GetConnectedVertex(id)
                    Dim rect As Rectangle

                    If node.label.IsPattern("C\d+") Then
                        ' 圆形
                        radius = radius * 0.5
                        rect = New Rectangle With {
                            .X = center.X - radius / 2,
                            .Y = center.Y - radius / 2,
                            .Width = radius,
                            .Height = radius
                        }

                        If Not hideCompoundCircle Then
                            Call circleShadow.Circle(g, center, radius)

                            Call g.FillEllipse(br, rect)
                            Call g.DrawEllipse(New Pen(DirectCast(br, SolidBrush).Color.Alpha(200).Darken, 10), rect)
                        End If
                    Else
                        ' 方形
                        rect = New Rectangle With {
                            .X = center.X - radius / 2.25,
                            .Y = center.Y - radius / 5,
                            .Width = radius * 1.125,
                            .Height = radius / 2.5
                        }

                        br = New SolidBrush(DirectCast(br, SolidBrush).Color.Alpha(240))

                        Call rectShadow.RoundRectangle(g, rect, 30)
                        Call g.FillPath(br, RoundRect.GetRoundedRectPath(rect, 30))
                        Call g.DrawPath(reactionShapeStroke, RoundRect.GetRoundedRectPath(rect, 30))
                    End If

                    Return rect
                End Function
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
            Dim categoryColors = allCategories _
                .Select(Function(c)
                            Return graph.vertex _
                                .First(Function(n)
                                           Return n.data("group.category") = c
                                       End Function) _
                                .data("group.category.color")
                        End Function) _
                .ToArray

            If Not allCategories.IsNullOrEmpty Then
                Call $"Network canvas will render {allCategories.Length} category data for convexHull...".__INFO_ECHO

                For Each category As String In allCategories
                    Call category.__INFO_ECHO
                Next
            End If

            Dim getFontSize As Func(Of Node, Single) =
                Function(node As Node) As Single
                    If node.label.IsPattern("C\d+") Then
                        Return 27
                    Else
                        Return 40
                    End If
                End Function

            Return NetworkVisualizer.DrawImage(
                net:=graph,
                background:="white",'"transparent",
                padding:="padding: 500px 500px 500px 500px;",
                canvasSize:=canvasSize,
                labelerIterations:=0,
                doEdgeBundling:=True,
                drawNodeShape:=drawNode,
                hullPolygonGroups:=New NamedValue(Of String) With {
                    .Name = "group.category",
                    .Value = allCategories.JoinBy(","),
                    .Description = categoryColors.JoinBy(",")
                },
                minLinkWidth:=3,
                nodeRadius:=300,
                edgeShadowDistance:=0,
                edgeDashTypes:=DashStyle.Solid,
                defaultEdgeColor:="black",
                getNodeLabel:=AddressOf getNodeLabel,
                getLabelPosition:=getLabelPositoon，
                labelTextStroke:=Nothing,
                labelFontBase:="font-style: normal; font-size: 24; font-family: " & FontFace.MicrosoftYaHei & ";",
                fontSize:=getFontSize,
                defaultLabelColor:="white",
                getLabelColor:=Function(node As Node) As Color
                                   If node.label.IsPattern("C\d+") Then
                                       Return Color.Black
                                   Else
                                       Return Color.White
                                   End If
                               End Function
            )
        End Function

        Private Function getNodeLabel(node As Node) As String
            If node.label.IsPattern("C\d+") Then
                Return compoundNames.TryGetValue(node.label, [default]:=node.data!label)
            ElseIf node.label.IsPattern("R\d+") Then
                Return reactionNames.TryGetValue(node.label, [default]:=node.data!label)
            Else
                Return node.label
            End If
        End Function
    End Module
End Namespace