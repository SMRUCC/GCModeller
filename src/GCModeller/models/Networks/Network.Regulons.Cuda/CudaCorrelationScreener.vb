Imports Microsoft.VisualBasic.Computing.ILCuda.Math
Imports Microsoft.VisualBasic.Computing.ILCuda.Runtime
Imports SMRUCC.genomics.Model.Network.Regulons.LinearRegression
Imports std = System.Math

Namespace Cuda

    ''' <summary>
    ''' 基于 CUDA 的相关性筛选后端：把"目标基因块 x 全部候选基因"的相关系数
    ''' 交给 <see cref="GpuBlas.Gemm"/> 在 float32 下做分块矩阵乘法。
    ''' 
    ''' 数学上与 <see cref="BlockCorrelationScreener"/> 完全一致：
    ''' 
    ''' ```
    ''' R(b, k) = (X[b] . X[k] / n - mean_b * mean_k) / (sd_b * sd_k)
    ''' ```
    ''' 
    ''' 其中 ``X`` 是 n x p 的转置表达矩阵（行 = 样本、列 = 候选基因），
    ''' 它在 <see cref="Initialize"/> 时一次性上传并常驻显存；每一块只传输
    ''' ``BlockSize x n`` 的目标块与 ``BlockSize x p`` 的结果块。
    ''' 
    ''' 只在 float32 上计算：float 的 7 位有效数字足以稳定地区分 top-k 候选，
    ''' 而且在消费级显卡上 float64 的吞吐只有 float32 的 1/64。
    ''' </summary>
    Public Class CudaCorrelationScreener : Implements ICorrelationScreener

        ''' <summary>
        ''' 一次处理的块大小（目标基因数）。
        ''' </summary>
        Public Const BlockSize As Integer = 64

        Private engine As CudaEngine
        Private pool As Integer()
        Private means As Double()
        Private sds As Double()
        Private sampleSize As Integer
        Private topK As Integer
        Private minAbsR As Double
        Private results As Integer()()

        ''' <summary>转置之后的表达矩阵，n x p 的行主序 float32，常驻显存</summary>
        Private deviceMatrix As DeviceBuffer(Of Single)

        ''' <summary>
        ''' 尝试创建 GPU 后端；没有可用的 CUDA 设备、驱动不可用或者显存不足的时候
        ''' 返回 <c>Nothing</c>，调用方应当回退到 CPU 实现。
        ''' </summary>
        ''' <param name="deviceOrdinal">使用第几块 GPU，默认 0</param>
        ''' <returns>可用的后端；创建失败时返回 <c>Nothing</c></returns>
        Public Shared Function TryCreate(Optional deviceOrdinal As Integer = 0) As ICorrelationScreener
            Try
                Dim options As New EngineOptions With {
                    .DeviceOrdinal = deviceOrdinal
                }
                Dim engine As CudaEngine = CudaEngine.TryCreate(options)

                If engine Is Nothing Then
                    Return Nothing
                End If

                Return New CudaCorrelationScreener(engine)
            Catch ex As Exception
                ' 没有显卡 / 驱动不匹配 / 显存不足：一律回退到 CPU，不把异常抛给调用方
                Return Nothing
            End Try
        End Function

        Private Sub New(engine As CudaEngine)
            Me.engine = engine
        End Sub

        ''' <summary>
        ''' 上传表达矩阵并一次性算完所有目标基因的候选集。
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

            Dim p As Integer = Me.pool.Length

            If Me.topK = 0 OrElse Me.sampleSize = 0 OrElse p = 0 Then
                Return
            End If

            Me.means = New Double(p - 1) {}
            Me.sds = New Double(p - 1) {}

            For k As Integer = 0 To p - 1
                Dim col As Double() = vectors(Me.pool(k))

                If col Is Nothing Then
                    Continue For
                End If

                Dim sum As Double = 0.0

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

            ' 转置成 n x p 的行主序矩阵：flat(i * p + k) = X[i][k]
            Dim flat As Single() = New Single(Me.sampleSize * p - 1) {}

            For i As Integer = 0 To Me.sampleSize - 1
                Dim offset As Integer = i * p

                For k As Integer = 0 To p - 1
                    Dim col As Double() = vectors(Me.pool(k))

                    If col IsNot Nothing Then
                        flat(offset + k) = CSng(col(i))
                    End If
                Next
            Next

            Me.deviceMatrix = New DeviceBuffer(Of Single)(flat.Length)
            Call Me.deviceMatrix.Write(flat)

            Dim blocks As Integer = (p + BlockSize - 1) \ BlockSize

            For block As Integer = 0 To blocks - 1
                Call ScreenBlock(vectors, block, p)
            Next
        End Sub

        ''' <summary>
        ''' 处理一个目标基因块：C(B x p) = A(B x n) * Xt(n x p)。
        ''' </summary>
        ''' <param name="vectors">全部基因的表达向量</param>
        ''' <param name="block">块下标</param>
        ''' <param name="p">候选基因数量</param>
        Private Sub ScreenBlock(vectors As Double()(), block As Integer, p As Integer)
            Dim start As Integer = block * BlockSize
            Dim last As Integer = std.Min(start + BlockSize, p) - 1
            Dim width As Integer = last - start + 1
            Dim n As Integer = sampleSize
            Dim a As Single() = New Single(width * n - 1) {}

            For b As Integer = 0 To width - 1
                Dim pos As Integer = start + b
                Dim col As Double() = vectors(pool(pos))
                Dim offset As Integer = b * n

                If col Is Nothing Then
                    Continue For
                End If

                For i As Integer = 0 To n - 1
                    a(offset + i) = CSng(col(i))
                Next
            Next

            Using deviceA As New DeviceBuffer(Of Single)(width * n),
                  deviceC As New DeviceBuffer(Of Single)(width * p)

                Call deviceA.Write(a)

                ' C(width x p) = A(width x n) * Xt(n x p)
                Call GpuBlas.Gemm(engine, deviceA, deviceMatrix, deviceC, width, p, n)

                Dim dots As Single() = deviceC.Read()

                For b As Integer = 0 To width - 1
                    Dim pos As Integer = start + b
                    Dim sb As Double = sds(pos)
                    Dim row As Double() = New Double(p - 1) {}

                    If sb <= 0.0R Then
                        results(pool(pos)) = New Integer() {}
                        Continue For
                    End If

                    Dim offset As Integer = b * p
                    Dim meanB As Double = means(pos)

                    For k As Integer = 0 To p - 1
                        Dim sk As Double = sds(k)

                        If sk <= 0.0R Then
                            Continue For
                        End If

                        Dim dot As Double = dots(offset + k) / n

                        row(k) = (dot - meanB * means(k)) / (sb * sk)
                    Next

                    results(pool(pos)) = CorrelationTopK.SelectTopK(row, pool, topK, minAbsR, pos)
                Next
            End Using
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
        ''' 释放显存缓冲与 CUDA 引擎。
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose
            results = Nothing
            means = Nothing
            sds = Nothing
            pool = Nothing

            If deviceMatrix IsNot Nothing Then
                Call deviceMatrix.Dispose()
                deviceMatrix = Nothing
            End If

            If engine IsNot Nothing Then
                Call engine.Dispose()
                engine = Nothing
            End If
        End Sub
    End Class

End Namespace
