Imports System.Net
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.MIME.application.json
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text
Imports SMRUCC.genomics.Visualize.Cytoscape.CytoscapeGraphView.Cyjs
Imports SMRUCC.genomics.Visualize.Cytoscape.Tables

Public Class v1 : Inherits cyREST

    ReadOnly api$

    Sub New(port As Integer, Optional host$ = "localhost")
        Call MyBase.New

        api = $"http://{host}:{port}/v1"
    End Sub

    Public Overrides Sub destroySession()
        Dim url As String = $"{api}/session"

        Using request As New WebClient
            Call request.UploadString(url, "DELETE", "")
        End Using
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

    Public Overrides Function putNetwork(network As [Variant](Of Cyjs, SIF()), Optional collection As String = Nothing, Optional title As String = Nothing) As NetworkReference
        Dim format As formats = If(network Like GetType(Cyjs), formats.json, formats.egdeList)
        ' 20200618 下面的参数是存在顺序之分的
        ' 不然会产生无法正常生成view的bug
        Dim query As New Dictionary(Of String, String) From {
            {"collection", collection},
            {"format", format.ToString},
            {"title", title},
            {"source", Nothing}
        }
        Dim url As String = $"{api}/networks?{query.BuildUrlData(escaping:=True, stripNull:=True)}"
        Dim text As String = If(format = formats.json, JSONSerializer.GetJson(New Upload.CyjsUpload(network.VA, title)), SIF.ToText(network.VB))
        'Dim file As New FileReference With {
        '    .ndex_uuid = 12345,
        '    .source_location = virtualFilesystem.addUploadData(text, If(format = formats.json, "json", "txt")),
        '    .source_method = "GET"
        '}
        ' Dim refJson As String = JSONSerializer.GetJson({file})

        Using request As New WebClient
            request.Headers.Add("Content-Type", If(format = formats.json, "application/json", "plain/text"))
            text = request.UploadData(url, Encodings.UTF8WithoutBOM.CodePage.GetBytes(text)).GetString
        End Using

        Return text.LoadJSON(Of NetworkReference)
    End Function

    Public Overrides Function saveSession(file As String) As Object
        Dim url As String = $"{api}/session?file={file.UrlEncode}"
        Dim result As String

        Using request As New WebClient
            request.Headers.Add("Content-Type", "application/json")
            result = request.UploadString(url, "POST", "")
        End Using

        Return result
    End Function

    Public Overrides Function applyLayout(networkId As Integer, Optional algorithmName$ = "force-directed") As String
        Dim url = $"{api}/apply/layouts/{algorithmName}/{networkId}"
        Return url.GET
    End Function

    Public Overrides Function networksNames() As String()
        Throw New NotImplementedException()
    End Function
End Class
