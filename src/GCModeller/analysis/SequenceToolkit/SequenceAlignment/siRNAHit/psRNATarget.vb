Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman
Imports SMRUCC.genomics.SequenceModel.FASTA

Namespace siRNAHit

    ''' <summary>
    ''' psRNATarget 小RNA靶标预测算法实现。
    ''' 
    ''' 采用反向互补比对：miRNA 经 RNA 反向互补后作为正向 query 与 mRNA 做
    ''' Smith-Waterman 局部比对，再依据位置加权罚分体系计算期望值（Expectation）。
    ''' 期望值越低代表互补质量越好。
    ''' 支持 V1（2011）/ V2（2017，默认）两套 Schema。
    ''' </summary>
    Public Class psRNATarget : Implements miRNAMapper

        ''' <summary>Schema 版本：V1 种子区 2–8，V2 种子区 2–13（默认）。</summary>
        Public Enum Schema
            V1_2011
            V2_2017
        End Enum

        ' 罚分常数（参考 siRNA.md）
        Private Shared ReadOnly GAP_OPEN As Double = 2.0
        Private Shared ReadOnly GAP_EXT As Double = 0.5

        ' 默认最大期望值（V1=3.0, V2=5.0）
        Public Property MaxExpectation As Double = 5.0
        Public Property Version As Schema = Schema.V2_2017
        ' 最小比对长度（HSP size），psRNATarget 常规推荐 20
        Public Property MinHitLength As Integer = 17

        ' 位置权重表（miRNA 5'->3' 1-based，核心区高、末端低）
        '  第 1 位: 1
        '  第 2–13 位: 2（核心配对区）
        '  第 14–15 位: 1（相对宽松）
        '  第 16–21 位: 1（3' 末端宽松）
        Private Shared ReadOnly weightV2 As Double() = {
            1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2,
            1, 1, 1, 1, 1, 1
        }
        ' V1：种子区 2–8 权重高，其余宽松
        Private Shared ReadOnly weightV1 As Double() = {
            1, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1,
            1, 1, 1, 1, 1, 1
        }

        ''' <summary>基线（非种子区）错配罚分基数；G:U 半值。</summary>
        Private Const PEN_MISMATCH As Double = 1.0
        Private Const PEN_WOBBLE As Double = 0.5
        Private Const PEN_GAP As Double = 1.0

        ''' <summary>V2 种子区（2–13）内允许的最大非 G:U 错配数。</summary>
        Private Const SEED_MAX_MISMATCH_V2 As Integer = 2
        ''' <summary>V1 种子区（2–8）内允许的最大非 G:U 错配数（默认无硬性限制，设较大值）。</summary>
        Private Const SEED_MAX_MISMATCH_V1 As Integer = 99

        ''' <summary>靶标可及性评估器（UPE），默认关闭。</summary>
        Public Property Accessibility As IAccessibilityEvaluator = New DisabledAccessibility()

        ''' <summary>
        ''' 对单条 miRNA 在整条 mRNA 上做位置加权期望计算。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Score(mirna As String, mrna As String) As siRNAHit
            Dim revComp As String = mirna.ReverseComplementRNA()
            Dim hsp As LocalHSPMatch(Of Char) = RNASeqHelper.BestLocalHit(revComp, mrna)

            If hsp Is Nothing OrElse (hsp.toB - hsp.fromB + 1) < MinHitLength Then
                Return Nothing
            End If

            Dim hit As siRNAHit = siRNAHit.FromHSP(mirna, hsp, "psRNATarget")
            Dim expectation As Double = ComputeExpectation(mirna, hsp)
            ' 叠加靶标可及性（默认关闭 → 0）
            expectation += Accessibility.UPE(mrna, hsp.fromB, hsp.toB)
            hit.Expectation = expectation
            hit.TranslationInhibition = HasCenterMismatch(hsp)
            hit.Source = "psRNATarget"

            Return hit
        End Function

        ''' <summary>
        ''' 计算位置加权期望值：
        ''' E = Σ w_i · p_i + N_gap_open·P_open + N_gap_ext·P_ext
        ''' 其中 p_i 为配对罚分（WC=0, G:U=0.5, mismatch=1, gap=1）。
        ''' </summary>
        Friend Function ComputeExpectation(mirna As String, hsp As LocalHSPMatch(Of Char)) As Double
            Dim s1 As Char() = hsp.seq1
            Dim s2 As Char() = hsp.seq2
            Dim n As Integer = Math.Min(s1.Length, s2.Length)
            Dim weight As Double() = If(Version = Schema.V1_2011, weightV1, weightV2)

            Dim expectation As Double = 0.0
            Dim gapRun As Boolean = False

            For i As Integer = 0 To n - 1
                Dim pos As Integer = hsp.fromA + i
                Dim w As Double = PositionWeight(pos, weight)
                Dim t As RNASeqHelper.PairType = RNASeqHelper.ClassifyPair(s1(i), s2(i))
                Dim base As Double

                Select Case t
                    Case RNASeqHelper.PairType.WC : base = 0.0
                    Case RNASeqHelper.PairType.Wobble : base = PEN_WOBBLE
                    Case RNASeqHelper.PairType.Mismatch : base = PEN_MISMATCH
                    Case RNASeqHelper.PairType.Gap : base = PEN_GAP
                End Select

                expectation += w * base

                ' Gap 开放/延伸罚分
                If t = RNASeqHelper.PairType.Gap Then
                    If Not gapRun Then
                        expectation += GAP_OPEN
                        gapRun = True
                    Else
                        expectation += GAP_EXT
                    End If
                Else
                    gapRun = False
                End If
            Next

            Return expectation
        End Function

        ''' <summary>按 miRNA 位置返回权重（1-based，超出表长取末端宽松值）。</summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Shared Function PositionWeight(mirnaPos As Integer, weight As Double()) As Double
            If mirnaPos < 1 Then
                Return 1.0
            End If
            If mirnaPos > weight.Length Then
                Return 1.0
            End If
            Return weight(mirnaPos - 1)
        End Function

        ''' <summary>切割位点（miRNA 第 10–11 位）错配 → 翻译抑制候选。</summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Friend Function HasCenterMismatch(hsp As LocalHSPMatch(Of Char)) As Boolean
            Dim s1 As Char() = hsp.seq1
            Dim s2 As Char() = hsp.seq2
            Dim n As Integer = Math.Min(s1.Length, s2.Length)

            For i As Integer = 0 To n - 1
                Dim pos As Integer = hsp.fromA + i
                If pos = 10 OrElse pos = 11 Then
                    Dim t As RNASeqHelper.PairType = RNASeqHelper.ClassifyPair(s1(i), s2(i))
                    If t = RNASeqHelper.PairType.Mismatch Then
                        Return True
                    End If
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' 应用 psRNATarget 过滤：期望值达标，且种子区内非 G:U 错配不超过版本上限。
        ''' 种子区定义：V1 = 第 2–8 位，V2 = 第 2–13 位。
        ''' </summary>
        Public Function PassFilter(hit As siRNAHit, hsp As LocalHSPMatch(Of Char)) As Boolean
            If hit Is Nothing Then
                Return False
            End If
            If hit.Length < MinHitLength Then
                Return False
            End If
            If hit.Expectation > MaxExpectation Then
                Return False
            End If

            ' 统计种子区内的非 G:U 严格错配
            Dim seedMax As Integer = If(Version = Schema.V1_2011, SEED_MAX_MISMATCH_V1, SEED_MAX_MISMATCH_V2)
            Dim seedLo As Integer = 2
            Dim seedHi As Integer = If(Version = Schema.V1_2011, 8, 13)

            Dim s1 As Char() = hsp.seq1
            Dim s2 As Char() = hsp.seq2
            Dim n As Integer = Math.Min(s1.Length, s2.Length)
            Dim seedMis As Integer = 0

            For i As Integer = 0 To n - 1
                Dim pos As Integer = hsp.fromA + i
                If pos >= seedLo AndAlso pos <= seedHi Then
                    Dim t As RNASeqHelper.PairType = RNASeqHelper.ClassifyPair(s1(i), s2(i))
                    If t = RNASeqHelper.PairType.Mismatch Then
                        seedMis += 1
                    End If
                End If
            Next

            If seedMis > seedMax Then
                Return False
            End If

            Return True
        End Function

        ''' <summary>对一组候选 mRNA 执行预测，返回通过的命中集合。</summary>
        Public Iterator Function Run(mirna As FastaSeq, targets As IEnumerable(Of FastaSeq)) As IEnumerable(Of siRNAHit) Implements miRNAMapper.Run
            Dim query As String = mirna.Title.TrimStart(">"c)
            Dim mirnaSeq As String = mirna.SequenceData.ToUpper

            For Each t In targets
                Dim id As String = t.Title.TrimStart(">"c)
                Dim seq As String = t.SequenceData.ToUpper

                Dim revComp As String = mirnaSeq.ReverseComplementRNA()
                Dim hsp As LocalHSPMatch(Of Char) = RNASeqHelper.BestLocalHit(revComp, seq)
                Dim hit As siRNAHit = Score(mirnaSeq, seq)

                If PassFilter(hit, hsp) Then
                    hit.miRNA = query
                    hit.Target = id

                    Yield hit
                End If
            Next
        End Function
    End Class
End Namespace
