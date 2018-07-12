Imports Microsoft.VisualBasic.ComponentModel.Algorithm.BinaryTree
Imports Microsoft.VisualBasic.Data.csv
Imports Microsoft.VisualBasic.Data.csv.StorageProvider.Reflection
Imports SMRUCC.genomics.Interops.NCBI.Extensions.LocalBLAST.Application
Imports SMRUCC.genomics.Visualize.SyntenyVisualize
Imports Microsoft.VisualBasic.Language

Module test

    Sub Main()
        Dim data = "E:\2018-7-10\高粱对比筛选结果.csv".LoadCsv(Of align)
        Dim regions = data.PopulateRegions(stepOffset:=(1, 500)).ToArray

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

        For Each fragment In list1
            Call aTree.Add(
                key:=(fragment.sstart + fragment.send) / 2,
                value:=fragment,
                valueReplace:=False
            )
        Next

        Dim aVisited As New List(Of Double)

        For Each b As align In list2
            Dim i# = (b.sstart + b.send) / 2
            Dim align As align() = aTree.root _
                .Find(i, lociCompares) _
                .ClusterMembers

            If align.IsNullOrEmpty Then
                ' 空的，这意味着b在a上面没有同源的区域
            Else
                ' 有一段同源的区域

            End If

            aVisited += i
        Next

        ' 删掉所有访问过的节点，则在atree之中所剩余的节点都是a在b之中
        ' 找不到同源区域的contig片段
        For Each i As Double In aVisited
            Call aTree.Remove(key:=i)
        Next

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

End Class