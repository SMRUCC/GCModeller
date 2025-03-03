Imports System.IO
Imports System.Net.Http

Namespace Net.WebClient

    Public Class HttpDownloader : Inherits WebClient

        ReadOnly _url As String
        ReadOnly _localPath As String
        ReadOnly _bufferSize As Integer = 8192

        Public Sub New(url As String, localPath As String)
            _url = url
            _localPath = localPath
        End Sub

        Public Overrides Async Function DownloadFileAsync() As Task
            Using httpClient As New HttpClient()
                Using response As HttpResponseMessage = Await httpClient.GetAsync(_url, HttpCompletionOption.ResponseHeadersRead)
                    Call response.EnsureSuccessStatusCode()
                    ' implements download file
                    Await RequestStream(response)
                End Using
            End Using

            Call ProgressFinished()
        End Function

        Private Async Function RequestStream(response As HttpResponseMessage) As Task
            Dim totalBytes As Long? = response.Content.Headers.ContentLength

            Using contentStream As Stream = Await response.Content.ReadAsStreamAsync(),
                fileStream As FileStream = New FileStream(_localPath, FileMode.Create, FileAccess.Write, FileShare.None,
                                                          bufferSize:=_bufferSize,
                                                          useAsync:=True)
                Dim totalRead As Long = 0L
                Dim buffer As Byte() = New Byte(_bufferSize - 1) {}
                Dim isMoreToRead As Boolean = True
                Dim bytesRead As Integer

                Call ProgressUpdate(New ProgressChangedEventArgs(0, CLng(totalBytes)))

                While isMoreToRead
                    bytesRead = Await contentStream.ReadAsync(buffer, 0, buffer.Length)

                    If bytesRead <= 0 Then
                        isMoreToRead = False
                    Else
                        Await fileStream.WriteAsync(buffer, 0, bytesRead)
                        totalRead += bytesRead

                        Call ProgressUpdate(New ProgressChangedEventArgs(totalRead, CLng(totalBytes)))
                    End If
                End While
            End Using
        End Function
    End Class
End Namespace