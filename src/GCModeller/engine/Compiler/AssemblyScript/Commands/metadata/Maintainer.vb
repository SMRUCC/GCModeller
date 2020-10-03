Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Script

Namespace AssemblyScript.Commands

    ''' <summary>
    ''' maintainer or author information(meta data shortcut)
    ''' </summary>
    Public Class Maintainer : Inherits Command

        Public Property authorName As String
        Public Property email As String

        Sub New(tokens As Token())
            Dim info = tokens.Skip(1).ToArray

            authorName = info(0).text
            email = Strings.Trim(info.ElementAtOrDefault(1)?.text).Trim(""""c, "<"c, ">"c).Trim
        End Sub

        Sub New()
        End Sub

        Public Overrides Function Execute(env As Environment) As Object
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Return $"MAINTAINER {authorName} <{email}>"
        End Function
    End Class
End Namespace