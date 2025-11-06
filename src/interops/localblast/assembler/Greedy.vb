#Region "Microsoft.VisualBasic::5a1367a3dffdffa1a946729b7d914e1a, localblast\assembler\Greedy.vb"

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

    ' Module Greedy
    ' 
    '     Function: DeNovoAssembly, unionFasta
    ' 
    ' Class BitsPairwiseAligner
    ' 
    '     Constructor: (+1 Overloads) Sub New
    '     Function: align
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.DynamicProgramming
Imports Microsoft.VisualBasic.Language
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.SequenceModel
Imports SMRUCC.genomics.SequenceModel.FASTA

Public Module Greedy

    ' Greedy algorithm
    ' Given a Set Of sequence fragments the Object Is To find the shortest common supersequence.

    ' Сalculate pairwise alignments Of all fragments.
    ' Choose two fragments With the largest overlap.
    ' Merge chosen fragments.
    ' Repeat step 2 And 3 until only one fragment Is left.
    ' The result need Not be an optimal solution To the problem.

    ''' <summary>
    ''' 使用贪婪SCS算法进行测序数据的从头装配
    ''' </summary>
    ''' <param name="reads"></param>
    ''' <returns></returns>
    ''' <remarks>
    ''' 使用二叉树+SmithWaterman算法利用<see cref="SCS"/>进行基因组的从头装配
    ''' </remarks>
    <Extension>
    Public Function DeNovoAssembly(reads As IEnumerable(Of FastaSeq), Optional identity# = 0.7, Optional similar# = 0.3) As IEnumerable(Of FastaSeq)
        Dim readsList As FastaSeq() = reads.ToArray
        Dim avltree As New AVLClusterTree(Of Bits)(AddressOf New BitsPairwiseAligner(identity, similar).align, Function(fa) fa.GetSequenceData)
        Dim clusters As ClusterKey(Of Bits)()
        Dim n As Integer = readsList.Length
        Dim start&
        Dim cycles As i32 = 1

        Do While True
            start = App.ElapsedMilliseconds
            avltree.Clear()

            For Each read As Bits In readsList.Select(AddressOf Bits.FromNucleotide)
                Call avltree.Add(read)
            Next

            ' 然后合并每一个cluster中的reads为contig
            clusters = avltree.AsEnumerable.ToArray
            readsList = clusters _
                .AsParallel _
                .Select(AddressOf unionFasta) _
                .ToArray

            If readsList.Length <> n Then
                n = readsList.Length
            Else
                Exit Do
            End If

            Dim contigSize = readsList.Select(Function(fa) fa.Length).ToArray

            Call $" #cycle_{++cycles}  [{App.ElapsedMilliseconds - start}ms] {readsList.Length} reads left, average contig size={contigSize.Average} bytes in range [{contigSize.Min}, {contigSize.Max}].".debug
        Loop

        Return readsList
    End Function

    Private Function unionFasta(cluster As ClusterKey(Of Bits)) As FastaSeq
        Dim nucl As New List(Of String)

        For i As Integer = 0 To cluster.NumberOfKey - 1
            nucl.Add(cluster.Item(i).GetSequenceData)
        Next

        Dim scsUnion$

        If nucl.Count = 1 Then
            scsUnion = nucl(Scan0)
        Else
            scsUnion = (nucl.ShortestCommonSuperString)(Scan0)
        End If

        Dim unionFa As New FastaSeq With {
            .Headers = {cluster.Item(Scan0).title},
            .SequenceData = scsUnion
        }

        Return unionFa
    End Function
End Module

Public Class BitsPairwiseAligner

    ReadOnly identity, similar As Double

    ''' <summary>
    ''' 因为来源不同的序列片段的起始端或者末端都可能存在相同的区域, 
    ''' 所以得分阈值设置得过低可能会将不同来源的片段组装在一块, 
    ''' 产生错误的装配结果
    ''' </summary>
    ''' <param name="identityMinW"></param>
    ''' <param name="similarMinW"></param>
    Sub New(Optional identityMinW As Double = 0.85, Optional similarMinW As Double = 0.3)
        Me.similar = similarMinW
        Me.identity = identityMinW
    End Sub

    ''' <summary>
    ''' do pairwise alignment of two reads
    ''' </summary>
    ''' <param name="a"></param>
    ''' <param name="b"></param>
    ''' <returns></returns>
    Public Function align(a As Bits, b As Bits) As Integer
        ' 在这里不可以使用smith-waterman比对来进行比较,
        ' 假若测序数据是16sRNA, 因为16sRNA高度保守, 
        ' 使用Smith-waterman算法比较会出现reads几乎全部集中在root节点的问题
        Dim overlapSize = a.OverlapSize(b)
        Dim minLen = Math.Min(a.length, b.length)

        If overlapSize >= minLen * identity Then
            Return 0
        ElseIf overlapSize >= minLen * similar Then
            Return 1
        Else
            Return -1
        End If
    End Function
End Class
