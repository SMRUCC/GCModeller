Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Solvers
Imports Microsoft.VisualBasic.Math.SIMD
Imports std = System.Math

Namespace LinearRegression

    ''' <summary>
    ''' 使用<b>偏相关</b>给候选调控因子排序的筛选后端。
    ''' 
    ''' 零阶 Pearson 在横截面表达数据上会被"全局程序"污染：几乎所有基因都随
    ''' 细胞状态/线粒体含量/测序质量一起波动，于是任意两个基因都呈现正相关，
    ''' 建出来的网络天然只有"同向"的边 —— 敲降一个基因只会把整个模块一起拖下去。
    ''' 
    ''' 这里按 <c>WGCNA/PartialCorrelation.vb::FromData</c> 相同的 Frisch–Waugh–Lovell
    ''' 做法计算偏相关：先找出与随机抽样基因平均 |r| 最大的若干个 hub 基因
    ''' (实际数据里通常是 MT-*/RPL*/RPS* 这类全局程序基因)，把每个基因对
    ''' ``[1 | hub]`` 做 OLS 回归取残差，再在残差之间计算 Pearson。
    ''' 残差之间的相关就是以 hub 集合为条件集的偏相关。
    ''' </summary>
    ''' <remarks>
    ''' 之所以不直接引用 <c>WGCNA.vbproj</c>：该工程还拖入了 TensorFlow / ILCudaTensor /
    ''' 图像与可视化等重量级依赖，把它们带进 R# 插件是不可接受的。这里使用的是
    ''' 同一套算法(Core 的 OLS 最小二乘 + Pearson)，只是做了批量化的残差提取。
    ''' </remarks>
    Public Class PartialCorrelationScreener : Implements ICorrelationScreener

        ''' <summary>一次处理的目标基因数量</summary>
        Public Const BlockSize As Integer = 64

        ''' <summary>用作条件集的 hub 基因数量，默认 5；0 表示退化为普通 Pearson</summary>
        Public Property ConditionSize As Integer = 5

        ''' <summary>估计 hub 打分时使用的抽样基因数量，默认 200</summary>
        Public Property HubSampleSize As Integer = 200

        ''' <summary>
        ''' 为负相关候选保留的名额，小于等于 0 表示不启用符号配额。
        ''' </summary>
        ''' <returns></returns>
        Public Property NegativeQuota As Integer = -1 Implements ICorrelationScreener.NegativeQuota

        Private pool As Integer()
        Private sampleSize As Integer
        Private topK As Integer
        Private minAbsR As Double
        Private results As Integer()()

        ''' <summary>每个候选基因对 hub 集合回归之后的残差(已经归一化为单位长度)</summary>
        Private residual As Double()()

        ''' <summary>
        ''' 计算 hub 集合、提取残差并一次性算出每个目标基因的候选集。
        ''' </summary>
        ''' <param name="pool">允许作为候选调控因子的基因行号</param>
        ''' <param name="vectors">全部基因的表达向量，下标与基因行号一致</param>
        ''' <param name="topK">每个目标基因需要保留的候选数量</param>
        ''' <param name="minAbsR">相关系数绝对值下限，小于等于零表示不限制</param>
        Public Sub Initialize(pool As Integer(), vectors As Double()(), topK As Integer, minAbsR As Double) Implements ICorrelationScreener.Initialize
            If vectors Is Nothing OrElse vectors.Length = 0 Then
                Throw New ArgumentNullException(NameOf(vectors))
            End If

            Dim geneCount As Integer = vectors.Length

            Me.pool = If(pool, Enumerable.Range(0, geneCount).ToArray)
            Me.topK = std.Max(0, topK)
            Me.minAbsR = std.Max(0.0R, minAbsR)
            Me.sampleSize = If(vectors(Me.pool(0)) Is Nothing, 0, vectors(Me.pool(0)).Length)
            Me.results = New Integer(geneCount - 1)() {}

            For i As Integer = 0 To geneCount - 1
                Me.results(i) = New Integer() {}
            Next

            Dim p As Integer = Me.pool.Length

            If Me.topK = 0 OrElse Me.sampleSize = 0 OrElse p = 0 Then
                Return
            End If

            Dim n As Integer = Me.sampleSize
            Dim means As Double() = New Double(p - 1) {}
            Dim sds As Double() = New Double(p - 1) {}

            For k As Integer = 0 To p - 1
                Dim col As Double() = vectors(Me.pool(k))

                If col Is Nothing Then
                    Continue For
                End If

                Dim sum As Double = 0.0

                For s As Integer = 0 To n - 1
                    sum += col(s)
                Next

                Dim mean As Double = sum / n
                Dim variance As Double = 0.0

                For s As Integer = 0 To n - 1
                    Dim d As Double = col(s) - mean

                    variance += d * d
                Next

                means(k) = mean
                sds(k) = std.Sqrt(variance / n)
            Next

            Dim hubCount As Integer = std.Min(std.Max(0, ConditionSize), std.Max(0, p - 2))
            Dim hubs As Integer() = If(hubCount > 0, FindHubs(vectors, means, sds, hubCount), New Integer() {})

            ' 残差化：每个基因对 [1 | hub] 做 OLS 取残差，并归一化为单位长度
            Me.residual = New Double(p - 1)() {}
            Dim design As Double(,) = BuildDesign(vectors, hubs, -1)

            For k As Integer = 0 To p - 1
                Dim col As Double() = vectors(Me.pool(k))

                If col Is Nothing Then
                    Continue For
                End If

                Dim selfId As Integer = Array.IndexOf(hubs, k)
                Dim dg As Double(,) = If(selfId >= 0, BuildDesign(vectors, hubs, selfId), design)
                Dim r As Double() = Residualize(dg, col)
                Dim norm As Double = 0.0

                For s As Integer = 0 To r.Length - 1
                    norm += r(s) * r(s)
                Next

                If norm > 1.0E-12 Then
                    Dim inv As Double = 1.0 / std.Sqrt(norm)

                    For s As Integer = 0 To r.Length - 1
                        r(s) *= inv
                    Next

                    Me.residual(k) = r
                End If
            Next

            Dim blocks As Integer = (p + BlockSize - 1) \ BlockSize

            Call Parallel.For(0, blocks, Sub(block As Integer)
                                                Call ScreenBlock(block)
                                            End Sub)
        End Sub

        ''' <summary>
        ''' 在与随机抽样基因的平均 |r| 最大的基因里挑出若干个 hub 作为条件集。
        ''' </summary>
        Private Function FindHubs(vectors As Double()(), means As Double(), sds As Double(), hubCount As Integer) As Integer()
            Dim p As Integer = pool.Length
            Dim n As Integer = sampleSize
            Dim probes As Integer = std.Min(std.Max(1, HubSampleSize), p)
            Dim stepSize As Double = p / CDbl(probes)
            Dim score As Double() = New Double(p - 1) {}

            For b As Integer = 0 To probes - 1
                Dim probe As Integer = std.Min(p - 1, CInt(std.Floor(b * stepSize)))
                Dim ik As Integer = pool(probe)

                If sds(probe) <= 0.0R OrElse vectors(ik) Is Nothing Then
                    Continue For
                End If

                For k As Integer = 0 To p - 1
                    If k = probe Then
                        Continue For
                    End If
                    If sds(k) <= 0.0R OrElse vectors(pool(k)) Is Nothing Then
                        Continue For
                    End If

                    Dim r As Double = SIMDIntrinsics.DotFma(vectors(pool(k)), vectors(ik)) / n
                    r = (r - means(k) * means(probe)) / (sds(k) * sds(probe))

                    score(k) += std.Abs(r)
                Next
            Next

            Dim order As Integer() = Enumerable _
                .Range(0, p) _
                .OrderByDescending(Function(k) score(k)) _
                .Take(hubCount) _
                .ToArray

            Return order
        End Function

        ''' <summary>
        ''' 构造 <c>[1 | hub]</c> 的设计矩阵；<paramref name="excludeHub"/> 指向的 hub
        ''' 会被剔除(用于避免"基因以自身为条件"的退化情形)。
        ''' </summary>
        Private Function BuildDesign(vectors As Double()(), hubs As Integer(), excludeHub As Integer) As Double(,)
            Dim n As Integer = sampleSize
            Dim width As Integer = 1

            For c As Integer = 0 To hubs.Length - 1
                If c <> excludeHub Then
                    width += 1
                End If
            Next

            Dim design As Double(,) = New Double(n - 1, width - 1) {}

            For s As Integer = 0 To n - 1
                design(s, 0) = 1.0
            Next

            Dim offset As Integer = 0

            For c As Integer = 0 To hubs.Length - 1
                If c = excludeHub Then
                    Continue For
                End If

                offset += 1

                Dim col As Double() = vectors(pool(hubs(c)))

                If col Is Nothing Then
                    Continue For
                End If

                For s As Integer = 0 To n - 1
                    design(s, offset) = col(s)
                Next
            Next

            Return design
        End Function

        ''' <summary>用最小二乘拟合 <paramref name="design"/> 并返回残差。</summary>
        Private Shared Function Residualize(design As Double(,), y As Double()) As Double()
            Dim beta As Double() = OLS.LeastSquares(design, y)
            Dim r As Double() = New Double(y.Length - 1) {}
            Dim width As Integer = design.GetLength(1)

            For s As Integer = 0 To y.Length - 1
                Dim fit As Double = 0.0

                For c As Integer = 0 To width - 1
                    fit += design(s, c) * beta(c)
                Next

                r(s) = y(s) - fit
            Next

            Return r
        End Function

        ''' <summary>在残差矩阵上计算相关并取出每个目标基因的 top-k。</summary>
        Private Sub ScreenBlock(block As Integer)
            Dim start As Integer = block * BlockSize
            Dim last As Integer = std.Min(start + BlockSize, pool.Length) - 1
            Dim width As Integer = last - start + 1
            Dim rows As Double()() = New Double(width - 1)() {}
            Dim b As Integer

            For b = 0 To width - 1
                rows(b) = New Double(pool.Length - 1) {}
            Next

            For k As Integer = 0 To pool.Length - 1
                Dim colK As Double() = residual(k)

                If colK Is Nothing Then
                    Continue For
                End If

                For b = 0 To width - 1
                    Dim colB As Double() = residual(start + b)

                    If colB Is Nothing OrElse start + b = k Then
                        Continue For
                    End If

                    ' 残差向量已经归一化，点积即 Pearson 相关
                    rows(b)(k) = SIMDIntrinsics.DotFma(colB, colK)
                Next
            Next

            For b = 0 To width - 1
                Dim pos As Integer = start + b

                results(pool(pos)) = CorrelationTopK.SelectTopKBalanced(rows(b), pool, topK, minAbsR, pos, NegativeQuota)
            Next
        End Sub

        ''' <summary>取出某个目标基因的候选调控因子。</summary>
        ''' <param name="target">目标基因的行号</param>
        ''' <returns>候选调控因子的行号；没有结果的时候返回空数组</returns>
        Public Function Query(target As Integer) As Integer() Implements ICorrelationScreener.Query
            If results Is Nothing OrElse target < 0 OrElse target >= results.Length Then
                Return New Integer() {}
            End If

            Return results(target)
        End Function

        ''' <summary>释放初始化阶段缓存的残差矩阵。</summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            residual = Nothing
            results = Nothing
            pool = Nothing
        End Sub
    End Class

End Namespace
