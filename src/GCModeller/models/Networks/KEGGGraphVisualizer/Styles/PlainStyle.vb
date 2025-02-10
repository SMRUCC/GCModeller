#Region "Microsoft.VisualBasic::50017ba4328d7fd79e02137130c22e5e, models\Networks\KEGGGraphVisualizer\Styles\PlainStyle.vb"

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

    '   Total Lines: 116
    '    Code Lines: 96 (82.76%)
    ' Comment Lines: 3 (2.59%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 17 (14.66%)
    '     File Size: 4.63 KB


    '     Class PlainStyle
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

    Public Class PlainStyle : Inherits RenderStyle

        Public Overrides ReadOnly Property edgeDashType As DashStyle
            Get
                Return DashStyle.Solid
            End Get
        End Property

        Dim convexHullCategoryStyle As Dictionary(Of String, String)

        Public Sub New(graph As NetworkGraph,
                       convexHullCategoryStyle As Dictionary(Of String, String),
                       Optional enzymeColorSchema$ = "Set1:c8",
                       Optional compoundColorSchema$ = "Clusters")

            Call MyBase.New(
                graph:=graph,
                enzymeColorSchema:=enzymeColorSchema,
                compoundColorSchema:=compoundColorSchema
            )

            ' render edge color
            ' no convex hull polygon draw
            For Each edge As Edge In graph.graphEdges
                ' edge color by node category
                Dim catU = edge.U.data("group.category")
                Dim catV = edge.V.data("group.category")

                Dim u = convexHullCategoryStyle.TryGetValue(catU)
                Dim v = convexHullCategoryStyle.TryGetValue(catV)

                If Not u.StringEmpty AndAlso u = v Then
                    edge.data.style = New Pen(u.TranslateColor)
                ElseIf Not u.StringEmpty AndAlso Not v.StringEmpty Then
                    edge.data.style = New Pen({u.TranslateColor, v.TranslateColor}.Average)
                ElseIf Not u.StringEmpty Then
                    edge.data.style = New Pen(u.TranslateColor)
                ElseIf Not v.StringEmpty Then
                    edge.data.style = New Pen(v.TranslateColor)
                Else
                    edge.data.style = Pens.Black
                End If
            Next

            Me.convexHullCategoryStyle = convexHullCategoryStyle
        End Sub

        Public Overrides Function getFontSize(node As Node) As Single
            If node.label.IsPattern("C\d+") Then
                Return 36
            Else
                Return 24
            End If
        End Function

        Public Overrides Function drawNode(id As String, g As IGraphics, br As Brush, radius As Single(), center As PointF) As RectangleF
            Return getNodeLayout(id, radius, center)
        End Function

        Public Overrides Function getLabelColor(node As Node) As Color
            If node.label.IsPattern("R\d+") Then
                Return Color.Black
            Else
                Dim category = node.data("group.category")

                If (Not category.StringEmpty) AndAlso convexHullCategoryStyle.ContainsKey(category) Then
                    Return convexHullCategoryStyle(category).TranslateColor
                End If

                If Not node.data("group.category.color").StringEmpty Then
                    Return node.data("group.category.color").TranslateColor
                Else
                    Return Color.Black
                End If
            End If
        End Function

        Public Overrides Function getHullPolygonGroups() As NamedValue(Of String)
            Return Nothing
        End Function
    End Class
End Namespace
