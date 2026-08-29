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
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports SMRUCC.genomics.Analysis.BNLearn.Intervention
Imports SMRUCC.genomics.Analysis.BNLearn.ParameterLearning
Imports SMRUCC.genomics.Analysis.BNLearn.StructureLearning

Namespace Core.WGCNADBN

    ''' <summary>
    ''' 基于 WGCNA 模块划分的贝叶斯子网络训练 + 全局虚拟扰动流水线
    ''' </summary>
    Public Class WGCNASubnetworkPipeline

        ' ============================================================
        ' 1. 主入口
        ' ============================================================

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
        ' 7. 传播方法
        ' ============================================================

        ''' <summary>雅可比矩阵多步线性传播</summary>
        Private Function PropagateJacobian(sourceIdx As Integer) As GlobalPerturbationResult
            Dim n As Integer = _genes.Length
            Dim delta = New Double(n - 1) {}
            delta(sourceIdx) = InterventionValue(sourceIdx)

            Dim current = CType(delta.Clone(), Double())
            Dim total = New Double(n - 1) {}
            Dim result As New GlobalPerturbationResult() With {
                .SourceGene = _genes(sourceIdx),
                .Method = PropagationMethod.Jacobian,
                .Mode = DefaultMode(),
                .GeneNames = _genes
            }
            result.StepEffects.Add(CType(delta.Clone(), Double()))

            Dim steps As Integer = 0
            For t = 1 To MaxSteps
                Dim [next] = MatrixVectorMul(_A, current)
                For i = 0 To n - 1
                    total(i) += [next](i)
                Next
                result.StepEffects.Add([next])
                steps = t

                Dim normCur = Norm(current)
                Dim normNxt = Norm([next])
                If normCur < 1.0E-9 Then Exit For
                If normNxt / normCur < Tolerance Then Exit For
                current = [next]
            Next

            result.Effects = total
            result.Steps = steps
            Return result
        End Function

        ''' <summary>级联采样：在全局聚合网络上做多步 do-演算传播</summary>
        Private Function PropagateCascade(sourceIdx As Integer) As GlobalPerturbationResult
            Dim spec As New InterventionSpec() With {
                .GeneName = _genes(sourceIdx),
                .GeneIndex = sourceIdx,
                .Mode = DefaultMode()
            }

            Dim analyzer As New BnInterventionAnalyzer(_globalNet, _exprStd)
            Dim res = analyzer.DynamicIntervention(spec, MaxSteps, NSamples, RandomSeed)

            Dim result As New GlobalPerturbationResult() With {
                .SourceGene = _genes(sourceIdx),
                .Method = PropagationMethod.CascadeSampling,
                .Mode = DefaultMode(),
                .GeneNames = _genes,
                .Effects = CType(res.FoldChanges.Clone(), Double()),
                .Steps = MaxSteps
            }
            result.StepEffects.Add(CType(res.FoldChanges.Clone(), Double()))
            Return result
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

        Private Function DefaultMode() As Intervention.InterventionMode
            ' 默认做敲低（Knockout），与 BNLearnWorkflow.KnockoutGene 一致
            Return Intervention.InterventionMode.Knockout
        End Function

        Private Function InterventionValue(sourceIdx As Integer) As Double
            ' 雅可比传播需要的是「相对野生型的扰动增量 Δx0」，而非绝对干预值。
            ' 标准化数据野生型均值≈0、SD≈1；Knockout 下调 1 个 SD、Overexpression 上调 3 倍、
            ' Knockdown 下调 2 倍（与 BnInterventionAnalyzer 中采样所用的偏离尺度一致）。
            ' 注意：不能用 GetInterventionValue(0,1) —— Knockout 返回绝对干预值 0，
            ' 在标准化数据（野生型均值已是 0）下扰动增量为 0，导致传播全 0。
            Select Case DefaultMode()
                Case Intervention.InterventionMode.Knockout
                    Return -1.0
                Case Intervention.InterventionMode.Overexpression
                    Return 3.0
                Case Intervention.InterventionMode.Knockdown
                    Return -2.0
                Case Else
                    Return 0.0
            End Select
        End Function

        ' ---- 线性代数辅助 ----
        Private Function MatrixVectorMul(A As Double(,), v As Double()) As Double()
            Dim n = v.Length
            Dim out = New Double(n - 1) {}
            For i = 0 To n - 1
                Dim s As Double = 0
                For j = 0 To n - 1
                    s += A(i, j) * v(j)
                Next
                out(i) = s
            Next
            Return out
        End Function

        Private Function Norm(v As Double()) As Double
            Dim s As Double = 0
            For i = 0 To v.Length - 1
                s += v(i) * v(i)
            Next
            Return Math.Sqrt(s)
        End Function

    End Class

End Namespace
