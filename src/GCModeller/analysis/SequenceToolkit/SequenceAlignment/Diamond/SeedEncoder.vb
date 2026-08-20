' 间隔种子编码器 (Seed Encoder)
'
' 将一条蛋白序列按指定的间隔种子形状抽取残基,并用缩减字母表编码为整数哈希码。
' 编码规则:遍历形状中从左到右的每一个匹配位,取该位置对应的缩减类编号 (0..10),
' 按 base-11 滚动累加:
'     code = code * AlphabetSize + reducedClass
' 这样:查询序列与参考序列在匹配位上的缩减残基完全一致时,得到相同的整数编码,
' 即可用于双索引的哈希连接(相等即命中)。
'
' 编码空间:权重 W 时编码域为 [0, 11^W)。W <= 10 时 11^10 ≈ 2.6e10 < Long.MaxValue,
' 安全落在 Int64 范围内。

Imports System.Runtime.CompilerServices

Namespace DIAMOND

    ''' <summary>
    ''' 序列中一个种子命中的位置记录(相对序列内坐标)。
    ''' </summary>
    Public Structure SeedHitPosition
        ''' <summary>该种子形状在序列上的起始位置(匹配位 0 对应的索引)。</summary>
        Public ReadOnly Position As Integer
        ''' <summary>该位置的种子整数编码。</summary>
        Public ReadOnly Code As Long

        Sub New(position As Integer, code As Long)
            Me.Position = position
            Me.Code = code
        End Sub
    End Structure

    Public Module SeedEncoder

        ''' <summary>
        ''' 对单条序列(已转为 Char 数组或直接为 String)按给定形状枚举所有种子编码。
        ''' </summary>
        ''' <param name="sequence">原始蛋白序列(大小写不敏感)。</param>
        ''' <param name="seed">间隔种子形状。</param>
        ''' <returns>每个可放置形状的位置对应的 (起始位置, 编码)。</returns>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Iterator Function EnumerateSeeds(sequence As String, seed As SpacedSeed) As IEnumerable(Of SeedHitPosition)
            If sequence Is Nothing OrElse sequence.Length < seed.Length Then
                Return
            End If

            ' 预计算缩减类编号数组,避免重复映射
            Dim reduced As Integer() = ReducedAlphabet.Encode(sequence)
            Dim len = seed.Length
            Dim mask = seed.Shape
            Dim weight = seed.Weight

            For start As Integer = 0 To reduced.Length - len
                Yield New SeedHitPosition(start, EncodeAt(reduced, start, mask, len, weight))
            Next
        End Function

        ''' <summary>
        ''' 计算序列在 [start, start+len) 窗口内、按形状掩码抽取的种子编码。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function EncodeAt(reduced As Integer(), start As Integer, mask As Long, len As Integer, weight As Integer) As Long
            Dim code As Long = 0

            For j As Integer = 0 To len - 1
                If (mask And (1L << j)) <> 0 Then
                    code = code * ReducedAlphabet.AlphabetSize + reduced(start + j)
                End If
            Next

            Return code
        End Function

        ''' <summary>
        ''' 批量构建一条序列在给定一组形状下的全部种子编码。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Iterator Function EnumerateSeeds(sequence As String, seeds As SpacedSeed()) As IEnumerable(Of (seedIndex As Integer, hit As SeedHitPosition))
            For s As Integer = 0 To seeds.Length - 1
                For Each h In EnumerateSeeds(sequence, seeds(s))
                    Yield (s, h)
                Next
            Next
        End Function
    End Module
End Namespace
