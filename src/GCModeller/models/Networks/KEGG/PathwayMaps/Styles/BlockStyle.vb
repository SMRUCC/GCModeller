#Region "Microsoft.VisualBasic::070a3a170843280634b5f6142c697a5d, models\Networks\KEGG\PathwayMaps\Styles\BlockStyle.vb"

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

'     Class BlockStyle
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes
Imports Microsoft.VisualBasic.MIME.Markup.HTML.CSS

Namespace PathwayMaps.RenderStyles

    ''' <summary>
    ''' The default style
    ''' </summary>
    Public Class BlockStyle : Inherits RenderStyle

        Sub New(nodes As Dictionary(Of String, Node), graph As NetworkGraph,
                Optional reactionShapeStrokeCSS$ = "stroke: white; stroke-width: 5px; stroke-dash: dash;",
                Optional hideCompoundCircle As Boolean = True)

            Call MyBase.New(nodes, graph)

            Me.reactionShapeStroke = Stroke.TryParse(reactionShapeStrokeCSS)
            Me.hideCompoundCircle = hideCompoundCircle
        End Sub

        Public Overrides Function getFontSize(node As Node) As Single
            If node.label.IsPattern("C\d+") Then
                Return 36
            Else
                Return 36
            End If
        End Function

        Dim reactionShapeStroke As Pen
        Dim rectShadow As New Shadow(10, 30, 1.125, 1.25)
        Dim circleShadow As New Shadow(130, 45, 2, 2)
        Dim hideCompoundCircle As Boolean = True

        Public Overrides Function drawNode(id As String, g As IGraphics, br As Brush, radius As Single, center As PointF) As RectangleF
            Dim node As Node = Nodes(id)
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
                            .X = center.X - radius / 2,
                            .Y = center.Y - radius / 5,
                            .Width = radius,
                            .Height = radius / 2.5
                        }

                br = New SolidBrush(DirectCast(br, SolidBrush).Color.Alpha(240))

                Call rectShadow.RoundRectangle(g, rect, 30)
                Call g.FillPath(br, RoundRect.GetRoundedRectPath(rect, 30))
                Call g.DrawPath(reactionShapeStroke, RoundRect.GetRoundedRectPath(rect, 30))
            End If

            Return rect
        End Function
    End Class
End Namespace
