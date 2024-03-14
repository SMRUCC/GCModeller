Imports Microsoft.VisualBasic.Text

Public Class PathwayName

    Public Property id As String
    Public Property name As String
    Public Property tax_name As String

    Public Overrides Function ToString() As String
        Return $"{id}: {name}"
    End Function

    Public Shared Iterator Function LoadInternal() As IEnumerable(Of PathwayName)
        For Each line As String In My.Resources.ReactomePathways.LineTokens
            Dim t As String() = line.Split(ASCII.TAB)
            Dim name As New PathwayName With {
                .id = t(0),
                .name = t(1),
                .tax_name = t(2)
            }

            Yield name
        Next
    End Function

End Class
