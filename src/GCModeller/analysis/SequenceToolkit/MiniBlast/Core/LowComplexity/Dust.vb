
Namespace Core

    Public Module Dust

        ''' <summary>DUST 掩码</summary>
        ''' <param name="codes">核酸编码（NtAlphabet）</param>
        ''' <param name="level">0-100 尺度阈值（默认 20）</param>
        ''' <param name="window">窗口宽（默认 64）</param>
        Public Function Mask(codes As Int32(), level As Integer, window As Integer) As Boolean()
            Dim n = codes.Length
            Dim mask1(n - 1) As Boolean
            If n < 3 Then Return mask1

            Dim thresholdRaw = level / 10.0
            Dim counts As New Dictionary(Of Int32, Integer)()
            Dim w = Math.Min(window, n)

            ' 首窗口计数（3-mer key = c0*16 + c1*4 + c2；含歧义字符的 3-mer 视为唯一不计对）
            Dim keys(n - 3) As Int32
            For p As Integer = 0 To n - 3
                If codes(p) < 4 AndAlso codes(p + 1) < 4 AndAlso codes(p + 2) < 4 Then
                    keys(p) = codes(p) * 16 + codes(p + 1) * 4 + codes(p + 2)
                Else
                    keys(p) = -1
                End If
            Next

            Dim pairs As Long = 0   ' Σ C(C-1)/2 的增量维护
            Dim inWindow = Math.Min(w - 2, n - 2)

            For p As Integer = 0 To inWindow - 1
                AddWord(counts, keys(p), pairs)
            Next

            ' 滑窗（步长 1）
            For x As Integer = 0 To n - 3
                Dim score = pairs / Math.Max(1.0, CDbl(inWindow))
                If score > thresholdRaw Then
                    ' 标记窗口内出现 ≥2 次的 3-mer
                    For p As Integer = x To Math.Min(x + inWindow - 1, n - 3)
                        Dim k = keys(p)
                        If k >= 0 AndAlso counts.ContainsKey(k) AndAlso counts(k) >= 2 Then
                            mask1(p) = True : mask1(p + 1) = True : mask1(p + 2) = True
                        End If
                    Next
                End If
                ' 滑动：移除 x，加入 x + inWindow
                If x + inWindow - 1 <= n - 3 Then RemoveWord(counts, keys(x), pairs)
                If x + inWindow <= n - 3 Then AddWord(counts, keys(x + inWindow), pairs)
            Next

            Return mask1
        End Function

        Private Sub AddWord(counts As Dictionary(Of Int32, Integer), key As Int32, ByRef pairs As Long)
            If key < 0 Then Return
            Dim c As Integer = 0
            counts.TryGetValue(key, c)
            pairs += c
            counts(key) = c + 1
        End Sub

        Private Sub RemoveWord(counts As Dictionary(Of Int32, Integer), key As Int32, ByRef pairs As Long)
            If key < 0 Then Return
            Dim c As Integer = 0
            If counts.TryGetValue(key, c) Then
                pairs -= (c - 1)
                If c <= 1 Then counts.Remove(key) Else counts(key) = c - 1
            End If
        End Sub

    End Module

End Namespace
