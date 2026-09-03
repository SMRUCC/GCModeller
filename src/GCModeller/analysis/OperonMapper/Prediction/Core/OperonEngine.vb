' ============================================================================
' OperonEngine.vb — 主引擎：信号计算 → HMM 整合 → 操纵子装配
' ----------------------------------------------------------------------------
' 流程 [operon.md]：
'   1. 相邻对枚举 + 链向分类（§1.3 步骤1）
'   2. UniOP 距离模型（§1.4）：先验 q + 闭式后验
'   3. 序列信号（终止子/启动子，需 FASTA）
'   4. 比较基因组信号（条形码/保守对/PCBBH，需同源映射+参考注释）
'   5. 功能注释匹配
'   6. HMM 整合（Viterbi + 前向后向）
'   7. 操纵子装配：Viterbi 状态 = 同操纵子的相邻对合并；反链对强制断开
' ============================================================================

Namespace OperonPredictor.Core

    Public Class EngineOptions

        Public Integration As New IntegrationOptions()
        Public UseSequenceSignals As Boolean = False      ' 有 FASTA 才开
        Public UseComparative As Boolean = False          ' 有同源映射才开
        Public UseFunction As Boolean = False

    End Class

    Public Class Engine

        ''' <summary>全流程预测；返回（每对信号, Viterbi 决策）与操纵子列表</summary>
        Public Shared Function Predict(genes As List(Of Gene),
                                       fasta As Dictionary(Of String, String),
                                       homology As HomologySignals,
                                       functions As Dictionary(Of String, String),
                                       opts As EngineOptions) As Tuple(Of List(Of AdjacentPair), List(Of PairSignals), Double)

            Dim pairs = GeneModel.EnumeratePairs(genes)
            If pairs.Count = 0 Then Return Tuple.Create(pairs, New List(Of PairSignals)(), 0.5)

            ' ---- UniOP 统计 [§1.4] ----
            Dim mCount = pairs.Where(Function(p) p.IsSameStrand).Count
            Dim oCount = pairs.Count - mCount
            Dim qPrior = UniopModel.ComputePrior(mCount, oCount)
            Dim sameD = pairs.Where(Function(p) p.IsSameStrand).Select(Function(p) CDbl(p.Igd)).ToList()
            Dim convD = pairs.Where(Function(p) p.Relation = StrandRelation.Convergent).
                             Select(Function(p) CDbl(p.Igd)).ToList()
            Dim divD = pairs.Where(Function(p) p.Relation = StrandRelation.Divergent).
                            Select(Function(p) CDbl(p.Igd)).ToList()
            Dim uniop As New UniopModel(sameD, convD, divD, mCount, oCount)

            ' ---- 每对信号 ----
            Dim signals As New List(Of PairSignals)()
            For Each pr In pairs
                Dim s As New PairSignals()
                If pr.IsSameStrand Then
                    ' 距离 [§1.1/§1.4]
                    s.UniopPosterior = uniop.Posterior(pr.Igd)
                    ' 终止子/启动子 [特征5]
                    If opts.UseSequenceSignals Then
                        Dim fmTerm As Boolean = False
                        Dim fmProm As Boolean = False
                        Dim seq = SignalScan.IntergenicSequence(fasta, pr.A, pr.B, fmTerm, fmProm)
                        If seq IsNot Nothing Then
                            s.TerminatorStrength = SignalScan.ScanTerminator(seq, fmTerm)
                            s.PromoterStrength = SignalScan.ScanPromoter(seq, fmProm)
                        End If
                    End If
                    ' 比较基因组 [§2]
                    If opts.UseComparative AndAlso homology IsNot Nothing AndAlso homology.TotalRefs() > 0 Then
                        Dim h As Int32 = 0
                        Dim ru As Int32 = 0
                        homology.BarcodeStats(pr.A.Id, pr.B.Id, h, ru)
                        s.BarcodeHamming = h
                        s.BarcodeRefs = ru
                        s.BarcodeLlr = HomologySignals.BarcodeLlr(h, ru,
                            opts.Integration.PBarcodeIn, opts.Integration.PBarcodeOut)
                        s.ConservedCount = homology.ConservedPairCount(pr.A.Id, pr.B.Id)
                        s.ConservedLlr = HomologySignals.ConservedLlr(s.ConservedCount,
                            homology.TotalRefs(), opts.Integration.PConservedIn, opts.Integration.PConservedOut)
                        s.PcbbhCount = homology.PcbbhCount(pr.A.Id, pr.B.Id)
                    End If
                    ' 功能 [特征4]
                    If opts.UseFunction AndAlso functions IsNot Nothing Then
                        Dim fa As String = Nothing
                        Dim fb As String = Nothing
                        functions.TryGetValue(pr.A.Id, fa)
                        functions.TryGetValue(pr.B.Id, fb)
                        If fa IsNot Nothing AndAlso fb IsNot Nothing AndAlso fa.Length > 0 AndAlso fb.Length > 0 Then
                            s.FunctionalMatch = (fa = fb)
                        End If
                    End If
                    Integrator.ScorePair(s, qPrior, opts.Integration)
                Else
                    ' 反链对：硬边界 [operon.md 特征2]
                    s.UniopPosterior = 0.0
                    s.CombinedPosterior = 0.0
                    s.HmmPosterior = 0.0
                    s.ViterbiState = False
                End If
                signals.Add(s)
            Next

            ' ---- HMM 整合（同链对按 run 分段）[§2.4] ----
            Dim runs As New List(Of List(Of PairSignals))()
            Dim cur As New List(Of PairSignals)()
            For i = 0 To pairs.Count - 1
                If pairs(i).IsSameStrand Then
                    cur.Add(signals(i))
                Else
                    If cur.Count > 0 Then runs.Add(cur)
                    cur = New List(Of PairSignals)()
                End If
            Next
            If cur.Count > 0 Then runs.Add(cur)
            Integrator.RunHmm(runs, qPrior, opts.Integration)

            Return Tuple.Create(pairs, signals, qPrior)
        End Function

        ''' <summary>Viterbi 路径 → 操纵子列表（跨反链对强制断开）</summary>
        Public Shared Function AssembleOperons(pairs As List(Of AdjacentPair),
                                               signals As List(Of PairSignals)) As List(Of OperonInfo)
            Dim operons As New List(Of OperonInfo)()
            If pairs.Count = 0 Then Return operons
            Dim current As New List(Of Gene) From {pairs(0).A}
            Dim currentPairPosts As New List(Of Double)()
            Dim curContig = pairs(0).A.Contig
            Dim curStrand = pairs(0).A.Strand

            For i = 0 To pairs.Count - 1
                Dim pr = pairs(i)
                Dim sg = signals(i)
                Dim merge = pr.IsSameStrand AndAlso sg.ViterbiState AndAlso
                            pr.A.Contig = curContig AndAlso pr.A.Strand = curStrand
                If merge Then
                    If current.Count = 0 Then current.Add(pr.A)
                    current.Add(pr.B)
                    currentPairPosts.Add(sg.HmmPosterior)
                Else
                    operons.Add(MakeOperon(operons.Count + 1, current, currentPairPosts))
                    current = New List(Of Gene) From {pr.A, pr.B}
                    currentPairPosts = New List(Of Double)()
                    If pr.IsSameStrand Then currentPairPosts.Add(sg.HmmPosterior)
                    curContig = pr.B.Contig
                    curStrand = pr.B.Strand
                End If
            Next
            If current.Count > 0 Then operons.Add(MakeOperon(operons.Count + 1, current, currentPairPosts))
            Return operons
        End Function

        Private Shared Function MakeOperon(id As Int32, geneList As List(Of Gene),
                                           pairPosts As List(Of Double)) As OperonInfo
            Dim info As New OperonInfo With {
                .OperonId = $"op_{id}",
                .Contig = geneList(0).Contig,
                .Strand = geneList(0).Strand.ToString(),
                .Start = geneList.Min(Function(g) g.StartMin),
                .[End] = geneList.Max(Function(g) g.EndMax),
                .NumGenes = geneList.Count}
            info.Genes = geneList.Select(Function(g) g.Id).ToList()
            info.GeneStarts = geneList.Select(Function(g) g.StartMin).ToList()
            info.GeneEnds = geneList.Select(Function(g) g.EndMax).ToList()
            If pairPosts.Count > 0 Then
                info.MeanPairPosterior = pairPosts.Average()
            Else
                info.MeanPairPosterior = 1.0     ' 单基因操纵子
            End If
            Return info
        End Function

    End Class

    ''' <summary>操纵子（结果对象）</summary>
    Public Class OperonInfo

        Public OperonId As String
        Public Contig As String
        Public Strand As String
        Public Start As Int32
        Public [End] As Int32
        Public NumGenes As Int32
        Public Genes As List(Of String)
        Public GeneStarts As List(Of Int32)
        Public GeneEnds As List(Of Int32)
        Public MeanPairPosterior As Double

    End Class

End Namespace
