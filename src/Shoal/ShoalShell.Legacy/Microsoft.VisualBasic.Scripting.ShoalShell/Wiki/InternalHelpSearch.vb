Imports System.Text
Imports Microsoft.VisualBasic.ConsoleDevice.Utility

Namespace Wiki

    ''' <summary>
    ''' Query the help information in the local content.(通过注册表信息进行帮助信息的查找)
    ''' </summary>
    ''' <remarks></remarks>
    Public Class InternalHelpSearch : Inherits ShoalShell.Runtime.Objects.ObjectModels.IScriptEngineComponent

        Dim _IndexedManual As Boolean

        Sub New(ShoalHost As ShoalShell.Runtime.Objects.ShellScript, preferIndexedManual As Boolean)
            Call MyBase.New(ShoalHost)
            _IndexedManual = preferIndexedManual
        End Sub

        Public Function Match(keyword As String, ShowManual As Boolean) As String
            Dim LQuery = (From item As DelegateHandlers.TypeLibraryRegistry.RegistryNodes.Namespace
                          In _RuntimeEnvironment._Interpreter._DelegateRegistry.RegisteredModules.AsParallel
                          Let result As String = item.Match(keyword)
                          Where Not String.IsNullOrEmpty(result)
                          Select result).ToArray

            If LQuery.IsNullOrEmpty Then
                Return ""
            End If

            Dim sBuilder As StringBuilder = New StringBuilder("Shoal internal Wiki could not match an extractly item, these result maybe is what you want to search:" &
                                                              vbCrLf &
                                                              vbCrLf &
                                                              String.Format("All of the result has been show below, matched for keyword [{0}]:" & vbCrLf & vbCrLf, keyword))
            Dim Title As String = sBuilder.ToString

            For Each strValue As String In LQuery
                Call sBuilder.AppendLine(strValue)
            Next

            Dim Message As String = sBuilder.ToString

            If ShowManual Then
                Using Manual As Microsoft.VisualBasic.ConsoleDevice.Utility.ManualPages =
                    If(Me._IndexedManual, New IndexedManual(LQuery, Title), New ManualPages(Strings.Split(Message, vbCrLf)))

                    Call Manual.ShowManual()
                End Using
            End If

            Return Message
        End Function

        Private Function InternalListAllImportsCommand() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Dim ImportsCommands = ScriptEngine.InternalEntryPointManager.ImportsCommandEntryPoints
            Dim CommandNameMaxLength As Integer = (From strKey As String In ImportsCommands.Keys Select Len(strKey)).ToArray.Max

            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine("System Basically Command & Currently Imported Commands" & vbCrLf)
            Call sBuilder.AppendLine(vbCrLf & String.Format("     {0} Command(s)", ImportsCommands.Count))
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine(String.Format("  MethodEntry{0}    Return Type", New String(" "c, CommandNameMaxLength)))
            Call sBuilder.AppendLine(String.Format("+---{0}+----------------------------------------------------", New String("-"c, 1.5 * CommandNameMaxLength - 3)))

            For Each MethodEntryPoint In ImportsCommands
                Call sBuilder.AppendLine(String.Format("  {0}{1} {2} {3}", MethodEntryPoint.Key, New String(" "c, 1.5 * CommandNameMaxLength - Len(MethodEntryPoint.Key)),
                                                                           MethodEntryPoint.Value.First.EntryPoint.MethodEntryPoint.ReturnType.FullName,
                                                                           If(MethodEntryPoint.Value.IsOverloaded, String.Format("(+ {0} overloads)", MethodEntryPoint.Value.CountOfOverloadsCommand), "")))
            Next

            Dim TempShell = ScriptEngine.InternalEntryPointManager.TempShellCommands

            If Not TempShell.IsNullOrEmpty Then

                Call sBuilder.AppendLine()
                Call sBuilder.AppendLine(String.Format("    {0} Temp Shell Command(s) available in current work directory" & vbCrLf, TempShell.Count))
                Call sBuilder.AppendLine("-Name---------------------------File----------------------")

                CommandNameMaxLength = (From strKey As String In TempShell.Keys Select Len(strKey)).ToArray.Max

                For Each ShellEntry As KeyValuePair(Of String, String) In TempShell
                    Call sBuilder.AppendLine(String.Format(" {0}  {1} {2}", ShellEntry.Key, New String(" "c, CommandNameMaxLength - Len(ShellEntry.Key)), ShellEntry.Value))
                Next
            End If

            Call Console.WriteLine(sBuilder.ToString)

            Return sBuilder.ToString
        End Function

        Private Function InternalListMountDevice() As String
            Dim sBuilder As StringBuilder = New StringBuilder(2048)

            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine("!List of the mounts external IO device:")
            Call sBuilder.AppendLine("--------------------------------------------------------------------------------------------")
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine("==============================================")
            Call sBuilder.AppendLine(" [+] Input_Devices Mount Points")
            Call sBuilder.AppendLine("==============================================" & vbCrLf)
            Call sBuilder.AppendLine(String.Join(vbCrLf, ScriptEngine._InputSupport.GetMountEntries))
            Call sBuilder.AppendLine(vbCrLf & vbCrLf)
            Call sBuilder.AppendLine("==============================================")
            Call sBuilder.AppendLine(" [+] Console_Output_Devices Mount Points")
            Call sBuilder.AppendLine("==============================================" & vbCrLf)
            Call sBuilder.AppendLine(String.Join(vbCrLf, ScriptEngine._OutputSupport.GetMountEntries))
            Call sBuilder.AppendLine(vbCrLf & vbCrLf)
            Call sBuilder.AppendLine("==============================================")
            Call sBuilder.AppendLine(" [+] Stream_Output_Device Mount Points")
            Call sBuilder.AppendLine("==============================================" & vbCrLf)
            Call sBuilder.AppendLine(String.Join(vbCrLf, ScriptEngine._IOSupport.GetMountEntries))

            Call Console.WriteLine(sBuilder.ToString)

            Return sBuilder.ToString
        End Function

        ''' <summary>
        ''' Gets the help information of a specific object in the shoal shell system.(获取帮助信息，如果实在无法进行精确匹配的话，系统会尝试进行模糊匹配)
        ''' </summary>
        ''' <param name="obj">Object Name</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetHelpInfo(obj As String, ShowManual As Boolean) As String

            If String.IsNullOrEmpty(obj) Then
                Return InternalListAllImportsCommand()
            ElseIf String.Equals(obj, "namespace", StringComparison.OrdinalIgnoreCase) Then
                Dim s As String = String.Join("  ", Me.ScriptEngine._ImportsNamespace.ToArray)   '显示所有已经导入的命名空间
                s = vbCrLf & String.Format("   {0} Imported Namespace(s)", ScriptEngine._ImportsNamespace.Count) & vbCrLf & vbCrLf & "   " & s & vbCrLf
                Console.WriteLine(s)
                Return s
            ElseIf String.Equals(obj, "mount", StringComparison.OrdinalIgnoreCase) Then '返回已经挂载的IO设备
                Return InternalListMountDevice()
            Else '将参数之中的::符号转换为空格
                obj = obj.Replace("::", " ")
            End If

            Try
                Return InternalGetMethodHelpInfo(obj)
            Catch ex As Exception

                Try
                    Dim [Module] As Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry.RegistryNodes.[Module] =
                        _RuntimeEnvironment._Interpreter._DelegateRegistry.TryGetModule(obj, _RuntimeEnvironment._EngineMemoryDevice)

                    Return InternalPrintLibrary([Module])
                Catch ex___ As Exception
                    Return Match(keyword:=obj, ShowManual:=ShowManual)
                End Try
            End Try
        End Function

        Private Function InternalGetMethodHelpInfo(cmdName As String) As String
            Dim Method = _RuntimeEnvironment._Interpreter.TryGetCommand(cmdName)
            Dim sBuilder As StringBuilder = New StringBuilder(2048)

            If Not Method.IsOverloaded Then
                Call sBuilder.AppendLine(Me._RuntimeEnvironment._Interpreter._InternalMethodInvoker.GetDescription(Method.NonOverloadsMethod.MethodEntryPoint))
            Else
                Call sBuilder.AppendLine(String.Format("Command ""{0}""  (+ {1} Overloads)" & vbCrLf, cmdName, Method.CountOfOverloadsCommand))
                Call sBuilder.AppendLine()

                Dim i As Integer = 0

                For Each OverloadsMethod In Method
                    Call sBuilder.Append(String.Format("  ------> [{0}]  ", i.MoveNext))
                    Call sBuilder.AppendLine(OverloadsMethod.GetDescription(AddressOf _RuntimeEnvironment._Interpreter._InternalMethodInvoker.GetDescription))
                    Call sBuilder.AppendLine()
                Next
            End If

            Call Console.WriteLine(vbCrLf & sBuilder.ToString)

            Return sBuilder.ToString
        End Function

        ''' <summary>
        ''' 目标是一个类型库，则输出详细信息
        ''' </summary>
        ''' <param name="Module"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Shared Function InternalPrintLibrary([Module] As Microsoft.VisualBasic.Scripting.ShoalShell.DelegateHandlers.TypeLibraryRegistry.RegistryNodes.[Module]) As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Dim MaxLength As Integer = (From strKey As String In [Module].Keys Select Len(strKey)).ToArray.Max
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine("Module Namespace:   " & [Module].Namespace)

            For Each Entry In [Module].OriginalAssemblys
                Call sBuilder.AppendLine("Assembly path:      " & Entry.Key.Assembly.Location)
                Call sBuilder.AppendLine("Assembly Lib:       " & Entry.Key.FullName)
                Call sBuilder.AppendLine("Module Description: " & Entry.Value)
                Call sBuilder.AppendLine()
            Next

            Call sBuilder.AppendLine(vbCrLf & String.Format("     {0} Command(s)", [Module].Count))
            Call sBuilder.AppendLine()
            Call sBuilder.AppendLine(String.Format("  MethodEntry{0}Return Type", New String(" "c, MaxLength)))
            Call sBuilder.AppendLine(String.Format("+---{0}+----------------------------------------------------", New String("-"c, 1.5 * MaxLength - 3)))

            For Each MethodEntry In [Module].Keys
                Dim InternalCreateEntryPointDescription = Function(Entry As System.Reflection.MethodInfo) As String
                                                              Return String.Format("  {0}{1} {2}", MethodEntry, New String(" "c, 1.5 * MaxLength - Len(MethodEntry)), Entry.ReturnType.FullName)
                                                          End Function
                Dim EntryList = [Module](MethodEntry)
                If EntryList.Count = 1 Then
                    Call sBuilder.AppendLine(InternalCreateEntryPointDescription(EntryList.First))
                Else
                    For i As Integer = 0 To EntryList.Count - 1
                        Call sBuilder.AppendLine(InternalCreateEntryPointDescription(EntryList(i)) & String.Format("  (+ {0} Overloads)", i))
                    Next
                End If
            Next

            Call Console.WriteLine(sBuilder.ToString)

            Return sBuilder.ToString
        End Function

        Public Overrides Function ToString() As String
            Return _RuntimeEnvironment.ToString
        End Function
    End Class
End Namespace