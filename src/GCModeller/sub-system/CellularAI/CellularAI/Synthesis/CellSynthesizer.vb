Imports Microsoft.VisualBasic.ComponentModel.Collection
Imports Microsoft.VisualBasic.Math.LinearAlgebra.Matrix.Compressed
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Model
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Placement
Imports SMRUCC.genomics.Analysis.Illumina.CellularAI.Randomizer
Imports std = System.Math

Namespace Synthesis

    ''' <summary>
    ''' 一个扰动的细胞合成：把整数总量 + 文库大小 + 方向约束变成
    ''' <c>n_cells × G</c> 的非负整数计数矩阵。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 流程与上游 <c>synthesize</c> 一致：
    ''' </para>
    ''' <list type="number">
    ''' <item><description><b>文库大小排列</b>：把目标库大小随机置换（同一组大小、不同细胞），
    ''' 避免"最大的细胞总是拿最高的分位"这种系统性偏斜；</description></item>
    ''' <item><description><b>不可调用基因</b>（<c>mean_cpm ≤ eligible_cpm</c> 但总量 &gt; 0）：
    ''' 按库大小概率<b>突发散布</b>——把总量拆成若干块、每块落在一个随机细胞上，
    ''' 形成"少数细胞、大幅上调"的形态，而不是均匀撒开；</description></item>
    ''' <item><description><b>可调用基因</b>：先生成 <c>J×n</c> 的独立噪声，再用 copula 把
    ''' "基因名次"与"文库大小名次"耦合，交给 <see cref="BlockPlacement"/> 逐基因求解；
    ''' </description></item>
    ''' <item><description><b>降级重解</b>：被判为"反向离群"的基因用<b>实际库大小</b>再解一次
    ''' （rank-neutral），因为这些基因的计数变了，其它基因的分母也变了。</description></item>
    ''' </list>
    ''' <para>
    ''' <b>随机性来源</b>：全部来自 <see cref="SynthesisJob.Seed"/> 派生的
    ''' <see cref="Pcg64"/>；文库排列与突发散布用<b>克隆出来的子流</b>，
    ''' 保证"同种子同结果"且与并行度无关。
    ''' </para>
    ''' </remarks>
    Public Module CellSynthesizer

        ''' <summary>合成一个扰动。</summary>
        Public Function Synthesize(engine As SynthesisEngine, job As SynthesisJob,
                                   model As CellularAiModel) As CsrMatrix
            If engine Is Nothing Then
                Throw New ArgumentNullException(NameOf(engine))
            End If
            If job Is Nothing Then
                Throw New ArgumentNullException(NameOf(job))
            End If

            Call job.Validate()

            Dim options As SynthesisOptions = model.Synthesis
            Dim generator As New Pcg64(job.Seed)
            Dim n As Integer = job.CellCount
            Dim nGenes As Integer = engine.GeneCount

            ' ---- 1) 逐基因的目标 z ----
            Dim zTargets As Double() = engine.ZTargets(job.PlanIndex, job.PlanSign, job.PlanPriority,
                                                       job.NudgeSign, job.NudgeStrength, options)

            ' ---- 2) 文库大小置换（用克隆子流，不推进主流）----
            Dim libraries As Double() = DirectCast(job.Libraries.Clone(), Double())
            Dim child As Pcg64 = generator.Clone()

            Call child.Shuffle(libraries)

            Dim libraryTotal As Double = 0.0
            Dim geneTotal As Double = 0.0

            For i As Integer = 0 To n - 1
                libraryTotal += libraries(i)
            Next

            For g As Integer = 0 To nGenes - 1
                geneTotal += job.Totals(g)
            Next

            If std.Abs(libraryTotal - geneTotal) > 1.0 Then
                Throw New ArgumentException(
                    $"job '{job.Name}': the libraries sum {libraryTotal:F3} but the gene totals sum {geneTotal:F3}",
                    NameOf(job))
            End If

            ' ---- 3) 计数矩阵 ----
            Dim counts As Double()() = RectangularArray.Matrix(Of Double)(n, nGenes)

            ' ---- 4) 不可调用基因：突发散布 ----
            Dim scatter As Pcg64 = child.Clone()

            For g As Integer = 0 To nGenes - 1
                If engine.Eligible(g) OrElse job.Totals(g) <= 0.0 Then
                    Continue For
                End If

                Call ScatterBursts(counts, g, job.Totals(g), libraries, options.NeBurst, scatter)
            Next

            ' ---- 5) 可调用基因：copula + 逐基因放置求解 ----
            Dim genes As New List(Of Integer)

            For g As Integer = 0 To nGenes - 1
                If engine.Eligible(g) Then
                    Call genes.Add(g)
                End If
            Next

            Dim index As Integer() = genes.ToArray
            Dim J As Integer = index.Length

            If J > 0 Then
                Dim noise(J - 1)() As Double
                Dim totals(J - 1) As Double
                Dim targets(J - 1) As Double
                Dim controlCpm(J - 1) As Double
                Dim tolerance(J - 1) As Double
                Dim zt(J - 1) As Double

                For idx As Integer = 0 To J - 1
                    Dim gene As Integer = index(idx)

                    noise(idx) = New Double(n - 1) {}
                    totals(idx) = job.Totals(gene)
                    targets(idx) = job.Targets(gene)
                    controlCpm(idx) = engine.MeanCpm(gene)
                    zt(idx) = zTargets(gene)
                    tolerance(idx) = If(std.Abs(zt(idx)) >= options.PlannedZ, options.ZtolPlan, options.Ztol)
                Next

                ' 噪声按"逐基因"抽取，顺序固定（先基因后细胞）
                For k As Integer = 0 To J - 1
                    For c As Integer = 0 To n - 1
                        noise(k)(c) = generator.StandardNormal()
                    Next
                Next

                Call SynthesisEngineAccess.Bind(engine)

                Dim result As BlockPlaceResult

                Try
                    result = BlockPlacement.Solve(New BlockPlaceRequest With {
                        .Genes = index,
                        .Totals = totals,
                        .ZTargets = zt,
                        .Noise = noise,
                        .Libraries = libraries,
                        .Targets = targets,
                        .ControlCpm = controlCpm,
                        .ZTolerance = tolerance,
                        .Kernel = ShiftKernel.FromModel(model),
                        .PlannedZ = options.PlannedZ,
                        .OvershootZ = options.OvershootZ,
                        .RepairR = options.RepairR,
                        .RMax = options.RMax,
                        .Bisections = options.NBis,
                        .Ltol = options.Ltol
                    })
                Finally
                    Call SynthesisEngineAccess.Release()
                End Try

                For m As Integer = 0 To J - 1
                    Dim gene As Integer = index(m)

                    For c As Integer = 0 To n - 1
                        counts(c)(gene) = std.Max(result.Counts(m)(c), 0)
                    Next
                Next

                ' ---- 6) 降级基因用实际库大小重解 ----
                Call ResolveDemoted(engine, job, model, result, zTargets, index, noise, libraries, counts, generator)
            End If

            ' ---- 7) 输出 CSR ----
            Dim dense As Double()() = RectangularArray.Matrix(Of Double)(n, nGenes)

            For c As Integer = 0 To n - 1
                For g As Integer = 0 To nGenes - 1
                    dense(c)(g) = std.Max(std.Floor(counts(c)(g)), 0.0)
                Next
            Next

            Return CompressedBuilder.FromDense(dense)
        End Function

        ''' <summary>批量合成（按扰动并行；每个作业只写自己的槽位）。</summary>
        Public Function SynthesizeAll(engine As SynthesisEngine, jobs As SynthesisJob(),
                                      model As CellularAiModel,
                                      Optional degreeOfParallelism As Integer = 0) As CsrMatrix()
            If jobs Is Nothing Then
                Throw New ArgumentNullException(NameOf(jobs))
            End If

            Dim blocks(jobs.Length - 1) As CsrMatrix

            If jobs.Length = 0 Then
                Return blocks
            End If

            Dim work As Long = CLng(jobs.Length) * jobs(0).CellCount * std.Max(engine.GeneCount, 1)

            If degreeOfParallelism = 1 OrElse jobs.Length < 2 OrElse work < 2000000L Then
                For i As Integer = 0 To jobs.Length - 1
                    blocks(i) = Synthesize(engine, jobs(i), model)
                Next
            Else
                Dim options As New System.Threading.Tasks.ParallelOptions()

                options.MaxDegreeOfParallelism = If(degreeOfParallelism > 0, degreeOfParallelism, -1)

                Call System.Threading.Tasks.Parallel.For(0, jobs.Length, options,
                    Sub(i As Integer)
                        blocks(i) = Synthesize(engine, jobs(i), model)
                    End Sub)
            End If

            Return blocks
        End Function

