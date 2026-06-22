Imports System.IO
Imports System.Runtime.CompilerServices
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix

''' <summary>
''' 结构方程模型 (SEM) 模块 - 基于路径分析 (Path Analysis) 的实现
''' 
''' 路径分析是 SEM 的特例，处理的是观测变量之间的因果关系网络。
''' 通过对每个内生变量做 OLS 回归，得到标准化路径系数，
''' 进而计算直接效应、间接效应和总效应。
''' 
''' 模型拟合评估：
'''  - 拟合协方差矩阵 S 与模型隐含协方差矩阵 Σ(θ) 的差异
'''  - 计算 χ²、RMSEA、CFI、GFI、NFI 等拟合指数
''' 
''' 显著性检验使用 Bootstrap 重采样方法。
''' </summary>
Public Module SEM

    ''' <summary>
    ''' 拟合路径分析 SEM 模型
    ''' 
    ''' paths: 路径定义列表，每项为 (from, to)，from 是外生/中介变量索引，to 是内生变量索引
    ''' data: 原始数据矩阵 (n × p)，列为变量
    ''' varNames: 变量名称
    ''' </summary>
    <Extension>
    <MethodImpl(MethodImplOptions.AggressiveInlining)>
    Public Function FitPathAnalysis(model As CausalModel, Optional strict As Boolean = False) As SEMResult
        Return FitPathAnalysis(model.data, model.varNames, New List(Of (fromIdx As Integer, toIdx As Integer))(model.AsPathTuple), strict:=strict)
    End Function

    ''' <summary>
    ''' 拟合路径分析 SEM 模型
    ''' 
    ''' paths: 路径定义列表，每项为 (from, to)，from 是外生/中介变量索引，to 是内生变量索引
    ''' data: 原始数据矩阵 (n × p)，列为变量
    ''' varNames: 变量名称
    ''' </summary>
    Public Function FitPathAnalysis(data As Double(,),
                                    varNames As String(),
                                    paths As List(Of (fromIdx As Integer, toIdx As Integer)),
                                    strict As Boolean) As SEMResult

        Dim result As New SEMResult()
        result.VarNames = varNames
        result.Paths = paths
        result.SampleSize = data.GetLength(0)
        result.NumVariables = data.GetLength(1)

        ' 1. 标准化数据（Z-score），这样回归系数即标准化路径系数
        Dim Z = Statistics.Standardize(data)

        ' 2. 计算样本相关/协方差矩阵
        result.SampleCorr = Statistics.CorrelationMatrix(data)
        result.SampleCov = Statistics.CovarianceMatrix(data)

        ' 3. 识别内生变量与外生变量
        Dim endogenousSet As New HashSet(Of Integer)
        For Each p In paths
            endogenousSet.Add(p.toIdx)
        Next
        result.EndogenousVars = endogenousSet

        ' 4. 对每个内生变量做 OLS 回归（在标准化数据上）
        Dim pathCoefDict As New Dictionary(Of (Integer, Integer), Double)
        Dim seDict As New Dictionary(Of (Integer, Integer), Double)
        Dim tDict As New Dictionary(Of (Integer, Integer), Double)
        Dim pDict As New Dictionary(Of (Integer, Integer), Double)

        ' 记录每个内生变量的回归 R²
        Dim r2Dict As New Dictionary(Of Integer, Double)

        ' 找出每个内生变量的所有预测变量
        Dim predictorsOf As New Dictionary(Of Integer, List(Of Integer))
        For Each p In paths
            If Not predictorsOf.ContainsKey(p.toIdx) Then
                predictorsOf(p.toIdx) = New List(Of Integer)()
            End If
            predictorsOf(p.toIdx).Add(p.fromIdx)
        Next

        ' 对每个内生变量执行回归
        For Each endoKV In predictorsOf
            Dim endoIdx = endoKV.Key
            Dim predictors = endoKV.Value

            ' 构造 X 矩阵（标准化数据中选取预测变量列）
            Dim n = Z.GetLength(0)
            Dim X(n - 1, predictors.Count - 1) As Double
            For i = 0 To n - 1
                For j = 0 To predictors.Count - 1
                    X(i, j) = Z(i, predictors(j))
                Next
            Next

            ' 构造 y 向量
            Dim y(n - 1) As Double
            For i = 0 To n - 1
                y(i) = Z(i, endoIdx)
            Next

            ' 注意：标准化数据上回归不需要截距（截距应为 0）
            ' 但为了使用 OLSRegression 函数，我们仍加上截距列
            Dim XWithIntercept = Statistics.AddIntercept(X)
            Dim ols = Statistics.OLSRegression(y, XWithIntercept, strict)

            ' 记录 R²
            r2Dict(endoIdx) = ols.R2

            ' 记录路径系数（跳过截距项，索引 0 是截距）
            For j = 0 To predictors.Count - 1
                Dim fromIdx = predictors(j)
                pathCoefDict((fromIdx, endoIdx)) = ols.Coefficients(j + 1)
                seDict((fromIdx, endoIdx)) = ols.StdErrors(j + 1)
                tDict((fromIdx, endoIdx)) = ols.TValues(j + 1)
                pDict((fromIdx, endoIdx)) = ols.PValues(j + 1)
            Next
        Next

        result.PathCoefficients = pathCoefDict
        result.StdErrors = seDict
        result.TValues = tDict
        result.PValues = pDict
        result.RSquared = r2Dict

        ' 5. 计算直接、间接、总效应
        ComputeEffects(result)
        ' 6. 计算模型隐含协方差矩阵与拟合指数
        ComputeModelImpliedCov(result, strict)
        ComputeFitIndices(result)

        Return result
    End Function

    ''' <summary>
    ''' 计算直接效应、间接效应、总效应
    ''' 直接效应 = 路径系数本身
    ''' 间接效应 = 沿所有中介路径的路径系数乘积之和
    ''' 总效应 = 直接 + 间接
    ''' </summary>
    Private Sub ComputeEffects(result As SEMResult)
        Dim direct As New Dictionary(Of (Integer, Integer), Double)
        Dim indirect As New Dictionary(Of (Integer, Integer), Double)
        Dim total As New Dictionary(Of (Integer, Integer), Double)

        Dim numVar = result.NumVariables

        ' 直接效应 = 路径系数
        For Each kv In result.PathCoefficients
            direct(kv.Key) = kv.Value
        Next

        ' 间接效应：枚举所有 from -> to 对，找所有长度 >= 2 的路径
        For fromIdx = 0 To numVar - 1
            For toIdx = 0 To numVar - 1
                If fromIdx = toIdx Then Continue For

                ' 找所有从 fromIdx 到 toIdx 的简单路径（不重复经过节点）
                Dim allPaths = FindAllPaths(result, fromIdx, toIdx)

                Dim ind = 0.0
                For Each path In allPaths
                    If path.Count >= 2 Then
                        ' 路径长度 >= 2 表示至少经过一个中介
                        Dim prod = 1.0
                        For i = 0 To path.Count - 2
                            prod *= result.PathCoefficients((path(i), path(i + 1)))
                        Next
                        ind += prod
                    End If
                Next

                If Math.Abs(ind) > 0.000000000001 Then
                    indirect((fromIdx, toIdx)) = ind
                End If
            Next
        Next

        ' 总效应 = 直接 + 间接
        Dim allKeys As New HashSet(Of (Integer, Integer))
        For Each k In direct.Keys : allKeys.Add(k) : Next
        For Each k In indirect.Keys : allKeys.Add(k) : Next

        For Each k In allKeys
            Dim d = If(direct.ContainsKey(k), direct(k), 0.0)
            Dim i = If(indirect.ContainsKey(k), indirect(k), 0.0)
            total(k) = d + i
        Next

        result.DirectEffects = direct
        result.IndirectEffects = indirect
        result.TotalEffects = total
    End Sub

    ''' <summary>使用 DFS 找所有从 start 到 end 的简单路径</summary>
    Private Function FindAllPaths(result As SEMResult, start As Integer, end_ As Integer) As List(Of List(Of Integer))
        Dim allPaths As New List(Of List(Of Integer))()
        Dim currentPath As New List(Of Integer)()
        Dim visited(result.NumVariables - 1) As Boolean

        ' 构建邻接表
        Dim adj(result.NumVariables - 1) As List(Of Integer)
        For i = 0 To result.NumVariables - 1
            adj(i) = New List(Of Integer)()
        Next
        For Each kv In result.PathCoefficients
            adj(kv.Key.Item1).Add(kv.Key.Item2)
        Next

        DFS(start, end_, adj, visited, currentPath, allPaths)
        Return allPaths
    End Function

    Private Sub DFS(current As Integer, end_ As Integer,
                    adj As List(Of Integer)(), visited As Boolean(),
                    currentPath As List(Of Integer),
                    allPaths As List(Of List(Of Integer)))
        visited(current) = True
        currentPath.Add(current)

        If current = end_ Then
            If currentPath.Count >= 2 Then
                allPaths.Add(New List(Of Integer)(currentPath))
            End If
        Else
            For Each neighbor In adj(current)
                If Not visited(neighbor) Then
                    DFS(neighbor, end_, adj, visited, currentPath, allPaths)
                End If
            Next
        End If

        currentPath.RemoveAt(currentPath.Count - 1)
        visited(current) = False
    End Sub

    ''' <summary>
    ''' 计算模型隐含协方差矩阵 Σ(θ)
    ''' 对于递归路径模型：Σ = (I - B)^{-1} Ψ (I - B)^{-T}
    ''' 其中 B 是路径系数矩阵，Ψ 是外生变量协方差矩阵 + 残差方差对角阵
    ''' 简化实现：使用 Wright 的路径追踪规则
    ''' </summary>
    Private Sub ComputeModelImpliedCov(result As SEMResult, strict As Boolean)
        Dim p = result.NumVariables
        Dim B(p - 1, p - 1) As Double  ' 路径系数矩阵（行=效应变量，列=原因变量）
        For Each kv In result.PathCoefficients
            B(kv.Key.Item2, kv.Key.Item1) = kv.Value
        Next

        ' I - B
        Dim IB(p - 1, p - 1) As Double
        For i = 0 To p - 1
            For j = 0 To p - 1
                If i = j Then
                    IB(i, j) = 1.0 - B(i, j)
                Else
                    IB(i, j) = -B(i, j)
                End If
            Next
        Next

        ' (I - B)^{-1}
        Dim IBInv As Double(,)
        Try
            IBInv = MatrixOps.Inverse(IB, strict)
        Catch ex As Exception
            ' 奇异矩阵，使用伪逆近似
            ReDim IBInv(p - 1, p - 1)
            For i = 0 To p - 1
                IBInv(i, i) = 1.0
            Next
        End Try

        ' 构造 Ψ 矩阵：外生变量的协方差 + 内生变量的残差方差
        Dim Psi(p - 1, p - 1) As Double
        For i = 0 To p - 1
            If result.EndogenousVars.Contains(i) Then
                ' 内生变量：残差方差 = 1 - R²（标准化情况下）
                Dim r2 = If(result.RSquared.ContainsKey(i), result.RSquared(i), 0.0)
                Psi(i, i) = 1.0 - r2
            Else
                ' 外生变量：方差 = 1（标准化）
                Psi(i, i) = 1.0
            End If
        Next

        ' 外生变量之间的协方差
        For i = 0 To p - 1
            For j = 0 To p - 1
                If i <> j AndAlso Not result.EndogenousVars.Contains(i) AndAlso Not result.EndogenousVars.Contains(j) Then
                    Psi(i, j) = result.SampleCorr(i, j)
                End If
            Next
        Next

        ' Σ(θ) = (I-B)^{-1} Ψ (I-B)^{-T}
        Dim IBInvT = MatrixOps.Transpose(IBInv)
        Dim implied = MatrixOps.Multiply(MatrixOps.Multiply(IBInv, Psi), IBInvT)
        result.ImpliedCov = implied
    End Sub

    ''' <summary>计算模型拟合指数</summary>
    Private Sub ComputeFitIndices(result As SEMResult)
        Dim p = result.NumVariables
        Dim n = result.SampleSize

        ' 计算 S 与 Σ(θ) 的差异
        Dim S = result.SampleCorr  ' 使用相关矩阵作为 S（标准化）
        Dim Sigma = result.ImpliedCov

        Dim diff(p - 1, p - 1) As Double
        For i = 0 To p - 1
            For j = 0 To p - 1
                diff(i, j) = S(i, j) - Sigma(i, j)
            Next
        Next

        ' F0 = 0.5 * trace((S^{-1} Σ - I)^2) 或简化为差异的 Frobenius 范数
        Dim f0 = 0.0
        For i = 0 To p - 1
            For j = 0 To p - 1
                f0 += diff(i, j) ^ 2
            Next
        Next
        f0 *= 0.5

        ' 自由度 = 数据点数 - 估计参数数
        Dim numDataPoints = p * (p + 1) / 2  ' 对称矩阵独立元素数
        Dim numParams = result.PathCoefficients.Count + result.EndogenousVars.Count  ' 路径系数 + 残差方差
        ' 加上外生变量协方差
        Dim numExo = p - result.EndogenousVars.Count
        numParams += numExo * (numExo + 1) / 2
        Dim df = Math.Max(1, numDataPoints - numParams)

        ' χ² = (n - 1) * F0
        result.ChiSquare = (n - 1) * f0
        result.DF = df
        result.ChiSquarePValue = Statistics.TDistTwoTail(Math.Sqrt(result.ChiSquare), df)  ' 近似
        ' 更精确：χ² 分布 p 值
        result.ChiSquarePValue = ChiSquarePValue(result.ChiSquare, df)

        ' RMSEA = sqrt(max(F0/df, 0))
        result.RMSEA = Math.Sqrt(Math.Max(f0 / df, 0.0))

        ' GFI = 1 - sum(diff^2) / sum(S^2)
        Dim sumDiff2 = 0.0, sumS2 = 0.0
        For i = 0 To p - 1
            For j = 0 To p - 1
                sumDiff2 += diff(i, j) ^ 2
                sumS2 += S(i, j) ^ 2
            Next
        Next
        result.GFI = 1.0 - sumDiff2 / sumS2

        ' NFI = (χ²_null - χ²) / χ²_null，其中 χ²_null 是零模型（所有变量独立）
        Dim chiNull = 0.0
        For i = 0 To p - 1
            For j = 0 To p - 1
                If i <> j Then
                    chiNull += S(i, j) ^ 2
                End If
            Next
        Next
        chiNull *= (n - 1)
        result.NFI = If(chiNull > 0, (chiNull - result.ChiSquare) / chiNull, 0.0)

        ' CFI = 1 - max(χ² - df, 0) / max(χNull - dfNull, 0)
        Dim dfNull = numDataPoints - p
        result.CFI = 1.0 - Math.Max(result.ChiSquare - df, 0.0) / Math.Max(chiNull - dfNull, 0.0)

        ' SRMR = sqrt(mean(diff_offdiag^2))
        Dim sumOff2 = 0.0
        Dim countOff = 0
        For i = 0 To p - 1
            For j = 0 To p - 1
                If i <> j Then
                    sumOff2 += diff(i, j) ^ 2
                    countOff += 1
                End If
            Next
        Next
        result.SRMR = Math.Sqrt(sumOff2 / countOff)
    End Sub

    ''' <summary>卡方分布 p 值（基于不完全 Gamma 函数）</summary>
    Public Function ChiSquarePValue(x As Double, df As Double) As Double
        If x <= 0 Then Return 1.0
        ' P(X > x) = 1 - P(X <= x) = 1 - Gamma(df/2, x/2) / Gamma(df/2)
        ' = Q(df/2, x/2) 上尾概率
        Return GammaQ(df / 2.0, x / 2.0)
    End Function

    ''' <summary>正则化上尾不完全 Gamma 函数 Q(a, x)</summary>
    Public Function GammaQ(a As Double, x As Double) As Double
        If x < 0 OrElse a <= 0 Then Return 1.0
        If x < a + 1.0 Then
            ' 级数展开
            Return 1.0 - GammaPSeries(a, x)
        Else
            ' 连分数
            Return GammaQContinuedFraction(a, x)
        End If
    End Function

    Private Function GammaPSeries(a As Double, x As Double) As Double
        Dim MAXIT = 300
        Dim EPS = 0.000000000001
        Dim FPMIN = 1.0E-300
        Dim gln = Statistics.LogGamma(a)
        Dim ap = a
        Dim sum = 1.0 / a
        Dim del = sum
        For n = 1 To MAXIT
            ap += 1.0
            del *= x / ap
            sum += del
            If Math.Abs(del) < Math.Abs(sum) * EPS Then Exit For
        Next
        Return sum * Math.Exp(-x + a * Math.Log(x) - gln)
    End Function

    Private Function GammaQContinuedFraction(a As Double, x As Double) As Double
        Dim MAXIT = 300
        Dim EPS = 0.000000000001
        Dim FPMIN = 1.0E-300
        Dim gln = Statistics.LogGamma(a)
        Dim b = x + 1.0 - a
        Dim c = 1.0 / FPMIN
        Dim d = 1.0 / b
        Dim h = d
        For i = 1 To MAXIT
            Dim an = -i * (i - a)
            b += 2.0
            d = an * d + b
            If Math.Abs(d) < FPMIN Then d = FPMIN
            c = b + an / c
            If Math.Abs(c) < FPMIN Then c = FPMIN
            d = 1.0 / d
            Dim del = d * c
            h *= del
            If Math.Abs(del - 1.0) < EPS Then Exit For
        Next
        Return Math.Exp(-x + a * Math.Log(x) - gln) * h
    End Function

    ''' <summary>
    ''' Bootstrap 显著性检验
    ''' 对路径系数、间接效应进行 Bootstrap 重采样
    ''' </summary>
    Public Function BootstrapSEM(model As CausalModel, numBoot As Integer, seed As Integer, Optional strict As Boolean = False) As BootstrapResult
        Dim rng As New Random(seed)
        Dim result As New BootstrapResult()

        ' 收集所有需要 Bootstrap 的量
        Dim pathKeys As New List(Of (Integer, Integer))
        Dim paths As New List(Of (Integer, Integer))(model.AsPathTuple)
        For Each p In paths
            If Not pathKeys.Contains(p) Then pathKeys.Add(p)
        Next

        ' 存储 Bootstrap 样本
        Dim pathBootSamples As New Dictionary(Of (Integer, Integer), List(Of Double))
        For Each k In pathKeys
            pathBootSamples(k) = New List(Of Double)()
        Next

        Dim indirectKeys As New List(Of (Integer, Integer))()

        ' 先在原数据上跑一次得到间接效应的键
        Dim baseResult = FitPathAnalysis(model.data, model.varNames, paths, strict)
        For Each k In baseResult.IndirectEffects.Keys
            indirectKeys.Add(k)
        Next

        Dim indirectBootSamples As New Dictionary(Of (Integer, Integer), List(Of Double))
        For Each k In indirectKeys
            indirectBootSamples(k) = New List(Of Double)()
        Next

        ' Bootstrap 循环
        For Each b As Integer In TqdmWrapper.Range(0, numBoot)
            Dim bootData = Statistics.BootstrapSample(model.data, rng)
            Try
                Dim bootResult = FitPathAnalysis(bootData, model.varNames, paths, strict)

                For Each k In pathKeys
                    If bootResult.PathCoefficients.ContainsKey(k) Then
                        pathBootSamples(k).Add(bootResult.PathCoefficients(k))
                    Else
                        pathBootSamples(k).Add(0.0)
                    End If
                Next

                For Each k In indirectKeys
                    If bootResult.IndirectEffects.ContainsKey(k) Then
                        indirectBootSamples(k).Add(bootResult.IndirectEffects(k))
                    Else
                        indirectBootSamples(k).Add(0.0)
                    End If
                Next
            Catch ex As Exception
                ' 跳过失败的 Bootstrap 样本
                Continue For
            End Try
        Next

        ' 计算 Bootstrap 标准误与置信区间
        result.PathBootSE = New Dictionary(Of (Integer, Integer), Double)
        result.PathBootCI = New Dictionary(Of (Integer, Integer), (Double, Double))
        For Each k In pathKeys
            Dim samples = pathBootSamples(k).ToArray()
            If samples.Length > 0 Then
                result.PathBootSE(k) = samples.SD
                Dim lo = Statistics.Quantile(samples, 0.025)
                Dim hi = Statistics.Quantile(samples, 0.975)
                result.PathBootCI(k) = (lo, hi)
            End If
        Next

        result.IndirectBootSE = New Dictionary(Of (Integer, Integer), Double)
        result.IndirectBootCI = New Dictionary(Of (Integer, Integer), (Double, Double))
        For Each k In indirectKeys
            Dim samples = indirectBootSamples(k).ToArray()
            If samples.Length > 0 Then
                result.IndirectBootSE(k) = samples.SD
                Dim lo = Statistics.Quantile(samples, 0.025)
                Dim hi = Statistics.Quantile(samples, 0.975)
                result.IndirectBootCI(k) = (lo, hi)
            End If
        Next

        result.NumBootstraps = numBoot
        Return result
    End Function

    ''' <summary>打印 SEM 结果</summary>
    Public Sub PrintSEMResult(result As SEMResult, Optional bootResult As BootstrapResult = Nothing)
        Console.WriteLine("="c, 80)
        Console.WriteLine("结构方程模型 (SEM) - 路径分析结果")
        Console.WriteLine("="c, 80)
        Console.WriteLine($"样本量 N = {result.SampleSize}, 变量数 = {result.NumVariables}")
        Console.WriteLine()

        ' 1. 路径系数表
        Console.WriteLine("-"c, 80)
        Console.WriteLine("一、路径系数 (标准化)")
        Console.WriteLine("-"c, 80)
        Console.WriteLine($"{"路径",-30}{"系数",12}{"SE",12}{"t值",10}{"p值",10}{"显著性",8}")
        Console.WriteLine("-"c, 80)
        For Each kv In result.PathCoefficients
            Dim fromName = result.VarNames(kv.Key.Item1)
            Dim toName = result.VarNames(kv.Key.Item2)
            Dim coef = kv.Value
            Dim se = result.StdErrors(kv.Key)
            Dim t = result.TValues(kv.Key)
            Dim p = result.PValues(kv.Key)
            Dim sig = If(p < 0.001, "***", If(p < 0.01, "**", If(p < 0.05, "*", "ns")))
            Console.WriteLine($"{fromName + " -> " + toName,-30}{coef,12:F4}{se,12:F4}{t,10:F3}{p,10:F4}{sig,8}")
        Next
        Console.WriteLine($"注: *** p<0.001, ** p<0.01, * p<0.05, ns 不显著")
        Console.WriteLine()

        ' 2. R²
        Console.WriteLine("-"c, 80)
        Console.WriteLine("二、内生变量 R² (解释方差比例)")
        Console.WriteLine("-"c, 80)
        For Each kv In result.RSquared
            Console.WriteLine($"  {result.VarNames(kv.Key),-25} R² = {kv.Value:F4}")
        Next
        Console.WriteLine()

        ' 3. 效应分解
        Console.WriteLine("-"c, 80)
        Console.WriteLine("三、效应分解 (直接 / 间接 / 总效应)")
        Console.WriteLine("-"c, 80)
        Console.WriteLine($"{"路径",-30}{"直接",12}{"间接",12}{"总效应",12}")
        Console.WriteLine("-"c, 80)
        Dim allPairs As New HashSet(Of (Integer, Integer))
        For Each k In result.DirectEffects.Keys : allPairs.Add(k) : Next
        For Each k In result.IndirectEffects.Keys : allPairs.Add(k) : Next
        For Each k In result.TotalEffects.Keys : allPairs.Add(k) : Next

        For Each k In allPairs
            Dim fromName = result.VarNames(k.Item1)
            Dim toName = result.VarNames(k.Item2)
            Dim d = If(result.DirectEffects.ContainsKey(k), result.DirectEffects(k), 0.0)
            Dim i = If(result.IndirectEffects.ContainsKey(k), result.IndirectEffects(k), 0.0)
            Dim t = If(result.TotalEffects.ContainsKey(k), result.TotalEffects(k), 0.0)
            Console.WriteLine($"{fromName + " -> " + toName,-30}{d,12:F4}{i,12:F4}{t,12:F4}")
        Next
        Console.WriteLine()

        ' 4. Bootstrap 结果
        If bootResult IsNot Nothing Then
            Console.WriteLine("-"c, 80)
            Console.WriteLine($"四、Bootstrap 显著性检验 (重采样次数 = {bootResult.NumBootstraps})")
            Console.WriteLine("-"c, 80)
            Console.WriteLine("路径系数 Bootstrap:")
            Console.WriteLine($"{"路径",-30}{"系数",12}{"BootSE",12}{"95%CI下",12}{"95%CI上",12}{"显著",8}")
            Console.WriteLine("-"c, 80)
            For Each kv In result.PathCoefficients
                Dim fromName = result.VarNames(kv.Key.Item1)
                Dim toName = result.VarNames(kv.Key.Item2)
                Dim coef = kv.Value
                Dim bse = If(bootResult.PathBootSE.ContainsKey(kv.Key), bootResult.PathBootSE(kv.Key), 0.0)
                Dim ci = If(bootResult.PathBootCI.ContainsKey(kv.Key), bootResult.PathBootCI(kv.Key), (0.0, 0.0))
                Dim sig = If((ci.Item1 > 0 AndAlso ci.Item2 > 0) OrElse (ci.Item1 < 0 AndAlso ci.Item2 < 0), "Yes", "No")
                Console.WriteLine($"{fromName + " -> " + toName,-30}{coef,12:F4}{bse,12:F4}{ci.Item1,12:F4}{ci.Item2,12:F4}{sig,8}")
            Next
            Console.WriteLine()

            If bootResult.IndirectBootSE.Count > 0 Then
                Console.WriteLine("间接效应 Bootstrap:")
                Console.WriteLine($"{"路径",-30}{"间接效应",12}{"BootSE",12}{"95%CI下",12}{"95%CI上",12}{"显著",8}")
                Console.WriteLine("-"c, 80)
                For Each kv In result.IndirectEffects
                    Dim fromName = result.VarNames(kv.Key.Item1)
                    Dim toName = result.VarNames(kv.Key.Item2)
                    Dim ind = kv.Value
                    Dim bse = If(bootResult.IndirectBootSE.ContainsKey(kv.Key), bootResult.IndirectBootSE(kv.Key), 0.0)
                    Dim ci = If(bootResult.IndirectBootCI.ContainsKey(kv.Key), bootResult.IndirectBootCI(kv.Key), (0.0, 0.0))
                    Dim sig = If((ci.Item1 > 0 AndAlso ci.Item2 > 0) OrElse (ci.Item1 < 0 AndAlso ci.Item2 < 0), "Yes", "No")
                    Console.WriteLine($"{fromName + " -> " + toName,-30}{ind,12:F4}{bse,12:F4}{ci.Item1,12:F4}{ci.Item2,12:F4}{sig,8}")
                Next
                Console.WriteLine()
            End If
        End If

        ' 5. 模型拟合指数
        Console.WriteLine("-"c, 80)
        Console.WriteLine("五、模型拟合指数")
        Console.WriteLine("-"c, 80)
        Console.WriteLine($"  χ² (Chi-square)     = {result.ChiSquare:F4}")
        Console.WriteLine($"  df (自由度)          = {result.DF}")
        Console.WriteLine($"  χ² p-value          = {result.ChiSquarePValue:F4}")
        Console.WriteLine($"  RMSEA               = {result.RMSEA:F4}  (优良 < 0.05, 良好 < 0.08)")
        Console.WriteLine($"  CFI                 = {result.CFI:F4}  (优良 > 0.95, 良好 > 0.90)")
        Console.WriteLine($"  GFI                 = {result.GFI:F4}  (优良 > 0.95, 良好 > 0.90)")
        Console.WriteLine($"  NFI                 = {result.NFI:F4}  (优良 > 0.95, 良好 > 0.90)")
        Console.WriteLine($"  SRMR                = {result.SRMR:F4}  (优良 < 0.05, 良好 < 0.08)")
        Console.WriteLine()
    End Sub

