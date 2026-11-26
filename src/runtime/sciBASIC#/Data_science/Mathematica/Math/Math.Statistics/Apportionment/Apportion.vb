Imports sysmath = System.Math

Namespace Apportionment

    ''' <summary>
    ''' 整数分摊：把连续的"比例/份额"转成"总和精确等于给定值的非负整数"。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>最大余数法（Hamilton 法）</b>：先按比例取整（向下），再把剩余的 <c>total - Σfloor</c>
    ''' 个单位依次分给小数部分最大的项。这保证了
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>总和精确等于给定目标值；</description></item>
    ''' <item><description>每项与理想值的偏差小于 1；</description></item>
    ''' <item><description>结果只依赖输入（并列时按下标稳定排序），可复现。</description></item>
    ''' </list>
    ''' <para>
    ''' <b>质量守恒调整</b> <see cref="ConserveMass"/> 处理相反的问题：已经有一组整数，
    ''' 但总和与目标不符。补足按给定顺序均摊；削减则从最大值往下削平（水位法），
    ''' 保证结果非负且总量精确。
    ''' </para>
    ''' </remarks>
    Public Module Apportion

        ''' <summary>
        ''' 最大余数法：把比例向量分摊成总和为 <paramref name="total"/> 的非负整数
        ''' </summary>
        ''' <param name="fractions">非负比例（不要求归一化）</param>
        ''' <param name="total">目标总和</param>
        ''' <returns>长度与输入一致的整数数组</returns>
        ''' <exception cref="ArgumentException">比例之和为 0 或含负值，或 <paramref name="total"/> 为负</exception>
        Public Function LargestRemainder(fractions As Double(), total As Long) As Long()
            If fractions Is Nothing Then Throw New ArgumentNullException(NameOf(fractions))
            If total < 0 Then Throw New ArgumentOutOfRangeException(NameOf(total), total, "the target total must not be negative")
            If fractions.Length = 0 Then
                If total = 0 Then Return New Long() {}

                Throw New ArgumentException("cannot apportion a positive total onto an empty fraction vector", NameOf(fractions))
            End If

            Dim sum As Double = 0.0

            For i As Integer = 0 To fractions.Length - 1
                Dim f As Double = fractions(i)

                If Double.IsNaN(f) OrElse f < 0.0 Then
                    Throw New ArgumentException($"fraction at index {i} is {(If(Double.IsNaN(f), "NaN", f.ToString("G6")))}, must be finite and non-negative", NameOf(fractions))
                End If

                sum += f
            Next

            If sum <= 0.0 Then
                Throw New ArgumentException("the fractions sum to zero, there is no mass to apportion", NameOf(fractions))
            End If

            Dim counts As Long() = New Long(fractions.Length - 1) {}
            Dim remainders As Double() = New Double(fractions.Length - 1) {}
            Dim assigned As Long = 0

            For i As Integer = 0 To fractions.Length - 1
                Dim ideal As Double = fractions(i) / sum * total
                Dim floor As Long = CLng(sysmath.Floor(ideal))

                counts(i) = floor
                remainders(i) = ideal - floor
                assigned += floor
            Next

            Dim residual As Long = total - assigned

            If residual > 0 Then
                ' 按小数部分降序；并列时按下标升序，保证结果确定
                Dim order As Integer() = New Integer(fractions.Length - 1) {}

                For i As Integer = 0 To order.Length - 1
                    order(i) = i
                Next

                Dim comparer As New Comparison(Of Integer)(
                    Function(a As Integer, b As Integer)
                        Dim c As Integer = remainders(b).CompareTo(remainders(a))

                        If c <> 0 Then Return c

                        Return a.CompareTo(b)
                    End Function)

                Call Array.Sort(order, comparer)

                For i As Integer = 0 To CInt(residual) - 1
                    counts(order(i Mod order.Length)) += 1
                Next
            End If

            Return counts
        End Function

        ''' <summary>
        ''' 把一组整数调整到指定总和（非负、总量精确）
        ''' </summary>
        ''' <param name="counts">当前整数（不会被修改）</param>
        ''' <param name="target">目标总和</param>
        ''' <param name="order">
        ''' 补足时的优先顺序（例如按重要性/名次排序）；<c>Nothing</c> 表示按下标顺序
        ''' </param>
        ''' <returns>新的整数数组</returns>
        ''' <remarks>
        ''' 削减采用水位法：从最大值开始逐个削平，因此不会出现负值，
        ''' 也不会把某一项削减到远低于其他项（与"反复减最大值"相比是 <c>O(n log n)</c> 而不是
        ''' <c>O(超出量)</c>）。
        ''' </remarks>
        ''' <exception cref="ArgumentException">目标为负，或总和不足而无法在非负约束下削到目标</exception>
        Public Function ConserveMass(counts As Long(), target As Long, Optional order As Integer() = Nothing) As Long()
            If counts Is Nothing Then Throw New ArgumentNullException(NameOf(counts))
            If target < 0 Then Throw New ArgumentOutOfRangeException(NameOf(target), target, "the target total must not be negative")

            Dim out As Long() = New Long(counts.Length - 1) {}
            Dim sum As Long = 0

            For i As Integer = 0 To counts.Length - 1
                If counts(i) < 0 Then
                    Throw New ArgumentException($"count at index {i} is negative ({counts(i)})", NameOf(counts))
                End If

                out(i) = counts(i)
                sum += counts(i)
            Next

            If counts.Length = 0 Then
                If target = 0 Then Return out

                Throw New ArgumentException("cannot distribute a positive target onto an empty vector", NameOf(counts))
            End If

            If sum = target Then Return out

            If sum < target Then
                Dim sequence As Integer() = If(order, Identity(counts.Length))
                Dim deficit As Long = target - sum
                Dim share As Long = deficit \ sequence.Length
                Dim extra As Long = deficit - share * sequence.Length

                For i As Integer = 0 To sequence.Length - 1
                    Dim index As Integer = sequence(i)

                    If index < 0 OrElse index >= out.Length Then
                        Throw New ArgumentException($"order index {index} is out of range [0, {out.Length})", NameOf(order))
                    End If

                    out(index) += share

                    If i < extra Then out(index) += 1
                Next

                Return out
            End If

            ' 需要削减：从最大值往下削平（水位法）
            Dim excess As Long = sum - target
            Dim indices As Integer() = Identity(out.Length)
            Dim comparer As New Comparison(Of Integer)(
                Function(a As Integer, b As Integer)
                    Dim c As Integer = out(b).CompareTo(out(a))

                    If c <> 0 Then Return c

                    Return a.CompareTo(b)
                End Function)

            Call Array.Sort(indices, comparer)

            Dim cumulative As Long = 0

            For i As Integer = 0 To indices.Length - 1
                cumulative += out(indices(i))

                Dim next1 As Long = If(i + 1 < indices.Length, out(indices(i + 1)), 0L)

                If cumulative - (i + 1) * next1 >= excess Then
                    Dim keep As Long = cumulative - excess
                    Dim level As Long = keep \ (i + 1)
                    Dim remainder As Long = keep - level * (i + 1)

                    For j As Integer = 0 To i
                        out(indices(j)) = level + If(j < remainder, 1L, 0L)
                    Next

                    Return out
                End If
            Next

            ' 理论上前面的循环一定会在某一步命中；走到这里说明输入不满足预期的守恒条件
            Throw New ArgumentException(
                $"cannot reduce the total from {sum} to {target} while keeping all counts non-negative", NameOf(target))
        End Function

        ''' <summary>
        ''' 线性缩放到指定总和
        ''' </summary>
        ''' <param name="values">非负值</param>
        ''' <param name="total">目标总和</param>
        ''' <returns>新数组；当前总和非正时返回全 0</returns>
        Public Function NormalizeToTotal(values As Double(), total As Double) As Double()
            If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))

            Dim out As Double() = New Double(values.Length - 1) {}
            Dim sum As Double = 0.0

            For i As Integer = 0 To values.Length - 1
                If values(i) > 0.0 Then sum += values(i)
            Next

            If sum <= 0.0 Then Return out

            Dim scale As Double = total / sum

            For i As Integer = 0 To values.Length - 1
                If values(i) > 0.0 Then out(i) = values(i) * scale
            Next

            Return out
        End Function

        ''' <summary>
        ''' 判定一组整数是否"非负且总和等于目标"
        ''' </summary>
        ''' <param name="counts"></param>
        ''' <param name="target"></param>
        ''' <returns></returns>
        Public Function IsIntegerMass(counts As Long(), target As Long) As Boolean
            If counts Is Nothing Then Return target = 0

            Dim sum As Long = 0

            For i As Integer = 0 To counts.Length - 1
                If counts(i) < 0 Then Return False

                sum += counts(i)
            Next

            Return sum = target
        End Function

        ''' <summary>
        ''' 生成 <c>0 .. n-1</c> 的下标数组
        ''' </summary>
        ''' <param name="n"></param>
        ''' <returns></returns>
        Public Function Identity(n As Integer) As Integer()
            If n < 0 Then Throw New ArgumentOutOfRangeException(NameOf(n), n, "the length must not be negative")

            Dim out As Integer() = New Integer(n - 1) {}

            For i As Integer = 0 To n - 1
                out(i) = i
            Next

            Return out
        End Function

    End Module
End Namespace
