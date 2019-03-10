﻿#Region "Microsoft.VisualBasic::1a3f6fb328b6ff3e360418f6dba7a8b5, gr\network-visualization\NetworkCanvas\Canvas.vb"

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

    ' Class Canvas
    ' 
    '     Properties: AutoRotate, DynamicsRadius, FdgArgs, Graph, ShowLabel
    '                 ViewDistance
    ' 
    '     Sub: [Stop], __invokePaint, __invokeSet, __physicsUpdates, Canvas_Disposed
    '          Canvas_Load, Canvas_Paint, Run, SetFDGParams, SetRotate
    '          WriteLayout
    ' 
    ' /********************************************************************************/

#End Region

Imports System.ComponentModel
Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts.Interfaces
Imports Microsoft.VisualBasic.Parallel.Tasks

''' <summary>
''' Controls for view the network model.
''' </summary>
Public Class Canvas

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="space3D">Is 3D network viewer canvas</param>
    ''' <returns></returns>
    Public Property Graph(Optional space3D As Boolean = False) As NetworkGraph
        Get
            If net Is Nothing Then
                Call __invokeSet(New NetworkGraph, Me.space3D)
            End If

            Return net
        End Get
        Set(value As NetworkGraph)
            Me.space3D = space3D

            __invokeSet(value, Me.space3D)
#If DEBUG Then
            ' Call value.GetJson(True).__DEBUG_ECHO
#End If
        End Set
    End Property

    ''' <summary>
    ''' Render and layout engine works in 3D mode?
    ''' </summary>
    Dim space3D As Boolean

    Private Sub __invokeSet(g As NetworkGraph, space As Boolean)
        Dim showLabel As Boolean = Me.ShowLabel

        net = g

        If Not inputs Is Nothing Then
            Call inputs.Dispose()
            GC.SuppressFinalize(inputs)
            inputs = Nothing
        End If

        If space Then
            fdgPhysics = New ForceDirected3D(net, FdgArgs.Stiffness, FdgArgs.Repulsion, FdgArgs.Damping)
            fdgRenderer = New Renderer3D(
                Function() paper,
                Function() New Rectangle(New Point, Size),
                fdgPhysics, DynamicsRadius)
            inputs = New Input3D(Me)
        Else
            fdgPhysics = New ForceDirected2D(net, FdgArgs.Stiffness, FdgArgs.Repulsion, FdgArgs.Damping)
            fdgRenderer = New Renderer(
                Function() paper,
                Function() New Rectangle(New Point, Size),
                fdgPhysics)
            inputs = New InputDevice(Me)
        End If

        Me.ShowLabel = showLabel
    End Sub

    Public ReadOnly Property FdgArgs As ForceDirectedArgs = Parameters.Load

    Public Sub SetRotate(x As Double)
        If Not space3D Then
        Else
            DirectCast(fdgRenderer, Renderer3D).rotate = x
        End If
    End Sub

    Public Sub SetFDGParams(value As ForceDirectedArgs)
        FdgArgs.Damping = value.Damping
        FdgArgs.Repulsion = value.Repulsion
        FdgArgs.Stiffness = value.Stiffness

        Call fdgPhysics.SetPhysics(
            value.Stiffness,
            value.Repulsion,
            value.Damping)
    End Sub

    ''' <summary>
    ''' The network data model for the visualization 
    ''' </summary>
    Dim net As NetworkGraph
    ''' <summary>
    ''' Layout provider engine
    ''' </summary>
    Protected Friend fdgPhysics As IForceDirected
    ''' <summary>
    ''' The graphics updates thread.
    ''' </summary>
    Protected Friend timer As New UpdateThread(30, AddressOf __invokePaint)
    Protected Friend physicsEngine As New UpdateThread(30, AddressOf __physicsUpdates)
    ''' <summary>
    ''' The graphics rendering provider
    ''' </summary>
    Protected Friend fdgRenderer As Renderer
    ''' <summary>
    ''' GDI+ interface for the canvas control.
    ''' </summary>
    Dim paper As Graphics

    Public Property AutoRotate As Boolean = True
    Public Property DynamicsRadius As Boolean = False

    Public Property ViewDistance As Double
        Get
            If Not space3D Then
                Return 0
            Else
                Return DirectCast(fdgRenderer, Renderer3D).ViewDistance
            End If
        End Get
        Set(value As Double)
            If space3D Then
                DirectCast(fdgRenderer, Renderer3D).ViewDistance = value
            End If
        End Set
    End Property

    <DefaultValue(True)>
    Public Property ShowLabel As Boolean
        Get
            If fdgRenderer Is Nothing Then
                Return False
            End If
            Return DirectCast(fdgRenderer, IGraphicsEngine).ShowLabels
        End Get
        Set(value As Boolean)
            DirectCast(fdgRenderer, IGraphicsEngine).ShowLabels = value
        End Set
    End Property

    Private Sub __invokePaint()
        On Error Resume Next

        Call Me.Invoke(Sub() Call Invalidate())

        If _AutoRotate Then
            Static r As Double = -100.0R
            r += 0.4
            Call SetRotate(r)
        End If
    End Sub

    Private Sub __physicsUpdates()
        SyncLock fdgRenderer
            If Not fdgRenderer Is Nothing Then
                Call fdgRenderer.PhysicsEngine.Calculate(0.05F)
            End If
        End SyncLock
    End Sub

    Private Sub Canvas_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        paper = e.Graphics
        paper.CompositingQuality = Drawing2D.CompositingQuality.HighQuality
        paper.SmoothingMode = Drawing2D.SmoothingMode.HighQuality

        Call fdgRenderer.Draw(0.05F, physicsUpdate:=False)
    End Sub

    Dim inputs As InputDevice

    Private Sub Canvas_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Graph Is Nothing Then  ' 假若在load之间已经加载graph数据则在这里会清除掉先前的数据，所以需要判断一下
            Graph = New NetworkGraph
        End If

        timer.ErrHandle = AddressOf App.LogException
        timer.Start()
        physicsEngine.ErrHandle = AddressOf App.LogException
        physicsEngine.Start()
    End Sub

    Public Sub [Stop]()
        Call timer.Stop()
        Call physicsEngine.Stop()
    End Sub

    Public Sub Run()
        Call timer.Start()
        Call physicsEngine.Start()
    End Sub

    ''' <summary>
    ''' Write the node layout position into its extensions data, for generates the svg graphics.
    ''' </summary>
    Public Sub WriteLayout()
        Call Graph.WriteLayouts(fdgPhysics)
    End Sub

    Private Sub Canvas_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        timer.Dispose()
        physicsEngine.Dispose()
    End Sub
End Class
