Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports System.Collections.ObjectModel

Namespace DelegateHandlers.EntryPointHandlers

    Partial Class ImportsEntryPointManager

        Dim _InternalEntryPointLoader As InternalEntryPointLoader

        Public ReadOnly Property MethodLoader As InternalEntryPointLoader
            Get
                Return _InternalEntryPointLoader
            End Get
        End Property

        Public Class InternalEntryPointLoader : Inherits ShoalShell.Runtime.Objects.ObjectModels.IScriptEngineComponent

            Dim InternalManager As ImportsEntryPointManager

            Sub New(ImportsEntryPointManager As ImportsEntryPointManager)
                Call MyBase.New(ImportsEntryPointManager.ScriptEngine)
                InternalManager = ImportsEntryPointManager
            End Sub

            ''' <summary>
            ''' 
            ''' </summary>
            ''' <param name="Command"></param>
            ''' <returns></returns>
            ''' <remarks></remarks>
            Public Function GetCommand(Command As CommandLine.CommandLine) As Func(Of Object)
                Dim EntryName As String = Command.Name.ToLower

                If Me.ScriptEngine._InternalEntryPointManager._TEMP_HANDLERS.ContainsKey(EntryName) Then                    '临时脚本命令是具有最高的优先级的
                    Return Function() Me._RuntimeEnvironment.InternalSourceScript(
                               ShellScript:=FileIO.FileSystem.ReadAllText(InternalManager._TEMP_HANDLERS(EntryName)),
                               parameters:=(From p In Command.ToArray Select New KeyValuePair(Of String, Object)(p.Key, InternalManager.ScriptEngine.ScriptEngineMemoryDevice.TryGetValue(p.Value))).ToArray)
                End If

                If InternalManager._InternalDelegateHash.InternalHashDictionary.ContainsKey(EntryName) Then  '其次是在脚本之中创建的Delegate函数指针
                    Return Function() Me._RuntimeEnvironment.Interpreter._InternalMethodInvoker.CallMethod(EntryPoint:=InternalManager._InternalDelegateHash.InternalHashDictionary(EntryName), argvs:=Command, MemoryDevice:=_RuntimeEnvironment._EngineMemoryDevice, TypeSignature:="")
                End If

                If InternalManager._InternalImportsEntryPointHash.InternalHashDictionary.ContainsKey(EntryName) Then  '最后才从外部程序包之中调用命令
                    Dim Method As ShoalShell.DelegateHandlers.EntryPointHandlers.CommandMethodEntryPoint =
                        InternalManager._InternalImportsEntryPointHash.InternalHashDictionary(EntryName)
                    Return Function() Me._RuntimeEnvironment._Interpreter._InternalMethodInvoker.CallMethod(EntryPoint:=Method, argvs:=Command, MemoryDevice:=_RuntimeEnvironment._EngineMemoryDevice, TypeSignature:="")
                Else
                    Throw New ShoalShell.Runtime.Objects.ObjectModels.Exceptions.MethodNotFoundException(Command.Name, "")
                End If
            End Function
        End Class
    End Class
End Namespace