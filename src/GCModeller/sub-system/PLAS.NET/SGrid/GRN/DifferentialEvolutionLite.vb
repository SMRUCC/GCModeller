Imports Microsoft.VisualBasic.Linq
Imports std = System.Math

Namespace GRN

    ''' <summary>
    ''' 差分进化的求解结果
    ''' </summary>
    Public Class DifferentialEvolutionResult

        ''' <summary>
        ''' 最优的参数向量
        ''' </summary>
        ''' <returns></returns>
        Public Property Best As Double()
        ''' <summary>
        ''' 最优的目标函数值
        ''' </summary>
        ''' <returns></returns>
        Public Property Fitness As Double
        ''' <summary>
        ''' 实际执行的迭代次数
        ''' </summary>
        ''' <returns></returns>
        Public Property Iterations As Integer
        ''' <summary>
        ''' 目标函数的求值次数
        ''' </summary>
        ''' <returns></returns>
        Public Property Evaluations As Long

        Public Overrides Function ToString() As String
            Return $"DE: fitness={Fitness.ToString("G6")}, iterations={Iterations}, evaluations={Evaluations}"
        End Function
    End Class

    ''' <summary>
    ''' A compact self contained differential evolution implementation (rand/1/bin).
    ''' (自包含的紧凑差分进化实现)
    ''' </summary>
    ''' <remarks>
    ''' 之所以在这里自行实现而不再调用 <c>MachineLearning\Darwinism\DifferentialEvolution.Evolution</c>：
    ''' 该函数的目标函数形参声明为 <c>Func(Of Individual, Double)</c>，但内部会将它强转为
    ''' <c>GAF.Fitness(Of Individual)</c> 委托，两个委托的签名不一致，运行时必然抛出
    ''' <c>InvalidCastException</c>，故无法在该场景下直接使用。
    ''' 本实现遵循标准的 DE/rand/1/bin 策略，语义与 <c>DifferentialEvolution.Evolution</c> 一致。
    ''' </remarks>
    Public Module DifferentialEvolutionLite

        ''' <summary>
        ''' 在给定的上下界之内最小化目标函数
        ''' </summary>
        ''' <param name="target">目标函数，越小越好</param>
        ''' <param name="lower">参数下界</param>
        ''' <param name="upper">参数上界</param>
        ''' <param name="populationSize">种群大小</param>
        ''' <param name="maxIterations">最大迭代次数</param>
        ''' <param name="F">差分权重，取值区间 [0,2]</param>
        ''' <param name="CR">交叉概率，取值区间 [0,1]</param>
        ''' <param name="threshold">收敛阈值</param>
        ''' <param name="seed">随机数种子，零表示使用系统种子</param>
        ''' <returns></returns>
        Public Function Minimize(target As Func(Of Double(), Double),
                                 lower As Double(),
                                 upper As Double(),
                                 Optional populationSize As Integer = 60,
                                 Optional maxIterations As Integer = 3000,
                                 Optional F As Double = 0.8R,
                                 Optional CR As Double = 0.7R,
                                 Optional threshold As Double = 0.0001R,
                                 Optional seed As Integer = 0) As DifferentialEvolutionResult

            If lower Is Nothing OrElse upper Is Nothing Then
                Throw New ArgumentNullException(NameOf(lower))
            End If

            Dim dims As Integer = lower.Length

            If dims = 0 Then
                Throw New ArgumentException("The dimension of the target problem should not be zero!")
            End If

            If populationSize < 4 Then
                populationSize = 4
            End If

            Dim rnd As Random = If(seed <> 0, New Random(seed), New Random())
            Dim population As Double()() = New Double(populationSize - 1)() {}
            Dim fitness As Double() = New Double(populationSize - 1) {}
            Dim evaluations As Long = 0
            Dim bestIndex As Integer = 0

            For i As Integer = 0 To populationSize - 1
                population(i) = randomVector(lower, upper, rnd)
                fitness(i) = target(population(i))
                evaluations += 1

                If fitness(i) < fitness(bestIndex) Then
                    bestIndex = i
                End If
            Next

            Dim iterations As Integer = 0

            For iteration As Integer = 1 To maxIterations
                iterations = iteration

                For i As Integer = 0 To populationSize - 1
                    ' rand/1/bin
                    Dim r1 As Integer = pickExcept(rnd, populationSize, i)
                    Dim r2 As Integer = pickExcept(rnd, populationSize, i, r1)
                    Dim r3 As Integer = pickExcept(rnd, populationSize, i, r1, r2)
                    Dim trial As Double() = New Double(dims - 1) {}
                    Dim forceIndex As Integer = rnd.Next(dims)

                    For d As Integer = 0 To dims - 1
                        If d = forceIndex OrElse rnd.NextDouble < CR Then
                            Dim value As Double = population(r1)(d) + F * (population(r2)(d) - population(r3)(d))

                            trial(d) = std.Max(std.Min(value, upper(d)), lower(d))
                        Else
                            trial(d) = population(i)(d)
                        End If
                    Next

                    Dim score As Double = target(trial)
                    evaluations += 1

                    If score < fitness(i) Then
                        population(i) = trial
                        fitness(i) = score

                        If score < fitness(bestIndex) Then
                            bestIndex = i
                        End If
                    End If
                Next

                If fitness(bestIndex) <= threshold Then
                    Exit For
                End If
            Next

            Return New DifferentialEvolutionResult With {
                .Best = CType(population(bestIndex).Clone(), Double()),
                .Fitness = fitness(bestIndex),
                .Iterations = iterations,
                .Evaluations = evaluations
            }
        End Function

        Private Function randomVector(lower As Double(), upper As Double(), rnd As Random) As Double()
            Dim v As Double() = New Double(lower.Length - 1) {}

            For i As Integer = 0 To v.Length - 1
                v(i) = lower(i) + (upper(i) - lower(i)) * rnd.NextDouble
            Next

            Return v
        End Function

        Private Function pickExcept(rnd As Random, size As Integer, ParamArray excludes As Integer()) As Integer
            Dim value As Integer

            Do
                value = rnd.Next(size)
            Loop While excludes.Contains(value)

            Return value
        End Function
    End Module
End Namespace
