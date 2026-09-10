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
    ''' <see cref="HttpGet"/>, <see cref="HttpPost"/>, <see cref="HttpPut"/> or
    ''' <see cref="HttpDelete"/> (inheriting <see cref="ExportAPIAttribute"/>) are
    ''' registered as request handlers, keyed by the http method and the annotated
    ''' url. An url containing ``{name}`` placeholders is registered as a dynamic
    ''' template route (the captured values are exposed through
    ''' <see cref="HttpRequest.RouteData"/>). The router also supports manual
    ''' registration of handlers through <see cref="Register(String, String, AppHandler)"/>.
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
        ''' a parsed dynamic url template route containing ``{name}`` placeholders,
        ''' for example ``/v3-flatcontainer/{id}/index.json``.
        ''' </summary>
        Private Class RouteTemplate

            ''' <summary>
            ''' the upper-case http method this template is bound to.
            ''' </summary>
            Public ReadOnly method As String

            ''' <summary>
            ''' the original url template text (kept for diagnostics).
            ''' </summary>
            Public ReadOnly template As String

            ''' <summary>
            ''' the template path split into segments (leading/trailing slashes trimmed).
            ''' </summary>
            Public ReadOnly segments As String()

            ''' <summary>
            ''' the route entry invoked when this template matches.
            ''' </summary>
            Public ReadOnly entry As RouteEntry

            Public Sub New(httpMethod As String, url As String, entry As RouteEntry)
                Me.method = httpMethod
                Me.template = url
                Me.segments = normalize(url).Split("/"c)
                Me.entry = entry
            End Sub

            ''' <summary>
            ''' try to match the given request path segments, capturing the
            ''' ``{name}`` placeholder values into <paramref name="captures"/>.
            ''' </summary>
            ''' <param name="pathSegments">the request path split into segments.</param>
            ''' <param name="captures">the captured placeholder values on success.</param>
            ''' <returns><c>True</c> when the path matches this template.</returns>
            Public Function Match(pathSegments As String(), ByRef captures As Dictionary(Of String, String)) As Boolean
                If segments.Length <> pathSegments.Length Then
                    Return False
                End If

                Dim data As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)

                For i As Integer = 0 To segments.Length - 1
                    Dim seg As String = segments(i)

                    If seg.Length > 1 AndAlso seg.StartsWith("{"c) AndAlso seg.EndsWith("}"c) Then
                        Dim name As String = seg.Substring(1, seg.Length - 2)
                        data(name) = pathSegments(i).UrlDecode
                    ElseIf Not String.Equals(seg, pathSegments(i), StringComparison.OrdinalIgnoreCase) Then
                        Return False
                    End If
                Next

                captures = data
                Return True
            End Function
        End Class

        ''' <summary>
        ''' exact route tables keyed by the http method and normalized url path.
        ''' The dictionary key is formatted as ``METHOD path`` (for example
        ''' ``GET user/info``).
        ''' </summary>
        ReadOnly exactRoutes As New Dictionary(Of String, RouteEntry)

        ''' <summary>
        ''' the dynamic url template routes (containing ``{name}`` placeholders),
        ''' evaluated in registration order when no exact route matched.
        ''' </summary>
        ReadOnly templateRoutes As New List(Of RouteTemplate)

        Dim wfs As WebFileSystemListener

        ''' <summary>
        ''' the static file system listener mounted through
        ''' <see cref="MountFs(WebFileSystemListener)"/>; may be <c>Nothing</c>
        ''' when this router only handles clr application routes.
        ''' </summary>
        ''' <returns>the mounted <see cref="WebFileSystemListener"/>, or <c>Nothing</c>.</returns>
        Public ReadOnly Property FileSystem As WebFileSystemListener
            Get
                Return wfs
            End Get
        End Property

        ''' <summary>
        ''' the number of registered route entries (exact + template), useful for diagnostics.
        ''' </summary>
        ''' <returns>the total count of registered routes.</returns>
        Public ReadOnly Property Routes As Integer
            Get
                Return exactRoutes.Count + templateRoutes.Count
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
        ''' and register every method that is annotated with <see cref="HttpGet"/>,
        ''' <see cref="HttpPost"/>, <see cref="HttpPut"/> or <see cref="HttpDelete"/>
        ''' and whose signature matches <c>Sub(HttpRequest, HttpResponse)</c>.
        ''' The url may contain ``{name}`` placeholders to define a dynamic route.
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
                Dim putAttr As HttpPut = method.GetCustomAttribute(Of HttpPut)()
                Dim deleteAttr As HttpDelete = method.GetCustomAttribute(Of HttpDelete)()

                If getAttr Is Nothing AndAlso postAttr Is Nothing AndAlso putAttr Is Nothing AndAlso deleteAttr Is Nothing Then
                    Continue For
                End If

                ' signature must be: Sub(HttpRequest, HttpResponse)
                If Not matchSignature(method) Then
                    Call $"Skip route method '{type.Name}.{method.Name}' due to incompatible signature.".warning()
                    Continue For
                End If

                Dim httpMethod As String
                Dim url As String
                Dim attribute As ExportAPIAttribute

                If getAttr IsNot Nothing Then
                    httpMethod = "GET" : url = getAttr.Url : attribute = getAttr
                ElseIf postAttr IsNot Nothing Then
                    httpMethod = "POST" : url = postAttr.Url : attribute = postAttr
                ElseIf putAttr IsNot Nothing Then
                    httpMethod = "PUT" : url = putAttr.Url : attribute = putAttr
                Else
                    httpMethod = "DELETE" : url = deleteAttr.Url : attribute = deleteAttr
                End If

                Dim entry As New RouteEntry With {
                    .target = controller,
                    .method = method,
                    .handler = Nothing
                }

                Call addRoute(httpMethod, url, entry)
                Call $"registered {attribute.ToString} -> {type.Name}.{method.Name}".debug()
            Next

            Return Me
        End Function

        ''' <summary>
        ''' register a route entry keyed by the given http method and url. a url
        ''' containing ``{name}`` placeholders is registered as a dynamic template
        ''' route, otherwise it is registered as an exact route.
        ''' </summary>
        ''' <param name="httpMethod">the http method name (GET/POST/PUT/DELETE...).</param>
        ''' <param name="url">the url path, optionally with ``{name}`` placeholders.</param>
        ''' <param name="entry">the route entry to register.</param>
        Private Sub addRoute(httpMethod As String, url As String, entry As RouteEntry)
            Dim method As String = normalizeMethod(httpMethod)

            If url.IndexOf("{"c) >= 0 Then
                templateRoutes.Add(New RouteTemplate(method, url, entry))
            Else
                exactRoutes(method & " " & normalize(url)) = entry
            End If
        End Sub

        ''' <summary>
        ''' manually register a handler delegate for the given http method and url.
        ''' The method is honored as-is (GET/POST/PUT/DELETE...), so the same path
        ''' may be registered once per http method. The url may contain ``{name}``
        ''' placeholders for a dynamic route.
        ''' </summary>
        ''' <param name="httpMethod">the http method name, e.g. "GET", "POST" or "PUT".</param>
        ''' <param name="url">the url path to match, e.g. "/user/info" or "/pkg/{id}".</param>
        ''' <param name="handler">the handler delegate matching <see cref="AppHandler"/>.</param>
        ''' <returns>this router, for fluent registration chaining.</returns>
        Public Function Register(httpMethod As String, url As String, handler As AppHandler) As HttpRouter
            Dim entry As New RouteEntry With {
                .target = Nothing,
                .method = Nothing,
                .handler = handler
            }

            Call addRoute(httpMethod, url, entry)
            Call $"registered {normalizeMethod(httpMethod)} route '{url}' (manual)".debug()

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
            Dim method As String = normalizeMethod(request.HTTPMethod)
            Dim path As String = normalize(request.URL.path)

            Dim entry As RouteEntry = Nothing
            Dim routeData As Dictionary(Of String, String) = Nothing

            If Not exactRoutes.TryGetValue(method & " " & path, entry) Then
                ' no exact route hit: fallback to the dynamic url templates
                Dim pathSegments As String() = If(path.Length = 0, New String() {}, path.Split("/"c))

                For Each route As RouteTemplate In templateRoutes
                    If Not String.Equals(route.method, method, StringComparison.OrdinalIgnoreCase) Then
                        Continue For
                    End If

                    If route.Match(pathSegments, routeData) Then
                        entry = route.entry
                        Exit For
                    End If
                Next
            End If

            If entry IsNot Nothing Then
                ' expose the captured url template parameters to the handler
                request.RouteData = routeData

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
                Call $"no route registered for {request.HTTPMethod} '{path}'".warning()
                Call response.WriteError(HTTP_RFC.RFC_NOT_FOUND, $"404 Not Found: {request.HTTPMethod} {path}")
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
        ''' normalize an http method name to its upper-case form, defaulting to
        ''' ``GET`` when the value is empty.
        ''' </summary>
        ''' <param name="httpMethod">the raw http method name.</param>
        ''' <returns>the upper-case method name.</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function normalizeMethod(httpMethod As String) As String
            If httpMethod.StringEmpty Then
                Return "GET"
            Else
                Return httpMethod.Trim.ToUpperInvariant
            End If
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
