Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports CompressedMatrix = Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed.CsrMatrix
Imports stdf = System.Math

Namespace LinearAlgebra.Matrix

    ''' <summary>
    ''' 截断主成分分析（Truncated PCA）："样本 × 特征"形态数据的低秩分解、载荷与解释方差。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>为什么需要它</b>：<see cref="RandomizedSVD"/> 只做"任意线性算子的低秩 SVD"，
    ''' 不理解"中心化/标准化"这类统计语义；而主成分分析的实质是
    ''' "先对数据做列变换，再对变换后的矩阵做 SVD"。本类负责这一层包装：
    ''' </para>
    ''' <list type="number">
    ''' <item><description>按列（每个特征）求均值（以及可选的尺度）；</description></item>
    ''' <item><description>把 <c>A = (X − 1·μᵀ)</c>（或再除以尺度 <c>σ</c>）表达成<b>算子</b>，
    ''' 交给 <see cref="RandomizedSVD.FromOperator"/>，避免为 <c>6000×18000</c> 的输入复制一份中心化矩阵；</description></item>
    ''' <item><description>把 SVD 的结果翻译成统计量：载荷 <see cref="Components"/>（特征空间，<c>k×p</c>）、
    ''' 得分 <see cref="Scores"/>（样本空间，<c>n×k</c>）、<see cref="ExplainedVariance"/> 与
    ''' <see cref="ExplainedVarianceRatio"/>。</description></item>
    ''' </list>
    ''' <para>
    ''' <b>与上游 CellularAI 的口径对应</b>（<c>cellularai/prediction/totals.py</c> 的 <c>control_pcs</c>）：
    ''' 输入是"细胞 × 基因"（samples × features），先按<b>基因（列）</b>中心化，
    ''' 再做 SVD，取 <c>Vt[:k]</c> 与 <c>sv**2</c>。因此：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description><see cref="Components"/> ↔ <c>Vt[:k]</c>（<c>k×p</c> 的特征空间载荷）；</description></item>
    ''' <item><description><see cref="ExplainedVariance"/> ↔ <c>sv**2</c>，<b>不除以 <c>n−1</c></b>。
    ''' 这是上游的口径，也等于"该主成分承载的平方和"；需要样本方差时请自行除以自由度。</description></item>
    ''' </list>
    ''' <para>
    ''' <b>算子构造</b>：令 <c>A = (X − 1μᵀ)·diag(1/σ)</c>，则对任意向量 <c>v</c>（长度 p）
    ''' 与 <c>u</c>（长度 n）有
    ''' <c>A·v = X·(v/σ) − μ·(v/σ)·1</c>、
    ''' <c>Aᵀ·u = (Xᵀ·u − μ·Σu)/σ</c>，
    ''' 因此只需要"矩阵乘向量"与"转置矩阵乘向量"两个原语，稀疏输入也能直接参与分解。
    ''' </para>
    ''' <para>
    ''' <b>尺度约定</b>：<c>scale:=True</c> 时用<b>总体</b>标准差（分母为 n，与
    ''' <c>StandardScaler</c> 一致）；常数列（方差为 0）的尺度守卫成 1，避免除零。
    ''' </para>
    ''' <para>
    ''' <b>输入类型</b>：稠密锯齿数组与 <see cref="CompressedMatrix"/>（CSR）。三元组
    ''' <see cref="SparseMatrix"/> 没有按列统计方差所需的元素级访问入口，需要先转成 CSR。
    ''' </para>
    ''' </remarks>
    Public Class TruncatedPCA

        Private ReadOnly m_nSamples As Integer
        Private ReadOnly m_nFeatures As Integer
        Private ReadOnly m_requestedRank As Integer

        Private m_components As Double()()      ' k × p，特征空间载荷（= Vᵀ）
        Private m_scores As Double()()          ' n × k，得分（= U·Σ）
        Private m_singular As Double()          ' k
        Private m_variance As Double()          ' k，= σ²
        Private m_ratio As Double()             ' k
        Private m_mean As Double()              ' p
        Private m_scale As Double()             ' p
        Private m_center As Boolean
        Private m_standardize As Boolean
        Private m_rank As Integer
        Private m_retained As Integer
        Private m_tolerance As Double
        Private m_totalVariance As Double

