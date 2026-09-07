' ============================================================================
' IpmFbaAdapter.vb — FBA 问题 ⇄ 内点法标准形 的适配层
' ----------------------------------------------------------------------------
' FBA 的数学模型：
'     max Σ c_j·v_j
'     s.t.  S·v = 0            （稳态假设：每个代谢物的生成速率 = 消耗速率）
'           lb_j ≤ v_j ≤ ub_j  （不可逆反应 lb = 0；可逆反应 lb < 0）
'
' 转换到内点法标准形（0 ≤ x ≤ u）只需一次**下界平移**，不增加任何行或列：
'     v = lb + x   ⇒   S·x = −S·lb，  0 ≤ x ≤ ub − lb，  obj = cᵀlb + cᵀx
'
' 之所以必须保留上下界：没有上界时 FBA 会在零通量点退化/无界
' （质量平衡约束右端恒为 0，零向量永远可行），
' 内点法对有界变量是原生支持的（w = u − x、对偶 z、Θ = 1/(s/x + z/w)）。
'
' 大规模表现的关键：全程走 CSR 稀疏矩阵，不物化稠密矩阵
' （GEM 14777×16107 的稠密存储需要 1.9 GB，必然 OOM）。
' ============================================================================

Imports System.Diagnostics
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming
Imports Microsoft.VisualBasic.Math.LinearAlgebra.LinearProgramming.IPMCrossover
Imports std = System.Math

''' <summary>
''' FBA 求解器到内点法引擎的桥接（FBA 侧唯一入口）
''' </summary>
Public Module IpmFbaAdapter

    ''' <summary>
    ''' 由 FBA 的化学计量矩阵 + 流量上下界 + 目标反应构造内点法标准形
    ''' </summary>
    ''' <param name="fbaMat">FBA 矩阵（稀疏化学计量矩阵 + 反应流量上下界）</param>
    ''' <param name="opt">最大化（生物量）或最小化</param>
    Public Function CreateStandard(fbaMat As Matrix,
                                   Optional opt As OptimizationType = OptimizationType.MAX) As StandardForm
        Dim stoichiometry As LpSparseMatrix = GetStoichiometry(fbaMat)
        Dim bounds As (lb As Double(), ub As Double()) = fbaMat.GetFluxBounds()
        Dim names As String() = fbaMat.Flux.Keys.ToArray
        Dim objective As Double() = fbaMat.GetTargetCoefficients()
        Dim rhs As Double() = New Double(fbaMat.NumOfCompounds - 1) {}
        Dim sense As String = If(opt.Description.ToLowerInvariant.StartsWith("max"), "max", "min")

        Return StandardForm.FromSparse(
            csr:=stoichiometry,
            rhs:=rhs,
            obj:=objective,
            lb:=bounds.lb,
            ub:=bounds.ub,
            varNames:=names,
            sense:=sense
        )
    End Function

    ''' <summary>
    ''' 用内点法求解 FBA 问题
    ''' </summary>
    ''' <returns>
    ''' + <see cref="LPPSolution.ObjectiveFunctionValue"/> 为目标（生物量）通量
    ''' + <see cref="LPPSolution.GetSolution()"/> 为每个反应的通量值
    ''' </returns>
    Public Function Run(fbaMat As Matrix,
                        Optional opt As OptimizationType = OptimizationType.MAX) As LPPSolution
        Dim sf As StandardForm = CreateStandard(fbaMat, opt)
        Dim watch As Stopwatch = Stopwatch.StartNew

        Console.WriteLine($"run IPM solver for FBA problem! [{sf.M} x {sf.N}], {sf.Mat.NonZeros} non-zeros")

        Dim result As LPPSolution = LppSolver.SolveStandard(sf)

        watch.Stop()

        If Not String.IsNullOrEmpty(result.failureMessage) Then
            Console.WriteLine($"FBA IPM solver failed in {watch.ElapsedMilliseconds} ms: {result.failureMessage}")
            Console.WriteLine(result.SolutionLog)
            Return result
        End If

        Console.WriteLine($"FBA IPM problem solved in {watch.ElapsedMilliseconds} ms, objective = {result.ObjectiveFunctionValue}")

        Call ClampToBounds(result, fbaMat)

        Return result
    End Function

    ''' <summary>取稀疏化学计量矩阵（兼容只设置了稠密矩阵的模型对象）</summary>
    Private Function GetStoichiometry(fbaMat As Matrix) As LpSparseMatrix
        If fbaMat.Stoichiometry IsNot Nothing Then
            Return fbaMat.Stoichiometry
        End If

        Return LpSparseMatrix.FromJagged(fbaMat.Matrix)
    End Function

    ''' <summary>
    ''' 内点解严格保持在可行域内部，数值误差可能让个别通量越界 1e-9 量级，
    ''' 这里夹回 [lb, ub]，保证下游（质量平衡残差、通量统计）看到的是可行解。
    ''' </summary>
    Private Sub ClampToBounds(sol As LPPSolution, fbaMat As Matrix)
        Dim clamped As Integer = 0

        For i As Integer = 0 To sol.solution.Length - 1
            Dim name As String = sol.variableNames(i)

            If Not fbaMat.Flux.ContainsKey(name) Then
                Continue For
            End If

            Dim range As DoubleRange = fbaMat.Flux(name)
            Dim v As Double = sol.solution(i)

            If v < range.Min Then
                sol.solution(i) = range.Min
                clamped += 1
            ElseIf v > range.Max Then
                sol.solution(i) = range.Max
                clamped += 1
            End If
        Next

        If clamped > 0 Then
            Console.WriteLine($"  {clamped} flux values clamped back into the flux bounds")
        End If
    End Sub

End Module
