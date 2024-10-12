﻿#Region "Microsoft.VisualBasic::a195a382c23b9a7a772df3ae61a26721, models\Networks\KEGGGraphVisualizer\FunctionalNetwork.vb"

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

    '   Total Lines: 216
    '    Code Lines: 169 (78.24%)
    ' Comment Lines: 15 (6.94%)
    '    - Xml Docs: 73.33%
    ' 
    '   Blank Lines: 32 (14.81%)
    '     File Size: 9.24 KB


    ' Module FunctionalNetwork
    ' 
    '     Function: applyLayout, VisualizeKEGG
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.ComponentModel.DataStructures
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.visualize.Network
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.GraphAPI
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.SpringForce
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.BitmapImage
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Math2D.ConvexHull
Imports Microsoft.VisualBasic.Imaging.LayoutModel
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render
Imports Microsoft.VisualBasic.Scripting.Runtime
Imports Microsoft.VisualBasic.Imaging.Driver


#If NET48 Then
Imports Pen = System.Drawing.Pen
Imports Pens = System.Drawing.Pens
Imports Brush = System.Drawing.Brush
Imports Font = System.Drawing.Font
Imports Brushes = System.Drawing.Brushes
Imports SolidBrush = System.Drawing.SolidBrush
Imports DashStyle = System.Drawing.Drawing2D.DashStyle
Imports Image = System.Drawing.Image
Imports Bitmap = System.Drawing.Bitmap
Imports GraphicsPath = System.Drawing.Drawing2D.GraphicsPath
Imports FontStyle = System.Drawing.FontStyle
#Else
Imports Pen = Microsoft.VisualBasic.Imaging.Pen
Imports Pens = Microsoft.VisualBasic.Imaging.Pens
Imports Brush = Microsoft.VisualBasic.Imaging.Brush
Imports Font = Microsoft.VisualBasic.Imaging.Font
Imports Brushes = Microsoft.VisualBasic.Imaging.Brushes
Imports SolidBrush = Microsoft.VisualBasic.Imaging.SolidBrush
Imports DashStyle = Microsoft.VisualBasic.Imaging.DashStyle
Imports Image = Microsoft.VisualBasic.Imaging.Image
Imports Bitmap = Microsoft.VisualBasic.Imaging.Bitmap
Imports GraphicsPath = Microsoft.VisualBasic.Imaging.GraphicsPath
Imports FontStyle = Microsoft.VisualBasic.Imaging.FontStyle
#End If

