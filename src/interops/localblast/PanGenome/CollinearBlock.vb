''' <summary>
''' 共线性区块定义
''' </summary>
Public Class CollinearBlock
    Public Property Genome1 As String
    Public Property Genome2 As String
    Public Property Chr1 As String
    Public Property Chr2 As String
    ' 区块包含的基因对
    Public Property OrthologyLinks As OrthologyLink()
    ' 评估指标：得分或E-value（可选）
    Public Property Score As Double
End Class