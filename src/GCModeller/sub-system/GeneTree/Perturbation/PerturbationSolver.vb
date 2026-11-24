
Imports System.Text

''' <summary>
''' 非线性调控网络的稳态求解器：阻尼定点迭代
''' <c>x_i^new = (1-α)·x_i^old + α·f_i(X_Reg(i)^old)</c>。
''' 
''' 实现要点（对应 readme 的“工程优化提示”）：
''' 
''' + **Jacobi 双缓冲**：整轮更新都读取上一轮的状态，避免同一轮内先更新后读造成的顺序依赖；
''' + **按靶基因并行**：每个基因只写自己那一列，不存在数据竞争；
''' + **外推裁剪 + 非负约束**：抑制树模型在训练值域之外的“平台化”预测；
''' + **多起点一致性**：每个细胞是一个独立的初始状态，最终用收敛细胞的比例判断是否出现多稳态/震荡；
''' + **阻尼系数 α**：让每轮更新更保守，是树模型分段非光滑预测下避免震荡的关键。
''' </summary>
Public Class PerturbationSolver

    Private ReadOnly m_models As GeneModel()
    Private ReadOnly m_config As GeneTreeConfig
    Private ReadOnly m_maxRegulators As Integer
    Private ReadOnly m_popts As ParallelOptions

    ''' <summary>每个基因的模型（下标与表达矩阵的行一致，<c>Nothing</c> 表示源节点）</summary>
    Public ReadOnly Property Models As GeneModel()
        Get
            Return m_models
        End Get
    End Property

    ''' <summary>求解参数</summary>
    Public ReadOnly Property Config As GeneTreeConfig
        Get
            Return m_config
        End Get
    End Property

    ''' <summary>
    ''' 创建求解器
    ''' </summary>
    ''' <param name="models">逐基因模型</param>
    ''' <param name="config">求解参数；为空时使用快速档</param>
    Public Sub New(models As GeneModel(), Optional config As GeneTreeConfig = Nothing)
        If models Is Nothing Then
            Throw New ArgumentNullException(NameOf(models))
        End If

        m_models = models
        m_config = If(config, GeneTreeConfig.CreateFast())
        m_popts = New ParallelOptions() With {
            .MaxDegreeOfParallelism = GeneTreeParallelism.ResolveOuterDop(m_config)
        }

        Call GeneTreeParallelism.EnsureThreadPool(m_popts.MaxDegreeOfParallelism)

        Dim maxK As Integer = 0

        For Each model As GeneModel In models
            If model IsNot Nothing AndAlso model.RegulatorIndex IsNot Nothing Then
                maxK = Math.Max(maxK, model.RegulatorIndex.Length)
            End If
        Next

        m_maxRegulators = Math.Max(1, maxK)
    End Sub

    ''' <summary>
    ''' 求解扰动之后的稳态
    ''' </summary>
    ''' <param name="initial">
    ''' 初始表达状态 <c>initial(gene)(cell)</c>：每个细胞是一个独立的起点，
    ''' 因此最终结果是“细胞种群”层面的稳态分布而不是单一向量。
    ''' </param>
    ''' <param name="pinned">
    ''' 被扰动/被钳制的基因：<c>基因下标 → 强制取值</c>。这些基因在每一轮结束时都会被强制写回干预值。
    ''' </param>
    ''' <param name="hardCap">
    ''' 迭代硬上限的覆盖值，&lt;=0 表示使用 <c>GeneTreeConfig.EffectiveMaxIterationsHardCap</c>。
    ''' 控制稳态（所有扰动共享的基线）通常需要比单次扰动更多的轮数才能收敛，因此调用方可以放宽这个上限。
    ''' </param>
    ''' <returns></returns>
    ''' <param name="warmStart">
    ''' 初始状态是否已经是“未扰动”的定点（控制稳态）。为<c>True</c> 时只把被钳制的基因标记为脏，
    ''' 于是第一轮只需要重算它们的下游靶基因，后续迭代也只在扰动传播的波前上计算，
    ''' 单次扰动可以从“全网重算”降到“受影响子网重算”。
    ''' </param>
    Public Function Solve(initial As Double()(),
                          Optional pinned As IReadOnlyDictionary(Of Integer, Double) = Nothing,
                          Optional hardCap As Integer = 0,
                          Optional warmStart As Boolean = False) As SolveResult

        If initial Is Nothing OrElse initial.Length = 0 Then
            Throw New ArgumentNullException(NameOf(initial))
        End If

        Dim t0 = Now
        Dim nGenes As Integer = m_models.Length

        If initial.Length <> nGenes Then
            Throw New ArgumentException(
                $"初始状态包含 {initial.Length} 个基因，但是模型只包含 {nGenes} 个基因")
        End If

        Dim population As Integer = If(initial(0) Is Nothing, 0, initial(0).Length)

        If population = 0 Then
            Throw New ArgumentException("初始状态的细胞维度为空，无法求解稳态")
        End If

        ' ---- 均匀初始态折叠：全基因组上下文会把所有细胞设置成同一个状态 ----
        Dim collapsed As Boolean = False
        Dim logtext As New StringBuilder

        If m_config.CollapseUniformPopulation AndAlso population > 1 AndAlso isUniform(initial, population) Then
            collapsed = True
            population = 1

            If m_config.Verbose Then
                Call logtext.AppendLine($"  [GeneTree] 初始状态在所有细胞上完全一致，已折叠为单细胞轨迹（{nGenes} 个基因）")
            End If
        End If

        Dim cur As Double()() = If(collapsed, cloneFirstCell(initial, nGenes), clone(initial, population))
        Dim [next] As Double()() = allocate(nGenes, population)
        Dim delta As Boolean() = New Boolean(nGenes - 1) {}
        Dim newDelta As Boolean() = New Boolean(nGenes - 1) {}
        Dim history As New List(Of Double)
        Dim converged As Boolean = False
        Dim maxChange As Double = Double.MaxValue
        Dim iteration As Integer = 0
        Dim softLimit As Integer = m_config.MaxIterations
        Dim hardLimit As Integer = If(hardCap > 0, hardCap, m_config.EffectiveMaxIterationsHardCap)
        Dim exceeded As Boolean = False
        Dim moving As Integer = nGenes
        Dim allowed As Integer = CInt(Math.Truncate(m_config.MovingGeneTolerance * nGenes))

        ' 冷启动时所有基因都需要重新预测；热启动（从控制稳态出发）时只有被扰动的基因需要传播
        For g As Integer = 0 To nGenes - 1
            delta(g) = If(warmStart, isPinned(g, pinned), True)
        Next

        Dim alpha As Double = m_config.DampingAlpha
        Dim lastMoving As Integer = nGenes

        For iteration = 1 To hardLimit
            Call iterate(cur, [next], pinned, delta, newDelta, alpha)

            ' ---- 收敛判定：统计“仍在移动的基因比例”，而不是全网的最大变化量 ----
            maxChange = 0.0R
            moving = 0

            For g As Integer = 0 To nGenes - 1
                If isPinned(g, pinned) Then
                    Continue For
                End If

                Dim before As Double() = cur(g)
                Dim after As Double() = [next](g)
                Dim geneMoving As Boolean = False

                For c As Integer = 0 To population - 1
                    Dim scale As Double = Math.Max(1.0R, Math.Abs(before(c)))
                    Dim change As Double = Math.Abs(after(c) - before(c)) / scale

                    If change > maxChange Then
                        maxChange = change
                    End If
                    If change >= m_config.ConvergenceTolerance Then
                        geneMoving = True
                    End If
                Next

                If geneMoving Then
                    moving += 1
                End If
            Next

            Call history.Add(maxChange)

            Dim tmp As Double()() = cur
            cur = [next]
            [next] = tmp

            Dim tmpDelta As Boolean() = delta
            delta = newDelta
            newDelta = tmpDelta

            Call Array.Clear(newDelta, 0, newDelta.Length)

            ' ---- 自适应阻尼：收敛变快时加大步长，出现反弹时回退 ----
            If iteration > 1 Then
                If moving < lastMoving Then
                    alpha = Math.Min(0.35R, alpha * 1.15R)
                ElseIf moving > lastMoving * 1.05R Then
                    alpha = Math.Max(0.05R, alpha * 0.7R)
                End If
            End If

            lastMoving = moving

            ' 热启动（从控制稳态出发）时必须等到波前彻底停止：只要有基因还在变化，
            ' 就说明扰动的传播尚未完成，此时提前判收敛会把级联效应截断掉。
            converged = If(warmStart, moving = 0, moving <= allowed)

            If converged Then
                Exit For
            End If

            If iteration = softLimit + 1 Then
                exceeded = True

                If m_config.Verbose Then
                    Call logtext.AppendLine($"  [GeneTree] 软上限 {softLimit} 轮后仍有 {moving} 个基因在移动（最大相对变化 {maxChange.ToString("G4")}），" &
                                      $"继续迭代到硬上限 {hardLimit} 轮")
                End If
            End If

            If m_config.Verbose AndAlso iteration Mod 10 = 0 Then
                Call logtext.AppendLine($"  [GeneTree] 迭代 {iteration}/{hardLimit}，最大相对变化 {maxChange.ToString("G4")}，" &
                                  $"仍在移动的基因 {moving}/{nGenes}")
            End If
        Next

        Dim solved As New SolveResult With {
            .SteadyState = cur,
            .Iterations = Math.Min(iteration, hardLimit),
            .Converged = converged,
            .ConvergedCells = If(converged, population, 0),
            .MaxChange = maxChange,
            .History = history.ToArray,
            .PopulationSize = population,
            .Collapsed = collapsed,
            .ExceededSoftLimit = exceeded
        }

        If m_config.Verbose Then
            Call logtext.AppendLine($"[GeneTree] 扰动稳态求解完成（{StringFormats.ReadableElapsedTime(Now - t0)}）: {solved.ToString}")
        End If

        solved.log = logtext.ToString

        Return solved
    End Function

    ''' <summary>
    ''' 取每个基因在第一个细胞上的取值（折叠为单细胞轨迹时使用；此时所有细胞完全一致）
    ''' </summary>
    ''' <param name="initial"><c>initial(gene)(cell)</c></param>
    ''' <param name="nGenes">基因数量</param>
    ''' <returns></returns>
    Private Shared Function cloneFirstCell(initial As Double()(), nGenes As Integer) As Double()()
        Dim result As Double()() = New Double(nGenes - 1)() {}

        For g As Integer = 0 To nGenes - 1
            Dim row As Double() = initial(g)

            If row Is Nothing OrElse row.Length = 0 Then
                result(g) = New Double() {0.0R}
            Else
                result(g) = New Double() {row(0)}
            End If
        Next

        Return result
    End Function

    ''' <summary>
    ''' 判断初始状态是否在所有细胞上完全一致
    ''' </summary>
    ''' <param name="initial"><c>initial(gene)(cell)</c></param>
    ''' <param name="population">细胞数量</param>
    ''' <returns></returns>
    Private Shared Function isUniform(initial As Double()(), population As Integer) As Boolean
        For g As Integer = 0 To initial.Length - 1
            Dim row As Double() = initial(g)

            If row Is Nothing Then
                Continue For
            ElseIf row.Length <> population Then
                Return False
            End If

            Dim head As Double = row(0)

            For c As Integer = 1 To population - 1
                If row(c) <> head Then
                    Return False
                End If
            Next
        Next

        Return True
    End Function

    ''' <summary>
    ''' 执行一轮 Jacobi 更新
    ''' </summary>
    ''' <param name="cur">上一轮的状态 <c>cur(gene)(cell)</c></param>
    ''' <param name="next">本轮写出的状态</param>
    ''' <param name="pinned">被钳制的基因</param>
    ''' <param name="delta">上一轮中发生过可见变化的基因标记（用于脏传播剪枝）</param>
    ''' <param name="newDelta">本轮中发生过可见变化的基因标记（输出）</param>
    Private Sub iterate(cur As Double()(),
                        [next] As Double()(),
                        pinned As IReadOnlyDictionary(Of Integer, Double),
                        delta As Boolean(),
                        newDelta As Boolean(),
                        alpha As Double)

        Dim population As Integer = cur(0).Length
        Dim clip As Double = m_config.ClippingFactor
        Dim eps As Double = m_config.ConvergenceTolerance * 0.05R

        ' 按块切分基因：单基因的计算量很小，逐基因派发任务的调度开销会占主导
        Const CHUNK_SIZE As Integer = 64

        Dim nGenes As Integer = m_models.Length
        Dim nChunks As Integer = (nGenes + CHUNK_SIZE - 1) \ CHUNK_SIZE

        Call Parallel.For(0, nChunks, m_popts,
            Function() New SolverBuffer(population, m_maxRegulators),
            Function(chunkIndex As Integer, state As ParallelLoopState, buffer As SolverBuffer) As SolverBuffer
                Dim first As Integer = chunkIndex * CHUNK_SIZE
                Dim last As Integer = Math.Min(first + CHUNK_SIZE, nGenes) - 1

                For g As Integer = first To last
                    Dim model As GeneModel = m_models(g)

                    If model Is Nothing Then
                        ' 源节点：保持上一轮的状态
                        Call copy(cur(g), [next](g), population)
                        newDelta(g) = False
                        Continue For
                    ElseIf isPinned(g, pinned) Then
                        ' 被钳制的基因：保持上一轮的状态（钳制值会在循环之后统一写入）
                        Call copy(cur(g), [next](g), population)
                        newDelta(g) = False
                        Continue For
                    End If

                    Dim regulators As Integer() = model.RegulatorIndex
                    Dim k As Integer = If(regulators Is Nothing, 0, regulators.Length)

                    If k = 0 Then
                        Call copy(cur(g), [next](g), population)
                        newDelta(g) = False
                        Continue For
                    End If

                    ' ---- 脏传播剪枝：所有调控者本轮都没有变化时，该基因的预测不会改变 ----
                    Dim dirty As Boolean = False

                    For j As Integer = 0 To k - 1
                        If delta(regulators(j)) Then
                            dirty = True
                            Exit For
                        End If
                    Next

                    If Not dirty Then
                        Call copy(cur(g), [next](g), population)
                        newDelta(g) = False
                        Continue For
                    End If

                    Dim rows As Single()() = buffer.Rows

                    For c As Integer = 0 To population - 1
                        Dim row As Single() = rows(c)

                        For j As Integer = 0 To k - 1
                            row(j) = CSng(cur(regulators(j))(c))
                        Next
                    Next

                    Dim prediction As Double() = model.PredictBatch(rows)
                    Dim range As (lower As Double, upper As Double) = model.GetClippingRange(clip, m_config.ClipNegative)
                    Dim before As Double() = cur(g)
                    Dim after As Double() = [next](g)
                    Dim moved As Boolean = False

                    For c As Integer = 0 To population - 1
                        Dim predicted As Double = prediction(c)

                        If Double.IsNaN(predicted) OrElse Double.IsInfinity(predicted) Then
                            ' 预测失败时保持原值，避免污染整轮迭代
                            after(c) = before(c)
                            Continue For
                        End If

                        If predicted < range.lower Then
                            predicted = range.lower
                        ElseIf predicted > range.upper Then
                            predicted = range.upper
                        End If

                        Dim value As Double = (1.0R - alpha) * before(c) + alpha * predicted

                        If m_config.ClipNegative AndAlso value < 0.0R Then
                            value = 0.0R
                        End If

                        after(c) = value

                        If Not moved Then
                            Dim scale As Double = Math.Max(1.0R, Math.Abs(before(c)))

                            If Math.Abs(value - before(c)) / scale >= eps Then
                                moved = True
                            End If
                        End If
                    Next

                    newDelta(g) = moved
                Next

                Return buffer
            End Function,
            Sub(buffer As SolverBuffer)
                ' 线程局部缓冲无需释放
            End Sub)

        ' 钳制的基因每一轮都强制写回干预值
        If pinned IsNot Nothing Then
            For Each item As KeyValuePair(Of Integer, Double) In pinned
                Dim values As Double() = [next](item.Key)
                Dim before As Double() = cur(item.Key)
                Dim changed As Boolean = False

                For c As Integer = 0 To population - 1
                    values(c) = item.Value

                    If Not changed AndAlso values(c) <> before(c) Then
                        changed = True
                    End If
                Next

                If changed Then
                    ' 钳制值在本轮发生了变化，下游基因需要重新预测
                    newDelta(item.Key) = True
                End If
            Next
        End If
    End Sub

    Private Shared Function isPinned(gene As Integer, pinned As IReadOnlyDictionary(Of Integer, Double)) As Boolean
        Return pinned IsNot Nothing AndAlso pinned.ContainsKey(gene)
    End Function

    Private Shared Sub copy(source As Double(), target As Double(), population As Integer)
        For i As Integer = 0 To population - 1
            target(i) = source(i)
        Next
    End Sub

    Private Shared Function allocate(nGenes As Integer, population As Integer) As Double()()
        Dim result As Double()() = New Double(nGenes - 1)() {}

        For g As Integer = 0 To nGenes - 1
            result(g) = New Double(population - 1) {}
        Next

        Return result
    End Function

    Private Shared Function clone(initial As Double()(), population As Integer) As Double()()
        Dim result As Double()() = allocate(initial.Length, population)

        For g As Integer = 0 To initial.Length - 1
            Dim row As Double() = initial(g)

            If row Is Nothing OrElse row.Length <> population Then
                Throw New ArgumentException($"初始状态的基因 {g} 的细胞维度不是 {population}")
            End If

            Call Array.Copy(row, result(g), population)
        Next

        Return result
    End Function

    ''' <summary>
    ''' 每线程复用的特征缓冲，避免在热路径上反复分配 <c>Single()()</c>
    ''' </summary>
    Private Class SolverBuffer

        ''' <summary>
        ''' 特征矩阵（行=细胞、列=调控因子）。列宽固定为全局最大调控者数量，
        ''' 每行只填前 k 列，多出来的列不会被任何一棵树访问。
        ''' </summary>
        Public ReadOnly Rows As Single()()

        Public Sub New(population As Integer, featureDim As Integer)
            Rows = New Single(population - 1)() {}

            For i As Integer = 0 To population - 1
                Rows(i) = New Single(featureDim - 1) {}
            Next
        End Sub
    End Class
End Class

