Imports std = System.Math
Imports SMRUCC.genomics.Analysis.HTS.DataFrame

' ============================================================================
' 第六部分（续）：MIC 最大信息系数计算
' ============================================================================
'
' 参考文献:
'   Reshef, D.N. et al. (2011). Detecting novel associations in large data sets.
'   Science, 334(6062), 1518-1524.
'
' 核心思想:
'   MIC (Maximal Information Coefficient) 是一种基于网格划分的信息论统计量，
'   用于检测两个变量之间的任意关联关系（包括线性和非线性）。
'   其核心思想是：如果两个变量之间存在某种关联，那么在散点图上总能找到一种
'   网格划分，使得归一化互信息接近 1。
'
' 算法流程:
'   1. 对所有满足 a*b <= B(n) 的网格大小 (a, b)：
'      a. 在 x 轴上划分 a 个箱，y 轴上划分 b 个箱
'      b. 找到使互信息最大的最优网格划分（等频划分 + DP优化）
'      c. 计算归一化互信息 I*(D; a,b) / log(min(a,b))
'   2. MIC = max over all (a,b) of the normalized MI
'
' 参数 B(n) = n^alpha（默认 alpha=0.6）控制搜索空间的上界
' MIC 取值范围 [0, 1]，值越大表示关联越强
' ============================================================================

