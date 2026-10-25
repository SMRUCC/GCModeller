Imports System.Text
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.Darwinism
Imports Microsoft.VisualBasic.Math.Framework.Optimization.LBFGSB
Imports PriorEdge = SMRUCC.genomics.Analysis.BNLearn.Core.RegulatoryEdge
Imports PriorNet = SMRUCC.genomics.Analysis.BNLearn.Core.PriorNetwork
Imports std = System.Math

Namespace GRN

    ''' <summary>
    ''' The objective function of the decoupled S-system inference for one single gene.
    ''' (单个基因的解耦拟合目标函数)
    ''' </summary>
    ''' <remarks>
    ''' 目标函数取「斜率残差平方和」，即：
    ''' f = sum_k (alpha*prod(X^g) - beta*prod(X^h) - S_ik)^2 / scale + L1 * sum(|g| + |h|)
    ''' 其中所有的幂律项都通过对数域求和之后再取指数来计算，避免逐点调用 Pow 成为性能瓶颈。
    ''' </remarks>
    Public Class SlopeProblem

        ''' <summary>
        ''' 候选调控因子的对数表达量，第一维是候选因子、第二维是时间点
        ''' </summary>
        ''' <returns></returns>
        Public Property LnX As Double()()
        ''' <summary>
        ''' 目标基因的观测斜率
        ''' </summary>
        ''' <returns></returns>
        Public Property Observed As Double()
        ''' <summary>
        ''' 残差的归一化系数
        ''' </summary>
        ''' <returns></returns>
        Public Property Scale As Double
        ''' <summary>
        ''' L1 稀疏惩罚的权重
        ''' </summary>
        ''' <returns></returns>
        Public Property L1 As Double
        ''' <summary>
        ''' 自环在候选调控因子之中的下标，负数表示当前基因不包含自环
        ''' </summary>
        ''' <returns></returns>
        Public Property SelfIndex As Integer = -1
        ''' <summary>
        ''' 稳定性惩罚的权重：稳态的渐近稳定要求对数增益矩阵的对角元为负数，
        ''' 故这里会对正的自环净弹性施加惩罚
        ''' </summary>
        ''' <returns></returns>
        Public Property StabilityWeight As Double = 1.0R

        ''' <summary>
        ''' 候选调控因子是否被先验知识所支持；为空的时候不启用先验软约束
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorMask As Boolean()
        ''' <summary>
        ''' 先验知识所给出的调控方向（+1 激活 / -1 抑制 / 0 未知）；为空的时候不启用符号约束
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorSign As Integer()
        ''' <summary>
        ''' 非先验交互的软惩罚强度
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorPenalty As Double = 0
        ''' <summary>
        ''' 符号违背的惩罚强度
        ''' </summary>
        ''' <returns></returns>
        Public Property PriorSignPenalty As Double = 0

        ''' <summary>
        ''' 计算归一化的目标函数值（越小越好）
        ''' </summary>
        ''' <param name="p">参数向量 [ln(alpha), ln(beta), g(0..k-1), h(0..k-1)]</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 目标函数取「相对均方根斜率误差」再乘以一个无量纲的惩罚因子：
        ''' 相对误差项保证了目标函数的量级与表达尺度无关，
        ''' 而乘性的惩罚因子（稀疏 + 稳定性）则保证了惩罚项不会压过残差项。
        ''' </remarks>
        Public Function Objective(p As Double()) As Double
            Dim m As Integer = Observed.Length
            Dim k As Integer = (p.Length - 2) \ 2
            Dim err As Double = 0
            Dim penalty As Double = 1.0R

            For t As Integer = 0 To m - 1
                Dim la As Double = p(0)
                Dim lb As Double = p(1)

                For c As Integer = 0 To k - 1
                    Dim lx As Double = LnX(c)(t)

                    la += p(2 + c) * lx
                    lb += p(2 + k + c) * lx
                Next

                Dim a As Double = std.Exp(clampExp(la))
                Dim b As Double = std.Exp(clampExp(lb))
                Dim d As Double = (a - b) - Observed(t)

                err += d * d
            Next

            Dim relative As Double = std.Sqrt(err / m) / Scale

            If L1 > 0 Then
                Dim l1 As Double = 0

                For c As Integer = 0 To k - 1
                    l1 += std.Abs(p(2 + c)) + std.Abs(p(2 + k + c))
                Next

                penalty += L1 * l1
            End If

            If SelfIndex >= 0 AndAlso StabilityWeight > 0 Then
                Dim netSelf As Double = p(2 + SelfIndex) - p(2 + k + SelfIndex)

                If netSelf > 0 Then
                    penalty += StabilityWeight * netSelf
                End If
            End If

            penalty += priorPenaltyFactor(p, k)

            Dim f As Double = relative * penalty

            If Double.IsNaN(f) OrElse Double.IsInfinity(f) Then
                Return 1.0E+12
            End If

            Return f
        End Function

        ''' <summary>
        ''' 计算先验知识的乘性惩罚因子：非先验交互的质量占比 + 方向违背量
        ''' </summary>
        ''' <param name="p">参数向量</param>
        ''' <param name="k">候选调控因子的数量</param>
        ''' <returns></returns>
        ''' <remarks>
        ''' 惩罚量全部采用**相对量**（质量占比与平均值）而不是绝对值，
        ''' 这样惩罚强度与候选因子数量无关，也不会随着动力学阶的绝对大小而失控；
        ''' 这一点与本目标函数的「相对误差 × 乘性惩罚」设计是一致的。
        ''' 
        ''' 自环（<c>j = i</c>）总是被视为先验支持：
        ''' 它在 S-system 之中主要承担降解项的自我衰减作用（h_ii &gt; 0），
        ''' 而整理过的转录因子-靶基因数据库一般不会收录这类自我降解关系。
        ''' </remarks>
        Private Function priorPenaltyFactor(p As Double(), k As Integer) As Double
            If PriorMask Is Nothing Then
                Return 0
            End If
            If PriorPenalty <= 0 AndAlso (PriorSign Is Nothing OrElse PriorSignPenalty <= 0) Then
                Return 0
            End If

            Dim mass As Double = 0
            Dim unsupported As Double = 0
            Dim violations As Double = 0
            Dim nSign As Integer = 0

            For c As Integer = 0 To k - 1
                Dim g As Double = p(2 + c)
                Dim h As Double = p(2 + k + c)
                Dim w As Double = std.Abs(g) + std.Abs(h)

                mass += w

                If Not PriorMask(c) Then
                    unsupported += w
                End If

                If Not PriorSign Is Nothing AndAlso PriorSign(c) <> 0 Then
                    Dim net As Double = g - h
                    Dim violation As Double = -PriorSign(c) * net

                    nSign += 1

                    If violation > 0 Then
                        violations += violation
                    End If
                End If
            Next

            Dim factor As Double = 0

            If mass > 1.0E-12 AndAlso PriorPenalty > 0 Then
                factor += PriorPenalty * (unsupported / mass)
            End If
            If nSign > 0 AndAlso PriorSignPenalty > 0 Then
                factor += PriorSignPenalty * (violations / nSign)
            End If

            Return factor
        End Function

        ''' <summary>
        ''' 计算未归一化的斜率残差平方和
        ''' </summary>
        ''' <param name="p"></param>
        ''' <returns></returns>
        Public Function RawSSE(p As Double()) As Double
            Dim m As Integer = Observed.Length
            Dim k As Integer = (p.Length - 2) \ 2
            Dim err As Double = 0

            For t As Integer = 0 To m - 1
                Dim la As Double = p(0)
                Dim lb As Double = p(1)

                For c As Integer = 0 To k - 1
                    Dim lx As Double = LnX(c)(t)

                    la += p(2 + c) * lx
                    lb += p(2 + k + c) * lx
                Next

                Dim d As Double = (std.Exp(clampExp(la)) - std.Exp(clampExp(lb))) - Observed(t)

                err += d * d
            Next

            Return err
        End Function

        ''' <summary>
        ''' 指数运算的溢出保护
        ''' </summary>
        Public Shared Function clampExp(x As Double) As Double
            If Double.IsNaN(x) Then
                Return -700
            ElseIf x > 700 Then
                Return 700
            ElseIf x < -700 Then
                Return -700
            Else
                Return x
            End If
        End Function
    End Class

    ''' <summary>
    ''' The candidate solution of the differential evolution: a real valued parameter vector with box bounds.
    ''' </summary>
    Friend Class ParameterIndividual : Implements IIndividual

        ''' <summary>
        ''' 参数向量
        ''' </summary>
        ''' <returns></returns>
        Public Property Genes As Double()
        ''' <summary>
        ''' 参数下界
        ''' </summary>
        ''' <returns></returns>
        Public Property Lower As Double()
        ''' <summary>
        ''' 参数上界
        ''' </summary>
        ''' <returns></returns>
        Public Property Upper As Double()
        ''' <summary>
        ''' 目标函数
        ''' </summary>
        ''' <returns></returns>
        Public Property Problem As SlopeProblem

        Private rnd As Random

        Public Property MutationRate As Double Implements IIndividual.MutationRate

        Public Function Yield(i As Integer) As Double Implements IIndividual.Yield
            Return Genes(i)
        End Function

        Public Sub Put(i As Integer, value As Double) Implements IIndividual.Put
            Genes(i) = clamp(value, Lower(i), Upper(i))
        End Sub

        Public Function Clone() As Object Implements System.ICloneable.Clone
            Return New ParameterIndividual With {
                .Genes = CType(Genes.Clone(), Double()),
                .Lower = Lower,
                .Upper = Upper,
                .Problem = Problem,
                .MutationRate = MutationRate,
                .rnd = rnd
            }
        End Function

        Private ReadOnly Property Identity As String Implements IIndividual.Identity
            Get
                Dim sb As New StringBuilder()
                Dim stepSize As Integer = std.Max(1, Genes.Length \ 8)

                For i As Integer = 0 To Genes.Length - 1 Step stepSize
                    Call sb.Append(Genes(i).ToString("G6"))
                Next

                Return sb.ToString
            End Get
        End Property

        Public Function Crossover(another As IIndividual) As System.Collections.Generic.IEnumerable(Of IIndividual) Implements IIndividual.Crossover
            Dim b As ParameterIndividual = TryCast(another, ParameterIndividual)

            If b Is Nothing Then
                Return New IIndividual() {}
            End If

            Dim alpha As Double = rnd.NextDouble
            Dim c1 As Double() = New Double(Genes.Length - 1) {}
            Dim c2 As Double() = New Double(Genes.Length - 1) {}

            For i As Integer = 0 To Genes.Length - 1
                Dim x As Double = Genes(i) * alpha + b.Genes(i) * (1 - alpha)
                Dim y As Double = b.Genes(i) * alpha + Genes(i) * (1 - alpha)

                c1(i) = clamp(x, Lower(i), Upper(i))
                c2(i) = clamp(y, Lower(i), Upper(i))
            Next

            Return New IIndividual() {
                newChild(c1), newChild(c2)
            }
        End Function

        Public Function Mutate() As IIndividual Implements IIndividual.Mutate
            Dim g As Double() = CType(Genes.Clone(), Double())

            For i As Integer = 0 To g.Length - 1
                If rnd.NextDouble < MutationRate Then
                    Dim span As Double = Upper(i) - Lower(i)
                    Dim u1 As Double = std.Max(rnd.NextDouble, 1.0E-12)
                    Dim u2 As Double = rnd.NextDouble
                    Dim gauss As Double = std.Sqrt(-2 * std.Log(u1)) * std.Cos(2 * std.PI * u2)

                    g(i) = clamp(g(i) + gauss * span * 0.1, Lower(i), Upper(i))
                End If
            Next

            Return newChild(g)
        End Function

        Private Function newChild(genes As Double()) As ParameterIndividual
            Return New ParameterIndividual With {
                .Genes = genes,
                .Lower = Lower,
                .Upper = Upper,
                .Problem = Problem,
                .MutationRate = MutationRate,
                .rnd = rnd
            }
        End Function

        Private Shared Function clamp(x As Double, lo As Double, hi As Double) As Double
            If Double.IsNaN(x) Then
                Return (lo + hi) / 2
            End If

            Return std.Max(std.Min(x, hi), lo)
        End Function

        ''' <summary>
        ''' 依据上下界创建一个随机的个体
        ''' </summary>
        Public Shared Function Random(lower As Double(), upper As Double(), problem As SlopeProblem, seed As Random) As ParameterIndividual
            Dim g As Double() = New Double(lower.Length - 1) {}

            For i As Integer = 0 To g.Length - 1
                g(i) = lower(i) + (upper(i) - lower(i)) * seed.NextDouble
            Next

            Return New ParameterIndividual With {
                .Genes = g,
                .Lower = lower,
                .Upper = upper,
                .Problem = problem,
                .MutationRate = 0.3,
                .rnd = seed
            }
        End Function

        ''' <summary>
        ''' 依据给定的参数向量创建一个个体（用于继承历史最优解）
        ''' </summary>
        Public Shared Function FromVector(genes As Double(), lower As Double(), upper As Double(), problem As SlopeProblem, seed As Random) As ParameterIndividual
            Dim g As Double() = CType(genes.Clone(), Double())

            For i As Integer = 0 To g.Length - 1
                g(i) = clamp(g(i), lower(i), upper(i))
            Next

            Return New ParameterIndividual With {
                .Genes = g,
                .Lower = lower,
                .Upper = upper,
                .Problem = problem,
                .MutationRate = 0.3,
                .rnd = seed
            }
        End Function
    End Class

    ''' <summary>
    ''' The L-BFGS-B objective adapter of the <see cref="SlopeProblem"/>
    ''' </summary>
    Friend Class SlopeObjective : Inherits IGradFunction

        Public Property Problem As SlopeProblem

        Public Overrides Function evaluate(x As Double()) As Double
            Return Problem.Objective(x)
        End Function
    End Class

    ''' <summary>
    ''' The decoupled S-system parameter estimator: solve the parameters of each gene independently.
    ''' (逐基因解耦的S-system参数估计器)
    ''' </summary>
    ''' <remarks>
    ''' 参看 method.md 的步骤2：每一个基因只含有 2(n+1) 个参数，可以独立于其它基因求解，
    ''' 从而把 O(n^2) 的联合反演问题降解为 n 个低维子问题。
    ''' </remarks>
    Public Class DecoupledFitter

        ReadOnly series As ExpressionTimeSeries
        ReadOnly slopes As Double()()
        ReadOnly options As FittingOptions
        ReadOnly n As Integer
        ReadOnly m As Integer
        ''' <summary>
        ''' 本次拟合所使用的先验知识网络（已经过基因符号映射），为空表示不启用先验约束
        ''' </summary>
        ReadOnly prior As PriorNet

        ''' <summary>
        ''' 创建一个逐基因解耦的拟合器
        ''' </summary>
        ''' <param name="series"></param>
        ''' <param name="slopes">斜率矩阵，由 <see cref="SlopeEstimator"/> 计算得到</param>
        ''' <param name="options"></param>
        Sub New(series As ExpressionTimeSeries, slopes As Double()(), Optional options As FittingOptions = Nothing)
            Me.series = series
            Me.slopes = slopes

            If options Is Nothing Then
                options = New FittingOptions()
            End If

            Me.options = options
            Me.n = series.nGenes
            Me.m = series.nTimePoints

            If options.UsePrior Then
                Me.prior = options.Prior
            End If
        End Sub

        ''' <summary>
        ''' 执行逐基因的参数拟合
        ''' </summary>
        ''' <returns></returns>
        Public Function Fit() As SSystemParameters
            Dim result As New SSystemParameters With {
                .GeneNames = series.GeneNames,
                .OriginalNames = series.OriginalNames,
                .alpha = New Double(n - 1) {},
                .beta = New Double(n - 1) {},
                .G = New Double(n - 1)() {},
                .H = New Double(n - 1)() {},
                .SlopeSSE = New Double(n - 1) {},
                .SlopeRMSE = New Double(n - 1) {},
                .SlopeSST = New Double(n - 1) {},
                .Candidates = New Integer(n - 1)() {},
                .Converged = New Boolean(n - 1) {}
            }

            For i As Integer = 0 To n - 1
                result.G(i) = New Double(n - 1) {}
                result.H(i) = New Double(n - 1) {}
            Next

            Dim indices As Integer() = Enumerable.Range(0, n).ToArray

            If options.Parallel AndAlso n > 1 Then
                Call System.Threading.Tasks.Parallel.ForEach(indices, Sub(i) fitGene(i, result))
            Else
                For Each i As Integer In indices
                    Call fitGene(i, result)
                Next
            End If

            Return result
        End Function

        ''' <summary>
        ''' 筛选出目标基因的候选调控因子；未开启筛选的时候返回全部的基因
        ''' </summary>
        ''' <remarks>
        ''' 默认是软约束模式：保留全部基因作为候选（保留发现新调控关系的可能），
        ''' 先验知识只通过目标函数之中的惩罚项起作用。
        ''' 当 <see cref="FittingOptions.PriorCandidateRestriction"/> 打开的时候才退化为硬约束模式，
        ''' 此时候选因子只允许来自先验知识（并补上自环）；先验为空的时候仍然回退到全部基因。
        ''' </remarks>
        Private Function selectCandidates(i As Integer) As Integer()
            If options.PriorCandidateRestriction AndAlso Not prior Is Nothing Then
                Dim restricted As New System.Collections.Generic.List(Of Integer)

                For Each edge As PriorEdge In prior.RegulatorEdges(series.GeneNames(i))
                    Dim j As Integer = series.GeneIndex(edge.TF)

                    If j >= 0 AndAlso Not restricted.Contains(j) Then
                        Call restricted.Add(j)
                    End If
                Next

                If options.KeepSelfLoop AndAlso Not restricted.Contains(i) Then
                    Call restricted.Add(i)
                End If

                If restricted.Count > 0 Then
                    Return restricted _
                        .OrderBy(Function(j) j) _
                        .ToArray
                End If
            End If

            Dim all As New System.Collections.Generic.List(Of Integer)

            For j As Integer = 0 To n - 1
                If j = i AndAlso Not options.KeepSelfLoop Then
                    Continue For
                End If

                Call all.Add(j)
            Next

            If options.MaxRegulators > 0 AndAlso all.Count > options.MaxRegulators Then
                Dim xi As Double() = series.X(i)

                Return all _
                    .OrderByDescending(Function(j) std.Abs(correlation(xi, series.X(j)))) _
                    .Take(options.MaxRegulators) _
                    .OrderBy(Function(j) j) _
                    .ToArray
            End If

            Return all.ToArray
        End Function

        Private Function correlation(a As Double(), b As Double()) As Double
            Dim ma As Double = a.Average
            Dim mb As Double = b.Average
            Dim sab As Double = 0, saa As Double = 0, sbb As Double = 0

            For k As Integer = 0 To a.Length - 1
                Dim da As Double = a(k) - ma
                Dim db As Double = b(k) - mb

                sab += da * db
                saa += da * da
                sbb += db * db
            Next

            Dim denom As Double = std.Sqrt(saa * sbb)

            If denom < 1.0E-12 Then
                Return 0
            End If

            Return sab / denom
        End Function

        Private Sub fitGene(i As Integer, result As SSystemParameters)
            Dim candidates As Integer() = selectCandidates(i)
            Dim k As Integer = candidates.Length
            Dim observed As Double() = slopes(i)
            Dim mean As Double = observed.Average
            Dim variance As Double = observed.Sum(Function(x) (x - mean) ^ 2) / observed.Length
            Dim priorMask As Boolean() = Nothing
            Dim priorSign As Integer() = Nothing

            If Not prior Is Nothing Then
                Call buildPriorConstraints(i, candidates, priorMask, priorSign)
            End If

            Dim problem As New SlopeProblem With {
                .LnX = candidates _
                    .Select(Function(j) series.LnX(j)) _
                    .ToArray,
                .Observed = observed,
                .Scale = std.Max(std.Sqrt(variance), 1.0E-9),
                .L1 = options.L1Penalty,
                .SelfIndex = Array.IndexOf(candidates, i),
                .StabilityWeight = options.StabilityPenalty,
                .PriorMask = priorMask,
                .PriorSign = If(options.UsePriorSign, priorSign, Nothing),
                .PriorPenalty = options.PriorPenalty,
                .PriorSignPenalty = If(options.UsePriorSign, options.PriorSignPenalty, 0)
            }

            ' 每一个基因都使用与行号相关的确定性随机数种子：
            ' 这样在并行求解的时候各个基因之间不会共享 Random 实例（该类型并非线程安全），
            ' 并且同一个 Seed 配置下多次运行的结果完全一致。
            Dim geneSeed As Integer = If(options.Seed > 0, options.Seed + i * 7919, Environment.TickCount + i * 7919)
            Dim geneRnd As New Random(geneSeed)

            Dim dims As Integer = 2 + 2 * k
            Dim lower As Double() = New Double(dims - 1) {}
            Dim upper As Double() = New Double(dims - 1) {}
            Dim heuristic As Double() = New Double(dims - 1) {}

            Call buildBounds(lower, upper, k)
            Call buildHeuristic(heuristic, candidates, mean, lower, upper, i)

            Dim best As Double() = heuristic
            Dim bestScore As Double = problem.Objective(best)

            ' 阶段一：差分进化全局粗搜
            Try
                Dim de As DifferentialEvolutionResult = DifferentialEvolutionLite.Minimize(
                    target:=Function(p) problem.Objective(p),
                    lower:=lower,
                    upper:=upper,
                    populationSize:=options.DEPopulationSize,
                    maxIterations:=options.DEMaxIterations,
                    F:=options.DEF,
                    CR:=options.DECR,
                    threshold:=options.DEThreshold,
                    seed:=geneRnd.Next())

                If Not de Is Nothing AndAlso Not de.Best Is Nothing Then
                    Dim score As Double = problem.Objective(de.Best)

                    If score < bestScore Then
                        best = de.Best
                        bestScore = score
                    End If
                End If
            Catch ex As Exception
                Call Console.WriteLine($"[WARN] differential evolution failed on gene '{series.GeneNames(i)}': {ex.Message}")
            End Try

            ' 阶段二：L-BFGS-B 盒约束局部精修（分别从历史最优与启发式初值出发）
            For Each start As Double() In New Double()() {best, heuristic}
                Dim refined As Double() = refine(start, problem, lower, upper)

                If refined Is Nothing Then
                    Continue For
                End If

                Dim score As Double = problem.Objective(refined)

                If score < bestScore Then
                    best = refined
                    bestScore = score
                End If
            Next

            ' 回写参数
            result.alpha(i) = std.Exp(best(0))
            result.beta(i) = std.Exp(best(1))

            For c As Integer = 0 To k - 1
                result.G(i)(candidates(c)) = best(2 + c)
                result.H(i)(candidates(c)) = best(2 + k + c)
            Next

            result.SlopeSSE(i) = problem.RawSSE(best)
            result.SlopeRMSE(i) = std.Sqrt(result.SlopeSSE(i) / m)
            result.SlopeSST(i) = variance * m
            result.Candidates(i) = candidates
            result.Converged(i) = bestScore < 1.0E+11

            If options.Verbose Then
                Dim supported As Integer = If(priorMask Is Nothing, 0, priorMask.Where(Function(x) x).Count())

                Call Console.WriteLine($"  [{i + 1}/{n}] {series.GeneNames(i)}: alpha={result.alpha(i).ToString("G4")}, beta={result.beta(i).ToString("G4")}, rmse={result.SlopeRMSE(i).ToString("G4")}, candidates={k}, prior-supported={supported}")
            End If
        End Sub

        ''' <summary>
        ''' 依据先验知识为目标基因的候选调控因子构造软约束掩码与方向约束
        ''' </summary>
        ''' <param name="i">靶基因的行号</param>
        ''' <param name="candidates">候选调控因子的行号</param>
        ''' <param name="mask">先验是否支持该调控关系（自环总是视为支持）</param>
        ''' <param name="sign">先验方向：+1 激活、-1 抑制、0 未知</param>
        Private Sub buildPriorConstraints(i As Integer,
                                          candidates As Integer(),
                                          ByRef mask As Boolean(),
                                          ByRef sign As Integer())
            Dim target As String = series.GeneNames(i)

            mask = New Boolean(candidates.Length - 1) {}
            sign = New Integer(candidates.Length - 1) {}

            For c As Integer = 0 To candidates.Length - 1
                Dim source As String = series.GeneNames(candidates(c))

                ' 自环承担降解项的自我衰减作用，不参与先验约束
                If candidates(c) = i Then
                    mask(c) = True
                    sign(c) = 0
                    Continue For
                End If

                Dim edge As PriorEdge = prior.FindEdge(source, target)

                If Not edge Is Nothing Then
                    mask(c) = True
                    sign(c) = edge.EdgeSign()
                Else
                    mask(c) = False
                    sign(c) = 0
                End If
            Next
        End Sub

        Private Sub buildBounds(lower As Double(), upper As Double(), k As Integer)
            lower(0) = std.Log(std.Max(options.AlphaBounds.Min, 1.0E-6))
            upper(0) = std.Log(std.Max(options.AlphaBounds.Max, 1.0E-5))
            lower(1) = std.Log(std.Max(options.BetaBounds.Min, 1.0E-6))
            upper(1) = std.Log(std.Max(options.BetaBounds.Max, 1.0E-5))

            For c As Integer = 0 To k - 1
                lower(2 + c) = options.GBounds.Min
                upper(2 + c) = options.GBounds.Max
                lower(2 + k + c) = options.HBounds.Min
                upper(2 + k + c) = options.HBounds.Max
            Next
        End Sub

        ''' <summary>
        ''' 构造启发式初值：先用对数域的最小二乘回归给出 alpha/beta 与动力学阶的估计。
        ''' 参看 method.md 的步骤2：在非零数据之上取对数可以把幂律项线性化。
        ''' </summary>
        Private Sub buildHeuristic(h As Double(), candidates As Integer(), mean As Double, lower As Double(), upper As Double(), i As Integer)
            Dim k As Integer = candidates.Length
            Dim positive As New System.Collections.Generic.List(Of Integer)
            Dim negative As New System.Collections.Generic.List(Of Integer)

            For t As Integer = 0 To slopes(i).Length - 1
                Dim s As Double = slopes(i)(t)

                If s > 1.0E-6 Then
                    Call positive.Add(t)
                ElseIf s < -1.0E-6 Then
                    Call negative.Add(t)
                End If
            Next

            ' 合成项的初值：ln(S) = ln(alpha) + sum(g * lnX)
            Dim a0 As Double = If(mean > 0, mean, 0) + 1
            Dim b0 As Double = If(mean < 0, -mean, 0) + 1

            If positive.Count >= k + 1 Then
                Dim coef As Double() = logDomainFit(i, candidates, positive, False)

                If Not coef Is Nothing Then
                    a0 = std.Exp(coef(0))

                    For c As Integer = 0 To k - 1
                        h(2 + c) = clamp(coef(1 + c), lower(2 + c), upper(2 + c))
                    Next
                End If
            ElseIf k > 0 Then
                h(2 + k - 1) = clamp(0, lower(2 + k - 1), upper(2 + k - 1))
            End If

            ' 降解项的初值：ln(-S) = ln(beta) + sum(h * lnX)
            If negative.Count >= k + 1 Then
                Dim coef As Double() = logDomainFit(i, candidates, negative, True)

                If Not coef Is Nothing Then
                    b0 = std.Exp(coef(0))

                    For c As Integer = 0 To k - 1
                        h(2 + k + c) = clamp(coef(1 + c), lower(2 + k + c), upper(2 + k + c))
                    Next
                End If
            ElseIf k > 0 Then
                ' 没有负向的斜率，退化为一阶衰减的先验
                For c As Integer = 0 To k - 1
                    If candidates(c) = i Then
                        h(2 + k + c) = clamp(1.0R, lower(2 + k + c), upper(2 + k + c))
                    End If
                Next
            End If

            h(0) = clamp(std.Log(std.Max(a0, 1.0E-6)), lower(0), upper(0))
            h(1) = clamp(std.Log(std.Max(b0, 1.0E-6)), lower(1), upper(1))
        End Sub

        ''' <summary>
        ''' 在给定的时间点集合之上做对数域的线性回归
        ''' </summary>
        ''' <param name="candidates">候选调控因子</param>
        ''' <param name="rows">参与回归的时间点下标</param>
        ''' <param name="negate">是否对斜率取负（用于降解项的拟合）</param>
        ''' <returns>第一项为截距，其余为回归系数</returns>
        Private Function logDomainFit(gene As Integer, candidates As Integer(), rows As System.Collections.Generic.List(Of Integer), negate As Boolean) As Double()
            Dim nP As Integer = candidates.Length + 1
            Dim nS As Integer = rows.Count
            Dim X As Double(,) = New Double(nS - 1, nP - 1) {}
            Dim y As Double() = New Double(nS - 1) {}

            For r As Integer = 0 To nS - 1
                Dim t As Integer = rows(r)

                X(r, 0) = 1

                For c As Integer = 0 To candidates.Length - 1
                    X(r, 1 + c) = series.LnX(candidates(c))(t)
                Next

                Dim s As Double = slopes(gene)(t)

                y(r) = std.Log(std.Max(If(negate, -s, s), 1.0E-12))
            Next

            Return leastSquares(X, y, nS, nP)
        End Function

        ''' <summary>
        ''' 正规方程 + 高斯消元求解（带岭正则化的）最小二乘问题
        ''' </summary>
        ''' <remarks>
        ''' 时序表达矩阵之中各个基因的对数表达量之间高度共线，正规方程是病态的，
        ''' 故这里通过在对角线之上添加一个很小的岭参数来保证线性方程组可以稳定求解。
        ''' </remarks>
        Private Shared Function leastSquares(X As Double(,), y As Double(), nS As Integer, nP As Integer) As Double()
            Dim A As Double(,) = New Double(nP - 1, nP - 1) {}
            Dim b As Double() = New Double(nP - 1) {}
            Dim ridge As Double = 1.0E-3 * nS

            For i As Integer = 0 To nP - 1
                For j As Integer = i To nP - 1
                    Dim sum As Double = 0

                    For r As Integer = 0 To nS - 1
                        sum += X(r, i) * X(r, j)
                    Next

                    A(i, j) = sum
                    A(j, i) = sum
                Next

                If i > 0 Then
                    A(i, i) += ridge
                End If

                Dim sy As Double = 0

                For r As Integer = 0 To nS - 1
                    sy += X(r, i) * y(r)
                Next

                b(i) = sy
            Next

            ' 高斯消元（部分主元）
            For col As Integer = 0 To nP - 1
                Dim pivot As Integer = col
                Dim maxAbs As Double = std.Abs(A(col, col))

                For r As Integer = col + 1 To nP - 1
                    If std.Abs(A(r, col)) > maxAbs Then
                        maxAbs = std.Abs(A(r, col))
                        pivot = r
                    End If
                Next

                If maxAbs < 1.0E-12 Then
                    Return Nothing
                End If

                If pivot <> col Then
                    For j As Integer = 0 To nP - 1
                        Dim tmp As Double = A(col, j)

                        A(col, j) = A(pivot, j)
                        A(pivot, j) = tmp
                    Next

                    Dim tb As Double = b(col)

                    b(col) = b(pivot)
                    b(pivot) = tb
                End If

                For r As Integer = col + 1 To nP - 1
                    Dim factor As Double = A(r, col) / A(col, col)

                    For j As Integer = col To nP - 1
                        A(r, j) -= factor * A(col, j)
                    Next

                    b(r) -= factor * b(col)
                Next
            Next

            ' 回代
            Dim coef As Double() = New Double(nP - 1) {}

            For r As Integer = nP - 1 To 0 Step -1
                Dim sum As Double = b(r)

                For j As Integer = r + 1 To nP - 1
                    sum -= A(r, j) * coef(j)
                Next

                coef(r) = sum / A(r, r)

                If Double.IsNaN(coef(r)) OrElse Double.IsInfinity(coef(r)) Then
                    Return Nothing
                End If
            Next

            Return coef
        End Function

        Private Function refine(x0 As Double(), problem As SlopeProblem, lower As Double(), upper As Double()) As Double()
            Try
                Dim solver As New LBFGSB(New Parameters() With {
                    .max_iterations = options.LBFGSIterations
                })

                Return solver.minimize(New SlopeObjective With {.Problem = problem}, x0, lower, upper)
            Catch ex As Exception
                Return Nothing
            End Try
        End Function

        Private Shared Function clamp(x As Double, lo As Double, hi As Double) As Double
            If Double.IsNaN(x) Then
                Return (lo + hi) / 2
            End If

            Return std.Max(std.Min(x, hi), lo)
        End Function
    End Class
End Namespace
