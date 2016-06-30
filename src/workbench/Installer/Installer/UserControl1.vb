Public Class UserControl1

    Public Event [Next]()
    Public Event Cancel()

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        RaiseEvent Next()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If MsgBox("Cancel the software installation?", MsgBoxStyle.Question + MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            RaiseEvent Cancel()
        End If
    End Sub
End Class
