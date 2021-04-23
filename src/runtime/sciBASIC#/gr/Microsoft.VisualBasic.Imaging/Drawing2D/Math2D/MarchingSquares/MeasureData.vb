Namespace Drawing2D.Math2D.MarchingSquares

    ''' <summary>
    ''' 测量数据
    ''' </summary>
    Public Structure MeasureData
        ''' <summary>
        ''' 初始化测量数据
        ''' </summary>
        ''' <paramname="x">坐标x</param>
        ''' <paramname="y">坐标y</param>
        ''' <paramname="z">高度</param>
        Public Sub New(ByVal x As Single, ByVal y As Single, ByVal z As Single)
            Me.X = x
            Me.Y = y
            Me.Z = z
        End Sub

        ''' <summary>
        ''' 坐标X
        ''' </summary>
        Public X As Single

        ''' <summary>
        ''' 坐标Y
        ''' </summary>
        Public Y As Single

        ''' <summary>
        ''' 高度
        ''' </summary>
        Public Z As Single
    End Structure

End Namespace