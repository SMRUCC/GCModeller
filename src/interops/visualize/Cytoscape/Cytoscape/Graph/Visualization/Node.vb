#Region "Microsoft.VisualBasic::a8874b5fd7c3a3d8f15c142292f04cf8, visualize\Cytoscape\Cytoscape\Graph\Visualization\Node.vb"

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

'     Class Node
' 
'         Properties: Location, NodeModel, Radius, Rectangle
' 
'         Constructor: (+2 Overloads) Sub New
'         Function: getCosAlpha, getSinAlpha, OffSet, Point_getInterface, ToString
' 
' 
' /********************************************************************************/

#End Region

Imports System.Drawing
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.XGMML.File

Namespace CytoscapeGraphView

    ''' <summary>
    ''' 一般是绘制圆形
    ''' </summary>
    Public Class Node

        ''' <summary>
        ''' 半径
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Radius As Integer
        Public ReadOnly Property Location As Point

        Private Sub New(r As Integer, Location As Point)
            Me.Radius = r
            Me.Location = Location
            Rectangle = New Rectangle(New Point(Location.X - r, Location.Y - r), New Size(r * 2, r * 2))
            Me.ir = r * 1.1
        End Sub

        Sub New(Node As XGMMLnode, xScale As Double, yScale As Double)
            Call Me.New(((xScale + yScale) / 3) * (Node.graphics.w + Node.graphics.h) / 2,
                        New Point(Node.graphics.x * xScale, Node.graphics.y * yScale))
            Me.NodeModel = Node
        End Sub

        Public ReadOnly Property NodeModel As XGMMLnode

        ''' <summary>
        ''' 在画图的时候的圆形的正方形的绘图区域
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Rectangle As Rectangle

        Public Function OffSet(value As Point) As Node
            Me._Location = New Point(Location.X + value.X, Location.Y + value.Y)
            Me._Rectangle = New Rectangle(New Point(Location.X - Radius, Location.Y - Radius), Rectangle.Size)
            Return Me
        End Function

        Public Overrides Function ToString() As String
            Return $"{Location.ToString} // {NameOf(Radius)}:={Radius}"
        End Function

        ''' <summary>
        ''' 计算当前节点和另外一个节点的夹角的sin(alpha)
        ''' </summary>
        ''' <param name="Node"></param>
        ''' <returns></returns>
        Public Function getSinAlpha(Node As Node) As Double
            Return (Location.Y - Node.Location.Y) / (Math.Sqrt((Location.X - Node.Location.X) ^ 2 + (Location.Y - Node.Location.Y) ^ 2))
        End Function

        Public Function getCosAlpha(Node As Node) As Double
            Return (Node.Location.X - Location.X) / (Math.Sqrt((Location.X - Node.Location.X) ^ 2 + (Location.Y - Node.Location.Y) ^ 2))
        End Function

        ''' <summary>
        ''' Interface .Radius
        ''' </summary>
        Protected ReadOnly ir As Double

        Public Function Point_getInterface(Node As Node) As Point
            Dim we = ir * getCosAlpha(Node)
            Dim he = ir * getSinAlpha(Node)
            Dim e = New Point(Location.X + we, Location.Y - he)
            Return e
        End Function
    End Class
End Namespace
