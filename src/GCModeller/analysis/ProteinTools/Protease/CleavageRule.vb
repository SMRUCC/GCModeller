Public Class CleavageRule

    ''' <summary>
    ''' FullName
    ''' </summary>
    ''' <returns></returns>
    Public Property Name As String
    ''' <summary>
    ''' BriefName
    ''' </summary>
    ''' <returns></returns>
    Public Property Protease As String

#Region "in C-terminal direction"
    Public Property P4 As String
    Public Property P3 As String
    Public Property P2 As String
    Public Property P1 As String
#End Region

#Region "in C-terminal direction"
    Public Property P1C As String
    Public Property P2C As String
#End Region

End Class
