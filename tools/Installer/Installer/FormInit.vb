Public Class FormInit

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Close()
    End Sub

    Private Sub CommandLink1_Click(sender As Object, e As EventArgs) Handles CommandLink1.Click
        Me.Visible = False

        If New FormLicense().ShowDialog = DialogResult.OK Then
        Else
            Close()
        End If
    End Sub
End Class
