''' <summary>
''' 用于定义模拟测序区域的“热点”，即高丰度区域。
''' </summary>
Public Class RegionHotspot
    ''' <summary>
    ''' 热点区域的起始位置（0-based索引）。
    ''' </summary>
    Public Property Start As Integer

    ''' <summary>
    ''' 热点区域的结束位置（0-based索引）。
    ''' </summary>
    Public Property [End] As Integer

    ''' <summary>
    ''' 该热点的相对权重。权重越高，被选中的概率越大。
    ''' </summary>
    Public Property Weight As Double
End Class