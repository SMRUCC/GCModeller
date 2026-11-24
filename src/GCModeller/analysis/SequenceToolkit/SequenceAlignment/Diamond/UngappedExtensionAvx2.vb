' 无空位延伸 —— 批量实现入口
'
' 历史实现试图用 AVX2 做"1 查询 × 32 参考"的批量延伸,但受限于 VB.NET
' (无法使用 Span/stackalloc/Unsafe 引用加载),实际是逐 lane 调用
' Vector256.WithElement / GetElement —— 这两个 API 在 JIT 后等价于
' 逐元素标量搬运,反向放大了内存访问量,并且每个批次都要重新装载 4 组向量。
'
' 现在的实现回到本质:批量延伸的真正瓶颈是每个残基的打分查表,而不是
' 32 路累加。因此批量接口改为"逐条调用扁平查表版延伸"(单次内存访问的打分),
' 去掉了 WithElement 时代的伪向量开销,同时保持对外的
' <see cref="ExtendBatch"/> / <see cref="BatchWindow"/> / <see cref="MaxBatch"/> 不变,
' 使上层(以及历史调用点)无需修改。

Imports System.Runtime.CompilerServices
Imports SMRUCC.genomics.Analysis.SequenceAlignment.BestLocalAlignment

Namespace DIAMOND

    ''' <summary>
    ''' 批量无空位延伸入口(内部为扁平查表的标量实现)。
    ''' </summary>
    Public Class UngappedExtensionAvx2 : Inherits UngappedExtension

        ''' <summary>批量窗口步数(氨基酸数)。</summary>
        Public Const BatchWindow As Integer = 48

        ''' <summary>单批次可并行处理的参考序列条数。</summary>
        Public Const MaxBatch As Integer = 32

        Sub New(Optional dropoff As Integer = -8, Optional blosum As Blosum = Nothing)
            MyBase.New(dropoff, blosum)
        End Sub

        ''' <summary>
        ''' 批量无空位延伸:对固定查询位置 <paramref name="qPos"/>,处理
        ''' <paramref name="subjects"/> 中对应 <paramref name="subjectPositions"/> 的若干参考序列。
        ''' 返回与输入顺序一一对应的 <see cref="UngappedHit"/> 数组。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ExtendBatch(query As String, qPos As Integer, subjects As String(), subjectPositions As Integer(), subjectIds As Integer()) As UngappedHit()
            Dim n = subjects.Length
            Dim out(n - 1) As UngappedHit

            For i As Integer = 0 To n - 1
                out(i) = MyBase.Extend(query, qPos, subjects(i), subjectPositions(i))
            Next

            Return out
        End Function
    End Class
End Namespace
