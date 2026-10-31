#Region "Microsoft.VisualBasic::89f7bb51f1a1f92f58368ef67c7f843a, analysis\SequenceToolkit\SequenceAlignment\Diamond\ReducedAlphabet.vb"

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

    '   Total Lines: 91
    '    Code Lines: 39 (42.86%)
    ' Comment Lines: 40 (43.96%)
    '    - Xml Docs: 42.50%
    ' 
    '   Blank Lines: 12 (13.19%)
    '     File Size: 3.35 KB


    '     Module ReducedAlphabet
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Encode, Map
    ' 
    ' 
    ' /********************************************************************************/

#End Region

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
        ''' 直接用 256 项字节表替代原先的 Dictionary 两次哈希查找:
        ''' 该映射位于 Hamming 初筛与种子编码的最内层循环,是逐残基调用的热路径。
        ''' </summary>
        Private ReadOnly classTable As Byte()

        Sub New()
            classTable = New Byte(255) {}

            ' 默认全部落入通配类
            For i As Integer = 0 To 255
                classTable(i) = CByte(WildcardClass)
            Next

            Call SetClass("A"c, "G"c)
            Call SetClass("S"c, "T"c)
            Call SetClass("C"c)
            Call SetClass("D"c, "E"c)
            Call SetClass("N"c, "Q"c)
            Call SetClass("K"c, "R"c)
            Call SetClass("H"c)
            Call SetClass("I"c, "L"c, "M"c, "V"c)
            Call SetClass("F"c, "Y"c, "W"c)
            Call SetClass("P"c)
        End Sub

        Private Sub SetClass([class] As Integer, ParamArray residues As Char())
            For Each aa As Char In residues
                classTable(AscW(Char.ToUpper(aa))) = CByte([class])
                classTable(AscW(Char.ToLower(aa))) = CByte([class])
            Next
        End Sub

        ''' <summary>
        ''' 将单个氨基酸字符映射到缩减类编号 (0..10)。
        ''' 非标准 / 未知字符返回 <see cref="WildcardClass"/>。
        ''' 大小写不敏感。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Map(aa As Char) As Integer
            Dim code As Integer = AscW(aa)

            If code < 0 OrElse code > 255 Then
                Return WildcardClass
            End If

            Return classTable(code)
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

        ''' <summary>
        ''' 将整条蛋白序列写入指定的字节缓冲区(填充式编码,无额外分配)。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Sub EncodeTo(seq As String, buffer As Byte(), offset As Integer)
            For i As Integer = 0 To seq.Length - 1
                buffer(offset + i) = classTable(AscW(seq(i)))
            Next
        End Sub
    End Module
End Namespace

