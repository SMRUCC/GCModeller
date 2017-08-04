Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.GraphAPI
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.ConvexHull
Imports Microsoft.VisualBasic.Imaging.LayoutModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS
Imports SMRUCC.genomics.Assembly.KEGG.WebServices
Imports GraphLayout = Microsoft.VisualBasic.Data.visualize.Network.Layouts

Public Module FunctionalNetwork

    Public Const Delimiter$ = " == "

    ''' <summary>
    ''' Using for the group values inforamtion for <see cref="BuildModel"/> function.
    ''' </summary>
    ''' <returns></returns>
    Public Function KOGroupTable() As Dictionary(Of String, String)
        Return PathwayMapping _
            .DefaultKOTable _
            .ToDictionary(Function(KO) KO.Key,
                          Function(KO) KO.Value.Parent.Description)
    End Function

    ''' <summary>
    ''' 这个函数需要编写一个网络布局生成函数的参数配置文件
    ''' </summary>
    ''' <param name="model"></param>
    ''' <returns></returns>
    <Extension>
    Public Function VisualizeKEGG(model As NetworkTables,
                                  Optional layouts As ILayoutCoordinate() = Nothing,
                                  Optional size$ = "6000,5000",
                                  Optional colorSchema$ = "Set1:c9",
                                  Optional scale# = 4.5,
                                  Optional radius$ = "5,20",
                                  Optional KEGGNameFont$ = CSSFont.Win7LargerNormal,
                                  Optional margin% = 100,
                                  Optional groupLowerBounds% = 3,
                                  Optional quantile# = 0.5,
                                  Optional delimiter$ = FunctionalNetwork.Delimiter,
                                  Optional fontSizeFactor# = 2.5,
                                  Optional polygonStroke$ = Stroke.AxisGridStroke) As Image

        Dim graph As NetworkGraph = model _
            .CreateGraph(
                nodeColor:=Function(n)
                               Return (n!color).GetBrush
                           End Function) _
            .ScaleRadius(range:=radius)

        If layouts.IsNullOrEmpty Then
            Dim defaultFile$ = App.InputFile.ParentPath & "/" & GraphLayout.Parameters.DefaultFileName
            Dim parameters As ForceDirectedArgs = GraphLayout.Parameters.Load(defaultFile)

            ' 生成layout信息               
            Call graph.doRandomLayout
            Call graph.doForceLayout(showProgress:=True, parameters:=parameters)
        Else
            ' 直接使用所提供的布局信息
            Dim layoutTable = layouts.ToDictionary(Function(x) x.ID)

            For Each node In graph.nodes
                With layoutTable(node.ID)
                    Dim point As New FDGVector2(.X * 1000, .Y * 1000)
                    node.Data.initialPostion = point
                End With
            Next
        End If

        Dim graphNodes = graph.nodes.ToDictionary
        Dim nodeGroups = model.Nodes _
            .Select(Function(n)
                        Return Strings _
                            .Split(n.NodeType, delimiter) _
                            .Select(Function(path) (path, n))
                    End Function) _
            .IteratesALL _
            .GroupBy(Function(x) x.Item1) _
            .Where(Function(g)
                       Return (Not g.Key.StringEmpty) AndAlso
                            g.Key <> "KEGG Compound" AndAlso
                            g.Count >= groupLowerBounds
                   End Function) _
            .ToDictionary(Function(g) g.Key,
                          Function(nodes)
                              Return nodes _
                                  .Select(Function(x)
                                              Return graphNodes(x.Item2.ID)
                                          End Function) _
                                  .ToArray
                          End Function)
        Dim colors As New LoopArray(Of Color)(Designer.GetColors(colorSchema))

        If nodeGroups.Count > colors.Length Then
            Dim q = nodeGroups.Count * (1 - quantile)
            Dim keys$() = nodeGroups _
                .AsGroups _
                .IGrouping _
                .OrderByDegrees _
                .Take(q) _
                .Keys

            nodeGroups = keys.ToDictionary(Function(key) key,
                                           Function(key) nodeGroups(key))
        End If

        Dim nodePoints As Dictionary(Of Graph.Node, Point) = Nothing
        Dim image As Image

        Call $"{colors.Length} colors --> {nodeGroups.Count} KEGG pathways".__DEBUG_ECHO

        Dim KEGGColors As New Dictionary(Of String, (counts#, color As Color))
        Dim dash As Dictionary(Of String, DashStyle)
        Dim strokePen As Stroke = Stroke.TryParse(polygonStroke)

        With New Dictionary(Of String, DashStyle)
            !pathway_internal = DashStyle.Solid
            !Unknown = DashStyle.Dash
            !pathway_outbounds = DashStyle.Dash

            dash = .ref
        End With

        Using g As Graphics2D = graph _
            .DrawImage(canvasSize:=size,
                       scale:=scale,
                       nodePoints:=nodePoints,
                       edgeDashTypes:=dash,
                       fontSizeFactor:=fontSizeFactor) _
            .AsGDIImage _
            .CreateCanvas2D(directAccess:=True)

            For Each pathway In nodeGroups.SeqIterator
                Dim nodes = (+pathway).Value
                Dim name$ = (+pathway).Key
                Dim polygon As Point() = nodePoints.Selects(nodes)

                Try
                    polygon = ConvexHull.GrahamScan(polygon)  ' 计算出KEGG代谢途径簇的边界点
                Catch ex As Exception
                    Continue For
                End Try

                If polygon.Length = 3 Then
                    polygon = polygon.Enlarge(scale:=2)
                Else
                    polygon = polygon.Enlarge(scale:=1.25)
                End If

                With colors.Next
                    Dim pen As New Pen(.ref, strokePen.width) With {
                        .DashStyle = strokePen.dash
                    }
                    Dim fill As New SolidBrush(Color.FromArgb(40, .ref))

                    Call g.DrawPolygon(pen, polygon)
                    Call g.FillPolygon(fill, polygon)

                    KEGGColors.Add(name, (nodes.Length, .ref))
                End With
            Next

            image = g.ImageResource.CorpBlank(margin, blankColor:=Color.White)
        End Using

        ' 在图片的左下角加入代谢途径的名称
        Using g As Graphics2D = image.CreateCanvas2D(directAccess:=True)
            Dim font As Font = CSSFont.TryParse(KEGGNameFont).GDIObject
            Dim dy = 5
            Dim X = margin
            Dim Y = g.Height - (font.Height + dy) * KEGGColors.Count - margin
            Dim rectSize As New Size(50, font.Height)

            For Each PATH In KEGGColors
                Dim name$ = PATH.Key.StringReplace("\[.+?\]", "")
                Dim genes = PATH.Value.counts
                Dim color As Color = PATH.Value.color
                Dim b As New SolidBrush(color)

                g.FillRectangle(b, New Rectangle(New Point(X, Y), rectSize))
                g.DrawString(name, font, Brushes.Black, New PointF(X + dy + rectSize.Width, Y))

                Y += dy + font.Height
            Next
        End Using

        Return image
    End Function
End Module
