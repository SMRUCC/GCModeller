Namespace AssemblyScript.Commands

    ''' <summary>
    ''' from a base model
    ''' </summary>
    Public Class From : Inherits Command

        Public Property base As String

        Public Overrides Function Execute(env As Environment) As Object
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Return $"FROM {base};"
        End Function
    End Class
End Namespace