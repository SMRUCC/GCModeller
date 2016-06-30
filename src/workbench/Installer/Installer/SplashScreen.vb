Public Class SplashScreen

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If Me.Opacity < 1 Then
            Me.Opacity += 0.03
        Else
            Timer1.Enabled = False
            Call _Close()
        End If
    End Sub

    Private Sub _Close()
        Threading.Thread.Sleep(5000)
        Call Me.Close()
    End Sub

    Private Sub SplashScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Opacity = 0
        CheckForIllegalCrossThreadCalls = False
    End Sub
End Class
