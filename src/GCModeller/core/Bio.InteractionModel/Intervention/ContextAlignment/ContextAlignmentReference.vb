Imports System.IO
Imports System.Text

Namespace Intervention

    ''' <summary>
    ''' 上下文对齐所需的「训练侧参考统计量」。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 虚拟扰动的上下文（外部 pseudobulk、疾病态、上游扰动结果）与模型训练矩阵往往
    ''' 来自不同的归一化管线，数值尺度和分布形状都不一致。对齐的作用就是把外部 context
    ''' 映射回模型自身的表达空间，否则特征会落到决策树的极值叶子上（“平台化”），
    ''' 或者被线性/高斯模型当成若干倍的浓度差异。
    ''' </para>
    ''' <para>
    ''' 这个类型刻意<b>不持有表达矩阵</b>：<c>SMRUCC.genomics.BioSystem</c> 不能依赖
    ''' <c>analysis/HTS_matrix</c>（<c>GeneExpressionData</c> 定义在那里），因此各模型
    ''' 项目只把对齐真正需要的几组统计量填进来即可。不同的对齐模式所需的字段不同，
    ''' 由 <see cref="Validate(ContextAlignmentMode, String)"/> 负责提前失败并说明缺了什么。
    ''' </para>
    ''' <list type="table">
    ''' <listheader><term>模式</term><description>必需字段</description></listheader>
    ''' <item><term><see cref="ContextAlignmentMode.None"/></term><description>仅 <see cref="GeneNames"/></description></item>
    ''' <item><term><see cref="ContextAlignmentMode.Affine"/></term><description><see cref="Means"/> + <see cref="StandardDeviations"/></description></item>
    ''' <item><term><see cref="ContextAlignmentMode.LogAffine"/></term><description><see cref="PseudoCount"/>（<see cref="Offsets"/> 可选）</description></item>
    ''' <item><term><see cref="ContextAlignmentMode.Quantile"/></term><description><see cref="Population"/></description></item>
    ''' <item><term><see cref="ContextAlignmentMode.QuantileDelta"/></term><description><see cref="Population"/> + <see cref="Means"/></description></item>
    ''' </list>
    ''' </remarks>
    Public Class ContextAlignmentReference

        ''' <summary>模型基因名（下标与其余数组一致）。</summary>
        Public Property GeneNames As String()

        ''' <summary>逐基因的训练均值（模型空间）。</summary>
        ''' <remarks>
        ''' <see cref="ContextAlignmentMode.Affine"/> 用它做中心化；
        ''' <see cref="ContextAlignmentMode.QuantileDelta"/> 用它构造“源分布”。
        ''' </remarks>
        Public Property Means As Double()

        ''' <summary>逐基因的训练标准差（模型空间）。常量基因允许为 0。</summary>
        Public Property StandardDeviations As Double()

        ''' <summary>
        ''' 基因主序的训练种群 <c>Population(gene)(cell)</c>，即每个基因在训练数据中
        ''' 观测到的全部取值。
        ''' </summary>
        ''' <remarks>
        ''' <see cref="ContextAlignmentMode.Quantile"/> 与
        ''' <see cref="ContextAlignmentMode.QuantileDelta"/> 必需。允许包含
        ''' <c>NaN</c>/<c>Inf</c> 以及空行，对齐时会自动过滤（空行会退化为常量映射）。
        ''' </remarks>
        Public Property Population As Double()()

        ''' <summary>
        ''' 逐基因的对数平移常数 δ，语义为 <c>X = log2(max(raw,0) + pseudo) + δ</c>。
        ''' </summary>
        ''' <remarks>
        ''' 只被 <see cref="ContextAlignmentMode.LogAffine"/> 使用；为空时按全 0 处理，
        ''' 长度不足时缺失部分按 0 处理。
        ''' </remarks>
        Public Property Offsets As Double()

        ''' <summary>对数变换使用的伪计数（默认 1），必须为正数。</summary>
        Public Property PseudoCount As Double = 1.0R

        ''' <summary>逐基因的训练支持域下界（与上下文取值同一空间，用于越界诊断）。</summary>
        ''' <remarks>为空时退化为 <see cref="Population"/> 的逐基因最小/最大值。</remarks>
        Public Property SupportMin As Double()

        ''' <summary>逐基因的训练支持域上界（与上下文取值同一空间，用于越界诊断）。</summary>
        Public Property SupportMax As Double()

        Private _index As Dictionary(Of String, Integer)
        Private _sorted As Double()()
        Private ReadOnly _sync As New Object()

        ''' <summary>模型基因数量。</summary>
        Public ReadOnly Property NGene As Integer
            Get
                Return If(GeneNames Is Nothing, 0, GeneNames.Length)
            End Get
        End Property

        ''' <summary>基因名 → 下标（大小写不敏感，重名保留第一个）。</summary>
        Public ReadOnly Property GeneIndex As Dictionary(Of String, Integer)
            Get
                If _index Is Nothing Then
                    Dim map As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

                    If GeneNames IsNot Nothing Then
                        For i As Integer = 0 To GeneNames.Length - 1
                            Dim name As String = GeneNames(i)

                            If String.IsNullOrEmpty(name) Then
                                Continue For
                            End If
                            If Not map.ContainsKey(name) Then
                                map(name) = i
                            End If
                        Next
                    End If

                    _index = map
                End If

                Return _index
            End Get
        End Property

        ''' <summary>
        ''' 逐基因「已升序排序、且已剔除 NaN/Inf」的种群副本，惰性构建并缓存。
        ''' </summary>
        ''' <remarks>
        ''' 排序是 <c>O(N log N)</c>，若每次注入上下文都重排会让高频调用变得昂贵；
        ''' 这里按基因缓存一次，之后每次查询只需 <c>O(log N)</c> 的二分。
        ''' </remarks>
        Public ReadOnly Property SortedPopulation As Double()()
            Get
                If _sorted Is Nothing Then
                    SyncLock _sync
                        If _sorted Is Nothing Then
                            _sorted = buildSortedPopulation()
                        End If
                    End SyncLock
                End If

                Return _sorted
            End Get
        End Property

        Private Function buildSortedPopulation() As Double()()
            If Population Is Nothing Then
                Return Nothing
            End If

            Dim result(Population.Length - 1)() As Double

            For g As Integer = 0 To Population.Length - 1
                Dim values As Double() = Population(g)

                If values Is Nothing OrElse values.Length = 0 Then
                    result(g) = New Double() {}
                    Continue For
                End If

                Dim buffer(values.Length - 1) As Double
                Dim count As Integer = 0

                For i As Integer = 0 To values.Length - 1
                    Dim v As Double = values(i)

                    If Double.IsNaN(v) OrElse Double.IsInfinity(v) Then
                        Continue For
                    End If

                    buffer(count) = v
                    count += 1
                Next

                If count = 0 Then
                    result(g) = New Double() {}
                Else
                    Call Array.Sort(buffer, 0, count)
                    Call Array.Resize(buffer, count)
                    result(g) = buffer
                End If
            Next

            Return result
        End Function

        ''' <summary>是否携带可用的表达种群（<see cref="ContextAlignmentMode.Quantile"/> 的前置条件）。</summary>
        Public ReadOnly Property HasPopulation As Boolean
            Get
                If Population Is Nothing OrElse Population.Length = 0 Then
                    Return False
                End If

                For i As Integer = 0 To Population.Length - 1
                    If Population(i) IsNot Nothing AndAlso Population(i).Length > 0 Then
                        Return True
                    End If
                Next

                Return False
            End Get
        End Property

        ''' <summary>按基因名查下标（大小写不敏感）。</summary>
        Public Function TryGetIndex(name As String, ByRef index As Integer) As Boolean
            If String.IsNullOrEmpty(name) Then
                index = -1
                Return False
            End If

            Return GeneIndex.TryGetValue(name.Trim(), index)
        End Function

        ''' <summary>
        ''' 校验指定对齐模式所需的数据是否齐备；缺失时抛出可操作的异常。
        ''' </summary>
        ''' <param name="mode">即将使用的对齐模式</param>
        ''' <param name="caller">调用方名称（用于异常信息定位）</param>
        ''' <exception cref="InvalidOperationException">缺少该模式必需的数据</exception>
        ''' <remarks>
        ''' 这里刻意选择「直接失败」而不是静默降级为原值直灌：后者正是此前 FuzzyGene / BNLearn
        ''' 尺度错位问题的来源——用户以为对齐了，实际没有。
        ''' </remarks>
        Public Sub Validate(mode As ContextAlignmentMode, Optional caller As String = Nothing)
            Dim prefix As String = If(String.IsNullOrEmpty(caller), "", caller & ": ")

            If NGene = 0 Then
                Throw New InvalidOperationException($"{prefix}模型没有提供任何基因名，无法匹配上下文")
            End If

            Select Case mode
                Case ContextAlignmentMode.None
                    ' 仅做基因名匹配，不需要额外统计量
                Case ContextAlignmentMode.Affine
                    If Means Is Nothing OrElse StandardDeviations Is Nothing Then
                        Throw New InvalidOperationException(
                            $"{prefix}Affine 对齐需要每个基因的训练均值与标准差，但参考统计量没有提供")
                    End If
                    If Means.Length <> NGene OrElse StandardDeviations.Length <> NGene Then
                        Throw New InvalidOperationException(
                            $"{prefix}Affine 对齐的均值/标准差长度({Means.Length}/{StandardDeviations.Length})与基因数({NGene})不一致")
                    End If
                Case ContextAlignmentMode.LogAffine
                    If PseudoCount <= 0.0R OrElse Double.IsNaN(PseudoCount) OrElse Double.IsInfinity(PseudoCount) Then
                        Throw New InvalidOperationException(
                            $"{prefix}LogAffine 对齐的伪计数必须是正数，实际为 {PseudoCount}")
                    End If
                    If Offsets IsNot Nothing AndAlso Offsets.Length < NGene Then
                        Throw New InvalidOperationException(
                            $"{prefix}LogAffine 对齐的逐基因偏移长度({Offsets.Length})小于基因数({NGene})")
                    End If
                Case ContextAlignmentMode.Quantile
                    If Not HasPopulation Then
                        Throw New InvalidOperationException(
                            $"{prefix}Quantile 对齐需要模型携带训练表达种群（Population），" &
                            "请先完成训练或者加载已训练好的模型")
                    End If
                Case ContextAlignmentMode.QuantileDelta
                    If Not HasPopulation Then
                        Throw New InvalidOperationException(
                            $"{prefix}QuantileDelta 对齐需要模型携带训练表达种群（Population）")
                    End If
                    If Means Is Nothing OrElse Means.Length <> NGene Then
                        Throw New InvalidOperationException(
                            $"{prefix}QuantileDelta 对齐需要逐基因训练均值作为源分布，但其缺失或长度与基因数不符")
                    End If
                Case Else
                    Throw New InvalidOperationException($"{prefix}未知的对齐模式：{mode}")
            End Select
        End Sub

        ''' <summary>只提供均值/标准差的便捷构造（适用于 Affine 模式）。</summary>
        Public Shared Function FromStatistics(geneNames As String(),
                                              means As Double(),
                                              sds As Double()) As ContextAlignmentReference
            Return New ContextAlignmentReference With {
                .GeneNames = geneNames,
                .Means = means,
                .StandardDeviations = sds
            }
        End Function

        ''' <summary>诊断摘要。</summary>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()

            Call sb.Append($"{NGene} gene(s)")
            Call sb.Append($", population={(If(HasPopulation, "yes", "no"))}")
            Call sb.Append($", means={(If(Means Is Nothing, "no", "yes"))}")
            Call sb.Append($", support={(If(SupportMin Is Nothing OrElse SupportMax Is Nothing, "no", "yes"))}")

            Return sb.ToString()
        End Function

    End Class

End Namespace
