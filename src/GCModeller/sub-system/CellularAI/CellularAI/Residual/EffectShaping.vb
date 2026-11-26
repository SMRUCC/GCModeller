Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports std = System.Math

Namespace Residual

    ''' <summary>
    ''' 残差修正的"形状约束"：比值限幅、符号保持、与原始证据的一致性融合。
    ''' </summary>
    ''' <remarks>
    ''' 残差模型输出的是"修正量"，直接相加会让某些基因的效应翻倍或翻符号，
    ''' 而符号翻转在生物学上是灾难性的（把上调说成下调）。因此这里用三道约束：
    ''' </remarks>
    ''' <list type="number">
    ''' <item><description><b>比值限幅</b>：修正量按对数比 <c>exp(correction/query)</c> 作用，
    ''' 对数比饱和在 <c>±ln2</c>，因此最终幅度被限制在原值的 <c>[1/2, 2]</c> 倍。</description></item>
    ''' <item><description><b>符号保持</b>：任何使符号翻转或归零的修正都被回滚。</description></item>
    ''' <item><description><b>一致性融合</b>：修正后的剖面按 <c>w = 2uv/(u²+v²)</c>
    ''' （<c>u</c>、<c>v</c> 为原始与修正量归一后的幅度）拉回原始证据，
    ''' 两者量级越接近、方向越一致，融合权重越大。</description></item>
    ''' </list>
    Public Module EffectShaping

        ''' <summary>
        ''' 比值限幅：<c>query·exp(correction/query)</c>，对数比饱和在 ±ln2，因子限制在 <c>[1/2, 2]</c>。
        ''' </summary>
        Public Function BoundedRatio(query As Double()(), correction As Double()()) As Double()()
            Dim nGenes As Integer = query.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, query(0).Length)
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)
            Dim bound As Double = std.Log(2.0)

            For i As Integer = 0 To nGenes - 1
                For j As Integer = 0 To nPerts - 1
                    Dim parent As Double = query(i)(j)
                    Dim delta As Double = correction(i)(j)

                    If parent = 0.0 OrElse delta = 0.0 Then
                        result(i)(j) = parent
                        Continue For
                    End If

                    Dim exponent As Double

                    If std.Abs(delta) >= bound * std.Abs(parent) Then
                        Dim same As Boolean = (delta > 0.0) = (parent > 0.0)

                        exponent = If(same, bound, -bound)
                    Else
                        exponent = delta / parent
                    End If

                    Dim magnitude As Double = std.Abs(parent) * std.Exp(exponent)
                    Dim lower As Double = 0.5 * std.Abs(parent)
                    Dim upper As Double = 2.0 * std.Abs(parent)

                    If magnitude = 0.0 Then
                        magnitude = std.Abs(parent)
                    End If

                    result(i)(j) = std.Copysign(std.Max(std.Min(magnitude, upper), lower), parent)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 符号保持：翻转符号或归零的修正回滚为原值，自身靶始终保留原值。
        ''' </summary>
        Public Function PreserveSigns(query As Double()(), adjusted As Double()(),
                                      genes As String(), perts As String()) As Double()()
            Dim nGenes As Integer = query.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, query(0).Length)
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For i As Integer = 0 To nGenes - 1
                For j As Integer = 0 To nPerts - 1
                    Dim keep As Boolean = ((query(i)(j) > 0.0) AndAlso (adjusted(i)(j) > 0.0)) OrElse
                                          ((query(i)(j) < 0.0) AndAlso (adjusted(i)(j) < 0.0))

                    result(i)(j) = If(keep, adjusted(i)(j), query(i)(j))
                Next
            Next

            Dim index As New Dictionary(Of String, Integer)(StringComparer.Ordinal)

            For i As Integer = 0 To nGenes - 1
                index(genes(i)) = i
            Next

            For j As Integer = 0 To nPerts - 1
                Dim at As Integer = -1

                If index.TryGetValue(perts(j), at) Then
                    result(at)(j) = query(at)(j)
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 证据一致性融合：<c>a = x + w·(p − x)</c>，<c>w = 2uv/(u²+v²)</c>（仅同号项）。
        ''' </summary>
        ''' <param name="x">池化证据</param>
        ''' <param name="t">总量设计使用的证据</param>
        ''' <param name="p">残差修正后的剖面</param>
        Public Function EvidenceAgreement(x As Double()(), t As Double()(), p As Double()()) As Double()()
            Dim nGenes As Integer = x.Length
            Dim nPerts As Integer = If(nGenes = 0, 0, x(0).Length)
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nGenes, nPerts)

            For i As Integer = 0 To nGenes - 1
                For j As Integer = 0 To nPerts - 1
                    Dim xi As Double = x(i)(j)
                    Dim ti As Double = t(i)(j)
                    Dim pi As Double = p(i)(j)

                    result(i)(j) = xi

                    If pi = xi Then
                        Continue For
                    End If

                    Dim same As Boolean = ((xi > 0.0) AndAlso (ti > 0.0)) OrElse ((xi < 0.0) AndAlso (ti < 0.0))

                    If Not same Then
                        Continue For
                    End If

                    Dim m As Double = std.Max(std.Abs(xi), std.Abs(ti))

                    If m <= 0.0 Then
                        Continue For
                    End If

                    Dim u As Double = std.Abs(xi) / m
                    Dim v As Double = std.Abs(ti) / m
                    Dim denominator As Double = u * u + v * v
                    Dim w As Double = If(denominator > 0.0, std.Max(std.Min(2.0 * u * v / denominator, 1.0), 0.0), 0.0)

                    If w <= 0.0 Then
                        Continue For
                    End If
                    If w >= 1.0 Then
                        result(i)(j) = pi
                        Continue For
                    End If

                    Dim delta As Double = pi - xi
                    Dim value As Double = xi + w * delta

                    If Not Double.IsFinite(value) OrElse Not Double.IsFinite(delta) Then
                        Dim scale As Double = std.Max(std.Abs(xi), std.Abs(pi))

                        value = If(scale > 0.0, scale * ((1.0 - w) * (xi / scale) + w * (pi / scale)), xi)
                    End If

                    ' 融合结果必须落在两个端点之间，并且不允许符号翻转
                    Dim low As Double = std.Min(xi, pi)
                    Dim high As Double = std.Max(xi, pi)

                    value = std.Max(std.Min(value, high), low)

                    Dim rollback As Boolean = ((xi <> 0.0) AndAlso (value = 0.0)) OrElse
                                              ((xi > 0.0) AndAlso (value < 0.0)) OrElse
                                              ((xi < 0.0) AndAlso (value > 0.0))

                    result(i)(j) = If(rollback, xi, value)
                Next
            Next

            Return result
        End Function

    End Module
End Namespace
