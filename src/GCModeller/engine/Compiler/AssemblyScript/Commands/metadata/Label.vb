Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Script

Namespace AssemblyScript.Commands

    ''' <summary>
    ''' add meta data for the given model
    ''' </summary>
    Public Class Label : Inherits Command

        Public Property keyValues As KeyValuePair(Of String, String)()

        Sub New(tokens As Token())
            Dim keyValues = tokens.Skip(1) _
                .Split(Function(a) a.name = Script.Tokens.comma, DelimiterLocation.NotIncludes) _
                .Where(Function(a) Not a.Length = 0) _
                .ToArray

            Me.keyValues = keyValues _
                .Select(Function(a)
                            Dim key As String = stripValueString(a(0).text)
                            Dim val As String = stripValueString(a(2).text)

                            Return New KeyValuePair(Of String, String)(key, val)
                        End Function) _
                .ToArray
        End Sub

        Public Overrides Function Execute(env As Environment) As Object
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Return $"LABEL {keyValues.Select(Function(a) $"{a.Key}=""{a.Value}""").JoinBy(", ")}"
        End Function
    End Class
End Namespace