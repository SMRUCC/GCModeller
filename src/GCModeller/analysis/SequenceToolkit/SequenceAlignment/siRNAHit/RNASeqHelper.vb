Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman

Namespace siRNAHit

    ''' <summary>
    ''' RNA 序列操作辅助工具：反向互补、碱基配对分类以及驱动 Smith-Waterman
    ''' 比对所需的核酸专用 <see cref="GenericSymbol(Of Char)"/> 打分符号。
    ''' 
    ''' 注：<see cref="SMRUCC.genomics.SequenceModel.NucleicAcid.Complement"/> 仅处理
    ''' DNA（A/T/G/C），不识别 U，因此这里单独实现 RNA（A/U/G/C）的反向互补。
    ''' </summary>
    Public Module RNASeqHelper

        ''' <summary>
        ''' 碱基配对类型分类
        ''' </summary>
        Public Enum PairType
            ''' <summary>Watson-Crick 完美配对 (AU, UA, GC, CG)</summary>
            WC
            ''' <summary>G:U wobble 配对 (GU, UG)</summary>
            Wobble
            ''' <summary>非互补错配</summary>
            Mismatch
            ''' <summary>单侧凸起 / 缺口</summary>
            [Gap]
        End Enum

        ''' <summary>
        ''' 计算 RNA 序列的反向互补链（A↔U, G↔C）。大小写保留。
        ''' </summary>
        <Extension>
        Public Function ReverseComplementRNA(seq As String) As String
            If seq Is Nothing Then
                Return Nothing
            End If

            Dim chars As Char() = seq.ToCharArray()
            Array.Reverse(chars)

            For i As Integer = 0 To chars.Length - 1
                chars(i) = ComplementBase(chars(i))
            Next

            Return New String(chars)
        End Function

        ''' <summary>
        ''' 单个 RNA 碱基的互补：A↔U, G↔C；其余（含 T/N/-）原样返回。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ComplementBase(c As Char) As Char
            Select Case Char.ToUpper(c)
                Case "A"c : Return "U"c
                Case "U"c : Return "A"c
                Case "G"c : Return "C"c
                Case "C"c : Return "G"c
                Case "T"c : Return "A"c
                Case Else : Return c
            End Select
        End Function

        ''' <summary>
        ''' 将 miRNA（正向）字符与其靶位点上 mRNA 字符进行分类配对判定。
        ''' 由于比对时 query 已替换为 miRNA 的反向互补，因此比对串中两者同向，
        ''' 直接按相同位比较即可。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ClassifyPair(miRNA As Char, mrna As Char) As PairType
            If miRNA = "-"c OrElse mrna = "-"c Then
                Return PairType.Gap
            End If

            Dim a As Char = Char.ToUpper(miRNA)
            Dim b As Char = Char.ToUpper(mrna)

            If a = b Then
                Return PairType.WC
            End If

            ' G:U wobble：配对后互补 (G-U / U-G)
            If (a = "G"c AndAlso b = "U"c) OrElse (a = "U"c AndAlso b = "G"c) Then
                Return PairType.Wobble
            End If

            Return PairType.Mismatch
        End Function

        ''' <summary>
        ''' 构造用于小RNA靶标比对的核酸专用打分矩阵：
        ''' 匹配 +15、错配 -10（对应 TargetFinder 的 <c>-r +15/-10</c>）。
        ''' SW 分数仅用于定位最佳局部比对，最终的期望/罚分由位置加权体系计算。
        ''' </summary>
        Public Function NucleicAcidSymbol() As GenericSymbol(Of Char)
            Return New GenericSymbol(Of Char)(
                equals:=Function(x, y) x = y,
                similarity:=Function(x, y)
                                If x = y Then
                                    Return 15.0
                                Else
                                    Return -10.0
                                End If
                            End Function,
                toChar:=Function(x) x,
                empty:=Function() "-"c
            )
        End Function

        ''' <summary>
        ''' 在一条 mRNA 序列上以 miRNA 的反向互补为正向 query 进行 Smith-Waterman
        ''' 局部比对，返回得分最高的一条 HSP（轻量路径，不构建 DP 矩阵）。
        ''' </summary>
        ''' <param name="mirnaRevComp">miRNA 的反向互补序列</param>
        ''' <param name="mrna">候选 mRNA 正向序列</param>
        Public Function BestLocalHit(mirnaRevComp As String, mrna As String) As LocalHSPMatch(Of Char)
            Dim sw As New GSW(Of Char)(mirnaRevComp.ToArray, mrna.ToArray, NucleicAcidSymbol())
            Call sw.BuildMatrix()
            Return sw.GetBestHSP(0, 1)
        End Function
    End Module
End Namespace
