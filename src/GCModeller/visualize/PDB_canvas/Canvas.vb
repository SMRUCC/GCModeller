Imports System.Drawing
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.Parallel.Tasks
Imports SMRUCC.genomics.Data.RCSB.PDB

Public Class Canvas

    Dim __driver As New UpdateThread(45, AddressOf __update)
    Dim _model As ChainModel
    Dim _viewDistance As Integer = -40

    Public Sub LoadModel(path As String)
        Dim pdb As PDB = PDB.Load(path)
        _model = New ChainModel(pdb)
        Call Me.Invalidate()

        '    AutoRotate = True
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

    Public Property AutoRotate As Boolean
        Get
            Return _autoRotation
        End Get
        Set(value As Boolean)
            _autoRotation = value
            Call __driver.Start()
        End Set
    End Property

    Private Sub Canvas_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If Not _control Then
            Return
        ElseIf AutoRotate Then
            Return
        End If

        rotateX += (-usrCursor.X + e.X) / 10000
        rotateY += (-usrCursor.Y + e.Y) / 10000
        '   Call _model.RotateX(rotateX)
        ' Call _model.RotateY(rotateY)
        Call _model.Rotate(rotateX)
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
            rotateX += 0.25
            Call _model.Rotate(rotateX)
        End If
    End Sub

    Private Sub Canvas_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        If _model IsNot Nothing Then
            Call _model.UpdateGraph(e.Graphics, ClientSize, _viewDistance)
        End If
    End Sub

    Private Sub Canvas_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Canvas_MouseWheel(sender As Object, e As MouseEventArgs) Handles Me.MouseWheel
        _viewDistance += e.Delta / 300
        Call __update()
    End Sub
End Class
