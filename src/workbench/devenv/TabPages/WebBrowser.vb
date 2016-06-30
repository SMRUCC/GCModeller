Namespace TabPages

    Public Class WebBrowser : Inherits Microsoft.VisualBasic.MolkPlusTheme.Windows.Forms.Controls.WebBrowser

        Public Property MyHomePage As String

        Protected Overrides Function get_HomePage() As String
            Return MyHomePage
        End Function

        Private Sub WebBrowser_Load(sender As Object, e As EventArgs) Handles Me.Load
            Call Me.WebBrowser1.Navigate(MyHomePage)
        End Sub
    End Class
End Namespace