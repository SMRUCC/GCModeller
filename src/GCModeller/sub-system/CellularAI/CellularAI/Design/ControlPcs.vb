Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports std = System.Math

Namespace Design

    ''' <summary>
    ''' 控制细胞的基因空间主成分：抽样的对照细胞经 <c>log1p(TS·CPM)</c> 变换并逐基因中心化后，
    ''' 取前 <c>k</c> 个载荷与特征值。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 用途是把"每个基因各自独立的修正"变成"沿少数几个共表达程序的修正"：
    ''' <c>D' = D + λ·g_p·Vᵀ(V·D)</c> 会把偏离拉到主成分张成的子空间里，
    ''' 从而保留真实的共调控结构、抑制逐基因的噪声抖动。
    ''' </para>
    ''' <para>
    ''' <b>为什么用抽样而不是全部对照细胞</b>：上游固定抽 <c>pcs_cells = 6000</c> 个
    ''' （种子固定），既控制成本，也让不同上下文的主成分具有可比性（样本量相同）。
    ''' 抽样下标必须<b>排序</b>后再取（与上游 <c>np.sort</c> 一致），否则同样的种子会给出不同的行序，
    ''' 进而改变浮点累加顺序。
    ''' </para>
    ''' <para>
    ''' <b>特征值是 <c>σ²</c> 且不除以 n−1</b>：程序权重 <c>w = (ev/ev₀)^γ</c> 只用相对值，
    ''' 除以常数不改变结果，但保持与上游一致便于逐位对拍。
    ''' </para>
    ''' </remarks>
    Public Class ControlPcs

        ''' <summary>基因空间载荷（k×基因）。</summary>
        Public Property Loadings As Double()()

        ''' <summary>每个成分的特征值 <c>σ²</c>（长度 = 实际保留的秩）。</summary>
        Public Property Eigenvalues As Double()

        ''' <summary>实际保留的秩。</summary>
        Public ReadOnly Property Rank As Integer
            Get
                Return If(Loadings Is Nothing, 0, Loadings.Length)
            End Get
        End Property

        ''' <summary>
        ''' 计算控制细胞的主成分。
        ''' </summary>
        ''' <param name="controls">对照细胞矩阵（细胞 × 基因）</param>
        ''' <param name="options">总量设计参数（<c>pw_k</c> / <c>pcs_cells</c> / <c>pcs_seed</c>）</param>
        Public Shared Function Compute(controls As CsrMatrix, options As TotalsOptions) As ControlPcs
            If controls Is Nothing Then
                Throw New ArgumentNullException(NameOf(controls))
            End If

            Dim nCells As Integer = std.Min(options.PcsCells, controls.RowDimension)
            Dim k As Integer = std.Min(options.PwK, std.Min(nCells, controls.ColumnDimension))

            If nCells <= 0 OrElse k <= 0 Then
                Return New ControlPcs With {
                    .Loadings = RectangularArray.Matrix(Of Double)(0, controls.ColumnDimension),
                    .Eigenvalues = New Double(-1) {}
                }
            End If

            Dim rnd As New Random(options.PcsSeed)
            Dim indices As Integer() = MarginSolver.SampleIndices(rnd, controls.RowDimension, nCells, replace:=False)
            Dim sample As CsrMatrix = controls.Slice(indices)

            Return ComputeFromDense(ToLogCpm(sample, controls.ColumnDimension), k)
        End Function

        ''' <summary>
        ''' 直接对"已变换的稠密样本矩阵"（细胞 × 基因）做主成分；便于用mock数据做审计。
        ''' </summary>
        Public Shared Function ComputeFromDense(logCpm As Double()(), k As Integer) As ControlPcs
            If logCpm Is Nothing Then
                Throw New ArgumentNullException(NameOf(logCpm))
            End If

            Dim take As Integer = std.Min(k, std.Min(logCpm.Length, If(logCpm.Length = 0, 0, logCpm(0).Length)))

            If take <= 0 Then
                Return New ControlPcs With {
                    .Loadings = RectangularArray.Matrix(Of Double)(0, If(logCpm.Length = 0, 0, logCpm(0).Length)),
                    .Eigenvalues = New Double(-1) {}
                }
            End If

            ' TruncatedPCA 自行做列中心化（与上游 C -= C.mean(0) 一致）
            Dim pca As New TruncatedPCA(logCpm, take, center:=True, scale:=False,
                                        oversampling:=8, powerIterations:=2, seed:=0)

            Return New ControlPcs With {
                .Loadings = pca.Components,
                .Eigenvalues = pca.ExplainedVariance
            }
        End Function

        ''' <summary>把 CSR 子矩阵换成 <c>log1p(TS·CPM)</c> 的稠密矩阵。</summary>
        Private Shared Function ToLogCpm(sample As CsrMatrix, nGenes As Integer) As Double()()
            Dim nCells As Integer = sample.RowDimension
            Dim result As Double()() = RectangularArray.Matrix(Of Double)(nCells, nGenes)
            Dim librarySizes As Double() = sample.RowSums()

            For i As Integer = 0 To nCells - 1
                Dim librarySize As Double = std.Max(librarySizes(i), 1.0E-12)

                For j As Integer = 0 To nGenes - 1
                    result(i)(j) = 0.0
                Next

                For p As Integer = sample.Indptr(i) To sample.Indptr(i + 1) - 1
                    Dim gene As Integer = sample.Indices(p)

                    If gene >= 0 AndAlso gene < nGenes Then
                        result(i)(gene) = std.Log(1.0 + ProfileSpread.TS * sample.Values(p) / librarySize)
                    End If
                Next
            Next

            Return result
        End Function

        Public Overrides Function ToString() As String
            Return $"control PCs: {Rank} component(s)"
        End Function

    End Class
End Namespace
