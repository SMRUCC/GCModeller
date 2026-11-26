Imports std = System.Math

Namespace Randomizer

    ''' <summary>
    ''' 可播种的确定性随机源：同一种子给出<b>逐位相同</b>的序列。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' <b>为什么不用 <c>System.Random</c></b>：它的序列不保证跨运行时版本稳定，
    ''' 而合成结果由"操作顺序 + 随机数抽取顺序"决定——随机序列一变，整个预测矩阵就变。
    ''' </para>
    ''' <para>
    ''' <b>引擎说明（Level A）</b>：热路径采用 <c>xoshiro256+</c>（xoshiro 家族里专为浮点抽取设计的变体，
    ''' 只用加法、异或与移位，状态推进没有乘法），种子用 SplitMix64 扩散。
    ''' 之所以不用完整的 128 位 PCG64，是因为 VB 默认开启整数溢出检查，
    ''' 128 位回绕乘法需要额外的分段实现、且难以在仓库内独立验证；
    ''' <c>xoshiro256+</c> 的推进只依赖 64 位加法（由 <see cref="AddMod"/> 保证回绕语义），
    ''' 每一步都可以手算核对。
    ''' </para>
    ''' <para>
    ''' <b>与 numpy 的关系</b>：统计等价、同种子自一致，但<b>不</b>追求与
    ''' <c>numpy.random.default_rng</c> 的位流逐位相同。
    ''' </para>
    ''' </remarks>
    Public Class Pcg64

        Private Const TwoPowMinus53 As Double = 1.0 / 9007199254740992.0
        Private Const SeedStep As ULong = &H9E3779B97F4A7C15UL

        Private m_s0 As ULong
        Private m_s1 As ULong
        Private m_s2 As ULong
        Private m_s3 As ULong

        Private m_spare As Double
        Private m_hasSpare As Boolean

        ''' <summary>供 <see cref="Clone"/> 使用的空构造。</summary>
        Private Sub New()
        End Sub

        ''' <summary>由整数种子构造。</summary>
        Public Sub New(seed As Long)
            Dim state As ULong = CULng(seed And &HFFFFFFFFL)

            m_s0 = SplitMix(state)
            m_s1 = SplitMix(AddMod(m_s0, SeedStep))
            m_s2 = SplitMix(AddMod(m_s1, SeedStep))
            m_s3 = SplitMix(AddMod(m_s2, SeedStep))

            ' 避开全零状态
            If (m_s0 Or m_s1 Or m_s2 Or m_s3) = 0UL Then
                m_s0 = 1UL
            End If
        End Sub

        ''' <summary>复制当前状态（派生独立子流而不影响父流）。</summary>
        Public Function Clone() As Pcg64
            Return New Pcg64 With {
                .m_s0 = m_s0,
                .m_s1 = m_s1,
                .m_s2 = m_s2,
                .m_s3 = m_s3,
                .m_spare = m_spare,
                .m_hasSpare = m_hasSpare
            }
        End Function

        ''' <summary>下一个 64 位无符号整数。</summary>
        ''' <remarks>
        ''' <c>xoshiro256+</c>：输出 <c>s0 + s3</c>，然后推进状态。
        ''' 该变体的高位质量足够好，正好用于取 <c>NextDouble</c> 的 53 位。
        ''' </remarks>
        Public Function NextUInt64() As ULong
            Dim result As ULong = AddMod(m_s0, m_s3)
            Dim t As ULong = m_s1 << 17

            m_s2 = m_s2 Xor m_s0
            m_s3 = m_s3 Xor m_s1
            m_s1 = m_s1 Xor m_s2
            m_s0 = m_s0 Xor m_s3
            m_s2 = m_s2 Xor t
            m_s3 = RotateLeft(m_s3, 45)

            Return result
        End Function

        ''' <summary><c>[0,1)</c> 上的双精度浮点数（53 位精度）。</summary>
        Public Function NextDouble() As Double
            Return (NextUInt64() >> 11) * TwoPowMinus53
        End Function

        ''' <summary><c>[0, n)</c> 上的整数（拒绝采样，无取模偏差）。</summary>
        Public Function NextInt32(n As Integer) As Integer
            If n <= 0 Then
                Return 0
            End If

            Dim bound As ULong = CULng(n)
            Dim limit As ULong = ULong.MaxValue - (ULong.MaxValue Mod bound)
            Dim value As ULong = NextUInt64()

            While value >= limit
                value = NextUInt64()
            End While

            Return CInt(value Mod bound)
        End Function

        ''' <summary>标准正态（Box–Muller，缓存第二个值）。</summary>
        Public Function StandardNormal() As Double
            If m_hasSpare Then
                m_hasSpare = False
                Return m_spare
            End If

            Dim u1 As Double

            Do
                u1 = NextDouble()
            Loop While u1 <= 0.0

            Dim u2 As Double = NextDouble()
            Dim magnitude As Double = std.Sqrt(-2.0 * std.Log(u1))
            Dim angle As Double = 2.0 * std.PI * u2

            m_spare = magnitude * std.Sin(angle)
            m_hasSpare = True

            Return magnitude * std.Cos(angle)
        End Function

        ''' <summary>原地洗牌（Fisher–Yates）。</summary>
        Public Sub Shuffle(values As Double())
            For i As Integer = values.Length - 1 To 1 Step -1
                Dim j As Integer = NextInt32(i + 1)
                Dim swap As Double = values(i)

                values(i) = values(j)
                values(j) = swap
            Next
        End Sub

        ''' <summary>原地洗牌（整数）。</summary>
        Public Sub Shuffle(values As Integer())
            For i As Integer = values.Length - 1 To 1 Step -1
                Dim j As Integer = NextInt32(i + 1)
                Dim swap As Integer = values(i)

                values(i) = values(j)
                values(j) = swap
            Next
        End Sub

        ''' <summary><c>0..n-1</c> 的一个随机排列。</summary>
        Public Function Permutation(n As Integer) As Integer()
            Dim result(n - 1) As Integer

            For i As Integer = 0 To n - 1
                result(i) = i
            Next

            Call Shuffle(result)

            Return result
        End Function

        ''' <summary>有放回地抽 <paramref name="size"/> 个 <c>[0, n)</c> 的整数。</summary>
        Public Function Choice(n As Integer, size As Integer) As Integer()
            Dim result(size - 1) As Integer

            For i As Integer = 0 To size - 1
                result(i) = NextInt32(n)
            Next

            Return result
        End Function

        ''' <summary>模 2⁶⁴ 加法（32 位分段，避免溢出检查触发的异常）。</summary>
        Private Shared Function AddMod(a As ULong, b As ULong) As ULong
            Dim lowSum As ULong = (a And &HFFFFFFFFUL) + (b And &HFFFFFFFFUL)
            Dim highSum As ULong = (a >> 32) + (b >> 32) + (lowSum >> 32)

            Return ((highSum And &HFFFFFFFFUL) << 32) Or (lowSum And &HFFFFFFFFUL)
        End Function

        ''' <summary>模 2⁶⁴ 乘法（取 128 位乘积的低半部分）。</summary>
        Private Shared Function MulLow(a As ULong, b As ULong) As ULong
            Dim low As ULong

            Call std.BigMul(a, b, low)

            Return low
        End Function

        Private Shared Function RotateLeft(x As ULong, k As Integer) As ULong
            Return (x << k) Or (x >> (64 - k))
        End Function

        ''' <summary>SplitMix64：把任意种子扩散成状态字。</summary>
        Private Shared Function SplitMix(seed As ULong) As ULong
            Dim x As ULong = AddMod(seed, SeedStep)

            x = MulLow(x Xor (x >> 30), &HBF58476D1CE4E5B9UL)
            x = MulLow(x Xor (x >> 27), &H94D049BB133111EBUL)

            Return x Xor (x >> 31)
        End Function

        Public Overrides Function ToString() As String
            Return $"xoshiro256+ [{m_s0:X16}:{m_s1:X16}:{m_s2:X16}:{m_s3:X16}]"
        End Function

    End Class
End Namespace
