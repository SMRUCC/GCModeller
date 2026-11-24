Imports System
Imports System.Threading.Tasks
Imports std = System.Math

Namespace Solver

    ''' <summary>
    ''' 传播映射 <c>F</c> 在 z 空间的解析 Jacobian：<c>J(g, j) = Σ_r w̄_r(z) · θ[r·(n+1)+j]</c>。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这个量在 <c>GeneRegulationModel.LocalSensitivity</c> 里已经实现过（用于导出带符号的边），
    ''' 这里只是把它从「一个基因的一行」扩展成「整张稀疏矩阵」，并复用它作为牛顿法的导数。
    ''' </para>
    ''' <para>
    ''' <b>必须写清楚的近似</b>：严格求导时 <c>w̄_r</c> 也依赖 <c>z</c>（前件的高斯隶属度会随输入变化），
    ''' 这里刻意忽略 <c>∂w̄/∂z</c>，只保留一阶 Sugeno 结论的线性系数。
    ''' 这与项目既定口径一致（边权重、<c>LocalSensitivity</c> 都是这么算的），
    ''' 属于准牛顿（Gauss–Newton 型）近似：不改收敛点，只影响收敛路径。
    ''' </para>
    ''' <para>
    ''' 存储是 <c>N×K</c>（K = 该基因的调控因子数 ≤ <c>MaxRegulators</c>），
    ''' 18533 基因 × 4 输入 ≈ 0.6 MB，比稠密 <c>N²</c> 小五个数量级；
    ''' 因此矩阵无关的 Krylov 求解只需一次 <c>N·K</c> 的 matvec。
    ''' </para>
    ''' </remarks>
    Friend Class JacobianOperator

        Private ReadOnly _net As FuzzyGeneNetwork
        Private ReadOnly _rows As Double()()
        Private ReadOnly _rowNorm As Double()
        Private ReadOnly _indices As Integer()()
        Private ReadOnly _n As Integer
        Private ReadOnly _scratch As New System.Threading.ThreadLocal(Of Double())(Function() New Double(-1) {})

        ''' <summary>绑定网络的 Jacobian 算子（缓冲按基因数一次性分配）。</summary>
        Public Sub New(network As FuzzyGeneNetwork)
            _net = network
            _n = network.NGene
            _rows = New Double(_n - 1)() {}
            _rowNorm = New Double(_n - 1) {}
            _indices = network.RegulatorIndexTable
        End Sub

        ''' <summary>基因数。</summary>
        Public ReadOnly Property NGene As Integer
            Get
                Return _n
            End Get
        End Property

        ''' <summary>第 <paramref name="gene"/> 行的灵敏度（长度 = 该基因的调控因子数；未建模为 <c>Nothing</c>）。</summary>
        Public Function Row(gene As Integer) As Double()
            Return _rows(gene)
        End Function

        ''' <summary>
        ''' 在当前状态上重建全部行。
        ''' </summary>
        ''' <param name="z">当前状态（z 空间）</param>
        ''' <param name="workers">并行度（<c>1</c> 表示串行）</param>
        ''' <remarks>
        ''' 每个基因只写自己的行，因此按基因并行天然无竞争、与线程调度无关；
        ''' 行内顺序固定为「规则升序累加」，与 <c>LocalSensitivity</c> 逐位一致。
        ''' </remarks>
        Public Sub Build(z As Double(), workers As Integer)
            If workers > 1 AndAlso _n >= 2048 Then
                Dim options As New ParallelOptions With {.MaxDegreeOfParallelism = workers}
                Dim chunk As Integer = std.Max(128, (_n + workers * 4 - 1) \ (workers * 4))

                Call Parallel.For(0, (_n + chunk - 1) \ chunk, options,
                    Sub(c)
                        Dim start As Integer = c * chunk

                        Call BuildRange(z, start, std.Min(start + chunk, _n))
                    End Sub)
            Else
                Call BuildRange(z, 0, _n)
            End If
        End Sub

        Private Sub BuildRange(z As Double(), start As Integer, ends As Integer)
            For g As Integer = start To ends - 1
                Dim model = _net.Models(g)

                If model Is Nothing Then
                    _rows(g) = Nothing
                    _rowNorm(g) = 0.0

                    Continue For
                End If

                Dim inputs As Integer() = _indices(g)

                If inputs.Length = 0 OrElse inputs.Length <> model.Network.InputCount Then
                    _rows(g) = Nothing
                    _rowNorm(g) = 0.0

                    Continue For
                End If

                Dim row As Double() = _scratch.Value

                If row.Length < inputs.Length Then
                    ReDim row(inputs.Length - 1)
                    _scratch.Value = row
                End If

                For j As Integer = 0 To inputs.Length - 1
                    row(j) = z(inputs(j))
                Next

                Dim sensitivity As Double()

                Try
                    sensitivity = model.LocalSensitivity(row)
                Catch ex As Exception
                    ' 单个基因的灵敏度失败（规则被剪空等）按「无入边」处理，不中断整次求解
                    sensitivity = Nothing
                End Try

                _rows(g) = sensitivity

                Dim l1 As Double = 0.0

                If sensitivity IsNot Nothing Then
                    For j As Integer = 0 To sensitivity.Length - 1
                        l1 += std.Abs(sensitivity(j))
                    Next
                End If

                _rowNorm(g) = l1
            Next
        End Sub

        ''' <summary>矩阵无关的 <c>y = J·x</c>（未建模基因对应 0）。</summary>
        Public Sub Matvec(x As Double(), y As Double())
            For g As Integer = 0 To _n - 1
                Dim row As Double() = _rows(g)

                If row Is Nothing Then
                    y(g) = 0.0

                    Continue For
                End If

                Dim inputs As Integer() = _indices(g)
                Dim sum As Double = 0.0

                For j As Integer = 0 To row.Length - 1
                    sum += row(j) * x(inputs(j))
                Next

                y(g) = sum
            Next
        End Sub

        ''' <summary>逐行 L1 范数（谱半径的保守上界 <c>ρ ≤ ‖J‖∞</c>）。</summary>
        Public Function RowNorms() As Double()
            Return _rowNorm
        End Function

        ''' <summary>逐行 L1 范数的最大值（无穷范数）。</summary>
        Public Function InfinityNorm() As Double
            Dim result As Double = 0.0

            For g As Integer = 0 To _n - 1
                If _rowNorm(g) > result Then
                    result = _rowNorm(g)
                End If
            Next

            Return result
        End Function

        ''' <summary>
        ''' 幂迭代估计谱半径 <c>ρ(J)</c>。
        ''' </summary>
        ''' <param name="iterations">幂迭代次数</param>
        ''' <returns>谱半径估计；全零矩阵返回 0</returns>
        ''' <remarks>
        ''' 用全 1 向量做初值而不是随机向量：结果与调用顺序无关、可复现，
        ''' 而且生物网络的 Jacobian 通常是正/负混合但不正交于全 1 向量，收敛性足够。
        ''' 非对称矩阵可能有复特征值，此时幂迭代会在末几位振荡，
        ''' 因此取最后若干次估计的最大值（宁可高估，校准只会更保守）。
        ''' </remarks>
        Public Function SpectralRadius(iterations As Integer) As Double
            Dim v(_n - 1) As Double
            Dim w(_n - 1) As Double
            Dim scale As Double = 1.0 / std.Sqrt(std.Max(1, _n))

            For g As Integer = 0 To _n - 1
                v(g) = scale
            Next

            Dim estimate As Double = 0.0

            For k As Integer = 0 To std.Max(1, iterations) - 1
                Call Matvec(v, w)

                Dim sum As Double = 0.0

                For g As Integer = 0 To _n - 1
                    sum += w(g) * w(g)
                Next

                Dim norm As Double = std.Sqrt(sum)

                If norm < 1.0E-14 Then
                    Return 0.0
                End If

                ' v 的 L2 范数为 1 时，||J·v|| 就是瑞利商模长的一阶估计
                If k >= iterations \ 2 AndAlso norm > estimate Then
                    estimate = norm
                End If

                For g As Integer = 0 To _n - 1
                    v(g) = w(g) / norm
                Next
            Next

            Return estimate
        End Function

    End Class

End Namespace
