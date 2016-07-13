Imports System.Runtime.CompilerServices

Namespace Language.Perl

    Public Module Functions

        ''' <summary>
        ''' Treats ARRAY as a stack by appending the values of LIST to the end of ARRAY. The length of ARRAY 
        ''' increases by the length of LIST. Has the same effect as
        ''' 
        ''' ```perl
        ''' for my $value (LIST) {
        '''     $ARRAY[++$#ARRAY] = $value;
        ''' }
        ''' ```
        ''' 
        ''' but Is more efficient. Returns the number of elements in the array following the completed push.
        ''' Starting with Perl 5.14, an experimental feature allowed push to take a scalar expression. 
        ''' This experiment has been deemed unsuccessful, And was removed as of Perl 5.24.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="array"></param>
        ''' <param name="x"></param>
        ''' 
        <Extension>
        Public Sub Push(Of T)(ByRef array As T(), x As T)
            ReDim Preserve array(array.Length)
            array(array.Length - 1) = x
        End Sub

        ''' <summary>
        ''' Treats ARRAY as a stack by appending the values of LIST to the end of ARRAY. The length of ARRAY 
        ''' increases by the length of LIST. Has the same effect as
        ''' 
        ''' ```perl
        ''' for my $value (LIST) {
        '''     $ARRAY[++$#ARRAY] = $value;
        ''' }
        ''' ```
        ''' 
        ''' but Is more efficient. Returns the number of elements in the array following the completed push.
        ''' Starting with Perl 5.14, an experimental feature allowed push to take a scalar expression. 
        ''' This experiment has been deemed unsuccessful, And was removed as of Perl 5.24.
        ''' </summary>
        ''' <typeparam name="T"></typeparam>
        ''' <param name="array"></param>
        ''' <param name="LIST"></param>
        <Extension>
        Public Sub Push(Of T)(ByRef array As T(), LIST As IEnumerable(Of T))
            Dim source As T() = LIST.ToArray
            Dim tmp As T() = New T(array.Length + source.Length - 1) {}

            Call System.Array.ConstrainedCopy(array, Scan0, tmp, Scan0, array.Length)
            Call System.Array.ConstrainedCopy(source, Scan0, tmp, array.Length, source.Length)

            array = tmp
        End Sub
    End Module
End Namespace