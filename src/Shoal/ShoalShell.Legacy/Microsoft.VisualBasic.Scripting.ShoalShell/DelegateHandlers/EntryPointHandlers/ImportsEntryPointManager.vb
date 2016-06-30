Imports Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports System.Collections.ObjectModel
Imports Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints

Namespace DelegateHandlers.EntryPointHandlers

    Public Class ImportsEntryPointManager : Inherits ShoalShell.Runtime.Objects.ObjectModels.IScriptEngineComponent

        Implements Generic.IReadOnlyCollection(Of ShoalShell.DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint)
        Implements Generic.IReadOnlyDictionary(Of String, DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint)

        Dim _InternalImportsEntryPointHash As EntryPointHashTable = New EntryPointHashTable

        ''' <summary>
        ''' 与<see cref="_InternalImportsEntryPointHash"></see>所不同的是，这个存储的是Delegate，里面的命令会优先于<see cref="_InternalImportsEntryPointHash"></see>中的命令被调用
        ''' </summary>
        ''' <remarks></remarks>
        Dim _InternalDelegateHash As EntryPointHashTable = New EntryPointHashTable

        ''' <summary>
        ''' 当切换工作目录的时候，程序会自动扫描当前工作目录之下的所有可用的脚本文件，使用文件名作为临时的函数名
        ''' </summary>
        ''' <remarks></remarks>
        Dim _TEMP_HANDLERS As Dictionary(Of String, String) = New Dictionary(Of String, String)

        ''' <summary>
        ''' 请注意，这里面的所有的对象的键值都是小写字母的
        ''' </summary>
        ''' <remarks></remarks>
        Dim _HybridScriptingDelegates As Dictionary(Of String, String) = New Dictionary(Of String, String)

        ''' <summary>
        ''' 当前的工作目录之下的脚本文件都会被当作为临时的脚本命令来使用
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TempShellCommands As ReadOnlyDictionary(Of String, String)
            Get
                Return New ReadOnlyDictionary(Of String, String)(_TEMP_HANDLERS)
            End Get
        End Property

        Public ReadOnly Property ImportsCommandEntryPoints As Dictionary(Of String, DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint)
            Get
                Dim LQuery = (From item In _InternalImportsEntryPointHash.InternalHashDictionary Select cmdName = _InternalImportsEntryPointHash.InternalKeys(item.Key), item.Value).ToArray
                Return LQuery.ToDictionary(keySelector:=Function(item) item.cmdName, elementSelector:=Function(item) item.Value)
            End Get
        End Property

        Sub New(ScriptEngine As ShoalShell.Runtime.Objects.ShellScript, Dir As String, Disp_Inits_Message As Boolean)
            Call MyBase.New(ScriptEngine)
            Me._InternalEntryPointLoader = New InternalEntryPointLoader(Me)
            Call InternalScanTempShell(Dir, Disp_Inits_Message)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="name">大小写不敏感</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetHybridsScript(name As String) As String
            name = name.ToLower
            Return _HybridScriptingDelegates(name)
        End Function

        ''' <summary>
        ''' 旧的脚本会被新的脚本所替换
        ''' </summary>
        ''' <param name="name">大小写不敏感</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function DeclaresHybridScripting(Name As String, script As String) As Object
            Name = Name.ToLower
            If _HybridScriptingDelegates.ContainsKey(Name) Then
                Call _HybridScriptingDelegates.Remove(Name)
            End If

            Call _HybridScriptingDelegates.Add(Name, script)
            Return 0
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="dir"></param>
        ''' <param name="disp_inits_message">假若这个命令是在初始化的时候调用的话，则使用这个参数来根据配置数据来决定是否显示程序的初始化消息</param>
        ''' <remarks></remarks>
        Friend Sub InternalScanTempShell(dir As String, disp_inits_message As Boolean)
            Dim ScriptFiles = FileIO.FileSystem.GetFiles(dir, FileIO.SearchOption.SearchTopLevelOnly, "*.shl")

            Call Me._TEMP_HANDLERS.Clear()

            For Each file In ScriptFiles
                Dim FileName As String = FileIO.FileSystem.GetFileInfo(file).Name
                FileName = Mid(FileName, 1, Len(FileName) - 4).ToLower
                Call Me._TEMP_HANDLERS.Add(FileName, file)
            Next

            If disp_inits_message AndAlso Not ScriptFiles.IsNullOrEmpty Then
                Call Console.WriteLine("There are {0} command(s) avaliable in the current work directory ""{1}"".", ScriptFiles.Count, My.Computer.FileSystem.CurrentDirectory)
            End If
        End Sub

        ''' <summary>
        ''' 请使用这个方法导入实例对象之中的定义的命令
        ''' </summary>
        ''' <param name="InvokedObject"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ImportsInstanceNamespace(InvokedObject As Object) As Integer
            Dim Commands = (From EntryPoint As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo
                            In InternalAllInstanceCommands(InvokedObject.GetType)
                            Select EntryPoint.InvokeSet(Sub() EntryPoint.InvokeOnObject = InvokedObject)).ToList '解析出命令并连接目标实例对象与函数的执行入口点
            Return InternalMountEntryPoint(EntryPointList:=Commands)
        End Function

        Protected Function InternalAllInstanceCommands(Type As Type) As List(Of Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo)
            Dim InternalChunkList = Microsoft.VisualBasic.CommandLine.Interpreter.GetAllCommands(Type)
            Dim commandAttribute As System.Type = GetType(CommandAttribute)
            Dim commandsSource = (From MethodHandle As System.Reflection.MethodInfo
                                  In Type.GetMethods()
                                  Select Entry = MethodHandle.GetCustomAttributes(commandAttribute, True), MethodInfo = MethodHandle).ToArray
            Dim commandsInfo = (From methodInfo In commandsSource
                                Where Not methodInfo.Entry.IsNullOrEmpty
                                Let commandInfo = New CommandEntryPointInfo(TryCast(methodInfo.Entry.First, CommandAttribute), methodInfo.MethodInfo)
                                Select commandInfo
                                Order By commandInfo.Name Ascending).ToArray
            Call InternalChunkList.AddRange(commandsInfo)

            Return InternalChunkList
        End Function

        ''' <summary>
        ''' 使用本方法导入外部命令，这样子就可以直接调用方法而不需要每一个命令行都添加模块名称了(这个方法导入的是共享方法，对于实例方法不能够通过本方法进行导入)
        ''' </summary>
        ''' <param name="Namespace"></param>
        ''' <remarks></remarks>
        ''' 
        Public Function ImportsNamespace([Namespace] As String) As Integer
            If _RuntimeEnvironment._ImportsNamespace.IndexOf([Namespace].ToLower) > -1 Then
                Return 0  '已经将命名空间导入了，则跳过这个操作
            End If

            Dim Modules As KeyValuePair(Of System.Type, CommandLine.Reflection.Namespace)()
            Dim LoadedModule = _RuntimeEnvironment._Interpreter._DelegateRegistry.LoadedModules.get_LoadedModule([Namespace])

            If Not LoadedModule Is Nothing Then
                [Modules] = LoadedModule._InternalOriginalAssemblys.ToArray
            Else
                Dim assemblyPaths As String() = _RuntimeEnvironment._Interpreter._DelegateRegistry.GetAssemblyPaths([Namespace])

                Modules = (From item As KeyValuePair(Of CommandLine.Reflection.Namespace, Type)
                           In (From path As String
                               In assemblyPaths
                               Select _RuntimeEnvironment._Interpreter._DelegateRegistry._RegisteredModuleLoader.get_ModuleFromAssembly(path)).ToArray.MatrixToVector
                           Select New KeyValuePair(Of Type, CommandLine.Reflection.Namespace)(item.Value, item.Key)).ToArray
                Modules = (From [mod] In Modules Where String.Equals([mod].Value.Namespace, [Namespace], StringComparison.OrdinalIgnoreCase) Select [mod]).ToArray
            End If

            Call Console.WriteLine("<Imports Namespace> '{0}'", [Namespace])

            Return ImportsModule(Modules)
        End Function

        Public Function ImportsModule([Module] As Type) As Integer
            Dim Modules = RegistryModuleLoader.GetModule([Module])
            Call Console.WriteLine("<Imports Namespace> '{0}'", Modules.First.Value.Namespace)
            Return ImportsModule(Modules)
        End Function

        Public Function ImportsModule([Modules] As KeyValuePair(Of System.Type, Microsoft.VisualBasic.CommandLine.Reflection.[Namespace])()) As Integer
            Dim ModuleItem As ShoalShell.DelegateHandlers.TypeLibraryRegistry.RegistryNodes.Module = New RegistryNodes.Module(Modules.First.Value, Modules.First.Key)
            Dim i As Integer

            For Each [Module] In Modules

