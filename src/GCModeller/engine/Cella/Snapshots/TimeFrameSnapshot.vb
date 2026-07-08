Public Class TimeFrameSnapshot

    Public Property environment As SpotSnapshot()
    Public Property time As Double
    ''' <summary>
    ''' count of the cell in <see cref="CellSnapshot.taxonomy"/> group
    ''' </summary>
    ''' <returns></returns>
    Public Property cells As Dictionary(Of String, Double)

End Class
