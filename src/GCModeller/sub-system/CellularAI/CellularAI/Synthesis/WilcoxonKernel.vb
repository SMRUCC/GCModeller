Imports std = System.Math

Namespace Synthesis

    ''' <summary>
    ''' 精确秩和 z 的快速内核：把合成细胞的计数换成 CPM 栅格后，
    ''' 在一次线性扫描内完成"并列合并 + 对照侧上下界定位 + 方差校正"。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>为什么不用 <see cref="Hypothesis.Wilcoxon"/> 的逐基因实现</b>：那条路径会给每个基因
    ''' 建一次 <c>Dictionary</c> 去统计并列，单次调用很便宜；但合成内层"每扰动 × 每基因 × 数十次二分"
    ''' 的量级是 <c>10⁸</c> 次调用，字典的分配与哈希会成为瓶颈。这里的实现只做三件事：
    ''' 把样本排序、按游程（相同 CPM 的连续段）合并、对每个游程在对照序列上二分定位。
    ''' </para>
    ''' <para>
    ''' <b>float32 栅格</b>：CPM 先转成 <c>Single</c> 再参与比较。上游这么做是因为
    ''' 真实数据的 CPM 精度本来就只有 7 位有效数字，用 double 比较会把"本该并列"的值判成不同，
    ''' 从而系统性低估并列数、高估 z。这里的 <c>CSng</c> 往返正是为了复现这个语义。
    ''' </para>
    ''' <para>
    ''' <b>并列校正</b>：<c>tc = 1 − (tie_control + adj)/(N³−N)</c>，其中 <c>adj</c> 由每个样本游程
    ''' 与对照重叠数的立方差累加而来；它与对照侧自带的 <c>tie_control</c> 一起修正方差。
    ''' 漏掉这一项会让 z 在大量零计数的小基因上明显偏大。
    ''' </para>
    ''' </remarks>
    Public Module WilcoxonKernel

        ''' <summary>
        ''' 精确秩和 z（带并列校正）。
        ''' </summary>
        ''' <param name="counts">该扰动下每个细胞的基因计数（长度 n）</param>
        ''' <param name="librarySizes">每个细胞的总计数（长度 n）</param>
        ''' <param name="controlCpm">对照侧已排序的 CPM（长度 m，升序）</param>
        ''' <param name="controlTieSum">对照侧的并列和</param>
        ''' <returns>带符号的 z；样本与对照都没有可用离散度时返回 0</returns>
        Public Function ZExact(counts As Double(), librarySizes As Double(),
                               controlCpm As Double(), controlTieSum As Double) As Double
            Dim n As Integer = counts.Length
            Dim m As Integer = If(controlCpm Is Nothing, 0, controlCpm.Length)

            If n = 0 OrElse m = 0 Then
                Return 0.0
            End If

            ' 1) CPM 栅格（float32 往返）
            Dim grid(n - 1) As Single

            For c As Integer = 0 To n - 1
                Dim value As Double = If(librarySizes(c) > 0.0, 1000000.0 * counts(c) / librarySizes(c), 0.0)

                grid(c) = CSng(value)
            Next

            Dim sorted As Single() = DirectCast(grid.Clone(), Single())

            Call Array.Sort(sorted)

            ' 2) 游程扫描 + 上下界定位
            Dim u As Double = 0.0
            Dim adjustment As Double = 0.0
            Dim i As Integer = 0

            While i < n
                Dim v As Single = sorted(i)
                Dim j As Integer = i

                While j + 1 < n AndAlso sorted(j + 1) = v
                    j += 1
                End While

                Dim c As Double = j - i + 1
                Dim lo As Integer = LowerBound(controlCpm, v)
                Dim hi As Integer = UpperBound(controlCpm, v)
                Dim overlap As Double = hi - lo

                u += c * (lo + 0.5 * (hi - lo))

                Dim merged As Double = overlap + c

                adjustment += (merged * merged * merged - merged) - (overlap * overlap * overlap - overlap)

                i = j + 1
            End While

            Dim total As Double = m + n
            Dim correction As Double = 1.0 - (controlTieSum + adjustment) / (total * total * total - total)
            Dim variance As Double = correction * n * m * (total + 1.0) / 12.0

            If variance <= 1.0E-24 Then
                Return 0.0
            End If

            Return (u - n * m / 2.0) / std.Sqrt(variance)
        End Function

        ''' <summary>第一个 <c>≥ v</c> 的位置。</summary>
        Private Function LowerBound(values As Double(), v As Single) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = values.Length

            While lo < hi
                Dim mid As Integer = (lo + hi) \ 2

                If values(mid) < v Then
                    lo = mid + 1
                Else
                    hi = mid
                End If
            End While

            Return lo
        End Function

        ''' <summary>第一个 <c>&gt; v</c> 的位置。</summary>
        Private Function UpperBound(values As Double(), v As Single) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = values.Length

            While lo < hi
                Dim mid As Integer = (lo + hi) \ 2

                If values(mid) <= v Then
                    lo = mid + 1
                Else
                    hi = mid
                End If
            End While

            Return lo
        End Function

    End Module
End Namespace
