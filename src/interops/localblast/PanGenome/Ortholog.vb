
' 原始数据结构定义
Public Class Ortholog
    Public Property genome1 As String ' 实际上这里存储的是 GeneID1
    Public Property genome2 As String ' 实际上这里存储的是 GeneID2
    ' 其他属性在聚类分析中非必需，暂不使用
    Public Property identities1 As Double
    Public Property identities2 As Double
    Public Property evalue1 As Double
    Public Property evalue2 As Double
    Public Property bitscore1 As Double
    Public Property bitscore2 As Double
End Class