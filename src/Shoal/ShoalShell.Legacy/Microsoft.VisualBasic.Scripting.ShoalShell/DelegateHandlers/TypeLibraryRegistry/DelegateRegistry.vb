Imports System.Text
Imports Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry.RegistryNodes

Namespace DelegateHandlers.TypeLibraryRegistry

    Public Class LoadedModulesManager

        Protected _LoadedModules As Dictionary(Of String, [Module]) = New Dictionary(Of String, [Module])

        Public Sub Add(ns As CommandLine.Reflection.Namespace, [module] As Type)
            Dim Nspace = get_LoadedModule(ns.Namespace)
            If Nspace Is Nothing Then
                Nspace = New [Module](ns, [module])
                Call _LoadedModules.Add(ns.Namespace.ToLower, Nspace)
            Else
                Call Nspace.MergeNamespace(ns, [module])
            End If
        End Sub

        Public Function ModuleIsLoaded(ns As String) As Boolean
            Return Not get_LoadedModule(ns) Is Nothing
        End Function

        Public Function get_LoadedModule(ns As String) As [Module]
            Dim LQuery = (From item In _LoadedModules Where String.Equals(ns, item.Key, StringComparison.OrdinalIgnoreCase) Select item.Value).ToArray
            Return LQuery.FirstOrDefault
        End Function
    End Class

    ''' <summary>
    ''' The type registry of the external plugin module assembly, the assembly file should be a standard.NET DLL or exe which was written by VisualBasic or C#.
    ''' (外部命令必须通过这个类型注册表才可以被用户调用)
    ''' </summary>
    ''' <remarks></remarks>
    <Xml.Serialization.XmlRoot("ShoalShell.ExternalModuleRegistry", namespace:="Microsoft.VisualBasic.ShoalShell.ExternalModuleRegistry")>
    Public Class DelegateRegistry : Implements System.IDisposable

        Protected Friend _RegisteredModuleLoader As RegistryModuleLoader
        Protected Friend _FilePath As String
        Protected Friend _InnerList As List(Of RegistryNodes.Namespace) = New Generic.List(Of [Namespace])
        ''' <summary>
        ''' 这个挂载点列表是全部被一次性加载的
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend _HybridsScriptingEntrypoints As List(Of RegistryNodes.HybridScriptingModuleLoadEntry) = New Generic.List(Of RegistryNodes.HybridScriptingModuleLoadEntry)
        Protected Friend ReadOnly _LoadedModules As LoadedModulesManager = New LoadedModulesManager

        <Xml.Serialization.XmlAttribute> Public Property CurrentRegister As String

        Public ReadOnly Property LoadedModules As LoadedModulesManager
            Get
                Return _LoadedModules
            End Get
        End Property

        Sub New()
            CurrentRegister = My.Computer.Name
        End Sub

        ''' <summary>
        ''' This property list all of the module which was registered in the shoal shell.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RegisteredModules As RegistryNodes.Namespace()
            Get
                If _InnerList.IsNullOrEmpty Then
                    _InnerList = New Generic.List(Of [Namespace])
                End If
                Return _InnerList.ToArray
            End Get
            Set(value As RegistryNodes.Namespace())
                _InnerList = value.ToList
            End Set
        End Property

        ''' <summary>
        ''' 与外部脚本环境进行交互所需要的挂载点的注册表
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property HybridsScriptingEntrypoints As RegistryNodes.HybridScriptingModuleLoadEntry()
            Get
                Return _HybridsScriptingEntrypoints.ToArray
            End Get
            Set(value As RegistryNodes.HybridScriptingModuleLoadEntry())
                If value.IsNullOrEmpty Then
                    _HybridsScriptingEntrypoints = New Generic.List(Of HybridScriptingModuleLoadEntry)
                Else
                    _HybridsScriptingEntrypoints = value.ToList
                End If
            End Set
        End Property

        Public ReadOnly Property IsEmpty As Boolean
            Get
                Return Me.HybridsScriptingEntrypoints.IsNullOrEmpty AndAlso Me.RegisteredModules.IsNullOrEmpty
            End Get
        End Property

        Public Function GetMethod(assemblyName As String, cmdName As String, hostMemoryDevice As Runtime.Objects.I_MemoryManagementDevice) As System.Reflection.MethodInfo()
            Dim [Module] = TryGetModule(assemblyName.ToLower, hostMemoryDevice)
            Dim Method = [Module].Item(cmdName)
            Return Method
        End Function

        ''' <summary>
        ''' 返回错误的插件模块的数目
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckForLibraryConsists(showMessage As Boolean) As Integer
            If showMessage Then
                Call Console.WriteLine("Checking for the missing plugins....")
            End If

            Dim LQuery = (From nsEntry As RegistryNodes.Namespace
                          In Me.RegisteredModules
                          Let modPathList As String() = (From mldEntry As DelegateHandlers.TypeLibraryRegistry.RegistryNodes.ModuleLoadEntry
                                                     In nsEntry.Entries
                                                         Let path As String = Internal_getFullPath(mldEntry.AssemblyPath)
                                                         Where Not FileIO.FileSystem.FileExists(path)
                                                         Select path).ToArray
                          Where Not modPathList.IsNullOrEmpty
                          Select nsID = nsEntry.ModuleName, modPathList).ToList
            Call LQuery.AddRange((From hyLdEntry As DelegateHandlers.TypeLibraryRegistry.RegistryNodes.HybridScriptingModuleLoadEntry
                                  In Me.HybridsScriptingEntrypoints
                                  Let path As String = Internal_getFullPath(hyLdEntry.AssemblyPath)
                                  Where Not FileIO.FileSystem.FileExists(path)
                                  Select nsID = hyLdEntry.EntryId, modPathList = New String() {path}).ToArray)

            If LQuery.IsNullOrEmpty Then
                If showMessage Then
                    Call Console.WriteLine("None missing plugins, all good to go!")
                End If

                Return 0
            End If

            If Not showMessage Then
                Return LQuery.Count
            End If

            Dim sBuilder As StringBuilder = New StringBuilder(2048)

            For Each Entry In LQuery
                For Each path In Entry.modPathList
                    Call sBuilder.AppendLine($"[DEBUG]  Plugin module is missing ({Entry.nsID})   {path.ToFileURL}!!!")
                Next
            Next

            Call Console.WriteLine(sBuilder.ToString)
            Call Console.WriteLine($"There are {LQuery.Count } plugin modules was missing, if you have move the location of the plugins module show upon, please registered again...")
            Return LQuery.Count
        End Function

        Const TYPE_ACCESS_EXCEPTION As String = "ASSEMBLY_MODULE_LOAD_FAILURE:: Could not load any module from a path {0}, the assembly module file maybe deleted or not a standard .NET Class library!"

        ''' <summary>
        ''' 挂载一个模块
        ''' </summary>
        ''' <param name="assemblyPath"></param>
        ''' <remarks></remarks>
        Public Sub LoadLibrary(assemblyPath As String, hostMemoryDevice As Runtime.Objects.I_MemoryManagementDevice)
            Dim [Modules] = _RegisteredModuleLoader.get_ModuleFromAssembly(AssemblyPath:=FileIO.FileSystem.GetFileInfo(assemblyPath).FullName)

