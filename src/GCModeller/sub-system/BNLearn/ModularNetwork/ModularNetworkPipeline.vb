' ============================================================
' WGCNASubnetworkPipeline.vb
' ------------------------------------------------------------
' 依据 DBNBlocks.md 文档思路："分而治之训练、合而为一扰动"。
'
' 流程：
'   1) 基于 WGCNA 模块划分（GeneModuleColor[]）把基因切分为若干模块；
'   2) 对每个模块子集独立训练静态高斯贝叶斯子网络（结构学习 + 参数学习）；
'   3) 把各子网的回归系数拼成块对角全局系数矩阵 A，并（关键）补全
'      模块间边（用模块 eigengene 相关 + hub 基因间相关），得到整合的
'      全局网络（含模块内 + 模块间边），并统一学习全局 CPD；
'   4) 在整合后的全局网络上做虚拟扰动传播，支持两种方法：
'        - Jacobian（默认）：沿 A^k 多步线性传播至收敛；
'        - CascadeSampling：在全局网络上做多步 do-演算（DynamicIntervention）。
'   5) 导出全局扰动响应矩阵（gene × perturbation）TSV + 控制台摘要。
' ============================================================

Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.BNLearn.Core
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.ModularNetwork.WGCNA
Imports SMRUCC.genomics.Analysis.BNLearn.StructureLearning

Namespace ModularNetwork

    ''' <summary>
    ''' 基于 WGCNA 模块划分的贝叶斯子网络训练 + 全局虚拟扰动流水线
    ''' </summary>
    Public Class ModularNetworkPipeline

        ' ---- 全局扰动参数 ----
        ''' <summary>传播方法，默认 Jacobian（线性化雅可比多步传播）</summary>
        Public Property Propagation As PropagationMethod = PropagationMethod.Jacobian

        ''' <summary>最大传播步数（雅可比收敛上限 / 级联采样时间步数）</summary>
        Public Property MaxSteps As Integer = 50

        ''' <summary>雅可比收敛阈值：||e_{t+1}|| / ||e_t|| 小于该值即停止</summary>
        Public Property Tolerance As Double = 0.000001
        ''' <summary>参数学习与采样所用样本数</summary>
        Public Property NSamples As Integer = 10000

        ''' <summary>随机种子</summary>
        Public Property RandomSeed As Integer = 42

        ' ---- 训练参数（与 BNLearnWorkflow 风格一致） ----

        ''' <summary>
        ''' 是否对表达数据做标准化（z-score），默认 True
        ''' </summary>
        ''' <returns></returns>
        Public Property NormalizeData As Boolean = True

        ''' <summary>结构学习参数（算法/显著性阈值/最大父节点数/随机种子）</summary>
        Public Property StructureParams As New StructureLearningParams()

        ''' <summary>每个模块取 kME 最高的前 N 个基因作为模块接口（hub）</summary>
        Public Property HubTopN As Integer = 20

        ''' <summary>模块 eigengene 相关阈值：|cor| 超过才尝试补模块间边</summary>
        Public Property CrossModuleCorThreshold As Double = 0.3

        ''' <summary>hub 基因间相关阈值：|r| 超过才在对应基因间补跨模块边</summary>
        Public Property CrossGeneCorThreshold As Double = 0.4

        ''' <summary>跨模块边的初始权重缩放（最终由全局参数学习覆盖）</summary>
        Public Property CrossScale As Double = 0.5


        Dim model As BlockNetwork
        Dim infer As BlockPropagate

        Public Function GetModuleHubSources() As String()
            Return model.GetModuleHubSources.ToArray
        End Function

        ' ============================================================
        ' 1. 主入口
        ' ============================================================

        ''' <summary>
        ''' 模块切分 → 子网络训练 → 全局矩阵拼接
        ''' </summary>
        ''' <param name="assignment">WGCNA 模块划分结果（geneID / moduleColor / kME）</param>
        ''' <param name="expr">全局表达矩阵（基因 × 样本）</param>
        ''' <returns></returns>
        Public Function Learn(assignment As GeneModuleColor(), expr As GeneExpressionData) As ModularNetworkPipeline
            model = New BlockNetwork(expr, normalizeData:=NormalizeData) With {
                .CrossGeneCorThreshold = CrossGeneCorThreshold,
                .CrossModuleCorThreshold = CrossModuleCorThreshold,
                .CrossScale = CrossScale,
                .HubTopN = HubTopN,
                .StructureParams = StructureParams
            }
            infer = New BlockPropagate With {
                .Model = model.Learn(assignment),
                .MaxSteps = MaxSteps,
                .Tolerance = Tolerance,
                .NSamples = NSamples,
                .RandomSeed = RandomSeed
            }

            Return Me
        End Function

        ''' <summary>
        ''' 各源基因全局扰动。
        ''' </summary>
        ''' <param name="sources">扰动源基因列表</param>
        ''' <returns>每个扰动源的全局扰动结果</returns>
        Public Iterator Function InsilicoPerturbation(sources As IEnumerable(Of String), mode As InterventionMode) As IEnumerable(Of GlobalPerturbationResult)
            ' 确定扰动源
            Dim srcList As New List(Of String)(sources.SafeQuery)

            Call $"[WGCNASubnetworkPipeline] 虚拟扰动实验的代表基因共 {srcList.Count} 个".debug

            For Each src As String In srcList
                Dim gi As Integer = model.GetGlobalIndex(src)
                If gi < 0 Then
                    Call $"[WGCNASubnetworkPipeline] 警告: 扰动源 '{src}' 不在表达矩阵中，跳过".debug
                    Continue For
                End If
                Dim r As GlobalPerturbationResult
                If Propagation = PropagationMethod.Jacobian Then
                    r = infer.PropagateJacobian(gi, mode)
                Else
                    r = infer.PropagateCascade(gi, mode)
                End If

                Yield r

                Call r.ToString().debug
            Next
        End Function

        ' ============================================================
        ' 8. 结果导出
        ' ============================================================

        ''' <summary>
        ''' 写出全局扰动响应矩阵（行=基因，列=各扰动源）与每个源的明细 TSV，并打印摘要。
        ''' </summary>
        Public Sub SaveResults(results As List(Of GlobalPerturbationResult), outputDir As String)
            If Not Directory.Exists(outputDir) Then
                Directory.CreateDirectory(outputDir)
            End If

            ' 全局响应矩阵
            Dim sbMatrix As New StringBuilder()
            sbMatrix.Append("gene")
            For Each r In results
                sbMatrix.Append(vbTab).Append(r.SourceGene)
            Next
            sbMatrix.AppendLine()

            For i = 0 To model._genes.Length - 1
                sbMatrix.Append(model._genes(i))
                For Each r In results
                    sbMatrix.Append(vbTab).Append(r.Effects(i).ToString("F6"))
                Next
                sbMatrix.AppendLine()
            Next
            File.WriteAllText(Path.Combine(outputDir, "global_perturbation_responses.tsv"), sbMatrix.ToString())

            ' 每个源的明细
            For Each r In results
                Dim safe = New String(r.SourceGene.Where(Function(c) Char.IsLetterOrDigit(c)).ToArray())
                File.WriteAllText(Path.Combine(outputDir, "pert_" & safe & ".tsv"), r.ToTSV())
            Next

            ' 控制台摘要
            For Each r In results
                Console.WriteLine(r.ToString())
            Next
        End Sub

    End Class

End Namespace
