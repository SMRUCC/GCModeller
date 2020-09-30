Imports SMRUCC.genomics.Assembly.KEGG.DBGET

Namespace AssemblyScript.Commands

    Public MustInherit Class Entry

        Public MustOverride Function GetEntries() As String()

    End Class

    Public Class EntryIdVector : Inherits Entry

        Public Property id As String()

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

        Public Overrides Function GetEntries() As String()
            Throw New NotImplementedException()
        End Function

        Public Overrides Function ToString() As String
            Return $"{className.Description}::""{categoryPath.JoinBy("\")}\{matchPattern}"""
        End Function
    End Class
End Namespace