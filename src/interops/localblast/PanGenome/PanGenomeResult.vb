
''' <summary>
''' 分析结果存储结构（修改为支持多基因组）
''' </summary>
Public Class PanGenomeResult
    ' Key为基因家族ID，Value为该家族包含的所有基因ID列表
    Public Property GeneFamilies As New Dictionary(Of String, String())()

    ' 核心基因家族（所有品种都有）
    Public Property CoreGeneFamilies As String()
    ' 附属基因家族（部分品种有，但不是全部）
    Public Property DispensableGeneFamilies As String()
    ' 特异性基因家族（仅1个品种有）
    Public Property SpecificGeneFamilies As String()
    ' 单拷贝直系同源基因家族（每个品种仅1个拷贝）
    Public Property SingleCopyOrthologFamilies As String()

    ' 统计数据（修改为字典，Key为基因组名称，Value为该基因组基因总数）
    Public Property TotalGenesInGenomes As New Dictionary(Of String, Integer)()

    ' 1. PAV 矩阵
    ' Key为基因家族ID，Value为字典(Key为基因组名，Value为拷贝数/存在与否)
    Public Property PAVMatrix As New Dictionary(Of String, Dictionary(Of String, Integer))()

    ' 2. 泛基因组曲线数据
    ' 列表项为：加入的第N个基因组，总基因数，核心基因数
    Public Property PangenomeCurveData As PangenomeCurveData()

    ' 3. 共线性区块
    Public Property CollinearBlocks As CollinearBlock()

End Class

Public Class PangenomeCurveData

    Public Property GenomeCount As Integer
    Public Property TotalGenes As Integer
    Public Property CoreGenes As Integer

End Class