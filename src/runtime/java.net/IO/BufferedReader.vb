Imports Microsoft.VisualBasic.Language

Namespace IO

    ''' <summary>
    ''' Reads text from a character-input stream, buffering characters so as to provide for the efficient reading of characters, arrays, and lines.
    ''' The buffer size may be specified, or the default size may be used. The default is large enough for most purposes.
    ''' 
    ''' In general, each read request made of a Reader causes a corresponding read request to be made of the underlying character or byte stream. 
    ''' It is therefore advisable to wrap a BufferedReader around any Reader whose read() operations may be costly, such as FileReaders and InputStreamReaders. 
    ''' For example, BufferedReader in
    ''' 
    '''           = New BufferedReader(nNew FileReader("foo.in"))
    ''' 
    ''' will buffer the input from the specified file. Without buffering, each invocation of read() or readLine() could cause bytes to be read from the file, 
    ''' converted into characters, and then returned, which can be very inefficient.
    ''' Programs that use DataInputStreams for textual input can be localized by replacing each DataInputStream with an appropriate BufferedReader.
    ''' </summary>
    ''' <remarks>读取的结果是字符串类型</remarks>
    Public Class BufferedReader : Inherits Reader
        Implements IDisposable
        Implements Closeable, AutoCloseable, Readable

        Protected Friend _CacheBulk As String, _ChunkBuffer As String()
        Dim _currentLine As Char()

        ''' <summary>
        ''' 当前所读取的行指针
        ''' </summary>
        ''' <remarks></remarks>
        Protected p As i32 = 0, idx As Integer = 0
        Protected _size As Integer

        Protected Friend _reader As Reader

#Region "Constructor Detail"

        ''' <summary>
        ''' Creates a buffering character-input stream that uses an input buffer of the specified size.
        ''' </summary>
        ''' <param name="in">A Reader</param>
        ''' <param name="sz">Input-buffer size</param>
        ''' <remarks></remarks>
        Public Sub New([in] As IO.Reader,
                        sz As Integer)
            _reader = [in]
            _size = sz
        End Sub

        ''' <summary>
        ''' Creates a buffering character-input stream that uses a default-sized input buffer.
        ''' </summary>
        ''' <param name="in">A Reader</param>
        ''' <remarks></remarks>
        Public Sub New([in] As IO.Reader)
            _reader = [in]
            Call buffering()
        End Sub
#End Region

#Region "Method Detail"

        Private Sub buffering()
            _CacheBulk = Me._reader.readToEnd
            Me._ChunkBuffer = Strings.Split(Me._CacheBulk, vbLf)
        End Sub

        ''' <summary>
        ''' Reads a single character.
        ''' </summary>
        ''' <returns>The character read, as an integer in the range 0 to 65535 (0x00-0xffff), or -1 if the end of the stream has been reached</returns>
        ''' <remarks></remarks>
        Public Overrides Function read() As Integer
            If idx = _currentLine.Count Then
                _currentLine = readLine() '当前行已经读取完毕，进行换行
            End If

            Dim ch = _currentLine(idx)
            idx += 1
            Return AscW(ch)
        End Function

        ''' <summary>
        ''' Reads characters into a portion of an array.
        ''' This method implements the general contract of the corresponding read method of the Reader class. As an additional convenience, it attempts to read as many characters as possible by repeatedly invoking the read method of the underlying stream. This iterated read continues until one of the following conditions becomes true:
        ''' 
        ''' The specified number of characters have been read,
        ''' The read method of the underlying stream returns -1, indicating end-of-file, or
        ''' The ready method of the underlying stream returns false, indicating that further input requests would block.
        ''' If the first read on the underlying stream returns -1 to indicate end-of-file then this method returns -1. Otherwise this method returns the number of characters actually read.
        ''' Subclasses of this class are encouraged, but not required, to attempt to read as many characters as possible in the same fashion.
        ''' 
        ''' Ordinarily this method takes characters from this stream's character buffer, filling it from the underlying stream as necessary. If, however, the buffer is empty, the mark is not valid, and the requested length is at least as large as the buffer, then this method will read characters directly from the underlying stream into the given array. Thus redundant BufferedReaders will not copy data unnecessarily.
        ''' </summary>
        ''' <param name="cbuf">Destination buffer</param>
        ''' <param name="off">Offset at which to start storing characters</param>
        ''' <param name="len">Maximum number of characters to read</param>
        ''' <returns>The number of characters read, or -1 if the end of the stream has been reached</returns>
        ''' <remarks></remarks>
        Public Overrides Function read(cbuf As Char(),
             off As Integer,
           len As Integer) As Integer
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Reads a line of text. A line is considered to be terminated by any one of a line feed ('\n'), a carriage return ('\r'), or a carriage 
        ''' return followed immediately by a linefeed.
        ''' </summary>
        ''' <returns>A String containing the contents of the line, not including any line-termination characters, or null if the end of the stream 
        ''' has been reached</returns>
        ''' <remarks></remarks>
        Public Function readLine() As String
            If p < _ChunkBuffer.Count Then
                Dim strLine As String = Me._ChunkBuffer(++p)
                idx = 0
                Return strLine
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Skips characters.
        ''' </summary>
        ''' <param name="n">The number of characters to skip</param>
        ''' <returns>The number of characters actually skipped</returns>
        ''' <remarks></remarks>
        Public Overrides Function [skip](n As Long) As Long
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Tells whether this stream is ready to be read. A buffered character stream is ready if the buffer is not empty, or if the underlying character stream is ready.
        ''' </summary>
        ''' <returns>
        ''' True if the next read() is guaranteed not to block for input, false otherwise. Note that returning false does not guarantee that the next read will block.
        ''' </returns>
        ''' <remarks></remarks>
        Public Overrides Function ready() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Tells whether this stream supports the mark() operation, which it does.
        ''' </summary>
        ''' <returns>true if and only if this stream supports the mark operation.</returns>
        ''' <remarks></remarks>
        Public Overrides Function markSupported() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Marks the present position in the stream. Subsequent calls to reset() will attempt to reposition the stream to this point.
        ''' </summary>
        ''' <param name="readAheadLimit">Limit on the number of characters that may be read while still preserving the mark. An attempt to reset the stream after reading characters up to this limit or beyond may fail. A limit value larger than the size of the input buffer will cause a new buffer to be allocated whose size is no smaller than limit. Therefore large values should be used with care.</param>
        ''' <remarks></remarks>
        Public Overrides Sub mark(readAheadLimit As Integer)

        End Sub

        ''' <summary>
        ''' Resets the stream to the most recent mark.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub reset()

        End Sub

        ''' <summary>
        ''' Closes the stream and releases any system resources associated with it. Once the stream has been closed, further read(), ready(), mark(), reset(), or skip() invocations will throw an IOException. Closing a previously closed stream has no effect.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub close()
            Call Me._reader.close()
        End Sub

        Public Overloads Overrides Function read(cbuf() As Char) As Integer
            Throw New NotImplementedException
        End Function

        Public Overloads Overrides Function read(target As CharBuffer) As Integer
            Throw New NotImplementedException
        End Function

        Protected Friend Overrides Function readToEnd() As String
            Return _CacheBulk
        End Function
#End Region

        Protected Friend Overrides Function readAllLines() As String()
            Return Me._ChunkBuffer
        End Function
    End Class
End Namespace