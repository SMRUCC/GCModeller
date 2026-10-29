Imports System.Text
Imports System.Text.Encodings.Web
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports Microsoft.VisualBasic.DataMining.FuzzyLogic.TSK

Namespace Fitting

    ''' <summary>
    ''' 单个靶基因的神经模糊（Takagi–Sugeno）调控模型。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 一个实例就是一句话：「当这些调控因子处于某种模糊状态时，靶基因的表达近似为
    ''' 一个局部线性函数」。模型的输入顺序与 <see cref="Regulators"/> 一一对应，
    ''' 输出维数恒为 1（靶基因自身）。
    ''' </para>
    ''' <para>
    ''' 可解释性来自两方面：<see cref="RulesText"/> 给出带触发强度的 IF-THEN 规则，
    ''' <see cref="LocalSensitivity"/> 给出给定状态下每个调控因子的解析偏导
    ''' <c>∂x̂/∂x_j = Σ_r w̄_r · θ[r·(n+1)+j]</c>，其符号即调控方向。
    ''' </para>
    ''' </remarks>
    Public Class GeneRegulationModel

        ''' <summary>靶基因名称。</summary>
        Public Property TargetGene As String

        ''' <summary>输入变量（调控因子）名称，顺序即 <see cref="Network"/> 的输入顺序。</summary>
        Public Property Regulators As String()

        ''' <summary>推理内核（TSK 五层网络）。</summary>
        Public Property Network As TskNetwork

        ''' <summary>训练集决定系数。</summary>
        Public Property R2 As Double

        ''' <summary>训练集均方根误差（z 空间）。</summary>
        Public Property RMSE As Double

        ''' <summary>实际训练轮数。</summary>
        Public Property Iterations As Integer

        ''' <summary>是否因早停提前结束。</summary>
        Public Property Converged As Boolean

        ''' <summary>被剪掉的规则数量。</summary>
        Public Property PrunedRuleCount As Integer

        ''' <summary>每条规则在训练集上的平均归一化触发强度。</summary>
        Public Property RuleActivations As Double()

        ''' <summary>训练过程中的降级/告警信息。</summary>
        Public Property Warnings As String()

        ''' <summary>靶基因原始表达的均值（反标准化用）。</summary>
        Public Property Mean As Double

        ''' <summary>靶基因原始表达的标准差（反标准化与 Z 分数用）。</summary>
        Public Property Sd As Double

        ''' <summary>训练目标在 z 空间上的最小值（支持域下界）。</summary>
        Public Property SupportMin As Double

        ''' <summary>训练目标在 z 空间上的最大值（支持域上界）。</summary>
        Public Property SupportMax As Double

        ''' <summary>该模型是否来自滞后（时序）路线。</summary>
        Public Property IsLagged As Boolean

        ''' <summary>训练行数（诊断用）。</summary>
        Public Property SampleCount As Integer

        ''' <summary>规则数量。</summary>
        Public ReadOnly Property RuleCount As Integer
            Get
                Return If(Network Is Nothing, 0, Network.Rules.RuleCount)
            End Get
        End Property

        ''' <summary>在给定的调控因子取值（z 空间）上预测靶基因表达（z 空间）。</summary>
        Public Function Predict(regulatorValues As Double()) As Double
            If Network Is Nothing Then
                Throw New InvalidOperationException($"the target gene '{TargetGene}' has no trained model")
            End If

            Dim value As Double() = Network.PredictRow(regulatorValues)

            Return value(0)
        End Function

        ''' <summary>
        ''' 给定状态下的解析局部灵敏度 <c>∂x̂/∂x_j</c>（长度等于输入个数）。
        ''' </summary>
        ''' <param name="regulatorValues">调控因子的 z 值</param>
        ''' <returns>每个输入的偏导；正为激活、负为抑制</returns>
        ''' <remarks>
        ''' 一阶 Sugeno 结论 <c>f_r = Σ_j θ[r,j]·x_j + θ[r,n]</c> 对输入线性，
        ''' 因此 <c>∂x̂/∂x_j = Σ_r w̄_r·θ[r,j]</c>，一次前向即可拿到全部输入的导数，
        ''' 不需要逐维数值差分（那会慢 <c>2n</c> 倍且在隶属函数饱和处产生噪声）。
        ''' </remarks>
        Public Function LocalSensitivity(regulatorValues As Double()) As Double()
            If Network Is Nothing Then
                Throw New InvalidOperationException($"the target gene '{TargetGene}' has no trained model")
            End If

            Dim activation As Double() = Network.GetRuleActivations(regulatorValues)
            Dim n As Integer = Network.InputCount
            Dim result(n - 1) As Double
            Dim theta As Double(,) = Network.Theta

            For r As Integer = 0 To activation.Length - 1
                Dim w As Double = activation(r)

                If w = 0.0 Then
                    Continue For
                End If

                Dim offset As Integer = r * (n + 1)

                For j As Integer = 0 To n - 1
                    result(j) += w * theta(offset + j, 0)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 可读的 IF-THEN 规则列表（含平均触发强度与结论线性系数）。
        ''' </summary>
        ''' <returns>每行一条规则</returns>
        Public Function RulesText() As String()
            If Network Is Nothing Then
                Return New String() {}
            End If

            Dim n As Integer = Network.InputCount
            Dim count As Integer = Network.Rules.RuleCount
            Dim lines(count - 1) As String
            Dim theta As Double(,) = Network.Theta

            For r As Integer = 0 To count - 1
                Dim parts(n - 1) As String

                For i As Integer = 0 To n - 1
                    parts(i) = $"{Network.Inputs(i).Name} IS {Network.Inputs(i).FunctionLabel(Network.Rules.Antecedent(r, i))}"
                Next

                Dim activation As Double = If(RuleActivations IsNot Nothing AndAlso r < RuleActivations.Length,
                                              RuleActivations(r), 0.0)

                Dim terms(n) As String
                Dim offset As Integer = r * (n + 1)

                For j As Integer = 0 To n - 1
                    terms(j) = $"{theta(offset + j, 0).ToString("G5")}*{Network.Inputs(j).Name}"
                Next

                terms(n) = theta(offset + n, 0).ToString("G5")

                lines(r) = $"rule{r + 1} [{activation.ToString("P2")}] IF {String.Join(" AND ", parts)} " &
                           $"THEN {TargetGene} = {String.Join(" + ", terms)}"
            Next

            Return lines
        End Function

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"{TargetGene} <- {{{String.Join(", ", If(Regulators, New String() {}))}}} " &
                   $"({RuleCount} rules, R2={R2:G4}, RMSE={RMSE:G4})"
        End Function

#Region "序列化"

        ''' <summary>序列化为 JSON（模型本体以 TSK 自己的 JSON 形式内嵌）。</summary>
        Public Function ToJson(Optional indented As Boolean = True) As String
            Dim dto As New GeneModelDto With {
                .targetGene = TargetGene,
                .regulators = Regulators,
                .r2 = R2,
                .rmse = RMSE,
                .iterations = Iterations,
                .converged = Converged,
                .prunedRuleCount = PrunedRuleCount,
                .ruleActivations = RuleActivations,
                .warnings = Warnings,
                .mean = Mean,
                .sd = Sd,
                .supportMin = SupportMin,
                .supportMax = SupportMax,
                .isLagged = IsLagged,
                .sampleCount = SampleCount,
                .model = If(Network Is Nothing, Nothing, Network.ToJsonDto)
            }

            Return JsonSerializer.Serialize(dto, JsonOptions(indented))
        End Function

        ''' <summary>从 JSON 还原（不含训练现场，但可直接推理）。</summary>
        Public Shared Function FromJson(json As String) As GeneRegulationModel
            If String.IsNullOrEmpty(json) Then
                Throw New ArgumentException("the json text is empty", NameOf(json))
            End If

            Dim dto As GeneModelDto = JsonSerializer.Deserialize(Of GeneModelDto)(json, JsonOptions(False))

            If dto Is Nothing Then
                Throw New InvalidOperationException("the json text does not describe a gene regulation model")
            End If

            Return FromDto(dto)
        End Function

        ''' <summary>由传输对象还原。</summary>
        Friend Shared Function FromDto(dto As GeneModelDto) As GeneRegulationModel
            Dim model As New GeneRegulationModel With {
                .TargetGene = dto.targetGene,
                .Regulators = dto.regulators,
                .R2 = dto.r2,
                .RMSE = dto.rmse,
                .Iterations = dto.iterations,
                .Converged = dto.converged,
                .PrunedRuleCount = dto.prunedRuleCount,
                .RuleActivations = dto.ruleActivations,
                .Warnings = dto.warnings,
                .Mean = dto.mean,
                .Sd = dto.sd,
                .SupportMin = dto.supportMin,
                .SupportMax = dto.supportMax,
                .IsLagged = dto.isLagged,
                .SampleCount = dto.sampleCount
            }

            If Not dto.model Is Nothing Then
                model.Network = TskNetwork.FromModelDto(dto.model)
            End If

            Return model
        End Function

        ''' <summary>构造传输对象（zip 包按基因逐条写入时复用）。</summary>
        Friend Function ToDto() As GeneModelDto
            Return New GeneModelDto With {
                .targetGene = TargetGene,
                .regulators = Regulators,
                .r2 = R2,
                .rmse = RMSE,
                .iterations = Iterations,
                .converged = Converged,
                .prunedRuleCount = PrunedRuleCount,
                .ruleActivations = RuleActivations,
                .warnings = Warnings,
                .mean = Mean,
                .sd = Sd,
                .supportMin = SupportMin,
                .supportMax = SupportMax,
                .isLagged = IsLagged,
                .sampleCount = SampleCount,
                .modelJson = If(Network Is Nothing, Nothing, Network.ToJson(indented:=False))
            }
        End Function

        ''' <summary>统一的 JSON 选项：不转义中文基因名、忽略 Nothing 字段。</summary>
        Friend Shared Function JsonOptions(indented As Boolean) As JsonSerializerOptions
            Return New JsonSerializerOptions With {
                .WriteIndented = indented,
                .Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                .DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            }
        End Function

