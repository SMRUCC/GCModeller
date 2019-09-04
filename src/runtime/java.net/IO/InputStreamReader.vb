Imports Microsoft.VisualBasic.CommandLine
Imports Microsoft.VisualBasic.Language
Imports Oracle.Java.util.zip

Namespace IO

    Public Class StandardOutput : Inherits Java.IO.Reader
        Implements IDisposable
        Implements Closeable, AutoCloseable

        Dim IORedirect As IORedirect
        Dim chunkBuffer As New List(Of String)
        Dim idx As Integer, _p As i32
        Dim _currentLine As Char()
        Dim _avaliable As Boolean
        Dim _exitCode As Integer

        Sub New(StandardOutput As Microsoft.VisualBasic.CommandLine.IORedirect)
            IORedirect = StandardOutput
            AddHandler IORedirect.PrintOutput, AddressOf chunkBuffer.Add
            AddHandler IORedirect.ProcessExit, AddressOf process_exit
            _avaliable = True
        End Sub

        Public ReadOnly Property ExitValue As Integer
            Get
                Return _exitCode
            End Get
        End Property

        Private Sub process_exit(exitCode As Integer, exitTime As String)
            _avaliable = False
            _exitCode = exitCode
        End Sub

        Public Overrides Sub close()
            IORedirect = Nothing
            _avaliable = False
        End Sub

        Public ReadOnly Property Avaliable As Boolean
            Get
                Return _avaliable
            End Get
        End Property

        Public Overrides Sub mark(readlimit As Integer)

        End Sub

        Public Overrides Function markSupported() As Boolean
            Throw New NotImplementedException
        End Function

        Public Sub WaitForExit()
            Do While _avaliable
                Threading.Thread.Sleep(10)
            Loop
        End Sub

        Public Overloads Overrides Function read() As Integer
            If idx = _currentLine.Count Then
                _currentLine = chunkBuffer(++_p)
                idx = 0
            End If

            Dim ch = _currentLine(idx)
            idx += 1
            Return AscW(ch)
        End Function

        Public Overrides Sub reset()

        End Sub

        Public Overrides Function skip(n As Long) As Long
            Throw New NotImplementedException
        End Function

        Public Shared Function CreateObject(IORedirect As Microsoft.VisualBasic.CommandLine.IORedirect) As StandardOutput
            Return New StandardOutput(IORedirect)
        End Function

        Public Shared Function CreateObject(CommandLine As String) As StandardOutput
            Dim IORedirect As Microsoft.VisualBasic.CommandLine.IORedirect = CommandLine
            Call IORedirect.Start(waitForExit:=False)
            Return New StandardOutput(IORedirect)
        End Function

        Public Overloads Overrides Function read(cbuf() As Char) As Integer
            Throw New NotImplementedException
        End Function

        Public Overloads Overrides Function read(cbuf() As Char, off As Integer, len As Integer) As Integer
            Throw New NotImplementedException
        End Function

        Public Overloads Overrides Function read(target As CharBuffer) As Integer
            Throw New NotImplementedException
        End Function

        Protected Friend Overrides Function readToEnd() As String
            Throw New NotImplementedException
        End Function

        Public Overrides Function ready() As Boolean
            Throw New NotImplementedException
        End Function

        Public Function readLine() As String
            Dim p% = ++_p

            If p >= chunkBuffer.Count - 1 Then
                Return ""
            Else
                Me._currentLine = Me.chunkBuffer(p)
                idx = 0
                Return Me._currentLine
            End If
        End Function

        Protected Friend Overrides Function readAllLines() As String()
            Throw New NotImplementedException
        End Function
    End Class

    Public Class InputStreamReader : Inherits Java.IO.Reader
        Implements Global.System.IDisposable
        Implements Closeable, AutoCloseable, Readable

        Protected Friend _inputStream As InputStream

