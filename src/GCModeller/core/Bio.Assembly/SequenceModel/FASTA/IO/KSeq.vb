#Region "Microsoft.VisualBasic::0902c3310acfa11c7f68587318de0a3c, core\Bio.Assembly\SequenceModel\FASTA\IO\KSeq.vb"

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

    '   Total Lines: 61
    '    Code Lines: 24 (39.34%)
    ' Comment Lines: 28 (45.90%)
    '    - Xml Docs: 7.14%
    ' 
    '   Blank Lines: 9 (14.75%)
    '     File Size: 2.45 KB


    '     Class KSeq
    ' 
    '         Properties: Seq
    ' 
    '         Function: GetSequenceData, (+2 Overloads) Kmers, ToString
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports System.Xml.Serialization

Namespace SequenceModel.FASTA

    ' The MIT License
    '
    '   Copyright (c) 2008, 2009, 2011 Attractive Chaos <attractor@live.co.uk>
    '
    '   Permission is hereby granted, free of charge, to any person obtaining
    '   a copy of this software and associated documentation files (the
    '   "Software"), to deal in the Software without restriction, including
    '   without limitation the rights to use, copy, modify, merge, publish,
    '   distribute, sublicense, and/or sell copies of the Software, and to
    '   permit persons to whom the Software is furnished to do so, subject to
    '   the following conditions:
    '
    '   The above copyright notice and this permission notice shall be
    '   included in all copies or substantial portions of the Software.
    '
    '   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    '   EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
    '   MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    '   NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
    '   BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
    '   ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
    '   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    '   SOFTWARE.
    '

    ' Last Modified: 05MAR2012 

    ''' <summary>
    ''' k-mer sequence model
    ''' </summary>
    Public Class KSeq : Inherits ISequenceBuilder

        <XmlAttribute> Public Property Seq As String

        Public Overrides Function GetSequenceData() As String
            Return Seq
        End Function

        Public Overrides Function GetHashCode() As Integer
            Return If(Seq Is Nothing, 0, CalculateDirectQuaternaryHashCode(Seq))
        End Function

        Public Overrides Function ToString() As String
            Return If(Name, GetSequenceData())
        End Function

        Public Shared Iterator Function Kmers(seq_str As String, k As Integer) As IEnumerable(Of KSeq)
            For i As Integer = 0 To seq_str.Length - k
                Yield New KSeq With {
                    .Seq = seq_str.Substring(i, length:=k),
                    .Name = (i + 1).ToString
                }
            Next
        End Function

        <MethodImpl(MethodImplOptions.AggressiveInlining)>
        Public Shared Function Kmers(seq As ISequenceProvider, k As Integer) As IEnumerable(Of KSeq)
            Return Kmers(seq_str:=seq.GetSequenceData, k)
        End Function

        ''' <summary>
        ''' 计算DNA k-mer的直接四进制哈希值。
        ''' </summary>
        ''' <param name="kmer">输入的k-mer字符串（例如 "ATCG"）。</param>
        ''' <returns>计算出的哈希值（一个Long整数）。如果输入无效，则抛出异常。</returns>
        ''' <remarks>
        ''' 算法原理：
        ''' 1. 将k-mer的每个碱基映射为一个数字 (A=0, C=1, G=2, T=3)。
        ''' 2. 将整个k-mer串视为一个k位的四进制数。
        ''' 3. 通过迭代方式计算其十进制值：hash = (hash * 4) + current_value。
        ''' 例如，对于 k-mer "ACGT" (k=4):
        '''  A(0) C(1) G(2) T(3)
        '''  哈希值 = (((0 * 4 + 1) * 4 + 2) * 4 + 3) = 27
        ''' </remarks>
        Public Shared Function CalculateDirectQuaternaryHashCode(kmer As String) As Long
            ' 将输入转换为大写，使算法不区分大小写
            Dim upperKmer As String = kmer.ToUpper()
            ' 2. 初始化哈希值
            ' 使用Long类型以防止k值较大时整数溢出 (4^15 > 2^31)
            Dim hashValue As Long = 0

            Static baseInt As New Dictionary(Of Char, Integer) From {{"A"c, 0}, {"T"c, 1}, {"G"c, 2}, {"C"c, 3}}

            ' 3. 遍历k-mer中的每一个碱基
            For Each base As Char In upperKmer
                Dim numericValue As Integer

                ' 4. 将碱基字符转换为对应的数字
                If baseInt.TryGetValue(base, numericValue) Then
                    ' 5. 迭代计算哈希值
                    ' 这相当于将当前哈希值左移两位（乘以4），然后加上新碱基的值
                    hashValue = (hashValue << 2) + numericValue ' 位运算 << 2 等同于 * 4，但通常更快
                Else
                    ' 如果遇到非法字符（非A,C,G,T），则抛出异常
                    Throw New ArgumentException($"k-mer中包含非法字符: '{base}'")
                End If
            Next

            ' 6. 返回最终的哈希值
            Return hashValue
        End Function
    End Class
End Namespace
