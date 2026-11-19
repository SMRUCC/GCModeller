Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic.DataMining.FuzzyLogic.FuzzyCognitiveMaps
Imports SMRUCC.genomics.Analysis.FuzzyGene.Config
Imports SMRUCC.genomics.Analysis.FuzzyGene.Data
Imports std = System.Math

Namespace Hybrid

    ''' <summary>
    ''' 模块级模糊认知图（FCM）的结果：模块稳态、逐基因热启动初值与代理结论。
    ''' </summary>
    Public Class FcmModuleResult

        ''' <summary>FCM 是否在步数预算内收敛。</summary>
        Public Property Converged As Boolean

        ''' <summary>实际传播步数；未收敛时为 -1。</summary>
        Public Property Steps As Integer

        ''' <summary>检出极限环时的周期；0 表示没有检出。</summary>
        Public Property Period As Integer

        ''' <summary>模块激活（<c>[0, 1]</c>，与模块编号对应）。</summary>
        Public Property ModuleActivation As Double()

        ''' <summary>由模块稳态反推的逐基因初值（原始表达量）。</summary>
        Public Property GeneInitial As Double()

        ''' <summary>模块稳态反推的逐基因代理结论（原始表达量）。</summary>
        Public Property Surrogate As Double()

        ''' <summary>说明与降级信息。</summary>
        Public Property Notes As String

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"FCM module layer: {If(ModuleActivation Is Nothing, 0, ModuleActivation.Length)} module(s), " &
                   $"converged={Converged}, steps={Steps}, period={Period}"
        End Function

    End Class

    ''' <summary>
    ''' 模块级模糊认知图：有界激活 + 收敛保证，用于给逐基因求根提供热启动与代理结论。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>为什么需要这一层</b>：逐基因求根对初值敏感（多吸引子时落到哪个由初值决定），
    ''' 而 FCM 的激活被 <c>SigmoidActivation</c> 夹在 <c>[0,1]</c>、传播被
    ''' <c>ClampToFunction</c> 夹在值域内，因此它**必然有界**，`RunToConvergence` 也一定能给出
    ''' 「收敛到第几步」或「检出极限环」这两种明确结局。
    ''' 把模块稳态反推成逐基因初值，等于给细粒度求解器一个「全局自洽」的起点，
    ''' 大幅降低它落到无意义分支的概率。
    ''' </para>
    ''' <para>
    ''' <b>它不是替代品</b>：模块平均把个体差异抹平了，只能当粗粒度骨架。
    ''' 因此默认关闭（<see cref="HybridOptions.FcmWarmStart"/> = False），
    ''' 且反推的初值只是一个**起点**，最终结论仍由 TSK 求根给出。
    ''' </para>
    ''' <para>
    ''' 模块划分在拟合期用「抽样行上的 k-means」算好并随网络保存；
    ''' 这样做而不是在扰动期现场聚类，是因为扰动期只剩模型、没有表达矩阵。
    ''' </para>
    ''' </remarks>
    Public Class FcmModuleLayer

        ''' <summary>k-means 使用的抽样行数上限（模块划分不需要全量样本）。</summary>
        Private Const MaxSampleRows As Integer = 64

        ''' <summary>k-means 的最大迭代轮数。</summary>
        Private Const MaxKMeansIterations As Integer = 10

        Private ReadOnly _network As FuzzyGeneNetwork
        Private ReadOnly _config As FuzzyGeneConfig
        Private ReadOnly _modules As Integer()
        Private ReadOnly _moduleCount As Integer

        ''' <summary>绑定网络（网络必须带有模块划分）。</summary>
        Public Sub New(network As FuzzyGeneNetwork, Optional config As FuzzyGeneConfig = Nothing)
            If network Is Nothing Then
                Throw New ArgumentNullException(NameOf(network))
            End If

            _network = network
            _config = If(config, network.Config)
            _modules = network.Modules
            _moduleCount = If(_modules Is Nothing, 0, _modules.Max() + 1)
        End Sub

        ''' <summary>网络是否带有可用的模块划分。</summary>
        Public ReadOnly Property Available As Boolean
            Get
                Return _modules IsNot Nothing AndAlso _moduleCount >= 2
            End Get
        End Property

        ''' <summary>
        ''' 计算模块稳态，并反推逐基因初值。
        ''' </summary>
        ''' <param name="baseline">基线（原始表达量）</param>
        ''' <param name="clamps">钳制条件（原始表达量）</param>
        Public Function BuildInitial(baseline As Double(),
                                     Optional clamps As Dictionary(Of String, Double) = Nothing) As FcmModuleResult
            If Not Available Then
                Return Nothing
            End If

            Dim options As HybridOptions = _config.Hybrid
            Dim activation As Double() = ModuleActivation(baseline)
            Dim concepts(_moduleCount - 1) As FcmConcept

            For m As Integer = 0 To _moduleCount - 1
                concepts(m) = New FcmConcept("module" & m.ToString(), activation(m))
            Next

            Dim weights As Double(,) = ModuleWeights(baseline)
            Dim fcm As New FuzzyCognitiveMap(concepts, weights,
                                             New SigmoidActivation(options.FcmAlpha),
                                             includeSelfLoop:=False)

            Dim steps As Integer = fcm.RunToConvergence(options.FcmTolerance, options.FcmMaxSteps)
            Dim result As New FcmModuleResult With {
                .Converged = steps > 0,
                .Steps = steps
            }

            If steps < 0 Then
                ' 未收敛时显式检测极限环：模块级的持续振荡本身就是结论，
                ' 而且它的周期均值可以作为代理（比「跑满步数」诚实得多）
                Dim cycle = fcm.DetectCycle(options.FcmMaxSteps, options.FcmTolerance)

                If cycle IsNot Nothing Then
                    result.Period = cycle.Period
                End If
            End If

            result.ModuleActivation = fcm.Activation
            result.GeneInitial = ToGeneState(result.ModuleActivation, baseline, clamps)
            result.Surrogate = result.GeneInitial

            If steps < 0 Then
                result.Notes = $"the module map did not converge within {options.FcmMaxSteps} step(s)" &
                               If(result.Period > 0, $"; a limit cycle of period {result.Period} was detected and its state is used as a surrogate", "")
            End If

            Return result
        End Function

        ''' <summary>模块激活：模块内基因 z 值的均值过 sigmoid。</summary>
        Private Function ModuleActivation(baseline As Double()) As Double()
            Dim z As Double() = _network.ToZ(baseline)
            Dim sums(_moduleCount - 1) As Double
            Dim counts(_moduleCount - 1) As Integer

            For g As Integer = 0 To _network.NGene - 1
                Dim m As Integer = _modules(g)

                sums(m) += z(g)
                counts(m) += 1
            Next

            Dim activation(_moduleCount - 1) As Double
            Dim alpha As Double = _config.Hybrid.FcmAlpha

            For m As Integer = 0 To _moduleCount - 1
                Dim average As Double = If(counts(m) > 0, sums(m) / counts(m), 0.0)

                ' 夹到 ±8：sigmoid 的饱和区已经足够，再大的自变量只会让反推失真
                average = std.Max(-8.0, std.Min(8.0, average))
                activation(m) = 1.0 / (1.0 + std.Exp(-alpha * average))
            Next

            Return activation
        End Function

        ''' <summary>模块间权重：把逐基因 Jacobian 按模块求和后归一化。</summary>
        Private Function ModuleWeights(baseline As Double()) As Double(,)
            Dim weights(_moduleCount - 1, _moduleCount - 1) As Double
            Dim counts(_moduleCount - 1) As Integer
            Dim jacobian As New Solver.JacobianOperator(_network)

            Call jacobian.Build(_network.ToZ(baseline), 1)

            For g As Integer = 0 To _network.NGene - 1
                counts(_modules(g)) += 1
            Next

            Dim indices As Integer()() = _network.RegulatorIndexTable

            For g As Integer = 0 To _network.NGene - 1
                Dim row As Double() = jacobian.Row(g)

                If row Is Nothing Then
                    Continue For
                End If

                Dim target As Integer = _modules(g)
                Dim inputs As Integer() = indices(g)

                For j As Integer = 0 To row.Length - 1
                    weights(_modules(inputs(j)), target) += row(j)
                Next
            Next

            Dim peak As Double = 0.0

            For i As Integer = 0 To _moduleCount - 1
                For j As Integer = 0 To _moduleCount - 1
                    Dim n As Integer = std.Max(1, counts(j))

                    weights(i, j) /= n
                    peak = std.Max(peak, std.Abs(weights(i, j)))
                Next
            Next

            ' 权重整体压到 [-1, 1]：FCM 的传播是「加权和过 sigmoid」，
            ' 权重幅值超过 1 时单个模块就能把别的模块推到饱和，热启动会失真
            If peak > 1.0 Then
                For i As Integer = 0 To _moduleCount - 1
                    For j As Integer = 0 To _moduleCount - 1
                        weights(i, j) /= peak
                    Next
                Next
            End If

            Return weights
        End Function

        ''' <summary>把模块激活反推成逐基因原始表达量。</summary>
        Private Function ToGeneState(activation As Double(),
                                     baseline As Double(),
                                     clamps As Dictionary(Of String, Double)) As Double()
            Dim state(_network.NGene - 1) As Double

            For g As Integer = 0 To _network.NGene - 1
                Dim a As Double = activation(_modules(g))

                ' (a - 0.5) * 4 把 [0,1] 映到 [-2,2] 的 z 区间：
                ' sigmoid 的中心对应「等于训练均值」，两端对应 ±2σ
                Dim zValue As Double = (a - 0.5) * 4.0

                state(g) = _network.Means(g) + zValue * _network.SDs(g)
            Next

            If clamps IsNot Nothing Then
                For Each item As KeyValuePair(Of String, Double) In clamps
                    Dim idx As Integer = _network.IndexOfGene(item.Key)

                    If idx >= 0 Then
                        state(idx) = item.Value
                    End If
                Next
            End If

            Return state
        End Function

        ''' <summary>
        ''' 在表达视图上按「抽样行的 k-means」分配模块。
        ''' </summary>
        ''' <param name="view">预处理视图</param>
        ''' <param name="moduleCount">期望的模块数</param>
        ''' <returns>逐基因模块编号（长度 = 基因数）</returns>
        ''' <remarks>
        ''' 自己实现 Lloyd 迭代而不是复用 <c>TskNetworkBuilder.CreateScatter</c>：
        ''' 后者按「样本 × 输入维」聚类且要求把整张矩阵物化成 jagged 行，
        ''' 而这里要的是「基因 × 抽样维」——18000 基因 × 64 个抽样行只占 9 MB，
        ''' 按样本物化则是 3000 × 18000，完全不是同一个量级。
        ''' </remarks>
        Public Shared Function AssignModules(view As GeneExpressionView,
                                             moduleCount As Integer,
                                             Optional seed As Integer = 0) As Integer()
            If view Is Nothing Then
                Throw New ArgumentNullException(NameOf(view))
            End If

            Dim n As Integer = view.NGene
            Dim k As Integer = std.Max(2, std.Min(moduleCount, std.Max(2, n \ 50)))

            If n <= k Then
                ' 基因太少时每个基因一个模块，等价于「不做模块化」
                Dim trivial(n - 1) As Integer

                For g As Integer = 0 To n - 1
                    trivial(g) = g
                Next

                Return trivial
            End If

            Dim rows As Integer = std.Min(MaxSampleRows, std.Max(2, view.NRows))
            Dim samples(rows - 1) As Integer
            Dim stride As Double = view.NRows / CDbl(rows)

            For r As Integer = 0 To rows - 1
                samples(r) = std.Min(view.NRows - 1, CInt(std.Floor(r * stride)))
            Next

            ' 初值取等距下标的基因，避免随机初值带来的不可复现
            Dim centers(k - 1, rows - 1) As Double

            For c As Integer = 0 To k - 1
                Dim column As Double() = view.Columns(CInt(std.Floor(c * (n - 1) / CDbl(std.Max(1, k - 1)))))

                For r As Integer = 0 To rows - 1
                    centers(c, r) = column(samples(r))
                Next
            Next

            Dim modules(n - 1) As Integer
            Dim sums(k - 1, rows - 1) As Double
            Dim counts(k - 1) As Integer

            For iteration As Integer = 1 To MaxKMeansIterations
                For c As Integer = 0 To k - 1
                    counts(c) = 0

                    For r As Integer = 0 To rows - 1
                        sums(c, r) = 0.0
                    Next
                Next

                For g As Integer = 0 To n - 1
                    Dim column As Double() = view.Columns(g)
                    Dim best As Integer = 0
                    Dim bestDistance As Double = Double.MaxValue

                    For c As Integer = 0 To k - 1
                        Dim distance As Double = 0.0

                        For r As Integer = 0 To rows - 1
                            Dim d As Double = column(samples(r)) - centers(c, r)

                            distance += d * d

                            If distance >= bestDistance Then
                                Exit For
                            End If
                        Next

                        If distance < bestDistance Then
                            bestDistance = distance
                            best = c
                        End If
                    Next

                    modules(g) = best
                    counts(best) += 1

                    For r As Integer = 0 To rows - 1
                        sums(best, r) += column(samples(r))
                    Next
                Next

                For c As Integer = 0 To k - 1
                    If counts(c) = 0 Then
                        Continue For
                    End If

                    For r As Integer = 0 To rows - 1
                        centers(c, r) = sums(c, r) / counts(c)
                    Next
                Next
            Next

            Return modules
        End Function

    End Class

End Namespace
