Imports System.Text
Imports System.Text.Json
Imports System.Text.Json.Serialization
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.FuzzyGene.Data
Imports SMRUCC.genomics.Analysis.FuzzyGene.Fitting
Imports std = System.Math

    ''' <summary>
    ''' 由逐靶基因神经模糊模型拼成的基因调控网络（纯推理对象）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 每一行是一个基因，<see cref="Models"/> 的第 <c>g</c> 项是该基因作为靶基因时训练出的 TSK 模型
    ''' （没有可用调控因子的基因为 <c>Nothing</c>，其状态在传播中保持不变）。
    ''' </para>
    ''' <para>
    ''' 静态与滞后两条路线的对外语义完全一致：都是「下一状态 = F(当前状态)」。
    ''' 静态路线下 <c>F</c> 学的是 <c>x_g = f(regulators)</c>，迭代就是求不动点；
    ''' 滞后路线下 <c>F</c> 学的是 <c>x_g(t+1) = f(regulators(t))</c>，迭代就是时间推进。
    ''' 因此稳态求解、钳制扰动与多步级联共用同一套阻尼迭代器。
    ''' </para>
    ''' <para>
    ''' 内部一律在 z 空间迭代（与训练一致），对外接口收发原始表达量。
    ''' </para>
    ''' </remarks>
    Public Class FuzzyGeneNetwork

        Private ReadOnly _geneIndex As Dictionary(Of String, Integer)
        Private ReadOnly _regulatorIndices As Integer()()

        ''' <summary>
        ''' 逐步迭代时复用的输入行缓冲（线程局部，只增不减）。
        ''' </summary>
        ''' <remarks>
        ''' <see cref="StepZ"/> 每步要为每个基因凑一行调控因子取值；原来每次都新建一个数组，
        ''' 几万基因 × 上百步迭代下来是 GB 级的小对象垃圾。缓冲按"最大调控因子个数"增长，
        ''' 逐基因多出来的尾部不被读取。
        ''' </remarks>
        Private ReadOnly _inputScratch As New System.Threading.ThreadLocal(Of Double())(Function() New Double(-1) {})
        Private ReadOnly _config As FuzzyGeneConfig

        ''' <summary>参与建模的基因（顺序即 <see cref="Models"/> 的索引顺序）。</summary>
        Public Property GeneNames As String()

        ''' <summary>逐基因模型，未建模的位置为 <c>Nothing</c>。</summary>
        Public Property Models As GeneRegulationModel()

        ''' <summary>带符号调控边（由各靶基因模型的解析灵敏度汇总）。</summary>
        Public Property Edges As RegulationEdgeModel()

        ''' <summary>建模配置（决定传播步数、阻尼、限幅等默认行为）。</summary>
        Public ReadOnly Property Config As FuzzyGeneConfig
            Get
                Return _config
            End Get
        End Property

        ''' <summary>实际采用的拟合路线。</summary>
        Public Property Mode As FuzzyTargetMode

        ''' <summary>逐基因原始表达均值。</summary>
        Public Property Means As Double()

        ''' <summary>逐基因原始表达标准差。</summary>
        Public Property SDs As Double()

        ''' <summary>稳态迭代的起始状态（原始尺度）。</summary>
        Public Property BaselineRaw As Double()

        ''' <summary>逐基因训练支持域下界（z 空间）。</summary>
        Public Property SupportMin As Double()

        ''' <summary>逐基因训练支持域上界（z 空间）。</summary>
        Public Property SupportMax As Double()

        ''' <summary>基因数量。</summary>
        Public ReadOnly Property NGene As Integer
            Get
                Return GeneNames.Length
            End Get
        End Property

        ''' <summary>参与建模（有模型）的基因数量。</summary>
        Public ReadOnly Property NModelled As Integer
            Get
                Return Models.Count(Function(m) m IsNot Nothing)
            End Get
        End Property

        ''' <summary>是否为滞后（时序）路线。</summary>
        Public ReadOnly Property IsLagged As Boolean
            Get
                Return Mode = FuzzyTargetMode.LaggedRoute
            End Get
        End Property

        ''' <summary>
        ''' 模型是否在 z 空间训练。
        ''' </summary>
        ''' <remarks>
        ''' 为 <c>False</c>（配置里 <c>Standardize = False</c>）时内部坐标就是原始表达量，
        ''' 此时 <see cref="ToZ"/> / <see cref="ToRaw"/> 不做任何换算——
        ''' 否则「训练用原始值、推理用 z 值」会造成整体平移的隐性错误。
        ''' </remarks>
        Public ReadOnly Property Standardized As Boolean
            Get
                Return _standardized
            End Get
        End Property

        Private ReadOnly _standardized As Boolean

        ''' <summary>按结构创建网络（一般由 <see cref="Fitting.GeneNetworkFitter"/> 调用）。</summary>
        Public Sub New(geneNames As String(),
                       models As GeneRegulationModel(),
                       edges As RegulationEdgeModel(),
                       config As FuzzyGeneConfig,
                       means As Double(),
                       sds As Double(),
                       baseline As Double(),
                       supportMin As Double(),
                       supportMax As Double(),
                       mode As FuzzyTargetMode,
                       Optional standardized As Boolean = True)

            If geneNames Is Nothing OrElse geneNames.Length = 0 Then
                Throw New ArgumentException("the gene list is empty", NameOf(geneNames))
            End If
            If models Is Nothing OrElse models.Length <> geneNames.Length Then
                Throw New ArgumentException("the model list does not match the gene list", NameOf(models))
            End If

            Me.GeneNames = geneNames
            Me.Models = models
            Me.Edges = If(edges, New RegulationEdgeModel() {})
            Me.Mode = mode
            Me.Means = means
            Me.SDs = sds
            Me.BaselineRaw = baseline
            Me.SupportMin = supportMin
            Me.SupportMax = supportMax
            _config = If(config, New FuzzyGeneConfig())
            _standardized = standardized

            _geneIndex = BuildIndex(geneNames)
            _regulatorIndices = BuildRegulatorIndices(Me.Models, _geneIndex)
        End Sub

        Private Shared Function BuildIndex(geneNames As String()) As Dictionary(Of String, Integer)
            Dim index As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)

            For i As Integer = 0 To geneNames.Length - 1
                index(geneNames(i)) = i
            Next

            Return index
        End Function

        Private Shared Function BuildRegulatorIndices(models As GeneRegulationModel(),
                                                     index As Dictionary(Of String, Integer)) As Integer()()
            Dim result(models.Length - 1)() As Integer

            For g As Integer = 0 To models.Length - 1
                Dim model As GeneRegulationModel = models(g)

                If model Is Nothing OrElse model.Regulators Is Nothing Then
                    result(g) = New Integer() {}
                    Continue For
                End If

                Dim rows As New List(Of Integer)

                For Each gene As String In model.Regulators
                    Dim idx As Integer

                    If index.TryGetValue(gene, idx) Then
                        Call rows.Add(idx)
                    End If
                Next

                ' 输入个数必须与训练时一致，否则模型无法推理（例如包文件被手工改写）
                If model.Network IsNot Nothing AndAlso rows.Count <> model.Network.InputCount Then
                    result(g) = New Integer() {}
                Else
                    result(g) = rows.ToArray()
                End If
            Next

            Return result
        End Function

        ''' <summary>按基因名查下标（大小写不敏感），缺失时返回 -1。</summary>
        Public Function IndexOfGene(gene As String) As Integer
            Dim idx As Integer

            If Not String.IsNullOrEmpty(gene) AndAlso _geneIndex.TryGetValue(gene, idx) Then
                Return idx
            End If

            Return -1
        End Function

#Region "并行调度"

        ''' <summary>
        ''' 触发基因维并行的最小基因数。
        ''' </summary>
        ''' <remarks>
        ''' 单基因前向约 0.2~0.8 微秒，2048 个基因约 0.5~1.5 毫秒，远大于
        ''' <see cref="System.Threading.Tasks.Parallel.For(int,int,Action(Of Integer))"/>
        ''' 每次 5~20 微秒的调度开销；更小的网络直接走串行分支，
        ''' 避免「为几百个基因起线程池」造成的负收益。
        ''' </remarks>
        Private Const ParallelMinGenes As Integer = 2048

        ''' <summary>并行分块的最小基因数：用于摊薄每个分块的任务开销。</summary>
        Private Const MinChunkGenes As Integer = 128

        ''' <summary>
        ''' 脏位阈值：单步变化量低于它即认为该基因没有发生变化。
        ''' </summary>
        ''' <remarks>
        ''' 与 <c>TskLayers.MinimumActivationSum</c> 一样以具名常量表达，
        ''' 避免同一个魔法数字在稳态循环里出现两次。
        ''' </remarks>
        Private Const DirtyEpsilon As Double = 0.000000000001

        ''' <summary>
        ''' 本次迭代实际使用的并行度（<c>1</c> 表示串行）。
        ''' </summary>
        ''' <remarks>
        ''' 复用训练阶段的 <see cref="FuzzyGeneConfig.DegreeOfParallelism"/>：
        ''' <c>0</c> = 使用全部逻辑核心，<c>1</c> = 强制串行（单元测试与 A/B 基准都依赖这一点）。
        ''' </remarks>
        Private ReadOnly Property Workers As Integer
            Get
                Dim degree As Integer = _config.DegreeOfParallelism

                Return If(degree > 0, degree, System.Environment.ProcessorCount)
            End Get
        End Property

        ''' <summary>给定基因数是否值得启用并行。</summary>
        Private Shared Function ShouldParallelize(count As Integer, workers As Integer) As Boolean
            Return workers > 1 AndAlso count >= ParallelMinGenes
        End Function

        ''' <summary>
        ''' 按基因数与并行度计算分块大小。
        ''' </summary>
        ''' <remarks>
        ''' 目标是「每核约 4 个分块」以实现动态负载均衡（不同基因的规则数不同，
        ''' 静态均分很容易卡在最慢的那一块）；同时每块不小于 <see cref="MinChunkGenes"/>，
        ''' 且不小于「每核至少一块」，避免分块碎到调度开销盖过计算。
        ''' </remarks>
        Private Shared Function ChunkGenes(count As Integer, workers As Integer) As Integer
            Dim target As Integer = (count + workers * 4 - 1) \ std.Max(1, workers * 4)
            Dim chunk As Integer = std.Max(MinChunkGenes, target)
            Dim upper As Integer = std.Max(1, count \ std.Max(1, workers))

            If chunk > upper Then
                chunk = upper
            End If

            Return std.Max(1, chunk)
        End Function

        ''' <summary>构造本次迭代的并行选项（<see cref="System.Threading.Tasks.Parallel"/> 必须全限定）。</summary>
        Private Shared Function ParallelOptionsOf(workers As Integer) As System.Threading.Tasks.ParallelOptions
            Return New System.Threading.Tasks.ParallelOptions With {.MaxDegreeOfParallelism = workers}
        End Function

#End Region

#Region "推理"

        ''' <summary>单步预测（原始表达量进、原始表达量出）。</summary>
        Public Function Predict(state As Double()) As Double()
            Dim z As Double() = ToZ(state)
            Dim dirty As Boolean() = Repeat(True, NGene)

            Return ToRaw(StepZ(z, dirty, True))
        End Function

        ''' <summary>
        ''' 在给定钳制条件下迭代到稳态（返回原始表达量）。
        ''' </summary>
        ''' <param name="initial">初始状态（原始表达量，长度 = 基因数）</param>
        ''' <param name="clamps">被钳制的基因 → 固定值（原始表达量）；<c>Nothing</c> 表示不钳制</param>
        ''' <param name="steps">最大迭代步数；&lt;= 0 时取配置值</param>
        ''' <param name="relaxation">阻尼系数；<c>NaN</c> 时取配置值</param>
        ''' <param name="tolerance">收敛阈值；<c>NaN</c> 时取配置值</param>
        ''' <returns>稳态（原始表达量）</returns>
        Public Function SteadyState(initial As Double(),
                                    Optional clamps As Dictionary(Of String, Double) = Nothing,
                                    Optional steps As Integer = 0,
                                    Optional relaxation As Double = Double.NaN,
                                    Optional tolerance As Double = Double.NaN) As Double()

            If initial Is Nothing Then
                Throw New ArgumentNullException(NameOf(initial))
            End If
            If initial.Length <> NGene Then
                Throw New ArgumentException($"the state has {initial.Length} values but the network has {NGene} genes", NameOf(initial))
            End If

            Dim maxSteps As Integer = If(steps > 0, steps, _config.PropagationSteps)
            Dim omega As Double = If(Double.IsNaN(relaxation), _config.Relaxation, relaxation)
            Dim tol As Double = If(Double.IsNaN(tolerance), _config.ConvergenceTolerance, tolerance)

            If omega <= 0.0 OrElse omega > 1.0 Then
                omega = 0.5
            End If
            If tol <= 0.0 Then
                tol = 0.000001
            End If

            Dim z As Double() = ToZ(initial)
            Dim clampIndex As New List(Of Integer)
            Dim clampValue As New List(Of Double)

            Call ResolveClamps(clamps, z, clampIndex, clampValue)

            Dim n As Integer = NGene
            Dim workers As Integer = Workers

            ' 双缓冲：nextZ / nextDirty 每次迭代复用，不再每步分配两个 N 长数组
            Dim nextZ(n - 1) As Double
            Dim dirty As Boolean() = Repeat(True, n)
            Dim nextDirty(n - 1) As Boolean

            For stepIndex As Integer = 0 To maxSteps - 1
                ' 1) 基因维并行：只读 z / dirty，只写 nextZ（未重算的基因保持原值）
                Call StepZInto(z, dirty, stepIndex = 0, nextZ, workers)

                ' 2) 阻尼混合 + 收敛统计：max 精确可结合，分块合并顺序不影响结果
                Dim maxDelta As Double = BlendInto(z, nextZ, omega, nextDirty, workers)

                ' 3) 钳制回写必须串行：它和混合写的是同一批位置
                For k As Integer = 0 To clampIndex.Count - 1
                    z(clampIndex(k)) = clampValue(k)
                    nextDirty(clampIndex(k)) = False
                Next

                ' 4) 交换脏集缓冲：BlendRange 对 [0, N) 全段赋值，因此换过来的缓冲不需要清零
                Dim swapDirty As Boolean() = dirty

                dirty = nextDirty
                nextDirty = swapDirty

                If maxDelta < tol Then
                    Exit For
                End If
            Next

            Return ToRaw(z)
        End Function

        ''' <summary>
        ''' 多步级联模拟。
        ''' </summary>
        ''' <param name="initial">初始状态（原始表达量）</param>
        ''' <param name="clamps">钳制条件（原始表达量）</param>
        ''' <param name="steps">时间步数（含第 0 步的初始状态）</param>
        ''' <returns>
        ''' 轨迹 <c>[时间步, 基因]</c>，每个元素是长度为 1 的样本数组
        ''' （与 BNLearn 的 <c>InterventionResult.DynamicTrajectory</c> 约定一致）
        ''' </returns>
        Public Function SimulateCascade(initial As Double(),
                                        Optional clamps As Dictionary(Of String, Double) = Nothing,
                                        Optional steps As Integer = 0) As Double(,)()
            If initial Is Nothing Then
                Throw New ArgumentNullException(NameOf(initial))
            End If

            Dim count As Integer = If(steps > 0, steps, _config.PropagationSteps)

            If count < 1 Then
                count = 1
            End If

            Dim z As Double() = ToZ(initial)
            Dim clampIndex As New List(Of Integer)
            Dim clampValue As New List(Of Double)

            Call ResolveClamps(clamps, z, clampIndex, clampValue)

            Dim trajectory(count - 1, NGene - 1)() As Double

            Record(trajectory, 0, z)

            ' forceAll 为 True 时 dirty 不参与判定，复用同一个全 True 缓冲即可（不再每步 Repeat）
            Dim allDirty As Boolean() = Repeat(True, NGene)

            For t As Integer = 1 To count - 1
                z = StepZ(z, allDirty, True)

                For k As Integer = 0 To clampIndex.Count - 1
                    z(clampIndex(k)) = clampValue(k)
                Next

                Record(trajectory, t, z)
            Next

            Return trajectory
        End Function

        Private Sub Record(trajectory As Double(,)(), stepIndex As Integer, z As Double())
            Dim raw As Double() = ToRaw(z)

            For i As Integer = 0 To NGene - 1
                trajectory(stepIndex, i) = New Double() {raw(i)}
            Next
        End Sub

        ''' <summary>
        ''' 把钳制条件写进 z 状态，并登记被固定的基因下标与取值。
        ''' </summary>
        Private Sub ResolveClamps(clamps As Dictionary(Of String, Double),
                                  z As Double(),
                                  ByRef clampIndex As List(Of Integer),
                                  ByRef clampValue As List(Of Double))
            If clamps Is Nothing Then
                Return
            End If

            For Each item As KeyValuePair(Of String, Double) In clamps
                Dim idx As Integer = IndexOfGene(item.Key)

                If idx < 0 Then
                    Continue For
                End If

                Dim value As Double = item.Value

                If value.IsNaNImaginary Then
                    Continue For
                End If

                Dim zv As Double = ToZValue(idx, value)

                z(idx) = zv
                Call clampIndex.Add(idx)
                Call clampValue.Add(zv)
            Next
        End Sub

        ''' <summary>
        ''' 一步状态更新（分配结果数组的便捷入口）：只重算「有调控因子发生变化」的基因。
        ''' </summary>
        ''' <remarks>
        ''' 单次调用（<see cref="Predict"/>、<see cref="SimulateCascade"/>）用这个入口；
        ''' 稳态迭代走 <see cref="StepZInto"/> 的零分配路径。
        ''' </remarks>
        Private Function StepZ(z As Double(), dirty As Boolean(), forceAll As Boolean) As Double()
            Dim result(NGene - 1) As Double

            Call StepZInto(z, dirty, forceAll, result, Workers)

            Return result
        End Function

        ''' <summary>
        ''' 一步状态更新，结果写入 <paramref name="nextZ"/>；基因维按块并行。
        ''' </summary>
        ''' <param name="z">当前状态（z 空间，只读）</param>
        ''' <param name="dirty">上一步发生变化的基因（只读）</param>
        ''' <param name="forceAll">为 <c>True</c> 时忽略脏集，全量重算</param>
        ''' <param name="nextZ">目标缓冲，长度必须等于基因数</param>
        ''' <param name="workers">并行度（<c>1</c> 表示串行）</param>
        ''' <remarks>
        ''' <para>
        ''' 每个基因只写自己的 <c>nextZ(g)</c>，对 <c>z</c> / <c>dirty</c> 只读，
        ''' 因此按基因并行天然无竞争、也与线程调度无关：结果逐位可复现。
        ''' 线程局部缓冲（<see cref="_inputScratch"/>）在 <c>TskNetwork</c> 一侧同样是每线程一份，
        ''' 所以并行不需要任何锁。
        ''' </para>
        ''' <para>
        ''' <b>不能就地写回 <paramref name="z"/></b>：同一步内其他基因还要读旧状态。
        ''' </para>
        ''' </remarks>
        Private Sub StepZInto(z As Double(),
                              dirty As Boolean(),
                              forceAll As Boolean,
                              nextZ As Double(),
                              workers As Integer)

            Dim n As Integer = NGene

            ' 未被重算的基因必须逐位保持原值（也保证混合循环看到的是完整的下一步状态）
            Call Array.Copy(z, nextZ, n)

            If Not ShouldParallelize(n, workers) Then
                Call StepZRange(z, dirty, forceAll, nextZ, 0, n)

                Return
            End If

            Dim chunk As Integer = ChunkGenes(n, workers)
            Dim nChunks As Integer = (n + chunk - 1) \ chunk
            Dim options As System.Threading.Tasks.ParallelOptions = ParallelOptionsOf(workers)

            ' Parallel 必须全限定：Microsoft.VisualBasic.Parallel 会遮蔽同名类型
            System.Threading.Tasks.Parallel.For(0, nChunks, options,
                Sub(c)
                    Dim start As Integer = c * chunk

                    Call StepZRange(z, dirty, forceAll, nextZ, start, std.Min(start + chunk, n))
                End Sub)
        End Sub

        ''' <summary>
        ''' <see cref="StepZInto"/> 的逐基因内核，只处理 <c>[start, ends)</c> 区间。
        ''' </summary>
        Private Sub StepZRange(z As Double(),
                               dirty As Boolean(),
                               forceAll As Boolean,
                               nextZ As Double(),
                               start As Integer,
                               ends As Integer)

            ' 提到循环外：配置在一次迭代内不会变化，避免每个基因都做一次属性访问
            Dim clampToSupport As Boolean = _config.ClampToSupport

            For g As Integer = start To ends - 1
                Dim model As GeneRegulationModel = Models(g)

                If model Is Nothing Then
                    Continue For
                End If

                Dim inputs As Integer() = _regulatorIndices(g)

                If inputs.Length = 0 Then
                    Continue For
                End If
                If Not forceAll AndAlso Not AnyDirty(inputs, dirty) Then
                    Continue For
                End If

                Dim row As Double() = _inputScratch.Value

                If row.Length < inputs.Length Then
                    ReDim row(inputs.Length - 1)
                    _inputScratch.Value = row
                End If

                For j As Integer = 0 To inputs.Length - 1
                    row(j) = z(inputs(j))
                Next

                Dim value As Double

                Try
                    value = model.Predict(row)
                Catch ex As Exception
                    ' 单个基因的推理失败不应中断整个网络（例如规则被剪空）
                    value = z(g)
                End Try

                If value.IsNaNImaginary Then
                    value = z(g)
                End If

                If clampToSupport Then
                    Dim lower As Double = SupportMin(g) - 1.0
                    Dim upper As Double = SupportMax(g) + 1.0

                    If upper > lower Then
                        If value < lower Then
                            value = lower
                        ElseIf value > upper Then
                            value = upper
                        End If
                    End If
                End If

                nextZ(g) = value
            Next
        End Sub

        ''' <summary>
        ''' 阻尼混合 <c>z ← z + ω·(next − z)</c> + 收敛统计 + 脏位刷新；基因维按块并行。
        ''' </summary>
        ''' <param name="z">当前状态，就地更新</param>
        ''' <param name="nextZ">一步预测结果</param>
        ''' <param name="omega">阻尼系数</param>
        ''' <param name="nextDirty">脏位输出（整段赋值，无需预先清零）</param>
        ''' <param name="workers">并行度（<c>1</c> 表示串行）</param>
        ''' <returns>本步的最大变化量</returns>
        ''' <remarks>
        ''' <c>max</c> 精确可结合，因此分块归约的合并顺序不影响结果——
        ''' 并行与串行给出<b>逐位相同</b>的 <paramref name="maxDelta"/> 与 <paramref name="z"/>，
        ''' 这一点与 <c>Sum</c>/<c>Dot</c> 的分块归约（末位 ULP 会变）不同。
        ''' </remarks>
        Private Function BlendInto(z As Double(),
                                   nextZ As Double(),
                                   omega As Double,
                                   nextDirty As Boolean(),
                                   workers As Integer) As Double

            Dim n As Integer = NGene

            If Not ShouldParallelize(n, workers) Then
                Return BlendRange(z, nextZ, omega, nextDirty, 0, n)
            End If

            Dim chunk As Integer = ChunkGenes(n, workers)
            Dim nChunks As Integer = (n + chunk - 1) \ chunk
            Dim partials As Double() = New Double(nChunks - 1) {}
            Dim options As System.Threading.Tasks.ParallelOptions = ParallelOptionsOf(workers)

            ' 每个分块只写自己的 partials(c)，无锁、无跨块共享写
            System.Threading.Tasks.Parallel.For(0, nChunks, options,
                Sub(c)
                    Dim start As Integer = c * chunk

                    partials(c) = BlendRange(z, nextZ, omega, nextDirty, start, std.Min(start + chunk, n))
                End Sub)

            Dim maxDelta As Double = 0.0

            For i As Integer = 0 To nChunks - 1
                If partials(i) > maxDelta Then
                    maxDelta = partials(i)
                End If
            Next

            Return maxDelta
        End Function

        ''' <summary>
        ''' <see cref="BlendInto"/> 的逐元素内核，只处理 <c>[start, ends)</c> 区间。
        ''' </summary>
        ''' <remarks>
        ''' 脏位在这里是<b>直接赋值</b>而不是「先清零再条件置 True」：
        ''' 因为本函数覆盖 <c>[0, N)</c> 的每一个下标，两种写法完全等价，
        ''' 但直接赋值可以省掉每步一次 <c>Array.Clear</c>。
        ''' </remarks>
        Private Shared Function BlendRange(z As Double(),
                                           nextZ As Double(),
                                           omega As Double,
                                           nextDirty As Boolean(),
                                           start As Integer,
                                           ends As Integer) As Double

            Dim maxDelta As Double = 0.0

            For i As Integer = start To ends - 1
                Dim blended As Double = z(i) + omega * (nextZ(i) - z(i))
                Dim delta As Double = std.Abs(blended - z(i))

                z(i) = blended
                nextDirty(i) = delta > DirtyEpsilon

                If delta > maxDelta Then
                    maxDelta = delta
                End If
            Next

            Return maxDelta
        End Function

        Private Shared Function AnyDirty(indices As Integer(), dirty As Boolean()) As Boolean
            For Each idx As Integer In indices
                If dirty(idx) Then
                    Return True
                End If
            Next

            Return False
        End Function

        Private Shared Function Repeat(value As Boolean, count As Integer) As Boolean()
            Dim result(count - 1) As Boolean

            For i As Integer = 0 To count - 1
                result(i) = value
            Next

            Return result
        End Function

        ''' <summary>原始表达量向量 → z 向量。</summary>
        Public Function ToZ(state As Double()) As Double()
            If state Is Nothing Then
                Throw New ArgumentNullException(NameOf(state))
            End If
            If state.Length <> NGene Then
                Throw New ArgumentException($"the state has {state.Length} values but the network has {NGene} genes", NameOf(state))
            End If

            Dim z(NGene - 1) As Double

            For i As Integer = 0 To NGene - 1
                z(i) = ToZValue(i, state(i))
            Next

            Return z
        End Function

        ''' <summary>z 向量 → 原始表达量向量。</summary>
        Public Function ToRaw(z As Double()) As Double()
            If Not _standardized Then
                Return CType(z.Clone(), Double())
            End If

            Dim raw(NGene - 1) As Double

            For i As Integer = 0 To NGene - 1
                raw(i) = z(i) * SDs(i) + Means(i)
            Next

            Return raw
        End Function

        ''' <summary>单基因原始值 → z 值。</summary>
        Public Function ToZValue(gene As Integer, value As Double) As Double
            If Not _standardized Then
                Return value
            End If

            Dim sd As Double = SDs(gene)

            If std.Abs(sd) < 0.000000000001 Then
                sd = 1.0
            End If

            Return (value - Means(gene)) / sd
        End Function

