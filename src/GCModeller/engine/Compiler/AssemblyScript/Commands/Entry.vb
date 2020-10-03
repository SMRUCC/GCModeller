Imports SMRUCC.genomics.Assembly.KEGG.DBGET
Imports SMRUCC.genomics.GCModeller.Compiler.AssemblyScript.Script

Namespace AssemblyScript.Commands

    Public MustInherit Class Entry

        Public MustOverride Function GetEntries() As String()

    End Class

    Public Class EntryIdVector : Inherits Entry

        Public Property id As String()

        Sub New(tokens As Token())
            id = tokens _
                .Where(Function(a) a.name <> Script.Tokens.comma) _
                .Select(Function(a)
                            Return Command.stripValueString(a.text)
                        End Function) _
                .ToArray
        End Sub

        Public Overrides Function GetEntries() As String()
            Return id
        End Function

        Public Overrides Function ToString() As String
            Return GetEntries.JoinBy(",")
        End Function
    End Class

    Public Class CategoryEntry : Inherits Entry

        Public Property className As KEGGObjects
        Public Property categoryPath As String()
        Public Property matchPattern As String

        Sub New(tokens As Token())
            Dim className As String = tokens(0).text
            Dim reference As String = Command.stripValueString(tokens(2).text)
            Dim refTokens As String() = reference.Split("\"c)

            Me.className = mapKEGGObject(className)
            Me.categoryPath = refTokens.Take(refTokens.Length - 1).ToArray
            Me.matchPattern = refTokens(refTokens.Length - 1)
        End Sub

        Private Shared Function mapKEGGObject(className As String) As KEGGObjects
            Select Case className.ToUpper
                Case "KO" : Return KEGGObjects.Orthology
                Case "MAP" : Return KEGGObjects.Pathway
                Case "EC" : Return KEGGObjects.Reaction
                Case Else
                    Throw New DataException(className)
            End Select
        End Function

        Public Overrides Function GetEntries() As String()
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Return $"{className.Description}:""{categoryPath.JoinBy("\")}\{matchPattern}"""
        End Function
    End Class
End Namespace