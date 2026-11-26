Imports std = System.Math

Namespace Synthesis

    ''' <summary>
    ''' 由"分位锚点"生成整数计数，并保证<b>质量守恒</b>（总量精确等于设计总量）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 每个细胞拿到的是"对照 CPM 分层分位表"上的一个值：<c>idx = ⌊(shift + q − 0.5)·m⌋</c>，
    ''' 再按该细胞的文库大小折算成计数并四舍五入。这样得到的边缘分布与对照细胞一致，
    ''' 而 <c>q</c>（每个细胞的名次）由 copula 决定，因此基因之间、基因与文库大小之间
    ''' 都能保持想要的秩相关。
    ''' </para>
    ''' <para>
    ''' <b>总量通常不会自动对上</b>（四舍五入会漏一点或多一点），因此必须显式修正：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description><b>不足</b>：把差额平摊给分位最高的 <c>ntop</c> 个细胞（先各得 <c>⌊D/nd⌋</c>，
    ''' 余数按名次依次加 1），这样不会破坏"高表达细胞更高"的排序；</description></item>
    ''' <item><description><b>超出</b>：从最大的计数开始做"水位削平"——找到第一个满足
    ''' <c>Σ(前 i 个) − (i+1)·下一个值 ≥ E</c> 的前缀，把前缀整体压到同一个水位，
    ''' 余数同样按名次分配。这比"随机扣减"更稳定：不会把某个基因扣成 0 而破坏秩结构。</description></item>
    ''' </list>
    ''' </remarks>
    Public Module QuantileBuilder

        ''' <summary>
        ''' 由分位生成整数计数。
        ''' </summary>
        ''' <param name="q">每个细胞的名次分位（0~1，长度 n）</param>
        ''' <param name="ordering">按分位<b>降序</b>排列的细胞下标</param>
        ''' <param name="sortedCpm">对照侧已排序的 CPM（升序，长度 m）</param>
        ''' <param name="librarySizes">每个细胞的目标文库大小</param>
        ''' <param name="total">设计总量</param>
        ''' <param name="shift">分位平移量（外层二分求解的变量）</param>
        ''' <param name="ntop">质量不足时接受补足的细胞数上限</param>
        Public Function Build(q As Double(), ordering As Integer(), sortedCpm As Double(),
                              librarySizes As Double(), total As Double,
                              shift As Double, ntop As Integer) As Double()
            Dim n As Integer = q.Length
            Dim m As Integer = If(sortedCpm Is Nothing, 0, sortedCpm.Length)
            Dim counts(n - 1) As Double

            If n = 0 OrElse m = 0 Then
                Return counts
            End If

            Dim mass As Double = 0.0

            For c As Integer = 0 To n - 1
                Dim position As Double = shift + (q(c) - 0.5)
                Dim value As Double = 0.0

                If position > 0.0 Then
                    Dim index As Integer = CInt(std.Floor(position * m))

                    If index >= m Then
                        index = m - 1
                    End If
                    If index < 0 Then
                        index = 0
                    End If

                    value = sortedCpm(index)
                End If

                Dim scaled As Double = std.Floor(value * librarySizes(c) * 0.000001 + 0.5)

                counts(c) = If(scaled > 0.0, scaled, 0.0)
                mass += counts(c)
            Next

            If mass < total Then
                Call FillShortfall(counts, ordering, total - mass, ntop)
            ElseIf mass > total Then
                Call TrimExcess(counts, mass - total)
            End If

            Return counts
        End Function

        ''' <summary>
        ''' 稀疏化：只让 <c>nz = n − round(w·n)</c> 个细胞持有计数（总量不变），
        ''' 用于"该基因的 z 冲得太高、需要压低非零细胞数"的过冲修复。
        ''' </summary>
        Public Function BuildSparse(ordering As Integer(), total As Double, w As Double) As Double()
            Dim n As Integer = ordering.Length
            Dim counts(n - 1) As Double

            If n = 0 Then
                Return counts
            End If

            Dim nonzero As Integer = n - CInt(std.Floor(w * n + 0.5))

            If nonzero < 1 Then
                nonzero = 1
            End If

            Dim share As Double = std.Floor(total / nonzero)
            Dim remainder As Double = total - share * nonzero

            For j As Integer = 0 To nonzero - 1
                counts(ordering(j)) = share + If(j < remainder, 1.0, 0.0)
            Next

            Return counts
        End Function

        ''' <summary>
        ''' copula 混合：<c>e = r·z_l + √(1−r²)·e₀</c>，再按 <c>e</c> 的名次给出分位与降序下标。
        ''' </summary>
        ''' <param name="noise">独立的噪声样本（长度 n）</param>
        ''' <param name="libraryRank">文库大小的秩得分（长度 n）</param>
        ''' <param name="correlation">目标相关系数 <c>r</c></param>
        Public Function QuantilesFrom(noise As Double(), libraryRank As Double(),
                                      correlation As Double) As (Q As Double(), Ordering As Integer())
            Dim n As Integer = noise.Length
            Dim q(n - 1) As Double
            Dim ordering(n - 1) As Integer

            If n = 0 Then
                Return (q, ordering)
            End If

            Dim scale As Double = std.Sqrt(std.Max(1.0 - correlation * correlation, 0.0))
            Dim latent(n - 1) As Double

            For i As Integer = 0 To n - 1
                latent(i) = correlation * libraryRank(i) + scale * noise(i)
            Next

            Dim order As Integer() = Enumerable.Range(0, n).
                OrderBy(Function(i) latent(i)).ThenBy(Function(i) i).ToArray

            For i As Integer = 0 To n - 1
                q(order(i)) = (i + 0.5) / n
                ordering(i) = order(n - 1 - i)
            Next

            Return (q, ordering)
        End Function

        ''' <summary>该扰动下的平均 CPM（<c>1e6·Σ x/L / n</c>）。</summary>
        Public Function MeanCpm(counts As Double(), librarySizes As Double()) As Double
            If counts.Length = 0 Then
                Return 0.0
            End If

            Dim acc As Double = 0.0

            For c As Integer = 0 To counts.Length - 1
                If librarySizes(c) > 0.0 Then
                    acc += 1000000.0 * counts(c) / librarySizes(c)
                End If
            Next

            Return acc / counts.Length
        End Function

#Region "mass conservation"

        ''' <summary>总量不足：把差额平摊给分位最高的若干细胞。</summary>
        Private Sub FillShortfall(counts As Double(), ordering As Integer(), deficit As Double, ntop As Integer)
            Dim n As Integer = counts.Length
            Dim share As Integer = std.Min(ntop, CInt(deficit))

            If share > n Then
                share = n
            End If
            If share < 1 Then
                share = 1
            End If

            Dim perCell As Double = std.Floor(deficit / share)
            Dim remainder As Double = deficit - perCell * share

            For j As Integer = 0 To share - 1
                counts(ordering(j)) += perCell + If(j < remainder, 1.0, 0.0)
            Next
        End Sub

        ''' <summary>总量超出：从最大的计数开始做水位削平。</summary>
        Private Sub TrimExcess(counts As Double(), excess As Double)
            Dim n As Integer = counts.Length
            Dim order As Integer() = Enumerable.Range(0, n).
                OrderByDescending(Function(i) counts(i)).ThenBy(Function(i) i).ToArray
            Dim cumulative As Double = 0.0

            For i As Integer = 0 To n - 1
                cumulative += counts(order(i))

                Dim following As Double = If(i + 1 < n, counts(order(i + 1)), 0.0)

                If cumulative - (i + 1) * following >= excess Then
                    Dim keep As Double = cumulative - excess
                    Dim level As Double = std.Floor(keep / (i + 1))
                    Dim remainder As Double = keep - level * (i + 1)

                    For j As Integer = 0 To i
                        counts(order(j)) = level + If(j < remainder, 1.0, 0.0)
                    Next

                    Return
                End If
            Next
        End Sub

#End Region

    End Module
End Namespace
