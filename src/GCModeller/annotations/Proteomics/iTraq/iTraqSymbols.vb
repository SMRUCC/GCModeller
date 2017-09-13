''' <summary>
''' iTraq信号标记替换
''' </summary>
Public Class iTraqSymbols

    ''' <summary>
    ''' iTraq信号标记
    ''' </summary>
    ''' <returns></returns>
    Public Property Symbol As String
    ''' <summary>
    ''' 将质谱实验下机数据转录结果文件之中的信号标记<see cref="Symbol"/>替换为用户的样品<see cref="SampleID"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property SampleID As String
    ''' <summary>
    ''' 样品进行数据分析的时候所使用的名称
    ''' </summary>
    ''' <returns></returns>
    Public Property AnalysisID As String

    Public Overrides Function ToString() As String
        Return $"{Symbol} -> {SampleID}"
    End Function
End Class