1:              Call _RuntimeEnvironment._ImportsNamespace.Add([Module].Value.Namespace.ToLower)
                Call _RuntimeEnvironment._EngineMemoryDevice.ImportsConstant([Module].Key) '导入常量
                Call _RuntimeEnvironment.ImportsIOSupports([Module].Key)

                i += InternalMountEntryPoint(EntryPointList:=CommandLine.Interpreter.GetAllCommands([Module].Key))
            Next

            For Each Item As KeyValuePair(Of Type, Microsoft.VisualBasic.CommandLine.Reflection.Namespace) In Modules.Skip(1)
                Call ModuleItem.MergeNamespace(Item.Value, Item.Key)
            Next

            Call _RuntimeEnvironment.ImportsOutputHandlers(ModuleItem)
            Call _RuntimeEnvironment.ImportsNewNamespace()

            Return i
        End Function

        ''' <summary>
        ''' 从命名空间之中导入方法
        ''' </summary>
        ''' <param name="EntryPointList"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InternalMountEntryPoint(EntryPointList As List(Of CommandLine.Reflection.EntryPoints.CommandEntryPointInfo)) As Integer
            Dim i As Integer

            For Each MethodEntryPoint In EntryPointList
                i += InternalMountEntryPoint(EntryPointInfo:=MethodEntryPoint, Hash:=Me._InternalImportsEntryPointHash)
            Next

            Return i
        End Function

        Private Shared Function InternalMountEntryPoint(EntryPointInfo As CommandLine.Reflection.EntryPoints.CommandEntryPointInfo, Hash As EntryPointHashTable) As Integer
            Dim EntryPoint As DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint
            Dim CommandName As String = EntryPointInfo.Name.ToLower

            If Hash.InternalHashDictionary.ContainsKey(CommandName) Then
                EntryPoint = Hash(CommandName) '具有重载函数
            Else
                EntryPoint = New CommandMethodEntryPoint(CommandName, InitEntryPoint:=Nothing)      '没有重载函数
                Call Hash.InternalAddEntryPoint(CommandName, EntryPoint)
            End If

            '向入口点之中添加新的执行入口点
            Try
                Call EntryPoint.HashAddMethodEntryPoint(EntryPointInfo)
            Catch ex As Exception
