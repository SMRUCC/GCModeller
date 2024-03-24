Namespace MeSH

    Public Class XmlString

        Public Property [String] As String

        Public Overrides Function ToString() As String
            Return [String]
        End Function

    End Class

    Public Class XmlDate

        Public Property Year As Integer
        Public Property Month As Integer
        Public Property Day As Integer

        Public Overrides Function ToString() As String
            Return $"{Year}-{Month}-{Day}"
        End Function

    End Class
End Namespace