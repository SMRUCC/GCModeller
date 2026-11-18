Imports System.Text.Json.Serialization
Imports SMRUCC.genomics.BioSystem.Intervention

Namespace Perturbation.Analysis

    ''' <summary>
    ''' ω（阻尼系数）不变性自检的结果。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 同一扰动在若干 ω 上各求解一次，比较它们的<b>终态</b>：如果模型确实收敛到一个
    ''' 不动点，那么不同 ω 只是「走过去的路」不同，终点应当一致；终点不一致就说明
    ''' 迭代没有收敛，此时曲线本身不能当作结论。
    ''' </para>
    ''' <para>
    ''' <see cref="MaxAbsDeviation"/> / <see cref="MaxRelativeDeviation"/> 都是相对
    ''' <see cref="ReferenceOmega"/> 那一次算的，参考自身的偏差恒为 0。
    ''' 相对偏差的分母取「参考终态自身的幅度」，并至少为 1，避免接近 0 时被放大。
    ''' </para>
    ''' </remarks>
    Public Class OmegaSweepResult

        ''' <summary>被扰动的靶基因。</summary>
        Public Property Gene As String

        ''' <summary>干预模式。</summary>
        Public Property Mode As InterventionMode

        ''' <summary>每次求解使用的最大迭代步数。</summary>
        Public Property Steps As Integer

        ''' <summary>作为比较基准的那个 ω。</summary>
        Public Property ReferenceOmega As Double

        ''' <summary>判定「不变」所用的相对偏差阈值。</summary>
        Public Property InvariantTolerance As Double = 0.001

        ''' <summary>被扫描的 ω 列表（升序）。</summary>
        Public Property Omegas As Double()

        ''' <summary>每个 ω 是否收敛。</summary>
        Public Property Converged As Boolean()

        ''' <summary>每个 ω 实际执行的迭代步数。</summary>
        Public Property Iterations As Integer()

        ''' <summary>每个 ω 最后一步的最大变化量（z 空间）。</summary>
        Public Property LastMaxDelta As Double()

        ''' <summary>相对参考 ω 的终态最大绝对偏差。</summary>
        Public Property MaxAbsDeviation As Double()

        ''' <summary>相对参考 ω 的终态最大相对偏差。</summary>
        Public Property MaxRelativeDeviation As Double()

        ''' <summary>是否所有相对偏差都不超过 <see cref="InvariantTolerance"/>。</summary>
        Public Property Invariant As Boolean

        ''' <summary>基因名（与每个终态向量的下标一致）。</summary>
        Public Property GeneNames As String()

        ''' <summary>每个 ω 的终态（原始表达量）。</summary>
        Public Property FinalStates As New List(Of Double())()

        ''' <summary>整轮扫描的墙钟耗时，单位毫秒。</summary>
        Public Property ElapsedMs As Double

        ''' <summary>实际求解次数（含每个 ω 各自的野生型轨迹）。</summary>
        Public Property Runs As Integer

        ''' <summary>形状自洽检查：不一致时抛异常，避免把错位的数据写出去。</summary>
        Friend Sub Validate()
            Dim n As Integer = If(GeneNames Is Nothing, 0, GeneNames.Length)
            Dim runs As Integer = If(Omegas Is Nothing, 0, Omegas.Length)

            If n = 0 Then
                Throw New InvalidOperationException("the sweep does not describe any gene")
            End If
            If runs = 0 Then
                Throw New InvalidOperationException("the sweep does not contain any relaxation run")
            End If

            For Each check As Array In New Array() {Converged, Iterations, LastMaxDelta, MaxAbsDeviation, MaxRelativeDeviation}
                If check Is Nothing OrElse check.Length <> runs Then
                    Throw New InvalidOperationException("the per run arrays of the relaxation sweep are not aligned")
                End If
            Next

            For Each state As Double() In FinalStates
                If state Is Nothing OrElse state.Length <> n Then
                    Throw New InvalidOperationException("a final state of the relaxation sweep does not match the gene count")
                End If
            Next

            If FinalStates.Count <> runs Then
                Throw New InvalidOperationException("the final states of the relaxation sweep are not aligned")
            End If
        End Sub

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"omega sweep: {Gene} over {If(Omegas, New Double() {}).Length} relaxation value(s), " &
                   $"invariant={Invariant} (max relative deviation " &
                   $"{If(MaxRelativeDeviation Is Nothing OrElse MaxRelativeDeviation.Length = 0, 0.0, MaxRelativeDeviation.Max):G3})"
        End Function

    End Class

    ''' <summary>
    ''' 迭代步数不变性的自检结果。
    ''' </summary>
    ''' <remarks>
    ''' 以最长的步数作为参考：如果轨迹真的收敛，多跑几步不应该改变终点；
    ''' 反之说明「跑满步数就被截断」，曲线尾部的平台是假稳态。
    ''' </remarks>
    Public Class StepSweepResult

        ''' <summary>被扰动的靶基因。</summary>
        Public Property Gene As String

        ''' <summary>干预模式。</summary>
        Public Property Mode As InterventionMode

        ''' <summary>本次使用的 ω（<c>NaN</c> 表示取模型配置值）。</summary>
        Public Property Omega As Double

        ''' <summary>作为比较基准的步数（通常是列表里最大的那个）。</summary>
        Public Property ReferenceSteps As Integer

        ''' <summary>判定「不变」所用的相对偏差阈值。</summary>
        Public Property InvariantTolerance As Double = 0.001

        ''' <summary>被扫描的步数列表（升序）。</summary>
        Public Property StepsList As Integer()

        ''' <summary>每个步数实际执行的迭代次数（可能因提前收敛而少于请求值）。</summary>
        Public Property ActualIterations As Integer()

        ''' <summary>每个步数是否收敛。</summary>
        Public Property Converged As Boolean()

        ''' <summary>每个步数最后一步的最大变化量。</summary>
        Public Property LastMaxDelta As Double()

        ''' <summary>相对参考步数的终态最大绝对偏差。</summary>
        Public Property MaxAbsDeviation As Double()

        ''' <summary>相对参考步数的终态最大相对偏差。</summary>
        Public Property MaxRelativeDeviation As Double()

        ''' <summary>是否所有相对偏差都不超过 <see cref="InvariantTolerance"/>。</summary>
        Public Property Invariant As Boolean

        ''' <summary>基因名。</summary>
        Public Property GeneNames As String()

        ''' <summary>每个步数下的终态（原始表达量）。</summary>
        Public Property FinalStates As New List(Of Double())()

        ''' <summary>整轮扫描的墙钟耗时，单位毫秒。</summary>
        Public Property ElapsedMs As Double

        ''' <summary>实际求解次数（含野生型轨迹）。</summary>
        Public Property Runs As Integer

        ''' <summary>形状自洽检查。</summary>
        Friend Sub Validate()
            Dim n As Integer = If(GeneNames Is Nothing, 0, GeneNames.Length)
            Dim runs As Integer = If(StepsList Is Nothing, 0, StepsList.Length)

            If n = 0 Then
                Throw New InvalidOperationException("the step sweep does not describe any gene")
            End If
            If runs = 0 Then
                Throw New InvalidOperationException("the step sweep does not contain any run")
            End If

            For Each check As Array In New Array() {ActualIterations, Converged, LastMaxDelta, MaxAbsDeviation, MaxRelativeDeviation}
                If check Is Nothing OrElse check.Length <> runs Then
                    Throw New InvalidOperationException("the per run arrays of the step sweep are not aligned")
                End If
            Next

            If FinalStates.Count <> runs Then
                Throw New InvalidOperationException("the final states of the step sweep are not aligned")
            End If

            For Each state As Double() In FinalStates
                If state Is Nothing OrElse state.Length <> n Then
                    Throw New InvalidOperationException("a final state of the step sweep does not match the gene count")
                End If
            Next
        End Sub

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"step sweep: {Gene} over {If(StepsList, New Integer() {}).Length} step count(s), invariant={Invariant}"
        End Function

    End Class

    ''' <summary>
    ''' lag（首次响应帧号）排序的跨 ω 稳健性。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 对每个 ω 计算每个基因「首次偏离野生型并达到自身终态变化量
    ''' <see cref="ThresholdRatio"/> 倍」的帧号，然后跨 ω 两两计算 Spearman 秩相关。
    ''' </para>
    ''' <para>
    ''' 只保留<b>在所有 ω 下都响应</b>的基因：否则秩相关的分母会随 ω 变化，
    ''' 不同 ω 之间的 rho 根本不可比。
    ''' </para>
    ''' <para>
    ''' lag 是整数且并列极多，因此秩必须用平均秩，否则 rho 会系统性地虚高。
    ''' </para>
    ''' </remarks>
    Public Class LagRobustnessResult

        ''' <summary>被扰动的靶基因（不参与 lag 统计）。</summary>
        Public Property Gene As String

        ''' <summary>干预模式。</summary>
        Public Property Mode As InterventionMode

        ''' <summary>每次求解的迭代步数。</summary>
        Public Property Steps As Integer

        ''' <summary>触发阈值占终态变化量的比例。</summary>
        Public Property ThresholdRatio As Double = 0.1

        ''' <summary>被扫描的 ω 列表。</summary>
        Public Property Omegas As Double()

        ''' <summary>参与统计的基因（所有 ω 下都响应的那些）。</summary>
        Public Property GeneNames As String()

        ''' <summary>参与统计的基因在整张网络里的下标。</summary>
        Public Property GeneIndices As Integer()

        ''' <summary>原始阈值（每个基因自身终态变化量的 <see cref="ThresholdRatio"/> 倍）。</summary>
        Public Property Thresholds As Double()

        ''' <summary>lag 矩阵 <c>[omega, gene]</c>，单位是帧号（从 1 开始）。</summary>
        <JsonIgnore>
        Public Property Lag As Double(,)

        ''' <summary>Spearman 秩相关矩阵 <c>[omega, omega]</c>。</summary>
        <JsonIgnore>
        Public Property Spearman As Double(,)

        ''' <summary>计算每个 rho 时实际参与比较的基因数。</summary>
        <JsonIgnore>
        Public Property SpearmanN As Integer(,)

        ''' <summary>所有 rho 的平均值（不含对角线）。</summary>
        Public Property MeanRho As Double

        ''' <summary>整轮分析的墙钟耗时，单位毫秒。</summary>
        Public Property ElapsedMs As Double

        ''' <summary>实际求解次数（每个 ω 一次突变 + 一次野生型）。</summary>
        Public Property Runs As Integer

        ''' <summary>形状自洽检查。</summary>
        Friend Sub Validate()
            Dim n As Integer = If(GeneNames Is Nothing, 0, GeneNames.Length)
            Dim runs As Integer = If(Omegas Is Nothing, 0, Omegas.Length)

            If runs = 0 Then
                Throw New InvalidOperationException("the lag analysis does not contain any relaxation run")
            End If
            If GeneIndices Is Nothing OrElse GeneIndices.Length <> n Then
                Throw New InvalidOperationException("the lag analysis gene indices are not aligned")
            End If
            If Thresholds Is Nothing OrElse Thresholds.Length <> n Then
                Throw New InvalidOperationException("the lag analysis thresholds are not aligned")
            End If
            If Lag Is Nothing OrElse Lag.GetLength(0) <> runs OrElse Lag.GetLength(1) <> n Then
                Throw New InvalidOperationException("the lag matrix shape does not match [omega, gene]")
            End If
            If Spearman Is Nothing OrElse Spearman.GetLength(0) <> runs OrElse Spearman.GetLength(1) <> runs Then
                Throw New InvalidOperationException("the spearman matrix is not [omega, omega]")
            End If
        End Sub

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"lag robustness: {Gene}, {If(Omegas, New Double() {}).Length} omega value(s), " &
                   $"{If(GeneNames, New String() {}).Length} responding gene(s), mean rho={MeanRho:G3}"
        End Function

    End Class

    ''' <summary>
    ''' 剂量-响应扫描的结果。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 剂量 <c>d ∈ [0, 1]</c>，钳制值 = 基线 × (1 − d)：<c>d = 0</c> 把基因钉在基线，
    ''' <c>d = 1</c> 是完全敲除。
    ''' </para>
    ''' <para>
    ''' 注意 <c>d = 0</c> <b>不等于</b>「没有扰动」：基因被钉住不动，而野生型轨迹会从
    ''' 基线自由漂走，所以此时它的 fold change 恰好为 0（被钉在基线），
    ''' 但其他基因仍可能因为系统漂移而变化。
    ''' </para>
    ''' </remarks>
    Public Class DoseResponseResult

        ''' <summary>被扰动的靶基因。</summary>
        Public Property Gene As String

        ''' <summary>实际使用的目标基因下标。</summary>
        Public Property GeneIndex As Integer

        ''' <summary>每次求解的迭代步数。</summary>
        Public Property Steps As Integer

        ''' <summary>本次使用的 ω（<c>NaN</c> 表示取配置值）。</summary>
        Public Property Omega As Double

        ''' <summary>被扫描的剂量点（升序）。</summary>
        Public Property Doses As Double()

        ''' <summary>与剂量点一一对应的钳制值（原始表达量）。</summary>
        Public Property ClampValues As Double()

        ''' <summary>基因名。</summary>
        Public Property GeneNames As String()

        ''' <summary>fold change 矩阵 <c>[dose, gene]</c>（突变终态 − 对照基线）。</summary>
        <JsonIgnore>
        Public Property FoldChanges As Double(,)

        ''' <summary>百分比变化矩阵 <c>[dose, gene]</c>。</summary>
        <JsonIgnore>
        Public Property PercentChanges As Double(,)

        ''' <summary>Z 分数矩阵 <c>[dose, gene]</c>。</summary>
        <JsonIgnore>
        Public Property ZScores As Double(,)

        ''' <summary>显著性矩阵 <c>[dose, gene]</c>。</summary>
        <JsonIgnore>
        Public Property Significant As Boolean(,)

        ''' <summary>按最大剂量下的 |Z| 排序取前 K 个基因的下标。</summary>
        Public Property TopGenes As Integer()

        ''' <summary>整轮扫描的墙钟耗时，单位毫秒。</summary>
        Public Property ElapsedMs As Double

        ''' <summary>实际求解次数。</summary>
        Public Property Runs As Integer

        ''' <summary>形状自洽检查。</summary>
        Friend Sub Validate()
            Dim n As Integer = If(GeneNames Is Nothing, 0, GeneNames.Length)
            ' 局部变量不能叫 doses —— VB 大小写不敏感，会遮蔽 Doses 属性，
            ' 使下面的 If(Doses, ...) 自引用成 Integer
            Dim nDoses As Integer = If(Doses, New Double() {}).Length

            If n = 0 Then
                Throw New InvalidOperationException("the dose response does not describe any gene")
            End If
            If nDoses = 0 Then
                Throw New InvalidOperationException("the dose response does not contain any dose point")
            End If
            If ClampValues Is Nothing OrElse ClampValues.Length <> nDoses Then
                Throw New InvalidOperationException("the clamp values are not aligned with the dose points")
            End If
            If FoldChanges Is Nothing OrElse FoldChanges.GetLength(0) <> nDoses OrElse FoldChanges.GetLength(1) <> n Then
                Throw New InvalidOperationException("the fold change matrix is not [dose, gene]")
            End If
            If ZScores Is Nothing OrElse ZScores.GetLength(0) <> nDoses OrElse ZScores.GetLength(1) <> n Then
                Throw New InvalidOperationException("the z score matrix is not [dose, gene]")
            End If
            If PercentChanges Is Nothing OrElse PercentChanges.GetLength(0) <> nDoses OrElse PercentChanges.GetLength(1) <> n Then
                Throw New InvalidOperationException("the percent change matrix is not [dose, gene]")
            End If
            If Significant Is Nothing OrElse Significant.GetLength(0) <> nDoses OrElse Significant.GetLength(1) <> n Then
                Throw New InvalidOperationException("the significance matrix is not [dose, gene]")
            End If
        End Sub

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"dose response: {Gene} over {If(Doses, New Double() {}).Length} dose point(s), " &
                   $"{If(GeneNames, New String() {}).Length} gene(s), topK={If(TopGenes, New Integer() {}).Length}"
        End Function

    End Class

    ''' <summary>
    ''' 剂量 × 迭代步的二维响应曲面（只保留 Top-K 基因）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <see cref="Deltas"/> 的元素是「该剂量的突变轨迹在那一帧相对于野生型轨迹
    ''' 同一帧的变化量」，即 <c>mutant(t) − control(t)</c>，而不是相对基线的绝对变化。
    ''' 这样曲面里剩下的才是扰动效应，系统自身的漂移被减掉了。
    ''' </para>
    ''' <para>
    ''' 完整网格是 <c>剂量数 × 帧数 × 全部基因</c>，对 1.8 万基因来说太大，
    ''' 因此只保留按「最大剂量下的 |Z|」排序的前 K 个基因。
    ''' </para>
    ''' </remarks>
    Public Class DoseTimeSurfaceResult

        ''' <summary>被扰动的靶基因。</summary>
        Public Property Gene As String

        ''' <summary>每次求解的迭代步数。</summary>
        Public Property Steps As Integer

        ''' <summary>本次使用的 ω（<c>NaN</c> 表示取配置值）。</summary>
        Public Property Omega As Double

        ''' <summary>被扫描的剂量点（升序）。</summary>
        Public Property Doses As Double()

        ''' <summary>与剂量点一一对应的钳制值。</summary>
        Public Property ClampValues As Double()

        ''' <summary>保留的 Top-K 基因名。</summary>
        Public Property GeneNames As String()

        ''' <summary>保留的 Top-K 基因在整张网络里的下标。</summary>
        Public Property GeneIndices As Integer()

        ''' <summary>变化量立方体 <c>[dose, frame, gene]</c>；帧号 0 是全零（起点两者相同）。</summary>
        <JsonIgnore>
        Public Property Deltas As Double(,,)

        ''' <summary>帧数（= 迭代步数 + 1）。</summary>
        Public Property NFrames As Integer

        ''' <summary>整轮扫描的墙钟耗时，单位毫秒。</summary>
        Public Property ElapsedMs As Double

        ''' <summary>实际求解次数。</summary>
        Public Property Runs As Integer

        ''' <summary>形状自洽检查。</summary>
        Friend Sub Validate()
            Dim k As Integer = If(GeneNames Is Nothing, 0, GeneNames.Length)

            If k = 0 Then
                Throw New InvalidOperationException("the dose time surface does not contain any gene")
            End If
            If GeneIndices Is Nothing OrElse GeneIndices.Length <> k Then
                Throw New InvalidOperationException("the dose time surface gene indices are not aligned")
            End If
            If NFrames = 0 Then
                Throw New InvalidOperationException("the dose time surface does not contain any frame")
            End If
            If Deltas Is Nothing OrElse
               Deltas.GetLength(0) <> If(Doses, New Double() {}).Length OrElse
               Deltas.GetLength(1) <> NFrames OrElse
               Deltas.GetLength(2) <> k Then
                Throw New InvalidOperationException("the surface cube is not [dose, frame, gene]")
            End If
        End Sub

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"dose time surface: {Gene}, " &
                   $"{If(Doses, New Double() {}).Length} dose x {NFrames} frame x {If(GeneNames, New String() {}).Length} gene(s)"
        End Function

    End Class

    ''' <summary>
    ''' 组合扰动的非加性分析（终态与逐帧两个口径）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 加性期望 = <c>control + (A − control) + (B − control)</c>；
    ''' 非加性 = <c>joint − additive</c>：正值表示协同（联合比"各自之和"更强），
    ''' 负值表示拮抗。
    ''' </para>
    ''' <para>
    ''' 逐帧口径用同样的公式在每一帧上计算，能看出联合扰动的偏差是「同步出现」
    ''' 还是「延迟出现」——这是终态口径看不到的信息。
    ''' </para>
    ''' </remarks>
    Public Class CombinationTraceResult

        ''' <summary>第一个靶基因。</summary>
        Public Property GeneA As String

        ''' <summary>第二个靶基因。</summary>
        Public Property GeneB As String

        ''' <summary>干预模式。</summary>
        Public Property Mode As InterventionMode

        ''' <summary>每次求解的迭代步数。</summary>
        Public Property Steps As Integer

        ''' <summary>本次使用的 ω（<c>NaN</c> 表示取配置值）。</summary>
        Public Property Omega As Double

        ''' <summary>基因名。</summary>
        Public Property GeneNames As String()

        ''' <summary>单独敲 A 的终态（原始表达量）。</summary>
        Public Property SingleAMeans As Double()

        ''' <summary>单独敲 B 的终态。</summary>
        Public Property SingleBMeans As Double()

        ''' <summary>联合敲 A+B 的终态。</summary>
        Public Property JointMeans As Double()

        ''' <summary>加性期望（对照 + 两个单扰动效应之和）。</summary>
        Public Property AdditiveMeans As Double()

        ''' <summary>非加性效应（联合 − 加性期望）。</summary>
        Public Property NonAdditive As Double()

        ''' <summary>按 |非加性| 排序取前 K 个基因的下标。</summary>
        Public Property TopGenes As Integer()

        ''' <summary>非加性效应的最大绝对值。</summary>
        Public Property MaxAbsNonAdditive As Double

        ''' <summary>逐帧口径的非加性（只保留 Top-K 基因）。</summary>
        Public Property Trajectory As CombinationTrajectory

        ''' <summary>整轮分析的墙钟耗时，单位毫秒。</summary>
        Public Property ElapsedMs As Double

        ''' <summary>实际求解次数（对照 + A + B + A+B 各自的轨迹）。</summary>
        Public Property Runs As Integer

        ''' <summary>形状自洽检查。</summary>
        Friend Sub Validate()
            Dim n As Integer = If(GeneNames Is Nothing, 0, GeneNames.Length)

            If n = 0 Then
                Throw New InvalidOperationException("the combination analysis does not describe any gene")
            End If

            For Each vector As Double() In New Double()() {SingleAMeans, SingleBMeans, JointMeans, AdditiveMeans, NonAdditive}
                If vector Is Nothing OrElse vector.Length <> n Then
                    Throw New InvalidOperationException("the combination vectors are not aligned with the gene list")
                End If
            Next

            If Trajectory IsNot Nothing Then
                Call Trajectory.Validate()
            End If
        End Sub

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"combination trace: {GeneA}+{GeneB}, max |non additive| = {MaxAbsNonAdditive:G3}"
        End Function

    End Class

    ''' <summary>
    ''' 组合扰动的逐帧口径（只保留 Top-K 基因，避免立方体过大）。
    ''' </summary>
    Public Class CombinationTrajectory

        ''' <summary>保留的 Top-K 基因名。</summary>
        Public Property GeneNames As String()

        ''' <summary>帧数。</summary>
        Public Property NFrames As Integer

        ''' <summary>加性期望轨迹 <c>[frame, gene]</c>。</summary>
        <JsonIgnore>
        Public Property Additive As Double(,)

        ''' <summary>联合轨迹 <c>[frame, gene]</c>。</summary>
        <JsonIgnore>
        Public Property Joint As Double(,)

        ''' <summary>逐帧非加性 <c>[frame, gene]</c>。</summary>
        <JsonIgnore>
        Public Property NonAdditive As Double(,)

        ''' <summary>形状自洽检查。</summary>
        Friend Sub Validate()
            Dim k As Integer = If(GeneNames Is Nothing, 0, GeneNames.Length)

            If k = 0 Then
                Throw New InvalidOperationException("the combination trajectory does not contain any gene")
            End If
            If NFrames = 0 Then
                Throw New InvalidOperationException("the combination trajectory does not contain any frame")
            End If

            Call CheckCube(Additive, k, "additive")
            Call CheckCube(Joint, k, "joint")
            Call CheckCube(NonAdditive, k, "non additive")
        End Sub

        ''' <summary>单个立方体的形状检查（<paramref name="genes"/> 是基因维长度）。</summary>
        Private Sub CheckCube(cube As Double(,), genes As Integer, name As String)
            If cube Is Nothing OrElse cube.GetLength(0) <> NFrames OrElse cube.GetLength(1) <> genes Then
                Throw New InvalidOperationException($"the combination trajectory cube '{name}' is not [frame, gene]")
            End If
        End Sub

    End Class

End Namespace
