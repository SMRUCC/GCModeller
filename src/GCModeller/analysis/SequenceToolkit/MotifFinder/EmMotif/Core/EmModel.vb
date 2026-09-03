' ============================================================================
' EmModel.vb — EM motif 模型：PWM、E 步（三种位点分布模型）、M 步、似然
' ----------------------------------------------------------------------------
' [em.md §1 问题建模] 二分量混合：PWM θ（W×K 列独立）+ 背景 θ0（order-0）。
' 隐藏变量 Z_ij ∈ {0,1}（序列 i 位置 j 是 motif 起点）[em.md 隐藏变量]。
'
' [em.md §2 E 步] 窗口似然比 R_ij = Π_k θ_{k,S_i[j+k]}/θ_0[S_i[j+k]]（对数空间
'   累加防下溢；窗口含歧义字母 → R = −∞，Z = 0 排除）。
'
' 三种模型的后验（em.md §2 窗口级公式与 §6 约束的矛盾已按 Bailey & Elkan 1994
' 修正，详见 README）：
'   OOPS : Z_ij = R_ij / Σ_j' R_ij'                       （背景项相消，Σ_j Z = 1 精确）
'   ZOOPS: Z_ij = λR_ij / ((1−λ) + λΣ_j R_ij)             （序列级混合，Σ_j Z ≤ 1）
'          λ ← Σ_ij Z_ij / N（期望含位点序列数 / N）
'   ANR  : Z_ij = λR_ij / (λR_ij + (1−λ))                 （窗口独立，logistic 形式）
'          λ ← Σ_ij Z_ij / 窗口总数 [em.md §3 Step3]
'
' [em.md §3 M 步] n_{k,a} = Σ Z_ij·1[S=a]；θ_{k,a} = (n_{k,a}+b_a)/(Σn+Σb)。
'
' [em.md §4 收敛] 全观测似然：OOPS/ZOOPS 精确（bg(S)·[(1−λ)+λΣR] 形式），
'   ANR 为窗口独立式（与 Bailey & Elkan 1994 一致）。EM 每轮似然不降
'   （单调性自检：trace 逐轮比较）。
'
' 双链 [em.md §9]：revcomp 开启时每位置两个候选 (j, +) / (j, −)；
'   负链第 k 列读 enc(j+W−1−k) 的互补索引。
' ============================================================================

Imports System.Text
Imports Microsoft.VisualBasic.Math

