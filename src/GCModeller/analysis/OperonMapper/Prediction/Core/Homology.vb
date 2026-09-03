' ============================================================================
' Homology.vb — 比较基因组学信号 [operon.md §2]
' ----------------------------------------------------------------------------
' 1. 系统发育条形码 [§2.4 Bergman]：基因 g 的 0/1 向量（R 个参考基因组中
'    是否有同源）。相邻对 Hamming 距离 h → 二项 LLR：
'    LLR = log Binom(h; R, p_in) − log Binom(h; R, p_out)
'    （默认 p_in=0.15 / p_out=0.45，Bergman 拟合值的近似，文档化）
' 2. 保守基因对 [§2.5 Ermolaeva]：相邻对 (a,b) 在参考基因组 g 中同源物
'    相邻、同序、同相对方向 → 保守计数 n；LLR = log Binom(n;R,0.35) −
'    log Binom(n;R,0.05)。特异性 98%，灵敏覆盖有限 → 权重最高。
' 3. PCBBH [§2.1 Overbeek]：两基因在参考基因组中的最佳同源物相邻的参考数
'    （信息性计数，无 LLR；严谨的 BBH 需双向打分，此处为简化计数）。
' ============================================================================

Namespace OperonPredictor.Core

    Public Class HomologySignals

        Private ReadOnly _homology As Dictionary(Of String, Dictionary(Of String, Tuple(Of String, Double)))
        Private ReadOnly _refGenes As Dictionary(Of String, List(Of Gene))
        Private ReadOnly _refContigOf As Dictionary(Of String, String)   ' subject gene → refId
        Private _refSorted As Dictionary(Of String, List(Of Gene))       ' refId → 排序基因
        Private ReadOnly _refIdx As Dictionary(Of String, Dictionary(Of String, Int32))
        Public ReadOnly RefIds As List(Of String)

        Public Sub New(homology As Dictionary(Of String, Dictionary(Of String, Tuple(Of String, Double))),
                       refGenes As Dictionary(Of String, List(Of Gene)))
            _homology = homology
            _refGenes = refGenes
            RefIds = refGenes.Keys.OrderBy(Function(x) x).ToList()
            _refSorted = New Dictionary(Of String, List(Of Gene))()
            _refIdx = New Dictionary(Of String, Dictionary(Of String, Int32))()
            For Each kv In refGenes
                Dim gl = New List(Of Gene)(kv.Value)
                gl.Sort(Function(a, b) a.StartMin.CompareTo(b.StartMin))
                _refSorted(kv.Key) = gl
                Dim idxMap As New Dictionary(Of String, Int32)()
                For i = 0 To gl.Count - 1
                    idxMap(gl(i).Id) = i
                Next
                _refIdx(kv.Key) = idxMap
            Next
            ' subject gene → refId 索引；subject gene → 排序位置索引（避免 O(n) 查找）
            _refContigOf = New Dictionary(Of String, String)()
            For Each kv In refGenes
                For Each g In kv.Value
                    _refContigOf(g.Id) = kv.Key
                Next
            Next
        End Sub

        Private Function GeneIndex(refId As String, geneId As String) As Int32
            Dim idxMap As Dictionary(Of String, Int32) = Nothing
            If Not _refIdx.TryGetValue(refId, idxMap) Then Return -1
            Dim v As Int32 = -1
            If Not idxMap.TryGetValue(geneId, v) Then Return -1
            Return v
        End Function

        Public Function TotalRefs() As Int32
            Return RefIds.Count
        End Function

        ''' <summary>基因 g 在参考基因组 ref 中是否有同源</summary>
        Private Function HasHomolog(geneId As String, refId As String) As Boolean
            Dim perRef As Dictionary(Of String, Tuple(Of String, Double)) = Nothing
            If Not _homology.TryGetValue(geneId, perRef) Then Return False
            Return perRef.ContainsKey(refId)
        End Function

        ''' <summary>基因 g 在 ref 中的最佳同源 subject 基因名</summary>
        Private Function BestSubject(geneId As String, refId As String) As String
            Dim perRef As Dictionary(Of String, Tuple(Of String, Double)) = Nothing
            If Not _homology.TryGetValue(geneId, perRef) Then Return Nothing
            Dim t As Tuple(Of String, Double) = Nothing
            If Not perRef.TryGetValue(refId, t) Then Return Nothing
            Return t.Item1
        End Function

        ''' <summary>系统发育条形码 Hamming 距离（相邻对）与使用的参考数</summary>
        Public Sub BarcodeStats(geneA As String, geneB As String,
                                ByRef hamming As Int32, ByRef refsUsed As Int32)
            hamming = 0
            refsUsed = 0
            For Each refId In RefIds
                Dim ha = HasHomolog(geneA, refId)
                Dim hb = HasHomolog(geneB, refId)
                If ha OrElse hb Then
                    refsUsed += 1
                    If ha <> hb Then hamming += 1
                End If
            Next
        End Sub

        ''' <summary>
        ''' 保守基因对计数 [§2.5]：参考基因组中两基因的同源物相邻、同序、同相对方向。
        ''' </summary>
        Public Function ConservedPairCount(geneA As String, geneB As String) As Int32
            Dim n As Int32 = 0
            For Each refId In RefIds
                Dim subA = BestSubject(geneA, refId)
                Dim subB = BestSubject(geneB, refId)
                If subA Is Nothing OrElse subB Is Nothing Then Continue For
                If AreAdjacentSameOrder(_refSorted(refId), refId, subA, subB) Then n += 1
            Next
            Return n
        End Function

        ''' <summary>
        ''' PCBBH 计数 [§2.1]：两基因在参考基因组中的最佳同源物相邻（不要求同序）。
        ''' </summary>
        Public Function PcbbhCount(geneA As String, geneB As String) As Int32
            Dim n As Int32 = 0
            For Each refId In RefIds
                Dim subA = BestSubject(geneA, refId)
                Dim subB = BestSubject(geneB, refId)
                If subA Is Nothing OrElse subB Is Nothing Then Continue For
                If AreAdjacent(_refSorted(refId), refId, subA, subB) Then n += 1
            Next
            Return n
        End Function

        Private Function AreAdjacent(sortedGenes As List(Of Gene), refId As String, idA As String, idB As String) As Boolean
            Dim ia = GeneIndex(refId, idA)
            Dim ib = GeneIndex(refId, idB)
            If ia < 0 OrElse ib < 0 Then Return False
            If Math.Abs(ia - ib) <> 1 Then Return False
            ' 还要求基因组距离近（IGD ≤ 100）
            Dim left = sortedGenes(Math.Min(ia, ib))
            Dim right = sortedGenes(Math.Max(ia, ib))
            If left.Contig <> right.Contig Then Return False
            Dim gap = right.StartMin - left.EndMax - 1
            Return gap >= 0 AndAlso gap <= 100
        End Function

        Private Function AreAdjacentSameOrder(sortedGenes As List(Of Gene), refId As String, idA As String, idB As String) As Boolean
            Dim ia = GeneIndex(refId, idA)
            Dim ib = GeneIndex(refId, idB)
            If ia < 0 OrElse ib < 0 Then Return False
            If ib <> ia + 1 Then Return False      ' 同序：A 在前 B 在后
            Dim left = sortedGenes(ia)
            Dim right = sortedGenes(ib)
            If left.Contig <> right.Contig Then Return False
            If left.Strand <> right.Strand Then Return False   ' 同相对方向
            Dim gap = right.StartMin - left.EndMax - 1
            Return gap >= 0 AndAlso gap <= 100
        End Function

        ''' <summary>log 二项 pmf</summary>
        Public Shared Function LogBinomPmf(k As Int32, n As Int32, p As Double) As Double
            If p <= 0 Then Return If(k = 0, 0.0, Double.NegativeInfinity)
            If p >= 1 Then Return If(k = n, 0.0, Double.NegativeInfinity)
            Return LogGamma(n + 1) - LogGamma(k + 1) - LogGamma(n - k + 1) +
                   k * Math.Log(p) + (n - k) * Math.Log(1.0 - p)
        End Function

        Private Shared Function LogGamma(x As Double) As Double
            Dim g = 7.0
            Dim coef() As Double = {
                0.99999999999980993, 676.5203681218851, -1259.1392167224028,
                771.32342877765313, -176.61502916214059, 12.507343278686905,
                -0.13857109526572012, 0.0000099843695780195716, 0.00000015056327351493116}
            If x < 0.5 Then
                Return Math.Log(Math.PI / Math.Sin(Math.PI * x)) - LogGamma(1.0 - x)
            End If
            x -= 1.0
            Dim a = coef(0)
            Dim t = x + g + 0.5
            For i = 1 To 8
                a += coef(i) / (x + i)
            Next
            Return 0.5 * Math.Log(2.0 * Math.PI) + (x + 0.5) * Math.Log(t) - t + Math.Log(a)
        End Function

        ''' <summary>条形码二项 LLR（p_in vs p_out）</summary>
        Public Shared Function BarcodeLlr(hamming As Int32, refsUsed As Int32,
                                          pIn As Double, pOut As Double) As Double
            If refsUsed <= 0 Then Return 0.0
            Return LogBinomPmf(hamming, refsUsed, pIn) - LogBinomPmf(hamming, refsUsed, pOut)
        End Function

        ''' <summary>保守对二项 LLR（p_cons_in vs p_cons_out）</summary>
        Public Shared Function ConservedLlr(nCons As Int32, totalRefs As Int32,
                                            pIn As Double, pOut As Double) As Double
            If totalRefs <= 0 Then Return 0.0
            Return LogBinomPmf(nCons, totalRefs, pIn) - LogBinomPmf(nCons, totalRefs, pOut)
        End Function

    End Class

End Namespace
