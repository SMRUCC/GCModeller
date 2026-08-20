' DIAMOND 多查询调度接口 (Scheduler Boundary)
'
' 将"多查询集合如何被分发执行"抽象为接口边界,使 <see cref="DiamondBlastp"/> 的
' 多查询重载不依赖具体并行/分布式策略。默认实现为 <see cref="ParallelScheduler"/>
' (单机 PLINQ 并行);<see cref="DistributedScheduler"/> 提供跨节点分发的骨架接口,
' 便于后续接入真实分布式计算框架(如通过消息队列 / 远程过程调用),而不修改
' 比对算法本身。

Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace DIAMOND

    ''' <summary>
    ''' 多查询调度边界。
    ''' </summary>
    ''' <param name="queries">查询序列数组。</param>
    ''' <param name="subjectDb">参考蛋白库(只读共享)。</param>
    ''' <param name="perQuery">对单条查询执行 DIAMOND 流水线并返回命中的函数(由 DiamondBlastp 提供)。</param>
    ''' <returns>所有查询命中的聚合结果。</returns>
    Public Interface IDiamondScheduler
        Function Run(queries As FastaSeq(), subjectDb As IList(Of FastaSeq), perQuery As Func(Of FastaSeq, IEnumerable(Of DiamondHit))) As IEnumerable(Of DiamondHit)
    End Interface
End Namespace
