Imports Microsoft.VisualBasic.Scripting.ShoalShell.Interpreter.LDM

Namespace Runtime.Debugging

    Public Class ShellScriptDebuggerModel : Inherits SyntaxModel

        Public ReadOnly Property ScriptEngine As Runtime.ScriptEngine

        Sub New(LDM As SyntaxModel, ScriptEngine As ScriptEngine)
            Me.Expressions = LDM.Expressions
            Me.FilePath = LDM.FilePath
            Me.GotoJumpsLabel = LDM.GotoJumpsLabel
            Me.ScriptEngine = ScriptEngine
        End Sub

        Dim p As Integer

        Public Function Execute() As Object
            Throw New NotImplementedException
        End Function

        Protected Sub __executeScript()
            'If String.Equals(CodeLine.OrignialScriptLine, FLAG_STATEMENT_RETURN_STACK, StringComparison.OrdinalIgnoreCase) Then
            '    Call _InternalEXEC_Pointer.MoveNext()
            '    Return
            'ElseIf InternalExecuteGoto(CodeLine) Then
            '    Return
            'End If

            'Dim CurrentExecHandle = CodeLine.InvokeMethod.BeginInvoke(Nothing, Nothing)

            'Do While Not CurrentExecHandle.IsCompleted
            '    If _KillScript Then
            '        Call CurrentExecHandle.Free()
            '        Return
            '    End If

            '    Call Threading.Thread.Sleep(1)
            'Loop

            'Dim Value As Object = CodeLine.InvokeMethod.EndInvoke(CurrentExecHandle)
            'Dim sys As Objects.I_MemoryManagementDevice = ScriptEngine._EngineMemoryDevice
            'Call sys.InsertOrUpdate(CodeLine.VariableAssigned, Value)
        End Sub

        Dim _KillScript As Boolean = False

        Public Sub KillScript()
            _KillScript = True
        End Sub
    End Class
End Namespace