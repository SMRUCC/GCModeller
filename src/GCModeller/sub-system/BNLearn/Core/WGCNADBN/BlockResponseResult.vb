Imports System.Text
Imports Microsoft.VisualBasic.Linq

Namespace Core.WGCNADBN

    Public Class BlockResponseResult

        Public Property finalResponses As Dictionary(Of String, Double())
        Public Property trajectories As Dictionary(Of String, Dictionary(Of String, List(Of Double)))
        Public Property allgenes As String()

        Sub New()
            finalResponses = New Dictionary(Of String, Double())
            trajectories = New Dictionary(Of String, Dictionary(Of String, List(Of Double)))
            allgenes = New String() {}
        End Sub

        ''' <summary>
        ''' 导出全局虚拟扰动结果：基因 × 扰动源 响应矩阵 TSV + 每个扰动源的逐基因明细 TSV。
        ''' </summary>
        Public Sub SaveModularResults(outputDir As String)
            ' 全局响应矩阵（最终稳态，gene × perturbation）
            Dim sbMatrix As New StringBuilder()
            sbMatrix.Append("gene")
            For Each src In finalResponses.Keys
                sbMatrix.Append(vbTab).Append(src)
            Next
            sbMatrix.AppendLine()
            For i = 0 To allgenes.Length - 1
                sbMatrix.Append(allgenes(i))
                For Each src In finalResponses.Keys
                    sbMatrix.Append(vbTab).Append(finalResponses(src)(i).ToString("F6"))
                Next
                sbMatrix.AppendLine()
            Next

            Call sbMatrix.SaveTo(System.IO.Path.Combine(outputDir, "modular_global_perturbation_responses.tsv"))

            ' 每个扰动源明细（基因 \t 最终效应 \t 轨迹峰值）
            For Each src As String In trajectories.Keys
                Dim tr = trajectories(src)
                Dim sb As New StringBuilder()
                sb.AppendLine("gene" & vbTab & "final_effect" & vbTab & "peak_effect")
                For Each g In allgenes
                    If tr.ContainsKey(g) Then
                        Dim vec = tr(g)
                        Dim peak = vec.Max()
                        sb.AppendLine(String.Format("{0}{1}{2:F6}{3}{4:F6}", g, vbTab, vec(vec.Count - 1), vbTab, peak))
                    Else
                        sb.AppendLine(String.Format("{0}{1}1.000000{1}1.000000", g, vbTab))
                    End If
                Next
                Dim safe = New String(src.Where(Function(c) Char.IsLetterOrDigit(c)).ToArray())
                Call sb.SaveTo(System.IO.Path.Combine(outputDir, "modular_pert_" & safe & ".tsv"))
            Next

            Call $"GRN.SaveModularResults: 模块化全局扰动结果已导出至 {outputDir}".info
        End Sub

        ''' <summary>
        ''' ④ 全局级联虚拟扰动
        ''' </summary>
        ''' <param name="moduleDBs"></param>
        ''' <param name="knockGenes"></param>
        ''' <param name="dynamicSteps"></param>
        ''' <returns></returns>
        Public Shared Function ModularDBNIntervene(moduleDBs As BlockBayesianNetwork, knockGenes As IEnumerable(Of String), Optional dynamicSteps As Integer = 10) As BlockResponseResult
            ' ④ 全局级联虚拟扰动
            Dim finalResponses As New Dictionary(Of String, Double())()
            Dim trajectories As New Dictionary(Of String, Dictionary(Of String, List(Of Double)))()

            For Each geneId As String In knockGenes.SafeQuery
                Dim respVec As Double() = moduleDBs.CascadeIntervene(geneId, dynamicSteps, trajectories)
                finalResponses(geneId) = respVec
            Next

            Return New BlockResponseResult With {
                .finalResponses = finalResponses,
                .trajectories = trajectories,
                .allgenes = moduleDBs.allgenes
            }
        End Function

    End Class
End Namespace