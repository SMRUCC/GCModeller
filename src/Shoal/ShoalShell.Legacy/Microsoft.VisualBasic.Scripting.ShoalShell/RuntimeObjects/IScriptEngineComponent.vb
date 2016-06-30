Namespace Runtime.Objects.ObjectModels

    ''' <summary>
    ''' This type of the class object consist of the shoal shell scripting engine.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class IScriptEngineComponent

        Protected _RuntimeEnvironment As ShoalShell.Runtime.Objects.ShellScript

        ''' <summary>
        ''' Script host entry.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property ScriptEngine As ShoalShell.Runtime.Objects.ShellScript
            Get
                Return _RuntimeEnvironment
            End Get
        End Property

        Sub New(ScriptEngine As ShoalShell.Runtime.Objects.ShellScript)
            _RuntimeEnvironment = ScriptEngine
        End Sub

        Public Overrides Function ToString() As String
            Return _RuntimeEnvironment.ToString
        End Function
    End Class
End Namespace