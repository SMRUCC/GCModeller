#Region "Microsoft.VisualBasic::181166da2bcfd97db61195351b90c758, models\Networks\KEGG\PathwayMaps\Styles\PlainStyle.vb"

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

'     Class PlainStyle
' 
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging

Namespace PathwayMaps.RenderStyles

    Public Class PlainStyle : Inherits RenderStyle

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
                Dim u = convexHullCategoryStyle(edge.U.data("group.category"))
                Dim v = convexHullCategoryStyle(edge.V.data("group.category"))

                If u = v Then
                    edge.data.color = New SolidBrush(u.TranslateColor)
                Else
                    edge.data.color = New SolidBrush({u.TranslateColor, v.TranslateColor}.Average)
                End If
            Next
        End Sub

        Public Overrides Function getFontSize(node As Node) As Single
            Return 32
        End Function

        Public Overrides Function drawNode(id As String, g As IGraphics, br As Brush, radius As Single, center As PointF) As RectangleF
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace
