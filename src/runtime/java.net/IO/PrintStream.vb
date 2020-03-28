Imports System.Text
Imports Microsoft.VisualBasic.Language.C

Namespace IO

    Public Class PrintStream
        Implements Global.System.IDisposable
        Implements Closeable, Flushable, Oracle.Java.Lang.Appendable, AutoCloseable

        Dim _FilePath As String
        Dim _InnerBuilder As StringBuilder = New StringBuilder(1024)

        Public Overrides Function ToString() As String
            Return _FilePath
        End Function

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

#Region "Constructor Detail"

        ''' <summary>
        ''' Creates a new print stream. This stream will not flush automatically.
        ''' </summary>
        ''' <param name="out">The output stream to which values and objects will be printed</param>
        ''' <remarks></remarks>
        Public Sub New([out] As OutputStream)

        End Sub

        ''' <summary>
        ''' Creates a new print stream.
        ''' </summary>
        ''' <param name="out">The output stream to which values and objects will be printed</param>
        ''' <param name="autoFlush">A boolean; if true, the output buffer will be flushed whenever a byte array is written, one of the println methods is invoked, or a newline character or byte ('\n') is written</param>
        ''' <remarks></remarks>
        Public Sub New([out] As OutputStream,
                   autoFlush As Boolean)

        End Sub

        ''' <summary>
        ''' Creates a new print stream.
        ''' </summary>
        ''' <param name="out">The output stream to which values and objects will be printed</param>
        ''' <param name="autoFlush">A boolean; if true, the output buffer will be flushed whenever a byte array is written, one of the println methods is invoked, or a newline character or byte ('\n') is written</param>
        ''' <param name="encoding">The name of a supported character encoding</param>
        ''' <remarks></remarks>
        Public Sub New([out] As OutputStream,
                    autoFlush As Boolean,
                   encoding As String)

        End Sub

        ''' <summary>
        ''' Creates a new print stream, without automatic line flushing, with the specified file name. This convenience constructor creates the necessary intermediate OutputStreamWriter, which will encode characters using the default charset for this instance of the Java virtual machine.
        ''' </summary>
        ''' <param name="fileName">The name of the file to use as the destination of this print stream. If the file exists, then it will be truncated to zero size; otherwise, a new file will be created. The output will be written to the file and is buffered.</param>
        ''' <remarks></remarks>
        Public Sub New(fileName As String)
            Me._FilePath = fileName
        End Sub

        ''' <summary>
        ''' Creates a new print stream, without automatic line flushing, with the specified file name and charset. This convenience constructor creates the necessary intermediate OutputStreamWriter, which will encode characters using the provided charset.
        ''' </summary>
        ''' <param name="fileName">The name of the file to use as the destination of this print stream. If the file exists, then it will be truncated to zero size; otherwise, a new file will be created. The output will be written to the file and is buffered.</param>
        ''' <param name="csn">The name of a supported charset</param>
        ''' <remarks></remarks>
        Public Sub New(fileName As String,
                    csn As String)

        End Sub

        ''' <summary>
        ''' Creates a new print stream, without automatic line flushing, with the specified file. This convenience constructor creates the necessary intermediate OutputStreamWriter, which will encode characters using the default charset for this instance of the Java virtual machine.
        ''' </summary>
        ''' <param name="File">The file to use as the destination of this print stream. If the file exists, then it will be truncated to zero size; otherwise, a new file will be created. The output will be written to the file and is buffered.</param>
        ''' <remarks></remarks>
        Public Sub New(File As File)

        End Sub

        ''' <summary>
        ''' Creates a new print stream, without automatic line flushing, with the specified file and charset. This convenience constructor creates the necessary intermediate OutputStreamWriter, which will encode characters using the provided charset.
        ''' </summary>
        ''' <param name="File">The file to use as the destination of this print stream. If the file exists, then it will be truncated to zero size; otherwise, a new file will be created. The output will be written to the file and is buffered.</param>
        ''' <param name="csn">The name of a supported charset</param>
        ''' <remarks></remarks>
        Public Sub New(File As File,
                   csn As String)

        End Sub
#End Region