#Region "Constructors"

        ''' <summary>
        ''' 对稠密矩阵（样本 × 特征）拟合截断 PCA。
        ''' </summary>
        ''' <param name="data">待分解矩阵，<c>data(i)(j)</c> 为第 i 个样本的第 j 个特征</param>
        ''' <param name="k">保留的主成分个数</param>
        ''' <param name="center">是否按列中心化（默认 True）</param>
        ''' <param name="scale">是否按列标准化（默认 False；与 <c>center:=True</c> 搭配）</param>
        ''' <param name="oversampling">过采样维度，默认 8</param>
        ''' <param name="powerIterations">幂迭代次数，默认 2</param>
        ''' <param name="seed">随机测试矩阵的种子</param>
        ''' <param name="solver">小矩阵分解路径</param>
        Public Sub New(data As Double()(), k As Integer,
                       Optional center As Boolean = True,
                       Optional scale As Boolean = False,
                       Optional oversampling As Integer = 8,
                       Optional powerIterations As Integer = 2,
                       Optional seed As Integer = 0,
                       Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD)

            If data Is Nothing Then
                Throw New ArgumentNullException(NameOf(data))
            End If

            m_nSamples = data.Length
            m_nFeatures = If(m_nSamples = 0, 0, data(0).Length)
            m_requestedRank = k
            m_center = center
            m_standardize = scale AndAlso center

            Call ValidateParameters(k, oversampling, powerIterations)
            Call ColumnStatistics(data, center, scale)

            Dim apply As Func(Of Double(), Double()) =
                Function(v As Double()) DenseApply(data, v)
            Dim applyTranspose As Func(Of Double(), Double()) =
                Function(u As Double()) DenseApplyTranspose(data, u)

            Call Decompose(apply, applyTranspose, k, oversampling, powerIterations, seed, solver)
        End Sub

        ''' <summary>
        ''' 对 CSR 稀疏矩阵（样本 × 特征）拟合截断 PCA。
        ''' </summary>
        ''' <param name="data">待分解的 CSR 矩阵，例如单细胞计数矩阵（细胞 × 基因）</param>
        ''' <param name="k">保留的主成分个数</param>
        ''' <param name="center">是否按列中心化（默认 True）</param>
        ''' <param name="scale">是否按列标准化（默认 False）</param>
        ''' <param name="oversampling">过采样维度，默认 8</param>
        ''' <param name="powerIterations">幂迭代次数，默认 2</param>
        ''' <param name="seed">随机测试矩阵的种子</param>
        ''' <param name="solver">小矩阵分解路径</param>
        Public Sub New(data As CompressedMatrix, k As Integer,
                       Optional center As Boolean = True,
                       Optional scale As Boolean = False,
                       Optional oversampling As Integer = 8,
                       Optional powerIterations As Integer = 2,
                       Optional seed As Integer = 0,
                       Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD)

            If data Is Nothing Then
                Throw New ArgumentNullException(NameOf(data))
            End If

            m_nSamples = data.Rows
            m_nFeatures = data.Columns
            m_requestedRank = k
            m_center = center
            m_standardize = scale AndAlso center

            Call ValidateParameters(k, oversampling, powerIterations)
            Call ColumnStatistics(data, center, scale)

            Dim apply As Func(Of Double(), Double()) =
                Function(v As Double()) CsrApply(data, v)
            Dim applyTranspose As Func(Of Double(), Double()) =
                Function(u As Double()) CsrApplyTranspose(data, u)

            Call Decompose(apply, applyTranspose, k, oversampling, powerIterations, seed, solver)
        End Sub

#End Region

