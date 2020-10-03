Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Script

Namespace AssemblyScript.Commands

    Public Class Keywords : Inherits Command

        Public Property keywords As String()

        Sub New(tokens As Token())
            keywords = tokens.Skip(1) _
                .Where(Function(a)
                           Return Not a.name = Script.Tokens.comma
                       End Function) _
                .Select(Function(a) stripValueString(a.text)) _
                .ToArray
        End Sub

        Public Overrides Function Execute(env As Environment) As Object
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Return $"KEYWORDS {keywords.JoinBy(", ")}"
        End Function
    End Class
End Namespace