Imports sysmath = System.Math

Namespace RandomSources

    ''' <summary>
    ''' MT19937（Mersenne Twister）随机数源，是 <see cref="IRandomSource"/> 的默认实现。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 状态机严格按照 Matsumoto &amp; Nishimura (1998) 的参考实现：
    ''' <c>N = 624</c>、<c>M = 397</c>、<c>MATRIX_A = 0x9908B0DF</c>，
    ''' 初始化用 Knuth 的 <c>1812433253</c> 递推式，输出经过
    ''' <c>11 / 7 / 15 / 18</c> 四段 tempering。周期为 <c>2^19937 - 1</c>，
    ''' 均匀性足以支撑蒙特卡洛重采样（但不是密码学安全的）。
    ''' </para>
    ''' <para>
    ''' 为什么不用仓库里已有的 <c>MersenneTwisterFast</c>：该类型的带种子构造函数是
    ''' <c>Private</c> 且函数体为空（不会初始化状态数组），无参构造函数在
    ''' <c>mt</c> 仍为 <c>Nothing</c> 时就会调用 <c>nextInt()</c>，
    ''' 因此无法在外部以确定种子构造出一个可用的生成器。本实现保持同一算法，
    ''' 但把种子初始化做对，并且<b>不做静态共享</b>，避免并行下相互污染。
    ''' </para>
    ''' <para>
    ''' 32 位状态用 <see cref="Long"/> 承载并显式掩码，乘法通过
    ''' <see cref="Multiply32"/> 拆成 16 位半字完成，因此无论工程是否开启
    ''' VB 的整数溢出检查都不会抛 <see cref="OverflowException"/>。
    ''' </para>
    ''' </remarks>
    Public Class MersenneTwisterSource
        Implements IRandomSource

#Region "MT19937 parameters"

        Private Const N As Integer = 624
        Private Const M As Integer = 397
        Private Const MatrixA As Long = &H9908B0DFL
        Private Const UpperMask As Long = &H80000000L
        Private Const LowerMask As Long = &H7FFFFFFFL
        Private Const UInt32Mask As Long = &HFFFFFFFFL
        Private Const DefaultSeed As Long = 5489L

#End Region

        Private ReadOnly m_state As Long() = New Long(N - 1) {}
        Private m_index As Integer
        Private m_seed As Long
        Private m_nextNormal As Double
        Private m_hasNextNormal As Boolean

        ''' <summary>
        ''' 以指定种子构造；种子为 0 时使用 MT19937 的参考默认种子 5489
        ''' </summary>
        ''' <param name="seed"></param>
        Public Sub New(seed As Long)
            If seed = 0 Then seed = DefaultSeed

            m_seed = seed
            Call InitializeState(seed)
        End Sub

        ''' <summary>
        ''' 构造时使用的种子（<see cref="Derive"/> 用它派生确定性子流）
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property SeedValue As Long
            Get
                Return m_seed
            End Get
        End Property

        Public ReadOnly Property IsRepeatable As Boolean Implements IRandomSource.IsRepeatable
            Get
                Return True
            End Get
        End Property

#Region "State machine"

        Private Sub InitializeState(seed As Long)
            m_state(0) = seed And UInt32Mask

            For i As Integer = 1 To N - 1
                Dim prev As Long = m_state(i - 1)

                m_state(i) = (Multiply32(1812433253L, prev Xor (prev >> 30)) + i) And UInt32Mask
            Next

            m_index = N
            m_hasNextNormal = False
            m_nextNormal = 0.0
        End Sub

        ''' <summary>
        ''' 32 位无符号乘法（结果对 2^32 取模），用 16 位半字避免溢出
        ''' </summary>
        ''' <param name="a"></param>
        ''' <param name="b"></param>
        ''' <returns></returns>
        Private Shared Function Multiply32(a As Long, b As Long) As Long
            Dim aLo As Long = a And &HFFFFL
            Dim aHi As Long = (a >> 16) And &HFFFFL
            Dim bLo As Long = b And &HFFFFL
            Dim bHi As Long = (b >> 16) And &HFFFFL
            Dim lo As Long = aLo * bLo
            Dim mid As Long = ((aLo * bHi) + (aHi * bLo)) And &HFFFFL

            Return ((mid << 16) + lo) And UInt32Mask
        End Function

        Private Sub Twist()
            Dim kk As Integer = 0

            For kk = 0 To N - M - 1
                Dim y As Long = (m_state(kk) And UpperMask) Or (m_state(kk + 1) And LowerMask)

                m_state(kk) = m_state(kk + M) Xor (y >> 1) Xor If((y And 1L) = 1L, MatrixA, 0L)
            Next

            For kk = N - M To N - 2
                Dim y As Long = (m_state(kk) And UpperMask) Or (m_state(kk + 1) And LowerMask)

                m_state(kk) = m_state(kk + (M - N)) Xor (y >> 1) Xor If((y And 1L) = 1L, MatrixA, 0L)
            Next

            Dim yl As Long = (m_state(N - 1) And UpperMask) Or (m_state(0) And LowerMask)

            m_state(N - 1) = m_state(M - 1) Xor (yl >> 1) Xor If((yl And 1L) = 1L, MatrixA, 0L)
            m_index = 0
        End Sub

        ''' <summary>
        ''' 生成一个 32 位无符号随机整数（以 <see cref="Long"/> 承载，范围 <c>[0, 2^32)</c>）
        ''' </summary>
        ''' <returns></returns>
        Public Function NextUInt32() As Long
            If m_index >= N Then Call Twist()

            Dim y As Long = m_state(m_index)

            m_index += 1

            y = y Xor (y >> 11)
            y = y Xor ((y << 7) And &H9D2C5680L)
            y = y Xor ((y << 15) And &HEFC60000L)
            y = y Xor (y >> 18)

            Return y And UInt32Mask
        End Function

