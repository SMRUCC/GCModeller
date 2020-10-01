Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Script

Namespace AssemblyScript.Commands

    ''' <summary>
    ''' set compiler environment variable
    ''' </summary>
    Public Class Env : Inherits Command

        Public ReadOnly Property name As String
        Public ReadOnly Property value As String

        Sub New(tokens As Token())
            name = tokens(1).text
            value = tokens(3).text
        End Sub

        Public Overrides Function Execute(env As Environment) As Object
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace