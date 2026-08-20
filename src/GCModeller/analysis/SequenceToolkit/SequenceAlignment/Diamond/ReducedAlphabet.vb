' DIAMOND 缩减字母表 (Reduced Alphabet)
'
' 将 20 种标准氨基酸按物理化学性质聚类为 11 类,用于种子匹配阶段的编码。
' 同源序列发生保守替换(如 I<->V、D<->E)时,两种残基映射到同一缩减字母,
' 从而能在间隔种子命中阶段被同一枚种子捕获,显著提高远源同源的检出率。
'
' 注意:本表与 ProteinTools/ProteinMatrix/Linclust 的 13 字母表不同。
' DIAMOND 原始实现使用 11 类缩减字母表(文档要求 L/I/V 归为一类)。
'
' 11 类映射(原始字母 -> 缩减类编号 0..10):
'   0: A, G          (小/极性)
'   1: S, T          (羟基)
'   2: C             (半胱氨酸,特殊二硫键)
'   3: D, E          (酸性)
'   4: N, Q          (酰胺)
'   5: K, R          (碱性)
'   6: H             (组氨酸)
'   7: I, L, M, V    (疏水脂肪族;L/I/V 同组,符合文档要求)
'   8: F, Y, W       (芳香族)
'   9: P             (脯氨酸,刚性)
'  10: X / * / 其它非标准残基 (通配/未知,保留为单独一类)
'
' 编码基数 = 11,种子整数哈希按 base-11 滚动累加,保证同形状同编码可哈希连接。

Imports System.Runtime.CompilerServices

Namespace DIAMOND

    Public Module ReducedAlphabet

        ''' <summary>
        ''' 缩减字母表大小(哈希基数)。
        ''' </summary>
        Public Const AlphabetSize As Integer = 11

        ''' <summary>
        ''' 未知 / 非标准氨基酸映射到的缩减类编号(通配类)。
        ''' </summary>
        Public Const WildcardClass As Integer = 10

        ''' <summary>
        ''' 原始氨基酸 -> 缩减类编号 的查找表。
        ''' </summary>
        Private ReadOnly classMap As Dictionary(Of Char, Integer)

        Sub New()
            classMap = New Dictionary(Of Char, Integer) From {
                {"A"c, 0}, {"G"c, 0},
                {"S"c, 1}, {"T"c, 1},
                {"C"c, 2},
                {"D"c, 3}, {"E"c, 3},
                {"N"c, 4}, {"Q"c, 4},
                {"K"c, 5}, {"R"c, 5},
                {"H"c, 6},
                {"I"c, 7}, {"L"c, 7}, {"M"c, 7}, {"V"c, 7},
                {"F"c, 8}, {"Y"c, 8}, {"W"c, 8},
                {"P"c, 9}
            }
        End Sub

        ''' <summary>
        ''' 将单个氨基酸字符映射到缩减类编号 (0..10)。
        ''' 非标准 / 未知字符返回 <see cref="WildcardClass"/>。
        ''' 大小写不敏感。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Map(aa As Char) As Integer
            Dim key = Char.ToUpper(aa)

            If classMap.ContainsKey(key) Then
                Return classMap(key)
            Else
                Return WildcardClass
            End If
        End Function

        ''' <summary>
        ''' 将整条蛋白序列编码为缩减类编号数组。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Encode(seq As String) As Integer()
            Dim codes As Integer() = New Integer(seq.Length - 1) {}

            For i As Integer = 0 To seq.Length - 1
                codes(i) = Map(seq(i))
            Next

            Return codes
        End Function
    End Module
End Namespace
