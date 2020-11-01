Imports System


    ''' <summary>
    ''' Exception signaling that the R engine failed to evaluate a statement
    ''' </summary>
    Public Class EvaluationException
        Inherits Exception
        ''' <summary>
        ''' Create an exception for a statement that failed to be evaluate by e.g. R_tryEval
        ''' </summary>
        ''' <param name="errorMsg">The last error message of the failed evaluation in the R engine</param>
        Public Sub New(ByVal errorMsg As String)
            MyBase.New(errorMsg)
        End Sub

        ''' <summary>
        ''' Create an exception for a statement that failed to be evaluate by e.g. R_tryEval
        ''' </summary>
        ''' <param name="errorMsg">The last error message of the failed evaluation in the R engine</param>
        ''' <param name="innerException">The exception that was caught and triggered the creation of this evaluation exception</param>
        Public Sub New(ByVal errorMsg As String, ByVal innerException As Exception)
            MyBase.New(errorMsg, innerException)
        End Sub
    End Class

