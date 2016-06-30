Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder
Imports RDotNET.Extensions.VisualBasic.Services.ScriptBuilder.RTypes

Namespace utils

    ''' <summary>
    ''' Generate all combinations of the elements of x taken m at a time. If x is a positive integer, returns all combinations of the elements of seq(x) taken m at a time. 
    ''' If argument FUN is not NULL, applies a function given by the argument to each point. If simplify is FALSE, returns a list; 
    ''' otherwise returns an array, typically a matrix. ... are passed unchanged to the FUN function, if specified.
    ''' 
    ''' Factors x are accepted from R 3.1.0 (although coincidentally they worked for simplify = FALSE in earlier versions).
    ''' </summary>
    <RFunc("combn")> Public Class combn : Inherits IRToken

        ''' <summary>
        ''' vector source for combinations, or integer n for x &lt;- seq_len(n).
        ''' </summary>
        ''' <returns></returns>
        Public Property x As RExpression
        ''' <summary>
        ''' number of elements to choose.
        ''' </summary>
        ''' <returns></returns>
        Public Property m As Integer
        ''' <summary>
        ''' function to be applied to each combination; default NULL means the identity, i.e., to return the combination (vector of length m).
        ''' </summary>
        ''' <returns></returns>
        Public Property FUN As RExpression = NULL
        ''' <summary>
        ''' logical indicating if the result should be simplified to an array (typically a matrix); if FALSE, the function returns a list. Note that when simplify = TRUE as by default, the dimension of the result is simply determined from FUN(1st combination) (for efficiency reasons). This will badly fail if FUN(u) is not of constant length.
        ''' </summary>
        ''' <returns></returns>
        Public Property simplify As Boolean = True
    End Class
End Namespace