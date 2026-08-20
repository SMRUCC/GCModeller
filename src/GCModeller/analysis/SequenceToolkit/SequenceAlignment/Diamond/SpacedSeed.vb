' DIAMOND 间隔种子 (Spaced Seed) 形状集
'
' 间隔种子用一个由"匹配位"(1) 和"忽略位"(0) 组成的形状从序列中抽取残基。
' 例如形状 11011011(权重 5,长度 8)只要求位置 0,1,3,4,6 的残基一致,
' 中间"忽略位"允许任意残基。相较 BLAST 的连续 3-mer,同样权重的间隔种子
' 对突变和插入缺失容忍度更高——只要突变不落在匹配位上,种子依然命中。
'
' 形状以 Long 位掩码表示:第 j 位为 1 表示形状的第 j 个位置是匹配位。
' 形状的总长度 = 最高匹配位索引 + 1;权重 = 掩码中 1 的个数 (popcount)。
'
' DIAMOND 通过 SpEED 工具离线计算各灵敏度模式的最优形状集。本实现按文档
' 要求(数量与权重)确定性地生成等价权重的间隔种子:
'   fast           : 2   个权重 10 的形状
'   sensitive      : 16  个权重 8  的形状
'   very-sensitive : 14  个权重 7  的形状
'   ultra-sensitive: 64  个权重 7  形状

Imports System.Numerics
Imports System.Runtime.CompilerServices

Namespace DIAMOND

    ''' <summary>
    ''' DIAMOND 灵敏度模式。本质是在种子数量与过滤严格度之间滑动。
    ''' </summary>
    Public Enum SensitivityMode
        ''' <summary>默认模式:2 个权重 10 的形状,约 8000 倍于 BLASTP 加速。</summary>
        Fast = 0
        ''' <summary>16 个权重 8 形状。</summary>
        Sensitive = 1
        ''' <summary>14 个权重 7 形状,接近 BLASTP 灵敏度。</summary>
        VerySensitive = 2
        ''' <summary>64 个权重 7 形状,灵敏度与 BLASTP 持平。</summary>
        UltraSensitive = 3
    End Enum

    ''' <summary>
    ''' 间隔种子形状(位掩码 + 权重)。
    ''' </summary>
    Public Structure SpacedSeed

        ''' <summary>
        ''' 匹配位掩码。第 j 位为 1 表示形状第 j 个位置是匹配位。
        ''' </summary>
        Public ReadOnly Shape As Long

        ''' <summary>
        ''' 权重:掩码中匹配位的数量 (popcount)。
        ''' </summary>
        Public ReadOnly Weight As Integer

        Sub New(shape As Long, weight As Integer)
            Me.Shape = shape
            Me.Weight = weight
        End Sub

        ''' <summary>
        ''' 形状总长度(最高匹配位索引 + 1)。
        ''' </summary>
        Public ReadOnly Property Length As Integer
            Get
                If Shape = 0 Then
                    Return 0
                End If

                Return 64 - MathLeadingZeroCount(Shape)
            End Get
        End Property

        ''' <summary>
        ''' 计算一个 Long 值的前导零个数(.NET 10 提供 LeadingZeroCount)。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Shared Function MathLeadingZeroCount(value As Long) As Integer
            Return System.Numerics.BitOperations.LeadingZeroCount(CULng(value))
        End Function

        Public Overrides Function ToString() As String
            Dim bits As Char() = New Char(Length - 1) {}

            For j As Integer = 0 To Length - 1
                bits(j) = If((Shape And (1L << j)) <> 0, "1"c, "0"c)
            Next

            Return New String(bits) & $" (w={Weight}, len={Length})"
        End Function
    End Structure

    Public Module SpacedSeeds

        ''' <summary>
        ''' 各灵敏度模式对应的 (形状数量, 权重)。
        ''' </summary>
        Private ReadOnly config As Dictionary(Of SensitivityMode, (count As Integer, weight As Integer)) = New Dictionary(Of SensitivityMode, (Integer, Integer)) From {
            {SensitivityMode.Fast, (2, 10)},
            {SensitivityMode.Sensitive, (16, 8)},
            {SensitivityMode.VerySensitive, (14, 7)},
            {SensitivityMode.UltraSensitive, (64, 7)}
        }

        ''' <summary>
        ''' 获取指定灵敏度模式的间隔种子形状集。
        ''' </summary>
        Public Function GetSeeds(mode As SensitivityMode) As SpacedSeed()
            Dim cfg = config(mode)
            Return GenerateSeeds(cfg.count, cfg.weight)
        End Function

        ''' <summary>
        ''' 确定性生成 <paramref name="count"/> 个权重为 <paramref name="weight"/> 的间隔种子。
        ''' 生成策略:在长度 L = weight + k (k 为间隔位数) 的窗口内,枚举恰好 k 个间隔位
        ''' (其余 weight 个为匹配位) 的所有组合。k 从 1 起递增,直到累积出足够数量的
        ''' 不同形状,从而保证返回数量严格等于 <paramref name="count"/> 且每个形状权重正确。
        ''' </summary>
        Public Function GenerateSeeds(count As Integer, weight As Integer) As SpacedSeed()
            Dim seeds As New List(Of SpacedSeed)
            Dim seen As New HashSet(Of Long)

            Dim k As Integer = 1

            While seeds.Count < count AndAlso k <= Math.Max(weight, 24)
                Dim L As Integer = weight + k
                Dim full As Long = (1L << L) - 1   ' 长度 L 内 L 个匹配位
                Dim limit As Long = 1L << L

                ' 枚举所有恰好含 k 个间隔位的掩码 (popcount == k)
                For m As Long = 0 To limit - 1
                    If PopCount(m) <> k Then
                        Continue For
                    End If

                    ' 形状 = 全匹配位 异或 间隔掩码(清掉间隔位)
                    Dim shape As Long = full Xor m

                    If Not seen.Contains(shape) Then
                        seen.Add(shape)
                        seeds.Add(New SpacedSeed(shape, weight))

                        If seeds.Count >= count Then
                            Exit While
                        End If
                    End If
                Next

                k += 1
            End While

            Return seeds.ToArray
        End Function

        ''' <summary>
        ''' 计算一个 Long 值的 popcount(置位个数)。
        ''' </summary>
        Private Function PopCount(value As Long) As Integer
            Return CInt(System.Numerics.BitOperations.PopCount(CULng(value)))
        End Function
    End Module
End Namespace
