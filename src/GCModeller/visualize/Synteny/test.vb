#Region "Microsoft.VisualBasic::9c6e44d532d9e06f2526de46813ee00d, visualize\Synteny\test.vb"

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

    ' Module test
    ' 
    '     Function: RefLociRange, TupleMapping
    ' 
    '     Sub: batch, Main
    ' 
    ' Class align
    ' 
    '     Properties: bit_score, evlaue, indent, length, mismatch
    '                 qend, qlength, Qseqid, qstart, send
    '                 Si, Sseid, sstart
    ' 
    '     Function: ToString
    ' 
    ' /********************************************************************************/

#End Region

Imports System.Runtime.CompilerServices
Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.ComponentModel.Ranges
Imports Microsoft.VisualBasic.ComponentModel.Ranges.Model
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports Microsoft.VisualBasic.Language.UnixBash
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application.NtMapping

Module test

    Sub Main()

        Call batch()

        Pause()



        Dim a = "E:\2018-7-10\高粱对比筛选结果.csv".LoadCsv(Of align)
        Dim b = "E:\2018-7-10\玉米比对筛选结果.csv".LoadCsv(Of align)
        Dim data = TupleMapping(a, b).ToArray
        '  Dim regions = data.PopulateRegions(stepOffset:=(5000, 5000)).ToArray

        Call data.SaveTo("./dddd.csv")

        Pause()
    End Sub

    Sub batch()

        Dim pathList = (ls - l - r - "*.gff" <= "D:\OneDrive\20181020\extract").ToArray
        Dim gffs = pathList.ToDictionary(Function(path) path.BaseName)

        For Each dir As String In ls - l - lsDIR <= "D:\OneDrive\20181020\mappings"

            Dim queryName = dir.BaseName.Replace(".Blastn-BlastnMaps", "")

            For Each map In ls - l - "*.csv" <= dir
                Dim refname = map.BaseName

                Call Apps.SyntenyVisual.PlotMapping(map, gffs(queryName), gffs(refname), ribbon:="RdYlBu:c8", grep:="tokens | first")
            Next

        Next

        Pause()
    End Sub

    ''' <summary>
    ''' 比较两个没有参考基因组的测序结果之间的共线性
    ''' 因为contig没有参考基因组，所以无法知道contig的位置
    ''' 但是却可以通过subject来了解相对位置
    ''' 两个列表必须要具有共同的subject比对结果
    ''' </summary>
    ''' <param name="list1"></param>
    ''' <param name="list2"></param>
    ''' <returns></returns>
    Public Iterator Function TupleMapping(list1 As IEnumerable(Of align),
                                          list2 As IEnumerable(Of align),
                                          Optional tolerance% = 1000) As IEnumerable(Of align)

        ' assembly 1        subject         assembly2        subject
        ' contig1 [1, 2000]  [5000, 7000]   contig1 [1, 600]  [5000, 5500]
        '
        Dim lociCompares As Comparison(Of Double) =
            Function(a, b) As Integer
                If Math.Abs(a - b) <= tolerance Then
                    Return 0
                ElseIf a < b Then
                    Return -1
                Else
                    Return 1
                End If
            End Function

        ' 首先需要找出subject之间的重叠的区域
        ' 然后将重叠区域的assembly1和assembly2的contig都拿出来即可
        Dim aTree As New AVLTree(Of Double, align)(lociCompares)
        Dim bTree As New AVLTree(Of Double, align)(lociCompares)

        For Each fragment In list1
            Call aTree.Add(
                key:=(fragment.sstart + fragment.send) / 2,
                value:=fragment,
                valueReplace:=False
            )
        Next

        For Each fragment In list2
            Call bTree.Add(
                key:=(fragment.sstart + fragment.send) / 2,
                value:=fragment,
                valueReplace:=False
            )
        Next

        Dim bfragments = bTree.root.PopulateNodes.ToArray

        For Each blist As align() In bfragments.Select(Function(tree) tree.Members)
            For Each b In blist.Select(AddressOf RefLociRange).Union
                Dim i# = (b.Min + b.Max) / 2
                Dim align As align() = aTree.root _
                    .Find(i, lociCompares) _
                    .Members

                If align.IsNullOrEmpty Then
                    ' 空的，这意味着b在a上面没有同源的区域
                Else
                    ' 有一段同源的区域
                    For Each region In align.Select(AddressOf RefLociRange).Union
                        Yield New align With {
                            .qstart = b.Min,
                            .qend = b.Max,
                            .sstart = region.Min,
                            .send = region.Max
                        }
                    Next
                End If
            Next

            ' aVisited += i
        Next

        ' 删掉所有访问过的节点，则在atree之中所剩余的节点都是a在b之中
        ' 找不到同源区域的contig片段
        ' For Each i As Double In aVisited
        ' Call aTree.Remove(key:=i)
        ' Next
    End Function

    <Extension>
    Public Function RefLociRange(align As align) As IntRange
        Return New IntRange(align.sstart, align.send)
    End Function
End Module

Public Class align : Implements IMapping

    Public Property Qseqid As String Implements IMapping.Qname
    Public Property Sseid As String Implements IMapping.Sname
    Public Property indent As Double
    Public Property mismatch As Double
    Public Property qlength As Double
    Public Property length As Double
    Public Property Si As Double
    Public Property qstart As Integer Implements IMapping.Qstart
    Public Property qend As Integer Implements IMapping.Qstop
    Public Property sstart As Integer Implements IMapping.Sstart
    Public Property send As Integer Implements IMapping.Sstop
    Public Property evlaue As Double

    <Column("bit-score")>
    Public Property bit_score As Double

    Public Overrides Function ToString() As String
        Return $"[{sstart}, {send}]"
    End Function

End Class
