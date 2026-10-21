Imports System.Text
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.MachineLearning.Darwinism
Imports Microsoft.VisualBasic.Math.Framework.Optimization.LBFGSB
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
    Friend Class SlopeProblem

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
        ''' 计算归一化的目标函数值（越小越好）
        ''' </summary>
        ''' <param name="p">参数向量 [ln(alpha), ln(beta), g(0..k-1), h(0..k-1)]</param>
        ''' <returns></returns>
        Public Function Objective(p As Double()) As Double
            Dim m As Integer = Observed.Length
            Dim k As Integer = (p.Length - 2) \ 2
            Dim err As Double = 0
            Dim penalty As Double = 0

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

            For c As Integer = 0 To k - 1
                penalty += std.Abs(p(2 + c)) + std.Abs(p(2 + k + c))
            Next

            Dim f As Double = err / Scale + L1 * penalty

            If Double.IsNaN(f) OrElse Double.IsInfinity(f) Then
                Return 1.0E+12
            End If

            Return f
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
        ReadOnly rnd As Random

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
            Me.rnd = If(options.Seed > 0, New Random(options.Seed), New Random())
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
        Private Function selectCandidates(i As Integer) As Integer()
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

            Dim problem As New SlopeProblem With {
                .LnX = candidates _
                    .Select(Function(j) series.LnX(j)) _
                    .ToArray,
                .Observed = observed,
                .Scale = m * std.Max(variance, 1.0E-9),
                .L1 = options.L1Penalty
            }

            Dim dims As Integer = 2 + 2 * k
            Dim lower As Double() = New Double(dims - 1) {}
            Dim upper As Double() = New Double(dims - 1) {}
            Dim heuristic As Double() = New Double(dims - 1) {}

            Call buildBounds(lower, upper, k)
            Call buildHeuristic(heuristic, candidates, mean, lower, upper, i)

            Dim best As Double() = heuristic
            Dim bestScore As Double = problem.Objective(best)

            ' 阶段一：差分进化全局粗搜
            Dim factory As [New](Of ParameterIndividual) =
                Function(s As Random) ParameterIndividual.Random(lower, upper, problem, s)
            Dim evaluate As Func(Of ParameterIndividual, Double) =
                Function(ind) problem.Objective(ind.Genes)

            Try
                Dim de As ParameterIndividual = DifferentialEvolution.Evolution(
                    target:=evaluate,
                    [new]:=factory,
                    N:=dims,
                    F:=options.DEF,
                    CR:=options.DECR,
                    threshold:=options.DEThreshold,
                    maxIterations:=options.DEMaxIterations,
                    PopulationSize:=options.DEPopulationSize,
                    iteratePrints:=Sub(o)
                                       ' 静默：逐基因的迭代报告会淹没控制台
                                   End Sub,
                    parallel:=False)

                If Not de Is Nothing Then
                    Dim score As Double = problem.Objective(de.Genes)

                    If score < bestScore Then
                        best = de.Genes
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
            result.Converged(i) = bestScore < 1.0E+11

            If options.Verbose Then
                Call Console.WriteLine($"  [{i + 1}/{n}] {series.GeneNames(i)}: alpha={result.alpha(i).ToString("G4")}, beta={result.beta(i).ToString("G4")}, rmse={result.SlopeRMSE(i).ToString("G4")}")
            End If
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
        ''' 构造启发式初值：alpha - beta*X_i，即一阶衰减的先验形式
        ''' </summary>
        Private Sub buildHeuristic(h As Double(), candidates As Integer(), mean As Double, lower As Double(), upper As Double(), i As Integer)
            Dim a0 As Double = If(mean > 0, mean, 0) + 1
            Dim b0 As Double = If(mean < 0, -mean, 0) + 1

            h(0) = clamp(std.Log(a0), lower(0), upper(0))
            h(1) = clamp(std.Log(b0), lower(1), upper(1))

            For c As Integer = 0 To candidates.Length - 1
                If candidates(c) = i Then
                    ' 降解项对自身的一阶依赖是一个合理的先验
                    h(2 + candidates.Length + c) = clamp(1.0R, lower(2 + candidates.Length + c), upper(2 + candidates.Length + c))
                End If
            Next
        End Sub

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
