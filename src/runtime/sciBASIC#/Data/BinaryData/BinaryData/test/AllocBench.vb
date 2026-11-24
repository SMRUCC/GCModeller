Option Strict On
Option Explicit On

' /********************************************************************************/
'  Microsoft.VisualBasic.Data.IO ArrayPool 基准
'  Data\BinaryData\BinaryData\test\AllocBench.vb
' /********************************************************************************/

Imports System
Imports System.Buffers
Imports System.IO
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Debugging
Imports Microsoft.VisualBasic.Data.IO
Imports Microsoft.VisualBasic.Data.IO.Snappy

''' <summary>
''' <c>ArrayPool</c> 池化改造的分配量基准。
'''
''' 所有测量值都通过 <see cref="GC.GetAllocatedBytesForCurrentThread"/> 与
''' <see cref="GC.CollectionCount(Int32)"/> 获取，并通过 <see cref="DebugAudit"/>
''' 写入测试报告，作为改造前后的量化对比依据。
''' </summary>
Module AllocBench

    Private Const KiB As Integer = 1024
    Private Const MiB As Integer = 1024 * 1024

    ''' <summary>
    ''' 单个 64 KiB Snappy 分块的压缩输出上界，对应改动前每个分块都要新分配的字节数之一。
    ''' </summary>
    Private Const MaxChunkCompressed As Integer = 76490

    ''' <summary>
    ''' Snappy 编码器哈希表的字节数（16384 个 Integer）。改动前每个 64 KiB 分片都会重建一次。
    ''' </summary>
    Private Const SnappyHashTableBytes As Integer = 65536

    ''' <summary>基准用例的迭代次数。</summary>
    Private Const RentIterations As Integer = 8192

    ''' <summary>
    ''' 运行全部基准并把结果写入 DebugAudit 报告。
    ''' </summary>
    Public Sub Run()
        Console.WriteLine()
        Console.WriteLine("===== ArrayPool 分配量基准 =====")

        Call BenchRentVsNew()
        Call BenchSnappyFramed()
    End Sub

    ' ------------------------------------------------------------------
    ' 1) 租借 vs 每次新建：ArrayPool 核心收益的直接对照
    ' ------------------------------------------------------------------
    Private Sub BenchRentVsNew()
        Dim pool As ArrayPool(Of Byte) = ArrayPool(Of Byte).Shared

        ' 预热：完成首次分桶
        Dim warm As Byte() = pool.Rent(MaxChunkCompressed)

        Call pool.Return(warm)

        Dim before As Long = GC.GetAllocatedBytesForCurrentThread()
        Dim gcBefore As Integer = GC.CollectionCount(0)

        For i As Integer = 1 To RentIterations
            Dim buffer As Byte() = pool.Rent(MaxChunkCompressed)

            buffer(0) = CByte(i And &HFF)
            Call pool.Return(buffer)
        Next

        Dim pooled As Long = GC.GetAllocatedBytesForCurrentThread() - before
        Dim pooledGc As Integer = GC.CollectionCount(0) - gcBefore

        before = GC.GetAllocatedBytesForCurrentThread()
        gcBefore = GC.CollectionCount(0)

        For i As Integer = 1 To RentIterations
            Dim buffer As Byte() = New Byte(MaxChunkCompressed - 1) {}

            buffer(0) = CByte(i And &HFF)
        Next

        Dim fresh As Long = GC.GetAllocatedBytesForCurrentThread() - before
        Dim freshGc As Integer = GC.CollectionCount(0) - gcBefore

        Call Report("bench:rent-vs-new:pooled", pooled, pooledGc)
        Call Report("bench:rent-vs-new:new", fresh, freshGc)

        ' 池化路径的分配量应当比每次新建低两个数量级
        Call TestSupport.AssertTrue(pooled * 100 < fresh,
                                    $"ArrayPool 租借应当远低于每次新建：pooled={pooled} fresh={fresh}")
    End Sub

    ' ------------------------------------------------------------------
    ' 2) Snappy 分块流往返：改动前每个分块都要新建
    '    输出缓冲、哈希表、裁剪副本与读取载荷
    ' ------------------------------------------------------------------
    Private Sub BenchSnappyFramed()
        Dim data As Byte() = CreateSample(2 * MiB)

        ' 预热
        Call SnappyRoundTrip(data)
        Call SnappyRoundTrip(data)

        Dim before As Long = GC.GetAllocatedBytesForCurrentThread()
        Dim gcBefore As Integer = GC.CollectionCount(0)

        Call SnappyRoundTrip(data)

        Dim allocated As Long = GC.GetAllocatedBytesForCurrentThread() - before
        Dim gen0 As Integer = GC.CollectionCount(0) - gcBefore

        Call Report("bench:snappy-framed-2MiB:measured", allocated, gen0)

        ' 记录改动前这些路径上「每个分块」都会发生的分配量上界，便于人工核对报告
        Dim chunks As Integer = (data.Length + 65535) \ 65536
        Dim removed As Long = CLng(chunks) * (MaxChunkCompressed + SnappyHashTableBytes + MaxChunkCompressed)

        Call Report("bench:snappy-framed-2MiB:removed-upper-bound", removed, 0)
        Call TestSupport.AssertTrue(allocated < removed,
                                    $"池化后的实测分配量应低于改造前每分块临时缓冲的上界：allocated={allocated} removed<={removed}")
    End Sub

    ''' <summary>
    ''' 通过 Snappy 分块流完成一次压缩 + 解压往返，并校验长度一致。
    ''' </summary>
    Private Sub SnappyRoundTrip(data As Byte())
        Using compressed As New MemoryStream()
            Using writer As New SnappyOutputStream(compressed, leaveOpen:=True)
                Call writer.Write(data, 0, data.Length)
            End Using

            Call compressed.Seek(0, SeekOrigin.Begin)

            Using reader As New SnappyInputStream(compressed, leaveOpen:=True)
                Dim restored As Byte() = BufferPool.ReadToEnd(reader)

                If restored.LongLength <> data.LongLength Then
                    Throw New InvalidDataException($"Snappy 往返长度不一致：{restored.LongLength} vs {data.LongLength}")
                End If
            End Using
        End Using
    End Sub

    ''' <summary>
    ''' 生成一段可压缩性适中的样本数据（重复的文本片段 + 变化的数值后缀）。
    ''' </summary>
    Private Function CreateSample(size As Integer) As Byte()
        Dim text As New StringBuilder()
        Dim i As Integer = 0

        While text.Length < size
            Call text.Append("row-").Append(i Mod 9973).Append(":the quick brown fox jumps over the lazy dog;")

            i += 1
        End While

        Dim bytes As Byte() = Encoding.UTF8.GetBytes(text.ToString())
        Dim result As Byte() = New Byte(size - 1) {}

        Call Array.Copy(bytes, result, System.Math.Min(size, bytes.Length))

        Return result
    End Function

    ''' <summary>
    ''' 把一次测量的结果写入调试审计报告。
    ''' </summary>
    Private Sub Report(name As String, allocated As Long, gen0 As Integer)
        Call DebugAudit.Mark(name & ":allocated", allocated)
        Call DebugAudit.Mark(name & ":gen0", gen0)

        Call Console.WriteLine($"{name,-46} allocated={allocated,12} B  gen0={gen0}")
    End Sub

End Module
