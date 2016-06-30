Friend Class FormSplash

    'Private Const CS_DROPSHADOW = &H20000
    'Private Const GCL_STYLE = (-26)

    'Private Declare Function GetClassLong Lib "user32" Alias "GetClassLongA" ( hwnd As Integer,  nIndex As Integer) As Integer
    'Private Declare Function SetClassLong Lib "user32" Alias "SetClassLongA" ( hwnd As Integer,  nIndex As Integer,  dwNewLong As Integer) As Integer

    Private Sub SplashScreen_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'SetClassLong(Handle, GCL_STYLE, GetClassLong(Handle, GCL_STYLE) Or CS_DROPSHADOW)

        CheckForIllegalCrossThreadCalls = False
        'Label3.Text = String.Format("{0} {1}", Label3.Text, My.Application.Info.Version.ToString)
        PictureBox1.Size = PictureBox1.BackgroundImage.Size
        Me.Size = PictureBox1.Size
        Version.Text = String.Format("Version: {0}", My.Application.Info.Version.ToString)
    End Sub
End Class