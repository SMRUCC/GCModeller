Imports System.Diagnostics
Imports System.Threading
Imports std = System.Math

''' <summary>
''' GeneTree 训练与求解的并行度决策。
''' 
''' 这里的核心结论（也是此前“128 核服务器只跑 8 线程”问题的根因）：
''' 
''' + 训练是**按基因并行**的（<c>Parallel.For</c> 覆盖 1.8 万个靶基因），单个基因的拟合必须**串行**；
''' + 如果每个 <c>GBM.fit</c> 内部再启动一层 <c>Parallel.For</c>（TGBoost 建树时按层/按列并行），
'''   就形成了嵌套并行：外层 N 个线程各自阻塞等待内层任务，而 .NET 线程池的线程注入是
'''   缓慢的（爬山算法，每秒只补充少量线程），于是大量 worker 卡在等待上，
'''   实测表现就是“CPU 用不满、只有个位数线程在跑”；
''' + 因此自动档（<c>ThreadsPerModel = -1</c>）在**外层并行**时把单模型线程数解析为 1，
'''   只有在外层串行（<c>MaxDegreeOfParallelism = 1</c>）时才让单个模型吃满全部核心。
''' </summary>
Public Module GeneTreeParallelism

    ''' <summary>
    ''' 当前进程可用的逻辑处理器数量（已考虑进程亲和性与容器/cgroup 限制）
    ''' </summary>
    ''' <returns></returns>
    Public Function AvailableProcessors() As Integer
        Return std.Max(1, Environment.ProcessorCount)
    End Function

    ''' <summary>
    ''' 机器上的逻辑处理器总数（可能大于 <see cref="AvailableProcessors"/>）
    ''' </summary>
    ''' <remarks>
    ''' Windows 上超过 64 个逻辑处理器的机器会被划分成多个 processor group，
    ''' .NET 默认只使用当前 group（因此 <c>Environment.ProcessorCount</c> 可能只有 64）；
    ''' 如果是容器/cgroup 限制或者进程亲和性绑核，这里会返回被限制之后的可用数量。
    ''' </remarks>
    ''' <returns>无法探测时返回 <see cref="AvailableProcessors"/></returns>
    Public Function LogicalProcessors() As Integer
        Dim available As Integer = AvailableProcessors()

        Try
            ' 亲和性掩码（Windows）：进程被绑核时会小于环境值
            Dim mask As Long = CLng(Process.GetCurrentProcess().ProcessorAffinity.ToInt64())
            Dim affinity As Integer = popCount(mask)

            If affinity > 0 AndAlso affinity < available Then
                Return affinity
            End If
        Catch ex As Exception
            ' 非 Windows 平台或者权限不足时忽略
        End Try

        Return available
    End Function

    Private Function popCount(mask As Long) As Integer
        Dim count As Integer = 0

        While mask <> 0
            mask = mask And (mask - 1)
            count += 1
        End While

        Return count
    End Function

    ''' <summary>
    ''' 解析外层并行的实际并行度
    ''' </summary>
    ''' <param name="config">训练/求解配置</param>
    ''' <returns>
    ''' <c>MaxDegreeOfParallelism</c> 大于 0 时原样返回，否则返回
    ''' <see cref="AvailableProcessors"/>（即“自动”）
    ''' </returns>
    Public Function ResolveOuterDop(config As GeneTreeConfig) As Integer
        If config IsNot Nothing AndAlso config.MaxDegreeOfParallelism > 0 Then
            Return config.MaxDegreeOfParallelism
        End If

        Return AvailableProcessors()
    End Function

    ''' <summary>
    ''' 解析单个基因模型拟合时的线程数
    ''' </summary>
    ''' <param name="config">配置</param>
    ''' <param name="outerDop">外层并行度</param>
    ''' <returns>
    ''' + <c>ThreadsPerModel &gt; 0</c>：显式指定；
    ''' + <c>ThreadsPerModel = 0</c>：强制串行；
    ''' + <c>ThreadsPerModel &lt; 0</c>（自动）：外层并行时取 1（避免嵌套并行），外层串行时取全部核心
    ''' </returns>
    Public Function ResolveThreadsPerModel(config As GeneTreeConfig, outerDop As Integer) As Integer
        If config Is Nothing Then
            Return If(outerDop > 1, 1, AvailableProcessors())
        End If
        If config.ThreadsPerModel > 0 Then
            Return config.ThreadsPerModel
        End If
        If config.ThreadsPerModel = 0 Then
            Return 1
        End If

        ' 自动档：外层已经并行了，模型内部就不要再抢线程
        Return If(outerDop > 1, 1, AvailableProcessors())
    End Function

    ''' <summary>
    ''' 预热线程池：确保 <c>Parallel.For</c> 能立刻拿到足够的 worker 线程
    ''' </summary>
    ''' <param name="workers">期望的最小 worker 数量</param>
    ''' <remarks>
    ''' .NET 线程池的最小线程数默认等于 <c>Environment.ProcessorCount</c>，在“外层并行 + 每个任务耗时较长”
    ''' 的场景下会按爬山算法缓慢补充线程，训练刚启动时容易出现“只用了个位数线程”的观感。
    ''' 这里显式把最小线程数抬到目标并行度，之后所有 <c>Parallel.For</c> 都能立即满负荷。
    ''' </remarks>
    Public Sub EnsureThreadPool(workers As Integer)
        If workers <= 0 Then
            Return
        End If

        Try
            Dim minWorker As Integer
            Dim minIo As Integer

            ThreadPool.GetMinThreads(minWorker, minIo)

            If minWorker < workers Then
                Call ThreadPool.SetMinThreads(workers, std.Max(minIo, workers))
            End If
        Catch ex As Exception
            ' 设置失败不影响正确性，只是并行度会由线程池自行爬升
        End Try
    End Sub

    ''' <summary>
    ''' 一行可读的并行度诊断信息（训练开始时打印，便于确认是否真的用上了全部核心）
    ''' </summary>
    ''' <param name="outerDop">外层并行度</param>
    ''' <param name="threadsPerModel">单模型线程数</param>
    ''' <returns></returns>
    Public Function Describe(outerDop As Integer, threadsPerModel As Integer) As String
        Dim available As Integer = AvailableProcessors()
        Dim logical As Integer = LogicalProcessors()
        Dim minWorker As Integer
        Dim minIo As Integer

        ThreadPool.GetMinThreads(minWorker, minIo)

        Dim text As String = $"外层并行={outerDop}（Environment.ProcessorCount={available}，亲和性可用={logical}），" &
                             $"单模型线程={threadsPerModel}，线程池最小 worker={minWorker}"

        If logical < available Then
            text &= "；提示: 进程亲和性把可用核心限制到了 " & logical
        End If

        ' Windows 上每 64 个逻辑核会被划到一个 processor group，.NET 默认只使用当前组；
        ' 可用 DOTNET_Thread_UseAllCpuGroups=1 让线程池跨组调度。
        If available >= 64 AndAlso Environment.OSVersion.Platform = PlatformID.Win32NT Then
            text &= "；提示: 该机器可能使用了多个 processor group（每组 64 逻辑核），若 CPU 仍用不满可设置环境变量 DOTNET_Thread_UseAllCpuGroups=1"
        End If

        Return text
    End Function

End Module
