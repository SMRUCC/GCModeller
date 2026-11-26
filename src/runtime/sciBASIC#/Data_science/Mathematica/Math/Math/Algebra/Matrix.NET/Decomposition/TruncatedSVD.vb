Imports Microsoft.VisualBasic.ComponentModel.Collection

Namespace LinearAlgebra.Matrix

    ''' <summary>
    ''' 截断奇异值分解（Truncated Singular Value Decomposition）：稀疏矩阵的兼容入口。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 本类现在只是 <see cref="RandomizedSVD"/> 的<b>薄封装</b>：算法（Halko–Martinsson–Tropp 的
    ''' 随机化 SVD，即 sklearn <c>TruncatedSVD(solver="randomized")</c> 所用的方法）、幂迭代、
    ''' QR 重正交与容差截断全部由 <see cref="RandomizedSVD"/> 实现，这里只负责：
    ''' </para>
    ''' <list type="bullet">
    ''' <item><description>保持历史上"只接受 <see cref="SparseMatrix"/>"的入口形态；</description></item>
    ''' <item><description>保持输出<b>恰好 k 列</b>的尺寸契约（数值秩不足 k 时用零列补齐）；</description></item>
    ''' <item><description>保持旧的默认参数（<c>oversampling = 10</c>、<c>powerIterations = 1</c>），
    ''' 避免既有调用方的行为漂移。</description></item>
    ''' </list>
    ''' <para>
    ''' 与完整的 <see cref="SingularValueDecomposition"/>（LINPACK 稠密算法，需要把矩阵完全稠密化，
    ''' <c>O(m·n)</c> 内存）不同，本算法只依赖稀疏矩阵-向量乘法（<c>A·x</c> 与 <c>Aᵀ·y</c>，
    ''' 均为 <c>O(nnz)</c>），时间复杂度 <c>O(nnz·ℓ·(2q+3) + (m+n)·ℓ² + ℓ³)</c>、
    ''' 空间 <c>O(nnz + (m+n)·ℓ)</c>，其中 <c>ℓ = k + oversampling</c>、<c>q</c> 为幂迭代次数。
    ''' </para>
    ''' <para>
    ''' 需要稠密数组或 CSR 输入、或者需要"按容差自动截断有效秩"时，请直接使用
    ''' <see cref="RandomizedSVD"/>；需要"样本 × 特征"形态的主成分分析（基因空间载荷 +
    ''' 解释方差）时请使用 <see cref="TruncatedPCA"/>。
    ''' </para>
    ''' </remarks>
    Public Class TruncatedSVD

#Region "Class variables"

        ''' <summary>
        ''' 左奇异向量矩阵 U（m×k，列正交）。
        ''' </summary>
        Dim m_valueU As Double()()

        ''' <summary>
        ''' 右奇异向量矩阵 V（n×k，列正交）。
        ''' </summary>
        Dim m_valueV As Double()()

        ''' <summary>
        ''' 前 k 个奇异值（降序；数值秩不足的位置为 0）。
        ''' </summary>
        Dim m_s As Double()

        ''' <summary>
        ''' 降维结果矩阵（m×k，= U·Σ = A·V，稠密非稀疏）。
        ''' </summary>
        Dim m_reduced As Double()()

        ''' <summary>
        ''' 矩阵的行数 m。
        ''' </summary>
        Dim m As Integer

        ''' <summary>
        ''' 矩阵的列数 n。
        ''' </summary>
        Dim n As Integer

        ''' <summary>
        ''' 实际保留的分量个数 k（输出始终为 k 列）。
        ''' </summary>
        Dim k As Integer

        ''' <summary>
        ''' 随机子空间捕捉到的数值秩（诊断量，见 <see cref="RandomizedSVD.Rank"/>）。
        ''' </summary>
        Dim m_rank As Integer

        ''' <summary>
        ''' 谱容差（诊断量，见 <see cref="RandomizedSVD.SpectrumTolerance"/>）。
        ''' </summary>
        Dim m_tolerance As Double

#End Region

