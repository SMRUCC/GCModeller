#Region "Microsoft.VisualBasic::de86934d1515e4dd1623f4ad2a9322ba, ..\workbench\devenv\Controls\ShellScriptTerm.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xieguigang (xie.guigang@live.com)
    '       xie (genetics@smrucc.org)
    ' 
    ' Copyright (c) 2016 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.

#End Region

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

