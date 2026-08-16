#Region "Microsoft.VisualBasic::http-router, src\Flute\Http\Core\HttpRouter.vb"

' Author:
' 
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

'   Total Lines: 
'    Code Lines: 
' Comment Lines: 
'    - Xml Docs: 
' 
'   Blank Lines: 
'     File Size: 


'     Class HttpRouter
' 
'         Properties: Routes
' 
'         Constructor: (+2 Overloads) Sub New
' 
'         Function: RegisterController, Register
' 
'         Sub: AppHandler
' 
' 
' 
' /********************************************************************************/

#End Region

Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports Flute.Http.Core.HttpSocket
Imports Flute.Http.Core.Message
Imports Flute.Http.Core.Message.HttpHeader
Imports Flute.Http.FileSystem
Imports Microsoft.VisualBasic.Net.Http

Namespace Core

    ''' <summary>
    ''' A request router that implements <see cref="IAppHandler"/> by reflecting
    ''' over a user supplied clr object instance. Public methods annotated with
    ''' <see cref="HttpGet"/> or <see cref="HttpPost"/> (inheriting
    ''' <see cref="ExportAPIAttribute"/>) are registered as request handlers,
    ''' keyed by the http method and the annotated url. The router also supports
    ''' manual registration of handlers through <see cref="Register(String, String, AppHandler)"/>.
    ''' </summary>
    ''' <remarks>
    ''' The handler signature must match <see cref="AppHandler"/>:
    ''' <c>Sub(HttpRequest, HttpResponse)</c>.
    ''' </remarks>
    Public Class HttpRouter : Implements IAppHandler

        ''' <summary>
        ''' a single resolved route entry, either backed by a reflected method
        ''' (with its owning <see name="RouteEntry.target"/> instance) or by a manually
        ''' registered delegate.
        ''' </summary>
        Private Class RouteEntry

            ''' <summary>
            ''' the controller instance that owns <see cref="method"/>; nothing
            ''' for manually registered delegates.
            ''' </summary>
            Public target As Object

            ''' <summary>
            ''' the reflected method to invoke; nothing for manually registered delegates.
            ''' </summary>
            Public method As MethodInfo

            ''' <summary>
            ''' the manually registered handler delegate; nothing for reflected methods.
            ''' </summary>
            Public handler As AppHandler

            ''' <summary>
            ''' invoke this route entry against the given request/response pair.
            ''' </summary>
            Public Sub Invoke(request As HttpRequest, response As HttpResponse)
                If handler IsNot Nothing Then
                    Call handler(request, response)
                Else
                    Call method.Invoke(target, {request, response})
                End If
            End Sub
        End Class

        ''' <summary>
        ''' route tables keyed by the normalized url path for each http method.
        ''' </summary>
        ReadOnly getRoutes As New Dictionary(Of String, RouteEntry)
        ReadOnly postRoutes As New Dictionary(Of String, RouteEntry)

        Dim wfs As WebFileSystemListener

        ''' <summary>
        ''' the number of registered route entries (get + post), useful for diagnostics.
        ''' </summary>
        ''' <returns>the total count of registered routes.</returns>
        Public ReadOnly Property Routes As Integer
            Get
                Return getRoutes.Count + postRoutes.Count
            End Get
        End Property

        ''' <summary>
        ''' create an empty router; handlers have to be registered through
        ''' <see cref="RegisterController(Object)"/> or
        ''' <see cref="Register(String, String, AppHandler)"/>.
        ''' </summary>
        Sub New()
        End Sub

        ''' <summary>
        ''' create a router and immediately reflect over the given controller
        ''' instance to register its <see cref="HttpGet"/> / <see cref="HttpPost"/>
        ''' annotated methods.
        ''' </summary>
        ''' <param name="controller">the clr object instance whose public methods are scanned.</param>
        Sub New(controller As Object)
            Call RegisterController(controller)
        End Sub

        Public Function MountFs(fs As WebFileSystemListener) As HttpRouter
            Me.wfs = fs
            Return Me
        End Function

        ''' <summary>
        ''' reflect over the public instance methods of <paramref name="controller"/>
        ''' and register every method that is annotated with <see cref="HttpGet"/> or
        ''' <see cref="HttpPost"/> and whose signature matches
        ''' <c>Sub(HttpRequest, HttpResponse)</c>.
        ''' </summary>
        ''' <param name="controller">the clr object instance to scan; null is ignored.</param>
        ''' <returns>this router, for fluent registration chaining.</returns>
        Public Function RegisterController(controller As Object) As HttpRouter
            If controller Is Nothing Then
                Return Me
            End If

            Dim type As Type = controller.GetType()

            For Each method As MethodInfo In type.GetMethods(BindingFlags.Public Or BindingFlags.Instance)
                Dim getAttr As HttpGet = method.GetCustomAttribute(Of HttpGet)()
                Dim postAttr As HttpPost = method.GetCustomAttribute(Of HttpPost)()

                If getAttr Is Nothing AndAlso postAttr Is Nothing Then
                    Continue For
                End If

                ' signature must be: Sub(HttpRequest, HttpResponse)
                If Not matchSignature(method) Then
                    Call $"Skip route method '{type.Name}.{method.Name}' due to incompatible signature.".warning()
                    Continue For
                End If

                Dim url As String = If(getAttr IsNot Nothing, getAttr.Url, postAttr.Url)
                Dim entry As New RouteEntry With {
                    .target = controller,
                    .method = method,
                    .handler = Nothing
                }

                If getAttr IsNot Nothing Then
                    getRoutes(normalize(url)) = entry
                    Call $"registered GET route {getAttr.ToString} -> {type.Name}.{method.Name}".debug()
                Else
                    postRoutes(normalize(url)) = entry
                    Call $"registered POST route {postAttr.ToString} -> {type.Name}.{method.Name}".debug()
                End If
            Next

            Return Me
        End Function

        ''' <summary>
        ''' manually register a handler delegate for the given http method and url.
        ''' </summary>
        ''' <param name="httpMethod">
        ''' the upper-case http method name, e.g. "GET" or "POST". Only GET and POST
        ''' are routed; any other value is treated as a GET route.
        ''' </param>
        ''' <param name="url">the url path to match, e.g. "/user/info".</param>
        ''' <param name="handler">the handler delegate matching <see cref="AppHandler"/>.</param>
        ''' <returns>this router, for fluent registration chaining.</returns>
        Public Function Register(httpMethod As String, url As String, handler As AppHandler) As HttpRouter
            Dim entry As New RouteEntry With {
                .target = Nothing,
                .method = Nothing,
                .handler = handler
            }
            Dim key As String = normalize(url)

            If String.Equals(httpMethod, "POST", StringComparison.OrdinalIgnoreCase) Then
                postRoutes(key) = entry
                Call $"registered POST route '{key}' (manual)".debug()
            Else
                getRoutes(key) = entry
                Call $"registered GET route '{key}' (manual)".debug()
            End If

            Return Me
        End Function

        ''' <summary>
        ''' the <see cref="IAppHandler"/> entry point. Dispatches the request to the
        ''' matching route based on the actual request type (<see cref="HttpPOSTRequest"/>
        ''' vs <see cref="HttpRequest"/>) and the url path, returning HTTP 404 when no
        ''' route matches.
        ''' </summary>
        ''' <param name="request">the parsed incoming http request.</param>
        ''' <param name="response">the response object to be written to the client.</param>
        Public Sub AppHandler(request As HttpRequest, response As HttpResponse) Implements IAppHandler.AppHandler
            If wfs IsNot Nothing AndAlso wfs.CheckResourceFileExists(request) Then
                Call wfs.WebHandler(request, response)
            Else
                Call HandleClrAppProcessor(request, response)
            End If
        End Sub

        Private Sub HandleClrAppProcessor(request As HttpRequest, response As HttpResponse)
            Dim table As Dictionary(Of String, RouteEntry)
            Dim entry As RouteEntry = Nothing
            Dim key As String = normalize(request.URL.path)

            ' POST requests arrive as HttpPOSTRequest and are routed against the
            ' post table; everything else (GET / other methods) uses the get table.
            If TypeOf request Is HttpPOSTRequest Then
                table = postRoutes
            Else
                table = getRoutes
            End If

            If table.TryGetValue(key, entry) Then
                Try
                    Call entry.Invoke(request, response)
                Catch ex As TargetInvocationException
                    ' unwrap the real exception thrown inside the handler
                    Call App.LogException(ex.InnerException)
                    Call response.WriteError(HTTP_RFC.RFC_INTERNAL_SERVER_ERROR, ex.InnerException.Message)
                Catch ex As Exception
                    Call App.LogException(ex)
                    Call response.WriteError(HTTP_RFC.RFC_INTERNAL_SERVER_ERROR, ex.Message)
                End Try
            Else
                Call $"no route registered for {request.HTTPMethod} '{key}'".warning()
                Call response.WriteError(HTTP_RFC.RFC_NOT_FOUND, $"404 Not Found: {request.HTTPMethod} {key}")
            End If
        End Sub

        ''' <summary>
        ''' normalize a url path by trimming leading/trailing slashes so that
        ''' "/user/info" and "user/info" compare equal. The comparison is then done
        ''' by exact ordinal match against the request path.
        ''' </summary>
        ''' <param name="url">the raw url path.</param>
        ''' <returns>the normalized, slash-trimmed path.</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function normalize(url As String) As String
            If url Is Nothing Then
                Return ""
            End If
            Return url.Trim("/"c)
        End Function

        ''' <summary>
        ''' test whether a reflected method matches the required handler signature:
        ''' a <c>Sub</c> taking exactly two parameters of type
        ''' <see cref="HttpRequest"/> (or a derived type) and <see cref="HttpResponse"/>.
        ''' </summary>
        ''' <param name="method">the method to validate.</param>
        ''' <returns><c>True</c> when the signature is compatible.</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function matchSignature(method As MethodInfo) As Boolean
            If Not method.ReturnType Is GetType(Void) Then
                Return False
            End If

            Dim params As ParameterInfo() = method.GetParameters()

            If params.Length <> 2 Then
                Return False
            End If

            ' the second parameter must be exactly HttpResponse
            If Not GetType(HttpResponse).IsAssignableFrom(params(1).ParameterType) Then
                Return False
            End If

            ' the first parameter must be HttpRequest (HttpPOSTRequest is a subclass,
            ' so IsAssignableFrom covers both).
            If Not GetType(HttpRequest).IsAssignableFrom(params(0).ParameterType) Then
                Return False
            End If

            Return True
        End Function
    End Class
End Namespace
