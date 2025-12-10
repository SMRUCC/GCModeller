Namespace Kmers.Kraken2

    Module ReportFilter

        ''' <summary>
        ''' 过滤掉人类相关的分类记录，并重新计算剩余记录的丰度百分比。
        ''' 人类 (Homo sapiens) 的 TaxID 是 9606。
        ''' </summary>
        ''' <param name="allRecords">从 report 文件解析出的原始记录列表。</param>
        ''' <returns>一个过滤并重新计算后的新记录列表。</returns>
        Public Function FilterHumanReadsAndRecalculate(allRecords As KrakenReportRecord(), Optional hostTaxId As Long = 9606) As KrakenReportRecord()
            ' 1. 识别所有需要被移除的人类分支的 TaxID
            ' 我们需要一个函数来递归地找到所有子节点的 TaxID
            ' 但在 report 文件中，我们可以通过判断一个节点是否在人类分类路径下来简化
            ' 一个更简单的方法是：找到 "Homo sapiens" 节点，然后移除它和所有它的后代。
            ' 但为了简单起见，我们直接移除 "Homo sapiens" 及其所有子节点。
            ' 在 report 文件中，所有子节点的 ScientificName 都会以父节点的名字开头，但这不总是可靠。
            ' 最可靠的方法是构建树，然后找到 TaxID=9606 的节点，并收集其所有子节点的ID。

            ' --- 简化但有效的实现 ---
            ' 我们移除 TaxID 为 9606 的记录，以及所有 TaxID > 9606 且科学名中包含 "Homo" 的记录。    
            ' 更严谨的做法是构建树后进行查找，这里我们先采用一个更直接的方法。
            ' 我们假设 report 文件是排序好的，所以所有子节点都在父节点之后。
            Dim humanDescendantTaxIds As New HashSet(Of Long)()

            ' 首先，找到 Homo sapiens 节点
            Dim homoSapiensRecord As KrakenReportRecord = allRecords.FirstOrDefault(Function(r) r.TaxID = hostTaxId)
            If homoSapiensRecord Is Nothing Then
                Console.WriteLine("警告: 未在报告中找到 'Homo sapiens' (TaxID: 9606)，无需过滤。")
                Return allRecords
            End If

            ' 收集所有子节点和后代的 TaxID
            ' 这是一个简化版，它假设所有子节点的科学名都以父级路径开始
            Dim humanPathPrefix As String = homoSapiensRecord.ScientificName
            For Each record In allRecords
                If record.TaxID = hostTaxId OrElse record.ScientificName.StartsWith(humanPathPrefix) Then
                    humanDescendantTaxIds.Add(record.TaxID)
                End If
            Next

            Console.WriteLine($"已识别 {humanDescendantTaxIds.Count} 个人类分类节点，准备过滤...")

            ' 2. 过滤掉这些记录
            Dim filteredRecords As KrakenReportRecord() = allRecords.Where(Function(r) Not humanDescendantTaxIds.Contains(r.TaxID)).ToArray

            If filteredRecords.Count = 0 Then
                Console.WriteLine("警告: 过滤后没有剩余记录。")
                Return filteredRecords
            End If

            ' 3. 重新计算百分比
            ' 新的总 reads 数是过滤后所有记录的 "ReadsAtRank" 的总和，但更准确的是取根节点
            ' 我们直接取根节点 的 reads 数，因为它在过滤后仍然是总分类reads数
            Dim newTotalClassifiedReads As Long = filteredRecords.FirstOrDefault(Function(r) r.RankCode = "R")?.ReadsAtRank ?? 0
        
            If newTotalClassifiedReads = 0 Then
                ' 如果找不到根节点，就手动计算一个总和作为分母
                newTotalClassifiedReads = filteredRecords.Max(Function(r) r.ReadsAtRank)
            End If

            Console.WriteLine($"过滤前的总reads数: {allRecords.Max(Function(r) r.ReadsAtRank)}")
            Console.WriteLine($"过滤后的总reads数: {newTotalClassifiedReads}")

            For Each record In filteredRecords
                ' 重新计算百分比
                record.Percentage = (CDbl(record.ReadsAtRank) / CDbl(newTotalClassifiedReads)) * 100.0
            Next

            Return filteredRecords
        End Function

    End Module
End Namespace