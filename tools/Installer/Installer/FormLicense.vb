Public Class FormLicense

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Button1.Enabled = CheckBox1.Checked
    End Sub

    Private Sub FormLicense_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckBox1.Checked = False
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Close()
    End Sub
End Class