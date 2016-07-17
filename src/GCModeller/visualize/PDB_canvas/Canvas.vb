Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports SMRUCC.genomics.Data.RCSB.PDB

Public Class Canvas

    Dim __driver As New UpdateThread(30, AddressOf __update)
    Dim _model As ChainModel
    Dim _viewDistance As Integer = 10

    Public Sub LoadModel(path As String)
        Dim pdb As PDB = PDB.Load(path)
        _model = New ChainModel(pdb)
        Call Me.Invalidate()
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

    Dim rotate As Double

    Private Sub Canvas_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If Not _control Then
            Return
        End If

        rotate += (-usrCursor.X + e.X) / 1000
        Call _model.Rotate(rotate)
    End Sub

    Private Sub Canvas_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        _control = False
        Call __driver.Stop()
    End Sub

    Private Sub __update()
        Call Invoke(Sub() Call Me.Invalidate())
    End Sub

    Private Sub Canvas_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        If _model IsNot Nothing Then
            Call _model.UpdateGraph(e.Graphics, ClientSize, _viewDistance)
        End If
    End Sub
End Class
