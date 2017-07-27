Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.Scripting.MetaData
Imports Microsoft.VisualBasic.Scripting.ShoalShell
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Configuration
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.SCOM

''' <summary>
''' This module define the shoal commandlines for the command line interpreter.
''' </summary>
''' <remarks></remarks>
''' 
<PackageAttribute("Shoal",
                  Category:=APICategories.CLI_MAN,
                  Url:="http://gcmodeller.org",
                  Description:="This module define the shoal commandlines for the command line interpreter.",
                  Publisher:="GCModeller")>
Module CLI

    <ExportAPI("--version", Info:="Print the version of the shoal shell in the console.")>
    Public Function Version() As Integer
        Call Console.WriteLine($"Shoal Shell {My.Application.Info.Version.ToString}")
        Return 0
    End Function

    <ExportAPI("~", Info:="Start the shoal shell in the current directory, not using the directory in the profile data.")>
    Public Function Start() As Integer
        Dim work As String = Environment.CurrentDirectory
        Return Program.ScriptShellTerminal(-1, work)
    End Function

    <ExportAPI("set", Info:="Setting up the shoal environment variables, you can using var command to view all of the avaliable variable in the shoal shell.",
        Usage:="set <var_Name> <string_value>", Example:="set lastdirasinit true")>
    Public Function SetValue(argvs As CommandLine.CommandLine) As Integer
        Using Profile = Config.Default
            Dim var As String = argvs.Parameters.First
            Dim value As String = argvs.Parameters(1)
            Call Profile.Set(var, value)
        End Using

        Return 0
    End Function

    <ExportAPI("var", Info:="Get the environment variable value in the shoal shell, if a variable name is not specific, then the shoal will list all of the variable value in shoal.",
        Usage:="var [<var_Name>]", Example:="var registry_location")>
    Public Function GetValue(argvs As CommandLine.CommandLine) As Integer
        Dim Profile = Config.Default
        Call Console.WriteLine(Profile.View(argvs.Parameters.FirstOrDefault))
        Return 0
    End Function

    <ExportAPI("::", Info:="Execute one script line, this command is useful for the shoal API development and debugging.",
        Usage:=":: <scriptline>",
        Example:="shoal :: ""hello world!"" -> msgbox title ""This is a hello world tesing example!""")>
    Public Function Shell(scriptLine As CommandLine.CommandLine) As Integer
        Dim strLine As String = Mid(scriptLine.CLICommandArgvs, 3).Trim
        If Not String.IsNullOrEmpty(strLine) Then
            strLine = strLine.GetString(wrapper:="""")
        Else
            Call Console.WriteLine("Syntax error on the shell commandline!")
            Return -1
        End If

        Using ScriptHost As ScriptEngine = New ScriptEngine(Config.Default.SettingsData) '(ShowInitializeMessage:=False, LibraryRegistry:=Program.Configuration.TargetSettingsData.get_RegistryFile)
            ' Call ScriptHost.Imports(GetType(InternalCommands))
            Return ScriptHost.Exec(strLine)
        End Using
    End Function

    <ExportAPI("-register_modules", Info:="Register the shellscript API module assembly DLL or assembly exe file to the shellscript type registry.",
        Usage:="-register_modules -path <assemnly_dll_file> [-module_name <string_name>]")>
    <Argument("-path", False, Description:="the assembly file path of the API module that you are going to register in the shellscript type library")>
    <Argument("-module_name", True, Description:="The module name for the register type namespace, if the target " &
        "assembly just have one shellscript namespace, then this switch value will override the namespace attribute value if " &
        "the value of this switch is not null, when there are more than one shellscript namespace was declared in the module, then this switch opetion will be disabled.")>
    Public Function RegisterModule(argvs As CommandLine.CommandLine) As Integer
        Dim Path As String = argvs("-path")
        Dim AssemblyName As String = argvs("-module_name")

        If String.IsNullOrEmpty(Path) Then
            Return -1
        End If

        If Not String.IsNullOrEmpty(Path) AndAlso Not FileIO.FileSystem.FileExists(Path) Then
            Call Console.WriteLine("FILE_NOT_FOUND: " & Path)
            Return -2
        End If

        Dim Db As SPM.PackageModuleDb = SPM.PackageModuleDb.Load(Program.Configuration.SettingsData.GetRegistryFile)

        Using SPM As New SPM.ShoalPackageMgr(Db)
            Call SPM.Imports(Path)
        End Using

        Return 0
    End Function

    <ExportAPI("-scan.plugins", Usage:="-scan.plugins -dir <dir>[ -ext *.*/*.dll/*.exe/*.lib /top_only /clean]",
        Info:="Scanning all of the avaliable shoal plugin modules in the specific directory and install all of them into the shoal registry.",
        Example:="-scan.plugins -dir ./ -ext *.dll")>
    Public Function ScanPlugins(args As CommandLine.CommandLine) As Integer
        Call RuntimeEnvironment.ScanPlugins(args)

        Dim ScriptHost As ScriptEngine = New ScriptEngine(Config.Default.SettingsData)
        Return ScriptHost.Exec("Wiki.Http_Server::Doc.Build")
    End Function

    '<Command("-show_info", Info:="Print the shoal script meta data information, which was define as the macro in the script file.",
    '    Usage:="-show_info <file_path>", Example:="-show_info /home/xieguigang/Desktop/macro_test.shl")>
    'Public Function ShowInfo(argvs As CommandLine.CommandLine) As Integer
    '    Dim ScriptFile As String = argvs.Parameters.First
    '    If FileIO.FileSystem.FileExists(ScriptFile) Then
    '        Call Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ObjectModels.ScriptInfo.LoadInfo(ScriptFile, Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.CreateFromFile(Program.Configuration.TargetSettingsData.GetRegistryFile)).DisplayInfo()
    '    Else
    '        Call Console.WriteLine("'{0}' is not exists on the filesystem!", ScriptFile)
    '        Return -1
    '    End If

    '    Return 0
    'End Function

    <ExportAPI("-start",
        Info:="Start the shoal shell using the user custom data.",
        Usage:="-start -init_dir <inits_dir> -registry <regustry_xml> -imports <dll_paths>")>
    Public Function Start(argvs As CommandLine.CommandLine) As Integer
        Throw New NotImplementedException
    End Function

    ''' <summary>
    ''' 以调试模式启动脚本引擎
    ''' </summary>
    ''' <param name="argvs"></param>
    ''' <returns></returns>
    ''' <remarks>本应用程序作为客户端，与主机程序进行TcpSocket通信来完成调试信息的数据</remarks>
    ''' 
    <ExportAPI("/debug", Info:="Start the shoal shell in debug output mode.", Usage:="/debug listener_port <listen_port> [-work <working_Dir>]")>
    Public Function DEBUG(argvs As CommandLine.CommandLine) As Integer
        Dim Port As Integer = argvs.GetInt32("listener_port")
        Dim Work As String = argvs("-work")
        Return Program.ScriptShellTerminal(Port, Work)
    End Function

    <ExportAPI("--logs.show")>
    Public Function ShowLogs() As Integer
        Dim Dir As String = App.LogErrDIR
        Call Process.Start(Dir)
        Return 0
    End Function
End Module