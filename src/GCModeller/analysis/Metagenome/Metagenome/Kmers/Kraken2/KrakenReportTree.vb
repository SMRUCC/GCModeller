#Region "Microsoft.VisualBasic::f8a4bf00cb03eb6d0f24e71eb6662f4f, analysis\Metagenome\Metagenome\Kmers\Kraken2\KrakenReportTree.vb"

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

    '   Total Lines: 100
    '    Code Lines: 64 (64.00%)
    ' Comment Lines: 18 (18.00%)
    '    - Xml Docs: 44.44%
    ' 
    '   Blank Lines: 18 (18.00%)
    '     File Size: 4.41 KB


    '     Class KrakenReportTree
    ' 
    '         Constructor: (+2 Overloads) Sub New
    '         Function: BuildTree, GetQuantifyData
    ' 
    '     Module TaxonomyTreeBuilder
    ' 
    '         Function: BuildTaxonomyTree
    ' 
    ' 
    ' /********************************************************************************/

#End Region

Imports Microsoft.VisualBasic.Data.GraphTheory

Namespace Kmers.Kraken2

    Public Class KrakenReportTree : Inherits Tree(Of KrakenReportRecord)

        Sub New(data As KrakenReportRecord)
            Me.Data = data
            Me.label = data.TaxID.ToString
        End Sub

        Sub New()
        End Sub

        Public Iterator Function GetQuantifyData() As IEnumerable(Of KrakenReportRecord)
            If IsLeaf Then
                Yield Data
            Else
                For Each child As Tree(Of KrakenReportRecord) In Childs.Values
                    For Each data As KrakenReportRecord In DirectCast(child, KrakenReportTree).GetQuantifyData
                        Yield data
                    Next
                Next
            End If
        End Function

        Public Shared Function BuildTree(nodes As IEnumerable(Of KrakenReportRecord)) As KrakenReportTree
            Dim root As New KrakenReportTree(New KrakenReportRecord With {.Percentage = 100, .TaxID = 0})

            For Each rootNode As KrakenReportTree In TaxonomyTreeBuilder.BuildTaxonomyTree(nodes)
                Call root.Childs.Add(rootNode)
            Next

            Return root
        End Function
    End Class

    Module TaxonomyTreeBuilder

        ''' <summary>
        ''' 定义分类等级的层级深度，用于判断父子关系
        ''' </summary>
        ReadOnly RankHierarchy As New Dictionary(Of String, Integer) From {
            {"U", -1}, {"R", 0}, {"D", 1}, {"K", 2}, {"P", 3},
            {"C", 4}, {"O", 5}, {"F", 6}, {"G", 7}, {"S", 8},
            {"S1", 9} ' 处理亚种等更细的级别
        }

        ''' <summary>
        ''' 将扁平的 KrakenReportRecord 列表构建成树状结构。
        ''' </summary>
        ''' <param name="flatRecords">从 report 文件解析出的记录列表。</param>
        ''' <returns>代表树根节点的列表 (通常只有一个 root 节点)。</returns>
        Public Iterator Function BuildTaxonomyTree(flatRecords As IEnumerable(Of KrakenReportRecord)) As IEnumerable(Of KrakenReportTree)
            ' 使用一个栈来跟踪当前路径上的节点
            Dim nodeStack As New Stack(Of KrakenReportTree)()
            Dim skip_warns As New List(Of String)

            For Each record In flatRecords
                Dim newNode As New KrakenReportTree(record) With {
                    .Childs = New Dictionary(Of String, Tree(Of KrakenReportRecord))
                }

                ' 检查当前记录的 RankCode 是否在我们定义的层级中
                If Not RankHierarchy.ContainsKey(record.RankCode) Then
                    ' 如果是未知等级，跳过或可以抛出异常
                    ' Call $"警告: 未知的 RankCode '{record.RankCode}' 在记录 '{record.ScientificName}' 中，已跳过。".warning
                    Call skip_warns.Add($"{record.RankCode}.{record.ScientificName}")
                    Continue For
                End If
                Dim currentRankLevel As Integer = RankHierarchy(record.RankCode)

                ' --- 核心逻辑：找到父节点 ---
                ' 从栈中弹出所有等级大于或等于当前节点的节点。
                ' 这样做之后，栈顶的节点（如果存在）就是当前节点的父节点。
                While nodeStack.Count > 0 AndAlso RankHierarchy(nodeStack.Peek().Data.RankCode) >= currentRankLevel
                    nodeStack.Pop()
                End While

                ' 如果栈不为空，则栈顶元素是父节点
                If nodeStack.Count > 0 Then
                    Dim parentNode As KrakenReportTree = nodeStack.Peek()
                    newNode.Parent = parentNode
                    parentNode.Childs.Add(newNode)
                Else
                    ' 如果栈为空，说明这是一个根节点
                    Yield newNode
                End If

                ' 将当前节点压入栈中，作为后续节点的潜在父节点
                nodeStack.Push(newNode)
            Next

            If skip_warns.Any Then
                Call $"found {skip_warns.Count} unknown rankcode records: {skip_warns.JoinBy("; ")}, these records has been skipped".warning
            End If
        End Function
    End Module

End Namespace
