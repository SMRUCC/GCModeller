Namespace Core

    ''' <summary>数据库条目（编码 + 掩码一次生成）</summary>
    Public Class DbEntry

        Public Id As String
        Public Description As String
        Public Codes() As Int32
        Public Mask() As Boolean
        Public Length As Integer

    End Class

    Public Class DbStatistics

        Public Sequences As Long
        Public Residues As Long

    End Class
End Namespace