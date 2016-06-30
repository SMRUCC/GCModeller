Imports System.Drawing

Public Class LabelButton

    Public Shadows Event Click()

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        RaiseEvent Click()
    End Sub

    Public Overrides Property Text As String
        Get
            Return MyBase.Text
        End Get
        Set(value As String)
            MyBase.Text = value
            '只输入ASCII字符
            BackgroundImage = DrawText(value)
            Size = BackgroundImage.Size
            Button1.Location = New Point With {.X = 0, .Y = 20}
        End Set
    End Property

    Function DrawText(Text As String) As Image
        Dim size As Size = New Size With {.Width = Len(Text) * 7.5, .Height = 25}
        Dim bmp As New Bitmap(size.Width, size.Height)
        Dim gr As Graphics = Graphics.FromImage(bmp)

        gr.DrawString(Text, New Font("Microsoft YaHei", 8), Brushes.White, New Point With {.X = 0, .Y = 1})

        Return bmp
    End Function

    Private Sub LabelButton_Load(sender As Object, e As EventArgs) Handles Me.Load
        Button1.UI = New MolkPlusTheme.Visualise.Elements.ButtonResource With {
            .Normal = My.Resources.LabelButtonNormal,
            .PreLight = My.Resources.LabelButtonPrelight,
            .Active = My.Resources.LabelButtonPrelight}
    End Sub
End Class
