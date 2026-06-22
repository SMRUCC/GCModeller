Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ApplicationServices.Terminal.ProgressBar.Tqdm
Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix

''' <summary>
''' 偏最小二乘路径建模 (PLS-PM) 模块
''' 
''' PLS-PM 是一种基于方差的 SEM 方法，适用于小样本、非正态数据、
''' 含潜变量和显变量的复杂因果模型。
''' 
''' 算法步骤：
''' 1. 初始化外模型权重 w_j
''' 2. 计算潜变量得分 Y_j = Σ w_jk * x_jk (标准化)
''' 3. 计算内模型权重 e_ji (路径加权方案 / 重心方案 / 因子加权方案)
''' 4. 更新外模型权重：
'''    - Mode A (反映型): w_j = (1/n) X_j' Y_j (相关系数)
'''    - Mode B (形成型): w_j = (X_j' X_j)^{-1} X_j' Y_j
''' 5. 重复 2-4 直到收敛
''' 6. 计算最终路径系数、R²、载荷、共同度、冗余度
''' 7. Bootstrap 显著性检验
''' </summary>
Public Module PLSPM

    <Extension>
    Public Function FitPLSPM(model As CausalModel,
                             Optional maxIter As Integer = 300,
                             Optional tol As Double = 0.0000001) As PLSPMResult

        Dim manifestIndex As Index(Of String) = model.varNames.Indexing
        Dim paths = model.AsPathTuple.ToArray

        Return FitPLSPM(model.data, manifestIndex, model.latentDefs, paths, maxIter, tol)
    End Function

    ''' <summary>
    ''' 拟合 PLS-PM 模型
    ''' 
    ''' manifestData: 显变量数据矩阵 (n × p)
    ''' latentVars: 潜变量定义列表，每项为 (名称, 显变量索引列表, 模式)
    '''              模式: "A" = 反映型 (Reflective), "B" = 形成型 (Formative)
    ''' innerPaths: 内模型路径列表，每项为 (fromLatentIdx, toLatentIdx)
    ''' </summary>
    Public Function FitPLSPM(manifestData As Double(,),
                             manifestNames As Index(Of String),
                             latentVars As LatentDefinition(),
                             innerPaths As (fromIdx As Integer, toIdx As Integer)(),
                             Optional maxIter As Integer = 300,
                             Optional tol As Double = 0.0000001,
                             Optional strict As Boolean = False) As PLSPMResult

        Dim result As New PLSPMResult()
        Dim n = manifestData.GetLength(0)
        Dim numLatent = latentVars.Length

        result.LatentNames = latentVars.Select(Function(lv) lv.varName).ToArray()
        result.NumLatents = numLatent
        result.NumManifest = manifestData.GetLength(1)
        result.SampleSize = n
        result.InnerPaths = innerPaths
        result.LatentDefs = latentVars

        ' 1. 标准化显变量数据
        Dim Z = Statistics.Standardize(manifestData)
        result.StandardizedData = Z

        ' 2. 初始化外模型权重
        Dim outerWeights As New List(Of Double())()
        For Each lv In latentVars
            Dim numMV = lv.featureIDs.Length
            Dim w(numMV - 1) As Double
            For k = 0 To numMV - 1
                w(k) = 1.0 / Math.Sqrt(numMV)  ' 均匀初始化
            Next
            outerWeights.Add(w)
        Next

        ' 3. 迭代算法
        Dim latentScores(n - 1, numLatent - 1) As Double
        Dim prevScores(n - 1, numLatent - 1) As Double

        For iter = 1 To maxIter
            ' 保存上一次的潜变量得分
            For i = 0 To n - 1
                For j = 0 To numLatent - 1
                    prevScores(i, j) = latentScores(i, j)
                Next
            Next

            ' Step 1: 计算潜变量得分（外模型估计）
            For j = 0 To numLatent - 1
                Dim lv = latentVars(j)
                Dim w = outerWeights(j)
                Dim numMV = lv.featureIDs.Length

                For i = 0 To n - 1
                    Dim score = 0.0
                    For k = 0 To numMV - 1
                        score += w(k) * Z(i, manifestNames(lv.featureIDs(k)))
                    Next
                    latentScores(i, j) = score
                Next
            Next

            ' 标准化潜变量得分（均值为0，方差为1）
            For j = 0 To numLatent - 1
                Dim mean = 0.0
                For i = 0 To n - 1
                    mean += latentScores(i, j)
                Next
                mean /= n
                Dim var = 0.0
                For i = 0 To n - 1
                    var += (latentScores(i, j) - mean) ^ 2
                Next
                var /= (n - 1)
                Dim std = Math.Sqrt(var)
                If std < 1.0E-30 Then std = 1.0
                For i = 0 To n - 1
                    latentScores(i, j) = (latentScores(i, j) - mean) / std
                Next
            Next

            ' Step 2: 内模型估计 - 计算内模型权重
            Dim innerWeights(numLatent - 1, numLatent - 1) As Double
            ComputeInnerWeights(latentScores, innerPaths, innerWeights, numLatent, n, strict)

            ' Step 3: 计算内部潜变量得分（使用内模型权重）
            Dim innerScores(n - 1, numLatent - 1) As Double
            For i = 0 To n - 1
                For j = 0 To numLatent - 1
                    Dim score = 0.0
                    For k = 0 To numLatent - 1
                        score += innerWeights(j, k) * latentScores(i, k)
                    Next
                    innerScores(i, j) = score
                Next
            Next

            ' 标准化内部得分
            For j = 0 To numLatent - 1
                Dim mean = 0.0
                For i = 0 To n - 1
                    mean += innerScores(i, j)
                Next
                mean /= n
                Dim var = 0.0
                For i = 0 To n - 1
                    var += (innerScores(i, j) - mean) ^ 2
                Next
                var /= (n - 1)
                Dim std = Math.Sqrt(var)
                If std < 1.0E-30 Then std = 1.0
                For i = 0 To n - 1
                    innerScores(i, j) = (innerScores(i, j) - mean) / std
                Next
            Next

            ' Step 4: 更新外模型权重
            For j = 0 To numLatent - 1
                Dim lv = latentVars(j)
                Dim numMV = lv.featureIDs.Length
                Dim w(numMV - 1) As Double

                If lv.mode = MeasurementModels.A Then
                    ' Mode A (反映型): w_k = corr(x_k, Y_inner)
                    For k = 0 To numMV - 1
                        Dim xCol(n - 1) As Double
                        Dim yCol(n - 1) As Double
                        For i = 0 To n - 1
                            xCol(i) = Z(i, manifestNames(lv.featureIDs(k)))
                            yCol(i) = innerScores(i, j)
                        Next
                        w(k) = Statistics.Pearson(xCol, yCol)
                    Next
                Else
                    ' Mode B (形成型): w = (X'X)^{-1} X'Y_inner
                    Dim XMat(n - 1, numMV - 1) As Double
                    For i = 0 To n - 1
                        For k = 0 To numMV - 1
                            XMat(i, k) = Z(i, lv.featureIDs(k))
                        Next
                    Next
                    Dim Xt = MatrixOps.Transpose(XMat)
                    Dim XtX = MatrixOps.Multiply(Xt, XMat)
                    Dim yVec(n - 1, 0) As Double
                    For i = 0 To n - 1
                        yVec(i, 0) = innerScores(i, j)
                    Next
                    Dim Xty = MatrixOps.Multiply(Xt, yVec)
                    Try
                        Dim XtXInv = MatrixOps.Inverse(XtX, strict)
                        Dim wMat = MatrixOps.Multiply(XtXInv, Xty)
                        For k = 0 To numMV - 1
                            w(k) = wMat(k, 0)
                        Next
                    Catch ex As Exception
                        ' 奇异矩阵，回退到 Mode A
                        For k = 0 To numMV - 1
                            Dim xCol(n - 1) As Double
                            Dim yCol(n - 1) As Double
                            For i = 0 To n - 1
                                xCol(i) = Z(i, manifestNames(lv.featureIDs(k)))
                                yCol(i) = innerScores(i, j)
                            Next
                            w(k) = Statistics.Pearson(xCol, yCol)
                        Next
                    End Try
                End If

                outerWeights(j) = w
            Next

            ' Step 5: 检查收敛
            Dim maxDiff = 0.0
            For i = 0 To n - 1
                For j = 0 To numLatent - 1
                    Dim diff = Math.Abs(latentScores(i, j) - prevScores(i, j))
                    If diff > maxDiff Then maxDiff = diff
                Next
            Next

            If maxDiff < tol AndAlso iter > 1 Then
                result.NumIterations = iter
                Exit For
            End If
            result.NumIterations = iter
        Next

        ' 4. 最终计算：外模型载荷、共同度、路径系数、R²
        result.LatentScores = latentScores
        result.OuterWeights = outerWeights

        ' 外模型载荷 = corr(x_k, Y_j)
        Dim loadings As New List(Of Double())()
        Dim communalities As New List(Of Double)()
        For j = 0 To numLatent - 1
            Dim lv = latentVars(j)
            Dim numMV = lv.featureIDs.Length
            Dim load(numMV - 1) As Double
            Dim comm = 0.0
            For k = 0 To numMV - 1
                Dim xCol(n - 1) As Double
                Dim yCol(n - 1) As Double
                For i = 0 To n - 1
                    xCol(i) = Z(i, manifestNames(lv.featureIDs(k)))
                    yCol(i) = latentScores(i, j)
                Next
                load(k) = Statistics.Pearson(xCol, yCol)
                comm += load(k) ^ 2
            Next
            loadings.Add(load)
            communalities.Add(comm / numMV)
        Next
        result.Loadings = loadings
        result.Communalities = communalities

        ' 计算外模型权重（标准化后的最终权重）
        Dim finalWeights As New List(Of Double())()
        For j = 0 To numLatent - 1
            Dim lv = latentVars(j)
            Dim numMV = lv.featureIDs.Length
            Dim w = outerWeights(j)
            ' 归一化权重使潜变量方差为 1
            Dim norm = 0.0
            For k = 0 To numMV - 1
                norm += w(k) ^ 2
            Next
            norm = Math.Sqrt(norm)
            If norm < 1.0E-30 Then norm = 1.0
            Dim fw(numMV - 1) As Double
            For k = 0 To numMV - 1
                fw(k) = w(k) / norm
            Next
            finalWeights.Add(fw)
        Next
        result.FinalOuterWeights = finalWeights

        ' 内模型路径系数: 对每个内生潜变量，回归到其预测潜变量
        Dim pathCoefDict As New Dictionary(Of (Integer, Integer), Double)
        Dim r2Dict As New Dictionary(Of Integer, Double)
        Dim seDict As New Dictionary(Of (Integer, Integer), Double)
        Dim tDict As New Dictionary(Of (Integer, Integer), Double)
        Dim pDict As New Dictionary(Of (Integer, Integer), Double)

        ' 找出每个内生潜变量的预测变量
        Dim predictorsOf As New Dictionary(Of Integer, List(Of Integer))
        For Each p In innerPaths
            If Not predictorsOf.ContainsKey(p.Item2) Then
                predictorsOf(p.Item2) = New List(Of Integer)()
            End If
            predictorsOf(p.Item2).Add(p.Item1)
        Next

        For Each endoKV In predictorsOf
            Dim endoIdx = endoKV.Key
            Dim predictors = endoKV.Value

            Dim X(n - 1, predictors.Count - 1) As Double
            Dim y(n - 1) As Double
            For i = 0 To n - 1
                For j = 0 To predictors.Count - 1
                    X(i, j) = latentScores(i, predictors(j))
                Next
                y(i) = latentScores(i, endoIdx)
            Next

            Dim XWithIntercept = Statistics.AddIntercept(X)
            Dim ols = Statistics.OLSRegression(y, XWithIntercept, strict, makeWarn:=False)

            r2Dict(endoIdx) = ols.R2
            For j = 0 To predictors.Count - 1
                Dim fromIdx = predictors(j)
                pathCoefDict((fromIdx, endoIdx)) = ols.Coefficients(j + 1)
                seDict((fromIdx, endoIdx)) = ols.StdErrors(j + 1)
                tDict((fromIdx, endoIdx)) = ols.TValues(j + 1)
                pDict((fromIdx, endoIdx)) = ols.PValues(j + 1)
            Next
        Next

        result.PathCoefficients = pathCoefDict
        result.RSquared = r2Dict
        result.StdErrors = seDict
        result.TValues = tDict
        result.PValues = pDict

        ' 计算直接、间接、总效应
        ComputePLSPMEffects(result)

        ' 计算 Redundancy (冗余度) = Communality × R²
        Dim redundancies As New List(Of Double)()
        For j = 0 To numLatent - 1
            If r2Dict.ContainsKey(j) Then
                redundancies.Add(communalities(j) * r2Dict(j))
            Else
                redundancies.Add(0.0)
            End If
        Next
        result.Redundancies = redundancies

        ' 计算 GoF (Goodness of Fit) = sqrt(mean(communalities) × mean(R² of endogenous))
        Dim meanComm = 0.0
        For Each c In communalities
            meanComm += c
        Next
        meanComm /= communalities.Count

        Dim meanR2 = 0.0
        Dim r2Count = 0
        For Each kv In r2Dict
            meanR2 += kv.Value
            r2Count += 1
        Next
        If r2Count > 0 Then meanR2 /= r2Count

        result.GoF = Math.Sqrt(meanComm * meanR2)

        Return result
    End Function

    ''' <summary>
    ''' 计算内模型权重 - 使用路径加权方案 (Path Weighting Scheme)
    ''' 对每个内生潜变量：使用其预测潜变量的多元回归系数
    ''' 对每个外生潜变量：使用与其最相关的内生潜变量的相关系数
    ''' </summary>
    Private Sub ComputeInnerWeights(scores As Double(,),
                                    innerPaths As (Integer, Integer)(),
                                    innerWeights As Double(,),
                                    numLatent As Integer, n As Integer, strict As Boolean)
        ' 识别内生和外生潜变量
        Dim endogenousSet As New HashSet(Of Integer)
        For Each p In innerPaths
            endogenousSet.Add(p.Item2)
        Next

        ' 找出每个潜变量的邻居（有路径连接的）
        Dim neighborsOf As New Dictionary(Of Integer, List(Of Integer))
        For j = 0 To numLatent - 1
            neighborsOf(j) = New List(Of Integer)()
        Next
        For Each p In innerPaths
            If Not neighborsOf(p.Item1).Contains(p.Item2) Then
                neighborsOf(p.Item1).Add(p.Item2)
            End If
            If Not neighborsOf(p.Item2).Contains(p.Item1) Then
                neighborsOf(p.Item2).Add(p.Item1)
            End If
        Next

        ' 对每个潜变量计算内模型权重
        For j = 0 To numLatent - 1
            If endogenousSet.Contains(j) Then
                ' 内生变量：使用预测变量的回归系数
                ' 找出指向 j 的潜变量
                Dim predictors As New List(Of Integer)()
                For Each p In innerPaths
                    If p.Item2 = j Then predictors.Add(p.Item1)
                Next

                If predictors.Count = 0 Then Continue For

                ' 多元回归 j ~ predictors
                Dim X(n - 1, predictors.Count - 1) As Double
                Dim y(n - 1) As Double
                For i = 0 To n - 1
                    For k = 0 To predictors.Count - 1
                        X(i, k) = scores(i, predictors(k))
                    Next
                    y(i) = scores(i, j)
                Next

                Dim XWithIntercept = Statistics.AddIntercept(X)
                Dim ols = Statistics.OLSRegression(y, XWithIntercept, strict, makeWarn:=False)

                For k = 0 To predictors.Count - 1
                    innerWeights(j, predictors(k)) = ols.Coefficients(k + 1)
                Next
            Else
                ' 外生变量：使用与其最相关的邻居的相关系数
                For Each neighbor In neighborsOf(j)
                    Dim xCol(n - 1) As Double
                    Dim yCol(n - 1) As Double
                    For i = 0 To n - 1
                        xCol(i) = scores(i, j)
                        yCol(i) = scores(i, neighbor)
                    Next
                    innerWeights(j, neighbor) = Statistics.Pearson(xCol, yCol)
                Next
            End If
        Next
    End Sub

    ''' <summary>计算 PLS-PM 的直接、间接、总效应</summary>
    Private Sub ComputePLSPMEffects(result As PLSPMResult)
        Dim direct As New Dictionary(Of (Integer, Integer), Double)
        Dim indirect As New Dictionary(Of (Integer, Integer), Double)
        Dim total As New Dictionary(Of (Integer, Integer), Double)

        ' 直接效应
        For Each kv In result.PathCoefficients
            direct(kv.Key) = kv.Value
        Next

        ' 间接效应：DFS 找所有路径
        For fromIdx = 0 To result.NumLatents - 1
            For toIdx = 0 To result.NumLatents - 1
                If fromIdx = toIdx Then Continue For

                Dim allPaths = FindAllPathsPLS(result, fromIdx, toIdx)
                Dim ind = 0.0
                For Each path In allPaths
                    If path.Count >= 3 Then  ' 至少经过一个中介
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

        ' 总效应
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

    Private Function FindAllPathsPLS(result As PLSPMResult, start As Integer, end_ As Integer) As List(Of List(Of Integer))
        Dim allPaths As New List(Of List(Of Integer))()
        Dim currentPath As New List(Of Integer)()
        Dim visited(result.NumLatents - 1) As Boolean

        Dim adj(result.NumLatents - 1) As List(Of Integer)
        For i = 0 To result.NumLatents - 1
            adj(i) = New List(Of Integer)()
        Next
        For Each kv In result.PathCoefficients
            adj(kv.Key.Item1).Add(kv.Key.Item2)
        Next

        DFSPLS(start, end_, adj, visited, currentPath, allPaths)
        Return allPaths
    End Function

    Private Sub DFSPLS(current As Integer, end_ As Integer,
                       adj As List(Of Integer)(), visited As Boolean(),
                       currentPath As List(Of Integer),
                       allPaths As List(Of List(Of Integer)))
        visited(current) = True
        currentPath.Add(current)

        If current = end_ Then
            allPaths.Add(New List(Of Integer)(currentPath))
        Else
            For Each neighbor In adj(current)
                If Not visited(neighbor) Then
                    DFSPLS(neighbor, end_, adj, visited, currentPath, allPaths)
                End If
            Next
        End If

        currentPath.RemoveAt(currentPath.Count - 1)
        visited(current) = False
    End Sub

    ''' <summary>
    ''' PLS-PM Bootstrap 显著性检验
    ''' </summary>
    ''' 
    <Extension>
    Public Function BootstrapPLSPM(model As CausalModel,
                                   numBoot As Integer,
                                   seed As Integer) As PLSPMBootstrapResult

        Return BootstrapPLSPM(model.data, model.varNames, model.latentDefs, model.AsPathTuple.ToArray, numBoot, seed)
    End Function

    ''' <summary>
    ''' PLS-PM Bootstrap 显著性检验
    ''' </summary>
    Public Function BootstrapPLSPM(manifestData As Double(,),
                                   manifestNames As String(),
                                   latentVars As LatentDefinition(),
                                   innerPaths As (fromIdx As Integer, toIdx As Integer)(),
                                   numBoot As Integer,
                                   seed As Integer) As PLSPMBootstrapResult

        Dim rng As New Random(seed)
        Dim result As New PLSPMBootstrapResult()
        Dim manifestIndex As Index(Of String) = manifestNames.Indexing

        ' 基线结果
        Dim baseResult = FitPLSPM(manifestData, manifestIndex, latentVars, innerPaths)

        ' 收集键
        Dim pathKeys As New List(Of (Integer, Integer))
        For Each k In baseResult.PathCoefficients.Keys
            pathKeys.Add(k)
        Next

        Dim indirectKeys As New List(Of (Integer, Integer))
        For Each k In baseResult.IndirectEffects.Keys
            indirectKeys.Add(k)
        Next

        Dim loadingsKeys As New List(Of (Integer, Integer))
        For j = 0 To latentVars.Count - 1
            For k = 0 To latentVars(j).featureIDs.Length - 1
                loadingsKeys.Add((j, k))
            Next
        Next

        ' 存储样本
        Dim pathSamples As New Dictionary(Of (Integer, Integer), List(Of Double))
        For Each k In pathKeys
            pathSamples(k) = New List(Of Double)()
        Next

        Dim indirectSamples As New Dictionary(Of (Integer, Integer), List(Of Double))
        For Each k In indirectKeys
            indirectSamples(k) = New List(Of Double)()
        Next

        Dim loadingsSamples As New Dictionary(Of (Integer, Integer), List(Of Double))
        For Each k In loadingsKeys
            loadingsSamples(k) = New List(Of Double)()
        Next

        Call VBDebugger.EchoLine($"make bootstrap(nboot={numBoot}) of the PLS-PM...")

        ' Bootstrap 循环
        For Each b As Integer In TqdmWrapper.Range(0, numBoot)
            Dim bootData = Statistics.BootstrapSample(manifestData, rng)
            Try
                Dim bootResult = FitPLSPM(bootData, manifestIndex, latentVars, innerPaths)

                For Each k In pathKeys
                    If bootResult.PathCoefficients.ContainsKey(k) Then
                        pathSamples(k).Add(bootResult.PathCoefficients(k))
                    Else
                        pathSamples(k).Add(0.0)
                    End If
                Next

                For Each k In indirectKeys
                    If bootResult.IndirectEffects.ContainsKey(k) Then
                        indirectSamples(k).Add(bootResult.IndirectEffects(k))
                    Else
                        indirectSamples(k).Add(0.0)
                    End If
                Next

                For Each k In loadingsKeys
                    If bootResult.Loadings(k.Item1).Length > k.Item2 Then
                        loadingsSamples(k).Add(bootResult.Loadings(k.Item1)(k.Item2))
                    Else
                        loadingsSamples(k).Add(0.0)
                    End If
                Next
            Catch ex As Exception
                Continue For
            End Try
        Next

        ' 计算标准误和置信区间
        result.PathBootSE = New Dictionary(Of (Integer, Integer), Double)
        result.PathBootCI = New Dictionary(Of (Integer, Integer), (Double, Double))
        For Each k In pathKeys
            Dim samples = pathSamples(k).ToArray()
            If samples.Length > 0 Then
                result.PathBootSE(k) = samples.SD
                result.PathBootCI(k) = (Statistics.Quantile(samples, 0.025), Statistics.Quantile(samples, 0.975))
            End If
        Next

        result.IndirectBootSE = New Dictionary(Of (Integer, Integer), Double)
        result.IndirectBootCI = New Dictionary(Of (Integer, Integer), (Double, Double))
        For Each k In indirectKeys
            Dim samples = indirectSamples(k).ToArray()
            If samples.Length > 0 Then
                result.IndirectBootSE(k) = samples.SD
                result.IndirectBootCI(k) = (Statistics.Quantile(samples, 0.025), Statistics.Quantile(samples, 0.975))
            End If
        Next

        result.LoadingsBootSE = New Dictionary(Of (Integer, Integer), Double)
        result.LoadingsBootCI = New Dictionary(Of (Integer, Integer), (Double, Double))
        For Each k In loadingsKeys
            Dim samples = loadingsSamples(k).ToArray()
            If samples.Length > 0 Then
                result.LoadingsBootSE(k) = samples.SD
                result.LoadingsBootCI(k) = (Statistics.Quantile(samples, 0.025), Statistics.Quantile(samples, 0.975))
            End If
        Next

        result.NumBootstraps = numBoot
        Return result
    End Function

    ''' <summary>打印 PLS-PM 结果</summary>
    Public Sub PrintPLSPMResult(result As PLSPMResult, manifestNames As String(), Optional bootResult As PLSPMBootstrapResult = Nothing)
        Console.WriteLine("="c, 80)
        Console.WriteLine("偏最小二乘路径建模 (PLS-PM) 结果")
        Console.WriteLine("="c, 80)
        Console.WriteLine($"样本量 N = {result.SampleSize}, 潜变量数 = {result.NumLatents}, 显变量数 = {result.NumManifest}")
        Console.WriteLine($"迭代收敛次数 = {result.NumIterations}")
        Console.WriteLine()

        ' 1. 外模型（测量模型）
        Console.WriteLine("-"c, 80)
        Console.WriteLine("一、外模型 (测量模型) - 载荷与权重")
        Console.WriteLine("-"c, 80)
        For j = 0 To result.NumLatents - 1
            Dim lv = result.LatentDefs(j)
            Console.WriteLine($"潜变量 [{lv.varName}] (Mode {lv.mode.Description}):")
            Console.WriteLine($"  {"显变量",-20}{"载荷",12}{"权重",12}{"共同度",12}")
            For k = 0 To lv.featureIDs.Length - 1
                Dim mvName = lv.featureIDs(k)
                ' Dim mvName = manifestNames(mvIdx)
                Dim load = result.Loadings(j)(k)
                Dim w = result.FinalOuterWeights(j)(k)
                Dim comm = load * load
                Console.WriteLine($"  {mvName,-20}{load,12:F4}{w,12:F4}{comm,12:F4}")
            Next
            Console.WriteLine($"  -> 块共同度 = {result.Communalities(j):F4}")
            Console.WriteLine()
        Next

        ' 2. 内模型（结构模型）
        Console.WriteLine("-"c, 80)
        Console.WriteLine("二、内模型 (结构模型) - 路径系数")
        Console.WriteLine("-"c, 80)
        Console.WriteLine($"{"路径",-30}{"系数",12}{"SE",12}{"t值",10}{"p值",10}{"显著",8}")
        Console.WriteLine("-"c, 80)
        For Each kv In result.PathCoefficients
            Dim fromName = result.LatentNames(kv.Key.Item1)
            Dim toName = result.LatentNames(kv.Key.Item2)
            Dim coef = kv.Value
            Dim se = result.StdErrors(kv.Key)
            Dim t = result.TValues(kv.Key)
            Dim p = result.PValues(kv.Key)
            Dim sig = If(p < 0.001, "***", If(p < 0.01, "**", If(p < 0.05, "*", "ns")))
            Console.WriteLine($"{fromName + " -> " + toName,-30}{coef,12:F4}{se,12:F4}{t,10:F3}{p,10:F4}{sig,8}")
        Next
        Console.WriteLine($"注: *** p<0.001, ** p<0.01, * p<0.05, ns 不显著")
        Console.WriteLine()

        ' 3. R² 与冗余度
        Console.WriteLine("-"c, 80)
        Console.WriteLine("三、内生潜变量 R² 与冗余度")
        Console.WriteLine("-"c, 80)
        Console.WriteLine($"{"潜变量",-25}{"R²",12}{"共同度",12}{"冗余度",12}")
        For j = 0 To result.NumLatents - 1
            Dim r2 = If(result.RSquared.ContainsKey(j), result.RSquared(j), 0.0)
            Dim comm = result.Communalities(j)
            Dim red = result.Redundancies(j)
            Console.WriteLine($"{result.LatentNames(j),-25}{r2,12:F4}{comm,12:F4}{red,12:F4}")
        Next
        Console.WriteLine()

        ' 4. 效应分解
        Console.WriteLine("-"c, 80)
        Console.WriteLine("四、效应分解 (直接 / 间接 / 总效应)")
        Console.WriteLine("-"c, 80)
        Console.WriteLine($"{"路径",-30}{"直接",12}{"间接",12}{"总效应",12}")
        Console.WriteLine("-"c, 80)
        Dim allPairs As New HashSet(Of (Integer, Integer))
        For Each k In result.DirectEffects.Keys : allPairs.Add(k) : Next
        For Each k In result.IndirectEffects.Keys : allPairs.Add(k) : Next
        For Each k In result.TotalEffects.Keys : allPairs.Add(k) : Next
        For Each k In allPairs
            Dim fromName = result.LatentNames(k.Item1)
            Dim toName = result.LatentNames(k.Item2)
            Dim d = If(result.DirectEffects.ContainsKey(k), result.DirectEffects(k), 0.0)
            Dim i = If(result.IndirectEffects.ContainsKey(k), result.IndirectEffects(k), 0.0)
            Dim t = If(result.TotalEffects.ContainsKey(k), result.TotalEffects(k), 0.0)
            Console.WriteLine($"{fromName + " -> " + toName,-30}{d,12:F4}{i,12:F4}{t,12:F4}")
        Next
        Console.WriteLine()

        ' 5. Bootstrap
        If bootResult IsNot Nothing Then
            Console.WriteLine("-"c, 80)
            Console.WriteLine($"五、Bootstrap 显著性检验 (重采样次数 = {bootResult.NumBootstraps})")
            Console.WriteLine("-"c, 80)
            Console.WriteLine("路径系数 Bootstrap:")
            Console.WriteLine($"{"路径",-30}{"系数",12}{"BootSE",12}{"95%CI下",12}{"95%CI上",12}{"显著",8}")
            Console.WriteLine("-"c, 80)
            For Each kv In result.PathCoefficients
                Dim fromName = result.LatentNames(kv.Key.Item1)
                Dim toName = result.LatentNames(kv.Key.Item2)
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
                    Dim fromName = result.LatentNames(kv.Key.Item1)
                    Dim toName = result.LatentNames(kv.Key.Item2)
                    Dim ind = kv.Value
                    Dim bse = If(bootResult.IndirectBootSE.ContainsKey(kv.Key), bootResult.IndirectBootSE(kv.Key), 0.0)
                    Dim ci = If(bootResult.IndirectBootCI.ContainsKey(kv.Key), bootResult.IndirectBootCI(kv.Key), (0.0, 0.0))
                    Dim sig = If((ci.Item1 > 0 AndAlso ci.Item2 > 0) OrElse (ci.Item1 < 0 AndAlso ci.Item2 < 0), "Yes", "No")
                    Console.WriteLine($"{fromName + " -> " + toName,-30}{ind,12:F4}{bse,12:F4}{ci.Item1,12:F4}{ci.Item2,12:F4}{sig,8}")
                Next
                Console.WriteLine()
            End If
        End If

        ' 6. GoF
        Console.WriteLine("-"c, 80)
        Console.WriteLine("六、模型整体拟合")
        Console.WriteLine("-"c, 80)
        Console.WriteLine($"  GoF (Goodness of Fit) = {result.GoF:F4}  (良好 > 0.36, 中等 > 0.25, 弱 > 0.10)")
        Console.WriteLine()
    End Sub

End Module

''' <summary>PLS-PM 结果</summary>
Public Class PLSPMResult
    Public Property SampleSize As Integer
    Public Property NumLatents As Integer
    Public Property NumManifest As Integer
    Public Property NumIterations As Integer
    Public Property LatentNames As String()
    Public Property LatentDefs As LatentDefinition()
    Public Property InnerPaths As (from As Integer, [to] As Integer)()

    Public Property StandardizedData As Double(,)
    Public Property LatentScores As Double(,)
    Public Property OuterWeights As List(Of Double())
    Public Property FinalOuterWeights As List(Of Double())
    Public Property Loadings As List(Of Double())
    Public Property Communalities As List(Of Double)
    Public Property Redundancies As List(Of Double)

    Public Property PathCoefficients As Dictionary(Of (Integer, Integer), Double)
    Public Property StdErrors As Dictionary(Of (Integer, Integer), Double)
    Public Property TValues As Dictionary(Of (Integer, Integer), Double)
    Public Property PValues As Dictionary(Of (Integer, Integer), Double)
    Public Property RSquared As Dictionary(Of Integer, Double)

    Public Property DirectEffects As Dictionary(Of (Integer, Integer), Double)
    Public Property IndirectEffects As Dictionary(Of (Integer, Integer), Double)
    Public Property TotalEffects As Dictionary(Of (Integer, Integer), Double)

    Public Property GoF As Double
End Class

''' <summary>PLS-PM Bootstrap 结果</summary>
Public Class PLSPMBootstrapResult : Inherits BootstrapResult

    Public Property LoadingsBootSE As Dictionary(Of (Integer, Integer), Double)
    Public Property LoadingsBootCI As Dictionary(Of (Integer, Integer), (Double, Double))

End Class
