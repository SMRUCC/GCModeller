Imports System

Namespace Config

    ''' <summary>
    ''' 稳态求解策略。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 之所以要区分「求根」与「时间推进」：静态路线学到的是 <c>x_g = F(regulators)</c> 的回归面，
    ''' 它的正确提法是解 <c>r(z) = F(z) - z = 0</c>（求根），而不是把它当时间步反复施加；
    ''' 滞后路线学到的是 <c>x(t+1) = F(x(t))</c>，那才是一个离散动力系统，只能推进并观察吸引子。
    ''' 把两者混在同一个「阻尼迭代 + maxDelta」里，是此前收敛判据无法自洽的根源之一。
    ''' </para>
    ''' <para>
    ''' 默认 <see cref="SolverMode.Auto"/>：静态路线走 <see cref="SolverMode.Newton"/>，
    ''' 滞后路线走 <see cref="SolverMode.Advance"/>。
    ''' </para>
    ''' </remarks>
    Public Enum SolverMode
        ''' <summary>按网络路线自动选择：静态 → Newton，滞后 → Advance。</summary>
        Auto = 0
        ''' <summary>带解析 Jacobian 的 Levenberg–Marquardt / 牛顿求根。</summary>
        Newton = 1
        ''' <summary>Anderson 加速的阻尼迭代（无法建立 Jacobian 时的次优选择）。</summary>
        AndersonDamped = 2
        ''' <summary>旧行为：逐帧阻尼迭代 + 脏位优化（保留用于回归对照）。</summary>
        LegacyDamped = 3
        ''' <summary>时间推进：只产出轨迹与吸引子，不承诺不动点。</summary>
        Advance = 4
    End Enum

    ''' <summary>
    ''' 稳态求解器配置。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 全部参数都只影响「怎么解」，不影响「解什么」：<c>F</c> 由逐靶基因模型决定，
    ''' 因此换求解器不会改变模型语义，只会改变同一组方程被求解得稳不稳、快不快。
    ''' </para>
    ''' <para>
    ''' <see cref="ClampInputToSupport"/> 与 <see cref="ClampOutputToSupport"/> 是外推防线：
    ''' 虚拟敲除会把输入推到训练支持域之外，此时高斯前件几乎全部失活、
    ''' 结论项退化成最外侧规则的无界线性外推。输入侧夹断保证「模型只在见过的工作区间里被询问」。
    ''' </para>
    ''' </remarks>
    Public Class SolverOptions

        ''' <summary>求解策略。</summary>
        Public Property Mode As SolverMode = SolverMode.Auto

        ''' <summary>
        ''' 牛顿/LM 的最大迭代轮数（每轮一次 Jacobian + 一次或多次线搜索）。
        ''' </summary>
        ''' <remarks>
        ''' 与 <see cref="FuzzyGeneConfig.PropagationSteps"/> 不同：后者是旧阻尼迭代的帧预算，
        ''' 求根模式下它只作为回落链（Anderson / LegacyDamped）的步数预算。
        ''' </remarks>
        Public Property MaxNewtonIterations As Integer = 20

        ''' <summary>每轮 Newton 内允许的 λ（LM 阻尼）上调次数。</summary>
        Public Property MaxLambdaRetries As Integer = 6

        ''' <summary>LM 阻尼的初始值。</summary>
        Public Property Lambda0 As Double = 0.001

        ''' <summary>步长被接受时 λ 的缩小倍数。</summary>
        Public Property LambdaDown As Double = 3.0

        ''' <summary>步长被拒绝时 λ 的放大倍数。</summary>
        Public Property LambdaUp As Double = 10.0

        ''' <summary>回溯线搜索的最大折半次数。</summary>
        Public Property LineSearchSteps As Integer = 12

        ''' <summary>Anderson 加速的历史深度（帧数）。</summary>
        Public Property AndersonDepth As Integer = 5

        ''' <summary>
        ''' 稠密求解的基因数上限；超过则改用「稀疏 matvec + BiCGStab」的矩阵无关实现。
        ''' </summary>
        ''' <remarks>
        ''' 稠密矩阵占 <c>N²·8</c> 字节：1024 基因约 8.4 MB，2048 基因约 33.5 MB，
        ''' 再往上就该换 Krylov 了。
        ''' </remarks>
        Public Property DenseThreshold As Integer = 1024

        ''' <summary>BiCGStab 的最大迭代次数。</summary>
        Public Property KrylovMaxIterations As Integer = 60

        ''' <summary>BiCGStab 的相对残差阈值。</summary>
        Public Property KrylovTolerance As Double = 0.000001

        ''' <summary>判定发散的倍数：残差超过初始残差该倍数即中止。</summary>
        Public Property DivergenceFactor As Double = 1000.0

        ''' <summary>极限环检测窗口（帧数）；0 表示不做循环检测。</summary>
        Public Property CycleWindow As Integer = 16

        ''' <summary>谱半径幂迭代的最大次数。</summary>
        Public Property SpectralPowerIterations As Integer = 20

        ''' <summary>
        ''' 是否把输入（调控因子取值）夹断到训练支持域
        ''' <c>[TskInputVariable.Minimum, Maximum]</c>。
        ''' </summary>
        ''' <remarks>
        ''' 这是虚拟扰动的外推防线：不夹断时，敲除基因的 z 值可能落到 -5 甚至 -10，
        ''' 高斯前件全部失活，输出由最外侧规则线性外推，数值上等价于「问模型一个它从没见过的问题」。
        ''' </remarks>
        Public Property ClampInputToSupport As Boolean = True

        ''' <summary>是否把输出（靶基因预测）夹断到 <c>[SupportMin-1, SupportMax+1]</c>。</summary>
        Public Property ClampOutputToSupport As Boolean = True

        ''' <summary>
        ''' 是否启用脏位优化（只重算调控因子发生变化的基因）。
        ''' </summary>
        ''' <remarks>
        ''' 求根模式需要对整个状态求同一时刻的 <c>F</c>，脏位优化会让「没变化的基因」保留旧值，
        ''' 在数学上仍然是同一个不动点（因为旧值等于新值），因此可以安全保留；
        ''' 但把它关掉能得到最直白的语义，调试时有用。
        ''' </remarks>
        Public Property EnableDirtySet As Boolean = True

        ''' <summary>逐属性校验并返回自身。</summary>
        Public Function Validate() As SolverOptions
            If MaxNewtonIterations < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(MaxNewtonIterations), MaxNewtonIterations, "the Newton iteration budget must be positive")
            End If
            If MaxLambdaRetries < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(MaxLambdaRetries), MaxLambdaRetries, "the lambda retry count must not be negative")
            End If
            If Lambda0 <= 0.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(Lambda0), Lambda0, "the initial lambda must be positive")
            End If
            If LambdaDown <= 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(LambdaDown), LambdaDown, "the lambda down factor must be greater than 1")
            End If
            If LambdaUp <= 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(LambdaUp), LambdaUp, "the lambda up factor must be greater than 1")
            End If
            If LineSearchSteps < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(LineSearchSteps), LineSearchSteps, "the line search must have at least one step")
            End If
            If AndersonDepth < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(AndersonDepth), AndersonDepth, "the Anderson depth must not be negative")
            End If
            If DenseThreshold < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(DenseThreshold), DenseThreshold, "the dense threshold must be positive")
            End If
            If KrylovMaxIterations < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(KrylovMaxIterations), KrylovMaxIterations, "the Krylov iteration budget must be positive")
            End If
            If KrylovTolerance <= 0.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(KrylovTolerance), KrylovTolerance, "the Krylov tolerance must be positive")
            End If
            If DivergenceFactor <= 1.0 Then
                Throw New ArgumentOutOfRangeException(NameOf(DivergenceFactor), DivergenceFactor, "the divergence factor must be greater than 1")
            End If
            If CycleWindow < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(CycleWindow), CycleWindow, "the cycle window must not be negative")
            End If
            If SpectralPowerIterations < 1 Then
                Throw New ArgumentOutOfRangeException(NameOf(SpectralPowerIterations), SpectralPowerIterations, "the power iteration count must be positive")
            End If

            Return Me
        End Function

        ''' <summary>浅拷贝（本类型没有数组字段）。</summary>
        Public Function Clone() As SolverOptions
            Return DirectCast(Me.MemberwiseClone(), SolverOptions)
        End Function

        ''' <summary>单行摘要（用于日志与训练汇总）。</summary>
        Public Overrides Function ToString() As String
            Return $"{Mode}: newton<={MaxNewtonIterations}, dense<={DenseThreshold}, anderson={AndersonDepth}, " &
                   $"krylov<={KrylovMaxIterations}, cycles<={CycleWindow}, " &
                   $"clamp in/out={ClampInputToSupport}/{ClampOutputToSupport}"
        End Function

    End Class

End Namespace
