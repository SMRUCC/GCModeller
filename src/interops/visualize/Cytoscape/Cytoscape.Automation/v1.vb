#Region "Microsoft.VisualBasic::1c333923955f5ff0420c6ec23369bbaa, visualize\Cytoscape\Cytoscape.Automation\v1.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:

    ' Class v1
    ' 
    '     Constructor: (+1 Overloads) Sub New
    ' 
    '     Function: applyLayout, getView, getViewReference, layouts, networksNames
    '               putNetwork, saveSession
    ' 
    '     Sub: destroySession
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Net
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
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

    Public Overrides Function getViewReference() As Integer
        Dim url = $"{api}/networks/views/currentNetworkView"
        Dim json = url.GET

        Return json.LoadJSON(Of view).data("networkViewSUID").DoCall(AddressOf Integer.Parse)
    End Function

    Public Overrides Function getView(networkId As Integer, viewId As Integer) As Cyjs
        Dim url$ = $"{api}/networks/{networkId}/views/{viewId}"
        Dim json As String = url.GET

        Return json.LoadJSON(Of Cyjs)
    End Function
End Class
