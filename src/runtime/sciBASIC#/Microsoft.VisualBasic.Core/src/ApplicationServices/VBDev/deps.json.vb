Namespace ApplicationServices.Development.NetCore5

    ''' <summary>
    ''' read deps.json for .net 5 assembly
    ''' </summary>
    Public Class deps

        Public Property runtimeTarget As runtimeTarget
        Public Property compilationOptions As compilationOptions
        Public Property targets As Dictionary(Of String, target)
        Public Property libraries As Dictionary(Of String, library)

        ''' <summary>
        ''' get list of project reference name
        ''' </summary>
        ''' <returns></returns>
        Public Function GetReferenceProject() As IEnumerable(Of String)
            Return From entry As KeyValuePair(Of String, library)
                   In libraries
                   Let ref As library = entry.Value
                   Where ref.type = "project"
                   Select entry.Key.StringReplace("/\d+(\.\d+)+", "")
        End Function

    End Class

    Public Class target
        Public Property dependencies As Dictionary(Of String, String)
    End Class

    Public Class library
        Public Property type As String
        Public Property serviceable As Boolean
        Public Property sha512 As String
        Public Property path As String
        Public Property hashPath As String
    End Class

    Public Class runtimeTarget

        Public Property name As String
        Public Property signature As String

    End Class

    Public Class compilationOptions

    End Class
End Namespace