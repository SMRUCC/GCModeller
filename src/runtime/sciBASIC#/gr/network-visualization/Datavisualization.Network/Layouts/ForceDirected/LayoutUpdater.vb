﻿#Region "Microsoft.VisualBasic::e52f3611a0b15cccbe78b1f14113789e, gr\network-visualization\Datavisualization.Network\Layouts\ForceDirected\LayoutUpdater.vb"

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

    '     Module LayoutUpdater
    ' 
    '         Sub: Updates
    '         Class __layoutGenerator
    ' 
    '             Constructor: (+1 Overloads) Sub New
    '             Sub: Clear, drawEdge, drawNode
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.Interfaces

Namespace Layouts

    Public Module LayoutUpdater

        Private Class __layoutGenerator : Inherits AbstractRenderer

            Public Sub New(iForceDirected As IForceDirected)
                MyBase.New(iForceDirected)
            End Sub

            Public Overrides Sub Clear()

            End Sub

            Protected Overrides Sub drawEdge(iEdge As Edge, iPosition1 As AbstractVector, iPosition2 As AbstractVector)
            End Sub

            ''' <summary>
            ''' 在这里更新节点的位置
            ''' </summary>
            ''' <param name="iNode"></param>
            ''' <param name="iPosition"></param>
            Protected Overrides Sub drawNode(iNode As Node, iPosition As AbstractVector)
                iNode.Data.initialPostion = iPosition
            End Sub
        End Class

        <Extension>
        Public Sub Updates(Of T As AbstractVector)(engine As ForceDirected(Of T), ByRef net As NetworkGraph, Optional loops As Integer = 100)
            Dim updater As New __layoutGenerator(engine)

            For i As Integer = 0 To loops - 1
                Call updater.Draw(0.05F)
            Next
        End Sub
    End Module
End Namespace