#Region "Constructor  Detail"
        Sub New(stream As GZIPInputStream)

        End Sub

        Protected Friend Sub New()
        End Sub

        ''' <summary>
        ''' Creates an InputStreamReader that uses the default charset.
        ''' </summary>
        ''' <param name="in">An InputStream</param>
        ''' <remarks></remarks>
        Public Sub New([in] As InputStream)
            _inputStream = [in]
        End Sub

        ''' <summary>
        ''' Creates an InputStreamReader that uses the named charset.
        ''' </summary>
        ''' <param name="in">An InputStream</param>
        ''' <param name="charsetName">The name of a supported charset</param>
        ''' <remarks></remarks>
        Public Sub New([in] As InputStream,
                         charsetName As String)
            Dim encoding As Global.System.Text.Encoding = Global.System.Text.Encoding.GetEncoding(charsetName)

        End Sub

        ''' <summary>
        ''' Creates an InputStreamReader that uses the given charset.
        ''' </summary>
        ''' <param name="in">An InputStream</param>
        ''' <param name="cs">A charset</param>
        ''' <remarks></remarks>
        Public Sub New([in] As InputStream,
                         cs As Charset)

        End Sub

        ''' <summary>
        ''' Creates an InputStreamReader that uses the given charset decoder.
        ''' </summary>
        ''' <param name="in">An InputStream</param>
        ''' <param name="dec">A charset decoder</param>
        ''' <remarks></remarks>
        Public Sub New([in] As InputStream,
                         dec As CharsetDecoder)

        End Sub
#End Region

#Region "Method Detail"
        ''' <summary>
        ''' Returns the name of the character encoding being used by this stream.
        ''' If the encoding has an historical name then that name is returned; otherwise the encoding's canonical name is returned.
        ''' 
        ''' If this instance was created with the InputStreamReader(InputStream, String) constructor then the returned name, being unique for the encoding, may differ from the name passed to the constructor. This method will return null if the stream has been closed.
        ''' </summary>
        ''' <returns>The historical name of this encoding, or null if the stream has been closed</returns>
        ''' <remarks></remarks>
        Public Function getEncoding() As String
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Reads a single character.
        ''' </summary>
        ''' <returns>The character read, or -1 if the end of the stream has been reached</returns>
        ''' <remarks></remarks>
        Public Overrides Function read() As Integer
            Return _inputStream.read
        End Function

        ''' <summary>
        ''' Reads characters into a portion of an array.
        ''' </summary>
        ''' <param name="cbuf">Destination buffer</param>
        ''' <param name="offset">Offset at which to start storing characters</param>
        ''' <param name="length">Maximum number of characters to read</param>
        ''' <returns>The number of characters read, or -1 if the end of the stream has been reached</returns>
        ''' <remarks></remarks>
        Public Overrides Function read(cbuf As Char(),
                 offset As Integer,
                 length As Integer) As Integer
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Tells whether this stream is ready to be read. An InputStreamReader is ready if its input buffer is not empty, or if bytes are available to be read from the underlying byte stream.
        ''' </summary>
        ''' <returns>True if the next read() is guaranteed not to block for input, false otherwise. Note that returning false does not guarantee that the next read will block.</returns>
        ''' <remarks></remarks>
        Public Overrides Function ready() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Closes the stream and releases any system resources associated with it. Once the stream has been closed, further read(), ready(), mark(), reset(), or skip() invocations will throw an IOException. Closing a previously closed stream has no effect.
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub close()
            If Not _inputStream Is Nothing Then Call _inputStream.close()
        End Sub

        Public Overrides Function ToString() As String
            Return _inputStream.ToString
        End Function

        Public Overrides Sub mark(readAheadLimit As Integer)

        End Sub

        Public Overrides Function markSupported() As Boolean
            Throw New NotImplementedException
        End Function

        Public Overloads Overrides Function read(cbuf() As Char) As Integer
            Throw New NotImplementedException
        End Function

        Public Overloads Overrides Function read(target As CharBuffer) As Integer
            Throw New NotImplementedException
        End Function

        Public Overrides Sub reset()

        End Sub

        Public Overrides Function skip(n As Long) As Long
            Throw New NotImplementedException
        End Function

        Protected Friend Overrides Function readToEnd() As String
            Throw New NotImplementedException
        End Function
#End Region

        Protected Friend Overrides Function readAllLines() As String()
            Throw New NotImplementedException
        End Function
    End Class

    ''' <summary>
    ''' Convenience class for reading character files. The constructors of this class assume that the default character encoding 
    ''' and the default byte-buffer size are appropriate. To specify these values yourself, construct 
    ''' an <see cref="Java.IO.InputStreamReader"></see> on a <see cref="Java.IO.FileInputStream"></see>.
    ''' FileReader is meant for reading streams of characters. For reading streams of raw bytes, consider using a 
    ''' <see cref="Java.IO.FileInputStream"></see>.
    ''' </summary>
    ''' <remarks></remarks>
    Public Class FileReader : Inherits InputStreamReader
        Implements Closeable, Readable
        Implements IDisposable

        Protected Friend _fileName As String

        Sub New(fileName As String)
            ' TODO: Complete member initialization 
            Me._fileName = fileName
        End Sub

        Public Overrides Function ToString() As String
            Return _fileName
        End Function

        Protected Friend Overrides Function readToEnd() As String
            Return Global.System.IO.File.ReadAllText(_fileName)
        End Function

        Protected Friend Overrides Function readAllLines() As String()
            Dim strLines As String() = (From strLine As String In Strings.Split(Me.readToEnd, vbLf) Select strLine.TrimNewLine("")).ToArray
            Return strLines
        End Function
    End Class

    Public Class FileInputStream : Inherits Java.IO.InputStream
        Implements IDisposable
        Implements Closeable, AutoCloseable

        Protected Friend _fileName As String

        Sub New(fileName As String)
            Call MyBase.New(fileName)
        End Sub

        Sub New(File As File)
            Call MyBase.New(File.absolutePath)
        End Sub

        Public Overrides Function ToString() As String
            Return _fileName
        End Function

        Public Overrides Sub close() Implements AutoCloseable.close

        End Sub

        Public Overrides Function available() As Integer
            Throw New NotImplementedException
        End Function

        Public Overrides Sub mark(readlimit As Integer)

        End Sub

        Public Overrides Function markSupported() As Boolean
            Throw New NotImplementedException
        End Function

        Public Overloads Overrides Function read() As Integer
            Throw New NotImplementedException
        End Function

        Public Overloads Overrides Function read(b() As Byte) As Integer
            Throw New NotImplementedException
        End Function

        Public Overloads Function read(b() As SByte) As Integer
            Throw New NotImplementedException
        End Function

        Public Overloads Overrides Function read(b() As Byte, off As Integer, len As Integer) As Integer
            Throw New NotImplementedException
        End Function

        Public Overrides Sub reset()

        End Sub

        Public Overrides Function skip(n As Long) As Long
            Throw New NotImplementedException
        End Function

        Public Shared Widening Operator CType(filePath As String) As FileInputStream
            Return New FileInputStream(filePath)
        End Operator

        Public Overrides ReadOnly Property Length As Integer
            Get
                Throw New NotImplementedException
            End Get
        End Property
    End Class

    ''' <summary>
    ''' A resource that must be closed when it is no longer needed.
    ''' </summary>
    ''' <remarks></remarks>
    Public Interface AutoCloseable

        ''' <summary>
        ''' Closes this resource, relinquishing any underlying resources. This method is invoked automatically on objects managed by the try-with-resources statement.
        ''' While this interface method is declared to throw Exception, implementers are strongly encouraged to declare concrete implementations of the close method to throw more specific exceptions, or to throw no exception at all if the close operation cannot fail.
        ''' 
        ''' Implementers of this interface are also strongly advised to not have the close method throw InterruptedException. This exception interacts with a thread's interrupted status, and runtime misbehavior is likely to occur if an InterruptedException is suppressed. More generally, if it would cause problems for an exception to be suppressed, the AutoCloseable.close method should not throw it.
        ''' 
        ''' Note that unlike the close method of Closeable, this close method is not required to be idempotent. In other words, calling this close method more than once may have some visible side effect, unlike Closeable.close which is required to have no effect if called more than once. However, implementers of this interface are strongly encouraged to make their close methods idempotent.
        ''' </summary>
        ''' <remarks></remarks>
        Sub close()
    End Interface

    Public MustInherit Class InputStream : Inherits Global.System.IO.FileStream
        Implements IDisposable
        Implements Closeable

        Sub New(filePath As String)
            Call MyBase.New(filePath, Global.System.IO.FileMode.OpenOrCreate) '只从文件之中读取数据
        End Sub

