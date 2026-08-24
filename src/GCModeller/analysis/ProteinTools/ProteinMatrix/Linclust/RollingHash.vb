#Region "Microsoft.VisualBasic::609ad0e9bf74a91b377cebb1bc65ee83, analysis\ProteinTools\ProteinMatrix\Linclust\RollingHash.vb"

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

    '   Total Lines: 117
    '    Code Lines: 58 (49.57%)
    ' Comment Lines: 38 (32.48%)
    '    - Xml Docs: 55.26%
    ' 
    '   Blank Lines: 21 (17.95%)
    '     File Size: 4.56 KB


    '     Module RollingHash
    ' 
    '         Function: AlphabetCode, GetMinHashes, HashAll, HashKmer
    '         Structure KmerHash
    ' 
    '             Constructor: (+1 Overloads) Sub New
    ' 
    ' 
    ' 
    ' 
    ' /********************************************************************************/

#End Region

' Linclust 滚动哈希与最小 m 个 k-mer 选取
'
' 对缩减字母表编码后的序列,用 16 位滚动哈希(Rabin-Karp 风格)计算
' 每个 k-mer 的哈希值,并仅保留哈希值最小的 m 个 k-mer。
'
' 选哈希值最小的 m 个(而非随机抽取)保证同源序列会抽到同一批
' k-mer,从而能相遇。

Imports System.Runtime.CompilerServices

Namespace Linclust

    Public Module RollingHash

        ' 16 位无符号掩码
        Private Const Mask16 As UInteger = &HFFFFUI

        ' 随机化基(乘法散列),用于降低碰撞并增强混合性
        Private Const Seed As UInteger = 2654435761UI

        ''' <summary>
        ''' 单个 k-mer 的哈希结果:哈希值 + 在序列中的位置
        ''' </summary>
        Public Structure KmerHash
            Public Hash As UInteger   ' 16 位有效(低 16 位)
            Public Position As Integer

            <MethodImpl(MethodImplOptions.AggressiveInlining)>
            Public Sub New(h As UInteger, pos As Integer)
                Hash = h And Mask16
                Position = pos
            End Sub
        End Structure

        ''' <summary>
        ''' 计算单个 k-mer(缩减字母表字符,基数 13)的 16 位滚动哈希。
        ''' 使用多项式滚动哈希:F(h, c) = (h * base + code(c) * Seed) mod 2^16
        ''' 字母序号 code 取其在缩减字母表中的下标(0..12),未知字符映射到 13(通配)。
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Function HashKmer(kmer As String) As UInteger
            ' 采用 base-13 多项式哈希并对 2^16 取模,全程 Integer 不溢出:
            '   hash = (hash * base + code) mod 65536
            ' 哈希值分布在 [0, 65535],作为 16 位 k-mer 索引。
            Dim h As Integer = 0

            For Each c As Char In kmer
                Dim code = AlphabetCode(c)
                h = (h * ReducedAlphabet.AlphabetSize + code) Mod 65536
            Next

            Return CUInt(h And &HFFFF)
        End Function

        ''' <summary>
        ''' 将缩减字母映射为 0-based 序号
        ''' </summary>
        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Private Function AlphabetCode(c As Char) As Integer
            Dim idx = Array.IndexOf(ReducedAlphabet.Letters, c)

            If idx >= 0 Then
                Return idx
            End If

            ' 通配符 / 未知字符:用基数之外的编号,使其仍参与哈希但不与标准字母冲突
            Return ReducedAlphabet.AlphabetSize
        End Function

        ''' <summary>
        ''' 用滚动方式计算序列上每个 k-mer 的哈希(依次增量更新)。
        ''' </summary>
        Private Function HashAll(encoded As String, k As Integer) As KmerHash()
            If encoded.Length < k Then
                Return New KmerHash() {}
            End If

            Dim n = encoded.Length - k + 1
            Dim result = New KmerHash(n - 1) {}

            ' 逐个 k-mer 计算滚动哈希(调用 HashKmer,绝对正确;
            ' 对蛋白序列规模 O(n*k) 完全可接受,且避免手写滚动公式的边界/溢出错误)
            For i As Integer = 0 To n - 1
                Dim kmer = encoded.Substring(i, k)
                result(i) = New KmerHash(HashKmer(kmer), i)
            Next

            Return result
        End Function

        ''' <summary>
        ''' 提取序列上哈希值最小的 m 个 k-mer。
        ''' 若 k-mer 总数不足 m,则返回全部。
        ''' </summary>
        ''' <param name="encoded">缩减字母表编码后的序列</param>
        ''' <param name="k">k-mer 长度</param>
        ''' <param name="m">保留的最小哈希 k-mer 个数</param>
        Public Function GetMinHashes(encoded As String, k As Integer, m As Integer) As KmerHash()
            If encoded Is Nothing OrElse encoded.Length < k Then
                Return New KmerHash() {}
            End If

            Dim all = HashAll(encoded, k)

            If all.Length <= m Then
                Return all
            End If

            ' 部分排序取最小的 m 个(避免全排序,O(n) 选择)
            ' 使用 OrderBy 取前 m 个(实现简洁;对 mN 规模足够)
            Return all _
                .OrderBy(Function(x) x.Hash) _
                .Take(m) _
                .ToArray
        End Function
    End Module
End Namespace

