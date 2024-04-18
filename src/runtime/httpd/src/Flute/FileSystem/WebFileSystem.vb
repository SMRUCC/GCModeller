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