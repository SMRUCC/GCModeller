Namespace TabPages

    Public Class Options

        Private Sub Options_Load(sender As Object, e As EventArgs) Handles MyBase.Load
            ComboLanguage.SelectedIndex = Program.Dev2Profile.IDE.Language
            Label4.Text = String.Format("Configuration File: {0}", Settings.File.DefaultXmlFile)
        End Sub

        Private Sub ComboLanguage_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboLanguage.SelectedIndexChanged
            Program.Dev2Profile.IDE.Language = ComboLanguage.SelectedIndex
        End Sub

        Private Sub Options_Resize(sender As Object, e As EventArgs) Handles Me.Resize

        End Sub
    End Class
End Namespace