
' 分析结果存储结构
Public Class PanGenomeResult
    ' Key为基因家族ID，Value为该家族包含的所有基因ID列表
    Public Property GeneFamilies As New Dictionary(Of Integer, List(Of String))()

    ' 核心基因家族（三个品种都有）
    Public Property CoreGeneFamilies As New List(Of Integer)()
    ' 附属基因家族（1个或2个品种有）
    Public Property DispensableGeneFamilies As New List(Of Integer)()
    ' 特异性基因家族（仅1个品种有）
    Public Property SpecificGeneFamilies As New List(Of Integer)()
    ' 单拷贝直系同源基因家族（每个品种仅1个拷贝）
    Public Property SingleCopyOrthologFamilies As New List(Of Integer)()

    ' 统计数据
    Public Property TotalGenesInGenome1 As Integer
    Public Property TotalGenesInGenome2 As Integer
    Public Property TotalGenesInGenome3 As Integer
End Class