#Region "Microsoft.VisualBasic::841ecbe5fc67aee052137fca4b7044f3, visualize\Cytoscape\Cytoscape\Graph\Visualization\GraphDrawing.vb"

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

    '     Module GraphDrawing
    ' 
    '         Function: __calculation, getRectange, getSize, (+2 Overloads) InvokeDrawing
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Math2D
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.ReferenceMap
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML

Namespace CytoscapeGraphView

    Public Module GraphDrawing

        ''' <summary>
        ''' 可选的
        ''' </summary>
        ''' <param name="str"></param>
        ''' <returns></returns>
        Public Function getSize(str As String) As Size
            If String.IsNullOrEmpty(str) Then
                Return Nothing
            End If

            Dim Tokens As String() = str.Split(","c)
            If Tokens.Count > 1 Then
                Return New Size(Val(Tokens(Scan0)), Val(Tokens(1)))
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' 假若目标网络模型文件是从Cytoscape之中导出来的，并且带有位置信息，则可以使用本方法仅会绘制更高质量的图片
        ''' </summary>
        ''' <param name="graph"></param>
        ''' <param name="size"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InvokeDrawing(graph As Graph, size As Size) As Image
            Dim Bitmap As Bitmap = New Bitmap(size.Width, size.Height)
            Dim grSize = graph.Size

            Call $"{NameOf(size)}:={size.ToString }".__DEBUG_ECHO

            Dim xScale As Double = (size.Width / grSize.Width) * graph.Graphics.ScaleFactor
            Dim yScale As Double = (size.Height / grSize.Height) * graph.Graphics.ScaleFactor

            Using GrDevice As Drawing.Graphics = Drawing.Graphics.FromImage(Bitmap)
                Call GrDevice.FillRectangle(Brushes.White, New Rectangle(New Point, size))

                Dim Nodes = graph.Nodes.ToDictionary(Function(n) n.id,
                                                     Function(x) New Node(x, xScale, yScale))

                Dim RECT = getRectange(Nodes.Values.ToArray)  '得到绘图区域，然后将区域移动到设备的中间
                Dim offSet = New Point((size.Width - RECT.Width) / 2 - RECT.X, (size.Height - RECT.Height) / 2 - RECT.Y)

                For Each node In Nodes.Values
                    Call node.OffSet(offSet)
                Next

                Dim ss = (xScale + yScale) / 2
                Dim ssLabel = ss * 0.5

                For Each Edge In graph.Edges
                    Dim pt1 = Nodes(Edge.source), pt2 = Nodes(Edge.target)
                    Dim a = pt1.Point_getInterface(pt2)
                    Dim b = pt2.Point_getInterface(pt1) '这个点是箭头的指向
                    Dim Color As Color = Edge.Graphics.LineColor
                    Dim pen = New Pen(Color, Edge.Graphics.Width * ss)

                    pen.DashStyle = Drawing2D.DashStyle.Dash
                    pen.EndCap = Drawing2D.LineCap.ArrowAnchor

                    Call GrDevice.DrawLine(pen, a, b)

                    '中点
                    Dim m = New Point((a.X + b.X) / 2, (a.Y + b.Y) / 2)
                    Dim lbFont = Edge.Graphics.GetLabelFont(ssLabel)
                    Dim lbColor = Edge.Graphics.FontColor
                    Dim lb As String = Edge.Graphics.EdgeLabel
                    Dim sz = GrDevice.MeasureString(lb, lbFont)

                    m = New Point(m.X - sz.Width / 2, m.Y - sz.Height / 2)
                    Call GrDevice.DrawString(lb, lbFont, New SolidBrush(lbColor), m)
                Next


                For Each node In Nodes.Values

                    Dim brush As New SolidBrush(node.NodeModel.Graphics.FillColor)
                    Call GrDevice.FillPie(brush, node.Rectangle, 0, 360)

                    Dim Font As Font = node.NodeModel.Graphics.GetLabelFont(ssLabel)
                    Dim sz = GrDevice.MeasureString(node.NodeModel.label, Font)

                    Call GrDevice.DrawString(node.NodeModel.label,
                                             Font,
                                              New SolidBrush(node.NodeModel.Graphics.LabelColor),
                                             New Point(node.Rectangle.X + (node.Rectangle.Width - sz.Width) / 2, node.Rectangle.Y + (node.Rectangle.Height - sz.Height) / 2))
                Next

            End Using

            Return Bitmap
        End Function

        Private Function getRectange(Nodes As Node()) As Rectangle
            Dim X = (From n In Nodes.AsParallel Select value = n.Location.X).ToArray
            Dim Y = (From n In Nodes.AsParallel Select value = n.Location.Y).ToArray
            Return New Rectangle(x:=X.Min, y:=Y.Min, width:=X.Max - X.Min, height:=Y.Max - Y.Min)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="Graph">Cytoscape网络模型</param>
        ''' <param name="refMap">参考代谢途径</param>
        ''' <param name="map">需要进行Mapping的KEGG物种编号</param>
        ''' <param name="Size"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Extension>
        Public Function InvokeDrawing(Graph As Graph,
                                      refMap As ReferenceMapData,
                                      map As String(),
                                      Optional Size As String = "",
                                      Optional alpha As Integer = 255,
                                      Optional Scale As Double = 1) As Image

            Dim _size As Size = If(String.IsNullOrEmpty(Size),
                Graph.GetSize(Scale),
                New Size(Val(Size.Split(CChar(",")).First), Val(Size.Split(CChar(",")).Last)))

            Using gdi As Graphics2D = _size.CreateGDIDevice
                Dim offset As Point = New Point(30, 35)

                Call Size.__DEBUG_ECHO

                Dim Nodes = Graph.Nodes.ToDictionary(Function(n) n.id)
                Dim Colors = GenerateColorProfiles(map)

                For Each Edge In Graph.Edges
                    Dim pt1 = Nodes(Edge.source), pt2 = Nodes(Edge.target)
                    Call gdi.DrawLine(New Pen(Brushes.Gray, 2),
                                     New Point(pt1.Graphics.x * Scale, pt1.Graphics.y * Scale).OffSet2D(offset),
                                     New Point(pt2.Graphics.x * Scale, pt2.Graphics.y * Scale).OffSet2D(offset))
                Next

                For Each Node In Graph.Nodes
                    Dim Orthology = refMap.GetReaction(Node("KEGG_ENTRY").Value).SSDBs
                    Dim KO_sp As String() = (From Entry In (From ort In Orthology Select ort.Value).ToArray.Unlist Select Entry.speciesID Distinct).ToArray
                    Dim ColorList = (From sp As String In KO_sp Where Colors.ContainsKey(sp) Select sp, sp_Color = Colors(sp)).ToArray
                    Dim Color As Color

                    If Not ColorList.IsNullOrEmpty Then
                        Dim R = (From cl In ColorList Select clR = CDbl(cl.sp_Color.R)).ToArray.Average
                        Dim G = (From cl In ColorList Select clG = CDbl(cl.sp_Color.G)).ToArray.Average
                        Dim B = (From cl In ColorList Select clB = CDbl(cl.sp_Color.B)).ToArray.Average
                        Color = Drawing.Color.FromArgb(alpha, R, G, B)

                        Call gdi.Graphics.DrawString(String.Join("; ", (From cl In ColorList Select cl.sp).ToArray), New Font(FontFace.Ubuntu, 6), Brushes.Red, New Point(Node.Graphics.x * Scale, Node.Graphics.y * Scale - Node.Graphics.h * 0.2))
                    Else
                        Color = System.Drawing.Color.FromArgb(alpha, Drawing.Color.Blue)
                    End If

                    Dim IsPie As Boolean
                    Dim Rect As Rectangle = __calculation(Node, IsPie, offset, Scale)
                    ' Dim BigRect As Rectangle = New Rectangle(Microsoft.VisualBasic.OffSet(Rect.Location, 5, 5), New Size(Rect.Size.Width + 2.5, Rect.Height + 2.5))

                    If IsPie Then
                        '     Call Gr.Gr_Device.FillPie(Brushes.White, BigRect, 0, 360)
                        Call gdi.Graphics.FillPie(New SolidBrush(Color), Rect, 0, 360)
                    Else
                        '   Call Gr.Gr_Device.FillRectangle(Brushes.White, BigRect)
                        Call gdi.Graphics.FillRectangle(New SolidBrush(Color), Rect)
                    End If

                    Call gdi.Graphics.DrawString(Node.label, New Font("Ubuntu", 10), Brushes.Black, New Point(Node.Graphics.x * Scale, Node.Graphics.y * Scale))
                Next

                Return gdi.ImageResource
            End Using
        End Function

        Private Function __calculation(Node As XGMML.Node, ByRef IsPie As Boolean, OffSet As Point, Scale As Integer) As Rectangle
            Dim rt As Double = Math.Abs(Node.Graphics.w - Node.Graphics.h) / Math.Min(Node.Graphics.w, Node.Graphics.h)
            Dim DEntry = Node("Degree")
            Dim Degree As Integer = If(DEntry Is Nothing, 1, CInt(Val(DEntry.Value)))

            If Degree = 0 Then
                Degree = 1
            Else
                Degree = Math.Sqrt(Degree)
                If Degree = 0 Then
                    Degree = 1
                End If
            End If

            Dim RectSize = New Size(Node.Graphics.w * Degree, Node.Graphics.h * Degree)
            Dim RectLoc As New Point(Node.Graphics.x * Scale - RectSize.Width / 2,
                                     Node.Graphics.y * Scale - RectSize.Height / 2)

            IsPie = Not rt > 0.05

            Return New Rectangle(RectLoc.OffSet2D(OffSet), RectSize)
        End Function
    End Module
End Namespace