Public Module FunctionalNetwork

    ''' <summary>
    ''' 直接使用所提供的布局信息
    ''' </summary>
    ''' <param name="graph"></param>
    ''' <param name="layouts"></param>
    ''' <returns></returns>
    <Extension>
    Public Function applyLayout(graph As NetworkGraph, layouts As ILayoutCoordinate()) As NetworkGraph
        Dim layoutTable = layouts.ToDictionary(Function(x) x.ID)

        For Each node In graph.vertex
            With layoutTable(node.label)
                Dim point As New FDGVector2(.X * 1000, .Y * 1000)
                node.data.initialPostion = point
            End With
        Next

        Return graph
    End Function

    ''' <summary>
    ''' 这个函数需要编写一个网络布局生成函数的参数配置文件
    ''' </summary>
    ''' <param name="graph"></param>
    ''' <returns></returns>
    <Extension>
    Public Function VisualizeKEGG(graph As NetworkGraph,
                                  Optional layouts As ILayoutCoordinate() = Nothing,
                                  Optional size$ = "10000,7000",
                                  Optional colorSchema$ = "Set1:c9",
                                  Optional radius$ = "50,350",
                                  Optional KEGGNameFont$ = CSSFont.Win7LargerNormal,
                                  Optional margin% = 100,
                                  Optional groupLowerBounds% = 3,
                                  Optional quantile# = 0.5,
                                  Optional delimiter$ = SimpleBuilder.Delimiter,
                                  Optional polygonStroke$ = Stroke.AxisGridStroke,
                                  Optional ppi As Integer = 100) As Image

        Call graph.ApplyAnalysis()

        For Each node In graph.vertex
            node.data.size = {Val(node.data(NamesOf.REFLECTION_ID_MAPPING_DEGREE))}
        Next

        Call graph.ScaleRadius(range:=DoubleRange.TryParse(radius))

        If layouts.IsNullOrEmpty Then
            Dim defaultFile$ = App.InputFile.ParentPath & "/" & SpringForce.Parameters.DefaultFileName
            Dim parameters As ForceDirectedArgs = SpringForce.Parameters.Load(defaultFile, New ForceDirectedArgs With {.Damping = 0.2, .Iterations = 2000, .Repulsion = 1000, .Stiffness = 80})

            ' 生成layout信息               
            Call graph.doRandomLayout
            Call graph.doForceLayout(showProgress:=True, parameters:=parameters)
        Else
            ' 直接使用所提供的布局信息
            Call graph.applyLayout(layouts)
        End If

        Dim graphNodes As Dictionary(Of Graph.Node) = graph.vertex.ToDictionary
        Dim nodeGroups = graph.vertex _
            .Select(Function(n)
                        Return Strings _
                            .Split(n.data(NamesOf.REFLECTION_ID_MAPPING_NODETYPE), delimiter) _
                            .Select(Function(path)
                                        Return (path, n)
                                    End Function)
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
            Dim q As Double = nodeGroups.Count * (1 - quantile)
            Dim keys$() = nodeGroups _
                .AsGroups _
                .IGrouping _
                .OrderByDegrees _
                .Take(q) _
                .Keys

            nodeGroups = keys.ToDictionary(Function(key) key,
                                           Function(key)
                                               Return nodeGroups(key)
                                           End Function)
        End If

        Dim image As Image

        Call $"{colors.Length} colors --> {nodeGroups.Count} KEGG pathways".__DEBUG_ECHO

        Dim KEGGColors As New Dictionary(Of String, (counts#, color As Color))
        Dim dash As Dictionary(Of String, DashStyle)
        Dim strokePen As Stroke = Stroke.TryParse(polygonStroke)

        With New Dictionary(Of String, DashStyle)
            !pathway_internal = DashStyle.Solid
            !Unknown = DashStyle.Dash
            !pathway_outbounds = DashStyle.Dash

            dash = .ByRef
        End With

        Dim nodePoints = CanvasScaler.CalculateNodePositions(graph, size.SizeParser, g.DefaultPadding)
        Dim graphViz = graph _
            .DrawImage(canvasSize:=size,
                       edgeDashTypes:=dash,
                       minLinkWidth:=5,
                       nodeRadius:=DirectMapRadius(),
                       fontSize:=DirectMapRadius(0.5),
                       labelerIterations:=0,
                       labelTextStroke:=Nothing
            ) _
            .AsGDIImage

        Using g As IGraphics = DriverLoad.CreateGraphicsDevice(graphViz, direct_access:=True)

            For Each pathway In nodeGroups.SeqIterator
                Dim nodes = (+pathway).Value
                Dim name$ = (+pathway).Key
                Dim polygon As PointF() = nodes.Select(Function(n) nodePoints(n.label)).ToArray

                Try
                    ' 计算出KEGG代谢途径簇的边界点
                    polygon = ConvexHull.GrahamScan(polygon)
                Catch ex As Exception
                    Continue For
                End Try

                If polygon.Length = 3 Then
                    polygon = polygon.Enlarge(scale:=2)
                Else
                    polygon = polygon.Enlarge(scale:=1.25)
                End If

                With colors.Next
                    Dim pen As New Pen(.ByRef, strokePen.width) With {
                        .DashStyle = strokePen.dash
                    }
                    Dim fill As New SolidBrush(Color.FromArgb(40, .ByRef))

                    Call g.DrawPolygon(pen, polygon)
                    Call g.FillPolygon(fill, polygon)

                    KEGGColors.Add(name, (nodes.Length, .ByRef))
                End With
            Next

            image = DirectCast(g, GdiRasterGraphics).ImageResource
            ' image = g.ImageResource.CorpBlank(margin, blankColor:=Color.White)
        End Using

        ' 在图片的左下角加入代谢途径的名称
        Using g As IGraphics = DriverLoad.CreateGraphicsDevice(image, direct_access:=True)
            Dim css As CSSEnvirnment = g.LoadEnvironment
            Dim font As Font = css.GetFont(CSSFont.TryParse(KEGGNameFont))
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
