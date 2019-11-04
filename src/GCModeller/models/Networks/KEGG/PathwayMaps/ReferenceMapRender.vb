Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Driver
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace PathwayMaps

    Public Module ReferenceMapRender

        ''' <summary>
        ''' 将完成node和edge布局操作的网络模型进行渲染
        ''' </summary>
        ''' <param name="model"></param>
        ''' <returns></returns>
        <Extension>
        Public Function Render(model As XGMMLgraph,
                               Optional canvasSize$ = "11480,9200",
                               Optional enzymeColorSchema$ = "Set1:c8",
                               Optional compoundColorSchema$ = "Clusters") As GraphicsData

            Dim graph As NetworkGraph = model.ToNetworkGraph
            Dim nodes As New Dictionary(Of String, Node)
            Dim fluxCategory = EnzymaticReaction.LoadFromResource.GroupBy(Function(r) r.Entry.Key).ToDictionary(Function(r) r.Key, Function(r) r.First)
            Dim compoundCategory = CompoundBrite.CompoundsWithBiologicalRoles.GroupBy(Function(c) c.entry.Key).ToDictionary(Function(c) c.Key, Function(c) c.First.class)
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

            Dim drawNode As DrawNodeShape =
                Sub(id$, g As IGraphics, br As Brush, radius!, center As PointF)
                    Dim node As Node = nodes(id)

                    br = New SolidBrush(DirectCast(br, SolidBrush).Color.Alpha(240))

                    If node.label.IsPattern("C\d+") Then
                        ' 圆形
                        Dim rect As New Rectangle With {
                            .X = center.X - radius,
                            .Y = center.Y + radius,
                            .Width = radius,
                            .Height = radius
                        }

                        Call g.FillEllipse(br, rect)
                    Else
                        ' 方形
                        Dim rect As New Rectangle With {
                            .X = center.X - radius * 3 / 4,
                            .Y = center.Y + radius / 2,
                            .Width = radius,
                            .Height = radius / 2
                        }

                        Call g.FillRectangle(br, rect)
                    End If
                End Sub

            Return NetworkVisualizer.DrawImage(
                net:=graph,
                padding:="padding: 300px 300px 300px 300px;",
                canvasSize:=canvasSize,
                labelerIterations:=5,
                doEdgeBundling:=True,
                drawNodeShape:=drawNode,
                minLinkWidth:=8,
                nodeRadius:=220,
                edgeShadowDistance:=5,
                defaultEdgeColor:=NameOf(Color.Gray)
            )
        End Function
    End Module
End Namespace