﻿#Region "Microsoft.VisualBasic::c39e12450832444ed2ed8211c95252bd, models\Networks\KEGGGraphVisualizer\Styles\BlockStyle.vb"

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

    '   Total Lines: 126
    '    Code Lines: 101 (80.16%)
    ' Comment Lines: 8 (6.35%)
    '    - Xml Docs: 37.50%
    ' 
    '   Blank Lines: 17 (13.49%)
    '     File Size: 5.23 KB


    '     Class BlockStyle
    ' 
    '         Properties: edgeDashType
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: drawNode, getFontSize, getHullPolygonGroups, getLabelColor
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Shapes
Imports Microsoft.VisualBasic.MIME.Html.CSS
Imports Microsoft.VisualBasic.MIME.Html.Render

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

Namespace PathwayMaps.RenderStyles

    ''' <summary>
    ''' The default style
    ''' </summary>
    Public Class BlockStyle : Inherits RenderStyle

        Public Overrides ReadOnly Property edgeDashType As DashStyle
            Get
                Return DashStyle.Dot
            End Get
        End Property

        Sub New(graph As NetworkGraph,
                convexHullCategoryStyle As (allCategory As String(), categoryColors As String()),
                Optional reactionShapeStrokeCSS$ = "stroke: white; stroke-width: 5px; stroke-dash: dash;",
                Optional enzymeColorSchema$ = "Set1:c8",
                Optional compoundColorSchema$ = "Clusters",
                Optional hideCompoundCircle As Boolean = True)

            Call MyBase.New(
                graph:=graph,
                enzymeColorSchema:=enzymeColorSchema,
                compoundColorSchema:=compoundColorSchema
            )

            Me.reactionShapeStroke = Stroke.TryParse(reactionShapeStrokeCSS)
            Me.hideCompoundCircle = hideCompoundCircle
            Me.convexHullCategoryStyle = convexHullCategoryStyle
        End Sub

        Public Overrides Function getFontSize(node As Node) As Single
            'If node.label.IsPattern("C\d+") Then
            '    Return 36
            'Else
            '    Return 36
            'End If
            Return 32
        End Function

        Dim reactionShapeStroke As Stroke
        Dim rectShadow As New Shadow(10, 30, 1.125, 1.25)
        Dim circleShadow As New Shadow(130, 45, 2, 2)
        Dim hideCompoundCircle As Boolean = True
        Dim convexHullCategoryStyle As (allCategory As String(), categoryColors As String())

        Public Overrides Function drawNode(id As String, g As IGraphics, br As Brush, radius As Single(), center As PointF) As RectangleF
            Dim node As Node = nodes(id)
            Dim rect As Rectangle = getNodeLayout(id, radius, center)
            Dim css As CSSEnvirnment = g.LoadEnvironment

            If node.label.IsPattern("C\d+") Then
                If Not hideCompoundCircle Then
                    Call circleShadow.Circle(g, center, radius.Average)

                    Call g.FillEllipse(br, rect)
                    Call g.DrawEllipse(New Pen(DirectCast(br, SolidBrush).Color.Alpha(200).Darken, 10), rect)
                End If
            Else
                br = New SolidBrush(DirectCast(br, SolidBrush).Color.Alpha(240))

                Call rectShadow.RoundRectangle(g, rect, 30)
                Call g.FillPath(br, RoundRect.GetRoundedRectPath(rect, 30))
                Call g.DrawPath(css.GetPen(reactionShapeStroke), RoundRect.GetRoundedRectPath(rect, 30))
            End If

            Return rect
        End Function

        Dim yellow As Color = "#f5f572".TranslateColor

        Public Overrides Function getLabelColor(node As Node) As Color
            If node.label.IsPattern("C\d+") Then
                Return Color.Black
            ElseIf DirectCast(node.data.color, SolidBrush).Color.EuclideanDistance(yellow) <= 30 Then
                Return Color.DarkBlue
            Else
                Return Color.White
            End If
        End Function

        Public Overrides Function getHullPolygonGroups() As NamedValue(Of String)
            Return New NamedValue(Of String) With {
                .Name = "group.category",
                .Value = convexHullCategoryStyle.allCategory.JoinBy(","),
                .Description = convexHullCategoryStyle.categoryColors.JoinBy(",")
            }
        End Function
    End Class
End Namespace
