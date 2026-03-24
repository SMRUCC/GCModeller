
''' <summary>
''' 分析结果存储结构（修改为支持多基因组）
''' </summary>
Public Class PanGenomeResult
    ' Key为基因家族ID，Value为该家族包含的所有基因ID列表
    Public Property GeneFamilies As New Dictionary(Of String, List(Of String))()

    ' 核心基因家族（所有品种都有）
    Public Property CoreGeneFamilies As New List(Of Integer)()
    ' 附属基因家族（部分品种有，但不是全部）
    Public Property DispensableGeneFamilies As New List(Of Integer)()
    ' 特异性基因家族（仅1个品种有）
    Public Property SpecificGeneFamilies As New List(Of Integer)()
    ' 单拷贝直系同源基因家族（每个品种仅1个拷贝）
    Public Property SingleCopyOrthologFamilies As New List(Of Integer)()

    ' 统计数据（修改为字典，Key为基因组名称，Value为该基因组基因总数）
    Public Property TotalGenesInGenomes As New Dictionary(Of String, Integer)()
End Class