#End Region

    End Class

    ''' <summary>单个靶基因模型的传输对象。</summary>
    Public Class GeneModelDto

        ''' <summary>靶基因名称。</summary>
        Public Property targetGene As String
        ''' <summary>调控因子名称。</summary>
        Public Property regulators As String()
        ''' <summary>训练集决定系数。</summary>
        Public Property r2 As Double
        ''' <summary>训练集均方根误差。</summary>
        Public Property rmse As Double
        ''' <summary>训练轮数。</summary>
        Public Property iterations As Integer
        ''' <summary>是否早停。</summary>
        Public Property converged As Boolean
        ''' <summary>被剪掉的规则数量。</summary>
        Public Property prunedRuleCount As Integer
        ''' <summary>规则平均触发强度。</summary>
        Public Property ruleActivations As Double()
        ''' <summary>训练告警。</summary>
        Public Property warnings As String()
        ''' <summary>靶基因原始表达均值。</summary>
        Public Property mean As Double
        ''' <summary>靶基因原始表达标准差。</summary>
        Public Property sd As Double
        ''' <summary>训练目标最小值（z 空间）。</summary>
        Public Property supportMin As Double
        ''' <summary>训练目标最大值（z 空间）。</summary>
        Public Property supportMax As Double
        ''' <summary>是否滞后路线。</summary>
        Public Property isLagged As Boolean
        ''' <summary>训练行数。</summary>
        Public Property sampleCount As Integer
        ''' <summary>TSK 网络的 JSON 文本。</summary>
        Public Property model As TskModelDto

    End Class

End Namespace
