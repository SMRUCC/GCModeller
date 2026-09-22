' ============================================================
' CellFate.vb - 细胞命运（细胞类型）与分化规则
' ============================================================
' 类器官的核心特征是「一群祖细胞自组织成多种终末细胞类型」。这里把
' 「命运」建模为一个带规则的蓝图别名：
'
'   * 蓝图（CellaBlueprint）提供该命运的基因集、代谢网络、耦合映射，
'     以及几何偏好（PreferredRadius）与标志基因（Markers）；
'   * CellFateDefinition 只额外提供**分化规则**：能从哪些前体命运而来、
'     需要哪些外源生长因子、被谁诱导 / 侧向抑制、基础概率与代数门槛、
'     以及分化代价。
'
' 分化 = 换蓝图（重建六个子网络），物质型状态按名称尽可能继承，
' 因此分化前后细胞的身份（id / 代次 / 谱系）连续，只有表达程序改变。
' ============================================================

Imports Microsoft.VisualBasic.Linq

''' <summary>
''' 一个细胞命运（细胞类型）的分化规则
''' </summary>
Public Class CellFateDefinition

    ''' <summary>命运 id（与 <see cref="CellaBlueprint.SpeciesName"/> 一致）</summary>
    Public Property Id As String

    ''' <summary>该命运的蓝图</summary>
    Public Property Blueprint As CellaBlueprint

    ''' <summary>显示名称</summary>
    Public Property Name As String

    ''' <summary>
    ''' 允许从哪些前体命运分化而来；为空表示只能由外部播种产生
    ''' </summary>
    Public Property Precursors As String()

    ''' <summary>需要达到的最小代数（对应<see cref="VirtualCella.Generation"/>）</summary>
    Public Property MinGeneration As Integer = 1

    ''' <summary>
    ''' 需要的外源生长因子节拍：信号通道 → 需要达到的最小活性（0~1）。
    ''' 任一通道活性为 0 则这一命运本步完全不可能分化出来。
    ''' </summary>
    Public Property RequiredSignals As Dictionary(Of String, Double)

    ''' <summary>被哪些命运诱导：命运 id → 邻域细胞数阈值（达到即饱和诱导）</summary>
    Public Property InducedBy As Dictionary(Of String, Double)

    ''' <summary>被哪些命运侧向抑制：命运 id → 邻域细胞数阈值（达到即饱和抑制）</summary>
    Public Property InhibitedBy As Dictionary(Of String, Double)

    ''' <summary>每步的基础分化概率（会被各种条件进一步缩放）</summary>
    Public Property BaseProbability As Double = 0.02

    ''' <summary>
    ''' 分化代价：分化瞬间保留的生物量比例（0.6 表示消耗掉 40%）
    ''' </summary>
    Public Property BiomassRetention As Double = 0.7

    Public Overrides Function ToString() As String
        Return $"{Id} ({Name})"
    End Function

End Class

''' <summary>
''' 细胞命运目录：按 id 检索命运定义，并按前体关系反查可分化去向
''' </summary>
Public Class FateCatalog

    Private ReadOnly defs As New Dictionary(Of String, CellFateDefinition)(StringComparer.OrdinalIgnoreCase)

    Public Sub Add(def As CellFateDefinition)
        If def Is Nothing OrElse def.Id Is Nothing Then
            Return
        End If

        defs(def.Id) = def
    End Sub

    Public Function Has(id As String) As Boolean
        Return id IsNot Nothing AndAlso defs.ContainsKey(id)
    End Function

    Public Function ById(id As String) As CellFateDefinition
        Dim def As CellFateDefinition = Nothing

        If id IsNot Nothing Then
            Call defs.TryGetValue(id, def)
        End If

        Return def
    End Function

    Public ReadOnly Property Count As Integer
        Get
            Return defs.Count
        End Get
    End Property

    Public ReadOnly Property All As IEnumerable(Of CellFateDefinition)
        Get
            Return defs.Values
        End Get
    End Property

    Public ReadOnly Property Ids As String()
        Get
            Return defs.Keys.ToArray()
        End Get
    End Property

    ''' <summary>只能由外部播种产生的命运（无前体）</summary>
    Public Function RootFates() As String()
        Return defs.Values _
            .Where(Function(d) d.Precursors.IsNullOrEmpty) _
            .Select(Function(d) d.Id) _
            .ToArray()
    End Function

    ''' <summary>从指定前体命运可以直接分化出的所有命运</summary>
    Public Function CandidatesFrom(precursorId As String) As CellFateDefinition()
        If precursorId Is Nothing Then
            Return {}
        End If

        Return defs.Values _
            .Where(Function(d) Not d.Precursors.IsNullOrEmpty AndAlso
                                d.Precursors.Contains(precursorId, StringComparer.OrdinalIgnoreCase)) _
            .ToArray()
    End Function

    ''' <summary>该命运是否为终末分化（没有任何后继）</summary>
    Public Function IsTerminal(fateId As String) As Boolean
        Return CandidatesFrom(fateId).Length = 0
    End Function

End Class
