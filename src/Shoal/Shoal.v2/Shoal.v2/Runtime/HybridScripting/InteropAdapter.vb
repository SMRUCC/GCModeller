Imports System.Reflection
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.SCOM

Namespace Runtime.HybridsScripting

    ''' <summary>
    ''' 和外部脚本进行编程计算
    ''' </summary>
    ''' <param name="Line">外部脚本的脚本命令行</param>
    ''' <returns>返回所输入的脚本的文本行所执行的结果</returns>
    Delegate Function Evaluation(Line As String) As Object
    ''' <summary>
    ''' 从Shoal语言环境之中向外部脚本环境设置变量的值
    ''' </summary>
    ''' <param name="var">在外部脚本环境之中的变量的名称</param>
    ''' <param name="value">值，需要进行类型转换的</param>
    ''' <returns>是否成功</returns>
    Delegate Function SetValue(var As String, value As Object) As Boolean

    Public Class InteropAdapter : Inherits RuntimeComponent
        Implements IDisposable

        ''' <summary>
        ''' 小写的键名称
        ''' </summary>
        Dim _Environments As SortedDictionary(Of String, EntryPoint) = New SortedDictionary(Of String, EntryPoint)
        Dim _currentEnvironment As EntryPoint

        ''' <summary>
        ''' Attach the hybrid scripting environment onto the script host.
        ''' </summary>
        ''' <param name="pointName"></param>
        ''' <remarks></remarks>
        Public Function RedirectStream(pointName As String) As Boolean
            _currentEnvironment = _Environments(pointName.ToLower)

            Call $"Dynamic load the external ""{ _currentEnvironment.Language.Language}"" runtime enviroment entry point, this may taks a while...".__DEBUG_ECHO
            Call _currentEnvironment.InitInvoke()

            If _currentEnvironment.ConservedString Is Nothing Then
                Call "[WARNING] Empty string type convertor, data convertion maybe failure in the hybrid scripting!".__DEBUG_ECHO
            End If

            Call $"Load entry point and connect to the ""{ _currentEnvironment.Language.Language}"" runtime enviroment successfully!".__DEBUG_ECHO

            Return True
        End Function

        ''' <summary>
        ''' 这个函数会自动调用<see cref="Runtime.ScriptEngine.Strings"/>进行格式化的
        ''' </summary>
        ''' <param name="script"></param>
        ''' <returns></returns>
        Public Function Evaluate(script As String) As Object
            script = ScriptEngine.Strings.Format(script)
            Return _currentEnvironment.EvaluateInvoke(script)
        End Function

        Sub New(ScriptEngine As ShoalShell.Runtime.ScriptEngine)
            Call MyBase.New(ScriptEngine)

            If Not ScriptEngine.PMgrDb.HybridEnvironments.IsNullOrEmpty Then
                For Each Environment In ScriptEngine.PMgrDb.HybridEnvironments
                    Call [Imports](Environment)
                Next
            End If
        End Sub

        '''' <summary>
        '''' 
        '''' </summary>
        '''' <param name="envir">这个参数需要已经去除了第一个!标识符，会临时切换混合编程的脚本环境</param>
        '''' <param name="Cmdl">脚本对象名[ 参数列表]</param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Function Evaluate(envir As String, Cmdl As CommandLine.CommandLine) As Object
        '    Dim ScriptText As String '= ScriptEngine.InternalEntryPointManager.GetHybridsScript(Cmdl.Name)
        '    Dim Environment = _Environments(envir.ToLower)

        '    For Each var In Cmdl '其中的参数的值为变量引用，假若找不到变量值，则一律转换为字符串
        '        Dim value As Object = ScriptEngine.MMUDevice.GetValue(var.Value)

        '        If value.GetType = GetType(String) Then '说明不存在目标变量，则现在需要使用类型转换来转换目标字符串参数值
        '            Dim type As Char = var.Key.Last

        '            If Environment.DataConvertors.ContainsKey(type) Then
        '                value = Environment.Converts(type, InputHandler.ToString(value))
        '            Else
        '                value = Environment.ReservedConvert(InputHandler.ToString(value))  '假若找不到转换器，则默认转换为字符串
        '            End If
        '        End If
        '        Call Environment.SetValueInvoke(var.Key, value)
        '    Next

        '    ScriptText = ScriptEngine.Strings.Format(ScriptText)

        '    Return Environment.EvaluateInvoke(ScriptText)
        'End Function

        Public Function SetValue(variableName As String, value As Object) As Boolean
            variableName = ScriptEngine.Strings.Format(variableName)
            Return _currentEnvironment.SetValueInvoke(variableName, value)
        End Function

        Public Function [Imports](Environment As SPM.Nodes.HybridEnvir) As Boolean
            Dim assm As Assembly = Environment.LoadAssembly

            If assm Is Nothing Then
                Call $"!!! Missing Module @ {Environment.Path.ToFileURL}..".__DEBUG_ECHO
                Return False
            End If

            Dim EntryPoint = EnvironmentParser.[Imports](assm.GetType(Environment.TypeId))

            If EntryPoint.IsNull Then
                Return False
            End If

            Dim name As String = EntryPoint.Language.Language.ToLower

            If _Environments.ContainsKey(name) Then
                Call $"Script entry point ""{name}"": { EntryPoint.TypeFullName}::{EntryPoint.Init.Name} is already mount in the shellscript".__DEBUG_ECHO
                Call _Environments.Remove(name)
            End If

            Call _Environments.Add(name, EntryPoint)
            Call ScriptEngine.MMUDevice.MappingImports.Imports(EntryPoint.DeclaredAssemblyType)

            Return True
        End Function
    End Class
End Namespace