Public Class options3d
    Public Property enabled As Boolean?
    Public Property alpha As Double?
    Public Property beta As Double?
    Public Property depth As Double?
    Public Property viewDistance As Double?
    Public Property fitToPlot As Boolean?
    Public Property frame As frame3DOptions

    Public Overrides Function ToString() As String
        If enabled Then
            Return NameOf(enabled)
        Else
            Return $"Not {NameOf(enabled)}"
        End If
    End Function
End Class

Public Class frame3DOptions
    Public Property bottom As frameOptions
    Public Property back As frameOptions
    Public Property side As frameOptions
End Class

Public Class frameOptions
    Public Property size As Double?
    Public Property color As String
End Class