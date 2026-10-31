''' <summary>
''' 一次定点迭代求解的结果
''' </summary>
Public Class SolveResult

    ''' <summary>
    ''' 稳态表达矩阵：<c>steadyState(gene)(cell)</c>
    ''' </summary>
    Public Property SteadyState As Double()() = New Double()() {}

    ''' <summary>实际执行的迭代轮数</summary>
    Public Property Iterations As Integer

    ''' <summary>是否所有细胞都达到了收敛阈值</summary>
    Public Property Converged As Boolean

    ''' <summary>达到收敛阈值的细胞数量</summary>
    Public Property ConvergedCells As Integer

    ''' <summary>最后一轮的最大相对变化量</summary>
    Public Property MaxChange As Double

    ''' <summary>每一轮的最大相对变化量（用于诊断震荡）</summary>
    Public Property History As Double() = New Double() {}

    ''' <summary>参与迭代的细胞（初始状态）数量</summary>
    Public Property PopulationSize As Integer

    ''' <summary>基因数量</summary>
    Public ReadOnly Property GeneCount As Integer
        Get
            Return SteadyState.Length
        End Get
    End Property

    ''' <summary>
    ''' 稳态下每个基因在细胞种群上的均值
    ''' </summary>
    ''' <returns></returns>
    Public Function GetMeans() As Double()
        Dim result As Double() = New Double(SteadyState.Length - 1) {}

        For g As Integer = 0 To SteadyState.Length - 1
            Dim values As Double() = SteadyState(g)

            If values IsNot Nothing AndAlso values.Length > 0 Then
                Dim sum As Double = 0.0R

                For c As Integer = 0 To values.Length - 1
                    sum += values(c)
                Next

                result(g) = sum / values.Length
            End If
        Next

        Return result
    End Function

    ''' <summary>
    ''' 稳态下每个基因在细胞种群上的标准差（样本标准差）
    ''' </summary>
    ''' <returns></returns>
    Public Function GetSDs() As Double()
        Dim means As Double() = GetMeans()
        Dim result As Double() = New Double(SteadyState.Length - 1) {}

        For g As Integer = 0 To SteadyState.Length - 1
            Dim values As Double() = SteadyState(g)

            If values IsNot Nothing AndAlso values.Length > 1 Then
                Dim sse As Double = 0.0R

                For c As Integer = 0 To values.Length - 1
                    sse += (values(c) - means(g)) ^ 2
                Next

                result(g) = Math.Sqrt(sse / (values.Length - 1))
            End If
        Next

        Return result
    End Function

    Public Overrides Function ToString() As String
        Return $"{GeneCount} genes x {PopulationSize} cells, {Iterations} iterations, " &
               $"converged={Converged} ({ConvergedCells}/{PopulationSize} cells), max change={MaxChange.ToString("G4")}"
    End Function
End Class

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
        m_popts = New ParallelOptions()

        If m_config.MaxDegreeOfParallelism > 0 Then
            m_popts.MaxDegreeOfParallelism = m_config.MaxDegreeOfParallelism
        End If

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
    ''' <returns></returns>
    Public Function Solve(initial As Double()(), Optional pinned As IReadOnlyDictionary(Of Integer, Double) = Nothing) As SolveResult
        If initial Is Nothing OrElse initial.Length = 0 Then
            Throw New ArgumentNullException(NameOf(initial))
        End If

        Dim nGenes As Integer = m_models.Length

        If initial.Length <> nGenes Then
            Throw New ArgumentException(
                $"初始状态包含 {initial.Length} 个基因，但是模型只包含 {nGenes} 个基因")
        End If

        Dim population As Integer = If(initial(0) Is Nothing, 0, initial(0).Length)

        If population = 0 Then
            Throw New ArgumentException("初始状态的细胞维度为空，无法求解稳态")
        End If

        Dim cur As Double()() = clone(initial, population)
        Dim [next] As Double()() = allocate(nGenes, population)
        Dim history As New List(Of Double)
        Dim cellConverged As Boolean() = Nothing
        Dim converged As Boolean = False
        Dim maxChange As Double = Double.MaxValue
        Dim iteration As Integer = 0

        For iteration = 1 To m_config.MaxIterations
            Call iterate(cur, [next], pinned)

            ' ---- 收敛判定 ----
            cellConverged = New Boolean(population - 1) {}

            maxChange = 0.0R

            For g As Integer = 0 To nGenes - 1
                If isPinned(g, pinned) Then
                    Continue For
                End If

                Dim before As Double() = cur(g)
                Dim after As Double() = [next](g)

                For c As Integer = 0 To population - 1
                    Dim scale As Double = Math.Max(1.0R, Math.Abs(before(c)))
                    Dim change As Double = Math.Abs(after(c) - before(c)) / scale

                    If change > maxChange Then
                        maxChange = change
                    End If
                    If change >= m_config.ConvergenceTolerance Then
                        cellConverged(c) = True
                    End If
                Next
            Next

            ' cellConverged 复用为“本轮有变化”的标记，取反即得收敛的细胞
            Dim stillMoving As Integer = cellConverged.Count(Function(x) x)

            Call history.Add(maxChange)

            Dim tmp As Double()() = cur
            cur = [next]
            [next] = tmp

            If maxChange < m_config.ConvergenceTolerance Then
                converged = True
                Exit For
            End If

            If m_config.Verbose AndAlso iteration Mod 10 = 0 Then
                Console.WriteLine($"  [GeneTree] 迭代 {iteration}/{m_config.MaxIterations}，最大相对变化 {maxChange.ToString("G4")}，" &
                                  $"{population - stillMoving}/{population} 个细胞已收敛")
            End If
        Next

        Dim convergedCells As Integer = 0

        If cellConverged IsNot Nothing Then
            convergedCells = cellConverged.Count(Function(x) Not x)
        End If

        Return New SolveResult With {
            .SteadyState = cur,
            .Iterations = Math.Min(iteration, m_config.MaxIterations),
            .Converged = converged,
            .ConvergedCells = convergedCells,
            .MaxChange = maxChange,
            .History = history.ToArray,
            .PopulationSize = population
        }
    End Function

    ''' <summary>
    ''' 执行一轮 Jacobi 更新
    ''' </summary>
    Private Sub iterate(cur As Double()(), [next] As Double()(), pinned As IReadOnlyDictionary(Of Integer, Double))
        Dim population As Integer = cur(0).Length
        Dim clip As Double = m_config.ClippingFactor
        Dim alpha As Double = m_config.DampingAlpha

        Call Parallel.For(0, m_models.Length, m_popts,
            Function() New SolverBuffer(population, m_maxRegulators),
            Function(g As Integer, state As ParallelLoopState, buffer As SolverBuffer) As SolverBuffer
                Dim model As GeneModel = m_models(g)

                If model Is Nothing OrElse isPinned(g, pinned) Then
                    ' 源节点与被钳制的基因：保持上一轮的状态（钳制值会在循环之后统一写入）
                    Call copy(cur(g), [next](g), population)
                    Return buffer
                End If

                Dim regulators As Integer() = model.RegulatorIndex
                Dim k As Integer = If(regulators Is Nothing, 0, regulators.Length)

                If k = 0 Then
                    Call copy(cur(g), [next](g), population)
                    Return buffer
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

                For c As Integer = 0 To population - 1
                    values(c) = item.Value
                Next
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

