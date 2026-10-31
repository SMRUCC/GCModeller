Imports std = System.Math

Namespace ARACNE

    ''' <summary>
    ''' 互信息显著性阈值 ``tau`` 的估计结果
    ''' </summary>
    Public Class MiThreshold

        ''' <summary>
        ''' 阈值的确定方式
        ''' </summary>
        ''' <returns></returns>
        Public Property mode As MiThresholdMode

        ''' <summary>
        ''' 最终使用的互信息阈值：互信息低于这个数值的边会被过滤掉
        ''' </summary>
        ''' <returns></returns>
        Public Property value As Double

        ''' <summary>
        ''' 零分布所取的分位数(<see cref="MiThresholdMode.Fixed"/> 模式之下没有意义)
        ''' </summary>
        ''' <returns></returns>
        Public Property quantile As Double

        ''' <summary>
        ''' 置换零分布的平均互信息(也就是“没有任何调控关系”时的估计偏置)
        ''' </summary>
        ''' <returns></returns>
        Public Property nullMean As Double

        ''' <summary>
        ''' 置换零分布的标准差
        ''' </summary>
        ''' <returns></returns>
        Public Property nullSD As Double

        ''' <summary>
        ''' 置换零分布的最大互信息
        ''' </summary>
        ''' <returns></returns>
        Public Property nullMax As Double

        ''' <summary>
        ''' 参与零分布估计的基因对数量
        ''' </summary>
        ''' <returns></returns>
        Public Property samples As Integer

        ''' <summary>
        ''' 每一个基因对被洗牌的次数
        ''' </summary>
        ''' <returns></returns>
        Public Property replicates As Integer

        ''' <summary>
        ''' 零分布所使用的互信息估计器
        ''' </summary>
        ''' <returns></returns>
        Public Property estimator As MiEstimator

        ''' <summary>
        ''' 显著性倍率：真实边所要求的互信息至少是零分布平均值的多少倍。
        ''' 
        ''' 这个数值用于判断阈值是否“过于宽松”(接近零分布的噪声水平)，
        ''' 小于 1 说明当前的阈值完全落在噪声区间之内。
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SignalRatio As Double
            Get
                If nullMean <= 0.0R Then
                    Return Double.PositiveInfinity
                Else
                    Return value / nullMean
                End If
            End Get
        End Property

        Public Overrides Function ToString() As String
            If mode = MiThresholdMode.Fixed Then
                Return $"tau = {value.ToString("F4")} (fixed)"
            Else
                Return $"tau = {value.ToString("F4")} ({quantile.ToString("P0")} quantile of {samples} x {replicates} permutation, " &
                    $"null mean = {nullMean.ToString("F4")}, sd = {nullSD.ToString("F4")})"
            End If
        End Function
    End Class

    ''' <summary>
    ''' 基于置换零分布的互信息显著性阈值估计(readme 第四章第 2 节)。
    ''' 
    ''' 直接把互信息的阈值硬编码成 ``0.05`` 之类的常数是不可靠的：互信息的估计值
    ''' 依赖于样本数量、离散化粒度以及估计器自身的偏置，不同的数据集之间完全不可比。
    ''' 
    ''' 正确的做法是构造“没有任何调控关系”的零分布：
    ''' 
    ''' 1. 对基因的表达向量沿着<b>样本维度</b>独立地随机打乱(保留各自的边缘分布，
    '''    破坏基因之间的依赖结构)；
    ''' 2. 用<b>同一个</b>估计器重新计算互信息；
    ''' 3. 取零分布的 95%(或者 99%)分位数作为阈值 ``tau``。
    ''' 
    ''' 这样即使估计器本身存在向上的偏置，偏置也会同样出现在零分布之中，
    ''' 从而被阈值抵消掉大部分。
    ''' </summary>
    <HideModuleName>
    Public Module PermutationThreshold

        ''' <summary>
        ''' 估计互信息的显著性阈值 ``tau``
        ''' </summary>
        ''' <param name="vectors">参与计算的基因表达向量(下标与基因下标一致)</param>
        ''' <param name="pairs">候选基因对，零分布的样本会从这里随机抽取</param>
        ''' <param name="estimator">互信息估计器(必须与真实边所使用的估计器一致)</param>
        ''' <param name="options">参数配置</param>
        ''' <returns></returns>
        Public Function Estimate(vectors As Double()(),
                                 pairs As IEnumerable(Of (u As Integer, v As Integer)),
                                 estimator As MiEstimator,
                                 Optional options As AracneOptions = Nothing) As MiThreshold

            Dim opt As AracneOptions = If(options, New AracneOptions())
            Dim result As New MiThreshold With {
                .mode = opt.thresholdMode,
                .estimator = estimator,
                .quantile = If(opt.thresholdMode = MiThresholdMode.Fixed, 0.0R, opt.permutationQuantile)
            }

            If opt.thresholdMode = MiThresholdMode.Fixed Then
                result.value = opt.miThreshold

                Return result
            End If

            Dim all As (u As Integer, v As Integer)() = pairs.ToArray

            If all.Length = 0 Then
                result.value = opt.miThreshold

                Return result
            End If

            Dim wanted As Integer = opt.permutationPairs

            If wanted <= 0 Then
                ' 自动推算：最多 1000 个基因对，同时不超过候选对的 10%
                wanted = std.Max(100, std.Min(1000, all.Length \ 10))
            End If

            wanted = std.Min(wanted, all.Length)

            Dim replicates As Integer = std.Max(1, opt.permutationReplicates)
            Dim rng As New Random(opt.randomSeed)
            Dim picks As Integer() = MutualInformation.SampleIndices(all.Length, wanted, rng)
            Dim fn As MiEstimatorFn = MutualInformation.GetEstimator(estimator, opt)
            Dim null As New List(Of Double)(picks.Length * replicates)

            For Each idx As Integer In picks
                Dim pair As (u As Integer, v As Integer) = all(idx)
                Dim x As Double() = vectors(pair.u)
                Dim y As Double() = vectors(pair.v)

                For r As Integer = 1 To replicates
                    ' 两个基因各自独立洗牌：样本之间的对应关系被完全破坏，
                    ' 但是每一个基因的边缘分布保持不变
                    Dim sx As Double() = MutualInformation.Shuffle(x, rng)
                    Dim sy As Double() = MutualInformation.Shuffle(y, rng)
                    Dim value As Double = fn(sx, sy)

                    If Not Double.IsNaN(value) Then
                        Call null.Add(value)
                    End If
                Next
            Next

            result.samples = picks.Length
            result.replicates = replicates

            If null.Count = 0 Then
                result.value = opt.miThreshold

                Return result
            End If

            Dim sorted As Double() = null.OrderBy(Function(v) v).ToArray
            Dim mean As Double = sorted.Average
            Dim sd As Double = If(sorted.Length > 1,
                std.Sqrt(sorted.Sum(Function(v) (v - mean) ^ 2) / (sorted.Length - 1)),
                0.0R)

            result.nullMean = mean
            result.nullSD = sd
            result.nullMax = sorted(sorted.Length - 1)
            result.value = std.Max(0.0R, MutualInformation.Quantile(sorted, opt.permutationQuantile))

            Return result
        End Function
    End Module
End Namespace