#End Region

#Region "可解释性导出"

        ''' <summary>全部规则的文本描述（按靶基因分段）。</summary>
        Public Function RulesText() As String()
            Dim lines As New List(Of String)

            For g As Integer = 0 To NGene - 1
                Dim model As GeneRegulationModel = Models(g)

                If model Is Nothing Then
                    Continue For
                End If

                Call lines.Add($"# {model.TargetGene} <- {{{String.Join(", ", If(model.Regulators, New String() {}))}}}")

                For Each rule As String In model.RulesText()
                    Call lines.Add(rule)
                Next

                Call lines.Add("")
            Next

            Return lines.ToArray()
        End Function

        ''' <summary>SIF 网络文本（<c>Source  pp  Target</c>）。</summary>
        Public Function EdgesToSif() As String
            Dim sb As New StringBuilder()

            For Each edge As RegulationEdgeModel In Edges
                Call sb.AppendLine(edge.ToSifLine())
            Next

            Return sb.ToString()
        End Function

        ''' <summary>CSV 边表文本（含表头）。</summary>
        Public Function EdgesToCsv() As String
            Dim sb As New StringBuilder()

            Call sb.AppendLine(RegulationEdgeModel.CsvHeader())

            For Each edge As RegulationEdgeModel In Edges
                Call sb.AppendLine(edge.ToCsvLine())
            Next

            Return sb.ToString()
        End Function

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"FuzzyGene network: {NGene} genes ({NModelled} modelled), {Edges.Length} edges, " &
                   $"{String.Join(",", Models.Where(Function(m) m IsNot Nothing).Sum(Function(m) m.RuleCount))} rules, " &
                   $"mode={Mode}"
        End Function

#End Region

#Region "序列化"

        ''' <summary>序列化为 JSON（含全部模型）。</summary>
        Public Function ToJson(Optional indented As Boolean = True) As String
            Return JsonSerializer.Serialize(ToDto(), JsonOptions(indented))
        End Function

        ''' <summary>
        ''' 构造传输对象。
        ''' </summary>
        ''' <param name="includeModels">为 False 时只导出 manifest 部分（zip 写入时模型按基因分文件）</param>
        Friend Function ToDto(Optional includeModels As Boolean = True) As FuzzyGeneNetworkDto
            Dim dto As New FuzzyGeneNetworkDto With {
                .formatVersion = "1.0",
                .createdAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                .mode = Mode,
                .geneNames = GeneNames,
                .means = Means,
                .sds = SDs,
                .baseline = BaselineRaw,
                .supportMin = SupportMin,
                .supportMax = SupportMax,
                .config = _config,
                .standardized = _standardized,
                .edges = Edges
            }

            If includeModels Then
                dto.models = Models.Select(Function(m) If(m Is Nothing, Nothing, m.ToDto())).ToArray()
            End If

            Return dto
        End Function

        ''' <summary>从 JSON 还原。</summary>
        Public Shared Function FromJson(json As String) As FuzzyGeneNetwork
            If String.IsNullOrEmpty(json) Then
                Throw New ArgumentException("the json text is empty", NameOf(json))
            End If

            Dim dto As FuzzyGeneNetworkDto = JsonSerializer.Deserialize(Of FuzzyGeneNetworkDto)(json, JsonOptions(False))

            If dto Is Nothing OrElse dto.geneNames Is Nothing Then
                Throw New InvalidOperationException("the json text does not describe a fuzzy gene network")
            End If

            Return FromDto(dto)
        End Function

        ''' <summary>由传输对象还原（zip 读取路径复用）。</summary>
        Friend Shared Function FromDto(dto As FuzzyGeneNetworkDto) As FuzzyGeneNetwork
            Dim models(dto.geneNames.Length - 1) As GeneRegulationModel

            If dto.models IsNot Nothing Then
                For i As Integer = 0 To std.Min(dto.models.Length, models.Length) - 1
                    If dto.models(i) IsNot Nothing Then
                        models(i) = GeneRegulationModel.FromDto(dto.models(i))
                    End If
                Next
            End If

            Return New FuzzyGeneNetwork(
                dto.geneNames,
                models,
                If(dto.edges, New RegulationEdgeModel() {}),
                If(dto.config, New FuzzyGeneConfig()),
                If(dto.means, New Double(dto.geneNames.Length - 1) {}),
                If(dto.sds, Repeat(1.0, dto.geneNames.Length)),
                If(dto.baseline, New Double(dto.geneNames.Length - 1) {}),
                If(dto.supportMin, New Double(dto.geneNames.Length - 1) {}),
                If(dto.supportMax, New Double(dto.geneNames.Length - 1) {}),
                dto.mode,
                dto.standardized)
        End Function

        Private Shared Function Repeat(value As Double, count As Integer) As Double()
            Dim result(count - 1) As Double

            For i As Integer = 0 To count - 1
                result(i) = value
            Next

            Return result
        End Function

        ''' <summary>JSON 选项（枚举按名称写出，中文基因名不转义）。</summary>
        Friend Shared Function JsonOptions(indented As Boolean) As JsonSerializerOptions
            Dim options As New JsonSerializerOptions With {
                .WriteIndented = indented
            }

            Call options.Converters.Add(New JsonStringEnumConverter())

            Return options
        End Function

#End Region

    End Class

    ''' <summary>调控网络包的传输对象（manifest 与整体 JSON 共用）。</summary>
    Public Class FuzzyGeneNetworkDto

        ''' <summary>格式版本。</summary>
        Public Property formatVersion As String
        ''' <summary>创建时间。</summary>
        Public Property createdAt As String
        ''' <summary>拟合路线。</summary>
        Public Property mode As FuzzyTargetMode
        ''' <summary>基因名。</summary>
        Public Property geneNames As String()
        ''' <summary>逐基因原始均值。</summary>
        Public Property means As Double()
        ''' <summary>逐基因原始标准差。</summary>
        Public Property sds As Double()
        ''' <summary>稳态迭代起始状态。</summary>
        Public Property baseline As Double()
        ''' <summary>支持域下界（z 空间）。</summary>
        Public Property supportMin As Double()
        ''' <summary>支持域上界（z 空间）。</summary>
        Public Property supportMax As Double()
        ''' <summary>建模配置。</summary>
        Public Property config As FuzzyGeneConfig
        ''' <summary>模型是否在 z 空间训练（缺省视为 True，对齐历史包）。</summary>
        Public Property standardized As Boolean = True
        ''' <summary>带符号调控边。</summary>
        Public Property edges As RegulationEdgeModel()
        ''' <summary>逐基因模型（未建模的位置为 <c>Nothing</c>）。</summary>
        Public Property models As GeneModelDto()

    End Class

