#Region "Microsoft.VisualBasic::9ddaf0b832792bb20510cd82b4fbd470, analysis\ProteinTools\ProteinMatrix\Linclust\ReducedAlphabet.vb"

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

    '   Total Lines: 97
    '    Code Lines: 46 (47.42%)
    ' Comment Lines: 36 (37.11%)
    '    - Xml Docs: 44.44%
    ' 
    '   Blank Lines: 15 (15.46%)
    '     File Size: 3.42 KB


    '     Module ReducedAlphabet
    ' 
    '         Properties: Letters
    ' 
    '         Constructor: (+1 Overloads) Sub New
    '         Function: Encode, Map
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' Linclust 缩减字母表(Reduced Alphabet)
'
' 将 20 种标准氨基酸合并为 13 个字母,提高突变容忍下的 k-mer 命中率。
' 该表基于 BLOSUM62 矩阵迭代合并"互信息损失最小"的字母对得到,
' 是 Linclust / MMseqs2 使用的标准 13 字母缩减表。
'
' 映射关系(原始字母 -> 缩减字母,共 13 组):
'   A           -> A
'   C           -> C
'   D, N        -> B   (酸性/酰胺)
'   E, Q        -> Z   (酸性/酰胺)
'   F, Y, W     -> F   (芳香族)
'   G           -> G
'   H, R, K     -> H   (碱性 + 组氨酸)
'   I, L, M, V  -> I   (疏水/小侧链)
'   P           -> P
'   S, T        -> S   (羟基)
' 非标准字符(如 X、B、Z、U、O 等)映射为通配符 '.',以容忍未知残基。

Imports System.Runtime.CompilerServices

Namespace Linclust

    Public Module ReducedAlphabet

        ''' <summary>
        ''' 缩减字母表大小(基数,用于滚动哈希)
        ''' </summary>
        Public Const AlphabetSize As Integer = 13

        ''' <summary>
        ''' 非标准 / 未知氨基酸的缩减映射(通配符)
        ''' </summary>
        Public Const Wildcard As Char = "."c

        ''' <summary>
        ''' 标准 13 字母缩减表的字母集合
        ''' </summary>
        Public ReadOnly Property Letters As Char() = {"A"c, "B"c, "C"c, "F"c, "G"c, "H"c, "I"c, "P"c, "S"c, "T"c, "W"c, "Y"c, "Z"c}

        ' 原始氨基酸 -> 缩减字母 的查找表(大写)
        Private ReadOnly alphabetMap As Dictionary(Of Char, Char)

        Sub New()
            alphabetMap = New Dictionary(Of Char, Char) From {
                {"A"c, "A"c},
                {"C"c, "C"c},
                {"D"c, "B"c}, {"N"c, "B"c},
                {"E"c, "Z"c}, {"Q"c, "Z"c},
                {"F"c, "F"c}, {"Y"c, "F"c}, {"W"c, "F"c},
                {"G"c, "G"c},
                {"H"c, "H"c}, {"R"c, "H"c}, {"K"c, "H"c},
                {"I"c, "I"c}, {"L"c, "I"c}, {"M"c, "I"c}, {"V"c, "I"c},
                {"P"c, "P"c},
                {"S"c, "S"c}, {"T"c, "S"c}
            }
        End Sub

        ''' <summary>
        ''' 将单个氨基酸字符映射到缩减字母表。
        ''' 非标准字符返回通配符 <see cref="Wildcard"/>。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Map(aa As Char) As Char
            Dim key = Char.ToUpper(aa)

            If alphabetMap.ContainsKey(key) Then
                Return alphabetMap(key)
            Else
                ' 已为缩减字母(B/Z/F/H/I/S 等)直接保留
                If Array.IndexOf(Letters, key) >= 0 Then
                    Return key
                End If

                Return Wildcard
            End If
        End Function

        ''' <summary>
        ''' 将整条蛋白序列编码为缩减字母表序列。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function Encode(seq As String) As String
            If seq Is Nothing Then
                Return ""
            End If

            Dim chars As Char() = New Char(seq.Length - 1) {}

            For i As Integer = 0 To seq.Length - 1
                chars(i) = Map(seq(i))
            Next

            Return New String(chars)
        End Function
    End Module
End Namespace

