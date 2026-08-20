' 单机并行调度器 (PLINQ)
'
' 基于 PLINQ (.AsParallel) 对查询集合分块并行,每个查询独立完成单查询流水线。
' 参照本项目 <see cref="CDHit"/> 的 .AsParallel 并行风格。
'
' 线程安全约定(由 <see cref="DiamondBlastp"/> 保证):
'   - 参考蛋白库与按形状缓存的 ReferenceIndex 为只读共享;
'   - 每个查询线程独立构建自己的查询侧索引,互不干扰;
'   - ReferenceIndex 的懒加载写入已在 DiamondBlastp 内用 SyncLock 保护。
' 因此本调度器无需额外加锁即可安全并行。

Imports System.Linq
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DIAMOND

    ''' <summary>
    ''' 单机 PLINQ 并行调度器(默认多查询调度策略)。
    ''' </summary>
    Public Class ParallelScheduler : Implements IDiamondScheduler

        ''' <summary>并行度;为 0 表示由运行时自动选择(Environment.ProcessorCount)。</summary>
        Public ReadOnly DegreeOfParallelism As Integer

        Sub New(Optional degreeOfParallelism As Integer = 0)
            Me.DegreeOfParallelism = degreeOfParallelism
        End Sub

        Public Function Run(queries As FastaSeq(), subjectDb As IList(Of FastaSeq), perQuery As Func(Of FastaSeq, IEnumerable(Of DiamondHit))) As IEnumerable(Of DiamondHit) Implements IDiamondScheduler.Run
            Dim q = queries.AsParallel()

            If DegreeOfParallelism > 0 Then
                q = q.WithDegreeOfParallelism(DegreeOfParallelism)
            End If

            ' 保留顺序非必需,使用无序执行以获得更好吞吐;结果按查询顺序聚合由调用方决定
            Return q.SelectMany(Function(query) perQuery(query)).ToArray
        End Function
    End Class
End Namespace