#Region "helpers"

        ''' <summary>
        ''' 突发散布：把 <paramref name="total"/> 拆成 <c>ne_burst</c> 大小的块，
        ''' 每块落在按库大小概率随机抽到的细胞上。
        ''' </summary>
        Private Sub ScatterBursts(counts As Double()(), gene As Integer, total As Double,
                                  libraries As Double(), neBurst As Integer, generator As Pcg64)
            Dim n As Integer = libraries.Length
            Dim chunk As Integer = std.Max(neBurst, 1)
            Dim bursts As Integer = CInt(std.Ceiling(total / chunk))

            If bursts <= 0 Then
                Return
            End If

            Dim baseSize As Double = std.Floor(total / bursts)
            Dim remainder As Double = total - baseSize * bursts

            ' 累积分布（按库大小）
            Dim cumulative(n - 1) As Double
            Dim sum As Double = 0.0

            For i As Integer = 0 To n - 1
                sum += std.Max(libraries(i), 0.0)
                cumulative(i) = sum
            Next

            If sum <= 0.0 Then
                Return
            End If

            For b As Integer = 0 To bursts - 1
                Dim size As Double = baseSize + If(b < remainder, 1.0, 0.0)

                If size <= 0.0 Then
                    Continue For
                End If

                Dim target As Double = generator.NextDouble() * sum
                Dim cell As Integer = LowerBound(cumulative, target)

                counts(cell)(gene) += size
            Next
        End Sub

        Private Function LowerBound(values As Double(), target As Double) As Integer
            Dim lo As Integer = 0
            Dim hi As Integer = values.Length - 1

            While lo < hi
                Dim mid As Integer = (lo + hi) \ 2

                If values(mid) < target Then
                    lo = mid + 1
                Else
                    hi = mid
                End If
            End While

            Return lo
        End Function

        ''' <summary>
        ''' 降级基因（<c>zlim = -1</c> 且目标 z 被清零）用实际库大小重解一次。
        ''' </summary>
        Private Sub ResolveDemoted(engine As SynthesisEngine, job As SynthesisJob, model As CellularAiModel,
                                   result As BlockPlaceResult, zTargets As Double(), index As Integer(),
                                   noise As Double()(), libraries As Double(), counts As Double()(),
                                   generator As Pcg64)
            Dim n As Integer = libraries.Length
            Dim actual(n - 1) As Double

            For c As Integer = 0 To n - 1
                Dim sum As Double = 0.0

                For g As Integer = 0 To engine.GeneCount - 1
                    sum += counts(c)(g)
                Next

                actual(c) = std.Max(sum, 1.0)
            Next

            Dim ranks As Double() = Planning.CallSizing.RankOf(actual)
            Dim zl(n - 1) As Double

            For i As Integer = 0 To n - 1
                zl(i) = Microsoft.VisualBasic.Math.Statistics.NormalDistribution.InvCDF((ranks(i) + 0.5) / n)
            Next

            Dim kernel As ShiftKernel = ShiftKernel.FromModel(model)

            Call SynthesisEngineAccess.Bind(engine)

            Try
                For j As Integer = 0 To index.Length - 1
                    Dim gene As Integer = index(j)

                    If result.ZLimits(j) <> -1.0 Then
                        Continue For
                    End If
                    If result.ZTargets(j) <> 0.0 OrElse zTargets(gene) = 0.0 Then
                        Continue For
                    End If

                    Dim sorted As Double() = engine.SortedCpm(gene)
                    Dim tie As Double = engine.TieSum(gene)
                    Dim quantiles = QuantileBuilder.QuantilesFrom(noise(j), zl, 0.0)
                    Dim run = BlockSolver.SolveShift(quantiles.Q, quantiles.Ordering, sorted, tie,
                                                     actual, job.Totals(gene), 0.0, kernel)

                    For c As Integer = 0 To n - 1
                        counts(c)(gene) = std.Max(run.Counts(c), 0.0)
                    Next
                Next
            Finally
                Call SynthesisEngineAccess.Release()
            End Try
        End Sub

#End Region

    End Module
End Namespace