End Module

''' <summary>SEM 结果</summary>
Public Class SEMResult
    Public Property VarNames As String()
    Public Property Paths As List(Of (Integer, Integer))
    Public Property SampleSize As Integer
    Public Property NumVariables As Integer
    Public Property EndogenousVars As HashSet(Of Integer)

    Public Property PathCoefficients As Dictionary(Of (Integer, Integer), Double)
    Public Property StdErrors As Dictionary(Of (Integer, Integer), Double)
    Public Property TValues As Dictionary(Of (Integer, Integer), Double)
    Public Property PValues As Dictionary(Of (Integer, Integer), Double)
    Public Property RSquared As Dictionary(Of Integer, Double)

    Public Property DirectEffects As Dictionary(Of (Integer, Integer), Double)
    Public Property IndirectEffects As Dictionary(Of (Integer, Integer), Double)
    Public Property TotalEffects As Dictionary(Of (Integer, Integer), Double)

    Public Property SampleCov As Double(,)
    Public Property SampleCorr As Double(,)
    Public Property ImpliedCov As Double(,)

    Public Property ChiSquare As Double
    Public Property DF As Integer
    Public Property ChiSquarePValue As Double
    Public Property RMSEA As Double
    Public Property CFI As Double
    Public Property GFI As Double
    Public Property NFI As Double
    Public Property SRMR As Double

    ''' <summary>
    ''' Endogenous variable R² (proportion of variance explained)
    ''' </summary>
    ''' <returns></returns>
    Public Function GetEndogenousVariable() As Dictionary(Of String, Double)
        Dim R2 As New Dictionary(Of String, Double)

        For Each kv In RSquared
            R2(_VarNames(kv.Key)) = kv.Value
        Next

        Return R2
    End Function

    Public Overrides Function ToString() As String
        Dim sb As New StringBuilder
        Dim text As New StringWriter(sb)

        ' 5. 模型拟合指数
        text.WriteLine($"  χ² (Chi-square) = {ChiSquare:F4}")
        text.WriteLine($"  df (自由度)      = {DF}")
        text.WriteLine($"  χ² p-value      = {ChiSquarePValue:F4}")
        text.WriteLine($"  RMSEA           = {RMSEA:F4}  (优良 < 0.05, 良好 < 0.08)")
        text.WriteLine($"  CFI             = {CFI:F4}  (优良 > 0.95, 良好 > 0.90)")
        text.WriteLine($"  GFI             = {GFI:F4}  (优良 > 0.95, 良好 > 0.90)")
        text.WriteLine($"  NFI             = {NFI:F4}  (优良 > 0.95, 良好 > 0.90)")
        text.WriteLine($"  SRMR            = {SRMR:F4}  (优良 < 0.05, 良好 < 0.08)")
        text.WriteLine()

        Call text.Flush()

        Return sb.ToString
    End Function

End Class

''' <summary>Bootstrap 结果</summary>
Public Class BootstrapResult
    Public Property NumBootstraps As Integer
    Public Property PathBootSE As Dictionary(Of (Integer, Integer), Double)
    Public Property PathBootCI As Dictionary(Of (Integer, Integer), (Double, Double))
    Public Property IndirectBootSE As Dictionary(Of (Integer, Integer), Double)
    Public Property IndirectBootCI As Dictionary(Of (Integer, Integer), (Double, Double))
End Class
