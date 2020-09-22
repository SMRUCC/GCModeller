Namespace AssemblyScript.Commands

    ''' <summary>
    ''' the assembly compiler commands
    ''' </summary>
    Public MustInherit Class Command

        Public ReadOnly Property commandName As String
            Get
                Return MyClass.GetType.Name
            End Get
        End Property

        Public MustOverride Function Execute(env As Environment) As Object
        Public MustOverride Overrides Function ToString() As String
    End Class
End Namespace