' 种子形状的快速编码计划 (Seed Shape Plan)
'
' <see cref="SpacedSeed"/> 只描述位掩码,直接用它逐位置编码需要在内层循环里做
' 位测试 (mask And (1L << j)) 判断当前位是否参与编码,分支与位移开销明显。
'
' <see cref="SeedPlan"/> 在比对开始前把形状"预编译"为匹配位偏移数组:
'   - Offsets : 形状中所有权重位相对形状起始的偏移(升序);
'   - Span    : 形状总长度(最高匹配位偏移 + 1);
'   - Weight  : 权重;
'   - Powers  : 每一位在 base-11 编码中的乘数(权重位 11 的幂)。
'
' 这样编码就退化成一次权重长度的"乘加"循环,无分支、无位运算,
' 且生成的编码与原 <see cref="SeedEncoder.EncodeAt"/> 完全一致。

Imports System.Runtime.CompilerServices

Namespace DIAMOND

    ''' <summary>
    ''' 间隔种子形状的预编译编码计划(只读,可跨线程共享)。
    ''' </summary>
    Public NotInheritable Class SeedPlan

        Public ReadOnly Seed As SpacedSeed

        ''' <summary>形状总长度(最高匹配位偏移 + 1)。</summary>
        Public ReadOnly Span As Integer

        ''' <summary>权重(参与编码的残基个数)。</summary>
        Public ReadOnly Weight As Integer

        ''' <summary>匹配位相对形状起始的偏移(升序)。</summary>
        Public ReadOnly Offsets As Integer()

        Sub New(seed As SpacedSeed)
            Me.Seed = seed
            Me.Span = seed.Length
            Me.Weight = seed.Weight

            Dim offsets As New List(Of Integer)

            For j As Integer = 0 To Math.Max(0, Me.Span - 1)
                If (seed.Shape And (1L << j)) <> 0 Then
                    offsets.Add(j)
                End If
            Next

            Me.Offsets = offsets.ToArray
        End Sub

        ''' <summary>
        ''' 在缩减字母表缓冲区上计算从 <paramref name="start"/> 开始的种子编码。
        ''' base-11 滚动累加,与 <see cref="SeedEncoder"/> 的定义一致。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Encode(reduced As Byte(), start As Integer) As Long
            Dim offsets As Integer() = Me.Offsets
            Dim weight As Integer = Me.Weight
            Dim code As Long = 0

            For j As Integer = 0 To weight - 1
                code = code * ReducedAlphabet.AlphabetSize + reduced(start + offsets(j))
            Next

            Return code
        End Function

        ''' <summary>
        ''' 在"氨基酸编码 + 缩减表"组合缓冲区上计算种子编码。
        ''' </summary>
        ''' <param name="packed">氨基酸编码扁平缓冲区。</param>
        ''' <param name="reducedOfCode">氨基酸编码 -> 缩减类别 查表。</param>
        ''' <param name="start">起始位置。</param>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Encode(packed As Byte(), reducedOfCode As Byte(), start As Integer) As Long
            Dim offsets As Integer() = Me.Offsets
            Dim weight As Integer = Me.Weight
            Dim code As Long = 0

            For j As Integer = 0 To weight - 1
                code = code * ReducedAlphabet.AlphabetSize + reducedOfCode(packed(start + offsets(j)))
            Next

            Return code
        End Function
    End Class
End Namespace
