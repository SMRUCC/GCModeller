' ============================================================================
' EmSearch.vb — 搜索编排：种子生成 → 逐种子 EM → 择优 → 多 motif 屏蔽重跑
' ----------------------------------------------------------------------------
' [em.md §5 初始化] 三种种子策略：
'   enriched（默认）: 统计全部 W-mer（双链时取规范形=正/反链字典序小者）出现
'                     次数，按 count/期望 比值降序取前 seedCount 个确定性种子
'   random          : 从数据中随机抽 seedCount 个窗口（--seed 可复现）
'   all             : 全部去重 W-mer（上限 maxSeeds，防组合爆炸）
' [em.md §5] 逐种子运行完整 EM，取对数似然最高者（同分取 E-value 小者）。
' [em.md §7 多 motif] 屏蔽已发现 motif 的位点（Z > 0.5 窗口字母置歧义）后重跑。
' [em.md §7 停止] 达到 nmotifs 或新 motif E-value > evalueMax。
' [em.md §9 宽度范围] minw..maxw 逐宽度评估，每个 motif 槽位取 LLR 最高宽度。
' ============================================================================

Namespace EmMotif.Core

    Public Class SearchOptions

        Public Model As SiteModel = SiteModel.Zoops
        Public MinW As Int32 = 8
        Public MaxW As Int32 = 8
        Public NumMotifs As Int32 = 1
        Public Revcomp As Boolean = False
        Public SeedStrategy As String = "enriched"     ' enriched | random | all
        Public SeedCount As Int32 = 20
        Public MaxSeeds As Int32 = 200
        Public Pseudocount As Double = 0.1
        Public MaxIter As Int32 = 200
        Public Epsilon As Double = 0.0001              ' ΔLL 收敛阈 [em.md §4]
        Public EvalueMax As Double = 10.0              ' [em.md §7] 停止阈值
        Public RngSeed As Int32 = 0

    End Class

    ''' <summary>单个 motif 的完整 EM 结果</summary>
    Public Class EmMotifResult

        Public Width As Int32
        Public Model As SiteModel
        Public Pwm As Double(,)
        Public Lambda As Double
        Public LogLikelihood As Double
        Public LogLikelihoodRatio As Double
        Public Evalue As Double
        Public Iterations As Int32
        Public Converged As Boolean
        Public Consensus As String
        Public LogLikTrace As List(Of Double)
        Public Sites As List(Of SitePosterior)     ' 按序列扁平存储
        Public SiteSeqIndex As List(Of Int32)      ' 位点所属序列下标
        Public TotalWindows As Double

    End Class

    Public Class EmSearch

        Private ReadOnly _encList As List(Of Int32())
        Private ReadOnly _alpha As Alphabet
        Private ReadOnly _opts As SearchOptions
        Private ReadOnly _rng As Random
        Private ReadOnly _masked As List(Of Int32())    ' 屏蔽后的编码（多 motif 用）

        Public Sub New(encList As List(Of Int32()), alpha As Alphabet, opts As SearchOptions)
            _encList = encList
            _alpha = alpha
            _opts = opts
            _rng = If(opts.RngSeed = 0, New Random(), New Random(opts.RngSeed))
            _masked = New List(Of Int32())()
            For Each enc In encList
                _masked.Add(CType(enc.Clone(), Int32()))
            Next
        End Sub

        ''' <summary>主入口：发现 numMotifs 个 motif（含宽度范围择优）</summary>
        Public Function Discover() As List(Of EmMotifResult)
            Dim results As New List(Of EmMotifResult)()
            For m = 1 To Math.Max(1, _opts.NumMotifs)
                Dim best As EmMotifResult = Nothing
                For w = _opts.MinW To _opts.MaxW
                    If Not WidthFeasible(w) Then Continue For
                    Dim r = DiscoverOneWidth(w)
                    If r Is Nothing Then Continue For
                    If best Is Nothing OrElse BetterThan(r, best) Then best = r
                Next
                If best Is Nothing Then Exit For
                ' [em.md §7 停止] E-value 超阈值
                If best.Evalue > _opts.EvalueMax Then Exit For
                results.Add(best)
                MaskSites(best)
            Next
            Return results
        End Function

        Private Function WidthFeasible(w As Int32) As Boolean
            For Each enc In _masked
                If enc.Length >= w Then Return True
            Next
            Return False
        End Function

        Private Shared Function BetterThan(a As EmMotifResult, b As EmMotifResult) As Boolean
            If Math.Abs(a.LogLikelihood - b.LogLikelihood) > 0.000000001 Then
                Return a.LogLikelihood > b.LogLikelihood
            End If
            Return a.Evalue < b.Evalue
        End Function

        ''' <summary>单宽度：种子生成 → 逐种子 EM → 最优</summary>
        Private Function DiscoverOneWidth(w As Int32) As EmMotifResult
            Dim seeds = GenerateSeeds(w)
            If seeds.Count = 0 Then Return Nothing
            Dim bg = ComputeBackground()
            Dim best As EmMotifResult = Nothing
            For Each seed In seeds
                Dim r = RunEm(w, seed, bg)
                If r Is Nothing Then Continue For
                If best Is Nothing OrElse BetterThan(r, best) Then best = r
            Next
            Return best
        End Function

        ''' <summary>order-0 背景频率（伪计数拉普拉斯平滑）</summary>
        Private Function ComputeBackground() As Double()
            Dim cnt(_alpha.Size - 1) As Double
            Dim total As Double = 0
            For Each enc In _encList
                For Each a In enc
                    If a >= 0 Then
                        cnt(a) += 1.0
                        total += 1.0
                    End If
                Next
            Next
            Dim pc = 0.1
            Dim freq(_alpha.Size - 1) As Double
            Dim denom = total + pc * _alpha.Size
            For a = 0 To _alpha.Size - 1
                freq(a) = (cnt(a) + pc) / denom
            Next
            Return freq
        End Function

        ''' <summary>[em.md §5] 种子生成（对屏蔽后的序列）</summary>
        Private Function GenerateSeeds(w As Int32) As List(Of Int32())
            Dim seeds As New List(Of Int32())()
            If _opts.SeedStrategy = "random" Then
                ' 随机窗口种子
                Dim pool As New List(Of Tuple(Of Int32, Int32))()
                For si = 0 To _masked.Count - 1
                    Dim nwin = _masked(si).Length - w + 1
                    For j = 0 To nwin - 1
                        pool.Add(Tuple.Create(si, j))
                    Next
                Next
                If pool.Count = 0 Then Return seeds
                Dim take = Math.Min(_opts.SeedCount, pool.Count)
                Dim used As New HashSet(Of Int32)()
                While used.Count < take AndAlso used.Count < pool.Count
                    Dim k = _rng.Next(pool.Count)
                    If used.Contains(k) Then Continue While
                    used.Add(k)
                    Dim si = pool(k).Item1
                    Dim j = pool(k).Item2
                    Dim enc = _masked(si)
                    Dim valid = True
                    For t = 0 To w - 1
                        If enc(j + t) < 0 Then
                            valid = False
                            Exit For
                        End If
                    Next
                    If valid Then
                        Dim seed(w - 1) As Int32
                        For t = 0 To w - 1
                            seed(t) = enc(j + t)
                        Next
                        seeds.Add(seed)
                    End If
                End While
                Return seeds
            End If

            ' enriched / all：W-mer 计数（双链取规范形）
            Dim counter As New Dictionary(Of String, Double())()
            Dim meta As New Dictionary(Of String, Int32())()
            For si = 0 To _masked.Count - 1
                Dim enc = _masked(si)
                Dim nwin = enc.Length - w + 1
                For j = 0 To nwin - 1
                    Dim ok = True
                    Dim fwd(w - 1) As Int32
                    Dim rcv(w - 1) As Int32
                    For t = 0 To w - 1
                        Dim a = enc(j + t)
                        If a < 0 Then
                            ok = False
                            Exit For
                        End If
                        fwd(t) = a
                        If _alpha.SupportsRevcomp AndAlso _opts.Revcomp Then
                            rcv(t) = _alpha.Complement(enc(j + w - 1 - t))
                        Else
                            rcv(t) = a
                        End If
                    Next
                    If Not ok Then Continue For
                    Dim keyF = KeyOf(fwd)
                    Dim keyR = KeyOf(rcv)
                    Dim useKey = If(_opts.Revcomp AndAlso _alpha.SupportsRevcomp AndAlso
                                    String.CompareOrdinal(keyR, keyF) < 0, keyR, keyF)
                    Dim useArr = If(useKey = keyF, fwd, rcv)
                    If Not counter.ContainsKey(useKey) Then
                        counter(useKey) = New Double(w - 1) {}
                        For t = 0 To w - 1
                            counter(useKey)(t) = 0
                        Next
                        meta(useKey) = useArr
                    End If
                    ' 列计数 +1
                    For t = 0 To w - 1
                        counter(useKey)(t) += 1
                    Next
                Next
            Next

            ' 打分：count（出现次数）；期望均匀时 count 即富集度。
            ' 为稳健用 count 直接排序（enriched 取前 seedCount；all 上限 maxSeeds）
            Dim ranked As New List(Of Tuple(Of Double, String))()
            For Each kv In counter
                Dim c As Double = 0
                For t = 0 To w - 1
                    c = Math.Max(c, kv.Value(t))
                Next
                ranked.Add(Tuple.Create(c, kv.Key))
            Next
            ranked.Sort(Function(a, b)
                            Dim c = b.Item1.CompareTo(a.Item1)
                            If c <> 0 Then Return c
                            Return String.CompareOrdinal(a.Item2, b.Item2)
                        End Function)

            Dim limit = If(_opts.SeedStrategy = "all", Math.Min(_opts.MaxSeeds, ranked.Count),
                           Math.Min(_opts.SeedCount, ranked.Count))
            For idx = 0 To limit - 1
                seeds.Add(meta(ranked(idx).Item2))
            Next
            Return seeds
        End Function

        Private Function KeyOf(arr As Int32()) As String
            Dim sb = New System.Text.StringBuilder()
            For Each a In arr
                sb.Append(a.ToString("X"c))
            Next
            Return sb.ToString()
        End Function

        ''' <summary>单种子完整 EM（E 步 → M 步 → 收敛）[em.md §2-4]</summary>
        Private Function RunEm(w As Int32, seed() As Int32, bg() As Double) As EmMotifResult
            Dim model As New EmModel(w, _alpha, _opts.Model, bg, _opts.Pseudocount)
            model.InitFromSeed(seed)
            Dim prev = model.Clone()

            Dim trace As New List(Of Double)()
            Dim sitesList As New List(Of List(Of SitePosterior))()
            For i = 0 To _masked.Count - 1
                sitesList.Add(New List(Of SitePosterior)())
            Next
            Dim ll = model.FullLogLik(_masked, sitesList)
            trace.Add(ll)
            Dim converged = False
            Dim iters = 0

            For it = 1 To _opts.MaxIter
                iters = it
                ' E 步 [em.md §2]
                For si = 0 To _masked.Count - 1
                    sitesList(si) = model.EStep(_masked(si), _opts.Revcomp)
                Next
                ' M 步 [em.md §3]
                Dim nextModel = model.Clone()
                nextModel.MStep(_masked, sitesList, _opts.Revcomp)
                model = nextModel
                ' 收敛判据 [em.md §4]：ΔLL < ε
                Dim newLl = model.FullLogLik(_masked, sitesList)
                trace.Add(newLl)
                Dim delta = Math.Abs(newLl - trace(trace.Count - 2))
                ll = newLl
                If delta < _opts.Epsilon Then
                    converged = True
                    Exit For
                End If
                Dim dPwm = model.MaxDeltaTo(prev)
                prev = model.Clone()
                If dPwm < 0.000000000001 Then
                    converged = True
                    Exit For
                End If
            Next

            ' 最终 E 步（保证 sites 与最终 PWM 一致）
            sitesList.Clear()
            For si = 0 To _masked.Count - 1
                sitesList.Add(model.EStep(_masked(si), _opts.Revcomp))
            Next

            ' LLR 与 E-value
            Dim llr = model.SoftLlr(sitesList)
            Dim df = CDbl(_alpha.Size - 1) * w
            Dim totalWindows As Double = 0
            Dim strands = If(_opts.Revcomp AndAlso _alpha.SupportsRevcomp, 2.0, 1.0)
            For Each enc In _masked
                Dim nw = enc.Length - w + 1
                If nw > 0 Then totalWindows += nw
            Next
            totalWindows *= strands
            Dim evalue = ChiSquare.MotifEValue(llr, df, totalWindows)

            ' 位点扁平化
            Dim flat As New List(Of SitePosterior)()
            Dim flatSeq As New List(Of Int32)()
            For si = 0 To sitesList.Count - 1
                For Each sp In sitesList(si)
                    If sp.Z > 0.000000001 Then
                        flat.Add(sp)
                        flatSeq.Add(si)
                    End If
                Next
            Next

            Dim result As New EmMotifResult With {
                .Width = w,
                .Model = _opts.Model,
                .Pwm = model.Pwm,
                .Lambda = model.Lambda,
                .LogLikelihood = ll,
                .LogLikelihoodRatio = llr,
                .Evalue = evalue,
                .Iterations = iters,
                .Converged = converged,
                .Consensus = model.Consensus(),
                .LogLikTrace = trace,
                .Sites = flat,
                .SiteSeqIndex = flatSeq,
                .TotalWindows = totalWindows}
            Return result
        End Function

        ''' <summary>[em.md §7] 屏蔽 motif 位点（Z &gt; 0.5 的窗口字母置 −1 歧义）</summary>
        Private Sub MaskSites(r As EmMotifResult)
            For idx = 0 To r.Sites.Count - 1
                If r.Sites(idx).Z <= 0.5 Then Continue For
                Dim si = r.SiteSeqIndex(idx)
                Dim enc = _masked(si)
                Dim j = r.Sites(idx).Pos
                For t = 0 To r.Width - 1
                    enc(j + t) = -1
                Next
            Next
        End Sub

    End Class

End Namespace
