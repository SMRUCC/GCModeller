' ============================================================================
' SignalScan.vb — 终止子（发夹+U串）与启动子（-35/-10 框）序列扫描
' ----------------------------------------------------------------------------
' [operon.md 特征5] 操纵子边界应有上游启动子（σ 因子结合位点）与下游
'   Rho 非依赖型终止子（发夹+U串）；若两个同向基因之间检测到强终止子，
'   则大概率属于两个操纵子。
'
' 终止子扫描（需基因组 FASTA）：茎 4–12 bp（Watson-Crick 配对，容 1 错配）、
'   环 3–8 nt、环后 12 nt 内 ≥4 连续 T；评分 = 0.45·配对率 + 0.35·min(1,U/8)
'   + 0.2·min(1,(茎−4)/6) + GC 茎 0.05 奖励，截断 [0,1]。
'   读框约定：以"上游基因链"的读框扫描（负链基因 → 扫 revcomp(区间)）。
' 启动子扫描：-35 TTGACA / -10 TATAAT，各容 ≤2 错配，框起始间距 15–19 bp
'   （σ70 经典 17±2）；评分 = 1 − 总错配/8。以"下游基因链"读框扫描。
' ============================================================================

Namespace OperonPredictor.Core

    Public Module SignalScan

        Private Function Revcomp(s As String) As String
            Dim ch = s.ToCharArray()
            Array.Reverse(ch)
            For i = 0 To ch.Length - 1
                Select Case ch(i)
                    Case "A"c : ch(i) = "T"c
                    Case "T"c : ch(i) = "A"c
                    Case "G"c : ch(i) = "C"c
                    Case "C"c : ch(i) = "G"c
                    Case "U"c : ch(i) = "A"c
                End Select
            Next
            Return New String(ch)
        End Function

        ''' <summary>
        ''' 在序列 seq 上扫描 Rho 非依赖型终止子强度 [0,1]。
        ''' frameMinus=True 时先转成 revcomp（负链读框）。
        ''' </summary>
        Public Function ScanTerminator(seq As String, frameMinus As Boolean) As Double
            Dim s = If(frameMinus, Revcomp(seq), seq)
            Dim n = s.Length
            Dim best As Double = 0
            If n < 15 Then Return 0
            For loopLen = 3 To 8
                For stem = 4 To Math.Min(12, (n - loopLen) \ 2)
                    Dim maxI = n - 2 * stem - loopLen
                    For i = 0 To maxI
                        ' 茎配对评分（左 stem vs 右 stem 的反向互补）
                        Dim pairs As Int32 = 0
                        Dim gc As Int32 = 0
                        For k = 0 To stem - 1
                            Dim a = s(i + k)
                            Dim b = s(i + 2 * stem + loopLen - 1 - k)
                            If IsWc(a, b) Then
                                pairs += 1
                                If (a = "G"c AndAlso b = "C"c) OrElse (a = "C"c AndAlso b = "G"c) Then gc += 1
                            End If
                        Next
                        If pairs < stem - 1 Then Continue For     ' 容 1 错配
                        Dim frac = pairs / CDbl(stem)
                        ' U 串：环后 12 nt 内最长连续 T
                        Dim utract As Int32 = 0
                        Dim cur As Int32 = 0
                        Dim tailStart = i + 2 * stem + loopLen
                        Dim tailEnd = Math.Min(n, tailStart + 12)
                        For t = tailStart To tailEnd - 1
                            If s(t) = "T"c OrElse s(t) = "U"c Then
                                cur += 1
                                If cur > utract Then utract = cur
                            Else
                                cur = 0
                            End If
                        Next
                        If utract < 4 Then Continue For
                        Dim score = 0.45 * frac + 0.35 * Math.Min(1.0, utract / 8.0) +
                                    0.2 * Math.Min(1.0, (stem - 4) / 6.0)
                        If gc >= stem * 0.5 Then score += 0.05
                        If score > best Then best = Math.Min(1.0, score)
                    Next
                Next
            Next
            Return best
        End Function

        Private Function IsWc(a As Char, b As Char) As Boolean
            Return (a = "A"c AndAlso b = "T"c) OrElse (a = "T"c AndAlso b = "A"c) OrElse
                   (a = "G"c AndAlso b = "C"c) OrElse (a = "C"c AndAlso b = "G"c)
        End Function

        ''' <summary>
        ''' -35/-10 框扫描强度 [0,1]；frameMinus=True 时先转 revcomp。
        ''' </summary>
        Public Function ScanPromoter(seq As String, frameMinus As Boolean) As Double
            Dim s = If(frameMinus, Revcomp(seq), seq)
            Dim n = s.Length
            Dim best As Double = 0
            If n < 21 Then Return 0
            For i = 0 To n - 20
                Dim mm35 = HammingTo(s, i, "TTGACA")
                If mm35 > 2 Then Continue For
                For spacing = 15 To 19
                    Dim j = i + spacing
                    If j + 6 > n Then Exit For
                    Dim mm10 = HammingTo(s, j, "TATAAT")
                    If mm10 <= 2 Then
                        Dim sc = 1.0 - (mm35 + mm10) / 8.0
                        If sc > best Then best = sc
                    End If
                Next
            Next
            Return best
        End Function

        Private Function HammingTo(s As String, pos As Int32, motif As String) As Int32
            Dim mm As Int32 = 0
            For k = 0 To motif.Length - 1
                If s(pos + k) <> motif(k) Then mm += 1
            Next
            Return mm
        End Function

        ''' <summary>
        ''' 提取两基因间区间序列（含上游基因 3' 端 15 nt 重叠，终止子可跨基因末端）。
        ''' 返回 Nothing 表示无序列可用。
        ''' </summary>
        Public Function IntergenicSequence(fasta As Dictionary(Of String, String),
                                           a As Gene, b As Gene,
                                           ByRef frameMinusTerm As Boolean,
                                           ByRef frameMinusProm As Boolean) As String
            If fasta Is Nothing OrElse Not fasta.ContainsKey(a.Contig) Then Return Nothing
            Dim contigSeq = fasta(a.Contig)
            If contigSeq Is Nothing OrElse contigSeq.Length = 0 Then Return Nothing
            Dim seqStart = Math.Max(1, a.EndMax + 1 - 15)     ' 含上游 3' 端 15nt
            Dim seqEnd = Math.Min(contigSeq.Length, Math.Max(b.StartMin - 1, seqStart))
            If seqEnd < seqStart Then Return Nothing
            frameMinusTerm = (a.Strand = "-"c)
            frameMinusProm = (b.Strand = "-"c)
            Return contigSeq.Substring(seqStart - 1, seqEnd - seqStart + 1)
        End Function

    End Module

End Namespace
