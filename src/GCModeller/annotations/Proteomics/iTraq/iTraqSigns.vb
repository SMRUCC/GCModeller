Imports Microsoft.VisualBasic.Data.csv.IO
Imports Microsoft.VisualBasic.Language

''' <summary>
''' iTraq信号标记替换
''' </summary>
Public Class iTraqSigns

    ''' <summary>
    ''' iTraq信号标记
    ''' </summary>
    ''' <returns></returns>
    Public Property Sign As String
    ''' <summary>
    ''' 将质谱实验下机数据转录结果文件之中的信号标记<see cref="Sign"/>替换为用户的样品<see cref="SampleID"/>
    ''' </summary>
    ''' <returns></returns>
    Public Property SampleID As String

    Public Overrides Function ToString() As String
        Return $"{Sign} -> {SampleID}"
    End Function
End Class
