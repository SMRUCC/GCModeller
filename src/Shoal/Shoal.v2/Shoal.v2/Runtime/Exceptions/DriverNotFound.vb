Namespace Runtime.Exceptions

    Public Class DriverNotFound : Inherits RuntimeException

        Public Property Driver As String

        Sub New(Message As String, ScriptEngine As Runtime.ScriptEngine)
            Call MyBase.New(Message, ScriptEngine)
        End Sub

        Sub New(Message As String, InnerException As Exception, ScriptEngine As Runtime.ScriptEngine)
            Call MyBase.New(Message, InnerException, ScriptEngine)
        End Sub
    End Class
End Namespace