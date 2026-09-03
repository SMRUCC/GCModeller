' ============================================================================
' TestData.vb — 测试用的确定性数据工厂
' ----------------------------------------------------------------------------
' 设计原则：
'   1. 每个用例自带 New Random(fixedSeed)，随机流不跨用例共享 —— 任何用例可
'      单独运行且结果可复现（原 SelfTest 使用模块级 _rng 单例，用例间顺序耦合）。
'   2. 植入（plant）数据同时返回「真值」（位点下标、链向），供恢复质量断言使用。
'   3. 反向互补在测试内独立实现（不复用 Alphabet.Revcomp），保证是真正的交叉验证。
' ============================================================================

Option Strict On

Imports System.IO

Namespace EmMotif

    ''' <summary>一个植入位点的真值</summary>
    Public Class SiteTruth

        ''' <summary>所属序列下标</summary>
        Public Property SeqIndex As Integer

        ''' <summary>0-based 起始下标；负数表示该序列未植入位点</summary>
        Public Property Pos As Integer

        ''' <summary>是否植入在负链（序列中写入的是 motif 的反向互补）</summary>
        Public Property StrandMinus As Boolean

        Public Overrides Function ToString() As String
            Return $"seq{SeqIndex}@{Pos}{(If(StrandMinus, "-", "+"))}"
        End Function

    End Class

    ''' <summary>植入结果：序列集合 + 全部位点的真值</summary>
    Public Class PlantResult

        Public ReadOnly Sequences As New List(Of String)()
        Public ReadOnly Sites As New List(Of SiteTruth)()

        ''' <summary>按序列下标取出该序列的全部植入位点</summary>
        Public Function SitesOf(seqIndex As Integer) As List(Of SiteTruth)
            Dim out As New List(Of SiteTruth)()
            For Each s In Sites
                If s.SeqIndex = seqIndex Then out.Add(s)
            Next
            Return out
        End Function

        ''' <summary>含有至少一个植入位点的序列条数</summary>
        Public ReadOnly Property SequencesWithSite As Integer
            Get
                Dim seen As New HashSet(Of Integer)()
                For Each s In Sites
                    seen.Add(s.SeqIndex)
                Next
                Return seen.Count
            End Get
        End Property

    End Class

    Public Module TestData

        Public Const DnaLetters As String = "ACGT"
        Public Const ProteinLetters As String = "ACDEFGHIKLMNPQRSTVWY"

        ''' <summary>每个用例独立取随机流，避免用例间顺序耦合</summary>
        Public Function MakeRng(seed As Integer) As Random
            Return New Random(seed)
        End Function

        ''' <summary>
        ''' 独立实现的反向互补（DNA）。刻意不复用 Alphabet.Revcomp，
        ''' 以便与被测实现做交叉验证。
        ''' </summary>
        Public Function RevcompOf(seq As String) As String
            Dim ch = seq.ToCharArray()
            Array.Reverse(ch)
            Dim map As New Dictionary(Of Char, Char)() From {
                {"A"c, "T"c}, {"T"c, "A"c}, {"C"c, "G"c}, {"G"c, "C"c},
                {"U"c, "A"c}, {"a"c, "T"c}, {"t"c, "A"c}, {"c"c, "G"c}, {"g"c, "C"c}}
            For i = 0 To ch.Length - 1
                Dim c As Char
                If map.TryGetValue(ch(i), c) Then ch(i) = c
            Next
            Return New String(ch)
        End Function

        ''' <summary>
        ''' 生成随机背景序列并在其中植入 motif（支持突变、缺失比例、负链比例、每条多位点）。
        ''' </summary>
        Public Function Plant(letters As String, seqCount As Integer, seqLen As Integer,
                              motif As String, mutationRate As Double, withSiteRatio As Double,
                              seed As Integer,
                              Optional revcompFraction As Double = 0.0,
                              Optional sitesPerSequence As Integer = 1) As PlantResult
            Dim rng = New Random(seed)
            Dim w = motif.Length
            Dim rc = RevcompOf(motif)
            Dim result As New PlantResult()

            For i = 0 To seqCount - 1
                Dim ch(seqLen - 1) As Char
                For t = 0 To seqLen - 1
                    ch(t) = letters(rng.Next(letters.Length))
                Next

                If rng.NextDouble() < withSiteRatio Then
                    For s = 1 To sitesPerSequence
                        Dim pos = rng.Next(0, seqLen - w + 1)
                        Dim minus = (rng.NextDouble() < revcompFraction)
                        Dim site = If(minus, rc, motif).ToCharArray()
                        For k = 0 To w - 1
                            If rng.NextDouble() < mutationRate Then
                                ' 突变到「任意字母」，可能出现与原文相同的字母，
                                ' 与真实生物序列的退化情形一致
                                site(k) = letters(rng.Next(letters.Length))
                            End If
                        Next
                        Array.Copy(site, 0, ch, pos, w)
                        result.Sites.Add(New SiteTruth With {
                            .SeqIndex = i, .Pos = pos, .StrandMinus = minus})
                    Next
                End If

                result.Sequences.Add(New String(ch))
            Next

            Return result
        End Function

        ''' <summary>DNA 植入数据（默认 12% 突变率、80% 序列含位点）</summary>
        Public Function PlantDna(seqCount As Integer, seqLen As Integer, motif As String,
                                 seed As Integer,
                                 Optional mutationRate As Double = 0.12,
                                 Optional withSiteRatio As Double = 0.8,
                                 Optional revcompFraction As Double = 0.0,
                                 Optional sitesPerSequence As Integer = 1) As PlantResult
            Return Plant(DnaLetters, seqCount, seqLen, motif, mutationRate, withSiteRatio,
                         seed, revcompFraction, sitesPerSequence)
        End Function

        ''' <summary>蛋白植入数据</summary>
        Public Function PlantProtein(seqCount As Integer, seqLen As Integer, motif As String,
                                     seed As Integer,
                                     Optional mutationRate As Double = 0.1,
                                     Optional withSiteRatio As Double = 1.0) As PlantResult
            Return Plant(ProteinLetters, seqCount, seqLen, motif, mutationRate, withSiteRatio, seed)
        End Function

        ''' <summary>批量编码</summary>
        Public Function EncodeAll(seqs As List(Of String), alpha As Alphabet) As List(Of Int32())
            Dim out As New List(Of Int32())()
            For Each s In seqs
                out.Add(alpha.Encode(s))
            Next
            Return out
        End Function

        ''' <summary>order-0 背景频率（拉普拉斯平滑；与 EmSearch.ComputeBackground 同式）</summary>
        Public Function BgOf(encs As List(Of Int32()), alpha As Alphabet,
                             Optional pseudo As Double = 0.1) As Double()
            Dim cnt(alpha.Size - 1) As Double
            Dim total As Double = 0
            For Each enc In encs
                For Each a In enc
                    If a >= 0 Then
                        cnt(a) += 1.0
                        total += 1.0
                    End If
                Next
            Next
            Dim freq(alpha.Size - 1) As Double
            Dim denom = total + pseudo * alpha.Size
            For a = 0 To alpha.Size - 1
                freq(a) = (cnt(a) + pseudo) / denom
            Next
            Return freq
        End Function

        ''' <summary>均匀背景（用于人工构造、便于手算期望值的用例）</summary>
        Public Function UniformBg(size As Integer) As Double()
            Dim bg(size - 1) As Double
            For i = 0 To size - 1
                bg(i) = 1.0 / size
            Next
            Return bg
        End Function

        ''' <summary>两个等长字符串逐位相同的个数（共识恢复质量的度量）</summary>
        Public Function MatchCount(a As String, b As String) As Integer
            Dim n = Math.Min(a.Length, b.Length)
            Dim c = 0
            For i = 0 To n - 1
                If a(i) = b(i) Then c += 1
            Next
            Return c
        End Function

        ''' <summary>在 needles 中任取一个，返回与 text 的最大匹配数（多 motif 用例用）</summary>
        Public Function BestMatchCount(text As String, ParamArray needles As String()) As Integer
            Dim best = -1
            For Each nd In needles
                Dim m = MatchCount(text, nd)
                If m > best Then best = m
            Next
            Return best
        End Function

        ''' <summary>
        ''' 定位随生成输出复制的测试数据文件（em_test\*.fa）。
        ''' 依次探测 基准目录\em_test\file、基准目录\file、以及源码树路径（便于 IDE 内直接跑）。
        ''' </summary>
        Public Function FindDataFile(fileName As String) As String
            Dim candidates As New List(Of String)() From {
                Path.Combine(AppContext.BaseDirectory, "em_test", fileName),
                Path.Combine(AppContext.BaseDirectory, fileName),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "em_test", fileName),
                Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "em_test", fileName)
            }
            For Each p In candidates
                If File.Exists(p) Then Return Path.GetFullPath(p)
            Next
            Return Nothing
        End Function

    End Module

End Namespace
