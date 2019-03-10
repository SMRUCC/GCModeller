﻿#Region "Microsoft.VisualBasic::e0bea63a5303a0284e283fa9c046f405, gr\network-visualization\NetworkCanvas\SVG.vb"

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

    ' Module SVGExtensions
    ' 
    '     Function: DefaultStyle, ToSVG
    '     Delegate Function
    ' 
    '         Function: __getRadius, Get2DPoint, Get3DPoint
    ' 
    '         Sub: WriteLayouts
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.Interfaces
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.d3js.SVG
Imports Microsoft.VisualBasic.Imaging.d3js.SVG.CSS
Imports Microsoft.VisualBasic.Imaging.Drawing3D
Imports Microsoft.VisualBasic.Imaging.Drawing3D.Math3D
Imports Microsoft.VisualBasic.Imaging.SVG
Imports Microsoft.VisualBasic.Imaging.SVG.CSS
Imports Microsoft.VisualBasic.Imaging.SVG.XML
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.Markup.HTML

''' <summary>
''' <see cref="NetworkGraph"/> to svg doc
''' </summary>
Public Module SVGExtensions

    Public Function DefaultStyle() As CSS.DirectedForceGraph
        Return New DirectedForceGraph With {
            .link = New CssValue With {
                .stroke = "#CCC",
                .strokeOpacity = "0.85",
                .strokeWidth = "6"
            },
            .node = New CssValue With {
                .strokeWidth = "0.5px",
                .strokeOpacity = "0.8",
                .stroke = "#FFF",
                .opacity = "0.85"
            },
            .text = New Font
        }
    End Function

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="graph"></param>
    ''' <param name="style">Default value is <see cref="DefaultStyle"/></param>
    ''' <param name="size">The export canvas size</param>
    ''' <returns></returns>
    <Extension>
    Public Function ToSVG(graph As NetworkGraph,
                          size As Size,
                          Optional style As CSS.DirectedForceGraph = Nothing,
                          Optional is3D As Boolean = False,
                          Optional viewDistance As Integer = -120) As SVGXml

        Dim rect As New Rectangle(New Point, size)
        Dim getPoint As IGetPoint = If(
            is3D,
            New IGetPoint(AddressOf Get3DPoint),
            New IGetPoint(AddressOf Get2DPoint))
        Dim nodes As circle() =
            LinqAPI.Exec(Of circle) <= From n As Graph.Node
                                       In graph.nodes
                                       Let pos As Point = getPoint(n, rect, viewDistance)
                                       Let c As Color = If(
                                               TypeOf n.Data.Color Is SolidBrush,
                                               DirectCast(n.Data.Color, SolidBrush).Color,
                                               Color.Black)
                                       Let r As Single = n.__getRadius
                                       Let pt As Point =
                                               New Point(CInt(pos.X - r / 2), CInt(pos.Y - r / 2))
                                       Select New circle With {
                                           .class = "node",
                                           .cx = pt.X,
                                           .cy = pt.Y,
                                           .r = r,
                                           .style = $"fill: rgb({c.R}, {c.G}, {c.B});"
                                       }
        Dim links As line() =
            LinqAPI.Exec(Of line) <= From edge As Edge
                                     In graph.edges
                                     Let source As Graph.Node = edge.U
                                     Let target As Graph.Node = edge.V
                                     Let pts As Point = getPoint(source, rect, viewDistance)
                                     Let ptt As Point = getPoint(target, rect, viewDistance)
                                     Let rs As Single = source.__getRadius / 2,
                                         rt As Single = target.__getRadius / 2
                                     Select New line With {
                                         .class = "link",
                                         .x1 = pts.X - rs,
                                         .x2 = ptt.X - rt,
                                         .y1 = pts.Y - rs,
                                         .y2 = ptt.Y - rt
                                     }
        Dim labels As SVG.XML.text() = LinqAPI.Exec(Of SVG.XML.text) <=
 _
            From n As Graph.Node
            In graph.nodes
            Let pos As Point = getPoint(n, rect, viewDistance)
            Select New SVG.XML.text With {
                .x = CStr(pos.X),
                .y = CStr(pos.Y),
                .value = n.ID,
                .class = "text"
            }
        Dim svg As New SVGXml With {
            .defs = New CSSStyles With {
                .styles = {
                    New XmlMeta.CSS With {
                        .style = If(style Is Nothing, DefaultStyle(), style).ToString
                    }
                }
            },
            .width = size.Width & "px",
            .height = size.Height & "px",
            .lines = links,
            .circles = nodes,
            .texts = labels
        }

        Return svg
    End Function

    Public Delegate Function IGetPoint(node As Graph.Node, rect As Rectangle, viewDistance As Integer) As Point

    <Extension>
    Public Function Get2DPoint(node As Graph.Node, rect As Rectangle, viewDistance As Integer) As Point
        Return Renderer.GraphToScreen(TryCast(node.Data.initialPostion, FDGVector2), rect)
    End Function

    <Extension>
    Public Function Get3DPoint(node As Graph.Node, rect As Rectangle, viewDistance As Integer) As Point
        Dim d3 As FDGVector3 = TryCast(node.Data.initialPostion, FDGVector3)
        Dim pt3 As New Point3D(d3.x, d3.y, d3.z)
        Return pt3.Project(rect.Width, rect.Height, 256, viewDistance).PointXY
    End Function

    <Extension>
    Private Function __getRadius(n As Graph.Node) As Single
        Dim r As Single = n.Data.radius
        Dim rd As Single = If(r = 0!, If(n.Data.Neighborhoods < 30, n.Data.Neighborhoods * 9, n.Data.Neighborhoods * 7), r)
        Dim r2 As Single = If(rd = 0, 10.0!, rd) / 2.5!

        Return r2
    End Function

    ''' <summary>
    ''' Write the node layout position into its extensions data, for generates the svg graphics.
    ''' </summary>
    ''' <param name="graph"></param>
    ''' <param name="engine"></param>
    <Extension>
    Public Sub WriteLayouts(ByRef graph As NetworkGraph, engine As IForceDirected)
        If TypeOf engine Is ForceDirected2D Then
            For Each node As Graph.Node In graph.nodes
                node.Data.initialPostion =
                    New FDGVector2(engine.GetPoint(node).position.Point2D)
            Next
        ElseIf TypeOf engine Is ForceDirected3D Then
            For Each node As Graph.Node In graph.nodes
                Dim pos = engine.GetPoint(node).position
                node.Data.initialPostion =
                    New FDGVector3(pos.x, pos.y, pos.z)
            Next
        End If
    End Sub
End Module
