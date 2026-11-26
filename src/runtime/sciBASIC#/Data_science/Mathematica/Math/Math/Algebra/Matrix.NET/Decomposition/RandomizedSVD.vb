Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports stdf = System.Math

Namespace LinearAlgebra.Matrix

    ''' <summary>
    ''' 小矩阵分解路径：决定 ℓ×ℓ 级别的分解是走 SmallSVD 还是走 Gram 特征分解。
    ''' </summary>
    Public Enum RandomizedSvdSolver

        ''' <summary>
        ''' 对 <c>Qᵀ·A</c> 的转置（即 <c>Aᵀ·Q</c>）直接做经济型 SVD，再回代得到 <c>U = Q·U_b</c>。
        ''' </summary>
        ''' <remarks>
        ''' 这是 Halko–Martinsson–Tropp 原文推荐的收尾方式：条件数不会被平方，
        ''' 小奇异值的相对精度最好，因此是默认路径。
        ''' </remarks>
        SmallSVD = 0

        ''' <summary>
        ''' 对 ℓ×ℓ 的 Gram 矩阵 <c>Qᵀ·A·Aᵀ·Q</c> 做对称特征分解，再取 <c>σ = √λ</c>。
        ''' </summary>
        ''' <remarks>
        ''' 该路径不构造 <c>n×n</c> 的右奇异向量矩阵（在 <c>n ≫ ℓ</c> 时能省内存），
        ''' 但 Gram 矩阵把条件数平方，约为 1e-8 以下的小奇异值会失真。
        ''' 保留它是为了与 <see cref="TruncatedSVD"/> 的早期数值路径对拍。
        ''' </remarks>
        GramEigen = 1
    End Enum

    ''' <summary>
    ''' 随机化截断奇异值分解（Halko, Martinsson &amp; Tropp, 2009, Algorithm 4.4）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 目标：对 <c>m×n</c> 矩阵 <c>A</c> 求前 <c>k</c> 个奇异三元组，使得
    ''' <c>A ≈ U·Σ·Vᵀ</c>，其中 <c>U</c> 为 <c>m×k</c> 列正交、<c>V</c> 为 <c>n×k</c> 列正交。
    ''' </para>
    ''' <para>
    ''' 与稠密的 <see cref="SingularValueDecomposition"/> 不同，这里只需要"矩阵乘向量"与
    ''' "转置矩阵乘向量"两种原语，因此对稠密数组、<see cref="SparseMatrix"/> 与
    ''' <see cref="Compressed.CsrMatrix"/> 可以共用同一份主流程（三条入口只负责绑定算子，
    ''' 在同一 <c>seed</c> 下数值结果完全一致）。
    ''' </para>
    ''' <para>
    ''' <b>算法步骤</b>：
    ''' </para>
    ''' <list type="number">
    ''' <item><description>
    ''' <c>ℓ = min(k + oversampling, min(m, n))</c>，用<b>局部种子随机源</b>生成高斯矩阵 <c>Ω</c>（n×ℓ）。
    ''' </description></item>
    ''' <item><description>范围查找：<c>Y = A·Ω</c>，再取经济型 QR 的正交因子 <c>Q = qr(Y)</c>（m×ℓ）。</description></item>
    ''' <item><description>
    ''' 子空间幂迭代 <c>powerIterations</c> 轮；每轮做<b>两次</b> QR 重正交：
    ''' <c>Z = qr(Aᵀ·Q)</c>、<c>Q = qr(A·Z)</c>。中间那一步的正交化用于压制舍入误差在
    ''' <c>AᵀA</c> 的幂上被放大（"re-orthogonalization"）。
    ''' </description></item>
    ''' <item><description>
    ''' <c>W = Aᵀ·Q</c>（= <c>(QᵀA)ᵀ</c>），对 <c>W</c> 做经济型 SVD 得 <c>W = V_b·Σ·U_bᵀ</c>，
    ''' 于是 <c>U = Q·U_b</c>、<c>V = V_b</c>（<see cref="RandomizedSvdSolver.SmallSVD"/>）。
    ''' </description></item>
    ''' <item><description>
    ''' 按容差截断有效秩：<c>tol = eps·max(m,n)·max(σ₁,1)</c>，只保留 <c>σ &gt; tol</c> 的分量。
    ''' </description></item>
    ''' </list>
    ''' <para>
    ''' <b>复杂度</b>：<c>O(nnz·ℓ·(2q+3) + (m+n)·ℓ² + ℓ³)</c>，
    ''' 空间 <c>O(nnz + (m+n)·ℓ)</c>，其中 <c>q</c> 为幂迭代次数。全程不会把输入稠密化。
    ''' </para>
    ''' <para>
    ''' <b>可复现性</b>：随机源是本对象私有的 <see cref="Random"/>（<c>New Random(seed)</c> 加本地
    ''' Box–Muller 变换），不触碰 <c>RandomExtensions.NextGaussian()</c> 那样的全局共享状态。
    ''' 因此同一 <c>seed</c> 在同一运行时版本内重复调用会得到逐位相同的结果；
    ''' 但 <see cref="Random"/> 的位流在不同 .NET 大版本之间不作保证，跨版本复现需要固定运行时。
    ''' </para>
    ''' <para>
    ''' <b>空输入与越界</b>：<c>m = 0</c>、<c>n = 0</c> 或 <c>k = 0</c> 时返回空结果而不是抛异常；
    ''' <c>k &gt; min(m, n)</c>、<c>oversampling &lt; 0</c>、<c>powerIterations &lt; 0</c> 抛
    ''' <see cref="ArgumentException"/>。
    ''' </para>
    ''' </remarks>
    Public Class RandomizedSVD

        Private m_apply As Func(Of Double(), Double())
        Private m_applyTranspose As Func(Of Double(), Double())

        Private m_rows As Integer
        Private m_columns As Integer
        Private m_requestedRank As Integer
        Private m_oversampling As Integer
        Private m_powerIterations As Integer
        Private m_seed As Integer
        Private m_solver As RandomizedSvdSolver
        Private m_applications As Long

        Private m_valueU As Double()()
        Private m_valueV As Double()()
        Private m_s As Double()
        Private m_reduced As Double()()
        Private m_numericalRank As Integer
        Private m_tolerance As Double
        Private m_retainedRank As Integer
        Private m_capturedVariance As Double

#Region "Constructors"

        ''' <summary>
        ''' 对稠密锯齿数组（<c>m×n</c>，行优先）做随机化截断 SVD。
        ''' </summary>
        ''' <param name="A">待分解矩阵，<c>A(i)(j)</c> 为第 i 行第 j 列</param>
        ''' <param name="k">请求保留的分量个数；<c>0</c> 表示只求秩而不保留分量</param>
        ''' <param name="oversampling">过采样维度（ℓ = k + oversampling），默认 8</param>
        ''' <param name="powerIterations">幂迭代次数，默认 2（与上游 CellularAI 的 <c>response_basis</c> 一致）</param>
        ''' <param name="seed">随机测试矩阵的种子；同一 seed 的结果逐位一致</param>
        ''' <param name="solver">小矩阵分解路径，默认 <see cref="RandomizedSvdSolver.SmallSVD"/></param>
        Public Sub New(A As Double()(), k As Integer,
                       Optional oversampling As Integer = 8,
                       Optional powerIterations As Integer = 2,
                       Optional seed As Integer = 0,
                       Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD)

            If A Is Nothing Then
                Throw New ArgumentNullException(NameOf(A))
            End If

            Dim rows As Integer = A.Length
            Dim columns As Integer = If(rows = 0, 0, A(0).Length)

            Call Initialize(
                apply:=Function(v As Double()) DenseMultiply(A, v),
                applyTranspose:=Function(v As Double()) DenseMultiplyTranspose(A, v),
                rows:=rows,
                columns:=columns,
                k:=k,
                oversampling:=oversampling,
                powerIterations:=powerIterations,
                seed:=seed,
                solver:=solver)
        End Sub

        ''' <summary>
        ''' 对稀疏矩阵（三元组存储）做随机化截断 SVD。
        ''' </summary>
        ''' <param name="A">待分解的稀疏矩阵</param>
        ''' <param name="k">请求保留的分量个数</param>
        ''' <param name="oversampling">过采样维度，默认 8</param>
        ''' <param name="powerIterations">幂迭代次数，默认 2</param>
        ''' <param name="seed">随机测试矩阵的种子</param>
        ''' <param name="solver">小矩阵分解路径</param>
        Public Sub New(A As SparseMatrix, k As Integer,
                       Optional oversampling As Integer = 8,
                       Optional powerIterations As Integer = 2,
                       Optional seed As Integer = 0,
                       Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD)

            If A Is Nothing Then
                Throw New ArgumentNullException(NameOf(A))
            End If

            Call Initialize(
                apply:=Function(v As Double()) A.Multiply(v),
                applyTranspose:=Function(v As Double()) A.MultiplyTranspose(v),
                rows:=A.RowDimension,
                columns:=A.ColumnDimension,
                k:=k,
                oversampling:=oversampling,
                powerIterations:=powerIterations,
                seed:=seed,
                solver:=solver)
        End Sub

        ''' <summary>
        ''' 对 CSR 压缩稀疏矩阵做随机化截断 SVD。
        ''' </summary>
        ''' <param name="A">待分解的 CSR 矩阵（例如单细胞计数矩阵）</param>
        ''' <param name="k">请求保留的分量个数</param>
        ''' <param name="oversampling">过采样维度，默认 8</param>
        ''' <param name="powerIterations">幂迭代次数，默认 2</param>
        ''' <param name="seed">随机测试矩阵的种子</param>
        ''' <param name="solver">小矩阵分解路径</param>
        Public Sub New(A As Compressed.CsrMatrix, k As Integer,
                       Optional oversampling As Integer = 8,
                       Optional powerIterations As Integer = 2,
                       Optional seed As Integer = 0,
                       Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD)

            If A Is Nothing Then
                Throw New ArgumentNullException(NameOf(A))
            End If

            Call Initialize(
                apply:=Function(v As Double()) A.Multiply(v),
                applyTranspose:=Function(v As Double()) A.MultiplyTranspose(v),
                rows:=A.RowDimension,
                columns:=A.ColumnDimension,
                k:=k,
                oversampling:=oversampling,
                powerIterations:=powerIterations,
                seed:=seed,
                solver:=solver)
        End Sub

        ''' <summary>
        ''' 内部空构造：供 <see cref="FromOperator"/> 这类工厂在没有具体矩阵对象时使用。
        ''' </summary>
        Private Sub New()
        End Sub

        ''' <summary>
        ''' 委托工厂：把任意的"算子 + 转置算子"包装成一次随机化截断 SVD。
        ''' </summary>
        ''' <param name="apply"><c>v ↦ A·v</c>，输入长度必须等于 <paramref name="columns"/>，输出长度必须等于 <paramref name="rows"/></param>
        ''' <param name="applyTranspose"><c>v ↦ Aᵀ·v</c>，输入长度必须等于 <paramref name="rows"/>，输出长度必须等于 <paramref name="columns"/></param>
        ''' <param name="rows">逻辑行数 m</param>
        ''' <param name="columns">逻辑列数 n</param>
        ''' <param name="k">请求保留的分量个数</param>
        ''' <param name="oversampling">过采样维度，默认 8</param>
        ''' <param name="powerIterations">幂迭代次数，默认 2</param>
        ''' <param name="seed">随机测试矩阵的种子</param>
        ''' <param name="solver">小矩阵分解路径</param>
        ''' <remarks>
        ''' 这个入口用于"矩阵本身不需要物化"的场景，例如 <see cref="TruncatedPCA"/> 需要把
        ''' "中心化（+ 标准化）"表达成算子、而不想为 <c>6000×18000</c> 的输入再复制一份稠密矩阵。
        ''' 算子必须是确定性的纯函数：同一输入必须返回同一输出，否则幂迭代与 QR 的语义会被破坏。
        ''' </remarks>
        Public Shared Function FromOperator(apply As Func(Of Double(), Double()),
                                            applyTranspose As Func(Of Double(), Double()),
                                            rows As Integer,
                                            columns As Integer,
                                            k As Integer,
                                            Optional oversampling As Integer = 8,
                                            Optional powerIterations As Integer = 2,
                                            Optional seed As Integer = 0,
                                            Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD) As RandomizedSVD
            If apply Is Nothing Then
                Throw New ArgumentNullException(NameOf(apply))
            End If
            If applyTranspose Is Nothing Then
                Throw New ArgumentNullException(NameOf(applyTranspose))
            End If

            Dim svd As New RandomizedSVD()

            Call svd.Initialize(apply, applyTranspose, rows, columns, k,
                                oversampling, powerIterations, seed, solver)

            Return svd
        End Function

