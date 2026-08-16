#Region "Microsoft.VisualBasic::546398d31d78da34688125bb5f18356c, src\Flute\FileSystem\WebFileSystem.vb"

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

    '   Total Lines: 176
    '    Code Lines: 116 (65.91%)
    ' Comment Lines: 27 (15.34%)
    '    - Xml Docs: 48.15%
    ' 
    '   Blank Lines: 33 (18.75%)
    '     File Size: 6.64 KB


    '     Class WebFileSystemListener
    ' 
    '         Properties: fs, webContext, wwwroot
    ' 
    '         Constructor: (+2 Overloads) Sub New
    ' 
    '         Function: CheckResourceFileExists, CommonGetPath, ContainsPathTraversal
    ' 
    '         Sub: (+2 Overloads) HostStaticFile, WebHandler
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports Flute.Http.Core
Imports Flute.Http.Core.Message
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Unit
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace FileSystem

    ''' <summary>
    ''' a static file server listener: combine this object with the
    ''' <see cref="Flute.Http.Core.HttpSocket"/> module to serve files from one or
    ''' more <see cref="FileSystem"/> roots over http.
    ''' </summary>
    Public Class WebFileSystemListener

        ''' <summary>
        ''' the set of file system roots that this listener serves files from.
        ''' </summary>
        Public Property fs As FileSystem()

        ''' <summary>
        ''' the first (primary) file system's root environment, exposed as the web
        ''' application context.
        ''' </summary>
        ''' <returns>the root <see cref="IFileSystemEnvironment"/> of the first file system.</returns>
        Public ReadOnly Property webContext As IFileSystemEnvironment
            Get
                Return fs(0).wwwroot
            End Get
        End Property

        ''' <summary>
        ''' the physical folder path of the primary wwwroot, when it is backed by a
        ''' real directory; otherwise <c>Nothing</c>.
        ''' </summary>
        ''' <returns>the wwwroot folder path, or <c>Nothing</c>.</returns>
        Public ReadOnly Property wwwroot As String
            Get
                If TypeOf fs(0).wwwroot Is Microsoft.VisualBasic.FileIO.Directory Then
                    Return DirectCast(fs(0).wwwroot, Microsoft.VisualBasic.FileIO.Directory).folder
                Else
                    Return Nothing
                End If
            End Get
        End Property

        ''' <summary>
        ''' create an empty web file system listener (roots added later).
        ''' </summary>
        Sub New()
        End Sub

        ''' <summary>
        ''' create a web file system listener serving the given file system roots.
        ''' </summary>
        ''' <param name="wwwroot">the file system roots to serve files from.</param>
        Sub New(ParamArray wwwroot As FileSystem())
            fs = wwwroot
        End Sub

        ''' <summary>
        ''' the request handler: resolve the request path, then host the matching
        ''' static file (or first matching root) back to the client.
        ''' </summary>
        ''' <param name="request">the incoming http request.</param>
        ''' <param name="response">the response to write the file to.</param>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub WebHandler(request As HttpRequest, response As HttpResponse)
            Dim path As String = CommonGetPath(request)

            If _fs.Length = 1 Then
                Call HostStaticFile(_fs(0), path, response)
            Else
                For Each dir As FileSystem In fs
                    If dir.FileExists(path) Then
                        Call HostStaticFile(dir, request, response)
                        Exit For
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' test whether the requested resource exists in any of the served file
        ''' systems.
        ''' </summary>
        ''' <param name="request">the incoming http request.</param>
        ''' <returns><c>True</c> when the resource exists.</returns>
        Public Function CheckResourceFileExists(request As HttpRequest) As Boolean
            Dim path As String = CommonGetPath(request)

            If _fs.Length = 1 Then
                Return _fs(0).FileExists(path)
            End If

            ' processing multiple folder resource
            For Each dir As FileSystem In fs
                If dir.FileExists(path) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Private Shared Function CommonGetPath(ByRef request As HttpRequest) As String
            Dim url As URL = request.URL
            Dim path As String = url.path

            If Not path.StringEmpty AndAlso path.Last = "/"c Then
                ' target url path is a directory path
                ' but request a file at here, so we needs
                ' to redirect to index.html
                path = path.TrimEnd("/"c) & "/index.html"
            End If

            ' 20250227
            ' deal with the possible url encode string parts
            Return path.UrlDecode
        End Function

        ''' <summary>
        ''' threshold (bytes) below which the whole file is buffered into memory,
        ''' above which it is streamed to the client to avoid large memory usage.
        ''' </summary>
        Const STREAM_THRESHOLD% = ByteSize.MB

        Private Shared Sub HostStaticFile(ByRef fs As FileSystem, ByRef path As String, ByRef response As HttpResponse)
            ' security: prevent path traversal (../) attacks by ensuring the
            ' resolved physical path stays inside the wwwroot directory.
            If ContainsPathTraversal(path) Then
                Call response.WriteError(HTTP_RFC.RFC_FORBIDDEN, "403 Forbidden: path traversal detected")
                Return
            End If

            Dim mime As ContentType = fs.GetContentType(path)
            Dim fileSize As Integer = fs.GetFileSize(path)

            response.AccessControlAllowOrigin = "*"

            If fileSize <= STREAM_THRESHOLD Then
                ' small file: read fully into memory and send
                Dim res As Byte() = fs.GetByteBuffer(path)
                Dim content As New Content With {
                    .type = mime.MIMEType,
                    .length = res.Length
                }

                response _
                    .WriteHttp(content) _
                    .SendData(res)

                Erase res
            Else
                ' large file: stream directly from the source stream to the client
                Using fileStream As Stream = fs.GetResource(path)
                    Dim content As New Content With {
                        .type = mime.MIMEType,
                        .length = CInt(fileSize)
                    }

                    response.WriteHttp(content)

                    If fileStream IsNot Nothing Then
                        Call fileStream.CopyTo(response.response.BaseStream, HttpProcessor.BUF_SIZE)
                    End If

                    Call response.Flush()
                End Using
            End If
        End Sub

        ''' <summary>
        ''' resolve the request path, then host the matching static file (or a 404
        ''' error response when the resource does not exist).
        ''' </summary>
        ''' <param name="fs">the file system to serve the resource from.</param>
        ''' <param name="request">the incoming http request.</param>
        ''' <param name="response">the response to write the file to.</param>
        Public Shared Sub HostStaticFile(fs As FileSystem, request As HttpRequest, response As HttpResponse)
            Dim path As String = CommonGetPath(request)

            If fs.FileExists(path) Then
                Call HostStaticFile(fs, path, response)
            Else
                Call response.WriteError(HTTP_RFC.RFC_NOT_FOUND, "404 NOT FOUND: " & path.Replace("<", "&lt;"))
            End If
        End Sub

        ''' <summary>
        ''' detect path traversal attempts like ``../`` or ``..\`` that could
        ''' escape the wwwroot directory. Also normalises the path and checks
        ''' that the resolved full path still starts with the wwwroot prefix.
        ''' </summary>
        Private Shared Function ContainsPathTraversal(path As String) As Boolean
            If path.StringEmpty Then Return False

            ' reject obvious traversal sequences
            If path.Contains("..") Then Return True

            ' reject absolute paths that could target drives or UNC shares
            If path.StartsWith("/") AndAlso path.Length > 1 AndAlso path(1) <> "/"c Then
                ' a single leading slash is a normal absolute URL path, leave it
            End If

            ' check for Windows drive letters or UNC paths
            If path.Length >= 2 AndAlso path(1) = ":"c Then Return True
            If path.StartsWith("\\") Then Return True

            Return False
        End Function
    End Class
End Namespace
