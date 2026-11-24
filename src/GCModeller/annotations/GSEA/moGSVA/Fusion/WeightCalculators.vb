Imports std = System.Math

Namespace Fusion

    ''' <summary>
    ''' 组学权重的计算器（method.md 步骤4 的「组学加权融合策略」）。
    ''' </summary>
    ''' <remarks>
    ''' 所有计算都在<b>单条通路</b>的分数向量上进行：
    ''' 输入是该通路在各组学层上的 ES 向量，输出是各层权重且保证 Σw = 1。
    ''' 任何异常情况（全零、NaN、无穷）都会回退到等权，绝不产生 NaN。
    ''' </remarks>
    Public Module WeightCalculators

        ''' <summary>
        ''' 计算某条通路的组学权重。
        ''' </summary>
        ''' <param name="vectors">omics → 该通路的 ES 向量（长度 = 样本数）</param>
        ''' <param name="memberCount">omics → 该通路的成员数 |S_k|</param>
        ''' <param name="method">融合策略</param>
        ''' <param name="balanceFeatureSize">是否叠加特征数平衡项</param>
        ''' <returns>omics → 归一化后的权重</returns>
        Public Function Compute(vectors As Dictionary(Of String, Double()),
                                memberCount As Dictionary(Of String, Integer),
                                method As FusionMethods,
                                balanceFeatureSize As Boolean) As Dictionary(Of String, Double)

            Dim names As String() = vectors.Keys.ToArray
            Dim scores As New Dictionary(Of String, Double)(StringComparer.Ordinal)

            If names.Length = 0 Then
                Return scores
            End If
            If names.Length = 1 Then
                scores(names(0)) = 1.0
                Return scores
            End If

            For Each name As String In names
                Dim v As Double() = vectors(name)

                Select Case method
                    Case FusionMethods.Equal
                        scores(name) = 1.0

                    Case FusionMethods.Variance
                        scores(name) = 1.0 / (VarianceOf(v) + 1.0E-12)

                    Case FusionMethods.Entropy
                        scores(name) = BinnedEntropy(v)

                    Case FusionMethods.MutualInformation
                        Dim total As Double = 0
                        Dim pairs As Integer = 0

                        For Each other As String In names
                            If other = name Then
                                Continue For
                            End If

                            total += MutualInformation(v, vectors(other))
                            pairs += 1
                        Next

                        scores(name) = If(pairs = 0, 1.0, total / pairs)

                    Case FusionMethods.Balanced
                        scores(name) = BalanceFactor(MemberCountOf(memberCount, name))

                    Case Else
                        scores(name) = 1.0
                End Select
            Next

            If balanceFeatureSize AndAlso method <> FusionMethods.Balanced Then
                For Each name As String In names
                    scores(name) *= BalanceFactor(MemberCountOf(memberCount, name))
                Next
            End If

            Return Normalize(scores, names)
        End Function

        ''' <summary>
        ''' 组学特征数平衡项 <c>1 / log2(2 + |S_k|)</c>。
        ''' </summary>
        Public Function BalanceFactor(memberCount As Integer) As Double
            If memberCount < 0 Then
                memberCount = 0
            End If

            Return 1.0 / std.Log(2.0 + memberCount, 2.0)
        End Function

        ''' <summary>
        ''' 归一化权重向量；异常时回退等权。
        ''' </summary>
        Public Function Normalize(scores As Dictionary(Of String, Double), names As String()) As Dictionary(Of String, Double)
            Dim clean As New Dictionary(Of String, Double)(StringComparer.Ordinal)
            Dim total As Double = 0

            For Each name As String In names
                Dim w As Double

                If Not scores.TryGetValue(name, w) Then
                    w = 0
                End If
                If Double.IsNaN(w) OrElse Double.IsInfinity(w) OrElse w < 0 Then
                    w = 0
                End If

                clean(name) = w
                total += w
            Next

            If total <= 0 OrElse Double.IsNaN(total) OrElse Double.IsInfinity(total) Then
                Dim w As Double = 1.0 / names.Length

                For Each name As String In names
                    clean(name) = w
                Next

                Return clean
            End If

            For Each name As String In names
                clean(name) /= total
            Next

            Return clean
        End Function

        ''' <summary>
        ''' 样本方差（无偏估计，n-1）。
        ''' </summary>
        Public Function VarianceOf(v As Double()) As Double
            If v Is Nothing OrElse v.Length < 2 Then
                Return 0
            End If

            Dim mean As Double = 0

            For Each x As Double In v
                mean += x
            Next

            mean /= v.Length

            Dim ss As Double = 0

            For Each x As Double In v
                ss += (x - mean) ^ 2
            Next

            Return ss / (v.Length - 1)
        End Function

        ''' <summary>
        ''' 分箱香农熵（自然对数），取值范围越大、分布越均匀则熵越大。
        ''' </summary>
        Public Function BinnedEntropy(v As Double(), Optional bins As Integer = 10) As Double
            Dim lo As Double = Double.MaxValue
            Dim hi As Double = Double.MinValue
            Dim n As Integer = 0

            For Each x As Double In v
                If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then
                    Continue For
                End If
                If x < lo Then
                    lo = x
                End If
                If x > hi Then
                    hi = x
                End If
                n += 1
            Next

            If n = 0 OrElse hi <= lo Then
                Return 0
            End If

            Dim counts As Integer() = New Integer(bins - 1) {}

            For Each x As Double In v
                If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then
                    Continue For
                End If

                Dim b As Integer = CInt((x - lo) / (hi - lo) * (bins - 1))

                If b < 0 Then
                    b = 0
                ElseIf b > bins - 1 Then
                    b = bins - 1
                End If

                counts(b) += 1
            Next

            Dim h As Double = 0

            For Each c As Integer In counts
                If c > 0 Then
                    Dim p As Double = c / n

                    h -= p * std.Log(p)
                End If
            Next

            Return h
        End Function

        ''' <summary>
        ''' 两个分数向量之间的分箱互信息（自然对数）。
        ''' </summary>
        Public Function MutualInformation(x As Double(), y As Double(), Optional bins As Integer = 8) As Double
            If x Is Nothing OrElse y Is Nothing Then
                Return 0
            End If

            Dim n As Integer = std.Min(x.Length, y.Length)

            If n < 2 Then
                Return 0
            End If

            Dim bx As Integer() = Binize(x, bins)
            Dim by As Integer() = Binize(y, bins)
            Dim cntX As Integer() = New Integer(bins - 1) {}
            Dim cntY As Integer() = New Integer(bins - 1) {}
            Dim cntXY As Integer() = New Integer(bins * bins - 1) {}
            Dim total As Integer = 0

            For i As Integer = 0 To n - 1
                If bx(i) < 0 OrElse by(i) < 0 Then
                    Continue For
                End If

                cntX(bx(i)) += 1
                cntY(by(i)) += 1
                cntXY(bx(i) * bins + by(i)) += 1
                total += 1
            Next

            If total = 0 Then
                Return 0
            End If

            Dim mi As Double = 0

            For a As Integer = 0 To bins - 1
                For b As Integer = 0 To bins - 1
                    Dim c As Integer = cntXY(a * bins + b)

                    If c = 0 Then
                        Continue For
                    End If

                    Dim pxy As Double = c / total
                    Dim px As Double = cntX(a) / total
                    Dim py As Double = cntY(b) / total

                    mi += pxy * std.Log(pxy / (px * py))
                Next
            Next

            Return mi
        End Function

        Private Function Binize(v As Double(), bins As Integer) As Integer()
            Dim result As Integer() = New Integer(v.Length - 1) {}
            Dim lo As Double = Double.MaxValue
            Dim hi As Double = Double.MinValue

            For Each x As Double In v
                If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then
                    Continue For
                End If
                If x < lo Then
                    lo = x
                End If
                If x > hi Then
                    hi = x
                End If
            Next

            If hi <= lo Then
                For i As Integer = 0 To v.Length - 1
                    result(i) = If(Double.IsNaN(v(i)) OrElse Double.IsInfinity(v(i)), -1, 0)
                Next

                Return result
            End If

            For i As Integer = 0 To v.Length - 1
                Dim x As Double = v(i)

                If Double.IsNaN(x) OrElse Double.IsInfinity(x) Then
                    result(i) = -1
                    Continue For
                End If

                Dim b As Integer = CInt((x - lo) / (hi - lo) * (bins - 1))

                If b < 0 Then
                    b = 0
                ElseIf b > bins - 1 Then
                    b = bins - 1
                End If

                result(i) = b
            Next

            Return result
        End Function

        Private Function MemberCountOf(memberCount As Dictionary(Of String, Integer), name As String) As Integer
            Dim n As Integer

            If memberCount IsNot Nothing AndAlso memberCount.TryGetValue(name, n) Then
                Return n
            Else
                Return 0
            End If
        End Function

    End Module

End Namespace
