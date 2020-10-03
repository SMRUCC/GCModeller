Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Script

Namespace AssemblyScript.Commands

    ''' <summary>
    ''' add item nodes to cellular network
    ''' </summary>
    Public Class Add : Inherits Modification

        Sub New(tokens As Token())
            tokens = tokens.Skip(1).ToArray

            If tokens.Any(Function(a) a.name = Script.Tokens.comma) Then
                entry = New EntryIdVector(tokens)
            Else
                entry = New CategoryEntry(tokens)
            End If
        End Sub

        Public Overrides Function Execute(env As Environment) As Object
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace