Namespace Clustal

    ''' <summary>
    ''' Gonnet 250 替换矩阵(Gonnet, Cohen &amp; Benner, Science 256:1443-1445, 1992)。
    ''' </summary>
    ''' <remarks>
    ''' <para>
    ''' 表中的数值与 <c>data/BLOSUM/GONNET.txt</c> 一致，也就是该矩阵的整数取整版本，
    ''' 数值单位为 1/3 bit(与 HHsuite/HHalign 的 gonnet 表相同)，因此换算为 bit 时
    ''' 需要乘以 <see cref="UnitScale"/>。
    ''' </para>
    ''' <para>
    ''' 注意：本表**不是** <c>ClustalW</c> 发行的浮点 gonnet 文件(半 bit 单位)的精确定值，
    ''' 而是同一矩阵的整数近似。对于远源序列的灵敏度，这个差异是可控的，并且可以通过
    ''' <see cref="UnitScale"/> 调整整体的打分尺度。
    ''' </para>
    ''' </remarks>
    Public Module Gonnet250

        ''' <summary>
        ''' 矩阵的行列顺序(22 个符号，含通配残基 X 与终止符 *)
        ''' </summary>
        Public Const Letters As String = "CSTPAGNDEQHRKMILVFYWX*"

        ''' <summary>
        ''' 表值到 bit 的换算系数：表中的数值以 1/3 bit 为单位
        ''' </summary>
        Public Const UnitScale As Double = 1.0R / 3.0R

        ''' <summary>
        ''' 22 x 22 的整数打分表
        ''' </summary>
        Public ReadOnly Table As Integer()()

        Private ReadOnly index As Integer()
        Private ReadOnly wildcard As Integer

        Sub New()
            Table = {
                New Integer() {12, 0, 0, -3, 0, -2, -2, -3, -3, -2, -1, -2, -3, -1, -1, -2, 0, -1, 0, -1, -3, -8},
                New Integer() {0, 2, 2, 0, 1, 0, 1, 0, 0, 0, 0, 0, 0, -1, -2, -2, -1, -3, -2, -3, 0, -8},
                New Integer() {0, 2, 2, 0, 1, -1, 0, 0, 0, 0, 0, 0, 0, -1, -1, -1, 0, -2, -2, -4, 0, -8},
                New Integer() {-3, 0, 0, 8, 0, -2, -1, -1, 0, 0, -1, -1, -1, -2, -3, -2, -2, -4, -3, -5, -1, -8},
                New Integer() {0, 1, 1, 0, 2, 0, 0, 0, 0, 0, -1, -1, 0, -1, -1, -1, 0, -2, -2, -4, 0, -8},
                New Integer() {-2, 0, -1, -2, 0, 7, 0, 0, -1, -1, -1, -1, -1, -4, -4, -4, -3, -5, -4, -4, -1, -8},
                New Integer() {-2, 1, 0, -1, 0, 0, 4, 2, 1, 1, 1, 0, 1, -2, -3, -3, -2, -3, -1, -4, 0, -8},
                New Integer() {-3, 0, 0, -1, 0, 0, 2, 5, 3, 1, 0, 0, 0, -3, -4, -4, -3, -4, -3, -5, -1, -8},
                New Integer() {-3, 0, 0, 0, 0, -1, 1, 3, 4, 2, 0, 0, 1, -2, -3, -3, -2, -4, -3, -4, -1, -8},
                New Integer() {-2, 0, 0, 0, 0, -1, 1, 1, 2, 3, 1, 2, 2, -1, -2, -2, -2, -3, -2, -3, -1, -8},
                New Integer() {-1, 0, 0, -1, -1, -1, 1, 0, 0, 1, 6, 1, 1, -1, -2, -2, -2, 0, 2, -1, -1, -8},
                New Integer() {-2, 0, 0, -1, -1, -1, 0, 0, 0, 2, 1, 5, 3, -2, -2, -2, -2, -3, -2, -2, -1, -8},
                New Integer() {-3, 0, 0, -1, 0, -1, 1, 0, 1, 2, 1, 3, 3, -1, -2, -2, -2, -3, -2, -4, -1, -8},
                New Integer() {-1, -1, -1, -2, -1, -4, -2, -3, -2, -1, -1, -2, -1, 4, 2, 3, 2, 2, 0, -1, -1, -8},
                New Integer() {-1, -2, -1, -3, -1, -4, -3, -4, -3, -2, -2, -2, -2, 2, 4, 3, 3, 1, -1, -2, -1, -8},
                New Integer() {-2, -2, -1, -2, -1, -4, -3, -4, -3, -2, -2, -2, -2, 3, 3, 4, 2, 2, 0, -1, -1, -8},
                New Integer() {0, -1, 0, -2, 0, -3, -2, -3, -2, -2, -2, -2, -2, 2, 3, 2, 3, 0, -1, -3, -1, -8},
                New Integer() {-1, -3, -2, -4, -2, -5, -3, -4, -4, -3, 0, -3, -3, 2, 1, 2, 0, 7, 5, 4, -2, -8},
                New Integer() {0, -2, -2, -3, -2, -4, -1, -3, -3, -2, 2, -2, -2, 0, -1, 0, -1, 5, 8, 4, -2, -8},
                New Integer() {-1, -3, -4, -5, -4, -4, -4, -5, -4, -3, -1, -2, -4, -1, -2, -1, -3, 4, 4, 14, -4, -8},
                New Integer() {-3, 0, 0, -1, 0, -1, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -2, -2, -4, -1, -8},
                New Integer() {-8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, -8, 1}
            }

            index = New Integer(255) {}
            For i As Integer = 0 To 255
                index(i) = Letters.Length - 2   ' 默认落到 X
            Next

            For i As Integer = 0 To Letters.Length - 1
                index(AscW(Letters(i))) = i
                index(AscW(Char.ToLowerInvariant(Letters(i)))) = i
            Next

            wildcard = Letters.IndexOf("X"c)
        End Sub

        ''' <summary>
        ''' 以整数取整值的形式获取两个残基之间的打分(单位 1/3 bit)
        ''' </summary>
        Public Function RawScore(a As Char, b As Char) As Integer
            Return Table(IndexOf(a))(IndexOf(b))
        End Function

        ''' <summary>
        ''' 以 bit 为单位获取两个残基之间的替换打分
        ''' </summary>
        Public Function Score(a As Char, b As Char) As Double
            Return RawScore(a, b) * UnitScale
        End Function

        ''' <summary>
        ''' 字符 -> 矩阵行列索引
        ''' </summary>
        Public Function IndexOf(ch As Char) As Integer
            Dim i As Integer = AscW(ch)

            If i < 0 OrElse i > 255 Then
                Return wildcard
            Else
                Return index(i)
            End If
        End Function
    End Module

    ''' <summary>
    ''' 蛋白质/核酸的背景残基频率，用于 profile HMM 的发射概率先验与伪计数。
    ''' </summary>
    Public Module BackgroundFrequencies

        ''' <summary>
        ''' 与 <see cref="SequenceAlphabet.ProteinLetters"/> 顺序一致的 20 种氨基酸背景频率
        ''' (Robinson &amp; Robinson / HHsuite 使用的标准分布，总和为 1)
        ''' </summary>
        Public ReadOnly Protein As Double() = {
            0.074, 0.025, 0.054, 0.054, 0.047, 0.074, 0.026, 0.068, 0.058, 0.099,
            0.025, 0.045, 0.039, 0.034, 0.052, 0.057, 0.051, 0.073, 0.013, 0.032
        }

        ''' <summary>
        ''' 核酸(ACGT)的均匀背景分布
        ''' </summary>
        Public ReadOnly DNA As Double() = {0.25, 0.25, 0.25, 0.25}

        ''' <summary>
        ''' 依据字母表取背景频率向量(长度等于字母表大小，含通配编码)
        ''' </summary>
        Public Function ForAlphabet(alphabet As SequenceAlphabet) As Double()
            Dim size As Integer = alphabet.Size
            Dim raw As Double() = If(alphabet.Kind = SequenceType.Protein, Protein, DNA)
            Dim n As Integer = Math.Min(raw.Length, size)
            Dim total As Double = 0
            Dim p As Double() = New Double(size - 1) {}

            For i As Integer = 0 To n - 1
                p(i) = raw(i)
                total += raw(i)
            Next

            ' 通配编码使用剩余概率
            Dim rest As Double = Math.Max(0.0, 1.0 - total)

            For i As Integer = n To size - 1
                p(i) = rest / Math.Max(1, size - n)
            Next

            If total <= 0 Then
                ' 极端情况下退化为均匀分布
                For i As Integer = 0 To size - 1
                    p(i) = 1.0R / size
                Next
            End If

            Return p
        End Function
    End Module
End Namespace
