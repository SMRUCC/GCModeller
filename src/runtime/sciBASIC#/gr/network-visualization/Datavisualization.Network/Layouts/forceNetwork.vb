﻿#Region "Microsoft.VisualBasic::8bf7d1500902e14a80ffc40ccd29e9fb, gr\network-visualization\Datavisualization.Network\Layouts\forceNetwork.vb"

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

    '     Module forceNetwork
    ' 
    '         Sub: (+2 Overloads) doForceLayout, doRandomLayout
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Terminal
Imports Microsoft.VisualBasic.Terminal.ProgressBar

Namespace Layouts

    Public Module forceNetwork

        ''' <summary>
        ''' <see cref="Parameters.Load"/>
        ''' </summary>
        ''' <param name="net"></param>
        ''' <param name="parameters"></param>
        ''' <param name="showProgress"></param>
        <ExportAPI("Layout.ForceDirected")>
        <Extension>
        Public Sub doForceLayout(ByRef net As NetworkGraph, parameters As ForceDirectedArgs, Optional showProgress As Boolean = False)
            With parameters
                Call net.doForceLayout(.Stiffness, .Repulsion, .Damping, .Iterations, showProgress:=showProgress)
            End With
        End Sub

        ''' <summary>
        ''' Applies the force directed layout. Parameter can be read from a ``*.ini`` profile file by using <see cref="Parameters.Load"/>
        ''' (如果有些时候这个函数不起作用的话，考虑一下在调用这个函数之前，先使用<see cref="doRandomLayout"/>初始化随机位置)
        ''' </summary>
        ''' <param name="net"></param>
        ''' <param name="iterations">网络的布局layout的计算迭代次数</param>
        ''' <param name="Stiffness">
        ''' 密度：影响网络的节点的距离，这个参数值越小，则网络节点的相互之间的距离越大，即这个密度参数值越小，则单位面积内的节点数量越少
        ''' </param>
        ''' <param name="Damping">
        ''' 阻尼：这个参数值越小，则在计算layout的时候，某一个节点所能够影响到的节点数量也越少。即某一个节点的位置调整之后，被影响的其他节点的数量也越少。
        ''' 这个参数值介于0-1之间，超过1的时候网络永远也不会处于稳定的状态
        ''' </param>
        ''' <param name="Repulsion">
        ''' 排斥力：节点之间的排斥力的大小，当这个参数值越大的时候，节点之间的排斥力也会越大，则节点之间的距离也越远。反之节点之间的距离也越近
        ''' </param>
        <ExportAPI("Layout.ForceDirected")>
        <Extension>
        Public Sub doForceLayout(ByRef net As NetworkGraph,
                                 Optional Stiffness# = 80,
                                 Optional Repulsion# = 4000,
                                 Optional Damping# = 0.83,
                                 Optional iterations% = 1000,
                                 Optional showProgress As Boolean = False)

            Dim physicsEngine As New ForceDirected2D(net, Stiffness, Repulsion, Damping)
            Dim tick As Action(Of Integer)
            Dim progress As ProgressBar = Nothing

            If showProgress Then
                Dim ticking As New ProgressProvider(iterations)
                Dim ETA$
                Dim details$
                Dim args$ = New ForceDirectedArgs With {
                    .Damping = Damping,
                    .Iterations = iterations,
                    .Repulsion = Repulsion,
                    .Stiffness = Stiffness
                }.GetJson

                progress = New ProgressBar("Do Force Directed Layout...", 1, CLS:=showProgress)
                tick = Sub(i%)
                           ETA = "ETA=" & ticking _
                               .ETA(progress.ElapsedMilliseconds) _
                               .FormatTime
                           details = args & $" ({i}/{iterations}) " & ETA
                           progress.SetProgress(ticking.StepProgress, details)
                       End Sub
            Else
                tick = Sub(i%)
                       End Sub
            End If

            For i As Integer = 0 To iterations
                Call physicsEngine.Calculate(0.05F)
                Call tick(i)
            Next

            Call physicsEngine.EachNode(
                Sub(node, point)
                    node.Data.initialPostion = point.position
                End Sub)

            If Not progress Is Nothing Then
                Call progress.Dispose()
            End If
        End Sub

        <Extension>
        Public Sub doRandomLayout(ByRef net As NetworkGraph)
            Dim rnd As New Random

            For Each x As Node In net.nodes
                x.Data.initialPostion = New FDGVector2 With {
                    .x = rnd.NextDouble * 1000,
                    .y = rnd.NextDouble * 1000
                }
            Next
        End Sub
    End Module
End Namespace
