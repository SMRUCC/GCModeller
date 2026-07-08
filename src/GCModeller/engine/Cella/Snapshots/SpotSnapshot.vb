''' <summary>
''' 某一个单元格在某一数据帧上的快照数据
''' </summary>
Public Class SpotSnapshot

    Public Property x As Integer
    Public Property y As Integer
    Public Property z As Integer
    ''' <summary>
    ''' 当前单元格内的细胞列表
    ''' </summary>
    ''' <returns></returns>
    Public Property cells As CellSnapshot()
    ''' <summary>
    ''' 当前单元格内的外部物质组成信息，[molecule_id => mass_contents]
    ''' </summary>
    ''' <returns></returns>
    Public Property externals As Dictionary(Of String, Double)
    ''' <summary>
    ''' 当前网格环境内的ph值
    ''' </summary>
    ''' <returns></returns>
    Public Property ph As Double
    ''' <summary>
    ''' 摄氏度为单位的温度
    ''' </summary>
    ''' <returns></returns>
    Public Property temperature As Double

End Class
