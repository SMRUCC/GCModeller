Imports System.Net
Imports Microsoft.VisualBasic.Serialization.JSON
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Cyjs
Imports SMRUCC.genomics.Visualize.Cytoscape.Tables

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

    Public Overrides Function putNetwork(network As Cyjs, Optional collection As String = Nothing, Optional title As String = Nothing, Optional source As String = Nothing, Optional format As formats = formats.json) As Object
        Dim query As New Dictionary(Of String, String) From {
            {"collection", collection},
            {"title", title},
            {"source", source},
            {"format", format.ToString}
        }
        Dim url As String = $"{api}/networks?{query.BuildUrlData(escaping:=True, stripNull:=True)}"
        Dim json As String = network.GetJson

        Using request As New WebClient
            request.Headers.Add("Content-Type", "application/json")
            json = request.UploadString(url, "POST", json)
        End Using

        Return json
    End Function
End Class