#Region "Method Detail"

        ''' <summary>
        ''' Reads the next byte of data from the input stream. The value byte is returned as an int in the range 0 to 255. If no byte is available because the end of the stream has been reached, the value -1 is returned. This method blocks until input data is available, the end of the stream is detected, or an exception is thrown.
        ''' A subclass must provide an implementation of this method.
        ''' </summary>
        ''' <returns>the next byte of data, or -1 if the end of the stream is reached.</returns>
        ''' <remarks></remarks>
        Public MustOverride Overloads Function read() As Integer

        ''' <summary>
        ''' Reads some number of bytes from the input stream and stores them into the buffer array b. The number of bytes actually read is returned as an integer. This method blocks until input data is available, end of file is detected, or an exception is thrown.
        ''' If the length of b is zero, then no bytes are read and 0 is returned; otherwise, there is an attempt to read at least one byte. If no byte is available because the stream is at the end of the file, the value -1 is returned; otherwise, at least one byte is read and stored into b.
        ''' 
        ''' The first byte read is stored into element b[0], the next one into b[1], and so on. The number of bytes read is, at most, equal to the length of b. Let k be the number of bytes actually read; these bytes will be stored in elements b[0] through b[k-1], leaving elements b[k] through b[b.length-1] unaffected.
        ''' 
        ''' The read(b) method for class InputStream has the same effect as:
        ''' 
        ''' read(b, 0, b.length) 
        ''' </summary>
        ''' <param name="b">the buffer into which the data is read.</param>
        ''' <returns>the total number of bytes read into the buffer, or -1 is there is no more data because the end of the stream has been reached.</returns>
        ''' <remarks></remarks>
        Public MustOverride Overloads Function read(b As Byte()) As Integer

        ''' <summary>
        ''' Reads up to len bytes of data from the input stream into an array of bytes. An attempt is made to read as many as len bytes, but a smaller number may be read. The number of bytes actually read is returned as an integer.
        ''' This method blocks until input data is available, end of file is detected, or an exception is thrown.
        ''' 
        ''' If len is zero, then no bytes are read and 0 is returned; otherwise, there is an attempt to read at least one byte. If no byte is available because the stream is at end of file, the value -1 is returned; otherwise, at least one byte is read and stored into b.
        ''' 
        ''' The first byte read is stored into element b[off], the next one into b[off+1], and so on. The number of bytes read is, at most, equal to len. Let k be the number of bytes actually read; these bytes will be stored in elements b[off] through b[off+k-1], leaving elements b[off+k] through b[off+len-1] unaffected.
        ''' 
        ''' In every case, elements b[0] through b[off] and elements b[off+len] through b[b.length-1] are unaffected.
        ''' 
        ''' The read(b, off, len) method for class InputStream simply calls the method read() repeatedly. If the first such call results in an IOException, that exception is returned from the call to the read(b, off, len) method. If any subsequent call to read() results in a IOException, the exception is caught and treated as if it were end of file; the bytes read up to that point are stored into b and the number of bytes read before the exception occurred is returned. The default implementation of this method blocks until the requested amount of input data len has been read, end of file is detected, or an exception is thrown. Subclasses are encouraged to provide a more efficient implementation of this method.
        ''' </summary>
        ''' <param name="b">the buffer into which the data is read.</param>
        ''' <param name="off">the start offset in array b at which the data is written.</param>
        ''' <param name="len">the maximum number of bytes to read.</param>
        ''' <returns>the total number of bytes read into the buffer, or -1 if there is no more data because the end of the stream has been reached.</returns>
        ''' <remarks></remarks>
        Public MustOverride Overloads Function read(b As Byte(),
                         off As Integer,
                         len As Integer) As Integer

        ''' <summary>
        ''' Skips over and discards n bytes of data from this input stream. The skip method may, for a variety of reasons, end up skipping over some smaller number of bytes, possibly 0. This may result from any of a number of conditions; reaching end of file before n bytes have been skipped is only one possibility. The actual number of bytes skipped is returned. If n is negative, no bytes are skipped.
        ''' The skip method of this class creates a byte array and then repeatedly reads into it until n bytes have been read or the end of the stream has been reached. Subclasses are encouraged to provide a more efficient implementation of this method. For instance, the implementation may depend on the ability to seek.
        ''' </summary>
        ''' <param name="n">the number of bytes to be skipped.</param>
        ''' <returns>the actual number of bytes skipped.</returns>
        ''' <remarks></remarks>
        Public MustOverride Function skip(n As Long) As Long

        ''' <summary>
        ''' Returns an estimate of the number of bytes that can be read (or skipped over) from this input stream without blocking by the next invocation of a method for this input stream. The next invocation might be the same thread or another thread. A single read or skip of this many bytes will not block, but may read or skip fewer bytes.
        ''' Note that while some implementations of InputStream will return the total number of bytes in the stream, many will not. It is never correct to use the return value of this method to allocate a buffer intended to hold all data in this stream.
        ''' 
        ''' A subclass' implementation of this method may choose to throw an IOException if this input stream has been closed by invoking the close() method.
        ''' 
        ''' The available method for class InputStream always returns 0.
        ''' 
        ''' This method should be overridden by subclasses.
        ''' </summary>
        ''' <returns>an estimate of the number of bytes that can be read (or skipped over) from this input stream without blocking or 0 when it reaches the end of the input stream.</returns>
        ''' <remarks></remarks>
        Public MustOverride Function available() As Integer

        ''' <summary>
        ''' Closes this input stream and releases any system resources associated with the stream.
        ''' The close method of InputStream does nothing.
        ''' </summary>
        ''' <remarks></remarks>
        Public MustOverride Overloads Sub close() Implements Closeable.close

        ''' <summary>
        ''' Marks the current position in this input stream. A subsequent call to the reset method repositions this stream at the last marked position so that subsequent reads re-read the same bytes.
        ''' The readlimit arguments tells this input stream to allow that many bytes to be read before the mark position gets invalidated.
        ''' 
        ''' The general contract of mark is that, if the method markSupported returns true, the stream somehow remembers all the bytes read after the call to mark and stands ready to supply those same bytes again if and whenever the method reset is called. However, the stream is not required to remember any data at all if more than readlimit bytes are read from the stream before reset is called.
        ''' 
        ''' Marking a closed stream should not have any effect on the stream.
        ''' 
        ''' The mark method of InputStream does nothing.
        ''' </summary>
        ''' <param name="readlimit">the maximum limit of bytes that can be read before the mark position becomes invalid.</param>
        ''' <remarks></remarks>
        Public MustOverride Sub mark(readlimit As Integer)

        ''' <summary>
        ''' Repositions this stream to the position at the time the mark method was last called on this input stream.
        ''' The general contract of reset is:
        ''' 
        ''' If the method markSupported returns true, then:
        ''' If the method mark has not been called since the stream was created, or the number of bytes read from the stream since mark was last called is larger than the argument to mark at that last call, then an IOException might be thrown.
        ''' If such an IOException is not thrown, then the stream is reset to a state such that all the bytes read since the most recent call to mark (or since the start of the file, if mark has not been called) will be resupplied to subsequent callers of the read method, followed by any bytes that otherwise would have been the next input data as of the time of the call to reset.
        ''' If the method markSupported returns false, then:
        ''' The call to reset may throw an IOException.
        ''' If an IOException is not thrown, then the stream is reset to a fixed state that depends on the particular type of the input stream and how it was created. The bytes that will be supplied to subsequent callers of the read method depend on the particular type of the input stream.
        ''' The method reset for class InputStream does nothing except throw an IOException.
        ''' </summary>
        ''' <remarks></remarks>
        Public MustOverride Sub reset()

        ''' <summary>
        ''' Tests if this input stream supports the mark and reset methods. Whether or not mark and reset are supported is an invariant property of a particular input stream instance. The markSupported method of InputStream returns false.
        ''' </summary>
        ''' <returns>true if this stream instance supports the mark and reset methods; false otherwise.</returns>
        ''' <remarks></remarks>
        Public MustOverride Function markSupported() As Boolean

        Public MustOverride Overloads ReadOnly Property Length As Integer
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Overloads Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
                    Call close()
                End If

                ' TODO:  释放非托管资源(非托管对象)并重写下面的 Finalize()。
                ' TODO:  将大型字段设置为 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  仅当上面的 Dispose( disposing As Boolean)具有释放非托管资源的代码时重写 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 不要更改此代码。    请将清理代码放入上面的 Dispose( disposing As Boolean)中。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' Visual Basic 添加此代码是为了正确实现可处置模式。
        Public Overloads Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace