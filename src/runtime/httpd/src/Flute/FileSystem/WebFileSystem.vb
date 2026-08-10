#Region "Microsoft.VisualBasic::4ccebc252c5bd523a95317123888a5ce, src\Flute\FileSystem\WebFileSystem.vb"

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
'    Code Lines: 85 (73.28%)
' Comment Lines: 10 (8.62%)
'    - Xml Docs: 40.00%
' 
'   Blank Lines: 21 (18.10%)
'     File Size: 3.96 KB


'     Class WebFileSystemListener
' 
'         Properties: fs, webContext, wwwroot
' 
'         Constructor: (+2 Overloads) Sub New
' 
'         Function: CheckResourceFileExists, CommonGetPath
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
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace FileSystem

    ''' <summary>
    ''' combine this object with the <see cref="Flute.Http.Core.HttpSocket"/> 
    ''' module to create a simple local filesystem listener
    ''' </summary>
    Public Class WebFileSystemListener

        Public Property fs As FileSystem()

        Public ReadOnly Property webContext As IFileSystemEnvironment
            Get
                Return fs(0).wwwroot
            End Get
        End Property

        Public ReadOnly Property wwwroot As String
            Get
                If TypeOf fs(0).wwwroot Is Microsoft.VisualBasic.FileIO.Directory Then
                    Return DirectCast(fs(0).wwwroot, Microsoft.VisualBasic.FileIO.Directory).folder
                Else
                    Return Nothing
                End If
            End Get
        End Property

        Sub New()
        End Sub

        Sub New(ParamArray wwwroot As FileSystem())
            fs = wwwroot
        End Sub

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
        Const STREAM_THRESHOLD% = 1024 * 1024

        Private Shared Sub HostStaticFile(ByRef fs As FileSystem, ByRef path As String, ByRef response As HttpResponse)
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

        Public Shared Sub HostStaticFile(fs As FileSystem, request As HttpRequest, response As HttpResponse)
            Dim path As String = CommonGetPath(request)

            If fs.FileExists(path) Then
                Call HostStaticFile(fs, path, response)
            Else
                Call response.WriteError(HTTP_RFC.RFC_NOT_FOUND, "404 NOT FOUND: " & path.Replace("<", "&lt;"))
            End If
        End Sub
    End Class
End Namespace
