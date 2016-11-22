Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Terminal.STDIO
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Configuration
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Scripting

''' <summary>
''' The shoal program main entry.
''' </summary>
''' <remarks></remarks>
Module Program

    Public Const STATUS_PREALPHA As String = "pre-alpha"
    Public Const STATUS_BETA As String = "beta"
    Public Const SHOAL_SHELL As String = "Shoal Shell"

    ''' <summary>
    ''' Shoal主程序的欢迎文本
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private ReadOnly Property ConsoleSplashPrintPage As String
        Get
            Return _
<CP>LANS Shoal Shell [version {0} - {1};  {2}]
Copyright (c) 2015 SMRUCC SystemsBiology. All Rights Reserved.

Shoal was developed by:  xieguigang(xie.guigang@gcmodeller.org)
                         Miss asuka(amethyst.asuka@gcmodeller.org)

  "A lot of fish in a shoal, in a gigantic scientific ocean."

Shoal running cross-platform(Windows7/8/10, Linux/Ubuntu, OS X), and you can hybrid scripting using shoal with R/Perl
ShoalShell project source code is available on Github:  

    https://github.com/smrucc/shoal

Commands quick guide:

  libraries  - list all of the installed package for shoal shell

  library    - Install a assembly library for Shoal
               Usage: library &lt;assembly_path&gt;
               The alias command for the "library" is "install"

  ?          - Get the help information about the installed package or command
               Usage: ? Namespace/&lt;command name&gt;/keyword 

  !          - Attaching the external hybrid scripting language environment entry point onto Shoal
               Usage: !&lt;hybrid_script_name&gt;

  imports    - Imports the namespace from a installed package
               Usage: imports namespace

  source     - Calling the script file on your filesystem
               Usage: source &lt;script_file&gt; argvs

  ver()      - Display the version of Shoal
  wiki()     - Open the wiki page to get help information, this command required a Internet connection.
               Usage: wiki() &lt;keyword&gt;

  q()        - quit the shoal shell

For more details help information, please visit the WIKI page on Shoal shell's wiki: http://wiki.gcmodeller.org/shoal/

Program files and source code was distributed under the GPL3 Licensed to "{3}", using license() command to view the license details.

</CP>
        End Get
    End Property

    Public ReadOnly Property Configuration As Settings(Of Config) = Config.Default

    Public Function Main() As Integer
        Return GetType(CLI).RunCLI(App.CommandLine, AddressOf Program.ExecuteScriptFile, AddressOf Program.ExecuteEmpty)
    End Function

    Private Function ExecuteScriptFile(path As String, args As CommandLine.CommandLine) As Integer
        Return Program.RunScriptFile(path, args:=args.ToArray)
    End Function

    Private Function ExecuteEmpty() As Integer
        Return Program.ScriptShellTerminal(-1, "")
    End Function

    Private Function RunScriptFile(ScriptFile As String, args As NamedValue(Of String)()) As Integer
        Using scriptEngine As ScriptEngine = New ScriptEngine(Config.Default.SettingsData)
            Return __runScriptFile(scriptEngine, ScriptFile, args)
        End Using
    End Function

    Private Function __runScriptFile(ScriptEngine As ScriptEngine, ScriptFile As String, args As NamedValue(Of String)()) As Integer
        Call ScriptEngine.Imports(GetType(InternalCommands))

        For Each item In args
            Call ScriptEngine.MMUDevice.WriteMemory(item.Name, item.Value)
        Next

        Dim currentWork As String = My.Computer.FileSystem.CurrentDirectory
        Dim work As String = FileIO.FileSystem.GetParentPath(ScriptFile)

        If String.IsNullOrEmpty(work) Then
            work = "./"
        End If

        My.Computer.FileSystem.CurrentDirectory = work
        Dim value = ScriptEngine.Source(ScriptFile)
        My.Computer.FileSystem.CurrentDirectory = currentWork

        If value Is Nothing Then ' 当API是一个Sub过程的时候会返回System.Void，具体值为Nothing，在这里不太好进行判断
            Return 1
        End If

        Dim i As Integer = If(
            InputHandler.Convertible(value.GetType, GetType(Integer)),
            InputHandler.CTypeDynamic(InputHandler.ToString(value), GetType(Integer)),
            If(value Is Nothing, 1, 0))

        Return i
    End Function

    ''' <summary>
    ''' 打开Shoal的交互终端
    ''' </summary>
    ''' <returns></returns>
    ''' <param name="work">空字符串表示使用配置文件之中的路径，反之使用本参数所制定的路径做出初始化路径</param>
    ''' <remarks></remarks>
    Public Function ScriptShellTerminal(ListenerPort As Integer, work As String) As Integer
        Dim usr As String = If(String.IsNullOrEmpty(My.Computer.Name), "EMPTY_USER_NAME", My.Computer.Name)

        Console.BackgroundColor = ConsoleColor.DarkCyan
        Console.ForegroundColor = ConsoleColor.White

        Call Console.WriteLine(Program.ConsoleSplashPrintPage,
                               My.Application.Info.Version.ToString,
                               STATUS_BETA,
                               $"{Environment.OSVersion.Platform.ToString} - {Environment.OSVersion.Version.ToString}",
                               usr)

        work = If(String.IsNullOrEmpty(work),
            Program.Configuration.SettingsData.InitDirectory,
            FileIO.FileSystem.GetDirectoryInfo(work).FullName)

        My.Computer.FileSystem.CreateDirectory(work)
        My.Computer.FileSystem.CurrentDirectory = work

        Using ScriptEngine As ScriptEngine = If(ListenerPort > 0,
            New Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Debugging.Debugger(
                DebugListenerPort:=ListenerPort,
                Config:=Program.Configuration.SettingsData),
            New ScriptEngine(Program.Configuration.SettingsData))
            Return __scriptShellTerminal(ScriptEngine, ListenerPort > 0)
        End Using
    End Function

    Public Function __scriptShellTerminal(ScriptEngine As ScriptEngine, DebuggerMode As Boolean) As Integer
        Call ScriptEngine.Imports(GetType(InternalCommands))

        If DebuggerMode Then
            Call __runDebugger(ScriptEngine)
        Else
            Call __runTerminal(ScriptEngine)
        End If

        Call Console.WriteLine("[MESSAGE] Auto save shoal environment variables..." & vbCrLf & "   -----> ""file:///{0}""", Program.Configuration.SettingsData.FilePath)
        Call Program.Configuration.SettingsData.Save()
        Call Console.WriteLine("[DONE!]")

        Return 0
    End Function

    Private Sub __runTerminal(ScriptEngine As ScriptEngine)
        Do While True
            Dim input As String = scanf("$  ", ConsoleColor.Yellow)

            If String.Equals(input, "q()", StringComparison.OrdinalIgnoreCase) Then
                Exit Do
            Else
                Call ScriptEngine.Exec(input)
            End If
        Loop
    End Sub

    Private Sub __runDebugger(ScriptEngine As ScriptEngine)
        Dim Debugger As Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Debugging.Debugger =
            DirectCast(ScriptEngine, Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Debugging.Debugger)

        Do While Not Debugger.DebuggerExit
            Call Threading.Thread.Sleep(2000)
        Loop
    End Sub
End Module