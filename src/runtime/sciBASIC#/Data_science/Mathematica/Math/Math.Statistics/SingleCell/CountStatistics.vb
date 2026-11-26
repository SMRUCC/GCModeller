Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports Microsoft.VisualBasic.Math.Statistics.OrderStatistics
Imports std = System.Math

Namespace SingleCell

    ''' <summary>
    ''' 计数矩阵的统计算子：分块 CPM 均值、深度分位、检出率、delete-one Jackknife 矩。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 所有算子都只做 <c>O(nnz)</c> 或 <c>O(nnz + chunk·G)</c> 的单遍扫描，
    ''' 不会把整张矩阵稠密化。需要"逐细胞稠密视图"的算法（如 Jackknife 矩）
    ''' 通过参数控制单次驻留的细胞块大小。
    ''' </para>
    ''' </remarks>
    Public Module CountStatistics

        ''' <summary>
        ''' 每个基因的平均每细胞 CPM。
        ''' </summary>
        ''' <param name="counts">细胞 × 基因的计数矩阵</param>
        ''' <param name="chunkSize">预留参数（本实现是稀疏单遍扫描，不需要分块）；负值会被忽略</param>
        ''' <returns>长度 = 基因数</returns>
        ''' <remarks>
        ''' <c>CPM(i, g) = 1e6 · x(i, g) / max(library(i), 1)</c>，
        ''' 零计数贡献 0，因此稀疏结构足以精确计算均值，不需要稠密化。
        ''' </remarks>
        Public Function ChunkedCpmMean(counts As CsrMatrix, Optional chunkSize As Integer = 2048) As Double()
            If counts Is Nothing Then Throw New ArgumentNullException(NameOf(counts))

            Dim cells As Integer = counts.Rows
            Dim genes As Integer = counts.Columns
            Dim indptr As Integer() = counts.Indptr
            Dim indices As Integer() = counts.Indices
            Dim values As Double() = counts.Values
            Dim accumulator As Double() = New Double(genes - 1) {}

            For cell As Integer = 0 To cells - 1
                Dim total As Double = 0.0

                For k As Integer = indptr(cell) To indptr(cell + 1) - 1
                    total += values(k)
                Next

                total = std.Max(total, 1.0)

                For k As Integer = indptr(cell) To indptr(cell + 1) - 1
                    accumulator(indices(k)) += 1000000.0 * values(k) / total
                Next
            Next

            If cells > 0 Then
                For g As Integer = 0 To genes - 1
                    accumulator(g) /= cells
                Next
            End If

            Return accumulator
        End Function

        ''' <summary>
        ''' 每个基因的检出率：CPM 超过阈值的细胞占比
        ''' </summary>
        ''' <param name="counts">细胞 × 基因的计数矩阵</param>
        ''' <param name="cpmThreshold">判定阈值</param>
        ''' <returns>长度 = 基因数，取值 <c>[0, 1]</c></returns>
        Public Function DetectionRate(counts As CsrMatrix, Optional cpmThreshold As Double = 5.0) As Double()
            If counts Is Nothing Then Throw New ArgumentNullException(NameOf(counts))

            Dim cells As Integer = counts.Rows
            Dim genes As Integer = counts.Columns
            Dim indptr As Integer() = counts.Indptr
            Dim indices As Integer() = counts.Indices
            Dim values As Double() = counts.Values
            Dim hits As Double() = New Double(genes - 1) {}

            For cell As Integer = 0 To cells - 1
                Dim total As Double = 0.0

                For k As Integer = indptr(cell) To indptr(cell + 1) - 1
                    total += values(k)
                Next

                total = std.Max(total, 1.0)

                For k As Integer = indptr(cell) To indptr(cell + 1) - 1
                    If 1000000.0 * values(k) / total > cpmThreshold Then
                        hits(indices(k)) += 1.0
                    End If
                Next
            Next

            If cells > 0 Then
                For g As Integer = 0 To genes - 1
                    hits(g) /= cells
                Next
            End If

            Return hits
        End Function

        ''' <summary>
        ''' 生成"每细胞 CPM"的稠密视图（细胞 × 基因）。
        ''' </summary>
        ''' <param name="counts">计数矩阵</param>
        ''' <param name="chunkSize">预留参数；本实现按行单遍扫描，不需要分块</param>
        ''' <returns>稠密数组，零计数位置的 CPM 为 0</returns>
        ''' <remarks>
        ''' 内存占用是 <c>细胞数 × 基因数</c> 个 double（例如 400 × 18000 ≈ 58 MB）。
        ''' 只应在"实验组样本量较小"的场景使用——这正是差异表达检验的典型场景。
        ''' </remarks>
        Public Function ToCpmGrid(counts As CsrMatrix, Optional chunkSize As Integer = -1) As Double()()
            If counts Is Nothing Then Throw New ArgumentNullException(NameOf(counts))

            Dim cells As Integer = counts.Rows
            Dim genes As Integer = counts.Columns
            Dim indptr As Integer() = counts.Indptr
            Dim indices As Integer() = counts.Indices
            Dim values As Double() = counts.Values
            Dim grid As Double()() = New Double(cells - 1)() {}

            For cell As Integer = 0 To cells - 1
                Dim row As Double() = New Double(genes - 1) {}
                Dim total As Double = 0.0

                For k As Integer = indptr(cell) To indptr(cell + 1) - 1
                    total += values(k)
                Next

                total = std.Max(total, 1.0)

                For k As Integer = indptr(cell) To indptr(cell + 1) - 1
                    row(indices(k)) = 1000000.0 * values(k) / total
                Next

                grid(cell) = row
            Next

            Return grid
        End Function

        ''' <summary>
        ''' 给定概率序列，对一组观测值取 R type-7 分位数
        ''' </summary>
        ''' <param name="values">观测值（顺序任意）</param>
        ''' <param name="probabilities">概率序列</param>
        ''' <returns>与 <paramref name="probabilities"/> 等长</returns>
        Public Function DepthQuantiles(values As Double(), probabilities As Double()) As Double()
            If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))
            If probabilities Is Nothing Then Throw New ArgumentNullException(NameOf(probabilities))
            If values.Length = 0 Then Return New Double(probabilities.Length - 1) {}

            Dim sorted As Double() = DirectCast(values.Clone(), Double())

            Call Array.Sort(sorted)

            Dim out As Double() = New Double(probabilities.Length - 1) {}

            For i As Integer = 0 To probabilities.Length - 1
                out(i) = Quantiles.Type7(sorted, probabilities(i))
            Next

            Return out
        End Function

        ''' <summary>
        ''' 目标库大小剖面：分层分位 → 缩放到总深度 → 幂次调整离散度 → 再缩放回总深度。
        ''' </summary>
        ''' <param name="librarySizes">对照组的每个细胞测序深度</param>
        ''' <param name="totalDepth">目标总深度（= 每个细胞的目标深度 × 细胞数）</param>
        ''' <param name="spread">幂次离散度（1 = 保持对照组的离散程度）</param>
        ''' <param name="nCells">目标细胞数</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 这是"在给定总深度下，让合成细胞的库大小分布与对照组同形"的标准做法：
        ''' 先取对照组库大小的分层分位，把总量对齐到目标深度，
        ''' 再用 <c>median·(v/median)^spread</c> 调整相对离散度，最后重新对齐总量
        ''' （幂变换会改变总和，因此必须再缩放一次）。
        ''' </remarks>
        Public Function LibraryProfile(librarySizes As Double(),
                                       totalDepth As Double,
                                       Optional spread As Double = 0.85,
                                       Optional nCells As Integer = 400) As QuantileProfile

            If librarySizes Is Nothing Then Throw New ArgumentNullException(NameOf(librarySizes))
            If nCells <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(nCells), nCells, "the cell count must be positive")

            ' 空细胞（库大小 0）会得到 0 的目标库大小，这里先剔除，避免全部目标库退化为 0
            Dim positive As New List(Of Double)(librarySizes.Length)

            For i As Integer = 0 To librarySizes.Length - 1
                If librarySizes(i) > 0.0 Then Call positive.Add(librarySizes(i))
            Next

            Dim source As Double() = If(positive.Count > 0, positive.ToArray, librarySizes)

            Dim profile As QuantileProfile = QuantileProfile.Create(source, nCells)

            profile = profile.RescaleTo(totalDepth)
            profile = profile.Spread(spread)

            Return profile.RescaleTo(totalDepth)
        End Function

        ''' <summary>
        ''' delete-one Jackknife 矩：<c>((n-1)/n) Σ_g Σ_i (v_ig - mean_g)²</c>，
        ''' 其中 <c>v_ig = log1p(TS · (P_g - x_ig) / (S - s_i))</c>。
        ''' </summary>
        ''' <param name="counts">细胞 × 基因的计数矩阵</param>
        ''' <param name="chunkSize">一次驻留的细胞数（控制内存：<c>chunkSize × 基因数</c> 个 double）</param>
        ''' <param name="normalizer">剖面归一化常数 TS，默认 50000</param>
        ''' <returns>
        ''' "合成 <c>n</c> 个细胞时，基因表达剖面相对留一法均值的总离散度"，
        ''' 用于与真实扰动细胞的对应量做比较
        ''' </returns>
        ''' <remarks>
        ''' <para>
        ''' 用"流式"实现避免存储 <c>n × G</c> 的浮点矩阵：
        ''' <c>Σ_i (v - mean)² = Σ v² - (Σ v)²/n</c>，因此只需要两组 <c>G</c> 长度的累加器。
        ''' </para>
        ''' <para>
        ''' 复杂度 <c>O(n·G)</c>（每个细胞都要遍历全部基因），与分块大小无关；
        ''' 分块只影响峰值内存。
        ''' </para>
        ''' </remarks>
        Public Function JackknifeMoment(counts As CsrMatrix,
                                        Optional chunkSize As Integer = 512,
                                        Optional normalizer As Double = 50000.0) As Double

            If counts Is Nothing Then Throw New ArgumentNullException(NameOf(counts))

            Dim cells As Integer = counts.Rows
            Dim genes As Integer = counts.Columns

            If cells < 2 OrElse genes = 0 Then Return 0.0

            Dim chunk As Integer = If(chunkSize <= 0, 512, chunkSize)
            Dim columnSums As Double() = counts.ColumnSums()
            Dim total As Double = 0.0

            For g As Integer = 0 To genes - 1
                total += columnSums(g)
            Next

            Dim accSquare As Double() = New Double(genes - 1) {}
            Dim accSum As Double() = New Double(genes - 1) {}
            Dim buffer As Double()() = New Double(std.Min(chunk, cells) - 1)() {}

            For i As Integer = 0 To buffer.Length - 1
                buffer(i) = New Double(genes - 1) {}
            Next

            Dim start As Integer = 0

            While start < cells
                Dim rows As Integer = std.Min(chunk, cells - start)

                ' 载入这一块细胞的稠密视图
                For i As Integer = 0 To rows - 1
                    Call Array.Clear(buffer(i), 0, genes)
                Next

                Dim indptr As Integer() = counts.Indptr
                Dim indices As Integer() = counts.Indices
                Dim values As Double() = counts.Values

                For i As Integer = 0 To rows - 1
                    Dim cell As Integer = start + i
                    Dim row As Double() = buffer(i)

                    For k As Integer = indptr(cell) To indptr(cell + 1) - 1
                        row(indices(k)) = values(k)
                    Next
                Next

                For i As Integer = 0 To rows - 1
                    Dim cell As Integer = start + i
                    Dim row As Double() = buffer(i)
                    Dim librarySize As Double = 0.0

                    For g As Integer = 0 To genes - 1
                        librarySize += row(g)
                    Next

                    Dim remaining As Double = std.Max(total - librarySize, 1.0E-12)

                    For g As Integer = 0 To genes - 1
                        Dim v As Double = std.Log(1.0 + normalizer * (columnSums(g) - row(g)) / remaining)

                        accSquare(g) += v * v
                        accSum(g) += v
                    Next
                Next

                start += rows
            End While

            Dim n As Double = cells
            Dim moment As Double = 0.0

            For g As Integer = 0 To genes - 1
                moment += accSquare(g) - accSum(g) * accSum(g) / n
            Next

            Return (n - 1.0) / n * moment
        End Function

        ''' <summary>
        ''' delete-one Jackknife 方差（均值的留一法方差）：<c>((n-1)/n) Σ (x_i - mean)²</c>
        ''' </summary>
        ''' <param name="values"></param>
        ''' <returns>样本少于 2 个时返回 0</returns>
        Public Function LeaveOneOutVariance(values As Double()) As Double
            If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))
            If values.Length < 2 Then Return 0.0

            Dim n As Double = values.Length
            Dim mean As Double = 0.0

            For i As Integer = 0 To values.Length - 1
                mean += values(i)
            Next

            mean /= n

            Dim sum As Double = 0.0

            For i As Integer = 0 To values.Length - 1
                Dim d As Double = values(i) - mean

                sum += d * d
            Next

            Return (n - 1.0) / n * sum
        End Function

    End Module
End Namespace
