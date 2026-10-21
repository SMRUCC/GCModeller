Imports Microsoft.VisualBasic.Data.Bootstrapping.ElasticNet
Imports std = System.Math

Namespace LinearRegression

    ''' <summary>
    ''' 弹性网络在单个目标基因上面的拟合结果
    ''' 
    ''' 这里的系数与截距都已经还原到原始的表达量尺度之上，可以直接与
    ''' 普通最小二乘、LASSO 的结果进行横向比较。
    ''' </summary>
    Friend Class ElasticNetFitResult

        ''' <summary>
        ''' 模型的常数项
        ''' </summary>
        ''' <returns></returns>
        Friend Property Intercept As Double

        ''' <summary>
        ''' 每一个调控因子的回归系数
        ''' </summary>
        ''' <returns></returns>
        Friend Property Weights As Double()

        ''' <summary>
        ''' 拟合优度 R^2
        ''' </summary>
        ''' <returns></returns>
        Friend Property R2 As Double

        ''' <summary>
        ''' 残差平方和
        ''' </summary>
        ''' <returns></returns>
        Friend Property SSE As Double

        ''' <summary>
        ''' 总平方和
        ''' </summary>
        ''' <returns></returns>
        Friend Property SST As Double

        ''' <summary>
        ''' 最终选用的正则化强度
        ''' </summary>
        ''' <returns></returns>
        Friend Property Alpha As Double

        ''' <summary>
        ''' 最终选用的 L1 混合比例
        ''' </summary>
        ''' <returns></returns>
        Friend Property L1Ratio As Double

        ''' <summary>
        ''' 非零回归系数的数量
        ''' </summary>
        ''' <returns></returns>
        Friend Property NonZero As Integer

        ''' <summary>
        ''' 坐标下降是否在迭代上限之内收敛
        ''' </summary>
        ''' <returns></returns>
        Friend Property Converged As Boolean

        ''' <summary>
        ''' 正则化路径的步数(单点拟合的时候为 1)
        ''' </summary>
        ''' <returns></returns>
        Friend Property Steps As Integer
    End Class

    ''' <summary>
    ''' 弹性网络求解器在基因调控网络推断之上的封装
    ''' 
    ''' 这个模块只负责「给定设计矩阵与观测值，求出一组回归系数」，
    ''' 候选调控因子的筛选、并行调度以及网络导出都在
    ''' <see cref="GeneRegulatoryNetwork"/> 之中完成。
    ''' </summary>
    ''' <remarks>
    ''' 输入的设计矩阵 **不包含** 常数项列：弹性网络自带截距，
    ''' 而且截距不会被惩罚。
    ''' </remarks>
    Friend Module ElasticNetRegressor

        ''' <summary>
        ''' 按照配置之中固定的 l1_ratio 求解正则化路径，并挑选一个步骤
        ''' </summary>
        ''' <param name="X">行是样本、列是调控因子的设计矩阵，不含常数项列</param>
        ''' <param name="y">观测值向量</param>
        ''' <param name="names">特征列的名称</param>
        ''' <param name="options">弹性网络的参数</param>
        ''' <param name="maxFeatures">每一个模型允许保留的最大非零系数数量，小于等于零表示不限制</param>
        ''' <returns></returns>
        Friend Function FitByPath(X As Double()(),
                                  y As Double(),
                                  names As String(),
                                  options As ElasticNetOptions,
                                  maxFeatures As Integer) As ElasticNetFitResult

            Return FitByPathWithRatio(X, y, names, options, options.l1Ratio, maxFeatures)
        End Function

        ''' <summary>
        ''' 按照指定的 l1_ratio 求解正则化路径，并挑选一个步骤
        ''' </summary>
        ''' <param name="X">设计矩阵</param>
        ''' <param name="y">观测值向量</param>
        ''' <param name="names">特征列的名称</param>
        ''' <param name="options">弹性网络的参数</param>
        ''' <param name="l1Ratio">本次求解所使用的 L1 混合比例</param>
        ''' <param name="maxFeatures">允许保留的最大非零系数数量</param>
        ''' <returns>求解失败的时候返回空引用</returns>
        Friend Function FitByPathWithRatio(X As Double()(),
                                           y As Double(),
                                           names As String(),
                                           options As ElasticNetOptions,
                                           l1Ratio As Double,
                                           maxFeatures As Integer) As ElasticNetFitResult

            Return FitByPathColumnsWithRatio(ToColumns(X), y, names, options, l1Ratio, maxFeatures)
        End Function

        ''' <summary>
        ''' 按默认的 l1_ratio 求解正则化路径，入参为列主序的特征矩阵
        ''' </summary>
        ''' <param name="columns">特征列，<c>columns(j)</c> 是第 j 个特征在所有样本之上的取值</param>
        ''' <param name="y">观测值向量</param>
        ''' <param name="names">特征列的名称</param>
        ''' <param name="options">弹性网络的参数</param>
        ''' <param name="maxFeatures">允许保留的最大非零系数数量</param>
        ''' <returns></returns>
        Friend Function FitByPathColumns(columns As Double()(),
                                         y As Double(),
                                         names As String(),
                                         options As ElasticNetOptions,
                                         maxFeatures As Integer) As ElasticNetFitResult

            Return FitByPathColumnsWithRatio(columns, y, names, options, options.l1Ratio, maxFeatures)
        End Function

        ''' <summary>
        ''' 按照指定的 l1_ratio 求解正则化路径，入参为列主序的特征矩阵。
        ''' 
        ''' 与行主序版本完全等价，但省掉了 <see cref="ToColumns"/> 的一次转置，
        ''' 并且把 <paramref name="maxFeatures"/> 传给求解器用于路径早停。
        ''' </summary>
        ''' <param name="columns">特征列</param>
        ''' <param name="y">观测值向量</param>
        ''' <param name="names">特征列的名称</param>
        ''' <param name="options">弹性网络的参数</param>
        ''' <param name="l1Ratio">本次求解所使用的 L1 混合比例</param>
        ''' <param name="maxFeatures">允许保留的最大非零系数数量</param>
        ''' <returns>求解失败的时候返回空引用</returns>
        Friend Function FitByPathColumnsWithRatio(columns As Double()(),
                                                  y As Double(),
                                                  names As String(),
                                                  options As ElasticNetOptions,
                                                  l1Ratio As Double,
                                                  maxFeatures As Integer) As ElasticNetFitResult

            Dim path As ElasticNetPath = ElasticNetFitting.RegularizationPathColumns(
                columns:=columns,
                y:=y,
                l1Ratio:=l1Ratio,
                numberOfAlphas:=options.numberOfAlphas,
                alphaRatio:=options.alphaRatio,
                standardize:=True,
                tolerance:=options.tolerance,
                maxIterations:=options.maxIterations,
                featureNames:=names,
                maxFeatures:=maxFeatures
            )

            If path Is Nothing OrElse path.Count = 0 Then
                Return Nothing
            End If

            Dim stepId As Integer = PickPathStep(path, maxFeatures)

            If stepId < 0 Then
                Return Nothing
            End If

            Return EnforceMaxFeatures(FromPath(path, stepId, y), columns, y, maxFeatures)
        End Function

        ''' <summary>
        ''' 行主序的特征矩阵转成"每个特征一列"的列主序形式。
        ''' </summary>
        ''' <param name="X">行主序特征矩阵</param>
        ''' <returns></returns>
        Private Function ToColumns(X As Double()()) As Double()()
            If X Is Nothing OrElse X.Length = 0 Then
                Return New Double()() {}
            End If

            Dim n As Integer = X.Length
            Dim p As Integer = X(0).Length
            Dim columns As Double()() = New Double(p - 1)() {}

            For j As Integer = 0 To p - 1
                Dim col As Double() = New Double(n - 1) {}

                For i As Integer = 0 To n - 1
                    col(i) = X(i)(j)
                Next

                columns(j) = col
            Next

            Return columns
        End Function

        ''' <summary>
        ''' 对单个目标基因做 K 折交叉验证，自动选择 (alpha, l1_ratio)
        ''' </summary>
        ''' <param name="X">设计矩阵</param>
        ''' <param name="y">观测值向量</param>
        ''' <param name="names">特征列的名称</param>
        ''' <param name="options">弹性网络的参数</param>
        ''' <param name="maxFeatures">允许保留的最大非零系数数量</param>
        ''' <returns></returns>
        Friend Function FitByCrossValidation(X As Double()(),
                                             y As Double(),
                                             names As String(),
                                             options As ElasticNetOptions,
                                             maxFeatures As Integer) As ElasticNetFitResult

            Dim cv As ElasticNetCVResult = CrossValidateOnce(X, y, names, options)

            If cv Is Nothing OrElse cv.Fit Is Nothing Then
                ' 交叉验证失败的时候退化为固定比例的正则化路径
                Return FitByPathWithRatio(X, y, names, options, options.l1Ratio, maxFeatures)
            End If

            If maxFeatures > 0 AndAlso cv.Fit.NonZeroCount > maxFeatures Then
                ' 交叉验证选出来的模型过于稠密：改用最优的 l1_ratio 重新跑路径选步，
                ' 保证「每个基因最多保留 maxFeatures 个调控因子」这一约束在所有模式下都成立
                Return FitByPathWithRatio(X, y, names, options, cv.BestL1Ratio, maxFeatures)
            End If

            Return FromFit(cv.Fit, cv.BestAlpha, cv.BestL1Ratio)
        End Function

        ''' <summary>
        ''' 使用指定的 (alpha, l1_ratio) 做单点拟合
        ''' </summary>
        ''' <param name="X">设计矩阵</param>
        ''' <param name="y">观测值向量</param>
        ''' <param name="names">特征列的名称</param>
        ''' <param name="options">弹性网络的参数</param>
        ''' <param name="alpha">正则化强度</param>
        ''' <param name="l1Ratio">L1 混合比例</param>
        ''' <param name="maxFeatures">允许保留的最大非零系数数量</param>
        ''' <returns></returns>
        Friend Function FitFixed(X As Double()(),
                                 y As Double(),
                                 names As String(),
                                 options As ElasticNetOptions,
                                 alpha As Double,
                                 l1Ratio As Double,
                                 maxFeatures As Integer) As ElasticNetFitResult

            If Double.IsNaN(alpha) OrElse alpha <= 0.0R Then
                Return Nothing
            End If

            Dim fit As ElasticNetFit = ElasticNetFitting.Fit(
                x:=X,
                y:=y,
                alpha:=alpha,
                l1Ratio:=l1Ratio,
                standardize:=True,
                tolerance:=options.tolerance,
                maxIterations:=options.maxIterations,
                featureNames:=names
            )

            If fit Is Nothing Then
                Return Nothing
            End If

            If maxFeatures > 0 AndAlso fit.NonZeroCount > maxFeatures Then
                ' 共享的超参数在当前基因之上过于稠密，退化为该比例下的路径选步
                Return FitByPathWithRatio(X, y, names, options, l1Ratio, maxFeatures)
            End If

            ' 单点拟合的稀疏性已经满足约束，不需要再裁剪

            Return FromFit(fit, alpha, l1Ratio)
        End Function

        ''' <summary>
        ''' 执行一次交叉验证，返回完整的交叉验证结果
        ''' </summary>
        ''' <param name="X">设计矩阵</param>
        ''' <param name="y">观测值向量</param>
        ''' <param name="names">特征列的名称</param>
        ''' <param name="options">弹性网络的参数</param>
        ''' <returns></returns>
        Friend Function CrossValidateOnce(X As Double()(),
                                          y As Double(),
                                          names As String(),
                                          options As ElasticNetOptions) As ElasticNetCVResult

            Return ElasticNetFitting.CrossValidate(
                x:=X,
                y:=y,
                l1Ratios:=options.cvL1Ratios,
                numberOfAlphas:=options.cvAlphaSteps,
                folds:=options.NormalizedFolds(y.Length),
                standardize:=True,
                tolerance:=options.tolerance,
                maxIterations:=options.maxIterations,
                featureNames:=names
            )
        End Function

        ''' <summary>
        ''' 在正则化路径之上挑选一个步骤
        ''' 
        ''' 挑选规则：在「非零系数数量不超过 <paramref name="maxFeatures"/>」的步骤之中，
        ''' 取 R^2 最高的那一步；如果没有满足约束的步骤，则退化为非零系数数量最少的
        ''' 那一步(而不是路径的末端，末端的模型往往过于稠密)。
        ''' </summary>
        ''' <param name="path">正则化路径</param>
        ''' <param name="maxFeatures">允许保留的最大非零系数数量，小于等于零表示不限制</param>
        ''' <returns>步骤的下标；路径为空的时候返回 -1</returns>
        Friend Function PickPathStep(path As ElasticNetPath, maxFeatures As Integer) As Integer
            If path Is Nothing OrElse path.Count = 0 Then
                Return -1
            End If

            Dim bestId As Integer = -1
            Dim bestR2 As Double = Double.NegativeInfinity

            For k As Integer = 0 To path.Count - 1
                If path.NonZeroCounts(k) = 0 Then
                    Continue For
                End If
                If maxFeatures > 0 AndAlso path.NonZeroCounts(k) > maxFeatures Then
                    Continue For
                End If

                Dim r2 As Double = path.RSquared(k)

                If Double.IsNaN(r2) OrElse Double.IsInfinity(r2) Then
                    Continue For
                End If
                If r2 > bestR2 Then
                    bestR2 = r2
                    bestId = k
                End If
            Next

            If bestId >= 0 Then
                Return bestId
            End If

            ' 没有任何一步满足 maxFeatures 约束：退化为"解释力最强"的那一步，
            ' 后面由 EnforceMaxFeatures 裁剪到 maxFeatures。
            ' 
            ' 注意不要退化成"非零系数数量最少的那一步"：在近似共线很强的数据上，
            ' 路径第二步就会涌入成百上千个系数，而 alpha 最大处的第一步只剩下
            ' 浮点噪声级别的系数 —— 取它等于得到一个 r2 = 0 的常数模型，
            ' 后续虚拟扰动当然也就完全没有响应。
            Dim fallbackId As Integer = -1
            Dim fallbackR2 As Double = Double.NegativeInfinity

            For k As Integer = 0 To path.Count - 1
                If path.NonZeroCounts(k) = 0 Then
                    Continue For
                End If

                Dim r2 As Double = path.RSquared(k)

                If Double.IsNaN(r2) OrElse Double.IsInfinity(r2) Then
                    Continue For
                End If
                If r2 > fallbackR2 Then
                    fallbackR2 = r2
                    fallbackId = k
                End If
            Next

            Return fallbackId
        End Function

        ''' <summary>
        ''' 从多个基因的交叉验证结果之中取超参数的中位数
        ''' 
        ''' alpha 跨越多个数量级，因此取的是对数尺度之上的中位数(几何中位数)；
        ''' l1_ratio 直接取算术中位数。
        ''' </summary>
        ''' <param name="results">各个代表性基因的交叉验证结果</param>
        ''' <returns>
        ''' 中位数超参数；没有可用的交叉验证结果的时候返回 ``(-1, -1)``。
        ''' </returns>
        Friend Function MedianHyperparameters(results As IEnumerable(Of ElasticNetCVResult)) As (alpha As Double, l1Ratio As Double)
            If results Is Nothing Then
                Return (-1.0R, -1.0R)
            End If

            Dim valid As ElasticNetCVResult() = results _
                .Where(Function(cv) cv IsNot Nothing AndAlso cv.Fit IsNot Nothing) _
                .Where(Function(cv) IsFinite(cv.BestAlpha) AndAlso cv.BestAlpha > 0.0R) _
                .ToArray

            If valid.Length = 0 Then
                Return (-1.0R, -1.0R)
            End If

            Dim l1 As Double = Median(valid.Select(Function(cv) cv.BestL1Ratio))
            Dim logAlpha As Double = Median(valid.Select(Function(cv) std.Log(cv.BestAlpha)))

            Return (std.Exp(logAlpha), l1)
        End Function

        ''' <summary>
        ''' 强制满足「每个基因最多保留 maxFeatures 个调控因子」的约束
        ''' 
        ''' 正则化路径上相邻两步的非零系数数量可能发生跳跃，当路径之上没有任何一步
        ''' 落在约束范围之内的时候(常见于样本很少、噪声很大的数据)，
        ''' 这里直接保留绝对值最大的若干项，并重新计算截距与拟合优度，
        ''' 以保证「每个基因最多 k 个上游调控因子」这一网络语义不被破坏。
        ''' </summary>
        ''' <param name="result">路径上面挑选出来的解</param>
        ''' <param name="columns">列主序的特征矩阵</param>
        ''' <param name="y">观测值向量</param>
        ''' <param name="maxFeatures">允许保留的最大非零系数数量，小于等于零表示不限制</param>
        ''' <returns></returns>
        Private Function EnforceMaxFeatures(result As ElasticNetFitResult,
                                            columns As Double()(),
                                            y As Double(),
                                            maxFeatures As Integer) As ElasticNetFitResult

            If result Is Nothing OrElse maxFeatures <= 0 Then
                Return result
            End If
            If result.Weights Is Nothing OrElse result.Weights.Length = 0 Then
                Return result
            End If
            If result.NonZero <= maxFeatures Then
                Return result
            End If

            Dim keep As Integer() = Enumerable _
                .Range(0, result.Weights.Length) _
                .Where(Function(j) result.Weights(j) <> 0.0R) _
                .OrderByDescending(Function(j) std.Abs(result.Weights(j))) _
                .Take(maxFeatures) _
                .ToArray

            If keep.Length = 0 Then
                Return result
            End If

            Dim weights As Double() = New Double(result.Weights.Length - 1) {}

            For Each j As Integer In keep
                weights(j) = result.Weights(j)
            Next

            ' 截断之后模型发生变化，需要重新计算截距与拟合优度
            Dim n As Integer = y.Length
            Dim p As Integer = weights.Length
            Dim means As Double() = New Double(p - 1) {}
            Dim intercept As Double = y.Average

            For j As Integer = 0 To p - 1
                Dim col As Double() = columns(j)
                Dim sum As Double = 0.0R

                For i As Integer = 0 To n - 1
                    sum += col(i)
                Next

                means(j) = sum / n
                intercept -= weights(j) * means(j)
            Next

            Dim sse As Double = 0.0R
            Dim yhat As Double() = New Double(n - 1) {}

            ' yhat 按列累加：yhat += w_j * column_j，一次遍历一整列
            For j As Integer = 0 To p - 1
                If weights(j) = 0.0R Then
                    Continue For
                End If

                Dim col As Double() = columns(j)
                Dim w As Double = weights(j)

                For i As Integer = 0 To n - 1
                    yhat(i) += w * col(i)
                Next
            Next

            For i As Integer = 0 To n - 1
                sse += (y(i) - (intercept + yhat(i))) ^ 2
            Next

            Dim sst As Double = RegressionStatistics.TotalSumOfSquares(y)

            result.Weights = weights
            result.Intercept = intercept
            result.NonZero = keep.Length
            result.SSE = sse
            result.SST = sst
            result.R2 = RegressionStatistics.Rsquared(sse, sst)

            Return result
        End Function

        ''' <summary>
        ''' 从正则化路径的一个步骤构造拟合结果
        ''' </summary>
        Private Function FromPath(path As ElasticNetPath, stepId As Integer, y As Double()) As ElasticNetFitResult
            Dim r2 As Double = path.RSquared(stepId)
            Dim sst As Double = RegressionStatistics.TotalSumOfSquares(y)
            Dim result As New ElasticNetFitResult With {
                .Intercept = path.Intercepts(stepId),
                .Weights = path.getCoefficients(stepId),
                .R2 = r2,
                .Alpha = path.Alphas(stepId),
                .L1Ratio = path.L1Ratio,
                .NonZero = path.NonZeroCounts(stepId),
                .Converged = True,
                .Steps = path.Count,
                .SST = sst
            }

            If IsFinite(r2) AndAlso sst > 0.0R Then
                result.SSE = sst * (1.0R - r2)
            Else
                result.SSE = 0.0R
            End If

            Return result
        End Function

        ''' <summary>
        ''' 从单点拟合的结果构造拟合结果
        ''' </summary>
        Private Function FromFit(fit As ElasticNetFit, alpha As Double, l1Ratio As Double) As ElasticNetFitResult
            Return New ElasticNetFitResult With {
                .Intercept = fit.Intercept,
                .Weights = fit.Coefficients,
                .R2 = fit.R2,
                .SSE = fit.SSE,
                .SST = fit.SST,
                .Alpha = alpha,
                .L1Ratio = l1Ratio,
                .NonZero = fit.NonZeroCount,
                .Converged = fit.Converged,
                .Steps = 1
            }
        End Function

        ''' <summary>
        ''' 计算一组数值的中位数
        ''' </summary>
        Private Function Median(values As IEnumerable(Of Double)) As Double
            Dim sorted As Double() = values.OrderBy(Function(x) x).ToArray

            If sorted.Length = 0 Then
                Return 0.0R
            End If

            Dim mid As Integer = sorted.Length \ 2

            If sorted.Length Mod 2 = 1 Then
                Return sorted(mid)
            Else
                Return (sorted(mid - 1) + sorted(mid)) / 2.0R
            End If
        End Function

        ''' <summary>
        ''' 判断数值是否为一个有限值
        ''' </summary>
        Private Function IsFinite(x As Double) As Boolean
            Return Not Double.IsNaN(x) AndAlso Not Double.IsInfinity(x)
        End Function
    End Module
End Namespace