#Const DEBUG = 0
#If DEBUG Then
                Call EntryPoint.HashAddMethodEntryPoint(EntryPointInfo)
#End If
                Dim exMessage As String = EntryPointInfo.EntryPointFullName & vbCrLf & vbCrLf & ex.ToString
                Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.InterpreterException(exMessage)
            End Try

            Return 1
        End Function

        ''' <summary>
        ''' 这个函数是为了Delegate和混合编程的Delegate所准备的
        ''' </summary>
        ''' <param name="EntryPoint"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InternalMountEntryPoint(EntryPoint As System.Reflection.MethodInfo) As Boolean
            Dim InfoData As CommandLine.Reflection.CommandAttribute = New CommandAttribute(EntryPoint.Name)
            Dim EntryPointInfo As CommandLine.Reflection.EntryPoints.CommandEntryPointInfo = New CommandEntryPointInfo(InfoData, EntryPoint)
            Return InternalMountEntryPoint(EntryPointInfo, Me._InternalDelegateHash) > 0
        End Function

        ''' <summary>
        ''' Delegate也可以与导入的函数产生重载
        ''' </summary>
        ''' <param name="Delegate">Delegate函数指针</param>
        ''' <remarks></remarks>
        Public Sub DeclaresDelegate([Delegate] As ShoalShell.Interpreter.Reflection.Delegate)
            Call InternalMountEntryPoint(EntryPoint:=[Delegate])
        End Sub

#Region "Implements Generic.IReadOnlyCollection(Of ShoalShell.DelegateHandlers.EntryPointHandlers.CommandMethod)"

        Public Iterator Function GetEnumerator() As IEnumerator(Of CommandMethodEntryPoint) Implements IEnumerable(Of CommandMethodEntryPoint).GetEnumerator
            For Each ItemObject In _InternalImportsEntryPointHash.InternalHashDictionary
                Yield ItemObject.Value
            Next
        End Function

        Public ReadOnly Property Count As Integer Implements IReadOnlyCollection(Of CommandMethodEntryPoint).Count, IReadOnlyCollection(Of KeyValuePair(Of String, CommandMethodEntryPoint)).Count
            Get
                Return _InternalImportsEntryPointHash.InternalHashDictionary.Count
            End Get
        End Property

        Public Iterator Function GetEnumerator1() As IEnumerator Implements IEnumerable.GetEnumerator
            Yield GetEnumerator()
        End Function
#End Region

#Region "Implements Generic.IReadOnlyDictionary(Of String, DelegateHandlers.EntryPointHandlers.CommandMethod)"

        Public Function GetEnumerator2() As IEnumerator(Of KeyValuePair(Of String, CommandMethodEntryPoint)) Implements IEnumerable(Of KeyValuePair(Of String, CommandMethodEntryPoint)).GetEnumerator
            Return Me._InternalImportsEntryPointHash.InternalHashDictionary.GetEnumerator
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="CommandName">对大小写不敏感</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExistsCommand(CommandName As String) As Boolean Implements IReadOnlyDictionary(Of String, CommandMethodEntryPoint).ContainsKey
            CommandName = CommandName.ToLower
            Return _TEMP_HANDLERS.ContainsKey(CommandName) OrElse Me._InternalDelegateHash.InternalHashDictionary.ContainsKey(CommandName) OrElse Me._InternalImportsEntryPointHash.InternalHashDictionary.ContainsKey(CommandName)
        End Function

        ''' <summary>
        ''' 大小写不敏感
        ''' </summary>
        ''' <param name="key"></param>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Default Public ReadOnly Property EntryPoint(key As String) As CommandMethodEntryPoint Implements IReadOnlyDictionary(Of String, CommandMethodEntryPoint).Item
            Get
                Dim value As CommandMethodEntryPoint = Nothing
                Call TryGetValue(key, value)
                Return value
            End Get
        End Property

        ''' <summary>
        ''' 获取所有已经导入进来的方法的名称
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ImportsMethods As IEnumerable(Of String) Implements IReadOnlyDictionary(Of String, CommandMethodEntryPoint).Keys
            Get
                Return Me._InternalImportsEntryPointHash.InternalKeys.Values
            End Get
        End Property

        ''' <summary>
        ''' 大小写不敏感，所以无需在进行ToLower的额外处理，临时脚本命令会优先于内部命令被调用
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function TryGetValue(key As String, ByRef value As CommandMethodEntryPoint) As Boolean Implements IReadOnlyDictionary(Of String, CommandMethodEntryPoint).TryGetValue
            key = key.ToLower
            value = _InternalImportsEntryPointHash(key)
            Return True
        End Function

        Private Function __getTempShellCommand(Command As CommandLine.CommandLine)
            Dim tmp_command = Me._TEMP_HANDLERS(Command.Name.ToLower)  '获取临时的脚本路径
            Dim tmp_tokens As String() = {New String() {"source", tmp_command}, Command.Parameters}.MatrixToVector
            Command = Microsoft.VisualBasic.CommandLine.CommandLine.Join(tmp_tokens)
            Return Function() Me.ScriptEngine.Interpreter.InternalCommands.Source(Command)
        End Function

        Public ReadOnly Property Values As IEnumerable(Of CommandMethodEntryPoint) Implements IReadOnlyDictionary(Of String, CommandMethodEntryPoint).Values
            Get
                Return _InternalImportsEntryPointHash.InternalHashDictionary.Values
            End Get
        End Property
#End Region
    End Class
End Namespace