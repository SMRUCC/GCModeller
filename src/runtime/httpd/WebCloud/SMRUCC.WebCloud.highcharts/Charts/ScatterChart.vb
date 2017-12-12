Namespace ScatterChart

    Public Class scatterOptions
        Public Property marker As marker
        Public Property states As states
        Public Property tooltip As tooltip
    End Class

    Public Class marker
        Public Property radius As Double
        Public Property states As states
    End Class

    Public Class states
        Public Property hover As effect
    End Class

    Public Class effect
        Public Property enabled As Boolean
        Public Property lineColor As String
        Public Property marker As markerOptions
    End Class

    Public Class markerOptions
        Public Property enabled As Boolean
    End Class

    Public Class ScatterSerial
        Public Property name As String
        Public Property color As String
        Public Property colorByPoint As Boolean
        Public Property data As Double()()
    End Class
End Namespace
