Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Parallel.Tasks

Public Class Canvas

    Dim __driver As New UpdateThread(30, AddressOf __update)
    Dim _model As ChainModel
    Dim _viewDistance As Integer = 10

    Private Sub Canvas_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Canvas_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown

    End Sub

    Private Sub Canvas_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove

    End Sub

    Private Sub Canvas_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp

    End Sub

    Private Sub __update()
        Call Me.Invalidate()
    End Sub

    Private Sub Canvas_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Call _model.UpdateGraph(e.Graphics, ClientSize, _viewDistance)
    End Sub
End Class
