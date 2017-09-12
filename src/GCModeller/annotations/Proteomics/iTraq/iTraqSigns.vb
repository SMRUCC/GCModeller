Imports Microsoft.VisualBasic.Data.csv.IO

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

    ''' <summary>
    ''' 将原始数据之中的标记替换为用户的样品名称
    ''' </summary>
    ''' <param name="header"></param>
    ''' <returns></returns>
    Public Function Replace(header As RowObject) As RowObject

    End Function
End Class
