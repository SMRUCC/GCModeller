#Region "Microsoft.VisualBasic::fd983c5b064766bdc7348d30dbcba38e, analysis\SequenceToolkit\SequenceAlignment\siRNAHit\RNASeqHelper.vb"

    ' Author:
    ' 
    '       asuka (amethyst.asuka@gcmodeller.org)
    '       xie (genetics@smrucc.org)
    '       xieguigang (xie.guigang@live.com)
    ' 
    ' Copyright (c) 2018 GPL3 Licensed
    ' 
    ' 
    ' GNU GENERAL PUBLIC LICENSE (GPL3)
    ' 
    ' 
    ' This program is free software: you can redistribute it and/or modify
    ' it under the terms of the GNU General Public License as published by
    ' the Free Software Foundation, either version 3 of the License, or
    ' (at your option) any later version.
    ' 
    ' This program is distributed in the hope that it will be useful,
    ' but WITHOUT ANY WARRANTY; without even the implied warranty of
    ' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    ' GNU General Public License for more details.
    ' 
    ' You should have received a copy of the GNU General Public License
    ' along with this program. If not, see <http://www.gnu.org/licenses/>.



    ' /********************************************************************************/

    ' Summaries:


    ' Code Statistics:

    '   Total Lines: 122
    '    Code Lines: 70 (57.38%)
    ' Comment Lines: 37 (30.33%)
    '    - Xml Docs: 94.59%
    ' 
    '   Blank Lines: 15 (12.30%)
    '     File Size: 4.93 KB


    '     Module RNASeqHelper
    ' 
    ' 
    '         Enum PairType
    ' 
    '             [Gap], Mismatch, WC, Wobble
    ' 
    ' 
    ' 
    '  
    ' 
    '     Function: BestLocalHit, ClassifyPair, ComplementBase, NucleicAcidSymbol, ReverseComplementRNA
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming
Imports Microsoft.VisualBasic.DataMining.DynamicProgramming.SmithWaterman

Namespace siRNAHit

    ''' <summary>
    ''' RNA 序列操作辅助工具：反向互补、碱基配对分类以及驱动 Smith-Waterman
    ''' 比对所需的核酸专用 <see cref="GenericSymbol(Of Char)"/> 打分符号。
    ''' 
    ''' 注：Bio.Assembly 的 NucleicAcid.Complement 仅处理 DNA（A/T/G/C），不识别 U，
    ''' 因此这里单独实现 RNA（A/U/G/C）的反向互补。
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
        ''' 将 DNA/RNA 序列归一化到 RNA 字母表：大写化并将 T 转换为 U。
        ''' </summary>
        ''' <param name="seq">原始序列，允许 DNA（含 T）或 RNA（含 U），允许小写。</param>
        ''' <returns>大写且仅含 A/U/G/C 的 RNA 序列；输入为 Nothing 时返回 Nothing。</returns>
        ''' <remarks>
        ''' BLASTN 的 fasta 输入与输出均为 DNA 字母表（含 T），而 miRNA 配对规则定义在
        ''' RNA 字母表上。若不做归一化，DNA 的 T 将永远无法与 RNA 规则中的 U 匹配，
        ''' 导致全部位点被误判为错配。
        ''' </remarks>
        <Extension>
        Public Function NormalizeRNA(seq As String) As String
            If seq Is Nothing Then
                Return Nothing
            End If

            Dim chars As Char() = seq.ToCharArray()

            For i As Integer = 0 To chars.Length - 1
                chars(i) = NormalizeRNABase(chars(i))
            Next

            Return New String(chars)
        End Function

        ''' <summary>
        ''' 单个碱基归一化到 RNA 字母表：大写化并将 T 转换为 U；'-' 与 N 等原样返回。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function NormalizeRNABase(c As Char) As Char
            Dim b As Char = Char.ToUpper(c)

            If b = "T"c Then
                Return "U"c
            Else
                Return b
            End If
        End Function

        ''' <summary>
        ''' 将 miRNA（正向）字符与其靶位点上 mRNA 字符进行分类配对判定。
        ''' 由于比对时 query 已替换为 miRNA 的反向互补，因此比对串中两者同向，
        ''' 直接按相同位比较即可。
        ''' </summary>
        ''' <param name="queryRevComp">
        ''' 比对串中 query 侧的碱基，即 miRNA 反向互补链上的字符（5'->3'）。
        ''' </param>
        ''' <param name="mrna">
        ''' 比对串中 subject 侧的碱基，即 mRNA 正义链上的字符（5'->3'）。
        ''' </param>
        ''' <remarks>
        ''' 同向框架下的配对规则是**方向敏感**的，两侧参数不可互换：
        ''' 
        ''' 设 duplex 中 miRNA 碱基为 m、与配对的 mRNA 碱基为 t（二者互补，m-t 取 A-U / G-C / G:U）。
        ''' 本函数输入为 a = complement(m)（即 revcomp(miRNA) 对应位）与 b = t，于是：
        ''' 
        '''   + Watson-Crick 完美配对 ⇒ a = b
        '''   + G:U wobble：m=G,t=U ⇒ a=complement(G)=C, b=U ⇒ (C,U)；
        '''                m=U,t=G ⇒ a=complement(U)=A, b=G ⇒ (A,G)
        '''   + 反向的 (G,A) 表示 m=C 对 t=A、(U,C) 表示 m=A 对 t=C，二者均为**真实错配**
        ''' 
        ''' 故同向框架的 wobble 必须写作 (C,U)/(A,G)，而非互补框架下的 (G,U)/(U,G)。
        ''' 若输入来自 BLASTN 的 qseq/sseq（miRNA vs revcomp(mRNA)）框架，请改用
        ''' <see cref="ClassifyBlastPair"/>。
        ''' </remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ClassifyPair(queryRevComp As Char, mrna As Char) As PairType
            If queryRevComp = "-"c OrElse mrna = "-"c Then
                Return PairType.Gap
            End If

            Dim a As Char = NormalizeRNABase(queryRevComp)
            Dim b As Char = NormalizeRNABase(mrna)

            If a = b Then
                Return PairType.WC
            End If

            ' G:U wobble 在同向框架下的表现形式（方向敏感）
            If (a = "C"c AndAlso b = "U"c) OrElse (a = "A"c AndAlso b = "G"c) Then
                Return PairType.Wobble
            End If

            Return PairType.Mismatch
        End Function

        ''' <summary>
        ''' BLASTN 的 HSP 配对判定：qseq/sseq 是**同向一致（identity）**框架，
        ''' 即匹配位点上是**相同字母**而非互补字母。
        ''' </summary>
        ''' <param name="mirnaBase">
        ''' 比对列上 query 侧的碱基，即 miRNA 正向序列的字符（5'->3'）。
        ''' minus 链命中时 BLAST 的 query 恒为 plus，故 qseq 就是 miRNA 的 5'->3' 片段。
        ''' </param>
        ''' <param name="targetRevCompBase">
        ''' 比对列上 subject 侧的碱基。minus 链命中时 BLAST 输出的是 mRNA 片段的反向互补，
        ''' 因此它与 <paramref name="mirnaBase"/> 同向 5'->3'。
        ''' </param>
        ''' <remarks>
        ''' 本框架与 <see cref="ClassifyPair"/> 的框架恰好相差"两侧各取互补"，
        ''' 因此这里两侧取互补后复用 <see cref="ClassifyPair"/>，保证配对规则只有一处定义。
        ''' 展开后的等价规则为：相等 ⇒ WC；(G,A)/(U,C) ⇒ G:U wobble；其余 ⇒ 错配。
        ''' </remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function ClassifyBlastPair(mirnaBase As Char, targetRevCompBase As Char) As PairType
            If mirnaBase = "-"c OrElse targetRevCompBase = "-"c Then
                Return PairType.Gap
            End If

            Return ClassifyPair(ComplementBase(mirnaBase), ComplementBase(targetRevCompBase))
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

        ''' <summary>
        ''' 把 Smith-Waterman 局部比对的**列号**换算成 miRNA 5'->3' 的 **1-based 坐标**。
        ''' </summary>
        ''' <param name="mirna">miRNA 正向序列（取其长度）。</param>
        ''' <param name="hsp">
        ''' 以 miRNA 反向互补链为 query、mRNA 为 subject 得到的局部比对
        ''' （即 <see cref="BestLocalHit"/> 的返回值）。
        ''' </param>
        ''' <param name="i">比对列号（0-based）。</param>
        ''' <returns>该比对列对应的 miRNA 5'->3' 1-based 坐标。</returns>
        ''' <remarks>
        ''' query 是 miRNA 的反向互补，query 下标 j（0-based，由 <c>hsp.fromA</c> 起算）
        ''' 对应 miRNA 下标 <c>L-1-j</c>，故 1-based 的 miRNA 坐标为
        ''' <c>L - j = L - (fromA + i)</c>，其中 L 为 miRNA 长度。
        ''' 
        ''' 直接把 <c>hsp.fromA + i</c> 当作 miRNA 位置会同时犯两重错误：
        ''' 一是 <c>fromA</c> 本身是 0-based 的 query 下标（见 LocalHSPMatch 构造中的
        ''' <c>seq1.Skip(fromA)</c>），二是忽略了反向互补带来的镜像关系。
        ''' </remarks>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function MirnaPosition(mirna As String, hsp As LocalHSPMatch(Of Char), i As Integer) As Integer
            If mirna Is Nothing Then
                Return i + 1
            End If

            Return mirna.Length - (hsp.fromA + i)
        End Function
    End Module
End Namespace

