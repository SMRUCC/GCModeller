Imports System.Text.RegularExpressions
Imports System.Reflection

Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ObjectModels.Exceptions
Imports Microsoft.VisualBasic.Scripting.ShoalShell.DeviceDriver
Imports Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry
Imports Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.EntryPointHandlers

Namespace Interpreter

    'REM <- ns function ${ns er df ${dse sd sds $adds}}

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <remarks></remarks>
    Public Class Interpreter : Inherits ShoalShell.Runtime.Objects.ObjectModels.IScriptEngineComponent

        Protected Friend _DelegateRegistry As DelegateRegistry
        Dim _IOSupport As IODeviceDriver
        Dim _OutputSupport As OutputDeviceDriver
        Dim _InputSupport As InputDeviceDriver

        Protected Friend _InternalMethodInvoker As ShoalShell.DelegateHandlers.EntryPointHandlers.MethodDelegateCaller
        Protected Friend _InternalCommands As InternalCommands

        Public Const VARIABLE_NAME As String = "[^$^&^ ]\S*"
        Public Const VARIABLE_ASSIGNMENT As String = VARIABLE_NAME & "\s+(<-|<<|>>|[~]|>|<=|<)\s+"

        Public ReadOnly Property InternalCommands As InternalCommands
            Get
                Return _InternalCommands
            End Get
        End Property

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="ScriptHost"></param>
        ''' <param name="LibraryRegistry">注册表文件的文件路径</param>
        ''' <remarks></remarks>
        Sub New(ScriptHost As Runtime.Objects.ShellScript, LibraryRegistry As String, Optional ShowInitializeMessage As Boolean = False, Optional preferIndexedManual As Boolean = True)
            Call MyBase.New(ScriptHost)
            Call InternalInitialize(ScriptHost, LibraryRegistry.LoadXml(Of ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry), ShowInitializeMessage, preferIndexedManual)
        End Sub

        Sub New(ScriptHost As Runtime.Objects.ShellScript, LibraryRegistry As ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry, Optional ShowInitializeMessage As Boolean = False, Optional preferIndexedManual As Boolean = True)
            Call MyBase.New(ScriptHost)
            Call InternalInitialize(ScriptHost, LibraryRegistry, ShowInitializeMessage, preferIndexedManual)
        End Sub

        Private Sub InternalInitialize(ScriptHost As Runtime.Objects.ShellScript, LibraryRegistry As ShoalShell.DelegateHandlers.TypeLibraryRegistry.DelegateRegistry, ShowInitializeMessage As Boolean, preferIndexedManual As Boolean)
            If ShowInitializeMessage Then Call Console.WriteLine("[Start to Initialize...]")

            Call LibraryRegistry.CheckForLibraryConsists(ShowInitializeMessage)

            _DelegateRegistry = LibraryRegistry
            ScriptEngine._InternalEntryPointManager = New DelegateHandlers.EntryPointHandlers.ImportsEntryPointManager(ScriptEngine, My.Computer.FileSystem.CurrentDirectory, disp_inits_message:=ShowInitializeMessage)
            _InternalCommands = New InternalCommands(ScriptHost, ShowInitializeMessage, preferIndexedManual)
            _InternalMethodInvoker = New MethodDelegateCaller(ScriptHost)

            '加载外部脚本的注册表
            If Not _DelegateRegistry.HybridsScriptingEntrypoints.IsNullOrEmpty Then
                If ShowInitializeMessage Then Call Console.WriteLine("Loading hybrid scripting entries....")

                For Each EntryPoint As RegistryNodes.HybridScriptingModuleLoadEntry In _DelegateRegistry.HybridsScriptingEntrypoints
                    Call _RuntimeEnvironment._HybridScriptingInteropEntryPointHandler.LoadEntryPoints(AssemblyFile:=DelegateHandlers.TypeLibraryRegistry.DelegateRegistry.Internal_getFullPath(EntryPoint.AssemblyPath))
                Next
            End If

            If ShowInitializeMessage Then Call Console.WriteLine("Initialize the Output device...")
            _IOSupport = New IODeviceDriver(ScriptHost._EngineMemoryDevice)
            _OutputSupport = New OutputDeviceDriver(ScriptHost._EngineMemoryDevice)
            _InputSupport = New InputDeviceDriver(ScriptHost._EngineMemoryDevice)
            ScriptHost._IOSupport = _IOSupport
            ScriptHost._OutputSupport = _OutputSupport
            ScriptHost._InputSupport = _InputSupport

            If ShowInitializeMessage Then
                Call Console.WriteLine("IO device mounted, you can using command ""? mount"" to view the mounted device in shoal shell.")
                Call Console.WriteLine()
                Call Console.WriteLine("[Done!]" & vbCrLf)
            End If
        End Sub

        ''' <summary>
        ''' Parsing the <paramref name="ShellScript">script text</paramref> into the script objectmodel and then return for execute.
        ''' </summary>
        ''' <param name="ShellScript"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' #为注释符，解析器会自动忽略改行
        ''' </remarks>
        Public Function CreateScriptObject(ShellScript As String) As Runtime.Objects.ObjectModels.ShellScript
            Dim Commands As List(Of String) = ShoalShell.Interpreter.DelegateDeclaration.TryParse(ShellScript.Replace(":=", " "), ScriptEngine.InternalEntryPointManager).ToList
            Dim CommandList = ProcessIncludes(Commands, New List(Of String))

            Dim LQuery = (From i As Integer In CommandList.Sequence
                          Let CmdlToken = CommandList(i)
                          Select InternalTryParse(Command:=CmdlToken, LineNumber:=i)).ToArray   '当脚本之中有library和imports命令的时候，会被tryparse函数首先执行

            Dim ResumrFlag As Integer() = (From i As Integer In CommandList.Sequence
                                           Let strToken = CommandList(i)
                                           Where strToken.CLICommandArgvs.First = "#"c AndAlso InStr(strToken.CLICommandArgvs.Replace("#", "").Trim, "on error resume next", CompareMethod.Text) = 1
                                           Select i).ToArray

            If Not ResumrFlag.IsNullOrEmpty Then
                Dim i = ResumrFlag.First + 1

                If Not i > CommandList.Count - 2 Then
                    Dim strTemp = CommandList(ResumrFlag.First + 1)
                    Dim XQuery = (From item In LQuery Where String.Equals(strTemp.CLICommandArgvs, item.OrignialScriptLine) Select item).First
                    XQuery.OnErrorResumeNext = True
                End If
            End If

            Return New Runtime.Objects.ObjectModels.ShellScript(LQuery, Me.ScriptEngine)
        End Function

        ''' <summary>
        ''' include的作用就是将多个脚本源文件合并在一起进行执行，本函数会将include申明的所在行替换为目标脚本的内容
        ''' </summary>
        ''' <param name="source"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function ProcessIncludes(source As List(Of String), ByRef IncludeList As List(Of String)) As List(Of CommandLine.CommandLine)
#Const DEBUG = 1

#If DEBUG Then
            Dim LQuery = (From i As Integer In source.Sequence
                          Let strItemLine As String = source(i)
                          Let Tokens = CommandLine.CommandLine.TryParse(strItemLine.Trim, DuplicatedAllowed:=True)
                          Where String.Equals(Tokens.Name, "include", StringComparison.OrdinalIgnoreCase)
                          Let sourceFile As String = Tokens.Tokens(1)
                          Select LineNumber = i, sourceLine = strItemLine, sourceFile
                          Order By LineNumber Ascending).ToArray
#Else
            Dim LQuery = (From i As Integer In source.Sequence.AsParallel
                      Let strItemLine As String = source(i)
                      Let Tokens = CommandLine.CommandLine.TryParse(strItemLine.Trim)
                      Where String.Equals(Tokens.Name, "include", StringComparison.OrdinalIgnoreCase)
                      Let sourceFile As String = Tokens.Tokens(1)
                      Select LineNumber = i, sourceLine = strItemLine, sourceFile
                      Order By LineNumber Ascending).ToArray
#End If
            Dim sourceList = (From strLine As String In source Select CommandLine.CommandLine.TryParse(strLine, True)).ToList

            If LQuery.IsNullOrEmpty Then
                Return sourceList
            End If

            Call IncludeList.AddRange((From item In LQuery Select item.sourceFile).ToArray)

            If CheckCycleReference(IncludeList) = True Then
                Throw New Runtime.Objects.ObjectModels.Exceptions.CircularReferencesException With {.IncludeList = IncludeList.ToArray}
            End If

            For Each Line In LQuery
                Call sourceList.RemoveAt(Line.LineNumber)
                Call sourceList.InsertRange(Line.LineNumber, InternalProcessIncludes(path:=Line.sourceFile, IncludedList:=IncludeList))
            Next

            Return sourceList
        End Function

        ''' <summary>
        ''' 当函数返回TRUE的时候，表示有循环引用，则抛出错误
        ''' </summary>
        ''' <param name="includeList"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function CheckCycleReference(includeList As Generic.IEnumerable(Of String)) As Boolean
            Dim LQuery = (From s As String In includeList.AsParallel Select FileIO.FileSystem.GetFileInfo(s).FullName Group By FullName Into Group).ToArray
            Dim Check = (From item In LQuery Where item.Group.Count > 1 Select 1).ToArray
            Return Not Check.IsNullOrEmpty
        End Function

        ''' <summary>
        ''' tokens 0 -&gt; include
        ''' tokens 1 
        ''' </summary>
        ''' <param name="path"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InternalProcessIncludes(path As String, IncludedList As List(Of String)) As List(Of CommandLine.CommandLine)
            Dim Commands As List(Of String) = ShoalShell.Interpreter.DelegateDeclaration.TryParse(FileIO.FileSystem.ReadAllText(path), ScriptEngine.InternalEntryPointManager).ToList
            Dim source = ProcessIncludes(Commands, IncludedList)
            Return source
        End Function

        Private Function TypeSpecificVector(argvs As String, typeid As String) As Object
            Dim array = Me._InternalCommands.CreateArray(argvs)
            Dim castMethod = BuildInModules.System.Array.CastedTypes(typeid.ToLower)
            Dim LQuery = castMethod(array)
            Return LQuery
        End Function

        Private Function InternalDelayGetValue(ref As String) As Object
            Dim ScriptRef = Me.CreateScriptObject(ref)
            Dim value = ScriptRef.Execute
            Return value
        End Function

        ''' <summary>
        ''' 返回变量名和变量的赋值方法；对于方法调用，总是将得到的值赋值给一个<see cref="ShoalShell.Runtime.Objects.I_MemoryManagementDevice.CONSERVED_SYSTEM_VARIABLE">保留的系统变量</see>
        ''' </summary>
        ''' <param name="Command"></param>
        ''' <param name="ExtensionMethodVariable">这个参数是由<see cref="TryParseExtensionMethodSyntax"></see>函数所传递进来的</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function InternalTryParse(Command As CommandLine.CommandLine, LineNumber As Integer, Optional ExtensionMethodVariable As String = "") As Runtime.Objects.ObjectModels.ScriptCodeLine
            Dim ExtMethod As Runtime.Objects.ObjectModels.ScriptCodeLine
            Dim VariableName As String = Regex.Match(Command.CLICommandArgvs, VARIABLE_ASSIGNMENT, RegexOptions.IgnoreCase).Value
            Dim GetValue As Func(Of Object) = Function() -1
            Dim OriginalCommand As String = Command.CLICommandArgvs
            Dim ReturnType As String

            '请不要再修改本函数之中的语法对象的解析顺序了，当前的解析顺序已经很好的满足了语法特性和解析效率的需求了

            Dim InternalTextProceeding As ShoalShell.Runtime.Objects.ObjectModels.ScriptCodeLine = ShoalShell.Interpreter.InternalTextEngine.SyntaxParser(Command, Me.ScriptEngine._EngineMemoryDevice)

            If Not InternalTextProceeding Is Nothing Then
                InternalTextProceeding.LineNumber = LineNumber
                InternalTextProceeding.LineFlag = LineNumber

                Return InternalTextProceeding
            End If

            If InStr(OriginalCommand, " > ") > 0 Then
                Dim Test As String = "$" & VariableName
                If InStr(OriginalCommand, Test) >= 1 Then
                    VariableName = Test
                End If
                Dim SavePath As String = OriginalCommand.Replace(VariableName, "").Trim
                SavePath = SavePath.GetString(wrapper:="""")
                VariableName = VariableName.Replace(" > ", "").Trim
                GetValue = Function() _IOSupport.WriteData(VariableName, SavePath)
                Return New Runtime.Objects.ObjectModels.ScriptCodeLine With
                       {
                           .LineNumber = LineNumber,
                           .LineFlag = LineNumber,
                           .VariableAssigned = "$",
                           .OrignialScriptLine = OriginalCommand,
                           .InvokeMethod = GetValue}
            End If

            If InStr(OriginalCommand, " < ") > 0 Then
                Dim Right As String = OriginalCommand.Replace(VariableName, "").Trim
                Dim typeid As String = Regex.Match(Right, "\(.+\)").Value
                Right = Right.Replace(typeid, "").Trim
                Right = Right.GetString(wrapper:="""")
                VariableName = VariableName.Replace(" < ", "").Trim
                typeid = Mid(typeid, 2)
                typeid = Mid(typeid, 1, Len(typeid) - 1)

                GetValue = Function() As Object
                               Dim source As Object = Me.ScriptEngine._EngineMemoryDevice.TryGetValue(Right)
                               Dim value = _InputSupport.TypeCasting(source, typeid)
                               Return value
                           End Function

                Return New Runtime.Objects.ObjectModels.ScriptCodeLine With
                       {
                           .LineNumber = LineNumber,
                           .LineFlag = LineNumber,
                           .VariableAssigned = VariableName,
                           .OrignialScriptLine = OriginalCommand,
                           .InvokeMethod = GetValue}
            End If

            If InStr(OriginalCommand, " <= ") > 0 Then '进行集合相关的操作

                Dim ArrayGetItem As String = OriginalCommand.Replace(VariableName, "").Trim

                ' [$collection] <= element '向集合的尾部添加一个元素
                Dim testTemp As String = Regex.Match(VariableName, "\[\$.+?\]").Value
                If Not String.IsNullOrEmpty(testTemp) Then
                    VariableName = Mid(testTemp, 2, Len(testTemp) - 2) '集合名称
                    VariableName = Mid(VariableName, 2)
                    Dim value = Function() InternalDelayGetValue(ArrayGetItem)
                    GetValue = Function() BuildInModules.System.Array.AppendCollection(Me._RuntimeEnvironment._EngineMemoryDevice.TryGetValue(VariableName), element:=value())

                    GoTo COLLECTION_LINE
                Else



                End If

                VariableName = VariableName.Replace(" <= ", "").Trim

                Const SPECIFIC_TYPE_VECTOR As String = "\((integer|double|boolean|string|long)\)\s+("".+""|\S+)"

                Dim tmp As String = Regex.Match(ArrayGetItem, pattern:=SPECIFIC_TYPE_VECTOR, options:=RegexOptions.IgnoreCase).Value
                If Not String.IsNullOrEmpty(tmp) Then
                    Dim type As String = Regex.Match(tmp, "\(.+?\)").Value
                    Dim typeId = Mid(type, 2)
                    typeId = Mid(typeId, 1, Len(typeId) - 1)
                    tmp = tmp.Replace(type, "").Trim
                    GetValue = Function() TypeSpecificVector(argvs:=tmp, typeid:=typeId)
                Else
                    GetValue = Function() BuildInModules.System.Array.get_Item(ArrayGetItem, Memory:=Me._RuntimeEnvironment._EngineMemoryDevice)
                End If

COLLECTION_LINE: Return New Runtime.Objects.ObjectModels.ScriptCodeLine With {
                    .LineNumber = LineNumber, .LineFlag = LineNumber,
                    .VariableAssigned = VariableName,
                    .OrignialScriptLine = OriginalCommand,
                    .InvokeMethod = GetValue,
                    .ReturnType = GetType(Object).FullName}
            End If

            If InStr(OriginalCommand, " => ") > 0 Then
                Dim var As String = Mid(OriginalCommand, InStr(OriginalCommand, " => "))
                Dim Expression As String = OriginalCommand.Replace(var, "").Trim
                VariableName = var.Replace(" => ", "").Trim
                GetValue = Function() BuildInModules.System.Array.set_Item(expression:=Expression, [object]:=VariableName, memory:=Me._RuntimeEnvironment._EngineMemoryDevice)
                Return New Runtime.Objects.ObjectModels.ScriptCodeLine With {
                    .LineNumber = LineNumber, .LineFlag = LineNumber,
                    .VariableAssigned = VariableName,
                    .OrignialScriptLine = OriginalCommand,
                    .InvokeMethod = GetValue}
            End If

            ' Pushing the .NET variable into the hybirds scripting environment
            If InStr(OriginalCommand, " >> ") > 0 Then
                Dim Test As String = "$" & VariableName
                If InStr(OriginalCommand, Test) >= 1 Then
                    VariableName = Test
                End If
                Dim ExternalScriptVariable As String = OriginalCommand.Replace(VariableName, "").Trim
                VariableName = VariableName.Replace(" >> ", "").Trim
                GetValue = Function() _RuntimeEnvironment.ExternalScriptInteropSetValue(ExternalScriptVariable, Me._RuntimeEnvironment._EngineMemoryDevice.TryGetValue(VariableName))
                Return New Runtime.Objects.ObjectModels.ScriptCodeLine With {
                    .LineNumber = LineNumber,
                    .LineFlag = LineNumber,
                    .VariableAssigned = VariableName,
                    .OrignialScriptLine = OriginalCommand,
                    .InvokeMethod = GetValue}
            End If

            'invoke the external hybrid programming script statement and get the return value.(调用外部的脚本并返回计算结果)
            If InStr(OriginalCommand, " << ") > 0 Then
                Dim ExternalScript As String = OriginalCommand.Replace(VariableName, "").Trim
                VariableName = VariableName.Replace(" << ", "").Trim

                If ExternalScript.First = "!"c Then  '引用了之前所解析好的脚本块
                    Dim Environment As String = Regex.Match(ExternalScript, "!\S+").Value
                    Dim Cmdl As CommandLine.CommandLine = CommandLine.CommandLine.TryParse(ExternalScript.Replace(Environment, "").Trim)

                    Environment = Mid(Environment, 2)
                    GetValue = Function() _RuntimeEnvironment._HybridScriptingInteropEntryPointHandler.Evaluate(Environment, Cmdl)
                Else
                    GetValue = Function() _RuntimeEnvironment.ExternalScriptInteropEvaluate(ExternalScript)
                End If

                Return New Runtime.Objects.ObjectModels.ScriptCodeLine With {
                    .LineNumber = LineNumber, .LineFlag = LineNumber,
                    .VariableAssigned = VariableName,
                    .OrignialScriptLine = OriginalCommand,
                    .InvokeMethod = GetValue}
            End If

            'self type cast
            'equals to the statement:  var <- $var -> ctype
            If InStr(OriginalCommand, " ~ ") > 0 Then
                Dim NewToken As String = VariableName.Replace(" ~ ", " -> ")
                Command = OriginalCommand.Replace(VariableName, NewToken)
                Dim Code = InternalTryParse(Command, LineNumber)
                Code.VariableAssigned = VariableName.Split.First
                Return Code
            End If

            If OriginalCommand.First = "$"c Then  '假若开头为$符号，则意味着本行命令可能是内存中的变量的引用，也可能为拓展函数
                ExtMethod = TryParseExtensionMethodSyntax(OriginalCommand, LineNumber)
                If Not ExtMethod Is Nothing Then
                    ExtMethod.OrignialScriptLine = OriginalCommand
                    Return ExtMethod
                End If

                Return New Runtime.Objects.ObjectModels.ScriptCodeLine With {
                    .LineNumber = LineNumber, .LineFlag = LineNumber, .OrignialScriptLine = Command.CLICommandArgvs,
                    .VariableAssigned = "$",
                    .InvokeMethod = Function() _OutputSupport.HandleOutput(variable:=Command.CLICommandArgvs)}
            End If
            If OriginalCommand.First = "&" Then '假若开头为&符号，则意味着本行命令可能是引用内存中的常数
                Return New Runtime.Objects.ObjectModels.ScriptCodeLine With {
                    .LineNumber = LineNumber, .LineFlag = LineNumber, .OrignialScriptLine = Command.CLICommandArgvs,
                    .VariableAssigned = "$",
                    .InvokeMethod = Function() _RuntimeEnvironment.PrintConstant(Command.CLICommandArgvs)}
            End If

            If OriginalCommand.First = "!" Then 'Mount the hybrid programming entry point.(挂载混合编程的脚本环境的执行载入点)
                Dim Name As String = Mid(OriginalCommand, 2).Trim
                Return New Runtime.Objects.ObjectModels.ScriptCodeLine With {
                 .LineNumber = LineNumber, .LineFlag = LineNumber, .OrignialScriptLine = Command.CLICommandArgvs,
                 .VariableAssigned = "$",
                 .InvokeMethod = Function() _RuntimeEnvironment.AttachesEntryPoint(Name)}
            End If

            Dim ExeType As ShoalShell.Runtime.Objects.ObjectModels.ScriptCodeLine.PreExecuteTypes

            If String.IsNullOrEmpty(VariableName) Then '本命令可能是对系统内部命令进行调用，以及没有Call的命令调用
                If InStr(OriginalCommand, "call ", CompareMethod.Text) = 1 Then
                    Command = Mid(OriginalCommand, 6).Trim
                End If

                ExtMethod = TryParseExtensionMethodSyntax(Command.CLICommandArgvs, LineNumber)
                If Not ExtMethod Is Nothing Then
                    ExtMethod.OrignialScriptLine = OriginalCommand
                    Return ExtMethod
                End If

                VariableName = Runtime.Objects.I_MemoryManagementDevice.CONSERVED_SYSTEM_VARIABLE
                Dim Handle = TryParseCommand(Command, ExtensionMethodVariable, ExeType)
                GetValue = Handle.Value
                ReturnType = Handle.Key.FullName
            Else
                If InStr(OriginalCommand, VariableName) = 1 Then '本句脚本命令调用了一个方法，然后将得到的值返回给一个变量
                    Dim strCommand = Mid(OriginalCommand.Trim, Len(VariableName) + 1)
                    VariableName = VariableName.Split.First

                    ExtMethod = TryParseExtensionMethodSyntax(strCommand, LineNumber)
                    If Not ExtMethod Is Nothing Then
                        ExtMethod.VariableAssigned = VariableName
                        ExtMethod.OrignialScriptLine = OriginalCommand
                        Return ExtMethod '拓展方法通过本路径返回，对于其他方法调用，则在函数的最后进行处理
                    End If

                    Dim Handle = TryParseCommand(strCommand, ExtensionMethodVariable, ExeType)
                    GetValue = Handle.Value
                    ReturnType = Handle.Key.FullName
                Else '语法错误
                    Throw New SyntaxErrorException("") With {.ScriptLine = Command.CLICommandArgvs, .LineNumber = LineNumber}
                End If
            End If

            Return New Runtime.Objects.ObjectModels.ScriptCodeLine With
                   {
                       .PreExecuteType = ExeType,
                       .ReturnType = ReturnType,
                       .LineNumber = LineNumber, .LineFlag = LineNumber,
                       .VariableAssigned = VariableName,
                       .OrignialScriptLine = OriginalCommand,
                       .InvokeMethod = GetValue
            }
        End Function

        Private Function TryParseExtensionMethodSyntax(Command As String, LineNumber As Integer) As Runtime.Objects.ObjectModels.ScriptCodeLine
            Dim p As Integer = InStr(Command, " -> ")

            If Not p > 0 Then Return Nothing '为拓展函数语法

            Dim ExtVariable As String = Mid(Command, 1, p - 1).Trim

            If ExtVariable.First = """"c AndAlso ExtVariable.Last = """"c Then
                ExtVariable = Mid(ExtVariable, 2, Len(ExtVariable) - 2)  '在这里不需要TRIM操作了，因为是取目标双引号之中的字符串
            End If

            Dim CodeLine = InternalTryParse(Mid(Command, p + 3).Trim, LineNumber, ExtVariable)
            If InStr(ExtVariable, "call ") Then
                ExtVariable = Mid(ExtVariable, 6).Trim
            End If

            CodeLine.ExtensionMethodVariable = ExtVariable

            Return CodeLine
        End Function

        Public Const EXTENSION_OPERATOR As String = "[extension$operator]"

        Private Shared Function ReGenerateExtensionCommand(Command As CommandLine.CommandLine, ExtendedVariable As String) As CommandLine.CommandLine
            Dim Tokens = Command.Tokens.ToList
            Call Tokens.AddRange(New String() {EXTENSION_OPERATOR, ExtendedVariable})
            Return CommandLine.CommandLine.TryParse(CommandLine.CommandLine.Join(Tokens), DuplicatedAllowed:=True)
        End Function

        ''' <summary>
        ''' 这里是处理已经导入的命令
        ''' </summary>
        ''' <param name="cmdInfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function InternalGetImportsCommandEntryPoint(cmdInfo As CommandLine.CommandLine, ByRef ExeType As ShoalShell.Runtime.Objects.ObjectModels.ScriptCodeLine.PreExecuteTypes) As KeyValuePair(Of Type, Func(Of Object))
            Dim cmd = Me._RuntimeEnvironment.InternalEntryPointManager.MethodLoader.GetCommand(cmdInfo)

            ExeType = Runtime.Objects.ObjectModels.ScriptCodeLine.PreExecuteTypes.Normal

            If String.Equals(cmdInfo.Name, "imports", StringComparison.OrdinalIgnoreCase) OrElse String.Equals(cmdInfo.Name, "library", StringComparison.OrdinalIgnoreCase) Then
                Call cmd()
                If String.Equals(cmdInfo.Name, "imports", StringComparison.OrdinalIgnoreCase) Then
                    ExeType = Runtime.Objects.ObjectModels.ScriptCodeLine.PreExecuteTypes.Imports
                Else
                    ExeType = Runtime.Objects.ObjectModels.ScriptCodeLine.PreExecuteTypes.Library
                End If
                Return New KeyValuePair(Of Type, Func(Of Object))(GetType(String), Function() cmdInfo.CLICommandArgvs)
            ElseIf String.Equals(cmdInfo.Name, "for.each", StringComparison.OrdinalIgnoreCase) Then
                Me._InternalCommands._InternalVariableName_ForEach = cmdInfo("in")
                If Me._InternalCommands._InternalVariableName_ForEach.First = "$"c OrElse Me._InternalCommands._InternalVariableName_ForEach.First = "&"c Then
                    If Len(Me._InternalCommands._InternalVariableName_ForEach) > 1 Then
                        Me._InternalCommands._InternalVariableName_ForEach = Mid(Me._InternalCommands._InternalVariableName_ForEach, 2)
                    End If
                End If
                Return New KeyValuePair(Of Type, Func(Of Object))(GetType(Object), cmd)
            ElseIf String.Equals(cmdInfo.Name, "dowhile", StringComparison.OrdinalIgnoreCase) Then
                Me._InternalCommands._InternalVariableName_DoWhile = cmdInfo("test")
                If Me._InternalCommands._InternalVariableName_DoWhile.First = "$"c OrElse Me._InternalCommands._InternalVariableName_DoWhile.First = "&"c Then
                    If Len(Me._InternalCommands._InternalVariableName_DoWhile) > 1 Then
                        Me._InternalCommands._InternalVariableName_DoWhile = Mid(Me._InternalCommands._InternalVariableName_DoWhile, 2)
                    End If
                End If
                Return New KeyValuePair(Of Type, Func(Of Object))(GetType(Object), cmd)
            Else
                Return New KeyValuePair(Of Type, Func(Of Object))(GetType(Object), cmd)
            End If
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="castMethod">其实函数返回的也是一个集合</param>
        ''' <param name="value"></param>
        ''' <returns></returns>
        Private Function CastValue(castMethod As Func(Of Object(), Object), value As String) As Object

            Dim val As Object = Me.ScriptEngine.GetValue(value)
            val = castMethod({value})

            Dim IList = DirectCast(val, IEnumerable)
            Dim OnlyFirstValue = IList(0)
            Return OnlyFirstValue

        End Function

        ''' <summary>
        ''' 对于拓展方法而言，所调用的目标拓展方法之中的第一个参数为调用目标拓展方法的对象
        ''' </summary>
        ''' <param name="cmdInfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function TryParseCommand(cmdInfo As CommandLine.CommandLine, ExtendedVariable As String, ByRef ExeType As ShoalShell.Runtime.Objects.ObjectModels.ScriptCodeLine.PreExecuteTypes) As KeyValuePair(Of Type, Func(Of Object))
            If Not String.IsNullOrEmpty(ExtendedVariable) Then '假若目标为拓展方法，则会在这里重新生成命令行,添加一个-operator的参数
                cmdInfo = ReGenerateExtensionCommand(cmdInfo, ExtendedVariable)
            End If

            If Me._RuntimeEnvironment.InternalEntryPointManager.ExistsCommand(cmdInfo.Tokens.First) Then '这里是处理已经导入的命令
                Return InternalGetImportsCommandEntryPoint(cmdInfo, ExeType)
            End If

            If cmdInfo.Tokens.Count = 1 Then  '仅解析出一个词元的时候，假若认为目标对象为AssemblyName但是找不到方法，所以认为其应该是一个赋值用的变量
                Dim GetValue As Func(Of Object) = Function() As Object
                                                      Dim var As String = cmdInfo.Tokens.First()
                                                      Dim obj As Object = If(_RuntimeEnvironment._EngineMemoryDevice.ExistsVariable(var),
                                                                             _RuntimeEnvironment._EngineMemoryDevice.TryGetValue(var),
                                                                             _RuntimeEnvironment._EngineMemoryDevice.FormatString(var))
                                                      Call Console.WriteLine("    = [0] {0}", obj.ToString)
                                                      Return obj
                                                  End Function
                Return New KeyValuePair(Of Type, Func(Of Object))(GetType(String), GetValue)
            End If

            If Regex.Match(cmdInfo.Tokens(0), "\(.+?\)").Success Then

                ' 进行强制类型转换
                Dim TypeRef As String = Mid(cmdInfo.Tokens(0), 2, Len(cmdInfo.Tokens(0)) - 2)
                Dim value As String = cmdInfo.Tokens(1)

                Dim castMethod As Func(Of Object(), Object) = BuildInModules.System.Array.CastedTypes(TypeRef.ToLower)
                Return New KeyValuePair(Of Type, Func(Of Object))(GetType(Object), Function() CastValue(castMethod, value))

            End If

            Dim AssemblyName As String = cmdInfo.Tokens.First
            Dim Command As String = Mid(cmdInfo.CLICommandArgvs, 1 + Len(AssemblyName)).Trim
            Dim cmdName As String = cmdInfo.Tokens(1)

            If InStr(AssemblyName, "::") > 0 Then
                Dim Tokens As String() = Strings.Split(AssemblyName, "::")
                AssemblyName = Tokens.First
                cmdName = Tokens.Last
                Command = "system->invoke() " & Command
            End If

            '下面的代码是处理没有导入的方法，所有的通过命名空间导入的方法都是共享方法

            Dim MethodInfo As System.Reflection.MethodInfo() = _DelegateRegistry.GetMethod(AssemblyName, cmdName, _RuntimeEnvironment._EngineMemoryDevice)
            Dim Invoke As Func(Of Object)
            Dim ReturnType As Type

            If MethodInfo.Count = 1 Then
                Dim EntryPoint = MethodInfo.First
                Invoke = Function() Me._InternalMethodInvoker.CallMethod(EntryPoint, Command, _RuntimeEnvironment._EngineMemoryDevice)
                ReturnType = EntryPoint.ReturnType
            Else
                Dim CallEntry = New ShoalShell.DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint("VB$AnonymousDirectlyNamespace.Invoke", MethodInfo)
                Invoke = Function() Me._InternalMethodInvoker.CallMethod(EntryPoint:=CallEntry, argvs:=Command, MemoryDevice:=_RuntimeEnvironment._EngineMemoryDevice, TypeSignature:="")
                ReturnType = GetType(System.Object)
            End If

            Return New KeyValuePair(Of Type, Func(Of Object))(ReturnType, Invoke)
        End Function

        Public Function TryGetCommand(Command As String) As DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint
            Dim cmdInfo = CommandLine.CommandLine.TryParse(Command)

            If Me._RuntimeEnvironment.InternalEntryPointManager.ExistsCommand(cmdInfo.Tokens.First) Then
                Return Me.ScriptEngine.InternalEntryPointManager(cmdInfo.Tokens.First)
            End If

            If cmdInfo.Tokens.Count = 1 Then  '仅解析出一个词元的时候，假若认为目标对象为AssemblyName但是找不到方法，抛出错误
                Throw New SyntaxErrorException("Command name was not specific!") With {.ScriptLine = Command}
            End If

            Dim AssemblyName As String = cmdInfo.Tokens.First : Command = Mid(Command, 1 + Len(AssemblyName)).Trim
            Dim cmdName As String = cmdInfo.Tokens(1)
            Dim MethodInfo As System.Reflection.MethodInfo() = _DelegateRegistry.GetMethod(AssemblyName, cmdName, _RuntimeEnvironment._EngineMemoryDevice)

            Return DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint.NamespaceDirectlyCalled(MethodInfo)
        End Function
    End Class
End Namespace