#Region "Constructor"

        ''' <summary>
        ''' 对高维稀疏矩阵执行截断的奇异值分解。
        ''' </summary>
        ''' <param name="A">
        ''' 待分解的 m×n 稀疏矩阵，可以是任意的非对称矩阵。
        ''' </param>
        ''' <param name="k">
        ''' 保留的分量个数（目标降维维度），必须满足 1 ≤ k ≤ min(m, n)。
        ''' </param>
        ''' <param name="oversampling">
        ''' 过采样维度，默认 10（保持历史默认值）。随机化算法使用 ℓ = k + oversampling 个随机
        ''' 采样方向来提升捕捉范围子空间的精度，之后再截断回前 k 个分量。
        ''' </param>
        ''' <param name="powerIterations">
        ''' 幂迭代次数，默认 1（保持历史默认值；0 表示不迭代）。当奇异值谱衰减较慢时，
        ''' 增加幂迭代次数可以显著提升精度，代价是每次迭代额外执行 2ℓ 次稀疏矩阵-向量乘法。
        ''' </param>
        ''' <param name="seed">
        ''' 随机测试矩阵的种子（新增的可选参数）。同一 seed 重复调用会得到逐位相同的结果；
        ''' 默认 0 表示使用固定种子而不是系统随机源，从而保证可复现。
        ''' </param>
        ''' <param name="solver">
        ''' 小矩阵分解路径（新增的可选参数）：默认 <see cref="RandomizedSvdSolver.SmallSVD"/>；
        ''' 传 <see cref="RandomizedSvdSolver.GramEigen"/> 可以复现历史的 Gram 特征分解路径。
        ''' </param>
        Public Sub New(A As SparseMatrix, k As Integer,
                       Optional oversampling As Integer = 10,
                       Optional powerIterations As Integer = 1,
                       Optional seed As Integer = 0,
                       Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD)

            ' ---------------- 参数校验（历史语义：k 必须为正） ----------------
            If A Is Nothing Then
                Throw New ArgumentNullException(NameOf(A))
            End If
            If k < 1 Then
                Throw New ArgumentException($"参数 k({k}) 必须大于等于 1！", NameOf(k))
            End If
            If oversampling < 0 Then
                Throw New ArgumentException($"参数 oversampling({oversampling}) 不能为负数！", NameOf(oversampling))
            End If
            If powerIterations < 0 Then
                Throw New ArgumentException($"参数 powerIterations({powerIterations}) 不能为负数！", NameOf(powerIterations))
            End If

            Dim engine As New RandomizedSVD(A, k, oversampling, powerIterations, seed, solver)

            Call Bind(engine, k)
        End Sub

        ''' <summary>
        ''' 把引擎的结果装进历史尺寸契约：输出固定 k 列，数值秩不足的部分保留零列。
        ''' </summary>
        Private Sub Bind(engine As RandomizedSVD, k As Integer)
            m = engine.RowCount
            n = engine.ColumnCount
            Me.k = k
            m_rank = engine.Rank
            m_tolerance = engine.SpectrumTolerance

            m_s = New Double(k - 1) {}
            m_valueU = RectangularArray.Matrix(Of Double)(m, k)
            m_valueV = RectangularArray.Matrix(Of Double)(n, k)

            Dim retained As Integer = engine.RetainedRank

            If retained > 0 Then
                Dim u As Double()() = engine.U
                Dim v As Double()() = engine.V
                Dim s As Double() = engine.SingularValues

                For c As Integer = 0 To retained - 1
                    m_s(c) = s(c)

                    For i As Integer = 0 To m - 1
                        m_valueU(i)(c) = u(i)(c)
                    Next

                    For i As Integer = 0 To n - 1
                        m_valueV(i)(c) = v(i)(c)
                    Next
                Next
            End If

            m_reduced = RectangularArray.Matrix(Of Double)(m, k)

            For i As Integer = 0 To m - 1
                For c As Integer = 0 To k - 1
                    m_reduced(i)(c) = m_valueU(i)(c) * m_s(c)
                Next
            Next
        End Sub

#End Region

