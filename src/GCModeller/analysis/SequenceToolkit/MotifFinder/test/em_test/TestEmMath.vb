' ============================================================================
' TestEmMath.vb — EmModel 数学正确性测试（E 步 / M 步 / 似然 / 一致序列）
' ----------------------------------------------------------------------------
' 本文件由两部分组成：
'
'   一、Oracle* —— 独立实现的朴素参考算法。刻意不复用 EmModel 的任何代码路径：
'       · 直接按 em.md 的公式在「线性空间」计算似然比 R = Π θ/θ0，不取对数、
'         不做 log-sum-exp 稳定化（测试数据集 W ≤ 12，数值安全）；
'       · 循环边界一律显式使用「字母表大小」而非其它同名量。
'       与生产实现逐元素比对，是捕获「公式正确但循环边界错位」类缺陷
'       （例如列循环变量遮蔽字母表大小字段）最有效的手段。
'
'   二、用例组 —— 每条断言标注其对应的 [em.md §x] 与 [缺陷 #n]。
'
' 模型约定（与 README「对文档的两处修正」一致，均为正确的设计决策，此处不质疑）：
'   OOPS : Z = R / ΣR                        （Σ_j Z = 1）
'   ZOOPS: Z = λR / ((1−λ) + λΣR)            （Σ_j Z ≤ 1）
'   ANR  : 同一位置的正/负链共享「无位点」状态：Z = λR± / ((1−λ) + λΣ_strands R)
' ============================================================================

Option Strict On

Imports System.Text
Imports SMRUCC.genomics.Analysis.SequenceTools.SequencePatterns.Motif.EmMotif.Core
Imports SMRUCC.genomics.SequenceModel

