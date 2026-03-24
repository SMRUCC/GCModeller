''' <summary>
''' 共线性区块定义
''' </summary>
Public Class CollinearBlock
    Public Property Genome1 As String
    Public Property Genome2 As String
    Public Property Chr1 As String
    Public Property Chr2 As String
    ''' <summary>
    ''' 区块包含的基因对
    ''' </summary>
    ''' <returns></returns>
    Public Property OrthologyLinks As OrthologyLink()
    ''' <summary>
    ''' TODO: 评估指标：得分或E-value
    ''' </summary>
    ''' <returns></returns>
    Public Property Score As Double
End Class