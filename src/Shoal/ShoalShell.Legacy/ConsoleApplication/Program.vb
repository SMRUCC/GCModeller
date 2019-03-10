Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ConsoleDevice.STDIO

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
Copyright (c) 2014 LANS SystemsBiology Engineering Workstation.

Shoal was developed by:  xieguigang(xie.guigang@gmail.com)

  "A lot of fish in a shoal, in a gigantic scientific ocean."

Shoal running cross-platform(Windows7/8/10, Linux/Ubuntu, OS X), and you can hybrid scripting using shoal with R/Perl
ShoalShell project source code is available on SourceForge via SVN command:  

    svn checkout svn://svn.code.sf.net/p/shoal/Source/ shoal-Source.

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

For more details help information, please visit the WIKI page on Shoal shell's SourceForge project home: sourceforge.net/projects/shoal/wiki

Program files and source code was distributed under the GPL3 Licensed to "{3}", using license() command to view the license details.

</CP>
        End Get
    End Property

    Public ReadOnly Property Configuration As Microsoft.VisualBasic.ComponentModel.Settings.Settings(Of Config) =
        Config.Default

    Public Function Main() As Integer
        Return GetType(CommandLines).RunCLI(Command,
                                            AddressOf Program.ExecuteScriptFile,
                                            AddressOf Program.ExecuteEmpty)
    End Function

    Private Function ExecuteScriptFile(path As String, args As CommandLine.CommandLine) As Integer
        Return Program.RunScriptFile(path, argvs:=args.ToArray)
    End Function

    Private Function ExecuteEmpty() As Integer
        Return Program.ScriptShellTerminal(-1, "")
    End Function

    Private Function RunScriptFile(ScriptFile As String, argvs As KeyValuePair(Of String, String)()) As Integer
        Using ScriptHost As Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ShellScript =
            New Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ShellScript(
            ShowInitializeMessage:=False,
            LibraryRegistry:=Program.Configuration.TargetSettingsData.get_RegistryFile)
            Return __runScriptFile(ScriptHost, ScriptFile, argvs)
        End Using
    End Function

    Private Function __runScriptFile(ScriptHost As Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ShellScript,
                                     ScriptFile As String,
                                     argvs As KeyValuePair(Of String, String)()) As Integer

        Call ScriptHost.Imports(GetType(InternalCommands))

        For Each item In argvs
            Call ScriptHost.ScriptEngineMemoryDevice.InsertOrUpdate(item.Key, item.Value)
        Next

        Dim currentWork As String = My.Computer.FileSystem.CurrentDirectory
        Dim work As String = FileIO.FileSystem.GetParentPath(ScriptFile)

        If String.IsNullOrEmpty(work) Then
            work = "./"
        End If

        My.Computer.FileSystem.CurrentDirectory = work
        ScriptHost.ExceptionHandleRedirect = ScriptFile & ".ERROR.log"
        Dim i = ScriptHost.EXEC(ShellScript:=FileIO.FileSystem.ReadAllText(ScriptFile))
        My.Computer.FileSystem.CurrentDirectory = currentWork

        Return i
    End Function

    ''' <summary>
    ''' 打开Shoal的交互终端
    ''' </summary>
    ''' <returns></returns>
    ''' <param name="work">空字符串表示使用配置文件之中的路径，反之使用本参数所制定的路径做出初始化路径</param>
    ''' <remarks></remarks>
    Public Function ScriptShellTerminal(ListenerPort As Integer, work As String) As Integer
        Dim UserName As String = If(String.IsNullOrEmpty(My.Computer.Name), "EMPTY_USER_NAME", My.Computer.Name)

        Call System.Console.WriteLine(Program.ConsoleSplashPrintPage,
                                      My.Application.Info.Version.ToString,
                                      STATUS_BETA,
                                      $"{Environment.OSVersion.Platform.ToString} - {Environment.OSVersion.Version.ToString}",
                                      UserName)

        work = If(String.IsNullOrEmpty(work),
            Program.Configuration.TargetSettingsData.get_InitDirectory,
            FileIO.FileSystem.GetDirectoryInfo(work).FullName)

        My.Computer.FileSystem.CreateDirectory(work)
        My.Computer.FileSystem.CurrentDirectory = work

        Using ScriptEngine As Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ShellScript =
            If(ListenerPort > 0,
                New Scripting.ShoalShell.Runtime.Debugging.Debugger(
                    LibraryRegistry:=Program.Configuration.TargetSettingsData.get_RegistryFile,
                    DebugListenerPort:=ListenerPort,
                    ShowInitializeMessage:=True),
                New Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ShellScript(
                    ShowInitializeMessage:=True,
                    LibraryRegistry:=Program.Configuration.TargetSettingsData.get_RegistryFile))
            Return __scriptShellTerminal(ScriptEngine, ListenerPort > 0)
        End Using
    End Function

    Public Function __scriptShellTerminal(ScriptEngine As Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ShellScript,
                                          DebuggerMode As Boolean) As Integer
        Call ScriptEngine.Imports(GetType(InternalCommands))

        If DebuggerMode Then
            Call __runDebugger(ScriptEngine)
        Else
            Call __runTerminal(ScriptEngine)
        End If

        Call Console.WriteLine("[MESSAGE] Auto save shoal environment variables..." & vbCrLf & "   -----> ""file:///{0}""", Program.Configuration.TargetSettingsData.FilePath)
        Call Program.Configuration.TargetSettingsData.Save()
        Call Console.WriteLine("[DONE!]")

        Return 0
    End Function

    Private Sub __runTerminal(ScriptEngine As Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ShellScript)
        Do While True
            Dim CommandLine As String = scanf("$  ")
            If String.Equals(CommandLine, "q()", StringComparison.OrdinalIgnoreCase) Then
                Exit Do
            Else
                Call ScriptEngine.EXEC(ShellScript:=CommandLine)
            End If
        Loop
    End Sub

    Private Sub __runDebugger(ScriptEngine As Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ShellScript)
        Dim Debugger = DirectCast(ScriptEngine, Scripting.ShoalShell.Runtime.Debugging.Debugger)

        Do While Not Debugger.DebuggerExit
            Call Threading.Thread.Sleep(2000)
        Loop
    End Sub
End Module