#End Region

#Region "Public Properties"

        ''' <summary>
        ''' 左奇异向量矩阵 <c>U</c>（<c>m×r</c>，列正交），<c>r = RetainedRank</c>。
        ''' </summary>
        ''' <returns>内部数组的深拷贝</returns>
        Public ReadOnly Property U As Double()()
            Get
                Return CloneMatrix(m_valueU)
            End Get
        End Property

        ''' <summary>
        ''' 前 <c>r</c> 个奇异值，按降序排列（<c>r = RetainedRank</c>）。
        ''' </summary>
        ''' <returns>内部数组的拷贝</returns>
        Public ReadOnly Property SingularValues As Double()
            Get
                If m_s Is Nothing Then
                    Return New Double(-1) {}
                End If

                Return DirectCast(m_s.Clone(), Double())
            End Get
        End Property

        ''' <summary>
        ''' 右奇异向量矩阵 <c>V</c>（<c>n×r</c>，列正交），满足 <c>A ≈ U·Σ·Vᵀ</c>。
        ''' </summary>
        ''' <returns>内部数组的深拷贝</returns>
        Public ReadOnly Property V As Double()()
            Get
                Return CloneMatrix(m_valueV)
            End Get
        End Property

        ''' <summary>
        ''' 分量矩阵（<c>r×n</c>），即 <c>Vᵀ</c>：第 i 行是第 i 个右奇异向量。
        ''' </summary>
        ''' <remarks>
        ''' 对"样本 × 特征"形态的数据，这个矩阵就是<b>特征空间（基因空间）的载荷</b>，
        ''' 与 <see cref="TruncatedPCA.Components"/> 的含义一致。
        ''' </remarks>
        Public ReadOnly Property Components As Double()()
            Get
                Dim rank As Integer = m_retainedRank

                If rank = 0 Then
                    Return New Double(-1)() {}
                End If

                Dim loadings As Double()() = RectangularArray.Matrix(Of Double)(rank, m_columns)

                For i As Integer = 0 To rank - 1
                    For j As Integer = 0 To m_columns - 1
                        loadings(i)(j) = m_valueV(j)(i)
                    Next
                Next

                Return loadings
            End Get
        End Property

        ''' <summary>
        ''' 降维结果 <c>U·Σ</c>（<c>m×r</c>），等价于 <c>A·V</c>（在截断子空间上的最优近似）。
        ''' </summary>
        Public ReadOnly Property ReducedMatrix As Double()()
            Get
                Return CloneMatrix(m_reduced)
            End Get
        End Property

        ''' <summary>
        ''' 容差截断后的数值秩：在 ℓ 个小奇异值中统计 <c>σ &gt; SpectrumTolerance</c> 的个数。
        ''' </summary>
        ''' <remarks>
        ''' 这个量描述"被随机子空间捕捉到的谱里有多少个可信方向"，
        ''' 与请求的 <see cref="RequestedRank"/> 无关；实际返回的分量个数见 <see cref="RetainedRank"/>。
        ''' </remarks>
        Public ReadOnly Property Rank As Integer
            Get
                Return m_numericalRank
            End Get
        End Property

        ''' <summary>
        ''' 实际返回的分量个数 <c>r = min(RequestedRank, Rank)</c>；
        ''' <see cref="U"/>、<see cref="SingularValues"/>、<see cref="V"/> 都按这个长度输出。
        ''' </summary>
        Public ReadOnly Property RetainedRank As Integer
            Get
                Return m_retainedRank
            End Get
        End Property

        ''' <summary>
        ''' 调用方请求的分量个数 <c>k</c>。
        ''' </summary>
        Public ReadOnly Property RequestedRank As Integer
            Get
                Return m_requestedRank
            End Get
        End Property

        ''' <summary>
        ''' 谱容差 <c>eps·max(m,n)·max(σ₁,1)</c>，用于判定"可忽略的奇异值"。
        ''' </summary>
        Public ReadOnly Property SpectrumTolerance As Double
            Get
                Return m_tolerance
            End Get
        End Property

        ''' <summary>
        ''' 本次随机子空间捕捉到的总方差 <c>Σ σᵢ²</c>（对 ℓ 个小奇异值求和，不只是保留的 k 个）。
        ''' </summary>
        ''' <remarks>
        ''' 由于 <c>‖A‖_F² = Σ σᵢ²</c>，当有效秩小于 ℓ（即子空间已经覆盖全部非零谱）时，
        ''' 这个量就等于 <c>‖A‖_F²</c>，可以作为"解释方差比"的分母；
        ''' 反之它只是真实总方差的一个下界。需要严格的 <c>‖A‖_F</c> 时请由调用方自行按输入计算。
        ''' </remarks>
        Public ReadOnly Property CapturedVariance As Double
            Get
                Return m_capturedVariance
            End Get
        End Property

        ''' <summary>
        ''' 过采样维度（ℓ = k + oversampling，再与 min(m, n) 取小）。
        ''' </summary>
        Public ReadOnly Property Oversampling As Integer
            Get
                Return m_oversampling
            End Get
        End Property

        ''' <summary>
        ''' 子空间幂迭代次数（每次迭代含两次 QR 重正交）。
        ''' </summary>
        Public ReadOnly Property PowerIterations As Integer
            Get
                Return m_powerIterations
            End Get
        End Property

        ''' <summary>
        ''' 本次分解使用的随机种子。
        ''' </summary>
        Public ReadOnly Property Seed As Integer
            Get
                Return m_seed
            End Get
        End Property

        ''' <summary>
        ''' 小矩阵分解路径。
        ''' </summary>
        Public ReadOnly Property Solver As RandomizedSvdSolver
            Get
                Return m_solver
            End Get
        End Property

        ''' <summary>
        ''' 实际执行的"矩阵乘向量"次数（含转置方向），用于性能核对与回归监控。
        ''' </summary>
        Public ReadOnly Property OperatorApplications As Long
            Get
                Return m_applications
            End Get
        End Property

        ''' <summary>
        ''' 输入矩阵的行数 <c>m</c>。
        ''' </summary>
        Public ReadOnly Property RowCount As Integer
            Get
                Return m_rows
            End Get
        End Property

        ''' <summary>
        ''' 输入矩阵的列数 <c>n</c>。
        ''' </summary>
        Public ReadOnly Property ColumnCount As Integer
            Get
                Return m_columns
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' 一步式入口：对稠密数组做随机化截断 SVD 并返回降维矩阵（<c>m×k</c>）。
        ''' </summary>
        Public Shared Function Reduce(A As Double()(), k As Integer,
                                      Optional oversampling As Integer = 8,
                                      Optional powerIterations As Integer = 2,
                                      Optional seed As Integer = 0,
                                      Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD) As Double()()
            Return New RandomizedSVD(A, k, oversampling, powerIterations, seed, solver).ReducedMatrix
        End Function

        ''' <summary>
        ''' 一步式入口：对稀疏矩阵做随机化截断 SVD 并返回降维矩阵（<c>m×k</c>）。
        ''' </summary>
        Public Shared Function Reduce(A As SparseMatrix, k As Integer,
                                      Optional oversampling As Integer = 8,
                                      Optional powerIterations As Integer = 2,
                                      Optional seed As Integer = 0,
                                      Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD) As Double()()
            Return New RandomizedSVD(A, k, oversampling, powerIterations, seed, solver).ReducedMatrix
        End Function

        ''' <summary>
        ''' 一步式入口：对 CSR 矩阵做随机化截断 SVD 并返回降维矩阵（<c>m×k</c>）。
        ''' </summary>
        Public Shared Function Reduce(A As Compressed.CsrMatrix, k As Integer,
                                      Optional oversampling As Integer = 8,
                                      Optional powerIterations As Integer = 2,
                                      Optional seed As Integer = 0,
                                      Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD) As Double()()
            Return New RandomizedSVD(A, k, oversampling, powerIterations, seed, solver).ReducedMatrix
        End Function

#End Region

#Region "Core pipeline"

        ''' <summary>
        ''' 主流程：参数校验 → 范围查找 → 幂迭代 → 小矩阵分解 → 容差截断。
        ''' </summary>
        Private Sub Initialize(apply As Func(Of Double(), Double()),
                               applyTranspose As Func(Of Double(), Double()),
                               rows As Integer,
                               columns As Integer,
                               k As Integer,
                               oversampling As Integer,
                               powerIterations As Integer,
                               seed As Integer,
                               solver As RandomizedSvdSolver)

            If k < 0 Then
                Throw New ArgumentException($"parameter k({k}) must not be negative!", NameOf(k))
            End If
            If oversampling < 0 Then
                Throw New ArgumentException($"parameter oversampling({oversampling}) must not be negative!", NameOf(oversampling))
            End If
            If powerIterations < 0 Then
                Throw New ArgumentException($"parameter powerIterations({powerIterations}) must not be negative!", NameOf(powerIterations))
            End If

            m_apply = apply
            m_applyTranspose = applyTranspose
            m_rows = rows
            m_columns = columns
            m_requestedRank = k
            m_oversampling = oversampling
            m_powerIterations = powerIterations
            m_seed = seed
            m_solver = solver
            m_applications = 0

            If rows = 0 OrElse columns = 0 Then
                Call EmptyResult()
                Return
            End If
            If k > stdf.Min(rows, columns) Then
                Throw New ArgumentException(
                    $"parameter k({k}) can not exceed the smaller dimension min(m,n) = {stdf.Min(rows, columns)}!", NameOf(k))
            End If
            If k = 0 Then
                Call EmptyResult()
                Return
            End If

            ' ℓ = min(k + oversampling, min(m, n))：过采样维度不能把子空间撑得比矩阵还大
            Dim ell As Integer = stdf.Min(k + oversampling, stdf.Min(rows, columns))

            ' 1) 局部种子随机源生成 n×ℓ 的高斯测试矩阵 Ω
            Dim omega As Double()() = MakeGaussianNoise(columns, ell, seed)

            ' 2) 范围查找：Y = A·Ω，再经济型 QR 得到正交基 Q（m×ℓ）
            Dim Q As Double()() = QrEconomy(ApplyBatch(omega, ell), rows, ell)

            ' 3) 幂迭代：每轮两次 QR 重正交，压制 AᵀA 幂次上的舍入放大
            For pass As Integer = 1 To powerIterations
                Dim Z As Double()() = ApplyTransposeBatch(Q, ell)
                Dim Qz As Double()() = QrEconomy(Z, columns, ell)
                Q = QrEconomy(ApplyBatch(Qz, ell), rows, ell)
            Next

            ' 4) W = Aᵀ·Q = (Qᵀ·A)ᵀ
            Dim W As Double()() = ApplyTransposeBatch(Q, ell)

            If solver = RandomizedSvdSolver.GramEigen Then
                Call SolveByGram(Q, W, ell)
            Else
                Call SolveBySmallSvd(Q, W, ell)
            End If
        End Sub

        ''' <summary>
        ''' 收尾路径 A：对 <c>W = Aᵀ·Q</c>（n×ℓ，等价于 <c>(QᵀA)ᵀ</c>）做经济型 SVD。
        ''' </summary>
        ''' <remarks>
        ''' <c>W = V_b·Σ·U_bᵀ</c>，因此 <c>U = Q·U_b</c>、<c>V = V_b</c>。
        ''' 由于 <c>W</c> 的行数 n 不小于列数 ℓ，LINPACK 的实现只会构造
        ''' <c>n×ℓ</c> 的左因子与 <c>ℓ×ℓ</c> 的右因子，不会出现 <c>n×n</c> 的中间矩阵。
        ''' </remarks>
        Private Sub SolveBySmallSvd(Q As Double()(), W As Double()(), ell As Integer)
            Dim svd As New SingularValueDecomposition(New NumericMatrix(W, m_columns, ell))
            Dim sv As Double() = svd.SingularValues.ToArray()
            Dim leftFactor As Double()() = svd.U.ArrayPack(deepcopy:=True)   ' n×ℓ，即 V_b
            Dim rightFactor As Double()() = svd.V.ArrayPack(deepcopy:=True)  ' ℓ×ℓ，即 U_b

            m_tolerance = SpectrumCutoff(sv)
            m_numericalRank = CountAbove(sv, m_tolerance)
            m_capturedVariance = SumSquares(sv)

            Dim kept As Integer = stdf.Min(m_requestedRank, m_numericalRank)

            m_retainedRank = kept
            m_s = New Double(kept - 1) {}
            m_valueU = RectangularArray.Matrix(Of Double)(m_rows, kept)
            m_valueV = RectangularArray.Matrix(Of Double)(m_columns, kept)

            For c As Integer = 0 To kept - 1
                m_s(c) = sv(c)

                For i As Integer = 0 To m_rows - 1
                    Dim acc As Double = 0.0

                    For t As Integer = 0 To ell - 1
                        acc += Q(i)(t) * rightFactor(t)(c)
                    Next

                    m_valueU(i)(c) = acc
                Next

                For i As Integer = 0 To m_columns - 1
                    m_valueV(i)(c) = leftFactor(i)(c)
                Next
            Next

            Call BuildReduced()
        End Sub

        ''' <summary>
        ''' 收尾路径 B：对 ℓ×ℓ 的 Gram 矩阵 <c>Qᵀ·A·Aᵀ·Q</c> 做对称特征分解。
        ''' </summary>
        Private Sub SolveByGram(Q As Double()(), W As Double()(), ell As Integer)
            Dim Y2 As Double()() = ApplyBatch(W, ell)                 ' A·W（m×ℓ）
            Dim gram As Double()() = RectangularArray.Matrix(Of Double)(ell, ell)

            For a As Integer = 0 To ell - 1
                For b As Integer = 0 To ell - 1
                    Dim acc As Double = 0.0

                    For i As Integer = 0 To m_rows - 1
                        acc += Q(i)(a) * Y2(i)(b)
                    Next

                    gram(a)(b) = acc
                Next
            Next

            ' 对称化：Gram 矩阵理论上对称，浮点舍入会破坏精确对称性，
            ' 而对称特征分解的路径判定依赖精确对称
            For a As Integer = 0 To ell - 1
                For b As Integer = a + 1 To ell - 1
                    Dim avg As Double = (gram(a)(b) + gram(b)(a)) / 2.0

                    gram(a)(b) = avg
                    gram(b)(a) = avg
                Next
            Next

            Dim eig As New EigenvalueDecomposition(New NumericMatrix(gram, ell, ell))
            Dim lambda As Double() = eig.RealEigenvalues
            Dim vectors As Double()() = eig.V.ArrayPack(deepcopy:=True)
            Dim sv(ell - 1) As Double

            For i As Integer = 0 To ell - 1
                sv(i) = stdf.Sqrt(stdf.Max(lambda(i), 0.0))
            Next

            Dim order As Integer() = Enumerable.Range(0, ell) _
                .OrderByDescending(Function(i) sv(i)) _
                .ToArray

            Dim sorted(ell - 1) As Double

            For i As Integer = 0 To ell - 1
                sorted(i) = sv(order(i))
            Next

            m_tolerance = SpectrumCutoff(sorted)
            m_numericalRank = CountAbove(sorted, m_tolerance)
            m_capturedVariance = SumSquares(sorted)

            Dim kept As Integer = stdf.Min(m_requestedRank, m_numericalRank)

            m_retainedRank = kept
            m_s = New Double(kept - 1) {}
            m_valueU = RectangularArray.Matrix(Of Double)(m_rows, kept)
            m_valueV = RectangularArray.Matrix(Of Double)(m_columns, kept)

            For c As Integer = 0 To kept - 1
                m_s(c) = sorted(c)

                Dim eigen As Integer = order(c)

                For i As Integer = 0 To m_rows - 1
                    Dim acc As Double = 0.0

                    For t As Integer = 0 To ell - 1
                        acc += Q(i)(t) * vectors(t)(eigen)
                    Next

                    m_valueU(i)(c) = acc
                Next

                For i As Integer = 0 To m_columns - 1
                    Dim acc As Double = 0.0

                    For t As Integer = 0 To ell - 1
                        acc += W(i)(t) * vectors(t)(eigen)
                    Next

                    m_valueV(i)(c) = If(m_s(c) > 0.0, acc / m_s(c), 0.0)
                Next
            Next

            Call BuildReduced()
        End Sub

        ''' <summary>
        ''' 由 <see cref="U"/> 与奇异值组装 <c>U·Σ</c>。
        ''' </summary>
        Private Sub BuildReduced()
            Dim kept As Integer = m_retainedRank

            m_reduced = RectangularArray.Matrix(Of Double)(m_rows, kept)

            For i As Integer = 0 To m_rows - 1
                For c As Integer = 0 To kept - 1
                    m_reduced(i)(c) = m_valueU(i)(c) * m_s(c)
                Next
            Next
        End Sub

        ''' <summary>
        ''' 空输入的约定：三个因子都是"零列"矩阵（行数保留、列数为 0）。
        ''' </summary>
        Private Sub EmptyResult()
            m_s = New Double(-1) {}
            m_valueU = RectangularArray.Matrix(Of Double)(m_rows, 0)
            m_valueV = RectangularArray.Matrix(Of Double)(m_columns, 0)
            m_reduced = RectangularArray.Matrix(Of Double)(m_rows, 0)
            m_numericalRank = 0
            m_retainedRank = 0
            m_tolerance = 0.0
            m_capturedVariance = 0.0
        End Sub

        ''' <summary>
        ''' 谱容差：<c>eps·max(m,n)·max(σ₁,1)</c>，与 <see cref="SingularValueDecomposition.Rank"/> 的口径一致。
        ''' </summary>
        Private Function SpectrumCutoff(sv As Double()) As Double
            Dim sigma As Double = If(sv IsNot Nothing AndAlso sv.Length > 0, sv(0), 0.0)

            Return Epsilon * stdf.Max(m_rows, m_columns) * stdf.Max(sigma, 1.0)
        End Function

        ''' <summary>
        ''' <c>Σ xᵢ²</c>，用于统计被捕捉到的总方差。
        ''' </summary>
        Private Shared Function SumSquares(x As Double()) As Double
            Dim sum As Double = 0.0

            If x Is Nothing Then
                Return 0.0
            End If

            For i As Integer = 0 To x.Length - 1
                sum += x(i) * x(i)
            Next

            Return sum
        End Function

        Private Shared Function CountAbove(sv As Double(), tolerance As Double) As Integer
            Dim count As Integer = 0

            If sv Is Nothing Then
                Return 0
            End If

            For i As Integer = 0 To sv.Length - 1
                If sv(i) > tolerance Then
                    count += 1
                End If
            Next

            Return count
        End Function

#End Region

#Region "Linear operator wrappers"

        ''' <summary>
        ''' 批量右乘：<c>M = A·X</c>，返回 <c>m×ℓ</c>（X 为 <c>n×ℓ</c>）。
        ''' </summary>
        Private Function ApplyBatch(X As Double()(), ell As Integer) As Double()()
            m_applications += ell

            Dim result As Double()() = RectangularArray.Matrix(Of Double)(m_rows, ell)
            Dim column As Double() = New Double(m_columns - 1) {}

            For j As Integer = 0 To ell - 1
                For i As Integer = 0 To m_columns - 1
                    column(i) = X(i)(j)
                Next

                Dim y As Double() = m_apply(column)

                If y Is Nothing OrElse y.Length <> m_rows Then
                    Throw New InvalidOperationException(
                        $"the linear operator returned {(If(y Is Nothing, 0, y.Length))} value(s), but {m_rows} was required")
                End If

                For i As Integer = 0 To m_rows - 1
                    result(i)(j) = y(i)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 批量转置右乘：<c>M = Aᵀ·X</c>，返回 <c>n×ℓ</c>（X 为 <c>m×ℓ</c>）。
        ''' </summary>
        Private Function ApplyTransposeBatch(X As Double()(), ell As Integer) As Double()()
            m_applications += ell

            Dim result As Double()() = RectangularArray.Matrix(Of Double)(m_columns, ell)
            Dim column As Double() = New Double(m_rows - 1) {}

            For j As Integer = 0 To ell - 1
                For i As Integer = 0 To m_rows - 1
                    column(i) = X(i)(j)
                Next

                Dim y As Double() = m_applyTranspose(column)

                If y Is Nothing OrElse y.Length <> m_columns Then
                    Throw New InvalidOperationException(
                        $"the transposed linear operator returned {(If(y Is Nothing, 0, y.Length))} value(s), but {m_columns} was required")
                End If

                For i As Integer = 0 To m_columns - 1
                    result(i)(j) = y(i)
                Next
            Next

            Return result
        End Function

        ''' <summary>
        ''' 稠密矩阵乘向量：<c>y = A·v</c>。
        ''' </summary>
        Private Shared Function DenseMultiply(A As Double()(), v As Double()) As Double()
            Dim rows As Integer = A.Length
            Dim y As Double() = New Double(rows - 1) {}

            If rows = 0 Then
                Return y
            End If

            Dim columns As Integer = A(0).Length

            For i As Integer = 0 To rows - 1
                Dim row As Double() = A(i)
                Dim acc As Double = 0.0

                For j As Integer = 0 To columns - 1
                    acc += row(j) * v(j)
                Next

                y(i) = acc
            Next

            Return y
        End Function

        ''' <summary>
        ''' 稠密转置乘向量：<c>y = Aᵀ·v</c>。
        ''' </summary>
        Private Shared Function DenseMultiplyTranspose(A As Double()(), v As Double()) As Double()
            Dim rows As Integer = A.Length

            If rows = 0 Then
                Return New Double(-1) {}
            End If

            Dim columns As Integer = A(0).Length
            Dim y As Double() = New Double(columns - 1) {}

            For i As Integer = 0 To rows - 1
                Dim row As Double() = A(i)
                Dim scale As Double = v(i)

                ' 单细胞/表达矩阵通常很稀疏，零元直接跳过可以省掉大量乘加
                If scale = 0.0 Then
                    Continue For
                End If

                For j As Integer = 0 To columns - 1
                    y(j) += row(j) * scale
                Next
            Next

            Return y
        End Function

#End Region

#Region "Private helpers"

        ''' <summary>
        ''' 机器精度（2⁻⁵²）。
        ''' </summary>
        Private Shared ReadOnly Property Epsilon As Double
            Get
                Return 2.0 ^ -52.0
            End Get
        End Property

        ''' <summary>
        ''' 用局部种子随机源生成 <c>n×ℓ</c> 的标准正态矩阵（Box–Muller 变换）。
        ''' </summary>
        ''' <remarks>
        ''' 逐行、逐列固定顺序调用 <see cref="Random.NextDouble"/>，因此同一 seed 的位流完全一致；
        ''' 不使用 <c>RandomExtensions.NextGaussian()</c>（那是全局共享状态，无法按调用点复现）。
        ''' </remarks>
        Private Shared Function MakeGaussianNoise(n As Integer, ell As Integer, seed As Integer) As Double()()
            Dim rnd As New Random(seed)
            Dim omega As Double()() = RectangularArray.Matrix(Of Double)(n, ell)
            Dim twoPi As Double = 2.0 * stdf.PI

            For i As Integer = 0 To n - 1
                Dim row As Double() = omega(i)

                For j As Integer = 0 To ell - 1
                    ' u1 取 (0,1]，避免 log(0) 产生 -Inf
                    Dim u1 As Double = 1.0 - rnd.NextDouble()
                    Dim u2 As Double = rnd.NextDouble()

                    row(j) = stdf.Sqrt(-2.0 * stdf.Log(u1)) * stdf.Cos(twoPi * u2)
                Next
            Next

            Return omega
        End Function

        ''' <summary>
        ''' 对 <c>m×ℓ</c>（m ≥ ℓ）矩阵做经济型 QR 并返回 m×ℓ 的正交因子。
        ''' </summary>
        Private Shared Function QrEconomy(Y As Double()(), m As Integer, ell As Integer) As Double()()
            If ell = 0 Then
                Return RectangularArray.Matrix(Of Double)(m, 0)
            End If

            Dim qr As New QRDecomposition(New NumericMatrix(Y, m, ell))

            Return qr.Q.ArrayPack(deepcopy:=True)
        End Function

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
