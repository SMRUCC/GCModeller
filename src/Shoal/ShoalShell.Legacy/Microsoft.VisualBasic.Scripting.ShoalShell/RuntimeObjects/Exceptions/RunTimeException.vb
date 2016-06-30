Imports System.Text

Namespace Runtime.Objects.ObjectModels.Exceptions

    Public Class ScriptRunTimeException : Inherits ShoalScriptException

        Public Property InnerExceptionValue As String
        Dim MemoryDump As Runtime.Objects.I_MemoryManagementDevice

        Sub New(message As String, MemoryDump As Runtime.Objects.I_MemoryManagementDevice)
            MyBase.New(message)
            Me.InnerExceptionValue = message
            Me.MemoryDump = MemoryDump
        End Sub

        Public Overrides Function ToString() As String
            Dim sBuilder As StringBuilder = New StringBuilder(1024)
            Call sBuilder.AppendLine(String.Format("[ERROR_LINE]" & vbCrLf & "   ---->  {0}  ............................. line: {1}", ScriptLine, LineNumber) & vbCrLf)
            Call sBuilder.AppendLine(New String("-", 30) & vbCrLf)
            Call sBuilder.AppendLine("[ShellScript Memory Dump Details]" & vbCrLf)
            Call sBuilder.AppendLine(" " & MemoryDump.ExportMemoryDetails)
            Call sBuilder.AppendLine(vbCrLf & "[Internal System.Exception Caller Stack Details Information]" & vbCrLf & vbCrLf & " " & InnerExceptionValue)

            Return sBuilder.ToString
        End Function

        Public Overrides ReadOnly Property ExceptionType As String
            Get
                Return GetType(ScriptRunTimeException).FullName
            End Get
        End Property
    End Class
End Namespace
