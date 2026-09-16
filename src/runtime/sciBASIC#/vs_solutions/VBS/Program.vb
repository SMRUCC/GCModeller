Imports Microsoft.VisualBasic.CommandLine
Imports VBScriptHost.Script

Module Program

    ''' <summary>
    ''' vbs ./script.vb --arg1=xxx --arg2=xxx
    ''' vbs make-project ./script.vb [--verbose] [--no-build] [--force]
    ''' </summary>
    ''' <param name="args"></param>
    Public Function Main(args As String()) As Integer
        If args.IsNullOrEmpty Then
            Call PrintUsage()

            Return 0
        End If

        ' 子命令分发: make-project 之外的第一个词元一律视为脚本文件路径(保持原有行为不变)
        If String.Equals(args(0), "make-project", StringComparison.OrdinalIgnoreCase) Then
            Return MakeProject.Run(args)
        End If

        Dim cmdl As CommandLine = CommandLine.BuildFromArguments(args, NoSubCommand:=False)
        Dim scriptFile As String = args(0)
        Dim verbose As Boolean = cmdl("--verbose")
        Dim vbs As ScriptParseResult = VBScript.ParseScript(scriptFile, verbose:=verbose)

        Using script As ScriptRuntime = vbs.CompileScript(debug:=verbose)
            Return script.Run(args)
        End Using
    End Function

    Private Sub PrintUsage()
        Call Console.WriteLine("vbs </path/to/script.vb> [--arg1=val1 --arg2=val2 ...]")
        Call Console.WriteLine("vbs make-project </path/to/script.vb> [--verbose] [--no-build] [--force]")
    End Sub
End Module
