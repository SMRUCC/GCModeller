Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.GPS.Config
Imports SMRUCC.genomics.Analysis.GPS.Data
Imports SMRUCC.genomics.Analysis.GPS.Model
Imports std = System.Math

Namespace Perturbation

    ''' <summary>
    ''' 组合扰动的非加性效应分析结果（method.md §6.3）。
    ''' </summary>
    Public Class CombinationResult

        ''' <summary>联合扰动的完整结果</summary>
        Public Property Joint As InterventionResult

        ''' <summary>Bliss 独立性假设下的加性期望 <c>[基因]</c>（raw log-CPM 尺度）</summary>
        Public Property Additive As Double()

        ''' <summary>非加性效应 <c>joint − additive</c> <c>[基因]</c></summary>
        Public Property Synergy As Double()

        ''' <summary>非加性效应的最大绝对值</summary>
        Public Property SynergyScore As Double

        ''' <summary>非加性效应最强的基因（按 |synergy| 降序）</summary>
        Public Property TopSynergyGenes As List(Of (Gene As String, Effect As Double))

    End Class

    ''' <summary>
    ''' 虚拟扰动引擎（method.md §6）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 三类扰动都通过改写<b>输入窗口</b>实现（表达式扰动）：
    ''' 把目标基因在窗口内所有时间步的标准化取值改写为干预值对应的标准化值，
    ''' 再前向传播得到扰动后的全基因组预测。这与"删除调控边"的网络扰动相比
    ''' 更贴近 CRISPR 实验的语义（蛋白功能缺失 → 转录本被清除），
    ''' 且不需要模型暴露可学习的邻接矩阵。
    ''' </para>
    ''' <para>
    ''' 扰动效应定义为 <c>DE = ŷ_pert − ŷ_ctrl</c>：两者都由同一个模型预测，
    ''' 因而模型自身的系统偏差在差分中抵消，<c>DE</c> 只反映扰动带来的变化。
    ''' 这也意味着 <see cref="InterventionResult.WildtypeMeans"/> 存放的是
    ''' <b>模型给出的对照预测</b>（不是实测均值）；实测均值可由调用方另行对照。
    ''' </para>
    ''' <para>
    ''' 全部推理在双精度下进行，且不启用 dropout，保证同一输入多次调用结果一致。
    ''' </para>
    ''' </remarks>
    Public Class VirtualPerturbationEngine

        Private ReadOnly model As GRNModel
        Private ReadOnly expression As PreprocessedExpression
        Private ReadOnly baselineWindow As Double(,)
        Private ReadOnly geneIndex As Dictionary(Of String, Integer)
        Private ReadOnly significanceZ As Double

        Private _control As Double()

        ''' <summary>基因名称</summary>
        Public ReadOnly Property GeneNames As String()
            Get
                Return expression.GeneNames
            End Get
        End Property

        ''' <summary>基因数量</summary>
        Public ReadOnly Property NumGenes As Integer
            Get
                Return expression.NGene
            End Get
        End Property

        ''' <summary>
        ''' 创建扰动引擎。
        ''' </summary>
        ''' <param name="model">已训练的模型</param>
        ''' <param name="expression">预处理后的表达数据（提供均值/标准差用于反标准化）</param>
        ''' <param name="baselineWindow">对照窗口 <c>[基因, w]</c>（标准化空间）</param>
        ''' <param name="significanceZ">显著性判据阈值</param>
        Public Sub New(model As GRNModel,
                       expression As PreprocessedExpression,
                       baselineWindow As Double(,),
                       Optional significanceZ As Double = 0.5)

            If model Is Nothing Then Throw New ArgumentNullException(NameOf(model))
            If expression Is Nothing Then Throw New ArgumentNullException(NameOf(expression))
            If baselineWindow Is Nothing Then Throw New ArgumentNullException(NameOf(baselineWindow))

            Me.model = model
            Me.expression = expression
            Me.baselineWindow = CType(baselineWindow.Clone(), Double(,))
            Me.significanceZ = significanceZ

            Me.geneIndex = New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To expression.GeneNames.Length - 1
                Me.geneIndex(expression.GeneNames(i)) = i
            Next
        End Sub

        ''' <summary>对照（未扰动）预测，raw log-CPM 尺度。</summary>
        ''' <returns>每个基因的对照预测</returns>
        Public Function ControlPrediction() As Double()
            If _control Is Nothing Then
                _control = PredictRaw(baselineWindow)
            End If

            Return _control
        End Function

        ''' <summary>
        ''' 预测（组合）扰动后的全基因组响应。
        ''' </summary>
        ''' <param name="specs">干预定义集合；多个元素表示组合扰动</param>
        ''' <returns>标准干预结果</returns>
        Public Function Predict(specs As IEnumerable(Of InterventionSpec)) As InterventionResult
            Dim list As List(Of InterventionSpec) = If(specs, Enumerable.Empty(Of InterventionSpec)()).ToList()

            If list.Count = 0 Then
                Throw New ArgumentException("至少需要一个干预定义", NameOf(specs))
            End If

            Dim n As Integer = NumGenes
            Dim indices As New List(Of Integer)()
            Dim names As New List(Of String)()

            For Each spec As InterventionSpec In list
                Dim idx As Integer = -1

                If spec.GeneIndex >= 0 AndAlso spec.GeneIndex < n AndAlso
                   String.Equals(expression.GeneNames(spec.GeneIndex), spec.GeneName, StringComparison.OrdinalIgnoreCase) Then
                    idx = spec.GeneIndex
                Else
                    ' 注意：TryGetValue 失败时会把 ByRef 参数写成默认值 0，
                    ' 因此必须先用临时变量接收，否则未知基因会被误当作 0 号基因
                    Dim resolved As Integer

                    If geneIndex.TryGetValue(spec.GeneName, resolved) Then
                        idx = resolved
                    End If
                End If

                If idx >= 0 AndAlso Not indices.Contains(idx) Then
                    Call indices.Add(idx)
                    Call names.Add(expression.GeneNames(idx))
                End If
            Next

            If indices.Count = 0 Then
                Return CreateUndefinedResult(list(0))
            End If

            ' ---- 构造扰动后的输入窗口 ----
            Dim window As Double(,) = CType(baselineWindow.Clone(), Double(,))
            Dim perturbedSet As New HashSet(Of Integer)(indices)
            Dim interventionRaw As Double() = New Double(indices.Count - 1) {}

            For k As Integer = 0 To indices.Count - 1
                Dim i As Integer = indices(k)
                Dim sd As Double = std.Max(expression.GeneSDs(i), 1.0E-08)
                Dim raw As Double = list(k).GetInterventionValue(expression.GeneMeans(i), expression.GeneSDs(i))
                Dim normalized As Double = (raw - expression.GeneMeans(i)) / sd

                interventionRaw(k) = raw

                For t As Integer = 0 To window.GetLength(1) - 1
                    window(i, t) = normalized
                Next
            Next

            Dim control As Double() = ControlPrediction()
            Dim mutant As Double() = PredictRaw(window)

            ' 被扰动基因的表达由干预值直接决定，不叠加网络预测
            ' （与 GEARS 的处理一致：敲除/敲低的表型是实验施加的结果，不是模型的推断）
            For k As Integer = 0 To indices.Count - 1
                mutant(indices(k)) = std.Max(0.0, interventionRaw(k))
            Next
            Dim result As New InterventionResult With {
                .Spec = If(list.Count = 1, list(0), CombineSpec(list, names, indices)),
                .WildtypeMeans = CType(control.Clone(), Double()),
                .WildtypeSDs = CType(expression.GeneSDs.Clone(), Double()),
                .MutantMeans = mutant,
                .GeneNames = CType(expression.GeneNames.Clone(), String()),
                .FoldChanges = New Double(n - 1) {},
                .PercentChanges = New Double(n - 1) {},
                .ZScores = New Double(n - 1) {},
                .IsSignificant = New Boolean(n - 1) {}
            }

            For i As Integer = 0 To n - 1
                Dim delta As Double = mutant(i) - control(i)

                result.FoldChanges(i) = delta

                If std.Abs(control(i)) > 1.0E-10 Then
                    result.PercentChanges(i) = delta / std.Abs(control(i)) * 100.0
                End If

                Dim sd As Double = std.Max(expression.GeneSDs(i), 1.0E-08)

                result.ZScores(i) = delta / sd
                result.IsSignificant(i) = perturbedSet.Contains(i) OrElse
                                          std.Abs(result.ZScores(i)) > significanceZ
            Next

            Return result
        End Function

        ''' <summary>
        ''' 组合扰动的非加性（协同/拮抗）分析。
        ''' </summary>
        ''' <param name="specs">组合干预定义</param>
        ''' <returns>联合结果、加性期望与非加性效应</returns>
        Public Function PredictCombination(specs As List(Of InterventionSpec)) As CombinationResult
            If specs Is Nothing OrElse specs.Count < 2 Then
                Throw New ArgumentException("组合扰动至少需要两个基因", NameOf(specs))
            End If

            Dim joint As InterventionResult = Predict(specs)
            Dim n As Integer = NumGenes
            Dim control As Double() = ControlPrediction()
            Dim additive As Double() = CType(control.Clone(), Double())

            ' 加性期望 = 对照 + Σ(单基因扰动效应)
            For Each spec As InterventionSpec In specs
                Dim singleResult As InterventionResult = Predict(New InterventionSpec() {spec})

                For i As Integer = 0 To n - 1
                    additive(i) += singleResult.MutantMeans(i) - control(i)
                Next
            Next

            Dim synergy As Double() = New Double(n - 1) {}
            Dim score As Double = 0.0

            For i As Integer = 0 To n - 1
                synergy(i) = joint.MutantMeans(i) - additive(i)
                score = std.Max(score, std.Abs(synergy(i)))
            Next

            Dim top As List(Of (Gene As String, Effect As Double)) =
                Enumerable.Range(0, n).
                    OrderByDescending(Function(i) std.Abs(synergy(i))).
                    Take(20).
                    Select(Function(i) (Gene:=expression.GeneNames(i), Effect:=synergy(i))).
                    ToList()

            Return New CombinationResult With {
                .Joint = joint,
                .Additive = additive,
                .Synergy = synergy,
                .SynergyScore = score,
                .TopSynergyGenes = top
            }
        End Function

        ''' <summary>
        ''' 多步递归级联模拟：扰动效应随时间推移的传播轨迹（method.md §6.3）。
        ''' </summary>
        ''' <param name="specs">干预定义</param>
        ''' <param name="steps">前向推进的时间步数</param>
        ''' <returns>
        ''' 轨迹 <c>[时间步, 基因]</c>，每个元素是长度为 1 的样本数组
        ''' （写入 <see cref="InterventionResult.DynamicTrajectory"/>）；
        ''' 第 0 步为对照预测
        ''' </returns>
        Public Function SimulateCascade(specs As IEnumerable(Of InterventionSpec), steps As Integer) As Double(,)()
            If steps < 1 Then steps = 1

            Dim list As List(Of InterventionSpec) = specs.ToList()
            Dim n As Integer = NumGenes
            Dim window As Double(,) = CType(baselineWindow.Clone(), Double(,))

            ' 施加扰动
            For Each spec As InterventionSpec In list
                Dim idx As Integer

                If Not geneIndex.TryGetValue(spec.GeneName, idx) Then Continue For

                Dim sd As Double = std.Max(expression.GeneSDs(idx), 1.0E-08)
                Dim raw As Double = spec.GetInterventionValue(expression.GeneMeans(idx), expression.GeneSDs(idx))
                Dim normalized As Double = (raw - expression.GeneMeans(idx)) / sd

                For t As Integer = 0 To window.GetLength(1) - 1
                    window(idx, t) = normalized
                Next
            Next

            Dim trajectory As Double(,)() = New Double(steps, n - 1)() {}
            Dim control As Double() = ControlPrediction()

            For i As Integer = 0 To n - 1
                trajectory(0, i) = New Double() {control(i)}
            Next

            Dim width As Integer = window.GetLength(1)

            For t As Integer = 1 To steps
                Dim normalized As Tensor = model.Forward(window, False)
                Dim raw As Double() = Denormalize(normalized)

                For i As Integer = 0 To n - 1
                    trajectory(t, i) = New Double() {raw(i)}
                Next

                ' 滑窗前进：丢弃最早一列，把本次预测的标准化值追加到末尾
                For c As Integer = 0 To width - 2
                    For i As Integer = 0 To n - 1
                        window(i, c) = window(i, c + 1)
                    Next
                Next

                For i As Integer = 0 To n - 1
                    window(i, width - 1) = normalized.Data(i)
                Next
            Next

            Return trajectory
        End Function

        ''' <summary>对输入窗口做前向并反标准化为 raw log-CPM。</summary>
        ''' <param name="window">标准化窗口</param>
        ''' <returns>raw log-CPM 尺度预测</returns>
        Private Function PredictRaw(window As Double(,)) As Double()
            Return Denormalize(model.Forward(window, False))
        End Function

        ''' <summary>把标准化预测反算回 raw log-CPM 并裁剪为非负。</summary>
        ''' <param name="normalized">标准化预测 <c>[N, 1]</c></param>
        ''' <returns>raw log-CPM 预测</returns>
        Private Function Denormalize(normalized As Tensor) As Double()
            Dim n As Integer = NumGenes
            Dim result As Double() = New Double(n - 1) {}

            For i As Integer = 0 To n - 1
                result(i) = std.Max(0.0, normalized.Data(i) * expression.GeneSDs(i) + expression.GeneMeans(i))
            Next

            Return result
        End Function

        ''' <summary>构造组合扰动的合并 spec。</summary>
        Private Function CombineSpec(specs As List(Of InterventionSpec),
                                     names As List(Of String),
                                     indices As List(Of Integer)) As InterventionSpec
            Return New InterventionSpec With {
                .GeneName = String.Join("+", names),
                .GeneIndex = indices(0),
                .Mode = specs(0).Mode
            }
        End Function

        ''' <summary>目标基因全部不在建模范围内时的降级结果。</summary>
        ''' <param name="spec">原始干预定义</param>
        ''' <returns>Undefined=True 的空结果</returns>
        Private Function CreateUndefinedResult(spec As InterventionSpec) As InterventionResult
            Dim n As Integer = NumGenes

            Return New InterventionResult With {
                .Spec = spec,
                .Undefined = True,
                .GeneNames = CType(expression.GeneNames.Clone(), String()),
                .WildtypeMeans = CType(ControlPrediction().Clone(), Double()),
                .WildtypeSDs = CType(expression.GeneSDs.Clone(), Double()),
                .MutantMeans = CType(ControlPrediction().Clone(), Double()),
                .FoldChanges = New Double(n - 1) {},
                .PercentChanges = New Double(n - 1) {},
                .ZScores = New Double(n - 1) {},
                .IsSignificant = New Boolean(n - 1) {}
            }
        End Function

    End Class

End Namespace
