Imports Oracle.Java.IO
Imports System.IO
Imports System.IO.Compression

Namespace util.zip

    Public Class GZIPInputStream : Implements IO.AutoCloseable, IO.Closeable

        Dim _GZipStream As GZipStream

        Sub New(stream As InputStream)
            _GZipStream = New GZipStream(stream, CompressionMode.Decompress)
        End Sub

        Sub New(fileIn As FileInputStream)
            _GZipStream = New GZipStream(fileIn, CompressionMode.Decompress)
        End Sub

        ''' <summary>
        ''' Closes this input stream and releases any system resources associated with the stream.
        ''' </summary>
        Public Sub close() Implements AutoCloseable.close, Closeable.close
            Call Me._GZipStream.Close()
        End Sub

        ''' <summary>
        ''' Reads uncompressed data into an array Of bytes. If len Is Not zero, the method will block until some input can be decompressed; otherwise, no bytes are read And 0 Is returned.
        ''' </summary>
        ''' <param name="buf">the buffer into which the data Is read</param>
        ''' <param name="off">the start offset in the destination array b</param>
        ''' <param name="len">the maximum number of bytes read</param>
        ''' <returns>the actual number Of bytes read, Or -1 If the End Of the compressed input stream Is reached</returns>
        Public Function read(buf As Byte(), off As Integer, len As Integer) As Integer

        End Function


        Public Function read(buffer1() As SByte) As Integer
            Throw New NotImplementedException()
        End Function
    End Class
End Namespace