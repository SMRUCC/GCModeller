Namespace AssemblyScript.Commands

    ''' <summary>
    ''' set compiler environment variable
    ''' </summary>
    Public Class Env : Inherits Command

        Public ReadOnly Property name As String
        Public ReadOnly Property value As String

        Public Overrides Function Execute(env As Environment) As Object
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace