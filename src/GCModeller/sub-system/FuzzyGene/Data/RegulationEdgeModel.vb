Imports System.Text

Namespace Data

    ''' <summary>
    ''' 由神经模糊模型读出的带符号调控边。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <see cref="Weight"/> 是靶基因模型在训练状态上对某个调控因子的平均局部灵敏度
    ''' <c>∂x̂_target / ∂x_regulator</c>，由结论系数按规则触发强度加权得到：
    ''' 正值表示激活，负值表示抑制，绝对值即调控强度。
    ''' </para>
    ''' <para>
    ''' <see cref="Confidence"/> 在该边来自先验网络时取先验置信度，否则取表达相关性
    ''' <c>|Pearson|</c>；<see cref="IsPrior"/> 标记这条边是否被先验网络支持，
    ''' 便于把「数据支持 + 先验支持」与「纯数据推断」分开汇报。
    ''' </para>
    ''' </remarks>
    Public Class RegulationEdgeModel

        ''' <summary>调控因子（上游）。</summary>
        Public Property Source As String

        ''' <summary>靶基因（下游）。</summary>
        Public Property Target As String

        ''' <summary>平均局部灵敏度：正为激活、负为抑制。</summary>
        Public Property Weight As Double

        ''' <summary>置信度：先验置信度，或数据相关性的绝对值。</summary>
        Public Property Confidence As Double

        ''' <summary>该边是否被先验调控网络支持。</summary>
        Public Property IsPrior As Boolean

        ''' <summary>等价的布尔语义：<c>True</c> 表示激活。</summary>
        Public ReadOnly Property IsActivation As Boolean
            Get
                Return Weight >= 0.0
            End Get
        End Property

        ''' <summary>SIF 行（<c>source  pp  target</c>）。</summary>
        Public Function ToSifLine() As String
            Return $"{Source}{ChrW(9)}pp{ChrW(9)}{Target}"
        End Function

        ''' <summary>CSV 表头。</summary>
        Public Shared Function CsvHeader() As String
            Return "Source,Target,Weight,Regulation,Confidence,IsPrior"
        End Function

        ''' <summary>CSV 行。</summary>
        Public Function ToCsvLine() As String
            Dim kind As String = If(IsActivation, "activation", "repression")

            Return String.Join(",", {
                Escape(Source),
                Escape(Target),
                Weight.ToString("G6"),
                kind,
                Confidence.ToString("G6"),
                If(IsPrior, "TRUE", "FALSE")
            })
        End Function

        ''' <summary>可读摘要。</summary>
        Public Overrides Function ToString() As String
            Dim arrow As String = If(IsActivation, "->", "-|")

            Return $"{Source} {arrow} {Target} (w={Weight:G4}, conf={Confidence:G3}, prior={IsPrior})"
        End Function

        Private Shared Function Escape(text As String) As String
            Dim value As String = If(text, "")
            Dim sb As New StringBuilder(value.Length + 2)
            Dim quoted As Boolean = value.IndexOfAny(New Char() {","c, """"c, ChrW(10), ChrW(13)}) >= 0

            If quoted Then
                Call sb.Append(""""c)
            End If

            For Each ch As Char In value
                If ch = """"c Then
                    Call sb.Append(""""c)
                End If

                Call sb.Append(ch)
            Next

            If quoted Then
                Call sb.Append(""""c)
            End If

            Return sb.ToString()
        End Function

    End Class

End Namespace
