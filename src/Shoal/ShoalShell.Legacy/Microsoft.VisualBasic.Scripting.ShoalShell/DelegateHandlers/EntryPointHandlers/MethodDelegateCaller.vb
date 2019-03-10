Imports System.Text.RegularExpressions
Imports System.Text

Namespace DelegateHandlers.EntryPointHandlers

    ''' <summary>
    ''' 对于第一个参数为脚本引擎的类型，则可以在调用的时候直接忽略第一个参数
    ''' </summary>
    ''' <remarks></remarks>
    Public Class MethodDelegateCaller : Inherits ShoalShell.Runtime.Objects.ObjectModels.IScriptEngineComponent

        ''' <summary>
        ''' This method required the host memory, when this attribute was applied on a method, 
        ''' then the host memory object will be append to the last of the parameter value list.
        ''' </summary>
        ''' <remarks></remarks>
        <AttributeUsage(AttributeTargets.Method, allowmultiple:=False, inherited:=True)>
        Public Class RequiredHostMemory : Inherits Attribute

            Public Overrides Function ToString() As String
                Return "This method required the host memory."
            End Function
        End Class

        Private ReadOnly CommandLineTypeName As String = GetType(Microsoft.VisualBasic.CommandLine.CommandLine).FullName
        Protected Shared ReadOnly StringTYPE As System.Type = GetType(String)

        Sub New(ScriptEngine As ShoalShell.Runtime.Objects.ShellScript)
            Call MyBase.New(ScriptEngine:=ScriptEngine)
        End Sub

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <returns>参数名，参数类型</returns>
        ''' <remarks></remarks>
        Public Function GetParameters([Delegate] As System.Reflection.MethodInfo) As KeyValuePair(Of Microsoft.VisualBasic.Scripting.EntryPointMetaData.ParameterAlias, System.Type)()
            Dim Parameters As System.Reflection.ParameterInfo() = [Delegate].GetParameters
            Dim LQuery = (From pInfo As System.Reflection.ParameterInfo
                          In Parameters
                          Let ap = Microsoft.VisualBasic.Scripting.EntryPointMetaData.ParameterAlias.GetParameterNameAlias(pInfo, [Default]:=Nothing)
                          Select New KeyValuePair(Of Microsoft.VisualBasic.Scripting.EntryPointMetaData.ParameterAlias, System.Type)(ap, pInfo.ParameterType)).ToArray '  由于拓展方法传递参数的需要，请不要排序而将原始顺序给打乱
            Return LQuery
        End Function

        Const EXCEPTION_METHOD_MISSING_REQUIRED_PARAMETER As String = "MISSING_PARAMETER:: Could not found the required parameter ""{0}""!"

        Private Shared Function InternalCallCommandLine(Parameter As System.Reflection.ParameterInfo,
                                                        _Delegate As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo,
                                                        argvs As CommandLine.CommandLine,
                                                        MemoryBlocks As Runtime.Objects.I_MemoryManagementDevice) As Object
            If argvs.Parameters.IsNullOrEmpty Then

                If Parameter.IsOptional Then
                    Return InternalDelegateInvoke(_Delegate, New Object() {Parameter.DefaultValue}, MemoryBlocks.ScriptEngine)
                Else
                    Dim ExMessage As String = String.Format(EXCEPTION_METHOD_MISSING_REQUIRED_PARAMETER, Parameter.Name)
                    Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.ScriptRunTimeException(ExMessage, MemoryBlocks)
                End If
            End If

            If argvs.Parameters.First.First = "$" Then  '命令行对象的参数之中只有最基本的数据类型
                Dim strValue = MemoryBlocks.Item(Mid(argvs.Parameters.First, 2)).ToString
                argvs = CommandLine.CommandLine.TryParse("call " & strValue)
            ElseIf argvs.Parameters.First.First = "&" Then
                Dim strValue = MemoryBlocks.GetConstant(Mid(argvs.Parameters.First, 2)).ToString
                argvs = CommandLine.CommandLine.TryParse("call " & strValue)
            End If

            Return InternalDelegateInvoke(_Delegate, New Object() {argvs}, MemoryBlocks.ScriptEngine)
        End Function

        Private Shared Function InternalCallOnlyOneParameterMethod(Parameter As System.Reflection.ParameterInfo,
                                                                   _Delegate As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo,
                                                                   argvs As CommandLine.CommandLine,
                                                                   MemoryDevice As Runtime.Objects.I_MemoryManagementDevice) As Object
            If argvs.Parameters.IsNullOrEmpty Then
                If Parameter.IsOptional Then
                    Return InternalDelegateInvoke(_Delegate, New Object() {Parameter.DefaultValue}, MemoryDevice.ScriptEngine)
                ElseIf Parameter.ParameterType = GetType(ShoalShell.Runtime.Objects.ShellScript) Then
                    Return InternalDelegateInvoke(_Delegate, New Object() {MemoryDevice.ScriptEngine}, MemoryDevice.ScriptEngine)
                Else
                    Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.ScriptRunTimeException(String.Format(EXCEPTION_METHOD_MISSING_REQUIRED_PARAMETER, Parameter.Name), MemoryDevice)
                End If
            End If

            Dim argvsValue As String = argvs.Parameters.First '只有一个参数则可以直接忽略参数名，这里得到的是参数值的表达式，对于拓展方法，这里需要额外的处理
            If String.Equals(argvsValue, Parameter.Name, StringComparison.OrdinalIgnoreCase) Then
                '当第一个参数和目标函数的参数名一致的时候，则认为第二个参数才是真正想要的参数，对于其他的任意情况，任然维持原来的形式
                If argvs.Parameters.Count > 1 Then
                    argvsValue = argvs.Parameters(1)
                End If
            End If

            If String.Equals(ShoalShell.Interpreter.Interpreter.EXTENSION_OPERATOR, argvsValue) Then
                argvsValue = argvs.Parameters(1) '对于拓展方法，列表之中的第二个元素才是想要的参数信息
            End If

            Dim paraValue As Object = InternalGetVariable(Parameter, argvsValue, MemoryDevice)

            Return InternalDelegateInvoke(_Delegate, New Object() {paraValue}, MemoryDevice.ScriptEngine)
        End Function

        Private Shared Function InternalGetVariable(p As System.Reflection.ParameterInfo, refValue As String, MemoryDevice As ShoalShell.Runtime.Objects.I_MemoryManagementDevice) As Object
            Dim pType As Type = p.ParameterType

            If pType = StringTYPE Then
                refValue = MemoryDevice.FormatString(refValue)
                Return refValue
            End If

            If refValue.First = "$"c Then
                refValue = Mid(refValue, 2)
                Dim paraValue = MemoryDevice.Item(refValue)
                If InternalCreateBasicValue.ContainsKey(pType) Then
                    paraValue = InternalCreateBasicValue(pType)(paraValue)
                End If
                Return paraValue
            ElseIf refValue.First = "&"c Then
                Return MemoryDevice.GetConstant(Mid(refValue, 2))
            ElseIf refValue.First = "*"c Then
                Return MemoryDevice.ScriptEngine.InternalEntryPointManager.MethodLoader.GetCommand(Mid(refValue, 2))
            ElseIf String.Equals(refValue, "%") Then
                Return MemoryDevice
            Else
                If InternalCreateBasicValue.ContainsKey(pType) Then
                    Dim objValue = InternalCreateBasicValue(pType)(refValue)

                    Return objValue
                Else
                    Return refValue
                End If
            End If
        End Function

        Private Shared Function InternalCallMultipleParametersMethod(Parameters As KeyValuePair(Of Microsoft.VisualBasic.Scripting.EntryPointMetaData.ParameterAlias, Type)(),
                                                                     _Delegate As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo,
                                                                     argvs As CommandLine.CommandLine,
                                                                     MemoryDevice As Runtime.Objects.I_MemoryManagementDevice) As Object

            Dim paraList As List(Of Object) = New System.Collections.Generic.List(Of Object)  '在这里生成与Parameters中的元素的顺序一致的参数列表

            If argvs.ContainsParameter(Interpreter.Interpreter.EXTENSION_OPERATOR) Then
                If Parameters.First.Value = GetType(ShoalShell.Runtime.Objects.ShellScript) Then
                    Call argvs.Add(Parameters(1).Key.Alias, argvs(Interpreter.Interpreter.EXTENSION_OPERATOR))
                Else
                    Call argvs.Add(Parameters.First.Key.Alias, argvs(Interpreter.Interpreter.EXTENSION_OPERATOR))
                End If
            End If

            If Parameters.First.Value = GetType(ShoalShell.Runtime.Objects.ShellScript) Then
                Call paraList.Add(MemoryDevice.ScriptEngine)
                Parameters = Parameters.Skip(1).ToArray
            End If

            For Each p As KeyValuePair(Of Microsoft.VisualBasic.Scripting.EntryPointMetaData.ParameterAlias, Type) In Parameters
                Dim valueRef As String = argvs.Item(p.Key.Alias)

                If String.IsNullOrEmpty(valueRef) Then
                    If p.Key.AssociatedParameterInfo.IsOptional Then
                        Call paraList.Add(p.Key.AssociatedParameterInfo.DefaultValue)
                        Continue For
                    End If

                    '说明在用户传递进来的参数列表中找不到函数调用说需要的一个参数，则抛出错误
                    Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.ScriptRunTimeException(String.Format(EXCEPTION_METHOD_MISSING_REQUIRED_PARAMETER, p.Key), MemoryDevice)
                End If

                Call paraList.Add(InternalGetVariable(p.Key.AssociatedParameterInfo, valueRef, MemoryDevice))
            Next

            Return InternalDelegateInvoke(_Delegate, paraList.ToArray, MemoryDevice.ScriptEngine)
        End Function

        ''' <summary>
        ''' 这个函数处理共享方法和实例方法的调用
        ''' </summary>
        ''' <param name="Delegate"></param>
        ''' <param name="argvs"></param>
        ''' <param name="MemoryDevice"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CallMethod([Delegate] As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo,
                                   argvs As CommandLine.CommandLine,
                                   MemoryDevice As Runtime.Objects.I_MemoryManagementDevice) As Object

            Dim Parameters = GetParameters([Delegate].MethodEntryPoint)  '首先得到方法的参数列表

            If Parameters.IsNullOrEmpty Then '这个方法是不需要任何参数的
                Return InternalDelegateInvoke([Delegate], New Object() {}, Me.ScriptEngine)
            ElseIf Parameters.Count = 1 AndAlso String.Equals(Parameters.First.Value.FullName, CommandLineTypeName) Then '这个方法仅包含有一个参数，并且为命令行格式，则直接传递参数
                Return InternalCallCommandLine(Parameters.First.Key.AssociatedParameterInfo, [Delegate], argvs, MemoryDevice)
            ElseIf Parameters.Count = 1 Then  '该方法仅包含有一个参数，则直接从内存中查找出变量然后赋值
                Return InternalCallOnlyOneParameterMethod(Parameters.First.Key.AssociatedParameterInfo, [Delegate], argvs, MemoryDevice)
            Else
                Return InternalCallMultipleParametersMethod(Parameters, [Delegate], argvs, MemoryDevice)
            End If
        End Function

        ''' <summary>
        ''' 这个方法处理共享方法的调用
        ''' </summary>
        ''' <param name="Delegate"></param>
        ''' <param name="argvs"></param>
        ''' <param name="MemoryDevice"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CallMethod([Delegate] As System.Reflection.MethodInfo, argvs As CommandLine.CommandLine, MemoryDevice As Runtime.Objects.I_MemoryManagementDevice) As Object
            Dim EntryInfo As New Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo(New CommandLine.Reflection.CommandAttribute("VB$AnonymousSharedMethod"), Invoke:=[Delegate])
            Return CallMethod(EntryInfo, argvs, MemoryDevice)
        End Function

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="EntryPoint"></param>
        ''' <param name="argvs"></param>
        ''' <param name="MemoryDevice"></param>
        ''' <param name="TypeSignature">函数会根据这个值来决定重载函数的调用，可以为空字符串</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CallMethod(EntryPoint As ShoalShell.DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint,
                                   argvs As CommandLine.CommandLine,
                                   MemoryDevice As Runtime.Objects.I_MemoryManagementDevice,
                                   TypeSignature As String) As Object

            If Not EntryPoint.IsOverloaded Then '目标函数不是一个重载函数，则直接调用
                Return CallMethod(EntryPoint.NonOverloadsMethod, argvs, MemoryDevice)
            Else
                Return InternalCallOverloadsMethod(EntryPoint, argvs, MemoryDevice, TypeSignature)
            End If
        End Function

        Private Function InternalCallOverloadsMethod(EntryPoint As ShoalShell.DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint,
                                                     argvs As CommandLine.CommandLine,
                                                     MemoryDevice As Runtime.Objects.I_MemoryManagementDevice,
                                                     TypeSignature As String) As Object

            Dim paraList As Dictionary(Of String, Object) = Nothing

            If argvs.Parameters.Count = 1 Then
                paraList = New Dictionary(Of String, Object)
                paraList.Add(Interpreter.Interpreter.EXTENSION_OPERATOR, MemoryDevice.TryGetValue(argvs.Parameters.First))       '函数可能只有一个参数
            Else
                paraList = InternalGetVariable(argvs, MemoryDevice)
            End If

            Dim CalledMethodEntryPoint = FindOverloadsMethod(EntryPoint, paraList, MemoryDevice, TypeSignature)
            Return CallMethod(CalledMethodEntryPoint, argvs, MemoryDevice)
        End Function

        Public Function FindOverloadsMethod(EntryPoint As ShoalShell.DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint,
                                                     argvs As Dictionary(Of String, Object),
                                                     MemoryDevice As Runtime.Objects.I_MemoryManagementDevice,
                                                     TypeSignature As String) As CommandLine.Reflection.EntryPoints.CommandEntryPointInfo
            Dim ParameterTypeSignature = (From item As KeyValuePair(Of String, Object) In argvs
                                          Select KEY = item.Key.ToLower,
                                                 TYPE = item.Value.GetType).ToArray.ToDictionary(keySelector:=Function(item) item.KEY, elementSelector:=Function(item) item.TYPE)
            Dim CalledMethodEntryPoint = EntryPoint.getMethodInfo(ParameterTypeSignature, TypeSignature)
            Return CalledMethodEntryPoint
        End Function

        ''' <summary>
        ''' 拓展方法的函数肯定会有一个参数，并且该函数的第一个参数为拓展方法的目标参数对象
        ''' </summary>
        ''' <param name="argvs"></param>
        ''' <param name="MemoryDevice"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function InternalGetVariable(argvs As CommandLine.CommandLine, MemoryDevice As Runtime.Objects.I_MemoryManagementDevice) As Dictionary(Of String, Object)
            Dim paraDict As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
            Dim ExtTemp As String = ""

            If argvs.ContainsParameter(Interpreter.Interpreter.EXTENSION_OPERATOR) Then
                Dim ref As String = argvs(Interpreter.Interpreter.EXTENSION_OPERATOR)
                Call paraDict.Add(Interpreter.Interpreter.EXTENSION_OPERATOR, MemoryDevice.TryGetValue(ref))
                Call argvs.Remove(Interpreter.Interpreter.EXTENSION_OPERATOR)
                ExtTemp = ref

                For Each p In argvs.ToArray
                    Dim value As Object = MemoryDevice.TryGetValue(p.Value)  '获取参数引用表达式所引用的在内存设备之中的映射数据
                    Call paraDict.Add(p.Key, value)
                Next

                Call argvs.Add(Interpreter.Interpreter.EXTENSION_OPERATOR, ExtTemp)
            Else
                For Each p In argvs.ToArray
                    Dim value As Object = MemoryDevice.TryGetValue(p.Value)  '获取参数引用表达式所引用的在内存设备之中的映射数据
                    Call paraDict.Add(p.Key, value)
                Next
            End If

            Return paraDict
        End Function

        Private Shared Function InternalDelegateInvoke(EntryPointHandle As Microsoft.VisualBasic.CommandLine.Reflection.EntryPoints.CommandEntryPointInfo, argvs As Object(), ScriptEngine As ShoalShell.Runtime.Objects.ShellScript) As Object
            If EntryPointHandle.MethodEntryPoint.IsStatic Then Return EntryPointHandle.Invoke(argvs, Nothing)

            Dim Entry As System.Type = EntryPointHandle.MethodEntryPoint.DeclaringType
            Dim Target As Object = If(Entry = GetType(ShoalShell.Interpreter.InternalCommands), ScriptEngine._Interpreter._InternalCommands, EntryPointHandle.InvokeOnObject)

            Return EntryPointHandle.Invoke(argvs, Target)
        End Function

        ''' <summary>
        ''' 基本数据类型的转换操作
        ''' </summary>
        ''' <remarks></remarks>
        Friend Shared ReadOnly InternalCreateBasicValue As Dictionary(Of System.Type, Func(Of Object, Object)) =
            New Dictionary(Of Type, Func(Of Object, Object)) From {
 _
                {GetType(Long), Function(Value As Object) CLng(Val(Value.ToString))},
                {GetType(Single), Function(value As Object) CSng(Val(value.ToString))},
                {GetType(Double), Function(value As Object) Val(value.ToString)},
                {GetType(Date), Function(value As Object) CDate(value.ToString)},
                {GetType(String), Function(value As Object) value.ToString},
                {GetType(Integer), Function(value As Object) CInt(Val(value.ToString))},
                {GetType(Boolean), Function(value As Object) CBool(value.ToString)},
                {GetType(Microsoft.VisualBasic.CommandLine.CommandLine), Function(value As Object) CommandLine.CommandLine.TryParse(value.ToString)}}

        ''' <summary>
        ''' Get the help information description for the target command method.(获取一个方法的详细帮助信息)
        ''' </summary>
        ''' <param name="MethodInfo"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetDescription(MethodInfo As System.Reflection.MethodInfo, Optional ParameterSignature As String = "") As String
            Dim EntryInfo = DirectCast(MethodInfo.GetCustomAttributes(CommandLine.Reflection.CommandAttribute.TypeInfo, True).First, CommandLine.Reflection.CommandAttribute)
            Dim ParameterInfo = MethodInfo.GetParameters
            Dim sBuilder As StringBuilder = New StringBuilder(1024)

            Call sBuilder.AppendLine(String.Format("  (*IntPtr) <Function Entry Pointer ""{0}"" bytes>", MethodInfo.MethodHandle.GetFunctionPointer))
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine(String.Format("Description Summary for command ""{0}""", EntryInfo.Name))
            Call sBuilder.AppendLine(String.Format("Description:  {0}", If(String.IsNullOrEmpty(EntryInfo.Info), "No descript data", EntryInfo.Info)))
            Call sBuilder.AppendLine(String.Format("Return:       {0}", MethodInfo.ReturnType.FullName))

            If ParameterInfo.IsNullOrEmpty Then
                Call sBuilder.AppendLine(vbCrLf & "This command does not required of any parameters.")
            Else
                Call InternalGetParametersDescription(sBuilder:=sBuilder,
                                                      ParameterInfo:=(From pInfo As System.Reflection.ParameterInfo
                                                                      In ParameterInfo
                                                                      Select Microsoft.VisualBasic.Scripting.EntryPointMetaData.ParameterAlias.GetParameterNameAlias(pInfo, [Default]:=Nothing)).ToArray,
                                                      MethodEntryPoint:=MethodInfo,
                                                      ParameterSignature:=ParameterSignature)
            End If

            Return sBuilder.ToString
        End Function

        Private Shared Sub InternalGetParametersDescription(ByRef sBuilder As StringBuilder, ParameterInfo As Microsoft.VisualBasic.Scripting.EntryPointMetaData.ParameterAlias(), MethodEntryPoint As System.Reflection.MethodInfo, ParameterSignature As String)
            Dim MaxLength As Integer = (From p In ParameterInfo Select Len(p.Alias)).ToArray.Max
            Dim MaxTypeLength As Integer = (From p In ParameterInfo Select Len(p.AssociatedParameterInfo.ParameterType.FullName)).ToArray.Max

            Call sBuilder.AppendLine(vbCrLf & String.Format("     {0} Parameter(s)", ParameterInfo.Count) & If(String.IsNullOrEmpty(ParameterSignature), "", vbTab & ParameterSignature) & vbCrLf)
            Call sBuilder.AppendLine(String.Format("+-ParameterName{0}-+-ParameterType-------------------------------------------------------------------------------------",
                                                   New String("-"c, If(MaxLength > Len("parametername"), MaxLength - Len("ParameterName") + 4, MaxLength))))
            For Each pEntry As Microsoft.VisualBasic.Scripting.EntryPointMetaData.ParameterAlias In ParameterInfo
                Dim p = pEntry.AssociatedParameterInfo
                Call sBuilder.AppendLine(String.Format("  {0}{1}    {2}{3}  {4}{5}", If(p.IsOptional, String.Format("[{0}]", pEntry.Alias), pEntry.Alias),
                                                                                  New String(" "c, MaxLength - Len(pEntry.Alias) + 10),
                                                                                  p.ParameterType.FullName,
                                                                                  New String(" ", MaxTypeLength - Len(p.ParameterType.FullName)),
                                                                                  If(p.IsOptional, If(p.DefaultValue Is Nothing, "= NULL", "= " & p.DefaultValue.ToString), ""),
                                                                                  If(String.IsNullOrEmpty(pEntry.Description), "", "   // " & pEntry.Description)))
            Next

            Dim ParameterDescriptions As Object() = MethodEntryPoint.GetCustomAttributes(attributeType:=GetType(Microsoft.VisualBasic.CommandLine.Reflection.ParameterDescription), inherit:=True)

            If Not ParameterDescriptions.IsNullOrEmpty Then
                Call sBuilder.AppendLine(vbCrLf & vbCrLf & "Parameter detail information:" & vbCrLf)

                For Each desc As Microsoft.VisualBasic.CommandLine.Reflection.ParameterDescription In (From item In ParameterDescriptions Select DirectCast(item, Microsoft.VisualBasic.CommandLine.Reflection.ParameterDescription))
                    Call sBuilder.AppendLine(desc.ToString)
                Next
            End If
        End Sub
    End Class
End Namespace