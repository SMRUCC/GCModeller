Namespace Options

    ''' <summary>
    ''' 扫描/延伸选项
    ''' </summary>
    Public Class SeedExtendOptions

        Public Property WordSize As Integer = 11
        ''' <summary>
        ''' A
        ''' </summary>
        ''' <returns></returns>
        Public Property WindowTwoHit As Integer = 40
        Public Property UseTwoHit As Boolean = True
        ''' <summary>
        ''' blastn 默认
        ''' </summary>
        ''' <returns></returns>
        Public Property XdropUngapBits As Double = 20
        Public Property XdropGapBits As Double = 30
        Public Property XdropGapFinalBits As Double = 100
        Public Property GapOpen As Double = 5
        Public Property GapExtend As Double = 2
        Public Property MaxCellsPerExtension As Long = 4000000

    End Class
End Namespace