#Region "Public Properties"

        ''' <summary>
        ''' 左奇异向量矩阵 U（m×k，列正交）。A ≈ U·Σ·Vᵀ。
        ''' </summary>
        ''' <returns>返回内部数组的副本，修改返回值不会影响分解结果。</returns>
        Public ReadOnly Property U As Double()()
            Get
                Return CloneMatrix(m_valueU)
            End Get
        End Property

        ''' <summary>
        ''' 前 k 个奇异值，按从大到小降序排列。Σ = diag(SingularValues)。
        ''' </summary>
        ''' <returns>返回内部数组的副本。</returns>
        Public ReadOnly Property SingularValues As Double()
            Get
                Return DirectCast(m_s.Clone(), Double())
            End Get
        End Property

        ''' <summary>
        ''' 右奇异向量矩阵 V（n×k，列正交）。A ≈ U·Σ·Vᵀ。
        ''' </summary>
        ''' <returns>返回内部数组的副本。</returns>
        Public ReadOnly Property V As Double()()
            Get
                Return CloneMatrix(m_valueV)
            End Get
        End Property

        ''' <summary>
        ''' 分量矩阵（k×n），即 V 的转置。第 i 行是第 i 个右奇异向量的转置，
        ''' 可用于将任意的 n 维观测向量投影降维到 k 维：y = x·Componentsᵀ。
        ''' </summary>
        ''' <returns>返回新生成的 k×n 稠密矩阵。</returns>
        Public ReadOnly Property Components As Double()()
            Get
                Dim comp As Double()() = RectangularArray.Matrix(Of Double)(k, n)

                For i As Integer = 0 To k - 1
                    For j As Integer = 0 To n - 1
                        comp(i)(j) = m_valueV(j)(i)
                    Next
                Next

                Return comp
            End Get
        End Property

        ''' <summary>
        ''' 降维结果矩阵（m×k，稠密非稀疏），等于 U·Σ = A·V。
        ''' <para>
        ''' 将原始高维稀疏矩阵 A 的每一行（n 维稀疏向量）投影到前 k 个
        ''' 最大奇异值对应的右奇异向量所张成的子空间中，得到 k 维稠密表示。
        ''' </para>
        ''' </summary>
        ''' <returns>返回内部数组的副本，修改返回值不会影响分解结果。</returns>
        Public ReadOnly Property ReducedMatrix As Double()()
            Get
                Return CloneMatrix(m_reduced)
            End Get
        End Property

        ''' <summary>
        ''' 随机子空间捕捉到的数值秩（按容差 <c>eps·max(m,n)·max(σ₁,1)</c> 判定）。
        ''' </summary>
        Public ReadOnly Property Rank As Integer
            Get
                Return m_rank
            End Get
        End Property

        ''' <summary>
        ''' 谱容差。
        ''' </summary>
        Public ReadOnly Property SpectrumTolerance As Double
            Get
                Return m_tolerance
            End Get
        End Property

#End Region

#Region "Public Methods"

        ''' <summary>
        ''' 一步式的便捷入口：对高维稀疏矩阵执行截断 SVD 并直接返回降维结果。
        ''' </summary>
        ''' <param name="A">待降维的 m×n 稀疏矩阵</param>
        ''' <param name="k">目标降维维度，1 ≤ k ≤ min(m, n)</param>
        ''' <param name="oversampling">过采样维度，默认 10</param>
        ''' <param name="powerIterations">幂迭代次数，默认 1</param>
        ''' <param name="seed">随机测试矩阵的种子</param>
        ''' <param name="solver">小矩阵分解路径</param>
        ''' <returns>m×k 的稠密（非稀疏）降维矩阵（= U·Σ = A·V）</returns>
        Public Shared Function Reduce(A As SparseMatrix, k As Integer,
                                      Optional oversampling As Integer = 10,
                                      Optional powerIterations As Integer = 1,
                                      Optional seed As Integer = 0,
                                      Optional solver As RandomizedSvdSolver = RandomizedSvdSolver.SmallSVD) As Double()()
            Return New TruncatedSVD(A, k, oversampling, powerIterations, seed, solver).ReducedMatrix
        End Function

#End Region

#Region "Private Helpers"

        ''' <summary>
        ''' 生成锯齿二维数组的深拷贝。
        ''' </summary>
        Private Shared Function CloneMatrix(X As Double()()) As Double()()
            If X Is Nothing OrElse X.Length = 0 Then
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
