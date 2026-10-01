#Region "Microsoft.VisualBasic::MergeCloseModules, annotations\WGCNA\WGCNA\Algorithm\MergeCloseModules.vb"

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

#End Region

Imports Microsoft.VisualBasic.DataMining.HierarchicalClustering
Imports Microsoft.VisualBasic.Linq
Imports SMRUCC.genomics.Analysis.HTS.DataFrame
Imports std = System.Math

''' <summary>
''' 跨块模块合并（对应 R WGCNA 的 <c>mergeCloseModules</c>）
''' </summary>
''' <remarks>
''' 分块计算的固有代价是：同一个生物学模块里的基因可能被预聚类分到不同的块，
''' 于是会在两个块里各自形成一个"半个模块"。
''' 
''' <para>
''' R 的补救办法是 <c>mergeCloseModules</c>：在<b>全体基因</b>上重新计算每个模块的
''' 特征基因，用特征基因之间的相关性作为模块间的不相似度
''' （<c>dissim = 1 - cor(ME_i, ME_j)</c>），再做一次 average-linkage 层次聚类，
''' 并按 <c>mergeCutHeight</c> 剪切，凡落在同一组的模块合并为一个。
''' </para>
''' 
''' <para>
''' 这一步的规模只有 O(K^2 · nSamples)（K 为模块数），相对块内的 O(B^3) TOM 可以忽略。
''' </para>
''' </remarks>
Public Module MergeCloseModules

    ''' <summary>
    ''' 合并特征基因高度相似的模块
    ''' </summary>
    ''' <param name="samples">完整表达矩阵（用于在全基因范围内重算特征基因）</param>
    ''' <param name="modules">跨块汇总的模块字典（模块名 → 基因列表）</param>
    ''' <param name="cutHeight">
    ''' 合并的高度阈值。特征基因不相似度（1 - 相关系数）小于该值的模块会被合并。
    ''' 默认 0.15，与 R WGCNA 的 <c>mergeCutHeight</c> 一致。
    ''' </param>
    ''' <returns>合并后的模块字典；模块名按大小降序重新编号为 <c>M1, M2, ...</c></returns>
    Public Function Merge(samples As Matrix,
                          modules As Dictionary(Of String, String()),
                          Optional cutHeight As Double = 0.15) As Dictionary(Of String, String())

        Dim names As String() = modules.Keys.ToArray()
        Dim K As Integer = names.Length

        If K <= 1 Then
            Return Renumber(modules)
        End If

        ' 在全体基因上重算每个模块的特征基因
        Dim eigengenes As Double()() = New Double(K - 1)() {}

        For i As Integer = 0 To K - 1
            eigengenes(i) = ModuleEigengene.Compute(samples, modules(names(i)))
        Next

        ' 模块间不相似度
        Dim dissim As Double()() = New Double(K - 1)() {}

        For i As Integer = 0 To K - 1
            Dim row(K - 1) As Double

            For j As Integer = 0 To K - 1
                If i = j Then
                    row(j) = 0
                Else
                    row(j) = 1 - ModuleEigengene.Pearson(eigengenes(i), eigengenes(j))
                End If
            Next

            dissim(i) = row
        Next

        ' 层次聚类 + 固定高度剪切
        Dim alg As ClusteringAlgorithm = New DefaultClusteringAlgorithm()
        Dim tree As Cluster = alg.performClustering(dissim, names, New AverageLinkageStrategy())
        Dim groups As Dictionary(Of String, String()) = CutByHeight(tree, cutHeight)

        ' 按分组合并基因
        Dim merged As New Dictionary(Of String, List(Of String))

        For Each kv In groups
            Dim list As New List(Of String)

            For Each modName As String In kv.Value
                If modules.ContainsKey(modName) Then
                    Call list.AddRange(modules(modName))
                End If
            Next

            If list.Count > 0 Then
                merged(kv.Key) = list
            End If
        Next

        Dim result As New Dictionary(Of String, String())

        For Each kv In merged
            result(kv.Key) = kv.Value.ToArray()
        Next

        Return Renumber(result)
    End Function

    ''' <summary>
    ''' 自顶向下收集「首次低于剪切高度」的子树
    ''' </summary>
    Private Iterator Function CollectByHeight(node As Cluster, cutHeight As Double) As IEnumerable(Of Cluster)
        If node.isLeaf OrElse node.DistanceValue <= cutHeight Then
            Yield node
        Else
            For Each child As Cluster In node.Children
                For Each c As Cluster In CollectByHeight(child, cutHeight)
                    Yield c
                Next
            Next
        End If
    End Function

    ''' <summary>
    ''' 剪切树并转成「组名 → 模块名列表」
    ''' </summary>
    Private Function CutByHeight(tree As Cluster, cutHeight As Double) As Dictionary(Of String, String())
        Dim result As New Dictionary(Of String, String())
        Dim i As Integer = 0

        For Each c As Cluster In CollectByHeight(tree, cutHeight)
            i += 1
            result("G" & i) = c.LeafNames.ToArray()
        Next

        Return result
    End Function

    ''' <summary>
    ''' 按模块大小降序重新编号为 M1, M2, ...
    ''' </summary>
    Private Function Renumber(modules As Dictionary(Of String, String())) As Dictionary(Of String, String())
        Dim ordered = modules _
            .OrderByDescending(Function(kv) kv.Value.Length) _
            .ToArray()
        Dim result As New Dictionary(Of String, String())
        Dim i As Integer = 0

        For Each kv In ordered
            i += 1
            result("M" & i) = kv.Value
        Next

        Return result
    End Function
End Module
