Namespace Search

    ''' <summary>
    ''' The configuration of a topology search run: the scope of the topology
    ''' space, the sampling budget, the step protocol, the functional
    ''' thresholds, the simulation settings and the execution strategy.
    ''' (一次拓扑搜索运行的配置：拓扑空间范围、采样预算、阶跃协议、功能阈值、仿真设置与执行策略)
    ''' </summary>
    ''' <remarks>
    ''' The defaults follow the paper: the complete connected space of the three
    ''' node network (at most 9 links), 10,000 sampled parameter sets per
    ''' topology, the 20% input step from 0.5 to 0.6, the functional region
    ''' S > 1 and P > 10 and the over representation threshold Q > 10. A full
    ''' scale run at the default sample count needs on the order of 1.6 × 10^8
    ''' circuit simulations, so the sample count and the topology limit are the
    ''' knobs for a validation run on a single machine.
    ''' </remarks>
    Public Class TopologySearchOptions

        ''' <summary>
        ''' The maximum number of active links of the searched topologies: 9 for
        ''' the complete space and 3 for the minimal networks of the forward
        ''' search strategy of the paper.
        ''' (被搜索拓扑的最大链接数：9 为完整空间，3 为论文正向策略的最小网络)
        ''' </summary>
        Public Property MaxLinks As Integer = 9

        ''' <summary>
        ''' The maximum number of topologies to search, 0 for no limit. The
        ''' topologies are taken in the identifier order, so a limited run is
        ''' deterministic and reproducible.
        ''' (被搜索拓扑的数量上限，0 为不限；按编号顺序取，保证有限运行确定性可复现)
        ''' </summary>
        Public Property MaxTopologies As Integer = 0

        ''' <summary>The number of sampled parameter sets per topology, 10,000 by the paper.</summary>
        Public Property Samples As Integer = 10000

        ''' <summary>The base seed of the search run, every topology derives its own sampler seed from it.</summary>
        Public Property Seed As Integer = 42

        ''' <summary>The initial input level of the step protocol, 0.5 by the paper.</summary>
        Public Property I1 As Double = 0.5

        ''' <summary>The stepped input level of the step protocol, 0.6 by the paper.</summary>
        Public Property I2 As Double = 0.6

        ''' <summary>The sensitivity threshold of the functional region, 1 by the paper.</summary>
        Public Property SThreshold As Double = 1.0

        ''' <summary>The precision threshold of the functional region, 10 by the paper.</summary>
        Public Property PThreshold As Double = 10.0

        ''' <summary>The Q threshold of the over representation, 10 by the paper.</summary>
        Public Property QThreshold As Integer = 10

        ''' <summary>The simulation options of the circuit simulator.</summary>
        Public Property SimulationOptions As New Simulation.CircuitSimulator.SimulationOptions()

        ''' <summary>
        ''' The maximum number of parallel threads, 0 for the processor count of
        ''' the machine.
        ''' (最大并行线程数，0 为机器的处理器数量)
        ''' </summary>
        Public Property MaxDegreeOfParallelism As Integer = 0

        ''' <summary>
        ''' The checkpoint directory, an empty string disables the checkpointing.
        ''' (断点目录，空字符串关闭断点功能)
        ''' </summary>
        Public Property CheckpointDir As String = ""

        ''' <summary>
        ''' Whether an existing checkpoint of a topology is reused instead of
        ''' evaluating the topology again.
        ''' (是否复用已存在的拓扑断点而非重新评估)
        ''' </summary>
        Public Property ResumeFromCheckpoints As Boolean = True

        ''' <summary>
        ''' Whether the circuit records of every well behaved circuit are kept
        ''' for the reports, instead of only the functional circuits. Keeping
        ''' every circuit increases the memory footprint of a large run.
        ''' (是否保留全部良态电路的记录供报告使用，而非只保留功能电路)
        ''' </summary>
        Public Property KeepAllCircuits As Boolean = False

        ''' <summary>
        ''' The Hamming distance cutoff of the topology clustering, 3 by the
        ''' paper. A negative value disables the clustering.
        ''' (拓扑聚类的 Hamming 距离截断值，论文取 3；负值关闭聚类)
        ''' </summary>
        Public Property ClusterCutoff As Double = 3.0

        ''' <summary>
        ''' Whether the design table experiment of the motif augmentations is
        ''' run after the search.
        ''' (是否在搜索后运行基序增强的设计表实验)
        ''' </summary>
        Public Property RunDesignTable As Boolean = True

        ''' <summary>
        ''' The sample count of the design table experiment, 0 for the sample
        ''' count of the search itself.
        ''' (设计表实验的采样数，0 为使用搜索本身的采样数)
        ''' </summary>
        Public Property DesignTableSamples As Integer = 0

        ''' <summary>
        ''' Whether the diagnostics that the ODE solver writes to the standard
        ''' error stream are suppressed during the parallel evaluation. The
        ''' solver reports its internal Newton failures there, which are handled
        ''' by the step rejection of the solver and would flood the stream of a
        ''' large search run.
        ''' (是否在并行评估期间抑制 ODE 求解器写入标准错误流的诊断信息)
        ''' </summary>
        Public Property SuppressSolverDiagnostics As Boolean = True

        ''' <summary>
        ''' The effective number of parallel threads of the run.
        ''' (运行的实际并行线程数)
        ''' </summary>
        Public Function EffectiveParallelism() As Integer
            If MaxDegreeOfParallelism > 0 Then
                Return MaxDegreeOfParallelism
            Else
                Return Environment.ProcessorCount
            End If
        End Function

        ''' <summary>
        ''' The sample count of the design table experiment.
        ''' (设计表实验实际使用的采样数)
        ''' </summary>
        Public Function EffectiveDesignTableSamples() As Integer
            If DesignTableSamples > 0 Then
                Return DesignTableSamples
            Else
                Return Samples
            End If
        End Function

    End Class

End Namespace
