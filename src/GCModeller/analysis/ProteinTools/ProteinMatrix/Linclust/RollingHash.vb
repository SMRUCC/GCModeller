' Linclust 滚动哈希与最小 m 个 k-mer 选取
'
' 对缩减字母表编码后的序列,用 16 位滚动哈希(Rabin-Karp 风格)计算
' 每个 k-mer 的哈希值,并仅保留哈希值最小的 m 个 k-mer。
'
' 选哈希值最小的 m 个(而非随机抽取)保证同源序列会抽到同一批
' k-mer,从而能相遇。

Namespace SMRUCC.genomics.Model.MotifGraph.ProteinStructure.Linclust

    Public Module RollingHash

        ' 16 位无符号掩码
        Private Const Mask16 As UInteger = &HFFFFUI

        ' 随机化基(乘法散列),用于降低碰撞并增强混合性
        Private Const Seed As UInteger = 2654435761UI

        ''' <summary>
        ''' 单个 k-mer 的哈希结果:哈希值 + 在序列中的位置
        ''' </summary>
        Public Structure KmerHash
            Public Hash As UInteger   ' 16 位有效(低 16 位)
            Public Position As Integer

            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Public Sub New(h As UInteger, pos As Integer)
                Hash = h And Mask16
                Position = pos
            End Sub
        End Structure

        ''' <summary>
        ''' 计算单个 k-mer(缩减字母表字符,基数 13)的 16 位滚动哈希。
        ''' 使用多项式滚动哈希:F(h, c) = (h * base + code(c) * Seed) mod 2^16
        ''' 字母序号 code 取其在缩减字母表中的下标(0..12),未知字符映射到 13(通配)。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function HashKmer(kmer As String) As UInteger
            Dim h As UInteger = 0
            Dim code As Integer

            For Each c As Char In kmer
                code = AlphabetCode(c)
                ' 乘法散列混合,确保良好分布
                h = (h * AlphabetSize + CUInt(code)) * Seed
            Next

            Return h And Mask16
        End Function

        ''' <summary>
        ''' 将缩减字母映射为 0-based 序号
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function AlphabetCode(c As Char) As Integer
            Dim idx = Array.IndexOf(ReducedAlphabet.Letters, c)

            If idx >= 0 Then
                Return idx
            End If

            ' 通配符 / 未知字符:用基数之外的编号,使其仍参与哈希但不与标准字母冲突
            Return ReducedAlphabet.AlphabetSize
        End Function

        ''' <summary>
        ''' 用滚动方式计算序列上每个 k-mer 的哈希(依次增量更新)。
        ''' </summary>
        Private Function HashAll(encoded As String, k As Integer) As KmerHash()
            If encoded.Length < k Then
                Return New KmerHash(-1) {}
            End If

            Dim n = encoded.Length - k + 1
            Dim result = New KmerHash(n - 1) {}

            ' 第一个 k-mer
            Dim h As UInteger = 0
            For i As Integer = 0 To k - 1
                h = (h * ReducedAlphabet.AlphabetSize + CUInt(AlphabetCode(encoded(i)))) * Seed
            Next
            result(0) = New KmerHash(h, 0)

            ' 滚动更新:F_n = (F_{n-1} * base - code(prev) * base^k + code(new)) * Seed
            ' 其中 base^k 预计算(对基数 13 取模意义不大,这里直接用整数溢出的 2^32 环,
            ' 但需保持一致:采用 ((h - prevCode*power) * base + newCode) * Seed 形式)
            Dim power As UInteger = 1
            For i As Integer = 1 To k - 1
                power = power * ReducedAlphabet.AlphabetSize
            Next

            For i As Integer = 1 To n - 1
                Dim prevCode = CUInt(AlphabetCode(encoded(i - 1)))
                Dim newCode = CUInt(AlphabetCode(encoded(i + k - 1)))

                ' 去掉最左侧字符贡献
                Dim hNoPrev As UInteger = h - prevCode * power
                h = (hNoPrev * ReducedAlphabet.AlphabetSize + newCode) * Seed
                result(i) = New KmerHash(h, i)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 提取序列上哈希值最小的 m 个 k-mer。
        ''' 若 k-mer 总数不足 m,则返回全部。
        ''' </summary>
        ''' <param name="encoded">缩减字母表编码后的序列</param>
        ''' <param name="k">k-mer 长度</param>
        ''' <param name="m">保留的最小哈希 k-mer 个数</param>
        Public Function GetMinHashes(encoded As String, k As Integer, m As Integer) As KmerHash()
            If encoded Is Nothing OrElse encoded.Length < k Then
                Return New KmerHash() {}
            End If

            Dim all = HashAll(encoded, k)

            If all.Length <= m Then
                Return all
            End If

            ' 部分排序取最小的 m 个(避免全排序,O(n) 选择)
            ' 使用 OrderBy 取前 m 个(实现简洁;对 mN 规模足够)
            Return all _
                .OrderBy(Function(x) x.Hash) _
                .Take(m) _
                .ToArray
        End Function
    End Module
End Namespace
