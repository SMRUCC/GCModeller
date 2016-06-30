Friend Class NCBIViewer

    Public Const VIEWER As String = "NCBI_Viewer"

    Dim c2Profile As Settings.File = Settings.Session.Initialize() '  LANS.SystemsBiology.NCBI.BLAST.Extensions.Profile = LANS.SystemsBiology.NCBI.BLAST.Extensions.Profile.XmlFile
    Dim GenbankViewer As GenbankViewer

    Friend LocalBLAST As LocalBlast

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Using File As System.Windows.Forms.OpenFileDialog = New System.Windows.Forms.OpenFileDialog
            File.Filter = "Program Assembly(*.exe)|*.exe"
            If File.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                txtBlastBin.Text = File.FileName
                Out(String.Format("Variable 'BlastBin' has been changed to a new value: {0}", txtBlastBin.Text))
            End If
        End Using
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim s As String = SelectFolder()

        If Not String.IsNullOrEmpty(s) Then
            txtBlastDB.Text = s
            Out(String.Format("Variable 'BlastDB' has been changed to a new value: {0}", s))
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        c2Profile.BlastBin = txtBlastBin.Text
        c2Profile.BlastDb = txtBlastDB.Text

        Out("Save c2 program profile data.")
        Call c2Profile.Save()
    End Sub

    Private Sub Initialize()
        Out("Loading profile data...", ConsoleColor.White, VIEWER)
        txtBlastBin.Text = c2Profile.BlastBin
        Out(String.Format("Blast bin: {0}", c2Profile.BlastBin), ConsoleColor.White, VIEWER)
        If String.IsNullOrEmpty(txtBlastBin.Text) OrElse Not FileIO.FileSystem.DirectoryExists(txtBlastBin.Text) Then
            Out("The 'BlastBin' directory value is not a valid string or not exists! External command calling maybe failure!",
                ConsoleColor.Yellow, VIEWER, Microsoft.VisualBasic.Logging.MSG_TYPES.WRN)
        End If
        txtBlastDB.Text = c2Profile.BlastDb
        Out(String.Format("Blast db: {0}", c2Profile.BlastDb), ConsoleColor.White, VIEWER)
        If String.IsNullOrEmpty(txtBlastDB.Text) OrElse Not FileIO.FileSystem.DirectoryExists(txtBlastDB.Text) Then
            Out("The 'BlastDB' directory value is not a valid string or not exists! External command calling maybe failure!",
                ConsoleColor.Yellow, VIEWER, Microsoft.VisualBasic.Logging.MSG_TYPES.WRN)
        End If
    End Sub

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AddHandler Me.Load, Sub() Call Initialize()
        LinkLabel4.Visible = False
    End Sub

    Private Sub NCBIViewer_Load(sender As Object, e As EventArgs) Handles Me.Load
        GenbankViewer = New GenbankViewer(Me)
        LocalBLAST = New LocalBlast(Me)

        OpenFileDialog1.Filter = "NCBI Genbank Flat File(*.gbk;*.gb)|*.gbk;*.gb"
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        Using File As System.Windows.Forms.OpenFileDialog = New System.Windows.Forms.OpenFileDialog
            File.Filter = "Program Assembly(*.exe)|*.exe"
            If File.ShowDialog = System.Windows.Forms.DialogResult.OK Then
                TextBox4.Text = File.FileName
                Out(String.Format("Variable 'formatdb' has been changed to a new value: {0}", TextBox4.Text))
            End If
        End Using
    End Sub
End Class
