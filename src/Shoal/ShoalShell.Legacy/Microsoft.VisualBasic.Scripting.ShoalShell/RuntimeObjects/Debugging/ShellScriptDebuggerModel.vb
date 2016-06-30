Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ObjectModels
Imports Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects

Namespace Runtime.Debugging

    Public Class ShellScriptDebuggerModel : Inherits Microsoft.VisualBasic.Scripting.ShoalShell.Runtime.Objects.ObjectModels.ShellScript

        Sub New(CodeLines As Generic.IEnumerable(Of ScriptCodeLine), ScriptEngine As ShoalShell.Runtime.Objects.ShellScript)
            Call MyBase.New(CodeLines, ScriptEngine)
        End Sub

        Protected Overrides Sub InternalExecuteScript(CodeLine As ObjectModels.ScriptCodeLine)
            If String.Equals(CodeLine.OrignialScriptLine, FLAG_STATEMENT_RETURN_STACK, StringComparison.OrdinalIgnoreCase) Then
                Call _InternalEXEC_Pointer.MoveNext()
                Return
            ElseIf InternalExecuteGoto(CodeLine) Then
                Return
            End If

            Dim CurrentExecHandle = CodeLine.InvokeMethod.BeginInvoke(Nothing, Nothing)

            Do While Not CurrentExecHandle.IsCompleted
                If _KillScript Then
                    Call CurrentExecHandle.Free()
                    Return
                End If

                Call Threading.Thread.Sleep(1)
            Loop

            Dim Value As Object = CodeLine.InvokeMethod.EndInvoke(CurrentExecHandle)
            Dim sys As Objects.I_MemoryManagementDevice = ScriptEngine._EngineMemoryDevice
            Call sys.InsertOrUpdate(CodeLine.VariableAssigned, Value)
        End Sub

        Dim _KillScript As Boolean = False

        Protected Overrides Function ExitScript() As Boolean
            Return _KillScript
        End Function

        Public Function KillScript() As Boolean
            _KillScript = True
            Return True
        End Function
    End Class
End Namespace