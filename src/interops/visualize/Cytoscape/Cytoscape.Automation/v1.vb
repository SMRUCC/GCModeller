Imports Microsoft.VisualBasic.Serialization.JSON

Public Class v1 : Inherits cyREST

    ReadOnly api$

    Sub New(port As Integer, Optional host$ = "localhost")
        api = $"http://{host}:{port}/v1"
    End Sub

    ''' <summary>
    ''' GET list of layout algorithms
    ''' </summary>
    ''' <returns></returns>
    Public Overrides Function layouts() As String()
        Dim url As String = $"{api}/apply/layouts"
        Dim json As String = url.GET

        Return json.LoadJSON(Of String())
    End Function

    Public Overrides Function networksNames() As String()
        Throw New NotImplementedException()
    End Function

    Public Overrides Function putNetwork() As Object
        Throw New NotImplementedException()
    End Function
End Class
