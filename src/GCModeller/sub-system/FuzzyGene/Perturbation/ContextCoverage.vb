Imports System.Text

Namespace Perturbation

    ''' <summary>
    ''' 上下文注入（<c>SetContext</c> / <c>LoadContext</c>）的诊断结果。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 上下文替换的是「扰动分析的对照基线」，语义上相当于把实验体系放进另一个转录组背景：
    ''' 对照（野生型）态与突变体态都从该初始条件出发各自迭代到稳态，
    ''' 因此 <see cref="SMRUCC.genomics.Analysis.BNLearn.Intervention.InterventionResult.FoldChanges"/>
    ''' 变成「同一 context 下的突变体 vs 野生型」。
    ''' </para>
    ''' <para>
    ''' 报告里必须能看出三件事，否则结果无法解释：
    ''' 模型里的哪些基因真的被上下文覆盖了（<see cref="Matched"/>）、
    ''' 哪些还在沿用训练数据的均值（<see cref="MissingGenes"/>）、
    ''' 以及哪些取值落在训练支持域之外（<see cref="OutsideSupport"/>，迭代时会被限幅，
    ''' 等于这部分输入被“剪平”了）。
    ''' </para>
    ''' </remarks>
    Public Class ContextCoverage

        ''' <summary>模型基因总数。</summary>
        Public Property NGene As Integer

        ''' <summary>上下文中提供的有效条目数（同名基因以最后一次出现的为准）。</summary>
        Public Property Provided As Integer

        ''' <summary>命中模型基因的条目数。</summary>
        Public Property Matched As Integer

        ''' <summary>因为取值不是有限数（NaN / ±Inf / 空）而被跳过的条目数。</summary>
        Public Property InvalidValues As Integer

        ''' <summary>命中模型、但取值落在训练支持域之外的基因数（迭代时会被限幅到边界）。</summary>
        Public Property OutsideSupport As Integer

        ''' <summary>模型中未被上下文覆盖、继续沿用训练基线的基因名。</summary>
        Public Property MissingGenes As String()

        ''' <summary>上下文实际使用的基因名列（<c>LoadContext</c> 在多列中自动挑选）。</summary>
        Public Property GeneColumn As String

        ''' <summary>覆盖率 = <see cref="Matched"/> / <see cref="NGene"/>。</summary>
        Public ReadOnly Property Coverage As Double
            Get
                If NGene <= 0 Then
                    Return 0.0
                End If

                Return Matched / CDbl(NGene)
            End Get
        End Property

        ''' <summary>上下文是否至少覆盖了一个基因（否则不会被应用）。</summary>
        Public ReadOnly Property IsApplied As Boolean
            Get
                Return Matched > 0
            End Get
        End Property

        ''' <summary>多行摘要。</summary>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine($"context: {Matched}/{NGene} model genes covered ({Coverage:P1}), " &
                               $"{Provided} entries provided, {InvalidValues} invalid value(s) skipped")

            If Not String.IsNullOrEmpty(GeneColumn) Then
                Call sb.AppendLine($"  gene column: {GeneColumn}")
            End If
            If OutsideSupport > 0 Then
                Call sb.AppendLine($"  {OutsideSupport} gene(s) fall outside the training support and are clamped during the propagation")
            End If

            Dim missing As String() = If(MissingGenes, New String() {})

            If missing.Length > 0 Then
                Dim preview As String = String.Join(", ", missing.Take(10))

                Call sb.AppendLine($"  {missing.Length} model gene(s) keep the training baseline: {preview}" &
                                   If(missing.Length > 10, ", ...", ""))
            End If

            Return sb.ToString().TrimEnd()
        End Function

    End Class

End Namespace
