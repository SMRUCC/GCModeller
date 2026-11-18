Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

Namespace Data

    ''' <summary>
    ''' 稳态求解过程的逐帧快照（阻尼迭代序列）。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 这里记录的是 <c>z ← z + ω·(F(z) − z)</c> 的<b>求解轨迹</b>，不是生物时间序列：
    ''' 改变 <see cref="Relaxation"/> 会改变曲线的形状，但只要收敛，终点不变。
    ''' 因此第 <c>t</c> 帧的正确说法是「第 t 次迭代」，而不是「第 t 个时间点」。
    ''' 只有当网络处在滞后（时序）路线且 <c>ω = 1</c> 时，一帧才对应一个采样间隔。
    ''' </para>
    ''' <para>
    ''' 每一帧都是<b>已施加钳制</b>之后的完整状态（原始表达量），所以被扰动的基因
    ''' 在轨迹上是一条水平线；这一点由记录点落在钳制回写之后保证。
    ''' </para>
    ''' <para>
    ''' <see cref="States"/> 与 <see cref="MaxDeltas"/> 的长度关系：
    ''' 每执行一步迭代追加一帧、同时追加一个 <c>maxDelta</c>，因此正常收敛时
    ''' <c>States.Count = Iterations + 1</c>、<c>MaxDeltas.Count = Iterations</c>；
    ''' 第 0 帧（起点）没有对应的变化量。
    ''' </para>
    ''' </remarks>
    Public Class SteadyStateTrace

        ''' <summary>轨迹的基因名（列顺序，与每帧数组的长度一致）。</summary>
        Public Property GeneNames As String()

        ''' <summary>逐帧状态（原始表达量）：<c>States(t)(g)</c> 是第 t 帧、第 g 个基因的取值。</summary>
        ''' <remarks><c>States(0)</c> 是起点（已施加钳制）。</remarks>
        Public Property States As New List(Of Double())()

        ''' <summary>逐帧最大变化量（z 空间），长度通常为 <c>States.Count − 1</c>。</summary>
        Public Property MaxDeltas As New List(Of Double)()

        ''' <summary>本次求解使用的阻尼系数 ω。</summary>
        Public Property Relaxation As Double

        ''' <summary>本次求解使用的收敛阈值（z 空间，按 <c>maxDelta</c> 判定）。</summary>
        Public Property Tolerance As Double

        ''' <summary>
        ''' 是否为「<c>maxDelta &lt; tolerance</c>」退出。
        ''' </summary>
        ''' <remarks>
        ''' 为 <c>False</c> 表示步数耗尽仍未收敛：轨迹后半段的平台是截断造成的，
        ''' 不能当成稳态。
        ''' </remarks>
        Public Property Converged As Boolean

        ''' <summary>是否为滞后（时序）路线；只有它为 <c>True</c> 且 ω = 1 时帧号才等于时间步。</summary>
        Public Property Lagged As Boolean

        Private _iterations As Integer = -1

        ''' <summary>实际执行的迭代步数（不含第 0 帧）。</summary>
        Public ReadOnly Property Iterations As Integer
            Get
                Return If(_iterations >= 0, _iterations, std.Max(0, States.Count - 1))
            End Get
        End Property

        ''' <summary>最后一帧（终态，原始表达量）。</summary>
        Public ReadOnly Property Final As Double()
            Get
                Return If(States.Count = 0, Nothing, States(States.Count - 1))
            End Get
        End Property

        ''' <summary>帧数。</summary>
        Public ReadOnly Property NFrames As Integer
            Get
                Return States.Count
            End Get
        End Property

        ''' <summary>基因数。</summary>
        Public ReadOnly Property NGene As Integer
            Get
                Return If(GeneNames Is Nothing, 0, GeneNames.Length)
            End Get
        End Property

        ''' <summary>
        ''' 第 <paramref name="frame"/> 帧的变化量；第 0 帧没有变化量，返回 <c>NaN</c>。
        ''' </summary>
        ''' <param name="frame">帧序号（0 为起点）</param>
        Public Function DeltaAt(frame As Integer) As Double
            Dim index As Integer = frame - 1

            If index < 0 OrElse index >= MaxDeltas.Count Then
                Return Double.NaN
            End If

            Return MaxDeltas(index)
        End Function

        ''' <summary>追加一帧（供 <see cref="FuzzyGeneNetwork"/> 的迭代器调用）。</summary>
        ''' <param name="state">该帧的原始表达量（长度必须等于 <see cref="GeneNames"/>）</param>
        ''' <param name="maxDelta">产生这一帧的那一步的最大变化量（第 0 帧传 0）</param>
        Friend Sub Push(state As Double(), maxDelta As Double)
            If state Is Nothing Then
                Throw New ArgumentNullException(NameOf(state))
            End If

            Call States.Add(state)

            ' 第 0 帧是起点，没有「产生它的那一步」
            If States.Count > 1 Then
                Call MaxDeltas.Add(maxDelta)
            End If
        End Sub

        ''' <summary>
        ''' 收口：记录实际迭代步数，并保证最后一帧与求解器的返回值逐位一致。
        ''' </summary>
        ''' <param name="finalState">求解器返回的终态（原始表达量）</param>
        ''' <param name="iterations">实际执行的迭代步数</param>
        ''' <remarks>
        ''' 末帧与终态不一致只可能出现在「一次迭代都没执行」或记录点被裁剪的情况下；
        ''' 这里补一帧而不是覆盖，避免改动已经记录下来的历史。
        ''' </remarks>
        Friend Sub Seal(finalState As Double(), iterations As Integer)
            _iterations = std.Max(0, iterations)

            If finalState Is Nothing Then
                Return
            End If
            If States.Count = 0 Then
                Call States.Add(CloneState(finalState))

                Return
            End If
            If SameState(States(States.Count - 1), finalState) Then
                Return
            End If

            Call States.Add(CloneState(finalState))
        End Sub

        ''' <summary>
        ''' 转成「行 = 基因、列 = 迭代帧」的表达矩阵。
        ''' </summary>
        ''' <param name="prefix">列名（样本 ID）前缀，列名形如 <c>{prefix}000</c></param>
        ''' <param name="tag">矩阵标签；<c>Nothing</c> 时自动生成</param>
        ''' <returns>可直接交给 <c>SaveMatrix</c> / <c>SaveFile</c> 的矩阵</returns>
        ''' <remarks>
        ''' 与 <c>RegulationMatrixExporter</c> 使用同一种组装方式，因此下游
        ''' （<c>write.expr_matrix</c>、<c>pheatmap</c>、<c>HTSMatrixReader</c>）的行为完全一致。
        ''' </remarks>
        Public Function ToMatrix(Optional prefix As String = "t", Optional tag As String = Nothing) As Matrix
            If States.Count = 0 Then
                Throw New InvalidOperationException("the trace does not contain any frame")
            End If

            Dim frames As Integer = States.Count
            Dim genes As Integer = NGene
            Dim rows(genes - 1) As DataFrameRow

            For g As Integer = 0 To genes - 1
                Dim values(frames - 1) As Double

                For t As Integer = 0 To frames - 1
                    values(t) = States(t)(g)
                Next

                rows(g) = New DataFrameRow With {
                    .geneID = GeneNames(g),
                    .experiments = values
                }
            Next

            Dim columns(frames - 1) As String
            Dim samplePrefix As String = If(prefix, "t")

            For t As Integer = 0 To frames - 1
                columns(t) = samplePrefix & t.ToString("000")
            Next

            Return New Matrix With {
                .tag = If(tag, $"fuzzygene steady-state trace ({genes} genes x {frames} frames, omega={Relaxation:G3})"),
                .sampleID = columns,
                .expression = rows
            }
        End Function

        Private Shared Function CloneState(state As Double()) As Double()
            Return CType(state.Clone(), Double())
        End Function

        Private Shared Function SameState(a As Double(), b As Double()) As Boolean
            If a Is Nothing OrElse b Is Nothing Then
                Return False
            End If
            If a.Length <> b.Length Then
                Return False
            End If

            For i As Integer = 0 To a.Length - 1
                If a(i) <> b(i) Then
                    Return False
                End If
            Next

            Return True
        End Function

        ''' <summary>单行摘要。</summary>
        Public Overrides Function ToString() As String
            Return $"steady-state trace: {NGene} genes x {NFrames} frame(s), " &
                   $"iterations={Iterations}, converged={Converged}, omega={Relaxation:G3}, tol={Tolerance:G3}"
        End Function

    End Class

End Namespace
