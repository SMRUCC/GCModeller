Imports Microsoft.VisualBasic.Math.Numerics.RootFinding
Imports sysmath = System.Math

Namespace Copula

    ''' <summary>
    ''' Gaussian copula 的秩相关反演：把"目标秩得分"与噪声按指定相关系数混合，
    ''' 再转成分位锚点，从而在<b>不改变每个边缘分布</b>的前提下注入指定的秩相关。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这是 Iman–Conover 类方法的连续版本：给定目标变量的秩得分 <c>z</c>（通常是
    ''' 库大小或某个参考变量的正态分数）和独立噪声 <c>e0</c>，
    ''' </para>
    ''' <code>
    ''' e = r · z + √(1 - r²) · e0
    ''' </code>
    ''' <para>
    ''' <c>e</c> 与 <c>z</c> 的相关系数恰好是 <c>r</c>，而 <c>e</c> 的名次只用于选分位锚点，
    ''' 因此每个变量的边缘分布保持不变。这正是"让基因表达的分位与测序深度同向变化"的数学依据。
    ''' </para>
    ''' <para>
    ''' 相关系数本身通常没有解析解（它要通过"合成后的统计量是否落在容差内"来判定），
    ''' 因此提供 <see cref="FindCorrelation"/>：把任意目标函数交给一维求根器，
    ''' 不收敛时返回 <c>Converged = False</c> 与最佳点，而不是抛异常。
    ''' </para>
    ''' </remarks>
    Public Module GaussianCopula

        ''' <summary>
        ''' 按相关系数混合目标秩得分与噪声
        ''' </summary>
        ''' <param name="targetRanks">目标秩得分（例如库大小的正态分数）</param>
        ''' <param name="noise">独立标准正态噪声（长度必须一致）</param>
        ''' <param name="correlation">目标秩相关系数，落在 <c>[-1, 1]</c>（超出会被截断）</param>
        ''' <returns><c>e = r·z + √(1-r²)·e0</c></returns>
        Public Function Mix(targetRanks As Double(), noise As Double(), correlation As Double) As Double()
            If targetRanks Is Nothing Then Throw New ArgumentNullException(NameOf(targetRanks))
            If noise Is Nothing Then Throw New ArgumentNullException(NameOf(noise))
            If targetRanks.Length <> noise.Length Then
                Throw New ArgumentException(
                    $"noise length {noise.Length} does not match the rank length {targetRanks.Length}", NameOf(noise))
            End If

            Dim r As Double = correlation

            If r > 1.0 Then r = 1.0
            If r < -1.0 Then r = -1.0

            Dim complement As Double = sysmath.Sqrt(sysmath.Max(1.0 - r * r, 0.0))
            Dim out As Double() = New Double(targetRanks.Length - 1) {}

            For i As Integer = 0 To targetRanks.Length - 1
                out(i) = r * targetRanks(i) + complement * noise(i)
            Next

            Return out
        End Function

        ''' <summary>
        ''' 把隐变量的名次转成分位锚点 <c>(i + 0.5)/n</c>，并给出降序名次
        ''' </summary>
        ''' <param name="latent">隐变量</param>
        ''' <returns></returns>
        Public Function RanksToQuantiles(latent As Double()) As QuantileRanks
            If latent Is Nothing Then Throw New ArgumentNullException(NameOf(latent))

            Dim n As Integer = latent.Length
            Dim quantiles As Double() = New Double(n - 1) {}
            Dim order As Integer() = Apportionment.Apportion.Identity(n)

            If n = 0 Then Return New QuantileRanks With {.Quantiles = quantiles, .Order = order}

            Dim comparer As New Comparison(Of Integer)(
                Function(a As Integer, b As Integer)
                    Dim c As Integer = latent(a).CompareTo(latent(b))

                    If c <> 0 Then Return c

                    ' 并列时按下标稳定排序，保证结果可复现
                    Return a.CompareTo(b)
                End Function)

            Call Array.Sort(order, comparer)

            Dim descending As Integer() = New Integer(n - 1) {}

            For i As Integer = 0 To n - 1
                quantiles(order(i)) = (i + 0.5) / n
                descending(i) = order(n - 1 - i)
            Next

            Return New QuantileRanks With {.Quantiles = quantiles, .Order = descending}
        End Function

        ''' <summary>
        ''' 一次完整的采样：混合 + 名次转分位
        ''' </summary>
        ''' <param name="targetRanks"></param>
        ''' <param name="noise"></param>
        ''' <param name="correlation"></param>
        ''' <returns></returns>
        Public Function Sample(targetRanks As Double(), noise As Double(), correlation As Double) As CopulaSample
            Dim latent As Double() = Mix(targetRanks, noise, correlation)
            Dim ranks As QuantileRanks = RanksToQuantiles(latent)

            Return New CopulaSample With {
                .Latent = latent,
                .Quantiles = ranks.Quantiles,
                .Order = ranks.[Order],
                .Correlation = correlation,
                .Converged = True
            }
        End Function

        ''' <summary>
        ''' 以目标函数为输入搜索相关系数（有界二分）
        ''' </summary>
        ''' <param name="objective">给定 <c>r</c> 返回待匹配的统计量（例如每细胞平均 CPM）</param>
        ''' <param name="target">目标值</param>
        ''' <param name="lower">搜索下界</param>
        ''' <param name="upper">搜索上界</param>
        ''' <param name="options">求根参数；缺省 40 次迭代、1e-3 绝对容差</param>
        ''' <returns>
        ''' 收敛时 <see cref="RootResult.Converged"/> 为 <c>True</c>；目标不可达时
        ''' <see cref="RootResult.Capped"/> 为 <c>True</c> 且 <see cref="RootResult.Value"/> 取端点
        ''' </returns>
        Public Function FindCorrelation(objective As Func(Of Double, Double),
                                        target As Double,
                                        Optional lower As Double = -0.99,
                                        Optional upper As Double = 0.99,
                                        Optional options As RootOptions = Nothing) As RootResult

            If objective Is Nothing Then Throw New ArgumentNullException(NameOf(objective))

            Dim opt As RootOptions = options

            If opt Is Nothing Then
                opt = New RootOptions With {
                    .MaxIterations = 40,
                    .AbsoluteTolerance = 0.001,
                    .RelativeTolerance = 0.001
                }
            End If

            Return RootSolver.Bisect(objective, lower, upper, target, opt)
        End Function

        ''' <summary>
        ''' Spearman 秩相关（并列取平均秩），用于诊断"注入的秩相关是否符合预期"
        ''' </summary>
        ''' <param name="x"></param>
        ''' <param name="y"></param>
        ''' <returns>无相关或输入退化时返回 0</returns>
        Public Function Spearman(x As Double(), y As Double()) As Double
            If x Is Nothing Then Throw New ArgumentNullException(NameOf(x))
            If y Is Nothing Then Throw New ArgumentNullException(NameOf(y))
            If x.Length <> y.Length Then
                Throw New ArgumentException($"length mismatch: {x.Length} vs {y.Length}", NameOf(y))
            End If
            If x.Length < 2 Then Return 0.0

            Dim rankX As Double() = AverageRanks(x)
            Dim rankY As Double() = AverageRanks(y)
            Dim meanX As Double = rankX.Average
            Dim meanY As Double = rankY.Average
            Dim covariance As Double = 0.0
            Dim varianceX As Double = 0.0
            Dim varianceY As Double = 0.0

            For i As Integer = 0 To x.Length - 1
                Dim dx As Double = rankX(i) - meanX
                Dim dy As Double = rankY(i) - meanY

                covariance += dx * dy
                varianceX += dx * dx
                varianceY += dy * dy
            Next

            If varianceX <= 0.0 OrElse varianceY <= 0.0 Then Return 0.0

            Return covariance / sysmath.Sqrt(varianceX * varianceY)
        End Function

        ''' <summary>
        ''' 平均秩（并列取平均）
        ''' </summary>
        ''' <param name="values"></param>
        ''' <returns></returns>
        Public Function AverageRanks(values As Double()) As Double()
            If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))

            Dim n As Integer = values.Length
            Dim ranks As Double() = New Double(n - 1) {}
            Dim index As Integer() = Apportionment.Apportion.Identity(n)

            Call Array.Sort(index, New Comparison(Of Integer)(Function(a As Integer, b As Integer) values(a).CompareTo(values(b))))

            Dim i As Integer = 0

            While i < n
                Dim j As Integer = i

                While j + 1 < n AndAlso values(index(j + 1)) = values(index(i))
                    j += 1
                End While

                Dim average As Double = (i + j) / 2.0 + 1.0

                For k As Integer = i To j
                    ranks(index(k)) = average
                Next

                i = j + 1
            End While

            Return ranks
        End Function
    End Module
End Namespace