Namespace EmMotif.Core

    ''' <summary>E 步输出：一个候选位点（位置 j、链、后验 Z、logR）</summary>
    Public Structure SitePosterior

        Public Pos As Int32
        Public StrandMinus As Boolean
        Public Z As Double
        Public LogR As Double

        Public Sub New(pos As Int32, strandMinus As Boolean, z As Double, logR As Double)
            Me.Pos = pos
            Me.StrandMinus = strandMinus
            Me.Z = z
            Me.LogR = logR
        End Sub

    End Structure

    Public Class EmModel

        Public ReadOnly W As Int32                 ' motif 宽度
        Public ReadOnly K As Int32                 ' 字母表大小
        Public ReadOnly AlphabetRef As Alphabet
        Public ReadOnly Model As SiteModel
        Public Pwm As Double(,)                    ' [k, a]
        Public Background() As Double              ' θ0（order-0）
        Public Lambda As Double                    ' 先验/混合权重
        Public Pseudocount As Double

        Public Sub New(w As Int32, alpha As Alphabet, model As SiteModel,
                       bg() As Double, pseudocount As Double)
            Me.W = w
            Me.AlphabetRef = alpha
            Me.K = alpha.Size
            Me.Model = model
            Me.Background = bg
            Me.Pseudocount = pseudocount
            ReDim Pwm(w - 1, K - 1)
            Lambda = If(model = SiteModel.Oops, 1.0, 0.5)
        End Sub

        ''' <summary>
        ''' 从种子 W-mer 构建 PWM：one-hot + 伪计数 [em.md §5]。
        ''' 结果：θ_{col,a} = (pc + 1[a = seed_col]) / (1 + K·pc)。
        '''
        ''' 注意：列下标一律用 col，字母表大小一律用 Me.K。VB 不区分大小写，
        ''' 若把列循环变量写成 K/k，会遮蔽字母表大小字段 Me.K，使内层
        ''' 「For a = 0 To K - 1」退化为按列号遍历（见 CODE_REVIEW 缺陷 #1）。
        ''' </summary>
        Public Sub InitFromSeed(seed() As Int32)
            For col As Integer = 0 To W - 1
                For a = 0 To Me.K - 1
                    Pwm(col, a) = Pseudocount
                Next
                Dim seedLetter = If(seed(col) >= 0 AndAlso seed(col) < Me.K, seed(col), -1)
                If seedLetter >= 0 Then Pwm(col, seedLetter) += 1.0

                Dim s As Double = 0
                For a = 0 To Me.K - 1
                    s += Pwm(col, a)
                Next
                For a = 0 To Me.K - 1
                    Pwm(col, a) /= s
                Next
            Next
        End Sub

        ''' <summary>
        ''' 单序列 E 步：返回全部候选位点的后验 Z（按模型归一化）。
        ''' revcomp：返回 (j,+) 与 (j,−) 双倍候选。
        ''' </summary>
        Public Function EStep(enc() As Int32, revcomp As Boolean) As List(Of SitePosterior)
            Dim L = enc.Length
            Dim nwin = L - W + 1
            Dim cands As New List(Of SitePosterior)()
            If nwin <= 0 Then Return cands

            ' 候选 logR
            Dim candPos As New List(Of Int32)()
            Dim candMinus As New List(Of Boolean)()
            Dim candLogR As New List(Of Double)()
            For j = 0 To nwin - 1
                Dim fwd = WindowLogR(enc, j, False)
                If Not Double.IsNegativeInfinity(fwd) Then
                    candPos.Add(j) : candMinus.Add(False) : candLogR.Add(fwd)
                End If
                If revcomp Then
                    Dim rv = WindowLogR(enc, j, True)
                    If Not Double.IsNegativeInfinity(rv) Then
                        candPos.Add(j) : candMinus.Add(True) : candLogR.Add(rv)
                    End If
                End If
            Next
            Dim n = candLogR.Count
            If n = 0 Then Return cands

            ' 数值稳定：对 logR 做 max 平移
            Dim mx = Double.NegativeInfinity
            For t = 0 To n - 1
                If candLogR(t) > mx Then mx = candLogR(t)
            Next
            Dim relR(n - 1) As Double      ' exp(logR − mx)
            Dim sumRel As Double = 0
            For t = 0 To n - 1
                relR(t) = Math.Exp(candLogR(t) - mx)
                sumRel += relR(t)
            Next

            Select Case Model
                Case SiteModel.Oops
                    ' Z = relR / Σ relR（softmax，背景与 λ 相消）
                    For t = 0 To n - 1
                        cands.Add(New SitePosterior(candPos(t), candMinus(t), relR(t) / sumRel, candLogR(t)))
                    Next
                Case SiteModel.Zoops
                    ' Z = λR / ((1−λ) + λΣR)；R_j = relR_j·e^mx，λΣR = e^mx·λ·ΣrelR
                    ' 分母 = (1−λ) + e^mx·λ·sumRel → log 空间组合
                    Dim logLamSumR = mx + Math.Log(Lambda) + Math.Log(sumRel)
                    Dim logOneMinus = log1p(-Lambda)
                    Dim logDen = LogAdd(logLamSumR, logOneMinus)
                    For t = 0 To n - 1
                        Dim logZ = Math.Log(Lambda) + candLogR(t) - logDen
                        cands.Add(New SitePosterior(candPos(t), candMinus(t), Math.Exp(logZ), candLogR(t)))
                    Next
                Case Else ' ANR
                    ' 单链：Z = λR/(λR + 1−λ)（logistic 形式）
                    ' 双链：同一位置的正/负链候选共享一个「无位点」状态
                    '       Z(j,±) = λR(j,±) / ((1−λ) + λR(j,+) + λR(j,−))
                    ' 这里按「位置」聚合，不依赖候选在列表中的成对排列顺序，
                    ' 避免正链候选缺失时把负链的 logR 错当成正链输出 [缺陷 #9]。
                    If revcomp Then
                        Dim sumByPos As New Dictionary(Of Int32, Double)()
                        For t = 0 To n - 1
                            Dim s As Double = 0
                            sumByPos.TryGetValue(candPos(t), s)
                            sumByPos(candPos(t)) = s + relR(t)
                        Next
                        For t = 0 To n - 1
                            ' λ·Σ_group R = e^mx·λ·Σ_group relR
                            Dim logLamSumR = mx + Math.Log(Lambda) + Math.Log(sumByPos(candPos(t)))
                            Dim logDen = LogAdd(logLamSumR, log1p(-Lambda))
                            Dim z = Math.Exp(Math.Log(Lambda) + candLogR(t) - logDen)
                            cands.Add(New SitePosterior(candPos(t), candMinus(t), z, candLogR(t)))
                        Next
                    Else
                        Dim logOdds0 = log1p(-Lambda) - Math.Log(Lambda)
                        For t = 0 To n - 1
                            Dim x = logOdds0 - candLogR(t)
                            Dim z As Double
                            If x > 700 Then
                                z = 0.0
                            ElseIf x < -700 Then
                                z = 1.0
                            Else
                                z = 1.0 / (1.0 + Math.Exp(x))
                            End If
                            cands.Add(New SitePosterior(candPos(t), candMinus(t), z, candLogR(t)))
                        Next
                    End If
            End Select
            Return cands
        End Function

        ''' <summary>窗口 logR：Σ_k log(θ_{k,a}/θ0,a)；含歧义字母 → −∞</summary>
        Public Function WindowLogR(enc() As Int32, j As Int32, minus As Boolean) As Double
            Dim lr As Double = 0
            For k As Integer = 0 To W - 1
                ' 必须先判原始编码是否为歧义（−1），再取互补：
                ' 否则负链分支会把 −1 传给 Complement 造成越界 [缺陷 #6]。
                ' 注意正/负链读的是同一组位置 {j..j+W−1}，因此两者的有效性判定一致。
                Dim raw As Int32 = enc(j + If(minus, W - 1 - k, k))
                If raw < 0 Then Return Double.NegativeInfinity

                Dim a As Int32 = If(minus, AlphabetRef.Complement(raw), raw)
                If a < 0 Then Return Double.NegativeInfinity

                Dim th = Pwm(k, a)
                Dim b0 = Background(a)
                If th <= 0 OrElse b0 <= 0 Then Return Double.NegativeInfinity
                lr += Math.Log(th / b0)
            Next
            Return lr
        End Function

        ''' <summary>
        ''' M 步：由全部序列的位点后验重估 PWM 与 λ [em.md §3]。
        ''' </summary>
        Public Sub MStep(encList As List(Of Int32()), sitesList As List(Of List(Of SitePosterior)),
                         Optional revcomp As Boolean = False)
            ' 加权计数 [em.md §3 Step1]：n_{col,a} = Σ Z_ij·1[S_i(j+col) = a]
            Dim counts(W - 1, Me.K - 1) As Double
            For si = 0 To encList.Count - 1
                Dim enc = encList(si)
                For Each sp In sitesList(si)
                    If sp.Z <= 0 Then Continue For
                    For col As Integer = 0 To W - 1
                        Dim raw As Int32 = enc(sp.Pos + If(sp.StrandMinus, W - 1 - col, col))
                        Dim a As Int32 = If(sp.StrandMinus, AlphabetRef.Complement(raw), raw)
                        If a >= 0 Then counts(col, a) += sp.Z
                    Next
                Next
            Next
            ' 伪计数 + 归一化 [em.md §3 Step2]
            ' 循环边界必须是字母表大小 Me.K，不能是列号 [缺陷 #2]
            For col As Integer = 0 To W - 1
                Dim s As Double = 0
                For a = 0 To Me.K - 1
                    counts(col, a) += Pseudocount
                Next
                For a = 0 To Me.K - 1
                    s += counts(col, a)
                Next
                For a = 0 To Me.K - 1
                    Pwm(col, a) = counts(col, a) / s
                Next
            Next
            ' λ 更新 [em.md §3 Step3]
            If Model = SiteModel.Zoops Then
                Dim totalZ As Double = 0
                For Each sites In sitesList
                    For Each sp In sites
                        totalZ += sp.Z
                    Next
                Next
                Lambda = Clamp(totalZ / Math.Max(1, sitesList.Count), 0.001, 0.999)
            ElseIf Model = SiteModel.Anr Then
                Dim totalZ As Double = 0
                For Each sites In sitesList
                    For Each sp In sites
                        totalZ += sp.Z
                    Next
                Next
                Dim nwinTotal As Double = 0
                For Each enc In encList
                    Dim nw = enc.Length - W + 1
                    If nw > 0 Then nwinTotal += nw
                Next
                If revcomp Then nwinTotal *= 2.0         ' 双链候选窗口总数
                Lambda = Clamp(totalZ / Math.Max(1.0, nwinTotal), 0.0001, 0.9999)
            Else
                Lambda = 1.0
            End If
        End Sub

        ''' <summary>
        ''' 全观测对数似然 [em.md §1/§4]：
        '''   OOPS : P(S_i) = bg(S_i)·(1/nCand)·Σ_c R_c
        '''   ZOOPS: P(S_i) = bg(S_i)·[(1−λ) + λ·Σ_c R_c]
        '''   ANR  : P(S_i) = bg(S_i)·Π_j [(1−λ) + λ·Σ_strands R(j,·)]
        '''
        ''' 似然只取决于模型参数（PWM θ、λ）与数据本身，与 E 步给出的后验无关，
        ''' 因此这里不再接收后验列表，链模式改由调用方显式传入。
        ''' 旧实现用「后验里有没有负链条目」反推是否双链，导致 EM 首轮
        ''' （后验列表为空）按单链计算、次轮起按双链计算，LL 轨迹出现假的跳变，
        ''' 既破坏了单调性保证也污染了 ΔLL 收敛判据 [缺陷 #7]。
        ''' </summary>
        Public Function FullLogLik(encList As List(Of Int32()), Optional revcomp As Boolean = False) As Double
            Dim ll As Double = 0

            For Each enc In encList
                ' 背景项：所有有效字母
                For Each a In enc
                    If a >= 0 Then ll += Math.Log(Background(a))
                Next

                Dim nwin = enc.Length - W + 1
                If nwin <= 0 Then Continue For

                ' 有效候选的 logR（j 升序；双链时同一位置正链在前、负链在后）
                Dim lrs As New List(Of Double)()
                Dim posOf As New List(Of Int32)()
                For j = 0 To nwin - 1
                    Dim f = WindowLogR(enc, j, False)
                    If Not Double.IsNegativeInfinity(f) Then
                        lrs.Add(f)
                        posOf.Add(j)
                    End If
                    If revcomp Then
                        Dim r = WindowLogR(enc, j, True)
                        If Not Double.IsNegativeInfinity(r) Then
                            lrs.Add(r)
                            posOf.Add(j)
                        End If
                    End If
                Next

                If lrs.Count = 0 Then
                    ' 该序列没有任何可用候选窗口：ZOOPS 下只能取「无位点」状态
                    If Model = SiteModel.Zoops Then ll += log1p(-Lambda)
                    Continue For
                End If

                ' 数值稳定：对 logR 做 max 平移
                Dim mx = Double.NegativeInfinity
                For Each v In lrs
                    If v > mx Then mx = v
                Next

                Select Case Model
                    Case SiteModel.Oops
                        ' Σ_c R_c，再乘位点位置的均匀先验 1/nCand
                        ' （nCand 只取决于歧义字符分布，与 θ 无关，不影响 EM 单调性）[缺陷 #14]
                        Dim sumR As Double = 0
                        For Each v In lrs
                            sumR += Math.Exp(v - mx)
                        Next
                        ll += mx + Math.Log(sumR) - Math.Log(lrs.Count)

                    Case SiteModel.Zoops
                        ' log[(1−λ) + λ·Σ_c R_c]
                        Dim sumR As Double = 0
                        For Each v In lrs
                            sumR += Math.Exp(v - mx)
                        Next
                        ll += LogAdd(mx + Math.Log(Lambda) + Math.Log(sumR), log1p(-Lambda))

                    Case Else ' ANR
                        ' 逐位置累乘；同一位置的正/负链共享一个「无位点」状态，
                        ' 与 EStep 的 ANR 分支语义一致
                        Dim sumByPos As New Dictionary(Of Int32, Double)()
                        For t = 0 To lrs.Count - 1
                            Dim s As Double = 0
                            sumByPos.TryGetValue(posOf(t), s)
                            sumByPos(posOf(t)) = s + Math.Exp(lrs(t) - mx)
                        Next
                        For Each kv In sumByPos
                            ll += LogAdd(mx + Math.Log(Lambda) + Math.Log(kv.Value), log1p(-Lambda))
                        Next
                End Select
            Next

            Return ll
        End Function

        ''' <summary>软 LLR 检验统计量 = 2·Σ Z_ij·logR_ij（χ² 近似用）</summary>
        Public Function SoftLlr(sitesList As List(Of List(Of SitePosterior))) As Double
            Dim s As Double = 0
            For Each sites In sitesList
                For Each sp In sites
                    If sp.Z > 0 AndAlso Not Double.IsNegativeInfinity(sp.LogR) Then
                        s += sp.Z * sp.LogR
                    End If
                Next
            Next
            Return 2.0 * s
        End Function

        ''' <summary>
        ''' 每列最大概率碱基 → 一致序列 [em.md §4 输出]。
        ''' argmax 必须遍历完整字母表（Me.K），不能只遍历到列号 [缺陷 #3]。
        ''' </summary>
        Public Function Consensus() As String
            Dim sb = New StringBuilder()
            For col As Integer = 0 To W - 1
                Dim bestA = 0
                For a = 1 To Me.K - 1
                    If Pwm(col, a) > Pwm(col, bestA) Then bestA = a
                Next
                sb.Append(AlphabetRef.Letters(bestA))
            Next
            Return sb.ToString()
        End Function

        ''' <summary>
        ''' PWM 与旧版的最大元素变化（收敛判据之二 [em.md §4]）。
        ''' 必须比较全部 W×K 个格子，不能只比较到列号 [缺陷 #4]。
        ''' </summary>
        Public Function MaxDeltaTo(other As EmModel) As Double
            Dim mx As Double = 0
            For col As Integer = 0 To W - 1
                For a = 0 To Me.K - 1
                    Dim d = Math.Abs(Pwm(col, a) - other.Pwm(col, a))
                    If d > mx Then mx = d
                Next
            Next
            Return mx
        End Function

        Public Function Clone() As EmModel
            Dim m As New EmModel(W, AlphabetRef, Model, Background, Pseudocount) With {.Lambda = Lambda}
            Array.Copy(Pwm, m.Pwm, Pwm.Length)
            Return m
        End Function

        ' ---- 工具 ----

        Private Shared Function LogAdd(a As Double, b As Double) As Double
            If Double.IsNegativeInfinity(a) Then Return b
            If Double.IsNegativeInfinity(b) Then Return a
            If a > b Then
                Return a + log1p(Math.Exp(b - a))
            End If
            Return b + log1p(Math.Exp(a - b))
        End Function

        Private Shared Function Clamp(v As Double, lo As Double, hi As Double) As Double
            If v < lo Then Return lo
            If v > hi Then Return hi
            Return v
        End Function

    End Class

End Namespace
