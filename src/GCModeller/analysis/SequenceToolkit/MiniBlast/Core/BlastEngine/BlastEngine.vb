' ============================================================================
' BlastEngine.vb — 搜索引擎编排
' ----------------------------------------------------------------------------
' [README §一] seed-and-extend 五阶段总编排：
'   阶段1 过滤（DUST/SEG）→ 阶段2 建 word 查找表 → 阶段3 扫描数据库命中种子
'   → 阶段4/5 两-hit + 延伸（SeedExtend.vb）→ 统计换算与结果整形
'
' 数据库预处理一次（编码 + 掩码），供所有查询复用。
' ============================================================================

Imports MiniBlast.Model
Imports MiniBlast.Options
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace Core

    Public Class BlastEngine

        ''' <summary>
        ''' 触发 gapped 延伸的裕量：无 gap HSP 需达到的显著性为
        ''' EvalueCutoff 的该倍数（即 E_pass = EvalueCutoff × 本值）才做 gapped 延伸。
        ''' 无 gap 得分只是最终得分的下界，留裕量可避免过早剪掉真正的同源。
        ''' </summary>
        Public Const GappedTriggerMargin As Double = 0.001

        ''' <summary>单查询全流程</summary>
        Public Shared Function RunQuery(query As FastaSeq,
                                        db As List(Of DbEntry),
                                        dbStats As DbStatistics,
                                        opts As BlastOptions) As QueryResult
            Dim qLen = query.SequenceData.Length
            Dim qCodes As Int32()
            Dim qMask() As Boolean

            If opts.Program = "blastn" Then
                qCodes = NtAlphabet.Encode(query.SequenceData)
                qMask = If(opts.Dust,
                           Dust.Mask(qCodes, opts.DustLevel, 64),
                           New Boolean(qCodes.Length - 1) {})
            Else
                qCodes = AaAlphabet.Encode(query.SequenceData)
                qMask = If(opts.Seg,
                           SegFilter.Mask(qCodes, 12, 2.2, 2.5),
                           New Boolean(qCodes.Length - 1) {})
            End If

            ' ---- 阶段2：word 查找表 ----
            Dim lookup As IWordLookup
            Dim scorer As IScorer
            Dim ka As KaParams

            If opts.Program = "blastn" Then
                scorer = New NtScorer(opts.Reward, opts.Penalty)
                If opts.Task = "dc-megablast" Then
                    lookup = New DcWordLookup(qCodes, qMask,
                                              If(opts.Task = "dc-megablast", "101101100101101101", "111010010110010111"))
                Else
                    lookup = New NtWordLookup(qCodes, qMask, opts.WordSize)
                End If
                ka = KarlinAltschul.NtParams(opts.Reward, opts.Penalty)
            Else
                Dim aaScorer = New AaScorer(opts.Matrix)
                scorer = aaScorer
                lookup = New AaWordLookup(qCodes, qMask, opts.WordSize, aaScorer, opts.Threshold)
                ka = KarlinAltschul.ProteinParams(opts.Matrix)
            End If

            ' 触发 gapped 延伸的 raw 分阈值（无 gap HSP 得分低于此值不做 gapped）：
            '   E = K·m·n·e^(-λS) ≤ E_pass  ⇔  S ≥ ln(K·m·n / E_pass) / λ
            ' E_pass 取 evalue 截止的 GappedTriggerMargin 倍留裕量（无 gap 得分只是下界，
            ' gapped 延伸后通常更高）。
            ' 注意：原式写成 ln(K·m·n * E_pass)/λ，把除写成了乘，门槛约为应有值的一半。
            Dim ePass = Math.Max(opts.EvalueCutoff * GappedTriggerMargin, 1.0E-300)
            Dim sMin As Double = Math.Max(5.0,
                Math.Log(ka.K * CDbl(qLen) * CDbl(dbStats.Residues) / ePass) / ka.Lambda)

            ' ---- 阶段3/4/5：扫描 + 延伸 ----
            Dim seOpts As New SeedExtendOptions With {
                .WordSize = opts.WordSize,
                .WindowTwoHit = opts.WindowTwoHit,
                .UseTwoHit = opts.UseTwoHit,
                .XdropUngapBits = opts.XdropUngap,
                .XdropGapBits = opts.XdropGap,
                .XdropGapFinalBits = opts.XdropGapFinal,
                .GapOpen = opts.GapOpen,
                .GapExtend = opts.GapExtend
            }
            Dim scanner = New SeedScanner(scorer, ka.Lambda, seOpts, opts.Program = "blastn")
            Dim perHit As New Dictionary(Of String, List(Of RawHsp))()

            For Each entry In db
                Dim hsps = scanner.ScanSequence(lookup, entry.Codes, entry.Mask, qCodes, sMin)
                If hsps.Count > 0 Then
                    perHit(entry.Id) = hsps
                End If
            Next

            ' ---- 统计换算 + 整形 ----
            Return BuildResult(query, db, perHit, opts, ka, scorer, qCodes, qLen, dbStats)
        End Function

        Private Shared Function BuildResult(query As FastaSeq,
                                            db As List(Of DbEntry),
                                            perHit As Dictionary(Of String, List(Of RawHsp)),
                                            opts As BlastOptions,
                                            ka As KaParams,
                                            scorer As IScorer,
                                            qCodes As Int32(),
                                            qLen As Integer,
                                            dbStats As DbStatistics) As QueryResult
            Dim qr As New QueryResult With {
                .Id = query.locus_tag,
                .Description = query.Title,
                .Length = qLen
            }

            ' db id → entry 映射
            Dim entryMap As New Dictionary(Of String, DbEntry)()
            For Each e In db
                entryMap(e.Id) = e
            Next

            Dim hitList As New List(Of Hit)()
            For Each kvp In perHit
                Dim entry = entryMap(kvp.Key)
                Dim hsps = kvp.Value

                ' 去重：按坐标去重 + 高分包含低分剔除
                Dim deduped = DedupeHsps(hsps, opts.MaxHsps)

                Dim hspList As New List(Of Hsp)()
                For Each raw In deduped
                    ' [comp_based_stats=1] 简化组成校正（README §4.4）：
                    ' 以查询×命中残基组成重估 λ（模式 2/3 的条件矩阵校正未实现，
                    ' 回落为模式 1；未校正时保持基准 λ）
                    Dim eff As KaParams = ka
                    If opts.Program = "blastp" AndAlso opts.CompBasedStats >= 1 Then
                        Dim aaScorer = DirectCast(scorer, AaScorer)
                        eff = KarlinAltschul.AdjustedParams(aaScorer, qCodes, entry.Codes, ka)
                    End If

                    ' bit_score 与 evalue 必须使用同一套 (λ, K)，否则 [式5-2] 与 [式5-1]
                    ' 不再互为恒等，下游按 bit_score 反推 E 值会得到错误结果。
                    Dim h = New Hsp With {
                        .Score = raw.RawScore,
                        .BitScore = Math.Round(eff.BitScore(raw.RawScore), 3),
                        .Evalue = eff.EValue(CDbl(qLen), CDbl(entry.Length), raw.RawScore),
                        .Identities = raw.Identities,
                        .Positives = raw.Positives,
                        .Gaps = raw.Gaps,
                        .QueryFrom = raw.QueryFrom + 1,
                        .QueryTo = raw.QueryTo + 1,
                        .SubjectFrom = raw.SubjectFrom + 1,
                        .SubjectTo = raw.SubjectTo + 1,
                        .QueryFrame = If(opts.Program = "blastn", 1, 0),
                        .QuerySeq = raw.QueryAlign,
                        .Midline = raw.Midline,
                        .SubjectSeq = raw.SubjectAlign
                    }
                    If h.Evalue <= opts.EvalueCutoff Then
                        hspList.Add(h)
                    End If
                Next

                If hspList.Count > 0 Then
                    hspList.Sort(Function(a, b) a.Evalue.CompareTo(b.Evalue))
                    hitList.Add(New Hit With {
                        .Id = entry.Id,
                        .Description = entry.Description,
                        .Length = entry.Length,
                        .Hsps = hspList
                    })
                End If
            Next

            hitList.Sort(Function(a, b) a.Hsps(0).Evalue.CompareTo(b.Hsps(0).Evalue))
            If hitList.Count > opts.MaxTargetSeqs Then
                hitList = hitList.Take(opts.MaxTargetSeqs).ToList()
            End If
            qr.Hits = hitList
            Return qr
        End Function

        ''' <summary>HSP 去重：同坐标剔除 + 已保留高分矩形完全包含的低分剔除</summary>
        Private Shared Function DedupeHsps(hsps As List(Of RawHsp), maxHsps As Integer) As List(Of RawHsp)
            Dim kept As New List(Of RawHsp)()
            Dim seen As New HashSet(Of String)()

            Call hsps.Sort(Function(a, b) b.RawScore.CompareTo(a.RawScore))

            For Each h In hsps
                Dim coordKey = $"{h.QueryFrom}:{h.QueryTo}:{h.SubjectFrom}:{h.SubjectTo}"
                If seen.Contains(coordKey) Then Continue For
                Dim contained = False
                For Each k In kept
                    If h.QueryFrom >= k.QueryFrom AndAlso h.QueryTo <= k.QueryTo AndAlso
                       h.SubjectFrom >= k.SubjectFrom AndAlso h.SubjectTo <= k.SubjectTo Then
                        contained = True
                        Exit For
                    End If
                Next
                If Not contained Then
                    kept.Add(h)
                    seen.Add(coordKey)
                End If
                If kept.Count >= maxHsps Then Exit For
            Next
            Return kept
        End Function

    End Class

End Namespace
