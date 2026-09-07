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
Imports System.Text
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

        ' 预求解：去掉空行与重复行。
        ' 这一步对基因组规模模型是**必需**的——重复的行会让 S·Θ·Sᵀ 奇异，
        ' 正规方程的解被放大到 1e8~1e11，内点法完全走不动。
        Dim rhs2 As Double() = Nothing

        stoichiometry = PresolveRows(stoichiometry, rhs, rhs2)

        Return StandardForm.FromSparse(
            csr:=stoichiometry,
            rhs:=rhs2,
            obj:=objective,
            lb:=bounds.lb,
            ub:=bounds.ub,
            varNames:=names,
            sense:=sense
        )
    End Function

    ''' <summary>
    ''' 行预求解：删除全零行、合并完全相同的行
    ''' </summary>
    ''' <param name="csr">化学计量矩阵</param>
    ''' <param name="rhs">约束右端项</param>
    ''' <param name="rhsOut">压缩后的右端项</param>
    ''' <returns>压缩后的化学计量矩阵（列数不变）</returns>
    Private Function PresolveRows(csr As LpSparseMatrix, rhs As Double(),
                                  ByRef rhsOut As Double()) As LpSparseMatrix
        Dim m As Integer = csr.Rows
        Dim keep As New List(Of Integer)()
        Dim seen As New Dictionary(Of String, Integer)()
        Dim nDup As Integer = 0
        Dim nZero As Integer = 0
        Dim tol As Double = 0.0000001

        For i As Integer = 0 To m - 1
            Dim p0 As Integer = csr.RowPtr(i)
            Dim p1 As Integer = csr.RowPtr(i + 1) - 1

            If p1 < p0 Then
                ' 全零行：右端必须为 0，否则问题不可行
                nZero += 1

                If std.Abs(rhs(i)) > tol Then
                    Throw New ArgumentException($"第 {i + 1} 行没有非零系数但右端项为 {rhs(i)}，问题不可行")
                End If

                Continue For
            End If

            Dim key As New StringBuilder()

            For p As Integer = p0 To p1
                key.Append(csr.ColIdx(p)).Append(":"c).Append(csr.Values(p).ToString("R")).Append(";"c)
            Next

            Dim sig As String = key.ToString()

            If seen.ContainsKey(sig) Then
                ' 重复行：只有当右端也一致时才是冗余约束，否则不可行
                Dim j As Integer = seen(sig)

                If std.Abs(rhs(i) - rhs(j)) > tol * (1.0 + std.Abs(rhs(j))) Then
                    Throw New ArgumentException($"第 {i + 1} 行与第 {j + 1} 行系数相同但右端项不同，问题不可行")
                End If

                nDup += 1
                Continue For
            End If

            seen(sig) = i
            keep.Add(i)
        Next

        If nZero = 0 AndAlso nDup = 0 Then
            rhsOut = rhs
            Return csr
        End If
        ' 重新打包 CSR
        Dim nnz As Integer = 0


        For Each i As Integer In keep
            nnz += csr.RowPtr(i + 1) - csr.RowPtr(i)
        Next

        Dim rowPtr As Integer() = New Integer(keep.Count) {}
        Dim colIdx As Integer() = New Integer(std.Max(nnz, 1) - 1) {}
        Dim values As Double() = New Double(std.Max(nnz, 1) - 1) {}
        Dim newRhs As Double() = New Double(keep.Count - 1) {}
        Dim q As Integer = 0

        For k As Integer = 0 To keep.Count - 1
            Dim i As Integer = keep(k)

            rowPtr(k) = q
            newRhs(k) = rhs(i)

            For p As Integer = csr.RowPtr(i) To csr.RowPtr(i + 1) - 1
                colIdx(q) = csr.ColIdx(p)
                values(q) = csr.Values(p)
                q += 1
            Next
        Next

        rowPtr(keep.Count) = q
        rhsOut = newRhs

        Console.WriteLine($"  presolve: {m} → {keep.Count} rows (删除 {nZero} 个空行, 合并 {nDup} 个重复行)")

        Return New LpSparseMatrix(keep.Count, csr.Columns, rowPtr, colIdx, values)
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

        Call ApplyTuning()

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

    ''' <summary>
    ''' 数值调参开关（环境变量，便于在基因组规模问题上做 A/B 对比）
    '''   FBA_IPM_PCG=1      正规方程强制走 PCG（跳过稀疏 LDLᵀ）
    '''   FBA_IPM_CHOLNNZ=n 稀疏 LDLᵀ 的 fill-in 上限
    '''   FBA_IPM_MAXITER=n 内点法最大迭代数
    ''' </summary>
    Private Sub ApplyTuning()
        If Environment.GetEnvironmentVariable("FBA_IPM_PCG") = "1" Then
            SparseLpMatrix.CholRowLimit = 0
        End If

        Dim nnzLimit As String = Environment.GetEnvironmentVariable("FBA_IPM_CHOLNNZ")

        If Not String.IsNullOrEmpty(nnzLimit) Then
            SparseLpMatrix.MaxCholNnz = CLng(Val(nnzLimit))
        End If

        Dim maxIter As String = Environment.GetEnvironmentVariable("FBA_IPM_MAXITER")

        If Not String.IsNullOrEmpty(maxIter) Then
            MaxIPMIterations = CInt(Val(maxIter))
        End If
    End Sub

    ''' <summary>内点法最大迭代数（默认 200）</summary>
    Public Property MaxIPMIterations As Integer = 200

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