#Region "Method Detail"

        ''' <summary>
        ''' Flushes the stream. This is done by writing any buffered output bytes to the underlying output stream and then flushing that stream.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub flush() Implements Flushable.flush

        End Sub

        ''' <summary>
        ''' Closes the stream. This is done by flushing the stream and then closing the underlying output stream.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub close() Implements AutoCloseable.close, Closeable.close
            Call _InnerBuilder.ToString.SaveTo(_FilePath)
        End Sub

        ''' <summary>
        ''' Flushes the stream and checks its error state. The internal error state is set to true when the underlying output stream throws an IOException other than InterruptedIOException, and when the setError method is invoked. If an operation on the underlying output stream throws an InterruptedIOException, then the PrintStream converts the exception back into an interrupt by doing:
        '''   Thread.currentThread().interrupt();
        ''' 
        ''' or the equivalent.
        ''' </summary>
        ''' <returns>true if and only if this stream has encountered an IOException other than InterruptedIOException, or the setError method has been invoked</returns>
        ''' <remarks></remarks>
        Public Function checkError() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Sets the error state of the stream to true.
        ''' This method will cause subsequent invocations of checkError() to return true until clearError() is invoked.
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub setError()

        End Sub

        ''' <summary>
        ''' Clears the internal error state of this stream.
        ''' This method will cause subsequent invocations of checkError() to return false until another write operation fails and invokes setError().
        ''' </summary>
        ''' <remarks></remarks>
        Protected Sub clearError()

        End Sub

        ''' <summary>
        ''' Writes the specified byte to this stream. If the byte is a newline and automatic flushing is enabled then the flush method will be invoked.
        ''' Note that the byte is written as given; to write a character that will be translated according to the platform's default character encoding, use the print(char) or println(char) methods.
        ''' </summary>
        ''' <param name="b">The byte to be written</param>
        ''' <remarks></remarks>
        Public Sub write(b As Integer)

        End Sub

        ''' <summary>
        ''' Writes len bytes from the specified byte array starting at offset off to this stream. If automatic flushing is enabled then the flush method will be invoked.
        ''' Note that the bytes will be written as given; to write characters that will be translated according to the platform's default character encoding, use the print(char) or println(char) methods.
        ''' </summary>
        ''' <param name="buf">A byte array</param>
        ''' <param name="off">Offset from which to start taking bytes</param>
        ''' <param name="len">Number of bytes to write</param>
        ''' <remarks></remarks>
        Public Sub write(buf As Byte(),
                  off As Integer,
                  len As Integer)

        End Sub

        ''' <summary>
        ''' Prints a boolean value. The string produced by String.valueOf(boolean) is translated into bytes according to the platform's default character encoding, and these bytes are written in exactly the manner of the write(int) method.
        ''' </summary>
        ''' <param name="b">The boolean to be printed</param>
        ''' <remarks></remarks>
        Public Sub print(b As Boolean)

        End Sub

        ''' <summary>
        ''' Prints a character. The character is translated into one or more bytes according to the platform's default character encoding, and these bytes are written in exactly the manner of the write(int) method.
        ''' </summary>
        ''' <param name="c">The char to be printed</param>
        ''' <remarks></remarks>
        Public Sub print(c As Char)

        End Sub

        ''' <summary>
        ''' Prints an integer. The string produced by String.valueOf(int) is translated into bytes according to the platform's default character encoding, and these bytes are written in exactly the manner of the write(int) method.
        ''' </summary>
        ''' <param name="i">The int to be printed</param>
        ''' <remarks></remarks>
        Public Sub print(i As Integer)

        End Sub

        ''' <summary>
        ''' Prints a long integer. The string produced by String.valueOf(long) is translated into bytes according to the platform's default character encoding, and these bytes are written in exactly the manner of the write(int) method.
        ''' </summary>
        ''' <param name="l">The long to be printed</param>
        ''' <remarks></remarks>
        Public Sub print(l As Long)

        End Sub

        ''' <summary>
        ''' Prints a floating-point number. The string produced by String.valueOf(float) is translated into bytes according to the platform's default character encoding, and these bytes are written in exactly the manner of the write(int) method.
        ''' </summary>
        ''' <param name="f">The float to be printed</param>
        ''' <remarks></remarks>
        Public Sub print(f As Float)

        End Sub

        ''' <summary>
        ''' Prints a double-precision floating-point number. The string produced by String.valueOf(double) is translated into bytes according to the platform's default character encoding, and these bytes are written in exactly the manner of the write(int) method.
        ''' </summary>
        ''' <param name="d">The double to be printed</param>
        ''' <remarks></remarks>
        Public Sub print(d As Double)

        End Sub

        ''' <summary>
        ''' Prints an array of characters. The characters are converted into bytes according to the platform's default character encoding, and these bytes are written in exactly the manner of the write(int) method.
        ''' </summary>
        ''' <param name="s">The array of chars to be printed</param>
        ''' <remarks></remarks>
        Public Sub print(s As Char())

        End Sub

        ''' <summary>
        ''' Prints a string. If the argument is null then the string "null" is printed. Otherwise, the string's characters are converted into bytes according to the platform's default character encoding, and these bytes are written in exactly the manner of the write(int) method.
        ''' </summary>
        ''' <param name="s">The String to be printed</param>
        ''' <remarks></remarks>
        Public Sub print(s As String)

        End Sub

        ''' <summary>
        ''' Prints an object. The string produced by the String.valueOf(Object) method is translated into bytes according to the platform's default character encoding, and these bytes are written in exactly the manner of the write(int) method.
        ''' </summary>
        ''' <param name="obj">The Object to be printed</param>
        ''' <remarks></remarks>
        Public Sub print(obj As Object)

        End Sub

        ''' <summary>
        ''' Terminates the current line by writing the line separator string. The line separator string is defined by the system property line.separator, and is not necessarily a single newline character ('\n').
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub println()

        End Sub

        ''' <summary>
        ''' Prints a boolean and then terminate the line. This method behaves as though it invokes print(boolean) and then println().
        ''' </summary>
        ''' <param name="x">The boolean to be printed</param>
        ''' <remarks></remarks>
        Public Sub println(x As Boolean)

        End Sub

        ''' <summary>
        ''' Prints a character and then terminate the line. This method behaves as though it invokes print(char) and then println().
        ''' </summary>
        ''' <param name="x">The char to be printed.</param>
        ''' <remarks></remarks>
        Public Sub println(x As Char)

        End Sub

        ''' <summary>
        ''' Prints an integer and then terminate the line. This method behaves as though it invokes print(int) and then println().
        ''' </summary>
        ''' <param name="x">The int to be printed.</param>
        ''' <remarks></remarks>
        Public Sub println(x As Integer)

        End Sub

        ''' <summary>
        ''' Prints a long and then terminate the line. This method behaves as though it invokes print(long) and then println().
        ''' </summary>
        ''' <param name="x">a The long to be printed.</param>
        ''' <remarks></remarks>
        Public Sub println(x As Long)

        End Sub

        ''' <summary>
        ''' Prints a float and then terminate the line. This method behaves as though it invokes print(float) and then println().
        ''' </summary>
        ''' <param name="x">The float to be printed.</param>
        ''' <remarks></remarks>
        Public Sub println(x As Float)

        End Sub

        ''' <summary>
        ''' Prints a double and then terminate the line. This method behaves as though it invokes print(double) and then println().
        ''' </summary>
        ''' <param name="x">The double to be printed.</param>
        ''' <remarks></remarks>
        Public Sub println(x As Double)

        End Sub

        ''' <summary>
        ''' Prints an array of characters and then terminate the line. This method behaves as though it invokes print(char[]) and then println().
        ''' </summary>
        ''' <param name="x">an array of chars to print.</param>
        ''' <remarks></remarks>
        Public Sub println(x As Char())

        End Sub

        ''' <summary>
        ''' Prints a String and then terminate the line. This method behaves as though it invokes print(String) and then println().
        ''' </summary>
        ''' <param name="x">The String to be printed.</param>
        ''' <remarks></remarks>
        Public Sub println(x As String)

        End Sub

        ''' <summary>
        ''' Prints an Object and then terminate the line. This method calls at first String.valueOf(x) to get the printed object's string value, then behaves as though it invokes print(String) and then println().
        ''' </summary>
        ''' <param name="x">The Object to be printed.</param>
        ''' <remarks></remarks>
        Public Sub println(x As Object)

        End Sub

        ''' <summary>
        ''' A convenience method to write a formatted string to this output stream using the specified format string and arguments.
        ''' An invocation of this method of the form out.printf(format, args) behaves in exactly the same way as the invocation
        ''' 
        '''     out.format(Format, args)
        ''' </summary>
        ''' <param name="format">A format string as described in Format string syntax</param>
        ''' <param name="args">Arguments referenced by the format specifiers in the format string. If there are more arguments than format specifiers, the extra arguments are ignored. The number of arguments is variable and may be zero. The maximum number of arguments is limited by the maximum dimension of a Java array as defined by The Java™ Virtual Machine Specification. The behaviour on a null argument depends on the conversion.</param>
        ''' <returns>This output stream</returns>
        ''' <remarks></remarks>
        Public Function printf(format As String,
                          ParamArray args As Object()) As PrintStream
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' A convenience method to write a formatted string to this output stream using the specified format string and arguments.
        ''' An invocation of this method of the form out.printf(l, format, args) behaves in exactly the same way as the invocation
        ''' 
        '''           out.format(l, Format, args)
        ''' </summary>
        ''' <param name="l">The locale to apply during formatting. If l is null then no localization is applied.</param>
        ''' <param name="format">A format string as described in Format string syntax</param>
        ''' <param name="args">Arguments referenced by the format specifiers in the format string. If there are more arguments than format specifiers, the extra arguments are ignored. The number of arguments is variable and may be zero. The maximum number of arguments is limited by the maximum dimension of a Java array as defined by The Java™ Virtual Machine Specification. The behaviour on a null argument depends on the conversion.</param>
        ''' <returns>This output stream</returns>
        ''' <remarks></remarks>
        Public Function printf(l As Locale,
                          format As String,
                         ParamArray args As Object()) As PrintStream
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Writes a formatted string to this output stream using the specified format string and arguments.
        ''' The locale always used is the one returned by Locale.getDefault(), regardless of any previous invocations of other formatting methods on this object.
        ''' </summary>
        ''' <param name="sformat">A format string as described in Format string syntax</param>
        ''' <param name="args">Arguments referenced by the format specifiers in the format string. If there are more arguments than format specifiers, the extra arguments are ignored. The number of arguments is variable and may be zero. The maximum number of arguments is limited by the maximum dimension of a Java array as defined by The Java™ Virtual Machine Specification. The behaviour on a null argument depends on the conversion.</param>
        ''' <returns>This output stream</returns>
        ''' <remarks></remarks>
        Public Function format(sformat As String,
                          ParamArray args As Object()) As PrintStream
            sformat = sprintf(sformat, (From obj As Object In args Let str As Func(Of String) = Function() obj.ToString Select str()).ToArray)
            Call _InnerBuilder.Append(sformat)
            Return Me
        End Function

        ''' <summary>
        ''' Writes a formatted string to this output stream using the specified format string and arguments.
        ''' </summary>
        ''' <param name="l">The locale to apply during formatting. If l is null then no localization is applied.</param>
        ''' <param name="sformat">A format string as described in Format string syntax</param>
        ''' <param name="args">Arguments referenced by the format specifiers in the format string. If there are more arguments than format specifiers, the extra arguments are ignored. The number of arguments is variable and may be zero. The maximum number of arguments is limited by the maximum dimension of a Java array as defined by The Java™ Virtual Machine Specification. The behaviour on a null argument depends on the conversion.</param>
        ''' <returns>This output stream</returns>
        ''' <remarks></remarks>
        Public Function format(l As Locale,
                  sformat As String,
                  ParamArray args As Object()) As PrintStream
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Appends the specified character sequence to this output stream.
        ''' An invocation of this method of the form out.append(csq) behaves in exactly the same way as the invocation
        ''' 
        '''          out.print(csq.toString())
        ''' Depending on the specification of toString for the character sequence csq, the entire sequence may not be appended. For instance, invoking then toString method of a character buffer will return a subsequence whose content depends upon the buffer's position and limit.
        ''' </summary>
        ''' <param name="csq">The character sequence to append. If csq is null, then the four characters "null" are appended to this output stream.</param>
        ''' <returns>This output stream</returns>
        ''' <remarks></remarks>
        Public Function append(csq As CharSequence) As PrintStream
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Appends a subsequence of the specified character sequence to this output stream.
        ''' An invocation of this method of the form out.append(csq, start, end) when csq is not null, behaves in exactly the same way as the invocation
        ''' 
        '''    out.print(csq.subSequence(start, end).toString()) 
        ''' </summary>
        ''' <param name="csq">The character sequence from which a subsequence will be appended. If csq is null, then characters will be appended as if csq contained the four characters "null".</param>
        ''' <param name="start">The index of the first character in the subsequence</param>
        ''' <param name="end">The index of the character following the last character in the subsequence</param>
        ''' <returns>This output stream</returns>
        ''' <remarks></remarks>
        Public Function append(csq As CharSequence,
                          start As Integer,
                           [end] As Integer) As PrintStream
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Appends the specified character to this output stream.
        ''' An invocation of this method of the form out.append(c) behaves in exactly the same way as the invocation
        ''' 
        '''      out.print(c)
        ''' </summary>
        ''' <param name="c">The 16-bit character to append</param>
        ''' <returns>This output stream</returns>
        ''' <remarks></remarks>
        Public Function append(c As Char) As PrintStream
            Throw New NotImplementedException
        End Function

        Public Function append1(c As Char) As Java.Lang.Appendable Implements Lang.Appendable.append
            Return append(c)
        End Function
#End Region

        Public Function append1(csq As CharSequence) As Lang.Appendable Implements Lang.Appendable.append
            Return append(csq)
        End Function

        Public Function append1(csq As CharSequence, start As Integer, [end] As Integer) As Lang.Appendable Implements Lang.Appendable.append
            Return append(csq, start, [end])
        End Function
    End Class
End Namespace