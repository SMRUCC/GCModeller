Imports System.Net.Sockets
Imports Flute.Http.Core
Imports Flute.Http.FileSystem
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Text

Public Class FileHost : Inherits HttpServer

    ReadOnly virtual As New FileSystem(App.CurrentDirectory)

    Public Sub New(port As Integer, Optional threads As Integer = -1)
        MyBase.New(port, threads)
    End Sub

    Public Function addUploadFile(file As String) As String
        Dim res As String = "/" & file.GetFullPath.Replace(":/", "/").Split("/"c).Select(AddressOf UrlEncode).JoinBy("/")
        Call virtual.AddMapping(res, file)
        Return $"http://localhost:{localPort}{res}"
    End Function

    Public Function addUploadData(data As String, ext$) As String
        Dim res As String = App.NextTempName & $"/upload.{ext}"
        Dim type As ContentType

        Select Case ext.ToLower
            Case "json"
                type = New ContentType With {.Details = MIME.Json, .MIMEType = MIME.Json}
            Case "txt", "sif"
                type = New ContentType With {.Details = "plain/text", .MIMEType = "plain/text"}
            Case Else
                Throw New NotImplementedException(ext)
        End Select

        Call virtual.AddCache(res, Encodings.UTF8WithoutBOM.CodePage.GetBytes(data), type)
        Return $"http://localhost:{localPort}/{res}"
    End Function

    Public Overrides Sub handleGETRequest(p As HttpProcessor)
        Dim path As String = p.http_url
        Dim handler = p.openResponseStream

        If virtual.FileExists(path) Then
            Call handler.WriteHeader(virtual.GetContentType(path).MIMEType, virtual.GetFileSize(path))
            Call p.openResponseStream.Write(virtual.GetByteBuffer(path))
        Else
            Call p.openResponseStream.WriteError(404, "invalid file")
        End If
    End Sub

    Public Overrides Sub handlePOSTRequest(p As HttpProcessor, inputData As String)
        Throw New NotImplementedException()
    End Sub

    Public Overrides Sub handleOtherMethod(p As HttpProcessor)
        Throw New NotImplementedException()
    End Sub

    Protected Overrides Function getHttpProcessor(client As TcpClient, bufferSize As Integer) As HttpProcessor
        Return New HttpProcessor(client, Me, bufferSize)
    End Function
End Class