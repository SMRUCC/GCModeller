' ============================================================================
' Integrator.vb — 多信号概率整合 + 贝叶斯 HMM [operon.md §2.4 / 引言]
' ----------------------------------------------------------------------------
' [operon.md 引言] "只有把它们在概率框架下组合，才能同时达到较高的灵敏度
'   和特异性（>85%）"——整合器：
'   1. 每对相邻基因计算各信号 LLR（log-odds，同操纵子 vs 边界）：
'        距离（UniOP 后验 logit 化）/ 条形码二项 / 保守对二项 /
'        终止子（强终止子 → 反证据）/ 启动子（边界证据）/ 功能同类别奖励
'   2. 联合 log-odds = Σ w_i·LLR_i（朴素贝叶斯组合，权重=信号可靠度）
'   3. HMM [§2.4 Bergman]：状态 = 相邻对"同操纵子/边界"，发射 = 联合 LLR，
'      转移带持续性参数 ρ（操纵子倾向延续）；Viterbi 全局解码 + 前向后向
'      后验。反链对硬边界（截断 HMM 链）。
' ============================================================================

Imports Microsoft.VisualBasic.Math

Namespace OperonPredictor.Core

    Public Class IntegrationOptions

        Public WPrior As Double = 1.0          ' 距离信号权重
        Public WBarcode As Double = 1.0        ' 条形码权重
        Public WConserved As Double = 1.2      ' 保守对权重（特异性 98%）
        Public WTerminator As Double = 0.8     ' 终止子权重
        Public WPromoter As Double = 0.4       ' 启动子权重
        Public WFunction As Double = 0.3       ' 功能权重
        Public PBarcodeIn As Double = 0.15
        Public PBarcodeOut As Double = 0.45
        Public PConservedIn As Double = 0.35
        Public PConservedOut As Double = 0.05
        Public Persistence As Double = 0.5     ' HMM 持续性 ρ
        Public LlrClamp As Double = 8.0        ' 单信号 LLR 截断

    End Class

    ''' <summary>单对的多维信号打分</summary>
    Public Class PairSignals

        Public UniopPosterior As Double = -1     ' -1 = 不可用
        Public BarcodeHamming As Int32 = -1
        Public BarcodeRefs As Int32 = 0
        Public BarcodeLlr As Double = 0
        Public ConservedCount As Int32 = -1
        Public ConservedLlr As Double = 0
        Public PcbbhCount As Int32 = -1
        Public TerminatorStrength As Double = -1
        Public PromoterStrength As Double = -1
        Public FunctionalMatch As Boolean? = Nothing
        Public LlrDistance As Double = 0
        Public LlrTerminator As Double = 0
        Public LlrPromoter As Double = 0
        Public LlrFunction As Double = 0
        Public CombinedLlr As Double = 0
        Public CombinedPosterior As Double = 0.5
        Public HmmPosterior As Double = 0.5
        Public ViterbiState As Boolean = False

    End Class

    Public Class Integrator

        Public Shared Function Sigmoid(x As Double) As Double
            If x > 35 Then Return 1.0
            If x < -35 Then Return 0.0
            Return 1.0 / (1.0 + Math.Exp(-x))
        End Function

        Public Shared Function Logit(p As Double) As Double
            p = Math.Max(0.0001, Math.Min(0.9999, p))
            Return Math.Log(p / (1.0 - p))
        End Function

        Private Shared Function ClampLlr(v As Double, lim As Double) As Double
            Return Math.Max(-lim, Math.Min(lim, v))
        End Function

        ''' <summary>计算单对全部信号 LLR 与联合 log-odds</summary>
        Public Shared Function ScorePair(s As PairSignals, qPrior As Double,
                                         opts As IntegrationOptions) As PairSignals
            ' 距离：UniOP 后验 logit
            If s.UniopPosterior >= 0 Then
                s.LlrDistance = ClampLlr(Logit(s.UniopPosterior), opts.LlrClamp)
            Else
                s.LlrDistance = 0
            End If
            ' 终止子：T=0 → +0.25w（弱正），T=1 → −w（强反证据）
            If s.TerminatorStrength >= 0 Then
                s.LlrTerminator = opts.WTerminator * (0.25 * (1.0 - s.TerminatorStrength) - s.TerminatorStrength)
            Else
                s.LlrTerminator = 0
            End If
            ' 启动子：边界证据
            If s.PromoterStrength >= 0 Then
                s.LlrPromoter = -opts.WPromoter * s.PromoterStrength
            Else
                s.LlrPromoter = 0
            End If
            ' 功能：同类别奖励
            If s.FunctionalMatch.HasValue AndAlso s.FunctionalMatch.Value Then
                s.LlrFunction = opts.WFunction
            Else
                s.LlrFunction = 0
            End If

            s.CombinedLlr = opts.WPrior * s.LlrDistance + opts.WBarcode * s.BarcodeLlr +
                            opts.WConserved * s.ConservedLlr + s.LlrTerminator +
                            s.LlrPromoter + s.LlrFunction
            s.CombinedPosterior = Sigmoid(s.CombinedLlr)
            Return s
        End Function

        ''' <summary>
        ''' 对同链对序列（连续 run）跑 HMM：Viterbi + 前向后向。
        ''' 返回每对 Viterbi 状态（True=同操纵子）与 FB 后验（就地写入）。
        ''' </summary>
        Public Shared Sub RunHmm(runs As List(Of List(Of PairSignals)), qPrior As Double,
                                 opts As IntegrationOptions)
            Dim logQ = Math.Log(Math.Max(0.000001, qPrior))
            Dim log1Q = Math.Log(Math.Max(0.000001, 1.0 - qPrior))
            Dim pOo = Math.Min(0.999, qPrior + (1.0 - qPrior) * opts.Persistence)
            Dim pBo = Math.Max(0.001, qPrior * (1.0 - opts.Persistence))
            Dim lOo = Math.Log(pOo)
            Dim lOb = Math.Log(1.0 - pOo)
            Dim lBo = Math.Log(pBo)
            Dim lBb = Math.Log(1.0 - pBo)

            For Each run In runs
                Dim n = run.Count
                If n = 0 Then Continue For
                ' 发射（log 空间）
                Dim em(n - 1) As Double
                For i = 0 To n - 1
                    em(i) = run(i).CombinedLlr
                Next
                ' ---- Viterbi ----
                Dim d1 = em(0) + logQ
                Dim d0 = log1Q
                Dim backOp(n - 1) As Boolean
                Dim backBnd(n - 1) As Boolean
                For i = 1 To n - 1
                    Dim c1Op = d1 + lOo
                    Dim c1Bo = d0 + lBo
                    backOp(i) = c1Op >= c1Bo
                    Dim c1 = em(i) + Math.Max(c1Op, c1Bo)
                    Dim c0Op = d1 + lOb
                    Dim c0Bnd = d0 + lBb
                    backBnd(i) = c0Op >= c0Bnd
                    Dim c0 = Math.Max(c0Op, c0Bnd)
                    d1 = c1
                    d0 = c0
                Next
                Dim states(n - 1) As Boolean
                states(n - 1) = d1 >= d0
                For i = n - 1 To 1 Step -1
                    If states(i) Then
                        states(i - 1) = backOp(i)
                    Else
                        states(i - 1) = backBnd(i)
                    End If
                Next
                ' ---- 前向后向（log 空间 logaddexp）----
                Dim a1(n - 1) As Double
                Dim a0(n - 1) As Double
                a1(0) = em(0) + logQ
                a0(0) = log1Q
                For i = 1 To n - 1
                    a1(i) = em(i) + LogAddExp(a1(i - 1) + lOo, a0(i - 1) + lBo)
                    a0(i) = LogAddExp(a1(i - 1) + lOb, a0(i - 1) + lBb)
                Next
                Dim b1(n - 1) As Double
                Dim b0(n - 1) As Double
                For i = n - 2 To 0 Step -1
                    b1(i) = LogAddExp(lOo + em(i + 1) + b1(i + 1), lOb + b0(i + 1))
                    b0(i) = LogAddExp(lBo + em(i + 1) + b1(i + 1), lBb + b0(i + 1))
                Next
                For i = 0 To n - 1
                    Dim pOp = a1(i) + b1(i)
                    Dim pBnd = a0(i) + b0(i)
                    run(i).HmmPosterior = Sigmoid(pOp - pBnd)   ' log 差 → sigmoid
                    run(i).ViterbiState = states(i)
                Next
            Next
        End Sub

        Private Shared Function LogAddExp(a As Double, b As Double) As Double
            If Double.IsNegativeInfinity(a) Then Return b
            If Double.IsNegativeInfinity(b) Then Return a
            If a > b Then
                Return a + log1p(Math.Exp(b - a))
            End If
            Return b + log1p(Math.Exp(a - b))
        End Function

    End Class

End Namespace
