Public Class InstallationConf

    Private Sub InstallationConf_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Using SplashScreen As SplashScreen = New SplashScreen
            Call SplashScreen.ShowDialog()
        End Using
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using Dir As FolderBrowserDialog = New FolderBrowserDialog
            Dir.ShowNewFolderButton = True

            If Dir.ShowDialog = Windows.Forms.DialogResult.OK Then
                TextBox1.Text = Dir.SelectedPath
            End If
        End Using
    End Sub

    Private Sub UserControl11_Cancel() Handles UserControl11.Cancel
        Call Me.Close()
    End Sub

    Private Sub UserControl11_Next() Handles UserControl11.Next
        Call (New ExtractPackage).Show()
        Call Me.Close()
    End Sub
End Class