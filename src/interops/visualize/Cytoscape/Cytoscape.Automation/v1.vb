Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
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

    Public Overrides Function putNetwork(network As IEnumerable(Of SIF), Optional collection As String = Nothing, Optional title As String = Nothing, Optional source As String = Nothing, Optional format As formats = formats.egdeList) As Object
        Dim text As String = SIF.ToText(network)
        Dim query As New Dictionary(Of String, String) From {
            {"collection", collection},
            {"title", title},
            {"source", source},
            {"format", format.ToString}
        }
        Dim url As String = $"{api}/networks?{query.BuildUrlData}"
        Dim json As String = url.POSTFile(Encodings.UTF8WithoutBOM.CodePage.GetBytes(text))

        Return json
    End Function
End Class
