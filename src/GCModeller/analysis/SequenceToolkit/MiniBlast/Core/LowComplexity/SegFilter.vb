' ============================================================================
' LowComplexity.vb — 低复杂度区域过滤（DUST / SEG）
' ----------------------------------------------------------------------------
' DUST（Morgulis et al. 2006, 对称版）：
'   窗口 W=64、word=3，窗口得分 S = Σ_v C_v(C_v-1)/2 / (W-1)，
'   阈值 = level/10（默认 level=20 → 2.0，对应 blast -dust 20 64 1）。
'   超阈值窗口内出现次数 ≥2 的 3-mer 被标记，最终合并成遮蔽区间。
'
' SEG（Wootton & Federhen 1993，简化实现）：
'   窗口 W=12，香农熵 H = -Σ p·log2(p)（bits），
'   触发阈值 K1=2.2，延伸终止 K2=2.5（对应 -seg 12 2.2 2.5）。
'
' 输出：遮蔽掩码 Boolean()（soft masking：仅排除种子，不排除延伸打分）
' ============================================================================

Namespace Core

    Public Module SegFilter

        ''' <summary>SEG 掩码（窗口熵简化实现）</summary>
        ''' <param name="codes">蛋白编码（AaAlphabet，仅统计标准 20 氨基酸）</param>
        Public Function Mask(codes As Int32(), window As Integer, k1 As Double, k2 As Double) As Boolean()
            Dim n = codes.Length
            Dim mask1(n - 1) As Boolean
            If n < window Then Return mask1

            Dim log2 = Math.Log(2.0)

            For x As Integer = 0 To n - window
                Dim h = WindowEntropyBits(codes, x, window, log2)
                If h < k1 Then
                    ' 标记窗口并按 K2 阈值向两侧延伸
                    For p As Integer = x To x + window - 1
                        mask1(p) = True
                    Next
                    ' 向右延伸
                    Dim endPos = x + window
                    Do While endPos < n
                        Dim w2 = endPos - x + 1
                        If WindowEntropyBits(codes, x, w2, log2) >= k2 Then Exit Do
                        mask1(endPos) = True
                        endPos += 1
                    Loop
                    ' 向左延伸
                    Dim startPos = x - 1
                    Do While startPos >= 0
                        Dim w2 = endPos - startPos
                        If WindowEntropyBits(codes, startPos, w2, log2) >= k2 Then Exit Do
                        mask1(startPos) = True
                        startPos -= 1
                    Loop
                End If
            Next

            Return mask1
        End Function

        ''' <summary>窗口香农熵（bits，仅统计标准 20 氨基酸）</summary>
        Private Function WindowEntropyBits(codes As Int32(), start As Integer, w As Integer, log2 As Double) As Double
            Dim freq(19) As Integer
            Dim total As Integer = 0
            For i As Integer = start To start + w - 1
                If codes(i) < 20 Then
                    freq(codes(i)) += 1
                    total += 1
                End If
            Next
            If total = 0 Then Return 20.0   ' 无标准残基：视为高复杂度
            Dim h As Double = 0.0
            For i As Integer = 0 To 19
                If freq(i) > 0 Then
                    Dim p = freq(i) / CDbl(total)
                    h -= p * Math.Log(p) / log2
                End If
            Next
            Return h
        End Function

    End Module

End Namespace
