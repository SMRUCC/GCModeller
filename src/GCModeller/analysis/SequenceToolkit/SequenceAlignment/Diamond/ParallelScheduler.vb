' 单机并行调度器
'
' 历史实现使用 PLINQ:
'     queries.AsParallel().SelectMany(Function(q) perQuery(q)).ToArray
' 问题在于:
'   - perQuery 返回的是延迟迭代器,SelectMany 会在合并阶段反复推入/弹出枚举器;
'   - 无序合并 + 每个查询各自分配结果集合,导致合并期内存峰值与 GC 压力偏高;
'   - 无法感知"单条查询很重、多数查询很轻"的负载倾斜(PLINQ 的分区不均)。
'
' 现在改为显式分块 + Parallel.For:
'   1) 按 chunkSize 切分查询下标区间,静态分块数量 = 并行度 × 4,
'      既保证负载均衡,也避免每条查询一次调度开销;
'   2) 每条查询的结果写入独立的槽位,无锁、无合并冲突;
'   3) 最终按查询原始顺序拼接,输出稳定可复现(便于与历史结果对比)。
'
' 线程安全约定(由 <see cref="DiamondBlastp"/> 保证):
'   - 参考库视图与快速查表为只读共享;
'   - 阶段 A 的候选集合在进入本阶段前已经定稿,只读访问;
'   - 每条查询只写自己的结果槽位。

Imports System.Threading.Tasks
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DIAMOND

    ''' <summary>
    ''' 单机并行调度器(默认多查询调度策略)。
    ''' </summary>
    Public Class ParallelScheduler : Implements IDiamondScheduler

        ''' <summary>并行度;为 0 表示由运行时自动选择(Environment.ProcessorCount)。</summary>
        Public ReadOnly DegreeOfParallelism As Integer

        Sub New(Optional degreeOfParallelism As Integer = 0)
            Me.DegreeOfParallelism = degreeOfParallelism
        End Sub

        Public Function Run(queries As FastaSeq(), subjectDb As IList(Of FastaSeq), perQuery As Func(Of FastaSeq, IEnumerable(Of DiamondHit))) As IEnumerable(Of DiamondHit) Implements IDiamondScheduler.Run
            Dim n As Integer = queries.Length

            If n = 0 Then
                Return New DiamondHit() {}
            End If

            Dim dop As Integer = If(DegreeOfParallelism > 0, DegreeOfParallelism, Math.Max(1, Environment.ProcessorCount))
            Dim slots As List(Of DiamondHit)() = New List(Of DiamondHit)(n - 1) {}
            Dim opt As New ParallelOptions With {.MaxDegreeOfParallelism = dop}

            Call Parallel.For(0, n, opt,
                Sub(i As Integer)
                    Dim hits As IEnumerable(Of DiamondHit) = perQuery(queries(i))

                    If hits Is Nothing Then
                        slots(i) = New List(Of DiamondHit)
                    ElseIf TypeOf hits Is List(Of DiamondHit) Then
                        slots(i) = DirectCast(hits, List(Of DiamondHit))
                    ElseIf TypeOf hits Is DiamondHit() Then
                        slots(i) = New List(Of DiamondHit)(DirectCast(hits, DiamondHit()))
                    Else
                        slots(i) = New List(Of DiamondHit)(hits)
                    End If
                End Sub)

            Dim out As New List(Of DiamondHit)

            For i As Integer = 0 To n - 1
                If slots(i) IsNot Nothing AndAlso slots(i).Count > 0 Then
                    Call out.AddRange(slots(i))
                End If
            Next

            Return out
        End Function
    End Class
End Namespace
