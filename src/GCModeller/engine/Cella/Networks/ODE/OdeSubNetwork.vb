' ============================================================
' OdeSubNetwork.vb - 基于 CVODE 的常微分方程子系统基类
' ============================================================
' 翻译、周转、信号转导三个子系统都是典型的刚性系统（速率常数跨越数个
' 数量级），统一用 Sundials CVODE 的 BDF（最高 5 阶）求解。
'
' 约定：
'   1. 子类实现 RHS(t, y, ydot) 给出方程右端；可选择性重写 Jacobian 提供
'      解析雅可比，显著减少 Newton 迭代次数。
'   2. 求解器在构造时创建、Initialize 一次，之后只做 Integrate；不每步 new。
'   3. Advance(dt) 返回 False 表示求解失败，调用方（子类）应回退到上一状态，
'      而不是静默继续——否则一个发散点会污染整个培养环境。
' ============================================================

Imports Microsoft.VisualBasic.Math.Sundials.CVODE

Public MustInherit Class OdeSubNetwork : Inherits SubNetwork
    Implements IDisposable

    Protected ReadOnly solver As CVODESolver
    Protected ReadOnly n As Integer
    Protected ReadOnly y As NVector

    ''' <summary>子系统内部累积时间</summary>
    Protected t As Double = 0.0

    Private disposed As Boolean = False

    ''' <summary>每累计这么多步就重启一次求解器，避免内部步数计数器溢出</summary>
    Protected Property RebaseThreshold As Integer = 5000

    ''' <summary>最近一次推进的 CVODE 返回状态（诊断用）</summary>
    Public ReadOnly Property LastStatus As CVODEStatus
        Get
            Return _lastStatus
        End Get
    End Property

    Private _lastStatus As CVODEStatus = CVODEStatus.Success

    Sub New(cell As VirtualCella,
            dimension As Integer,
            Optional name As String = Nothing,
            Optional relTol As Double = 0.000001,
            Optional absTol As Double = 0.000000001,
            Optional withJacobian As Boolean = True)

        Call MyBase.New(cell, name)

        If dimension <= 0 Then
            Throw New ArgumentException("ODE 系统维度必须为正数", NameOf(dimension))
        End If

        n = dimension
        y = New NVector(n)

        Dim options As New CVODEOptions With {
            .RelativeTolerance = relTol,
            .AbsoluteTolerance = absTol,
            .MaxOrder = 5,
            .MaxNewtonIterations = 100
        }

        solver = New CVODESolver(CVODEMethod.BDF, AddressOf RHS, n, options)

        If withJacobian Then
            Call solver.SetJacobianFunction(AddressOf Jacobian)
        End If
    End Sub

    ' ==================== 子类必须 / 可选实现 ====================

    ''' <summary>方程右端：把 dx/dt 写入 <paramref name="ydot"/></summary>
    Protected MustOverride Sub RHS(t As Double, y As NVector, ydot As NVector)

    ''' <summary>解析雅可比；默认不提供（CVODE 将退回有限差分）</summary>
    Protected Overridable Sub Jacobian(t As Double, y As NVector, fy As NVector, J As DenseMatrix)
        ' 默认实现留空：不使用解析雅可比时不要调用 SetJacobianFunction
    End Sub

    ' ==================== 生命周期 ====================

    ''' <summary>
    ''' 用给定的初值初始化求解器（每个细胞只需调用一次）
    ''' </summary>
    Protected Sub InitializeFrom(initial As Double())
        If initial Is Nothing OrElse initial.Length <> n Then
            Throw New ArgumentException($"初始状态维度必须为 {n}", NameOf(initial))
        End If

        Call Array.Copy(initial, y.Data, n)

        Dim status As CVODEStatus = solver.Initialize(0.0, y)

        If status <> CVODEStatus.Success Then
            Throw New InvalidOperationException($"CVODE 初始化失败：{status}")
        End If

        t = 0.0
    End Sub

    ''' <summary>
    ''' 从当前时间重启求解器：用于外部（非本 ODE）对状态做了跳变之后重新起算
    ''' </summary>
    Protected Sub ResetState(values As Double())
        If values Is Nothing OrElse values.Length <> n Then
            Throw New ArgumentException($"状态维度必须为 {n}", NameOf(values))
        End If

        Call Array.Copy(values, y.Data, n)

        Dim status As CVODEStatus = solver.Initialize(t, y)

        If status <> CVODEStatus.Success Then
            Throw New InvalidOperationException($"CVODE 重启失败：{status}")
        End If
    End Sub

    ''' <summary>
    ''' 把系统推进 dt 个时间单位
    ''' </summary>
    ''' <returns>成功返回 True；求解失败（发散 / 步长过小 / 超过最大步数）返回 False</returns>
    Protected Function Advance(dt As Double) As Boolean
        If dt <= 0 Then
            Return True
        End If

        Dim target As Double = t + dt
        Dim status As CVODEStatus = solver.Integrate(target, y)

        If status = CVODEStatus.TooManySteps OrElse solver.TotalSteps > RebaseThreshold Then
            ' 重启求解器：从当前已提交的状态继续，清空内部步数计数器
            status = solver.Initialize(t, y)

            If status <> CVODEStatus.Success Then
                _lastStatus = status
                Call CountFailure()

                Return False
            End If

            status = solver.Integrate(target, y)
        End If

        _lastStatus = status

        If status <> CVODEStatus.Success AndAlso status <> CVODEStatus.TStopReturn Then
            ' 单次起步 / 步长失配是刚启动时常有的小故障：重启求解器重试一次，
            ' 仍失败才判定为发散并交由调用方回退
            status = solver.Initialize(t, y)

            If status = CVODEStatus.Success Then
                status = solver.Integrate(target, y)
            End If

            _lastStatus = status
        End If

        If status = CVODEStatus.Success OrElse status = CVODEStatus.TStopReturn Then
            t = target
            Return True
        End If

        Call CountFailure()

        Return False
    End Function

    ''' <summary>读取当前状态向量（拷贝，避免外部误改）</summary>
    Protected Function CurrentState() As Double()
        Dim out As Double() = New Double(n - 1) {}

        Call Array.Copy(y.Data, out, n)

        Return out
    End Function

    Public Overrides Function GetStats() As Dictionary(Of String, Double)
        Return New Dictionary(Of String, Double) From {
            {"time", t},
            {"ode_steps", solver.TotalSteps},
            {"ode_order", solver.CurrentOrder},
            {"ode_status", CDbl(CInt(_lastStatus))},
            {"failed", CDbl(FailedSteps)}
        }
    End Function

    ' ==================== IDisposable ====================

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposed Then
            If disposing Then
                Call solver.Dispose()
            End If

            disposed = True
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Call Dispose(True)
        Call GC.SuppressFinalize(Me)
    End Sub

End Class
