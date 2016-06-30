Imports System.Drawing

Public Class OutputConsole
    Dim _popout As Boolean = False

    Public Sub PopOut()
        If _popout Then
            Visible = False
            _popout = False
        Else
            Me.Visible = True
            Me.BringToFront()
            _popout = True
        End If
    End Sub

    Public Sub Close()
        Visible = False
        _popout = False
    End Sub

    Private Sub PopoutTAB_Load(sender As Object, e As EventArgs) Handles Me.Load
        TextBox1.Location = New Drawing.Point With {.X = 2, .Y = 20}
        Label1.Location = New Drawing.Point With {.X = 2, .Y = 2}

        Call Me.Close()
    End Sub

    Private Sub PopoutTAB_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        TextBox1.Size = New Drawing.Point With {.X = Width - 4, .Y = Height - 21}
        Label1.Width = Width - 4
    End Sub

    Public Sub Write(data As String)
        TextBox1.AppendText("  " & data & vbCrLf)
    End Sub

    Dim Sizing As Boolean = False
    Dim UserCursor As Point

    Private Sub Label1_MouseDown(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Label1.MouseDown
        Sizing = True
        UserCursor = e.Location
    End Sub

    Private Sub Label1_MouseMove(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Label1.MouseMove
        If Sizing Then
            Dim pLocation = New Point With {
                  .X = Me.Location.X,
                  .Y = Me.Location.Y - UserCursor.Y + e.Y}

            Me.Height = Me.Height + (UserCursor.Y - e.Y)
            Me.Location = pLocation
        End If
    End Sub

    Private Sub Label1_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles Label1.MouseUp
        Sizing = False
    End Sub
End Class
