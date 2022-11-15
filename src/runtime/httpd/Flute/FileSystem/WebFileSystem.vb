Imports Flute.Http.Core.Message
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes

Namespace FileSystem

    ''' <summary>
    ''' combine this object with the <see cref="Flute.Http.Core.HttpSocket"/> 
    ''' module to create a simple local filesystem listener
    ''' </summary>
    Public Class WebFileSystemListener

        Public Property fs As FileSystem

        Public Sub WebHandler(request As HttpRequest, response As HttpResponse)
            Dim url As URL = request.URL
            Dim path As String = url.path

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
                Call response.WriteError(404, path)
            End If
        End Sub
    End Class
End Namespace