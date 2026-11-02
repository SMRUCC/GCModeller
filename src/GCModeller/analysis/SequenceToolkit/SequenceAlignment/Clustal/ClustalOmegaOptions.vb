Namespace Clustal

    ''' <summary>
    ''' 后验解码方式
    ''' </summary>
    Public Enum DecodeMode
        ''' <summary>
        ''' 最大准确度解码(HHalign 的默认做法，先做前向/后向求后验概率，再最大化后验和)
        ''' </summary>
        MaximumAccuracy = 0
        ''' <summary>
        ''' Viterbi 解码(最优单一路径，不做后验概率，内存开销小)
        ''' </summary>
        Viterbi = 1
    End Enum

    ''' <summary>
    ''' Clustal Omega 流水线的参数对象
    ''' </summary>
    ''' <remarks>
    ''' 默认值与 clustalo 1.2.x 保持一致：mBed 聚类上限 100 条、蛋白 k-mer=2、
    ''' gap open 6 bit、gap extend 1 bit、不做迭代精炼、MAC 后验缓冲上限 2048 MB。
    ''' </remarks>
    Public Class ClustalOmegaOptions

        ''' <summary>
        ''' 序列类型，默认由输入内容自动判定
        ''' </summary>
        Public Property SeqType As SequenceType = SequenceType.Auto
        ''' <summary>
        ''' k-mer 长度，0 表示按序列类型自动取值(蛋白 2、核酸 5)
        ''' </summary>
        Public Property KmerSize As Integer = 0
        ''' <summary>
        ''' mBed 聚类时单个簇的序列条数上限
        ''' </summary>
        Public Property ClusterSize As Integer = 100
        ''' <summary>
        ''' 是否跳过 mBed 直接计算完整的 N x N 距离矩阵(仅适合小数据集)
        ''' </summary>
        Public Property FullDistanceMatrix As Boolean = False
        ''' <summary>
        ''' 允许走全距离矩阵分支的最大序列条数
        ''' </summary>
        Public Property MaxFullDistanceSequences As Integer = 512
        ''' <summary>
        ''' 迭代精炼的轮数，0 表示关闭
        ''' </summary>
        Public Property Iterations As Integer = 0
        ''' <summary>
        ''' 空位开放罚分(单位 bit，正数)
        ''' </summary>
        Public Property GapOpenBits As Double = 6.0
        ''' <summary>
        ''' 空位延伸罚分(单位 bit，正数)
        ''' </summary>
        Public Property GapExtendBits As Double = 1.0
        ''' <summary>
        ''' 终端空位的罚分缩放系数。
        ''' 
        ''' 取 0 时终端空位完全免费(纯半全局比对)，此时两条长度相近的 profile 可以通过
        ''' "自由滑动"得到只覆盖局部最优片段的退化比对；取 1 时终端空位与内部空位同罚
        ''' (纯全局比对)，对真实存在的末端悬挂过于严苛。默认 0.25 是在两者之间的折中。
        ''' </summary>
        Public Property EndGapScale As Double = 0.25
        ''' <summary>
        ''' MAC 后验概率缓冲的内存上限(MB)，超限时自动降级为 Viterbi 解码
        ''' </summary>
        Public Property MacRamMb As Integer = 2048
        ''' <summary>
        ''' 解码方式
        ''' </summary>
        Public Property Decode As DecodeMode = DecodeMode.MaximumAccuracy
        ''' <summary>
        ''' 随机种子，同一个种子可以保证同一份输入得到完全一致的结果
        ''' </summary>
        Public Property Seed As Integer = 20111
        ''' <summary>
        ''' 并行线程数，0 表示使用运行时的默认值
        ''' </summary>
        Public Property Threads As Integer = 0
        ''' <summary>
        ''' mBed 使用的种子序列条数，0 表示按 min(N, max(16, 2*ceil(log2 N))) 自动取值
        ''' </summary>
        Public Property SeedCount As Integer = 0

        ''' <summary>
        ''' 复制一份参数对象
        ''' </summary>
        Public Function Clone() As ClustalOmegaOptions
            Return New ClustalOmegaOptions With {
                .SeqType = SeqType,
                .KmerSize = KmerSize,
                .ClusterSize = ClusterSize,
                .FullDistanceMatrix = FullDistanceMatrix,
                .MaxFullDistanceSequences = MaxFullDistanceSequences,
                .Iterations = Iterations,
                .GapOpenBits = GapOpenBits,
                .GapExtendBits = GapExtendBits,
                .EndGapScale = EndGapScale,
                .MacRamMb = MacRamMb,
                .Decode = Decode,
                .Seed = Seed,
                .Threads = Threads,
                .SeedCount = SeedCount
            }
        End Function

        ''' <summary>
        ''' 校验参数并返回一份修正过非法取值的副本
        ''' </summary>
        ''' <exception cref="ArgumentException">当参数无法被修正时抛出</exception>
        Public Function Validate() As ClustalOmegaOptions
            Dim copy As ClustalOmegaOptions = Clone()

            If copy.KmerSize < 0 Then
                Throw New ArgumentException($"k-mer 长度不可以为负数: {copy.KmerSize}", NameOf(KmerSize))
            End If

            If copy.ClusterSize < 2 Then
                copy.ClusterSize = 2
            End If

            If copy.MaxFullDistanceSequences < 2 Then
                copy.MaxFullDistanceSequences = 2
            End If

            If copy.Iterations < 0 Then
                copy.Iterations = 0
            End If

            If copy.GapOpenBits <= 0 Then
                Throw New ArgumentException($"gap open 罚分必须为正数(单位 bit): {copy.GapOpenBits}", NameOf(GapOpenBits))
            End If

            If copy.GapExtendBits <= 0 Then
                Throw New ArgumentException($"gap extend 罚分必须为正数(单位 bit): {copy.GapExtendBits}", NameOf(GapExtendBits))
            End If

            If copy.EndGapScale < 0 Then
                copy.EndGapScale = 0
            ElseIf copy.EndGapScale > 1 Then
                copy.EndGapScale = 1
            End If

            If copy.MacRamMb < 16 Then
                copy.MacRamMb = 16
            End If

            If copy.Threads < 0 Then
                copy.Threads = 0
            End If

            If copy.SeedCount < 0 Then
                copy.SeedCount = 0
            End If

            Return copy
        End Function

        ''' <summary>
        ''' 依据字母表解析实际使用的 k-mer 长度
        ''' </summary>
        Public Function EffectiveKmer(alphabet As SequenceAlphabet) As Integer
            If KmerSize > 0 Then
                Return KmerSize
            Else
                Return alphabet.DefaultKmer
            End If
        End Function

        ''' <summary>
        ''' 依据字母表解析 mBed 的种子序列条数
        ''' </summary>
        Public Function EffectiveSeedCount(n As Integer) As Integer
            If n <= 0 Then
                Return 0
            End If
            If SeedCount > 0 Then
                Return Math.Min(n, SeedCount)
            End If

            ' log2(N) 上取整，最少 16 条种子
            Dim bits As Integer = CInt(Math.Ceiling(Math.Log(n, 2.0)))
            Return Math.Min(n, Math.Max(16, bits * 2))
        End Function

        Public Overrides Function ToString() As String
            Return $"seqType={SeqType}, k={KmerSize}(auto), clusterSize={ClusterSize}, iterations={Iterations}, gapOpen={GapOpenBits}bit, gapExtend={GapExtendBits}bit, decode={Decode}"
        End Function
    End Class
End Namespace
