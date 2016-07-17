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
    End Sub

    Private Sub Canvas_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Dim _control As Boolean

    Private Sub Canvas_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        If _model Is Nothing Then
            Return
        Else
            _control = True
            Call __driver.Start()
        End If
    End Sub

    Private Sub Canvas_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If Not _control Then
            Return
        End If
    End Sub

    Private Sub Canvas_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        _control = False
        Call __driver.Stop()
    End Sub

    Private Sub __update()
        Call Me.Invalidate()
    End Sub

    Private Sub Canvas_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Call _model.UpdateGraph(e.Graphics, ClientSize, _viewDistance)
    End Sub
End Class
