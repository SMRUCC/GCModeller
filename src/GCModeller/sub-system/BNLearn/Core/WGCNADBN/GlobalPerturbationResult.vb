' ============================================================
' GlobalPerturbationResult.vb - 全局虚拟扰动结果结构
' ============================================================
' 对应 DBNBlocks.md 步骤4：输出全局扰动响应矩阵（gene × perturbation）
' 的单个扰动源结果，包含稳态效应向量、逐步效应与 Top 变化基因。
' ============================================================

Imports System.Text

Namespace Core.WGCNADBN

    ''' <summary>
    ''' 单个扰动源在全局网络上传播后的结果
    ''' </summary>
    Public Class GlobalPerturbationResult

        ''' <summary>扰动源基因名</summary>
        Public Property SourceGene As String

        ''' <summary>使用的传播方法</summary>
        Public Property Method As PropagationMethod

        ''' <summary>扰动模式（敲低/过表达/自定义）</summary>
        Public Property Mode As Intervention.InterventionMode

        ''' <summary>全局基因名列表（与 Effects 各分量对齐）</summary>
        Public Property GeneNames As String()

        ''' <summary>稳态全局效应向量（最终各基因的表达变化量，与 GeneNames 对齐）</summary>
        Public Property Effects As Double()

        ''' <summary>逐步效应向量（索引 = 传播步数，每个元素为长度 N 的向量），用于观察收敛过程</summary>
        Public Property StepEffects As New List(Of Double())()

        ''' <summary>实际传播步数</summary>
        Public Property Steps As Integer = 0

        ''' <summary>
        ''' 获取变化幅度最大的前 n 个基因（按 |效应| 排序）
        ''' </summary>
        Public Function GetTopChangedGenes(n As Integer) As List(Of (GeneName As String, Effect As Double))
            Dim indexed As New List(Of (idx As Integer, eff As Double))()
            For i = 0 To Effects.Length - 1
                indexed.Add((i, Effects(i)))
            Next
            indexed.Sort(Function(a, b) Math.Abs(b.eff).CompareTo(Math.Abs(a.eff)))

            Dim result As New List(Of (String, Double))()
            For i = 0 To Math.Min(n - 1, indexed.Count - 1)
                result.Add((GeneNames(indexed(i).idx), indexed(i).eff))
            Next
            Return result
        End Function

        ''' <summary>生成单源的 TSV 内容（基因 \t 效应）</summary>
        Public Function ToTSV() As String
            Dim sb As New StringBuilder()
            sb.AppendLine(String.Format("gene{0}{1}_effect", vbTab, SourceGene))
            For i = 0 To GeneNames.Length - 1
                sb.AppendLine(String.Format("{0}{1}{2:F6}", GeneNames(i), vbTab, Effects(i)))
            Next
            Return sb.ToString()
        End Function

        ''' <summary>控制台摘要</summary>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()
            sb.AppendLine(String.Format("=== 全局虚拟扰动: {0} ({1}, {2}) ===",
                                        SourceGene, Mode.ToString(), Method.ToString()))
            sb.AppendLine(String.Format("传播步数: {0}", Steps))
            sb.AppendLine("Top 变化基因:")
            For Each item In GetTopChangedGenes(20)
                sb.AppendLine(String.Format("  {0}: Δ={1:F4}", item.GeneName, item.Effect))
            Next
            Return sb.ToString()
        End Function

    End Class

End Namespace
