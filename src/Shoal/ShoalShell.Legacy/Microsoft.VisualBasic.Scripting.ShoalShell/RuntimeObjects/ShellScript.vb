Imports System.Text
Imports Microsoft.VisualBasic.Scripting.ShoalShell.DeviceDriver
Imports Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry

Namespace Runtime.Objects

    ''' <summary>
    ''' ShoalShell Scripting Host, shl.(ShoalShell的脚本编程的引擎，嵌入式脚本引擎)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ShellScript : Implements System.IDisposable

#Region "Fileds"

        Protected Friend _EngineMemoryDevice As I_MemoryManagementDevice
        Protected Friend _Interpreter As Interpreter.Interpreter

        ''' <summary>
        ''' 在当前的脚本环境之中已经导入的命名空间
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend _ImportsNamespace As List(Of String) = New List(Of String)
        Protected Friend _HybridScriptingInteropEntryPointHandler As HybridsScripting.HybridScriptingInteropHandler
        Protected Friend _IOSupport As IODeviceDriver
        Protected Friend _OutputSupport As OutputDeviceDriver
        Protected Friend _InputSupport As InputDeviceDriver

        Dim _DEBUG_TAG As String
        Dim [_Error] As String
        Protected Friend _RunningScript As Boolean
        Protected Friend _ImportsNewNamespace As Boolean

        ''' <summary>
        ''' 从外部所导入的命令以及内部的命令的集合
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend _InternalEntryPointManager As DelegateHandlers.EntryPointHandlers.ImportsEntryPointManager
        Protected Friend _InternalHelpSystem As ShoalShell.Wiki.InternalHelpSearch

#End Region

#Region "ReadOnly Property"

        Public ReadOnly Property ImportsDataSource As Runtime.Objects.ObjectModels.DataSourceMapping.DataSourceMappingHandler
            Get
                Return _EngineMemoryDevice._InternalImportsDataSource
            End Get
        End Property

        ''' <summary>
        ''' 脚本引擎的内存管理工具
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property ScriptEngineMemoryDevice As I_MemoryManagementDevice
            Get
                Return Me._EngineMemoryDevice
            End Get
        End Property

        Public ReadOnly Property Interpreter As Interpreter.Interpreter
            Get
                Return _Interpreter
            End Get
        End Property

        ''' <summary>
        ''' 在当前的脚本环境之中已经导入的命名空间
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property ImportsNamespaceList As String()
            Get
                Return _ImportsNamespace.ToArray
            End Get
        End Property

        Public ReadOnly Property HybridScriptingInteropEntryPoint As HybridsScripting.HybridScriptingInteropHandler
            Get
                Return _HybridScriptingInteropEntryPointHandler
            End Get
        End Property

        Public ReadOnly Property IODeviceManager As IODeviceDriver
            Get
                Return Me._IOSupport
            End Get
        End Property

        Public ReadOnly Property OutputDeviceManager As OutputDeviceDriver
            Get
                Return Me._OutputSupport
            End Get
        End Property

        Public ReadOnly Property InputDeviceDriver As InputDeviceDriver
            Get
                Return Me._InputSupport
            End Get
        End Property

        Public ReadOnly Property InternalEntryPointManager As DelegateHandlers.EntryPointHandlers.ImportsEntryPointManager
            Get
                Return Me._InternalEntryPointManager
            End Get
        End Property

        Public ReadOnly Property TypeLibraryRegistry As DelegateRegistry
            Get
                Return _Interpreter._DelegateRegistry
            End Get
        End Property
