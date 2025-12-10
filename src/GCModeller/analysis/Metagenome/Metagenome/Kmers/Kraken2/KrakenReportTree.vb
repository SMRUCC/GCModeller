Imports Microsoft.VisualBasic.Data.GraphTheory

Namespace Kmers.Kraken2

    Public Class KrakenReportTree : Inherits Tree(Of KrakenReportRecord)

        Sub New(data As KrakenReportRecord)
            Me.Data = data
        End Sub

        Sub New()
        End Sub

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

            For Each record In flatRecords
                Dim newNode As New KrakenReportTree(record)

                ' 检查当前记录的 RankCode 是否在我们定义的层级中
                If Not RankHierarchy.ContainsKey(record.RankCode) Then
                    ' 如果是未知等级，跳过或可以抛出异常
                    Call $"警告: 未知的 RankCode '{record.RankCode}' 在记录 '{record.ScientificName}' 中，已跳过。".warning
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
        End Function
    End Module

End Namespace