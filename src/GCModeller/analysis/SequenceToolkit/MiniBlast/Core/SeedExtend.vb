' ============================================================================
' SeedExtend.vb — 种子扫描与延伸（BLAST 核心引擎）
' ----------------------------------------------------------------------------
' [README §一] seed-and-extend 流程的第三至五阶段：
'
' 1. [§一.3 两-hit 法] 扫描数据库序列，word 命中按对角线 diag = i - j 分组；
'    同一对角线上两个非重叠命中相距 ≤ A（默认 40）才触发无 gap 延伸。
'    每条对角线维护"已延伸区"避免重复触发。
'
' 2. [§一.4 无 gap 延伸] 以触发区中点为种子沿对角线双向行走，
'    各侧得分相对该侧最优下降超过 X_ungap（bits，按 ΔS_raw = bits·ln2/λ
'    换算为 raw 分）即停止；最优段必含种子，双侧合并。
'
' 3. [§一.5 有 gap 延伸] 以无 gap 最优段中点为种子做两遍前向 X-drop
'    仿射间隙 DP（正向 + 反向前缀），每格记录 H/E/F 三状态回溯方向，
'    得分 H = max(对角+E, F)，E/F 为间隙状态，E = max(H-up-gapOpen,
'    E-up-gapExtend)。合并双向结果，状态机回溯生成比对字符串。
'
' 得分系统为 Double（megablast 动态 gap 代价 |2p-r|/2 可能为 x.5）。
' ============================================================================

Imports System.Text

