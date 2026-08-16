#Region "Microsoft.VisualBasic::50859296ae4da36084c23e055ffdc8f5, src\Flute\HttpMessage\HttpPOSTRequest.vb"

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


    ' Code Statistics:

    '   Total Lines: 96
    '    Code Lines: 75 (78.12%)
    ' Comment Lines: 9 (9.38%)
    '    - Xml Docs: 77.78%
    ' 
    '   Blank Lines: 12 (12.50%)
    '     File Size: 3.79 KB


    '     Class HttpPOSTRequest
    ' 
    '         Properties: POSTData
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: GetArguments, GetBoolean, HasValue
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Text
Imports Flute.Http.Core.HttpStream
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Language.Default
Imports any = Microsoft.VisualBasic.Scripting

Namespace Core.Message

    ''' <summary>
    ''' request for the http post request
    ''' </summary>
    Public Class HttpPOSTRequest : Inherits HttpRequest

        ''' <summary>
        ''' the parsed POST body, exposing form fields, json objects and uploaded
        ''' files extracted from the request body.
        ''' </summary>
        Public ReadOnly Property POSTData As PostReader

        Default Public Overrides ReadOnly Property Argument(name As String) As DefaultString
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                If URL.query.ContainsKey(name) Then
                    Return New DefaultString(URL.query(name).ElementAtOrNull(Scan0))
                ElseIf POSTData.Form.ContainsKey(name) Then
                    Return New DefaultString(POSTData.Form(name))
                ElseIf POSTData.Objects.ContainsKey(name) Then
                    Dim val As Object = POSTData.Objects(name)
                    Dim str As New DefaultString(any.ToString(val))
                    Return str
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Shared ReadOnly uploadfile As [Default](Of String) = NameOf(uploadfile)

        ''' <summary>
        ''' build a POST request from the given processor, decoding the raw post
        ''' body stored in the temporary input file through the supplied json parser.
        ''' </summary>
        ''' <param name="request">the http processor that carried the request.</param>
        ''' <param name="inputData$">一个临时文件的文件路径,POST上传的原始数据都被保存在这个临时文件中</param>
        ''' <param name="parseJSON">the optional json parser used to deserialize json bodies.</param>
        Sub New(request As HttpProcessor, inputData$, Optional parseJSON As PostReader.JSONParser = Nothing)
            Call MyBase.New(request)

            If inputData.FileLength > 0 AndAlso HttpHeaders.ContainsKey(HttpHeader.RequestHeaders.ContentType) Then
                POSTData = New PostReader(
                    inputData,
                    HttpHeaders(HttpHeader.RequestHeaders.ContentType),
                    Encoding.UTF8,
                    HttpHeaders.TryGetValue("fileName") Or uploadfile,
                    parseJSON:=parseJSON
                )
            Else
                POSTData = New PostReader(
                    input:=inputData,
                    contentType:="application/octet-stream",
                    encoding:=Encoding.ASCII,
                    fileName:=HttpHeaders.TryGetValue("fileName") Or uploadfile
                )
            End If
        End Sub

        ''' <summary>
        ''' get a POST argument as a boolean value, checking both the url query
        ''' and the parsed post body.
        ''' </summary>
        ''' <param name="name">the argument name.</param>
        ''' <returns>the parsed boolean, or <c>False</c> when absent.</returns>
        Public Overrides Function GetBoolean(name As String) As Boolean
            If HasValue(name) Then
                Return Argument(name).DefaultValue.ParseBoolean
            Else
                Return False
            End If
        End Function

        Public Overrides Function HasValue(name As String) As Boolean
            If Not URL.query.ContainsKey(name) Then
                Return POSTData.Form.ContainsKey(name)
            Else
                Return True
            End If
        End Function

        Public Overrides Function GetArguments() As Dictionary(Of String, Object)
            Dim args As Dictionary(Of String, Object) = MyBase.GetArguments

            ' add post json data
            If Not POSTData.Form Is Nothing Then
                For Each name As String In POSTData.Form.Keys
                    args(name) = POSTData.Form(name)
                Next
            End If
            If Not POSTData.Objects.IsNullOrEmpty Then
                For Each item As KeyValuePair(Of String, Object) In POSTData.Objects
                    args(item.Key) = item.Value
                Next
            End If

            Return args
        End Function
    End Class
End Namespace
