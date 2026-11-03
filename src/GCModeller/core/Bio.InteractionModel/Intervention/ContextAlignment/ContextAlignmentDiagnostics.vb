Imports System.Linq
Imports System.Text

Namespace Intervention

    ''' <summary>
    ''' 上下文对齐过程的统一诊断信息。
    ''' </summary>
    ''' <remarks>
    ''' 这个类型取代了各项目自带的零散诊断（FuzzyGene 的 <c>ContextCoverage</c>、
    ''' GeneTree 的 <c>ContextNote</c> / <c>ContextClampGenes</c> / <c>UnknownContextGenes</c>），
    ''' 保证「上下文到底覆盖了什么、有没有被裁剪」在任何项目上都用同一套口径回答。
    ''' </remarks>
    Public Class ContextAlignmentDiagnostics

        ''' <summary>本次使用的对齐模式。</summary>
        Public Property Mode As ContextAlignmentMode

        ''' <summary>模型基因总数。</summary>
        Public Property NGene As Integer

        ''' <summary>调用方提供的有效条目数（同名基因以最后一次出现的为准）。</summary>
        Public Property Provided As Integer

        ''' <summary>命中模型基因、并参与对齐的条目数。</summary>
        Public Property Matched As Integer

        ''' <summary>因为取值不是有限数（NaN / ±Inf）而被跳过的条目数。</summary>
        Public Property InvalidValues As Integer

        ''' <summary>无法映射到模型基因的名字（通常意味着基因 ID 空间不一致）。</summary>
        Public Property UnknownGenes As String()

        ''' <summary>模型里未被上下文覆盖、继续沿用训练基线的基因名。</summary>
        Public Property MissingGenes As String()

        ''' <summary>对齐之后仍落在训练支持域之外的基因数（会被限幅“剪平”的部分）。</summary>
        Public Property OutsideSupport As Integer

        ''' <summary>因为越界策略而被实际钳制到边界的基因数。</summary>
        Public Property ClampedGenes As Integer

        ''' <summary>是否真的执行了对齐（<see cref="ContextAlignmentMode.None"/> 时为 <c>False</c>）。</summary>
        Public Property Aligned As Boolean

        ''' <summary>
        ''' 对齐前后（仅命中基因）的 Spearman 秩相关系数。
        ''' </summary>
        ''' <remarks>
        ''' <c>1</c> 表示保序（Affine / LogAffine / Quantile 必然为 1）；
        ''' 明显小于 1 说明所用的策略改变了基因之间的相对高低（例如 QuantileDelta），
        ''' 此时“基因 A 比基因 B 高”的结论不再可靠。无法计算时为 <c>NaN</c>。
        ''' </remarks>
        Public Property SpearmanBeforeAfter As Double = Double.NaN

        ''' <summary>对齐过程的文字说明（降级、单基因、常量基因等）。</summary>
        Public Property Notes As New List(Of String)

        ''' <summary>覆盖率 = <see cref="Matched"/> / <see cref="NGene"/>。</summary>
        Public ReadOnly Property Coverage As Double
            Get
                If NGene <= 0 Then
                    Return 0.0R
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

        ''' <summary>追加一条说明（空字符串会被忽略）。</summary>
        Public Sub AddNote(note As String)
            If Not String.IsNullOrEmpty(note) Then
                Call Notes.Add(note)
            End If
        End Sub

        ''' <summary>多行摘要。</summary>
        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine($"context alignment: {Mode} ({Matched}/{NGene} model genes covered, {Coverage:P1}), " &
                               $"{Provided} entries provided, {InvalidValues} invalid value(s) skipped")

            Dim unknown As String() = If(UnknownGenes, New String() {})

            If unknown.Length > 0 Then
                Call sb.AppendLine($"  {unknown.Length} gene name(s) are not in the model: {preview(unknown)}")
            End If
            If OutsideSupport > 0 Then
                Call sb.AppendLine($"  {OutsideSupport} gene(s) fall outside the training support" &
                                   If(ClampedGenes > 0, $" ({ClampedGenes} clamped to the boundary)", " (reported only)"))
            End If

            Dim missing As String() = If(MissingGenes, New String() {})

            If missing.Length > 0 Then
                Call sb.AppendLine($"  {missing.Length} model gene(s) keep the training baseline: {preview(missing)}")
            End If
            If Not Double.IsNaN(SpearmanBeforeAfter) AndAlso Aligned Then
                Call sb.AppendLine($"  rank correlation before/after alignment: {SpearmanBeforeAfter:F4}")
            End If

            For Each note As String In Notes
                Call sb.AppendLine($"  note: {note}")
            Next

            Return sb.ToString().TrimEnd()
        End Function

        Private Shared Function preview(names As String()) As String
            Return String.Join(", ", names.Take(10)) & If(names.Length > 10, ", ...", "")
        End Function

    End Class

End Namespace