#Region "Public Properties"

        ''' <summary>
        ''' 特征空间载荷（<c>k×p</c>），第 i 行是第 i 个主成分在原始特征轴上的方向。
        ''' </summary>
        ''' <remarks>
        ''' 与上游 <c>control_pcs</c> 的 <c>Vt[:k]</c> 完全对应：
        ''' 如果输入是"细胞 × 基因"，这个矩阵就是<b>基因空间</b>的前 k 个主成分。
        ''' </remarks>
        Public ReadOnly Property Components As Double()()
            Get
                Return CloneMatrix(m_components)
            End Get
        End Property

        ''' <summary>
        ''' 样本空间得分（<c>n×k</c>），等于 <c>U·Σ</c>，也等于把中心化后的数据投影到载荷上。
        ''' </summary>
        Public ReadOnly Property Scores As Double()()
            Get
                Return CloneMatrix(m_scores)
            End Get
        End Property

        ''' <summary>
        ''' 前 k 个奇异值（降序）。
        ''' </summary>
        Public ReadOnly Property SingularValues As Double()
            Get
                Return DirectCast(m_singular.Clone(), Double())
            End Get
        End Property

        ''' <summary>
        ''' 解释方差 <c>σᵢ²</c>（<b>不除以 <c>n−1</c></b>，与上游 CellularAI 的口径一致）。
        ''' </summary>
        Public ReadOnly Property ExplainedVariance As Double()
            Get
                Return DirectCast(m_variance.Clone(), Double())
            End Get
        End Property

        ''' <summary>
        ''' 解释方差比 <c>σᵢ² / Σσ²</c>；分母是本次随机子空间捕捉到的总方差（见
        ''' <see cref="RandomizedSVD.CapturedVariance"/>）。
        ''' </summary>
        Public ReadOnly Property ExplainedVarianceRatio As Double()
            Get
                Return DirectCast(m_ratio.Clone(), Double())
            End Get
        End Property

        ''' <summary>
        ''' 每个特征的均值（中心化向量）；<c>center:=False</c> 时全为 0。
        ''' </summary>
        Public ReadOnly Property Mean As Double()
            Get
                Return DirectCast(m_mean.Clone(), Double())
            End Get
        End Property

        ''' <summary>
        ''' 每个特征的尺度（标准化因子）；<c>scale:=False</c> 时全为 1。
        ''' </summary>
        Public ReadOnly Property Scale As Double()
            Get
                Return DirectCast(m_scale.Clone(), Double())
            End Get
        End Property

        ''' <summary>
        ''' 实际返回的主成分个数 <c>r = min(k, 数值秩)</c>。
        ''' </summary>
        Public ReadOnly Property RetainedRank As Integer
            Get
                Return m_retained
            End Get
        End Property

        ''' <summary>
        ''' 请求的主成分个数 <c>k</c>。
        ''' </summary>
        Public ReadOnly Property RequestedRank As Integer
            Get
                Return m_requestedRank
            End Get
        End Property

        ''' <summary>
        ''' 随机子空间捕捉到的数值秩。
        ''' </summary>
        Public ReadOnly Property Rank As Integer
            Get
                Return m_rank
            End Get
        End Property

        ''' <summary>
        ''' 谱容差 <c>eps·max(n,p)·max(σ₁,1)</c>。
        ''' </summary>
        Public ReadOnly Property SpectrumTolerance As Double
            Get
                Return m_tolerance
            End Get
        End Property

        ''' <summary>
        ''' 本次分解捕捉到的总方差（<c>Σσ²</c> 对 ℓ 个分量求和）。
        ''' </summary>
        Public ReadOnly Property TotalVariance As Double
            Get
                Return m_totalVariance
            End Get
        End Property

        ''' <summary>
        ''' 样本数 n。
        ''' </summary>
        Public ReadOnly Property SampleCount As Integer
            Get
                Return m_nSamples
            End Get
        End Property

        ''' <summary>
        ''' 特征数 p。
        ''' </summary>
        Public ReadOnly Property FeatureCount As Integer
            Get
                Return m_nFeatures
            End Get
        End Property

        ''' <summary>
        ''' 是否做了按列中心化。
        ''' </summary>
        Public ReadOnly Property Centered As Boolean
            Get
                Return m_center
            End Get
        End Property

        ''' <summary>
        ''' 是否做了按列标准化。
        ''' </summary>
        Public ReadOnly Property Standardized As Boolean
            Get
                Return m_standardize
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' 把新的样本投影到已拟合的主成分上（<c>n×k</c> 的得分）。
        ''' </summary>
        ''' <param name="data">样本 × 特征 的稠密矩阵，列数必须等于 <see cref="FeatureCount"/></param>
        ''' <returns>得分矩阵；未保留任何主成分时返回"零列"矩阵</returns>
        Public Function Project(data As Double()()) As Double()()
            If data Is Nothing Then
                Throw New ArgumentNullException(NameOf(data))
            End If

            Dim rows As Integer = data.Length
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(rows, m_retained)

            For i As Integer = 0 To rows - 1
                Dim row As Double() = data(i)

                If row Is Nothing OrElse row.Length <> m_nFeatures Then
                    Throw New ArgumentException(
                        $"sample #{i} has {(If(row Is Nothing, 0, row.Length))} feature(s), but {m_nFeatures} is required", NameOf(data))
                End If

                For c As Integer = 0 To m_retained - 1
                    Dim acc As Double = 0.0
                    Dim loading As Double() = m_components(c)

                    For j As Integer = 0 To m_nFeatures - 1
                        Dim v As Double = (row(j) - m_mean(j)) / m_scale(j)

                        acc += v * loading(j)
                    Next

                    result(i)(c) = acc
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 把新的样本投影到已拟合的主成分上（CSR 输入）。
        ''' </summary>
        ''' <param name="data">样本 × 特征 的 CSR 矩阵</param>
        ''' <returns>得分矩阵（<c>n×k</c>）</returns>
        ''' <remarks>
        ''' 稀疏矩阵只遍历非零元：<c>Σ_j (x_ij − μ_j)/σ_j · w_j</c>
        ''' 被改写为 <c>Σ_{j∈nnz} x_ij·(w_j/σ_j) − Σ_j μ_j·w_j/σ_j</c>，
        ''' 第二项与样本无关，可以按主成分预先算好。
        ''' </remarks>
        Public Function Project(data As CompressedMatrix) As Double()()
            If data Is Nothing Then
                Throw New ArgumentNullException(NameOf(data))
            End If
            If data.Columns <> m_nFeatures Then
                Throw New ArgumentException(
                    $"the matrix has {data.Columns} column(s), but {m_nFeatures} is required", NameOf(data))
            End If

            Dim rows As Integer = data.Rows
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(rows, m_retained)

            ' 与样本无关的偏移项：Σ_j μ_j·w_j/σ_j
            Dim offsets As Double() = New Double(If(m_retained = 0, -1, m_retained - 1)) {}

            For c As Integer = 0 To m_retained - 1
                Dim loading As Double() = m_components(c)
                Dim acc As Double = 0.0

                For j As Integer = 0 To m_nFeatures - 1
                    acc += m_mean(j) * loading(j) / m_scale(j)
                Next

                offsets(c) = acc
            Next

            Dim indptr As Integer() = data.Indptr
            Dim indices As Integer() = data.Indices
            Dim values As Double() = data.Values

            For i As Integer = 0 To rows - 1
                Dim row As Double() = result(i)

                If m_retained = 0 Then
                    Continue For
                End If

                For c As Integer = 0 To m_retained - 1
                    row(c) = -offsets(c)
                Next

                For k As Integer = indptr(i) To indptr(i + 1) - 1
                    Dim x As Double = values(k)
                    Dim j As Integer = indices(k)

                    If x = 0.0 Then
                        Continue For
                    End If

                    For c As Integer = 0 To m_retained - 1
                        row(c) += x * m_components(c)(j) / m_scale(j)
                    Next
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 由得分近似还原（变换回）原始特征空间：<c>X̂ = scores·Components·σ + μ</c>。
        ''' </summary>
        ''' <param name="scores">得分矩阵（<c>n×r</c>，通常来自 <see cref="Scores"/> 或 <see cref="Project(Double()())"/>）</param>
        ''' <returns>样本 × 特征 的稠密矩阵</returns>
        ''' <remarks>
        ''' 当 <c>r</c> 等于矩阵的有效秩时，还原是精确的（在浮点误差内）；
        ''' 否则它给出"秩 r 的最佳近似"，即主成分子空间上的投影。
        ''' </remarks>
        Public Function InverseTransform(scores As Double()()) As Double()()
            If scores Is Nothing Then
                Throw New ArgumentNullException(NameOf(scores))
            End If

            Dim rows As Integer = scores.Length
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(rows, m_nFeatures)

            For i As Integer = 0 To rows - 1
                Dim score As Double() = scores(i)

                If score Is Nothing OrElse score.Length <> m_retained Then
                    Throw New ArgumentException(
                        $"score row #{i} has {(If(score Is Nothing, 0, score.Length))} component(s), but {m_retained} is required", NameOf(scores))
                End If

                Dim row As Double() = result(i)

                For j As Integer = 0 To m_nFeatures - 1
                    row(j) = m_mean(j)
                Next

                For c As Integer = 0 To m_retained - 1
                    Dim value As Double = score(c)

                    If value = 0.0 Then
                        Continue For
                    End If

                    Dim loading As Double() = m_components(c)

                    For j As Integer = 0 To m_nFeatures - 1
                        row(j) += value * loading(j) * m_scale(j)
                    Next
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 一步式入口：拟合并返回模型对象（稠密输入）。
        ''' </summary>
        Public Shared Function Fit(data As Double()(), k As Integer,
                                   Optional center As Boolean = True,
                                   Optional scale As Boolean = False,
                                   Optional oversampling As Integer = 8,
                                   Optional powerIterations As Integer = 2,
                                   Optional seed As Integer = 0,
                                   Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD) As TruncatedPCA
            Return New TruncatedPCA(data, k, center, scale, oversampling, powerIterations, seed, solver)
        End Function

        ''' <summary>
        ''' 一步式入口：拟合并返回模型对象（CSR 输入）。
        ''' </summary>
        Public Shared Function Fit(data As CompressedMatrix, k As Integer,
                                   Optional center As Boolean = True,
                                   Optional scale As Boolean = False,
                                   Optional oversampling As Integer = 8,
                                   Optional powerIterations As Integer = 2,
                                   Optional seed As Integer = 0,
                                   Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD) As TruncatedPCA
            Return New TruncatedPCA(data, k, center, scale, oversampling, powerIterations, seed, solver)
        End Function

#End Region

#Region "Core pipeline"

        Private Shared Sub ValidateParameters(k As Integer, oversampling As Integer, powerIterations As Integer)
            If k < 1 Then
                Throw New ArgumentException($"parameter k({k}) must be at least 1!", NameOf(k))
            End If
            If oversampling < 0 Then
                Throw New ArgumentException($"parameter oversampling({oversampling}) must not be negative!", NameOf(oversampling))
            End If
            If powerIterations < 0 Then
                Throw New ArgumentException($"parameter powerIterations({powerIterations}) must not be negative!", NameOf(powerIterations))
            End If
        End Sub

        ''' <summary>
        ''' 执行分解并把 SVD 的结果翻译成统计量。
        ''' </summary>
        Private Sub Decompose(apply As Func(Of Double(), Double()),
                              applyTranspose As Func(Of Double(), Double()),
                              k As Integer,
                              oversampling As Integer,
                              powerIterations As Integer,
                              seed As Integer,
                              solver As RandomizedSvdSolver)

            If m_nSamples = 0 OrElse m_nFeatures = 0 Then
                Call EmptyResult()
                Return
            End If
            If k > stdf.Min(m_nSamples, m_nFeatures) Then
                Throw New ArgumentException(
                    $"parameter k({k}) can not exceed the smaller dimension min(n,p) = {stdf.Min(m_nSamples, m_nFeatures)}!", NameOf(k))
            End If

            Dim svd As RandomizedSVD = RandomizedSVD.FromOperator(
                apply, applyTranspose, m_nSamples, m_nFeatures, k,
                oversampling, powerIterations, seed, solver)

            m_rank = svd.Rank
            m_retained = svd.RetainedRank
            m_tolerance = svd.SpectrumTolerance
            m_totalVariance = svd.CapturedVariance

            Dim singular As Double() = svd.SingularValues
            Dim scores As Double()() = svd.ReducedMatrix        ' n × r，= U·Σ
            Dim components As Double()() = svd.Components       ' r × p，= Vᵀ

            m_components = components
            m_scores = scores
            m_singular = New Double(If(m_retained = 0, -1, m_retained - 1)) {}
            m_variance = New Double(If(m_retained = 0, -1, m_retained - 1)) {}
            m_ratio = New Double(If(m_retained = 0, -1, m_retained - 1)) {}

            For i As Integer = 0 To m_retained - 1
                Dim sigma As Double = singular(i)

                m_singular(i) = sigma
                m_variance(i) = sigma * sigma
                m_ratio(i) = If(m_totalVariance > 0.0, sigma * sigma / m_totalVariance, 0.0)
            Next
        End Sub

        Private Sub EmptyResult()
            m_components = RectangularArray.Matrix(Of Double)(0, m_nFeatures)
            m_scores = RectangularArray.Matrix(Of Double)(m_nSamples, 0)
            m_singular = New Double(-1) {}
            m_variance = New Double(-1) {}
            m_ratio = New Double(-1) {}
            m_rank = 0
            m_retained = 0
            m_tolerance = 0.0
            m_totalVariance = 0.0
        End Sub

