Imports System.Globalization
Imports System.IO

Namespace siRNAHit

    Public Module BlastFilterMiRNATargets

        ' ---------- 打分结果容器 ----------
        ' Python 用 tuple 返回多值，VB.NET 用 Structure 等价实现
        Public Structure AlignmentScore
            Public Score As Double            ' 总罚分（越低越好）
            Public SeedMismatches As Integer  ' 种子区非 G:U 错配数
            Public TotalMismatches As Integer ' 全比对区非 G:U 错配数
            Public GuPairs As Integer         ' G:U wobble 配对数
        End Structure

        ' ---------- psRNATarget 打分函数 ----------
        ' 对应 Python 的 score_alignment(qseq, sseq, seed_start=2, seed_end=13, penalty_multiplier=1.5)
        Public Function ScoreAlignment(qseq As String,
                                       sseq As String,
                                       Optional seedStart As Integer = 2,
                                       Optional seedEnd As Integer = 13,
                                       Optional penaltyMultiplier As Double = 1.5) As AlignmentScore

            Dim result As New AlignmentScore With {
                .Score = 0.0, .SeedMismatches = 0,
                .TotalMismatches = 0, .GuPairs = 0}

            If qseq Is Nothing OrElse sseq Is Nothing Then Return result

            Dim n As Integer = Math.Min(qseq.Length, sseq.Length)

            ' 对应 Python: for i, (q_base, s_base) in enumerate(zip(qseq, sseq), start=1)
            For i As Integer = 1 To n
                Dim qBase As Char = Char.ToUpper(qseq(i - 1))
                Dim sBase As Char = Char.ToUpper(sseq(i - 1))

                ' 对应 Python: s_base = s_base.replace('T', 'U')
                ' 忠实还原原版行为：仅转换靶序列，不转换 miRNA 序列
                If sBase = "T"c Then sBase = "U"c

                ' Watson-Crick 配对：A-U / G-C，不罚分
                ' 注：BLASTN 输出的 gap '-' 会落入 Else 分支按错配计分，
                '     与 Python 原版行为一致
                If (qBase = "A"c AndAlso sBase = "U"c) OrElse
                   (qBase = "U"c AndAlso sBase = "A"c) OrElse
                   (qBase = "G"c AndAlso sBase = "C"c) OrElse
                   (qBase = "C"c AndAlso sBase = "G"c) Then
                    ' 无罚分
                ElseIf (qBase = "G"c AndAlso sBase = "U"c) OrElse
                       (qBase = "U"c AndAlso sBase = "G"c) Then
                    ' G:U wobble：罚 0.5 分
                    result.GuPairs += 1
                    result.Score += 0.5
                Else
                    ' 非 G:U 错配
                    result.TotalMismatches += 1
                    If i >= seedStart AndAlso i <= seedEnd Then
                        result.SeedMismatches += 1
                        result.Score += penaltyMultiplier      ' 种子区错配 ×1.5
                    Else
                        result.Score += 1.0
                    End If
                End If
            Next

            Return result
        End Function

        Public Sub BlastnFilter(inputFile As String,
                                Optional eCutoff As Double = 5.0,
                                Optional seedStart As Integer = 2,
                                Optional seedEnd As Integer = 13,
                                Optional maxSeedMm As Integer = 2,
                                Optional maxTotalMm As Integer = 8,
                                Optional maxGu As Integer = 7)

            ' 表头（12 列，与 Python 版完全一致）
            Console.WriteLine(String.Join(vbTab,
                "sRNA_id", "target_id", "target_start", "target_end",
                "strand", "evalue", "score", "seed_mm", "total_mm",
                "gu_pairs", "qseq", "sseq"))

            Using reader As New StreamReader(inputFile)
                While Not reader.EndOfStream
                    Dim line As String = reader.ReadLine()
                    If line Is Nothing Then Exit While
                    line = line.Trim()                       ' 对应 Python: line.strip()

                    If String.IsNullOrWhiteSpace(line) Then Continue While

                    Dim cols As String() = line.Split(vbTab)
                    If cols.Length < 12 Then Continue While  ' 对应 Python: if len(cols) < 12: continue

                    Dim qseqid As String = cols(0)
                    Dim sseqid As String = cols(1)
                    Dim sstart As String = cols(2)
                    Dim send As String = cols(3)
                    Dim sstrand As String = cols(6)
                    Dim qseq As String = cols(7)
                    Dim sseq As String = cols(8)
                    Dim evalueStr As String = cols(10)

                    ' E-value 解析：BLASTN 常输出科学计数法（如 2e-07），
                    ' 用 InvariantCulture 避免系统区域设置（如德语逗号小数点）干扰
                    Dim evalue As Double
                    If Not Double.TryParse(evalueStr, NumberStyles.Float,
                                           CultureInfo.InvariantCulture, evalue) Then Continue While

                    Dim scored As AlignmentScore =
                        ScoreAlignment(qseq, sseq, seedStart, seedEnd)

                    ' psRNATarget 过滤条件（与 Python 版一致）
                    If evalue <= eCutoff AndAlso
                       scored.SeedMismatches <= maxSeedMm AndAlso
                       scored.TotalMismatches <= maxTotalMm AndAlso
                       scored.GuPairs <= maxGu Then

                        ' score 统一保留 1 位小数（psRNATarget 分数均为 0.5 的整数倍）
                        Console.WriteLine(String.Join(vbTab,
                            qseqid, sseqid, sstart, send, sstrand,
                            evalueStr,
                            scored.Score.ToString("F1", CultureInfo.InvariantCulture),
                            scored.SeedMismatches.ToString(),
                            scored.TotalMismatches.ToString(),
                            scored.GuPairs.ToString(),
                            qseq, sseq))
                    End If
                End While
            End Using
        End Sub

    End Module
End Namespace