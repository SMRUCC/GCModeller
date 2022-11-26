#Region "Microsoft.VisualBasic::89f63e6405f47e6b468f3b8b5f243a40, GCModeller\models\Networks\KEGG\PathwayMaps\Styles\RenderStyle.vb"

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

    '   Total Lines: 97
    '    Code Lines: 80
    ' Comment Lines: 2
    '   Blank Lines: 15
    '     File Size: 4.16 KB


    '     Class RenderStyle
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: createNodeTable, getNodeLayout
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Drawing.Drawing2D
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Data.ChartPlots.Graphic
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Imaging.Drawing2D.Colors
Imports SMRUCC.genomics.Assembly.KEGG.DBGET.BriteHEntry

Namespace PathwayMaps.RenderStyles

    Public MustInherit Class RenderStyle

        Public MustOverride ReadOnly Property edgeDashType As DashStyle

        Protected Friend ReadOnly nodes As Dictionary(Of String, Node)
        Protected ReadOnly graph As NetworkGraph

        Sub New(graph As NetworkGraph, enzymeColorSchema$, compoundColorSchema$)
            Me.nodes = createNodeTable(graph, enzymeColorSchema$, compoundColorSchema$)
            Me.graph = graph
        End Sub

        Private Shared Function createNodeTable(graph As NetworkGraph, enzymeColorSchema$, compoundColorSchema$) As Dictionary(Of String, Node)
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

            Return nodes
        End Function

        Public MustOverride Function getFontSize(node As Node) As Single
        Public MustOverride Function drawNode(id$, g As IGraphics, br As Brush, radius!, center As PointF) As RectangleF
        Public MustOverride Function getLabelColor(node As Node) As Color
        Public MustOverride Function getHullPolygonGroups() As NamedValue(Of String)

        Protected Function getNodeLayout(id As String, radius As Single, center As PointF) As Rectangle
            Dim node As Node = nodes(id)
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
            Else
                ' 方形
                rect = New Rectangle With {
                    .X = center.X - radius / 2,
                    .Y = center.Y - radius / 5,
                    .Width = radius,
                    .Height = radius / 2.5
                }
            End If

            Return rect
        End Function

    End Class
End Namespace
