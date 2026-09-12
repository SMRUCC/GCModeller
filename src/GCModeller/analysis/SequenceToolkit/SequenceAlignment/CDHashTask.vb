#Region "Microsoft.VisualBasic::2c6544630e077a4e9e186802b2e5bf58, analysis\SequenceToolkit\SequenceAlignment\CDHashTask.vb"

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

    '   Total Lines: 36
    '    Code Lines: 27 (75.00%)
    ' Comment Lines: 1 (2.78%)
    '    - Xml Docs: 0.00%
    ' 
    '   Blank Lines: 8 (22.22%)
    '     File Size: 1.26 KB


    ' Class CDHashTask
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Sub: Solve
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Diagnostics
Imports System.Text
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.Data.Repository
Imports Microsoft.VisualBasic.Math.HashMaps.MinHash
Imports Microsoft.VisualBasic.Parallel
Imports SMRUCC.genomics.SequenceModel.FASTA

''' <summary>
''' 序列的 min-hash 签名计算任务（并行）
''' </summary>
''' <remarks>
''' 计算过程已经改造为零堆分配：直接在序列字符串的字符区间上面用滑动窗口计算 MurmurHash，
''' 不会再产生任何 Substring 或者 UTF8 byte[] 临时对象。
''' 
''' 另外，因为「取最小值」操作对于集合与多重集是完全等价的，所以也不再需要 HashSet 对 k-mer 做去重。
''' 在数百万条序列的数据集上面，这两项优化可以避免数十 GB 量级的临时内存分配。
''' </remarks>
Public Class CDHashTask : Inherits VectorTask

    ''' <summary>
    ''' min-hash 签名的长度（哈希函数的数量），与 <c>MinHash.CreateSequenceData</c> 的默认值保持一致
    ''' </summary>
    Public Const NumHashFunctions As Integer = 100

    Friend ReadOnly seqPool As FastaSeq()
    Friend ReadOnly minHash As SequenceItem()
    Friend k As Integer

    Public Sub New(seqPool As FastaSeq(), Optional verbose As Boolean = False, Optional workers As Integer? = Nothing)
        MyBase.New(seqPool.Length, verbose, workers)

        Me.seqPool = seqPool
        Me.minHash = New SequenceItem(seqPool.Length - 1) {}
    End Sub

    Protected Overrides Sub Solve(start As Integer, ends As Integer, cpu_id As Integer)
        Dim sw As Stopwatch = Stopwatch.StartNew

        For i As Integer = start To ends
            ' 每一个工作线程写入的都是互不重叠的下标区间，所以这里不需要加锁，
            ' 也不需要先攒到一个临时的List再拷贝回来
            Me.minHash(i) = New SequenceItem With {
                .ID = i,
                .Signature = CreateSignature(seqPool(i).SequenceData, k)
            }
        Next

        Call $"[cdhit] min-hash worker {cpu_id}: sequences [{start}, {ends}], elapsed {sw.ElapsedMilliseconds} ms".debug
    End Sub

    ''' <summary>
    ''' 直接在序列字符串上面通过滑动窗口生成 min-hash 签名
    ''' </summary>
    ''' <param name="seq">序列数据</param>
    ''' <param name="k">k-mer 的长度</param>
    ''' <returns>长度固定为 <see cref="NumHashFunctions"/> 的 min-hash 签名</returns>
    ''' <remarks>
    ''' 与原实现 <c>KSeq.KmerSpans(seq, k).CreateSequenceData(id, 100)</c> 的数值结果完全一致：
    ''' 
    ''' 1. 原实现先把 k-mer 放进 HashSet 去重再取最小值，而最小值对于集合与多重集是完全等价的，
    '''    所以可以直接对每一个 k-mer 出现位置取最小值，省掉 HashSet 与 Substring；
    ''' 2. 对于 ASCII 序列，UTF8 编码的结果就是字符本身的值，
    '''    所以可以直接在字符区间上面计算哈希，省掉 byte[] 分配。
    ''' </remarks>
    Friend Shared Function CreateSignature(seq As String, k As Integer) As UInteger()
        Dim signature As UInteger() = New UInteger(NumHashFunctions - 1) {}

        For i As Integer = 0 To signature.Length - 1
            signature(i) = UInteger.MaxValue
        Next

        If String.IsNullOrEmpty(seq) OrElse k <= 0 Then
            Return signature
        End If

        Dim L As Integer = seq.Length

        If L < k Then
            ' 没有足够长度的 k-mer，签名保持全部为最大值
            Return signature
        End If

        If isAscii(seq) Then
            ' ASCII 快速路径：直接在字符区间上面计算哈希，没有任何堆分配
            For i As Integer = 0 To L - k
                For j As Integer = 0 To NumHashFunctions - 1
                    Dim hashVal As UInteger = MurmurHash.MurmurHashCode3_x86_32(seq, i, k, CUInt(j))

                    If hashVal < signature(j) Then
                        signature(j) = hashVal
                    End If
                Next
            Next
        Else
            ' 非 ASCII 回退路径：保持与原来的实现完全一致的「取子串 + UTF8 编码」方式
            For i As Integer = 0 To L - k
                Dim buffer As Byte() = Encoding.UTF8.GetBytes(seq.Substring(i, k))

                For j As Integer = 0 To NumHashFunctions - 1
                    Dim hashVal As UInteger = MurmurHash.MurmurHashCode3_x86_32(buffer, CUInt(j))

                    If hashVal < signature(j) Then
                        signature(j) = hashVal
                    End If
                Next
            Next
        End If

        Return signature
    End Function

    ''' <summary>
    ''' 序列之中是否全部都是 ASCII 字符？
    ''' </summary>
    ''' <param name="seq"></param>
    ''' <returns></returns>
    Private Shared Function isAscii(seq As String) As Boolean
        For i As Integer = 0 To seq.Length - 1
            If AscW(seq(i)) > &H7F Then
                Return False
            End If
        Next

        Return True
    End Function
End Class
