Module Program

    Sub Main()
        Dim tmp As String = FileIO.FileSystem.GetTempFileName & "~/"
        Dim init As String = tmp & "/installer.exe"
        Dim framework As String = tmp & "/Microsoft.VisualBasic.Architecture.Framework_v3.0_22.0.76.201__8da45dcd8060cc9a.dll"

        Call FileIO.FileSystem.CreateDirectory(tmp)
        Call FileIO.FileSystem.WriteAllBytes(
            init,
            My.Resources.Installer,
            False)
        Call FileIO.FileSystem.WriteAllBytes(
            framework,
            My.Resources.Microsoft_VisualBasic_Architecture_Framework_v3_0_22_0_76_201__8da45dcd8060cc9a,
            False)

        Call runasAdmin(init)
    End Sub

    Private Sub runasAdmin(init As String)
        Dim startInfo As New ProcessStartInfo()
        startInfo.UseShellExecute = True
        startInfo.WorkingDirectory = Environment.CurrentDirectory
        startInfo.FileName = init
        startInfo.Verb = "runas"

        Try
            Dim p As Process = Process.Start(startInfo)
        Catch ex As Exception
            Call MsgBox(ex.ToString, MsgBoxStyle.Critical)
        End Try
    End Sub
End Module
