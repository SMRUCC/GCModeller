Namespace Placement

    ''' <summary>
    ''' 一个扰动的合成作业：目标总量、细胞文库大小、放置目标与方向约束。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 作业是"放置层"的产物、合成层的输入：它把设计层的<b>逐基因总量</b>与
    ''' 放置层的<b>逐基因目标平均 CPM</b>、<b>文库大小分布</b>、<b>方向/优先级</b>打包在一起。
    ''' </para>
    ''' <para>
    ''' <see cref="Seed"/> 由扰动名派生（见 <c>PerturbationSeed</c>），因此同一扰动在任何并行度下
    ''' 都能得到同一条随机流——这是"同 seed 两次运行计数逐元素相等"的基础。
    ''' </para>
    ''' </remarks>
    Public Class SynthesisJob

        ''' <summary>扰动名（用于诊断与种子派生）。</summary>
        Public Property Name As String

        ''' <summary>逐基因的目标总计数（长度 G，非负整数）。</summary>
        Public Property Totals As Double()

        ''' <summary>每个目标细胞的文库大小（长度 n_cells）。</summary>
        Public Property Libraries As Double()

        ''' <summary>逐基因的目标平均 CPM（长度 G）；≤0 表示该基因不做放置约束。</summary>
        Public Property Targets As Double()

        ''' <summary>计划内的响应基因下标（按优先级）。</summary>
        Public Property PlanIndex As Integer() = {}

        ''' <summary>计划基因的方向。</summary>
        Public Property PlanSign As Double() = {}

        ''' <summary>计划基因的优先级（1 = 最高）。</summary>
        Public Property PlanPriority As Double() = {}

        ''' <summary>逐基因的微调方向。</summary>
        Public Property NudgeSign As SByte()

        ''' <summary>逐基因的微调强度（0~1）。</summary>
        Public Property NudgeStrength As Double()

        ''' <summary>该扰动的随机种子。</summary>
        Public Property Seed As Long

        ''' <summary>细胞数。</summary>
        Public ReadOnly Property CellCount As Integer
            Get
                Return If(Libraries Is Nothing, 0, Libraries.Length)
            End Get
        End Property

        ''' <summary>基因数。</summary>
        Public ReadOnly Property GeneCount As Integer
            Get
                Return If(Totals Is Nothing, 0, Totals.Length)
            End Get
        End Property

        ''' <summary>校验形状与守恒（<c>ΣLt = ΣT</c>）。</summary>
        Public Sub Validate()
            If Totals Is Nothing OrElse Libraries Is Nothing Then
                Throw New ArgumentException($"job '{Name}' is incomplete", NameOf(Totals))
            End If

            Dim libraryTotal As Double = 0.0
            Dim geneTotal As Double = 0.0

            For i As Integer = 0 To Libraries.Length - 1
                libraryTotal += Libraries(i)
            Next

            For i As Integer = 0 To Totals.Length - 1
                geneTotal += Totals(i)
            Next

            If Math.Abs(libraryTotal - geneTotal) > 1.0 Then
                Throw New ArgumentException(
                    $"job '{Name}': the target libraries sum {libraryTotal:F3} but the gene totals sum {geneTotal:F3}",
                    NameOf(Libraries))
            End If
        End Sub

        Public Overrides Function ToString() As String
            Return $"synthesis job '{Name}': {CellCount} cell x {GeneCount} gene"
        End Function

    End Class
End Namespace