#End Region

#Region "IRandomSource"

        Public Function NextDouble() As Double Implements IRandomSource.NextDouble
            Return NextUInt32() * (1.0 / 4294967296.0)
        End Function

        Public Function NextNormal() As Double Implements IRandomSource.NextNormal
            If m_hasNextNormal Then
                m_hasNextNormal = False
                Return m_nextNormal
            End If

            Dim u1 As Double

            Do
                u1 = NextDouble()
            Loop While u1 <= 0.0

            Dim u2 As Double = NextDouble()
            Dim r As Double = sysmath.Sqrt(-2.0 * sysmath.Log(u1))
            Dim theta As Double = 2.0 * sysmath.PI * u2

            m_nextNormal = r * sysmath.Sin(theta)
            m_hasNextNormal = True

            Return r * sysmath.Cos(theta)
        End Function

        Public Function NextInt(bound As Integer) As Integer Implements IRandomSource.NextInt
            If bound <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(bound), bound, "bound must be positive")
            End If

            Dim v As Long = CLng(sysmath.Floor(NextDouble() * bound))

            If v < 0 Then Return 0
            If v >= bound Then Return bound - 1

            Return CInt(v)
        End Function

        Public Function Choice(n As Integer, Optional p As Double() = Nothing) As Integer Implements IRandomSource.Choice
            If n <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(n), n, "the number of candidates must be positive")
            End If

            If p Is Nothing Then Return NextInt(n)
            If p.Length <> n Then
                Throw New ArgumentException($"weight length {p.Length} does not match the candidate count {n}", NameOf(p))
            End If

            Dim total As Double = 0.0

            For i As Integer = 0 To n - 1
                total += p(i)
            Next

            If total <= 0.0 Then Return NextInt(n)

            Dim u As Double = NextDouble() * total
            Dim acc As Double = 0.0

            For i As Integer = 0 To n - 2
                acc += p(i)

                If u < acc Then Return i
            Next

            Return n - 1
        End Function

        Public Function Choices(n As Integer, size As Integer, Optional p As Double() = Nothing) As Integer() Implements IRandomSource.Choices
            If size < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(size), size, "sample size must not be negative")
            End If

            Dim out As Integer() = New Integer(size - 1) {}

            For i As Integer = 0 To size - 1
                out(i) = Choice(n, p)
            Next

            Return out
        End Function

        Public Function Sample(n As Integer, k As Integer, Optional replace As Boolean = False) As Integer() Implements IRandomSource.Sample
            If n <= 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(n), n, "the population size must be positive")
            End If
            If k < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(k), k, "sample size must not be negative")
            End If
            If Not replace AndAlso k > n Then
                Throw New ArgumentException($"cannot draw {k} distinct sample(s) from {n} candidate(s)", NameOf(k))
            End If

            If replace Then Return Choices(n, k)

            Dim index As Integer() = New Integer(n - 1) {}

            For i As Integer = 0 To n - 1
                index(i) = i
            Next

            ' 部分 Fisher-Yates：只需要前 k 个位置被随机化
            For i As Integer = 0 To k - 1
                Dim j As Integer = i + NextInt(n - i)
                Dim t As Integer = index(i)

                index(i) = index(j)
                index(j) = t
            Next

            Dim out As Integer() = New Integer(k - 1) {}

            Call Array.ConstrainedCopy(index, 0, out, 0, k)

            Return out
        End Function

        Public Function Permutation(n As Integer) As Integer() Implements IRandomSource.Permutation
            If n < 0 Then
                Throw New ArgumentOutOfRangeException(NameOf(n), n, "the element count must not be negative")
            End If

            Dim out As Integer() = New Integer(n - 1) {}

            For i As Integer = 0 To n - 1
                out(i) = i
            Next

            For i As Integer = n - 1 To 1 Step -1
                Dim j As Integer = NextInt(i + 1)
                Dim t As Integer = out(i)

                out(i) = out(j)
                out(j) = t
            Next

            Return out
        End Function

        Public Function Permutation(values As Double()) As Double() Implements IRandomSource.Permutation
            If values Is Nothing Then Throw New ArgumentNullException(NameOf(values))

            Dim order As Integer() = Permutation(values.Length)
            Dim out As Double() = New Double(values.Length - 1) {}

            For i As Integer = 0 To out.Length - 1
                out(i) = values(order(i))
            Next

            Return out
        End Function

        Public Function Derive(key As Long) As IRandomSource Implements IRandomSource.Derive
            Return New MersenneTwisterSource(MixSeed(m_seed, key))
        End Function

#End Region

        ''' <summary>
        ''' 由父种子与派生键混合出子种子（xorshift 混合，非密码学安全）
        ''' </summary>
        ''' <param name="seed"></param>
        ''' <param name="key"></param>
        ''' <returns></returns>
        Private Shared Function MixSeed(seed As Long, key As Long) As Long
            Dim x As Long = (seed And UInt32Mask) Xor (key And UInt32Mask)

            x = x Xor ((x << 13) And UInt32Mask)
            x = x Xor (x >> 17)
            x = x Xor ((x << 5) And UInt32Mask)
            x = x And UInt32Mask

            If x = 0 Then x = DefaultSeed

            Return x
        End Function

        Public Overrides Function ToString() As String
            Return $"MT19937(seed={m_seed})"
        End Function

    End Class
End Namespace
