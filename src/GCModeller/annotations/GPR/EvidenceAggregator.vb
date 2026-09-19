Imports System.Runtime.CompilerServices

''' <summary>
''' 证据聚合器：把多条相互独立的证据合并成一个 [0, 1] 区间内的置信分数。
''' 
''' 采用 noisy-OR 模型：
''' <code>
''' score = 1 - Π(1 - contribution_i)
''' </code>
''' 
''' 该公式满足以下性质，这正是关联打分所需的基本要求：
''' 
''' + **单调性**：增加任意一条证据（contribution &gt; 0）只会让分数升高或不变；
''' + **有界性**：结果永远落在 [0, 1] 区间内，不会因为证据堆叠而溢出；
''' + **可交换性**：与证据的枚举顺序无关，保证结果可复现；
''' + **可解释性**：每一条证据的贡献可以单独取出，用于结果表的"评分依据"列。
''' </summary>
Public Module EvidenceAggregator

    ''' <summary>
    ''' 聚合证据得到最终分数（默认上限为 1.0）
    ''' </summary>
    <Extension>
    Public Function Aggregate(evidences As IEnumerable(Of AssociationEvidence), Optional scoreCap As Double = 1.0) As Double
        If evidences Is Nothing Then Return 0

        Dim remain As Double = 1.0

        For Each item As AssociationEvidence In evidences
            Dim c As Double = item.Contribution

            If c >= 1.0 Then Return Clamp(1.0, scoreCap)

            remain *= (1.0 - c)
        Next

        Return Clamp(1.0 - remain, scoreCap)
    End Function

    ''' <summary>
    ''' 把分数限制到 ``[0, scoreCap]``
    ''' </summary>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function Clamp(score As Double, scoreCap As Double) As Double
        If Double.IsNaN(score) Then Return 0

        Dim top As Double = If(scoreCap <= 0, 1.0, scoreCap)

        If score < 0 Then Return 0
        If score > top Then Return top
        Return score
    End Function

    ''' <summary>
    ''' 生成证据来源的可读摘要，例如 ``DirectEC + OperonContext``，用于结果表的"评分依据"列
    ''' </summary>
    <Extension>
    Public Function Summarize(evidences As IEnumerable(Of AssociationEvidence), Optional maxItems As Integer = 4) As String
        If evidences Is Nothing Then Return ""

        Dim kinds As String() = evidences _
            .OrderByDescending(Function(e) e.Contribution) _
            .Select(Function(e) e.Kind.ToString) _
            .Distinct() _
            .Take(maxItems) _
            .ToArray

        Return String.Join("+", kinds)
    End Function

    ''' <summary>
    ''' 生成证据来源的详细描述（包含来源对象），用于调试与日志输出
    ''' </summary>
    <Extension>
    Public Function Describe(evidences As IEnumerable(Of AssociationEvidence), Optional maxItems As Integer = 3) As String
        If evidences Is Nothing Then Return ""

        Dim top = evidences _
            .OrderByDescending(Function(e) e.Contribution) _
            .Take(maxItems) _
            .Select(Function(e) e.ToString) _
            .ToArray

        Return String.Join("; ", top)
    End Function

End Module