#End Region

#Region "Column statistics"

        ''' <summary>
        ''' 稠密输入：一次扫描求每列的均值与尺度。
        ''' </summary>
        Private Sub ColumnStatistics(data As Double()(), center As Boolean, scale As Boolean)
            m_mean = New Double(If(m_nFeatures = 0, -1, m_nFeatures - 1)) {}
            m_scale = New Double(If(m_nFeatures = 0, -1, m_nFeatures - 1)) {}

            For j As Integer = 0 To m_nFeatures - 1
                m_scale(j) = 1.0
            Next

            If Not center Then
                Return
            End If

            For i As Integer = 0 To m_nSamples - 1
                Dim row As Double() = data(i)

                If row Is Nothing OrElse row.Length <> m_nFeatures Then
                    Throw New ArgumentException(
                        $"sample #{i} has {(If(row Is Nothing, 0, row.Length))} feature(s), but {m_nFeatures} is required", NameOf(data))
                End If

                For j As Integer = 0 To m_nFeatures - 1
                    m_mean(j) += row(j)
                Next
            Next

            For j As Integer = 0 To m_nFeatures - 1
                m_mean(j) /= m_nSamples
            Next

            If Not scale Then
                Return
            End If

            Dim squares As Double() = New Double(If(m_nFeatures = 0, -1, m_nFeatures - 1)) {}

            For i As Integer = 0 To m_nSamples - 1
                Dim row As Double() = data(i)

                For j As Integer = 0 To m_nFeatures - 1
                    Dim d As Double = row(j) - m_mean(j)

                    squares(j) += d * d
                Next
            Next

            For j As Integer = 0 To m_nFeatures - 1
                Dim variance As Double = squares(j) / m_nSamples
                Dim sd As Double = stdf.Sqrt(stdf.Max(variance, 0.0))

                ' 常数列没有尺度可言，守卫成 1，避免除零把整列变成 NaN
                m_scale(j) = If(sd > 0.0, sd, 1.0)
            Next
        End Sub

        ''' <summary>
        ''' CSR 输入：按列累加一阶矩与二阶矩。
        ''' </summary>
        Private Sub ColumnStatistics(data As CompressedMatrix, center As Boolean, scale As Boolean)
            m_mean = New Double(If(m_nFeatures = 0, -1, m_nFeatures - 1)) {}
            m_scale = New Double(If(m_nFeatures = 0, -1, m_nFeatures - 1)) {}

            For j As Integer = 0 To m_nFeatures - 1
                m_scale(j) = 1.0
            Next

            If Not center Then
                Return
            End If

            Dim indptr As Integer() = data.Indptr
            Dim indices As Integer() = data.Indices
            Dim values As Double() = data.Values
            Dim squares As Double() = New Double(If(m_nFeatures = 0, -1, m_nFeatures - 1)) {}

            For i As Integer = 0 To data.Rows - 1
                For k As Integer = indptr(i) To indptr(i + 1) - 1
                    Dim x As Double = values(k)
                    Dim j As Integer = indices(k)

                    m_mean(j) += x
                    squares(j) += x * x
                Next
            Next

            For j As Integer = 0 To m_nFeatures - 1
                Dim sum As Double = m_mean(j)

                m_mean(j) = sum / m_nSamples

                If scale Then
                    ' Var = E[x²] − E[x]²（总体方差）
                    Dim variance As Double = squares(j) / m_nSamples - m_mean(j) * m_mean(j)
                    Dim sd As Double = stdf.Sqrt(stdf.Max(variance, 0.0))

                    m_scale(j) = If(sd > 0.0, sd, 1.0)
                End If
            Next
        End Sub

