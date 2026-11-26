Imports CellularAI.Model
Imports std = System.Math

Namespace Synthesis

    ''' <summary>一次"逐基因放置求解"的输入打包。</summary>
    Public Class BlockPlaceRequest

        ''' <summary>参与求解的基因下标（<c>eligible</c>）。</summary>
        Public Property Genes As Integer()

        ''' <summary>逐基因的目标总量（与 <see cref="Genes"/> 同长）。</summary>
        Public Property Totals As Double()

        ''' <summary>逐基因的目标 z（与 <see cref="Genes"/> 同长；可能被降级改写）。</summary>
        Public Property ZTargets As Double()

        ''' <summary>逐基因的独立噪声（基因数 × 细胞数）。</summary>
        Public Property Noise As Double()()

        ''' <summary>每个细胞的文库大小（长度 n）。</summary>
        Public Property Libraries As Double()

        ''' <summary>逐基因的目标平均 CPM（与 <see cref="Genes"/> 同长）。</summary>
        Public Property Targets As Double()

        ''' <summary>逐基因的对照平均 CPM（用于"反向离群"判定）。</summary>
        Public Property ControlCpm As Double()

        ''' <summary>逐基因的 z 容差。</summary>
        Public Property ZTolerance As Double()

        ''' <summary>平移二分参数。</summary>
        Public Property Kernel As ShiftKernel

        ''' <summary>计划显著的判定阈值（<c>planned_z</c>）。</summary>
        Public Property PlannedZ As Double = 5.0

        ''' <summary>过冲判定阈值（<c>overshoot_z</c>）。</summary>
        Public Property OvershootZ As Double = 1.0

        ''' <summary>过冲修复的候选相关系数（<c>repair_r</c>）。</summary>
        Public Property RepairR As Double() = {0.25, 0.5, 0.99}

        ''' <summary>相关系数搜索的上限。</summary>
        Public Property RMax As Double = 0.99

        ''' <summary>外层 r 二分的步数（<c>n_bis</c>）。</summary>
        Public Property Bisections As Integer = 5

        ''' <summary>平均 CPM 的相对容差（<c>ltol</c>）。</summary>
        Public Property Ltol As Double = 0.01

    End Class

    ''' <summary>逐基因放置求解的结果。</summary>
    Public Class BlockPlaceResult

        ''' <summary>整数计数（基因数 × 细胞数），行序与 <see cref="BlockPlaceRequest.Genes"/> 一致。</summary>
        Public Property Counts As Integer()()

        ''' <summary>每个基因最终采用的相关系数。</summary>
        Public Property Correlations As Double()

        ''' <summary>每个基因的 z 容差（<c>-1</c> 表示被降级为 rank-neutral）。</summary>
        Public Property ZLimits As Double()

        ''' <summary>降级后的目标 z（与输入同长）。</summary>
        Public Property ZTargets As Double()

    End Class

    ''' <summary>
    ''' 逐基因的"放置"求解：先把 z 打到目标，再把平均表达量拉到放置目标。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>三段式控制流</b>（与上游 <c>_solve_block_place</c> 同构）：
    ''' </para>
    ''' <list type="number">
    ''' <item><description><b>r = 0</b>：先解一次，得到 z 与平均 CPM。这是"只按显著性排序"的基线。</description></item>
    ''' <item><description><b>过冲修复</b>：若该基因<b>不该显著</b>（<c>|zt| &lt; planned_z</c>）但解出来的
    ''' |z| 超出目标 <c>overshoot_z</c> 以上，说明"显著性排名"把它推得太高；此时扫 7 个候选相关系数
    ''' （含正负与 0），挑"z 在带内且 |log2(mcpm/mt)| 最小"的那个。</description></item>
    ''' <item><description><b>r 搜索</b>：否则若平均 CPM 与目标偏离超过 <c>ltol</c>，在
    ''' <c>[0, ±rmax]</c> 上二分 <c>r</c>，同时把 z 锁在 r=0 的精度 + <c>ztol</c> 之内
    ''' ——即"调形状但不许把显著性搞坏"。</description></item>
    ''' </list>
    ''' <para>
    ''' <b>降级规则</b>：非计划基因若解出来的平均表达落在"对照均值的另一侧"
    ''' （与微调方向相反），说明排序与方向冲突，把它降级为 rank-neutral（<c>zt = 0</c>、<c>zlim = -1</c>），
    ''' 由调用方用实际文库大小重解一次。静默保留会让整个扰动的方向谱出现反号基因。
    ''' </para>
    ''' </remarks>
    Public Module BlockPlacement

        ''' <summary>逐基因求解。</summary>
        Public Function Solve(request As BlockPlaceRequest) As BlockPlaceResult
            If request Is Nothing Then
                Throw New ArgumentNullException(NameOf(request))
            End If

            Dim genes As Integer() = request.Genes
            Dim nGenes As Integer = genes.Length
            Dim n As Integer = request.Libraries.Length

            Dim counts(nGenes - 1)() As Integer
            Dim correlations(nGenes - 1) As Double
            Dim limits(nGenes - 1) As Double
            Dim targets As Double() = DirectCast(request.ZTargets.Clone(), Double())
            Dim kernel As ShiftKernel = request.Kernel
            Dim tolerance As Double = kernel.Tolerance

            ' 文库大小的正态得分（名次 → Φ⁻¹((rank+0.5)/n)）
            Dim ranks As Double() = Planning.CallSizing.RankOf(request.Libraries)
            Dim zl(n - 1) As Double

            For i As Integer = 0 To n - 1
                zl(i) = Microsoft.VisualBasic.Math.Statistics.NormalDistribution.InvCDF((ranks(i) + 0.5) / n)
            Next

            For j As Integer = 0 To nGenes - 1
                Dim gene As Integer = genes(j)
                Dim key As String = Nothing      ' 占位：保持与上游一致的"逐基因独立"语义
                Dim sorted As Double() = SynthesisEngineAccess.SortedCpm(gene)
                Dim tie As Double = SynthesisEngineAccess.TieSum(gene)
                Dim total As Double = request.Totals(j)
                Dim target As Double = targets(j)
                Dim toleranceJ As Double = request.ZTolerance(j)

                ' ---- r = 0 ----
                Dim zero As (Q As Double(), Ordering As Integer()) =
                    QuantileBuilder.QuantilesFrom(request.Noise(j), zl, 0.0)
                Dim baseRun As (Counts As Double(), Z As Double) =
                    BlockSolver.SolveShift(zero.Q, zero.Ordering, sorted, tie,
                                           request.Libraries, total, target, kernel)
                Dim best As Double() = baseRun.Counts
                Dim bestZ As Double = baseRun.Z
                Dim bestCpm As Double = QuantileBuilder.MeanCpm(best, request.Libraries)
                Dim bestR As Double = 0.0
                Dim limit As Double = std.Max(std.Abs(baseRun.Z - target), tolerance) + toleranceJ
                Dim repaired As Boolean = std.Abs(target) < request.PlannedZ AndAlso
                                          (std.Abs(baseRun.Z) - std.Abs(target)) > request.OvershootZ

                Dim cpmTarget As Double = request.Targets(j)

                If repaired Then
                    Dim bestError As Double = std.Abs(baseRun.Z - target)
                    Dim bestGap As Double = Double.MaxValue
                    Dim inBand As Boolean = False
                    Dim ra As Double = request.RepairR(0)
                    Dim rb As Double = request.RepairR(1)
                    Dim rc As Double = request.RMax
                    Dim candidates As Double() = New Double() {ra, rb, rc, -ra, -rb, -rc, 0.0}

                    For ig As Integer = 0 To candidates.Length - 1
                        Dim attempt As (Counts As Double(), Z As Double, Cpm As Double) =
                            Probe(request, j, sorted, tie, zl, candidates(ig), total, target, kernel)
                        Dim gap As Double = TargetGap(attempt.Cpm, cpmTarget)
                        Dim error_ As Double = std.Abs(attempt.Z - target)

                        If error_ <= tolerance + toleranceJ Then
                            If (Not inBand) OrElse gap < bestGap Then
                                inBand = True
                                bestGap = gap
                                bestError = error_
                                bestCpm = attempt.Cpm
                                bestR = candidates(ig)
                                best = attempt.Counts
                            End If
                        ElseIf (Not inBand) AndAlso error_ < bestError Then
                            bestError = error_
                            bestGap = gap
                            bestCpm = attempt.Cpm
                            bestR = candidates(ig)
                            best = attempt.Counts
                        End If
                    Next

                    limit = std.Max(bestError, tolerance) + toleranceJ
                ElseIf cpmTarget > 0.0 AndAlso bestCpm > 0.0 Then
                    Dim deviation As Double = std.Log(bestCpm / cpmTarget, 2.0)
                    Dim bestDeviation As Double = std.Abs(deviation)

                    If bestDeviation >= request.Ltol Then
                        Dim direction As Double = If(deviation > 0.0, 1.0, -1.0)
                        Dim low As Double = 0.0
                        Dim high As Double = direction * request.RMax
                        Dim done As Boolean = False

                        ' 先探上界：若已经达标就直接收敛
                        Dim edge As (Counts As Double(), Z As Double, Cpm As Double) =
                            Probe(request, j, sorted, tie, zl, high, total, target, kernel)

                        If edge.Cpm > 0.0 AndAlso std.Abs(edge.Z - target) <= limit Then
                            Dim gapEdge As Double = std.Log(edge.Cpm / cpmTarget, 2.0)

                            If std.Abs(gapEdge) < bestDeviation Then
                                bestDeviation = std.Abs(gapEdge)
                                bestCpm = edge.Cpm
                                bestR = high
                                best = edge.Counts
                            End If
                            If gapEdge * direction > 0.0 Then
                                done = True
                            End If
                        End If

                        For iteration As Integer = 1 To std.Max(request.Bisections, 1)
                            If done OrElse bestDeviation < request.Ltol Then
                                Exit For
                            End If

                            Dim mid As Double = 0.5 * (low + high)
                            Dim attempt As (Counts As Double(), Z As Double, Cpm As Double) =
                                Probe(request, j, sorted, tie, zl, mid, total, target, kernel)
                            Dim okZ As Boolean = attempt.Cpm > 0.0 AndAlso std.Abs(attempt.Z - target) <= limit

                            If Not okZ Then
                                high = mid
                                Continue For
                            End If

                            Dim gapMid As Double = std.Log(attempt.Cpm / cpmTarget, 2.0)

                            If std.Abs(gapMid) < bestDeviation Then
                                bestDeviation = std.Abs(gapMid)
                                bestCpm = attempt.Cpm
                                bestR = mid
                                best = attempt.Counts
                            End If

                            If gapMid * direction > 0.0 Then
                                low = mid
                            Else
                                high = mid
                            End If
                        Next
                    End If
                End If

                counts(j) = Array.ConvertAll(best, Function(v) CInt(v))
                correlations(j) = bestR
                limits(j) = limit

                ' 反向离群 → 降级为 rank-neutral
                If target <> 0.0 AndAlso std.Abs(target) < request.PlannedZ AndAlso
                    cpmTarget > 0.0 AndAlso bestCpm > 0.0 AndAlso
                    (bestCpm - request.ControlCpm(j)) * target < 0.0 Then

                    targets(j) = 0.0
                    limits(j) = -1.0
                End If
            Next

            Return New BlockPlaceResult With {
                .Counts = counts,
                .Correlations = correlations,
                .ZLimits = limits,
                .ZTargets = targets
            }
        End Function

        ''' <summary>在给定相关系数下解一次，返回计数、z 与平均 CPM。</summary>
        Private Function Probe(request As BlockPlaceRequest, j As Integer, sorted As Double(), tie As Double,
                               zl As Double(), correlation As Double, total As Double,
                               target As Double, kernel As ShiftKernel) As (Counts As Double(), Z As Double, Cpm As Double)
            Dim quantiles As (Q As Double(), Ordering As Integer()) =
                QuantileBuilder.QuantilesFrom(request.Noise(j), zl, correlation)
            Dim run As (Counts As Double(), Z As Double) =
                BlockSolver.SolveShift(quantiles.Q, quantiles.Ordering, sorted, tie,
                                       request.Libraries, total, target, kernel)

            Return (run.Counts, run.Z, QuantileBuilder.MeanCpm(run.Counts, request.Libraries))
        End Function

        ''' <summary><c>|log2(mcpm / target)|</c>；任一非正时返回 0（与上游一致）。</summary>
        Private Function TargetGap(cpm As Double, target As Double) As Double
            If target > 0.0 AndAlso cpm > 0.0 Then
                Return std.Abs(std.Log(cpm / target, 2.0))
            End If

            Return 0.0
        End Function

    End Module

    ''' <summary>
    ''' 供放置求解访问"控制侧序统计"的桥：由 <see cref="CellSynthesizer"/> 在调用前绑定引擎。
    ''' </summary>
    ''' <remarks>
    ''' 之所以不把引擎作为参数层层传递，是因为放置求解是<b>纯数值</b>的（只读序统计表），
    ''' 而 VB 的元组/ByRef 组合会让签名变得难以阅读。绑定是单线程的、作用域仅限一次
    ''' <c>Synthesize</c> 调用，因此不会引入跨线程共享。
    ''' </remarks>
    Friend Module SynthesisEngineAccess

        Private m_engine As SynthesisEngine

        ''' <summary>绑定当前引擎（每次合成调用开始时设置）。</summary>
        Public Sub Bind(engine As SynthesisEngine)
            m_engine = engine
        End Sub

        ''' <summary>解除绑定。</summary>
        Public Sub Release()
            m_engine = Nothing
        End Sub

        Public Function SortedCpm(gene As Integer) As Double()
            Return m_engine.SortedCpm(gene)
        End Function

        Public Function TieSum(gene As Integer) As Double
            Return m_engine.TieSum(gene)
        End Function

    End Module
End Namespace
