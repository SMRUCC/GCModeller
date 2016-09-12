Imports RDotNET.Extensions.VisualBasic.SymbolBuilder.RScripts

Namespace API

    Partial Module base

        ''' <summary>
        ''' Generate regular sequences. seq is a standard generic with a default method. seq.int is a primitive which can be much faster but has a few restrictions. seq_along and seq_len are very fast primitives for two common cases.
        ''' </summary>
        ''' <param name="from">the starting and (maximal) end values of the sequence. Of length 1 unless just from is supplied as an unnamed argument.</param>
        ''' <param name="[to]">the starting and (maximal) end values of the sequence. Of length 1 unless just from is supplied as an unnamed argument.</param>
        ''' <param name="by">number: increment of the sequence.</param>
        ''' <param name="lengthOut">desired length of the sequence. A non-negative number, which for seq and seq.int will be rounded up if fractional.</param>
        ''' <param name="alongWith">take the length from the length of this argument.</param>
        ''' <param name="additionals">arguments passed to or from methods.</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Numerical inputs should all be finite (that is, not infinite, NaN or NA).
        ''' The interpretation Of the unnamed arguments Of seq And seq.int Is Not standard, And it Is recommended always To name the arguments When programming.
        ''' seq Is generic, And only the default method Is described here. Note that it dispatches on the class of the first argument irrespective of argument names. 
        ''' This can have unintended consequences if it Is called with just one argument intending this to be taken as along.with it Is much better to use seg_along in that case.
        ''' seq.int Is an internal generic which dispatches on methods for "seq" based on the class of the first supplied argument (before argument matching).
        ''' </remarks>
        Public Function seq(Optional from As Double = 1,
                            Optional [to] As Double = 1,
                            Optional by As String = NULL,
                            Optional lengthOut As String = NULL,
                            Optional alongWith As String = NULL,
                            Optional additionals As String() = Nothing) As String
            Dim tmp As String = App.NextTempName

            If alongWith <> NULL Then
                Call $"{tmp} <- seq(along.with= {alongWith}  {additionals.params})".丶
            Else
                If lengthOut <> NULL Then
                    If by = NULL Then
                        Call $"{tmp} <- seq(from = {from}, to = {[to]}, by = ((to - from)/(length.out - 1)), length.out = {lengthOut} {additionals.params})".丶
                    Else
                        Call $"{tmp} <- seq(from = {from}, to = {[to]}, length.out = {lengthOut} {additionals.params})".丶
                    End If
                Else
                    If by = NULL Then
                        Call $"{tmp} <- seq(from = {from}, to = {[to]} {additionals.params})".丶
                    Else
                        Call $"{tmp} <- seq(from = {from}, to = {[to]}, by = {by} {additionals.params})".丶
                    End If
                End If
            End If

            Return tmp
        End Function
    End Module
End Namespace