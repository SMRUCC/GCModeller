Imports Microsoft.VisualBasic.Scripting.ShoalShell

Public Class ShellScriptTerm

    Dim ShellScript As Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.ScriptEngine = New Runtime.ScriptEngine()

    '  Private Sub ShellControl1_CommandEntered(sender As Object, e As UILibrary.ShellControl.CommandEnteredEventArgs) Handles ShellControl1.CommandEntered
    ' Call ShellScript.EXEC(e.Command)
    '  End Sub

    Private Sub ShellScriptTerm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim p = Process.GetCurrentProcess
        p.StartInfo.RedirectStandardOutput = True

        Dim STDOutput = p.StandardOutput

        Call New Threading.Thread(Sub() Call DispSTDOutput(STDOutput)).Start()
    End Sub

    Private Sub DispSTDOutput(STDout As IO.StreamReader)
        Do While True
            '  Call ShellControl1.WriteText(STDout.ReadToEnd)
            Threading.Thread.Sleep(1)
        Loop
    End Sub
End Class
