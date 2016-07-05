Imports System.IO

Namespace util

    Public Class Scanner : Implements Global.System.IDisposable

        ReadOnly FilePath As String
        ReadOnly Reader As Global.System.IO.StreamReader

#Region "Constructor Detail"

        ''' <summary>
        ''' Constructs a new Scanner that produces values scanned from the specified source.
        ''' </summary>
        ''' <param name="source">A character source implementing the Readable interface</param>
        ''' <remarks></remarks>
        Public Sub New(source As Readable)

        End Sub

        ''' <summary>
        ''' Constructs a new Scanner that produces values scanned from the specified input stream. Bytes from the stream are converted into characters using the underlying platform's default charset.
        ''' </summary>
        ''' <param name="source">An input stream to be scanned</param>
        ''' <remarks></remarks>
        Public Sub New(source As IO.InputStream)

        End Sub

        ''' <summary>
        ''' Constructs a new Scanner that produces values scanned from the specified input stream. Bytes from the stream are converted into characters using the specified charset.
        ''' </summary>
        ''' <param name="source">An input stream to be scanned</param>
        ''' <param name="charsetName">The encoding type used to convert bytes from the stream into characters to be scanned</param>
        ''' <remarks></remarks>
        Public Sub New(source As IO.InputStream,
                charsetName As String)

        End Sub

        ''' <summary>
        ''' Constructs a new Scanner that produces values scanned from the specified file. Bytes from the file are converted into characters using the underlying platform's default charset.
        ''' </summary>
        ''' <param name="source">A file to be scanned</param>
        ''' <remarks></remarks>
        Public Sub New(source As Java.IO.File)
            FilePath = source.absolutePath
            Reader = New StreamReader(FilePath, True)
        End Sub

        ''' <summary>
        ''' Constructs a new Scanner that produces values scanned from the specified file. Bytes from the file are converted into characters using the specified charset.
        ''' </summary>
        ''' <param name="source">A file to be scanned</param>
        ''' <param name="charsetName">The encoding type used to convert bytes from the file into characters to be scanned</param>
        ''' <remarks></remarks>
        Public Sub New(source As File,
                        charsetName As String)

        End Sub

        ''' <summary>
        ''' Constructs a new Scanner that produces values scanned from the specified string.
        ''' </summary>
        ''' <param name="source">A string to scan</param>
        ''' <remarks></remarks>
        Public Sub New(source As String)

        End Sub

        ''' <summary>
        ''' Constructs a new Scanner that produces values scanned from the specified channel. Bytes from the source are converted into characters using the underlying platform's default charset.
        ''' </summary>
        ''' <param name="source">A channel to scan</param>
        ''' <remarks></remarks>
        Public Sub New(source As ReadableByteChannel)

        End Sub

        ''' <summary>
        ''' Constructs a new Scanner that produces values scanned from the specified channel. Bytes from the source are converted into characters using the specified charset.
        ''' </summary>
        ''' <param name="source">A channel to scan</param>
        ''' <param name="charsetName">The encoding type used to convert bytes from the channel into characters to be scanned</param>
        ''' <remarks></remarks>
        Public Sub New(source As ReadableByteChannel,
               charsetName As String)

        End Sub
#End Region

#Region "Method Detail"

        ''' <summary>
        ''' Closes this scanner.
        ''' If this scanner has not yet been closed then if its underlying readable also implements the Closeable interface then the readable's close method will be invoked. If this scanner is already closed then invoking this method will have no effect.
        ''' 
        ''' Attempting to perform search operations after a scanner has been closed will result in an IllegalStateException.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub close()
            Call Reader.Close()
        End Sub

        ''' <summary>
        ''' Returns the IOException last thrown by this Scanner's underlying Readable. This method returns null if no such exception exists.
        ''' </summary>
        ''' <returns>the last exception thrown by this scanner's readable</returns>
        ''' <remarks></remarks>
        Public Function ioException() As IOException
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns the Pattern this Scanner is currently using to match delimiters.
        ''' </summary>
        ''' <returns>this Scanner 's delimiting pattern.</returns>
        ''' <remarks></remarks>
        Public Function delimiter() As Pattern
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Sets this scanner's delimiting pattern to the specified pattern.
        ''' </summary>
        ''' <param name="pattern">A delimiting pattern</param>
        ''' <returns>this Scanner</returns>
        ''' <remarks></remarks>
        Public Function useDelimiter(pattern As Pattern) As Scanner
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Sets this scanner's delimiting pattern to a pattern constructed from the specified String.
        ''' An invocation of this method of the form useDelimiter(pattern) behaves in exactly the same way as the invocation hasDelimiter(Pattern.compile(pattern)).
        ''' </summary>
        ''' <param name="pattern">A string specifying a delimiting pattern</param>
        ''' <returns>this Scanner</returns>
        ''' <remarks></remarks>
        Public Function useDelimiter(pattern As String) As Scanner
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns this scanner's locale.
        ''' A Scanner 's locale affects many elements of its default primitive matching regular expressions; see localized numbers above.
        ''' </summary>
        ''' <returns>this Scanner 's locale</returns>
        ''' <remarks></remarks>
        Public Function locale() As Locale
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Sets this scanner's locale to the specified locale.
        ''' A Scanner 's locale affects many elements of its default primitive matching regular expressions; see localized numbers above.
        ''' </summary>
        ''' <param name="locale">A string specifying the locale to use</param>
        ''' <returns>this Scanner</returns>
        ''' <remarks></remarks>
        Public Function useLocale(locale As Locale) As Scanner
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns this scanner's default radix.
        ''' A Scanner 's radix affects elements of its default number matching regular expressions; see localized numbers above.
        ''' </summary>
        ''' <returns>the default radix of this scanner</returns>
        ''' <remarks></remarks>
        Public Function radix() As Integer
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Sets this scanner's default radix to the specified radix.
        '''     A Scanner 's radix affects elements of its default number matching regular expressions; see localized numbers above.
        '''
        ''' If the radix is less than Character.MIN_RADIX or greater than Character.MAX_RADIX, then an IllegalArgumentException is thrown.
        ''' </summary>
        ''' <param name="radix">The radix to use when scanning numbers</param>
        ''' <returns>this Scanner</returns>
        ''' <remarks></remarks>
        Public Function useRadix(radix As Integer) As Scanner
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns the match result of the last scanning operation performed by this scanner. This method throws IllegalStateException if no match has been performed, or if the last match was not successful.
        ''' The various nextmethods of Scanner make a match result available if they complete without throwing an exception. For instance, after an invocation of the nextInt() method that returned an int, this method returns a MatchResult for the search of the Integer regular expression defined above. Similarly the findInLine(java.lang.String), findWithinHorizon(java.lang.String, int), and skip(java.util.regex.Pattern) methods will make a match available if they succeed.
        ''' </summary>
        ''' <returns>a match result for the last match operation</returns>
        ''' <remarks></remarks>
        Public Function match() As MatchResult
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns the string representation of this Scanner. The string representation of a Scanner contains information that may be useful for debugging. The exact format is unspecified.
        ''' </summary>
        ''' <returns>The string representation of this scanner</returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Return FilePath
        End Function

        ''' <summary>
        ''' Returns true if this scanner has another token in its input. This method may block while waiting for input to scan. The scanner does not advance past any input.
        '''        Specified by
        ''' hasNext in interface Iterator
        ''' </summary>
        ''' <returns>true if and only if this scanner has another token</returns>
        ''' <remarks></remarks>
        Public Function hasNext() As Boolean
            Return Not Reader.EndOfStream
        End Function

        ''' <summary>
        ''' Finds and returns the next complete token from this scanner. A complete token is preceded and followed by input that matches the delimiter pattern. This method may block while waiting for input to scan, even if a previous invocation of hasNext() returned true.
        '''       Specified by
        ''' next in interface Iterator
        ''' </summary>
        ''' <returns>the next token</returns>
        ''' <remarks></remarks>
        Public Function [next]() As String
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' The remove operation is not supported by this implementation of Iterator.
        '''       Specified by
        ''' remove in interface Iterator
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub remove()

        End Sub

        ''' <summary>
        ''' Returns true if the next token matches the pattern constructed from the specified string. The scanner does not advance past any input.
        ''' An invocation of this method of the form hasNext(pattern) behaves in exactly the same way as the invocation hasNext(Pattern.compile(pattern)).
        ''' </summary>
        ''' <param name="pattern">a string specifying the pattern to scan</param>
        ''' <returns>true if and only if this scanner has another token matching the specified pattern</returns>
        ''' <remarks></remarks>
        Public Function hasNext(pattern As String) As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns the next token if it matches the pattern constructed from the specified string. If the match is successful, the scanner advances past the input that matched the pattern.
        ''' An invocation of this method of the form next(pattern) behaves in exactly the same way as the invocation next(Pattern.compile(pattern)).
        ''' </summary>
        ''' <param name="pattern">a string specifying the pattern to scan</param>
        ''' <returns>the next token</returns>
        ''' <remarks></remarks>
        Public Function [next](pattern As String) As String
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next complete token matches the specified pattern. A complete token is prefixed and postfixed by input that matches the delimiter pattern. This method may block while waiting for input. The scanner does not advance past any input.
        ''' </summary>
        ''' <param name="pattern">the pattern to scan for</param>
        ''' <returns>true if and only if this scanner has another token matching the specified pattern</returns>
        ''' <remarks></remarks>
        Public Function hasNext(pattern As Pattern) As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns the next token if it matches the specified pattern. This method may block while waiting for input to scan, even if a previous invocation of hasNext(Pattern) returned true. If the match is successful, the scanner advances past the input that matched the pattern.
        ''' </summary>
        ''' <param name="pattern">the pattern to scan for</param>
        ''' <returns>the next token</returns>
        ''' <remarks></remarks>
        Public Function [next](pattern As Pattern) As String
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if there is another line in the input of this scanner. This method may block while waiting for input. The scanner does not advance past any input.
        ''' </summary>
        ''' <returns>true if and only if this scanner has another line of input</returns>
        ''' <remarks></remarks>
        Public Function hasNextLine() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Advances this scanner past the current line and returns the input that was skipped. This method returns the rest of the current line, excluding any line separator at the end. The position is set to the beginning of the next line.
        ''' Since this method continues to search through the input looking for a line separator, it may buffer all of the input searching for the line to skip if no line separators are present.
        ''' </summary>
        ''' <returns>the line that was skipped</returns>
        ''' <remarks></remarks>
        Public Function nextLine() As String
            Return Reader.ReadLine
        End Function

        ''' <summary>
        ''' Attempts to find the next occurrence of a pattern constructed from the specified string, ignoring delimiters.
        ''' An invocation of this method of the form findInLine(pattern) behaves in exactly the same way as the invocation findInLine(Pattern.compile(pattern)).
        ''' </summary>
        ''' <param name="pattern">a string specifying the pattern to search for</param>
        ''' <returns>the text that matched the specified pattern</returns>
        ''' <remarks></remarks>
        Public Function findInLine(pattern As String) As String
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Attempts to find the next occurrence of the specified pattern ignoring delimiters. If the pattern is found before the next line separator, the scanner advances past the input that matched and returns the string that matched the pattern. If no such pattern is detected in the input up to the next line separator, then null is returned and the scanner's position is unchanged. This method may block waiting for input that matches the pattern.
        ''' Since this method continues to search through the input looking for the specified pattern, it may buffer all of the input searching for the desired token if no line separators are present.
        ''' </summary>
        ''' <param name="pattern">the pattern to scan for</param>
        ''' <returns>the text that matched the specified pattern</returns>
        ''' <remarks></remarks>
        Public Function findInLine(pattern As Pattern) As String
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Attempts to find the next occurrence of a pattern constructed from the specified string, ignoring delimiters.
        ''' An invocation of this method of the form findWithinHorizon(pattern) behaves in exactly the same way as the invocation findWithinHorizon(Pattern.compile(pattern, horizon)).
        ''' </summary>
        ''' <param name="pattern">a string specifying the pattern to search for</param>
        ''' <param name="horizon"></param>
        ''' <returns>the text that matched the specified pattern</returns>
        ''' <remarks></remarks>
        Public Function findWithinHorizon(pattern As String,
                                        horizon As Integer) As String
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Attempts to find the next occurrence of the specified pattern.
        ''' This method searches through the input up to the specified search horizon, ignoring delimiters. If the pattern is found the scanner advances past the input that matched and returns the string that matched the pattern. If no such pattern is detected then the null is returned and the scanner's position remains unchanged. This method may block waiting for input that matches the pattern.
        ''' 
        ''' A scanner will never search more than horizon code points beyond its current position. Note that a match may be clipped by the horizon; that is, an arbitrary match result may have been different if the horizon had been larger. The scanner treats the horizon as a transparent, non-anchoring bound (see Matcher.useTransparentBounds(boolean) and Matcher.useAnchoringBounds(boolean)).
        ''' 
        ''' If horizon is 0, then the horizon is ignored and this method continues to search through the input looking for the specified pattern without bound. In this case it may buffer all of the input searching for the pattern.
        ''' 
        ''' If horizon is negative, then an IllegalArgumentException is thrown.
        ''' </summary>
        ''' <param name="pattern">the pattern to scan for</param>
        ''' <param name="horizon"></param>
        ''' <returns>the text that matched the specified pattern</returns>
        ''' <remarks></remarks>
        Public Function findWithinHorizon(pattern As Pattern,
                                          horizon As Integer) As String
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Skips input that matches the specified pattern, ignoring delimiters. This method will skip input if an anchored match of the specified pattern succeeds.
        ''' If a match to the specified pattern is not found at the current position, then no input is skipped and a NoSuchElementException is thrown.
        ''' 
        ''' Since this method seeks to match the specified pattern starting at the scanner's current position, patterns that can match a lot of input (".*", for example) may cause the scanner to buffer a large amount of input.
        ''' 
        ''' Note that it is possible to skip something without risking a NoSuchElementException by using a pattern that can match nothing, e.g., sc.skip("[ \t]*").
        ''' </summary>
        ''' <param name="pattern">a string specifying the pattern to skip over</param>
        ''' <returns>this Scanner</returns>
        ''' <remarks></remarks>
        Public Function [skip](pattern As Pattern) As Scanner
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Skips input that matches a pattern constructed from the specified string.
        ''' An invocation of this method of the form skip(pattern) behaves in exactly the same way as the invocation skip(Pattern.compile(pattern)).
        ''' </summary>
        ''' <param name="pattern">a string specifying the pattern to skip over</param>
        ''' <returns>this Scanner</returns>
        ''' <remarks></remarks>
        Public Function skip(pattern As String) As Scanner
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as a boolean value using a case insensitive pattern created from the string "true|false". The scanner does not advance past the input that matched.
        ''' </summary>
        ''' <returns>true if and only if this scanner's next token is a valid boolean value</returns>
        ''' <remarks></remarks>
        Public Function hasNextBoolean() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input into a boolean value and returns that value. This method will throw InputMismatchException if the next token cannot be translated into a valid boolean value. If the match is successful, the scanner advances past the input that matched.
        ''' </summary>
        ''' <returns>the boolean scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextBoolean() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as a byte value in the default radix using the nextByte() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <returns>true if and only if this scanner's next token is a valid byte value</returns>
        ''' <remarks></remarks>
        Public Function hasNextByte() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as a byte value in the specified radix using the nextByte() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <param name="radix">the radix used to interpret the token as a byte value</param>
        ''' <returns>true if and only if this scanner's next token is a valid byte value</returns>
        ''' <remarks></remarks>
        Public Function hasNextByte(radix As Integer) As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as a byte.
        ''' An invocation of this method of the form nextByte() behaves in exactly the same way as the invocation nextByte(radix), where radix is the default radix of this scanner.
        ''' </summary>
        ''' <returns>the byte scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextByte() As Byte
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as a byte. This method will throw InputMismatchException if the next token cannot be translated into a valid byte value as described below. If the translation is successful, the scanner advances past the input that matched.
        ''' If the next token matches the Integer regular expression defined above then the token is converted into a byte value as if by removing all locale specific prefixes, group separators, and locale specific suffixes, then mapping non-ASCII digits into ASCII digits via Character.digit, prepending a negative sign (-) if the locale specific negative prefixes and suffixes were present, and passing the resulting string to Byte.parseByte with the specified radix.
        ''' </summary>
        ''' <param name="radix">the radix used to interpret the token as a byte value</param>
        ''' <returns>the byte scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextByte(radix As Integer) As Byte
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as a short value in the default radix using the nextShort() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <returns>true if and only if this scanner's next token is a valid short value in the default radix</returns>
        ''' <remarks></remarks>
        Public Function hasNextShort() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as a short value in the specified radix using the nextShort() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <param name="radix">the radix used to interpret the token as a short value</param>
        ''' <returns>true if and only if this scanner's next token is a valid short value in the specified radix</returns>
        ''' <remarks></remarks>
        Public Function hasNextShort(radix As Integer) As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as a short.
        ''' An invocation of this method of the form nextShort() behaves in exactly the same way as the invocation nextShort(radix), where radix is the default radix of this scanner.
        ''' </summary>
        ''' <returns>the short scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextShort() As Short
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as a short. This method will throw InputMismatchException if the next token cannot be translated into a valid short value as described below. If the translation is successful, the scanner advances past the input that matched.
        ''' If the next token matches the Integer regular expression defined above then the token is converted into a short value as if by removing all locale specific prefixes, group separators, and locale specific suffixes, then mapping non-ASCII digits into ASCII digits via Character.digit, prepending a negative sign (-) if the locale specific negative prefixes and suffixes were present, and passing the resulting string to Short.parseShort with the specified radix.
        ''' </summary>
        ''' <param name="radix">the radix used to interpret the token as a short value</param>
        ''' <returns>the short scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextShort(radix As Integer) As Short
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as an int value in the default radix using the nextInt() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <returns>true if and only if this scanner's next token is a valid int value</returns>
        ''' <remarks></remarks>
        Public Function hasNextInt() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as an int value in the specified radix using the nextInt() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <param name="radix">the radix used to interpret the token as an int value</param>
        ''' <returns>true if and only if this scanner's next token is a valid int value</returns>
        ''' <remarks></remarks>
        Public Function hasNextInt(radix As Integer) As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as an int.
        ''' An invocation of this method of the form nextInt() behaves in exactly the same way as the invocation nextInt(radix), where radix is the default radix of this scanner
        ''' </summary>
        ''' <returns>the int scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextInt() As Integer
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as an int. This method will throw InputMismatchException if the next token cannot be translated into a valid int value as described below. If the translation is successful, the scanner advances past the input that matched.
        ''' If the next token matches the Integer regular expression defined above then the token is converted into an int value as if by removing all locale specific prefixes, group separators, and locale specific suffixes, then mapping non-ASCII digits into ASCII digits via Character.digit, prepending a negative sign (-) if the locale specific negative prefixes and suffixes were present, and passing the resulting string to Integer.parseInt with the specified radix.
        ''' </summary>
        ''' <param name="radix">the radix used to interpret the token as an int value</param>
        ''' <returns>the int scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextInt(radix As Integer) As Integer
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as a long value in the default radix using the nextLong() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <returns>true if and only if this scanner's next token is a valid long value</returns>
        ''' <remarks></remarks>
        Public Function hasNextLong() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as a long value in the specified radix using the nextLong() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <param name="radix">the radix used to interpret the token as a long value</param>
        ''' <returns>true if and only if this scanner's next token is a valid long value</returns>
        ''' <remarks></remarks>
        Public Function hasNextLong(radix As Integer) As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as a long.
        ''' An invocation of this method of the form nextLong() behaves in exactly the same way as the invocation nextLong(radix), where radix is the default radix of this scanner.
        ''' </summary>
        ''' <returns>the long scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextLong() As Long
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as a long. This method will throw InputMismatchException if the next token cannot be translated into a valid long value as described below. If the translation is successful, the scanner advances past the input that matched.
        ''' If the next token matches the Integer regular expression defined above then the token is converted into an long value as if by removing all locale specific prefixes, group separators, and locale specific suffixes, then mapping non-ASCII digits into ASCII digits via Character.digit, prepending a negative sign (-) if the locale specific negative prefixes and suffixes were present, and passing the resulting string to Long.parseLong with the specified radix.
        ''' </summary>
        ''' <param name="radix">the radix used to interpret the token as an int value</param>
        ''' <returns>the long scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextLong(radix As Integer) As Long
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as a float value using the nextFloat() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <returns>true if and only if this scanner's next token is a valid float value</returns>
        ''' <remarks></remarks>
        Public Function hasNextFloat() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as a float. This method will throw InputMismatchException if the next token cannot be translated into a valid float value as described below. If the translation is successful, the scanner advances past the input that matched.
        ''' If the next token matches the Float regular expression defined above then the token is converted into a float value as if by removing all locale specific prefixes, group separators, and locale specific suffixes, then mapping non-ASCII digits into ASCII digits via Character.digit, prepending a negative sign (-) if the locale specific negative prefixes and suffixes were present, and passing the resulting string to Float.parseFloat. If the token matches the localized NaN or infinity strings, then either "Nan" or "Infinity" is passed to Float.parseFloat as appropriate.
        ''' </summary>
        ''' <returns>the float scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextFloat() As Float
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as a double value using the nextDouble() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <returns>true if and only if this scanner's next token is a valid double value</returns>
        ''' <remarks></remarks>
        Public Function hasNextDouble() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as a double. This method will throw InputMismatchException if the next token cannot be translated into a valid double value. If the translation is successful, the scanner advances past the input that matched.
        ''' If the next token matches the Float regular expression defined above then the token is converted into a double value as if by removing all locale specific prefixes, group separators, and locale specific suffixes, then mapping non-ASCII digits into ASCII digits via Character.digit, prepending a negative sign (-) if the locale specific negative prefixes and suffixes were present, and passing the resulting string to Double.parseDouble. If the token matches the localized NaN or infinity strings, then either "Nan" or "Infinity" is passed to Double.parseDouble as appropriate.
        ''' </summary>
        ''' <returns>the double scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextDouble() As Double
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as a BigInteger in the default radix using the nextBigInteger() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <returns>true if and only if this scanner's next token is a valid BigInteger</returns>
        ''' <remarks></remarks>
        Public Function hasNextBigInteger() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as a BigInteger in the specified radix using the nextBigInteger() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <param name="radix">the radix used to interpret the token as an integer</param>
        ''' <returns>true if and only if this scanner's next token is a valid BigInteger</returns>
        ''' <remarks></remarks>
        Public Function hasNextBigInteger(radix As Integer) As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as a BigInteger.
        ''' An invocation of this method of the form nextBigInteger() behaves in exactly the same way as the invocation nextBigInteger(radix), where radix is the default radix of this scanner.
        ''' </summary>
        ''' <returns>the BigInteger scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextBigInteger() As BigInteger
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as a BigInteger.
        ''' If the next token matches the Integer regular expression defined above then the token is converted into a BigInteger value as if by removing all group separators, mapping non-ASCII digits into ASCII digits via the Character.digit, and passing the resulting string to the BigInteger(String, int) constructor with the specified radix.
        ''' </summary>
        ''' <param name="radix">the radix used to interpret the token</param>
        ''' <returns>the BigInteger scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextBigInteger(radix As Integer) As BigInteger
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Returns true if the next token in this scanner's input can be interpreted as a BigDecimal using the nextBigDecimal() method. The scanner does not advance past any input.
        ''' </summary>
        ''' <returns>true if and only if this scanner's next token is a valid BigDecimal</returns>
        ''' <remarks></remarks>
        Public Function hasNextBigDecimal() As Boolean
            Throw New NotImplementedException
        End Function

        ''' <summary>
        ''' Scans the next token of the input as a BigDecimal.
        ''' If the next token matches the Decimal regular expression defined above then the token is converted into a BigDecimal value as if by removing all group separators, mapping non-ASCII digits into ASCII digits via the Character.digit, and passing the resulting string to the BigDecimal(String) constructor.
        ''' </summary>
        ''' <returns>the BigDecimal scanned from the input</returns>
        ''' <remarks></remarks>
        Public Function nextBigDecimal() As BigDecimal
            Throw New NotImplementedException
        End Function
#End Region

#Region "IDisposable Support"
        Private disposedValue As Boolean ' 检测冗余的调用

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
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
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 不要更改此代码。    请将清理代码放入上面的 Dispose (disposing As Boolean)中。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace

Public Class Float

    Dim _doubleValue As Double

    Sub New(num As String)
        _doubleValue = Val(num)
    End Sub

    Protected Friend Sub New()
    End Sub

    Public Overrides Function ToString() As String
        Return _doubleValue
    End Function

    Public Shared Widening Operator CType(n As Double) As Float
        Return New Float With {._doubleValue = n}
    End Operator

    Public Shared Widening Operator CType(numberString As String) As Float
        Return New Float(numberString)
    End Operator
End Class