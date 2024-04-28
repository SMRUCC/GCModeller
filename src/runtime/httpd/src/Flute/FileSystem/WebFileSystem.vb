#Region "Microsoft.VisualBasic::14ca3a71f3d0838f7584ff87970d0aa2, G:/GCModeller/src/runtime/httpd/src/Flute//FileSystem/WebFileSystem.vb"

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

    '   Total Lines: 52
    '    Code Lines: 35
    ' Comment Lines: 7
    '   Blank Lines: 10
    '     File Size: 1.89 KB


    '     Class WebFileSystemListener
    ' 
    '         Properties: fs
    ' 
    '         Sub: HostStaticFile, WebHandler
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Flute.Http.Core.Message
Imports Microsoft.VisualBasic.Net.HTTP
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace FileSystem

    ''' <summary>
    ''' combine this object with the <see cref="Flute.Http.Core.HttpSocket"/> 
    ''' module to create a simple local filesystem listener
    ''' </summary>
    Public Class WebFileSystemListener

        Public Property fs As FileSystem

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub WebHandler(request As HttpRequest, response As HttpResponse)
            Call HostStaticFile(fs, request, response)
        End Sub

        Public Shared Sub HostStaticFile(fs As FileSystem, request As HttpRequest, response As HttpResponse)
            Dim url As URL = request.URL
            Dim path As String = url.path

            If Not path.StringEmpty AndAlso path.Last = "/"c Then
                ' target url path is a directory path
                ' but request a file at here, so we needs
                ' to redirect to index.html
                path = path & "/index.html"
            End If

            response.AccessControlAllowOrigin = "*"

            If fs.FileExists(path) Then
                Dim mime As ContentType = fs.GetContentType(path)
                Dim res As Byte() = fs.GetByteBuffer(path)
                Dim content As New Content With {
                    .type = mime.MIMEType,
                    .length = res.Length
                }

                Call response _
                    .WriteHttp(content) _
                    .SendData(res)

                Erase res
            Else
                Call response.WriteError(HTTP_RFC.RFC_NOT_FOUND, "404 NOT FOUND: " & path.Replace("<", "&lt;"))
            End If
        End Sub
    End Class
End Namespace
