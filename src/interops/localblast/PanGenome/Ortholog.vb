
''' <summary>
''' 直系同源原始数据结构定义
''' </summary>
Public Class Ortholog

    ''' <summary>
    ''' gene id in genome1
    ''' </summary>
    ''' <returns></returns>
    Public Property gene1 As String
    Public Property gene2 As String

    ' 其他属性在聚类分析中非必需，暂不使用
    Public Property identities1 As Double
    Public Property identities2 As Double
    Public Property evalue1 As Double
    Public Property evalue2 As Double
    Public Property bitscore1 As Double
    Public Property bitscore2 As Double

End Class