''' <summary>
''' MIC (Maximal Information Coefficient) 最大信息系数计算模块
''' 可检测变量间的线性和非线性关联关系
''' </summary>
Public Module MICComputation

    ''' <summary>
    ''' MIC 算法参数配置
    ''' </summary>
    Public Class MICConfig
        ''' <summary>B(n) = n^Alpha，控制最大网格大小（默认 0.6，原论文推荐值）</summary>
        Public Alpha As Double = 0.6
        ''' <summary>最大网格箱数上限（-1 表示自动由 B(n) 决定）</summary>
        Public MaxBins As Integer = -1
        ''' <summary>是否使用动态规划搜索最优划分（True 更精确但更慢）</summary>
        Public UseOptimalPartition As Boolean = True
        ''' <summary>最优划分迭代次数（默认 3）</summary>
        Public OptimalIterations As Integer = 3
        ''' <summary>排列检验次数（用于计算 p 值，0 表示使用 Fisher z 近似）</summary>
        Public PermutationCount As Integer = 0
    End Class

    ''' <summary>
    ''' 从列联表（contingency table）计算互信息
    ''' MI = Sum_i Sum_j p_ij * log(p_ij / (p_i * p_j))
    ''' </summary>
    Private Function ComputeMI(
        counts As Double(,),
        nRows As Integer,
        nCols As Integer,
        nTotal As Double) As Double

        If nTotal <= 0 Then Return 0.0

        ' 计算行边际和列边际
        Dim rowSums As Double() = New Double(nRows - 1) {}
        Dim colSums As Double() = New Double(nCols - 1) {}
        For i As Integer = 0 To nRows - 1
            For j As Integer = 0 To nCols - 1
                rowSums(i) += counts(i, j)
                colSums(j) += counts(i, j)
            Next
        Next

        ' 计算互信息
        Dim mi As Double = 0.0
        For i As Integer = 0 To nRows - 1
            If rowSums(i) <= 0 Then Continue For
            For j As Integer = 0 To nCols - 1
                If counts(i, j) <= 0 OrElse colSums(j) <= 0 Then Continue For
                Dim p_ij As Double = counts(i, j) / nTotal
                Dim p_i As Double = rowSums(i) / nTotal
                Dim p_j As Double = colSums(j) / nTotal
                mi += p_ij * std.Log(p_ij / (p_i * p_j))
            Next
        Next

        Return mi
    End Function

    ''' <summary>
    ''' 计算边际熵 H(Y) = -Sum_y p(y) * log(p(y))
    ''' </summary>
    Private Function ComputeMarginalEntropy(counts As Double(), nTotal As Double) As Double
        Dim h As Double = 0.0
        For Each c As Double In counts
            If c > 0 Then
                Dim p As Double = c / nTotal
                h -= p * std.Log(p)
            End If
        Next
        Return h
    End Function

    ''' <summary>
    ''' 将数据分配到等频（equi-population）箱中
    ''' 每个箱包含大致相同数量的数据点
    ''' 返回每个数据点的箱编号（0-based）
    ''' </summary>
    Private Function AssignEquiPopulationBins(data As Double(), nBins As Integer) As Integer()
        Dim n As Integer = data.Length
        Dim bins As Integer() = New Integer(n - 1) {}
        Dim indices As Integer() = New Integer(n - 1) {}
        For i As Integer = 0 To n - 1
            indices(i) = i
        Next

        ' 按值排序索引
        Array.Sort(indices, Function(a, b) data(a).CompareTo(data(b)))

        ' 等频分配箱号
        For i As Integer = 0 To n - 1
            Dim binIdx As Integer = CInt(CLng(i) * nBins / n)
            If binIdx >= nBins Then binIdx = nBins - 1
            bins(indices(i)) = binIdx
        Next

        Return bins
    End Function

    ''' <summary>
    ''' 根据箱分配构建列联表
    ''' </summary>
    Private Function BuildContingencyTable(
        xBins As Integer(),
        yBins As Integer(),
        n As Integer,
        nXBins As Integer,
        nYBins As Integer) As Double(,)

        Dim counts As Double(,) = New Double(nXBins - 1, nYBins - 1) {}
        For i As Integer = 0 To n - 1
            counts(xBins(i), yBins(i)) += 1.0
        Next
        Return counts
    End Function

    ''' <summary>
    ''' 使用动态规划搜索一个轴上的最优划分
    ''' 给定另一个轴的箱分配，找到使条件熵最小的划分方案
    '''
    ''' 算法:
    '''   DP[k][i] = 前 i 个数据点分成 k 个箱的最小条件熵
    '''   转移: DP[k][i] = min over j of DP[k-1][j] + segment_entropy(j, i)
    '''
    ''' 数据必须已按待划分轴排序，otherBinAssignments 为对应的另一轴箱号
    ''' </summary>
    Private Function OptimalPartitionConditionalEntropy(
        otherBinAssignments As Integer(),
        n As Integer,
        nBins As Integer,
        nOtherBins As Integer) As Double

        If n <= nBins Then Return 0.0

        ' 预计算每个 other-axis 箱的前缀和
        Dim prefix As Integer(,) = New Integer(nOtherBins - 1, n) {}
        For y As Integer = 0 To nOtherBins - 1
            prefix(y, 0) = 0
            For i As Integer = 1 To n
                prefix(y, i) = prefix(y, i - 1) + If(otherBinAssignments(i - 1) = y, 1, 0)
            Next
        Next

        ' DP 表
        Dim dp As Double(,) = New Double(nBins, n) {}

        ' 初始化
        For k As Integer = 0 To nBins
            For i As Integer = 0 To n
                dp(k, i) = Double.MaxValue
            Next
        Next
        dp(0, 0) = 0.0

        ' DP 填表
        For k As Integer = 1 To nBins
            For i As Integer = k To n
                For j As Integer = k - 1 To i - 1
                    ' 计算从 j 到 i-1 的段的条件熵贡献
                    Dim nInSeg As Integer = i - j
                    Dim segEntropy As Double = 0.0
                    For y As Integer = 0 To nOtherBins - 1
                        Dim count As Integer = prefix(y, i) - prefix(y, j)
                        If count > 0 Then
                            Dim p As Double = count / CDbl(nInSeg)
                            segEntropy -= (count / CDbl(n)) * std.Log(p)
                        End If
                    Next

                    Dim val As Double = dp(k - 1, j) + segEntropy
                    If val < dp(k, i) Then
                        dp(k, i) = val
                    End If
                Next
            Next
        Next

        Return dp(nBins, n)
    End Function

    ''' <summary>
    ''' 计算边际熵 H 从箱分配
    ''' </summary>
    Private Function ComputeEntropyFromBins(
        binAssignments As Integer(),
        n As Integer,
        nBins As Integer) As Double

        Dim counts As Double() = New Double(nBins - 1) {}
        For Each b As Integer In binAssignments
            counts(b) += 1.0
        Next
        Return ComputeMarginalEntropy(counts, CDbl(n))
    End Function

    ''' <summary>
    ''' 使用等频划分计算归一化互信息
    ''' </summary>
    Private Function ComputeEquiPopNormalizedMI(
        x As Double(), y As Double(),
        n As Integer, a As Integer, b As Integer) As Double

        Dim xBins As Integer() = AssignEquiPopulationBins(x, a)
        Dim yBins As Integer() = AssignEquiPopulationBins(y, b)
        Dim counts As Double(,) = BuildContingencyTable(xBins, yBins, n, a, b)
        Dim mi As Double = ComputeMI(counts, a, b, CDbl(n))
        Dim normalizer As Double = std.Log(std.Min(a, b))

        If normalizer <= 0 Then Return 0.0
        Return mi / normalizer
    End Function

    ''' <summary>
    ''' 使用 DP 最优划分计算归一化互信息
    ''' 分别固定 y 轴优化 x 轴、固定 x 轴优化 y 轴，取最大 MI
    ''' </summary>
    Private Function ComputeOptimalNormalizedMI(
        x As Double(), y As Double(),
        n As Integer, a As Integer, b As Integer,
        nIterations As Integer) As Double

        Dim bestMI As Double = 0.0

        ' 初始划分：等频
        Dim xBins As Integer() = AssignEquiPopulationBins(x, a)
        Dim yBins As Integer() = AssignEquiPopulationBins(y, b)

        For iter As Integer = 0 To nIterations
            ' === 1. 固定 y 划分，优化 x 划分 ===
            ' 按 x 排序，获取排序后的 y 箱号
            Dim xSortedIdx As Integer() = New Integer(n - 1) {}
            For i As Integer = 0 To n - 1
                xSortedIdx(i) = i
            Next
            Array.Sort(xSortedIdx, Function(i1, i2) x(i1).CompareTo(x(i2)))

            Dim yBinsByX As Integer() = New Integer(n - 1) {}
            For i As Integer = 0 To n - 1
                yBinsByX(i) = yBins(xSortedIdx(i))
            Next

            ' DP 求最优 x 划分下的最小条件熵 H(Y|X)
            Dim condEntX As Double = OptimalPartitionConditionalEntropy(yBinsByX, n, a, b)
            Dim entY As Double = ComputeEntropyFromBins(yBins, n, b)
            Dim miFromX As Double = entY - condEntX
            If miFromX > bestMI Then bestMI = miFromX

            ' === 2. 固定 x 划分，优化 y 划分 ===
            Dim ySortedIdx As Integer() = New Integer(n - 1) {}
            For i As Integer = 0 To n - 1
                ySortedIdx(i) = i
            Next
            Array.Sort(ySortedIdx, Function(i1, i2) y(i1).CompareTo(y(i2)))

            Dim xBinsByY As Integer() = New Integer(n - 1) {}
            For i As Integer = 0 To n - 1
                xBinsByY(i) = xBins(ySortedIdx(i))
            Next

            ' DP 求最优 y 划分下的最小条件熵 H(X|Y)
            Dim condEntY As Double = OptimalPartitionConditionalEntropy(xBinsByY, n, b, a)
            Dim entX As Double = ComputeEntropyFromBins(xBins, n, a)
            Dim miFromY As Double = entX - condEntY
            If miFromY > bestMI Then bestMI = miFromY
        Next

        ' 也计算等频划分的 MI 作为备选
        Dim equiPopMI As Double = ComputeEquiPopNormalizedMI(x, y, n, a, b)

        ' bestMI 尚未归一化
        Dim normalizer As Double = std.Log(std.Min(a, b))
        If normalizer <= 0 Then Return equiPopMI

        Dim normalizedBestMI As Double = bestMI / normalizer
        Return std.Max(normalizedBestMI, equiPopMI)
    End Function

    ''' <summary>
    ''' 计算一对变量之间的 MIC 值
    '''
    ''' MIC = max_{a*b &lt;= B(n)} I*(D; a,b) / log(min(a,b))
    '''
    ''' 其中 I*(D; a,b) 是在所有 a*b 网格划分中能获得的最大互信息
    ''' </summary>
    ''' <param name="x">变量 x 的观测值数组</param>
    ''' <param name="y">变量 y 的观测值数组</param>
    ''' <param name="config">MIC 参数配置</param>
    ''' <returns>MIC 值，取值范围 [0, 1]</returns>
    Public Function ComputePair(
        x As Double(),
        y As Double(),
        Optional config As MICConfig = Nothing) As Double

        If config Is Nothing Then config = New MICConfig()

        If x Is Nothing OrElse y Is Nothing Then Return 0.0
        If x.Length <> y.Length OrElse x.Length < 4 Then Return 0.0

        Dim n As Integer = x.Length

        ' B(n) = n^Alpha，控制最大网格大小
        Dim B As Integer = CInt(std.Floor(std.Pow(n, config.Alpha)))
        If config.MaxBins > 0 Then
            B = std.Min(B, config.MaxBins * config.MaxBins)
        End If
        If B < 4 Then B = 4  ' 至少 2x2 网格

        Dim maxMIC As Double = 0.0

        ' 遍历所有满足 a*b <= B(n) 的网格大小
        Dim maxA As Integer = CInt(std.Floor(std.Sqrt(B)))
        For a As Integer = 2 To maxA
            Dim maxB_for_a As Integer = CInt(std.Floor(CDbl(B) / a))
            If maxB_for_a < 2 Then Continue For
            If maxB_for_a > n Then maxB_for_a = n

            For bi As Integer = 2 To maxB_for_a
                ' 计算该网格大小下的最大归一化互信息
                Dim normalizedMI As Double

                If config.UseOptimalPartition Then
                    ' 使用 DP 搜索最优划分
                    normalizedMI = ComputeOptimalNormalizedMI(
                        x, y, n, a, bi, config.OptimalIterations)
                Else
                    ' 使用等频划分
                    normalizedMI = ComputeEquiPopNormalizedMI(x, y, n, a, bi)
                End If

                If normalizedMI > maxMIC Then
                    maxMIC = normalizedMI
                End If
            Next
        Next

        Return maxMIC
    End Function

    ''' <summary>
    ''' 使用排列检验计算 MIC 的 p 值
    ''' 随机打乱 y 值并重新计算 MIC，统计超过原始 MIC 的比例
    ''' p = (count(MIC_perm >= MIC_obs) + 1) / (nPerm + 1)
    ''' </summary>
    Public Function ComputePValue(
        x As Double(),
        y As Double(),
        micObserved As Double,
        nPermutations As Integer,
        Optional config As MICConfig = Nothing) As Double

        If config Is Nothing Then config = New MICConfig()
        If nPermutations <= 0 Then nPermutations = 199

        Dim n As Integer = x.Length
        Dim rng As New Random()
        Dim countGreater As Integer = 0

        For perm As Integer = 0 To nPermutations - 1
            ' 随机打乱 y
            Dim yPerm As Double() = New Double(n - 1) {}
            Array.Copy(y, yPerm, n)

            ' Fisher-Yates 洗牌
            For i As Integer = n - 1 To 1 Step -1
                Dim j As Integer = rng.Next(i + 1)
                Dim temp As Double = yPerm(i)
                yPerm(i) = yPerm(j)
                yPerm(j) = temp
            Next

            ' 计算排列后的 MIC
            Dim micPerm As Double = ComputePair(x, yPerm, config)
            If micPerm >= micObserved Then
                countGreater += 1
            End If
        Next

        ' 加 1 避免返回 0（保守估计）
        Return CDbl(countGreater + 1) / CDbl(nPermutations + 1)
    End Function

    ''' <summary>
    ''' 使用 Fisher z 近似计算 MIC 的 p 值
    ''' 将 MIC 转换为等价的相关系数 r = sign * sqrt(MIC)
    ''' 然后使用 Fisher z 变换计算 p 值
    ''' </summary>
    Private Function ComputePValueFisherZ(
        mic As Double,
        n As Integer,
        Optional direction As Integer = 1) As Double

        If n < 4 Then Return 1.0
        If mic <= 0 Then Return 1.0

        ' 将 MIC 转换为等价的相关系数
        Dim r As Double = direction * std.Sqrt(mic)
        r = std.Max(-1.0, std.Min(1.0, r))

        ' Fisher z 变换
        Dim z As Double = 0.5 * std.Log((1 + r) / (1 - r))
        Dim se As Double = 1.0 / std.Sqrt(CDbl(n) - 3.0)
        Dim zStat As Double = std.Abs(z) / se

        ' 双侧 p 值（正态近似）
        Dim pValue As Double = 2.0 * (1.0 - MathHelpers.NormalCDF(zStat))
        Return std.Min(1.0, std.Max(0.0, pValue))
    End Function

    ''' <summary>
    ''' 计算 OTU 与代谢物表达矩阵之间的 MIC 交叉相关矩阵
    ''' 对每一对 OTU-代谢物 计算其 MIC 值
    ''' </summary>
    Public Function ComputeCrossCorrelation(
        otuMatrix As Matrix,
        metaboliteMatrix As Matrix,
        Optional config As MICConfig = Nothing) As SpearmanMICResult

        If config Is Nothing Then config = New MICConfig()

        Dim nOtu As Integer = otuMatrix.size
        Dim nMet As Integer = metaboliteMatrix.size
        Dim nSamples As Integer = otuMatrix.sample_count

        Dim result As New SpearmanMICResult()
        result.OtuIds = New String(nOtu - 1) {}
        result.MetaboliteIds = New String(nMet - 1) {}
        result.CorrelationMatrix = New Double(nOtu - 1, nMet - 1) {}
        result.PValueMatrix = New Double(nOtu - 1, nMet - 1) {}
        result.MethodName = "MIC"

        For i As Integer = 0 To nOtu - 1
            result.OtuIds(i) = otuMatrix(i).geneID
        Next
        For j As Integer = 0 To nMet - 1
            result.MetaboliteIds(j) = metaboliteMatrix(j).geneID
        Next

        ' 逐对计算 MIC
        For i As Integer = 0 To nOtu - 1
            Dim xData As Double() = otuMatrix(i)
            For j As Integer = 0 To nMet - 1
                Dim yData As Double() = metaboliteMatrix(j)

                ' 确保 x 和 y 长度一致
                Dim minLen As Integer = std.Min(xData.Length, yData.Length)
                Dim x As Double() = New Double(minLen - 1) {}
                Dim y As Double() = New Double(minLen - 1) {}
                Array.Copy(xData, x, minLen)
                Array.Copy(yData, y, minLen)

                ' 计算 MIC
                Dim mic As Double = ComputePair(x, y, config)
                result.CorrelationMatrix(i, j) = mic

                ' 计算 p 值
                If config.PermutationCount > 0 Then
                    ' 排列检验
                    result.PValueMatrix(i, j) = ComputePValue(
                        x, y, mic, config.PermutationCount, config)
                Else
                    ' Fisher z 近似
                    result.PValueMatrix(i, j) = ComputePValueFisherZ(mic, minLen)
                End If
            Next
        Next

        Return result
    End Function

    ''' <summary>
    ''' 对单个矩阵计算 MIC 内部相关矩阵（用于同类型 feature 间关联分析）
    ''' </summary>
    Public Function ComputeInternalMatrix(
        matrix As Matrix,
        Optional config As MICConfig = Nothing) As SpearmanMICResult

        If config Is Nothing Then config = New MICConfig()

        Dim nF As Integer = matrix.size
        Dim result As New SpearmanMICResult
        result.OtuIds = New String(nF - 1) {}
        result.MetaboliteIds = New String(nF - 1) {}
        result.CorrelationMatrix = New Double(nF - 1, nF - 1) {}
        result.PValueMatrix = New Double(nF - 1, nF - 1) {}
        result.MethodName = "MIC"

        For i As Integer = 0 To nF - 1
            result.OtuIds(i) = matrix(i).geneID
            result.MetaboliteIds(i) = matrix(i).geneID
            result.CorrelationMatrix(i, i) = 1.0
            result.PValueMatrix(i, i) = 0.0
        Next

        For i As Integer = 0 To nF - 2
            For j As Integer = i + 1 To nF - 1
                Dim x As Double() = matrix(i)
                Dim y As Double() = matrix(j)
                Dim mic As Double = ComputePair(x, y, config)
                result.CorrelationMatrix(i, j) = mic
                result.CorrelationMatrix(j, i) = mic

                Dim pVal As Double = ComputePValueFisherZ(mic, x.Length)
                result.PValueMatrix(i, j) = pVal
                result.PValueMatrix(j, i) = pVal
            Next
        Next

        Return result
    End Function

End Module