Namespace Core

    ''' <summary>延伸阶段产出的原始 HSP（未做统计换算）</summary>
    Public Class RawHsp

        Public Property QueryFrom As Integer      ' 0-based inclusive
        Public Property QueryTo As Integer
        Public Property SubjectFrom As Integer
        Public Property SubjectTo As Integer
        Public Property RawScore As Double
        Public Property QueryAlign As String
        Public Property SubjectAlign As String
        Public Property Midline As String
        Public Property Identities As Integer
        Public Property Positives As Integer
        Public Property Gaps As Integer

    End Class

    ''' <summary>扫描/延伸选项</summary>
    Public Class SeedExtendOptions

        Public Property WordSize As Integer = 11
        Public Property WindowTwoHit As Integer = 40       ' A
        Public Property UseTwoHit As Boolean = True
        Public Property XdropUngapBits As Double = 20      ' blastn 默认
        Public Property XdropGapBits As Double = 30
        Public Property XdropGapFinalBits As Double = 100
        Public Property GapOpen As Double = 5
        Public Property GapExtend As Double = 2
        Public Property MaxCellsPerExtension As Long = 4000000

    End Class

    Public Class SeedScanner

        Private ReadOnly _scorer As IScorer
        Private ReadOnly _lambda As Double
        Private ReadOnly _opts As SeedExtendOptions
        Private ReadOnly _isNt As Boolean

        Private Const Ln2 As Double = 0.69314718055994529

        Public Sub New(scorer As IScorer, lambda As Double, opts As SeedExtendOptions, isNt As Boolean)
            _scorer = scorer
            _lambda = lambda
            _opts = opts
            _isNt = isNt
        End Sub

        Private Function DecodeCode(code As Int32) As Char
            If _isNt Then Return NtAlphabet.Decode(code)
            Return AaAlphabet.Decode(code)
        End Function

        Private Function BitsToRaw(bits As Double) As Double
            ' Δbits = λ·ΔS/ln2  ⇒  ΔS_raw = bits·ln2/λ
            Return bits * Ln2 / _lambda
        End Function

        ''' <summary>
        ''' 对单条数据库序列扫描全部两-hit 触发点并做两级延伸。
        ''' </summary>
        ''' <param name="lookup">查询 word 查找表（Nt/Dc/Aa 均实现 IWordLookup）</param>
        ''' <param name="dbCodes">数据库序列编码</param>
        ''' <param name="dbMask">数据库序列掩码</param>
        ''' <param name="queryCodes">查询编码（供延伸打分）</param>
        ''' <param name="sMinRaw">触发 gapped 延伸的无 gap 得分下限</param>
        Public Function ScanSequence(lookup As IWordLookup,
                                     dbCodes As Int32(), dbMask() As Boolean,
                                     queryCodes As Int32(),
                                     sMinRaw As Double) As List(Of RawHsp)
            Dim results As New List(Of RawHsp)()
            Dim n = queryCodes.Length
            Dim m = dbCodes.Length
            If n < lookup.Span OrElse m < lookup.Span Then Return results

            Dim ws = lookup.WordSize
            Dim span = lookup.Span
            Dim lastHit As New Dictionary(Of Integer, Integer)()   ' diag -> 上一次命中 i
            Dim lastTrig As New Dictionary(Of Integer, Integer)()  ' diag -> 已延伸到的 i

            For j As Integer = 0 To m - span
                ' 掩码位置不做种子
                If dbMask IsNot Nothing AndAlso dbMask(j) Then Continue For
                Dim key = lookup.PackAt(dbCodes, j)
                If key = Long.MinValue Then Continue For   ' 含歧义，无法作种子

                Dim positions As List(Of Integer) = Nothing
                If Not lookup.TryGetPositions(key, positions) Then Continue For

                For Each i As Integer In positions
                    Dim diag = i - j
                    Dim trigger = False
                    Dim prevI As Integer = 0

                    If _opts.UseTwoHit Then
                        If lastHit.TryGetValue(diag, prevI) Then
                            Dim d = i - prevI
                            ' 非重叠（d ≥ word 长度）且相距 ≤ A
                            If d >= ws AndAlso d <= _opts.WindowTwoHit Then
                                Dim t As Integer = 0
                                If Not lastTrig.TryGetValue(diag, t) OrElse i > t Then
                                    trigger = True
                                End If
                            End If
                        End If
                    Else
                        Dim t2 As Integer = 0
                        If Not lastTrig.TryGetValue(diag, t2) OrElse i > t2 Then
                            trigger = True
                        End If
                    End If
                    lastHit(diag) = i

                    If Not trigger Then Continue For

                    ' ---- 无 gap 延伸：种子取两次命中中点（prevI 为首次命中）----
                    Dim ic As Integer = i
                    Dim jc As Integer = j
                    If _opts.UseTwoHit Then
                        ic = (prevI + i) \ 2
                        jc = ic - diag
                    End If
                    If ic < 0 OrElse jc < 0 OrElse ic >= n OrElse jc >= m Then
                        ic = i : jc = j
                    End If

                    Dim ug = UngappedExtend(queryCodes, dbCodes, ic, jc,
                                            _opts, BitsToRaw(_opts.XdropUngapBits))
                    lastTrig(diag) = Math.Max(i, ug.BestTo)

                    If ug.BestScore < sMinRaw Then Continue For

                    ' ---- [README §一.5] 两级有 gap 延伸 ----
                    ' 预延伸：X = xdrop_gap（小），仅求最优坐标，不带 traceback
                    Dim gapSeeds = UngappedBestMidpoint(ug)
                    Dim prelim = GappedExtend(queryCodes, dbCodes, gapSeeds.Item1, gapSeeds.Item2,
                                              _opts, BitsToRaw(_opts.XdropGapBits), False)
                    If prelim Is Nothing Then Continue For

                    ' 最终延伸：以预延伸最优段中点为种子，X = xdrop_gap_final（大），带 traceback
                    Dim midQ = (prelim.QueryFrom + prelim.QueryTo) \ 2
                    Dim midS = (prelim.SubjectFrom + prelim.SubjectTo) \ 2
                    Dim hsp = GappedExtend(queryCodes, dbCodes, midQ, midS,
                                           _opts, BitsToRaw(_opts.XdropGapFinalBits), True)
                    If hsp Is Nothing Then hsp = prelim
                    If hsp.RawScore < prelim.RawScore Then
                        hsp = prelim
                    End If
                    ComputeAlignStats(hsp)
                    results.Add(hsp)
                Next
            Next

            Return results
        End Function

        ''' <summary>无 gap 双侧延伸（最优段必含种子，各侧独立 X-drop）</summary>
        Public Function UngappedExtend(q As Int32(), s As Int32(),
                                       ic As Integer, jc As Integer,
                                       opts As SeedExtendOptions, xdropRaw As Double) As UngappedResult
            Dim n = q.Length, m = s.Length
            Dim seedScore = _scorer.Score(q(ic), s(jc))

            ' 左侧：score(p..ic-1) 累计，Lbest = max(0, max_p)
            Dim lbest As Double = 0
            Dim la As Integer = ic
            Dim sc As Double = 0
            Dim i = ic - 1, j = jc - 1
            While i >= 0 AndAlso j >= 0
                sc += _scorer.Score(q(i), s(j))
                If sc > lbest Then
                    lbest = sc
                    la = i
                End If
                If lbest - sc > xdropRaw Then Exit While
                i -= 1 : j -= 1
            End While

            ' 右侧
            Dim rbest As Double = 0
            Dim rb As Integer = ic
            sc = 0
            i = ic + 1 : j = jc + 1
            While i < n AndAlso j < m
                sc += _scorer.Score(q(i), s(j))
                If sc > rbest Then
                    rbest = sc
                    rb = i
                End If
                If rbest - sc > xdropRaw Then Exit While
                i += 1 : j += 1
            End While

            Return New UngappedResult With {
                .BestScore = seedScore + lbest + rbest,
                .BestFrom = la, .BestTo = rb,
                .SeedI = ic, .SeedJ = jc
            }
        End Function

        Private Shared Function UngappedBestMidpoint(ug As UngappedResult) As Tuple(Of Integer, Integer)
            Dim mid = (ug.BestFrom + ug.BestTo) \ 2
            Dim diag = ug.SeedI - ug.SeedJ
            Return Tuple.Create(mid, mid - diag)
        End Function

        ' ------------------------------------------------------------------
        ' X-drop 仿射间隙 DP（前向；后向通过反向前缀复用同一实现）
        ' 状态：H(i,j) 终止于对角列；E(i,j) 终止于 query 残基对 subject gap；
        '       F(i,j) 终止于 query gap 对 subject 残基。
        '   E = max( H(i-1,j) - gO , E(i-1,j) - gE )
        '   F = max( H(i,j-1) - gO , F(i,j-1) - gE )
        '   H = max( H(i-1,j-1) + sub , E , F )
        ' 按反对角线 t = (i-i0)+(j-j0) 迭代；每格记录 H/E/F 三状态回溯方向。
        ' ------------------------------------------------------------------
        Public Function GappedForward(q As Int32(), s As Int32(),
                                       si0 As Integer, sj0 As Integer, h0 As Double,
                                       opts As SeedExtendOptions, xdropRaw As Double,
                                       collectTrace As Boolean) As GappedForwardResult
            Dim umax = q.Length - 1 - si0
            Dim vmax = s.Length - 1 - sj0
            Dim NEG = -1.0E+15
            Dim best = h0
            Dim bu = 0, bv = 0
            Dim go = opts.GapOpen, ge = opts.GapExtend

            Dim prev2H As New Dictionary(Of Integer, Double)()  ' t-2: u -> H
            Dim prev1 As New Dictionary(Of Integer, Tuple(Of Double, Double, Double))() ' t-1: u -> (H,E,F)
            Dim traces As Dictionary(Of Integer, Dictionary(Of Integer, CellDir)) = Nothing
            If collectTrace Then traces = New Dictionary(Of Integer, Dictionary(Of Integer, CellDir))()
            prev1(0) = Tuple.Create(h0, NEG, NEG)
            If collectTrace Then
                Dim d0 As New Dictionary(Of Integer, CellDir)()
                d0(0) = New CellDir With {.DirH = -1, .DirE = -1, .DirF = -1}
                traces(0) = d0
            End If

            Dim totalCells As Long = 0
            Dim t As Integer = 1
            Dim alive = True

            While alive AndAlso t <= umax + vmax + 1
                alive = False
                Dim cur As New Dictionary(Of Integer, Tuple(Of Double, Double, Double))()
                Dim tdir As Dictionary(Of Integer, CellDir) = Nothing
                If collectTrace Then tdir = New Dictionary(Of Integer, CellDir)()
                Dim cutoff = best - xdropRaw
                Dim loU = Math.Max(0, t - vmax)
                Dim hiU = Math.Min(umax, t)

                For u As Integer = loU To hiU
                    Dim v = t - u

                    ' E：消耗 query 残基（subject gap），来自 (u-1, v) 的 H 或 E
                    Dim eVal = NEG, eDir As SByte = -1
                    Dim p As Tuple(Of Double, Double, Double) = Nothing
                    If u >= 1 AndAlso prev1.TryGetValue(u - 1, p) Then
                        If p.Item1 > NEG / 2 AndAlso p.Item1 - go > eVal Then
                            eVal = p.Item1 - go : eDir = 0
                        End If
                        If p.Item2 > NEG / 2 AndAlso p.Item2 - ge > eVal Then
                            eVal = p.Item2 - ge : eDir = 1
                        End If
                    End If
                    ' F：消耗 subject 残基（query gap），来自 (u, v-1) 的 H 或 F
                    Dim fVal = NEG, fDir As SByte = -1
                    If prev1.TryGetValue(u, p) Then
                        If p.Item1 > NEG / 2 AndAlso p.Item1 - go > fVal Then
                            fVal = p.Item1 - go : fDir = 0
                        End If
                        If p.Item3 > NEG / 2 AndAlso p.Item3 - ge > fVal Then
                            fVal = p.Item3 - ge : fDir = 2
                        End If
                    End If
                    ' 对角
                    Dim dVal = NEG
                    Dim ph As Double = 0
                    If prev2H.TryGetValue(u - 1, ph) Then
                        dVal = ph + _scorer.Score(q(si0 + u), s(sj0 + v))
                    End If

                    Dim hVal = Math.Max(dVal, Math.Max(eVal, fVal))
                    Dim hDir As SByte = 0S
                    If hVal = dVal Then
                        hDir = 0
                    ElseIf hVal = eVal Then
                        hDir = 1
                    Else
                        hDir = 2
                    End If

                    If hVal > best Then
                        best = hVal : bu = u : bv = v
                    End If

                    Dim hAlive = hVal >= cutoff
                    Dim eAlive = eVal >= cutoff
                    Dim fAlive = fVal >= cutoff
                    If Not (hAlive OrElse eAlive OrElse fAlive) Then Continue For

                    alive = True
                    totalCells += 1
                    If totalCells > opts.MaxCellsPerExtension Then
                        alive = False
                        Exit While
                    End If

                    cur(u) = Tuple.Create(If(hAlive, hVal, NEG),
                                          If(eAlive, eVal, NEG),
                                          If(fAlive, fVal, NEG))
                    If collectTrace Then
                        tdir(u) = New CellDir With {
                            .DirH = If(hAlive, hDir, CSByte(-1S)),
                            .DirE = If(eAlive, eDir, CSByte(-1S)),
                            .DirF = If(fAlive, fDir, CSByte(-1S))
                        }
                    End If
                Next

                ' 滚动：prev2H ← prev1 的 H；prev1 ← cur
                prev2H.Clear()
                For Each kv In prev1
                    If kv.Value.Item1 > NEG / 2 Then prev2H(kv.Key) = kv.Value.Item1
                Next
                prev1 = cur
                If collectTrace Then traces(t) = tdir
                t += 1
            End While

            Dim res As New GappedForwardResult With {
                .Best = best, .BestU = bu, .BestV = bv, .TotalCells = totalCells
            }
            If collectTrace Then res.Traces = traces
            Return res
        End Function

        ''' <summary>两级 gapped 延伸：最终延伸产出带比对字符串的 HSP</summary>
        Public Function GappedExtend(q As Int32(), s As Int32(),
                                     ic As Integer, jc As Integer,
                                     opts As SeedExtendOptions,
                                     xdropFinalRaw As Double,
                                     collect As Boolean) As RawHsp
            Dim h0 = _scorer.Score(q(ic), s(jc))

            ' 前向
            Dim fwd = GappedForward(q, s, ic, jc, h0, opts, xdropFinalRaw, collect)

            ' 后向：反向前缀（含种子）
            Dim rq(ic) As Int32
            For k As Integer = 0 To ic
                rq(k) = q(ic - k)
            Next
            Dim rs(jc) As Int32
            For k As Integer = 0 To jc
                rs(k) = s(jc - k)
            Next
            Dim bwd = GappedForward(rq, rs, 0, 0, h0, opts, xdropFinalRaw, collect)

            Dim total = fwd.Best + bwd.Best - h0
            If total <= 0 Then Return Nothing

            Dim hsp As New RawHsp With {.RawScore = total}
            If Not collect Then
                hsp.QueryFrom = ic - bwd.BestU
                hsp.QueryTo = ic + fwd.BestU
                hsp.SubjectFrom = jc - bwd.BestV
                hsp.SubjectTo = jc + fwd.BestV
                Return hsp
            End If

            ' ---- 状态机回溯，构建比对字符串 ----
            Dim fwdMoves = TracebackMoves(fwd.Traces, fwd.BestU, fwd.BestV)
            Dim bwdMoves = TracebackMoves(bwd.Traces, bwd.BestU, bwd.BestV)

            ' 前向段：从种子向外（moves 已反转成 seed→best 顺序）
            Dim fq As New StringBuilder(), fs As New StringBuilder()
            fq.Append(DecodeCode(q(ic)))
            fs.Append(DecodeCode(s(ic)))
            Dim qi = ic, sj = jc
            For Each mv As Byte In fwdMoves
                If mv = 0 Then
                    qi += 1 : sj += 1
                    fq.Append(DecodeCode(q(qi)))
                    fs.Append(DecodeCode(s(sj)))
                ElseIf mv = 1 Then
                    qi += 1
                    fq.Append(DecodeCode(q(qi)))
                    fs.Append("-"c)
                Else
                    sj += 1
                    fq.Append("-"c)
                    fs.Append(DecodeCode(s(sj)))
                End If
            Next

            ' 后向段：在反转坐标上从种子向外走，得到的字符串是原序列逆序，
            ' 最后整体反转拼接到前面
            Dim bq As New StringBuilder(), bs As New StringBuilder()
            bq.Append(DecodeCode(rq(0)))
            bs.Append(DecodeCode(rs(0)))
            qi = 0 : sj = 0
            For Each mv As Byte In bwdMoves
                If mv = 0 Then
                    qi += 1 : sj += 1
                    bq.Append(DecodeCode(rq(qi)))
                    bs.Append(DecodeCode(rs(sj)))
                ElseIf mv = 1 Then
                    qi += 1
                    bq.Append(DecodeCode(rq(qi)))
                    bs.Append("-"c)
                Else
                    sj += 1
                    bq.Append("-"c)
                    bs.Append(DecodeCode(rs(sj)))
                End If
            Next

            Dim bqS = bq.ToString().ToCharArray()
            Array.Reverse(bqS)
            Dim bsS = bs.ToString().ToCharArray()
            Array.Reverse(bsS)

            hsp.QueryAlign = New String(bqS) & fq.ToString().Substring(1)
            hsp.SubjectAlign = New String(bsS) & fs.ToString().Substring(1)
            hsp.QueryFrom = ic - bwd.BestU
            hsp.QueryTo = ic + fwd.BestU
            hsp.SubjectFrom = jc - bwd.BestV
            hsp.SubjectTo = jc + fwd.BestV
            Return hsp
        End Function

        ''' <summary>从 (bu,bv) 回溯到种子 (0,0)：返回 seed→best 顺序的移动序列</summary>
        Public Function TracebackMoves(traces As Dictionary(Of Integer, Dictionary(Of Integer, CellDir)),
                                   bu As Integer, bv As Integer) As List(Of Byte)
            Dim moves As New List(Of Byte)()
            Dim u = bu, v = bv, st As Integer = 0
            Dim tt = bu + bv
            While tt > 0 OrElse u <> 0 OrElse v <> 0 OrElse st <> 0
                Dim tdir As Dictionary(Of Integer, CellDir) = Nothing
                Dim ent As CellDir = Nothing
                If Not traces.TryGetValue(tt, tdir) OrElse Not tdir.TryGetValue(u, ent) Then
                    Exit While   ' 防御：理论上不应发生
                End If
                If st = 0 Then
                    If ent.DirH = 0 Then
                        moves.Add(0) : u -= 1 : v -= 1 : tt -= 2 : st = 0
                    ElseIf ent.DirH = 1 Then
                        ' H 经 E 到达：同格 DirE 决定前驱状态（gap-open→H / 延续→E）
                        moves.Add(1) : u -= 1 : tt -= 1
                        If ent.DirE = 0 Then
                            st = 0
                        ElseIf ent.DirE = 1 Then
                            st = 1
                        Else
                            Exit While
                        End If
                    ElseIf ent.DirH = 2 Then
                        moves.Add(2) : v -= 1 : tt -= 1
                        If ent.DirF = 0 Then
                            st = 0
                        ElseIf ent.DirF = 2 Then
                            st = 2
                        Else
                            Exit While
                        End If
                    Else
                        Exit While
                    End If
                ElseIf st = 1 Then
                    moves.Add(1) : u -= 1 : tt -= 1
                    If ent.DirE = 0 Then
                        st = 0
                    ElseIf ent.DirE = 1 Then
                        st = 1
                    Else
                        Exit While
                    End If
                Else
                    moves.Add(2) : v -= 1 : tt -= 1
                    If ent.DirF = 0 Then
                        st = 0
                    ElseIf ent.DirF = 2 Then
                        st = 2
                    Else
                        Exit While
                    End If
                End If
                If tt < 0 Then Exit While
            End While
            moves.Reverse()
            Return moves
        End Function

        ''' <summary>
        ''' 由比对字符串计算 identities / positives / gaps / midline。
        ''' positives：蛋白 = 打分 &gt; 0 的列；核酸 = 恒同列。
        ''' midline：恒同列输出残基字母，正分列输出 '+'，其余空格（BLAST 惯例）。
        ''' </summary>
        Private Sub ComputeAlignStats(hsp As RawHsp)
            Dim qa = hsp.QueryAlign
            Dim sa = hsp.SubjectAlign
            Dim idents = 0, poss = 0, gaps = 0
            Dim mid As New StringBuilder()
            For c As Integer = 0 To qa.Length - 1
                Dim a = qa(c)
                Dim b = sa(c)
                If a = "-"c OrElse b = "-"c Then
                    gaps += 1
                    mid.Append(" "c)
                ElseIf a = b Then
                    idents += 1
                    poss += 1
                    mid.Append(a)
                Else
                    Dim sc = _scorer.Score(EncodeChar(a), EncodeChar(b))
                    If sc > 0 Then
                        poss += 1
                        mid.Append("+"c)
                    Else
                        mid.Append(" "c)
                    End If
                End If
            Next
            hsp.Identities = idents
            hsp.Positives = poss
            hsp.Gaps = gaps
            hsp.Midline = mid.ToString()
        End Sub

        Private Function EncodeChar(ch As Char) As Int32
            If _isNt Then Return NtAlphabet.EncodeChar(ch)
            Return AaAlphabet.EncodeChar(ch)
        End Function

    End Class

    ''' <summary>打分器统一接口</summary>
    Public Interface IScorer

        Function Score(a As Int32, b As Int32) As Double

    End Interface

    Public Class CellDir

        Public DirH As SByte
        Public DirE As SByte
        Public DirF As SByte

    End Class

    Public Class UngappedResult

        Public BestScore As Double
        Public BestFrom As Integer
        Public BestTo As Integer
        Public SeedI As Integer
        Public SeedJ As Integer

    End Class

    Public Class GappedForwardResult

        Public Best As Double
        Public BestU As Integer
        Public BestV As Integer
        Public TotalCells As Long
        Public Traces As Dictionary(Of Integer, Dictionary(Of Integer, CellDir))

    End Class

End Namespace
