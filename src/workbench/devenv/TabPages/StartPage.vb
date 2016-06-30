Namespace TabPages

    Friend Class StartPage

        Private Sub StartPage_Load(sender As Object, e As EventArgs) Handles Me.Load
            WebBrowser1.Location = New Drawing.Point With {.X = 340, .Y = 0}
            WebBrowser1.Navigate(My.Application.Info.DirectoryPath & "/html/dev_startpage/index.html")
        End Sub

        Private Sub StartPage_Resize(sender As Object, e As EventArgs) Handles Me.Resize
            '  LineShape3.StartPoint = New Drawing.Point With {.X = 40, .Y = Height - 100}
            '   LineShape3.EndPoint = New Drawing.Point With {.X = 300, .Y = Height - 100}

            KeepsCheckbox.Location = New Drawing.Point With {.X = 45, .Y = Height - 70}
            ShowCheckbox.Location = New Drawing.Point With {.X = 45, .Y = Height - 45}

            WebBrowser1.Size = New Drawing.Size With {.Width = Width - 340, .Height = Height}
        End Sub
    End Class
End Namespace