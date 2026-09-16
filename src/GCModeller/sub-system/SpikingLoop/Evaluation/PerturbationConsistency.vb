' ============================================================================
' PerturbationConsistency.vb — 虚拟扰动有效性评估（readme 五.3 的等价替代）
'
' readme 五.3 的理想做法是"用真实 Perturb-seq 数据做基准，比较预测与观测的
' delta 相关性"。但在没有配套 Perturb-seq 数据时，仍可以用<b>先验调控方向</b>
' 作为弱监督基准，检查虚拟扰动是否产生了符合生物学直觉的方向性响应：
'
'   对每个转录因子 TF 做敲除（KO），观察其直接靶基因的表达变化方向：
'     若 TF→Target 是 Activator（激活）：KO 掉激活因子 → 靶基因应当<b>下降</b>
'     若 TF→Target 是 Inhibitor（抑制）：KO 掉抑制因子 → 靶基因应当<b>上升</b>
'   方向一致即记为"一致"，反之记为"不一致"；变化幅度低于阈值则记为"未显著"。
'
' 该比率的意义：它检验的是"模型是否学到了符号正确的调控关系"。
' 训练后期若先验结构正则（α）过强，模型会机械复现先验方向而使该比率虚高，
' 因此应与链路预测指标（AUROC）以及预测精度（PCC）联合解读。
' ============================================================================

Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.TensorFlow
Imports SMRUCC.genomics.Analysis.BNLearn
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.SpikingLoop.Perturbation
Imports std = System.Math

Namespace Evaluation

    ''' <summary>扰动方向一致性报告</summary>
    Public Class PerturbationConsistencyReport

        ''' <summary>被检验的转录因子数量</summary>
        Public Property NumTfs As Integer

        ''' <summary>参与方向检验的调控边总数</summary>
        Public Property TotalEdges As Integer

        ''' <summary>方向与先验一致的边数</summary>
        Public Property ConsistentEdges As Integer

        ''' <summary>方向与先验矛盾的边数</summary>
        Public Property InconsistentEdges As Integer

        ''' <summary>变化幅度低于阈值的边数（未显著）</summary>
        Public Property NotSignificantEdges As Integer

        ''' <summary>调控方向未知（Effector.Unknown）的边数</summary>
        Public Property UnknownEffectorEdges As Integer

        ''' <summary>方向一致性比率 = 一致 / (一致 + 矛盾)</summary>
        Public Property ConsistentRatio As Double

        ''' <summary>显著变化判定阈值</summary>
        Public Property Threshold As Double

        ''' <summary>每个 TF 的一致性明细</summary>
        Public Property PerTf As List(Of (tf As String, consistent As Integer, inconsistent As Integer, ratio As Double))

        Public Overrides Function ToString() As String
            Return $"方向一致性 = {ConsistentRatio:P1}（一致 {ConsistentEdges} / 矛盾 {InconsistentEdges} / " &
                   $"未显著 {NotSignificantEdges}），覆盖 {NumTfs} 个 TF"
        End Function

    End Class

    ''' <summary>虚拟扰动方向一致性评估</summary>
    Public Module PerturbationConsistency

        ''' <summary>
        ''' 对先验网络中的每个 TF 做敲除，检验其靶基因的响应方向是否与调控类型一致。
        ''' </summary>
        ''' <param name="engine">虚拟扰动引擎（基于已训练的模型）</param>
        ''' <param name="baseline">基线表达状态 [1, N]</param>
        ''' <param name="prior">子网络的先验调控网络</param>
        ''' <param name="geneNames">基因名列表（用于解析靶基因索引）</param>
        ''' <param name="threshold">显著变化阈值（表达偏移绝对值）</param>
        ''' <param name="maxTfs">最多检验多少个 TF（0 = 全部）</param>
        Public Function Evaluate(engine As VirtualPerturbationEngine, baseline As Tensor,
                                 prior As PriorNetwork, geneNames As String(),
                                 Optional threshold As Double = 0.001,
                                 Optional maxTfs As Integer = 0) As PerturbationConsistencyReport

            If engine Is Nothing Then Throw New ArgumentNullException(NameOf(engine))
            If prior Is Nothing Then Throw New ArgumentNullException(NameOf(prior))

            Dim index As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For i = 0 To geneNames.Length - 1
                If Not index.ContainsKey(geneNames(i)) Then index(geneNames(i)) = i
            Next

            ' 只检验"自身在子网络中、且至少有一个靶基因也在子网络中"的 TF
            Dim tfs = prior.Edges _
                .SafeQuery() _
                .Select(Function(e) e.TF) _
                .Where(Function(tf) index.ContainsKey(tf)) _
                .Distinct(StringComparer.OrdinalIgnoreCase) _
                .ToArray()

            If maxTfs > 0 AndAlso tfs.Length > maxTfs Then
                tfs = tfs.Take(maxTfs).ToArray()
            End If

            Dim report As New PerturbationConsistencyReport With {
                .Threshold = threshold,
                .PerTf = New List(Of (tf As String, consistent As Integer, inconsistent As Integer, ratio As Double))()
            }

            For Each tf In tfs
                Dim trajectory = engine.Run(baseline, New PerturbationSpec() {PerturbationSpec.Knockout(tf)})
                Dim delta = trajectory.Delta()

                Dim consistent = 0
                Dim inconsistent = 0

                For Each edge As RegulatoryEdge In prior.GetTargets(tf)
                    If String.Equals(edge.TargetGene, tf, StringComparison.OrdinalIgnoreCase) Then Continue For

                    Dim targetIdx = -1
                    If Not index.TryGetValue(edge.TargetGene, targetIdx) Then Continue For

                    Dim dv = delta(targetIdx)
                    report.TotalEdges += 1

                    If std.Abs(dv) <= threshold Then
                        report.NotSignificantEdges += 1
                        Continue For
                    End If

                    Select Case edge.RegulationType
                        Case Effector.Activator
                            ' 敲除激活因子 → 靶基因应下降
                            If dv < 0.0 Then consistent += 1 Else inconsistent += 1
                        Case Effector.Inhibitor
                            ' 敲除抑制因子 → 靶基因应上升
                            If dv > 0.0 Then consistent += 1 Else inconsistent += 1
                        Case Else
                            report.UnknownEffectorEdges += 1
                    End Select
                Next

                report.ConsistentEdges += consistent
                report.InconsistentEdges += inconsistent
                report.NumTfs += 1

                Dim judged = consistent + inconsistent
                report.PerTf.Add((tf, consistent, inconsistent,
                                  If(judged > 0, consistent / CDbl(judged), Double.NaN)))
            Next

            Dim total = report.ConsistentEdges + report.InconsistentEdges
            report.ConsistentRatio = If(total > 0, report.ConsistentEdges / CDbl(total), Double.NaN)

            Return report
        End Function

    End Module

End Namespace
