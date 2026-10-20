Imports System.Threading.Tasks
Imports Microsoft.VisualBasic.Math.SIMD
Imports std = System.Math

Namespace LinearRegression

    ''' <summary>
    ''' 基于分块矩阵乘法的相关性筛选后端（纯 CPU）。
    ''' 
    ''' 逐基因扫描的做法是"目标基因 x 全部基因"各算一次点积，内存流量为
    ''' ``O(基因数^2 * 样本数)`` 且几乎没有复用；这里改成按块计算：
    ''' 一次取 <see cref="BlockSize"/> 个目标基因（它们的向量合计约 1 MB，
    ''' 可以留在 L2 缓存里），然后遍历全部候选基因，每读入一列就与整块目标做点积。
    ''' 
    ''' 点数（浮点乘加）不变，但内存流量从"每个目标扫一遍全表"降到
    ''' "每一块扫一遍全表"，缓存命中率大幅提升；块与块之间再并行。
    ''' </summary>
    ''' <remarks>
    ''' 相关性使用总体标准差（除以 n）定义：
    ''' 
    ''' ```
    ''' r = (Σ(x_i * x_j) / n - mean_i * mean_j) / (sd_i * sd_j)
    ''' ```
    ''' 
    ''' 因此不需要额外复制一份标准化之后的矩阵，直接使用原始向量即可。
    ''' 与 <see cref="GeneRegulatoryNetwork"/> 内置的逐基因扫描相比，
    ''' 浮点归约顺序不同会带来末位差异，候选集合可能有极少数边界抖动。
    ''' </remarks>
    Public Class BlockCorrelationScreener : Implements ICorrelationScreener

        ''' <summary>
        ''' 一次处理的目标基因数量：64 x 1888 x 8B 约 1 MB，正好落在 L2 缓存内。
        ''' </summary>
        Public Const BlockSize As Integer = 64

        Private pool As Integer()
        Private means As Double()
        Private sds As Double()
        Private sampleSize As Integer
        Private topK As Integer
        Private minAbsR As Double
        Private results As Integer()()

        ''' <summary>
        ''' 计算每个目标基因的候选调控因子。
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
            Me.sampleSize = If(vectors(0) Is Nothing, 0, vectors(0).Length)
            Me.results = New Integer(geneCount - 1)() {}

            For i As Integer = 0 To geneCount - 1
                Me.results(i) = New Integer() {}
            Next

            If Me.topK = 0 OrElse Me.sampleSize = 0 OrElse Me.pool.Length = 0 Then
                Return
            End If

            Me.means = New Double(Me.pool.Length - 1) {}
            Me.sds = New Double(Me.pool.Length - 1) {}

            ' 预先算好每一列的均值与总体标准差，避免在块循环里重复统计
            For k As Integer = 0 To Me.pool.Length - 1
                Dim col As Double() = vectors(Me.pool(k))
                Dim sum As Double = 0.0

                If col Is Nothing Then
                    Continue For
                End If

                For i As Integer = 0 To Me.sampleSize - 1
                    sum += col(i)
                Next

                Dim mean As Double = sum / Me.sampleSize
                Dim variance As Double = 0.0

                For i As Integer = 0 To Me.sampleSize - 1
                    Dim d As Double = col(i) - mean

                    variance += d * d
                Next

                Me.means(k) = mean
                Me.sds(k) = std.Sqrt(variance / Me.sampleSize)
            Next

            Dim blocks As Integer = (Me.pool.Length + BlockSize - 1) \ BlockSize

            ' 块之间完全独立，每一块只写自己的目标基因对应的结果
            Call Parallel.For(0, blocks, Sub(block As Integer)
                                                Call ScreenBlock(vectors, block)
                                            End Sub)
        End Sub

        ''' <summary>
        ''' 处理一个目标基因块：与全部候选基因做点积，再逐行取 top-k。
        ''' </summary>
        ''' <param name="vectors">全部基因的表达向量</param>
        ''' <param name="block">块下标</param>
        Private Sub ScreenBlock(vectors As Double()(), block As Integer)
            Dim start As Integer = block * BlockSize
            Dim last As Integer = std.Min(start + BlockSize, pool.Length) - 1
            Dim width As Integer = last - start + 1
            Dim rows As Double()() = New Double(width - 1)() {}
            Dim b As Integer

            For b = 0 To width - 1
                rows(b) = New Double(pool.Length - 1) {}
            Next

            ' 外层遍历候选基因：每一列只从内存读一次，块内所有目标共享
            For k As Integer = 0 To pool.Length - 1
                Dim geneK As Integer = pool(k)
                Dim sk As Double = sds(k)
                Dim colK As Double() = vectors(geneK)

                If sk <= 0.0R OrElse colK Is Nothing Then
                    Continue For
                End If

                Dim meanK As Double = means(k)

                For b = 0 To width - 1
                    Dim pos As Integer = start + b
                    Dim sb As Double = sds(pos)

                    If sb <= 0.0R Then
                        Continue For
                    End If

                    Dim gene As Integer = pool(pos)

                    If gene = geneK Then
                        Continue For
                    End If

                    Dim colB As Double() = vectors(gene)

                    If colB Is Nothing Then
                        Continue For
                    End If

                    ' r = (x_b'x_k / n - mean_b * mean_k) / (sd_b * sd_k)
                    Dim dot As Double = SIMDIntrinsics.DotFma(colB, colK) / sampleSize

                    rows(b)(k) = (dot - means(pos) * meanK) / (sb * sk)
                Next
            Next

            For b = 0 To width - 1
                Dim pos As Integer = start + b
                Dim gene As Integer = pool(pos)

                results(gene) = CorrelationTopK.SelectTopK(rows(b), pool, topK, minAbsR, pos)
            Next
        End Sub

        ''' <summary>
        ''' 取出某个目标基因的候选调控因子。
        ''' </summary>
        ''' <param name="target">目标基因的行号</param>
        ''' <returns>候选调控因子的行号；没有结果的时候返回空数组</returns>
        Public Function Query(target As Integer) As Integer() Implements ICorrelationScreener.Query
            If results Is Nothing OrElse target < 0 OrElse target >= results.Length Then
                Return New Integer() {}
            End If

            Return results(target)
        End Function

        ''' <summary>
        ''' 释放初始化阶段缓存的候选集。
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            results = Nothing
            means = Nothing
            sds = Nothing
            pool = Nothing
        End Sub
    End Class

End Namespace
