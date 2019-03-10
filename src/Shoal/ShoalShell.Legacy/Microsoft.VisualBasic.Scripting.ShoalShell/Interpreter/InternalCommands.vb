Imports System.Text
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports System.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects
Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel
Imports System.Drawing
Imports Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry
Imports Microsoft.VisualBasic.Scripting.EntryPointMetaData

Namespace Interpreter

    ''' <summary>
    ''' 一个脚本程序中仅允许有一个内建命令的实例
    ''' </summary>
    ''' <remarks></remarks>
    Public Class InternalCommands : Inherits ShoalShell.Runtime.Objects.ObjectModels.IScriptEngineComponent

        Public _InternalVariableName_ForEach As String
        Public _InternalVariableName_DoWhile As String

        ''' <summary>
        ''' 脚本的执行入口点的定义
        ''' </summary>
        ''' <param name="Script">脚本中的内容</param>
        ''' <param name="parameters">如果为Nothing，则说明目标脚本的执行不需求参数</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Delegate Function ScriptSourceHandle(Script As String, parameters As KeyValuePair(Of String, Object)()) As Object

        <Command("basename", Info:="Get the name property of a specific file object or folder." & vbCrLf &
                                   "if the target path is a file, then returns its file name without its extension name.")>
        Public Function basename(path As String) As String
            If FileIO.FileSystem.DirectoryExists(path) Then
                Return FileIO.FileSystem.GetDirectoryInfo(path).Name
            Else
                Return IO.Path.GetFileNameWithoutExtension(path)
            End If
        End Function

        <ShoalShell.DelegateHandlers.EntryPointHandlers.MethodDelegateCaller.RequiredHostMemory>
        <Command("Collection.From")>
        Public Function CreateArray(argvs As String) As Object()

            If String.IsNullOrEmpty(argvs) Then
                Return New Object() {}
            End If

            If argvs.First = """"c AndAlso argvs.Last = """"c Then
                argvs = Mid(argvs, 2, Len(argvs) - 2)
            End If

            If InStr(argvs, ",") = 0 AndAlso argvs.First = "$"c Then
                Return CreateArray(ScriptEngine._EngineMemoryDevice.Item(Mid(argvs, 2).ToLower))
            ElseIf InStr(argvs, ",") = 0 AndAlso argvs.First = "&"c Then
                Return CreateArray(ScriptEngine._EngineMemoryDevice.GetConstant(Mid(argvs, 2)))
            End If

            Dim variables = ShoalShell.BuildInModules.System.Array.ParseVector(argvs)
            Dim paraList As List(Of Object) = New List(Of Object)

            For Each var As String In variables
                If var.First = "$"c Then
                    Call paraList.Add(ScriptEngine._EngineMemoryDevice.Item(Mid(var, 2).ToLower))
                ElseIf var.First = "&"c Then
                    Call paraList.Add(ScriptEngine._EngineMemoryDevice.GetConstant(Mid(var, 2)))
                Else
                    Call paraList.Add(var)
                End If
            Next

            Return paraList.ToArray
        End Function

        <Command("ver()")>
        Public Function Version() As Version
            Call Cowsay(String.Format("The version of Shoal is {0}", My.Application.Info.Version.ToString))
            Return My.Application.Info.Version
        End Function

        <Command("wiki()", info:="https://sourceforge.net/p/shoal/wiki/search/?q=keyword")>
        Public Function Wiki(<ParameterAlias("search.keyword")> Optional keyword As String = "") As String
            Dim url As String = If(String.IsNullOrEmpty(keyword), "https://sourceforge.net/p/shoal/wiki/", "https://sourceforge.net/p/shoal/wiki/search/?q=" & keyword.Replace(" ", "%20"))
            Call Process.Start(url)
            Return url
        End Function

        <Command("license()")>
        Public Function License() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Call sBuilder.AppendLine(My.Resources.license)
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine("Using gpl() command to see the whole GPL3 license.")

            Call Console.WriteLine(sBuilder.ToString)

            Return sBuilder.ToString
        End Function

        <Command("gpl()", info:="Prints the gpl3 license to the console screen that it applied to the shoal shell.")>
        Public Function GPL() As String
            Call Console.WriteLine(My.Resources.gpl)
            Return My.Resources.gpl
        End Function

        <Command("echo", info:="Print the message text on the terminal console.")>
        Public Function Echo(<ParameterAlias("Message", "")> Message As String) As String
            Call Console.WriteLine(Message)
            Return Message
        End Function

        Sub New(ScriptEngineHost As Runtime.Objects.ShellScript, disp_inits_message As Boolean, preferIndexedManual As Boolean)
            Call MyBase.New(ScriptEngineHost)

            Me.ScriptEngine._InternalHelpSystem = New Wiki.InternalHelpSearch(ScriptEngineHost, preferIndexedManual)
            Me.ScriptEngine.InternalEntryPointManager.ImportsInstanceNamespace(Me)
        End Sub

        ''' <summary>
        ''' 尝试获取脚本宿主内存之中的变量的值
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetValue(name As String) As Object
            Return _RuntimeEnvironment._EngineMemoryDevice.TryGetValue(name)
        End Function

        <Command("cowsay", info:="A cowsay trick to print the message on the console.")>
        Public Function Cowsay(Optional msg As String = "", Optional argvs As Microsoft.VisualBasic.CommandLine.CommandLine = Nothing) As String
            'If String.IsNullOrEmpty(msg) Then
            '    Return CowsayTricks.Cowsay("Moo. Hi!", "")
            'End If

            'If argvs Is Nothing Then
            '    Return CowsayTricks.Cowsay(msg, "")
            'End If

            'Dim Tokens = argvs.Tokens.Skip(2).ToArray
            'Dim pList As List(Of KeyValuePair(Of String, String)) = New Generic.List(Of KeyValuePair(Of String, String))
            'For i As Integer = 0 To Tokens.Count - 2
            '    Call pList.Add(New KeyValuePair(Of String, String)(Tokens(i), Tokens(i + 1)))
            'Next

            'Return CowsayTricks.Cowsay(argvs.Tokens(1), CommandLine.CommandLine.CreateObject("cowsay.argvs", pList.ToArray))
        End Function

        <Command("return.stack", info:="Force the shellscript memory return to the original caller stack.")>
        Public Function ReturnStack() As Boolean
            Return True
        End Function

        Public Function TryGetArrayValue(obj As Object) As Object()
            Dim Type As Type = obj.GetType
            If Array.IndexOf(Type.GetInterfaces, GetType(IEnumerable)) > -1 Then
                Return GetArray(en:=DirectCast(obj, IEnumerable))
            Else
                Throw New DataException("Target object is not a collection type!")
            End If
        End Function

        Private Function GetArray(en As IEnumerable) As Object()
            Dim LQuery = (From obj In en Select CType(obj, Object)).ToArray
            Return LQuery
        End Function

        <Command("do.while")>
        Public Function DoWhile(test As Object, [call] As String) As Object()
            Dim ReturnedValue As List(Of Object) = New List(Of Object)
            Dim argvs As CommandLine.CommandLine = CommandLine.CommandLine.TryParse([call])
            Dim ScriptFile As String = argvs.Name
            Dim sourceArgv As String() = argvs.Parameters
            '  Dim CallerStack = _ScriptEngine
            Dim IsSourceFile As Boolean = False
            Dim SourceDir As String = ""

            If FileIO.FileSystem.FileExists(ScriptFile) Then
                SourceDir = FileIO.FileSystem.GetParentPath(ScriptFile)
                ScriptFile = FileIO.FileSystem.ReadAllText(ScriptFile)
                IsSourceFile = True
            Else
                ScriptFile = [call]
            End If

            Do While _RuntimeEnvironment._EngineMemoryDevice.TryGetValue(_InternalVariableName_DoWhile).ToString.getBoolean
                Using Script As Runtime.Objects.ShellScript = New Runtime.Objects.ShellScript    '由于在新构造ShellScript对象的时候，会将本模块之中的_scriptHost进行替换，所以下面的语句直接操作_scriptHost对象就A相当于操作所被调用的脚本的宿主程序了

                    If Not sourceArgv.IsNullOrEmpty Then
                        For Each item In CommandLine.CommandLine.CreateParameterValues(sourceArgv, True)
                            Call _RuntimeEnvironment._EngineMemoryDevice.InsertOrUpdate(item.Key, ScriptEngine._EngineMemoryDevice.TryGetValue(item.Value))
                        Next
                    End If

                    Dim currentWork As String = My.Computer.FileSystem.CurrentDirectory
                    If IsSourceFile Then My.Computer.FileSystem.CurrentDirectory = SourceDir
                    Call Script.EXEC(ScriptFile)
                    If IsSourceFile Then My.Computer.FileSystem.CurrentDirectory = currentWork
                    Call ReturnedValue.Add(Script.GetStackValue)
                    Dim Memory = Script._EngineMemoryDevice
                    Call ScriptEngine._EngineMemoryDevice.SetValue(New KeyValuePair(Of String, Object)("$", Memory))
                    '   CommandsEntryHandler._ShellScriptHost = CallerStack
                End Using
            Loop

            Return ReturnedValue.ToArray
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="in"></param>
        ''' <param name="call">脚本命令或者脚本文件与参数列表</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <CommandLine.Reflection.Command("for.each", Info:="If the script has return value, then this function will return a value collection comes from the source function.")>
        Public Function ForEach([in] As Object, [call] As String) As Object()
            Dim argvs As CommandLine.CommandLine = CommandLine.CommandLine.TryParse([call])
            Dim ScriptFile As String = argvs.Name
            Dim sourceArgv As String() = argvs.Parameters
            '  Dim CallerStack = _ShellScriptHost
            Dim arrayData As Generic.IEnumerable(Of Object) = TryGetArrayValue([in])
            Dim IsSourceFile As Boolean = False
            Dim SourceDir As String = ""
            Dim ReturnedValue As List(Of Object) = New List(Of Object)

            If FileIO.FileSystem.FileExists(ScriptFile) Then
                SourceDir = FileIO.FileSystem.GetParentPath(ScriptFile)
                ScriptFile = FileIO.FileSystem.ReadAllText(ScriptFile)
                IsSourceFile = True
            Else
                ScriptFile = [call]
            End If

            For Each obj In arrayData
                Using Script As Runtime.Objects.ShellScript = New Runtime.Objects.ShellScript    '由于在新构造ShellScript对象的时候，会将本模块之中的_scriptHost进行替换，所以下面的语句直接操作_scriptHost对象就A相当于操作所被调用的脚本的宿主程序了

                    If Not sourceArgv.IsNullOrEmpty Then
                        For Each item In CommandLine.CommandLine.CreateParameterValues(sourceArgv, True)
                            Call _RuntimeEnvironment._EngineMemoryDevice.InsertOrUpdate(item.Key, ScriptEngine._EngineMemoryDevice.TryGetValue(item.Value))
                        Next
                    End If

                    Call _RuntimeEnvironment._EngineMemoryDevice.InsertOrUpdate(_InternalVariableName_ForEach, obj)

                    Dim currentWork As String = My.Computer.FileSystem.CurrentDirectory
                    If IsSourceFile Then My.Computer.FileSystem.CurrentDirectory = SourceDir
                    Call Script.EXEC(ScriptFile)
                    If IsSourceFile Then My.Computer.FileSystem.CurrentDirectory = currentWork
                    Call ReturnedValue.Add(Script.GetStackValue)
                    '    CommandsEntryHandler._ShellScriptHost = CallerStack
                End Using
            Next

            Return ReturnedValue.ToArray
        End Function

        <Command("if", Info:="The if test in the shoal shell scripting programming, " &
            "usage is:  if test condition_expression call expression;  " &
            "if the test condition expression is true then the expression of the call parameter will be execute, " &
            "otherwise if the test condition is false, the expression will be ignored.")>
        Public Function [If](test As Object, <ParameterAlias("then_call")> [call] As String) As Object
            Dim strValue As String = test.ToString

            _RuntimeEnvironment._EngineMemoryDevice._LastIf_Flag = strValue.getBoolean

            If _RuntimeEnvironment._EngineMemoryDevice._LastIf_Flag Then
                Call _RuntimeEnvironment.EXEC([call])
                Return _RuntimeEnvironment._EngineMemoryDevice.TryGetValue("$")
            Else
                Return -1
            End If
        End Function

        <Command("else")>
        Public Function [Else](Command As String) As Object
            If Not _RuntimeEnvironment._EngineMemoryDevice._LastIf_Flag Then
                Call _RuntimeEnvironment.EXEC(Command)
            End If

            Return _RuntimeEnvironment._EngineMemoryDevice.TryGetValue("$")
        End Function

        <Command("elseif")>
        Public Function [ElseIf](test As Object, [call] As String) As Object
            If Not _RuntimeEnvironment._EngineMemoryDevice._LastIf_Flag Then
                Return [If](test, [call])
            End If

            Return _RuntimeEnvironment._EngineMemoryDevice.TryGetValue("$")
        End Function

        <Command("return", info:="Returns the scripting variable value to the caller expression.")>
        Public Function ReturnValue(variable As Object) As Object
            Call _RuntimeEnvironment.PushStack(variable)
            Return _RuntimeEnvironment.GetStackValue
        End Function

        <Command("*free", info:="Free a variable in the shoal shell memory")>
        Public Function Free(variableName) As Boolean
            Dim Variable = (From item In _RuntimeEnvironment._EngineMemoryDevice Where String.Equals(item.Key, variableName, StringComparison.OrdinalIgnoreCase) Select item).First
            Call _RuntimeEnvironment._EngineMemoryDevice.Remove(Variable)
            Call FlushMemory()
            Return True
        End Function

        ''' <summary>
        ''' include &lt;filepath&gt;
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Command("Include", info:="include the target script file into this script file. This command will replace " &
            "the <include> declaration line by the target script file. so please consider the order of the include statements in your script.")>
        Public Function FileInclude(path As String) As String
            Return FileIO.FileSystem.ReadAllText(path)
        End Function

        ''' <summary>
        ''' 使用本方法导入外部命令，这样子就可以直接调用方法而不需要每一个命令行都添加模块名称了
        ''' </summary>
        ''' <param name="Namespace"></param>
        ''' <remarks></remarks>
        ''' 
        <Command("imports")>
        Public Function ImportsNamespace([Namespace] As String) As Integer
            Return ScriptEngine.InternalEntryPointManager.ImportsNamespace([Namespace])
        End Function

        ''' <summary>
        ''' 动态注册一个链接库，在进行挂载
        ''' </summary>
        ''' <param name="assemblyPath"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Command("library")>
        Public Function Library(<ParameterAlias("assembly.path", "The full path of the binary executable assembly file.")> assemblyPath As String) As Integer
            Call _RuntimeEnvironment._Interpreter._DelegateRegistry.RegisterAssemblyModule(assemblyPath, "")
            Call _RuntimeEnvironment._Interpreter._DelegateRegistry.Save()
            Call _RuntimeEnvironment._Interpreter._DelegateRegistry.LoadLibrary(assemblyPath, _RuntimeEnvironment._EngineMemoryDevice)
            Call _RuntimeEnvironment.ReloadEntryPoints()

            Return 0
        End Function

        <Command("cat", info:="Read the text file data from the parameter path and then print the text data on the console.")>
        Public Function Cat(path As String) As String
            Dim s As String = FileIO.FileSystem.ReadAllText(path)
            Call Console.WriteLine(s)
            Return s
        End Function

        <Command("Install", info:="Install a assembly module plugin from a specific local file. This is a alias command of the 'library' keyword, " &
            "unlike the 'library' command, this install command can not used in script file, only allowed used in interactive console.")>
        Public Function Install(<ParameterAlias("Assembly.Path", "The full path of the binary executable assembly file.")> assemblyPath As String) As Integer
            Return Library(assemblyPath)
        End Function

        ''' <summary>
        ''' 列举出所有已经注册的动态链接库
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <Command("libraries")>
        Public Function Libraries() As Integer
            If _RuntimeEnvironment._Interpreter._DelegateRegistry._InnerList.IsNullOrEmpty Then
                Call Console.WriteLine("Shoal didn't install any plugin module yet...")
                Return 0
            End If

            Dim MaxLength = (From item In _RuntimeEnvironment._Interpreter._DelegateRegistry._InnerList Select Len(item.ModuleName)).ToArray.Max
            Dim sBuilder As StringBuilder = New StringBuilder(1024)

            Call sBuilder.AppendLine(String.Format("  Namespaces{0} assembly library", New String(" "c, MaxLength - Len("namespaces") + 4)))
            Call sBuilder.AppendLine(String.Format("+----------{0}+-------------------------------------------------------------------------+", New String("-"c, MaxLength - Len("namespaces") + 4)))

            For Each item In (From itemobject In _RuntimeEnvironment._Interpreter._DelegateRegistry._InnerList Select itemobject Order By itemobject.ModuleName).ToArray
                Call sBuilder.AppendLine("  " & item.ModuleName)

                For Each Partition As DelegateHandlers.TypeLibraryRegistry.RegistryNodes.ModuleLoadEntry In item.Entries
                    Call sBuilder.AppendLine(String.Format(" {0}    @{1}", New String(" "c, MaxLength), Partition.AssemblyPath))
                Next
            Next
            Call Console.WriteLine(sBuilder.ToString)
            Return _RuntimeEnvironment._Interpreter._DelegateRegistry._InnerList.Count
        End Function

        <Command("?")>
        <ParameterDescription("ObjectName", Description:="The method name or the assembly module namespace.")>
        Public Function DisplayHelp(<ParameterAlias("object.name", "The method name/namespace/keyword value for the Wiki help search.")> Optional ObjectName As String = "") As String
            Dim str As String = Me._RuntimeEnvironment._InternalHelpSystem.GetHelpInfo(ObjectName, True)

            If String.IsNullOrEmpty(str) Then
                Call Console.WriteLine("Sorry, Shoal could not find any thing for the query keyword ""{0}""...  :-(", ObjectName)
            End If

            Return str
        End Function

        <Command("clear", info:="Clear the output console.")>
        Public Function Clear() As Integer
            Call Console.Clear()
            Return 0
        End Function

        <Command("pwd", info:="Gets the currently working directory of shoal shell.")>
        Public Function Pwd() As String
            Call Console.WriteLine(My.Computer.FileSystem.CurrentDirectory)
            Return My.Computer.FileSystem.CurrentDirectory
        End Function

        <Command("memory", info:="View the memory details in the shoal shell of current time.")>
        Public Function Memories() As String()
            Dim sBuilder As StringBuilder = New StringBuilder
            Dim maxLength As Integer
            Dim LQuery As String() = Nothing
            Dim Constants = _RuntimeEnvironment._EngineMemoryDevice.GetConstants

            If _RuntimeEnvironment._EngineMemoryDevice.IsNullOrEmpty AndAlso Constants.IsNullOrEmpty Then
                Const NO_VALUE = "Currently no variables exists in the script host memory!"
                Call Console.WriteLine(NO_VALUE)
                Return New String() {NO_VALUE}
            End If

            If Not _RuntimeEnvironment._EngineMemoryDevice.IsNullOrEmpty Then
                Call sBuilder.AppendLine()
                Call sBuilder.AppendLine(String.Format("    {0} VARIABLES", _RuntimeEnvironment._EngineMemoryDevice.Count))
                Call sBuilder.AppendLine()

                maxLength = (From item In _RuntimeEnvironment._EngineMemoryDevice Select Len(item.Key)).ToArray.Max
                LQuery = (From item In _RuntimeEnvironment._EngineMemoryDevice Select String.Format("{0} {1}--> {2}  //{3}", item.Key, New String(" "c, maxLength - Len(item.Key) + 2), item.Value.ToString, item.Value.GetType.FullName)).ToArray

                For Each Line As String In LQuery
                    Call sBuilder.AppendLine(Line)
                Next
            End If

            If Not Constants.IsNullOrEmpty Then
                maxLength = (From item In Constants Select Len(item.Key)).ToArray.Max
                LQuery = (From item In Constants Select String.Format("{0} {1}--> {2}  //{3}", item.Key, New String(" "c, maxLength - Len(item.Key) + 2), item.Value.ToString, item.Value.GetType.FullName)).ToArray
                Call sBuilder.AppendLine()
                Call sBuilder.AppendLine(String.Format("    {0} CONSTANTS", Constants.Count))

                For Each line In LQuery
                    Call sBuilder.AppendLine(line)
                Next
            End If

            Call Console.WriteLine(sBuilder.ToString)
            Return LQuery
        End Function

        ''' <summary>
        ''' 切换ShoalShell的当前工作目录
        ''' </summary>
        ''' <param name="to"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <Command("cd", info:="Value ""~"" for change the directory to SHOAL_HOME")>
        Public Function Cd([to] As String) As String
            If String.Equals([to], "~") Then
                [to] = My.Application.Info.DirectoryPath
            End If

            My.Computer.FileSystem.CurrentDirectory = [to]
            Call ScriptEngine.InternalEntryPointManager.InternalScanTempShell(dir:=My.Computer.FileSystem.CurrentDirectory, disp_inits_message:=True)
            Return [to]
        End Function

        ''' <summary>
        ''' 当指定了文件拓展名之后，函数只会返回文件名列表，其他的情况会返回文件名列表和文件夹列表
        ''' </summary>
        ''' <param name="argv"></param>
        ''' <returns></returns>
        <Command("ls", Info:="using -ext parameter to specific the parameter, -d to specific the target directory, empty value for the current directory.")>
        Public Function List(Optional argv As CommandLine.CommandLine = Nothing) As String()
            Dim ext As String = If(argv Is Nothing OrElse argv.IsNullOrEmpty OrElse String.IsNullOrEmpty(argv("-ext")), "", argv("-ext"))
            Dim TargetDir As String = If(argv Is Nothing OrElse argv.IsNullOrEmpty OrElse Not FileIO.FileSystem.DirectoryExists(argv.Parameters.First), "", argv.Parameters.First)

            If String.IsNullOrEmpty(TargetDir) Then
                TargetDir = If(argv Is Nothing OrElse argv.IsNullOrEmpty OrElse String.IsNullOrEmpty(argv("-d")), My.Computer.FileSystem.CurrentDirectory, argv("-d"))
            End If

            If String.IsNullOrEmpty(ext) Then
                If argv Is Nothing OrElse argv.Parameters.IsNullOrEmpty Then
                    ext = "*.*"
                Else
                    ext = If(FileIO.FileSystem.DirectoryExists(argv.Parameters.First), "*.*", argv.Parameters.First)
                End If
            End If

            Dim Dirs = FileIO.FileSystem.GetDirectories(TargetDir, FileIO.SearchOption.SearchTopLevelOnly)
            Dim Files = FileIO.FileSystem.GetFiles(TargetDir, FileIO.SearchOption.SearchTopLevelOnly, ext)
            Dim DriveInfo = FileIO.FileSystem.GetDriveInfo(TargetDir)
            Dim sBuilder As StringBuilder = New StringBuilder(1024)

            Call sBuilder.AppendLine(String.Format(" Volume in drive {0} is {1}", DriveInfo.RootDirectory, DriveInfo.VolumeLabel))
            Call sBuilder.AppendLine(String.Format(" Volume drive Format is {0}" & vbCrLf, DriveInfo.DriveFormat))
            Call sBuilder.AppendLine(String.Format(" Directory of {0}" & vbCrLf, TargetDir))
            If Not Dirs.IsNullOrEmpty Then Call sBuilder.AppendLine(String.Format("  {0} Directories", Dirs.Count))
            If Not Files.IsNullOrEmpty Then Call sBuilder.AppendLine(String.Format("  {0} Files", Files.Count))
            Call sBuilder.AppendLine()

            For Each Dir As String In Dirs
                Dim dirInfo = FileIO.FileSystem.GetDirectoryInfo(Dir)
                Call sBuilder.AppendLine(String.Format("{0} <DIR>  {1}", dirInfo.LastWriteTimeUtc.ToStringEx, dirInfo.Name))
            Next
            Call sBuilder.AppendLine()
            For Each File As String In Files
                Dim fileInfo = FileIO.FileSystem.GetFileInfo(File)

                If String.Equals(fileInfo.Extension, ".shl") Then
                    Call sBuilder.AppendLine(String.Format("{0} <SHL>  {1}", fileInfo.LastWriteTimeUtc.ToStringEx, fileInfo.Name))
                Else
                    Call sBuilder.AppendLine(String.Format("{0}        {1}", fileInfo.LastWriteTimeUtc.ToStringEx, fileInfo.Name))
                End If
            Next
            Call Console.WriteLine(sBuilder.ToString)

            If String.Equals(ext, "*.*") Then
                '返回所有
                Dim ChunkBuffer = Dirs.ToArray.Join(Files)
                Return ChunkBuffer.ToArray
            Else
                Return Files.ToArray
            End If
        End Function

        <Command("system", info:="Folk a process from the specific command line arguments of this function.")>
        Public Function ProcessStart(argv As CommandLine.CommandLine) As Process
            Dim file As String = FileIO.FileSystem.GetFileInfo(argv("-file")).FullName, arguments As String = argv("-argv")
            Dim Process As Process = New Process()
            Process.StartInfo = New System.Diagnostics.ProcessStartInfo(file)
            If Not String.IsNullOrEmpty(arguments) Then
                Process.StartInfo.Arguments = arguments
            End If
            Call Process.Start()
            Return Process
        End Function

        ''' <summary>
        ''' 脚本与脚本之间的参数传递是通过共享内存变量来实现的
        ''' </summary>
        ''' <param name="argv"></param>
        ''' <returns>返回目标脚本执行过后的Memory对象</returns>
        ''' <remarks></remarks>
        <Command("source", info:="Call the external script file in the shoal shell scripting, you can get the returns value from the 'Return' command.",
            usage:="source <script_file> [argumentsName argumentValue]")>
        Public Function Source(argv As CommandLine.CommandLine) As Runtime.Objects.I_MemoryManagementDevice
            '  Dim CallerStack = _ShellScriptHost

            Dim ScriptFile As String = FileIO.FileSystem.GetFileInfo(argv.Parameters.First).FullName
            Dim sourceArgv As String() = argv.Parameters.Skip(1).ToArray

            If Not sourceArgv.IsNullOrEmpty Then
                For Each item In CommandLine.CommandLine.CreateParameterValues(sourceArgv, True)
                    Call ScriptEngine._EngineMemoryDevice.InsertOrUpdate(item.Key, ScriptEngine._EngineMemoryDevice.TryGetValue(item.Value))
                Next
            End If

            Dim currentWork As String = My.Computer.FileSystem.CurrentDirectory
            My.Computer.FileSystem.CurrentDirectory = FileIO.FileSystem.GetParentPath(ScriptFile)
            Call ScriptEngine.EXEC(FileIO.FileSystem.ReadAllText(ScriptFile))
            My.Computer.FileSystem.CurrentDirectory = currentWork
            '    CommandsEntryHandler._ShellScriptHost = CallerStack

            Return Me._RuntimeEnvironment._EngineMemoryDevice
        End Function
    End Class
End Namespace