Namespace Runtime.Exceptions

    Public Class RuntimeException : Inherits Exception

        Public Property Script As Interpreter.LDM.Expressions.PrimaryExpression
        Public ReadOnly Property ScriptEngine As Runtime.ScriptEngine

        Sub New(Message As String, ScriptEngine As Runtime.ScriptEngine)
            Call MyBase.New(Message)
            Me.ScriptEngine = ScriptEngine
        End Sub

        Sub New(Message As String, InnerException As Exception, ScriptEngine As Runtime.ScriptEngine)
            Call MyBase.New(Message, InnerException)
            Me.ScriptEngine = ScriptEngine
        End Sub
    End Class
End Namespace