Imports Microsoft.VisualBasic

Public Class InstallFastaDb

    Dim Files As List(Of String)

    Public Sub LoadUninstall(Files As List(Of String))
        For Each File In Files
            Me.CheckedListBox1.Items.Add(String.Format("{0}  [{1}]", File.Replace("\", "/").Split(CChar("/")).Last.Split(CChar(".")).First, FileIO.FileSystem.GetFileInfo(File).Length))
        Next

        Me.Files = Files
    End Sub

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

    Sub New(Files As List(Of String))
        InitializeComponent()

        Call LoadUninstall(Files)

        AddHandler LinkLabel1.Click, Sub() Call Install()
    End Sub

    Sub Install()
        Out2(String.Format("Select {0} uninstalled database, start to install...", CheckedListBox1.CheckedIndices.Count))

        For Each i As Integer In CheckedListBox1.CheckedIndices
            Dim Process As Microsoft.VisualBasic.CommandLine.IORedirect
            Process = New Microsoft.VisualBasic.CommandLine.IORedirect(LocalBlast.c2, String.Format("build -i ""{0}"" -f fsa", Files(i)))

            AddHandler Process.DataArrival, Sub(s As String) Call Out2(s)
            AddHandler Process.ProcessExit, Sub(c As Integer, s As String) Out2(String.Format("Installation done!  c2 exit code = {0}, exitTime = {1}", c, s))

            Out2(String.Format("[{0}] Start to install database:{1}  {2}", i, vbCrLf, Files(i)))

            Process.Start(WaitForExit:=True)
        Next
    End Sub

    Private Sub Out2(s As String)
        Call Out(s, ConsoleColor.White, "C2", Microsoft.VisualBasic.Logging.MSG_TYPES.INF)
        TextBox1.AppendText(s & vbCrLf)
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        For i As Integer = 0 To CheckedListBox1.Items.Count - 1
            CheckedListBox1.SetItemCheckState(i, System.Windows.Forms.CheckState.Checked)
        Next
    End Sub

    Private Sub LinkLabel2_LinkClicked(sender As Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles LinkLabel2.LinkClicked
        Call LocalBlast.InstallNewDB()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Close()
    End Sub
End Class