#End Region

        ''' <summary>
        ''' 这个是为IDE的开发而准备的
        ''' </summary>
        ''' <remarks></remarks>
        Public Event ImportsNamespace()

        ''' <summary>
        ''' 当编译器导入了新的命名空间的时候，就会调用这个方法，将消息传递给IDE，更新IDE的界面
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend Sub ImportsNewNamespace()
            _ImportsNewNamespace = True
            RaiseEvent ImportsNamespace()
        End Sub

        Public ReadOnly Property ErrorMessage As String
            Get
                Return _Error
            End Get
        End Property

        Public Shared ReadOnly Property Keywords As String()
            Get
                Return New String() {"Imports", "Return", "echo", "Library", "Libraries", "Call"}
            End Get
        End Property

        ''' <summary>
        ''' Get the imports method from a specific namespace in the shoal shell script engine.(获取在脚本引擎之中已经被导入的命名空间之中的方法)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ImportsMethods As String()
            Get
                Return _InternalEntryPointManager.ImportsMethods.ToArray
            End Get
        End Property

        ''' <summary>
        ''' Gets the help information using a specific keyword for invoke the searching.(使用指定的关键词来搜索帮助信息)
        ''' </summary>
        ''' <param name="key"></param>
        ''' <param name="ShowManual">在Http帮助服务器之上请设定这个参数为False</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetHelpInfo(key As String, ShowManual As Boolean) As String
            Return _InternalHelpSystem.GetHelpInfo(key, ShowManual)
        End Function

        ''' <summary>
        ''' 向脚本引擎之中导入命名空间
        ''' </summary>
        ''' <typeparam name="T_Object"></typeparam>
        ''' <param name="obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function [Imports](Of [T_Object] As Class)(obj As T_Object) As Boolean
            Return _InternalEntryPointManager.ImportsInstanceNamespace(obj) > 0
        End Function

        ''' <summary>
        ''' Imports module.(导入模块之中的命令)
        ''' </summary>
        ''' <param name="TypeInfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function [Imports](TypeInfo As System.Type) As Boolean
            Return _InternalEntryPointManager.ImportsModule(TypeInfo) > 0
        End Function

        ''' <summary>
        ''' Interpreter对象之中的IOSupport对象的初始化需要Memory对象实例，所以请不要轻易修改这里的对象的初始化顺序
        ''' </summary>
        ''' <remarks></remarks>
        Sub New(Optional ShowInitializeMessage As Boolean = False)
            Call InternalInitialize(ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.CreateDefault, ShowInitializeMessage)
        End Sub

        Sub New(LibraryRegistry As ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry, Optional ShowInitializeMessage As Boolean = False)
            Call InternalInitialize(LibraryRegistry, ShowInitializeMessage)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="LibraryRegistry"></param>
        ''' <param name="ShowInitializeMessage"></param>
        ''' <remarks></remarks>
        Sub New(LibraryRegistry As String, Optional ShowInitializeMessage As Boolean = False)
            Dim Registry = ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.CreateFromFile(LibraryRegistry)
            Call InternalInitialize(Registry, ShowInitializeMessage)
        End Sub

        ''' <summary>
        ''' 脚本引擎内部初始化脚本编程环境
        ''' </summary>
        ''' <param name="LibraryRegistry"></param>
        ''' <param name="ShowInitializeMessage"></param>
        ''' <remarks></remarks>
        Private Sub InternalInitialize(LibraryRegistry As ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry, ShowInitializeMessage As Boolean)
            _EngineMemoryDevice = New I_MemoryManagementDevice(Me)
            _DEBUG_TAG = Now.ToString
            _HybridScriptingInteropEntryPointHandler = New HybridsScripting.HybridScriptingInteropHandler(Me)
            _Interpreter = New Interpreter.Interpreter(Me, LibraryRegistry, ShowInitializeMessage)

            On Error Resume Next
            Console.Title = "Shoal Shell"
        End Sub

        Public Function ImportsIOSupports([Module] As Type) As Integer
            Dim i = _IOSupport.ImportsHandler([Module])
            i += _InputSupport.ImportsHandler([Module])
            Return i
        End Function

        Public Function ImportsOutputHandlers([Module] As ShoalShell.DelegateHandlers.TypeLibraryRegistry.RegistryNodes.Module) As Integer
            For Each item In [Module].OriginalAssemblys
                Call _OutputSupport.ImportsHandler(item.Key)
            Next

            Return 0
        End Function

        ''' <summary>
        ''' 安装插件模块
        ''' </summary>
        ''' <param name="assembly">向Shoal脚本安装插件模块的dll或者exe文件</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InstallModules(assembly As String) As Boolean
            Return Me._Interpreter._DelegateRegistry.RegisterAssemblyModule(assembly, "")
        End Function

        Public Sub ReloadEntryPoints()
            If Not _Interpreter._DelegateRegistry._HybridsScriptingEntrypoints.IsNullOrEmpty Then
                For Each Entry In _Interpreter._DelegateRegistry._HybridsScriptingEntrypoints
                    Call _HybridScriptingInteropEntryPointHandler.LoadEntryPoints(DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.Internal_getFullPath(Entry.AssemblyPath))
                Next
            End If
        End Sub

        Public Sub ReloadLibraryRegistry()
            _Interpreter._DelegateRegistry = ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.CreateDefault
        End Sub

        ''' <summary>
        ''' 使用外部的脚本环境进行运算
        ''' </summary>
        ''' <param name="ScriptText"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function ExternalScriptInteropEvaluate(ScriptText As String) As Object
            Return _HybridScriptingInteropEntryPointHandler.Evaluate(ScriptText)
        End Function

        Public Function ExternalScriptInteropSetValue(variable As String, value As Object) As Boolean
            Return _HybridScriptingInteropEntryPointHandler.SetValue(variable, value)
        End Function

        ''' <summary>
        ''' 载入外部脚本的挂载点
        ''' </summary>
        ''' <param name="name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AttachesEntryPoint(name As String) As Boolean
            Try
                Call _HybridScriptingInteropEntryPointHandler.AttachEntryPoint(name)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End Function

        Public Sub PushStack(value As Object)
            Call _EngineMemoryDevice.PushStack(value)
        End Sub

        Public Function GetStackValue() As Object
            Return _EngineMemoryDevice.GetStackValue
        End Function

        ''' <summary>
        ''' 变量不存在的话则会返回字符串自身
        ''' </summary>
        ''' <param name="variableName">请使用变量引用表达式</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetValue(variableName As String) As Object
            Dim value = _EngineMemoryDevice.TryGetValue(variableName)
            Return value
        End Function

        Public Sub SetValue(varName As String, value As Object)
            Call _EngineMemoryDevice.SetValue(New KeyValuePair(Of String, Object)(varName, value))
        End Sub

        ''' <summary>
        ''' 日志文件路径，当发生错误的时候，假若本属性不为空，则会将错误的信息同时写入在这里，否则仅将错误信息写入主日志文件之中
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ExceptionHandleRedirect As String

        Public Function Compile(script As String) As Runtime.Objects.ObjectModels.ShellScript
            Return _Interpreter.CreateScriptObject(script)
        End Function

        ''' <summary>
        ''' Execute a shellscript file.
        ''' (执行一个脚本文件中的脚本文本内容，请注意，函数仅返回脚本的执行状态，0位执行成功，其他非0数字可能表示执行失败，
        ''' 假若目标脚本有return关键词返回执行结果，需要得到脚本的具体返回数据，请使用<see cref="ShellScript.GetStackValue"></see>方法获取。)
        ''' </summary>
        ''' <param name="ShellScript">
        ''' This script data should be a line of the script code that of which is user type input from the console terminal
        ''' or a text file content from the return value of <see cref="FileIO.FileSystem.ReadAllText"></see>
        ''' </param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EXEC(ShellScript As String) As Integer
            If String.IsNullOrEmpty(ShellScript.Replace(vbCrLf, "").Trim) Then
                Return -1
            Else

#Const RELEASE = True

#If RELEASE Then
                Try
#End If
                    Dim ObjectModel = _Interpreter.CreateScriptObject(ShellScript)

                    _RunningScript = True
                    _Error = ""

                    Call InternalExecuteScript(ObjectModel)

                    _ImportsNewNamespace = False
                    _RunningScript = False
#If RELEASE Then
                Catch ex As Exception
                    _RunningScript = False
                    Return HandleException(ex, ShellScript)
                End Try
#End If
                Return 0
            End If
        End Function

        Protected Overridable Sub InternalExecuteScript(Script As ShoalShell.Runtime.Objects.ObjectModels.ShellScript)
            Call Script.Execute()
        End Sub

        Protected Friend Function HandleException(ex As Exception, ShellScript As String) As Integer
            Dim ExceptionObject As String = ""

            If Not TypeOf ex Is Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ObjectModels.Exceptions.ScriptRunTimeException Then
                GoTo WRITE_LOG
            End If

            Dim Exception = DirectCast(ex, Runtime.Objects.ObjectModels.Exceptions.ScriptRunTimeException)
            Call Console.WriteLine("[!ERROR] @ ""{0}""", Exception.ScriptLine)
            ExceptionObject = Exception.ExceptionType

WRITE_LOG:  Using Logging As Logging.LogFile = New Logging.LogFile(String.Format("{0}/ShellScript_ERROR.log", My.Application.Info.DirectoryPath))
                Dim Err As String =
                    vbCrLf &
                    "--------------------------------------------------------------------------------------------------------------------------------------------------------" & vbCrLf &
                    "[Shoal shell script text details]" & vbCrLf & vbCrLf &
                    String.Join(vbCrLf, (From str In Strings.Split(ShellScript, vbCrLf) Select "  " & str).ToArray) & vbCrLf &
                    "--------------------------------------------------------------------------------------------------------------------------------------------------------" & vbCrLf & vbCrLf & vbCrLf &
                    ex.ToString & vbCrLf

                Call Console.WriteLine(vbCrLf & ex.ToString & vbCrLf)
                Call Logging.WriteLine(Err, [Object]:=ExceptionObject, Type:=VisualBasic.Logging.MSG_TYPES.ERR, WriteToScreen:=False)

                _Error = Err

                If Not String.IsNullOrEmpty(ExceptionHandleRedirect) Then

                    Using Redirect = New Logging.LogFile(ExceptionHandleRedirect)
                        Call Redirect.WriteLine(Err, ExceptionObject, Type:=VisualBasic.Logging.MSG_TYPES.ERR, WriteToScreen:=False)
                    End Using
                End If

                Return -1
            End Using
        End Function

        Public Function Source(path As String) As Integer
            Dim currentWork As String = My.Computer.FileSystem.CurrentDirectory
            My.Computer.FileSystem.CurrentDirectory = FileIO.FileSystem.GetParentPath(path)
            Dim i As Integer = EXEC(FileIO.FileSystem.ReadAllText(path))
            My.Computer.FileSystem.CurrentDirectory = currentWork

            Return i
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ShellScript">这个参数是脚本文件的文本内容，而非脚本文件的路径</param>
        ''' <param name="parameters"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InternalSourceScript(ShellScript As String, parameters As KeyValuePair(Of String, Object)()) As Object
            For Each pInfo In parameters
                Call Me._EngineMemoryDevice.InsertOrUpdate(pInfo.Key, pInfo.Value)
            Next

            Call EXEC(ShellScript)

            Dim StackValue = GetStackValue()

            If StackValue Is Nothing Then
                StackValue = _EngineMemoryDevice
            End If

            Return StackValue
        End Function

        ''' <summary>
        ''' 系统有一个默认的保留变量$
        ''' </summary>
        ''' <param name="VariableName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Function PrintVariable(VariableName As String) As Integer
            If Not String.Equals(VariableName, "$") Then
                VariableName = Mid(VariableName, 2) '去除开头的$引用字符
            End If

            Dim value = _EngineMemoryDevice.Item(VariableName)
            Dim Tokens As String() = GetPrintValue(value)
            If Tokens.IsNullOrEmpty Then
                Call Console.WriteLine("   = EMPTY_ARRAY")
            End If

            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Call sBuilder.AppendLine("   = [0]  " & Tokens.First)
            For i As Integer = 1 To Tokens.Count - 1
                Call sBuilder.AppendLine(String.Format("     [{0}]  {1}", i, Tokens(i)))
            Next
            Call Console.WriteLine(sBuilder.ToString)
            Return -1
        End Function

        ''' <summary>
        ''' 系统有一个默认的保留变量$
        ''' </summary>
        ''' <param name="VariableName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Protected Friend Function PrintConstant(VariableName As String) As Integer
            If Not String.Equals(VariableName, "&") Then
                VariableName = Mid(VariableName, 2) '去除开头的$引用字符
            End If

            Dim value = _EngineMemoryDevice.GetConstant(VariableName)
            Dim Tokens As String() = GetPrintValue(value)
            If Tokens.IsNullOrEmpty Then
                Call Console.WriteLine("   = EMPTY_ARRAY")
            End If

            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Call sBuilder.AppendLine("   = [0]  " & Tokens.First)
            For i As Integer = 1 To Tokens.Count - 1
                Call sBuilder.AppendLine(String.Format("     [{0}]  {1}", i, Tokens(i)))
            Next
            Call Console.WriteLine(sBuilder.ToString)
            Return -1
        End Function

        Public Overrides Function ToString() As String
            Return _DEBUG_TAG
        End Function

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
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

        ''' <summary>
        ''' 执行一行脚本并返回计算值
        ''' </summary>
        ''' <param name="Shoal"></param>
        ''' <param name="Script"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator <=(Shoal As ShoalShell.Runtime.Objects.ShellScript, Script As String) As Object
            Call Shoal.EXEC(Script)
            Return Shoal.GetStackValue
        End Operator

        ''' <summary>
        ''' 获取某一个变量
        ''' </summary>
        ''' <param name="Shoal"></param>
        ''' <param name="Variable"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Operator >=(Shoal As ShoalShell.Runtime.Objects.ShellScript, Variable As String) As Object
            Return Shoal.GetValue(Variable)
        End Operator
    End Class
End Namespace