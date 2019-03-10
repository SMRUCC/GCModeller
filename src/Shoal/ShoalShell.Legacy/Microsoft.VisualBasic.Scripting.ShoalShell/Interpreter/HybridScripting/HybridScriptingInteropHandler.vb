Imports System.Reflection

Namespace HybridsScripting

    Public Class HybridScriptingInteropHandler : Inherits ShoalShell.Runtime.Objects.ObjectModels.IScriptEngineComponent

        Implements System.IDisposable

        Public Structure EntryPoint

            ''' <summary>
            ''' Script name.(混合编程的脚本名称)
            ''' </summary>
            ''' <remarks></remarks>
            Dim Name As String
            Dim Init, Evaluate, SetValue As System.Reflection.MethodInfo
            ''' <summary>
            ''' Basic type data convert interface.(基本数据类型的转换接口)
            ''' </summary>
            ''' <remarks></remarks>
            Dim DataConvertors As KeyValuePair(Of Char, System.Reflection.MethodInfo)()
            ''' <summary>
            ''' When the <see cref="HybridScriptingInteropHandler.Evaluate">handlers</see> can not found the data type 
            ''' convert method for the basically type then it will try this system preserved string type convert 
            ''' method to convert the data as string as default.
            ''' (当<see cref="DataConvertors"></see>之中没有查找到目标类型的转换操作接口的时候，则默认使用本方法转换
            ''' 为字符串的格式，保留的字符串类型的转换函数)
            ''' </summary>
            ''' <remarks></remarks>
            Dim ConservedString As System.Reflection.MethodInfo
            Dim TypeFullName As String
            Dim DeclaredAssemblyType As System.Type

            Public Overrides Function ToString() As String
                Return Name
            End Function

            Public Sub InitInvoke()
                If Not Init Is Nothing Then
                    Call Init.Invoke(Nothing, Nothing)
                End If
            End Sub

            ''' <summary>
            '''This property indicated that the entry data which was parsing from the assembly module is valid or not.(可以使用本属性来判断目标解析数据是否可用)
            ''' </summary>
            ''' <value></value>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public ReadOnly Property IsNull As Boolean
                Get
                    Return String.IsNullOrEmpty(Name) OrElse Evaluate Is Nothing OrElse SetValue Is Nothing
                End Get
            End Property
        End Structure

        Dim _CurrentScriptPoint As Func(Of String, Object)
        Dim _CurrentSetValue As Func(Of String, Object, Boolean)
        Dim _EntryPoints As Dictionary(Of String, EntryPoint) = New Dictionary(Of String, EntryPoint)

        ''' <summary>
        ''' Attach the hybrid scripting environment onto the script host.
        ''' </summary>
        ''' <param name="pointName"></param>
        ''' <remarks></remarks>
        Public Sub AttachEntryPoint(pointName As String)
            Dim EntryPoint = _EntryPoints(pointName.ToLower)

            Call Console.WriteLine("Dynamic load the external ""{0}"" runtime enviroment entry point, this may taks a while...", EntryPoint.Name)
            Call EntryPoint.InitInvoke()
            _CurrentScriptPoint = Function(script As String) EntryPoint.Evaluate.Invoke(Nothing, {script})
            _CurrentSetValue = Function(variable As String, value As Object) EntryPoint.SetValue.Invoke(Nothing, {variable, value})

            If EntryPoint.ConservedString Is Nothing Then
                Call Console.WriteLine("[WARNING] Empty string type convertor, data convertion maybe failure in the hybrid scripting!")
                ConservedStringConvert = Nothing
            Else
                ConservedStringConvert = Function(value As String) EntryPoint.ConservedString.Invoke(Nothing, {value})
            End If

            Call DataConverters.Clear()
            For Each Convertor In EntryPoint.DataConvertors
                Call DataConverters.Add(Convertor.Key, Function(value As String) Convertor.Value.Invoke(Nothing, {value}))
            Next

            Call Console.WriteLine("Load entry point and connect to the ""{0}"" runtime enviroment successfully!", EntryPoint.Name)
        End Sub

        Public Function Evaluate(script As String) As Object
            script = ScriptEngine._EngineMemoryDevice.FormatString(script)
            Return _CurrentScriptPoint(script)
        End Function

        ''' <summary>
        ''' 这个对象是为<see cref="Evaluate"></see>所准备的，当所传递的参数为字符串的时候，则可以根据参数名的后缀来转换为相应的数据类型
        ''' </summary>
        ''' <remarks>
        ''' </remarks>
        Dim DataConverters As Dictionary(Of Char, Func(Of String, Object)) = New Dictionary(Of Char, Func(Of String, Object))
        Dim ConservedStringConvert As Func(Of String, Object)

        Sub New(ScriptHost As ShoalShell.Runtime.Objects.ShellScript)
            Call MyBase.New(ScriptHost)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="environment">这个参数需要已经去除了第一个!标识符，会临时切换混合编程的脚本环境</param>
        ''' <param name="Cmdl">脚本对象名[ 参数列表]</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Evaluate(environment As String, Cmdl As CommandLine.CommandLine) As Object
            Dim ScriptText As String = ScriptEngine.InternalEntryPointManager.GetHybridsScript(Cmdl.Name)

            Using TempEnvironment As HybridScriptingInteropHandler = New HybridScriptingInteropHandler(Me.ScriptEngine)
                TempEnvironment._EntryPoints = _EntryPoints
                Call TempEnvironment.AttachEntryPoint(environment)

                For Each var In Cmdl '其中的参数的值为变量引用，假若找不到变量值，则一律转换为字符串
                    Dim value As Object = ScriptEngine._Interpreter._InternalCommands.GetValue(var.Value)

                    If value.GetType = GetType(String) Then '说明不存在目标变量，则现在需要使用类型转换来转换目标字符串参数值
                        Dim type As Char = var.Key.Last

                        If TempEnvironment.DataConverters.ContainsKey(type) Then
                            value = TempEnvironment.DataConverters(type)(value.ToString)
                        Else
                            value = TempEnvironment.ConservedStringConvert(value.ToString)  '假若找不到转换器，则默认转换为字符串
                        End If
                    End If
                    Call TempEnvironment.SetValue(var.Key, value)
                Next

                ScriptText = ScriptEngine._EngineMemoryDevice.FormatString(ScriptText)

                Return TempEnvironment.Evaluate(ScriptText)
            End Using
        End Function

        Public Function SetValue(variableName As String, value As Object) As Boolean
            variableName = ScriptEngine._EngineMemoryDevice.FormatString(variableName)
            Return _CurrentSetValue(variableName, value)
        End Function

        Public Function LoadEntryPoints(AssemblyFile As String) As Integer
            If Not FileIO.FileSystem.FileExists(AssemblyFile) Then
                Call Console.WriteLine("!!! Missing Module @ {0}.", AssemblyFile)
                Return -1
            End If

            Dim EntryPoints = _LoadEntryPoints(System.Reflection.Assembly.LoadFrom(AssemblyFile))
            If EntryPoints.IsNullOrEmpty Then
                Return 0
            End If

            For Each EntryPoint In EntryPoints
                Dim Name As String = EntryPoint.Name.ToLower

                If _EntryPoints.ContainsKey(Name) Then
                    Call Console.WriteLine("Script entry point ""{0}"": {1}::{2} is already mount in the shellscript", Name, EntryPoint.TypeFullName, EntryPoint.Init.Name)
                    Return 0
                End If

                Call _EntryPoints.Add(Name, EntryPoint)
                Call ScriptEngine.ImportsDataSource.Imports(EntryPoint.DeclaredAssemblyType)
            Next

            Return EntryPoints.Count
        End Function

        Private Function _LoadEntryPoints(Assembly As System.Reflection.Assembly) As EntryPoint()
            Dim LQuery = (From Type As System.Type
                          In Assembly.GetTypes
                          Let EntryPoint As EntryPoint = LoadEntryPoint(Type)
                          Where Not EntryPoint.IsNull
                          Select EntryPoint).ToArray
            Return LQuery
        End Function

        Private Function LoadEntryPoint(Assembly As System.Type) As EntryPoint
            Dim attributes As Object() = Assembly.GetCustomAttributes(ShoalShell.HybridsScripting.ScriptEntryPoint.TypeInfo, True)

            If attributes.IsNullOrEmpty Then
                Return Nothing
            End If

            Dim EntryName As String = DirectCast(attributes(0), ShoalShell.HybridsScripting.ScriptEntryPoint).ScriptName
            Dim InitEntry As System.Reflection.MethodInfo = GetEntry(Assembly, ScriptEntryInterface.InterfaceTypes.EntryPointInit)
            Dim Evaluate As System.Reflection.MethodInfo = GetEntry(Assembly, ScriptEntryInterface.InterfaceTypes.Evaluate)
            Dim SetValue As System.Reflection.MethodInfo = GetEntry(Assembly, ScriptEntryInterface.InterfaceTypes.SetValue)
            Dim DataConvertors = GetEntries(Of HybridsScripting.DataConvert)(Assembly)
            Dim ConservedString As System.Reflection.MethodInfo = (From cMethod As KeyValuePair(Of HybridsScripting.DataConvert, System.Reflection.MethodInfo)
                                                                   In DataConvertors
                                                                   Where cMethod.Key.ConservedStringConvertor = True
                                                                   Select cMethod.Value).ToArray.FirstOrDefault
            If Evaluate Is Nothing Then
                Return Nothing
            Else
                Return New EntryPoint With
                       {
                           .DeclaredAssemblyType = Assembly,
                           .ConservedString = ConservedString,
                           .Name = EntryName,
                           .Init = InitEntry,
                           .Evaluate = Evaluate,
                           .TypeFullName = Assembly.FullName,
                           .SetValue = SetValue,
                           .DataConvertors = (From item In DataConvertors Select New KeyValuePair(Of Char, System.Reflection.MethodInfo)(item.Key.TypeChar, item.Value)).ToArray}
            End If
        End Function

        Private Function GetEntries(Of TEntryType As ScriptEntryInterface)(TypeInfo As System.Type) As KeyValuePair(Of TEntryType, System.Reflection.MethodInfo)()
            Dim EntryType As Type = GetType(TEntryType)
            Dim LQuery = (From LoadHandle As System.Reflection.MethodInfo
                          In TypeInfo.GetMethods(BindingFlags.Public Or BindingFlags.Static)
                          Let attributes As Object() = LoadHandle.GetCustomAttributes(EntryType, False)
                          Where Not attributes.IsNullOrEmpty
                          Select (From attr As Object
                                  In attributes
                                  Let Entry As TEntryType = DirectCast(attr, TEntryType)
                                  Select New KeyValuePair(Of TEntryType, System.Reflection.MethodInfo)(Entry, LoadHandle)).ToArray).ToArray
            Return LQuery.MatrixToVector
        End Function

        Private Function GetEntry(TypeInfo As System.Type, EntryType As ScriptEntryInterface.InterfaceTypes) As System.Reflection.MethodInfo
            Dim LQuery = (From LoadHandle As System.Reflection.MethodInfo
                          In TypeInfo.GetMethods(BindingFlags.Public Or BindingFlags.Static)
                          Let attributes As Object() = LoadHandle.GetCustomAttributes(ShoalShell.HybridsScripting.ScriptEntryInterface.TypeInfo, False)
                          Where Not attributes.IsNullOrEmpty AndAlso DirectCast(attributes(0), ShoalShell.HybridsScripting.ScriptEntryInterface).InterfaceType = EntryType
                          Select LoadHandle).ToArray
            Return LQuery.FirstOrDefault
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

    End Class
End Namespace