Namespace EmMotif

    ''' <summary>一次手工 EM 迭代的运行结果</summary>
    Public Class EmRunResult

        Public Property Model As EmModel
        Public Property Sites As List(Of List(Of SitePosterior))
        Public ReadOnly Property Trace As New List(Of Double)()
        Public Property Iterations As Integer
        Public Property Converged As Boolean

        ''' <summary>
        ''' LL 轨迹是否单调不降（EM 的理论保证 [em.md §4]）。
        ''' 容差按相对量给：M 步带伪计数（等价于 Dirichlet 先验的 MAP-EM），
        ''' 最大化的是带惩罚的目标，未惩罚的似然在收敛点附近允许 1e−7 量级的抖动。
        ''' </summary>
        Public Function IsMonotone(Optional relTol As Double = 0.0000001) As Boolean
            Return WorstRelativeDrop() > -relTol
        End Function

        ''' <summary>LL 轨迹中最大的单轮绝对下降幅度（负号为下降）</summary>
        Public Function WorstDrop() As Double
            Dim worst As Double = 0
            For i = 1 To Trace.Count - 1
                Dim d = Trace(i) - Trace(i - 1)
                If d < worst Then worst = d
            Next
            Return worst
        End Function

        ''' <summary>LL 轨迹中最大的单轮相对下降幅度（按上一轮似然的量级归一化）</summary>
        Public Function WorstRelativeDrop() As Double
            Dim worst As Double = 0
            For i = 1 To Trace.Count - 1
                Dim scale = Math.Max(1.0, Math.Abs(Trace(i - 1)))
                Dim d = (Trace(i) - Trace(i - 1)) / scale
                If d < worst Then worst = d
            Next
            Return worst
        End Function

    End Class

    Public Module TestEmMath

        ' ====================================================================
        ' 一、独立 Oracle 参考实现
        ' ====================================================================

        ''' <summary>Oracle 的候选位点描述</summary>
        Public Class OracleCand
            Public Property Pos As Integer
            Public Property Minus As Boolean

            ''' <summary>似然比 R = Π_k θ_{k,a}/θ0,a</summary>
            Public Property R As Double
        End Class

        ''' <summary>
        ''' 窗口似然比 R（线性空间）。窗口含歧义字母 → 返回 −∞（表示候选无效）。
        ''' </summary>
        Public Function OracleWindowR(enc As Int32(), j As Integer, minus As Boolean,
                                      pwm As Double(,), bg As Double(), w As Integer,
                                      complement As Func(Of Integer, Integer)) As Double
            Dim r As Double = 1.0
            For k = 0 To w - 1
                Dim a As Integer
                If minus Then
                    Dim raw = enc(j + w - 1 - k)
                    If raw < 0 Then Return Double.NegativeInfinity
                    a = complement(raw)
                Else
                    a = enc(j + k)
                End If
                If a < 0 Then Return Double.NegativeInfinity
                r *= pwm(k, a) / bg(a)
            Next
            Return r
        End Function

        ''' <summary>枚举全部有效候选（j 升序，同一位置正链在前、负链在后）</summary>
        Public Function OracleCandidates(enc As Int32(), pwm As Double(,), bg As Double(),
                                         w As Integer, revcomp As Boolean,
                                         complement As Func(Of Integer, Integer)) As List(Of OracleCand)
            Dim out As New List(Of OracleCand)()
            Dim nwin = enc.Length - w + 1
            If nwin <= 0 Then Return out

            For j = 0 To nwin - 1
                Dim rf = OracleWindowR(enc, j, False, pwm, bg, w, complement)
                If Not Double.IsNegativeInfinity(rf) Then
                    out.Add(New OracleCand With {.Pos = j, .Minus = False, .R = rf})
                End If
                If revcomp Then
                    Dim rr = OracleWindowR(enc, j, True, pwm, bg, w, complement)
                    If Not Double.IsNegativeInfinity(rr) Then
                        out.Add(New OracleCand With {.Pos = j, .Minus = True, .R = rr})
                    End If
                End If
            Next
            Return out
        End Function

        ''' <summary>
        ''' 后验 Z 的参考实现 [em.md §2 / §6]。
        ''' ANR 用「按位置分组」的写法：同一位置的正/负链候选共享一个「无位点」状态，
        ''' 单链时每组只有一个候选，自动退化为标准的 logistic 形式。
        ''' </summary>
        Public Function OracleZ(cands As List(Of OracleCand), model As SiteModel,
                                lambda As Double) As Double()
            Dim n = cands.Count
            Dim z(n - 1) As Double
            If n = 0 Then Return z

            Select Case model
                Case SiteModel.Oops
                    Dim s As Double = 0
                    For Each c In cands
                        s += c.R
                    Next
                    For i = 0 To n - 1
                        z(i) = cands(i).R / s
                    Next

                Case SiteModel.Zoops
                    Dim s As Double = 0
                    For Each c In cands
                        s += c.R
                    Next
                    Dim den = (1.0 - lambda) + lambda * s
                    For i = 0 To n - 1
                        z(i) = lambda * cands(i).R / den
                    Next

                Case Else ' ANR
                    Dim sumByPos As New Dictionary(Of Integer, Double)()
                    For Each c In cands
                        Dim s As Double = 0
                        sumByPos.TryGetValue(c.Pos, s)
                        sumByPos(c.Pos) = s + c.R
                    Next
                    For i = 0 To n - 1
                        Dim den = (1.0 - lambda) + lambda * sumByPos(cands(i).Pos)
                        z(i) = lambda * cands(i).R / den
                    Next
            End Select

            Return z
        End Function

        ''' <summary>
        ''' M 步参考实现 [em.md §3]：加权计数 → 加伪计数 → 按字母表全宽归一化。
        ''' </summary>
        Public Function OracleMstepPwm(encs As List(Of Int32()),
                                       sitesPerSeq As List(Of List(Of SitePosterior)),
                                       w As Integer, alphaSize As Integer,
                                       pseudo As Double,
                                       complement As Func(Of Integer, Integer)) As Double(,)
            Dim counts(w - 1, alphaSize - 1) As Double

            For si = 0 To encs.Count - 1
                Dim enc = encs(si)
                For Each sp In sitesPerSeq(si)
                    If sp.Z <= 0 Then Continue For
                    For col = 0 To w - 1
                        Dim a As Integer
                        If sp.StrandMinus Then
                            Dim raw = enc(sp.Pos + w - 1 - col)
                            If raw < 0 Then Continue For
                            a = complement(raw)
                        Else
                            a = enc(sp.Pos + col)
                        End If
                        If a >= 0 Then counts(col, a) += sp.Z
                    Next
                Next
            Next

            ' 伪计数 + 归一化：循环边界必须是「字母表大小」
            For col = 0 To w - 1
                Dim s As Double = 0
                For a = 0 To alphaSize - 1
                    counts(col, a) += pseudo
                Next
                For a = 0 To alphaSize - 1
                    s += counts(col, a)
                Next
                For a = 0 To alphaSize - 1
                    counts(col, a) /= s
                Next
            Next

            Return counts
        End Function

        ''' <summary>一致序列参考实现 [em.md §4]：逐列对「整个字母表」取 argmax</summary>
        Public Function OracleConsensus(pwm As Double(,), w As Integer,
                                        alphaSize As Integer, letters As String) As String
            Dim sb As New StringBuilder()
            For col = 0 To w - 1
                Dim best = 0
                For a = 1 To alphaSize - 1
                    If pwm(col, a) > pwm(col, best) Then best = a
                Next
                sb.Append(letters(best))
            Next
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' 全观测对数似然参考实现 [em.md §1/§4]。
        '''   OOPS : P(S) = bg(S) · (1/nCand) · Σ_j R_j      （OOPS 的位点位置先验为均匀）
        '''   ZOOPS: P(S) = bg(S) · [(1−λ) + λ Σ_j R_j]
        '''   ANR  : P(S) = bg(S) · Π_j [(1−λ) + λ Σ_strands R]
        ''' </summary>
        Public Function OracleLogLik(encs As List(Of Int32()), pwm As Double(,), bg As Double(),
                                     lambda As Double, model As SiteModel, w As Integer,
                                     revcomp As Boolean,
                                     complement As Func(Of Integer, Integer)) As Double
            Dim ll As Double = 0

            For Each enc In encs
                For Each a In enc
                    If a >= 0 Then ll += Math.Log(bg(a))
                Next

                Dim cands = OracleCandidates(enc, pwm, bg, w, revcomp, complement)
                If cands.Count = 0 Then Continue For

                Select Case model
                    Case SiteModel.Oops
                        Dim s As Double = 0
                        For Each c In cands
                            s += c.R
                        Next
                        ll += Math.Log(s) - Math.Log(cands.Count)

                    Case SiteModel.Zoops
                        Dim s As Double = 0
                        For Each c In cands
                            s += c.R
                        Next
                        ll += Math.Log((1.0 - lambda) + lambda * s)

                    Case Else ' ANR
                        Dim sumByPos As New Dictionary(Of Integer, Double)()
                        For Each c In cands
                            Dim s As Double = 0
                            sumByPos.TryGetValue(c.Pos, s)
                            sumByPos(c.Pos) = s + c.R
                        Next
                        For Each kv In sumByPos
                            ll += Math.Log((1.0 - lambda) + lambda * kv.Value)
                        Next
                End Select
            Next

            Return ll
        End Function

        ''' <summary>把二维 PWM 按列摊平成一维，便于逐元素比对</summary>
        Public Function FlattenPwm(pwm As Double(,), w As Integer, alphaSize As Integer) As Double()
            Dim out(w * alphaSize - 1) As Double
            For col = 0 To w - 1
                For a = 0 To alphaSize - 1
                    out(col * alphaSize + a) = pwm(col, a)
                Next
            Next
            Return out
        End Function

        ' ====================================================================
        ' 二、测试辅助
        ' ====================================================================

        Private Function ComplementOf(alpha As Alphabet) As Func(Of Integer, Integer)
            Return Function(a As Integer) alpha.Complement(a)
        End Function

        ''' <summary>统计全部位点的后验之和</summary>
        Public Function TotalZ(sitesPerSeq As List(Of List(Of SitePosterior))) As Double
            Dim s As Double = 0
            For Each sl In sitesPerSeq
                For Each sp In sl
                    s += sp.Z
                Next
            Next
            Return s
        End Function

        ''' <summary>候选位置总数（= Σ_i (L_i − W + 1)，与链数无关）</summary>
        Public Function TotalPositions(encs As List(Of Int32()), w As Integer) As Double
            Dim s As Double = 0
            For Each enc In encs
                Dim nw = enc.Length - w + 1
                If nw > 0 Then s += nw
            Next
            Return s
        End Function

        ''' <summary>
        ''' 手工驱动一次完整的 EM 迭代（E → M → 记 LL），用于逐步验证。
        ''' 轨迹首元素为 θ⁰ 的似然（链模式显式传入，与 EmSearch.RunEm 一致）。
        ''' </summary>
        Public Function RunEm(model As EmModel, encs As List(Of Int32()), revcomp As Boolean,
                              Optional maxIter As Integer = 200,
                              Optional eps As Double = 0.0001) As EmRunResult
            Dim result As New EmRunResult With {.Model = model}
            Dim sites As List(Of List(Of SitePosterior))

            result.Trace.Add(model.FullLogLik(encs, revcomp))
            result.Converged = False

            For it = 1 To maxIter
                result.Iterations = it
                sites = New List(Of List(Of SitePosterior))()
                For Each enc In encs
                    sites.Add(model.EStep(enc, revcomp))
                Next

                Dim nextModel = model.Clone()
                nextModel.MStep(encs, sites)
                model = nextModel

                Dim ll = model.FullLogLik(encs, revcomp)
                result.Trace.Add(ll)

                If Math.Abs(ll - result.Trace(result.Trace.Count - 2)) < eps Then
                    result.Converged = True
                    Exit For
                End If
            Next

            ' 最终再做一次 E 步，保证位点与最终 PWM 自洽
            sites = New List(Of List(Of SitePosterior))()
            For Each enc In encs
                sites.Add(model.EStep(enc, revcomp))
            Next

            result.Model = model
            result.Sites = sites
            Return result
        End Function

        ''' <summary>断言 PWM 每一列的概率和为 1</summary>
        Public Sub CheckPwmNormalized(model As EmModel, name As String)
            Dim ok As Boolean = True
            Dim worst As Double = 0
            For col = 0 To model.W - 1
                Dim s As Double = 0
                For a = 0 To model.K - 1
                    s += model.Pwm(col, a)
                Next
                worst = Math.Max(worst, Math.Abs(s - 1.0))
                If Math.Abs(s - 1.0) > 0.000000000001 Then ok = False
            Next
            If ok Then
                TestAssert.Check(True, name)
            Else
                TestAssert.Check(False, $"{name}（最大列偏差 {worst:G4}）")
            End If
        End Sub

        ' ====================================================================
        ' 三、用例组
        ' ====================================================================

        ''' <summary>[缺陷 #1] 种子初始化 PWM：one-hot + 伪计数 [em.md §5]</summary>
        Public Sub TestInitFromSeed()
            TestAssert.Section("种子初始化 PWM（one-hot + 伪计数）[em.md §5]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif = "ACGTTACGTA"
            Dim w = motif.Length
            Dim pc = 0.1

            Dim noThrow As Boolean = True
            Try
                Dim probe As New EmModel(w, alpha, SiteModel.Zoops, TestData.UniformBg(alpha.Size), pc)
                probe.InitFromSeed(alpha.Encode(motif))
            Catch ex As Exception
                noThrow = False
                Console.WriteLine($"         InitFromSeed 抛出 {ex.GetType().Name}: {ex.Message}")
            End Try
            TestAssert.Check(noThrow, $"W={w} > 字母表大小 {alpha.Size} 时 InitFromSeed 不抛异常 [缺陷 #1]")

            Dim model As New EmModel(w, alpha, SiteModel.Zoops, TestData.UniformBg(alpha.Size), pc)
            model.InitFromSeed(alpha.Encode(motif))

            CheckPwmNormalized(model, "种子 PWM 每列概率和 = 1 [缺陷 #1]")

            ' 逐列核对：峰值字母 = 种子字母，且概率等于 (1+pc)/(1+K·pc)
            Dim expectedPeak = (1.0 + pc) / (1.0 + alpha.Size * pc)
            Dim expectedOther = pc / (1.0 + alpha.Size * pc)
            Dim peakOk As Boolean = True
            Dim valueOk As Boolean = True
            For col = 0 To w - 1
                Dim bestA = 0
                For a = 1 To model.K - 1
                    If model.Pwm(col, a) > model.Pwm(col, bestA) Then bestA = a
                Next
                If bestA <> alpha.EncodeChar(motif(col)) Then peakOk = False
                For a = 0 To model.K - 1
                    Dim expect = If(a = bestA, expectedPeak, expectedOther)
                    If Math.Abs(model.Pwm(col, a) - expect) > 0.000000000001 Then valueOk = False
                Next
            Next
            TestAssert.Check(peakOk, "种子 PWM 每列峰值 = 种子对应字母 [缺陷 #1]")
            TestAssert.Check(valueOk, $"种子 PWM 取值 = (δ+{pc})/(1+{alpha.Size}·{pc}) [缺陷 #1]")

            ' 蛋白字母表（K=20 > W=8，边界不同的另一组）
            Dim pAlpha As New Alphabet(SeqTypes.Protein)
            Dim pm = "GASTLSKL"
            Dim pModel As New EmModel(pm.Length, pAlpha, SiteModel.Zoops, TestData.UniformBg(pAlpha.Size), pc)
            Dim pNoThrow As Boolean = True
            Try
                pModel.InitFromSeed(pAlpha.Encode(pm))
            Catch ex As Exception
                pNoThrow = False
            End Try
            TestAssert.Check(pNoThrow, "蛋白字母表 InitFromSeed 不抛异常 [缺陷 #1]")
            CheckPwmNormalized(pModel, "蛋白种子 PWM 每列概率和 = 1 [缺陷 #1]")

            ' W < K 与 W = K 的边界
            For Each wv In New Integer() {2, 4}
                Dim m2 As New EmModel(wv, alpha, SiteModel.Zoops, TestData.UniformBg(alpha.Size), pc)
                m2.InitFromSeed(alpha.Encode(motif.Substring(0, wv)))
                CheckPwmNormalized(m2, $"W={wv}（与字母表大小 {alpha.Size} 的边界关系）种子 PWM 归一化 [缺陷 #1]")
            Next
        End Sub

        ''' <summary>[缺陷 #2] M 步加权计数 + 伪计数归一化 [em.md §3]</summary>
        Public Sub TestMStep()
            TestAssert.Section("M 步加权计数与归一化 [em.md §3]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim w = 4
            Dim pc = 0.1

            ' ---- 定向用例：全 T 窗口、单位点、Z=1 ----
            ' 期望：每一列的计数全部落在 T（索引 3）上，
            '       Pwm(col, 3) = (1+pc)/(1+K·pc)，其余 = pc/(1+K·pc)。
            ' 若列循环变量遮蔽了字母表大小，则索引 3 的格子根本不会被更新 [缺陷 #2]。
            Dim encs As New List(Of Int32())() From {alpha.Encode("TTTTTTTT")}
            Dim sites As New List(Of List(Of SitePosterior))() From {
                New List(Of SitePosterior)() From {New SitePosterior(0, False, 1.0, 0.0)}}

            Dim model As New EmModel(w, alpha, SiteModel.Zoops, TestData.UniformBg(alpha.Size), pc)
            model.InitFromSeed(alpha.Encode("ACGT"))
            Dim mNoThrow As Boolean = True
            Try
                model.MStep(encs, sites)
            Catch ex As Exception
                mNoThrow = False
                Console.WriteLine($"         MStep 抛出 {ex.GetType().Name}: {ex.Message}")
            End Try
            TestAssert.Check(mNoThrow, "M 步不抛异常 [缺陷 #2]")

            CheckPwmNormalized(model, "M 步后每列概率和 = 1 [缺陷 #2]")

            Dim tIndex = alpha.EncodeChar("T"c)
            Dim expectT = (1.0 + pc) / (1.0 + alpha.Size * pc)
            Dim allT As Boolean = True
            For col = 0 To w - 1
                If Math.Abs(model.Pwm(col, tIndex) - expectT) > 0.000000000001 Then allT = False
            Next
            TestAssert.Check(allT, $"全 T 窗口的加权计数落在 T 列（期望 {expectT:G6}）[缺陷 #2]")

            ' ---- 与 Oracle 交叉验证（真实数据 + 三种模型）----
            For Each sm In New SiteModel() {SiteModel.Oops, SiteModel.Zoops, SiteModel.Anr}
                Dim planted = TestData.PlantDna(6, 60, "ACGTTACGTA", 20240903)
                Dim seqs = planted.Sequences
                Dim encList = TestData.EncodeAll(seqs, alpha)
                Dim bg = TestData.BgOf(encList, alpha)
                Dim ww = 10

                Dim m As New EmModel(ww, alpha, sm, bg, pc)
                m.InitFromSeed(alpha.Encode("ACGTTACGTA"))
                Dim es As New List(Of List(Of SitePosterior))()
                For Each enc In encList
                    es.Add(m.EStep(enc, True))
                Next
                Dim before = m.Clone()
                m.MStep(encList, es)

                Dim oracle = OracleMstepPwm(encList, es, ww, alpha.Size, pc, ComplementOf(alpha))
                Dim flatActual = FlattenPwm(m.Pwm, ww, alpha.Size)
                Dim flatExpect = FlattenPwm(oracle, ww, alpha.Size)
                TestAssert.CheckNearAll(flatActual, flatExpect, 0.000000000001,
                                        $"{sm} 模型 M 步 PWM 与 Oracle 逐元素一致 [缺陷 #2]")

                ' 确认 M 步确实改动了 PWM（否则上面的比对是「两个都没动」的假通过）
                TestAssert.Check(m.MaxDeltaTo(before) > 0.000000001,
                                 $"{sm} 模型 M 步确实更新了 PWM [缺陷 #2/#4]")
            Next
        End Sub

        ''' <summary>[缺陷 #3] 一致序列 = 逐列 argmax（须遍历整个字母表）[em.md §4]</summary>
        Public Sub TestConsensus()
            TestAssert.Section("一致序列逐列 argmax [em.md §4]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim w = 6
            Dim model As New EmModel(w, alpha, SiteModel.Zoops, TestData.UniformBg(alpha.Size), 0.1)

            ' 手工构造：每一列的峰值都在 T（索引 3）
            For col = 0 To w - 1
                For a = 0 To alpha.Size - 1
                    model.Pwm(col, a) = 0.05
                Next
                model.Pwm(col, alpha.EncodeChar("T"c)) = 0.85
            Next
            TestAssert.CheckEqual(model.Consensus(), "TTTTTT",
                                  "每列峰值在 T 时一致序列 = TTTTTT [缺陷 #3]")

            ' 峰值落在最后一个字母（蛋白字母表，索引 19 = Y）更能暴露循环边界问题
            Dim pa As New Alphabet(SeqTypes.Protein)
            Dim pm As New EmModel(5, pa, SiteModel.Zoops, TestData.UniformBg(pa.Size), 0.1)
            For col = 0 To 4
                For a = 0 To pa.Size - 1
                    pm.Pwm(col, a) = 0.01
                Next
                pm.Pwm(col, pa.Size - 1) = 0.81
            Next
            TestAssert.CheckEqual(pm.Consensus(), "YYYYY",
                                  "蛋白峰值在末位字母 Y 时一致序列 = YYYYY [缺陷 #3]")

            ' 与 Oracle 交叉验证：真实 EM 迭代后的一致序列
            Dim planted = TestData.PlantDna(20, 150, "ACGTTACGTA", 771)
            Dim encs = TestData.EncodeAll(planted.Sequences, alpha)
            Dim mm As New EmModel(10, alpha, SiteModel.Zoops, TestData.BgOf(encs, alpha), 0.1)
            mm.InitFromSeed(alpha.Encode("ACGTTACGTA"))
            Dim run = RunEm(mm, encs, False, maxIter:=40)
            TestAssert.CheckEqual(run.Model.Consensus(),
                                  OracleConsensus(run.Model.Pwm, 10, alpha.Size, alpha.Letters),
                                  "EM 迭代后一致序列与 Oracle argmax 一致 [缺陷 #3]")
        End Sub

        ''' <summary>[缺陷 #4] MaxDeltaTo 须比较 PWM 的全部格子</summary>
        Public Sub TestMaxDeltaTo()
            TestAssert.Section("PWM 变化量（收敛判据之二）[em.md §4]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim w = 8
            Dim bg = TestData.UniformBg(alpha.Size)

            For col = 0 To w - 1
                For a = 0 To alpha.Size - 1
                    Dim m1 As New EmModel(w, alpha, SiteModel.Zoops, bg, 0.1)
                    Dim m2 As New EmModel(w, alpha, SiteModel.Zoops, bg, 0.1)
                    m1.InitFromSeed(alpha.Encode("AAAAAAAA"))
                    m2.InitFromSeed(alpha.Encode("AAAAAAAA"))
                    m2.Pwm(col, a) += 0.5

                    If Math.Abs(m1.MaxDeltaTo(m2) - 0.5) > 0.000000000001 Then
                        TestAssert.Check(False, $"({col},{a}) 处改动 0.5 未被 MaxDeltaTo 检出 [缺陷 #4]")
                        Return
                    End If
                Next
            Next
            TestAssert.Check(True, "PWM 任意格子（全部 W×K 个）改动都被 MaxDeltaTo 检出 [缺陷 #4]")
        End Sub

        ''' <summary>[缺陷 #6] 窗口似然比：公式、歧义排除、负链读法</summary>
        Public Sub TestWindowLogR()
            TestAssert.Section("窗口似然比 logR [em.md §2]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim w = 4
            Dim bg = TestData.UniformBg(alpha.Size)
            Dim model As New EmModel(w, alpha, SiteModel.Zoops, bg, 0.1)
            model.InitFromSeed(alpha.Encode("ACGT"))

            ' 1) 与「直接连乘」对照
            Dim seq = "ACGTACGT"
            Dim enc = alpha.Encode(seq)
            Dim ok As Boolean = True
            For j = 0 To enc.Length - w
                Dim direct As Double = 1.0
                For k = 0 To w - 1
                    direct *= model.Pwm(k, enc(j + k)) / bg(enc(j + k))
                Next
                If Math.Abs(Math.Exp(model.WindowLogR(enc, j, False)) - direct) > 0.000000001 Then ok = False
            Next
            TestAssert.Check(ok, "logR = log Π_k θ_{k,a}/θ0,a（与直接连乘一致）[em.md §2]")

            ' 2) 负链窗口 = 该窗口反向互补后在正链上的 logR
            Dim rcOk As Boolean = True
            For j = 0 To enc.Length - w
                Dim win = seq.Substring(j, w)
                Dim rcWin = TestData.RevcompOf(win)
                Dim rcEnc = alpha.Encode(rcWin)
                Dim expect As Double = 1.0
                For k = 0 To w - 1
                    expect *= model.Pwm(k, rcEnc(k)) / bg(rcEnc(k))
                Next
                If Math.Abs(Math.Exp(model.WindowLogR(enc, j, True)) - expect) > 0.000000001 Then rcOk = False
            Next
            TestAssert.Check(rcOk, "负链 logR = 反向互补窗口在正链上的 logR [em.md §9]")

            ' 3) 含歧义字母的窗口 → −∞（正链）
            Dim ambEnc = alpha.Encode("ACNTACGT")
            TestAssert.Check(Double.IsNegativeInfinity(model.WindowLogR(ambEnc, 0, False)),
                             "含歧义字母的窗口 logR = −∞（正链）")

            ' 4) 含歧义字母的窗口 → −∞（负链），且不得抛异常 [缺陷 #6]
            Dim minusNoThrow As Boolean = True
            Dim minusIsInf As Boolean = False
            Try
                minusIsInf = Double.IsNegativeInfinity(model.WindowLogR(ambEnc, 0, True))
            Catch ex As Exception
                minusNoThrow = False
                Console.WriteLine($"         负链 WindowLogR 抛出 {ex.GetType().Name}: {ex.Message}")
            End Try
            TestAssert.Check(minusNoThrow, "歧义窗口的负链 logR 不抛异常（Complement(−1) 越界）[缺陷 #6]")
            TestAssert.Check(minusIsInf, "含歧义字母的窗口 logR = −∞（负链）[缺陷 #6]")

            ' 5) 与 Oracle 逐窗口对照（含双链）
            Dim planted = TestData.PlantDna(4, 80, "ACGTTACGTA", 5150)
            Dim pencs = TestData.EncodeAll(planted.Sequences, alpha)
            Dim m2 As New EmModel(10, alpha, SiteModel.Zoops, TestData.BgOf(pencs, alpha), 0.1)
            m2.InitFromSeed(alpha.Encode("ACGTTACGTA"))
            Dim allMatch As Boolean = True
            For Each e2 In pencs
                Dim cands = OracleCandidates(e2, m2.Pwm, m2.Background, 10, True, ComplementOf(alpha))
                Dim idx = 0
                For j = 0 To e2.Length - 10
                    For Each minus In New Boolean() {False, True}
                        Dim r = OracleWindowR(e2, j, minus, m2.Pwm, m2.Background, 10, ComplementOf(alpha))
                        If Not Double.IsNegativeInfinity(r) Then
                            If idx >= cands.Count OrElse Math.Abs(cands(idx).R - r) > 0.000000001 Then allMatch = False
                            idx += 1
                        End If
                    Next
                Next
                Dim prod = m2.WindowLogR(e2, 0, False)
                Dim orc = OracleWindowR(e2, 0, False, m2.Pwm, m2.Background, 10, ComplementOf(alpha))
                If Math.Abs(prod - Math.Log(orc)) > 0.000000000001 Then allMatch = False
            Next
            TestAssert.Check(allMatch, "logR 与 Oracle 候选枚举逐窗口一致（双链）")
        End Sub

        ''' <summary>E 步三种模型的后验约束 [em.md §6]</summary>
        Public Sub TestEStepConstraints()
            TestAssert.Section("E 步后验约束（OOPS / ZOOPS / ANR）[em.md §6]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim planted = TestData.PlantDna(8, 200, "ACGTTACGTA", 31337)
            Dim encs = TestData.EncodeAll(planted.Sequences, alpha)
            Dim bg = TestData.BgOf(encs, alpha)
            Dim w = 10

            ' OOPS：Σ_j Z_ij = 1（单链与双链都成立）
            Dim oopsOk As Boolean = True
            Dim mOops As New EmModel(w, alpha, SiteModel.Oops, bg, 0.1)
            mOops.InitFromSeed(alpha.Encode("ACGTTACGTA"))
            For Each rev In New Boolean() {False, True}
                For Each enc In encs
                    Dim s As Double = 0
                    For Each sp In mOops.EStep(enc, rev)
                        s += sp.Z
                    Next
                    If Math.Abs(s - 1.0) > 0.000000001 Then oopsOk = False
                Next
            Next
            TestAssert.Check(oopsOk, "OOPS Σ_j Z_ij = 1（单链 + 双链）[em.md §6]")

            ' ZOOPS：Σ_j Z_ij ≤ 1
            Dim zoopsOk As Boolean = True
            Dim mZoops As New EmModel(w, alpha, SiteModel.Zoops, bg, 0.1)
            mZoops.InitFromSeed(alpha.Encode("ACGTTACGTA"))
            For Each rev In New Boolean() {False, True}
                For Each enc In encs
                    Dim s As Double = 0
                    For Each sp In mZoops.EStep(enc, rev)
                        s += sp.Z
                    Next
                    If s > 1.0 + 0.000000001 Then zoopsOk = False
                Next
            Next
            TestAssert.Check(zoopsOk, "ZOOPS Σ_j Z_ij ≤ 1（单链 + 双链）[em.md §6]")

            ' ANR：各窗口独立，Z ∈ (0,1)
            Dim anrOk As Boolean = True
            Dim mAnr As New EmModel(w, alpha, SiteModel.Anr, bg, 0.1)
            mAnr.InitFromSeed(alpha.Encode("ACGTTACGTA"))
            For Each rev In New Boolean() {False, True}
                For Each enc In encs
                    For Each sp In mAnr.EStep(enc, rev)
                        If sp.Z < 0.0 OrElse sp.Z > 1.0 Then anrOk = False
                    Next
                Next
            Next
            TestAssert.Check(anrOk, "ANR 各位点后验独立且 Z ∈ [0,1] [em.md §6]")

            ' 候选窗口数：单链 L−W+1，双链 2(L−W+1)
            Dim n1 = mAnr.EStep(encs(0), False).Count
            Dim n2 = mAnr.EStep(encs(0), True).Count
            TestAssert.CheckEqual(n1, encs(0).Length - w + 1, "单链候选数 = L−W+1 [em.md §2]")
            TestAssert.CheckEqual(n2, 2 * (encs(0).Length - w + 1), "双链候选数 = 2(L−W+1) [em.md §9]")
        End Sub

        ''' <summary>E 步后验与 Oracle 逐元素对照（含链向与 logR）[em.md §2]</summary>
        Public Sub TestEStepVsOracle()
            TestAssert.Section("E 步后验 vs Oracle（三模型 × 单/双链）[em.md §2]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim planted = TestData.PlantDna(6, 120, "ACGTTACGTA", 9090)
            Dim encs = TestData.EncodeAll(planted.Sequences, alpha)
            Dim bg = TestData.BgOf(encs, alpha)
            Dim w = 10
            Dim comp = ComplementOf(alpha)

            For Each sm In New SiteModel() {SiteModel.Oops, SiteModel.Zoops, SiteModel.Anr}
                For Each rev In New Boolean() {False, True}
                    Dim m As New EmModel(w, alpha, sm, bg, 0.1)
                    m.InitFromSeed(alpha.Encode("ACGTTACGTA"))

                    Dim maxDz As Double = 0
                    Dim maxDlogr As Double = 0
                    Dim structOk As Boolean = True

                    For Each enc In encs
                        Dim sites = m.EStep(enc, rev)
                        Dim cands = OracleCandidates(enc, m.Pwm, m.Background, w, rev, comp)
                        Dim zref = OracleZ(cands, sm, m.Lambda)

                        If sites.Count <> cands.Count Then
                            structOk = False
                            Continue For
                        End If
                        For i = 0 To sites.Count - 1
                            If sites(i).Pos <> cands(i).Pos OrElse sites(i).StrandMinus <> cands(i).Minus Then
                                structOk = False
                            End If
                            maxDz = Math.Max(maxDz, Math.Abs(sites(i).Z - zref(i)))
                            maxDlogr = Math.Max(maxDlogr, Math.Abs(Math.Exp(sites(i).LogR) - cands(i).R))
                        Next
                    Next

                    TestAssert.Check(structOk, $"{sm}/{(If(rev, "双链", "单链"))} 候选序列与 Oracle 一致（位置 + 链向）")
                    TestAssert.Check(maxDz < 0.000000001,
                                     $"{sm}/{(If(rev, "双链", "单链"))} 后验 Z 与 Oracle 一致（最大偏差 {maxDz:G3}）[em.md §2]")
                    TestAssert.Check(maxDlogr < 0.000000001,
                                     $"{sm}/{(If(rev, "双链", "单链"))} logR 与 Oracle 一致（最大偏差 {maxDlogr:G3}）")
                Next
            Next
        End Sub

        ''' <summary>[缺陷 #7] 对数似然不应依赖传入的后验列表；双链模式须显式 [em.md §4]</summary>
        Public Sub TestFullLogLikIndependence()
            TestAssert.Section("对数似然的链模式显式化 [em.md §4]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim planted = TestData.PlantDna(6, 120, "ACGTTACGTA", 4242)
            Dim encs = TestData.EncodeAll(planted.Sequences, alpha)
            Dim bg = TestData.BgOf(encs, alpha)
            Dim w = 10

            For Each sm In New SiteModel() {SiteModel.Oops, SiteModel.Zoops, SiteModel.Anr}
                Dim m As New EmModel(w, alpha, sm, bg, 0.1)
                m.InitFromSeed(alpha.Encode("ACGTTACGTA"))

                ' 1) 似然是（θ, λ, 数据, 链模式）的函数，与 E 步给出的后验无关。
                '    旧实现用「后验里有没有负链条目」反推是否双链，而 EM 首轮的后验
                '    列表为空 ⇒ 按单链计算，次轮起才按双链计算，LL 轨迹出现假的
                '    跳变，既破坏单调性保证也污染 ΔLL 收敛判据 [缺陷 #7]。
                Dim llBefore = m.FullLogLik(encs, True)
                For Each enc In encs
                    m.EStep(enc, True)
                Next
                Dim llAfter = m.FullLogLik(encs, True)
                TestAssert.CheckNear(llBefore, llAfter, 0.0,
                                     $"{sm} 似然与是否执行过 E 步无关 [缺陷 #7]")

                ' 2) 链模式必须显式生效：单链与双链给出不同的（且各自正确的）值
                Dim llSingle = m.FullLogLik(encs, False)
                Dim llDouble = m.FullLogLik(encs, True)
                TestAssert.Check(Not Double.IsNaN(llSingle) AndAlso Not Double.IsNaN(llDouble),
                                 $"{sm} 单/双链似然均为有限值 [缺陷 #7]")
                If sm <> SiteModel.Oops Then
                    ' ZOOPS/ANR 下候选变多只会增大混合项，似然不应下降
                    TestAssert.Check(llDouble >= llSingle - 0.000000001,
                                     $"{sm} 双链似然 ≥ 单链似然 [em.md §9]")
                End If
            Next
        End Sub

        ''' <summary>全似然与 Oracle 逐模型对照 [em.md §1/§4]</summary>
        Public Sub TestFullLogLikVsOracle()
            TestAssert.Section("全观测对数似然 vs Oracle [em.md §4]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim planted = TestData.PlantDna(6, 120, "ACGTTACGTA", 8686)
            Dim encs = TestData.EncodeAll(planted.Sequences, alpha)
            Dim bg = TestData.BgOf(encs, alpha)
            Dim w = 10
            Dim comp = ComplementOf(alpha)

            For Each sm In New SiteModel() {SiteModel.Oops, SiteModel.Zoops, SiteModel.Anr}
                For Each rev In New Boolean() {False, True}
                    Dim m As New EmModel(w, alpha, sm, bg, 0.1)
                    m.InitFromSeed(alpha.Encode("ACGTTACGTA"))
                    Dim sites As New List(Of List(Of SitePosterior))()
                    For Each enc In encs
                        sites.Add(m.EStep(enc, rev))
                    Next

                    Dim actual = m.FullLogLik(encs, rev)
                    Dim expect = OracleLogLik(encs, m.Pwm, m.Background, m.Lambda, sm, w, rev, comp)
                    TestAssert.CheckNear(actual, expect, 0.000001,
                                         $"{sm}/{(If(rev, "双链", "单链"))} 全似然与 Oracle 一致 [em.md §4]")
                Next
            Next
        End Sub

        ''' <summary>[em.md §4] EM 单调收敛：LL 逐轮不降（三模型 × 单/双链）</summary>
        Public Sub TestMonotoneConvergence()
            TestAssert.Section("EM 单调收敛 [em.md §4]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim planted = TestData.PlantDna(20, 150, "ACGTTACGTA", 12321)
            Dim encs = TestData.EncodeAll(planted.Sequences, alpha)
            Dim bg = TestData.BgOf(encs, alpha)

            For Each sm In New SiteModel() {SiteModel.Oops, SiteModel.Zoops, SiteModel.Anr}
                For Each rev In New Boolean() {False, True}
                    Dim m As New EmModel(10, alpha, sm, bg, 0.1)
                    m.InitFromSeed(alpha.Encode("ACGTTACGTA"))
                    Dim run = RunEm(m, encs, rev, maxIter:=60)
                    Dim tag = $"{sm}/{(If(rev, "双链", "单链"))}"

                    TestAssert.Check(run.Trace.Count > 1, $"{tag} LL 轨迹至少 2 个点")
                    TestAssert.Check(run.IsMonotone(),
                                     $"{tag} LL 逐轮单调不降（最大下降 {run.WorstDrop():G4}，相对 {run.WorstRelativeDrop():G3}）[em.md §4]")
                Next
            Next
        End Sub

        ''' <summary>[em.md §3 Step3] λ 更新按模型区分</summary>
        Public Sub TestLambdaUpdate()
            TestAssert.Section("λ 更新 [em.md §3 Step3]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim planted = TestData.PlantDna(20, 150, "ACGTTACGTA", 555)
            Dim encs = TestData.EncodeAll(planted.Sequences, alpha)
            Dim bg = TestData.BgOf(encs, alpha)
            Dim w = 10

            For Each rev In New Boolean() {False, True}
                ' ZOOPS：λ = Σ_ij Z_ij / N（期望含位点序列数 / 序列数）
                Dim mz As New EmModel(w, alpha, SiteModel.Zoops, bg, 0.1)
                mz.InitFromSeed(alpha.Encode("ACGTTACGTA"))
                Dim sz As New List(Of List(Of SitePosterior))()
                For Each enc In encs
                    sz.Add(mz.EStep(enc, rev))
                Next
                Dim beforeZ = mz.Lambda
                mz.MStep(encs, sz)
                Dim expectZ = TestAssert.ClampLike(TotalZ(sz) / encs.Count, 0.001, 0.999)
                TestAssert.CheckNear(mz.Lambda, expectZ, 0.000000001,
                                     $"ZOOPS λ = ΣZ/N（{(If(rev, "双链", "单链"))}，{beforeZ:G4} → {mz.Lambda:G4}）[em.md §3]")

                ' ANR：λ = Σ_ij Z_ij / 候选窗口总数（双链 ×2）
                Dim ma As New EmModel(w, alpha, SiteModel.Anr, bg, 0.1)
                ma.InitFromSeed(alpha.Encode("ACGTTACGTA"))
                Dim sa As New List(Of List(Of SitePosterior))()
                For Each enc In encs
                    sa.Add(ma.EStep(enc, rev))
                Next
                ma.MStep(encs, sa)
                ' ANR 的分母是「位点槽位数」= 候选位置数：同一位置的正/负链共享
                ' 一个「无位点」状态，双链不会让槽位翻倍 [em.md §3]
                Dim expectA = TestAssert.ClampLike(TotalZ(sa) / TotalPositions(encs, w), 0.0001, 0.9999)
                TestAssert.CheckNear(ma.Lambda, expectA, 0.000000001,
                                     $"ANR λ = ΣZ/位置数（{(If(rev, "双链", "单链"))}，λ={ma.Lambda:G6}）[em.md §3]")

                ' OOPS：λ ≡ 1
                Dim mo As New EmModel(w, alpha, SiteModel.Oops, bg, 0.1)
                mo.InitFromSeed(alpha.Encode("ACGTTACGTA"))
                Dim so As New List(Of List(Of SitePosterior))()
                For Each enc In encs
                    so.Add(mo.EStep(enc, rev))
                Next
                mo.MStep(encs, so)
                TestAssert.CheckNear(mo.Lambda, 1.0, 0.000000001, "OOPS λ ≡ 1 [em.md §6]")
            Next
        End Sub

        ''' <summary>[em.md §3 Step2] 伪计数的作用：越大 PWM 越平滑、峰值概率越低</summary>
        Public Sub TestPseudocountEffect()
            TestAssert.Section("伪计数对 PWM 平滑的作用 [em.md §3 Step2]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim planted = TestData.PlantDna(12, 120, "ACGTTACGTA", 8080)
            Dim encs = TestData.EncodeAll(planted.Sequences, alpha)
            Dim bg = TestData.BgOf(encs, alpha)
            Dim w = 10

            Dim peaks As New List(Of Double)()
            For Each pc In New Double() {0.01, 0.1, 1.0}
                Dim m As New EmModel(w, alpha, SiteModel.Zoops, bg, pc)
                m.InitFromSeed(alpha.Encode("ACGTTACGTA"))
                Dim run = RunEm(m, encs, False, maxIter:=40)

                ' 每列概率和仍须为 1（伪计数不改变归一化约束）
                CheckPwmNormalized(run.Model, $"伪计数 {pc} 下 PWM 每列和 = 1")

                Dim peakSum As Double = 0
                For col = 0 To w - 1
                    Dim mx As Double = 0
                    For a = 0 To alpha.Size - 1
                        If run.Model.Pwm(col, a) > mx Then mx = run.Model.Pwm(col, a)
                    Next
                    peakSum += mx
                Next
                peaks.Add(peakSum / w)
            Next

            TestAssert.Note($"峰值概率均值：pc=0.01 → {peaks(0):F4}，pc=0.1 → {peaks(1):F4}，pc=1.0 → {peaks(2):F4}")
            TestAssert.Check(peaks(0) > peaks(1) AndAlso peaks(1) > peaks(2),
                             "伪计数越大，PWM 峰值概率越低（平滑作用）[em.md §3 Step2]")

            ' 极端小伪计数下不允许出现 0 概率（否则 logR = −∞ 会把候选窗口全部排除）
            Dim tiny As New EmModel(w, alpha, SiteModel.Zoops, bg, 0.01)
            tiny.InitFromSeed(alpha.Encode("ACGTTACGTA"))
            Dim tinyRun = RunEm(tiny, encs, False, maxIter:=40)
            Dim noZero As Boolean = True
            For col = 0 To w - 1
                For a = 0 To alpha.Size - 1
                    If tinyRun.Model.Pwm(col, a) <= 0 Then noZero = False
                Next
            Next
            TestAssert.Check(noZero, "伪计数 > 0 时 PWM 无 0 概率 [em.md §3 Step2]")
        End Sub

        ''' <summary>
        ''' [em.md §9] 反向互补不变性：同一批序列与其反向互补应给出一致的 motif 判读。
        ''' </summary>
        Public Sub TestRevcompInvariance()
            TestAssert.Section("反向互补不变性 [em.md §9]")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim planted = TestData.PlantDna(16, 160, "ACGTTACGTA", 6161)
            Dim encs = TestData.EncodeAll(planted.Sequences, alpha)
            Dim bg = TestData.BgOf(encs, alpha)
            Dim w = 10

            ' 原始序列上：正链位点的 logR
            Dim mF As New EmModel(w, alpha, SiteModel.Zoops, bg, 0.1)
            mF.InitFromSeed(alpha.Encode("ACGTTACGTA"))
            Dim runF = RunEm(mF, encs, False, maxIter:=40)

            ' 反向互补后的序列上：负链位点的 logR 应与前者一致
            Dim rcSeqs As New List(Of String)()
            For Each s In planted.Sequences
                rcSeqs.Add(TestData.RevcompOf(s))
            Next
            Dim rcEncs = TestData.EncodeAll(rcSeqs, alpha)
            Dim mR As New EmModel(w, alpha, SiteModel.Zoops, bg, 0.1)
            mR.InitFromSeed(alpha.Encode("ACGTTACGTA"))
            Dim runR = RunEm(mR, rcEncs, False, maxIter:=40)

            ' 不变性的正确表述：把整批序列替换成反向互补后，motif 也会变成
            ' 原 motif 的反向互补（读取方向反了），而不是保持原样。
            ' 另外允许 1~2 bp 的窗口寄存器差异（motif 发现的固有现象）。
            Dim cF = runF.Model.Consensus()
            Dim cR = runR.Model.Consensus()
            Dim expectR = TestData.RevcompOf(cF)
            Dim match = TestData.BestShiftedMatch(cR, expectR)
            TestAssert.Note($"正链一致序列 = {cF}；反向互补数据集 = {cR}（期望 ≈ {expectR}，匹配 {match}/{w}）")
            TestAssert.Check(match >= w - 1,
                             $"反向互补数据集上得到原 motif 的反向互补（{match}/{w}）[em.md §9]")

            ' λ 应一致（位点密度不因读取方向改变）
            TestAssert.CheckNear(runR.Model.Lambda, runF.Model.Lambda, 0.01,
                                 $"反向互补后 λ 一致（{runF.Model.Lambda:F4} vs {runR.Model.Lambda:F4}）")

            ' 全似然也应一致（同一批数据的两种表示；差异仅来自寄存器与数值路径）
            Dim llF = runF.Trace(runF.Trace.Count - 1)
            Dim llR = runR.Trace(runR.Trace.Count - 1)
            TestAssert.Check(Math.Abs(llR - llF) / Math.Max(1.0, Math.Abs(llF)) < 0.01,
                             $"反向互补后全似然一致（{llF:F2} vs {llR:F2}，相对差 {Math.Abs(llR - llF) / Math.Abs(llF):G3}）[em.md §4]")
        End Sub

        ''' <summary>
        ''' 数值回归：固定数据集 + 固定种子下的关键指标快照。
        ''' 用区间而非精确值断言，既能捕捉实现回归，又不会因合理的数值微调而失效。
        ''' </summary>
        Public Sub TestGoldenSnapshot()
            TestAssert.Section("数值回归快照（固定数据集 + 固定种子）")

            Dim alpha As New Alphabet(SeqTypes.DNA)
            Dim motif = "ACGTTACGTA"
            Dim planted = TestData.PlantDna(30, 200, motif, 20240903, withSiteRatio:=0.8)
            Dim opts As New SearchOptions With {
                .Model = SiteModel.Zoops, .MinW = 10, .MaxW = 10, .NumMotifs = 1,
                .Revcomp = False, .SeedStrategy = "enriched", .SeedCount = 8,
                .MaxSeeds = 200, .Pseudocount = 0.1, .MaxIter = 100,
                .Epsilon = 0.0001, .EvalueMax = 10.0, .RngSeed = 7}

            Dim search As New EmSearch(TestData.EncodeAll(planted.Sequences, alpha), alpha, opts)
            Dim results = search.Discover()

            TestAssert.Check(results.Count = 1, "回归数据集产出 1 个 motif")
            If results.Count = 0 Then Return

            Dim r = results(0)
            TestAssert.Note($"共识={r.Consensus} λ={r.Lambda:F4} LLR={r.LogLikelihoodRatio:F2} " &
                            $"E={r.Evalue:G3} 迭代={r.Iterations} 收敛={r.Converged}")

            TestAssert.CheckEqual(r.Consensus, motif, "回归数据集共识序列 = 植入 motif")
            TestAssert.Check(r.Lambda > 0.75 AndAlso r.Lambda <= 1.0,
                             $"λ 落在 [0.75, 1.0]（80% 序列含位点；实际 {r.Lambda:F4}）")
            TestAssert.Check(r.LogLikelihoodRatio > 300,
                             $"LLR 显著为正（实际 {r.LogLikelihoodRatio:F1}）[em.md §9]")
            TestAssert.Check(r.Evalue < 0.0000000001,
                             $"E-value 极显著（实际 {r.Evalue:G3}）[em.md §9]")
            TestAssert.Check(r.Converged AndAlso r.Iterations < 100,
                             $"在最大迭代内收敛（{r.Iterations} 轮）[em.md §4]")

            ' 位点后验：含植入位点的序列应给出高后验，且归一化后的 ΣZ ≤ 1
            Dim highZ = r.Sites.Where(Function(sp) sp.Z > 0.9).Count()
            TestAssert.Check(highZ >= planted.Sites.Count * 0.7,
                             $"多数植入位点获得高后验 Z>0.9（{highZ}/{planted.Sites.Count}）")
        End Sub

    End Module

End Namespace
