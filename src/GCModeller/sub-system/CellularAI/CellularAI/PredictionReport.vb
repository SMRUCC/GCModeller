Imports Microsoft.VisualBasic.ComponentModel.Collection

''' <summary>单项门控的结果。</summary>
Public Class GateResult

    ''' <summary>门控名称。</summary>
    Public Property Name As String

    ''' <summary>是否通过。</summary>
    Public Property Passed As Boolean

    ''' <summary>说明（失败时给出实际值与阈值）。</summary>
    Public Property Detail As String

End Class

''' <summary>
''' 一个上下文的预测报告：结构校验、设计诊断、评分与门控结论。
''' </summary>
''' <remarks>
''' <para>
''' 报告是"可直接被上游写进 JSON"的纯数据对象（没有文件 IO）。
''' <see cref="Accepted"/> 为 <c>False</c> 时 <see cref="Gates"/> 里必然有失败项——
''' 门控的设计原则是"宁可拒绝，也不要交出一个自己都不信的结果"。
''' </para>
''' </remarks>
Public Class PredictionReport

    ''' <summary>上下文名称。</summary>
    Public Property Context As String

    ''' <summary>是否通过全部门控。</summary>
    Public Property Accepted As Boolean

    ''' <summary>逐项门控结果。</summary>
    Public Property Gates As GateResult()

    ''' <summary>扰动名（与预测矩阵的行块顺序一致）。</summary>
    Public Property Perturbations As String()

    ''' <summary>每个扰动的细胞数。</summary>
    Public Property NCells As Integer

    ''' <summary>对照深度（中位文库大小）。</summary>
    Public Property Depth As Integer

    ''' <summary>基因数。</summary>
    Public Property GeneCount As Integer

    ''' <summary>控制侧信用（<c>P × mean(C_p)</c>）。</summary>
    Public Property Credit As Double

    ''' <summary>最终边距。</summary>
    Public Property Margin As Double

    ''' <summary>边距是否命中。</summary>
    Public Property MarginReached As Boolean

    ''' <summary>信用目标的固定点是否收敛。</summary>
    Public Property Converged As Boolean

    ''' <summary>实测离散度 / 信用目标。</summary>
    Public Property CreditShare As Double

    ''' <summary>幅度设计的幅度。</summary>
    Public Property Amplitude As Double

    ''' <summary>VCC 六项原始指标（提供参考侧时才有值）。</summary>
    Public Property Scores As Dictionary(Of String, Double)

    ''' <summary>锚点缩放后的总分（0~100）。</summary>
    Public Property Total As Double

    ''' <summary>完全没有证据的扰动。</summary>
    Public Property Unmeasured As String()

    ''' <summary>护栏计数快照。</summary>
    Public Property Guards As Dictionary(Of String, Double())

    Public Overrides Function ToString() As String
        Return $"{Context}: accepted={Accepted}, margin={Margin:G4}, credit={Credit:G4}, total={Total:F1}"
    End Function

End Class
