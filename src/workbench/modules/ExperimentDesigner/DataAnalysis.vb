Public Class DataAnalysis

    Public Property designs As DataGroup()

    Public ReadOnly Property size As Integer
        Get
            Return designs.Length
        End Get
    End Property

    Public ReadOnly Property experiment As DataGroup
        Get
            If size = 2 Then
                Return _designs(0)
            Else
                Return Nothing
            End If
        End Get
    End Property

    Public ReadOnly Property control As DataGroup
        Get
            If size = 2 Then
                Return _designs(1)
            Else
                Return Nothing
            End If
        End Get
    End Property

End Class

Public Class DataGroup

    Public Property sampleGroup As String
    Public Property sample_id As String()
    Public Property color As String
    Public Property shape As String

End Class