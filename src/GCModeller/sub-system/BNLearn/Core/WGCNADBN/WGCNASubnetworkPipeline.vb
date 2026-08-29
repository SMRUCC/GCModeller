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

Namespace Core.WGCNADBN

    ''' <summary>
    ''' 基于 WGCNA 模块划分的贝叶斯子网络训练 + 全局虚拟扰动流水线
    ''' </summary>
    Public Class WGCNASubnetworkPipeline

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

        Dim model As BlockNetwork
        Dim infer As BlockPropagate

        ' ============================================================
        ' 1. 主入口
        ' ============================================================

        Public Function Learn(assignment As GeneModuleColor(), expr As GeneExpressionData) As WGCNASubnetworkPipeline
            model = New BlockNetwork(expr)
            infer = New BlockPropagate With {
                .Model = model.Learn(assignment),
                .Propagation = Propagation,
                .MaxSteps = MaxSteps,
                .Tolerance = Tolerance,
                .NSamples = NSamples,
                .RandomSeed = RandomSeed
            }

            Return Me
        End Function

        ''' <summary>
        ''' 运行完整流程：模块切分 → 子网络训练 → 全局矩阵拼接 → 各源基因全局扰动。
        ''' </summary>
        ''' <param name="assignment">WGCNA 模块划分结果（geneID / moduleColor / kME）</param>
        ''' <param name="expr">全局表达矩阵（基因 × 样本）</param>
        ''' <param name="sources">扰动源基因列表；为 Nothing 时自动取每模块 kME 最高的代表基因</param>
        ''' <returns>每个扰动源的全局扰动结果</returns>
        Public Function Run(assignment As GeneModuleColor(),
                           expr As GeneExpressionData,
                           Optional sources As String() = Nothing) As List(Of GlobalPerturbationResult)

            Call Learn(assignment, expr)



            ' 确定扰动源
            Dim srcList As List(Of String)
            If sources Is Nothing OrElse sources.Length = 0 Then
                srcList = GetDefaultSources()
                Call $"[WGCNASubnetworkPipeline] 未指定扰动源，自动取每模块代表基因共 {srcList.Count} 个".debug
            Else
                srcList = New List(Of String)(sources)
            End If

            Dim results As New List(Of GlobalPerturbationResult)()
            For Each src In srcList
                Dim gi As Integer = GetGlobalIndex(src)
                If gi < 0 Then
                    Call $"[WGCNASubnetworkPipeline] 警告: 扰动源 '{src}' 不在表达矩阵中，跳过".debug
                    Continue For
                End If
                Dim r As GlobalPerturbationResult
                If Propagation = PropagationMethod.Jacobian Then
                    r = PropagateJacobian(gi)
                Else
                    r = PropagateCascade(gi)
                End If
                results.Add(r)
                Call r.ToString().debug
            Next

            Return results
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

            For i = 0 To _genes.Length - 1
                sbMatrix.Append(_genes(i))
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

        ' ============================================================
        ' 内部辅助
        ' ============================================================

        Private Function GetDefaultSources() As List(Of String)
            Dim src As New List(Of String)()
            For Each kv In _moduleHubs
                If kv.Value.Count > 0 Then
                    src.Add(kv.Value(0))
                End If
            Next
            Return src
        End Function

    End Class

End Namespace
