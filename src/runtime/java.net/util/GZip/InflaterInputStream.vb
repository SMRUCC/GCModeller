'Imports Oracle

'Namespace java.util.zip

'  ''' <summary>
'  ''' This class implements a stream filter for uncompressing data in the
'  '''  "deflate" compression format. It is also used as the basis for other
'  '''  decompression filters, such as GZIPInputStream.
'  ''' </summary>
'  Public Class InflaterInputStream
'    Inherits java.io.FilterInputStream
'    Implements java.io.Closeable, java.lang.AutoCloseable

'    ''' <summary>
'    ''' Decompressor for this stream.
'    ''' </summary>
'    Protected inf As java.util.zip.Inflater

'    ''' <summary>
'    ''' Input buffer for decompression.
'    ''' </summary>
'    Protected buf() As [byte]

'    ''' <summary>
'    ''' Length of input buffer.
'    ''' </summary>
'    Protected len As Integer

'    ''' <summary>
'    ''' Creates a new input stream with the specified decompressor and
'    '''  buffer size.
'    ''' </summary>
'    ''' <param name="in">the input stream</param>
'    ''' <param name="inf">the decompressor ("inflater")</param>
'    ''' <param name="size">the input buffer size</param>
'    Public Sub New([in] As java.io.InputStream, inf As java.util.zip.Inflater, size As Integer)

'    End Sub

'    ''' <summary>
'    ''' Creates a new input stream with the specified decompressor and a
'    '''  default buffer size.
'    ''' </summary>
'    ''' <param name="in">the input stream</param>
'    ''' <param name="inf">the decompressor ("inflater")</param>
'    Public Sub New([in] As java.io.InputStream, inf As java.util.zip.Inflater)

'    End Sub

'    ''' <summary>
'    ''' Creates a new input stream with a default decompressor and buffer size.
'    ''' </summary>
'    ''' <param name="in">the input stream</param>
'    Public Sub New([in] As java.io.InputStream)

'    End Sub

'    ''' <summary>
'    ''' Reads a byte of uncompressed data. This method will block until
'    '''  enough input is available for decompression.
'    ''' </summary>
'    ''' <returns>the byte read, or -1 if end of compressed input is reached</returns>
'    Public Overloads Overridable Function read() As Integer
'    End Function

'    ''' <summary>
'    ''' Reads uncompressed data into an array of bytes. If <code>len</code> is not
'    '''  zero, the method will block until some input can be decompressed; otherwise,
'    '''  no bytes are read and <code>0</code> is returned.
'    ''' </summary>
'    ''' <param name="b">the buffer into which the data is read</param>
'    ''' <param name="off">the start offset in the destination array <code>b</code></param>
'    ''' <param name="len">the maximum number of bytes read</param>
'    ''' <returns>the actual number of bytes read, or -1 if the end of the         compressed input is reached or a preset dictionary is needed</returns>
'    Public Overloads Overridable Function read(b() As [byte], off As Integer, len As Integer) As Integer
'    End Function

'    ''' <summary>
'    ''' Returns 0 after EOF has been reached, otherwise always return 1.
'    '''  <p>
'    '''  Programs should not count on this method to return the actual number
'    '''  of bytes that could be read without blocking.
'    ''' </summary>
'    ''' <returns>1 before EOF and 0 after EOF.</returns>
'    Public Overridable Function available() As Integer
'    End Function

'        ''' <summary>
'        ''' Skips specified number of bytes of uncompressed data.
'        ''' </summary>
'        ''' <param name="n">the number of bytes to skip</param>
'        ''' <returns>the actual number of bytes skipped.</returns>
'        Public Overridable Function skip(n As Long) As Long
'        End Function

'        ''' <summary>
'        ''' Closes this input stream and releases any system resources associated
'        '''  with the stream.
'        ''' </summary>
'        Public Overridable Sub close()
'    End Sub

'    ''' <summary>
'    ''' Fills input buffer with more data to decompress.
'    ''' </summary>
'    Protected Overridable Sub fill()
'    End Sub

'    ''' <summary>
'    ''' Tests if this input stream supports the <code>mark</code> and
'    '''  <code>reset</code> methods. The <code>markSupported</code>
'    '''  method of <code>InflaterInputStream</code> returns
'    '''  <code>false</code>.
'    ''' </summary>
'    ''' <returns>a <code>boolean</code> indicating if this stream type supports          the <code>mark</code> and <code>reset</code> methods.</returns>
'    Public Overridable Function markSupported() As [boolean]
'    End Function

'    ''' <summary>
'    ''' Marks the current position in this input stream.
'    ''' 
'    '''  <p> The <code>mark</code> method of <code>InflaterInputStream</code>
'    '''  does nothing.
'    ''' </summary>
'    ''' <param name="readlimit">the maximum limit of bytes that can be read before the mark position becomes invalid.</param>
'    Public Overridable Sub mark(readlimit As Integer)
'    End Sub

'    ''' <summary>
'    ''' Repositions this stream to the position at the time the
'    '''  <code>mark</code> method was last called on this input stream.
'    ''' 
'    '''  <p> The method <code>reset</code> for class
'    '''  <code>InflaterInputStream</code> does nothing except throw an
'    '''  <code>IOException</code>.
'    ''' </summary>
'    Public Overridable Sub reset()
'    End Sub
'  End Class
'End Namespace