#End Region

#Region "Transformed operators"

        ''' <summary>
        ''' <c>A·v</c>，其中 <c>A = (X − 1μᵀ)·diag(1/σ)</c>：<c>X·(v/σ) − μ·(v/σ)·1</c>。
        ''' </summary>
        Private Function DenseApply(data As Double()(), v As Double()) As Double()
            Dim w As Double() = New Double(If(m_nFeatures = 0, -1, m_nFeatures - 1)) {}
            Dim muw As Double = 0.0

            For j As Integer = 0 To m_nFeatures - 1
                w(j) = v(j) / m_scale(j)
                muw += m_mean(j) * w(j)
            Next

            Dim y As Double() = New Double(If(m_nSamples = 0, -1, m_nSamples - 1)) {}

            For i As Integer = 0 To m_nSamples - 1
                Dim row As Double() = data(i)
                Dim acc As Double = 0.0

                For j As Integer = 0 To m_nFeatures - 1
                    acc += row(j) * w(j)
                Next

                y(i) = acc - muw
            Next

            Return y
        End Function

        ''' <summary>
        ''' <c>Aᵀ·u</c>：<c>(Xᵀ·u − μ·Σu)/σ</c>。
        ''' </summary>
        Private Function DenseApplyTranspose(data As Double()(), u As Double()) As Double()
            Dim y As Double() = New Double(If(m_nFeatures = 0, -1, m_nFeatures - 1)) {}
            Dim sum As Double = 0.0

            For i As Integer = 0 To m_nSamples - 1
                Dim weight As Double = u(i)
                Dim row As Double() = data(i)

                sum += weight

                If weight = 0.0 Then
                    Continue For
                End If

                For j As Integer = 0 To m_nFeatures - 1
                    y(j) += row(j) * weight
                Next
            Next

            For j As Integer = 0 To m_nFeatures - 1
                y(j) = (y(j) - m_mean(j) * sum) / m_scale(j)
            Next

            Return y
        End Function

        ''' <summary>
        ''' CSR 版本的 <c>A·v</c>。
        ''' </summary>
        Private Function CsrApply(data As CompressedMatrix, v As Double()) As Double()
            Dim w As Double() = New Double(If(m_nFeatures = 0, -1, m_nFeatures - 1)) {}
            Dim muw As Double = 0.0

            For j As Integer = 0 To m_nFeatures - 1
                w(j) = v(j) / m_scale(j)
                muw += m_mean(j) * w(j)
            Next

            Dim raw As Double() = data.Multiply(w)

            For i As Integer = 0 To raw.Length - 1
                raw(i) -= muw
            Next

            Return raw
        End Function

        ''' <summary>
        ''' CSR 版本的 <c>Aᵀ·u</c>。
        ''' </summary>
        Private Function CsrApplyTranspose(data As CompressedMatrix, u As Double()) As Double()
            Dim raw As Double() = data.MultiplyTranspose(u)
            Dim sum As Double = 0.0

            For i As Integer = 0 To u.Length - 1
                sum += u(i)
            Next

            For j As Integer = 0 To raw.Length - 1
                raw(j) = (raw(j) - m_mean(j) * sum) / m_scale(j)
            Next

            Return raw
        End Function

#End Region

#Region "Private helpers"

        ''' <summary>
        ''' 锯齿二维数组的深拷贝。
        ''' </summary>
        Private Shared Function CloneMatrix(X As Double()()) As Double()()
            If X Is Nothing Then
                Return New Double(-1)() {}
            End If

            Dim copy As Double()() = New Double(X.Length - 1)() {}

            For i As Integer = 0 To X.Length - 1
                copy(i) = DirectCast(X(i).Clone(), Double())
            Next

            Return copy
        End Function

#End Region

    End Class
End Namespace
