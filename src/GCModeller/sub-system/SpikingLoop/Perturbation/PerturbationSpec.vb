' ============================================================================
' PerturbationSpec.vb — 单次虚拟扰动的规格（readme 2.3 扰动注入 / 四 虚拟扰动）
'
' 扰动类型沿用 BNLearn 既有的 InterventionMode 枚举（Knockout / Knockdown /
' Overexpression / Custom），从而与贝叶斯网络路线的干预实验在语义上保持一致，
' 便于两条流水线的结果互相对照。
'
' 与 BNLearn.Intervention.InterventionSpec 的差异（有意为之）：
'   BNLearn 版本按"野性型均值 ± k·标准差"决定干预值（针对连续表达量的分布）；
'   本版本工作在网络<b>输入电流</b>层，因此用 Strength ∈ [0,1] 表示扰动强度，
'   由虚拟扰动引擎折算为电流的置零 / 衰减 / 增强：
'     KO → I[g] = 0
'     KD → I[g] *= (1 − strength)
'     OE → I[g] += strength · I_max
'     Custom → I[g] = CustomValue
' ============================================================================

Imports SMRUCC.genomics.Analysis.BNLearn.Intervention

Namespace Perturbation

    ''' <summary>单个基因的虚拟扰动定义</summary>
    Public Class PerturbationSpec

        ''' <summary>目标基因名</summary>
        Public Property GeneName As String = ""

        ''' <summary>目标基因索引（由 <see cref="ResolveIndex"/> 按基因列表解析；−1 表示未解析）</summary>
        Public Property GeneIndex As Integer = -1

        ''' <summary>扰动模式</summary>
        Public Property Mode As InterventionMode = InterventionMode.Knockout

        ''' <summary>
        ''' 扰动强度（0~1）：KD 表示电流保留比例的补数，OE 表示叠加的电流倍数
        ''' （以 I_max = CurrentGain 为单位）。KO 忽略该值。
        ''' </summary>
        Public Property Strength As Double = 1.0

        ''' <summary>Custom 模式下的直接电流值</summary>
        Public Property CustomValue As Double = 0.0

#Region "工厂"

        ''' <summary>敲除：输入电流置零（readme：KO → I[:, g] = 0）</summary>
        Public Shared Function Knockout(gene As String) As PerturbationSpec
            Return New PerturbationSpec With {
                .GeneName = gene,
                .Mode = InterventionMode.Knockout,
                .Strength = 1.0
            }
        End Function

        ''' <summary>敲低：输入电流按 (1 − strength) 衰减</summary>
        Public Shared Function Knockdown(gene As String, Optional strength As Double = 0.5) As PerturbationSpec
            Return New PerturbationSpec With {
                .GeneName = gene,
                .Mode = InterventionMode.Knockdown,
                .Strength = strength
            }
        End Function

        ''' <summary>过表达：输入电流叠加 strength · I_max</summary>
        Public Shared Function Overexpress(gene As String, Optional strength As Double = 1.0) As PerturbationSpec
            Return New PerturbationSpec With {
                .GeneName = gene,
                .Mode = InterventionMode.Overexpression,
                .Strength = strength
            }
        End Function

#End Region

        ''' <summary>
        ''' 按基因列表解析基因索引。解析失败时抛出异常——静默跳过会让"扰动实验"
        ''' 变成"什么都没做"的空实验，属于必须尽早失败的情形。
        ''' </summary>
        Public Function ResolveIndex(geneNames As String()) As Integer
            If Not String.IsNullOrEmpty(GeneName) Then
                For i = 0 To geneNames.Length - 1
                    If String.Equals(geneNames(i), GeneName, StringComparison.OrdinalIgnoreCase) Then
                        GeneIndex = i
                        Return i
                    End If
                Next
            ElseIf GeneIndex >= 0 AndAlso GeneIndex < geneNames.Length Then
                GeneName = geneNames(GeneIndex)
                Return GeneIndex
            End If

            Throw New InvalidOperationException(
                $"扰动目标基因 '{GeneName}' 不在当前网络的基因列表中（共 {geneNames.Length} 个基因）")
        End Function

        Public Function Copy() As PerturbationSpec
            Return New PerturbationSpec With {
                .GeneName = GeneName,
                .GeneIndex = GeneIndex,
                .Mode = Mode,
                .Strength = Strength,
                .CustomValue = CustomValue
            }
        End Function

        Public Overrides Function ToString() As String
            Return $"{GeneName} [{Mode}] strength={Strength:G4}"
        End Function

    End Class

End Namespace
