Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports std = System.Math

Namespace Ridge

    ''' <summary>
    ''' 多响应岭回归的拟合结果：一组共享设计矩阵、多个响应列的系数矩阵。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 模型是<b>无截距</b>的线性模型 <c>Y ≈ X·B</c>（<c>X</c> 为 <c>n×f</c>、<c>B</c> 为 <c>f×k</c>）。
    ''' 之所以不单独估计截距：上游（CellularAI 的残差校正）把常数项当作普通特征列参与中心化，
    ''' 并且特征与响应本身都已居中，额外再加一个截距会破坏"同一列在训练与预测上语义一致"的前提。
    ''' </para>
    ''' <para>
    ''' <b>R² 的口径</b>：无截距模型的 <see cref="RSquared"/> 用<b>未中心化</b>的总平方和
    ''' （<c>Σy²</c>）作为分母，即 <c>1 − RSS/Σy²</c>；这与"有截距模型用 <c>Σ(y−ȳ)²</c>"
    ''' 不同，读数值时请按这个定义解释。
    ''' </para>
    ''' </remarks>
    Public Class MultiResponseRidgeFit

        ''' <summary>
        ''' 系数矩阵（<c>f×k</c>）：行对应特征，列对应响应。
        ''' </summary>
        Public Property Coefficients As Double()()

        ''' <summary>
        ''' 特征中心向量（长度 f）；<c>center:=False</c> 时全为 0。
        ''' </summary>
        ''' <remarks>
        ''' 预测时必须使用<b>同一个</b>中心向量（<c>x − FeatureCenter</c>），
        ''' 否则训练与推理的坐标系不一致。
        ''' </remarks>
        Public Property FeatureCenter As Double()

        ''' <summary>
        ''' 使用的正则化强度 λ（作用在 <c>XᵀX + λI</c> 的对角线上）。
        ''' </summary>
        Public Property Lambda As Double

        ''' <summary>
        ''' 实际参与拟合的样本行数（行掩码为 False 的行不计入）。
        ''' </summary>
        Public Property SampleCount As Integer

        ''' <summary>
        ''' 法方程 <c>XᵀX + λI</c> 的数值秩（等于 f 表示正定）。
        ''' </summary>
        Public Property Rank As Integer

        ''' <summary>
        ''' 每个响应的残差平方和。
        ''' </summary>
        Public Property RSS As Double()

        ''' <summary>
        ''' 每个响应的未中心化 R²（<c>1 − RSS/Σy²</c>）。
        ''' </summary>
        Public Property RSquared As Double()

        ''' <summary>
        ''' 帽子矩阵的有效自由度 <c>tr(H)</c>，其中 <c>H = X(XᵀX+λI)⁻¹Xᵀ</c>。
        ''' </summary>
        ''' <remarks>
        ''' λ → 0 时趋近于 f（每个特征各占一个自由度），λ → ∞ 时趋近于 0。
        ''' </remarks>
        Public Property DegreesOfFreedom As Double

        ''' <summary>
        ''' 该组系数是否由有效的拟合产生。<c>False</c> 表示样本数不足或法方程不可解，
        ''' 此时 <see cref="Coefficients"/> 全为 0，调用方应跳过该响应组。
        ''' </summary>
        Public Property Supported As Boolean

        ''' <summary>
        ''' 特征数 f。
        ''' </summary>
        Public ReadOnly Property FeatureCount As Integer
            Get
                Return If(FeatureCenter Is Nothing, 0, FeatureCenter.Length)
            End Get
        End Property

        ''' <summary>
        ''' 响应数 k。
        ''' </summary>
        Public ReadOnly Property ResponseCount As Integer
            Get
                Return If(RSquared Is Nothing, 0, RSquared.Length)
            End Get
        End Property

        ''' <summary>
        ''' 对单个样本做预测：<c>ŷ = (x − center)·B</c>，返回长度 k 的向量。
        ''' </summary>
        ''' <param name="x">特征向量，长度必须等于 <see cref="FeatureCount"/></param>
        ''' <returns>长度 <see cref="ResponseCount"/> 的预测值</returns>
        Public Function Predict(x As Double()) As Double()
            If x Is Nothing Then
                Throw New ArgumentNullException(NameOf(x))
            End If
            If x.Length <> FeatureCount Then
                Throw New ArgumentException(
                    $"the feature vector has {x.Length} value(s), but {FeatureCount} is required", NameOf(x))
            End If

            Dim k As Integer = ResponseCount
            Dim y As Double() = New Double(If(k = 0, -1, k - 1)) {}

            For c As Integer = 0 To k - 1
                Dim acc As Double = 0.0

                For j As Integer = 0 To FeatureCount - 1
                    acc += (x(j) - FeatureCenter(j)) * Coefficients(j)(c)
                Next

                y(c) = acc
            Next

            Return y
        End Function

        ''' <summary>
        ''' 对一批样本做预测（每行一个样本）。
        ''' </summary>
        Public Function Predict(x As Double()()) As Double()()
            If x Is Nothing Then
                Throw New ArgumentNullException(NameOf(x))
            End If

            Dim result As Double()() = RectangularArray.Matrix(Of Double)(x.Length, ResponseCount)

            For i As Integer = 0 To x.Length - 1
                result(i) = Me.Predict(x(i))
            Next

            Return result
        End Function

        Public Overrides Function ToString() As String
            Return $"[{FeatureCount} feature x {ResponseCount} response] ridge, lambda={Lambda:G4}, n={SampleCount}, rank={Rank}, supported={Supported}"
        End Function

    End Class

    ''' <summary>
    ''' 多响应岭回归（multi-response ridge regression）的闭式求解与逐基因批量入口。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>模型</b>：<c>min ‖Y − X·B‖_F² + λ‖B‖_F²</c>，其中 <c>X</c> 为 <c>n×f</c>、
    ''' <c>Y</c> 为 <c>n×k</c>、<c>B</c> 为 <c>f×k</c>。正规方程为
    ''' </para>
    ''' <code>
    ''' (XᵀX + λI)·B = XᵀY
    ''' </code>
    ''' <para>
    ''' 因此<b>一次求解就能同时得到 k 个响应的系数</b>——k 个响应共享同一个 <c>f×f</c> 的系数矩阵，
    ''' 计算量从"k 次独立的 f×f 求解"降到"一次分解 + k 次回代"。
    ''' λ 加在<b>全部 f 列</b>的对角线上（不豁免常数项列），这一点与上游一致：
    ''' 上游把常数项当作普通特征列参与中心化，再对整个列块统一加惩罚。
    ''' </para>
    ''' <para>
    ''' <b>行掩码</b>是这套实现的必要能力：上游（CellularAI 的 <c>ControlResidual.fit</c>）
    ''' 对每个基因只在"该记录确实测到、且控制侧也观测到"的那些行上回归，
    ''' 因此 <c>X</c>、<c>Y</c> 的行集合逐基因不同。掩码为 <c>False</c> 的行
    ''' 既不参与中心化均值，也不进入法方程。
    ''' </para>
    ''' <para>
    ''' <b>数值路径</b>：<c>XᵀX + λI</c> 在 <c>λ &gt; 0</c> 时理论上正定，优先用 Cholesky；
    ''' 若因极小的 λ 或强共线导致 <c>SPD = False</c>，回退到 <see cref="NumericMatrix.Solve(GeneralMatrix)"/>
    ''' （方阵走 LU、非方阵走 QR）。两条路径都失败时，
    ''' <see cref="Fit"/> 抛 <see cref="InvalidOperationException"/> 提示增大 λ；
    ''' <see cref="BatchFit"/> 则把该组标记为 <see cref="MultiResponseRidgeFit.Supported"/> = False
    ''' 并保留全零系数（与上游"系数初值为 0"的行为一致）。
    ''' </para>
    ''' <para>
    ''' <b>并行</b>：<see cref="BatchFit"/> 按"组"（基因）并行，每组只写自己的结果槽位，
    ''' 没有共享累加器，因此结果与并行度无关（逐位可复现）；工作量较小时自动退化为串行。
    ''' </para>
    ''' </remarks>
    Public Module MultiResponseRidge

        ''' <summary>
        ''' 默认的正则化强度（对应上游 CellularAI 的 <c>residual_ridge = 10</c>）。
        ''' </summary>
        Public Const DefaultLambda As Double = 10.0

        ''' <summary>
        ''' 默认的最小样本数门限（对应上游的 <c>residual_min_measured = 32</c>）。
        ''' </summary>
        Public Const DefaultMinimumRows As Integer = 32

        ''' <summary>
        ''' 串行/并行的切换阈值：总工作量（组数 × 行数 × 特征数）低于该值时串行。
        ''' </summary>
        Private Const ParallelWorkThreshold As Long = 2000000L

        ''' <summary>
        ''' 对一组多响应数据做闭式岭回归。
        ''' </summary>
        ''' <param name="x">设计矩阵（<c>n×f</c>），每一行是一个样本</param>
        ''' <param name="y">响应矩阵（<c>n×k</c>），每一行对应 <paramref name="x"/> 的同一行</param>
        ''' <param name="mask">
        ''' 行掩码（长度 n）；<c>Nothing</c> 表示全部参与。
        ''' <c>False</c> 的行既不参与中心化均值，也不进入法方程。
        ''' </param>
        ''' <param name="lambda">正则化强度，必须为正</param>
        ''' <param name="center">是否按列中心化（在参与行上求均值）；默认 True</param>
        ''' <returns>拟合结果（系数、特征中心、诊断量）</returns>
        Public Function Fit(x As Double()(), y As Double()(),
                            Optional mask As Boolean() = Nothing,
                            Optional lambda As Double = DefaultLambda,
                            Optional center As Boolean = True) As MultiResponseRidgeFit

            Dim model As MultiResponseRidgeFit = Solve(x, y, mask, lambda, center)

            If Not model.Supported Then
                Throw New InvalidOperationException(
                    $"the ridge system can not be solved (lambda = {lambda:G6}, n = {model.SampleCount}); " &
                    "please increase the lambda value or provide more rows")
            End If

            Return model
        End Function

        ''' <summary>
        ''' 一次求解的便捷入口：返回系数矩阵本体。
        ''' </summary>
        Public Function Coefficients(x As Double()(), y As Double()(),
                                     Optional mask As Boolean() = Nothing,
                                     Optional lambda As Double = DefaultLambda,
                                     Optional center As Boolean = True) As Double()()
            Return Fit(x, y, mask, lambda, center).Coefficients
        End Function

        ''' <summary>
        ''' 按已有模型预测（等价于 <see cref="MultiResponseRidgeFit.Predict(Double()())"/> 的转发入口）。
        ''' </summary>
        Public Function Predict(fit As MultiResponseRidgeFit, x As Double()()) As Double()()
            If fit Is Nothing Then
                Throw New ArgumentNullException(NameOf(fit))
            End If

            Return fit.Predict(x)
        End Function

        ''' <summary>
        ''' 逐组（逐基因）批量拟合：每组各自一套行掩码与法方程。
        ''' </summary>
        ''' <param name="features">
        ''' 三维锯齿数组：<c>features(组)(行)(特征)</c>，即每一组是一个 <c>n×f</c> 的设计矩阵。
        ''' 对于 CellularAI 的残差校正，组是"基因"、行是"训练记录"、特征是"控制表达特征"。
        ''' </param>
        ''' <param name="responses">
        ''' 三维锯齿数组：<c>responses(组)(行)(响应)</c>，与 <paramref name="features"/> 的行一一对应。
        ''' </param>
        ''' <param name="support">
        ''' 行掩码（<c>行×组</c>）：<c>support(record, gene)</c> 为 <c>True</c> 表示该行参与该组的拟合。
        ''' </param>
        ''' <param name="lambda">正则化强度，必须为正</param>
        ''' <param name="minimumRows">每组至少需要的参与行数；不足时该组标记为不支持</param>
        ''' <param name="degreeOfParallelism">并行度：0 = 全部逻辑核心，1 = 串行</param>
        ''' <returns>与组数等长的拟合结果数组（下标与输入一致）</returns>
        ''' <remarks>
        ''' 内存占用为 <c>组数 × 行数 × 特征数 × 8</c> 字节（例如 18000 组 × 512 行 × 6 特征 ≈ 440 MB），
        ''' 规模更大时请由调用方自行分块后多次调用。
        ''' </remarks>
        Public Function BatchFit(features As Double()()(),
                                 responses As Double()()(),
                                 support As Boolean(,),
                                 lambda As Double,
                                 Optional minimumRows As Integer = DefaultMinimumRows,
                                 Optional degreeOfParallelism As Integer = 0) As MultiResponseRidgeFit()

            If features Is Nothing Then
                Throw New ArgumentNullException(NameOf(features))
            End If
            If responses Is Nothing Then
                Throw New ArgumentNullException(NameOf(responses))
            End If
            If support Is Nothing Then
                Throw New ArgumentNullException(NameOf(support))
            End If
            If lambda <= 0.0 Then
                Throw New ArgumentException($"the ridge lambda must be a positive number, got {lambda:G6}", NameOf(lambda))
            End If
            If minimumRows < 1 Then
                Throw New ArgumentException($"the minimum row count must be positive, got {minimumRows}", NameOf(minimumRows))
            End If

            Dim groups As Integer = features.Length

            If responses.Length <> groups Then
                Throw New ArgumentException(
                    $"the response array has {responses.Length} group(s), but {groups} is required", NameOf(responses))
            End If

            Dim records As Integer = If(groups = 0, 0, If(features(0) Is Nothing, 0, features(0).Length))

            If support.GetLength(0) <> records OrElse support.GetLength(1) <> groups Then
                Throw New ArgumentException(
                    $"the support mask has shape [{support.GetLength(0)} x {support.GetLength(1)}], " &
                    $"but [{records} x {groups}] is required", NameOf(support))
            End If

            Dim result(groups - 1) As MultiResponseRidgeFit
            Dim featuresPerRow As Integer = If(records = 0 OrElse features(0)(0) Is Nothing, 0, features(0)(0).Length)

            Call CheckShapes(features, responses, groups, records, featuresPerRow)

            Dim body As Action(Of Integer) =
                Sub(g As Integer)
                    Dim fit As MultiResponseRidgeFit = SolveGroup(features(g), responses(g), support, g, lambda, minimumRows)

                    result(g) = fit
                End Sub

            Dim work As Long = CLng(groups) * records * std.Max(featuresPerRow, 1)

            If degreeOfParallelism = 1 OrElse groups < 2 OrElse work < ParallelWorkThreshold Then
                For g As Integer = 0 To groups - 1
                    Call body(g)
                Next
            Else
                Dim options As New System.Threading.Tasks.ParallelOptions()

                ' System.Threading.Tasks.Parallel 必须全限定：
                ' Microsoft.VisualBasic.Parallel 会遮蔽同名类型
                options.MaxDegreeOfParallelism = If(degreeOfParallelism > 0, degreeOfParallelism, -1)

                Call System.Threading.Tasks.Parallel.For(0, groups, options, body)
            End If

            Return result
        End Function

        ''' <summary>
        ''' 检查三维输入在行/列维度上是否自洽。
        ''' </summary>
        Private Sub CheckShapes(features As Double()()(), responses As Double()()(),
                                groups As Integer, records As Integer, featuresPerRow As Integer)

            For g As Integer = 0 To groups - 1
                Dim block As Double()() = features(g)
                Dim target As Double()() = responses(g)

                If block Is Nothing OrElse target Is Nothing Then
                    Throw New ArgumentException($"group #{g} contains no data", NameOf(features))
                End If
                If block.Length <> target.Length Then
                    Throw New ArgumentException(
                        $"group #{g} has {block.Length} feature row(s) but {target.Length} response row(s)", NameOf(responses))
                End If

                For i As Integer = 0 To block.Length - 1
                    If block(i) Is Nothing OrElse block(i).Length <> featuresPerRow Then
                        Throw New ArgumentException(
                            $"group #{g} row #{i} has {(If(block(i) Is Nothing, 0, block(i).Length))} feature(s), " &
                            $"but {featuresPerRow} is required", NameOf(features))
                    End If
                Next
            Next
        End Sub

        ''' <summary>
        ''' 单组求解：按掩码取行、求中心、组法方程、求解。
        ''' </summary>
        Private Function SolveGroup(block As Double()(), target As Double()(),
                                    support As Boolean(,), g As Integer,
                                    lambda As Double, minimumRows As Integer) As MultiResponseRidgeFit
            Dim records As Integer = block.Length
            Dim mask As Boolean() = New Boolean(If(records = 0, -1, records - 1)) {}
            Dim count As Integer = 0

            For i As Integer = 0 To records - 1
                Dim used As Boolean = support(i, g)

                mask(i) = used

                If used Then
                    count += 1
                End If
            Next

            If count < minimumRows Then
                Return NotSupported(block, target, lambda, count)
            End If

            Return Solve(block, target, mask, lambda, center:=True)
        End Function

        ''' <summary>
        ''' 样本不足时的占位结果：系数与中心全为 0，<see cref="MultiResponseRidgeFit.Supported"/> = False。
        ''' </summary>
        Private Function NotSupported(block As Double()(), target As Double()(),
                                      lambda As Double, count As Integer) As MultiResponseRidgeFit
            Dim f As Integer = If(block.Length = 0 OrElse block(0) Is Nothing, 0, block(0).Length)
            Dim k As Integer = If(target.Length = 0 OrElse target(0) Is Nothing, 0, target(0).Length)
            Dim coefficients As Double()() = RectangularArray.Matrix(Of Double)(f, k)
            Dim rss As Double() = New Double(If(k = 0, -1, k - 1)) {}
            Dim r2 As Double() = New Double(If(k = 0, -1, k - 1)) {}

            For c As Integer = 0 To k - 1
                r2(c) = 0.0
            Next

            Return New MultiResponseRidgeFit With {
                .Coefficients = coefficients,
                .FeatureCenter = New Double(If(f = 0, -1, f - 1)) {},
                .Lambda = lambda,
                .SampleCount = count,
                .Rank = 0,
                .RSS = rss,
                .RSquared = r2,
                .DegreesOfFreedom = 0.0,
                .Supported = False
            }
        End Function

        ''' <summary>
        ''' 无异常的单组求解核心：不可解时返回 <c>Supported = False</c> 而不是抛出。
        ''' </summary>
        Private Function Solve(x As Double()(), y As Double()(), mask As Boolean(),
                               lambda As Double, center As Boolean) As MultiResponseRidgeFit
            If x Is Nothing Then
                Throw New ArgumentNullException(NameOf(x))
            End If
            If y Is Nothing Then
                Throw New ArgumentNullException(NameOf(y))
            End If
            If lambda <= 0.0 Then
                Throw New ArgumentException($"the ridge lambda must be a positive number, got {lambda:G6}", NameOf(lambda))
            End If

            Dim n As Integer = x.Length

            If y.Length <> n Then
                Throw New ArgumentException(
                    $"the design matrix has {n} row(s) but the response matrix has {y.Length}", NameOf(y))
            End If
            If n = 0 Then
                Throw New ArgumentException("the ridge regression requires at least one row", NameOf(x))
            End If

            Dim f As Integer = If(x(0) Is Nothing, 0, x(0).Length)
            Dim k As Integer = If(y(0) Is Nothing, 0, y(0).Length)

            If f = 0 Then
                Throw New ArgumentException("the design matrix has no feature column", NameOf(x))
            End If
            If k = 0 Then
                Throw New ArgumentException("the response matrix has no response column", NameOf(y))
            End If
            If mask IsNot Nothing AndAlso mask.Length <> n Then
                Throw New ArgumentException(
                    $"the row mask has {mask.Length} entry(s), but {n} is required", NameOf(mask))
            End If

            For i As Integer = 0 To n - 1
                If x(i) Is Nothing OrElse x(i).Length <> f Then
                    Throw New ArgumentException(
                        $"row #{i} of the design matrix has {(If(x(i) Is Nothing, 0, x(i).Length))} feature(s), " & $"but {f} is required", NameOf(x))
                End If
                If y(i) Is Nothing OrElse y(i).Length <> k Then
                    Throw New ArgumentException(
                        $"row #{i} of the response matrix has {(If(y(i) Is Nothing, 0, y(i).Length))} response(s), " & $"but {k} is required", NameOf(y))
                End If
            Next

            ' 1) 掩码统计
            Dim used As Integer = 0

            For i As Integer = 0 To n - 1
                If mask Is Nothing OrElse mask(i) Then
                    used += 1
                End If
            Next

            If used = 0 Then
                Return NotSupported(x, y, lambda, 0)
            End If

            ' 2) 参与行上的列均值
            Dim means As Double() = New Double(f - 1) {}

            If center Then
                For i As Integer = 0 To n - 1
                    If mask IsNot Nothing AndAlso Not mask(i) Then
                        Continue For
                    End If

                    For j As Integer = 0 To f - 1
                        means(j) += x(i)(j)
                    Next
                Next

                For j As Integer = 0 To f - 1
                    means(j) /= used
                Next
            End If

            ' 3) 组法方程：G = XᵀX（对称，只算上三角再镜像）、R = XᵀY
            Dim gram As Double()() = RectangularArray.Matrix(Of Double)(f, f)
            Dim rhs As Double()() = RectangularArray.Matrix(Of Double)(f, k)
            Dim row As Double() = New Double(f - 1) {}

            For i As Integer = 0 To n - 1
                If mask IsNot Nothing AndAlso Not mask(i) Then
                    Continue For
                End If

                Dim xi As Double() = x(i)

                For j As Integer = 0 To f - 1
                    row(j) = xi(j) - means(j)
                Next

                For a As Integer = 0 To f - 1
                    Dim value As Double = row(a)

                    For b As Integer = a To f - 1
                        gram(a)(b) += value * row(b)
                    Next

                    Dim yi As Double() = y(i)

                    For c As Integer = 0 To k - 1
                        rhs(a)(c) += value * yi(c)
                    Next
                Next
            Next

            For a As Integer = 0 To f - 1
                For b As Integer = a + 1 To f - 1
                    gram(b)(a) = gram(a)(b)
                Next

                gram(a)(a) += lambda
            Next

            ' 4) 求解 (XᵀX + λI)·B = XᵀY
            Dim coefficients As Double()() = Nothing
            Dim rank As Integer = 0
            Dim traceInverse As Double = 0.0

            Call SolveSystem(gram, rhs, f, k, coefficients, rank, traceInverse)

            If coefficients Is Nothing Then
                Return NotSupported(x, y, lambda, used)
            End If

            ' 5) 拟合优度
            Dim rss As Double() = New Double(k - 1) {}
            Dim tss As Double() = New Double(k - 1) {}
            Dim residual As Double() = New Double(k - 1) {}

            For i As Integer = 0 To n - 1
                If mask IsNot Nothing AndAlso Not mask(i) Then
                    Continue For
                End If

                Dim xi As Double() = x(i)
                Dim yi As Double() = y(i)

                For c As Integer = 0 To k - 1
                    Dim yhat As Double = 0.0

                    For j As Integer = 0 To f - 1
                        yhat += (xi(j) - means(j)) * coefficients(j)(c)
                    Next

                    residual(c) = yi(c) - yhat
                    rss(c) += residual(c) * residual(c)
                    tss(c) += yi(c) * yi(c)
                Next
            Next

            Dim rsquared As Double() = New Double(k - 1) {}

            For c As Integer = 0 To k - 1
                rsquared(c) = If(tss(c) > 0.0, 1.0 - rss(c) / tss(c), 0.0)
            Next

            Return New MultiResponseRidgeFit With {
                .Coefficients = coefficients,
                .FeatureCenter = means,
                .Lambda = lambda,
                .SampleCount = used,
                .Rank = rank,
                .RSS = rss,
                .RSquared = rsquared,
                .DegreesOfFreedom = std.Max(f - lambda * traceInverse, 0.0),
                .Supported = True
            }
        End Function

        ''' <summary>
        ''' 解 <c>G·B = R</c>：优先 Cholesky，失败时回退到 <see cref="NumericMatrix.Solve(GeneralMatrix)"/>。
        ''' </summary>
        ''' <param name="gram">已经是 <c>XᵀX + λI</c> 的 f×f 对称矩阵（不会被修改）</param>
        ''' <param name="rhs">XᵀY（f×k）</param>
        ''' <param name="coefficients">求解结果；不可解时返回 <c>Nothing</c></param>
        ''' <param name="rank">法方程的数值秩</param>
        ''' <param name="traceInverse">tr(G⁻¹)，用于计算有效自由度</param>
        Private Sub SolveSystem(gram As Double()(), rhs As Double()(), f As Integer, k As Integer,
                                ByRef coefficients As Double()(), ByRef rank As Integer, ByRef traceInverse As Double)

            Dim normal As New NumericMatrix(gram, f, f)
            Dim targets As New NumericMatrix(rhs, f, k)
            Dim solution As GeneralMatrix = Nothing
            Dim inverse As GeneralMatrix = Nothing

            Try
                Dim chol As CholeskyDecomposition = normal.chol()

                If chol.SPD Then
                    solution = chol.Solve(targets)
                    inverse = chol.Solve(NumericMatrix.Identity(f))
                    rank = f
                End If
            Catch ex As Exception
                solution = Nothing
                inverse = Nothing
            End Try

            If solution Is Nothing Then
                ' 回退：方阵走 LU、非方阵走 QR（这里必然是方阵）
                Try
                    solution = normal.Solve(targets)
                    inverse = normal.Solve(NumericMatrix.Identity(f))
                    rank = New SingularValueDecomposition(normal).Rank
                Catch ex As Exception
                    coefficients = Nothing
                    rank = 0
                    traceInverse = 0.0

                    Return
                End Try
            End If

            Dim values As Double()() = solution.ArrayPack()
            Dim invValues As Double()() = inverse.ArrayPack()
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(f, k)

            For a As Integer = 0 To f - 1
                For c As Integer = 0 To k - 1
                    result(a)(c) = values(a)(c)
                Next
            Next

            Dim trace As Double = 0.0

            For i As Integer = 0 To f - 1
                trace += invValues(i)(i)
            Next

            coefficients = result
            traceInverse = If(Double.IsFinite(trace), trace, 0.0)
        End Sub

    End Module
End Namespace
