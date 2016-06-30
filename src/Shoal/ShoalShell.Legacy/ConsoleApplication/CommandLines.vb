Imports Microsoft.VisualBasic.CommandLine.Reflection

''' <summary>
''' This module define the shoal commandlines for the command line interpreter.
''' </summary>
''' <remarks></remarks>
Module CommandLines

    <Command("--version", Info:="Print the version of the shoal shell in the console.")>
    Public Function Version() As Integer
        Call Console.WriteLine($"Shoal Shell {My.Application.Info.Version.ToString}")
        Return 0
    End Function

    <Command("&", info:="Start the shoal shell in the current directory, not using the directory in the profile data.")>
    Public Function Start() As Integer
        Dim work As String = Environment.CurrentDirectory
        Return Program.ScriptShellTerminal(-1, work)
    End Function

    <Command("set", info:="Setting up the shoal environment variables, you can using var command to view all of the avaliable variable in the shoal shell.",
        usage:="set <var_Name> <string_value>", example:="set lastdirasinit true")>
    Public Function SetValue(argvs As CommandLine.CommandLine) As Integer
        Using Profile = Config.Default
            Dim var As String = argvs.Parameters.First
            Dim value As String = argvs.Parameters(1)
            Call Profile.Set(var, value)
        End Using

        Return 0
    End Function

    <Command("var", info:="Get the environment variable value in the shoal shell, if a variable name is not specific, then the shoal will list all of the variable value in shoal.",
        usage:="var [<var_Name>]", example:="var registry_location")>
    Public Function GetValue(argvs As CommandLine.CommandLine) As Integer
        Dim Profile = Config.Default
        Call Console.WriteLine(Profile.WriteLine(argvs.Parameters.FirstOrDefault))
        Return 0
    End Function

    <Command("::", info:="Execute one script line, this command is useful for the shoal API development and debugging.",
        usage:=":: <scriptline>",
        example:="shoal :: ""hello world!"" -> msgbox title ""This is a hello world tesing example!""")>
    Public Function Shell(scriptLine As CommandLine.CommandLine) As Integer
        Dim strLine As String = Mid(scriptLine.CLICommandArgvs, 3).Trim
        If Not String.IsNullOrEmpty(strLine) Then
            strLine = strLine.GetString(wrapper:="""")
        Else
            Call Console.WriteLine("Syntax error on the shell commandline!")
            Return -1
        End If

        Using ScriptHost As Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ShellScript =
            New Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ShellScript(ShowInitializeMessage:=False, LibraryRegistry:=Program.Configuration.TargetSettingsData.get_RegistryFile)
            Call ScriptHost.Imports(GetType(InternalCommands))
            Return ScriptHost.EXEC(ShellScript:=strLine)
        End Using
    End Function

    <Command("-register_modules", info:="Register the shellscript API module assembly DLL or assembly exe file to the shellscript type registry.",
        usage:="-register_modules -path <assemnly_dll_file> [-module_name <string_name>]")>
    <ParameterDescription("-path", False, Description:="the assembly file path of the API module that you are going to register in the shellscript type library")>
    <ParameterDescription("-module_name", True, Description:="The module name for the register type namespace, if the target " &
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

        Using Registry As Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry =
            Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.CreateDefault
            Call Registry.RegisterAssemblyModule(FileIO.FileSystem.GetFileInfo(Path).FullName, AssemblyName)
        End Using

        Return 0
    End Function

    <Command("-scan.plugins", usage:="-scan.plugins -dir <dir>[ -ext *.*/*.dll/*.exe/*.lib]",
        info:="Scanning all of the avaliable shoal plugin modules in the specific directory and install all of them into the shoal registry.",
        example:="-scan.plugins -dir ./ -ext *.dll")>
    Public Function ScanPlugins(argvs As CommandLine.CommandLine) As Integer
        Dim Dir As String = argvs("-dir"), Ext As String = argvs("-ext")
        Ext = If(String.IsNullOrEmpty(Ext), "*.*", Ext)

        Dim FilesForScan = FileIO.FileSystem.GetFiles(Dir, FileIO.SearchOption.SearchTopLevelOnly, Ext)
        Dim Registry As Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry =
            Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.CreateFromFile(Program.Configuration.TargetSettingsData.get_RegistryFile)

        For Each File As String In FilesForScan
            On Error Resume Next
            Call Registry.RegisterAssemblyModule(File, "")
        Next

        Call Registry.Save()

        Return 0
    End Function

    <Command("-show_info", info:="Print the shoal script meta data information, which was define as the macro in the script file.",
        usage:="-show_info <file_path>", example:="-show_info /home/xieguigang/Desktop/macro_test.shl")>
    Public Function ShowInfo(argvs As CommandLine.CommandLine) As Integer
        Dim ScriptFile As String = argvs.Parameters.First
        If FileIO.FileSystem.FileExists(ScriptFile) Then
            Call Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ObjectModels.ScriptInfo.LoadInfo(ScriptFile, Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.CreateFromFile(Program.Configuration.TargetSettingsData.get_RegistryFile)).DisplayInfo()
        Else
            Call Console.WriteLine("'{0}' is not exists on the filesystem!", ScriptFile)
            Return -1
        End If

        Return 0
    End Function

    <Command("-start",
        info:="Start the shoal shell using the user custom data.",
        usage:="-start -init_dir <inits_dir> -registry <regustry_xml> -imports <dll_paths>")>
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
    <Command("/debug", info:="Start the shoal shell in debug output mode.", usage:="/debug listener_port <listen_port> [-work <working_Dir>]")>
    Public Function DEBUG(argvs As CommandLine.CommandLine) As Integer
        Dim Port As Integer = Val(argvs("listener_port"))
        Dim Work As String = argvs("-work")
        Return Program.ScriptShellTerminal(Port, Work)
    End Function
End Module