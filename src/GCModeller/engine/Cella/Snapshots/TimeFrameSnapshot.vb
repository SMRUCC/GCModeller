''' <summary>
''' frame_xxx.json
''' </summary>
Public Class TimeFrameSnapshot

    ''' <summary>
    ''' <see cref="Metadata.shape"/>中为true的单元格位置上的实例快照数据
    ''' </summary>
    ''' <returns></returns>
    Public Property environment As SpotSnapshot()
    Public Property time As Double
    ''' <summary>
    ''' count of the cell in <see cref="CellSnapshot.taxonomy"/> group, [taxonomy => cell_count]
    ''' </summary>
    ''' <returns></returns>
    Public Property cells As Dictionary(Of String, Integer)

End Class
