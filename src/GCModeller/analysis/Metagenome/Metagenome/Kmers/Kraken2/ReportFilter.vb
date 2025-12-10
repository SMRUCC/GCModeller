Namespace Kmers.Kraken2

    Module ReportFilter

        ''' <summary>
        ''' 过滤掉人类相关的分类记录，并重新计算剩余记录的丰度百分比。
        ''' 人类 (Homo sapiens) 的 TaxID 是 9606。
        ''' </summary>
        ''' <param name="allRecords">从 report 文件解析出的原始记录列表。</param>
        ''' <returns>一个过滤并重新计算后的新记录列表。</returns>
        Public Function FilterHumanReadsAndRecalculate(allRecords As KrakenReportRecord(), hostTaxIDs As Long()) As KrakenReportRecord()
            ' 1. 识别所有需要被移除的人类分支的 TaxID
            ' 我们需要一个函数来递归地找到所有子节点的 TaxID
            ' 但在 report 文件中，我们可以通过判断一个节点是否在人类分类路径下来简化
            ' 一个更简单的方法是：找到 "Homo sapiens" 节点，然后移除它和所有它的后代。
            ' 但为了简单起见，我们直接移除 "Homo sapiens" 及其所有子节点。
            ' 在 report 文件中，所有子节点的 ScientificName 都会以父节点的名字开头，但这不总是可靠。
            ' 最可靠的方法是构建树，然后找到 TaxID=9606 的节点，并收集其所有子节点的ID。
            Dim treeRootNodes As KrakenReportTree() = TaxonomyTreeBuilder.BuildTaxonomyTree(allRecords).ToArray

            ' 1. 收集所有需要移除的宿主及其后代的 TaxID
            Dim taxIdsToRemove As New HashSet(Of Long)()

            For Each hostId As Long In hostTaxIDs
                Console.WriteLine($"正在查找宿主节点 TaxID: {hostId}...")
                Dim hostNode As KrakenReportTree = FindNodeByTaxId(treeRootNodes, hostId)

                If hostNode IsNot Nothing Then
                    Console.WriteLine($"找到宿主节点: {hostNode.Data.ScientificName}，正在收集其所有后代 TaxID...")
                    ' 递归收集该宿主节点及其所有子节点的 TaxID
                    GetAllDescendantTaxIds(hostNode, taxIdsToRemove)
                    Console.WriteLine($"已从宿主 {hostNode.Data.ScientificName} 收集到 {taxIdsToRemove.Count} 个相关 TaxID。")
                Else
                    Console.WriteLine($"警告: 未在分类树中找到 TaxID 为 {hostId} 的宿主节点。")
                End If
            Next

            If taxIdsToRemove.Count = 0 Then
                Console.WriteLine("没有找到任何需要过滤的宿主节点，返回原始数据。")
                Return allRecords
            End If

            ' 2. 使用 LINQ 过滤掉这些记录
            ' HashSet 的 Contains 操作非常快，适合用于大数据量的过滤
            Dim filteredRecords As KrakenReportRecord() = allRecords.Where(Function(r) Not taxIdsToRemove.Contains(r.TaxID)).ToArray

            If filteredRecords.Count = 0 Then
                Console.WriteLine("警告: 过滤后没有剩余记录。")
                Return filteredRecords
            End If

            ' 3. 重新计算百分比
            ' 新的总 reads 数是过滤后根节点 的 reads 数
            Dim newTotalClassifiedReads As Long = 0
            Dim rootNode As KrakenReportRecord = filteredRecords.FirstOrDefault(Function(r) r.RankCode = "R")
            If rootNode IsNot Nothing Then
                newTotalClassifiedReads = rootNode.ReadsAtRank
            End If

            If newTotalClassifiedReads = 0 Then
                Console.WriteLine("警告: 过滤后找不到根节点，无法准确计算百分比。丰度可能不准确。")
                ' 作为备选方案，可以取剩余记录中最大的 ReadsAtRank 值，但这不推荐
            Else
                Console.WriteLine($"过滤前的总reads数: {allRecords.FirstOrDefault(Function(r) r.RankCode = "R")?.ReadsAtRank}")
                Console.WriteLine($"过滤后的总reads数: {newTotalClassifiedReads}")

                For Each record In filteredRecords
                    ' 重新计算百分比
                    record.Percentage = (CDbl(record.ReadsAtRank) / CDbl(newTotalClassifiedReads)) * 100.0
                Next
            End If

            Return filteredRecords
        End Function

        ''' <summary>
        ''' 辅助函数：递归地收集一个节点及其所有后代的 TaxID。
        ''' </summary>
        ''' <param name="node">起始节点。</param>
        ''' <param name="taxIdSet">用于收集结果的 HashSet。</param>
        Private Sub GetAllDescendantTaxIds(node As KrakenReportTree, taxIdSet As HashSet(Of Long))
            If node Is Nothing Then
                Return
            End If
            ' 将当前节点的 TaxID 添加到集合中
            taxIdSet.Add(node.Data.TaxID)
            ' 递归处理所有子节点
            For Each child As KrakenReportTree In node.Childs.Values
                GetAllDescendantTaxIds(child, taxIdSet)
            Next
        End Sub

        ''' <summary>
        ''' 辅助函数：在树中根据 TaxID 查找节点 (广度优先搜索)。
        ''' </summary>
        ''' <param name="rootNodes">树的根节点列表。</param>
        ''' <param name="taxIdToFind">要查找的 TaxID。</param>
        ''' <returns>找到的节点，如果未找到则返回 Nothing。</returns>
        Public Function FindNodeByTaxId(rootNodes As KrakenReportTree(), taxIdToFind As Long) As KrakenReportTree
            If rootNodes Is Nothing OrElse rootNodes.Count = 0 Then Return Nothing

            Dim queue As New Queue(Of KrakenReportTree)(rootNodes)

            While queue.Count > 0
                Dim currentNode As KrakenReportTree = queue.Dequeue()
                If currentNode.Data.TaxID = taxIdToFind Then
                    Return currentNode
                End If
                ' 将所有子节点加入队列
                For Each child As KrakenReportTree In currentNode.Childs.Values
                    queue.Enqueue(child)
                Next
            End While

            Return Nothing ' 未找到
        End Function

    End Module
End Namespace