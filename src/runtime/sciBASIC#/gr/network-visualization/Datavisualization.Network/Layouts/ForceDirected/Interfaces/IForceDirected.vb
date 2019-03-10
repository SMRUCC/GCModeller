﻿#Region "Microsoft.VisualBasic::15ca8b146eacf0ec7aab16b515c5ab10, gr\network-visualization\Datavisualization.Network\Layouts\ForceDirected\Interfaces\IForceDirected.vb"

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

    '     Delegate Sub
    ' 
    ' 
    '     Delegate Sub
    ' 
    ' 
    '     Interface IForceDirected
    ' 
    '         Properties: Damping, graph, Repulsion, Stiffness, Threadshold
    '                     WithinThreashold
    ' 
    '         Function: GetBoundingBox, GetPoint, Nearest
    ' 
    '         Sub: Calculate, Clear, EachEdge, EachNode, SetPhysics
    ' 
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

'! 
'@file IForceDirected.cs
'@author Woong Gyu La a.k.a Chris. <juhgiyo@gmail.com>
'		<http://github.com/juhgiyo/epForceDirectedGraph.cs>
'@date August 08, 2013
'@brief ForceDirected Interface
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
'An Interface for the ForceDirected.
'
'

Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph

Namespace Layouts.Interfaces

    Public Delegate Sub EdgeAction(edge As Edge, spring As Spring)
    Public Delegate Sub NodeAction(edge As Node, point As LayoutPoint)

    Public Interface IForceDirected
        ReadOnly Property graph() As IGraph

        ReadOnly Property Stiffness() As Single

        ReadOnly Property Repulsion() As Single

        ReadOnly Property Damping() As Single

        Property Threadshold() As Single
        ' NOT Using
        ReadOnly Property WithinThreashold() As Boolean
        Sub Clear()

        ''' <summary>
        ''' Calculates the physics updates.
        ''' </summary>
        ''' <param name="iTimeStep"></param>
        Sub Calculate(iTimeStep As Single)
        Sub EachEdge(del As EdgeAction)
        Sub SetPhysics(Stiffness As Single, Repulsion As Single, Damping As Single)

        ''' <summary>
        ''' 节点的经过计算之后的当前位置可以从这个方法之中获取得到
        ''' </summary>
        ''' <param name="del"></param>
        Sub EachNode(del As NodeAction)
        Function Nearest(position As AbstractVector) As NearestPoint
        Function GetBoundingBox() As BoundingBox
        Function GetPoint(iNode As Node) As LayoutPoint
    End Interface
End Namespace
