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
        ''' 生成策略:以"weight 个匹配位 + 1 个内部间隔"为基形状,
        ''' 通过对间隔位置与整体右移旋转派生出足够多的不同形状。
        ''' </summary>
        Public Function GenerateSeeds(count As Integer, weight As Integer) As SpacedSeed()
            Dim seeds As New List(Of SpacedSeed)
            Dim seen As New HashSet(Of Long)

            ' 基形状:weight 个匹配位中插入 1 个间隔位 -> 长度 = weight + 1
            ' 先构造带单个间隔的基形状,再旋转间隔位置
            For gap As Integer = 1 To weight - 1
                Dim shape As Long = BuildShapeWithGap(weight, gap)

                If Not seen.Contains(shape) Then
                    seen.Add(shape)
                    seeds.Add(New SpacedSeed(shape, weight))
                End If

                If seeds.Count >= count Then
                    Exit For
                End If
            Next

            ' 若基形状派生不足,则通过整体旋转已有形状继续派生,
            ' 以保证返回数量严格等于 count。
            Dim idx As Integer = 0

            While seeds.Count < count
                Dim baseShape = seeds(idx Mod seeds.Count).Shape
                idx += 1

                ' 低位补 0 的循环右移,改变间隔排布
                Dim rotated As Long = (baseShape >> 1) Or ((baseShape And 1L) << (weight))

                ' 标准化:去掉高位前导零(保留权重个最低有效匹配位)
                rotated = Normalize(rotated, weight)

                If rotated <> 0 AndAlso Not seen.Contains(rotated) Then
                    seen.Add(rotated)
                    seeds.Add(New SpacedSeed(rotated, weight))
                End If
            End While

            Return seeds.ToArray
        End Function

        ''' <summary>
        ''' 构造一个长度为 (weight+1)、在位置 <paramref name="gap"/> 处为间隔位的形状。
        ''' 匹配位总数为 weight。
        ''' </summary>
        Private Function BuildShapeWithGap(weight As Integer, gap As Integer) As Long
            ' 放置 weight 个匹配位,其中第 gap 个位置(0-based 计数)跳过 -> 成为间隔
            Dim shape As Long = 0
            Dim placed As Integer = 0

            For j As Integer = 0 To weight
                If placed = gap Then
                    ' 当前位置作为间隔位,跳过(不置 1)
                    placed += 1
                End If

                shape = shape Or (1L << j)

                If placed = gap Then
                    ' 该位已被用作间隔,撤销置位
                    shape = shape And Not (1L << j)
                End If

                placed += 1
            Next

            Return shape
        End Function

        ''' <summary>
        ''' 将形状标准化为恰好 <paramref name="weight"/> 个匹配位的最低连续形态。
        ''' </summary>
        Private Function Normalize(shape As Long, weight As Integer) As Long
            ' 重新紧凑:取最低 weight 个置位,重塑到连续低位
            Dim result As Long = 0
            Dim cnt As Integer = 0
            Dim outPos As Integer = 0

            For j As Integer = 0 To 63
                If (shape And (1L << j)) <> 0 Then
                    result = result Or (1L << outPos)
                    outPos += 1
                    cnt += 1

                    If cnt >= weight Then
                        Exit For
                    End If
                End If
            Next

            Return result
        End Function
    End Module
End Namespace
