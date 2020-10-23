Public Class SynchronizedLines

    Public Property xData As Double()
    Public Property datasets As LineDataSet()

End Class

Public Class LineDataSet

    Public Property name As String
    Public Property type As String
    Public Property unit As String
    Public Property valueDecimals As Integer
    Public Property data As Double()

End Class