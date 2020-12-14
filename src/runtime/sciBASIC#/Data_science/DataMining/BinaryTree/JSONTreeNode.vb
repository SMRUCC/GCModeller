Public Class JSONTreeNode

    Public Property uuid As String
    Public Property members As String()
    Public Property left As JSONTreeNode
    Public Property right As JSONTreeNode

    Public Overrides Function ToString() As String
        Return uuid
    End Function

End Class
