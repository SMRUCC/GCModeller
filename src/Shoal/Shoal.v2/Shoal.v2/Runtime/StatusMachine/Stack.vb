Namespace Runtime

    Public Class Stack

        Public Property [If] As Boolean
        Public Property DoWhile As Interpreter.LDM.Expressions.PrimaryExpression

        ReadOnly _uid As String

        Sub New(uid As String)
            _uid = uid
        End Sub

        Public Overrides Function ToString() As String
            Return _uid
        End Function

    End Class
End Namespace