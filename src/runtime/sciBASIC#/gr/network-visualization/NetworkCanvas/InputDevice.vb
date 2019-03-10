﻿#Region "Microsoft.VisualBasic::28f82a53403b422ae84815b0547e60a3, gr\network-visualization\NetworkCanvas\InputDevice.vb"

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

    ' Class InputDevice
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: __getNode
    ' 
    '     Sub: Canvas_MouseDown, Canvas_MouseMove, Canvas_MouseUp, Canvas_MouseWheel, (+2 Overloads) Dispose
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.visualize.Network.Graph
Imports Microsoft.VisualBasic.Data.visualize.Network.Layouts

Public Class InputDevice : Implements IDisposable

    Protected WithEvents Canvas As Canvas

    Sub New(canvas As Canvas)
        Me.Canvas = canvas
    End Sub

    Protected Overridable Sub Canvas_MouseMove(sender As Object, e As MouseEventArgs) Handles Canvas.MouseMove
        If Not drag Then   ' 设置tooltip
            Return
        End If

        If dragNode IsNot Nothing Then
            Dim vec As FDGVector2 =
                    Canvas.fdgRenderer.ScreenToGraph(
                    New Point(e.Location.X, e.Location.Y))

            dragNode.Pinned = True
            Canvas.fdgPhysics.GetPoint(dragNode).position = vec
        Else
            dragNode = __getNode(e.Location)
        End If
    End Sub

    Protected dragNode As Node

    Protected Overridable Function __getNode(p As Point) As Node
        For Each node As Node In Canvas.Graph.nodes
            Dim r As Single = node.Data.radius
            Dim v As FDGVector2 = TryCast(Canvas.fdgPhysics.GetPoint(node).position, FDGVector2)
            Dim npt As Point =
                Renderer.GraphToScreen(v, Canvas.fdgRenderer.ClientRegion)
            Dim pt As New Point(CInt(npt.X - r / 2), CInt(npt.Y - r / 2))
            Dim rect As New Rectangle(pt, New Size(CInt(r), CInt(r)))

            If rect.Contains(p) Then
                Return node
            End If
        Next

        Return Nothing
    End Function

    Protected drag As Boolean

    Protected Overridable Sub Canvas_MouseDown(sender As Object, e As MouseEventArgs) Handles Canvas.MouseDown
        drag = True
        dragNode = __getNode(e.Location)
    End Sub

    Protected Overridable Sub Canvas_MouseUp(sender As Object, e As MouseEventArgs) Handles Canvas.MouseUp
        drag = False
        If dragNode IsNot Nothing Then
            dragNode.Pinned = False
            dragNode = Nothing
        End If
    End Sub

    Protected Overridable Sub Canvas_MouseWheel(sender As Object, e As MouseEventArgs) Handles Canvas.MouseWheel

    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                ' TODO: dispose managed state (managed objects).
                Canvas = Nothing
            End If

            ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
            ' TODO: set large fields to null.
        End If
        disposedValue = True
    End Sub

    ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
    'Protected Overrides Sub Finalize()
    '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' This code added by Visual Basic to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        Dispose(True)
        ' TODO: uncomment the following line if Finalize() is overridden above.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
