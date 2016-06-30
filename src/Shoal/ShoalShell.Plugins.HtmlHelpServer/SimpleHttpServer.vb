Imports System.Collections
Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

' offered to the public domain for any use with no restriction
' and also with no warranty of any kind, please enjoy. - David Jeske. 

' simple HTTP explanation
' http://www.jmarshall.com/easy/http/

Namespace Bend.Util

	Public Class HttpProcessor
		Public socket As TcpClient
		Public srv As HttpServer

        Dim _InternalInputStream As Stream

        Public outputStream As StreamWriter

        Public http_method As [String]
        Public http_url As [String]
        Public http_protocol_versionstring As [String]
        Public httpHeaders As New Hashtable()

        ''' <summary>
        ''' 10MB
        ''' </summary>
        ''' <remarks></remarks>
        Const MAX_POST_SIZE As Integer = 10 * 1024 * 1024

        Public Sub New(s As TcpClient, srv As HttpServer)
            Me.socket = s
            Me.srv = srv
        End Sub

        Private Function InternalStreamReadLine(InputStream As Stream) As String
            Dim NextChar As Integer
            Dim ChunkBuffer As New List(Of Char)

            While True
                NextChar = InputStream.ReadByte()
                If NextChar = Asc(ControlChars.Lf) Then
                    Exit While
                End If
                If NextChar = Asc(ControlChars.Cr) Then
                    Continue While
                End If
                If NextChar = -1 Then
                    Call Thread.Sleep(1)
                    Continue While
                End If

                Call ChunkBuffer.Add(Convert.ToChar(NextChar))
            End While

            Return New String(ChunkBuffer.ToArray)
        End Function

        Public Sub Process()
            ' we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            ' "processed" view of the world, and we want the data raw after the headers
            _InternalInputStream = New BufferedStream(socket.GetStream())

            ' we probably shouldn't be using a streamwriter for all output from handlers either
            outputStream = New StreamWriter(New BufferedStream(socket.GetStream()))
            Try
                parseRequest()
                readHeaders()
                If http_method.Equals("GET") Then
                    handleGETRequest()
                ElseIf http_method.Equals("POST") Then
                    HandlePOSTRequest()
                End If
            Catch e As Exception
                Console.WriteLine("Exception: " & e.ToString())
                writeFailure()
            End Try
            outputStream.Flush()
            ' bs.Flush(); // flush any remaining output
            _InternalInputStream = Nothing
            outputStream = Nothing
            ' bs = null;            
            socket.Close()
        End Sub

        Public Sub parseRequest()
            Dim request As [String] = InternalStreamReadLine(_InternalInputStream)
            Dim tokens As String() = request.Split(" "c)
            If tokens.Length <> 3 Then
                Throw New Exception("invalid http request line")
            End If
            http_method = tokens(0).ToUpper()
            http_url = tokens(1)
            http_protocol_versionstring = tokens(2)

            Console.WriteLine("starting: " & request)
        End Sub

        Public Sub readHeaders()
            Console.WriteLine("readHeaders()")
            Dim line As [String] = ""
            While (InlineAssignHelper(line, InternalStreamReadLine(_InternalInputStream))) IsNot Nothing
                If line.Equals("") Then
                    Console.WriteLine("got headers")
                    Return
                End If

                Dim separator As Integer = line.IndexOf(":"c)
                If separator = -1 Then
                    Throw New Exception("invalid http header line: " & line)
                End If
                Dim name As [String] = line.Substring(0, separator)
                Dim pos As Integer = separator + 1
                While (pos < line.Length) AndAlso (line(pos) = " "c)
                    ' strip any spaces
                    pos += 1
                End While

                Dim value As String = line.Substring(pos, line.Length - pos)
                Console.WriteLine("header: {0}:{1}", name, value)
                httpHeaders(name) = value
            End While
        End Sub

        Public Sub handleGETRequest()
            srv.handleGETRequest(Me)
        End Sub

        Private Const BUF_SIZE As Integer = 4096

        ''' <summary>
        ''' This post data processing just reads everything into a memory stream.
        ''' this is fine for smallish things, but for large stuff we should really
        ''' hand an input stream to the request processor. However, the input stream 
        ''' we hand him needs to let him see the "end of the stream" at this content 
        ''' length, because otherwise he won't know when he's seen it all! 
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub HandlePOSTRequest()

            Call Console.WriteLine("get post data start")

            Dim content_len As Integer = 0
            Dim ms As New MemoryStream()

            If Me.httpHeaders.ContainsKey("Content-Length") Then
                content_len = Convert.ToInt32(Me.httpHeaders("Content-Length"))
                If content_len > MAX_POST_SIZE Then
                    Throw New Exception([String].Format("POST Content-Length({0}) too big for this simple server", content_len))
                End If
                Dim buf As Byte() = New Byte(BUF_SIZE - 1) {}
                Dim to_read As Integer = content_len
                While to_read > 0
                    Console.WriteLine("starting Read, to_read={0}", to_read)

                    Dim numread As Integer = Me._InternalInputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read))
                    Console.WriteLine("read finished, numread={0}", numread)
                    If numread = 0 Then
                        If to_read = 0 Then
                            Exit While
                        Else
                            Throw New Exception("client disconnected during post")
                        End If
                    End If
                    to_read -= numread
                    ms.Write(buf, 0, numread)
                End While
                ms.Seek(0, SeekOrigin.Begin)
            End If

            Call Console.WriteLine("get post data end")
            Call srv.handlePOSTRequest(Me, New StreamReader(ms))
        End Sub

        Public Sub writeSuccess(Optional content_type As String = "text/html")
            ' this is the successful HTTP response line
            outputStream.WriteLine("HTTP/1.0 200 OK")
            ' these are the HTTP headers...          
            outputStream.WriteLine("Content-Type: " & content_type)
            outputStream.WriteLine("Connection: close")
            ' ..add your own headers here if you like

            outputStream.WriteLine("")
            ' this terminates the HTTP headers.. everything after this is HTTP body..
        End Sub

        Public Sub writeFailure()
            ' this is an http 404 failure response
            outputStream.WriteLine("HTTP/1.0 404 File not found")
            ' these are the HTTP headers
            outputStream.WriteLine("Connection: close")
            ' ..add your own headers here

            outputStream.WriteLine("")
            ' this terminates the HTTP headers.
        End Sub
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function
    End Class

    Public MustInherit Class HttpServer

        Protected _LocalPort As Integer

        Dim _InternalListener As TcpListener
        Dim Is_active As Boolean = True

        Public Sub New(port As Integer)
            Me._LocalPort = port
        End Sub

        Public Sub Listen()
            _InternalListener = New TcpListener(Net.IPAddress.Any, _LocalPort)
            _InternalListener.Start()

            While Is_active
                Dim s As TcpClient = _InternalListener.AcceptTcpClient()
                Dim processor As New HttpProcessor(s, Me)
                Dim thread__1 As New Thread(New ThreadStart(AddressOf processor.Process))
                Call thread__1.Start()
                Call Thread.Sleep(1)
            End While
        End Sub

        Public MustOverride Sub handleGETRequest(p As HttpProcessor)
        Public MustOverride Sub handlePOSTRequest(p As HttpProcessor, inputData As StreamReader)
    End Class
End Namespace



