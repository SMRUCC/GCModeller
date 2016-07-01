Namespace Lang

    Public Interface Appendable
#Region "Method Detail"

        ''' <summary>
        ''' Appends the specified character sequence to this Appendable.
        ''' Depending on which class implements the character sequence csq, the entire sequence may not be appended. For instance, if csq is a CharBuffer then the subsequence to append is defined by the buffer's position and limit.
        ''' </summary>
        ''' <param name="csq">The character sequence to append. If csq is null, then the four characters "null" are appended to this Appendable.</param>
        ''' <returns>A reference to this Appendable</returns>
        ''' <remarks></remarks>
        Function append(csq As CharSequence) As Appendable

        ''' <summary>
        ''' Appends a subsequence of the specified character sequence to this Appendable.
        ''' An invocation of this method of the form out.append(csq, start, end) when csq is not null, behaves in exactly the same way as the invocation
        ''' 
        ''' out.append(csq.subSequence(start, end)) 
        ''' </summary>
        ''' <param name="csq">The character sequence from which a subsequence will be appended. If csq is null, then characters will be appended as if csq contained the four characters "null".</param>
        ''' <param name="start">The index of the first character in the subsequence</param>
        ''' <param name="end">The index of the character following the last character in the subsequence</param>
        ''' <returns>A reference to this Appendable</returns>
        ''' <remarks></remarks>
        Function append(csq As CharSequence,
                         start As Integer,
                         [end] As Integer) As Appendable

        ''' <summary>
        ''' Appends the specified character to this Appendable.
        ''' </summary>
        ''' <param name="c">The character to append</param>
        ''' <returns>A reference to this Appendable</returns>
        ''' <remarks></remarks>
        Function append(c As Char) As Appendable
#End Region
    End Interface
End Namespace