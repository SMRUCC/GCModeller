Public Class SampleInfo

    ''' <summary>
    ''' 符合VisualBasic标识符语法的目标样品标识符
    ''' </summary>
    ''' <returns></returns>
    Public Property ID As String
    ''' <summary>
    ''' 显示名称，可能会含有一些奇怪的符号
    ''' </summary>
    ''' <returns></returns>
    Public Property sample_name As String
    ''' <summary>
    ''' 样品的实验设计分组信息
    ''' </summary>
    ''' <returns></returns>
    Public Property sample_group As String
    ''' <summary>
    ''' index编号
    ''' </summary>
    ''' <returns></returns>
    Public Property Order As Integer
    ''' <summary>
    ''' 绘图可视化的时候的颜色
    ''' </summary>
    ''' <returns></returns>
    Public Property color As String
    ''' <summary>
    ''' legend的形状
    ''' </summary>
    ''' <returns></returns>
    Public Property shapetype As String

    Public Overrides Function ToString() As String
        Return sample_name
    End Function
End Class
