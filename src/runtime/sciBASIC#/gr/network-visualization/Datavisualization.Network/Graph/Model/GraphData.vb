﻿#Region "Microsoft.VisualBasic::6bfcdac8df9dad77e5a50368885085a8, gr\network-visualization\Datavisualization.Network\Graph\Model\GraphData.vb"

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

    '     Class NodeData
    ' 
    '         Properties: Color, Force, initialPostion, mass, Neighborhoods
    '                     Neighbours, origID, radius, Weights
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Clone, ToString
    ' 
    '     Class EdgeData
    ' 
    '         Properties: length, weight
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Clone, ToString
    ' 
    '     Class GraphData
    ' 
    '         Properties: label
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

'! 
'@file PhysicsData.cs
'@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
'		<http://github.com/juhgiyo/epForceDirectedGraph.cs>
'@date August 08, 2013
'@brief PhysicsData Interface
'@version 1.0
'
'@section LICENSE
'
'The MIT License (MIT)
'
'Copyright (c) 2013 Woong Gyu La <juhgiyo@gmail.com>
'
'Permission is hereby granted, free of charge, to any person obtaining a copy
'of this software and associated documentation files (the "Software"), to deal
'in the Software without restriction, including without limitation the rights
'to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
'copies of the Software, and to permit persons to whom the Software is
'furnished to do so, subject to the following conditions:
'
'The above copyright notice and this permission notice shall be included in
'all copies or substantial portions of the Software.
'
'THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
'IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
'FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
'AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
'LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
'OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
'THE SOFTWARE.
'
'@section DESCRIPTION
'
'An Interface for the PhysicsData Class.
'
'

Imports System.Drawing
Imports System.Web.Script.Serialization
Imports Microsoft.VisualBasic.Data.visualize.Network.FileStream.Generic
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Serialization
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Graph

    Public Class NodeData : Inherits GraphData

        Public Sub New()
            MyBase.New()
            mass = 1.0F
            initialPostion = Nothing
            ' for merging the graph
            origID = ""
        End Sub

        Public ReadOnly Property Neighborhoods As Integer
            Get
                If Neighbours Is Nothing Then
                    Return 0
                Else
                    Return Neighbours.Length
                End If
            End Get
        End Property

        Public Property radius As Single
        Public Property mass() As Single
        Public Property initialPostion() As AbstractVector
        Public Property origID() As String
        Public Property Force As Point

        ''' <summary>
        ''' 颜色<see cref="SolidBrush"/>或者绘图<see cref="TextureBrush"/>
        ''' </summary>
        ''' <returns></returns>
        <ScriptIgnore>
        Public Property Color As Brush
        <DumpNode> Public Property Weights As Double()

        ''' <summary>
        ''' 与本节点相连接的其他节点的<see cref="Node.Label">编号</see>
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DumpNode> Public Property Neighbours As Integer()

        Public Function Clone() As NodeData
            Return DirectCast(Me.MemberwiseClone, NodeData)
        End Function

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

    Public Class EdgeData : Inherits GraphData

        Public Sub New()
            MyBase.New()
            length = 1.0F
        End Sub

        Public Property length() As Single
        Public Property weight As Double

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function

        Public Function Clone() As EdgeData
            Return DirectCast(Me.MemberwiseClone, EdgeData)
        End Function
    End Class

    Public Class GraphData : Inherits IDynamicsTable

        Public Sub New()
            label = ""
        End Sub

        ''' <summary>
        ''' The graph object display label.
        ''' (这个属性为显示的标题，与ID不一样，这个属性可能会出现重复值，所以不可以用这个标签来作为字典主键)
        ''' </summary>
        ''' <returns></returns>
        Public Property label() As String

        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class
End Namespace
