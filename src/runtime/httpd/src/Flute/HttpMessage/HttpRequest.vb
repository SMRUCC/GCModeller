#Region "Microsoft.VisualBasic::0807b6828754e152421075dd6e839036, src\Flute\HttpMessage\HttpRequest.vb"

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

    '   Total Lines: 116
    '    Code Lines: 68 (58.62%)
    ' Comment Lines: 31 (26.72%)
    '    - Xml Docs: 100.00%
    ' 
    '   Blank Lines: 17 (14.66%)
    '     File Size: 4.00 KB


    '     Class HttpRequest
    ' 
    '         Properties: HttpHeaders, HTTPMethod, HttpRequest, IsWWWRoot, Remote
    '                     URL, version
    ' 
    '         Constructor: (+3 Overloads) Sub New
    '         Function: GetArguments, GetBoolean, GetCookies, HasValue, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Flute.Http.Core.Message.HttpHeader
Imports Microsoft.VisualBasic.ComponentModel.DataSourceModel
Imports Microsoft.VisualBasic.Language.Default
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Serialization.JSON

Namespace Core.Message

    ''' <summary>
    ''' Data of the http request
    ''' </summary>
    Public Class HttpRequest

        ''' <summary>
        ''' GET/POST/PUT/DELETE.... the http request method, always upper case.
        ''' </summary>
        ''' <remarks>
        ''' http方法名是大写的
        ''' </remarks>
        ''' <returns>the upper-case http method name.</returns>
        Public ReadOnly Property HTTPMethod As String
        ''' <summary>
        ''' the parsed request url, including its path and query string.
        ''' </summary>
        ''' <returns>the <see cref="URL"/> of the request.</returns>
        Public ReadOnly Property URL As URL
        ''' <summary>
        ''' the http protocol version string declared by the client
        ''' (<see cref="HttpProcessor.http_protocol_versionstring"/>).
        ''' </summary>
        ''' <returns>the protocol version, e.g. HTTP/1.1.</returns>
        Public ReadOnly Property version As String
        ''' <summary>
        ''' the parsed request headers, keyed case-insensitively.
        ''' </summary>
        ''' <returns>the dictionary of request header name/value pairs.</returns>
        Public ReadOnly Property HttpHeaders As Dictionary(Of String, String)

        ''' <summary>
        ''' Remote client ip address
        ''' </summary>
        ''' <returns>the remote client ip address.</returns>
        Public ReadOnly Property Remote As String
        ''' <summary>
        ''' the underlying <see cref="HttpProcessor"/> that carried this request.
        ''' </summary>
        ''' <returns>the owning http processor instance.</returns>
        Public ReadOnly Property HttpRequest As HttpProcessor

        ''' <summary>
        ''' If current request url is indicates the HTTP root:  index.html
        ''' </summary>
        ''' <returns><c>True</c> when the url is exactly "/".</returns>
        Public ReadOnly Property IsWWWRoot As Boolean
            Get
                Return String.Equals("/", URL)
            End Get
        End Property

        Dim m_cookies As Cookies

        ''' <summary>
        ''' get a query string argument value by name (read from <see cref="URL"/>).
        ''' </summary>
        ''' <param name="name">the query argument name.</param>
        ''' <returns>the first query value, or an empty default string when absent.</returns>
        Default Public Overridable ReadOnly Property Argument(name As String) As DefaultString
            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Get
                Return New DefaultString(URL.query(name).ElementAtOrNull(Scan0))
            End Get
        End Property

        ''' <summary>
        ''' build a request object from the given <see cref="HttpProcessor"/>,
        ''' copying its method, url, version, headers and remote endpoint.
        ''' </summary>
        ''' <param name="request">the http processor that carried the request.</param>
        Sub New(request As HttpProcessor)
            HTTPMethod = request.http_method
            URL = New URL(request.http_url)
            version = request.http_protocol_versionstring
            HttpHeaders = request.httpHeaders
            Remote = request.socket.Client.RemoteEndPoint.ToString.Split(":"c).First
            HttpRequest = request
        End Sub

        ''' <summary>
        ''' create an empty request placeholder, used for object construction
        ''' before the real processor is attached.
        ''' </summary>
        Sub New()
        End Sub

        ''' <summary>
        ''' Debug use: build an in-memory GET request from the given named values.
        ''' </summary>
        ''' <param name="args">the query arguments of the synthetic request.</param>
        Friend Sub New(args As IEnumerable(Of NamedValue(Of String)))
            HTTPMethod = "GET"
            URL = URL.BuildUrl("memory://debug", query:=args)
            version = "2.1"
            HttpHeaders = New Dictionary(Of String, String)
            Remote = "127.0.0.1"
        End Sub

        ''' <summary>
        ''' get a query argument as a boolean value.
        ''' </summary>
        ''' <param name="name">the query argument name.</param>
        ''' <returns>the parsed boolean, or <c>False</c> when absent.</returns>
        Public Overridable Function GetBoolean(name As String) As Boolean
            If URL.query.ContainsKey(name) Then
                Return URL.query(name).ElementAtOrDefault(Scan0).ParseBoolean
            Else
                Return False
            End If
        End Function

        ''' <summary>
        ''' parse and cache the <see cref="Cookies"/> carried in the request headers.
        ''' </summary>
        ''' <returns>the parsed cookie collection.</returns>
        Public Function GetCookies() As Cookies
            If m_cookies Is Nothing Then
                m_cookies = Cookies.ParseCookies(HttpHeaders.TryGetValue(RequestHeaders.Cookie))
            End If

            Return m_cookies
        End Function

        ''' <summary>
        ''' get all query arguments as a name/object dictionary.
        ''' </summary>
        ''' <returns>a dictionary of query argument names and their values.</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overridable Function GetArguments() As Dictionary(Of String, Object)
            Return URL.query.ToDictionary(Function(a) a.Key, Function(a) CObj(a.Value))
        End Function

        ''' <summary>
        ''' test whether the given query argument name is present.
        ''' </summary>
        ''' <param name="name">the query argument name.</param>
        ''' <returns><c>True</c> when the argument exists.</returns>
        Public Overridable Function HasValue(name As String) As Boolean
            Return URL.query.ContainsKey(name)
        End Function

        ''' <summary>
        ''' the json representation of this request.
        ''' </summary>
        ''' <returns>the request serialized as json.</returns>
        Public Overrides Function ToString() As String
            Return Me.GetJson
        End Function
    End Class

End Namespace
