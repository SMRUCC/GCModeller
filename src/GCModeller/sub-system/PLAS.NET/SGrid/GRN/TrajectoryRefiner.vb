Imports Microsoft.VisualBasic.Linq
Imports Microsoft.VisualBasic.Math.Calculus.Dynamics.Data
Imports Microsoft.VisualBasic.Math.Framework.Optimization.LBFGSB
Imports SSOdes = SMRUCC.genomics.Analysis.SSystem.Kernel.ODEs
Imports std = System.Math

Namespace GRN

    ''' <summary>
    ''' The objective function of the trajectory refinement of one single gene.
    ''' (单个基因的轨迹拟合目标函数)
    ''' </summary>
    ''' <remarks>
    ''' 与斜率拟合不同，这里会对整个 ODE 系统做数值积分，然后比较模拟轨迹与观测轨迹之间的残差，
    ''' 因而可以修正斜率估计阶段的数值微分误差。
    ''' </remarks>
    Friend Class TrajectoryProblem : Inherits IGradFunction

        ''' <summary>
        ''' 当前的参数集合
        ''' </summary>
        ''' <returns></returns>
        Public Property Parameters As SSystemParameters
        ''' <summary>
        ''' 时序数据
        ''' </summary>
        ''' <returns></returns>
        Public Property Series As ExpressionTimeSeries
        ''' <summary>
        ''' 本次待优化的靶基因行号
        ''' </summary>
        ''' <returns></returns>
        Public Property GeneIndex As Integer
        ''' <summary>
        ''' 参与优化的候选调控因子
        ''' </summary>
        ''' <returns></returns>
        Public Property Candidates As Integer()
        ''' <summary>
        ''' 仿真配置
        ''' </summary>
        ''' <returns></returns>
        Public Property Options As FittingOptions

        ''' <summary>
        ''' 目标函数的调用计数器，用于防止轨迹精修阶段耗时失控
        ''' </summary>
        ''' <returns></returns>
        Public Property Evaluations As Long

        ''' <summary>
        ''' 参数向量的维度
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property Dims As Integer
            Get
                Return 2 + 2 * Candidates.Length
            End Get
        End Property

        ''' <summary>
        ''' 把参数向量写回到参数集合之中
        ''' </summary>
        ''' <param name="p"></param>
        Public Sub Apply(p As Double())
            Dim k As Integer = Candidates.Length
            Dim i As Integer = GeneIndex

            Parameters.alpha(i) = std.Exp(If(p(0) > 700, 700, p(0)))
            Parameters.beta(i) = std.Exp(If(p(1) > 700, 700, p(1)))

            For c As Integer = 0 To k - 1
                Parameters.G(i)(Candidates(c)) = p(2 + c)
                Parameters.H(i)(Candidates(c)) = p(2 + k + c)
            Next
        End Sub

        ''' <summary>
        ''' 从当前参数之中打包出目标基因的参数向量
        ''' </summary>
        ''' <returns></returns>
        Public Function Pack() As Double()
            Dim k As Integer = Candidates.Length
            Dim i As Integer = GeneIndex
            Dim p As Double() = New Double(Dims - 1) {}

            p(0) = std.Log(std.Max(Parameters.alpha(i), 1.0E-6))
            p(1) = std.Log(std.Max(Parameters.beta(i), 1.0E-6))

            For c As Integer = 0 To k - 1
                p(2 + c) = Parameters.G(i)(Candidates(c))
                p(2 + k + c) = Parameters.H(i)(Candidates(c))
            Next

            Return p
        End Function

        Public Overrides Function evaluate(x As Double()) As Double
            Evaluations += 1

            Dim backup As SSystemParameters = Parameters.Clone()

            Try
                Call Apply(x)

                Dim simulated As Double() = simulate(Parameters, Series, Options)

                If simulated Is Nothing Then
                    Return 1.0E+12
                End If

                Dim obs As Double() = Series.X(GeneIndex)
                Dim mean As Double = obs.Average
                Dim variance As Double = obs.Sum(Function(v) (v - mean) ^ 2) / obs.Length
                Dim scale As Double = obs.Length * std.Max(variance, 1.0E-9)
                Dim err As Double = 0

                For k As Integer = 0 To obs.Length - 1
                    Dim d As Double = simulated(k) - obs(k)

                    err += d * d
                Next

                Dim f As Double = err / scale

                If Double.IsNaN(f) OrElse Double.IsInfinity(f) Then
                    Return 1.0E+12
                End If

                Return f
            Catch ex As Exception
                Return 1.0E+12
            Finally
                ' 恢复到调用之前的参数状态：目标函数必须是纯函数
                Call restore(Parameters, backup)
            End Try
        End Function

        ''' <summary>
        ''' 积分当前的模型，并返回靶基因在观测时间点之上的模拟值
        ''' </summary>
        Private Shared Function simulate(parameters As SSystemParameters, series As ExpressionTimeSeries, options As FittingOptions) As Double()
            Dim net As New SSystemNetwork With {
                .Series = series,
                .Parameters = parameters,
                .Options = options
            }
            Dim model = net.CreateModel(finalTime:=series.TimeEnd)
            Dim runOptions = options.CreateRunOptions(series.TimeEnd)
            Dim output As ODEsOut = SSOdes.RunSystem(model, Nothing, runOptions)

            Return interpolate(output, parameters, series, parameters.GeneNames.Length - 1)
        End Function

        ''' <summary>
        ''' 把等间距的仿真结果线性插值到观测时间点之上
        ''' </summary>
        Private Shared Function interpolate(output As ODEsOut,
                                            parameters As SSystemParameters,
                                            series As ExpressionTimeSeries,
                                            gene As Integer) As Double()

            Dim name As String = parameters.GeneNames(gene)

            If output Is Nothing OrElse output.y Is Nothing OrElse Not output.y.ContainsKey(name) Then
                Return Nothing
            End If

            Dim x As Double() = output.x
            Dim y As Double() = output.y(name).Value
            Dim t As Double() = series.TimePoints
            Dim out As Double() = New Double(t.Length - 1) {}

            If x Is Nothing OrElse x.Length < 2 Then
                Return Nothing
            End If

            Dim cursor As Integer = 0

            For k As Integer = 0 To t.Length - 1
                Dim tk As Double = t(k)

                If tk <= x(0) Then
                    out(k) = y(0)
                    Continue For
                End If
                If tk >= x(x.Length - 1) Then
                    out(k) = y(y.Length - 1)
                    Continue For
                End If

                While cursor < x.Length - 2 AndAlso x(cursor + 1) < tk
                    cursor += 1
                End While

                Dim t0 As Double = x(cursor)
                Dim t1 As Double = x(cursor + 1)
                Dim dt As Double = t1 - t0

                If dt <= 0 Then
                    out(k) = y(cursor)
                Else
                    out(k) = y(cursor) + (y(cursor + 1) - y(cursor)) * (tk - t0) / dt
                End If

                If Double.IsNaN(out(k)) OrElse Double.IsInfinity(out(k)) Then
                    Return Nothing
                End If
            Next

            Return out
        End Function

        Private Shared Sub restore(target As SSystemParameters, backup As SSystemParameters)
            For i As Integer = 0 To target.nGenes - 1
                target.alpha(i) = backup.alpha(i)
                target.beta(i) = backup.beta(i)

                For j As Integer = 0 To target.nGenes - 1
                    target.G(i)(j) = backup.G(i)(j)
                    target.H(i)(j) = backup.H(i)(j)
                Next
            Next
        End Sub
    End Class

    ''' <summary>
    ''' The phase 2 parameter refinement by fitting the whole ODE trajectory.
    ''' (第二阶段的轨迹精修)
    ''' </summary>
    ''' <remarks>
    ''' 参看 method.md 的步骤4：先用全局优化确定拓扑与粗略的参数，
    ''' 再通过整段积分的轨迹残差做局部精修，以提升模型对未见时间点的预测能力。
    ''' 这里采用按基因分块的坐标下降方式，每次只优化一个基因的动力学参数。
    ''' </remarks>
    Public Module TrajectoryRefiner

        ''' <summary>
        ''' 对已经完成了稀疏化的参数集合做轨迹精修
        ''' </summary>
        ''' <param name="series"></param>
        ''' <param name="parameters">已经完成斜率拟合与稀疏化的参数集合</param>
        ''' <param name="options"></param>
        ''' <returns></returns>
        Public Function Refine(series As ExpressionTimeSeries,
                               parameters As SSystemParameters,
                               Optional options As FittingOptions = Nothing) As SSystemParameters

            If options Is Nothing Then
                options = New FittingOptions()
            End If

            Dim result As SSystemParameters = parameters.Clone()
            Dim n As Integer = result.nGenes
            Dim maxIterations As Integer = std.Max(1, std.Min(options.RefineMaxIterations, 50))

            For i As Integer = 0 To n - 1
                ' 只优化稀疏化之后仍然保留下来的交互
                Dim candidates As Integer() = Enumerable _
                    .Range(0, n) _
                    .Where(Function(j) result.G(i)(j) <> 0 OrElse result.H(i)(j) <> 0) _
                    .ToArray

                If candidates.Length = 0 Then
                    Continue For
                End If

                Dim problem As New TrajectoryProblem With {
                    .Parameters = result,
                    .Series = series,
                    .GeneIndex = i,
                    .Candidates = candidates,
                    .Options = options
                }

                Dim dims As Integer = problem.Dims
                Dim lower As Double() = New Double(dims - 1) {}
                Dim upper As Double() = New Double(dims - 1) {}

                lower(0) = std.Log(std.Max(options.AlphaBounds.Min, 1.0E-6))
                upper(0) = std.Log(std.Max(options.AlphaBounds.Max, 1.0E-5))
                lower(1) = std.Log(std.Max(options.BetaBounds.Min, 1.0E-6))
                upper(1) = std.Log(std.Max(options.BetaBounds.Max, 1.0E-5))

                For c As Integer = 0 To candidates.Length - 1
                    lower(2 + c) = options.GBounds.Min
                    upper(2 + c) = options.GBounds.Max
                    lower(2 + candidates.Length + c) = options.HBounds.Min
                    upper(2 + candidates.Length + c) = options.HBounds.Max
                Next

                Dim x0 As Double() = problem.Pack()

                Try
                    Dim solver As New LBFGSB(New Parameters() With {
                        .max_iterations = maxIterations
                    })
                    Dim x As Double() = solver.minimize(problem, x0, lower, upper)

                    If Not x Is Nothing Then
                        Call problem.Apply(x)
                    End If
                Catch ex As Exception
                    Call Console.WriteLine($"[WARN] trajectory refinement failed on gene '{result.GeneNames(i)}': {ex.Message}")
                End Try

                If options.Verbose Then
                    Call Console.WriteLine($"  [refine {i + 1}/{n}] {result.GeneNames(i)}: alpha={result.alpha(i).ToString("G4")}, beta={result.beta(i).ToString("G4")}, evaluations={problem.Evaluations}")
                End If
            Next

            Return result
        End Function
    End Module
End Namespace
