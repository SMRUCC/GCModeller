Public Class FormImagePlotDevice

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        MsgBox(My.Application.Info.Version.ToString, MsgBoxStyle.Information)
    End Sub

    Private Sub CloseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CloseToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub SaveAsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveAsToolStripMenuItem.Click
        Using sf = New SaveFileDialog With {.FileName = "ImageResource.png", .DefaultExt = "*.png"}
            sf.Filter = "Bitmap(*.bmp)|*.bmp|Jpeg Image(*.jpg, *.jpeg)|*.jpg*.jpeg|Portable Network Graphic(*.png)|*.png"

            If sf.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                Call PictureBox1.BackgroundImage.Save(sf.FileName)
            End If
        End Using
    End Sub
End Class
