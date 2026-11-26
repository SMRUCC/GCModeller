Namespace RandomSources

    ''' <summary>
    ''' 种子派生工具：把字符串（基因名、扰动名、样本名）稳定地映射为随机数种子。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 用途是"每个任务一个可复现的子流"：同一个基因名在任何进程、任何线程、任何机器上
    ''' 都得到同一个 32 位种子，因此并行执行顺序不会影响结果。
    ''' </para>
    ''' <para>
    ''' 采用 FNV-1a 32 位哈希：实现简单、无分配、分布足够均匀；
    ''' <b>不是密码学安全哈希</b>，不要用于安全用途。
    ''' </para>
    ''' </remarks>
    Public Module RandomSeeds

        Private Const FnvOffsetBasis As Long = 2166136261L
        Private Const FnvPrime As Long = 16777619L
        Private Const UInt32Mask As Long = &HFFFFFFFFL

        ''' <summary>
        ''' 由字符串计算 32 位种子（FNV-1a）
        ''' </summary>
        ''' <param name="name">任意文本；<c>Nothing</c> 视为空串</param>
        ''' <returns>落在 <c>[0, 2^32)</c> 的种子；结果不会为 0（0 会被哈希为 1）</returns>
        Public Function FromString(name As String) As Long
            Dim hash As Long = FnvOffsetBasis

            If Not String.IsNullOrEmpty(name) Then
                For Each ch As Char In name
                    hash = hash Xor AscW(ch)
                    hash = (hash * FnvPrime) And UInt32Mask
                Next
            End If

            If hash = 0 Then Return 1L

            Return hash
        End Function

        ''' <summary>
        ''' 由一个基种子与一个字符串键派生新种子
        ''' </summary>
        ''' <param name="seed">基种子</param>
        ''' <param name="key">任务键（如扰动名）</param>
        ''' <returns></returns>
        Public Function Derive(seed As Long, key As String) As Long
            Return Combine(seed, FromString(key))
        End Function

        ''' <summary>
        ''' 合并两个种子（线性同余混合后截断到 32 位）
        ''' </summary>
        ''' <param name="seed"></param>
        ''' <param name="key"></param>
        ''' <returns></returns>
        Public Function Combine(seed As Long, key As Long) As Long
            ' seed 先截断到 32 位，乘数 1000003 使其上界为 2^32 * 1e6 ≈ 4.3e15，仍在 Int64 范围内
            Dim mixed As Long = ((seed And UInt32Mask) * 1000003L + (key And UInt32Mask)) And UInt32Mask

            If mixed = 0 Then Return 1L

            Return mixed
        End Function

    End Module
End Namespace
