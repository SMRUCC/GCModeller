#Region "Microsoft.VisualBasic::2295f27ea2a8855f03ac32e0d8192eb8, visualize\PDB_canvas\Canvas.vb"

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
    '     Properties: AutoRotate
    ' 
    '     Sub: __update, Canvas_Disposed, Canvas_MouseDown, Canvas_MouseMove, Canvas_MouseUp
    '          Canvas_MouseWheel, Canvas_Paint, LoadModel
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Imaging
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Data.RCSB.PDB

Public Class Canvas

    ''' <summary>
    ''' The graphics update driver
    ''' </summary>
    Dim __driver As New UpdateThread(45, AddressOf __update) With {
        .ErrHandle = AddressOf App.LogException
    }
    Dim _model As ChainModel
    Dim _viewDistance As Integer = -48

    ''' <summary>
    ''' Load a pdb model and visualize this object on the screen.
    ''' </summary>
    ''' <param name="path">*.pdb file path</param>
    Public Sub LoadModel(path As String)
        Dim pdb As PDB = PDB.Load(path)
        _model = New ChainModel(pdb)
        Call Me.Invalidate()

        AutoRotate = True
    End Sub

    Dim usrCursor As Point
    Dim _control As Boolean

    Private Sub Canvas_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        If _model Is Nothing Then
            Return
        Else
            usrCursor = e.Location
            _control = True
            Call __driver.Start()
        End If
    End Sub

    Dim rotateX As Double
    Dim rotateY As Double
    Dim _autoRotation As Boolean = False

    ''' <summary>
    ''' Model auto rotation?
    ''' </summary>
    ''' <returns></returns>
    Public Property AutoRotate As Boolean
        Get
            Return _autoRotation
        End Get
        Set(value As Boolean)
            _autoRotation = value
            Call __driver.Start()
        End Set
    End Property

    Dim currentPoint As Point

    Private Sub Canvas_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        currentPoint = e.Location

        If Not _control Then
            Return
        ElseIf AutoRotate Then
            Return
        End If

        If Math.Abs(usrCursor.X - e.X) > 20 Then
            rotateX += (-usrCursor.X + e.X) / 200
            Call _model.RotateX(rotateX)
        Else
            rotateY += (-usrCursor.Y + e.Y) / 200
            Call _model.RotateY(rotateY)
        End If
        '  
        ' 
        '  Call _model.Rotate(rotateX)
    End Sub

    Private Sub Canvas_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        _control = False

        If Not AutoRotate Then
            Call __driver.Stop()
        End If
    End Sub

    Private Sub __update()
        Call Invoke(Sub() Call Me.Invalidate())

        If AutoRotate Then
            rotateX += 1
            Call _model.Rotate(rotateX)
        End If
    End Sub

    Private Sub Canvas_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        If _model IsNot Nothing Then
            Call _model.UpdateGraph(e.Graphics, ClientSize, _viewDistance)
        End If

        Call e.Graphics.DrawString($"{currentPoint.GetJson}; rX={rotateX},rY={rotateY}", New Font(FontFace.MicrosoftYaHei, 12), Brushes.Red, New Point(5, 33))
    End Sub

    Private Sub Canvas_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
        _viewDistance += e.Delta / 200
        Call __update()
    End Sub

    Private Sub Canvas_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        Call __driver.Dispose()
    End Sub
End Class
