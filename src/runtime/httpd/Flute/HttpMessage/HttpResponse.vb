#Region "Microsoft.VisualBasic::28c47be39aae8957b13f043a54d1eeac, WebCloud\SMRUCC.HTTPInternal\Core\HttpRequest\HttpResponse.vb"

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

'     Class HttpResponse
' 
'         Properties: AccessControlAllowOrigin
' 
'         Constructor: (+1 Overloads) Sub New
' 
'         Function: FlushAsync, (+3 Overloads) WriteAsync, (+4 Overloads) WriteLineAsync, writeSuccess
' 
'         Sub: __writeSuccess, Close, (+2 Overloads) Dispose, Flush, Redirect
'              SendFile, SetCookies, (+6 Overloads) Write, Write404, WriteHeader
'              (+3 Overloads) WriteHTML, WriteJSON, WriteLine, WriteXML
' 
'         Operators: <=, >=
' 
' 
' /********************************************************************************/

#End Region

Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.ComponentModel
Imports Microsoft.VisualBasic.Language.C
Imports Microsoft.VisualBasic.Net.Http
Imports Microsoft.VisualBasic.Net.Protocols.ContentTypes
Imports Microsoft.VisualBasic.Serialization.JSON
Imports Microsoft.VisualBasic.Text

Namespace Core.Message

    Public Delegate Sub HttpError(code%, Msg As String)

    Public Class HttpResponse : Inherits ITextWriter
        Implements IDisposable

        Friend ReadOnly response As StreamWriter
        Friend ReadOnly writeFailed As HttpError

        Dim __writeHTML As Boolean = False
        Dim __writeData As Boolean = False
        Dim __customHeaders As New Dictionary(Of String, String)

        Public Property AccessControlAllowOrigin As String

        Sub New(rep As StreamWriter, [error] As HttpError)
            response = rep
            writeFailed = [error]
        End Sub

        ''' <summary>
        ''' 在这里只需要将错误消息放进来就行了，页面使用自定义的模板
        ''' </summary>
        ''' <param name="message"></param>
        ''' <remarks>
        ''' calling <see cref="writeFailed"/> function to show error message to the browser
        ''' </remarks>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub WriteError(code$, message$)
            Call writeFailed(code, message)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Redirect(url As String)
            Call WriteHTML(<script>window.location='%s';</script>, url)
        End Sub

        Public Sub WriteHeader(MIMEType As String, contentLength As Integer)
            Call WriteHttp(New Content With {.length = contentLength, .type = MIMEType})
        End Sub

        Public Sub SendFile(path As String)
            Dim contentType$ = path.FileMimeType.MIMEType
            Call path.TransferBinary(contentType, Me)
        End Sub

        ''' <summary>
        ''' the function <see cref="WriteHttp(Content)"/> should 
        ''' be called before this function to send data payload.
        ''' </summary>
        ''' <param name="data"></param>
        Public Sub SendData(data As Byte())
            Using buffer As New MemoryStream(data)
                Call buffer.CopyTo(
                    destination:=response.BaseStream
                )
                Call response.Flush()
            End Using
        End Sub

        Public Sub WriteHTML(html As String)
            ' 如果writeData是True，则说明在这之前已经写了其他数据，就不写http头部了
            If Not __writeHTML AndAlso Not __writeData Then
                __writeHTML = writeSuccess()
            End If

            Call response.WriteLine(html)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub WriteHTML(html As StringBuilder)
            Call WriteHTML(html.ToString)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function writeSuccess() As Boolean
            Try
                Call WriteHttp(New Content With {.type = "text/html"})
            Catch ex As Exception
                Call App.LogException(ex)
            End Try

            Return True
        End Function

        Public Sub AddCustomHttpHeader(header As String, value As String)
            __customHeaders(header) = value
        End Sub

        ''' <summary>
        ''' 将需要保存到浏览器的数据通过response header的形式返回
        ''' </summary>
        ''' <param name="cookies"></param>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub SetCookies(cookies As Dictionary(Of String, String))
            __customHeaders(HeaderToString(HttpHeaderName.SetCookie)) = (
                From data As KeyValuePair(Of String, String)
                In cookies
                Select $"{data.Key}={data.Value}"
            ).JoinBy("; ")
        End Sub

        Public Sub WriteHttp(contentType As String, contentLength As Integer)
            Call WriteHttp(New Content With {.type = contentType, .length = contentLength})
        End Sub

        ''' <summary>
        ''' write http headers
        ''' </summary>
        ''' <param name="content"></param>
        ''' <remarks>
        ''' this function will terminates the http header and 
        ''' anything after this function calls should be the 
        ''' data payload to the browser.
        ''' </remarks>
        Public Function WriteHttp(content As Content) As HttpResponse
            ' this is the successful HTTP response line
            response.WriteLine("HTTP/1.0 200 OK")
            ' these are the HTTP headers...          
            response.WriteLine("Content-Type: " & content.type)
            response.WriteLine("Connection: close")
            ' ..add your own headers here if you like

            If Not AccessControlAllowOrigin.StringEmpty Then
                response.WriteLine("Access-Control-Allow-Origin: " & AccessControlAllowOrigin)
            End If

            Call content.WriteHeader(response)

            response.WriteLine(HttpProcessor.XPoweredBy)

            For Each header As KeyValuePair(Of String, String) In __customHeaders
                response.WriteLine($"{header.Key}: {header.Value}")
            Next

            ' this terminates the HTTP headers.. everything after this is HTTP body..
            response.WriteLine()
            response.Flush()

            Return Me
        End Function

        ''' <summary>
        ''' %s %d, etc
        ''' </summary>
        ''' <param name="html">C language like printf function format usage.</param>
        ''' <param name="args"></param>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub WriteHTML(html As XElement, ParamArray args As Object())
            Call WriteHTML(sprintf(html.ToString, args))
        End Sub

        ''' <summary>
        ''' 如果还没有添加header的话，这个函数会自动的添加http header
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="obj"></param>
        Public Sub WriteJSON(Of T)(obj As T, Optional indent As Boolean = False)
            Dim json As String = obj.GetJson(indent:=indent)
            Dim bytes As Byte() = TextEncodings.UTF8WithoutBOM.GetBytes(json)

            If Not __writeData Then
                __writeData = True
                Call WriteHttp(New Content With {.length = bytes.Length, .type = MIME.Json})
            End If

            Call response.BaseStream.Write(bytes, Scan0, bytes.Length)
            Call response.BaseStream.Flush()
        End Sub

        Public Sub WriteXML(Of T)(obj As T)
            __writeData = True
            Call response.WriteLine(obj.GetXml)
        End Sub

        Public Overloads Sub Write(byts As Byte())
            __writeData = True
            Call response.BaseStream.Write(byts, Scan0, byts.Length)
        End Sub

        Public Overloads Sub Write(byts As Byte(), offset As Integer, count As Integer)
            __writeData = True
            Call response.BaseStream.Write(byts, offset, count)
        End Sub

        ' Exceptions:
        '   T:System.Text.EncoderFallbackException:
        '     The current encoding does not support displaying half of a Unicode surrogate
        '     pair.

        ''' <summary>
        ''' Closes the current StreamWriter object and the underlying stream.
        ''' </summary>
        Public Sub Close()
            Call response.Close()
            Call response.Dispose()
        End Sub

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The current writer is closed.
        '
        '   T:System.IO.IOException:
        '     An I/O error has occurred.
        '
        '   T:System.Text.EncoderFallbackException:
        '     The current encoding does not support displaying half of a Unicode surrogate
        '     pair.

        ''' <summary>
        ''' Clears all buffers for the current writer and causes any buffered data to be
        ''' written to the underlying stream.
        ''' </summary>
        ''' 
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub Flush()
            Call response.Flush()
        End Sub

        ' Exceptions:
        '   T:System.IO.IOException:
        '     An I/O error occurs.
        '
        '   T:System.ObjectDisposedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and current writer is closed.
        '
        '   T:System.NotSupportedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and the contents of the buffer cannot be written to the underlying fixed
        '     size stream because the System.IO.StreamWriter is at the end the stream.

        ''' <summary>
        ''' Writes a character to the stream.
        ''' </summary>
        ''' <param name="value">The character to write to the stream.</param>
        Public Overloads Sub Write(value As Char)
            __writeData = True
            Call response.Write(value)
        End Sub

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and current writer is closed.
        '
        '   T:System.NotSupportedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and the contents of the buffer cannot be written to the underlying fixed
        '     size stream because the System.IO.StreamWriter is at the end the stream.
        '
        '   T:System.IO.IOException:
        '     An I/O error occurs.

        ''' <summary>
        ''' Writes a string to the stream.
        ''' </summary>
        ''' <param name="value">The string to write to the stream. If value is null, nothing is written.</param>
        Public Overrides Sub Write(value As String)
            Dim bytes As Byte() = Encoding.UTF8.GetBytes(value)

            If Not __writeData Then
                __writeData = True
                Call WriteHttp(New Content With {.length = bytes.Length, .type = MIME.Html})
            End If

            Call response.Write(value)
        End Sub

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Overrides Sub WriteLine(s As String)
            Call Write(value:=s & vbCrLf)
        End Sub

        ' Exceptions:
        '   T:System.IO.IOException:
        '     An I/O error occurs.
        '
        '   T:System.ObjectDisposedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and current writer is closed.
        '
        '   T:System.NotSupportedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and the contents of the buffer cannot be written to the underlying fixed
        '     size stream because the System.IO.StreamWriter is at the end the stream.

        ''' <summary>
        ''' Writes a character array to the stream.
        ''' </summary>
        ''' <param name="buffer">A character array containing the data to write. If buffer is null, nothing is
        ''' written.</param>
        Public Overloads Sub Write(buffer() As Char)
            __writeData = True
            Call response.Write(buffer)
        End Sub

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     buffer is null.
        '
        '   T:System.ArgumentException:
        '     The buffer length minus index is less than count.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     index or count is negative.
        '
        '   T:System.IO.IOException:
        '     An I/O error occurs.
        '
        '   T:System.ObjectDisposedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and current writer is closed.
        '
        '   T:System.NotSupportedException:
        '     System.IO.StreamWriter.AutoFlush is true or the System.IO.StreamWriter buffer
        '     is full, and the contents of the buffer cannot be written to the underlying fixed
        '     size stream because the System.IO.StreamWriter is at the end the stream.

        ''' <summary>
        ''' Writes a subarray of characters to the stream.
        ''' </summary>
        ''' <param name="buffer">A character array that contains the data to write.</param>
        ''' <param name="index">The character position in the buffer at which to start reading data.</param>
        ''' <param name="count">The maximum number of characters to write.</param>
        Public Overloads Sub Write(buffer() As Char, index As Integer, count As Integer)
            __writeData = True
            Call response.Write(buffer, index, count)
        End Sub

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream has been disposed.

        ''' <summary>
        ''' Clears all buffers for this stream asynchronously and causes any buffered data
        ''' to be written to the underlying device.
        ''' </summary>
        ''' <returns>A task that represents the asynchronous flush operation.</returns>
        <ComVisible(False)>
        Public Function FlushAsync() As Tasks.Task
            Return response.FlushAsync
        End Function

        '
        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.

        ''' <summary>
        ''' Writes a character to the stream asynchronously.
        ''' </summary>
        ''' <param name="value">The character to write to the stream.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteAsync(value As Char) As Tasks.Task
            __writeData = True
            Return response.WriteAsync(value)
        End Function

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.

        ''' <summary>
        ''' Writes a string to the stream asynchronously.
        ''' </summary>
        ''' <param name="value">The string to write to the stream. If value is null, nothing is written.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteAsync(value As String) As Tasks.Task
            __writeData = True
            Return response.WriteAsync(value)
        End Function

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     buffer is null.
        '
        '   T:System.ArgumentException:
        '     The index plus count is greater than the buffer length.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     index or count is negative.
        '
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.

        ''' <summary>
        ''' Writes a subarray of characters to the stream asynchronously.
        ''' </summary>
        ''' <param name="buffer">A character array that contains the data to write.</param>
        ''' <param name="index">The character position in the buffer at which to begin reading data.</param>
        ''' <param name="count">The maximum number of characters to write.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteAsync(buffer() As Char, index As Integer, count As Integer) As Tasks.Task
            __writeData = True
            Return response.WriteAsync(buffer, index, count)
        End Function

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.

        ''' <summary>
        ''' Writes a line terminator asynchronously to the stream.
        ''' </summary>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteLineAsync() As Tasks.Task
            __writeData = True
            Return response.WriteLineAsync
        End Function

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.

        ''' <summary>
        ''' Writes a character followed by a line terminator asynchronously to the stream.
        ''' </summary>
        ''' <param name="value">The character to write to the stream.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteLineAsync(value As Char) As Tasks.Task
            __writeData = True
            Return response.WriteLineAsync(value)
        End Function

        ' Exceptions:
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.
        ''' <summary>
        ''' Writes a string followed by a line terminator asynchronously to the stream.
        ''' </summary>
        ''' <param name="value">The string to write. If the value is null, only a line terminator is written.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteLineAsync(value As String) As Tasks.Task
            __writeData = True
            Return response.WriteLineAsync(value)
        End Function

        ' Exceptions:
        '   T:System.ArgumentNullException:
        '     buffer is null.
        '
        '   T:System.ArgumentException:
        '     The index plus count is greater than the buffer length.
        '
        '   T:System.ArgumentOutOfRangeException:
        '     index or count is negative.
        '
        '   T:System.ObjectDisposedException:
        '     The stream writer is disposed.
        '
        '   T:System.InvalidOperationException:
        '     The stream writer is currently in use by a previous write operation.

        ''' <summary>
        ''' Writes a subarray of characters followed by a line terminator asynchronously
        ''' to the stream.
        ''' </summary>
        ''' <param name="buffer">The character array to write data from.</param>
        ''' <param name="index">The character position in the buffer at which to start reading data.</param>
        ''' <param name="count">The maximum number of characters to write.</param>
        ''' <returns>A task that represents the asynchronous write operation.</returns>
        <ComVisible(False)>
        Public Function WriteLineAsync(buffer() As Char, index As Integer, count As Integer) As Tasks.Task
            __writeData = True
            Return response.WriteLineAsync(buffer, index, count)
        End Function

        ''' <summary>
        ''' url重定向跳转操作
        ''' </summary>
        ''' <param name="rep"></param>
        ''' <param name="url"></param>
        ''' <returns></returns>
        Public Shared Operator <=(rep As HttpResponse, url As String) As Boolean
            Call rep.Redirect(url)
            Return True
        End Operator

        Public Shared Operator >=(rep As HttpResponse, url As String) As Boolean
            Throw New NotSupportedException
        End Operator

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
            End If
            disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            ' TODO: uncomment the following line if Finalize() is overridden above.
            ' GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
