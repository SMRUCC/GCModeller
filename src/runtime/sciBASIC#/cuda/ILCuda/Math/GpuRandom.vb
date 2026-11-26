' ------------------------------------------------------------------------
' 设备端伪随机数的主机封装（ILCuda）
'
' 生成器是"计数器式"的：第 i 个随机数只由 (seed, subsequence, offset + i)
' 决定，与线程/块划分无关，因此同一组参数在任何设备上都给出相同的序列。
'
' 这里同时提供一份逐位一致的 CPU 参考实现（<see cref="Pcg64"/>），
' 它既是 GPU 不可用时的回退路径，也是审计对拍的基准。
'
' 精度口径：
'   * 均匀分布：整数位运算 + 一次 2^-53 缩放，CPU 与 GPU 逐位相同
'   * 正态分布：Box-Muller，sqrt/log/cos 属超越函数，按 1e-12 判据
' ------------------------------------------------------------------------

Imports std = System.Math
Imports Microsoft.VisualBasic.Computing.ILCuda.Runtime

Namespace Math

    ''' <summary>
    ''' PCG64（XSL RR 128/64）的计数器式实现，与 rng.cu 逐位一致。
    ''' </summary>
    Public Structure Pcg64

        ''' <summary>128 位乘法常数的低 64 位（标准 pcg64 的 multiplier）</summary>
        Public Const MultiplierLow As ULong = &H4385DF649FCCF645UL
        ''' <summary>128 位乘法常数的高 64 位</summary>
        Public Const MultiplierHigh As ULong = &H2360ED051FC65DA4UL
        ''' <summary>SplitMix64 的黄金分割增量</summary>
        Public Const Golden As ULong = &H9E3779B97F4A7C15UL
        ''' <summary>[0,1) 均匀 double 的分辨率倒数</summary>
        Public Const TwoPowMinus53 As Double = 1.0 / 9007199254740992.0
        ''' <summary>2π（与内核里的常量保持同一字面量）</summary>
        Public Const TwoPi As Double = 6.283185307179586476925286766559

        ''' <summary>SplitMix64 混合函数</summary>
        Public Shared Function SplitMix64(x As ULong) As ULong
            Dim v As ULong = x + Golden
            Dim z As ULong = v

            z = (z Xor (z >> 30)) * &HBF58476D1CE4E5B9UL
            z = (z Xor (z >> 27)) * &H94D049BB133111EBUL

            Return z Xor (z >> 31)
        End Function

        ''' <summary>128 位状态推进：state = state * M + inc（inc 只有低 64 位非零）</summary>
        Public Shared Sub [Step](ByRef hi As ULong, ByRef lo As ULong, inc As ULong)
            Dim aLo As ULong = lo
            Dim aHi As ULong = hi
            Dim p0lo As ULong

            ' 高 64 位通过 BigMul 取得，低 64 位同时返回
            Dim p0hi As ULong = std.BigMul(aLo, MultiplierLow, p0lo)
            Dim p1lo As ULong = aLo * MultiplierHigh
            Dim p2lo As ULong = aHi * MultiplierLow

            Dim newHi As ULong = p0hi + p1lo + p2lo
            Dim newLo As ULong = p0lo + inc

            If newLo < p0lo Then newHi += 1UL

            hi = newHi
            lo = newLo
        End Sub

        ''' <summary>XSL RR 128/64 输出函数</summary>
        Public Shared Function [Output](hi As ULong, lo As ULong) As ULong
            Dim rot As Integer = CInt(hi >> 58)
            Dim x As ULong = hi Xor lo

            Return (x >> rot) Or (x << ((64 - rot) And 63))
        End Function

        ''' <summary>第 <paramref name="counter"/> 个 64 位输出</summary>
        Public Shared Function [Next](seed As ULong, subsequence As ULong, counter As ULong) As ULong
            Dim hi As ULong = SplitMix64(seed Xor (subsequence * Golden))
            Dim lo As ULong = SplitMix64(hi Xor counter)
            Dim inc As ULong = (subsequence << 1) Or 1UL

            Call [Step](hi, lo, inc)

            Return [Output](hi, lo)
        End Function

        ''' <summary>[0, 1) 的 53 位均匀 double</summary>
        Public Shared Function Uniform(seed As ULong, subsequence As ULong, counter As ULong) As Double
            Return CDbl([Next](seed, subsequence, counter) >> 11) * TwoPowMinus53
        End Function

        ''' <summary>标准正态（Box-Muller，每元素消费两个计数器）</summary>
        Public Shared Function Normal(seed As ULong, subsequence As ULong, index As ULong) As Double
            Dim base As ULong = index * 2UL
            Dim u1 As Double = Uniform(seed, subsequence, base)
            Dim u2 As Double = Uniform(seed, subsequence, base + 1UL)

            If u1 <= 0.0 Then u1 = TwoPowMinus53

            Return std.Sqrt(-2.0 * std.Log(u1)) * std.Cos(TwoPi * u2)
        End Function
    End Structure

    ''' <summary>均匀 / 正态 / 置换 / 加权抽样</summary>
    Public Module GpuRandom

        ''' <summary>内核每 block 的线程数</summary>
        Public Const DefaultBlockSize As Integer = 256

        ''' <summary>低于该元素数直接走 CPU（内核调度与两次拷贝的固定开销）</summary>
        Public Const MinGpuElements As Integer = 1024

        ' ==================================================================
        ' 均匀分布
        ' ==================================================================

        ''' <summary>[0, 1) 均匀分布的 double 序列。</summary>
        Public Function Uniform(engine As CudaEngine,
                                count As Integer,
                                seed As ULong,
                                Optional subsequence As ULong = 0UL,
                                Optional offset As ULong = 0UL) As Double()
            If count <= 0 Then Return New Double() {}

            If Not CanUseGpu(engine, count, KernelNames.RngUniformF64) Then
                Return UniformCpu(count, seed, subsequence, offset)
            End If

            Using output As New DeviceBuffer(Of Double)(count)

                engine.GetKernel(KernelNames.RngUniformF64).Launch1D(
                    count, DefaultBlockSize, 0,
                    output, count, ToSigned(seed), ToSigned(subsequence), ToSigned(offset))

                Return output.Read()
            End Using
        End Function

        ''' <summary>[0, 1) 均匀分布的单精度序列。</summary>
        Public Function UniformSingle(engine As CudaEngine,
                                      count As Integer,
                                      seed As ULong,
                                      Optional subsequence As ULong = 0UL,
                                      Optional offset As ULong = 0UL) As Single()
            If count <= 0 Then Return New Single() {}

            If Not CanUseGpu(engine, count, KernelNames.RngUniformF32) Then
                Return UniformSingleCpu(count, seed, subsequence, offset)
            End If

            Using output As New DeviceBuffer(Of Single)(count)

                engine.GetKernel(KernelNames.RngUniformF32).Launch1D(
                    count, DefaultBlockSize, 0,
                    output, count, ToSigned(seed), ToSigned(subsequence), ToSigned(offset))

                Return output.Read()
            End Using
        End Function

        ' ==================================================================
        ' 正态分布
        ' ==================================================================

        ''' <summary>标准正态序列（Box-Muller）。</summary>
        Public Function Normal(engine As CudaEngine,
                               count As Integer,
                               seed As ULong,
                               Optional subsequence As ULong = 0UL,
                               Optional offset As ULong = 0UL) As Double()
            If count <= 0 Then Return New Double() {}

            If Not CanUseGpu(engine, count, KernelNames.RngNormalF64) Then
                Return NormalCpu(count, seed, subsequence, offset)
            End If

            Using output As New DeviceBuffer(Of Double)(count)

                engine.GetKernel(KernelNames.RngNormalF64).Launch1D(
                    count, DefaultBlockSize, 0,
                    output, count, ToSigned(seed), ToSigned(subsequence), ToSigned(offset))

                Return output.Read()
            End Using
        End Function

        ''' <summary>标准正态序列（单精度）。</summary>
        Public Function NormalSingle(engine As CudaEngine,
                                     count As Integer,
                                     seed As ULong,
                                     Optional subsequence As ULong = 0UL,
                                     Optional offset As ULong = 0UL) As Single()
            If count <= 0 Then Return New Single() {}

            If Not CanUseGpu(engine, count, KernelNames.RngNormalF32) Then
                Return NormalSingleCpu(count, seed, subsequence, offset)
            End If

            Using output As New DeviceBuffer(Of Single)(count)

                engine.GetKernel(KernelNames.RngNormalF32).Launch1D(
                    count, DefaultBlockSize, 0,
                    output, count, ToSigned(seed), ToSigned(subsequence), ToSigned(offset))

                Return output.Read()
            End Using
        End Function

        ' ==================================================================
        ' 置换与加权抽样
        ' ==================================================================

        ''' <summary>0..n-1 的一个置换（Fisher-Yates，顺序语义与参考实现一致）。</summary>
        Public Function Permutation(engine As CudaEngine,
                                    n As Integer,
                                    seed As ULong,
                                    Optional subsequence As ULong = 0UL,
                                    Optional offset As ULong = 0UL) As Integer()
            If n <= 0 Then Return New Integer() {}

            If Not CanUseGpu(engine, n, KernelNames.RngPermutation) Then
                Return PermutationCpu(n, seed, subsequence, offset)
            End If

            Using output As New DeviceBuffer(Of Integer)(n)

                engine.GetKernel(KernelNames.RngPermutation).Launch1D(
                    1, 1, 0,
                    output, n, ToSigned(seed), ToSigned(subsequence), ToSigned(offset))

                Return output.Read()
            End Using
        End Function

        ''' <summary>按累积分布抽样：返回每次抽样命中的下标。</summary>
        Public Function WeightedChoice(engine As CudaEngine,
                                       cdf As Double(),
                                       draws As Integer,
                                       seed As ULong,
                                       Optional subsequence As ULong = 0UL,
                                       Optional offset As ULong = 0UL) As Integer()
            If cdf Is Nothing Then Throw New ArgumentNullException(NameOf(cdf))
            If cdf.Length = 0 OrElse draws <= 0 Then Return New Integer() {}

            If Not CanUseGpu(engine, draws, KernelNames.RngChoiceF64) Then
                Return WeightedChoiceCpu(cdf, draws, seed, subsequence, offset)
            End If

            Using deviceCdf As New DeviceBuffer(Of Double)(cdf.Length),
                  output As New DeviceBuffer(Of Integer)(draws)

                deviceCdf.Write(cdf)

                engine.GetKernel(KernelNames.RngChoiceF64).Launch1D(
                    draws, DefaultBlockSize, 0,
                    deviceCdf, cdf.Length, output, draws,
                    ToSigned(seed), ToSigned(subsequence), ToSigned(offset))

                Return output.Read()
            End Using
        End Function

        ' ==================================================================
        ' CPU 参考实现（回退 + 审计基准）
        ' ==================================================================

        ''' <summary>均匀分布的 CPU 参考实现（与内核逐位一致）。</summary>
        Public Function UniformCpu(count As Integer, seed As ULong,
                                   Optional subsequence As ULong = 0UL,
                                   Optional offset As ULong = 0UL) As Double()
            Dim result(count - 1) As Double

            For i As Integer = 0 To count - 1
                result(i) = Pcg64.Uniform(seed, subsequence, offset + CULng(i))
            Next

            Return result
        End Function

        ''' <summary>均匀分布的 CPU 参考实现（单精度）。</summary>
        Public Function UniformSingleCpu(count As Integer, seed As ULong,
                                         Optional subsequence As ULong = 0UL,
                                         Optional offset As ULong = 0UL) As Single()
            Dim result(count - 1) As Single

            For i As Integer = 0 To count - 1
                result(i) = CSng(Pcg64.Uniform(seed, subsequence, offset + CULng(i)))
            Next

            Return result
        End Function

        ''' <summary>标准正态的 CPU 参考实现。</summary>
        Public Function NormalCpu(count As Integer, seed As ULong,
                                  Optional subsequence As ULong = 0UL,
                                  Optional offset As ULong = 0UL) As Double()
            Dim result(count - 1) As Double

            For i As Integer = 0 To count - 1
                result(i) = Pcg64.Normal(seed, subsequence, offset + CULng(i))
            Next

            Return result
        End Function

        ''' <summary>标准正态的 CPU 参考实现（单精度）。</summary>
        Public Function NormalSingleCpu(count As Integer, seed As ULong,
                                        Optional subsequence As ULong = 0UL,
                                        Optional offset As ULong = 0UL) As Single()
            Dim result(count - 1) As Single

            For i As Integer = 0 To count - 1
                result(i) = CSng(Pcg64.Normal(seed, subsequence, offset + CULng(i)))
            Next

            Return result
        End Function

        ''' <summary>置换的 CPU 参考实现。</summary>
        Public Function PermutationCpu(n As Integer, seed As ULong,
                                       Optional subsequence As ULong = 0UL,
                                       Optional offset As ULong = 0UL) As Integer()
            Dim result(n - 1) As Integer

            For i As Integer = 0 To n - 1
                result(i) = i
            Next

            For i As Integer = n - 1 To 1 Step -1
                Dim u As Double = Pcg64.Uniform(seed, subsequence, offset + CULng(i))
                Dim j As Integer = CInt(std.Floor(u * CDbl(i + 1)))

                If j >= 0 AndAlso j <= i AndAlso j <> i Then
                    Dim swap As Integer = result(i)

                    result(i) = result(j)
                    result(j) = swap
                End If
            Next

            Return result
        End Function

        ''' <summary>加权抽样的 CPU 参考实现。</summary>
        Public Function WeightedChoiceCpu(cdf As Double(), draws As Integer, seed As ULong,
                                          Optional subsequence As ULong = 0UL,
                                          Optional offset As ULong = 0UL) As Integer()
            Dim result(draws - 1) As Integer

            For k As Integer = 0 To draws - 1
                Dim u As Double = Pcg64.Uniform(seed, subsequence, offset + CULng(k))
                Dim lo As Integer = 0
                Dim hi As Integer = cdf.Length

                While lo < hi
                    Dim mid As Integer = (lo + hi) >> 1

                    If cdf(mid) > u Then
                        hi = mid
                    Else
                        lo = mid + 1
                    End If
                End While

                result(k) = If(lo < cdf.Length, lo, cdf.Length - 1)
            Next

            Return result
        End Function

        ' ==================================================================
        ' 内部工具
        ' ==================================================================

        ''' <summary>规模过小、引擎缺失或内核缺失时走 CPU。</summary>
        Public Function CanUseGpu(engine As CudaEngine, elements As Long, kernelName As String) As Boolean
            If engine Is Nothing Then Return False
            If elements < MinGpuElements Then Return False

            Dim kernel As CudaKernel = Nothing

            Return engine.TryGetKernel(kernelName, kernel)
        End Function

        ''' <summary>把 ULong 的位模式原样作为内核参数（内核声明为 unsigned long long）。</summary>
        Public Function ToSigned(value As ULong) As Long
            Return BitConverter.ToInt64(BitConverter.GetBytes(value), 0)
        End Function
    End Module
End Namespace
