Namespace Interpreter

    Public MustInherit Class MachineElement

        ''' <summary>
        ''' 这个函数是显示给<see cref="Runtime.Exceptions.RuntimeException"/>的，
        ''' 而函数<see cref="ToString()"/>则是显示给调试器看的
        ''' </summary>
        ''' <returns></returns>
        Public MustOverride Function ExceptionExpr() As String
    End Class
End Namespace