Imports Microsoft.VisualBasic.Linq
Imports PAdjust = Microsoft.VisualBasic.Math.FalseDiscoveryRate
Imports std = System.Math

Namespace KOBAS

    ''' <summary>
    ''' 多重检验校正方法，对应 KOBAS 的 <c>-n/--fdr</c> 参数。
    ''' </summary>
    Public Enum FdrMethod
        ''' <summary>Benjamini and Hochberg (1995)，对应 R <c>p.adjust(method = "BH")</c>。</summary>
        BH
        ''' <summary>Benjamini and Yekutieli (2001)，对应 R <c>p.adjust(method = "BY")</c>。</summary>
        BY
        ''' <summary>Storey 的 q 值方法，对应 R <c>qvalue</c> 包。</summary>
        QValue
        ''' <summary>不做校正。</summary>
        None
    End Enum

    ''' <summary>
    ''' 多重检验校正（KOBAS <c>discover.py</c> 中 <c>fdr_BH</c> / <c>fdr_BY</c> /
    ''' <c>fdr_QVALUE</c> 的 VB.NET 移植）。
    ''' </summary>
    ''' <remarks>
    ''' BH/BY 直接复用 <c>Microsoft.VisualBasic.Math.FalseDiscoveryRate</c> 之中
    ''' 已经与 R <c>p.adjust</c> 对齐的实现。QVALUE 在 KOBAS 中通过 R 的
    ''' <c>qvalue</c> 包计算，这里用 Storey 的 π0 估计（固定 λ）本地实现；
    ''' R 的 <c>qvalue</c> 默认会在 λ 网格上做平滑，因此两者在 π0 的估计上
    ''' 可能存在细微差异。
    ''' </remarks>
    Public Module FdrAdjust

        ''' <summary>
        ''' 对 p 值集合执行多重检验校正。
        ''' </summary>
        ''' <param name="pvalues">原始 p 值。</param>
        ''' <param name="method">校正方法，默认 BH。</param>
        ''' <returns>与输入同序的校正后 p 值（q 值），上限截断为 1。</returns>
        Public Function Adjust(pvalues As IEnumerable(Of Double), Optional method As FdrMethod = FdrMethod.BH) As Double()
            Dim values As Double() = pvalues.ToArray

            Select Case method
                Case FdrMethod.BH
                    Return PAdjust.BHCorrection(values)
                Case FdrMethod.BY
                    Return PAdjust.BenjaminiYekutieliCorrection(values)
                Case FdrMethod.QValue
                    Return StoreyQValues(values)
                Case Else
                    Return DirectCast(values.Clone(), Double())
            End Select
        End Function

        ''' <summary>
        ''' 使用 Storey 的 π0 估计计算 q 值。
        ''' </summary>
        ''' <param name="pvalues">原始 p 值。</param>
        ''' <param name="lambda">π0 估计所使用的阈值，默认 0.5。</param>
        ''' <returns>与输入同序的 q 值。</returns>
        Public Function StoreyQValues(pvalues As IEnumerable(Of Double), Optional lambda As Double = 0.5) As Double()
            Dim values As Double() = pvalues.ToArray
            Dim m As Integer = values.Length

            If m = 0 Then
                Return New Double() {}
            End If

            Dim above As Integer = values.Where(Function(p) p > lambda).Count()
            Dim pi0 As Double = above / ((1 - lambda) * m)

            pi0 = std.Min(1.0, std.Max(0.0, pi0))

            Dim order As Integer() = Enumerable.Range(0, m) _
                .OrderBy(Function(i) values(i)) _
                .ToArray
            Dim q As Double() = New Double(m - 1) {}
            Dim running As Double = 1.0

            For rank As Integer = m To 1 Step -1
                Dim index As Integer = order(rank - 1)
                Dim raw As Double = pi0 * m * values(index) / rank

                If raw < running Then
                    running = raw
                End If

                q(index) = std.Min(1.0, running)
            Next

            Return q
        End Function

        ''' <summary>
        ''' 把命令行之中的校正方法名称解析为 <see cref="FdrMethod"/>。
        ''' </summary>
        ''' <param name="name">
        ''' 方法名称，不区分大小写：<c>BH</c>、<c>BY</c>、<c>QVALUE</c>、<c>None</c>。
        ''' </param>
        ''' <returns></returns>
        ''' <exception cref="ArgumentException">方法名称不受支持时抛出。</exception>
        Public Function Parse(name As String) As FdrMethod
            Select Case If(name, "").Trim().ToUpperInvariant()
                Case "BH", "BENJAMINIHOCHBERG"
                    Return FdrMethod.BH
                Case "BY", "BENJAMINIYEKUTIELI"
                    Return FdrMethod.BY
                Case "QVALUE", "Q"
                    Return FdrMethod.QValue
                Case "NONE", ""
                    Return FdrMethod.None
                Case Else
                    Throw New ArgumentException($"Unsupported FDR correction method: '{name}'.")
            End Select
        End Function
    End Module
End Namespace
