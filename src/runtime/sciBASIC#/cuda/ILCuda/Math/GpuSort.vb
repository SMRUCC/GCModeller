' ------------------------------------------------------------------------
' 排序 / 查找 / 选择 的主机封装（ILCuda）
'
' 每个算子都提供三样东西：
'   1. 设备缓冲入口（数据已经在显存里，只做内核调度）
'   2. 主机数组入口（内部完成 H2D / D2H，适合一次性调用）
'   3. CPU 参考实现（GPU 不可用、规模过小、内核缺失时自动回退，
'      同时也是审计对拍的基准）
'
' 精度口径：F64 与 F32 双栈。F32 用于"与上游 float32 排序栅格一致"的
' 场合（例如每细胞 CPM 的并列判定），F64 用于一般数值。
'
' NaN 语义：升序排序时 NaN 一律沉到段尾；并列和里每个 NaN 自成一段。
' ------------------------------------------------------------------------

Imports std = System.Math
Imports Microsoft.VisualBasic.Computing.ILCuda.Runtime

Namespace Math

    ''' <summary>分段基数排序、批量二分查找、前 K 名选择与并列和</summary>
    Public Module GpuSort

        ''' <summary>排序族内核每 block 的线程数（必须与 sort.cu 的前缀和实现一致）</summary>
        Public Const DefaultBlockSize As Integer = 256

        ''' <summary>
        ''' 低于该元素数直接走 CPU：一次内核调度 + 两次 H2D/D2H 的固定开销
        ''' 会盖过小规模排序本身的成本。
        ''' </summary>
        Public Const MinGpuElements As Integer = 4096

        ' ==================================================================
        ' 分段排序：设备缓冲入口
        ' ==================================================================

        ''' <summary>
        ''' 原地分段排序（双精度）：<paramref name="keys"/> 被就地改写。
        ''' </summary>
        ''' <param name="keys">[segmentCount, segmentLength] 连续存放的键</param>
        ''' <param name="scratch">同样长度的乒乓缓冲，内容会被覆盖</param>
        Public Sub SortSegments(engine As CudaEngine,
                                keys As DeviceBuffer(Of Double),
                                scratch As DeviceBuffer(Of Double),
                                segmentLength As Integer,
                                segmentCount As Integer,
                                Optional stream As CudaStream = Nothing)
            If engine Is Nothing Then Throw New ArgumentNullException(NameOf(engine))
            RequireShape(keys, scratch, segmentLength, segmentCount)

            Dim kernel = engine.GetKernel(KernelNames.SortSegmentsF64)
            Dim config = New LaunchConfig(segmentCount, 1, DefaultBlockSize, 1, 0)
            Dim source = keys
            Dim target = scratch

            ' 双精度 8 个字节 -> 8 趟；偶数趟，所以最终结果回到 keys
            For pass As Integer = 0 To 7
                kernel.Launch(stream, config, source, target, segmentLength, pass * 8)

                Dim swap = source

                source = target
                target = swap
            Next
        End Sub

        ''' <summary>
        ''' 原地分段排序（单精度）：<paramref name="keys"/> 被就地改写。
        ''' </summary>
        Public Sub SortSegments(engine As CudaEngine,
                                keys As DeviceBuffer(Of Single),
                                scratch As DeviceBuffer(Of Single),
                                segmentLength As Integer,
                                segmentCount As Integer,
                                Optional stream As CudaStream = Nothing)
            If engine Is Nothing Then Throw New ArgumentNullException(NameOf(engine))
            RequireShape(keys, scratch, segmentLength, segmentCount)

            Dim kernel = engine.GetKernel(KernelNames.SortSegmentsF32)
            Dim config = New LaunchConfig(segmentCount, 1, DefaultBlockSize, 1, 0)
            Dim source = keys
            Dim target = scratch

            ' 单精度 4 个字节 -> 4 趟；偶数趟，所以最终结果回到 keys
            For pass As Integer = 0 To 3
                kernel.Launch(stream, config, source, target, segmentLength, pass * 8)

                Dim swap = source

                source = target
                target = swap
            Next
        End Sub

        ' ==================================================================
        ' 分段排序：主机数组入口
        ' ==================================================================

        ''' <summary>分段排序（双精度），返回新的数组，输入不被修改。</summary>
        Public Function SortSegments(engine As CudaEngine, keys As Double(), segmentLength As Integer) As Double()
            If keys Is Nothing Then Throw New ArgumentNullException(NameOf(keys))
            If segmentLength <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(segmentLength))
            If keys.Length Mod segmentLength <> 0 Then
                Throw New ArgumentException($"keys 长度 {keys.Length} 不是段长 {segmentLength} 的整数倍")
            End If

            Dim segmentCount As Integer = keys.Length \ segmentLength

            If Not CanUseGpu(engine, keys.Length, KernelNames.SortSegmentsF64) Then
                Return SortSegmentsCpu(keys, segmentLength)
            End If

            Using deviceKeys As New DeviceBuffer(Of Double)(keys.Length),
                  scratch As New DeviceBuffer(Of Double)(keys.Length)

                deviceKeys.Write(keys)
                SortSegments(engine, deviceKeys, scratch, segmentLength, segmentCount)

                Return deviceKeys.Read()
            End Using
        End Function

        ''' <summary>分段排序（单精度），返回新的数组，输入不被修改。</summary>
        Public Function SortSegments(engine As CudaEngine, keys As Single(), segmentLength As Integer) As Single()
            If keys Is Nothing Then Throw New ArgumentNullException(NameOf(keys))
            If segmentLength <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(segmentLength))
            If keys.Length Mod segmentLength <> 0 Then
                Throw New ArgumentException($"keys 长度 {keys.Length} 不是段长 {segmentLength} 的整数倍")
            End If

            Dim segmentCount As Integer = keys.Length \ segmentLength

            If Not CanUseGpu(engine, keys.Length, KernelNames.SortSegmentsF32) Then
                Return SortSegmentsCpu(keys, segmentLength)
            End If

            Using deviceKeys As New DeviceBuffer(Of Single)(keys.Length),
                  scratch As New DeviceBuffer(Of Single)(keys.Length)

                deviceKeys.Write(keys)
                SortSegments(engine, deviceKeys, scratch, segmentLength, segmentCount)

                Return deviceKeys.Read()
            End Using
        End Function

        ' ==================================================================
        ' 批量二分查找
        ' ==================================================================

        ''' <summary>
        ''' 对每个查询值给出它在对应排序段内的 lower / upper bound（双精度）。
        ''' </summary>
        ''' <param name="sorted">已升序排序的段（NaN 在段尾）</param>
        ''' <param name="queries">[segmentCount, queryPerSegment]，查询值必须是有限值</param>
        Public Function Bounds(engine As CudaEngine,
                               sorted As Double(),
                               queries As Double(),
                               segmentLength As Integer,
                               queryPerSegment As Integer) As (lower As Integer(), upper As Integer())
            If sorted Is Nothing Then Throw New ArgumentNullException(NameOf(sorted))
            If queries Is Nothing Then Throw New ArgumentNullException(NameOf(queries))
            If segmentLength <= 0 OrElse queryPerSegment <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(segmentLength))
            If sorted.Length Mod segmentLength <> 0 Then
                Throw New ArgumentException($"sorted 长度 {sorted.Length} 不是段长 {segmentLength} 的整数倍")
            End If

            Dim segmentCount As Integer = sorted.Length \ segmentLength

            If queries.Length <> segmentCount * queryPerSegment Then
                Throw New ArgumentException($"queries 长度 {queries.Length} 与 [{segmentCount} x {queryPerSegment}] 不一致")
            End If

            If Not CanUseGpu(engine, sorted.Length, KernelNames.SegBoundsF64) Then
                Return BoundsCpu(sorted, queries, segmentLength, queryPerSegment)
            End If

            Using deviceSorted As New DeviceBuffer(Of Double)(sorted.Length),
                  deviceQueries As New DeviceBuffer(Of Double)(queries.Length),
                  lower As New DeviceBuffer(Of Integer)(queries.Length),
                  upper As New DeviceBuffer(Of Integer)(queries.Length)

                deviceSorted.Write(sorted)
                deviceQueries.Write(queries)

                engine.GetKernel(KernelNames.SegBoundsF64).Launch(
                    New LaunchConfig(segmentCount, 1, DefaultBlockSize, 1, 0),
                    deviceSorted, deviceQueries, lower, upper, segmentLength, queryPerSegment)

                Return (lower.Read(), upper.Read())
            End Using
        End Function

        ''' <summary>对每个查询值给出它在对应排序段内的 lower / upper bound（单精度）。</summary>
        Public Function Bounds(engine As CudaEngine,
                               sorted As Single(),
                               queries As Single(),
                               segmentLength As Integer,
                               queryPerSegment As Integer) As (lower As Integer(), upper As Integer())
            If sorted Is Nothing Then Throw New ArgumentNullException(NameOf(sorted))
            If queries Is Nothing Then Throw New ArgumentNullException(NameOf(queries))
            If segmentLength <= 0 OrElse queryPerSegment <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(segmentLength))
            If sorted.Length Mod segmentLength <> 0 Then
                Throw New ArgumentException($"sorted 长度 {sorted.Length} 不是段长 {segmentLength} 的整数倍")
            End If

            Dim segmentCount As Integer = sorted.Length \ segmentLength

            If queries.Length <> segmentCount * queryPerSegment Then
                Throw New ArgumentException($"queries 长度 {queries.Length} 与 [{segmentCount} x {queryPerSegment}] 不一致")
            End If

            If Not CanUseGpu(engine, sorted.Length, KernelNames.SegBoundsF32) Then
                Return BoundsCpu(sorted, queries, segmentLength, queryPerSegment)
            End If

            Using deviceSorted As New DeviceBuffer(Of Single)(sorted.Length),
                  deviceQueries As New DeviceBuffer(Of Single)(queries.Length),
                  lower As New DeviceBuffer(Of Integer)(queries.Length),
                  upper As New DeviceBuffer(Of Integer)(queries.Length)

                deviceSorted.Write(sorted)
                deviceQueries.Write(queries)

                engine.GetKernel(KernelNames.SegBoundsF32).Launch(
                    New LaunchConfig(segmentCount, 1, DefaultBlockSize, 1, 0),
                    deviceSorted, deviceQueries, lower, upper, segmentLength, queryPerSegment)

                Return (lower.Read(), upper.Read())
            End Using
        End Function

        ' ==================================================================
        ' 前 K 名（单段）
        ' ==================================================================

        ''' <summary>
        ''' 单段前 <paramref name="k"/> 名（按值降序）的下标；值相同时按下标升序，结果确定。
        ''' 返回的数组长度可能小于 k（k 超过元素数时截断）。
        ''' </summary>
        ''' <remarks>
        ''' 只支持单段：它服务的是"基因轴"这一类整体排名（例如高变基因筛选）。
        ''' 多段场景请由调用方循环。
        ''' </remarks>
        Public Function TopKIndices(engine As CudaEngine, keys As Single(), k As Integer) As Integer()
            If keys Is Nothing Then Throw New ArgumentNullException(NameOf(keys))
            If keys.Length = 0 OrElse k <= 0 Then Return New Integer() {}

            Dim count As Integer = std.Min(k, keys.Length)

            If Not CanUseGpu(engine, keys.Length, KernelNames.SegSelectF32) Then
                Return TopKIndicesCpu(keys, count)
            End If

            Using deviceKeys As New DeviceBuffer(Of Single)(keys.Length),
                  threshold As New DeviceBuffer(Of Single)(1),
                  indices As New DeviceBuffer(Of Integer)(count),
                  written As New DeviceBuffer(Of Integer)(1)

                deviceKeys.Write(keys)

                engine.GetKernel(KernelNames.SegSelectF32).Launch(
                    New LaunchConfig(1, 1, DefaultBlockSize, 1, 0),
                    deviceKeys, keys.Length, count, threshold)

                Dim thresholdValue As Single = threshold.Read()(0)

                engine.GetKernel(KernelNames.SegCollectF32).Launch(
                    New LaunchConfig(1, 1, 1, 1, 0),
                    deviceKeys, keys.Length, thresholdValue, count, indices, written)

                Dim total As Integer = std.Min(written.Read()(0), count)

                If total <= 0 Then Return New Integer() {}

                Dim buffer As Integer() = indices.Read()
                Dim result(total - 1) As Integer

                Array.Copy(buffer, result, total)

                ' 内核只保证"集合"正确（按阈值收集 + 按下标扫描）；
                ' 展示顺序在这里统一收口：值降序、值相同按下标升序。
                Array.Sort(result, New TopKComparer(keys))

                Return result
            End Using
        End Function

        ' ==================================================================
        ' 并列和 Σ(t³ - t)
        ' ==================================================================

        ''' <summary>逐段并列和（双精度），长度 = 段数。</summary>
        Public Function TieSum(engine As CudaEngine, sorted As Double(), segmentLength As Integer) As Double()
            If sorted Is Nothing Then Throw New ArgumentNullException(NameOf(sorted))
            If segmentLength <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(segmentLength))
            If sorted.Length Mod segmentLength <> 0 Then
                Throw New ArgumentException($"sorted 长度 {sorted.Length} 不是段长 {segmentLength} 的整数倍")
            End If

            Dim segmentCount As Integer = sorted.Length \ segmentLength

            If Not CanUseGpu(engine, sorted.Length, KernelNames.SegTieSumF64) Then
                Return TieSumCpu(sorted, segmentLength)
            End If

            Using deviceSorted As New DeviceBuffer(Of Double)(sorted.Length),
                  output As New DeviceBuffer(Of Double)(segmentCount)

                deviceSorted.Write(sorted)

                engine.GetKernel(KernelNames.SegTieSumF64).Launch(
                    New LaunchConfig(segmentCount, 1, 1, 1, 0),
                    deviceSorted, output, segmentLength)

                Return output.Read()
            End Using
        End Function

        ''' <summary>逐段并列和（单精度），长度 = 段数。</summary>
        Public Function TieSum(engine As CudaEngine, sorted As Single(), segmentLength As Integer) As Double()
            If sorted Is Nothing Then Throw New ArgumentNullException(NameOf(sorted))
            If segmentLength <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(segmentLength))
            If sorted.Length Mod segmentLength <> 0 Then
                Throw New ArgumentException($"sorted 长度 {sorted.Length} 不是段长 {segmentLength} 的整数倍")
            End If

            Dim segmentCount As Integer = sorted.Length \ segmentLength

            If Not CanUseGpu(engine, sorted.Length, KernelNames.SegTieSumF32) Then
                Return TieSumCpu(sorted, segmentLength)
            End If

            Using deviceSorted As New DeviceBuffer(Of Single)(sorted.Length),
                  output As New DeviceBuffer(Of Double)(segmentCount)

                deviceSorted.Write(sorted)

                engine.GetKernel(KernelNames.SegTieSumF32).Launch(
                    New LaunchConfig(segmentCount, 1, 1, 1, 0),
                    deviceSorted, output, segmentLength)

                Return output.Read()
            End Using
        End Function

        ' ==================================================================
        ' CPU 参考实现（回退路径 + 审计基准）
        ' ==================================================================

        ''' <summary>分段排序的 CPU 参考实现（NaN 沉到段尾，-0.0 与 +0.0 视为相等）。</summary>
        Public Function SortSegmentsCpu(keys As Double(), segmentLength As Integer) As Double()
            Dim result As Double() = DirectCast(keys.Clone(), Double())
            Dim comparer As New NaNLastComparer()

            For segment As Integer = 0 To (result.Length \ segmentLength) - 1
                Array.Sort(result, segment * segmentLength, segmentLength, comparer)
            Next

            Return result
        End Function

        ''' <summary>分段排序的 CPU 参考实现（单精度）。</summary>
        Public Function SortSegmentsCpu(keys As Single(), segmentLength As Integer) As Single()
            Dim result As Single() = DirectCast(keys.Clone(), Single())
            Dim comparer As New NaNLastComparerSingle()

            For segment As Integer = 0 To (result.Length \ segmentLength) - 1
                Array.Sort(result, segment * segmentLength, segmentLength, comparer)
            Next

            Return result
        End Function

        ''' <summary>批量二分查找的 CPU 参考实现（双精度）。</summary>
        Public Function BoundsCpu(sorted As Double(),
                                  queries As Double(),
                                  segmentLength As Integer,
                                  queryPerSegment As Integer) As (lower As Integer(), upper As Integer())
            Dim segmentCount As Integer = sorted.Length \ segmentLength
            Dim lower(queries.Length - 1) As Integer
            Dim upper(queries.Length - 1) As Integer

            For segment As Integer = 0 To segmentCount - 1
                Dim offset As Integer = segment * segmentLength

                For q As Integer = 0 To queryPerSegment - 1
                    Dim index As Integer = segment * queryPerSegment + q
                    Dim value As Double = queries(index)

                    lower(index) = LowerBound(sorted, offset, segmentLength, value)
                    upper(index) = UpperBound(sorted, offset, segmentLength, value)
                Next
            Next

            Return (lower, upper)
        End Function

        ''' <summary>批量二分查找的 CPU 参考实现（单精度）。</summary>
        Public Function BoundsCpu(sorted As Single(),
                                  queries As Single(),
                                  segmentLength As Integer,
                                  queryPerSegment As Integer) As (lower As Integer(), upper As Integer())
            Dim segmentCount As Integer = sorted.Length \ segmentLength
            Dim lower(queries.Length - 1) As Integer
            Dim upper(queries.Length - 1) As Integer

            For segment As Integer = 0 To segmentCount - 1
                Dim offset As Integer = segment * segmentLength

                For q As Integer = 0 To queryPerSegment - 1
                    Dim index As Integer = segment * queryPerSegment + q
                    Dim value As Single = queries(index)

                    lower(index) = LowerBound(sorted, offset, segmentLength, value)
                    upper(index) = UpperBound(sorted, offset, segmentLength, value)
                Next
            Next

            Return (lower, upper)
        End Function

        ''' <summary>前 K 名的 CPU 参考实现（值降序，值相同按下标升序）。</summary>
        Public Function TopKIndicesCpu(keys As Single(), k As Integer) As Integer()
            Dim count As Integer = std.Min(k, keys.Length)
            Dim indices(keys.Length - 1) As Integer

            For i As Integer = 0 To indices.Length - 1
                indices(i) = i
            Next

            Dim comparer As New TopKComparer(keys)

            Array.Sort(indices, comparer)

            Dim result(count - 1) As Integer

            Array.Copy(indices, result, count)

            Return result
        End Function

        ''' <summary>逐段并列和的 CPU 参考实现（双精度）。</summary>
        Public Function TieSumCpu(sorted As Double(), segmentLength As Integer) As Double()
            Dim segmentCount As Integer = sorted.Length \ segmentLength
            Dim result(segmentCount - 1) As Double

            For segment As Integer = 0 To segmentCount - 1
                Dim offset As Integer = segment * segmentLength
                Dim acc As Double = 0.0
                Dim i As Integer = 0

                While i < segmentLength
                    Dim value As Double = sorted(offset + i)
                    Dim j As Integer = i + 1

                    While j < segmentLength AndAlso sorted(offset + j) = value
                        j += 1
                    End While

                    Dim t As Double = j - i

                    acc += t * t * t - t
                    i = j
                End While

                result(segment) = acc
            Next

            Return result
        End Function

        ''' <summary>逐段并列和的 CPU 参考实现（单精度）。</summary>
        Public Function TieSumCpu(sorted As Single(), segmentLength As Integer) As Double()
            Dim segmentCount As Integer = sorted.Length \ segmentLength
            Dim result(segmentCount - 1) As Double

            For segment As Integer = 0 To segmentCount - 1
                Dim offset As Integer = segment * segmentLength
                Dim acc As Double = 0.0
                Dim i As Integer = 0

                While i < segmentLength
                    Dim value As Single = sorted(offset + i)
                    Dim j As Integer = i + 1

                    While j < segmentLength AndAlso sorted(offset + j) = value
                        j += 1
                    End While

                    Dim t As Double = j - i

                    acc += t * t * t - t
                    i = j
                End While

                result(segment) = acc
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

        Private Sub RequireShape(keys As DeviceBuffer(Of Double), scratch As DeviceBuffer(Of Double),
                                 segmentLength As Integer, segmentCount As Integer)
            If keys Is Nothing Then Throw New ArgumentNullException(NameOf(keys))
            If scratch Is Nothing Then Throw New ArgumentNullException(NameOf(scratch))
            If segmentLength <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(segmentLength))
            If segmentCount <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(segmentCount))

            Dim expected As Integer = segmentLength * segmentCount

            If keys.Count <> expected Then
                Throw New ArgumentException($"keys 长度 {keys.Count} 与 [{segmentCount} x {segmentLength}] 不一致")
            End If
            If scratch.Count <> expected Then
                Throw New ArgumentException($"scratch 长度 {scratch.Count} 与 [{segmentCount} x {segmentLength}] 不一致")
            End If
        End Sub

        Private Sub RequireShape(keys As DeviceBuffer(Of Single), scratch As DeviceBuffer(Of Single),
                                 segmentLength As Integer, segmentCount As Integer)
            If keys Is Nothing Then Throw New ArgumentNullException(NameOf(keys))
            If scratch Is Nothing Then Throw New ArgumentNullException(NameOf(scratch))
            If segmentLength <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(segmentLength))
            If segmentCount <= 0 Then Throw New ArgumentOutOfRangeException(NameOf(segmentCount))

            Dim expected As Integer = segmentLength * segmentCount

            If keys.Count <> expected Then
                Throw New ArgumentException($"keys 长度 {keys.Count} 与 [{segmentCount} x {segmentLength}] 不一致")
            End If
            If scratch.Count <> expected Then
                Throw New ArgumentException($"scratch 长度 {scratch.Count} 与 [{segmentCount} x {segmentLength}] 不一致")
            End If
        End Sub

        Private Function LowerBound(sorted As Double(), offset As Integer, length As Integer, value As Double) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = length

            While lo < hi
                Dim mid As Integer = (lo + hi) >> 1

                If sorted(offset + mid) < value Then
                    lo = mid + 1
                Else
                    hi = mid
                End If
            End While

            Return lo
        End Function

        Private Function UpperBound(sorted As Double(), offset As Integer, length As Integer, value As Double) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = length

            While lo < hi
                Dim mid As Integer = (lo + hi) >> 1

                If sorted(offset + mid) <= value Then
                    lo = mid + 1
                Else
                    hi = mid
                End If
            End While

            Return lo
        End Function

        Private Function LowerBound(sorted As Single(), offset As Integer, length As Integer, value As Single) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = length

            While lo < hi
                Dim mid As Integer = (lo + hi) >> 1

                If sorted(offset + mid) < value Then
                    lo = mid + 1
                Else
                    hi = mid
                End If
            End While

            Return lo
        End Function

        Private Function UpperBound(sorted As Single(), offset As Integer, length As Integer, value As Single) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = length

            While lo < hi
                Dim mid As Integer = (lo + hi) >> 1

                If sorted(offset + mid) <= value Then
                    lo = mid + 1
                Else
                    hi = mid
                End If
            End While

            Return lo
        End Function

        ''' <summary>升序排序，NaN 一律沉到段尾（与内核的键变换口径一致）。</summary>
        Private Class NaNLastComparer
            Implements IComparer(Of Double)

            Public Function Compare(x As Double, y As Double) As Integer Implements IComparer(Of Double).Compare
                Dim xn As Boolean = Double.IsNaN(x)
                Dim yn As Boolean = Double.IsNaN(y)

                If xn Then Return If(yn, 0, 1)
                If yn Then Return -1

                Return x.CompareTo(y)
            End Function
        End Class

        ''' <summary>升序排序，NaN 一律沉到段尾（单精度）。</summary>
        Private Class NaNLastComparerSingle
            Implements IComparer(Of Single)

            Public Function Compare(x As Single, y As Single) As Integer Implements IComparer(Of Single).Compare
                Dim xn As Boolean = Single.IsNaN(x)
                Dim yn As Boolean = Single.IsNaN(y)

                If xn Then Return If(yn, 0, 1)
                If yn Then Return -1

                Return x.CompareTo(y)
            End Function
        End Class

        ''' <summary>值降序、值相同按下标升序；NaN 视作最小（排在最后）。</summary>
        Private Class TopKComparer
            Implements IComparer(Of Integer)

            Private ReadOnly m_keys As Single()

            Public Sub New(keys As Single())
                m_keys = keys
            End Sub

            Public Function Compare(a As Integer, b As Integer) As Integer Implements IComparer(Of Integer).Compare
                Dim x As Single = m_keys(a)
                Dim y As Single = m_keys(b)
                Dim xn As Boolean = Single.IsNaN(x)
                Dim yn As Boolean = Single.IsNaN(y)

                If xn OrElse yn Then
                    If xn AndAlso yn Then Return a.CompareTo(b)
                    Return If(xn, 1, -1)
                End If

                Dim c As Integer = y.CompareTo(x)

                Return If(c <> 0, c, a.CompareTo(b))
            End Function
        End Class
    End Module
End Namespace
