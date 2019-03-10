
''' <summary>
''' A Readable is a source of characters. Characters from a Readable are made available to callers of 
''' the read method via a CharBuffer.
''' </summary>
''' <remarks></remarks>
Public Interface Readable

    ''' <summary>
    ''' Attempts to read characters into the specified character buffer. The buffer is used as a repository of 
    ''' characters as-is: the only changes made are the results of a put operation. No flipping or rewinding 
    ''' of the buffer is performed.
    ''' </summary>
    ''' <param name="cb">the buffer to read characters into</param>
    ''' <returns>The number of char values added to the buffer, or -1 if this source of characters is at its end</returns>
    ''' <remarks></remarks>
    Function read(cb As CharBuffer) As Integer
End Interface

Namespace IO

    Public Interface Closeable

        ''' <summary>
        ''' Closes this stream and releases any system resources associated with it. If the stream is already 
        ''' closed then invoking this method has no effect.
        ''' </summary>
        ''' <remarks></remarks>
        Sub close()
    End Interface

    ''' <summary>
    ''' Abstract class for reading character streams. The only methods that a subclass must implement are 
    ''' read(char[], int, int) and close(). Most subclasses, however, will override some of the methods 
    ''' defined here in order to provide higher efficiency, additional functionality, or both.
    ''' </summary>
    ''' <remarks></remarks>
    Public MustInherit Class Reader
        Implements IDisposable, Closeable, Readable, AutoCloseable

#Region "Field Detail"

        ''' <summary>
        ''' The object used to synchronize operations on this stream. For efficiency, a character-stream object may use an object other than itself to protect critical sections. A subclass should therefore use the object in this field rather than this or a synchronized method.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend lock As Object
#End Region

#Region "Constructor Detail"

        ''' <summary>
        ''' Creates a new character-stream reader whose critical sections will synchronize on the reader itself.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Friend Sub New()
        End Sub

        ''' <summary>
        ''' Creates a new character-stream reader whose critical sections will synchronize on the given object.
        ''' </summary>
        ''' <param name="lock">The Object to synchronize on.</param>
        ''' <remarks></remarks>
        Protected Friend Sub New(lock As Object)
            Me.lock = lock
        End Sub
#End Region

#Region "Method Detail"

        Protected Friend MustOverride Function readToEnd() As String
        Protected Friend MustOverride Function readAllLines() As String()

        ''' <summary>
        ''' Attempts to read characters into the specified character buffer. The buffer is used as a repository of 
        ''' characters as-is: the only changes made are the results of a put operation. No flipping or rewinding 
        ''' of the buffer is performed.
        ''' </summary>
        ''' <param name="target">the buffer to read characters into</param>
        ''' <returns>The number of characters added to the buffer, or -1 if this source of characters is at its end</returns>
        ''' <remarks></remarks>
        Public MustOverride Function read(target As CharBuffer) As Integer Implements Readable.read

        ''' <summary>
        ''' Reads a single character. This method will block until a character is available, an I/O error occurs, 
        ''' or the end of the stream is reached.
        ''' Subclasses that intend to support efficient single-character input should override this method.
        ''' </summary>
        ''' <returns>
        ''' The character read, as an integer in the range 0 to 65535 (0x00-0xffff), or -1 if the end of the stream has been reached
        ''' </returns>
        ''' <remarks></remarks>
        Public MustOverride Function read() As Integer

        ''' <summary>
        ''' Reads characters into an array. This method will block until some input is available, an I/O error occurs, 
        ''' or the end of the stream is reached.
        ''' </summary>
        ''' <param name="cbuf">Destination buffer</param>
        ''' <returns>The number of characters read, or -1 if the end of the stream has been reached</returns>
        ''' <remarks></remarks>
        Public MustOverride Function read(cbuf As Char()) As Integer

        ''' <summary>
        ''' Reads characters into a portion of an array. This method will block until some input is available, 
        ''' an I/O error occurs, or the end of the stream is reached.
        ''' </summary>
        ''' <param name="cbuf">Destination buffer</param>
        ''' <param name="off">Offset at which to start storing characters</param>
        ''' <param name="len">Maximum number of characters to read</param>
        ''' <returns>The number of characters read, or -1 if the end of the stream has been reached</returns>
        ''' <remarks></remarks>
        Public MustOverride Function read(cbuf As Char(), off As Integer, len As Integer) As Integer

        ''' <summary>
        ''' Skips characters. This method will block until some characters are available, an I/O error occurs, 
        ''' or the end of the stream is reached.
        ''' </summary>
        ''' <param name="n">The number of characters to skip</param>
        ''' <returns>The number of characters actually skipped</returns>
        ''' <remarks></remarks>
        Public MustOverride Function [skip](n As Long) As Long

        ''' <summary>
        ''' Tells whether this stream is ready to be read.
        ''' </summary>
        ''' <returns>True if the next read() is guaranteed not to block for input, false otherwise. Note that 
        ''' returning false does not guarantee that the next read will block.</returns>
        ''' <remarks></remarks>
        Public MustOverride Function ready() As Boolean

        ''' <summary>
        ''' Tells whether this stream supports the mark() operation. The default implementation always returns false. 
        ''' Subclasses should override this method.
        ''' </summary>
        ''' <returns>true if and only if this stream supports the mark operation.</returns>
        ''' <remarks></remarks>
        Public MustOverride Function markSupported() As Boolean

        ''' <summary>
        ''' Marks the present position in the stream. Subsequent calls to reset() will attempt to reposition 
        ''' the stream to this point. Not all character-input streams support the mark() operation.
        ''' </summary>
        ''' <param name="readAheadLimit">Limit on the number of characters that may be read while still 
        ''' preserving the mark. After reading this many characters, attempting to reset the stream may fail.</param>
        ''' <remarks></remarks>
        Public MustOverride Sub mark(readAheadLimit As Integer)

        ''' <summary>
        ''' Resets the stream. If the stream has been marked, then attempt to reposition it at the mark. 
        ''' If the stream has not been marked, then attempt to reset it in some way appropriate to the 
        ''' particular stream, for example by repositioning it to its starting point. Not all character-input 
        ''' streams support the reset() operation, and some support reset() without supporting mark().
        ''' </summary>
        ''' <remarks></remarks>
        Public MustOverride Sub reset()

        ''' <summary>
        ''' Closes the stream and releases any system resources associated with it. Once the stream has 
        ''' been closed, further read(), ready(), mark(), reset(), or skip() invocations will throw an 
        ''' IOException. Closing a previously closed stream has no effect.
        ''' </summary>
        ''' <remarks></remarks>
        Public MustOverride Sub close() Implements Closeable.close, AutoCloseable.close
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  释放托管状态(托管对象)。
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
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region
    End Class
End Namespace