EXCEPTION_HANDLER:
            If [Modules].IsNullOrEmpty Then
                Throw New TypeAccessException(String.Format(TYPE_ACCESS_EXCEPTION, assemblyPath))
            Else
                For Each [Module] In Modules

                    If Not String.IsNullOrEmpty([Module].Key.Namespace) Then
                        Call Console.WriteLine("Load registered plugin module '{0}';  @""{1}""", [Module].Key.Namespace, assemblyPath)
                        Call _LoadedModules.Add([Module].Key, [Module].Value) '动态挂载模块
                        Call hostMemoryDevice.ImportsConstant([Module].Value)  '导入模块之中所定义的常数
                    End If
                Next
            End If
        End Sub

        Public Function TryGetModule(assemblyName As String, hostMemoryDevice As Runtime.Objects.I_MemoryManagementDevice) As [Module]
            If _LoadedModules.ModuleIsLoaded(assemblyName) Then Return _LoadedModules.get_LoadedModule(assemblyName)

            Dim AssemblyPathList As String() = GetAssemblyPaths(assemblyName)
            Dim [Modules] = (From path As String In AssemblyPathList Select _RegisteredModuleLoader.get_ModuleFromAssembly(path)).ToArray.MatrixToVector

EXCEPTION_HANDLER:
            If [Modules].IsNullOrEmpty Then
                Throw New TypeAccessException(String.Format("ASSEMBLY_MODULE_LOAD_FAILURE:: Could not load one of the namespace module ""{0}""!" & vbCrLf &
                                                            "    Path List:" & vbCrLf &
                                                            "         {1}", assemblyName, String.Join(";" & vbCrLf & "       ", AssemblyPathList)))
            End If

            For Each [Module] In Modules
                If Not String.IsNullOrEmpty([Module].Key.Namespace) Then
                    Call Console.WriteLine("Load registered plugin module '{0}';  @""{1}""", [Module].Value.FullName, [Module].Value.Assembly.Location)
                    Call _LoadedModules.Add([Module].Key, [Module].Value) '动态挂载模块
                    Call hostMemoryDevice.ImportsConstant([Module].Value)  '导入模块之中所定义的常数
                End If
            Next

            Dim LoadModule = LoadedModules.get_LoadedModule(assemblyName)
            If LoadModule.IsNullOrEmpty Then
                Modules = Nothing
                GoTo EXCEPTION_HANDLER
            Else
                Return LoadModule
            End If
        End Function

        Protected Friend Function GetAssemblyPaths(AssemblyName As String) As String()
            Dim [Module] As RegistryNodes.Namespace = GetNamespaceModule(AssemblyName)
            Dim pathList As String() = (From Entry As RegistryNodes.ModuleLoadEntry In [Module].Entries Select Internal_getFullPath(Entry.AssemblyPath) Distinct).ToArray
            Return pathList
        End Function

        ''' <summary>
        ''' 获取目标路径的全路径，这个函数要处理相对路径和全路径
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Friend Shared Function Internal_getFullPath(path As String) As String
            Dim FullPath As String = FileIO.FileSystem.GetFileInfo(path).FullName

            If Not FileIO.FileSystem.FileExists(FullPath) Then
                FullPath = My.Application.Info.DirectoryPath & "/" & path  '可能为相对路径

                If Not FileIO.FileSystem.FileExists(FullPath) Then
                    Return path
                Else
                    Return FullPath
                End If
            Else
                Return FullPath
            End If
        End Function

        Public Sub Save()
            Dim Xml As String = Me.GetXml
            Call Xml.SaveTo(_FilePath)
        End Sub

        Const MISSING_ASSEMBLY_TYPE As String = "MISSING_ASSEMBLY_TYPE:: Assembly module {0} is not found in the registry!"

        Public Function GetNamespaceModule(assemblyName As String) As RegistryNodes.Namespace
            Dim [Modules] As RegistryNodes.Namespace() = (From nsEntry As RegistryNodes.Namespace
                                                          In RegisteredModules
                                                          Where String.Equals(assemblyName, nsEntry.ModuleName, StringComparison.OrdinalIgnoreCase)
                                                          Select nsEntry).ToArray
            If Modules.IsNullOrEmpty Then
                Dim ex_Msg As String = String.Format(MISSING_ASSEMBLY_TYPE, assemblyName)
                Throw New MissingPrimaryKeyException(ex_Msg)
            Else
                Return Modules.First
            End If
        End Function

        Private Shared Sub InsertOrUpdateItem(Of EntryType As RegistryNodes.ModuleLoadEntry)(RegistryItem As EntryType, ByRef Registry As List(Of EntryType))
            Dim Items = (From item As EntryType In Registry Where item.Equals(Of EntryType)(RegistryItem) Select item).ToArray

            If Items.IsNullOrEmpty Then
                Call Console.WriteLine("Register new assembly module:  {0}  ""{1}"" " & vbCrLf &
                                       "   @{0}", RegistryItem.TypeId, RegistryItem.Guid, RegistryItem.AssemblyPath)
                Call Registry.Add(RegistryItem)  'INSERT
            Else
                Dim Entry = Items.First
                Call Entry.CopyFrom(RegistryItem)    'UPDATE
                Call Console.WriteLine("Update assembly module registry entry: {0}  ""{1}""" & vbCrLf &
                                       "   @{0}", RegistryItem.AssemblyPath, RegistryItem.TypeId)
            End If
        End Sub

        Private Shared Function RegistryInternalInsertOrUpdateRegistryEntryValue(nsName As String, Registry As List(Of RegistryNodes.Namespace), RegistryItem As ModuleLoadEntry) As RegistryNodes.Namespace
            Dim NamespaceEntries As RegistryNodes.Namespace() = (From nsEntry In Registry Where String.Equals(nsName, nsEntry.ModuleName, StringComparison.OrdinalIgnoreCase) Select nsEntry).ToArray
            Dim RegistryEntry As RegistryNodes.Namespace

            If NamespaceEntries.IsNullOrEmpty Then
                RegistryEntry = New RegistryNodes.Namespace With {.ModuleName = nsName, .Entries = New List(Of RegistryNodes.ModuleLoadEntry)}

                Call Console.WriteLine("Register new module namespace: {0}", nsName)
                Call Registry.Add(RegistryEntry)  'INSERT
                Call InsertOrUpdateItem(Of RegistryNodes.ModuleLoadEntry)(RegistryItem, RegistryEntry.Entries)
            Else
                RegistryEntry = NamespaceEntries.First    'UPDATE

                Call Console.WriteLine("Update assembly module registry entry: {0}  @{1}", nsName, RegistryItem.AssemblyPath)
                Call InsertOrUpdateItem(Of RegistryNodes.ModuleLoadEntry)(RegistryItem, RegistryEntry.Entries)
            End If

            Return RegistryEntry
        End Function

        ''' <summary>
        ''' Register the external assembly module entry points for dynamic load.(注册动态调用的外部模块)
        ''' </summary>
        ''' <param name="AssemblyPath"></param>
        ''' <param name="AssemblyName">一个Dll文件之内可能会定义多个模块，当仅定义一个模块的时候，则本参数会覆盖掉模块内的名称定义，当定义有多个模块的时候，则本参数不会起任何作用，假若留空，则使用模块之中的默认值</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RegisterAssemblyModule(AssemblyPath As String, AssemblyName As String) As Boolean
            Dim FileInfo = FileIO.FileSystem.GetFileInfo(AssemblyPath)
            Dim Modules = Me._RegisteredModuleLoader.get_ModuleFromAssembly(FileInfo.FullName)

            If Modules.IsNullOrEmpty Then
                Return False
            End If

            Dim EntryPoints = InternalGetExternalInteropEntryPoints(AssemblyPath)
            Dim c = Modules.First.Value.Assembly.CustomAttributes
            Dim Company As String = (From item In c Where item.AttributeType = GetType(System.Reflection.AssemblyCompanyAttribute) Let value = item.ConstructorArguments.First.Value.ToString Select value).First
            Dim Version As String = (From item In c Where item.AttributeType = GetType(System.Reflection.AssemblyFileVersionAttribute) Let value = item.ConstructorArguments.First.Value.ToString Select value).First
            Dim FrameworkVersion As String = (From item In c Where item.AttributeType = GetType(System.Runtime.Versioning.TargetFrameworkAttribute) Let value = item.ToString Select value).First
            Dim Updates As Long = FileInfo.LastWriteTime.ToBinary

            If Not EntryPoints.IsNullOrEmpty Then  '注册脚本的交互挂载点

                For Each Line In EntryPoints
                    Dim RegistryItem = New RegistryNodes.HybridScriptingModuleLoadEntry With
                                       {
                                           .Namespace = Line.Key.ScriptName,
                                           .TypeId = Line.Value.FullName,
                                           .EntryId = Line.Key.ScriptName,
                                           .AssemblyPath = Internal_getRelativePath(AssemblyPath),
                                           .Guid = Line.Value.GUID.ToString,
                                           .Company = Company,
                                           .Version = Version,
                                           .FrameworkVersion = FrameworkVersion,
                                           .UpdateTime = Updates}
                    Call InsertOrUpdateItem(RegistryItem, Registry:=Me._HybridsScriptingEntrypoints)
                Next
            End If

            If Modules.IsNullOrEmpty Then
                Return False
            End If

            Dim UpdateOrRegisterModule = Sub(ModuleToRegister As KeyValuePair(Of Microsoft.VisualBasic.CommandLine.Reflection.Namespace, Type))
                                             Dim ModuleName = If(String.IsNullOrEmpty(AssemblyName), ModuleToRegister.Key.Namespace, AssemblyName)
                                             Dim RegistryItem As ModuleLoadEntry = New ModuleLoadEntry With
                                                                                   {
                                                                                       .Namespace = ModuleToRegister.Key.Namespace,
                                                                                       .TypeId = ModuleToRegister.Value.FullName,
                                                                                       .AssemblyPath = Internal_getRelativePath(AssemblyPath),
                                                                                       .Guid = ModuleToRegister.Value.GUID.ToString,
                                                                                       .Company = Company,
                                                                                       .Version = Version,
                                                                                       .FrameworkVersion = FrameworkVersion,
                                                                                       .Description = ModuleToRegister.Key.Description,
                                                                                       .UpdateTime = Updates}
                                             RegistryItem.CommandHandles = (From item As CommandLine.Reflection.EntryPoints.CommandEntryPointInfo
                                                                      In CommandLine.Interpreter.GetAllCommands(ModuleToRegister.Value)
                                                                            Select New ShoalShell.DelegateHandlers.TypeLibraryRegistry.RegistryNodes.MethodMeta With
                                                                             {
                                                                                 .Name = item.Name, .Description = item.Info,
                                                                                 .ReturnedType = item.MethodEntryPoint.ReturnType.FullName,
                                                                                 .Parameters = (From p
                                                                                                In item.MethodEntryPoint.GetParameters
                                                                                                Select New Microsoft.VisualBasic.ComponentModel.KeyValuePair With
                                                                                                       {
                                                                                                           .Key = p.Name, .Value = p.ParameterType.FullName}).ToArray}).ToArray

                                             Call RegistryInternalInsertOrUpdateRegistryEntryValue(ModuleName, _InnerList, RegistryItem)
                                         End Sub

            If Modules.Count = 1 Then
                Dim ModuleToRegister = Modules.First
                Call UpdateOrRegisterModule(ModuleToRegister)
            Else
                For Each [Module] In Modules
                    Call UpdateOrRegisterModule([Module])
                Next
            End If

            Return True
        End Function

        ''' <summary>
        ''' 假若目标模块文件在与程序模块是同一个文件夹之下的，则会返回相对路径，假若为不同文件夹之下的，则会返回全路径
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function Internal_getRelativePath(path As String) As String
            Dim s1 As String = FileIO.FileSystem.GetParentPath(path)
            Dim s2 As String = My.Application.Info.DirectoryPath

            If String.Equals(s1, s2) Then
                Return "./" & FileIO.FileSystem.GetFileInfo(path).Name
            Else
                Return path
            End If
        End Function

        ''' <summary>
        ''' 解析混合编程的脚本环境的环境入口点
        ''' </summary>
        ''' <param name="AssemblyPath">模块文件的文件路径</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InternalGetExternalInteropEntryPoints(AssemblyPath As String) As KeyValuePair(Of ShoalShell.HybridsScripting.ScriptEntryPoint, System.Type)()
            Dim Assembly = System.Reflection.Assembly.LoadFrom(AssemblyPath)
            Dim Types = (From Type In Assembly.DefinedTypes
                         Let attrs As Object() = Type.GetCustomAttributes(attributeType:=ShoalShell.HybridsScripting.ScriptEntryPoint.TypeInfo, inherit:=True)
                         Where Not attrs.IsNullOrEmpty
                         Let EntryInfo = DirectCast(attrs.First, ShoalShell.HybridsScripting.ScriptEntryPoint)
                         Select New KeyValuePair(Of ShoalShell.HybridsScripting.ScriptEntryPoint, System.Type)(EntryInfo, Type)).ToArray
            Return Types
        End Function

        ''' <summary>
        ''' 默认的注册表配置文件，该文件是在与本程序同一个文件夹之下的以程序名开始的XML文件.在该文件之中包含有所有的类型注册信息
        ''' </summary>
        ''' <remarks></remarks>
        Protected Shared ReadOnly _DefaultFile As String = String.Format("{0}/B0DC054A-A16E-4249-8C3B-E3AB1123F13C.REG".ToLower, My.Application.Info.DirectoryPath)

        ''' <summary>
        ''' 默认的注册表配置文件，该文件是在与本程序同一个文件夹之下的以程序名开始的XML文件.在该文件之中包含有所有的类型注册信息
        ''' </summary>
        ''' <remarks></remarks>
        Public Shared ReadOnly Property DefaultFile As String
            Get
                Return _DefaultFile
            End Get
        End Property

        ''' <summary>
        ''' Load the registry file from a default registry file.(从<see cref="DefaultFile">默认的配置文件</see>加载注册表)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateDefault() As DelegateRegistry
            Return New RegistryModuleLoader(_DefaultFile, New String() {})._RegistryFile
        End Function

        ''' <summary>
        ''' 请使用本方法或者<see cref="CreateDefault"></see>方法进行创建，否则都将会创建失败
        ''' </summary>
        ''' <param name="XmlFile"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateFromFile(XmlFile As String) As DelegateRegistry
            Return New RegistryModuleLoader(XmlFile, New String() {})._RegistryFile
        End Function

        Public Overrides Function ToString() As String
            Return _FilePath
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    Call Me.GetXml.SaveTo(_FilePath)
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace